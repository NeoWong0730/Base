using NWFramework;
using System;
using System.Net.Sockets;

namespace HotFix
{
    public class AsyncEchoTest : Singleton<AsyncEchoTest>
    {
        private Socket _socket;
        private byte[] readBuff = new byte[1024];
        private string recvStr = string.Empty;

        public void Connection()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.BeginConnect("127.0.0.1", 8888, ConnectCallback, _socket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                socket.EndConnect(ar);
                Log.Info("Socket Connect Succ");
                _socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
            }
            catch(SocketException ex)
            {
                Log.Error("Socket Connect fail" + ex.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                int count = socket.EndReceive(ar);
                recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
                Log.Info(recvStr);

                socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
            }
            catch (SocketException ex)
            {
                Log.Error("Socket Receive fail" + ex.ToString());
            }
        }

        public void Send(string sendStr)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            _socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, _socket);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                int count = socket.EndSend(ar);
                Log.Info("Socket Send succ" + count);
            }
            catch (SocketException ex)
            {
                Log.Error("Socket Send fail" + ex.ToString());
            }
        }
    }

}