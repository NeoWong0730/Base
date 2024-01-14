using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：NPC移动到指定位置///
    /// 0: npcinfoid///
    /// 1: TargetPosX///
    /// 2: TargetPosY///
    /// 3: TargetPosZ///
    /// 4：Speed///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.NPCMoveToTargetPos)]
    public class WS_NPCBehaveAI_NPCMoveToTargetPos_SComponent : StateBaseComponent
    {
        string[] strs;
        uint npcinfoid;
        float TargetPosX;
        float TargetPosY;
        float TargetPosZ;
        float Speed;
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
                TargetPosX = float.Parse(strs[1]);
                TargetPosY = float.Parse(strs[2]);
                TargetPosZ = float.Parse(strs[3]);
                Speed = float.Parse(strs[4]);

                if (GameCenter.uniqueNpcs.TryGetValue(npcinfoid, out npc))
                {
                    if (npc.movementComponent.mNavMeshAgent != null && npc.movementComponent.mNavMeshAgent.isOnNavMesh && npc.movementComponent.mNavMeshAgent.isActiveAndEnabled)
                    {
                        npc.movementComponent.fMoveSpeed = Speed;
                        npc.movementComponent?.MoveTo(new Vector3(TargetPosX, TargetPosY, TargetPosZ), null, null, MoveToSuccess);
                    }
                    else
                    {
                        m_CurUseEntity.TranstionMultiStates(this);
                    }
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found Npc npcinfoid:{npcinfoid}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_NPCMoveToTargetPos_SComponent: {e.ToString()}");
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
            TargetPosX = TargetPosY = TargetPosZ = Speed = 0f;
            npc = null;

            base.Dispose();
        }
    }
}
