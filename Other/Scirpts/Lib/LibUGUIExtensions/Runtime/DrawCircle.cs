using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DrawCircle : MonoBehaviour {
#if UNITY_EDITOR
    private void Reset() {
        hideFlags = HideFlags.DontSaveInBuild;
    }

    public Transform center;
    public float r = 1f;
    public Color color = Color.red;

    private void OnDrawGizmos() {
        if (center != null) {
            var oldColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(center.position, r);
            Gizmos.color = oldColor;
        }
    }
#endif
}