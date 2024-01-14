using Lib.Core;
using Logic;
using Packet;
using Table;

public class MobDeadComponent : BaseComponent<MobEntity>, IAwake
{
    private ulong _effectModelId;

    public override void Dispose()
    {
        DelFx();
        
        base.Dispose();
    }

    public void Remove()
    {
        if (m_CurUseEntity != null && m_CurUseEntity.m_MobCombatComponent != null)
        {
            m_CurUseEntity.m_MobCombatComponent.PlaySuccessAnimation("action_idle", false, true);
        }

        Dispose();
    }

    public void Awake()
    {
        DebugUtil.Log(ELogType.eCombat, $"{(m_CurUseEntity.m_Go == null ? null : m_CurUseEntity.m_Go.name)}进入死亡组件MobDeadComponent");
        
        bool isNotOutDeath = true;
        if (m_CurUseEntity.m_MobCombatComponent.m_BeHitToFlyState)
        {
            isNotOutDeath = false;
        }
        else if (m_CurUseEntity.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Monster)
        {
            CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(m_CurUseEntity.m_MobCombatComponent.m_BattleUnit.UnitInfoId);
            if (cSVMonsterData == null || cSVMonsterData.dead_behavior != 1006u)
            {
                isNotOutDeath = false;
            }
        }

        if (isNotOutDeath)
        {
            _effectModelId = FxManager.Instance.ShowFX(1201001113u, null, m_CurUseEntity.m_Go, m_CurUseEntity.m_BindGameObjectDic);

            m_CurUseEntity.m_MobCombatComponent.OnShowMobUI(false);
        }
        else
            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnRemoveBattleUnit, m_CurUseEntity.m_MobCombatComponent.m_BattleUnit.UnitId);
    }

    public void DelFx()
    {
        DebugUtil.Log(ELogType.eCombat, $"{(m_CurUseEntity.m_Go == null ? null : m_CurUseEntity.m_Go.name)}退出死亡组件MobDeadComponent     _effectModelId:{_effectModelId.ToString()}");

        if (_effectModelId > 0ul)
        {
            FxManager.Instance.FreeFx(_effectModelId);
            _effectModelId = 0ul;
        }
    }
}
