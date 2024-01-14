using System;
using System.Collections.Generic;
using UnityEngine;

public class COWComponent<T> where T : Component {
    private readonly List<T> components = new List<T>();

    public int Count {
        get {
            return this.components.Count;
        }
    }
    public int RealCount { get; private set; }
    public T this[int index] {
        get {
            if (0 <= index && index < this.Count) {
                return this.components[index];
            }
            return null;
        }
    }

    public COWComponent<T> TryBuild(GameObject proto, Transform parent, int targetCount, Action<T, int/* index */> onInit = null) {
        proto.SetActive(false);
        while (targetCount > this.Count) {
            GameObject clone = GameObject.Instantiate<GameObject>(proto, parent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one;

            clone.SetActive(false);
            T t = clone.GetComponent<T>();
            onInit?.Invoke(t, this.Count);

            this.components.Add(t);                            
        }
        return this;
    }
    public COWComponent<T> TryRefresh(int targetCount, Action<T, int/* index */> onRrefresh) {
        this.RealCount = targetCount;
        int componentCount = this.Count;
        for (int i = 0; i < componentCount; ++i) {
            T cur = this.components[i];
            if (i < targetCount) {
                if (!cur.gameObject.activeSelf) {
                    cur.gameObject.SetActive(true);
                }
                onRrefresh?.Invoke(cur.GetComponent<T>(), i);
            }
            else {
                if (cur.gameObject.activeSelf) {
                    cur.gameObject.SetActive(false);
                }
            }
        }
        return this;
    }

    public COWComponent<T> TryBuildOrRefresh(GameObject proto, Transform parent, int targetCount, Action<T, int/* index */> onRrefresh, Action<T, int/* index */> onInit = null) {
        this.TryBuild(proto, parent, targetCount, onInit);
        return this.TryRefresh(targetCount, onRrefresh);
    }

    public void Clear() {
        for (int i = 0, count = this.components.Count; i < count; ++i) {
            T cur = this.components[i];
            GameObject.Destroy(cur);
        }

        this.components.Clear();
    }
}
