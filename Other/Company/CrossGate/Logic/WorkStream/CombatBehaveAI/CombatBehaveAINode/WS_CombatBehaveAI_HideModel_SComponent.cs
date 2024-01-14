using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.HideModel)]
public class WS_CombatBehaveAI_HideModel_SComponent : StateBaseComponent, IUpdate
{
    private float _totalTime;
    private float _time;
    private int _count;

    private Transform _trans;

    public override void Init(string str)
	{
        if (string.IsNullOrEmpty(str))
            _totalTime = 3f;
        else
            _totalTime = float.Parse(str);

        _time = 0f;
        _count = 0;

        _trans = ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).m_Trans;
    }

    public override void Dispose()
    {
        if (_trans != null)
            _trans.position = CombatHelp.FarV3;
        _trans = null;

        base.Dispose();
    }

    public void Update()
    {
        if (_time > _totalTime)
        {
            _trans.position = CombatHelp.FarV3;

            MobDeadComponent mobDeadComponent = ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).GetComponent<MobDeadComponent>();
            if (mobDeadComponent != null)
                mobDeadComponent.DelFx();

            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _time += Time.deltaTime;
        _count++;

        if (_count % 4 == 0)
        {
            _trans.position = CombatHelp.FarV3;
        }
        else if (_count % 2 == 0)
        {
            ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).m_MobCombatComponent.ResetPos();
        }
    }
}