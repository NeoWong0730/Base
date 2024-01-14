using UnityEngine;

[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.PlayEffect)]
public class WS_UIModelShow_PlayEffect_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;
        if (uiModelShowManager.m_Go == null)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        WS_UIModelShowDataComponent dataComponent = GetNeedComponent<WS_UIModelShowDataComponent>();

        uint effectTbId = uint.Parse(str);

        var effectModelId = FxManager.Instance.ShowFX(effectTbId, null, uiModelShowManager.m_Go, null, delegate (GameObject fxGo, ulong modelId)
        {
            if (modelId != 0ul)
                dataComponent.AddEffect(modelId, effectTbId);

            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }, -1f, false, 0f, false, 1f, 10001);

        if (effectModelId == 0ul)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }
        else
        {
            dataComponent.AddEffect(effectModelId, effectTbId);
        }
    }
}