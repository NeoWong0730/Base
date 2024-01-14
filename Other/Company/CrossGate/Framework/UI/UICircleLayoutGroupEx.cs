using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class UICircleLayoutGroupEx : UIBehaviour, ILayoutElement, ILayoutGroup, ILayoutController
{
    public enum EAnchor
    {
        Middle,
        Left,
        Right
    }
    [SerializeField]
    private int m_Radius = 20;

    [SerializeField,Range(0,1)]
    private float m_Sapce = 0.1f;

    [SerializeField,Range(0, 1)]
    private float m_StartAxis = 0.5f;

    [SerializeField]
    private EAnchor m_Anchor = EAnchor.Middle;
    public RectTransform rectTransform { get { return transform as RectTransform; } }

    public float minWidth { get { return rectTransform.rect.width; } }

    public float preferredWidth { get { return rectTransform.rect.width; } }

    public float flexibleWidth { get { return rectTransform.rect.width; } }

    public float minHeight { get { return rectTransform.rect.height; } }

    public float preferredHeight { get { return rectTransform.rect.height; } }

    public float flexibleHeight { get { return rectTransform.rect.height; } }

    public int layoutPriority { get { return 0; } }


    [System.NonSerialized] private List<RectTransform> m_RectChildren = new List<RectTransform>();
    protected List<RectTransform> rectChildren { get { return m_RectChildren; } }

    
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public  void CalculateLayoutInputHorizontal()
    {
        m_RectChildren.Clear();
        var toIgnoreList = ListPool<Component>.Get();
        for (int i = 0; i < rectTransform.childCount; i++)
        {
            var rect = rectTransform.GetChild(i) as RectTransform;
            if (rect == null || !rect.gameObject.activeInHierarchy)
                continue;

            rect.GetComponents(typeof(ILayoutIgnorer), toIgnoreList);

            if (toIgnoreList.Count == 0)
            {
                m_RectChildren.Add(rect);
                continue;
            }

            for (int j = 0; j < toIgnoreList.Count; j++)
            {
                var ignorer = (ILayoutIgnorer)toIgnoreList[j];
                if (!ignorer.ignoreLayout)
                {
                    m_RectChildren.Add(rect);
                    break;
                }
            }
        }

        ListPool<Component>.Release(toIgnoreList);
    }

    public  void CalculateLayoutInputVertical()
    {
        Calcal();
    }

    public  void SetLayoutHorizontal()
    {
        
    }

    public  void SetLayoutVertical()
    {
      
    }

    private void Calcal()
    {
        Vector3 pos = Vector3.zero;
        Vector3 starVec = Vector3.up * m_Radius + pos;

        int childrenCount = rectChildren.Count;

        float offset = getOffset();

        for (int i = 0; i < childrenCount; i++)
        {

            float angle = i * (m_Sapce * 360) + m_StartAxis * 360 + offset;

            Quaternion qua = Quaternion.AngleAxis(angle, Vector3.back);

            Vector3 point = qua* starVec;

            rectChildren[i].anchoredPosition = point;

            
        }

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rectChildren[i].anchorMin = new Vector2(0.5f, 0.5f);

            rectChildren[i].anchorMax = new Vector2(0.5f, 0.5f);

        }


    }

    private float getOffset()
    {
        int childrenCount = rectChildren.Count;

        return getOffset(childrenCount);
    }

    private float getOffset(int childrenCount)
    {
        float offset = 0;

        float totalAngle = (m_Sapce * 360) * (childrenCount - 1);

        if (m_Anchor == EAnchor.Middle)
        {
            offset = -totalAngle / 2;
        }

        if (m_Anchor == EAnchor.Left)
        {
            offset = -totalAngle;
        }
        return offset;
    }
    public Vector3 GetChildPosition(int index,int maxCount)
    {
        Vector3 pos = Vector3.zero;
        Vector3 starVec = Vector3.up * m_Radius + pos;

        float offset = getOffset(maxCount);

        float angle = index * (m_Sapce * 360) + m_StartAxis * 360 + offset;

        Quaternion qua = Quaternion.AngleAxis(angle, Vector3.back);

        Vector3 point = qua * starVec;

        return point;
    }
}
