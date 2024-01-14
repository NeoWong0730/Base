using DG.Tweening;
using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：玩家朝向指定虚拟Actor位置///
    /// 0: TargetUID///
    /// 1: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.MainHeroLookToTargetVirtualShowActor)]
    public class WS_NPCBehaveAI_MainHeroLookToTargetVirtualShowActor_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong targetUid;
        float time;
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

                targetUid = ulong.Parse(strs[0]);
                time = float.Parse(strs[1]);

                if (VirtualShowManager.Instance.TryGetValue(targetUid, out targetActor))
                {
                    GameCenter.mainHero.gameObject.transform.DOLookAt(targetActor.gameObject.transform.position, time, AxisConstraint.Y, Vector3.up).onComplete = LookAtCompleted;
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found targetVirtualSceneActor uid:{targetUid}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_MainHeroLookToTargetVirtualShowActor_SComponent: {e.ToString()}");
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
            targetUid = 0ul;
            time = 0f;
            targetActor = null;

            base.Dispose();
        }
    }
}
