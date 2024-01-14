using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ReplaceComponent<T> : MonoBehaviour where T : MonoBehaviour {
    public static T defaultT = null;
    public static T finalT = null;

    public T cur = null;

    public virtual Action<int> replace => null;

    private void Awake() {
        if (defaultT == null) {
            defaultT = gameObject.AddComponent<T>();
        }

        if (finalT == null) {
            finalT = defaultT;
            replace?.Invoke(3);
        }
        else {
            finalT.enabled = false;
            finalT = gameObject.AddComponent<T>();
            replace?.Invoke(1);
        }

        cur = gameObject.GetComponent<T>();
    }

    private void OnDestroy() {
        if (cur != defaultT) {
            if (finalT == cur) {
                finalT = defaultT;
                finalT.enabled = true;
                replace?.Invoke(2);
            }
        }
        else {
            // 不允许销毁default
            defaultT = finalT;
            replace?.Invoke(4);
        }
    }
}