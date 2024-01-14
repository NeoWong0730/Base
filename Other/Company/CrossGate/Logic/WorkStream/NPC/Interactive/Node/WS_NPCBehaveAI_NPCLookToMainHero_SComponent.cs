using DG.Tweening;
using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：NPC朝向玩家位置///
    /// 0: npcinfoid///
    /// 1: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.NPCLookToMainHero)]
    public class WS_NPCBehaveAI_NPCLookToMainHero_SComponent : StateBaseComponent
    {
        string[] strs;
        uint npcinfoid;
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

                npcinfoid = uint.Parse(strs[0]);
                time = float.Parse(strs[1]);

                if (GameCenter.uniqueNpcs.TryGetValue(npcinfoid, out npc))
                {
                    npc.transform.DOLookAt(GameCenter.mainHero.transform.position, time, AxisConstraint.Y, Vector3.up).onComplete = LookAtCompleted;
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found npc npcinfoid:{npcinfoid}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_NPCLookToMainHero_SComponent: {e.ToString()}");
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
            npcinfoid = 0u;
            time = 0f;
            npc = null;

            base.Dispose();
        }
    }
}
