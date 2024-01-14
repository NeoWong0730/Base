using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;

public class UI_ScrollGridEx : UIBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler,ILayoutGroup
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


    private bool isOnstart { get; set; } = false;
    protected override void Awake()
    {
        base.Awake();

        if(m_ViewPort == null)
        m_ViewPort = m_Content.parent as RectTransform;
    }
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

        UpdateBounds();

        if (m_Children.Count>0)
            FocusGrid(m_FocusIndex);

        isOnstart = true;
    }


  
    private void LoadChildern()
    {
        UI_ScrollGrid_Element[] Children = m_Content.GetComponentsInChildren<UI_ScrollGrid_Element>();

        for (int i = 0; i < Children.Length; i++)
        {
            AddElement(Children[i].transform as RectTransform);
        }
    }
    //protected override void OnEnable()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
       
    //}

    private Bounds mFocusBounds = new Bounds();

    private Bounds mContentBounds = new Bounds();

    private Bounds mViewBounds = new Bounds();

    private Vector2 mDragLastPosition = Vector3.zero;


    public void SetFocus(int index)
    {
        if (isOnstart == false)
        {
            m_FocusIndex = index;
            return;
        }

        UpdateBounds();

        FocusGrid(index);
    }


    private void FocusGrid(int index)
    {
        if (index >= m_Children.Count )
            index = m_Children.Count-1 > 0 ? (m_Children.Count -1) : 0;

        Vector2 offset = CalculateFocusElementOffset(index);

        Vector2 pos = m_Content.anchoredPosition + offset;    

        SetContentPosition(pos);

        int lastindex = FocusIndex;

        FocusIndex = index;


        if (lastindex != index && lastindex >= 0 && lastindex < m_Children.Count)
            m_Children[lastindex].LostFocus();

        if(index >= 0 && index < m_Children.Count)
           m_Children[index].Foucs();


    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    private Vector2 mStartLocalCursor = Vector2.zero;

    private Vector2 mStartContentPos = Vector2.zero;
    public void OnBeginDrag(PointerEventData eventData)
    {
        mDragLastPosition = eventData.position;

        mStartLocalCursor = Vector3.zero;

        bool result = RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ViewPort, eventData.position, eventData.pressEventCamera, out mStartLocalCursor);

        if (!result)
            return;

        mStartContentPos = m_Content.anchoredPosition;
    }
    public void OnDrag(PointerEventData eventData)
    {

        var localCursor = Vector2.zero;

        bool result = RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ViewPort, eventData.position, eventData.pressEventCamera, out localCursor);

        if (!result)
            return;

        UpdateBounds();

        var delta = localCursor - mStartLocalCursor;

        Vector2 pos = mStartContentPos + delta; // 目标位置

        Vector2 offset = CalculateOffset(pos - m_Content.anchoredPosition); //检查位置是否超出视口,返回差值//

       // Debug.LogError("pos offset:" + offset);

        pos += offset;

        SetContentPosition(pos);


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var normal = (m_Content.anchoredPosition - mStartContentPos).normalized;

        UpdateBounds();

        int index = CalculateFocusElement(normal);

        FocusGrid(index);

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
 

    public void SetLayoutHorizontal()
    {
        m_Children.Clear();

        LoadChildern();

        SetFocus(FocusIndex);
    }

    public void SetLayoutVertical()
    {
        //m_Children.Clear();

        //LoadChildern();
    }


    private void SetContentPosition(Vector2 position)
    {
        if (m_ScrollDir == EScrollDir.Horzi)
            position.y = m_Content.anchoredPosition.y;

        if (m_ScrollDir == EScrollDir.Vertical)
            position.x = m_Content.anchoredPosition.x;

        if (position != m_Content.anchoredPosition)
        {
            m_Content.anchoredPosition = position;

            UpdateBounds();
        }
    }

    private Vector2 CalculateFocusElementOffset(int index)
    {


        Vector2 offset = Vector2.zero;

        int count = index+1 ;

        if (m_ScrollDir == EScrollDir.Horzi)
        {
            offset.x = mFocusBounds.center.x - (ChildSize.x * count + Space * (count - 1) - ChildSize.x/2);

            offset.x = offset.x - mContentBounds.min.x;
        }

        if (m_ScrollDir == EScrollDir.Vertical)
        {
            offset.y = mFocusBounds.center.y - (ChildSize.y * count + Space * (count - 1) - ChildSize.y / 2);

            offset.y = offset.y - mContentBounds.min.y;
        }

        //Debug.LogError("offset:" + offset);

        return offset;
    }
    private int CalculateFocusElement(Vector2 normalDir)
    {
        int index = 0;


        if (m_ScrollDir == EScrollDir.Horzi)
        {

            var axisX = normalDir.x < 0 ? mFocusBounds.max.x : mFocusBounds.min.x;

            var offset = axisX - mContentBounds.min.x;

            float tempIndex = Mathf.Abs(offset) / (ChildSize.x + Space);

            float oindex = tempIndex - (int)tempIndex;

            float sizePre0 = (ChildSize.x / (ChildSize.x + Space)) * 0.3f;

            float sizePre1 = (Space / (ChildSize.x + Space)) + sizePre0;

            if (normalDir .x < 0)
                index = (int)tempIndex + (oindex > sizePre0 ? 0 : -1);
            else
                index = (int)tempIndex + (oindex < sizePre1 ? 0 : 1);

        }


        if (m_ScrollDir == EScrollDir.Vertical)
        {
            var axisY = normalDir.y < 0 ? mFocusBounds.max.y : mFocusBounds.min.y;

            var offset = axisY - mContentBounds.min.y;

            float tempIndex = Mathf.Abs(offset) / (ChildSize.y + Space);

            float oindex = tempIndex - (int)tempIndex;

            float sizePre0 = (ChildSize.y / (ChildSize.y + Space)) * 0.3f;

            float sizePre1 = (Space / (ChildSize.y + Space)) + sizePre0;


            if (normalDir.y < 0)
                index = (int)tempIndex + (oindex > sizePre0 ? 0 : -1);
            else
                index = (int)tempIndex + (oindex < sizePre1 ? 0 : 1);
        }

        if (index > m_Children.Count)
            index = m_Children.Count;

        return index;
    }
    private Vector2 CalculateOffset(Vector2 delta)
    {
        Vector2 offset = Vector2.zero;

        Vector2 min = mContentBounds.min;
        Vector2 max = mContentBounds.max;

        if (m_ScrollDir == EScrollDir.Horzi)
        {
            min.x += delta.x;
            max.x += delta.x;

            if (min.x > mFocusBounds.min.x)
                offset.x = mFocusBounds.min.x - min.x;
            else if (max.x < mFocusBounds.max.x)
                offset.x = mFocusBounds.max.x - max.x;

        }


        if (m_ScrollDir == EScrollDir.Vertical)
        {
            min.y += delta.y;
            max.y += delta.y;

            if (max.y < mFocusBounds.max.y)
                offset.y = mFocusBounds.max.y - max.y;
            else if (min.y > mFocusBounds.min.y)
                offset.y = mFocusBounds.min.y - min.y;
        }

        return offset;
    }

    private Bounds getFocusBounds()
    {
      //  Vector3[] Corners = new Vector3[4] { };


        var viewBounds = getViewBounds(m_ViewPort);
        var contentBounds = getContentBounds(m_Content);

        var centerX = m_ScrollDir == EScrollDir.Horzi ? (viewBounds.min.x + m_Axis * viewBounds.size.x) : contentBounds.center.x;
        var centerY = m_ScrollDir == EScrollDir.Horzi ? contentBounds.center.y : ( viewBounds.min.y + m_Axis * viewBounds.size.y);


        Bounds bounds = new Bounds(new Vector3(centerX,centerY), m_ChildSize);


        return bounds;

    }

    private Bounds getViewBounds(RectTransform view)
    {
        Bounds bounds = new Bounds(view.rect.center, view.rect.size);

        return bounds;
    }

    private Bounds getContentBounds(RectTransform rectTans)
    {
        Vector3[] viewCorners = new Vector3[4];

        rectTans.GetWorldCorners(viewCorners);

        Vector3 maxValue = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Vector3 minValue = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);


        var tolocal = m_ViewPort.worldToLocalMatrix;

        for (int i = 0; i < 4; i++)
        {
            var value = tolocal.MultiplyPoint3x4(viewCorners[i]);

            maxValue = Vector3.Max(value, maxValue);
            minValue = Vector3.Min(value, minValue);
        }

        Bounds bounds = new Bounds(minValue, Vector3.zero);

        bounds.Encapsulate(maxValue);

        return bounds;
    }


    private void UpdateBounds()
    {
        mViewBounds = getViewBounds(m_ViewPort);

        mContentBounds = getContentBounds(m_Content);


        mFocusBounds = getFocusBounds();
    }

    private void OnDrawGizmos()
    {
       

        //Bounds view = getViewBounds(m_ViewPort);
        //Gizmos.color = Color.black;
        //Gizmos.DrawCube(view.center, view.size);


        //Bounds content = getContentBounds(m_Content);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawCube(content.center, content.size);

        //Gizmos.color = Color.red;
        //Bounds focus = getFocusBounds();
        //Gizmos.DrawCube(focus.center, focus.size);
    }
}
