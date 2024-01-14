using DG.Tweening;
using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：虚拟Actor朝向指定NPC位置///
    /// 0: UID///
    /// 1: TargetNPCID///
    /// 2: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.VirtualShowActorLookToNPC)]
    public class WS_NPCBehaveAI_VirtualShowActorLookToNPC_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong uid;
        uint targetNpcID;
        float time;
        VirtualSceneActor actor;
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

                uid = ulong.Parse(strs[0]);
                targetNpcID = uint.Parse(strs[1]);
                time = float.Parse(strs[2]);

                if (VirtualShowManager.Instance.TryGetValue(uid, out actor))
                {
                    if (GameCenter.uniqueNpcs.TryGetValue(targetNpcID, out npc))
                    {
                        actor.transform.DOLookAt(npc.gameObject.transform.position, time, AxisConstraint.Y, Vector3.up).onComplete = LookAtCompleted;
                    }
                    else
                    {
                        DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found Npc npcinfoid:{targetNpcID}");
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
                DebugUtil.LogError($"WS_NPCBehaveAI_VirtualShowActorLookToNPC_SComponent: {e.ToString()}");
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
            targetNpcID = 0u;
            time = 0f;
            actor = null;
            npc = null;

            base.Dispose();
        }
    }
}
