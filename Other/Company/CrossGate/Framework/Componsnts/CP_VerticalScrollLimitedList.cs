using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CP_VerticalScrollLimitedList : MonoBehaviour
{
    public Transform circle;
    public List<Transform> nodes = new List<Transform>();
    private List<uint> ids = new List<uint>();
    public float ySize = 100f;
    [Range(0f, 6f)]
    public float checkOffsetAngle = 6f;
    public GameObject proto;
    public CP_ScrollRect scrollRect;
    [Range(0f, 359f)]
    private float refAngle = 120f;

    public List<Indexer> deQueue = new List<Indexer>();
    private Dictionary<int, NodeTuple> nodeTuples = new Dictionary<int, NodeTuple>();
    private List<CheckNode> checkNodes = new List<CheckNode>();

    public CP_ScrolCircleListItem inCircleItem;

    private Vector3 firstPosition;
    private float scrolRate;
    private Vector3 lastPosition = Vector3.zero;
    private RectTransform rectTransform;

    public bool isMoving
    {
        get { return scrollRect.velocity.y != 0f; }
    }

    private void Awake()
    {
        scrolRate = ySize / refAngle;
        rectTransform = transform as RectTransform;
        lastPosition = rectTransform.anchoredPosition3D;
        firstPosition = (proto.transform as RectTransform).anchoredPosition3D;
        proto.transform.SetParent(proto.transform.parent.parent, false);

        // 这里根据refAngle，去适配角度
        nodeTuples.Clear();
        nodeTuples.Add(2, new NodeTuple(2, nodes[1], new List<float>() { -refAngle }, checkOffsetAngle));
        nodeTuples.Add(3, new NodeTuple(3, nodes[2], new List<float>() { -refAngle * 2 }, checkOffsetAngle));
        nodeTuples.Add(1, new NodeTuple(1, nodes[0], new List<float>() { 0f, -refAngle * 3 }, checkOffsetAngle));
    }
    private void OnChange(CP_ScrolCircleListItem item, bool oldState, bool newState)
    {
        if (newState)
        {
            inCircleItem = item;
        }
    }
    public void SetData(List<uint> ids, System.Action<uint, CP_ScrolCircleListItem> onClicked, System.Action<uint, GameObject> onRefresh, System.Action<CP_ScrolCircleListItem, bool, bool> onCircleStateChanged)
    {
        this.ids = ids;
        deQueue.Clear();
        proto.gameObject.SetActive(false);
        for (int i = 0; i < ids.Count; ++i)
        {
            GameObject go = GameObject.Instantiate<GameObject>(proto);
            go.SetActive(true);
            CP_ScrolCircleListItem cp = go.GetComponent<CP_ScrolCircleListItem>();
            string id = ids[i].ToString();
            cp.SetParent(transform);

            // 复用的时候注意这里!!
            cp.onChange += OnChange;
#if UNITY_EDITOR
            cp.name = id;
#endif
            cp.id = ids[i];
            cp.index = i;
            cp.inCircle = false;
            cp.onCircleStateChanged += onCircleStateChanged;
            cp.SetAnchor(false);

            int k = i;
            go.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                onClicked?.Invoke(ids[k], cp);
            });
            onRefresh?.Invoke(ids[k], go);

            deQueue.Add(new Indexer(ids[i], cp));
        }

        ResetPosition();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, ySize * deQueue.Count);
        scrollRect.verticalNormalizedPosition = 1f;
    }
    // 可以优化为可重用的，这里犯懒就不这么做了！
    public void Clear()
    {
        foreach (var i in deQueue)
        {
            GameObject.Destroy(i.cp.gameObject);
        }
        deQueue.Clear();
        ids.Clear();
    }
    public void ResetPosition()
    {
        for (int i = 0; i < deQueue.Count; ++i)
        {
            Vector3 pos = new Vector3(firstPosition.x, firstPosition.y - i * ySize, firstPosition.z);
            deQueue[i].cp.rectTransform.anchoredPosition3D = pos;
            deQueue[i].cp.originalPosition = pos;
        }
    }
    public float verticalNormalizedPosition;
    private void Update()
    {
        verticalNormalizedPosition = scrollRect.verticalNormalizedPosition;
        float y = rectTransform.anchoredPosition3D.y;
        if (y != lastPosition.y)
        {
            float deltaY = lastPosition.y - y;
            OnScrollValueChanged(deltaY, deltaY > 0);
            lastPosition = rectTransform.anchoredPosition3D;
        }
    }
    // 总angle,n个循环,顺/逆时针
    private void OnScrollValueChanged(float totalDeltaY, bool clickwise)
    {
        float toAddAngle = totalDeltaY / scrolRate;
        toAddAngle *= -1f;
        // 顺时针：角度为负数，deltaY为正
        TryMatch(toAddAngle, clickwise);
        circle.localEulerAngles += new Vector3(0, 0, toAddAngle);
    }

    private bool lastRotateDir = false;
    private void TryMatch(float toAddAngle, bool clickwise)
    {
        float currentCircleAngle = GetCircleCurrentAngle();
        PopulatePassedCheckCount(currentCircleAngle, toAddAngle, clickwise);
        float finalAngle = GetNegetiveAngle(currentCircleAngle + toAddAngle);
        int checkNodeIndex = -1;
        bool inClosestCheckArea = false;
        if (checkNodes.Count >= 1)
        {
            for (int i = 0; i < checkNodes.Count; ++i)
            {
                checkNodeIndex = GetCurrentCheckIndex(checkNodes[i].angle);
                if (lastRotateDir != clickwise)
                {
                    inClosestCheckArea = false;
                }
                else
                {
                    inClosestCheckArea = checkNodeIndex == lastCheckNodeIndex;
                }

                if (!inClosestCheckArea)
                {
                    if (clickwise)
                    {
                        Indexer indexer0 = deQueue[0];
                        Indexer indexerLast = deQueue[deQueue.Count - 1];
                        if (indexer0.cp.index == 0 && !indexer0.cp.inCircle)
                        {
                            // nothing to do
                        }
                        else if (indexerLast.cp.index == 0 && indexerLast.cp.inCircle)
                        {
                            deQueue.RemoveAt(deQueue.Count - 1);
                            deQueue.Insert(0, indexerLast);
                            indexerLast.cp.OnCircleStateChanged(true, false);
                            indexerLast.cp.SetAnchor(false);
                            indexerLast.cp.SetParent(transform);
                            indexerLast.cp.SetOriginalPosition();
                            indexerLast.cp.SetSiblingIndex();
                        }
                        else
                        {
                            Add_Head(checkNodeIndex);
                            lastCheckNodeIndex = checkNodeIndex;
                        }
                    }
                    else
                    {
                        if (scrollRect.verticalNormalizedPosition <= 1f)
                        {
                            Remove_Head(checkNodeIndex);
                            lastCheckNodeIndex = checkNodeIndex;
                        }
                    }
                }
            }
        }
        else
        {
            lastCheckNodeIndex = -1;
        }

        lastRotateDir = clickwise;
    }
    private float GetCircleCurrentAngle()
    {
        float angle = circle.transform.localEulerAngles.z;
        return GetNegetiveAngle(angle);
    }
    public float GetNegetiveAngle(float angle)
    {
        angle %= 360f;
        angle -= 360f;
        angle %= 360f;
        return angle;
    }
    public float GetPositiveAngle(float angle)
    {
        angle %= 360f;
        angle += 360f;
        angle %= 360f;
        return angle;
    }
    private int lastCheckNodeIndex = -99;
    private int GetCurrentCheckIndex(float currentCircleAngle)
    {
        int index = -1;
        foreach (var kvp in nodeTuples)
        {
            foreach (var offset in kvp.Value.offsetAngles)
            {
                if (offset.x <= currentCircleAngle && currentCircleAngle <= offset.y)
                {
                    index = kvp.Key;
                    return index;
                }
            }
        }
        return index;
    }
    private void PopulatePassedCheckCount(float startAngle, float toAddAngle, bool clickwise)
    {
        checkNodes.Clear();
        int lastNodeIndex = -1;
        float finalAngle = startAngle + toAddAngle;
        if (clickwise)
        {
            for (float ag = startAngle; ag >= finalAngle;)
            {
                float t = GetNegetiveAngle(ag);
                lastNodeIndex = GetCurrentCheckIndex(t);
                if (lastNodeIndex != -1)
                {
                    checkNodes.Add(new CheckNode(t));
                    ag -= refAngle;
                }
                else
                {
                    ag -= checkOffsetAngle * 2;
                }
            }
        }
        else
        {
            for (float ag = startAngle; ag <= finalAngle;)
            {
                float t = GetNegetiveAngle(ag);
                lastNodeIndex = GetCurrentCheckIndex(t);
                if (lastNodeIndex != -1)
                {
                    checkNodes.Add(new CheckNode(t));
                    ag += refAngle;
                }
                else
                {
                    ag += checkOffsetAngle * 2;
                }
            }
        }

        // 处理finalAngle
        float tt = GetNegetiveAngle(finalAngle);
        int nodeIndex = GetCurrentCheckIndex(tt);
        if (nodeIndex != -1 && nodeIndex != lastNodeIndex)
        {
            checkNodes.Add(new CheckNode(tt));
        }
    }
    private int GetRightNodeIndex(int currentIndex)
    {
        if (currentIndex == 1) { return 2; }
        if (currentIndex == 2) { return 3; }
        else { return 1; }
    }
    private int GetLeftNodeIndex(int currentIndex)
    {
        if (currentIndex == 1) { return 3; }
        if (currentIndex == 2) { return 1; }
        else { return 2; }
    }
    // 顺时针
    private void Add_Head(int checkNodeIndex)
    {
        // 数据层面
        int index = deQueue.Count - 1;
        Indexer indexer = deQueue[index];
        deQueue.RemoveAt(index);
        deQueue.Insert(0, indexer);

        // 表现层面
        // head
        indexer.cp.SetParent(transform);
        indexer.cp.OnCircleStateChanged(true, false);
        indexer.cp.SetAnchor(false);
        indexer.cp.SetOriginalPosition();
        indexer.cp.SetSiblingIndex();
        indexer.cp.SetScale(1f);

        // tail
        int rightNodeIndex = GetRightNodeIndex(checkNodeIndex);
        indexer = deQueue[index];
        indexer.cp.SetParent(nodes[rightNodeIndex - 1]);
        indexer.cp.OnCircleStateChanged(false, true);
        indexer.cp.SetAnchor(true);
    }
    // 逆时针
    private void Remove_Head(int checkNodeIndex)
    {
        // 数据层面
        int index = 0;
        Indexer indexer = deQueue[index];
        deQueue.RemoveAt(index);
        deQueue.Add(indexer);

        // 表现层面
        // head
        indexer.cp.SetParent(nodes[checkNodeIndex - 1]);
        indexer.cp.OnCircleStateChanged(false, true);
        indexer.cp.SetAnchor(true);

        // tail
        indexer = deQueue[deQueue.Count - 2];
        indexer.cp.SetParent(transform);
        indexer.cp.OnCircleStateChanged(true, false);
        indexer.cp.SetAnchor(false);
        indexer.cp.SetOriginalPosition();
        indexer.cp.SetSiblingIndex();
        indexer.cp.SetScale(1f);
    }
}
