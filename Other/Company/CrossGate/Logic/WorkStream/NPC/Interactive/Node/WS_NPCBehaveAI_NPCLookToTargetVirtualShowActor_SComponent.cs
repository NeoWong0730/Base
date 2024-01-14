using DG.Tweening;
using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：NPC朝向指定虚拟Actor位置
    /// 0: npcInfoID///
    /// 1: TargetUID///
    /// 2: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.NPCLookToTargetVirtualShowActor)]
    public class WS_NPCBehaveAI_NPCLookToTargetVirtualShowActor_SComponent : StateBaseComponent
    {
        string[] strs;
        uint npcInfoID;
        ulong targetUid;
        float time;
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

                npcInfoID = uint.Parse(strs[0]);
                targetUid = ulong.Parse(strs[1]);
                time = float.Parse(strs[2]);

                if (GameCenter.uniqueNpcs.TryGetValue(npcInfoID, out npc))
                {
                    if (VirtualShowManager.Instance.TryGetValue(targetUid, out targetActor))
                    {
                        npc.transform.DOLookAt(targetActor.gameObject.transform.position, time, AxisConstraint.Y, Vector3.up).onComplete = LookAtCompleted;
                    }
                    else
                    {
                        DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found targetActor targetUid:{targetUid}");
                        m_CurUseEntity.TranstionMultiStates(this);
                    }
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found Npc npcinfoid:{npcInfoID}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_NPCLookToTargetVirtualShowActor_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        void LookAtCompleted()
        {
            m_CurUseEntity?.TranstionMultiStates(this);
        }

        public override void Dispose()
        {
            strs = null;
            npcInfoID = 0u;
            targetUid = 0ul;
            time = 0f;
            npc = null;
            targetActor = null;

            base.Dispose();
        }
    }
}