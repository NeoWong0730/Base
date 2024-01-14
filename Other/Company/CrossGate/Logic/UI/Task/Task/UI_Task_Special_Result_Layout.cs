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

namespace Logic {
    public class UI_Task_Special_Result_Layout {
        #region UI Variable Statement

        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public Text TaskName { get; private set; }
        public Text TaskContent { get; private set; }
        public Transform AwardItemProto { get; private set; }
        public Transform RewardItemProtoParent { get; private set; }
        public Button Image_BG_RectTransform { get; private set; }
        public GameObject loveGo;
        public GameObject challengeGo;
        public GameObject rewardTitle;

        public GameObject loveIcon { get; private set; }
        public GameObject challengeIcon { get; private set; }

        #endregion

        public void Parse(GameObject root) {
            mRoot = root;
            mTrans = root.transform;
            TaskName = mTrans.Find("Animator/View_Right/Image_Title/Text_Title").GetComponent<Text>();
            TaskContent = mTrans.Find("Animator/View_Right/Image_Title/Text_Message").GetComponent<Text>();

            loveIcon = mTrans.Find("Animator/View_Left/Image_Icon").gameObject;
            challengeIcon = mTrans.Find("Animator/View_Left/Image_Icon1").gameObject;

            AwardItemProto = mTrans.Find("Animator/View_Right/Grid_Award/AwardItem");
            RewardItemProtoParent = mTrans.Find("Animator/View_Right/Scroll_View/Viewport");
            Image_BG_RectTransform = mTrans.Find("Animator/Image_BG").GetComponent<Button>();
            loveGo = mTrans.Find("Animator/View_Left/Image_Title01").gameObject;
            challengeGo = mTrans.Find("Animator/View_Left/Image_Title02").gameObject;
            rewardTitle = mTrans.Find("Animator/View_Right/Text_Name").gameObject;
        }

        public void RegisterEvents(IListener listener) {
            Image_BG_RectTransform.onClick.AddListener(listener.OnCloseClicked);
        }

        public interface IListener {
            void OnCloseClicked();
        }
    }
}