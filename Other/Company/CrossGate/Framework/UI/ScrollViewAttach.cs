using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class CP_PositonFlag : MonoBehaviour
{
    public int Position { get; private set; }

    public void SetPosition(int pos)
    {
        Position = pos;
    }
}


public class ScrollViewAttach : MonoBehaviour, IEndDragHandler, IDragHandler
{
    private GameObject scrollviewRoot;
    private RectTransform content;
    private GridLayoutGroup group;
    private GameObject child;

    private float cellsize;
    public float maxScale = 1;
    public float minScale = 0.75f;
    private Action<int> onSelect;
    private Action<int> onCreate;
    private Action onDragLeft;
    private Action onDargRight;

    private bool bVaild = true;
    public int oldSelectIndex = -1;
    private int curAttachIndex;


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

    public ScrollViewAttach BindGameObject(GameObject scrollview)
    {
        scrollviewRoot = scrollview;
        ParseComponent();
        return this;
    }

    public void BindEventListener(Action<int> _onSelected, Action<int> _onCreated = null, Action _onDragLeft = null, Action _onDragRight = null)
    {
        onSelect = _onSelected;
        onCreate = _onCreated;
        onDragLeft = _onDragLeft;
        onDargRight = _onDragRight;
    }

    private void ParseComponent()
    {
        content = transform.Find("Viewport/Content").GetComponent<RectTransform>();
        group = content.GetComponent<GridLayoutGroup>();
        child = content.GetChild(0).gameObject;
        cellsize = (int)group.cellSize.x;
    }

    public ScrollViewAttach SetVaild(bool vaild)
    {
        bVaild = vaild;
        return this;
    }


    public void SetScale(float _maxScale, float _minScale)
    {
        maxScale = _maxScale;
        minScale = _minScale;
    }

    public void SetData(int dataCount)
    {
        if (onCreate != null)
        {
            onCreate.Invoke(0);
        }
        for (int i = 1; i < dataCount; i++)
        {
            GameObject go = GameObject.Instantiate<GameObject>(child);
            go.transform.SetParent(content);
            go.transform.localPosition = new Vector3(go.transform.position.x, go.transform.position.y, 0);
            go.transform.localScale = Vector3.one;
            if (onCreate != null)
            {
                onCreate.Invoke(i);
            }
        }
        if (bVaild)
        {
            SetContentLength();
        }
        for (int i = 0; i < content.childCount; i++)
        {
            GameObject go = content.GetChild(i).gameObject;
            go.AddComponent<CP_PositonFlag>().SetPosition(i);

            go.name = i.ToString();

            go.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { OnItemClicked(go.GetComponent<CP_PositonFlag>().Position); });
        }
    }


    private void OnItemClicked(int positionId)
    {
        for (int i = 0; i < content.childCount; i++)
        {
            if (i == positionId)
            {
                Attach(i, false);
                return;
            }
        }
    }

    private void SetContentLength()
    {
        int childChildCount = content.childCount;
        group.padding.left += (int)Mathf.Abs(content.anchoredPosition.x) - group.padding.left - (int)cellsize / 2;
        Vector2 sizedelta = new Vector2(childChildCount * cellsize + group.padding.left * 2 + (childChildCount - 1) * group.spacing.x, content.sizeDelta.y);
        content.sizeDelta = sizedelta;
    }

    public void Attach(int _attachindex, bool bForceTriggerEvent = true)
    {
        if (bVaild)
        {
            Vector2 toPos = new Vector2(-(_attachindex * (group.spacing.x + cellsize) + cellsize / 2 + group.padding.left), content.anchoredPosition.y);
            content.DOAnchorPos(toPos, 0.3f);
            for (int i = 0; i < content.childCount; i++)
            {
                if (i == _attachindex)
                    content.GetChild(i).localScale = new Vector3(maxScale, maxScale, maxScale);

                else
                    content.GetChild(i).localScale = new Vector3(minScale, minScale, minScale);
            }
        }
        if (oldSelectIndex != _attachindex)
        {
            onSelect?.Invoke(_attachindex);
            oldSelectIndex = _attachindex;
        }
        else if (bForceTriggerEvent)
        {
            onSelect?.Invoke(_attachindex);
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (bVaild)
        {
            float _x = content.anchoredPosition.x;
            float _xoffest = Mathf.Abs(_x) - (group.padding.left - group.spacing.x / 2);
            curAttachIndex = (int)_xoffest / ((int)cellsize + (int)group.spacing.x);
            for (int i = 0; i < content.childCount; i++)
            {
                if (i == curAttachIndex)
                {
                    content.GetChild(i).localScale = new Vector3(maxScale, maxScale, maxScale);
                }
                else
                {
                    content.GetChild(i).localScale = new Vector3(minScale, minScale, minScale);
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (bVaild)
        {
            float _x = content.anchoredPosition.x;
            float _xoffest = Mathf.Abs(_x) - (group.padding.left - group.spacing.x / 2);
            curAttachIndex = (int)_xoffest / ((int)cellsize + (int)group.spacing.x);

            if (oldSelectIndex > curAttachIndex)
            {
                onDargRight?.Invoke();
            }
            else if (oldSelectIndex < curAttachIndex)
            {
                onDragLeft?.Invoke();
            }
            oldSelectIndex = curAttachIndex;
            Attach(curAttachIndex);
        }
    }

    public void SetActive(bool active, int index)
    {
        scrollviewRoot.SetActive(active);
        if (active)
        {
            Attach(index);
        }
    }
}
