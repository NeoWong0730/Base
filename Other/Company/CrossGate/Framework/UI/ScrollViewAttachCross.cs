using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using Lib.Core;

public class ScrollViewAttachCross : MonoBehaviour,IEndDragHandler,IDragHandler
{
    private GameObject scrollviewRoot;
    private RectTransform content;
    public GridLayoutGroup group;
    public float objectDistance = 25;
    public float betweenObjectLerpScale = 0.15f;
    private GameObject child;
    private float staticSizedeltaX;
    private int curAttachIndex;
    private float cellsize;
    private int paddingLeft;
    private bool bValid;
    private Action<int> onSelect;
    
    public int childCount
    {
        get
        {
            return content.childCount;
        }
    }

    public GameObject GetChild(int i)
    {
        return content.GetChild(i).gameObject;
    }

    public ScrollViewAttachCross BindGameObject(GameObject scrollview)
    {
        scrollviewRoot = scrollview;
        ParseComponent();
        return this;
    }
    
    public void BindEventListener(Action<int> action)
    {
        onSelect = action;
    }

    private void ParseComponent()
    {
        content = transform.Find("Viewport/Content").GetComponent<RectTransform>();
        group = content.GetComponent<GridLayoutGroup>();
        child = transform.Find("Viewport").GetChild(0).gameObject;
        child.SetActive(false);
        cellsize = (int)group.cellSize.x;
        paddingLeft = group.padding.left;
        staticSizedeltaX = content.anchoredPosition.x;
    }

   
    public void SetData(int dataCount)
    {
        ClearChildern();

        SetContentLength(dataCount);
        minPosY = -(group.padding.top + cellsize / 2);
        for (int i = 0; i < dataCount; i++)
        {
            GameObject go = GameObject.Instantiate<GameObject>(child);
            go.transform.SetParent(content);
            go.transform.localPosition = new Vector3(leftP + (cellsize / 2) * (2 * i + 1) + (i + 1) * group.spacing.x, minPosY, 0);
            go.name = child.name;
            go.SetActive(true);
        }
        
        for (int i = 0; i < content.childCount; i++)
        {
            GameObject go = content.GetChild(i).gameObject;
            go.AddComponent<CP_PositonFlag>().SetPosition(i);
            go.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { OnItemClicked(go.GetComponent<CP_PositonFlag>().Position); });
        }
        content.transform.localPosition = new Vector2(staticSizedeltaX, content.transform.localPosition.y);
    }

    float leftP;
    float minPosY;
    private void SetContentLength(int dataCount)
    {
        int childChildCount = dataCount;
        leftP = (int)Mathf.Abs(staticSizedeltaX) - (int)cellsize / 2;
        Vector2 sizedelta = new Vector2(childChildCount * cellsize + leftP * 2 + (childChildCount - 1) * group.spacing.x + group.spacing.x, content.sizeDelta.y);
        content.sizeDelta = sizedelta;
    }

    private void ClearChildern()
    {
        FrameworkTool.DestroyChildren(content.gameObject);
    }


    private void OnItemClicked( int positionId)
    {
        for (int i = 0; i < content.childCount; i++)
        {
            if (i==positionId)
            {
                Attach(i);
                return;
            }
        }
    }

    public void Attach(int _attachindex)
    {
        Vector2 toPos = toPos = new Vector2(-(_attachindex * (group.spacing.x + cellsize) + cellsize / 2 + leftP), content.anchoredPosition.y); 
        content.DOAnchorPos(toPos,0.3f);
        for (int i = 0; i < content.childCount; i++)
        {
            if(objectDistance != 0)
                content.GetChild(i).localPosition = new Vector3(content.GetChild(i).localPosition.x, minPosY + Mathf.Abs(i - _attachindex) * objectDistance, 0);
            if (i == _attachindex)
            {
                content.GetChild(i).localScale = Vector3.one;
                
            }
            else
            {
                float scale = 1 - Mathf.Abs(_attachindex - i) * betweenObjectLerpScale;
                float scale_sub = scale > 0.5f ? scale : 0.5f;
                content.GetChild(i).localScale = new Vector3(scale_sub, scale_sub, scale_sub);
            }
                
        }
        onSelect?.Invoke(_attachindex);
    }

    //float pressX = -1;
    public void OnDrag(PointerEventData eventData)
    {
        /*if(pressX == -1)
        {
            pressX = eventData.pressPosition.x;
        }
        float drawX = eventData.position.x;
        float leftV = Mathf.Abs(pressX - drawX);
        float left = pressX - drawX > 0 ? -1 : 1;
        pressX = drawX;*/

        float _x = content.anchoredPosition.x;
        float _xoffest = Mathf.Abs(_x) - (leftP - group.spacing.x / 2);
        curAttachIndex = (int)_xoffest / (100 + (int)group.spacing.x);
        for (int i = 0; i < content.childCount; i++)
        {
            if (objectDistance != 0)
                content.GetChild(i).localPosition = new Vector3(content.GetChild(i).localPosition.x, minPosY + Mathf.Abs(i - curAttachIndex) * objectDistance, 0);
            //content.GetChild(i).localPosition = new Vector3(content.GetChild(i).localPosition.x, minPosY  + Mathf.Abs(i - curAttachIndex) * 15 + (i - curAttachIndex) * left * 5f, 0);
            if (i == curAttachIndex)
            {
                content.GetChild(i).localScale = Vector3.one;
            }
            else
            {
                float scale = 1 - Mathf.Abs(curAttachIndex - i) * betweenObjectLerpScale;
                float scale_sub = scale > 0.5f ? scale : 0.5f;
                content.GetChild(i).localScale = new Vector3(scale_sub, scale_sub, scale_sub);
                
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //pressX = -1;
        float _x = content.anchoredPosition.x;
        float _xoffest = Mathf.Abs(_x) - (leftP - group.spacing.x / 2);
        curAttachIndex = (int)_xoffest / (100 + (int)group.spacing.x);
        Attach(curAttachIndex);
    }

    public void SetActive(bool active)
    {
        scrollviewRoot.SetActive(active);
        bValid = active;
        if (active)
        {
            Attach(0);
        }
    }
}
