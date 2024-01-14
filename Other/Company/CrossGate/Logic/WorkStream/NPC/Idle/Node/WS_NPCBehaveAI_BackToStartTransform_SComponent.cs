using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：返回初始点位置///
    /// str: speed///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.BackToStartTransform)]
    public class WS_NPCBehaveAI_BackToStartTransform_SComponent : StateBaseComponent
    {
        WS_NPCDataComponent dataComponent;
        WS_NPCControllerEntity entity;
        float speed;
        public override void Init(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                speed = float.Parse(str);

                WS_NPCDataComponent dataComponent = GetNeedComponent<WS_NPCDataComponent>();

                var entity = (WS_NPCControllerEntity)m_CurUseEntity.m_StateControllerEntity;

                entity.m_MovementComponent.fMoveSpeed = speed;
                entity.m_MovementComponent.MoveTo(dataComponent.cacheStartPos, null, null, MoveToSuccess);

            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_BackToStartTransform_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }
        void MoveToSuccess()
        {
            m_CurUseEntity?.TranstionMultiStates(this);
        }

        public override void Dispose()
        {
            speed = 0f;
            entity = null;
            dataComponent = null;

            base.Dispose();
        }
    }
}
