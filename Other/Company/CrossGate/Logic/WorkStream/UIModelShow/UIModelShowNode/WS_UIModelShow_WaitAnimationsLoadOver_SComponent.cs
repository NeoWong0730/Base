using UnityEngine;

[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.WaitAnimationsLoadOver)]
public class WS_UIModelShow_WaitAnimationsLoadOver_SComponent : StateBaseComponent, IUpdate
{
    private float _time;
    private int _count;

    public override void Init(string str)
	{
        _count = int.Parse(str);
    }
    
    public void Update()
    {
        _time += Time.deltaTime;

        if (_time > 0.5f)
        {
            _time = 0f;

            WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;

            if (uiModelShowManager.m_AnimationComponent != null)
            {
                if (uiModelShowManager.m_AnimationComponent.GetAnimationsCount() >= _count)
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            else
            {
                if (uiModelShowManager.m_AnimationControl.GetAnimationsCount() >= _count)
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
        }
    }
}