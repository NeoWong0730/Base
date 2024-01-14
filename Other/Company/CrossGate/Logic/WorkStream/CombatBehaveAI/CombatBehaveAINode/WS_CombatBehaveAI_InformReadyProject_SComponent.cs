[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.InformReadyProject)]
public class WS_CombatBehaveAI_InformReadyProject_SComponent : StateBaseComponent, IUpdate
{
    private MobCombatComponent _protectedMobCombatComp;
    private MobCombatComponent _beProtectedMobCombatComp;

    private string _content;

    public override void Dispose()
    {
        _protectedMobCombatComp = null;
        _beProtectedMobCombatComp = null;

        _content = null;

        base.Dispose();
    }

    public override void Init(string str)
    {
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        
        int turnBehaveSkillTargetInfoIndex;
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = WS_CombatBehaveAIControllerEntity.
            GetTurnBehaveSkillTargetInfo(m_CurUseEntity, out turnBehaveSkillTargetInfoIndex);

        #region 检测目标是否在护卫中
        if (turnBehaveSkillTargetInfo != null)
        {
            bool isDoProtected = false;
            if (turnBehaveSkillTargetInfo.TargetUnitId > 0u)
            {
                MobEntity target = MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.TargetUnitId);
                if (target != null && target.m_MobCombatComponent != null &&
                    target.m_MobCombatComponent.m_IsDoProtected)
                {
                    isDoProtected = true;

                    _protectedMobCombatComp = target.m_MobCombatComponent;
                }
            }

            if (turnBehaveSkillTargetInfo.BeProtectUnitId > 0u)
            {
                MobEntity beProtectedMob = MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.BeProtectUnitId);
                if (beProtectedMob != null && beProtectedMob.m_MobCombatComponent != null &&
                    beProtectedMob.m_MobCombatComponent.m_IsDoProtected)
                {
                    isDoProtected = true;

                    _beProtectedMobCombatComp = beProtectedMob.m_MobCombatComponent;
                }
            }

            if (isDoProtected)
            {
                _content = str;
                return;
            }
        }
        #endregion

        DoInform(str, cbace, turnBehaveSkillTargetInfo, turnBehaveSkillTargetInfoIndex);
    }

    public void Update()
    {
        if ((_protectedMobCombatComp == null || !_protectedMobCombatComp.m_IsDoProtected) &&
            (_beProtectedMobCombatComp == null || !_beProtectedMobCombatComp.m_IsDoProtected))
        {
            DoInform(_content);
            return;
        }
    }

    private void DoInform(string content)
    {
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;

        int turnBehaveSkillTargetInfoIndex;
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = WS_CombatBehaveAIControllerEntity.
            GetTurnBehaveSkillTargetInfo(m_CurUseEntity, out turnBehaveSkillTargetInfoIndex);

        DoInform(content, cbace, turnBehaveSkillTargetInfo, turnBehaveSkillTargetInfoIndex);
    }

    private void DoInform(string content, WS_CombatBehaveAIControllerEntity cbace, 
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo, int turnBehaveSkillTargetInfoIndex)
    {
        uint workId = 0u;
        bool isReadyProject = false;
        if (!string.IsNullOrWhiteSpace(content))
        {
            uint[] us = CombatHelp.GetStrParseUint1Array(content);
            isReadyProject = us[0] == 0u;
            if (us.Length > 1)
                workId = us[1];
        }

        if (turnBehaveSkillTargetInfo != null && turnBehaveSkillTargetInfo.BeProtectUnitId > 0u)
        {
            WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();

            if (isReadyProject)
            {
                //TurnBehaveSkillTargetInfo preTbsti = dataComponent.GetTurnBehaveSkillTargetInfoByIndex(cbace, dataComponent.m_AttackTargetIndex - 1);
                //if (preTbsti != null)
                //{
                //    if (preTbsti.TargetUnitId == turnBehaveSkillTargetInfo.TargetUnitId &&
                //            preTbsti.BeProtectUnitId == turnBehaveSkillTargetInfo.BeProtectUnitId)
                //    {
                //        m_CurUseEntity.TranstionMultiStates(this);
                //        return;
                //    }
                //}

                if (cbace.DoTargetBlock(m_CurUseEntity, turnBehaveSkillTargetInfo,
                    turnBehaveSkillTargetInfoIndex, workId,
                    (int)CombatBehaveAIEnum.ReadyProtectBlock, true, true, true))
                {
                    m_CurUseEntity.TranstionMultiStates(this, 1, 1);
                    return;
                }
            }
            else
            {
                //if (cbace.m_SourcesTurnBehaveSkillInfo.TargetUnitCount > 1 &&
                //    dataComponent.m_AttackTargetIndex + 1 < cbace.m_SourcesTurnBehaveSkillInfo.TargetUnitCount)
                //{
                //    TurnBehaveSkillTargetInfo nextTbsti = dataComponent.GetTurnBehaveSkillTargetInfoByIndex(cbace, dataComponent.m_AttackTargetIndex + 1);
                //    if (nextTbsti != null)
                //    {
                //        if (nextTbsti.TargetUnitId == turnBehaveSkillTargetInfo.TargetUnitId &&
                //                nextTbsti.BeProtectUnitId == turnBehaveSkillTargetInfo.BeProtectUnitId)
                //        {
                //            turnBehaveSkillTargetInfo.IsProtectOver = false;

                //            m_CurUseEntity.TranstionMultiStates(this);
                //            return;
                //        }
                //    }
                //}

                turnBehaveSkillTargetInfo.IsProtectOver = true;
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
    }
}