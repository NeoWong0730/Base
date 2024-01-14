namespace Logic
{
    public class NpcActiveListenerComponent : Logic.Core.Component
    {
        public Npc Npc;
        public uint NpcId;
        public uint FunctionId;       

        public bool NoteNpc; //只有资源点npc,需要挂这个检测激活
        public Sys_Npc.MapResPointData ResPointData;

        protected override void OnConstruct()
        {
            base.OnConstruct();

            Npc = actor as Npc;
            NpcId = Npc.cSVNpcData.id;

            NoteNpc = Npc.cSVNpcData.type == (uint)ENPCType.Note;

            if (NoteNpc)
            {
                ResPointData = Sys_Npc.Instance.GetMapResPointData(Sys_Map.Instance.CurMapId, NpcId);
                FunctionId = Sys_Exploration.Instance.GetResPointFunctionOpenId(ResPointData.mainMarkType);
            }
        }

        protected override void OnDispose()
        {
            Npc = null;
            NpcId = 0u;
            ResPointData = null;
            FunctionId = 0u;

            base.OnDispose();
        }    
    }
}
