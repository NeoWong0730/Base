using Packet;
using Net;

namespace Logic
{
    public class ResourceSubmitFunction : FunctionBase
    {
        protected override bool CanExecute(bool CheckVisual = true)
        {
            uint id;
            return Sys_FamilyResBattle.Instance.HasResource(out id);
        }

        protected override void OnExecute()
        {
            CmdGuildBattleHandInResourceReq req = new CmdGuildBattleHandInResourceReq();
            req.NpcUid = npc.UID;

            NetClient.Instance.SendMessage((ushort)CmdGuildBattle.HandInResourceReq, req);
        }
    }
}
