using GameServer;
using GameServer.Model;
using Google.Protobuf;
using Serilog;
using System.Net.Sockets;

namespace Common.Network
{
    /// <summary>
    /// 通用网络连接，可以继承此类实现功能拓展
    /// 职责：发送消息，关闭连接，断开回调，接收消息回调，
    /// </summary>
    public class Connection : TypeAttributeStore
    {
        public delegate void DataReceivedHandler(Connection conn, IMessage message);
        public delegate void DisconnectedHandler(Connection conn);

        private Socket socket;

        public Socket Socket { get { return socket; } }

        public Player Player { get; set; }

        public event DataReceivedHandler DataReceived;
        public event DisconnectedHandler Disconnected;

        public Connection(Socket _socket)
        {
            socket = _socket;

            var sr = new SocketReceiver(socket);
            sr.OnDataReceived += OnDataReceived;
            sr.OnDisconnected += OnDisconnected;
            sr.Start();
        }

        private void OnDataReceived(byte[] data)
        {
            ushort code = GetUShort(data, 0);
            var msg = ProtoHelper.ParseFrom(code, data, 2, data.Length - 2);

            if (MessageRouter.Instance.Running)
            {
                MessageRouter.Instance.AddMessage(this, msg);
            }

            DataReceived?.Invoke(this, msg);
        }

        private void OnDisconnected()
        {
            Disconnected?.Invoke(this);
        }

        public void Close()
        {
            socket.Close();
        }

        public void Send(IMessage message)
        {
            using (var ds = DataStream.Allocate())
            {
                int code = ProtoHelper.SeqCode(message.GetType());
                ds.WriteInt(message.CalculateSize() + 2);
                ds.WriteUShort((ushort)code);
                message.WriteTo(ds);
                SocketSend(ds.ToArray());
            }
        }

        //通过socket发送原生数据
        private void SocketSend(byte[] data)
        {
            SocketSend(data, 0, data.Length);
        }

        //通过socket发送原生数据
        private void SocketSend(byte[] data, int offset, int len)
        {
            lock (this)
            {
                if (socket.Connected)
                {
                    //Log.Information($"send, len:{len}");
                    socket.BeginSend(data, offset, len, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            //Log.Information("SendCallback");
            // 发送的字节数
            int len = socket.EndSend(ar);
        }

        //前提是data必须是大端字节序
        private ushort GetUShort(byte[] data, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (ushort)((data[offset] << 8) | data[offset + 1]);
            }
            else
            {
                return (ushort)((data[offset + 1] << 8) | data[offset]);
            }
        }
    }
}
