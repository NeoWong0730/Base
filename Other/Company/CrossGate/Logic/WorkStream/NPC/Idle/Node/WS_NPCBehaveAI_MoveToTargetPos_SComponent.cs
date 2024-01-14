using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：自身向目标点位移///
    /// 0: TargetPosX///
    /// 1: TargetPosY///
    /// 2: TargetPosZ///
    /// 3: Speed///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.MoveToTargetPos)]
    public class WS_NPCBehaveAI_MoveToTargetPos_SComponent : StateBaseComponent
    {
        string[] strs;
        float TargetPosX;
        float TargetPosY;
        float TargetPosZ;
        float Speed;
        WS_NPCControllerEntity entity;

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
                Speed = float.Parse(strs[3]);

                entity = (WS_NPCControllerEntity)m_CurUseEntity.m_StateControllerEntity;

                entity.m_MovementComponent.fMoveSpeed = Speed;
                entity.m_MovementComponent.MoveTo(new Vector3(TargetPosX, TargetPosY, TargetPosZ), null, null, MoveToSuccess);
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_MoveToTargetPos_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        void MoveToSuccess()
        {
            m_CurUseEntity?.TranstionMultiStates(this);
        }

        public override void Dispose()
        {
            strs = null;
            TargetPosX = TargetPosY = TargetPosZ = Speed = 0f;
            entity = null;

            base.Dispose();
        }
    }
}
