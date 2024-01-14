using NWFramework;
using System.Net.Sockets;

namespace HotFix
{
    public class EchoTest : Singleton<EchoTest>
    {
        private Socket _socket;

        public void Connection()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect("127.0.0.1", 8888);
        }

        public void Send()
        {
            string sendStr = "Hello Server!";
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            _socket.Send(sendBytes);

            byte[] readBuff = new byte[1024];
            int count = _socket.Receive(readBuff);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff);
            Log.Info(recvStr);

            _socket.Close();
        }
    }
}