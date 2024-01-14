#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCustomMobDataTest : MonoBehaviour
{
    public MobEntity m_MobEntity;

    public void Awake()
    {
        foreach (var kv in MobManager.Instance.m_MobDic)
        {
            if (kv.Value.m_Go == gameObject)
            {
                m_MobEntity = kv.Value;
                break;
            }
        }
    }
}
#endif