using Table;
using UnityEngine;
using System.Collections.Generic;
using Lib.Core;

namespace Logic
{
    public class NPCActionListenerSystem : LevelSystemBase
    {
        public override void OnUpdate()
        {
            if (GameCenter.mainHero == null)
                return;

            for (int i = 0, len = GameCenter.npcsList.Count; i < len; ++i)
            {
                Excute(GameCenter.npcsList[i], GameCenter.mainHero.transform);
            }
        }

        private void Excute(Npc npc, Transform centerTransform)
        {
            CSVNpc.Data csvNpcData = npc.cSVNpcData;

            if (csvNpcData.TriggerPerformRange == 0)
                return;

            if (!npc.npcActionListenerComponent.actionlockFlag)
                return;

            float triggerPerformRange = csvNpcData.TriggerPerformRange / 10000f;
            if (!MathUtlilty.SafeDistanceLess(npc.transform, centerTransform, triggerPerformRange))
                return;

            npc.npcActionListenerComponent.actionlockFlag = false;

            uint behaviorid = csvNpcData.behaviorid == 0 ? 1 : csvNpcData.behaviorid;

            npc.wS_NPCManagerEntity = WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(behaviorid, 0, SwitchWorkStreamEnum.Stop_AllWorkStream
                , null, npc.npcActionListenerComponent.OnControllerOverAction, true, (int)NPCEnum.B_EnterTrigger, npc.uID);
        }
    }
}