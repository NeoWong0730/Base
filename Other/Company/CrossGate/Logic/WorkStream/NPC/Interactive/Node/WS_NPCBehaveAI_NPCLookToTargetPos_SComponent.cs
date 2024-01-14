using DG.Tweening;
using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：NPC朝向指定位置///
    /// 0: NPCInfoID///
    /// 1: TargetPosX///
    /// 2: TargetPosY///
    /// 3: TargetPosZ///
    /// 4: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.NPCLookToTargetPos)]
    public class WS_NPCBehaveAI_NPCLookToTargetPos_SComponent : StateBaseComponent
    {
        string[] strs;
        uint npcInfoID;
        float TargetPosX;
        float TargetPosY;
        float TargetPosZ;
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

                npcInfoID = uint.Parse(strs[0]);
                TargetPosX = float.Parse(strs[1]);
                TargetPosY = float.Parse(strs[2]);
                TargetPosZ = float.Parse(strs[3]);
                time = float.Parse(strs[4]);

                if (GameCenter.uniqueNpcs.TryGetValue(npcInfoID, out npc))
                {
                    npc.transform.DOLookAt(new Vector3(TargetPosX, TargetPosY, TargetPosZ), time, AxisConstraint.Y, Vector3.up).onComplete = LookAtCompleted;
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found Npc npcinfoid:{npcInfoID}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_NPCLookToTargetPos_SComponent: {e.ToString()}");
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
            TargetPosX = TargetPosY = TargetPosZ = 0f;
            npc = null;

            base.Dispose();
        }
    }
}