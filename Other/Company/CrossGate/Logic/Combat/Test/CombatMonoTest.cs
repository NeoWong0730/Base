#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMonoTest : MonoBehaviour
{
    public bool IsCycleBattle = true;
    public uint LevelId = 1u;
    public bool StartFight;

    public bool Flag;
    public GameObject Go;
    private WS_CommunalAIManagerEntity _communalAIManagerEntity;

    public void Awake()
    {
        //CombatManager.Instance.OnAwake();
    }

    public void OnEnable()
    {
        //CombatManager.Instance.OnEnable();

#if UNITY_EDITOR
        CombatManager.Instance.m_KeepCombat = 1u;
#endif
    }

    public void Update()
    {
        //CombatManager.Instance.OnUpdate();

#if UNITY_EDITOR
        if(IsCycleBattle)
            CombatManager.Instance.m_KeepCombat = LevelId;
        else
            CombatManager.Instance.m_KeepCombat = 0u;

        if (StartFight)
        {
            StartFight = false;

            Logic.Sys_Fight.Instance.StartFightReq(CombatManager.Instance.m_KeepCombat);
        }
#endif

        if (Flag)
        {
            Flag = false;

            if (_communalAIManagerEntity != null)
                _communalAIManagerEntity.Dispose();

            _communalAIManagerEntity = WS_CommunalAIManagerEntity.Start(1u, Go);
        }
    }

    public void OnDisable()
    {
        //CombatManager.Instance.OnDisable();

#if UNITY_EDITOR
        CombatManager.Instance.m_KeepCombat = 0u;
#endif
    }

    public void OnDestroy()
    {
        //CombatManager.Instance.OnDestroy();
    }
}
#endif