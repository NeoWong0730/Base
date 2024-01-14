using Lib.Core;
using Table;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.WeaponType)]
public class WS_CombatBehaveAI_WeaponType_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        var mob = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;

        CSVEquipment.Data edTb = CSVEquipment.Instance.GetConfData(mob.m_MobCombatComponent.m_WeaponId);
        if (edTb == null)
        {
            DebugUtil.LogError($"CSVEquipment表中没有Id ：{mob.m_MobCombatComponent.m_WeaponId.ToString()}");
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }
        
        m_CurUseEntity.TranstionMultiStates(this, 1, (int)edTb.equipment_type);
    }
}