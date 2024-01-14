using Table;
using Lib.Core;
using Net;
using Packet;

namespace Logic
{
    /// <summary>
    /// 世界Boss组件///
    /// </summary>
    public class WorldBossComponent : Logic.Core.Component
    {
        public Npc Npc
        {
            get;
            private set;
        }

        private uint battleID;

        public uint BattleID
        {
            get
            {
                return battleID;
            }
            set
            {
                if (battleID != value)
                {
                    battleID = value;
                    UpdateWorldBossFightState();

                    if (value != 0)
                    {
                        Npc.wS_NPCManagerEntity = WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(Npc.cSVNpcData.behaviorid, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, null, true, (int)NPCEnum.B_WorldBossEnterFight, Npc.uID);
                    }
                }             
            }
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();

            Npc = actor as Npc;

            //AddWorldBossHud();
        }

        protected override void OnDispose()
        {
            ClearWorldBossHuD();
            Npc = null;
            BattleID = 0;
            
          
            base.OnDispose();
        }

        public void OnBossBattleFailNtf(uint result)
        {
            if (Npc == null)
                return;

            ///怪物胜利
            if (result == 0)
            {
                Npc.wS_NPCManagerEntity = WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(Npc.cSVNpcData.behaviorid, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, null, true, (int)NPCEnum.B_WorldBossExitFight_Success, Npc.uID);
            }
            ///怪物失败///
            else if (result == 1)
            {
                Npc.wS_NPCManagerEntity = WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(Npc.cSVNpcData.behaviorid, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, null, true, (int)NPCEnum.B_WorldBossExitFight_Faild, Npc.uID);
            }
        }

        public void AddWorldBossHud()
        {
            if (Npc == null)
                return;

            CSVBOSSInformation.Data cSVBOSSInformationData = CSVBOSSInformation.Instance.GetConfData(Npc.cSVNpcData.id);
            if (cSVBOSSInformationData == null)
            {
                DebugUtil.LogError($"cSVBOSSInformationData is null id:{Npc.cSVNpcData.id}");
                return;
            }

            UpdateWorldBossHuDEvt updateWorldBossHuDEvt = new UpdateWorldBossHuDEvt();
            updateWorldBossHuDEvt.actorId = Npc.uID;
            updateWorldBossHuDEvt.iconId = CSVBOOSFightPlayMode.Instance.GetConfData(cSVBOSSInformationData.playMode_id).headIcon_id;
            updateWorldBossHuDEvt.level = cSVBOSSInformationData.BOSS_level;

            Sys_HUD.Instance.eventEmitter.Trigger<UpdateWorldBossHuDEvt>(Sys_HUD.EEvents.OnUpdateWorldBossHud, updateWorldBossHuDEvt);
        }

        void UpdateWorldBossFightState()
        {
            if (Npc == null)
                return;

            UpdateActorFightStateEvt updateActorFightStateEvt = new UpdateActorFightStateEvt();
            updateActorFightStateEvt.actorId = Npc.uID;
            updateActorFightStateEvt.state = (battleID != 0) ? true : false;

            Sys_HUD.Instance.eventEmitter.Trigger<UpdateActorFightStateEvt>(Sys_HUD.EEvents.OnUpdateSceneActorFightState, updateActorFightStateEvt);
        }

        void ClearWorldBossHuD()
        {
            if (Npc == null)
                return;

            ClearWorldBossHuDEvt ClearWorldBossHuDEvtData = new ClearWorldBossHuDEvt();
            ClearWorldBossHuDEvtData.actorId = Npc.uID;

            Sys_HUD.Instance.eventEmitter.Trigger<ClearWorldBossHuDEvt>(Sys_HUD.EEvents.OnClearWorldBossHud, ClearWorldBossHuDEvtData);
        }
    }
}
