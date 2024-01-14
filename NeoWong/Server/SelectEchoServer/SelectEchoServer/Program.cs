using System.Net;
using System.Net.Sockets;
using System.Reflection;

class MainClass
{
    private static Socket listenfd;
    private static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

    public static void Main(string[] args)
    {
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
        listenfd.Bind(ipEp);

        listenfd.Listen(0);
        Console.WriteLine("[服务器]启动成功");

        List<Socket> checkRead = new List<Socket>();

        while (true)
        {
            checkRead.Clear();
            checkRead.Add(listenfd);
            foreach (ClientState state in clients.Values)
            {
                checkRead.Add(state.socket);
            }

            Socket.Select(checkRead, null, null, 1000);

            foreach (Socket socket in checkRead)
            {
                if (socket == listenfd)
                {
                    ReadListenfd(socket);
                }
                else
                {
                    ReadClientfd(socket);
                }
            }
        }
    }

    private static void ReadListenfd(Socket listenfd)
    {
        Console.WriteLine("Accept");
        Socket clientfd = listenfd.Accept();
        ClientState state = new ClientState()
        {
            socket = clientfd,
        };
        clients.Add(clientfd, state);
    }

    private static bool ReadClientfd(Socket clientfd)
    {
        ClientState state = clients[clientfd];

        int count = 0;
        try
        {
            count = clientfd.Receive(state.readBuff);
        }
        catch (SocketException ex)
        {
            MethodInfo methodInfo = typeof(EventHandler).GetMethod("OnDisconnect");
            object[] objs = { state };
            methodInfo?.Invoke(null, objs);

            clientfd.Close();
            clients.Remove(clientfd);
            Console.WriteLine("Receive SocketException " + ex.ToString());
            return false;
        }

        if (count <= 0)
        {
            MethodInfo methodInfo = typeof(EventHandler).GetMethod("OnDisconnect");
            object[] objs = { state };
            methodInfo?.Invoke(null, objs);

            clientfd.Close();
            clients.Remove(clientfd);
            Console.WriteLine("Socket Close");
            return false;
        }

        string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
        Console.WriteLine("Receive " + recvStr);

        string[] split = recvStr.Split('|');
        if (split != null && split.Length > 1)
        {
            string msgName = split[0];
            string msgArgs = split[1];

            string funName = "Msg" + msgName;
            MethodInfo methodInfo = typeof(MsgHandler).GetMethod(funName);
            object[] objs = { state, msgArgs };
            methodInfo?.Invoke(null, objs);
        }

        return true;
    }
}