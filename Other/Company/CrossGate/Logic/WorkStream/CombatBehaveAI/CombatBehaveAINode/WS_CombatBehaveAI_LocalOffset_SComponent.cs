using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.LocalOffset)]
public class WS_CombatBehaveAI_LocalOffset_SComponent : StateBaseComponent, IUpdate
{
    private MobEntity _mob;
    private float _totalTime;
    private float _time;
    private float _offsetX;
    private float _offsetY;
    private float _offsetZ;
    private float _g;
    private Vector3 _originV3;

    public override void Dispose()
    {
        _mob = null;

        base.Dispose();
    }

    public override void Init(string str)
	{
        float[] fs = CombatHelp.GetStrParseFloat1Array(str);

        _mob = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;
        Transform trans = _mob.m_Trans;
        
        if (fs.Length < 4)
        {
            trans.position += trans.right * fs[0] + trans.up * fs[1] + trans.forward * fs[2];

            CombatCheckMobPosComponent combatCheckMobPosComponent = _mob.GetComponent<CombatCheckMobPosComponent>();
            if (combatCheckMobPosComponent != null)
            {
                combatCheckMobPosComponent.SetResetPos(trans.position, false);
            }

            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _originV3 = trans.position;
        _offsetX = fs[0];
        _offsetY = fs[1];
        _offsetZ = fs[2];
        _totalTime = fs[3];
        _g = fs[4];

        if (fs.Length > 5)
        {
            var typeMob = WS_CombatBehaveAIControllerEntity.GetMobByType(m_CurUseEntity, (int)fs[5]);
            if (typeMob != null)
                _mob = typeMob;
        }

        _time = 0f;
	}

    public void Update()
    {
        Transform trans = _mob.m_Trans;

        if (_time > _totalTime)
        {
            trans.position = _originV3 + trans.right * _offsetX + trans.up * _offsetY + trans.forward * _offsetZ;
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        float x = 0f;
        if (_offsetX != 0f)
        {
            x = (_offsetX / _totalTime + 0.5f * _g * _time) * _time;
            if (Mathf.Abs(x) > Mathf.Abs(_offsetX))
                x = _offsetX;
        }

        float y = 0f;
        if (_offsetY != 0f)
        {
            y = (_offsetY / _totalTime + 0.5f * _g * _time) * _time;
            if (Mathf.Abs(y) > Mathf.Abs(_offsetY))
                y = _offsetY;
        }

        float z = 0f;
        if (_offsetZ != 0f)
        {
            z = (_offsetZ / _totalTime + 0.5f * _g * _time) * _time;
            if (Mathf.Abs(z) > Mathf.Abs(_offsetZ))
                z = _offsetZ;
        }


        trans.position = _originV3 + trans.right * x + trans.up * y + trans.forward * z;

        _time += Time.deltaTime;
    }
}