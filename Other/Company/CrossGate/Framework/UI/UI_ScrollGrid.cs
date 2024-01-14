using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;

public class UI_ScrollGrid : UIBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public enum EScrollDir
    {
        Horzi,
        Vertical
    }

    public class ChangeEvent : UnityEvent<int>
    {

    }

    public RectTransform m_ViewPort;
    public RectTransform m_Content;

    [SerializeField, Range(0, 1)]
    private float m_Axis = 0.5f;
    public float Axis { get { return m_Axis; } set { m_Axis = Mathf.Clamp01(value); } }

    //[SerializeField,Range(0, 1)]
    //public float m_ScrollSpeed = 1;
    //public float ScrollSpeed { get { return m_ScrollSpeed; } set { m_ScrollSpeed = Mathf.Clamp01(value); } }

    [SerializeField]
    private Vector2 m_ChildSize = Vector2.zero;
    public Vector2 ChildSize { get { return m_ChildSize; } set { m_ChildSize = value; } }

    [SerializeField]
    private float m_Space = 0;
    public float Space { get { return m_Space; } set { m_Space = value; } }


    [SerializeField]
    private EScrollDir m_ScrollDir = EScrollDir.Horzi;
    public EScrollDir ScrollDir { get { return m_ScrollDir; } set { m_ScrollDir = value; } }


    [SerializeField]
    private Font m_Font;
    public Font TextFont { get { return m_Font; } set { m_Font = value; } }

    [SerializeField]
    private int m_FontSize;
    public int FontSize { get { return m_FontSize; } set { m_FontSize = value; } }
    [SerializeField]
    private Color m_TextColor;
    public Color TextColor { get { return m_TextColor; } set { m_TextColor = value; } }

    //UI_ScrollGrid_Element[] m_Children = new UI_ScrollGrid_Element[0];
    List<UI_ScrollGrid_Element> m_Children = new List<UI_ScrollGrid_Element>();
    GridLayoutGroup m_gridLayoutGroup;


   // private bool m_bDrag = false;

    private int m_FocusIndex = 0;
    private int  FocusIndex { get { return m_FocusIndex; } set {

            if (m_FocusIndex == value)
                return;

            m_FocusIndex = value;

            FocusChangeEvent.Invoke(m_FocusIndex);
        } }

    public ChangeEvent FocusChangeEvent = new ChangeEvent();

    // Use this for initialization
    protected override void Start()
    {
        if (m_Content != null)
        {
            // Children = m_Content.GetComponentsInChildren<UI_ScrollGrid_Element>();
            LoadChildern();
            m_gridLayoutGroup = m_Content.GetComponent<GridLayoutGroup>();

            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Content);
        }


        //VirPosition = Vector2.zero;

        Focus(FocusIndex);
    }

    private void LoadChildern()
    {
        UI_ScrollGrid_Element[] Children = m_Content.GetComponentsInChildren<UI_ScrollGrid_Element>();

        for (int i = 0; i < Children.Length; i++)
        {
            AddElement(Children[i].transform as RectTransform);
        }
    }
    protected override void OnEnable()
    {

        Focus(FocusIndex);
    }

    // Update is called once per frame
    //void Update()
    //{
       
    //}

    //void OnTransformChildrenChanged()
    //{
    //   // Debug.Log("scrollgrid children changed");
    //}

    private Vector2 mDragLastPosition = Vector3.zero;

    private Vector2 m_Position = Vector2.zero;

    public Vector2 VirPosition { get { return m_Position; } set { m_Position = value; SetPosition(m_Position); } }
    // 坐标以锚点 0.5f 为起始点，在横轴或者纵轴上。
    private Vector2 getAxisOffset()
    {
        float anchorMinValue = m_ScrollDir == EScrollDir.Horzi ? m_Content.anchorMin.x : m_Content.anchorMin.y;

        float size = m_ScrollDir == EScrollDir.Horzi ? m_ViewPort.rect.width : m_ViewPort.rect.height;

        float anchorDiff = anchorMinValue - 0.5f;

        float diffPos = size * anchorDiff;

        Vector2 offset = Vector2.zero;

        if (m_ScrollDir == EScrollDir.Horzi)
            offset.x = diffPos;
        else
            offset.y = diffPos;

        return offset;
    }

    // 将以锚点 0.5f 为起始点在横轴或者纵轴上的坐标 转换成 对应UI锚点的坐标
    private void SetPosition(Vector2 position)
    {

        RectTransform rectTransform = m_Content.transform as RectTransform;

        Vector2 diff = m_ScrollDir == EScrollDir.Horzi ? new Vector2(rectTransform.rect.width / 2, 0) : new Vector2(0, rectTransform.rect.height / 2);

        Vector2 realPos = VirtualToReal(position) - diff;

        rectTransform.anchoredPosition = realPos;

    }

    private Vector2 VirtualToReal(Vector2 virvalue)
    {
        Vector2 diffPos = getAxisOffset();


        return virvalue - diffPos;
    }

    private Vector2 RealToVirtual(Vector2 realvalue)
    {
        Vector2 diffPos = getAxisOffset();

        return realvalue + diffPos;

    }

    private int getGridIndex()
    {
        return getGridIndex(VirPosition);
    }

    private int getGridIndex(Vector2 alxsPos)
    {
        Vector2 virPos = alxsPos;

        float childSize = m_ScrollDir == EScrollDir.Horzi ? m_ChildSize.x : m_ChildSize.y;

        float curPos = m_ScrollDir == EScrollDir.Horzi ? virPos.x : virPos.y;

        float vlaue0 = ((curPos - childSize / 2) / (childSize + m_Space));

        float vlaue1 = vlaue0 - (int)(vlaue0);

        int index = (int)(vlaue0) + (vlaue1 > 0.5f ? 1 : 0);

        index = index > (m_Children.Count - 1) ? (m_Children.Count - 1) : index;

        index = Mathf.Max(index, 0);

        return index;
    }

    private Vector2 getVirPosition(int index)
    {
        float size = getChildSize();

        float offset = getMinValue() + (size + m_Space) * index;

        Vector2 pos = Vector2.zero;

        if (m_ScrollDir == EScrollDir.Horzi)
            pos.x = offset;
        else
            pos.y = offset;

        return pos;
    }
    public void FocusGrid(int index)
    {

    }
    private float getChildSize()
    {
        return m_ScrollDir == EScrollDir.Horzi ? m_ChildSize.x : m_ChildSize.y;
    }

    public float getMaxValue()
    {
        return getMinValue() + (m_Children.Count - 1) * (m_Space + getChildSize());
    }


    public float getMinValue()
    {
        return m_ScrollDir == EScrollDir.Horzi ? m_ChildSize.x / 2 : m_ChildSize.y / 2;
    }
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform rectTrans = m_Content.transform as RectTransform;

        Vector2 pos = VirPosition;


        if (m_ScrollDir == EScrollDir.Horzi)
        {
            pos.x += eventData.delta.x;
        }

        else
        {
            pos.y += eventData.delta.y;
        }


       // Debug.LogError(VirPosition);

        VirPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        int index = getGridIndex();

        Focus(index);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mDragLastPosition = eventData.position;
    }

    public void Focus(int index)
    {
        if (m_Children == null || index < 0 || index >= m_Children.Count)
            return;

        Vector2 target = getVirPosition(index);

        VirPosition = target;

        m_Children[index].Foucs();

        FocusIndex = index;

       // Debug.LogError("focus : " + index);
    }

    public void SetValus(string[] strings)
    {

    }

    public void SetValusRange(int min, int max)
    {
        if (m_Content == null)
            return;

        DestroyElements();

        for (int i = min; i <= max; i++)
        {
            RectTransform child = createElement();

            if (child == null)
                return;

            Text text = child.gameObject.AddComponent<Text>();

            text.text = i.ToString();

            text.alignment = TextAnchor.MiddleCenter;

            text.font = TextFont;

            text.fontSize = m_FontSize;

            text.color = m_TextColor;

            AddElement(child);

        }

        if (m_Content != null && IsActive())
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Content);
    }

    public void SetChildren(RectTransform[] children)
    {
        if (m_Content == null)
            return;

        for (int i = 0; i <= children.Length; i++)
        {
            RectTransform child = children[i];

            AddElement(child);
        }
    }

    private void AddElement(RectTransform rectTrans)
    {
        if (m_Content == null)
            return;

        UI_ScrollGrid_Element element = rectTrans.GetComponent<UI_ScrollGrid_Element>();

        if (element == null)
        {
            element = rectTrans.gameObject.AddComponent<UI_ScrollGrid_Element>();
        }

        rectTrans.SetParent(m_Content, false);

        if (m_Children.Contains(element) == false)
            m_Children.Add(element);
    }
    private RectTransform createElement()
    {
        if (m_Content == null)
            return null;

        GameObject element = new GameObject("element");

        RectTransform rect = element.AddComponent<RectTransform>();

        element.AddComponent<UI_ScrollGrid_Element>();


        return rect;

    }


    private void DestroyElements()
    {
        //if (m_Content == null)
        //    return;

        int childCount = m_Children.Count;

        for (int i = 0; i < childCount; i++)
        {
            DestroyImmediate(m_Children[i].gameObject);
        }

        m_Children.Clear();


    }
}
