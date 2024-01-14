using UnityEngine;

[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.WeaponModelAwayBone)]
public class WS_UIModelShow_WeaponModelAwayBone_SComponent : StateBaseComponent
{
    private string _findBoneName;

    public override void Dispose()
    {
        _findBoneName = null;

        base.Dispose();
    }

    public override void Init(string str)
	{
        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;
        
        InitWeapon(str, uiModelShowManager, uiModelShowManager.m_WeaponGo, uiModelShowManager.m_WeaponParentTrans);

        uiModelShowManager.GetOtherWeapon(out GameObject weapon02Go, out Transform weapon02ParentTrans);
        InitWeapon(str, uiModelShowManager, weapon02Go, weapon02ParentTrans);

        m_CurUseEntity.TranstionMultiStates(this);
	}

    private void InitWeapon(string str, WS_UIModelShowManagerEntity uiModelShowManager, 
        GameObject weaponGo, Transform weaponParentTrans)
    {
        if (weaponGo != null)
        {
            Transform weaponTrans = weaponGo.transform;

            WS_UIModelShowDataComponent dataComp = m_CurUseEntity.GetNeedComponent<WS_UIModelShowDataComponent>();

            if (string.IsNullOrWhiteSpace(str))
            {
                weaponTrans.SetParent(weaponParentTrans);

                SetOriginTrans(weaponTrans);
            }
            else
            {
                if (dataComp.m_BoneTrans == null && uiModelShowManager.m_Go != null)
                {
                    _findBoneName = str;
                    dataComp.m_BoneTrans = GetChildsBone(uiModelShowManager.m_Go.transform);
                }

                weaponTrans.SetParent(dataComp.m_BoneTrans);
            }
        }
    }

    private void SetOriginTrans(Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localEulerAngles = Vector3.zero;
        trans.localScale = Vector3.one;
    }

    private Transform GetChildsBone(Transform trans)
    {
        Transform boneTrans = null;
        for (int i = 0, childCount = trans.childCount; i < childCount; i++)
        {
            var child = trans.GetChild(i);
            if (child.name == _findBoneName)
            {
                return child;
            }

            boneTrans = GetChildsBone(child);
            if (boneTrans != null)
                return boneTrans;
        }

        return boneTrans;
    }
}