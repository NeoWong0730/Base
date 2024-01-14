using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Serilog;

namespace Common.Network
{
    /// <summary>
    /// 负责监听TCP网络端口，异步接收Socket连接
    /// -- Connected        有新的连接
    /// -- DataReceived     有新的消息
    /// -- Disconnected     有连接断开
    /// Start()         启动服务器
    /// Stop()          关闭服务器
    /// IsRunning     是否正在运行
    /// </summary>
    public class TcpServer
    {
        private IPEndPoint endPoint;
        private Socket serverSocket;
        private readonly int backlog = 100;

        public delegate void ConnectedHandler(Connection conn);
        public delegate void DataReceivedHandler(Connection conn, IMessage message);
        public delegate void DisconnectedHandler(Connection conn);

        //事件委托：新的连接
        public event ConnectedHandler Connected;
        //事件委托：收到消息
        public event DataReceivedHandler DataReceived;
        //事件委托：连接断开
        public event DisconnectedHandler Disconnected;

        public TcpServer(string host, int port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public TcpServer(string host, int port, int backlog) : this(host, port)
        {
            this.backlog = backlog;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(backlog);
                Log.Information("Start listening port：" + endPoint.Port);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnAccept;
                serverSocket.AcceptAsync(args);
            }
            else
            {
                Log.Debug("TcpServer is already running");
            }
        }

        private void OnAccept(object sender, SocketAsyncEventArgs e)
        {
            //连入的客户端
            Socket client = e.AcceptSocket;
            //继续接收下一位
            e.AcceptSocket = null;
            serverSocket.AcceptAsync(e);
            //真的有人连进来
            if (e.SocketError == SocketError.Success)
            {
                if (client != null)
                {
                    OnSocketConnected(client);
                }
            }
        }

        private void OnSocketConnected(Socket socket)
        {
            Connection conn = new Connection(socket);
            conn.DataReceived += OnDataReceived;
            conn.Disconnected += OnDisconnected;
            Connected?.Invoke(conn);
        }

        private void OnDisconnected(Connection conn)
        {
            Disconnected?.Invoke(conn);
        }

        private void OnDataReceived(Connection conn, IMessage message)
        {
            DataReceived?.Invoke(conn, message);
        }

        public bool IsRunning
        {
            get { return serverSocket != null; }
        }

        public void Stop()
        {
            serverSocket?.Close();
            serverSocket = null;
        }
    }
}
