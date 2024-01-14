using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：NPC移动到玩家位置///
    /// 0: npcinfoid///
    /// 1: Speed///
    /// 2: OffsetPosX///
    /// 3：OffsetPosY///
    /// 4: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.NPCMoveToMainHero)]
    public class WS_NPCBehaveAI_NPCMoveToMainHero_SComponent : StateBaseComponent
    {
        string[] strs;
        uint npcinfoid;
        float Speed;
        float OffsetPosX;
        float OffsetPosY;
        float OffsetPosZ;
        Npc npc;

        public override void Init(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                strs = CombatHelp.GetStrParse1Array(str);

                npcinfoid = uint.Parse(strs[0]);
                Speed = float.Parse(strs[1]);
                OffsetPosX = float.Parse(strs[2]);
                OffsetPosY = float.Parse(strs[3]);
                OffsetPosZ = float.Parse(strs[4]);

                if (GameCenter.uniqueNpcs.TryGetValue(npcinfoid, out npc))
                {
                    if (npc.movementComponent.mNavMeshAgent != null && npc.movementComponent.mNavMeshAgent.isOnNavMesh && npc.movementComponent.mNavMeshAgent.isActiveAndEnabled)
                    {
                        npc.movementComponent.fMoveSpeed = Speed;
                        npc.movementComponent?.MoveTo(GameCenter.mainHero.transform.position + new Vector3(OffsetPosX, OffsetPosY, OffsetPosZ), null, null, MoveToSuccess);
                    }
                    else
                    {
                        m_CurUseEntity.TranstionMultiStates(this);
                    }
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found npc npcinfoid:{npcinfoid}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_NPCMoveToMainHero_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        void MoveToSuccess()
        {
            m_CurUseEntity?.TranstionMultiStates(this);
        }

        public override void Dispose()
        {
            strs = null;
            npcinfoid = 0u;
            OffsetPosX = OffsetPosY = OffsetPosZ = 0f;
            npc = null;

            base.Dispose();
        }
    }
}