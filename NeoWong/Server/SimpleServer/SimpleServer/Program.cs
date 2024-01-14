class MainClass
{
    public static void Main(string[] args)
    {
        if (!DbManager.Connect("game", "127.0.0.1", 3306, "root", ""))
            return;

        MsgBase.SetMsgHelper(new MsgProtobufHelper());
        NetManager.StartLoop(8888);
    }
}