using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCheckCameraComponent : AComponent, IAwake, IUpdate
{
    public float _oldX;
    public float _oldY;
    public float _oldZ;

    private Transform _cameraTrans;

    private float _time;

    public override void Dispose()
    {
        if (MobManager.Instance != null && MobManager.Instance.m_MobDic != null)
        {
            foreach (var kv in MobManager.Instance.m_MobDic)
            {
                if (kv.Value == null)
                    continue;

                var mobCombatComponent = kv.Value.m_MobCombatComponent;
                if (mobCombatComponent == null)
                    continue;

                mobCombatComponent.m_NotSetTransPos = false;
            }
        }
        
        _cameraTrans = null;

        base.Dispose();
    }

    public void Awake()
    {
        _cameraTrans = CameraManager.mCamera.transform;
        Vector3 pos = _cameraTrans.position;
        _oldX = Get2Float(pos.x);
        _oldY = Get2Float(pos.y);
        _oldZ = Get2Float(pos.z);

        _time = 0f;
    }

    public void Update()
    {
#if UNITY_EDITOR
        if (CreateCombatDataTest.s_ClientType)
        {
            Dispose();
            return;
        }
#endif

        _time += Time.deltaTime;

        if (_time > 1f && !Net_Combat.Instance.m_RoundOver)
        {
            Dispose();
            return;
        }

        Vector3 pos = _cameraTrans.position;
        if (Get2Float(pos.x) != _oldX || Get2Float(pos.y) != _oldY || Get2Float(pos.z) != _oldZ)
        {
            _oldX = Get2Float(pos.x);
            _oldY = Get2Float(pos.y);
            _oldZ = Get2Float(pos.z);

            CombatManager.Instance.OnInitCombatData();

            foreach (var kv in MobManager.Instance.m_MobDic)
            {
                if (kv.Value == null)
                    continue;

                var mob = kv.Value;
                var mobCombatComponent = mob.m_MobCombatComponent;

                if (mobCombatComponent.m_DeathType != 1 && mob.m_Trans != null)
                    mobCombatComponent.RefreshPos(!mobCombatComponent.m_NotSetTransPos);

                DLogManager.Log(Lib.Core.ELogType.eCombat, $"CombatCheckCameraComponent 02   {kv.Value.m_Trans.name}--- {kv.Value.m_Trans.position}");
            }
        }
    }

    private float Get2Float(float f)
    {
        int i = (int)(f * 100f);
        return i / 100f;
    }
}
