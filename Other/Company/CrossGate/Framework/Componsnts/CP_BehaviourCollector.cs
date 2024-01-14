using System.Collections.Generic;
using UnityEngine;

public class CP_BehaviourCollector : MonoBehaviour
{
    // public string componentName = "DynamicBone";
    public Behaviour[] cps = new Behaviour[0];

    public void Awake()
    {
        cps = transform.GetComponentsInChildren<DynamicBone>(true) as Behaviour[];
    }

    public void Enable(bool toEnable)
    {
        foreach (var cp in cps)
        {
            cp.enabled = toEnable;
        }
    }
}
