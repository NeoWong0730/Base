using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using System;
using Framework;

// 菜单
// 点击人物控件 跳出菜单
// 发送消息，调整站位， 加为好友，委托指挥，升为队长，请离队伍
public partial class UI_Team_Member_Layout
{
    private Transform mItemMenuTransform;

    private RectTransform mMemberInfoMenu;

    private uint[] ItemMenuCommandArray = { 2002111 , 2002112, 2002113,
                                            2002114, 2002115, 2002116,
                                            2002098};

    public enum ECommandType
    {
        SendMsg = 0,//发送消息
        SetPositionOrder = 1,//调整站位
        AddFriend = 2,// 加为好友
        Delegate = 3,//委托指挥
        ToCapatain  =4,//升为队长
        RejectTeam = 5,//请离队伍
        CallBlack = 6,//召回队伍
    }
    class ItemMenuItem : MenuItem
    {

    }

    private List<ItemMenuItem> m_itemMenuItems = new List<ItemMenuItem>();

    // public Action<int> OnClickItemMenu;

    public ClickItemEvent OnClickItemMenu = new ClickItemEvent();
    private void LoadItemMenu(Transform root)
    {
        mItemMenuTransform = root.Find("Animator/View_Team/Button_Grid_menu");

        Button btn = mItemMenuTransform.GetComponent<Button>();
        btn.onClick.AddListener(OnClickClose);

        Transform menuTrans = mItemMenuTransform.Find("menus");

        mMemberInfoMenu = menuTrans as RectTransform;

        int itemcount = menuTrans.childCount;
        for (int i = 0; i < itemcount; i++)
        {
            Transform transform = menuTrans.Find("Button0" + (i + 1).ToString());

            if (transform == null)
                continue;

            ItemMenuItem itemMenuItem = new ItemMenuItem();
            itemMenuItem.Load(transform);

            itemMenuItem.Index = i;

            itemMenuItem.TextString = ItemMenuCommandArray[i];

            itemMenuItem.clickItemEvent.AddListener(OnClickItemMenuItem);

            m_itemMenuItems.Add(itemMenuItem);

            //if (i == 1)
            //    transform.gameObject.SetActive(false);
        }
    }

    public void SetCommand(ECommandType type, uint langue)
    {
        int index = (int)type;

        m_itemMenuItems[index].TextString = langue;
    }

    public void SetCommandState(ECommandType type, bool state)
    {
        int index = (int)type;

        // m_itemMenuItems[index].m_Btn.interactable = state;

        ButtonHelper.Enable(m_itemMenuItems[index].m_Btn, state);
    }
    private void OnClickItemMenuItem(int index)
    {
        OnClickItemMenu?.Invoke(index);
    }

    private void OnClickClose()
    {
        //ActiveItemMenu(false);

        m_listener?.CloseItemMenu();
    }
    public void ActiveItemMenu(bool b)
    {
        mItemMenuTransform.gameObject.SetActive(b);
    }

    public void SetInfoMenuPosition(Vector3 pos)
    {
        mMemberInfoMenu.position = mMemberInfoMenu.position + pos;
    }

    public void getInfoMenuWorldCorners(Vector3[] vectors)
    {
        mMemberInfoMenu.GetWorldCorners(vectors);
    }

    //点击队长显示的菜单按钮
    public void SetModeCpataion()
    {
        if (m_itemMenuItems.Count == 0)
            return;

        //m_itemMenuItems[1].m_Btn.interactable = true;
        //m_itemMenuItems[2].m_Btn.interactable = true;
        //m_itemMenuItems[3].m_Btn.interactable = true;
        //m_itemMenuItems[4].m_Btn.interactable = true;
        //m_itemMenuItems[5].m_Btn.interactable = true;
        //m_itemMenuItems[6].m_Btn.interactable = true;

        ButtonHelper.Enable(m_itemMenuItems[1].m_Btn, true);
        ButtonHelper.Enable(m_itemMenuItems[2].m_Btn, true);
        ButtonHelper.Enable(m_itemMenuItems[3].m_Btn, true);
        ButtonHelper.Enable(m_itemMenuItems[4].m_Btn, true);
        ButtonHelper.Enable(m_itemMenuItems[5].m_Btn, true);
        ButtonHelper.Enable(m_itemMenuItems[6].m_Btn, true);
    }

    //点击成员显示的菜单按钮
    public void SetModeMember()
    {
        if (m_itemMenuItems.Count == 0)
            return;

        ButtonHelper.Enable(m_itemMenuItems[1].m_Btn, false);
        ButtonHelper.Enable(m_itemMenuItems[2].m_Btn, false);
        ButtonHelper.Enable(m_itemMenuItems[3].m_Btn, false);
        ButtonHelper.Enable(m_itemMenuItems[4].m_Btn, false);
        ButtonHelper.Enable(m_itemMenuItems[5].m_Btn, false);
        ButtonHelper.Enable(m_itemMenuItems[6].m_Btn, false);

    }
}
