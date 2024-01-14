using Lib.Core;
using Net;
using Packet;
using UnityEngine;

namespace Logic
{
#if false
    public class ActiveMonsterComponent : Logic.Core.Component
    {
        protected Npc npc;
        Timer fightCDTimer;
        Timer hideCDTimer;
        public bool canFightFlag = true;
        public bool isHide = false;

        protected override void OnConstruct()
        {
            base.OnConstruct();
            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnEndBattle, OnEndBattle, true);
            npc = actor as Npc;
        }

        protected override void OnDispose()
        {
            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnEndBattle, OnEndBattle, false);
            npc = null;
            fightCDTimer?.Cancel();
            fightCDTimer = null;
            canFightFlag = true;

            hideCDTimer?.Cancel();
            hideCDTimer = null;
            isHide = false;

            base.OnDispose();
        }

        void OnEndBattle(CmdBattleEndNtf ntf)
        {
            canFightFlag = false;
            fightCDTimer?.Cancel();
            fightCDTimer = Timer.Register(npc.cSVNpcData.CombatCooling / 1000f, FightCDTimerCallback, null, false, true);

            if (ntf.BattleResult == 2 && ntf.NpcId == npc.uID)
            {
                NpcBattleCdEvt npcBattleCdEvt = new NpcBattleCdEvt();
                npcBattleCdEvt.actorId = npc.uID;
                npcBattleCdEvt.cd = (int)npc.cSVNpcData.CombatCooling / 1000;
                Sys_HUD.Instance.eventEmitter.Trigger<NpcBattleCdEvt>(Sys_HUD.EEvents.OnCreateNpcBattleCd, npcBattleCdEvt);
            }

            if (npc.cSVNpcData.subtype == 1)
            {
                if (ntf.BattleResult == 1 && ntf.NpcId == npc.uID)
                {
                    isHide = true;
                    npc.VisualComponent?.Checking();
                    hideCDTimer = Timer.Register(npc.cSVNpcData.RebirthTime / 1000f, HideCDTimerCallback, null, false, true);
                }
            }
            else if (npc.cSVNpcData.subtype == 3)
            {
                if (ntf.BattleResult == 1 && ntf.NpcId == npc.uID)
                {
                    NPCHelper.DeleteNPC(npc.uID);
                }
            }
        }

        void FightCDTimerCallback()
        {
            canFightFlag = true;
        }

        void HideCDTimerCallback()
        {
            isHide = false;
            npc?.VisualComponent?.Checking();
        }
    }
#else
    public class ActiveMonsterComponent : Logic.Core.Component
    {
        public float fCanShowTime = 0f;
        public bool isShow = true;

        private float fCanFightTime = 0f;
        private bool _canFightFlag = true;

        public bool canFightFlag
        {
            get
            {
                if (!_canFightFlag)
                {
                    if (fCanFightTime <= Time.unscaledTime)
                    {
                        _canFightFlag = true;
                    }
                }
                return _canFightFlag;
            }
        }        

        protected override void OnDispose()
        {
            fCanShowTime = 0;
            isShow = true;
            fCanFightTime = 0f;
            _canFightFlag = true;
        }

        /// <summary>
        /// 战斗CD
        /// </summary>
        /// <param name="time">npc.cSVNpcData.CombatCooling / 1000f</param>
        public void FightCD(float time)
        {
            _canFightFlag = false;
            fCanFightTime = Time.unscaledTime + time;
        }

        public void ShowCD(float time)
        {
            isShow = false;
            fCanShowTime = Time.unscaledTime + time;
        }
    }
#endif
}
