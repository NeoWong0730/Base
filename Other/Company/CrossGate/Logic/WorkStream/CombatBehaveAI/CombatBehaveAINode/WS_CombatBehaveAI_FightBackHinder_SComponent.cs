[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.FightBackHinder)]
public class WS_CombatBehaveAI_FightBackHinder_SComponent : StateBaseComponent
{
    private uint _beFightBackUnitId;
    private uint _fightBackUnitId;
    private string _flagStr;

    public override void Dispose()
    {
        CombatManager.Instance.m_EventEmitter.Handle<uint, uint, uint, string>(CombatManager.EEvents.WaitFightBackOver, WaitFightBackOverEvt, false);

        _flagStr = null;

        base.Dispose();
    }

    public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
        if (mob != null && mob.m_MobCombatComponent != null && mob.m_MobCombatComponent.m_BattleUnit != null)
        {
            TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = WS_CombatBehaveAIControllerEntity.GetTurnBehaveSkillTargetInfo(m_CurUseEntity);

            _beFightBackUnitId = mob.m_MobCombatComponent.m_BattleUnit.UnitId;
            _fightBackUnitId = turnBehaveSkillTargetInfo.TargetUnitId;
            _flagStr = str;

            CombatManager.Instance.m_EventEmitter.Handle<uint, uint, uint, string>(CombatManager.EEvents.WaitFightBackOver, WaitFightBackOverEvt, true);

            return;
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}

    private void WaitFightBackOverEvt(uint beFightBackUnitId, uint fightBackUnitId, uint fightBackType, string flagStr)
    {
        if (beFightBackUnitId != _beFightBackUnitId || fightBackUnitId != _fightBackUnitId || flagStr != _flagStr)
            return;

        if (fightBackType == 1)
            m_CurUseEntity.StateMachineOver();
        else
            m_CurUseEntity.TranstionMultiStates(this);
    }
}