//this file is auto created by QuickCode,you can edit it 
//do not need to care initialization of ui widget any more 
//------------------------------------------------------------
/*
*   @Author:冰块网络
*   DateTime:2019/11/20 14:49:33
*   Purpose:UI Componments Data Binding
*/
//-------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UI_TaskShareList_Layout
{
	public GameObject mRoot { get; private set; }
	public Transform mTrans { get; private set; }

	public Transform tabProto { get; private set; }
	public Transform awardProto { get; private set; }
    public Transform NoReward { get; private set; }
    public Transform HaveReward { get; private set; }
    public Transform HaveRewardParent { get; private set; }

    public Text TaskContent_RectTransform1 { get; private set; }
    public Text TaskContent_RectTransform2 { get; private set; }
    public Text TaskContent_RectTransform3 { get; private set; }

    public Text shareOwnDesc { get; private set; }
    public Text taskTitle { get; private set; }
    public Text taskDesc { get; private set; }

    public Button btnReject { get; private set; }
    public Button btnAccepet { get; private set; }
    public Button btnClose { get; private set; }

    public void Parse(GameObject root) 
	{
		mRoot = root;
		mTrans = root.transform;

        btnAccepet = mTrans.Find("Animator/View_Right_Content/Button_Receive").GetComponent<Button>();
        btnReject = mTrans.Find("Animator/View_Right_Content/Button_Refuse").GetComponent<Button>();
        btnClose = mTrans.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
        tabProto = mTrans.Find("Animator/View_Left_List/Scroll_View/TabList/ListItem");
        awardProto = mTrans.Find("Animator/View_Right_Content/Text_Award/Node/Item");
        NoReward = mTrans.Find("Animator/View_Right_Content/Image_Award/No_Award");
        HaveReward = mTrans.Find("Animator/View_Right_Content/Image_Award/Scroll_View");
        HaveRewardParent = mTrans.Find("Animator/View_Right_Content/Image_Award/Scroll_View/Viewport");

        TaskContent_RectTransform1 = mTrans.Find("Animator/View_Right_Content/Text_Target/GameObject/TaskContent1").GetComponent<Text>();
        TaskContent_RectTransform2 = mTrans.Find("Animator/View_Right_Content/Text_Target/GameObject/TaskContent2").GetComponent<Text>();
        TaskContent_RectTransform3 = mTrans.Find("Animator/View_Right_Content/Text_Target/GameObject/TaskContent3").GetComponent<Text>();

        shareOwnDesc = mTrans.Find("Animator/View_Left_List/Sender/Text").GetComponent<Text>();
        taskTitle = mTrans.Find("Animator/View_Right_Content/Text_Name/TaskTitle").GetComponent<Text>();
        taskDesc = mTrans.Find("Animator/View_Right_Content/Text_Describe/TaskDesc").GetComponent<Text>();
    }

	public void RegisterEvents(IListener listener)
	{
        btnClose.onClick.AddListener(listener.OnBtnCloseClicked);
        btnReject.onClick.AddListener(listener.OnBtnRejectClicked);
        btnAccepet.onClick.AddListener(listener.OnBtnAccepetClicked);
    }

	public interface IListener
	{
        void OnBtnCloseClicked();
        void OnBtnRejectClicked();
        void OnBtnAccepetClicked();
    }
}

