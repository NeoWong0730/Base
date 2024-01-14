using DG.Tweening;
using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：玩家朝向指定NPC位置///
    /// 0: TargetNPCID///
    /// 1: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.MainHeroLookToNPC)]
    public class WS_NPCBehaveAI_MainHeroLookToNPC_SComponent : StateBaseComponent
    {
        string[] strs;
        uint targetNpcID;
        float time;
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

                targetNpcID = uint.Parse(strs[0]);
                time = float.Parse(strs[1]);

                if (GameCenter.uniqueNpcs.TryGetValue(targetNpcID, out npc))
                {
                    GameCenter.mainHero.transform.DOLookAt(npc.gameObject.transform.position, time, AxisConstraint.Y, Vector3.up).onComplete = LookAtCompleted;
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found Npc npcinfoid:{targetNpcID}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_MainHeroLookToNPC_SComponent: {e.ToString()}");
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
            targetNpcID = 0u;
            time = 0f;
            npc = null;

            base.Dispose();
        }
    }
}
