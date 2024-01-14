using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;

namespace Logic
{
    public class UI_Attribute_LeftTabs : UIComponent
    {
        private List<Toggle> tabList = new List<Toggle>();
        private int lastIndex = -1;
        private GameObject m_TitleRedPoint;

        protected override void Loaded()
        {
            tabList.Clear();
            lastIndex = -1;

            Toggle[] toggles = transform.GetComponentsInChildren<Toggle>();
            foreach (Toggle toggle in toggles)
            {
                tabList.Add(toggle);
                toggle.onValueChanged.AddListener(var =>
                {
                    if (var)
                    {
                        int index = tabList.IndexOf(toggle);
                        OnSelect(index);
                    }
                });
            }
            toggles[0].isOn = true;

            m_TitleRedPoint = transform.Find("Scroll View/TabList/TabItem (4)/Image_Dot").gameObject;

            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnTitleGet, OnTitleGet, true);
            Sys_Title.Instance.eventEmitter.Handle(Sys_Title.EEvents.OnRefreshTitleRedState, RefreshTitleRedState, true);
        }

        public override void OnDestroy()
        {
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnTitleGet, OnTitleGet, false);
            Sys_Title.Instance.eventEmitter.Handle(Sys_Title.EEvents.OnRefreshTitleRedState, RefreshTitleRedState, false);
        }


        private void OnTitleGet(uint titleId)
        {
            RefreshTitleRedState();
        }

        public void RefreshTitleRedState()
        {
            m_TitleRedPoint.SetActive(Sys_Title.Instance.Red());
        }

        private void OnSelect(int _index)
        {
            if (lastIndex != _index)
            {
                lastIndex = _index;

                Sys_Attr.Instance.eventEmitter.Trigger<ERoleViewType>(Sys_Attr.EEvents.OnSelectRoleViewType, (ERoleViewType)(_index + 1));
            }
        }

        public void CheckAddpoint(bool isTrue)
        {
            //第二个加点
            tabList[1].gameObject.SetActive(isTrue);
        }

        public void CheckAdvance(bool isTrue)
        {
            //第三个进阶
            tabList[2].gameObject.SetActive(isTrue);
        }
        public void CheckProbe(bool isTrue)
        {
            //第四个探索
            tabList[3].gameObject.SetActive(isTrue);
        }

        public void OnDefaultSelect(int defaultIndex)
        {
            Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnSelectRoleViewType, (ERoleViewType)defaultIndex);
            tabList[defaultIndex - 1].isOn = true;
            lastIndex = defaultIndex - 1;
        }
    }
}


