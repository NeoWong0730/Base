using Net;
using Packet;

namespace Logic
{
    public class ActiveKnowledgeFunction : FunctionBase
    {
        protected override void OnExecute()
        {
            base.OnExecute();

            CmdNpcNpcDialogueReq req = new CmdNpcNpcDialogueReq();
            req.UNpcId = npc.uID;
            req.DialogueId = DialogueID;

            NetClient.Instance.SendMessage((ushort)CmdNpc.NpcDialogueReq, req);
        }

        public override bool IsValid()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(22000, false))
                return false;

            if (Sys_Knowledge.Instance.IsKnowledgeActive(ID))
                return false;

            return true;
        }
    }
}