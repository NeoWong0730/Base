using System.Collections.Generic;
using UnityEngine;

public class CP_TransformCollector : MonoBehaviour
{
    [System.Serializable]
    public class TransformNode
    {
        public Transform parent;
        public Transform node;
    }
    public TransformNode[] cps = new TransformNode[0];

    private void Awake()
    {
        foreach (var cp in cps)
        {
            cp.node.SetParent(cp.parent, false);
        }
    }
    public void Enable(bool toEnable)
    {
        foreach (var cp in cps)
        {
            cp.node.gameObject.SetActive(toEnable);
        }
    }
}
