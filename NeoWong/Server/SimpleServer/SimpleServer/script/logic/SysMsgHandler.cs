using System;

public partial class MsgHandler
{
    public static void MsgPing(ClientState c, MsgBase msgBase)
    {
        Console.WriteLine("MsgPing");
        c.lastPingTime = NetManager.GetTimeStamp();
        MsgPong msgPong = new MsgPong();
        NetManager.Send(c, msgPong);
    }
}


