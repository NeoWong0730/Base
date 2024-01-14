using UnityEngine;

[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.ShowWeaponModel)]
public class WS_UIModelShow_ShowWeaponModel_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;
        if (uiModelShowManager.m_WeaponGo != null)
            uiModelShowManager.m_WeaponGo.SetActive(int.Parse(str) == 1);

        uiModelShowManager.GetOtherWeapon(out GameObject weapon02Go, out Transform weapon02ParentTrans);
        if (weapon02Go != null)
            weapon02Go.SetActive(int.Parse(str) == 1);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}