using System.Collections.Generic;
using Framework;
using Logic.Core;
using UnityEngine;

// 处理ui统一元素的一些增量复制的一些重复逻辑
// MoveTo到index特定位置需要等待index的vd加载完毕
public class UIElementContainer<T_Vd, T_Data> where T_Vd : UIElement, new()
{
    [Range(1, 10)]
    public int updateRate = 1;
    public bool needCollectId = true;

    private List<T_Vd> vds = new List<T_Vd>();
    private Dictionary<int, T_Vd> id2vd = new Dictionary<int, T_Vd>();

    private IList<T_Data> dataList = new List<T_Data>();
    private System.Action<T_Vd, T_Data, int> refreshAction;
    private GameObject proto;
    private Transform parent;

    public System.Action onFinish;

    private bool updateFlag = true;

    public UIElementContainer(bool needCollectId = true, int updateRate = 3)
    {
        this.updateRate = updateRate;
        this.needCollectId = needCollectId;
    }

    public int Count { get { return vds.Count; } }
    public T_Vd this[int index] { get { return vds[index]; } }
    public bool TryGetVdByIndex(int index, out T_Vd vd)
    {
        bool ret = false;
        vd = null;
        if (0 <= index && index < vds.Count) { ret = true; vd = vds[index]; }
        return ret;
    }
    public bool TryGetVdById(int id, out T_Vd vd)
    {
        vd = null;
        return (id2vd.TryGetValue(id, out vd)); 
    }
    public int GetIndexByVd(T_Vd vd) { return vds.IndexOf(vd); }
    public void ForEach(System.Action<T_Vd> action) { vds.ForEach(action); }

    public void BuildOrRefresh(GameObject proto, Transform parent, IList<T_Data> dataList, System.Action<T_Vd, T_Data, int> refreshAction, int initCount = 1)
    {
        if (this.proto != null && proto != this.proto) { return; }

        this.updateFlag = true;
        this.proto = proto;
        this.parent = parent;
        this.dataList = dataList;
        this.refreshAction = refreshAction;

        proto.SetActive(false);
        id2vd.Clear();

        int dataLength = dataList.Count;
        int vdLength = vds.Count;
        // 先创建initCount个,目的是为了MoveTo可以不收到延迟加载的印象，所以直接clone moveto个元素
        if (vdLength < initCount && dataList.Count >= initCount)
        {
            for (int i = vdLength; i < initCount; ++i)
            {
                T_Vd vd = BuildOne();
                vds.Add(vd);
            }
        }

        dataLength = dataList.Count;
        vdLength = vds.Count;
        for (int i = 0; i < vdLength; ++i)
        {
            if (i < dataLength)
            {
                vds[i].Show();
                refreshAction?.Invoke(vds[i], dataList[i], i);

                if (needCollectId)
                {
                    id2vd.Add(vds[i].id, vds[i]);
                }
            }
            else
            {
                vds[i].Hide();
            }
        }
    }

    public void Update()
    {
        //TODO : 根据新的帧率限制需要调整
#if USE_SPLIT_FRAME
        //if (TimeManager.updateCount % 3 == 0)
        if (updateFlag && dataList != null)
#else
        if (updateFlag && updateRate >= 1 && dataList != null && Time.frameCount % updateRate == 0)
#endif
        {
            int dataLength = dataList.Count;
            int vdLength = vds.Count;
            if (vdLength < dataLength)
            {
                T_Vd vd = BuildOne();
                T_Data data = dataList[vds.Count];
                vd.Show();
                refreshAction?.Invoke(vd, data, vds.Count);

                if (needCollectId)
                {
                    id2vd.Add(vd.id, vd);
                }
                vds.Add(vd);

                updateFlag = true;
            }
            else
            {
                onFinish?.Invoke();
                updateFlag = false;
            }
        }
    }
    private T_Vd BuildOne()
    {
        Transform t = GameObject.Instantiate<GameObject>(proto).transform;
        t.SetParent(parent);
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;

        T_Vd vd = new T_Vd();
        vd.Init(t);
        return vd;
    }

    public void Clear()
    {
        updateFlag = false;
        for (int i = 0, length = vds.Count; i < length; ++i) {
            vds[i]?.OnDestroy();
        }
        vds.Clear();
        id2vd.Clear();
        //dataList.Clear();

        refreshAction = null;
        proto = null;
        parent = null;
    }
}
