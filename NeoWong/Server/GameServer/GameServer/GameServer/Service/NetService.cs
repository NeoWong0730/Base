using Common.Network;
using System.Net;
using Serilog;
using Proto.Basic;

namespace GameServer.Service
{
    /// <summary>
    /// 网络服务
    /// </summary>
    public class NetService
    {
        TcpServer tcpServer;

        public NetService()
        {
            tcpServer = new TcpServer("0.0.0.0", 8888);
            tcpServer.Connected += OnClientConnected;
            tcpServer.Disconnected += OnDisconnented;
        }

        private Dictionary<Connection, DateTime> heartBeatPairs = new Dictionary<Connection, DateTime>();

        public void Start()
        {
            //启动网络监听，指定消息包装类型
            tcpServer.Start();
            //启动消息分发器
            MessageRouter.Instance.Start(10);

            MessageRouter.Instance.Subscribe<HeartBeatRequest>(OnHeartBeatRequest);

            Timer timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        void TimerCallback(object state)
        {
            var now = DateTime.Now;
            foreach (var kv in heartBeatPairs)
            {
                var cha = now - kv.Value;
                if (cha.TotalSeconds > 10)
                {
                    //关闭超时的客户端连接
                    Connection conn = kv.Key;
                    conn.Close();
                    heartBeatPairs.Remove(conn);
                }
            }
        }

        void OnHeartBeatRequest(Connection conn, HeartBeatRequest req)
        {
            heartBeatPairs[conn] = DateTime.Now;
            //Log.Information("Get heartBeat package：" + conn);
            HeartBeatResponse resp = new HeartBeatResponse();
            conn.Send(resp);
        }

        private void OnClientConnected(Connection conn)
        {
            var endPoint = (IPEndPoint)conn.Socket.RemoteEndPoint;
            Log.Information($"Client connected: {endPoint.Address.ToString()}");

            heartBeatPairs[conn] = DateTime.Now;
        }

        private void OnDisconnented(Connection conn)
        {
            var endPoint = (IPEndPoint)conn.Socket.RemoteEndPoint;
            Log.Information($"Client disconnected: {endPoint.Address.ToString()}");

            heartBeatPairs.Remove(conn);
        }
    }
}
