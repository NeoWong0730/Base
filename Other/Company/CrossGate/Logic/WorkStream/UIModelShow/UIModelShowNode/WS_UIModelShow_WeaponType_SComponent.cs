using Lib.Core;
using Table;

[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.WeaponType)]
public class WS_UIModelShow_WeaponType_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;
        if (uiModelShowManager.m_WeaponId == 0u)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        CSVEquipment.Data edTb = CSVEquipment.Instance.GetConfData(uiModelShowManager.m_WeaponId);
        if (edTb == null)
        {
            DebugUtil.LogError($"CSVEquipment表中没有Id ：{uiModelShowManager.m_WeaponId.ToString()}");
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        m_CurUseEntity.TranstionMultiStates(this, 1, (int)edTb.equipment_type);
	}
}