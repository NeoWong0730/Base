public class Player
{
    public string id = "";

    public ClientState state;
    
    public Player(ClientState state)
    {
        this.state = state;
    }

    //临时数据，如：坐标
    public int x;
    public int y;
    public int z;

    //数据库数据
    public PlayerData data;

    /// <summary>
    /// 发送信息
    /// </summary>
    /// <param name="msgBase"></param>
    public void Send(MsgBase msgBase)
    {
        NetManager.Send(state, msgBase);
    }
}