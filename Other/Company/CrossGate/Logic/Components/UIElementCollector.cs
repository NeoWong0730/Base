using System;
using System.Collections.Generic;
using Logic.Core;
using UnityEngine;

// 处理ui统一元素的一些增量复制的一些重复逻辑
public class UIElementCollector<T_Vd> where T_Vd : UIElement, new() {
    public bool needCollectId = true;
    private readonly List<T_Vd> vds = new List<T_Vd>();
    private readonly Dictionary<int, T_Vd> id2vd = new Dictionary<int, T_Vd>();

    public int Count {
        get { return this.vds.Count; }
    }

    public int RealCount { get; private set; }

    public UIElementCollector(bool needCollectId = true) {
        this.needCollectId = needCollectId;
    }

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

    public bool TryGetVdById(int id, out T_Vd vd) {
        vd = null;
        return (this.id2vd.TryGetValue(id, out vd));
    }

    public int GetIndexByVd(T_Vd vd) {
        // 方案1：可以节省vd2Index的内存空间
        return this.vds.IndexOf(vd);
    }

    public void ForEach(System.Action<T_Vd> action) {
        if (action != null) {
            for (int i = 0, length = RealCount; i < length; ++i) {
                action.Invoke(vds[i]);
            }
        }
    }

    public void BuildOrRefresh<T_Data>(GameObject proto, Transform parent, List<T_Data> dataList, System.Action<T_Vd, T_Data, int> refreshAction) {
        proto.SetActive(false);
        int dataLength = dataList.Count;
        int vdLength = this.vds.Count;
        if (dataLength > vdLength) {
            for (int i = vdLength; i < dataLength; ++i) {
                Transform t = GameObject.Instantiate<GameObject>(proto).transform;
                t.SetParent(parent);
                t.localPosition = Vector3.zero;
                t.localScale = Vector3.one;

                T_Vd vd = new T_Vd();
                vd.Init(t.gameObject.transform);

                this.vds.Add(vd);
            }
        }

        RealCount = dataLength;

        this.id2vd.Clear();
        vdLength = this.vds.Count;
        for (int i = 0; i < vdLength; ++i) {
            if (i < dataLength) {
                T_Data data = dataList[i];
                this.vds[i].Show();
                refreshAction?.Invoke(this.vds[i], data, i);

                // after refresh, because function refresh set id
                // and id must be unique!
                if (this.needCollectId && !this.id2vd.ContainsKey(this.vds[i].id))
                    this.id2vd.Add(this.vds[i].id, this.vds[i]);
            }
            else {
                this.vds[i].Hide();
            }
        }
    }
    
    public void TryBuild(GameObject proto, Transform parent, int targetCount, Action<T_Vd, int/* index */> onInit = null) {
        proto.SetActive(false);
        while (targetCount > this.Count) {
            //TODO:优化 Transform的获取次数 用老师变量先缓存
            GameObject clone = GameObject.Instantiate<GameObject>(proto, parent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one;
            clone.SetActive(false);

            T_Vd vd = new T_Vd();
            vd.Init(clone.transform);
            onInit?.Invoke(vd, this.Count);
            this.vds.Add(vd);
        }
    }
    
    public void TryAdd(GameObject clone, Action<T_Vd, int/* index */> onInit = null) {
        T_Vd vd = new T_Vd();
        vd.Init(clone.transform);
        onInit?.Invoke(vd, this.Count);
        
        this.vds.Add(vd);
    }
        
    public void TryRefresh(int targetCount, Action<T_Vd, int/* index */> onRrefresh) {
        this.RealCount = targetCount;
        this.id2vd.Clear();
        int goCount = this.Count;
        for (int i = 0; i < goCount; ++i) {
            T_Vd cur = this.vds[i];
            if (i < targetCount) {
                if (!cur.gameObject.activeSelf) {
                    cur.gameObject.SetActive(true);
                }
                onRrefresh?.Invoke(cur, i);
                
                // after refresh, because function refresh set id
                // and id must be unique!
                if (this.needCollectId && !this.id2vd.ContainsKey(this.vds[i].id))
                    this.id2vd.Add(this.vds[i].id, this.vds[i]);
            }
            else {
                if (cur.gameObject.activeSelf) {
                    cur.gameObject.SetActive(false);
                }
            }
        }
    }
        
    public void TryBuildOrRefresh(GameObject proto, Transform parent, int targetCount, Action<T_Vd, int/* index */> onRrefresh, Action<T_Vd, int/* index */> onInit = null) {
        this.TryBuild(proto, parent, targetCount, onInit);
        this.TryRefresh(targetCount, onRrefresh);
    }
    
    public void TryBuildOrRefresh<T_Data>(GameObject proto, Transform parent, List<T_Data> dataList, Action<T_Vd, int/* index */> onRrefresh, Action<T_Vd, int/* index */> onInit = null) {
        this.TryBuild(proto, parent, dataList.Count, onInit);
        this.TryRefresh(dataList.Count, onRrefresh);
    }

    public bool CorrectId(ref int id, IList<uint> ls) {
        if (ls == null || ls.Count <= 0) {
            id = -1;
            return false;
        }
        else {
            if (id <= 0 || !ls.Contains((uint) id)) {
                id = (int) ls[0];
            }

            return true;
        }
    }

    public bool CorrectId(ref uint id, IList<uint> ls) {
        if (ls == null || ls.Count <= 0) {
            id = 0;
            return false;
        }
        else {
            if (id <= 0 || !ls.Contains(id)) {
                id = ls[0];
            }

            return true;
        }
    }

    public void Clear() {
        for (int i = 0, length = this.vds.Count; i < length; ++i) {
            this.vds[i]?.OnDestroy();
        }

        this.vds.Clear();
        this.id2vd.Clear();
        RealCount = 0;
    }
}