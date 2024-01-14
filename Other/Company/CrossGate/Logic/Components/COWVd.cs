using System;
using System.Collections.Generic;
using Logic.Core;
using UnityEngine;

public class COW<T> where T : new() {
    private readonly List<T> ls = new List<T>();
    public int Count {
        get {
            return this.ls.Count;
        }
    }
    public int RealCount { get; private set; }
    public T this[int index] {
        get {
            return this.ls[index];
        }
    }

    public void ForEach(Action<T> action) { this.ls.ForEach(action); }
    
    public COW<T> TryCopy<TE>(int targetCount, IList<TE> list, Func<int /*index*/, T> onCreate, Action<int /*index*/, TE, T> onRefresh) {
        RealCount = targetCount;
        while (targetCount > this.Count) {
            int index = this.Count;
            var r = onCreate.Invoke(index);
            this.ls.Add(r);
        }
        
        int goCount = this.Count;
        for (int i = 0; i < goCount; ++i) {
            T cur = this.ls[i];
            if (i < targetCount) {
                onRefresh?.Invoke(i, list[i], cur);
            }
        }
        return this;
    }
    
    public void Clear() {
        this.ls.Clear();
        RealCount = 0;
    }
}

public class COWVd<T_Vd> where T_Vd : UIComponent, new() {
    private readonly List<T_Vd> vds = new List<T_Vd>();

    public int Count {
        get {
            return this.vds.Count;
        }
    }
    public int RealCount { get; private set; }
    public T_Vd this[int index] {
        get {
            return this.vds[index];
        }
    }

    public void ForEach(Action<T_Vd> action) { this.vds.ForEach(action); }

    public COWVd<T_Vd> TryBuild(GameObject proto, Transform parent, int targetCount, Action<T_Vd, int/* index */> onInit = null) {
        proto.SetActive(false);
        while (targetCount > this.Count) {
            //TODO:优化 Transform的获取次数 用老师变量先缓存
            GameObject clone = GameObject.Instantiate<GameObject>(proto, parent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one;
            clone.SetActive(false);

            TryAdd(clone, onInit);
        }
        return this;
    }
    
    public T_Vd TryAdd(GameObject clone, Action<T_Vd, int/* index */> onInit = null) {
        T_Vd vd = new T_Vd();
        vd.Init(clone.transform);
        onInit?.Invoke(vd, this.Count);
        
        this.vds.Add(vd);
        return vd;
    }

    public COWVd<T_Vd> TryRefresh(int targetCount, Action<T_Vd, int/* index */> onRrefresh) {
        this.RealCount = targetCount;
        int goCount = this.Count;
        for (int i = 0; i < goCount; ++i) {
            T_Vd cur = this.vds[i];
            if (i < targetCount) {
                if (!cur.gameObject.activeSelf) {
                    cur.gameObject.SetActive(true);
                }
                onRrefresh?.Invoke(cur, i);
            }
            else {
                if (cur.gameObject.activeSelf) {
                    cur.gameObject.SetActive(false);
                }
            }
        }
        return this;
    }
    public COWVd<T_Vd> TryBuildOrRefresh(GameObject proto, Transform parent, int targetCount, Action<T_Vd, int/* index */> onRrefresh, Action<T_Vd, int/* index */> onInit = null) {
        this.TryBuild(proto, parent, targetCount, onInit);
        return this.TryRefresh(targetCount, onRrefresh);
    }

    public void Clear() {
        for (int i = 0, count = this.vds.Count; i < count; ++i) {
            T_Vd cur = this.vds[i];
            cur.OnDestroy();
        }
        this.vds.Clear();
        RealCount = 0;
    }
}
