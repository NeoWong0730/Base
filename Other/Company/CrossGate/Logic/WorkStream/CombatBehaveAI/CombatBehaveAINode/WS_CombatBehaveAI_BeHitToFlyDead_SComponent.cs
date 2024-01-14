using Lib.Core;
using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.BeHitToFlyDead)]
public class WS_CombatBehaveAI_BeHitToFlyDead_SComponent : StateBaseComponent, IUpdate
#if UNITY_EDITOR
    , IDrawGizmos
#endif
{
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_LeftUpPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_LeftUpPosZ), 0.5f);
        Gizmos.DrawLine(new Vector3(_orginX, _mobEntity.m_Trans.position.y, _orginZ),
            new Vector3(CombatManager.Instance.m_LeftUpPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_LeftUpPosZ));

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_RightUpPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_RightUpPosZ), 0.5f);
        Gizmos.DrawLine(new Vector3(_orginX, _mobEntity.m_Trans.position.y, _orginZ),
            new Vector3(CombatManager.Instance.m_RightUpPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_RightUpPosZ));

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_RightDownPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_RightDownPosZ), 0.5f);
        Gizmos.DrawLine(new Vector3(_orginX, _mobEntity.m_Trans.position.y, _orginZ),
            new Vector3(CombatManager.Instance.m_RightDownPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_RightDownPosZ));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_LeftDownPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_LeftDownPosZ), 0.5f);
        Gizmos.DrawLine(new Vector3(_orginX, _mobEntity.m_Trans.position.y, _orginZ),
            new Vector3(CombatManager.Instance.m_LeftDownPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_LeftDownPosZ));

        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawPolyLine(new Vector3(CombatManager.Instance.m_LeftUpPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_LeftUpPosZ),
            new Vector3(CombatManager.Instance.m_RightUpPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_RightUpPosZ),
            new Vector3(CombatManager.Instance.m_RightDownPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_RightDownPosZ),
            new Vector3(CombatManager.Instance.m_LeftDownPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_LeftDownPosZ),
            new Vector3(CombatManager.Instance.m_LeftUpPosX, _mobEntity.m_Trans.position.y, CombatManager.Instance.m_LeftUpPosZ));

        UnityEditor.Handles.DrawPolyLine(new Vector3(_orginX, _mobEntity.m_Trans.position.y, _orginZ),
            new Vector3(_orginX + _moveMaxLen * _moveX, _mobEntity.m_Trans.position.y, _orginZ + _moveMaxLen * _moveZ));
        UnityEditor.Handles.color = Color.white;
    }
#endif

    private MobEntity _mobEntity;

    private float _moveSpeed;
    private int _bounceSum;
    private float _rotationSpeed;

    private int _bounceCount;
    private Vector3 _eulerAngles;

    private float _moveX;
    private float _moveZ;

    private float _orginX;
    private float _orginZ;

    private float _moveMaxLen;
    private float _bounceMoveX;
    private float _bounceMoveZ;

    private float _time;
    public override void Init(string str)
	{
        if (string.IsNullOrEmpty(str))
        {
            DebugUtil.LogError($"WS_CombatBehaveAI_BeHitToFlyDead_SComponent  Init参数不对");
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        WS_CombatBehaveAIControllerEntity behaveAIController = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        _mobEntity = (MobEntity)behaveAIController.m_WorkStreamManagerEntity.Parent;
        var combatComponent = _mobEntity.m_MobCombatComponent;

        if (!behaveAIController.m_CurHpChangeData.m_Death || !combatComponent.m_BeHitToFlyState)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        float[] strs = CombatHelp.GetStrParseFloat1Array(str);
        _moveSpeed = strs[0];
        _bounceSum = (int)strs[1];
        _bounceCount = 0;
        _rotationSpeed = strs[2];
        _eulerAngles = _mobEntity.m_Trans.eulerAngles;

        _moveX = -_mobEntity.m_Trans.forward.x;
        _moveZ = -_mobEntity.m_Trans.forward.z;
        _bounceMoveX = _moveX;
        _bounceMoveZ = _moveZ;

        _moveMaxLen = 0f;

        _orginX = _mobEntity.m_Trans.position.x;
        _orginZ = _mobEntity.m_Trans.position.z;

        AudioUtil.PlayAudio(2107u);

        SetBounceData(_orginX, _orginZ);

        _time = 0f;
    }

    public void Update()
    {
        _eulerAngles.y += _rotationSpeed * Time.deltaTime;
        _mobEntity.m_Trans.eulerAngles = _eulerAngles;

        _time += Time.deltaTime;

        float moveLen = _time * _moveSpeed;

        if (_bounceCount >= _bounceSum)
        {
            if (_time > 1.5f)
            {
                _mobEntity.m_Trans.position = new Vector3(5000f, 5000f, 5000f);
                m_CurUseEntity.StateMachineOver();
            }
            else
            {
                float posX = _orginX + moveLen * _moveX;
                float posZ = _orginZ + moveLen * _moveZ;

                _mobEntity.m_Trans.position = new Vector3(posX, _mobEntity.m_Trans.position.y, posZ);
            }

            return;
        }

        if (moveLen < _moveMaxLen)
        {
            float posX = _orginX + moveLen * _moveX;
            float posZ = _orginZ + moveLen * _moveZ;

            _mobEntity.m_Trans.position = new Vector3(posX, _mobEntity.m_Trans.position.y, posZ);
        }
        else
        {
            AudioUtil.PlayAudio(2108u);

            float posX = _orginX + _moveMaxLen * _moveX;
            float posZ = _orginZ + _moveMaxLen * _moveZ;

            _mobEntity.m_Trans.position = new Vector3(posX, _mobEntity.m_Trans.position.y, posZ);

            _orginX = posX;
            _orginZ = posZ;
            _time = 0f;

            _bounceCount++;

            SetBounceData(posX, posZ);
        }
    }

    //获得当前反弹的移动单位速度和最大长度，下个反弹后反射单位速度
    public void SetBounceData(float posX, float posZ)
    {
        float ax = CombatManager.Instance.m_LeftUpPosX - posX;
        float az = CombatManager.Instance.m_LeftUpPosZ - posZ;

        float bx = CombatManager.Instance.m_RightUpPosX - posX;
        float bz = CombatManager.Instance.m_RightUpPosZ - posZ;

        float cx = CombatManager.Instance.m_RightDownPosX - posX;
        float cz = CombatManager.Instance.m_RightDownPosZ - posZ;

        float dx = CombatManager.Instance.m_LeftDownPosX - posX;
        float dz = CombatManager.Instance.m_LeftDownPosZ - posZ;

        float k1 = _bounceMoveZ * ax - az * _bounceMoveX;
        float k2 = _bounceMoveZ * bx - bz * _bounceMoveX;
        float k3 = _bounceMoveZ * cx - cz * _bounceMoveX;
        float k4 = _bounceMoveZ * dx - dz * _bounceMoveX;

        float axisX = 0f;
        float axisZ = 0f;

        if (k1 <= 0f && k2 > 0f) //上半部
        {
            axisX = CombatManager.Instance.m_Scene_U2D_AxisX;
            axisZ = CombatManager.Instance.m_Scene_U2D_AxisZ;

            if (_bounceCount > 0)
            {
                float targetPosX = (CombatManager.Instance.m_RightUpPosX + CombatManager.Instance.m_LeftUpPosX) * 0.5f;
                float targetPosZ = (CombatManager.Instance.m_RightUpPosZ + CombatManager.Instance.m_LeftUpPosZ) * 0.5f;

                _moveX = targetPosX - posX;
                _moveZ = targetPosZ - posZ;

                _moveMaxLen = Mathf.Sqrt(_moveX * _moveX + _moveZ * _moveZ);

                _moveX = _moveX / _moveMaxLen;
                _moveZ = _moveZ / _moveMaxLen;
            }
            else
                _moveMaxLen = (ax * axisX + az * axisZ) / (_moveX * axisX + _moveZ * axisZ);
        }
        else if (k2 <= 0f && k3 > 0f) //右半部
        {
            axisX = CombatManager.Instance.m_Scene_R2L_AxisX;
            axisZ = CombatManager.Instance.m_Scene_R2L_AxisZ;

            if (_bounceCount > 0)
            {
                float targetPosX = (CombatManager.Instance.m_RightDownPosX + CombatManager.Instance.m_RightUpPosX) * 0.5f;
                float targetPosZ = (CombatManager.Instance.m_RightDownPosZ + CombatManager.Instance.m_RightUpPosZ) * 0.5f;

                _moveX = targetPosX - posX;
                _moveZ = targetPosZ - posZ;

                _moveMaxLen = Mathf.Sqrt(_moveX * _moveX + _moveZ * _moveZ);

                _moveX = _moveX / _moveMaxLen;
                _moveZ = _moveZ / _moveMaxLen;
            }
            else
                _moveMaxLen = (bx * axisX + bz * axisZ) / (_moveX * axisX + _moveZ * axisZ);
        }
        else if (k3 <= 0f && k4 > 0f) //下半部
        {
            axisX = CombatManager.Instance.m_Scene_D2U_AxisX;
            axisZ = CombatManager.Instance.m_Scene_D2U_AxisZ;

            if (_bounceCount > 0)
            {
                float targetPosX = (CombatManager.Instance.m_LeftDownPosX + CombatManager.Instance.m_RightDownPosX) * 0.5f;
                float targetPosZ = (CombatManager.Instance.m_LeftDownPosZ + CombatManager.Instance.m_RightDownPosZ) * 0.5f;

                _moveX = targetPosX - posX;
                _moveZ = targetPosZ - posZ;

                _moveMaxLen = Mathf.Sqrt(_moveX * _moveX + _moveZ * _moveZ);

                _moveX = _moveX / _moveMaxLen;
                _moveZ = _moveZ / _moveMaxLen;
            }
            else
                _moveMaxLen = (cx * axisX + cz * axisZ) / (_moveX * axisX + _moveZ * axisZ);
        }
        else if (k1 > 0f && k4 <= 0f) //左半部
        {
            axisX = CombatManager.Instance.m_Scene_L2R_AxisX;
            axisZ = CombatManager.Instance.m_Scene_L2R_AxisZ;

            if (_bounceCount > 0)
            {
                float targetPosX = (CombatManager.Instance.m_LeftUpPosX + CombatManager.Instance.m_LeftDownPosX) * 0.5f;
                float targetPosZ = (CombatManager.Instance.m_LeftUpPosZ + CombatManager.Instance.m_LeftDownPosZ) * 0.5f;

                _moveX = targetPosX - posX;
                _moveZ = targetPosZ - posZ;

                _moveMaxLen = Mathf.Sqrt(_moveX * _moveX + _moveZ * _moveZ);

                _moveX = _moveX / _moveMaxLen;
                _moveZ = _moveZ / _moveMaxLen;
            }
            else
                _moveMaxLen = (dx * axisX + dz * axisZ) / (_moveX * axisX + _moveZ * axisZ);
        }

        if (_moveMaxLen < 0f)
            _moveMaxLen = -_moveMaxLen;

        float l = _moveX * axisX + _moveZ * axisZ;

        _bounceMoveX = _moveX - 2 * l * axisX;
        _bounceMoveZ = _moveZ - 2 * l * axisZ;
    }
}