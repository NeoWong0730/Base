using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public enum EPartnerTabType
    {
        PartnerList = 0,
        PartnerConfig = 1,
        PartnerProperty = 2,
        PartnerRuneEquip = 3,
        PartnerRuneBag = 4,
        PartnerRuneChange = 5,
        PartnerFetter = 6,
    }

    public class UI_Partner_Right_Tabs : UIParseCommon
    {
        public class TabType
        {
            private Transform transform;

            private CP_Toggle _toggle;
            private Text tabName1;
            private Text tabName2;
            public GameObject redDotGo;

            private int _tabIndex;
            private System.Action<int> _action;
            public void Init(Transform trans)
            {
                transform = trans;

                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnClickToggle);

                tabName1 = transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
                tabName2 = transform.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
                redDotGo = transform.Find("Red_Dot")?.gameObject;
            }

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    _action?.Invoke(_tabIndex);
                }
            }

            public void SetType(int index)
            {
                _tabIndex = index;
                //if (_tabIndex > 1) //暂时只有两个
                //    return;

                tabName1.text = tabName2.text = LanguageHelper.GetTextContent((uint)(2006001 + index));
            }

            public void Register(System.Action<int> action)
            {
                _action = action;
            }

            public void OnSelect(bool isOn)
            {
                _toggle.SetSelected(isOn, true);
            }
        }

        private List<TabType> listTabs = new List<TabType>();
        private IListener listener;

        protected override void Parse()
        {
            int length = Enum.GetValues(typeof(EPartnerTabType)).Length;

            Transform transParent = transform.Find("TabList");
            int count = transParent.childCount;
            for (int i = 0; i < count; ++i)
            {
                TabType tab = new TabType();
                tab.Init(transParent.GetChild(i));
                tab.SetType(i);
                tab.Register(OnTabSelect);

                listTabs.Add(tab);
            }
        }

        public override void Hide()
        {
            for (int i = 0; i < listTabs.Count; ++i)
                listTabs[i].OnSelect(false);
        }

        private void OnTabSelect(int index)
        {
            listener?.OnClickTabType((EPartnerTabType)index);
        }

        public void OnTabIndex(int tabIndex)
        {
            listTabs[tabIndex].OnSelect(true);
        }

        public void ActiveRedDot(int tabIndex, bool toShow) {
            listTabs[tabIndex].redDotGo?.SetActive(toShow);
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnClickTabType(EPartnerTabType _type);
        }
    }
}
