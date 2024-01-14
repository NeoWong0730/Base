using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.WaitAttack_CombineAttack)]
public class WS_CombatBehaveAI_WaitAttack_CombineAttack_SComponent : StateBaseComponent, IUpdate
{
    private float _time;

    public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;

        var mobCombatComponent = ((MobEntity)cbace.m_WorkStreamManagerEntity.Parent).m_MobCombatComponent;
        if (cbace.m_SourcesTurnBehaveSkillInfo == null || !cbace.m_SourcesTurnBehaveSkillInfo.IsConverAttack)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _time = 0f;
        
        CombatManager.Instance.m_EventEmitter.Handle<uint>(CombatManager.EEvents.WaitAttack_CombineAttack, WaitAttack_CombineAttackEvt, true);

        Net_Combat.Instance.DoCombineAttackWaitCount(cbace.m_BehaveAIControllParam == null ? -1 : cbace.m_BehaveAIControllParam.ExcuteTurnIndex);
	}

    public override void Dispose()
    {
        CombatManager.Instance.m_EventEmitter.Handle<uint>(CombatManager.EEvents.WaitAttack_CombineAttack, WaitAttack_CombineAttackEvt, false);

        base.Dispose();
    }

    private void WaitAttack_CombineAttackEvt(uint unitId)
    {
        _time = 0f;

        if (unitId != ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).m_MobCombatComponent.m_BattleUnit.UnitId)
            return;

        m_CurUseEntity.TranstionMultiStates(this);
    }

    public void Update()
    {
        if (_time > 5f)
        {
            var mobCombatComponent = ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).m_MobCombatComponent;

            Lib.Core.DebugUtil.LogError($"攻击者ClientNum:{mobCombatComponent.m_ClientNum.ToString()} 等待攻击_合击状态下 合击有问题 通过修正修复");

            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _time += Time.deltaTime;
    }
}