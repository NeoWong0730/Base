using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Indexer
{
    public uint id;
    public CP_ScrolCircleListItem cp;

    public Indexer(uint id, CP_ScrolCircleListItem cp)
    {
        this.id = id;
        this.cp = cp;
    }
}

[System.Serializable]
public struct CheckNode
{
    public float angle;

    public CheckNode(float angle)
    {
        this.angle = angle;
    }
}

[System.Serializable]
public struct NodeTuple
{
    public int index;
    public Transform node;
    public List<Vector2> offsetAngles;

    public NodeTuple(int index, Transform node, List<float> baseAngles, float offsetAngle)
    {
        this.index = index;
        this.node = node;

        this.offsetAngles = new List<Vector2>();
        for (int i = 0; i < baseAngles.Count; ++i)
        {
            float x = baseAngles[i] - offsetAngle;
            float y = baseAngles[i] + offsetAngle;
            this.offsetAngles.Add(new Vector2(x, y));
        }
    }
}

public class CP_ScrolCircleList : MonoBehaviour
{
    public Transform circle;
    public float ySize = 100f;
    public GameObject proto;
    [Range(0f, 15f)]
    public float checkOffsetAngle = 3f;

    public CP_ScrollRect scrollRect;
    public CP_VerticalCenterOnChild centerOnChild;

    public List<Transform> nodes = new List<Transform>();
    public List<Indexer> deQueue = new List<Indexer>();
    private Dictionary<int, NodeTuple> nodeTuples = new Dictionary<int, NodeTuple>();
    private List<CheckNode> checkNodes = new List<CheckNode>();
    private List<uint> ids = new List<uint>();

    public CP_ScrolCircleListItem inCircleItem;

    [Header("调试 观察")]
    private Vector3 firstPosition;
    private float scrolRate;
    private Vector3 lastPosition = Vector3.zero;

    public RectTransform rectTransform;

    public Vector3 centerOnAnchorPosition;
    public Vector3 upPosition;
    public Vector3 downPosition;

    public float GetToCenterOnDiffY(CP_ScrolCircleListItem cp)
    {
        return centerOnAnchorPosition.y - cp.rectTransform.anchoredPosition3D.y;
    }
    public float GetToCenterOnDiffY()
    {
        return centerOnAnchorPosition.y - upPosition.y;
    }
    public CP_ScrolCircleListItem GetFirst()
    {
        return deQueue[0].cp;
    }

    private void Awake()
    {
        scrolRate = ySize / 120f;
        rectTransform = transform as RectTransform;
        this.lastPosition = rectTransform.anchoredPosition3D;
        this.firstPosition = (proto.transform as RectTransform).anchoredPosition3D;
        proto.transform.SetParent(proto.transform.parent.parent, false);

        nodeTuples.Clear();
        nodeTuples.Add(2, new NodeTuple(2, nodes[1], new List<float>() { -120f }, checkOffsetAngle));
        nodeTuples.Add(3, new NodeTuple(3, nodes[2], new List<float>() { -240f }, checkOffsetAngle));
        nodeTuples.Add(1, new NodeTuple(1, nodes[0], new List<float>() { 0f, -360f }, checkOffsetAngle));
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
        // 最后一个元素在circle上面
        deQueue.Clear();
        proto.gameObject.SetActive(false);
        for (int i = 0; i < ids.Count; ++i)
        {
            Transform parent = i == ids.Count - 1 ? nodes[1] : transform;
            bool inCircle = (i == ids.Count - 1 );
            GameObject go = GameObject.Instantiate<GameObject>(proto);
            go.SetActive(true);
            CP_ScrolCircleListItem cp = go.GetComponent<CP_ScrolCircleListItem>();
            string id = ids[i].ToString();
            cp.SetParent(parent);

            // 复用的时候注意这里!!
            cp.onChange += OnChange;
#if UNITY_EDITOR
            cp.name = id;
#endif
            cp.id = ids[i];
            cp.inCircle = inCircle;
            cp.onCircleStateChanged += onCircleStateChanged;
            cp.SetAnchor(true);

            int k = i;
            go.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                onClicked?.Invoke(ids[k], cp);
            });
            onRefresh?.Invoke(ids[k], go);

            deQueue.Add(new Indexer(ids[i], cp));
        }

        ResetPosition();
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
            deQueue[i].cp.rectTransform .anchoredPosition3D = pos;
            if (i == 0)
            {
                centerOnAnchorPosition = upPosition = pos;
            }
            if (i == deQueue.Count - 2)
            {
                downPosition = pos;
            }
        }
    }
    private void Update()
    {
        float y = rectTransform.anchoredPosition3D.y;
        if (y != lastPosition.y)
        {
            float deltaY = lastPosition.y - y;

            centerOnAnchorPosition.y += deltaY;

            OnScrollValueChanged(deltaY, deltaY > 0);
            lastPosition = rectTransform.anchoredPosition3D;
        }
    }
    // 余angle,n个循环,顺/逆时针
    private void OnScrollValueChanged(float totalDeltaY, bool clickwise)
    {
        float toAddAngle = totalDeltaY / scrolRate;
        toAddAngle *= -1f;
        // 顺时针：角度为负数，deltaY为正
        TryMatch(toAddAngle, clickwise);
        circle.localEulerAngles += new Vector3(0, 0, toAddAngle);
    }

    private bool firstRotate = true;
    private bool lastRotateDir = false;
    private void TryMatch(float toAddAngle, bool clickwise)
    {
        float currentCircleAngle = GetCircleCurrentAngle();
        PopulatePassedCheckCount(currentCircleAngle, toAddAngle, clickwise);
        int currentNodeIndex = GetCurrentCheckIndex(currentCircleAngle);
        bool inClosestCheckArea = false;
        if (firstRotate)
        {
            lastRotateDir = clickwise;
            inClosestCheckArea = clickwise ? true : false;
        }
        else
        {
            if (lastRotateDir != clickwise)
            {
                lastRotateDir = clickwise;
                // set < -1 dont == -1
                lastCheckNodeIndex = -999;
                inClosestCheckArea = false;
            }
            else
            {
                inClosestCheckArea = currentNodeIndex == lastCheckNodeIndex;
            }
        }

        if (checkNodes.Count >= 1)
        {
            for (int i = 0; i < checkNodes.Count; ++i)
            {
                int checkNodeIndex = GetCurrentCheckIndex(checkNodes[i].angle);
                if (clickwise)
                {
                    if (!firstRotate && !inClosestCheckArea)
                    {
                        Add_Head(checkNodeIndex);
                    }
                }
                else
                {
                    if (!inClosestCheckArea)
                    {
                        Remove_Head(checkNodeIndex);
                    }
                }

                firstRotate = false;
                lastCheckNodeIndex = checkNodeIndex;
                inClosestCheckArea = false;
            }
        }
        else
        {
            firstRotate = false;
        }
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
    // 原始为1
    private int lastCheckNodeIndex = 1;
    private int GetCurrentCheckIndex(float currentCircleAngle)
    {
        int index = -1;
        foreach (var kvp in nodeTuples)
        {
            foreach (var offset in kvp.Value.offsetAngles)
            {
                if (offset.x <= currentCircleAngle && currentCircleAngle < offset.y)
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
                    ag -= 120f;
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
                    ag += 120f;
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
        indexer.cp.transform.SetAsFirstSibling();
        indexer.cp.OnCircleStateChanged(true, false);

        Vector3 offset = new Vector3(0f, ySize, 0f);
        upPosition += offset;
        downPosition += offset;
        indexer.cp.SetPosition(upPosition);
        indexer.cp.SetScale(1f);

        // tail
        int rightNodeIndex = GetRightNodeIndex(checkNodeIndex);
        indexer = deQueue[index];
        indexer.cp.SetParent(nodes[rightNodeIndex - 1]);
        indexer.cp.OnCircleStateChanged(false, true);
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

        // tail
        indexer = deQueue[deQueue.Count - 2];
        indexer.cp.SetParent(transform);
        indexer.cp.transform.SetAsLastSibling();
        indexer.cp.OnCircleStateChanged(true, false);

        Vector3 offset = new Vector3(0f, ySize, 0f);
        upPosition -= offset;
        downPosition -= offset;
        indexer.cp.SetPosition(downPosition);
        indexer.cp.SetScale(1f);
    }
}
