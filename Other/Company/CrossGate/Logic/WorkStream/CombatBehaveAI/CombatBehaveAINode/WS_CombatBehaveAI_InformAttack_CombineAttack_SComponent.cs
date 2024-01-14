using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.InformAttack_CombineAttack)]
public class WS_CombatBehaveAI_InformAttack_CombineAttack_SComponent : StateBaseComponent, IUpdate
{
    private float _delayTime;
    private float _time;

    public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity behaveAIController = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;

        if (behaveAIController.m_CurHpChangeData != null &&
            !CombatHelp.ContainAnimType(behaveAIController.m_CurHpChangeData.m_AnimType, Logic.AnimType.e_ConverAttack))
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        if (string.IsNullOrEmpty(str))
            _delayTime = 0f;
        else
            _delayTime = float.Parse(str);

        _time = 0f;
    }

    public void Update()
    {
        WS_CombatBehaveAIControllerEntity behaveAIController = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;

        if (_time > 3f)
        {
#if DEBUG_MODE
            var curMobEntity = (MobEntity)behaveAIController.m_WorkStreamManagerEntity.Parent;
            string attackClientNumStr = string.Empty;
            if (behaveAIController.m_BehaveAIControllParam != null)
            {
                NetExcuteTurnInfo excuteTurnData = Net_Combat.Instance.GetNetExcuteTurnInfo(behaveAIController.m_BehaveAIControllParam.ExcuteTurnIndex);
                foreach (var unitId in excuteTurnData.CombineAttack_SrcUnits)
                {
                    MobEntity mobEntity = MobManager.Instance.GetMob(unitId);
                    if (mobEntity != null)
                        attackClientNumStr += $" [{mobEntity.m_MobCombatComponent.m_ClientNum.ToString()}]";
                }
            }

            Lib.Core.DebugUtil.LogError($"被击者ClientNum:{curMobEntity.m_MobCombatComponent.m_ClientNum.ToString()}  攻击者ClientNum:{attackClientNumStr}  通知攻击_合击状态下 合击有问题 通过修正修复");
#endif

            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _time += Time.deltaTime;

        if (_time > _delayTime)
        {
            if (behaveAIController.m_BehaveAIControllParam == null)
            {
                m_CurUseEntity.TranstionMultiStates(this);
                return;
            }

            NetExcuteTurnInfo excuteTurnData = Net_Combat.Instance.GetNetExcuteTurnInfo(behaveAIController.m_BehaveAIControllParam.ExcuteTurnIndex);
            if (excuteTurnData == null)
            {
                m_CurUseEntity.TranstionMultiStates(this);
                return;
            }

            var curMobEntity = (MobEntity)behaveAIController.m_WorkStreamManagerEntity.Parent;
            
            MobEntity mobEntity = MobManager.Instance.GetMob(excuteTurnData.CombineAttack_SrcUnits[excuteTurnData.CombineAttack_ScrIndex]);
            if (mobEntity != null)
                curMobEntity.m_Trans.LookAt(mobEntity.m_Trans.position);

            excuteTurnData.CombineAttack_ScrIndex++;

            if (excuteTurnData.CombineAttack_ScrIndex < excuteTurnData.CombineAttack_SrcUnits.Count)
            {
                curMobEntity.m_MobCombatComponent.UpdateBehaveCount(-1);

                CombatManager.Instance.m_EventEmitter.Trigger(CombatManager.EEvents.WaitAttack_CombineAttack, excuteTurnData.CombineAttack_SrcUnits[excuteTurnData.CombineAttack_ScrIndex]);
            }
            else
            {
                m_CurUseEntity.TranstionMultiStates(this);
            }

            _delayTime = float.MaxValue;
        }
    }
}