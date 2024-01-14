using DG.Tweening;
using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：虚拟Actor朝向玩家位置///
    /// 0: UID///
    /// 1: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.VirtualShowActorLookToMainHero)]
    public class WS_NPCBehaveAI_VirtualShowActorLookToMainHero_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong uid;
        float time;
        VirtualSceneActor actor;

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
                time = float.Parse(strs[1]);

                if (VirtualShowManager.Instance.TryGetValue(uid, out actor))
                {
                    actor.gameObject.transform.DOLookAt(GameCenter.mainHero.transform.position, time, AxisConstraint.Y, Vector3.up).onComplete = LookAtCompleted;
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found VirtualSceneActor uid:{uid}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_VirtualShowActorLookToMainHero_SComponent: {e.ToString()}");
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
            uid = 0ul;
            time = 0f;
            actor = null;

            base.Dispose();
        }
    }
}