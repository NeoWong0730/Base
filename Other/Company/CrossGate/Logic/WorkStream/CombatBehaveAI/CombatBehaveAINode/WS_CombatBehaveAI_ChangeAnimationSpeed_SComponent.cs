using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.ChangeAnimationSpeed)]
public class WS_CombatBehaveAI_ChangeAnimationSpeed_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        var mobCombatComponent = ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).m_MobCombatComponent;
        float[] fs = CombatHelp.GetStrParseFloat1Array(str);
        if (fs[2] > 0f && fs[0] != fs[1])
            m_CurUseEntity.GetNeedComponent<ChangeAnimationSpeedComponent>().Init(mobCombatComponent, fs[0], fs[1], fs[2]);
        else
            mobCombatComponent.m_AnimationComponent.SetSpeed(fs[1]);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}
public class ChangeAnimationSpeedComponent : BaseComponent<StateMachineEntity>, IUpdate
{
    private MobCombatComponent _mobCombatComponent;
    private float _startSpeed;
    private float _endSpeed;
    private float _time;
    private float _perAddSpeed;

    public void Init(MobCombatComponent mobCombatComponent, float startSpeed, float endSpeed, float totalTime)
    {
        _mobCombatComponent = mobCombatComponent;
        _startSpeed = startSpeed;
        _endSpeed = endSpeed;

        _time = Time.time + totalTime;
        _perAddSpeed = (_endSpeed - _startSpeed) / totalTime;
    }

    public override void Dispose()
    {
        _mobCombatComponent = null;

        base.Dispose();
    }

    public void Update()
    {
        if (Time.time > _time)
        {
            _mobCombatComponent.m_AnimationComponent.SetSpeed(_endSpeed);
            Dispose();
            return;
        }

        _startSpeed += _perAddSpeed * Time.deltaTime;
        _mobCombatComponent.m_AnimationComponent.SetSpeed(_startSpeed);
    }
}