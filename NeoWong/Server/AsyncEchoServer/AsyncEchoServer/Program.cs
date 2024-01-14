using System.Net;
using System.Net.Sockets;

class MainClass
{
    /// <summary>
    /// 监听 Socket
    /// </summary>
    static Socket listenfd;

    /// <summary>
    /// 客户端 Socket 及状态信息
    /// </summary>
    static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

    public static void Main(string[] args)
    {
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
        listenfd.Bind(ipEp);

        listenfd.Listen(0);
        Console.WriteLine("[服务器]启动成功");

        listenfd.BeginAccept(AcceptCallback, listenfd);
        Console.ReadLine();
    }

    private static void AcceptCallback(IAsyncResult ar)
    {
        try
        {
            Console.WriteLine("[服务器]Accept");
            Socket? listenfd = ar.AsyncState as Socket;
            Socket clientfd = listenfd.EndAccept(ar);

            ClientState state = new ClientState()
            {
                socket = clientfd,
            };
            clients.Add(clientfd, state);
            clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

            listenfd.BeginAccept(AcceptCallback, listenfd);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Socket Accept fail" + ex.ToString());
        }
    }

    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            ClientState state = ar.AsyncState as ClientState;
            Socket clientfd = state.socket;
            int count = clientfd.EndReceive(ar);

            if (count == 0)
            {
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Socket Close");
                return;
            }

            string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
            Console.WriteLine($"[服务器接收] {recvStr}");
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes("echo" + recvStr);
            clientfd.Send(sendBytes);
            clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Socket Receive fail" + ex.ToString());
        }
    }
}