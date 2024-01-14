using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Framework;
using System;

namespace Logic
{
    public class UI_Attribute_Layout
    {
        public Transform transform;
        public Button closeBtn;
        public UI_CurrencyTitle UI_CurrencyTitle;

        public UI_Attribute_LeftTabs leftTabs;
        public UI_AttrView viewAttr;
        public UI_AddView viewAdd;
        public UI_Advance viewAdvance;
        public UI_ProbeView viewProp;
        public UI_Title viewTitle;

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();

            viewAttr = new UI_AttrView();
            viewAttr.Init(this.transform.Find("Animator/View_Attribute"));

            viewAdd = new UI_AddView();
            viewAdd.Init(this.transform.Find("Animator/View_Add"));

            viewAdvance = new UI_Advance();
            viewAdvance.Init(this.transform.Find("Animator/View_Advance"));

            viewProp = new UI_ProbeView();
            viewProp.Init(this.transform.Find("Animator/View_Probe"));


            viewTitle = new UI_Title();
            viewTitle.Init(transform.Find("Animator/View_Title"));

            leftTabs = new UI_Attribute_LeftTabs();
            leftTabs.Init(this.transform.Find("Animator/View_Left_Tabs"));

            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnCloseBtnClicked);
        }

        public interface IListener
        {
            void OnCloseBtnClicked();
        }
    }

    public class UI_Attribute : UIBase, UI_Attribute_Layout.IListener
    {
        private UI_Attribute_Layout layout = new UI_Attribute_Layout();

        private Dictionary<ERoleViewType, UIComponent> dictOpPanel;
        private int defaultType;
        private bool isHpmp;
        private int curERoleViewType;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            dictOpPanel = new Dictionary<ERoleViewType, UIComponent>();

            dictOpPanel.Add(ERoleViewType.ViewAttr, layout.viewAttr);
            dictOpPanel.Add(ERoleViewType.ViewAdd, layout.viewAdd);
            dictOpPanel.Add(ERoleViewType.ViewAdvance, layout.viewAdvance);
            dictOpPanel.Add(ERoleViewType.ViewProp, layout.viewProp);
            dictOpPanel.Add(ERoleViewType.ViewTitle, layout.viewTitle);
        }

        protected override void OnOpen(object arg)
        {
            if (arg == null)
            {
                defaultType = 1;
            }
            else
            {
                defaultType = Convert.ToInt32(arg);
                if (defaultType == 6)
                {
                    defaultType = 1;
                    isHpmp = true;
                }
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Attr.Instance.eventEmitter.Handle<ERoleViewType>(Sys_Attr.EEvents.OnSelectRoleViewType, OnSelectViewType, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle<ERoleViewType>(Sys_Attr.EEvents.OnChangeTab, OnChangeTab, toRegister);
            
       }

        protected override void ProcessEvents(bool toRegister)
        {   
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, OnUIEndExit, toRegister);
        }

        private void OnUIEndExit(uint stackID, int nID)
        {
            if (nID == (int)EUIID.UI_Mall || nID == (int)EUIID.UI_PointMall)
            {
                isHpmp = true;
            }
            else
            {
                isHpmp = false;
            }
        }
        protected override void OnShow()
        {
            layout.UI_CurrencyTitle.InitUi();
            layout.viewAttr.assetDependencies = transform.GetComponent<AssetDependencies>();
            layout.viewProp.assetDependencies = transform.GetComponent<AssetDependencies>();
            layout.viewTitle.assetDependencies = transform.GetComponent<AssetDependencies>();

            bool isAddpointOpen = Sys_FunctionOpen.Instance.IsOpen(10103, false);
            layout.leftTabs.CheckAddpoint(isAddpointOpen);
            bool isAdvanceOpen = Sys_FunctionOpen.Instance.IsOpen(10104, false);
            layout.leftTabs.CheckAddpoint(isAddpointOpen);
            bool isProbeOpen = Sys_FunctionOpen.Instance.IsOpen(10105, false);
            layout.leftTabs.CheckProbe(isProbeOpen);
            bool isInfoOpen = Sys_FunctionOpen.Instance.IsOpen(10106, false);
            layout.leftTabs.CheckProbe(isProbeOpen);
            if (curERoleViewType != 0)
            {
                defaultType = curERoleViewType;
            }
            layout.leftTabs.OnDefaultSelect(defaultType);
            layout.leftTabs.RefreshTitleRedState();

            Sys_Attr.Instance.GetPointSchemeReq();
        }

        protected override void OnHide()
        {
            foreach (var data in dictOpPanel)
            {
                data.Value.Hide();
            }
           // isHpmp = false;
        }

        protected override void OnDestroy()
        {
            layout.UI_CurrencyTitle?.Dispose();
            if (dictOpPanel != null)
            {
                foreach (var item in dictOpPanel)
                {
                    UIComponent uIComponent = item.Value;
                    item.Value.OnDestroy();
                }
            }
            layout.leftTabs.OnDestroy();
        }

        private void OnSelectViewType(ERoleViewType _type)
        {
            foreach (var data in dictOpPanel)
            {
                if (data.Key == _type)
                {
                    if(data.Key == ERoleViewType.ViewAttr && isHpmp)
                    {
                        layout.viewAttr.isHpMpShow = true;
                        isHpmp = false;
                    }
                    data.Value.Show();
                }
                else
                {
                    data.Value.Hide();
                }
            }
            curERoleViewType = (int)_type;
        }

        public void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Attribute);
            curERoleViewType = 0;
        }

        protected override void OnUpdate()
        {
            layout.viewAttr.ExecUpdate();
        }

        private void OnChangeTab(ERoleViewType type)
        {
            layout.leftTabs.OnDefaultSelect((int)type);
        }
    }
}
