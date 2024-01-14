class MsgHandler
{
    public static void MsgEnter(ClientState state, string msgArgs)
    {
        Console.WriteLine("MsgEnter" + msgArgs);
    }

    public static void MsgMove(ClientState state, string msgArgs)
    {
        Console.WriteLine("MsgMove" + msgArgs);
    }
}
