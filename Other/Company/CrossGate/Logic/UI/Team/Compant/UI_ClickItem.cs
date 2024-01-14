using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;
using Framework;

public class ClickItem
{
    public Transform mTransform { get; set; }

    public virtual void Load(Transform root)
    {
        mTransform = root;
    }

    public virtual void Show()
    {
        mTransform?.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        mTransform?.gameObject.SetActive(false);
    }

    public virtual ClickItem Clone()
    {
        return Clone<ClickItem>(this);
    }

    public bool isActive()
    {
        if (mTransform == null)
            return false;

        return mTransform.gameObject.activeSelf;
    }
    protected static T Clone<T>(T origin) where T : ClickItem, new()
    {
        T item = new T();

        if (origin.mTransform == null)
            return item;

        GameObject go = GameObject.Instantiate<GameObject>(origin.mTransform.gameObject);

        item.Load(go.transform);

        if (origin.mTransform.parent != null)
            go.transform.SetParent(origin.mTransform.parent, false);

        return item;
    }
}

public class IntClickItem : ClickItem
{
    public int Index { get; set; }

    public ClickItemEvent clickItemEvent = new ClickItemEvent();


}
public class ButtonIntClickItem : IntClickItem
{
    public Button Btn { get; set; }
 
    public override void Load(Transform root)
    {
        mTransform = root;

        Btn = mTransform.GetComponent<Button>();

        Btn.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        clickItemEvent?.Invoke(Index);
    }
    public override ClickItem Clone()
    {
        return Clone<ButtonIntClickItem>(this);
    }

}

public class ToggleIntClickItem : IntClickItem
{
    public Toggle Togg { get; set; }

    public override void Load(Transform root)
    {
        mTransform = root;

        Togg = mTransform.GetComponent<Toggle>();

        Togg.onValueChanged.AddListener(OnClick);
    }

    protected virtual void OnClick(bool b)
    {
        if(b)
            clickItemEvent?.Invoke(Index);

    }
    public override ClickItem Clone()
    {
        return Clone<ToggleIntClickItem>(this);
    }

}

public class CPToggleIntClickItem : IntClickItem
{
    public CP_Toggle Togg { get; set; }

    public override void Load(Transform root)
    {
        mTransform = root;

        Togg = mTransform.GetComponent<CP_Toggle>();

        Togg.onValueChanged.AddListener(OnClick);
    }

    protected virtual void OnClick(bool b)
    {
        if (b)
            clickItemEvent?.Invoke(Index);

    }
    public override ClickItem Clone()
    {
        return Clone<CPToggleIntClickItem>(this);
    }

}


public class RewardPropItem : ClickItem
{
    public PropItem m_Item;
    public PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);

    public EUIID m_SourceUiId = EUIID.Invalid;

    public override void Load(Transform transform)
    {
        base.Load(transform);

        m_Item = new PropItem();

        m_Item.BindGameObject(transform.gameObject);
    }

    public void SetItem(uint id, uint count)
    {
        m_ItemData.id = id;
        m_ItemData.count = count;
        m_ItemData.SetQuality(0);
        m_Item.SetData(new MessageBoxEvt() { sourceUiId = m_SourceUiId, itemData = m_ItemData });
    }

    public override ClickItem Clone()
    {
        return Clone<RewardPropItem>(this);
    }
}