using DG.Tweening;
using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：虚拟Actor朝向指定位置///
    /// 0: UID///
    /// 1: TargetPosX///
    /// 2: TargetPosY///
    /// 3: TargetPosZ///
    /// 4: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.VirtualShowActorLookToTargetPos)]
    public class WS_NPCBehaveAI_VirtualShowActorLookToTargetPos_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong uid;
        float TargetPosX;
        float TargetPosY;
        float TargetPosZ;
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
                TargetPosX = float.Parse(strs[1]);
                TargetPosY = float.Parse(strs[2]);
                TargetPosZ = float.Parse(strs[3]);
                time = float.Parse(strs[4]);

                if (VirtualShowManager.Instance.TryGetValue(uid, out actor))
                {
                    actor.gameObject.transform.DOLookAt(new Vector3(TargetPosX, TargetPosY, TargetPosZ), time, AxisConstraint.Y, Vector3.up).onComplete = LookAtCompleted;
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found VirtualSceneActor uid:{uid}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_VirtualShowActorLookToTargetPos_SComponent: {e.ToString()}");
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
            TargetPosX = TargetPosY = TargetPosZ = 0f;
            actor = null;

            base.Dispose();
        }
    }
}