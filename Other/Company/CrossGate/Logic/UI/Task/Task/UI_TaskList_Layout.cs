//this file is auto created by QuickCode,you can edit it
//do not need to care initialization of ui widget any more
//------------------------------------------------------------
/*
*   @Author:冰块网络
*   DateTime:2019/11/20 12:09:48
*   Purpose:UI Componments Data Binding
*/
//-------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Logic {
    public class UI_TaskList_Layout {
        #region UI Variable Statement

        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public RectTransform tabItemProto { get; private set; }
        public Toggle toggleTitle { get; private set; }
        public Text TaskTitle { get; private set; }
        public Text TaskState { get; private set; }
        public Text TaskDesc { get; private set; }
        public GameObject TaskGoalContent { get; private set; }
        public RectTransform AwardItemParent { get; private set; }
        public RectTransform ListItem { get; private set; }
        public Button btnGoto { get; private set; }
        public Button btnWatch { get; private set; }
        public Button btnShare { get; private set; }
        public Button btnGiveUp { get; private set; }
        public Text BtnGiveUpText { get; private set; }
        public Button BtnClose { get; private set; }
        public Button BtnTrace { get; private set; }
        public RectTransform NoReward { get; private set; }
        public RectTransform RewardItemProto { get; private set; }

        #endregion

        public void Parse(GameObject root) {
            mRoot = root;
            mTrans = root.transform;

            tabItemProto = mTrans.Find("Animator/View_LeftTabs/Scroll View/TabList/TabItem").GetComponent<RectTransform>();
            toggleTitle = mTrans.Find("Animator/View_Middle_List/Image_Title/Toggle_Title").GetComponent<Toggle>();
            BtnTrace = mTrans.Find("Animator/View_Middle_List/Image_Title/Toggle_Title/Background").GetComponent<Button>();
            TaskTitle = mTrans.Find("Animator/View_Right_Content/Text_Name/TaskTitle").GetComponent<Text>();
            TaskState = mTrans.Find("Animator/View_Right_Content/Text_Name/TaskState").GetComponent<Text>();
            TaskDesc = mTrans.Find("Animator/View_Right_Content/Text_Describe/TaskDesc").GetComponent<Text>();
            TaskGoalContent = mTrans.Find("Animator/View_Right_Content/Text_Target/Scroll View/GameObject/TaskContent").gameObject;
            AwardItemParent = mTrans.Find("Animator/View_Right_Content/Image_Award/Scroll_View/Viewport").GetComponent<RectTransform>();
            ListItem = mTrans.Find("Animator/View_Middle_List/Scroll_View/TabList/ListItem").GetComponent<RectTransform>();
            btnGoto = mTrans.Find("Animator/View_Right_Content/Button_Goto").GetComponent<Button>();
            btnWatch = mTrans.Find("Animator/View_Right_Content/Button_Watch").GetComponent<Button>();
            btnShare = mTrans.Find("Animator/View_Right_Content/Button_Share").GetComponent<Button>();
            btnGiveUp = mTrans.Find("Animator/View_Right_Content/Button_Give_Up").GetComponent<Button>();
            BtnGiveUpText = mTrans.Find("Animator/View_Right_Content/Button_Give_Up/Text").GetComponent<Text>();
            BtnClose = mTrans.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            NoReward = mTrans.Find("Animator/View_Right_Content/Image_Award/No_Award").GetComponent<RectTransform>();
            RewardItemProto = mTrans.Find("Animator/View_Right_Content/Text_Award/Node/Item").GetComponent<RectTransform>();
        }

        public void RegisterEvents(IListener listener) {
            btnGoto.onClick.AddListener(listener.OnButton_Goto_RectTransform);
            btnWatch.onClick.AddListener(listener.OnButtonWatch_RectTransform);
            btnShare.onClick.AddListener(listener.OnButton_Share_RectTransform);
            btnGiveUp.onClick.AddListener(listener.OnButton_Give_Up_RectTransform);
            BtnClose.onClick.AddListener(listener.OnButton_Close_RectTransform);
            BtnTrace.onClick.AddListener(listener.OnButton_Trace_RectTransform);
        }

        public interface IListener {
            void OnButton_Goto_RectTransform();
            void OnButtonWatch_RectTransform();
            void OnButton_Share_RectTransform();
            void OnButton_Give_Up_RectTransform();
            void OnButton_Close_RectTransform();
            void OnButton_Trace_RectTransform();
        }
    }
}