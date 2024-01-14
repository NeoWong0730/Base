using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MoveForward)]
public class WS_CombatBehaveAI_MoveForward_SComponent : StateBaseComponent, IUpdate
{
    private MobEntity _mobEntity;
    private float _speed;
    private float _time;

    public override void Init(string str)
	{
        _mobEntity = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;

        float[] fs = CombatHelp.GetStrParseFloat1Array(str);
        _speed = fs[0];
        _time = Time.time + fs[1];
    }

    public void Update()
    {
        if (Time.time > _time)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _mobEntity.m_Trans.position += Time.deltaTime * _speed * _mobEntity.m_Trans.forward;
    }
}