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
    public class UI_Task_Normal_Result_Layout {
        #region UI Variable Statement 

        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public Transform RewardItemProto { get; private set; }
        public Transform TinyRewardItemProto { get; private set; }
        public Button BtnCLose { get; private set; }

        #endregion

        public void Parse(GameObject root) {
            mRoot = root;
            mTrans = root.transform;
            RewardItemProto = mTrans.Find("Aniamtor/Scroll_View/Viewport/Item");
            TinyRewardItemProto = mTrans.Find("Aniamtor/Grid_Award/AwardItem");
            BtnCLose = mTrans.Find("Aniamtor/Image_BG").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener) {
            BtnCLose.onClick.AddListener(listener.OnCloseClicked);
        }

        public interface IListener {
            void OnCloseClicked();
        }
    }
}