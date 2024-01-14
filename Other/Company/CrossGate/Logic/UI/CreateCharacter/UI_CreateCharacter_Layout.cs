using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Logic
{
    public class UI_CreateCharacter_Layout
    {
        #region UI Variable Statement
        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public Button Btn_Return_Button { get; private set; }
        public Button Btn_Enter_Button { get; private set; }
        public Button Btn_Random { get; private set; }
        public InputField Input { get; private set; }
        public CP_ScrolCircleList scrollList { get; private set; }
        public CP_VerticalCenterOnChild centerOnChild { get; private set; }
        public Transform highlight { get; private set; }
        public Transform highlightParent { get; private set; }
        public Image charName;

        #endregion
        public void Parse(GameObject root)
        {
            mRoot = root;
            mTrans = root.transform;
            scrollList = mTrans.Find("Animator/ScrollItem/Scroll View/Viewport").GetComponent<CP_ScrolCircleList>();
            centerOnChild = mTrans.Find("Animator/ScrollItem/Scroll View/Viewport").GetComponent<CP_VerticalCenterOnChild>();

            highlight = mTrans.Find("Animator/ScrollItem/Scroll View/Image_Frame_BG");
            highlightParent = highlight.parent;
            Btn_Return_Button = mTrans.Find("Animator/Button_Close").GetComponent<Button>();
            Btn_Enter_Button = mTrans.Find("Animator/Btn_Login").GetComponent<Button>();
            Btn_Random = mTrans.Find("Animator/Text_Account/Button_Icon").GetComponent<Button>();
            Input = mTrans.Find("Animator/Text_Account/InputField").GetComponent<InputField>();
            charName = mTrans.Find("Animator/Name/Image_Name").GetComponent<Image>();
        }

        public void RegisterEvents(IListener listener)
        {
            Btn_Return_Button.onClick.AddListener(listener.OnReturn_ButtonClicked);
            Btn_Enter_Button.onClick.AddListener(listener.OnEnter_ButtonClicked);
            Btn_Random.onClick.AddListener(listener.OnRandom_ButtonClicked);
        }

        public interface IListener
        {
            void OnEnter_ButtonClicked();
            void OnReturn_ButtonClicked();
            void OnRandom_ButtonClicked();
        }
    }
}
