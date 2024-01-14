using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.RotationByTime)]
public class WS_CombatBehaveAI_RotationByTime_SComponent : StateBaseComponent, IUpdate
{
    private MobEntity _mobEntity;

    private float _rotationSpeed;
    private float _totalTime;
    
    private Vector3 _eulerAngles;

    public override void Init(string str)
	{
        float[] strs = CombatHelp.GetStrParseFloat1Array(str);

        _mobEntity = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;
        
        _rotationSpeed = strs[0];
        _totalTime = strs[1];
        _eulerAngles = _mobEntity.m_Trans.eulerAngles;
	}

    public void Update()
    {
        if (_totalTime < 0f)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _eulerAngles.y += _rotationSpeed * Time.deltaTime;
        _mobEntity.m_Trans.eulerAngles = _eulerAngles;

        _totalTime -= Time.deltaTime;
    }
}