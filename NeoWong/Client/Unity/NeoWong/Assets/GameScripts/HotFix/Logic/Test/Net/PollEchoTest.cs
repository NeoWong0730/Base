using NWFramework;
using System.Net.Sockets;

namespace HotFix
{
    public class PollEchoTest : Singleton<PollEchoTest>
    {
        private Socket _socket;

        public void Connection()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect("127.0.0.1", 8888);
        }

        public void Send(string sendStr)
        {
            if (_socket == null)
                return;

            if (_socket.Poll(0, SelectMode.SelectWrite))
            {
                byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
                _socket.Send(sendBytes);
            }
        }

        public void Update()
        {
            if (_socket == null)
                return;

            if (_socket.Poll(0, SelectMode.SelectRead))
            {
                byte[] readBuff = new byte[1024];
                int count = _socket.Receive(readBuff);
                string recvStr = System.Text.Encoding.Default.GetString(readBuff);
                Log.Info(recvStr);
            }
        }
    }
}