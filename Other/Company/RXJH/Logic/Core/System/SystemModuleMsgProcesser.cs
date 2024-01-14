using Client;
using Google.Protobuf;
using Net;

namespace Logic.Core {
    public struct SystemModuleMsgProcesser {
        public readonly enClientFirst first;

        public SystemModuleMsgProcesser(enClientFirst first) {
            this.first = first;
        }

        public void SendMessage(ushort second, IMessage message) {
            NetClient.Instance.SendMessage((ushort)first, second, message);
        }
        
        public void SendMessage(NetClient transfer, ushort second, IMessage message) {
            transfer?.SendMessage((ushort)first, second, message);
        }

        public void Listen(ushort request, ushort response, EventListenerDelegate listener, MessageParser parser, bool toRegister) {
            if (toRegister) {
                EventDispatcher.Instance.AddEventListener((ushort)first, request, response, listener, parser);
            }
            else {
                EventDispatcher.Instance.RemoveEventListener((ushort)first, response, listener);
            }
        }
    }
}