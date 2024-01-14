using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：NPC移动到指定虚拟Actor位置///
    /// 0: npcinfoid///
    /// 1: TargetUID///
    /// 2: Speed///
    /// 3：OffsetPosX///
    /// 4: OffsetPosY///
    /// 5: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.NPCMoveToTargetVirtualShowActor)]
    public class WS_NPCBehaveAI_NPCMoveToTargetVirtualShowActor_SComponent : StateBaseComponent
    {
        string[] strs;
        uint npcinfoid;
        ulong targetUid;
        float Speed;
        float OffsetPosX;
        float OffsetPosY;
        float OffsetPosZ;
        Npc npc;
        VirtualSceneActor targetActor;

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
                targetUid = ulong.Parse(strs[1]);
                Speed = float.Parse(strs[2]);
                OffsetPosX = float.Parse(strs[3]);
                OffsetPosY = float.Parse(strs[4]);
                OffsetPosZ = float.Parse(strs[5]);

                if (GameCenter.uniqueNpcs.TryGetValue(npcinfoid, out npc))
                {
                    if (VirtualShowManager.Instance.TryGetValue(targetUid, out targetActor))
                    {
                        if (npc.movementComponent.mNavMeshAgent != null && npc.movementComponent.mNavMeshAgent.isOnNavMesh && npc.movementComponent.mNavMeshAgent.isActiveAndEnabled)
                        {
                            npc.movementComponent.fMoveSpeed = Speed;
                            npc.movementComponent?.MoveTo(targetActor.transform.position + new Vector3(OffsetPosX, OffsetPosY, OffsetPosZ), null, null, MoveToSuccess);
                        }
                        else
                        {
                            m_CurUseEntity.TranstionMultiStates(this);
                        }
                    }
                    else
                    {
                        DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found targetActorVirtualSceneActor uid:{targetUid}");
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
                DebugUtil.LogError($"WS_NPCBehaveAI_NPCMoveToTargetVirtualShowActor_SComponent: {e.ToString()}");
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
            targetUid = 0ul;
            Speed = OffsetPosX = OffsetPosY = OffsetPosZ = 0f;
            npc = null;
            targetActor = null;

            base.Dispose();
        }
    }
}
