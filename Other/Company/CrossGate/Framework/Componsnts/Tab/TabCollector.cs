using System.Collections.Generic;
using UnityEngine;

public class TabCollector : MonoBehaviour {
    public GameObject tabProto;
    public Transform tabProtoParent;

    public List<Tab> tabs { get; private set; } = new List<Tab>();
    public int tabCount { get { return this.tabs.Count; } }

    public IList<uint> idList { get; private set; } = new List<uint>();
    public int idCount { get { return this.idList.Count; } }

    public uint currentId { get; private set; } = 0;
    public int currentIndex { get { return this.idList.IndexOf(this.currentId); } }

    public TabCollector SetIdList(IList<uint> targetIdList) {
        this.idList.Clear();
        if (targetIdList != null) {
            for (int i = 0, count = targetIdList.Count; i < count; ++i) {
                this.idList.Add(targetIdList[i]);
            }
        }
        return this;
    }
    public TabCollector TryRefresh<T>() where T : Tab {
        this.tabProto.SetActive(false);
        while (this.idCount > this.tabCount) {
            GameObject clone = GameObject.Instantiate(this.tabProto, this.tabProtoParent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one;

            // tabProto必须挂载T组件
            T cp = clone.GetComponent<T>();
            this.tabs.Add(cp);
        }

        for (int i = 0, count = this.tabCount; i < count; ++i) {
            if (i < this.idCount) {
                this.tabs[i].gameObject.SetActive(true);
                this.tabs[i].id = this.idList[i];
                this.tabs[i].Refresh();
            }
            else {
                this.tabs[i].gameObject.SetActive(false);
            }
        }

        return this;
    }

    protected bool TrySetSelected(uint id) {
        bool same = id == this.currentId && this.idList.IndexOf(id) == this.currentIndex;
        this.currentId = id;
        if (this.currentIndex != -1) {
            for (int i = 0, length = this.idCount; i < length; ++i) {
                this.tabs[i].SetSelected(this.tabs[i].id == id);
            }
            return true;
        }
        else {
            this.currentId = 0;
            return false;
        }
    }
    public bool TrySetSelected() {
        return this.TrySetSelected(this.currentId);
    }
    public virtual bool SetSelected(uint id) {
        return this.TrySetSelected(id);
    }
}