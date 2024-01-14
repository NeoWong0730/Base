using System.Net;
using System.Net.Sockets;

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

        while (true)
        {
            if (listenfd.Poll(0, SelectMode.SelectRead))
            {
                ReadListenfd(listenfd);
            }

            foreach (var clientState in clients.Values)
            {
                Socket clientfd = clientState.socket;
                if (clientfd.Poll(0, SelectMode.SelectRead))
                {
                    if (!ReadClientfd(clientfd))
                        break;
                }
            }

            System.Threading.Thread.Sleep(1);
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
            clientfd.Close();
            clients.Remove(clientfd);
            Console.WriteLine("Receive SocketException " + ex.ToString());
            return false;
        }

        if (count == 0)
        {
            clientfd.Close();
            clients.Remove(clientfd);
            Console.WriteLine("Socket Close");
            return false;
        }

        string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
        Console.WriteLine("Receive " + recvStr);
        string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + recvStr;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        foreach (ClientState cs in clients.Values)
        {
            cs.socket.Send(sendBytes);
        }

        return true;
    }
}