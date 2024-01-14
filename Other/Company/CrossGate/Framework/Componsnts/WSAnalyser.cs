using System;
using UnityEngine;
using UnityEngine.UI;

public class WSAnalyser : MonoBehaviour {
    public Vector3 p;
    public Vector3 e;
    public Vector3 s;

    public Transform localTarget;

    private void Update() {
        p = transform.position;
        e = transform.eulerAngles;
        s = transform.localScale;
    }

    [ContextMenu("CopyToTarget")]
    public void Set() {
        if (localTarget != null) {
            localTarget.position = p;
            localTarget.eulerAngles = e;
            localTarget.localScale = s;
        }
    }
}
