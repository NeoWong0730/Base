using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Lib.Core;

[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(ContentSizeFitter))]
public class InfinityGridLayoutGroup : MonoBehaviour
{
    /// <summary> 更新Item事件 </summary>
    public delegate void UpdateChildrenCallbackDelegate(int index, Transform trans);
    public UpdateChildrenCallbackDelegate updateChildrenCallback = null;

    private RectTransform rectTransform;
    private GridLayoutGroup gridLayoutGroup;
    private ContentSizeFitter contentSizeFitter;
    private ScrollRect scrollRect;

    [SerializeField]
    public int minAmount = 0;//实现无限滚动，需要的最少的child数量。屏幕上能看到的+一行看不到的，比如我在屏幕上能看到 2 行，每一行 2 个。则这个值为 2行*2个 + 1 行* 2个 = 6个。
    int amount = 0;
    int realIndex = -1;
    Vector2 startPosition;
    bool hasInit = false;
    Vector2 gridLayoutSize;
    Vector2 gridLayoutPos;
    List<RectTransform> children = new List<RectTransform>();
    Dictionary<Transform, Vector2> childsAnchoredPosition = new Dictionary<Transform, Vector2>();
    Dictionary<Transform, int> childsSiblingIndex = new Dictionary<Transform, int>();

    private Coroutine coroutine = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        contentSizeFitter = GetComponent<ContentSizeFitter>();
        //注册ScrollRect滚动回调;
        scrollRect = transform.GetComponentInParent<ScrollRect>();
        //scrollRect = transform.parent.GetComponent<ScrollRect>();
        //if (scrollRect == null)
        //    scrollRect = transform.parent.transform.parent.GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener((data) => { ScrollCallback(data); });
    }

    private void OnDisable()
    {
        _moveTimer?.Cancel();
        _moveTimer = null;

        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    void InitChildren()
    {
        if (!hasInit)
        {
            gridLayoutGroup.enabled = true;
            contentSizeFitter.enabled = true;

            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            gridLayoutGroup.enabled = false;
            contentSizeFitter.enabled = false;

            gridLayoutPos = rectTransform.anchoredPosition;
            gridLayoutSize = rectTransform.sizeDelta;

            //获取所有child anchoredPosition 以及 SiblingIndex;
            for (int index = 0; index < transform.childCount; index++)
            {
                Transform child = transform.GetChild(index);
                RectTransform childRectTrans = child as RectTransform;

                childsAnchoredPosition.Add(child, childRectTrans.anchoredPosition);
                childsSiblingIndex.Add(child, child.GetSiblingIndex());
            }
        }
        else
        {
            rectTransform.anchoredPosition = gridLayoutPos;
            rectTransform.sizeDelta = gridLayoutSize;
            children.Clear();
            realIndex = -1;

            //children重新设置上下顺序;
            foreach (var info in childsSiblingIndex)
            {
                info.Key.SetSiblingIndex(info.Value);
            }

            //children重新设置anchoredPosition;
            for (int index = 0; index < transform.childCount; index++)
            {
                Transform child = transform.GetChild(index);
                RectTransform childRectTrans = child as RectTransform;
                if (childsAnchoredPosition.ContainsKey(child))
                {
                    childRectTrans.anchoredPosition = childsAnchoredPosition[child];
                }
                else
                {
                    DebugUtil.LogError("childsAnchoredPosition no contain " + child.name);
                }
            }
        }

        //获取所有child;
        for (int index = 0; index < transform.childCount; index++)
        {
            Transform child = transform.GetChild(index);
            RectTransform childRectTrans = child as RectTransform;
            children.Add(childRectTrans);
            if (index < amount)
            {
                child.gameObject.SetActive(true);
                //初始化前面几个;
                UpdateChildrenCallback(index, child);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        startPosition = rectTransform.anchoredPosition;

        realIndex = children.Count - 1;

        hasInit = true;

        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            //如果小了一行，则需要把GridLayout的高度减去一行的高度;
            int row = (minAmount - amount) / gridLayoutGroup.constraintCount;
            if (row > 0)
            {
                rectTransform.sizeDelta -= new Vector2(0, (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y) * row);
            }
        }
        else
        {
            //如果小了一列，则需要把GridLayout的宽度减去一列的宽度;
            int column = (minAmount - amount) / gridLayoutGroup.constraintCount;
            if (column > 0)
            {
                rectTransform.sizeDelta -= new Vector2((gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x) * column, 0);
            }
        }
    }

    void ScrollCallback(Vector2 data)
    {
        UpdateChildren();
    }

    void UpdateChildren()
    {
        if (transform.childCount < minAmount)
        {
            return;
        }

        Vector2 currentPos = rectTransform.anchoredPosition;

        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            float offsetY = currentPos.y - startPosition.y;

            if (offsetY > 0)
            {
                //向上拉，向下扩展;
                if (realIndex >= amount - 1)
                {
                    startPosition = currentPos;
                    return;
                }

                float scrollRectUp = scrollRect.transform.TransformPoint(Vector3.zero).y;

                Vector3 childBottomLeft = new Vector3(children[0].anchoredPosition.x, children[0].anchoredPosition.y - gridLayoutGroup.cellSize.y, 0f);
                float childBottom = transform.TransformPoint(childBottomLeft).y;

                if (childBottom >= scrollRectUp)
                {
                    //移动到底部;
                    for (int index = 0; index < gridLayoutGroup.constraintCount; index++)
                    {
                        children[index].SetAsLastSibling();

                        children[index].anchoredPosition = new Vector2(children[index].anchoredPosition.x, children[children.Count - 1].anchoredPosition.y - gridLayoutGroup.cellSize.y - gridLayoutGroup.spacing.y);

                        realIndex++;

                        if (realIndex > amount - 1)
                        {
                            children[index].gameObject.SetActive(false);
                        }
                        else
                        {
                            UpdateChildrenCallback(realIndex, children[index]);
                        }
                    }

                    //GridLayoutGroup 底部加长;
                    rectTransform.sizeDelta += new Vector2(0, gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);

                    //更新child;
                    for (int index = 0; index < children.Count; index++)
                    {
                        children[index] = transform.GetChild(index).GetComponent<RectTransform>();
                    }
                }
            }
            else
            {
                //向下拉，下面收缩;
                if (realIndex + 1 <= children.Count)
                {
                    startPosition = currentPos;
                    return;
                }
                RectTransform scrollRectTransform = scrollRect.GetComponent<RectTransform>();
                Vector3 scrollRectAnchorBottom = new Vector3(0, -scrollRectTransform.rect.height - gridLayoutGroup.spacing.y, 0f);
                float scrollRectBottom = scrollRect.transform.TransformPoint(scrollRectAnchorBottom).y;

                Vector3 childUpLeft = new Vector3(children[children.Count - 1].anchoredPosition.x, children[children.Count - 1].anchoredPosition.y, 0f);

                float childUp = transform.TransformPoint(childUpLeft).y;

                if (childUp < scrollRectBottom)
                {
                    //把底部的一行 移动到顶部
                    for (int index = 0; index < gridLayoutGroup.constraintCount; index++)
                    {
                        children[children.Count - 1 - index].SetAsFirstSibling();

                        children[children.Count - 1 - index].anchoredPosition = new Vector2(children[children.Count - 1 - index].anchoredPosition.x, children[0].anchoredPosition.y + gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);

                        children[children.Count - 1 - index].gameObject.SetActive(true);

                        UpdateChildrenCallback(realIndex - children.Count - index, children[children.Count - 1 - index]);
                    }

                    realIndex -= gridLayoutGroup.constraintCount;

                    //GridLayoutGroup 底部缩短;
                    rectTransform.sizeDelta -= new Vector2(0, gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);

                    //更新child;
                    for (int index = 0; index < children.Count; index++)
                    {
                        children[index] = transform.GetChild(index).GetComponent<RectTransform>();
                    }
                }
            }
        }
        else
        {
            float offsetX = currentPos.x - startPosition.x;

            if (offsetX < 0)
            {
                //向左拉，向右扩展;
                if (realIndex >= amount - 1)
                {
                    startPosition = currentPos;
                    return;
                }

                float scrollRectLeft = scrollRect.transform.TransformPoint(Vector3.zero).x;

                Vector3 childBottomRight = new Vector3(children[0].anchoredPosition.x + gridLayoutGroup.cellSize.x, children[0].anchoredPosition.y, 0f);
                float childRight = transform.TransformPoint(childBottomRight).x;

                if (childRight <= scrollRectLeft)
                {
                    //移动到右边;
                    for (int index = 0; index < gridLayoutGroup.constraintCount; index++)
                    {
                        children[index].SetAsLastSibling();

                        children[index].anchoredPosition = new Vector2(children[children.Count - 1].anchoredPosition.x + gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x, children[index].anchoredPosition.y);

                        realIndex++;

                        if (realIndex > amount - 1)
                        {
                            children[index].gameObject.SetActive(false);
                        }
                        else
                        {
                            UpdateChildrenCallback(realIndex, children[index]);
                        }
                    }

                    //GridLayoutGroup 右侧加长;
                    rectTransform.sizeDelta += new Vector2(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x, 0);

                    //更新child;
                    for (int index = 0; index < children.Count; index++)
                    {
                        children[index] = transform.GetChild(index).GetComponent<RectTransform>();
                    }
                }
            }
            else
            {
                //向右拉，右边收缩;
                if (realIndex + 1 <= children.Count)
                {
                    startPosition = currentPos;
                    return;
                }
                RectTransform scrollRectTransform = scrollRect.GetComponent<RectTransform>();
                Vector3 scrollRectAnchorRight = new Vector3(scrollRectTransform.rect.width + gridLayoutGroup.spacing.x, 0, 0f);
                float scrollRectRight = scrollRect.transform.TransformPoint(scrollRectAnchorRight).x;

                Vector3 childUpLeft = new Vector3(children[children.Count - 1].anchoredPosition.x, children[children.Count - 1].anchoredPosition.y, 0f);

                float childLeft = transform.TransformPoint(childUpLeft).x;

                if (childLeft >= scrollRectRight)
                {
                    //把右边的一行 移动到左边;
                    for (int index = 0; index < gridLayoutGroup.constraintCount; index++)
                    {
                        children[children.Count - 1 - index].SetAsFirstSibling();

                        children[children.Count - 1 - index].anchoredPosition = new Vector2(children[0].anchoredPosition.x - gridLayoutGroup.cellSize.x - gridLayoutGroup.spacing.x, children[children.Count - 1 - index].anchoredPosition.y);

                        children[children.Count - 1 - index].gameObject.SetActive(true);

                        UpdateChildrenCallback(realIndex - children.Count - index, children[children.Count - 1 - index]);
                    }

                    //GridLayoutGroup 右侧缩短;
                    rectTransform.sizeDelta -= new Vector2(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x, 0);

                    //更新child;
                    for (int index = 0; index < children.Count; index++)
                    {
                        children[index] = transform.GetChild(index).GetComponent<RectTransform>();
                    }

                    realIndex -= gridLayoutGroup.constraintCount;
                }
            }
        }

        startPosition = currentPos;
    }

    void UpdateChildrenCallback(int index, Transform trans)
    {
        if (updateChildrenCallback != null)
        {
            updateChildrenCallback(index, trans);
        }
    }

    /// <summary> 设置内容数量 </summary>
    public void SetAmount(int count)
    {
        amount = count;
        InitChildren();
    }

    public void _SetAmount(int count)
    {
        amount = count;
    }

    private Timer _moveTimer;
    private Vector3 _endPos;
    private float _duration;

    public void MoveToCellIndex(int index, float duration = 0.2f)
    {
        if (index == 0)
            return;

        Vector3 position = Vector3.zero;

        if (scrollRect.vertical)
        {
            position.y = (index) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            position.y -= scrollRect.viewport.sizeDelta.y;
        }
        else if (scrollRect.horizontal)
        {
            position.x = (index) * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
            position.x -= scrollRect.viewport.sizeDelta.x;
        }
        

        scrollRect.StopMovement();

        if (coroutine != null)
            StopCoroutine(coroutine);

        _endPos = position;
        _duration = duration < 0.2f ? 0.2f : duration;

        coroutine = StartCoroutine(MoveToTarget());
    }

    protected IEnumerator MoveToTarget()
    {
        yield return new WaitForSeconds(0.2f);

        _moveTimer?.Cancel();
        _moveTimer = Timer.Register(_duration, null, OnMoving, false);
    }

    private void OnMoving(float time)
    {
        Vector3 startPos = rectTransform.anchoredPosition;
        Vector3 currentPosition = Vector3.Lerp(startPos, _endPos, time / _duration);
        //Debug.LogErrorFormat("cur == {0}", currentPosition.y);
        rectTransform.anchoredPosition = currentPosition;
    }
}
