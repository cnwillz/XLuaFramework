using System;
using System.Collections.Generic;
using Sproto;
using XLua;

namespace Net {
	[LuaCallCSharp]
	public class SprotoTcpSocket {
		public delegate void OnConnectCallback(SprotoTcpSocket socket);
		public delegate void OnCloseCallback(SprotoTcpSocket socket);
		public delegate void LogType(string msg);

		public event Action<SprotoTcpSocket> OnConnect;
		public event Action<SprotoTcpSocket> OnClose;
		public LogType Log = null;
		public TcpClientSocket TcpSocket;
		public ProtoDispatcher Dispatcher;
		public SprotoRpc Proto;
		private Int64 _messageId;
		private Int64 _sessionId;
		private Dictionary<Int64,ProtoDispatcher.MessageHandler> _sessions = new Dictionary<Int64,ProtoDispatcher.MessageHandler>();

		public SprotoTcpSocket (string fileS2C,string fileC2S,bool isbinary=false) {
			TcpSocket = new TcpClientSocket();
			TcpSocket.Register(_OnConnect,_OnClose,_OnMessage,_Log);
			Dispatcher = new ProtoDispatcher();
			SprotoMgr s2C = SprotoParser.ParseFile(fileS2C);
			SprotoMgr c2S = SprotoParser.ParseFile(fileC2S);
			Proto = new SprotoRpc(s2C,c2S);
		}

		public SprotoTcpSocket(SprotoMgr s2C,SprotoMgr c2S) {
			TcpSocket = new TcpClientSocket();
			TcpSocket.Register(_OnConnect,_OnClose,_OnMessage,_Log);
			Dispatcher = new ProtoDispatcher();
			Proto = new SprotoRpc(s2C,c2S);
		}
		//Connect,Disconnect,Dispatch use member TcpSocket todo?

		public void SendRequest(string proto,SprotoObject request=null,ProtoDispatcher.MessageHandler handler=null) {
			Int64 sessionId = 0;
			if (handler != null) {
				this._sessionId = this._sessionId + 1;
				sessionId = this._sessionId;
				_sessions.Add(sessionId,handler);
			}
			Int64 messageId = gen_message_id();
			
			string msg = String.Format("[{0}] op=SendRequest,proto={1},req={2},cb={3},session={4},type={5},request={6},response={7}",
				this.TcpSocket.Name,proto,request,handler,sessionId,request.type,0,0);
			this._Log(msg);
			RpcPackage package = Proto.PackRequest(proto,request,sessionId,messageId);
			TcpSocket.Send(package.data,package.size);
		}

		public void SendResponse(string proto,SprotoObject response,Int64 session) {
			Int64 messageId = gen_message_id();
			RpcPackage package = Proto.PackResponse(proto,response,session,messageId);
			TcpSocket.Send(package.data,package.size);
		}

		private Int64 gen_message_id() {
			_messageId = _messageId + 1;
			return _messageId;
		}

		private void _OnMessage(TcpClientSocket tcpSocket,byte[] data,int size) {
			RpcMessage message = Proto.UnpackMessage(data,size);
			string msg = String.Format("[{0}] op=OnMessage,proto={1},tag={2},ud={3},session={4},type={5},request={6},response={7}",
				TcpSocket.Name,message.proto,message.tag,message.ud,message.session,message.type,message.request,message.response);
			_Log(msg);
			if (message.type == "response") {
				Int64 session = message.session;
				ProtoDispatcher.MessageHandler handler = null;
				if (!_sessions.TryGetValue(session,out handler)) {
					return;
				}
				handler(this,message);
				return;
			}
			Dispatcher.Dispatch(this,message);
		}

		private void _OnConnect(TcpClientSocket tcpSocket) {
			if (OnConnect != null) {
				OnConnect(this);
			}
		}

		private void _OnClose(TcpClientSocket tcpSocket) {
			if (OnClose != null) {
				OnClose(this);
			}
		}

		private void _Log(string msg) {
			if (Log != null) {
				Log(msg);
			}
		}

	}
}
