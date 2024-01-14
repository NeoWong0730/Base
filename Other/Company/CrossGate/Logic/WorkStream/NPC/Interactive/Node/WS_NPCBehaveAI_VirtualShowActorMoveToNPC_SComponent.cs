using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：虚拟Actor移动到指定NPC位置///
    /// 0: UID///
    /// 1: TargetNPCID///
    /// 2: Speed///
    /// 3：OffsetPosX///
    /// 4: OffsetPosY///
    /// 5: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.VirtualShowActorMoveToNPC)]
    public class WS_NPCBehaveAI_VirtualShowActorMoveToNPC_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong uid;
        uint targetNPCID;
        float Speed;
        float OffsetPosX;
        float OffsetPosY;
        float OffsetPosZ;
        VirtualSceneActor actor;
        Npc targetNpc;

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

                uid = ulong.Parse(strs[0]);
                targetNPCID = uint.Parse(strs[1]);
                Speed = float.Parse(strs[2]);
                OffsetPosX = float.Parse(strs[3]);
                OffsetPosY = float.Parse(strs[4]);
                OffsetPosZ = float.Parse(strs[5]);

                if (VirtualShowManager.Instance.TryGetValue(uid, out actor))
                {
                    if (GameCenter.uniqueNpcs.TryGetValue(targetNPCID, out targetNpc))
                    {
                        actor.movementComponent.fMoveSpeed = Speed;
                        actor.movementComponent?.MoveTo(targetNpc.transform.position + new Vector3(OffsetPosX, OffsetPosY, OffsetPosZ), null, null, MoveToSuccess);
                    }
                    else
                    {
                        DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found targetNPC targetNPCID:{targetNPCID}");
                        m_CurUseEntity.TranstionMultiStates(this);
                    }
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found VirtualSceneActor uid:{uid}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_VirtualShowActorMoveToNPC_SComponent: {e.ToString()}");
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
            uid = 0ul;
            targetNPCID = 0u;
            Speed = OffsetPosX = OffsetPosY = OffsetPosZ = 0f;
            actor = null;
            targetNpc = null;

            base.Dispose();
        }
    }
}