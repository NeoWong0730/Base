using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Lib.Core.EventTrigger;

public class DropdownEx : Dropdown
{
    public delegate void VoidDelegate(BaseEventData eventData);
    public VoidDelegate onClickItem;

    public class DropdownShowEvent : UnityEvent<bool> { }

    public DropdownShowEvent m_OnShowDropListEvent = new DropdownShowEvent();

    public DropdownShowEvent onShowListEvent
    {
        get { return m_OnShowDropListEvent; }
        set { m_OnShowDropListEvent = value; }
    }
    protected override DropdownItem CreateItem(DropdownItem itemTemplate)
    {
        DropdownItem dropdownItem = base.CreateItem(itemTemplate);

        Lib.Core.EventTrigger.Get(dropdownItem.gameObject).AddEventListener(EventTriggerType.PointerClick, OnClickItem);

        return dropdownItem;
    }

    public void OnClickItem(BaseEventData eventData)
    {
        if (onClickItem != null) { onClickItem(eventData); }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        onShowListEvent.Invoke(true);

    }

    public override void OnCancel(BaseEventData eventData)
    {
        base.OnCancel(eventData);
        onShowListEvent.Invoke(false);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        onShowListEvent.Invoke(true);
    }

    protected override void DestroyBlocker(GameObject blocker)
    {
        base.DestroyBlocker(blocker);
        onShowListEvent.Invoke(false);
    }

    protected override GameObject CreateDropdownList(GameObject template)
    {
        return base.CreateDropdownList(template);
    }
}
