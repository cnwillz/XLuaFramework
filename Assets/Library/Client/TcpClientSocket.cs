using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Net {
	public class TcpClientSocket {
		private class Package {
			public byte[] Data;
			public int Size;
			public int SendPos;

			public Package (byte[] data,int size,int sendPos=0) {
				Data = data;
				Size = size;
				SendPos = sendPos;
			}
		}

		public delegate void OnConnectCallback (TcpClientSocket socket);
		public delegate void OnCloseCallback (TcpClientSocket socket);
		public delegate void OnMessageCallback (TcpClientSocket socket,byte[] bytes,int size);
		public delegate void LogType (string msg);
		private Socket _socket;
		private Thread _recvThread;
		private Thread _sendThread;
		private OnConnectCallback _onconnect;
		private OnCloseCallback _onclose;
		private OnMessageCallback _onmessage;
		private LogType _log;

		private string _name;
		private int _timeout;
		private int _sendHz;
		private int _headerLen;
		private bool _bigEndian;   // header_len is encode big_endian?

		private Queue<Package> _recvQueue =  new Queue<Package>();
		private Queue<Package> _sendQueue = new Queue<Package>();
		private ByteStream _reader = new ByteStream();

		public TcpClientSocket (string name=null,int headerLen=2,bool bigEndian=true,int timeout=50,int sendHz=5) {
			_name = name;
			// every 'timeout' millisecond send 'send_hz' package
			_timeout = timeout;
			_sendHz = sendHz;
			_headerLen = headerLen;
			_bigEndian = bigEndian;
			
			// sync socket + thread to send/receive
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public void Register(OnConnectCallback onConnect,OnCloseCallback onClose,
			OnMessageCallback onMessage,LogType log = null) {
			_onconnect = onConnect;
			_onclose = onClose;
			_onmessage = onMessage;
			_log = log ?? Console.WriteLine;
		}

		public void Connect (string host,int port) {
			if (Connected) {
				return;
			}
			if (_name == null) {
				_name = String.Format("{0}:{1}",host,port);
			}
			try {
				_socket.BeginConnect(host,port,OnConnect,null);
			} catch (Exception e) {
				Log(String.Format("[{0}] op=ConnectError,error={1}",_name,e));
			}
		}

		public void Disconnect () {
			if (Connected) {
				try {
					_socket.Close();
				} catch (Exception e) {
					Log(String.Format("[{0}] op=CloseError,error={1}",_name,e));
				}
				OnClose();
			}
		}

		public void Close () {
			Disconnect();
		}

		public void Send (byte[] data,int size=0) {
			if (size == 0) {
				size = data.Length;
			}
			if (!Connected) {
				Log(String.Format("[{0}] op=Send to a Closed Socket,size={1}",_name,size));
				return;
			}
			byte[] sendData = new byte[_headerLen+size];
			encode_message_size(size,sendData);
			for (int i = 0; i < size; i++) {
				sendData[_headerLen+i] = data[i];
			}
			size = _headerLen + size;
			_sendQueue.Enqueue(new Package(sendData,size));
		}

		// call this to dispatch receive message
		public void Dispatch(int recvHz=5) {
			//this.Log(String.Format("[{0}] op=Dispatch,connected={1},recvQueue={2},sendQueue={3}",this.name,this.Connected,this.recvQueue.Count,this.sendQueue.Count));
			int recvLimit = recvHz;
			while (_recvQueue.Count > 0) {
				Package package = _recvQueue.Dequeue();
				try {
					OnMessage(package.Data,package.Size);
				} catch (Exception e) {
					Log(String.Format("[{0}] op=OnMessageError,size={1},error={2}",_name,package.Size,e));
				}
				recvLimit = recvLimit - 1;
				if (recvLimit <= 0)
					break;
			}
		}

		public bool Connected {
			get {
				return (_socket != null) && _socket.Connected;
			}
		}

		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}

		private void SendThreadLoop() {
			while (Connected) {
				Thread.Sleep(_timeout);
				// double check
				if (!Connected)
					break;
				// send message
				int sendLimit = _sendHz;
				while (_sendQueue.Count > 0) {
					Package package = _sendQueue.Peek();
					int sendBytes = 0;
					try {
						sendBytes = _socket.Send(package.Data,package.SendPos,package.Size-package.SendPos,SocketFlags.None);
					} catch (Exception e) {
						Log(String.Format("[{0}] op=SendError,size={1},error={2}",_name,package.Size,e));
					}
					package.SendPos = package.SendPos + sendBytes;
					if (package.SendPos < package.Size)
						break;
					_sendQueue.Dequeue();
					sendLimit = sendLimit - 1;
					if (sendLimit <= 0)
						break;
				}
			}
		}

		private void ReceiveThreadLoop() {
			while (Connected) {
				Thread.Sleep(_timeout);
				// double check
				if (!Connected)
					break;
				// receive message
				int maxRecvBytes = 8192;
				int recvBytes = -1;
				SocketError errcode = SocketError.Success;
				try {
					// try expand
					_reader.Expand(maxRecvBytes);
					recvBytes = _socket.Receive(_reader.Buffer,_reader.Position,maxRecvBytes,SocketFlags.None,out errcode);
					if (recvBytes == 0) {
						Close();
					}
				} catch (Exception e) {
					Log(String.Format("[{0}] op=RecvError,error={1}",_name,e));
					if (errcode == SocketError.Shutdown || errcode == SocketError.HostDown) {
						Close();
					}
				}
				if (recvBytes > 0) {
					_reader.Seek(_reader.Position+recvBytes,ByteStream.SeekBegin);
				}
				int pos = 0;
				int len = _reader.Position;
				int unreadLen = len - pos;
				while (unreadLen >= _headerLen) {
					int messageSize = decode_message_size(_reader.Buffer,pos);
					if (unreadLen >= messageSize + _headerLen) {
						byte[] data = new byte[messageSize];
						for (int i=0; i < messageSize; i++) {
							data[i] = _reader.Buffer[i+pos+_headerLen];
						}
						pos = pos + _headerLen + messageSize;
						unreadLen = len - pos;
						_recvQueue.Enqueue(new Package(data,messageSize));
					} else {
						break;
					}
				}
				if (unreadLen > 0) {
					if (pos != 0) {
						// move data to start,avoid stream expand too big
						_reader.Seek(0,ByteStream.SeekBegin);
						_reader.Write(_reader.Buffer,pos,unreadLen);
					}
				} else if (pos > 0) {
					// recv a unbroken message
					_reader.Seek(0,ByteStream.SeekBegin);
				}
			}
		}

		private int decode_message_size(byte[] buffer,int pos=0) {
			int len = _headerLen;
			int size = 0;
			if (_bigEndian) {
				for (int i = 0; i < len; i++) {
					int offset = 8 * (len-i-1);
					size = size | (buffer[pos+i] << offset);
				}
			} else {
				for (int i = 0; i < len; i++) {
					int offset = 8 * i;
					size = size | (buffer[pos+i] << offset);
				}
			}
			return size;
		}

		private byte[] encode_message_size(int size,byte[] data=null) {
			int len = _headerLen;
			if (data == null)
				data = new byte[len];
			if (_bigEndian) {
				for (int i = 0; i < len; i++) {
					int offset = 8 * (len-i-1);
					byte b = (byte)((size >> offset) & 0xff);
					data[i] = b;
				}
			} else {
				for (int i = 0; i < len; i++) {
					int offset = 8 * i;
					byte b = (byte)((size >> offset) & 0xff);
					data[i] = b;
				}
			}
			return data;
		}

		private void OnConnect(IAsyncResult iar) {
			try {
				_socket.EndConnect(iar);
				_OnConnect();
			} catch (Exception e) {
				Log(String.Format("[{0}] op=ConnectError,error={1}",_name,e));
			}
		}

		private void _OnConnect() {
			Log(String.Format("[{0}] op=OnConnect",_name));
			if (_recvThread != null) {
				_recvThread.Abort ();
			}
			if (_sendThread != null) {
				_sendThread.Abort ();
			}
			_recvThread = new Thread(ReceiveThreadLoop);
			_sendThread = new Thread(SendThreadLoop);
			_recvThread.Start();
			_sendThread.Start();
			if (_onconnect != null)
				_onconnect(this);
		}

		private void OnClose() {
			Log(String.Format("[{0}] op=OnClose",_name));
			//this.recvThread.Abort();
			//this.sendThread.Abort();
			if (_onclose != null)
				_onclose(this);
		}

		private void OnMessage(byte[] data,int size) {
			//this.Log(String.Format("[{0}] op=OnMessage,size={1}",this.name,size));
			if (_onmessage != null)
				_onmessage(this,data,size);
		}

		private void Log(String msg) {
			if (_log != null) {
				_log(msg);
			}
		}
	}
}
