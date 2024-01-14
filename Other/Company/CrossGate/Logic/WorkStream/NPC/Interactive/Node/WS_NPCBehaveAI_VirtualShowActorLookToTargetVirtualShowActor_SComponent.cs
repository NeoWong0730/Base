using DG.Tweening;
using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：虚拟Actor朝向指定虚拟Actor位置///
    /// 0: UID///
    /// 1: TargetUID///
    /// 2: Time///
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.VirtualShowActorLookToTargetVirtualShowActor)]
    public class WS_NPCBehaveAI_VirtualShowActorLookToTargetVirtualShowActor_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong uid;
        ulong targetUid;
        float time;
        VirtualSceneActor actor;
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

                uid = ulong.Parse(strs[0]);
                targetUid = ulong.Parse(strs[1]);
                time = float.Parse(strs[2]);

                if (VirtualShowManager.Instance.TryGetValue(uid, out actor))
                {
                    if (VirtualShowManager.Instance.TryGetValue(targetUid, out targetActor))
                    {
                        actor.gameObject.transform.DOLookAt(targetActor.gameObject.transform.position, time, AxisConstraint.Y, Vector3.up).onComplete = LookAtCompleted;
                    }
                    else
                    {
                        DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found targetVirtualSceneActor uid:{targetUid}");
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
                DebugUtil.LogError($"WS_NPCBehaveAI_VirtualShowActorLookToTargetVirtualShowActor_SComponent: {e.ToString()}");
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
            uid = targetUid = 0ul;
            time = 0f;
            actor = null;
            targetActor = null;

            base.Dispose();
        }
    }
}