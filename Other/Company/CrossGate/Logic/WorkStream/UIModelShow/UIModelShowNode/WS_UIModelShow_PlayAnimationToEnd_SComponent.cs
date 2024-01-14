﻿[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.PlayAnimationToEnd)]
public class WS_UIModelShow_PlayAnimationToEnd_SComponent : StateBaseComponent
{
    public override void Init(string str)
	{
        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;

        if (uiModelShowManager == null || (uiModelShowManager.m_AnimationComponent == null && uiModelShowManager.m_AnimationControl == null) ||
            str == uiModelShowManager.m_CurPlayAanimtionName)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        if (uiModelShowManager.m_AnimationComponent != null)
        {
            try
            {
                uiModelShowManager.m_AnimationComponent.Stop(str);
            }
            catch
            {
                uiModelShowManager.m_AnimationComponent.StopPlayOverTime();
            }
            uiModelShowManager.m_AnimationComponent.CrossFadeSuccess(str, 0.4f, () => 
            {
                if (m_CurUseEntity != null)
                    m_CurUseEntity.TranstionMultiStates(this);
            });
            uiModelShowManager.m_CurPlayAanimtionName = str;
        }
        else
        {
            try
            {
                uiModelShowManager.m_AnimationControl.Stop(str);
            }
            catch 
            {
                uiModelShowManager.m_AnimationControl.StopPlayOverTime();
            }
            uiModelShowManager.m_AnimationControl.CrossFadeAction(str, 0.4f, () =>
            {
                if(m_CurUseEntity != null)
                    m_CurUseEntity.TranstionMultiStates(this);
            });
            uiModelShowManager.m_CurPlayAanimtionName = str;
        }
    }
}