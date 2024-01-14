public partial class MsgHandler
{
    /// <summary>
    /// 注册协议处理
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgRegisterReq(ClientState c, MsgBase msgBase)
    {
        MsgRegisterReq req = msgBase as MsgRegisterReq;
        MsgRegisterAck ack = new MsgRegisterAck();

        //注册
        if (DbManager.Register(req.id, req.pw))
        {
            DbManager.CreatePlayer(req.id);
            ack.result = 0;
        }
        else
        {
            ack.result = 1;
        }

        NetManager.Send(c, ack);
    }

    /// <summary>
    /// 登陆协议处理
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgLoginReq(ClientState c, MsgBase msgBase)
    {
        MsgLoginReq req = msgBase as MsgLoginReq;
        MsgLoginAck ack = new MsgLoginAck();

        //密码校验
        if (!DbManager.CheckPassword(req.id, req.pw))
        {
            ack.result = 1;
            NetManager.Send(c, ack);
            return;
        }

        //不允许再次登陆
        if (c.player != null)
        {
            ack.result = 1;
            NetManager.Send(c, ack);
            return;
        }

        //如果已经登陆，踢下线
        if (PlayerManager.IsOnline(req.id))
        {
            //发送踢下线协议
            Player other = PlayerManager.GetPlayer(req.id);
            MsgKickNtf ntf = new MsgKickNtf();
            ntf.reson = 0;
            other.Send(ntf);

            //断开连接
            NetManager.Close(other.state);
        }

        //获取玩家数据
        PlayerData playerData = DbManager.GetPlayerData(req.id);
        if (playerData == null)
        {
            ack.result = 1;
            NetManager.Send(c, ack);
            return;
        }

        //构建Player
        Player player = new Player(c);
        player.id = req.id;
        player.data = playerData;
        PlayerManager.AddPlayer(req.id, player);
        c.player = player;

        //返回协议
        ack.result = 0;
        player.Send(ack);
    }
}