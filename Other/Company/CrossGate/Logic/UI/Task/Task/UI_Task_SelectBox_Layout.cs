//this file is auto created by QuickCode,you can edit it 
//do not need to care initialization of ui widget any more 
//------------------------------------------------------------
/*
*   @Author:冰块网络
*   DateTime:2019/11/20 14:33:23
*   Purpose:UI Componments Data Binding
*/
//-------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_Task_SelectBox_Layout {
        #region UI Variable Statement 

        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public Transform proto { get; private set; }
        public Transform SureNode { get; private set; }
        public Button Button_Cancel_RectTransform { get; private set; }
        public Button Button_Confirm_RectTransform { get; private set; }
        public Button Button_Cancel_RectTransform3 { get; private set; }
        public Button Button_Confirm_RectTransform4 { get; private set; }

        public CP_ToggleRegistry Registry { get; private set; }

        #endregion

        public void Parse(GameObject root) {
            mRoot = root;
            mTrans = root.transform;
            proto = mTrans.Find("Animator/View_Choose_Award/Items/Item");
            Button_Cancel_RectTransform = mTrans.Find("Animator/View_Choose_Award/Button_Cancel").GetComponent<Button>();
            Button_Confirm_RectTransform = mTrans.Find("Animator/View_Choose_Award/Button_Confirm").GetComponent<Button>();

            SureNode = mTrans.Find("Animator/View_Give_Up");
            Button_Cancel_RectTransform3 = mTrans.Find("Animator/View_Give_Up/Button_Cancel").GetComponent<Button>();
            Button_Confirm_RectTransform4 = mTrans.Find("Animator/View_Give_Up/Button_Confirm").GetComponent<Button>();
            Registry = mTrans.Find("Animator/View_Choose_Award/Items").GetComponent<CP_ToggleRegistry>();
        }

        public void RegisterEvents(IListener listener) {
            Button_Cancel_RectTransform.onClick.AddListener(listener.OnButton_Cancel_RectTransform);
            Button_Confirm_RectTransform.onClick.AddListener(listener.OnButton_Confirm_RectTransform);
            Button_Cancel_RectTransform3.onClick.AddListener(listener.OnButton_Cancel_RectTransform3);
            Button_Confirm_RectTransform4.onClick.AddListener(listener.OnButton_Confirm_RectTransform4);
        }

        public interface IListener {
            void OnButton_Cancel_RectTransform();
            void OnButton_Confirm_RectTransform();
            void OnButton_Cancel_RectTransform3();
            void OnButton_Confirm_RectTransform4();
        }
    }
}