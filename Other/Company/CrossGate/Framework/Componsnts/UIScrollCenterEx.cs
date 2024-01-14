using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 循环的ScrollRect，需求：有翻页效果，然后可以索引到对应的位置
/// 需要注意：1.缓存池，设置个数2.有按页翻的效果
/// 先设置好content的长度或者宽度,然后进行初始化操作，对每个item进行初始化的处理
/// </summary>
public class UIScrollCenterEx : ScrollRect
{
    public enum Axis
    {
        Horizontal = 0,
        Vertical = 1
    }
    public enum Constraint
    {
        Flexible = 0,
        FixedColumnCount = 1,
        FixedRowCount = 2
    }
    public enum ScrollState
    {
        UnDrag = 0,
        Draging = 1,
        CheckDrag = 2
    }

    //-------------------public attributes---------------------------------

    [HideInInspector]
    public GameObject prefab;
    [HideInInspector]
    public bool isPage;//是否属于翻页功能
    [HideInInspector]
    public bool isVertical;

    [HideInInspector]
    public Axis startAxis;
    [HideInInspector]
    public Constraint constraint;
    [HideInInspector]
    public int constraintCount = 1;

    [HideInInspector]
    public int spacingX;//horizontalGap
    [HideInInspector]
    public int spacingY;//verticalGap
    [HideInInspector]
    public int paddingTop;
    [HideInInspector]
    public int paddingBottom;
    [HideInInspector]
    public int paddingLeft;
    [HideInInspector]
    public int paddingRight;
    [HideInInspector]
    public int poolCount = 4;

    //-------------------public isPage attributes---------------------------------

    [HideInInspector]
    public bool bLock = false;
    [HideInInspector]
    public float fCheckSpeed = 500.0f;
    [HideInInspector]
    public float fCenterSpeed = 20.0f;
    [HideInInspector]
    public float fThreshold = 30.0f;
    [HideInInspector]
    public Vector3 v3ZoomNormal = Vector3.one;
    [HideInInspector]
    public Vector3 v3ZoomCenter = Vector3.one;

    //------------------private attributes--------------------------------

    private RectTransform m_PoolContainer;
    private ScrollState m_state = ScrollState.UnDrag;
    private bool m_isAwake = false;
    public bool m_isInit { private set; get; }
    public int m_totalCount { private set; get; }
    public float m_width { private set; get; }
    public float m_height { private set; get; }
    public float m_contentWidth { private set; get; }
    public float m_contentHeight { private set; get; }
    public float m_cellWidth { private set; get; }//预制体的宽
    public float m_cellHeight { private set; get; }//预制体的高
    public int m_rowNum { private set; get; }//最多同时显示多少行
    public int m_colNum { private set; get; }//最多同时显示多少列
    public int m_realRowNum { private set; get; }//实际上需要显示多少行
    public int m_realColNum { private set; get; }//实际上需要显示多少列
    public int m_realPageNum { private set; get; }//有翻页功能时的页数
    public float m_pageGap { private set; get; }//一页的宽度与scrollView宽度的差距
    public float m_PageWidth { private set; get; }
    public float m_PageHeight { private set; get; }
    public float m_scrollOffset { private set; get; }
    public float m_scrollableValue { private set; get; }
    private bool m_requiresRefresh = false;
    private bool m_isInitPool = false;
    private bool m_isMoving = false;
    private List<int> m_showIds = new List<int>();
    private List<float> m_pageOffsets = new List<float>();

    //------------------private isPage attributes--------------------------------
    private int m_centerChildIndex;
    private float fBeginDrag;
    private float fDelta;

    public bool canDrag = true;

    #region delegate


    public delegate void UpdateChildrenCallbackDelegate(int index, Transform trans);
    public UpdateChildrenCallbackDelegate m_itemSetHandler = null;

    public delegate void OnCenterDelegate(int index);
    public OnCenterDelegate cb_CenterChildSettle = null;
    public Action<Transform> OnHide;
    #endregion


    #region public get
    public bool isAwake
    {
        get
        {
            return m_isAwake;
        }
    }
    public List<int> showIds
    {
        get { return m_showIds; }
    }
    public int centerChildIndex
    {
        get { return m_centerChildIndex; }
    }
    public List<float> pageOffsets
    {
        get { return m_pageOffsets; }
    }
    public List<Transform> poolItems
    {
        get
        {
            List<Transform> list = new List<Transform>();
            foreach (Transform item in poolQueue)
            {
                list.Add(item);
            }
            return list;
        }
    }
    public List<Transform> ShowPool
    {
        get
        {
            return showPool;
        }
    }
    #endregion

    #region LifeCycle
    void Update()
    {
        if (isPage)
        {
            switch (m_state)
            {
                case ScrollState.UnDrag:
                    break;
                case ScrollState.Draging:
                    DoCenter();
                    break;
                case ScrollState.CheckDrag:
                    CheckDoCenter();
                    break;
            }
        }
        if (m_requiresRefresh)
        {
            RefreshScrollItems();
        }
    }

    #endregion

    #region override drag
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if(canDrag)
        {
            base.OnBeginDrag(eventData);
            if (isPage)
            {
                OnBeginDragFunc(eventData);
            }
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            if (isPage)
            {
                bool flag = true;
                flag = OnDragFunc(eventData);

                if (flag == true)
                {
                    base.OnDrag(eventData);
                }
            }
            else
            {
                base.OnDrag(eventData);
            }
        }
    }
    
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            base.OnEndDrag(eventData);
            if (isPage)
            {
                OnEndDragFunc(eventData);
            }
        }
    }
    private void OnBeginDragFunc(PointerEventData pData)
    {
        StopDoCenter();
        fBeginDrag = isVertical ? pData.position.y : pData.position.x;
        SetItemScale(true);
    }
    private bool OnDragFunc(PointerEventData pData)
    {
        if (bLock)
        {
            float fCurDrag = isVertical ? pData.position.y : pData.position.x;
            if (isVertical)
            {
                if (Mathf.Abs(fCurDrag - fBeginDrag) > m_PageHeight)
                {
                    return false;
                }
            }
            else
            {
                if (Mathf.Abs(fCurDrag - fBeginDrag) > m_PageWidth)
                {
                    return false;
                }
            }
        }
        return true;
    }
    private void OnEndDragFunc(PointerEventData pData)
    {
        if (ShowPool.Count <= 1)
            return;
        if (bLock == true)
        {
            if (isVertical)
            {
                fDelta = fBeginDrag - pData.position.y;
            }
            else
            {
                fDelta = pData.position.x - fBeginDrag;
            }
            if (fDelta < -1 * fThreshold)
            {
                IncDecIndex(true);
            }
            else if (fDelta > fThreshold)
            {
                IncDecIndex(false);
            }
            else
            {
                SetCenterChild();
            }
        }
        else
        {
            m_state = ScrollState.CheckDrag;
        }
    }
    private void IncDecIndex(bool bInc)
    {
        int tmp_centerIndex = bInc ? m_centerChildIndex + 1 : m_centerChildIndex - 1;
        tmp_centerIndex = Mathf.Clamp(tmp_centerIndex, 1, m_realPageNum);
        if (tmp_centerIndex != m_centerChildIndex)
        {
            m_centerChildIndex = tmp_centerIndex;
            SetCenterChild();
        }
        else
        {
            SetItemScale(false);
        }
    }
    private bool CheckBorder()
    {
        if (isVertical)
        {
            bool isMin = content.anchoredPosition.y >= 0;
            bool isMax = content.anchoredPosition.y <= (content.rect.size.y - m_height);
            return isMin && isMax;
        }
        else
        {
            bool isMin = content.anchoredPosition.x <= 0;
            bool isMax = content.anchoredPosition.x >= m_width - content.rect.size.x;
            //bool isMax = Mathf.Abs(content.rect.size.x - m_width) >= Mathf.Abs(content.anchoredPosition.x);
            return isMin && isMax;
        }
    }
    
    private void CheckDoCenter()
    {
        if (velocity.magnitude > fCheckSpeed && CheckBorder() == true)
        {
            return;
        }
        FindNearlistPageIndex();
        SetCenterChild();
    }
    private void StopDoCenter()
    {
        m_state = ScrollState.UnDrag;
    }
    private void StartDoCenter()
    {
        if (velocity != Vector2.zero)
        {
            velocity = Vector2.zero;
        }
        m_state = ScrollState.Draging;
    }
    private void DoCenter()
    {
        float targetOffset = m_centerChildIndex - 1 < m_pageOffsets.Count && m_centerChildIndex - 1 >= 0 ? m_pageOffsets[m_centerChildIndex - 1] : 0;
        scrollOffset = Mathf.Lerp(scrollOffset, targetOffset, fCenterSpeed * Time.deltaTime);
        if (Mathf.Abs(targetOffset - scrollOffset) < 0.02f)
        {
            scrollOffset = targetOffset;
            m_state = ScrollState.UnDrag;
        }
    }
    #endregion

    #region Function

    void InitMovingType()
    {
        vertical = isVertical;
        horizontal = !isVertical;
    }

    void NormalizeTransform(Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localEulerAngles = Vector3.zero;
        t.localScale = Vector3.one;
    }

    void InitCellSize()
    {
        var rt = this.prefab.GetComponent<RectTransform>();
        m_cellWidth = rt.rect.width;
        m_cellHeight = rt.rect.height;

    }

    void InitRowColCount()
    {
        float realWidth = m_width - paddingLeft - paddingRight - m_cellWidth;
        if (realWidth < 0)
        {
            realWidth = 0;
        }
        m_colNum = (int)Mathf.Floor(realWidth / (m_cellWidth + spacingX)) + 1;
        float realHeight = m_height - paddingTop - paddingBottom - m_cellHeight;
        if (realHeight < 0)
        {
            realHeight = 0;
        }
        m_rowNum = (int)Mathf.Floor(realHeight / (m_cellHeight + spacingY)) + 1;

    }

    void CalculateRealRowColCount(int totalCount)
    {
        if (isPage == false)
        {
            if (isVertical == true)
            {
                switch (constraint)
                {
                    case Constraint.Flexible:
                        m_realColNum = m_colNum;
                        m_realRowNum = Mathf.CeilToInt(totalCount * 1f / m_colNum);
                        break;
                    case Constraint.FixedColumnCount:
                        m_realColNum = Mathf.Clamp(this.constraintCount, 1, m_colNum);
                        m_realRowNum = Mathf.CeilToInt(totalCount * 1f / m_realColNum);
                        break;
                    case Constraint.FixedRowCount:
                        m_realRowNum = Mathf.Clamp(this.constraintCount, 1, m_rowNum);
                        m_realColNum = Mathf.CeilToInt(totalCount * 1f / m_realRowNum);
                        break;
                }
            }
            else
            {
                switch (constraint)
                {
                    case Constraint.Flexible:
                        m_realRowNum = m_rowNum;
                        m_realColNum = Mathf.CeilToInt(totalCount * 1f / m_rowNum);
                        break;
                    case Constraint.FixedColumnCount:
                        m_realColNum = Mathf.Clamp(this.constraintCount, 1, m_colNum);
                        m_realRowNum = Mathf.CeilToInt(totalCount * 1f / m_realColNum);
                        break;
                    case Constraint.FixedRowCount:
                        m_realRowNum = Mathf.Clamp(this.constraintCount, 1, m_rowNum);
                        m_realColNum = Mathf.CeilToInt(totalCount * 1f / m_realRowNum);
                        break;
                }
            }
        }
        else
        {
            m_realColNum = m_colNum;
            m_realRowNum = m_rowNum;
            m_realPageNum = Mathf.CeilToInt(m_totalCount * 1f / (m_realColNum * m_realRowNum));
            m_PageWidth = paddingLeft + paddingRight + m_realColNum * m_cellWidth + (m_realColNum - 1) * spacingX;
            m_PageHeight = paddingTop + paddingBottom + m_realRowNum * m_cellHeight + (m_realRowNum - 1) * spacingY;
            m_pageGap = isVertical ? m_height - m_PageHeight : m_width - m_PageWidth;
        }
    }

    void InitContentSize()
    {
        if (isPage == false)
        {
            float tmp_width = paddingLeft + paddingRight + m_realColNum * m_cellWidth + (m_realColNum - 1) * spacingX;
            float tmp_height = paddingTop + paddingBottom + m_realRowNum * m_cellHeight + (m_realRowNum - 1) * spacingY;
            if (tmp_width < m_width)
                m_contentWidth = m_width;
            else
                m_contentWidth = tmp_width;
            if (tmp_height < m_height)
                m_contentHeight = m_height;
            else
                m_contentHeight = tmp_height;
        }
        else
        {
            if (isVertical)
            {
                m_contentWidth = m_PageWidth;
                m_contentHeight = m_PageHeight * m_realPageNum + m_pageGap;
            }
            else
            {
                m_contentWidth = m_PageWidth * m_realPageNum + m_pageGap;
                m_contentHeight = m_PageHeight;
            }
        }

        content.offsetMin = new Vector2(0, -m_contentHeight);
        content.offsetMax = new Vector2(m_contentWidth, 0);
        content.anchoredPosition = Vector2.zero;

        if (isVertical)
        {
            m_scrollableValue = m_contentHeight - m_height;
            if (m_scrollableValue < 0)
            {
                m_scrollableValue = 0;
            }
        }
        else
        {
            m_scrollableValue = m_contentWidth - m_width;
            if (m_scrollableValue < 0)
            {
                m_scrollableValue = 0;
            }
        }

    }

    Vector3 GetPosition(int index)
    {
        float x = paddingLeft;
        float y = -paddingTop;
        if (isPage == false)
        {
            if (startAxis == Axis.Horizontal)
            {
                index--;
                int n = index % m_realColNum;
                int m = (int)(index / m_realColNum);
                x += n * (m_cellWidth + spacingX);
                y -= m * (m_cellHeight + spacingY);
            }
            else
            {
                index--;
                int m = index % m_realRowNum;
                int n = (int)(index / m_realRowNum);
                x += n * (m_cellWidth + spacingX);
                y -= m * (m_cellHeight + spacingY);
            }
        }
        else
        {
            if (isVertical)
            {
                y -= m_pageGap / 2;
            }
            else
            {
                x += m_pageGap / 2;
            }
            int pageIndex = Mathf.CeilToInt(index * 1f / (m_realColNum * m_realRowNum));
            if (isVertical)
            {
                y -= (pageIndex - 1) * m_PageHeight;
            }
            else
            {
                x += (pageIndex - 1) * m_PageWidth;
            }
            int tmpCurPageIndex = index - (pageIndex - 1) * (m_realColNum * m_realRowNum);//当前页的索引角标
            tmpCurPageIndex--;
            if (startAxis == Axis.Horizontal)
            {
                int n = tmpCurPageIndex % m_realColNum;
                int m = (int)(tmpCurPageIndex / m_realColNum);
                x += n * (m_cellWidth + spacingX);
                y -= m * (m_cellHeight + spacingY);
            }
            else
            {
                int m = tmpCurPageIndex % m_realRowNum;
                int n = (int)(tmpCurPageIndex / m_realRowNum);
                x += n * (m_cellWidth + spacingX);
                y -= m * (m_cellHeight + spacingY);
            }
        }
        x += m_cellWidth / 2;
        y -= m_cellHeight / 2 + (m_contentHeight - m_cellHeight) / 2;
        return new Vector3(x, y, 0);
    }

    Vector3 GetPagePosition(int index)
    {
        if (!isPage)
        {
            return Vector3.zero;
        }
        float x = 0;
        float y = 0;
        int pageIndex = Mathf.CeilToInt(index * 1f / (m_realColNum * m_realRowNum));
        if (isVertical)
        {
            x = 0;
            y = -(pageIndex - 1) * m_PageHeight;
        }
        else
        {
            x = (pageIndex - 1) * m_PageWidth;
            y = 0;
        }
        //x += m_cellWidth / 2;
        //y -= m_cellHeight / 2;
        return new Vector3(x, y, 0);
    }

    void ScrollViewValueChanged(Vector2 newScrollValue)
    {
        if (!isAwake)
        {
            return;
        }
        if (!m_isInit)
        {
            return;
        }
        float relativeScroll = isVertical ? 1 - newScrollValue.y : newScrollValue.x;
        float tmp_scrollOffset = relativeScroll * m_scrollableValue;
        if (Mathf.Abs(m_scrollOffset - tmp_scrollOffset) > 0.01f)
        {
            m_requiresRefresh = true;
        }
        m_scrollOffset = tmp_scrollOffset;

    }

    void RefreshScrollItems()
    {
        CalculateCurShowIndexs();
        int count = showPool.Count - 1;
        for (int i = count; i >= 0; i--)
        {
            Transform tr = showPool[i];
            int index = int.Parse(tr.name);
            bool bContains = false;
            for (int j = 0; j < m_showIds.Count; j++)
            {
                if (m_showIds[j] == index)
                {
                    bContains = true;
                    break;
                }
            }
            if (bContains == false)
            {
                SaveToPool(tr);
            }
        }

        for (int i = 0; i < m_showIds.Count; i++)
        {
            int index = m_showIds[i];
            bool bContains = false;
            for (int j = 0; j < showPool.Count; j++)
            {
                Transform tr = showPool[j];
                if (int.Parse(tr.name) == index)
                {
                    bContains = true;
                    break;
                }
            }
            if (bContains == false)
            {
                Transform child = GetItemInPool();
                if (child != null)
                {
                    child.localPosition = GetPosition(index);
                    child.name = index.ToString();
                    if (isPage)
                    {
                        SetItemScale(child, index);
                    }
                    m_itemSetHandler(index - 1, child);
                }
            }
        }
        this.content.name = m_showIds.Count.ToString();

        m_requiresRefresh = false;
    }
    int GetCurMinIndex()//获取当前位置的最小索引
    {
        if (isVertical)//竖直
        {
            float tmp_offset = m_scrollOffset - paddingTop - m_cellHeight;
            if (tmp_offset < 0)
            {
                return 1;
            }
            //从1开始计算向下的增量
            float tmp_countY = tmp_offset / (m_cellHeight + spacingY);
            int countY = (int)tmp_countY + 1;
            if (startAxis == Axis.Horizontal)
            {
                return 1 + countY * m_realColNum;
            }
            else
            {
                return 1 + countY;
            }
        }
        else//水平
        {
            float tmp_offset = m_scrollOffset - paddingLeft - m_cellWidth;
            if (tmp_offset < 0)
            {
                return 1;
            }
            //从1开始计算向下的增量
            float tmp_countX = tmp_offset / (m_cellWidth + spacingX);
            int countX = (int)tmp_countX + 1;
            if (startAxis == Axis.Horizontal)
            {
                return 1 + countX;
            }
            else
            {
                return 1 + countX * m_realRowNum;
            }
        }
    }
    void CalculateCurShowIndexs()
    {
        if (isPage == false)
        {
            int m_needRowCount = 1;
            int m_needColCount = 1;
            //计算当前需要显示几行几列
            if (isVertical)
            {
                m_needColCount = m_colNum;
                m_needRowCount = m_rowNum + 2;

            }
            else
            {
                m_needRowCount = m_rowNum;
                m_needColCount = m_colNum + 2;

            }

            int minIndex = GetCurMinIndex();
            m_showIds.Clear();
            if (startAxis == Axis.Horizontal)
            {
                for (int i = minIndex; i < minIndex + m_needColCount; i++)
                {
                    for (int j = 0; j < m_needRowCount; j++)
                    {
                        int index = i + j * m_realColNum;
                        if (canShowIndex(index))
                        {
                            m_showIds.Add(index);
                        }
                    }
                }
            }
            else
            {
                for (int i = minIndex; i < minIndex + m_needRowCount; i++)
                {
                    for (int j = 0; j < m_needColCount; j++)
                    {
                        int index = i + j * m_realRowNum;
                        if (canShowIndex(index))
                        {
                            m_showIds.Add(index);
                        }
                    }
                }
            }
            m_showIds.Sort();
        }
        else
        {
            int showPageIndex = Mathf.CeilToInt((m_scrollOffset - m_pageGap / 2) / (isVertical ? m_PageHeight : m_PageWidth));//需要显示的第一页的索引
            int minIndex = (showPageIndex - 1) * (m_realColNum * m_realRowNum) + 1;
            int maxIndex = (showPageIndex + 2) * (m_realColNum * m_realRowNum);
            m_showIds.Clear();
            for (int index = minIndex; index <= maxIndex; index++)
            {
                if (canShowIndex(index))
                {
                    m_showIds.Add(index);
                }
            }
        }
    }
    bool canShowIndex(int index)
    {
        if (index < 1 || index > m_totalCount)
        {
            return false;
        }
        Vector3 pos = GetPosition(index);
        float nCurOffset = Mathf.Abs(isVertical ? pos.y : pos.x);
        float nMinOffset = m_scrollOffset - (isVertical ? m_cellHeight / 2 : m_cellWidth / 2);
        float nMaxOffset = m_scrollOffset + (isVertical ? m_height : m_width) + (isVertical ? m_cellHeight / 2 : m_cellWidth / 2);
        return nCurOffset >= nMinOffset && nCurOffset <= nMaxOffset;
    }
    void CalculateEachPageOffset()//计算每一页的偏移值
    {
        m_pageOffsets.Clear();
        if (isVertical)
        {
            for (int i = 1; i <= m_realPageNum; i++)
            {
                float tmp_offset = m_pageGap / 2 + m_PageHeight / 2 + (i - 1) * m_PageHeight - m_height / 2;
                m_pageOffsets.Add(tmp_offset);
            }
        }
        else
        {
            for (int i = 1; i <= m_realPageNum; i++)
            {
                float tmp_offset = m_pageGap / 2 + m_PageWidth / 2 + (i - 1) * m_PageWidth - m_width / 2;
                m_pageOffsets.Add(tmp_offset);
            }
        }
    }
    void FindNearlistPageIndex()
    {
        if (m_pageOffsets.Count == 0)
        {
            return;
        }
        if (m_pageOffsets.Count == 1)
        {
            m_centerChildIndex = 1;
            return;
        }
        for (int i = 0; i < m_pageOffsets.Count - 1; i++)
        {
            int index1 = i;
            int index2 = i + 1;
            float fMinOffset = m_pageOffsets[index1];
            float fMaxOffset = m_pageOffsets[index2];
            if (m_scrollOffset >= fMinOffset && m_scrollOffset < fMaxOffset)
            {
                float tmp_offset1 = Mathf.Abs(m_scrollOffset - fMinOffset);
                float tmp_offset2 = Mathf.Abs(m_scrollOffset - fMaxOffset);
                if (tmp_offset1 > tmp_offset2)
                {
                    m_centerChildIndex = index2 + 1;
                }
                else
                {
                    m_centerChildIndex = index1 + 1;
                }
                return;
            }
        }
        if (m_scrollOffset <= 0)
        {
            m_centerChildIndex = 1;
        }
        else
        {
            m_centerChildIndex = m_pageOffsets.Count;
        }
    }
    void SetItemScale(bool isNormal)
    {
        if(ShowPool.Count <= 1)
            return;
        int minIndex = (m_centerChildIndex - 1) * (m_realColNum * m_realRowNum) + 1;
        int maxIndex = m_centerChildIndex * (m_realColNum * m_realRowNum);

        for (int i = minIndex; i <= maxIndex; i++)
        {
            for (int j = 0; j < ShowPool.Count; j++)
            {
                if (ShowPool[j].name == i.ToString())
                {
                    ShowPool[j].localScale = isNormal ? v3ZoomNormal : v3ZoomCenter;
                }
            }
        }
    }
    void SetItemScale(Transform itemTr, int index)
    {
        int minIndex = (m_centerChildIndex - 1) * (m_realColNum * m_realRowNum) + 1;
        int maxIndex = m_centerChildIndex * (m_realColNum * m_realRowNum);
        if (index >= minIndex && index <= maxIndex)
        {
            itemTr.localScale = v3ZoomCenter;
        }
        else
        {
            itemTr.localScale = v3ZoomNormal;
        }
    }
    void CloseMoving()
    {
        this.horizontal = false;
        this.vertical = false;
    }
    IEnumerator AnimateToScrollR(float targetOffset, float speed)
    {
        float startScroll = this.scrollOffset;
        CloseMoving();
        while (this.scrollOffset < targetOffset)
        {
            this.scrollOffset += speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        this.scrollOffset = targetOffset;
        InitMovingType();
        m_isMoving = false;
    }

    IEnumerator AnimateToScrollL(float targetOffset, float speed)
    {
        float startScroll = this.scrollOffset;
        CloseMoving();
        while (this.scrollOffset > targetOffset)
        {
            this.scrollOffset -= speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        this.scrollOffset = targetOffset;
        InitMovingType();
        m_isMoving = false;
    }

    #endregion


    #region Public Function
    public void SetParam(GameObject _prefab, int _poolCount)
    {
        if (m_isAwake == true)
        {
            Debug.LogError("UIScrollGridEx is Already Inited ! ! !");
            return;
        }
        m_isAwake = true;
        onValueChanged.RemoveAllListeners();
        onValueChanged.AddListener(ScrollViewValueChanged);
        m_width = this.GetComponent<RectTransform>().rect.width;
        m_height = this.GetComponent<RectTransform>().rect.height;
        if (m_PoolContainer == null)
        {
            m_PoolContainer = new GameObject("ReusableCells", typeof(RectTransform)).GetComponent<RectTransform>();
            m_PoolContainer.SetParent(this.transform, false);
            m_PoolContainer.gameObject.SetActive(false);
        }
        content.anchorMin = new Vector2(0, 1);
        content.anchorMax = new Vector2(0, 1);
        content.pivot = new Vector2(0, 1);
        content.offsetMin = new Vector2();
        content.offsetMax = new Vector2();
        m_isInitPool = false;
        m_isInit = false;
        if (isPage)
        {
            m_centerChildIndex = 1;
        }
        if (_prefab != null)
        {
            this.prefab = _prefab;
        }
        if (_poolCount != 0)
        {
            this.poolCount = _poolCount;
        }
        InitMovingType();
        InitCellSize();
        InitRowColCount();
    }
    public void Init(int totalCount)
    {
        m_isInit = true;
        this.StopMovement();
        m_totalCount = totalCount;
        CalculateRealRowColCount(totalCount);
        InitContentSize();
        if (isPage)
        {
            CalculateEachPageOffset();
        }
        InitPool();//TODO:需要进行延迟实例化
        m_requiresRefresh = true;// To Fix 
    }
    public void UpdateIndex(int index)
    {
        for (int i = 0; i < showPool.Count; i++)
        {
            var child = showPool[i];
            if (child.name == index.ToString())
            {
                m_itemSetHandler(index - 1, child);
            }
        }
    }

    public void SetHideAction(Action<Transform> action)
    {
        OnHide = action;
    }

    public void SetCenterChild()
    {
        StopDoCenter();
        SetItemScale(false);
        StartDoCenter();
        cb_CenterChildSettle(m_centerChildIndex - 1);
    }

    public float scrollOffset
    {
        get
        {
            return m_scrollOffset;
        }
        set
        {
            if (m_isInit == false)
            {
                return;
            }
            if (this.m_scrollableValue == 0)
            {
                m_scrollOffset = 0;
                m_requiresRefresh = true;
                return;
            }
            value = Mathf.Clamp(value, 0, this.m_scrollableValue);
            // if (m_scrollOffset != value)
            // {

            // }
            m_scrollOffset = value;
            m_requiresRefresh = true;
            if (isVertical == true)
            {
                float relativeScroll = value / this.m_scrollableValue;
                this.verticalNormalizedPosition = 1 - relativeScroll;
            }
            else
            {
                float relativeScroll = value / this.m_scrollableValue;
                this.horizontalNormalizedPosition = relativeScroll;
            }
        }
    }
    public void SwitchIndex(int index, bool quickly, float speed = 100f)
    {
        if (m_isMoving)
        {
            return;
        }
        m_isMoving = true;
        index = Mathf.Clamp(index, 1, m_totalCount);
        Vector3 targetPos = isPage ? GetPagePosition(index) : GetPosition(index);
        float targetOffset = Mathf.Abs(isVertical ? targetPos.y : targetPos.x);
        if (quickly)
        {
            this.scrollOffset = targetOffset;
            m_isMoving = false;
            FindNearlistPageIndex();
            SetItemScale(false);
            cb_CenterChildSettle(index - 1);
        }
        else
        {
            if(this.scrollOffset < targetOffset)
            {
                StartCoroutine(AnimateToScrollR(targetOffset, speed));
            }
            else
            {
                StartCoroutine(AnimateToScrollL(targetOffset, speed));
            }
            
        }
    }
    public void PrePage(bool quickly = false)
    {
        if (!isPage)
        {
            return;
        }
        if (quickly)
        {
            m_centerChildIndex--;
            m_centerChildIndex = Mathf.Clamp(m_centerChildIndex, 1, m_realPageNum);
            scrollOffset = m_pageOffsets[m_centerChildIndex - 1];
        }
        else
        {
            IncDecIndex(false);
        }
    }
    public void NextPage(bool quickly = false)
    {
        if (!isPage)
        {
            return;
        }
        if (quickly)
        {
            m_centerChildIndex++;
            m_centerChildIndex = Mathf.Clamp(m_centerChildIndex, 1, m_realPageNum);
            scrollOffset = m_pageOffsets[m_centerChildIndex - 1];
        }
        else
        {
            IncDecIndex(true);
        }
    }
    #endregion


    #region Pool Controll
    private Queue<Transform> poolQueue;//隐藏池子
    private List<Transform> showPool;//显示池子
    void InitPool()
    {
        if (m_isInitPool == false)
        {
            m_isInitPool = true;
            poolQueue = new Queue<Transform>();
            showPool = new List<Transform>();
            for (int i = 0; i < poolCount; i++)
            {
                GameObject go = GameObject.Instantiate(prefab, this.m_PoolContainer);
                NormalizeTransform(go.transform);
                //RectTransform rectTrans = go.transform as RectTransform;
                //rectTrans.pivot = new Vector2( 0, 1 );
                //rectTrans.anchorMin = new Vector2( 0, 1 );
                //rectTrans.anchorMax = new Vector2( 0, 1 );
                go.SetActive(true);
                poolQueue.Enqueue(go.transform);
            }
        }
        else
        {
            ReleaseUsedItems();
        }
    }
    void ReleaseUsedItems()
    {
        int count = showPool.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            Transform tr = showPool[i];
            SaveToPool(tr);
        }
    }
    void SaveToPool(Transform tr)
    {
        OnHide?.Invoke(tr);
        showPool.Remove(tr);
        tr.SetParent(this.m_PoolContainer);
        NormalizeTransform(tr);
        poolQueue.Enqueue(tr);
    }
    Transform GetItemInPool()
    {
        if (poolQueue.Count == 0)
        {
            Debug.Log("poolQueue Count is zero !");
            return null;
        }
        Transform tr = poolQueue.Dequeue();
        tr.SetParent(this.content);
        NormalizeTransform(tr);
        showPool.Add(tr);
        return tr;
    }
    #endregion

}
