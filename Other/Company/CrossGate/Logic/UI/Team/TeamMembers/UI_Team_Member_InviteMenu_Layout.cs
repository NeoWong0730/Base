using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using Framework;

#region class menuItem
class MenuItem
{
    public int Index { get; set; }

    public Transform transform;
    public Button m_Btn;
    protected Text m_text;

    public ClickItemEvent clickItemEvent = new ClickItemEvent();

    protected uint m_TexID;
    public virtual uint TextString { get { return m_TexID; }
        set { if (m_TexID == value)
                return;
            m_TexID = value;
            TextHelper.SetText(m_text,m_TexID); } }

    public virtual void Load(Transform tf)
    {
        transform = tf;

        m_Btn = transform.GetComponent<Button>();

        m_text = transform.Find("Text").GetComponent<Text>();

        m_Btn.onClick.AddListener(OnClick);
    }

    protected virtual void OnClick()
    {
        clickItemEvent.Invoke(Index);
    }

}

#endregion
// 菜单

public partial class UI_Team_Member_Layout
{
    private Transform mInviteMenuTransform;

    private uint[] InviteMenuStrings = { 2002051, 2002052, 11763, 2002054 };

    public Action<int> OnClickInviteMenu;
    class InviteMenuItem:MenuItem
    {
        public override void Load(Transform tf)
        {
            base.Load(tf);
        }
    }

    private List<InviteMenuItem> inviteMenuItems = new List<InviteMenuItem>();
    private void LoadInviteMenu(Transform root)
    {
        mInviteMenuTransform = root.Find("Animator/View_Team/Button_Grid");

        Transform tf = mInviteMenuTransform.Find("Grid_Button");

        int itemCount = tf.childCount;

        for (int i = 0; i < itemCount; i++)
        {
           Transform item = tf.Find("Button0" + (i + 1).ToString());

            if (item == null)
                continue;

           InviteMenuItem imi = new InviteMenuItem();

            imi.Load(item);

            imi.Index = i;

            imi.TextString = InviteMenuStrings[i];

            imi.clickItemEvent.AddListener(OnClickInviteItem);

            inviteMenuItems.Add(imi);
        }


        Button closeBtn = mInviteMenuTransform.GetComponent<Button>();

        closeBtn.onClick.AddListener(OnClickInviteClose);
    }

    private void OnClickInviteItem(int index)
    {
        OnClickInviteMenu?.Invoke(index);
    }

    public void ActiveInviteMenu(bool active)
    {
        mInviteMenuTransform.gameObject.SetActive(active);
    }

    private void OnClickInviteClose()
    {
        ActiveInviteMenu(false);
    }
}
