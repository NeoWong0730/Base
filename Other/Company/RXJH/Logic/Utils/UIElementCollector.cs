using System;
using System.Collections.Generic;
using Logic.Core;
using UnityEngine;

// 处理ui统一元素的一些增量复制的一些重复逻辑
public class UIElementCollector<T_Vd> where T_Vd : UIComponent, new() {
    private readonly List<T_Vd> vds = new List<T_Vd>();

    public int Count {
        get { return this.vds.Count; }
    }

    public int RealCount { get; private set; }

    public T_Vd this[int index] {
        get { return this.vds[index]; }
    }

    public bool TryGetVdByIndex(int index, out T_Vd vd) {
        bool ret = false;
        vd = null;
        if (0 <= index && index < this.vds.Count) {
            ret = true;
            vd = this.vds[index];
        }

        return ret;
    }

    public void ForEach(System.Action<T_Vd> action) {
        if (action != null) {
            for (int i = 0, length = RealCount; i < length; ++i) {
                action.Invoke(vds[i]);
            }
        }
    }

    public void TryBuild(GameObject proto, Transform parent, int targetCount, Action<T_Vd, int /* index */> onInit = null) {
        proto.SetActive(false);
        while (targetCount > this.Count) {
            //TODO:优化 Transform的获取次数 用老师变量先缓存
            GameObject clone = GameObject.Instantiate<GameObject>(proto, parent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one;
            clone.SetActive(false);

            TryAdd(clone);
        }
    }

    public void TryAdd(GameObject clone, Action<T_Vd, int /* index */> onInit = null) {
        T_Vd vd = new T_Vd();
        vd.Init(clone.transform);
        onInit?.Invoke(vd, this.Count);

        this.vds.Add(vd);
    }

    public void TryRefresh(int targetCount, Action<T_Vd, int /* index */> onRrefresh) {
        this.RealCount = targetCount;
        int goCount = this.Count;
        for (int i = 0; i < goCount; ++i) {
            T_Vd cur = this.vds[i];
            if (i < targetCount) {
                if (!cur.transform.gameObject.activeSelf) {
                    cur.transform.gameObject.SetActive(true);
                }

                onRrefresh?.Invoke(cur, i);
            }
            else {
                if (cur.transform.gameObject.activeSelf) {
                    cur.transform.gameObject.SetActive(false);
                }
            }
        }
    }

    public void TryBuildOrRefresh(GameObject proto, Transform parent, int targetCount, Action<T_Vd, int /* index */> onRrefresh, Action<T_Vd, int /* index */> onInit = null) {
        this.TryBuild(proto, parent, targetCount, onInit);
        this.TryRefresh(targetCount, onRrefresh);
    }

    public void TryBuildOrRefresh<T_Data>(GameObject proto, Transform parent, IList<T_Data> dataList, Action<T_Vd, int /* index */> onRrefresh, Action<T_Vd, int /* index */> onInit = null) {
        this.TryBuild(proto, parent, dataList.Count, onInit);
        this.TryRefresh(dataList.Count, onRrefresh);
    }

    public void Clear() {
        for (int i = 0, length = this.vds.Count; i < length; ++i) {
            this.vds[i]?.OnDestroy();
        }

        this.vds.Clear();
        RealCount = 0;
    }
}