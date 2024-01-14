using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using Lib.Core;

public  class ClickItemGroup<T> where T : ClickItem,new()
{
    public List<T> items = new List<T>();

    private Action<T> mAddChildAction;

    /// <summary>
    /// 容器中的总数量
    /// </summary>
    public int Size { get { return items.Count; } }

    /// <summary>
    /// 当前显示的数量
    /// </summary>
    public int Count { get; set; }

    public T OriginItem { get; set; }

    /// <summary>
    /// 自动复制扩展,OriginItem 不为null时复制，否则复制items[0]
    /// </summary>
    public bool AutoClone { get; set; } = true;
    public void SetAddChildListenter(Action<T> action)
    {
        mAddChildAction = action;

        for (int i = 0; i < items.Count; i++)
        {
            mAddChildAction?.Invoke(items[i]);
        }
    }

    public void ClearAddChildListenter()
    {
        mAddChildAction = null;
    }
    public ClickItemGroup(T obj)
    {
        items.Add(obj);
    }

    public ClickItemGroup(Transform item)
    {
        T value = new T();

        value.Load(item);

        items.Add(value);
    }

    public ClickItemGroup()
    {
    }

    private void UpdateChildSize(int count)
    {
        if (!AutoClone || items.Count >= count)
            return;

        int offset = count - items.Count;

        var orgin = OriginItem == null ? (items.Count == 0 ? null : items[0]) : OriginItem;

        if (orgin == null)
        {
            DebugUtil.LogError("clone orgin is null");
            return;
        }

        for (int i = 0; i < offset; i++)
        {
            T item = orgin.Clone() as T;

            if (item != null)
            {
                items.Add(item);
                mAddChildAction?.Invoke(item);
            }

        }
    }
    public virtual void SetChildSize(int count)
    {
        UpdateChildSize(count);

        Count = items.Count < count ? items.Count : count;

        RefreshAction();
    }

    public void AddChild(T item)
    {
        if (item == null)
            return;

        items.Add(item);
        mAddChildAction?.Invoke(item);
    }

    public void AddChild(Transform transform)
    {
        if (transform == null)
            return;

        T value = new T();

        value.Load(transform);

        AddChild(value);
    }
    private void RefreshAction()
    {
        for (int i = 0; i < items.Count; i++)
        {
            var item = getAt(i);

            if (item == null)
                continue;

            if (i < Count)
                item.Show();
            else
                item.Hide();     

        }
    }

    public virtual T getAt(int index)
    {
        if (index >= items.Count)
            return null;

        return items[index];
    }

    public void SetOriginItem(Transform item)
    {
        T value = new T();

        value.Load(item);

        item.gameObject.SetActive(false);

        OriginItem = value;
    }
}


//public class ButtonClickItemGroup<T> : ClickItemGroup<T> where T : ButtonIntClickItem
//{
//    public ButtonClickItemGroup(T obj) : base(obj)
//    {
//    }
//    public override void SetChildSize(int count)
//    {
//        base.SetChildSize(count);
//    }


//}


