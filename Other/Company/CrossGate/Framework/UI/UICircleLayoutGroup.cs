using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UICircleLayoutGroup :LayoutGroup
{

    public int m_Radius = 20;

    public float m_Sapce = 10;

    public float m_StartAxis = 0.5f;

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

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
    }

    public override void CalculateLayoutInputVertical()
    {
        Calcal();
    }

    public override void SetLayoutHorizontal()
    {
        
    }

    public override void SetLayoutVertical()
    {
      
    }

    private void Calcal()
    {
        Vector3 pos = Vector3.zero;
        Vector3 starVec = Vector3.up * m_Radius + pos;


        
        for (int i = 0; i < rectChildren.Count; i++)
        {
           

            Quaternion qua = Quaternion.AngleAxis(i * m_Sapce + m_StartAxis * 360, Vector3.back);

            Vector3 point = qua* starVec;

            rectChildren[i].anchoredPosition = point;

            
        }

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rectChildren[i].anchorMin = new Vector2(0.5f, 0.5f);

            rectChildren[i].anchorMax = new Vector2(0.5f, 0.5f);

        }


    }
}
