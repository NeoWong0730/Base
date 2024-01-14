using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Lib.Core;

namespace Logic
{
    public class UI_LittleGame_OneTouchDraw_Layout
    {
        #region UI Variable Statement
        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }

        public EventTrigger eventTrigger;
        public Transform childNode;

        public RawImageLoader iconLoader;
        public Text countDownTime;
        public Text desc;

        public Image arrow;
        public LineRenderer lineRenderer;

        public Transform positionIndexer;
        public Transform fx;
        public Transform npcRoot;

        public Button buttonClose;
        public Button buttonTips;
        public Button buttonStart;
        public Text buttonStartText;
        public Image buttonStartImage;

        #endregion
        public void Parse(GameObject root)
        {
            mRoot = root;
            mTrans = root.transform;

            eventTrigger = mTrans.Find("Animator/View_bottom/Childs").GetComponent<EventTrigger>();
            fx = mTrans.Find("Animator/View_bottom/Fx/Fx_ui_OneTouchDraw01");

            childNode = mTrans.Find("Animator/View_bottom/Childs");
            npcRoot = mTrans.Find("Animator/NpcRoot");

            iconLoader = mTrans.Find("Animator/View_left/Image_Head").GetComponent<RawImageLoader>();
            countDownTime = mTrans.Find("Animator/View_Clock/Text_Time").GetComponent<Text>();
            desc = mTrans.Find("Animator/View_left/Text_Describe").GetComponent<Text>();

            arrow = mTrans.Find("Animator/View_bottom/Arrow").GetComponent<Image>();
            lineRenderer = mTrans.Find("Animator/View_bottom/LineRenderer").GetComponent<LineRenderer>();

            positionIndexer = mTrans.Find("Animator/PositionIndexer");

            buttonTips = mTrans.Find("Animator/View_left/Button_Tips").GetComponent<Button>();
            buttonClose = mTrans.Find("Animator/Btn_Close").GetComponent<Button>(); 
            buttonStart = mTrans.Find("Animator/View_left/Btn_01").GetComponent<Button>();
            buttonStartText = mTrans.Find("Animator/View_left/Btn_01/Text_01").GetComponent<Text>();
            buttonStartImage = mTrans.Find("Animator/View_left/Btn_01").GetComponent<Image>();
        }

        public void RegisterEvents(IListener listener)
        {
            buttonClose.onClick.AddListener(listener.OnReturn_ButtonClicked);
            buttonTips.onClick.AddListener(listener.OnTips_ButtonClicked);
            buttonStart.onClick.AddListener(listener.OnStart_ButtonClicked);
        }

        public interface IListener
        {
            void OnReturn_ButtonClicked();
            void OnStart_ButtonClicked();
            void OnTips_ButtonClicked();
        }
    }
}
