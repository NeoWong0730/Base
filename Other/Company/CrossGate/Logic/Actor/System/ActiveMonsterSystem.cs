using Lib.Core;
using Packet;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class ActiveMonsterSystem : LevelSystemBase
    {
        private bool needCheckShow = false;        

        public override void OnCreate()
        {
            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnEndBattle, OnEndBattle, true);
            
        }

        public override void OnDestroy()
        {
            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnEndBattle, OnEndBattle, false);
        }

        public override void OnUpdate()
        {
            if (needCheckShow)
            {
                needCheckShow = false;
                List<Npc> npcs = GameCenter.npcsList;

                for (int i = 0, len = npcs.Count; i < len; ++i)
                {
                    Npc npc = npcs[i];
                    if (npc.cSVNpcData.type == (uint)ENPCType.ActiveMonster)
                    {
                        Excute(npc);
                    }
                }
            }
        }

        private void Excute(Npc npc)
        {            
            ActiveMonsterComponent activeMonsterComponent = npc.activeMonsterComponent;
            if (!activeMonsterComponent.isShow)
            {
                if (activeMonsterComponent.fCanShowTime <= Time.unscaledTime)
                {
                    activeMonsterComponent.isShow = true;
                    npc.VisualComponent.Checking();
                }
                else
                {
                    needCheckShow = true;
                }
            }
        }

        void OnEndBattle(CmdBattleEndNtf ntf)
        {
            List<Npc> npcs = GameCenter.npcsList;
            Npc npc;

            for (int i = 0, len = npcs.Count; i < len; ++i)
            {
                npc = npcs[i];
                if (npc.cSVNpcData.type == (uint)ENPCType.ActiveMonster)
                {
                    npc.activeMonsterComponent.FightCD(npc.cSVNpcData.CombatCooling / 1000f);
                }
            }

            if (GameCenter.TryGetSceneNPC(ntf.NpcId, out npc))
            {
                ExcuteEndBattle(npc, ntf);
            }
        }

        private void ExcuteEndBattle(Npc npc, CmdBattleEndNtf ntf)
        {
            if (ntf.BattleResult == 2)
            {
                NpcBattleCdEvt npcBattleCdEvt = new NpcBattleCdEvt();
                npcBattleCdEvt.actorId = npc.uID;
                npcBattleCdEvt.cd = (int)npc.cSVNpcData.CombatCooling / 1000;
                Sys_HUD.Instance.eventEmitter.Trigger<NpcBattleCdEvt>(Sys_HUD.EEvents.OnCreateNpcBattleCd, npcBattleCdEvt);
            }
            else if (ntf.BattleResult == 1)
            {
                if (npc.cSVNpcData.subtype == 1)
                {
                    npc.activeMonsterComponent.ShowCD(Time.unscaledTime + npc.cSVNpcData.RebirthTime / 1000f);
                    needCheckShow = true;

                    npc.VisualComponent.Checking();
                }
                else if (npc.cSVNpcData.subtype == 3)
                {
                    NPCHelper.DeleteNPC(npc.uID);
                }
            }
        }
    }
}