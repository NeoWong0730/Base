using System.Net.Sockets;

class ClientState
{
    public Socket socket;
    public byte[] readBuff = new byte[1024];
}