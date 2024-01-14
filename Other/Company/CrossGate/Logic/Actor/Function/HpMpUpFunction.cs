using Packet;
using Net;

namespace Logic
{
    /// <summary>
    /// 血蓝回复功能///
    /// </summary>
    public class HpMpUpFunction : FunctionBase
    {
        protected override void OnExecute()
        {
            CmdGuildBattleRecoveryReq req = new CmdGuildBattleRecoveryReq();
            req.NpcUid = npc.UID;

            NetClient.Instance.SendMessage((ushort)CmdGuildBattle.RecoveryReq, req);
        }
    }
}
