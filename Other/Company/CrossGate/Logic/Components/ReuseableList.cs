using System;
using System.Collections.Generic;

public interface IReset {
    void Reset();
}

public class ReuseableList<T> where T : IReset, new() {
    private readonly List<T> vds = new List<T>();

    public int Count {
        get {
            return this.vds.Count;
        }
    }
    public int RealCount { get; private set; }
    public T this[int index] {
        get {
            if (0 <= index && index < this.RealCount) {
                return this.vds[index];
            }
            return default;
        }
    }

    public void ForEach(Action<T> action) { this.vds.ForEach(action); }

    public ReuseableList<T> TryBuild(int targetCount, Action<T, int/* index */> onInit = null) {
        while (targetCount > this.Count) {
            T vd = new T();
            vd.Reset();

            onInit?.Invoke(vd, this.Count);
            this.vds.Add(vd);
        }
        return this;
    }
    private ReuseableList<T> TryRefresh(int targetCount, Action<T, int/* index */> onRrefresh) {
        this.RealCount = targetCount;
        int goCount = this.Count;
        for (int i = 0; i < goCount; ++i) {
            T cur = this.vds[i];
            if (i < targetCount) {
                onRrefresh?.Invoke(cur, i);
            }
            else {
                cur.Reset();
            }
        }
        return this;
    }
    public ReuseableList<T> TryBuildOrRefresh(int targetCount, Action<T, int/* index */> onRrefresh, Action<T, int/* index */> onInit = null) {
        this.TryBuild(targetCount, onInit);
        return this.TryRefresh(targetCount, onRrefresh);
    }

    public void Clear() {
        for (int i = 0, count = this.vds.Count; i < count; ++i) {
            T cur = this.vds[i];
            cur.Reset();
        }
        this.vds.Clear();
        this.RealCount = 0;
    }
}
