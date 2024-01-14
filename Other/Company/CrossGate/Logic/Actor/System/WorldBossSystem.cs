using Logic.Core;
using Net;
using Packet;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class WorldBossSystem : LevelSystemBase
    {
        private float lastTime = 0;
        private float cd = 1.5f;

        public override void OnUpdate()
        {
            if (Time.unscaledTime < lastTime)
                return;

            lastTime = Time.unscaledTime + cd;

            for (int i = 0, len = GameCenter.worldBossList.Count; i < len; ++i)
            {
                Excute(GameCenter.worldBossList[i]);
            }
        }

        private void Excute(WorldBossNpc worldBoss)
        {
            uint npcId = worldBoss.cSVNpcData.id;

            if (!CSVBOSSInformation.Instance.TryGetValue(npcId, out CSVBOSSInformation.Data csvBoss))
                return;

            if (!CSVBOSSManual.Instance.TryGetValue(csvBoss.bossManual_id, out CSVBOSSManual.Data csvManual))
                return;

            if (Sys_Role.Instance.Role.Level < csvManual.unlockedLevel)
                return;

            if (!worldBoss.VisualComponent.Visiable)
                return;

            if (Sys_WorldBoss.Instance.IsUnlockedBossManual(csvBoss.bossManual_id))
                return;

            if (!Sys_Npc.Instance.IsInNpcArea(Sys_Map.Instance.CurMapId, npcId, GameCenter.mainHero.transform))
                return;

            Sys_WorldBoss.Instance.ReqUnlockBossManual(csvBoss.bossManual_id);
        }
    }
}