using Net;
using Packet;
using Table;

namespace Logic
{
#if false
    /// <summary>
    /// 小知识功能距离检测组件///
    /// </summary>
    public class LittleDistanceCheckComponent : NPCAreaCheckComponent
    {
        public uint knowid
        {
            get;
            set;
        }

        protected override bool CheckResult()
        {
            return !Sys_Knowledge.Instance.IsKnowledgeActive(knowid) && base.CheckResult();
        }

        protected override void Trigger()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(22000, false))
                return;

            CmdNpcNpcTriggerReq req = new CmdNpcNpcTriggerReq();
            req.UNpcId = Npc.uID;

            NetClient.Instance.SendMessage((ushort)CmdNpc.NpcTriggerReq, req);
        }

        protected override void OnDispose()
        {
            knowid = 0;
            base.OnDispose();
        }
    }
#endif
}
