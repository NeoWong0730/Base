using NWFramework;
using System;
using System.Net.Sockets;
using System.Net;

namespace HotFix
{
    public class NetOption
    {
        public string url { get; set; }
        public string host { get; set; }
        public int port { get; set; }
    }

    public interface IConnection
    {
        public void ConnectToServer(NetOption netOption);

        public void Send(byte[] buffer);

        public void Close();
    }

    /// <summary>
    /// 通用网络连接，可以继承此类实现功能拓展
    /// 职责：发送消息，关闭连接，断开回调，接收消息回调，
    /// </summary>
    public class Connection : IConnection
    {
        private Socket socket;

        public Socket Socket { get { return socket; } }

        //public Action<Connection, IMessage> DataReceived;
        public Action<IConnection> Disconnected;

        public void ConnectToServer(NetOption netOption)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(netOption.host), netOption.port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipe);
            Log.Info("连接到服务端");

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

            //DataReceived?.Invoke(this, msg);
        }

        private void OnDisconnected()
        {
            Disconnected?.Invoke(this);
        }

        public void Close()
        {
            socket.Close();
        }

        //public void Send(IMessage message)
        //{
        //    using (var ds = DataStream.Allocate())
        //    {
        //        int code = ProtoHelper.SeqCode(message.GetType());
        //        ds.WriteInt(message.CalculateSize() + 2);
        //        ds.WriteUShort((ushort)code);
        //        message.WriteTo(ds);
        //        Send(ds.ToArray());
        //    }
        //}

        //通过socket发送原生数据
        public void Send(byte[] data)
        {
            Send(data, 0, data.Length);
        }

        //通过socket发送原生数据
        private void Send(byte[] data, int offset, int len)
        {
            lock (this)
            {
                if (socket.Connected)
                {
                    socket.BeginSend(data, offset, len, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
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
