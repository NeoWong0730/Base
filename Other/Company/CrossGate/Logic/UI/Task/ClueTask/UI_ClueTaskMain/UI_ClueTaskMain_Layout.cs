using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using Framework;
using Logic;

namespace Logic
{
    public class UI_ClueTaskMain_Layout
    {
        public GameObject gameObject;
        public Transform transform;

        public GameObject tabProto;
        public Transform tabParent;
        public Button btnReturn;
        public Transform contengParent;

        public void Parse(GameObject root)
        {
            gameObject = root;
            transform = gameObject.transform;

            tabProto = transform.Find("Animator/View_Left_Tabs/Scroll/TabList/TabItem").gameObject;
            tabParent = transform.Find("Animator/View_Left_Tabs/Scroll/TabList");
            contengParent = transform.Find("Animator/Contents/Conents");
            btnReturn = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
        }
        public void RegisterEvents(IListener listener)
        {
            btnReturn.onClick.AddListener(listener.OnBtnReturnClicked);
        }

        public interface IListener
        {
            void OnBtnReturnClicked();
        }
    }
}