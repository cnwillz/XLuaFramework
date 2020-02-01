using System.Collections.Generic;
using Sproto;

namespace Net {
	public class ProtoDispatcher {
		public delegate void MessageHandler(SprotoTcpSocket client,RpcMessage message);

		private Dictionary<string,MessageHandler> _handlers = new Dictionary<string,MessageHandler>();

		public MessageHandler GetHandler(string proto) {
			MessageHandler handler = null;
			if (!_handlers.TryGetValue(proto,out handler)) {
					return null;
			}
			return handler;
		}

		public void AddHandler(string proto,MessageHandler handler) {
			_handlers.Add(proto,handler);
		}

		public bool RemoveHandler(string proto) {
			return _handlers.Remove(proto);
		}

		public bool DelHandler(string proto) {
			return RemoveHandler(proto);
		}

		public void Dispatch(SprotoTcpSocket client,RpcMessage message) {
			string proto = message.proto;
			MessageHandler handler = GetHandler(proto);
			if (handler == null) {
				return;
			}
			handler(client,message);
		}
	}
}
