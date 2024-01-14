[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.DelEffect)]
public class WS_UIModelShow_DelEffect_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        //删除特效
        if (string.IsNullOrEmpty(str))
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        uint effectTbId = uint.Parse(str);
        WS_UIModelShowDataComponent dataComponent = GetNeedComponent<WS_UIModelShowDataComponent>();
        for (int i = 0, count = dataComponent.m_EffectDataList.Count; i < count; i++)
        {
            var ed = dataComponent.m_EffectDataList[i];
            if (ed.m_EffectTbId == effectTbId)
            {
                FxManager.Instance.FreeFx(ed.m_EffectModelId);
                CombatObjectPool.Instance.Push(ed);
                dataComponent.m_EffectDataList.RemoveAt(i);
                break;
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
    }
}