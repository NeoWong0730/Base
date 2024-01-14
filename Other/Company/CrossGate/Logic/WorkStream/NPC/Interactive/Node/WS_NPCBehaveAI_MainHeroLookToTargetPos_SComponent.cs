using DG.Tweening;
using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：玩家朝向指定位置///
    /// 0: TargetPosX///
    /// 1: TargetPosY///
    /// 2: TargetPosZ///
    /// 3: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.MainHeroLookToTargetPos)]
    public class WS_NPCBehaveAI_MainHeroLookToTargetPos_SComponent : StateBaseComponent
    {
        string[] strs;
        float TargetPosX;
        float TargetPosY;
        float TargetPosZ;
        float time;

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

                TargetPosX = float.Parse(strs[0]);
                TargetPosY = float.Parse(strs[1]);
                TargetPosZ = float.Parse(strs[2]);
                time = float.Parse(strs[3]);

                GameCenter.mainHero.gameObject.transform.DOLookAt(new Vector3(TargetPosX, TargetPosY, TargetPosZ), time, AxisConstraint.Y, Vector3.up).onComplete = LookAtCompleted;
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_MainHeroLookToTargetPos_SComponent: {e.ToString()}");
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
            TargetPosX = TargetPosY = TargetPosZ = 0f;

            base.Dispose();
        }
    }
}
