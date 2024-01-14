using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Framework;
using System;

namespace Logic
{
    public class UI_Head_Layout
    {
        public Transform transform;
        public Button closeBtn;
        public GameObject message;

        public UI_Head_LeftTabs leftTabs;
        public UI_Head_View headView;
        public UI_HeadFrame_View headFrameView;
        public UI_ChatFrame_View chatFrameView;
        public UI_ChatBackground_View chatBackgroundView;
        public UI_ChatText_View chatTextView;
        public UI_TeamFalg_View teamFalgView;
        public UI_Right_Describe describeView;

        public void Init(Transform transform)
        {
            this.transform = transform;

            leftTabs = new UI_Head_LeftTabs();
            leftTabs.Init(this.transform.Find("Animator/View_Left/Scroll View/Viewport/Content"));

            headView = new UI_Head_View();
            headView.Init(this.transform.Find("Animator/View_Center/Scroll_View_Head"));
            headView.collectNum = transform.Find("Animator/View_Center/Image_Title/Text").GetComponent<Text>();

            headFrameView = new UI_HeadFrame_View();
            headFrameView.Init(this.transform.Find("Animator/View_Center/Scroll_View_HeadFrame"));
            headFrameView.collectNum = transform.Find("Animator/View_Center/Image_Title/Text").GetComponent<Text>();

            chatFrameView = new UI_ChatFrame_View();
            chatFrameView.Init(this.transform.Find("Animator/View_Center/Scroll_View_ChatFrame"));
            chatFrameView.collectNum = transform.Find("Animator/View_Center/Image_Title/Text").GetComponent<Text>();

            chatBackgroundView = new UI_ChatBackground_View();
            chatBackgroundView.Init(this.transform.Find("Animator/View_Center/Scroll_View_ChatBg"));
            chatBackgroundView.collectNum = transform.Find("Animator/View_Center/Image_Title/Text").GetComponent<Text>();

            chatTextView = new UI_ChatText_View();
            chatTextView.Init(this.transform.Find("Animator/View_Center/Scroll_View_ChatText"));
            chatTextView.collectNum = transform.Find("Animator/View_Center/Image_Title/Text").GetComponent<Text>();

            teamFalgView = new UI_TeamFalg_View();
            teamFalgView.Init(this.transform.Find("Animator/View_Center/Scroll_View_TeamLogo"));
            teamFalgView.collectNum = transform.Find("Animator/View_Center/Image_Title/Text").GetComponent<Text>();

            message = transform.Find("Animator/View_Right").gameObject;
            describeView = new UI_Right_Describe();
            describeView.BingGameObject(message);
            closeBtn = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
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

    public class UI_Head : UIBase, UI_Head_Layout.IListener
    {
        private UI_Head_Layout layout = new UI_Head_Layout();
        private Dictionary<EHeadViewType, UIComponent> openPanelDic;
        private int defaultType;
        private int curType;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            openPanelDic = new Dictionary<EHeadViewType, UIComponent>();

            openPanelDic.Add(EHeadViewType.HeadView, layout.headView);
            openPanelDic.Add(EHeadViewType.HeadFrameView, layout.headFrameView);
            openPanelDic.Add(EHeadViewType.ChatFrameView, layout.chatFrameView);
            openPanelDic.Add(EHeadViewType.ChatBackgraoudView, layout.chatBackgroundView);
            openPanelDic.Add(EHeadViewType.ChatTextView, layout.chatTextView);
            openPanelDic.Add(EHeadViewType.TeamFalgView, layout.teamFalgView);

            layout.describeView.assetDependencies = transform.GetComponent<AssetDependencies>();
        }

        protected override void OnOpen(object arg)
        {
            if (arg == null)
            {
                defaultType = 1;
            }
            else
            {
                defaultType = (int)arg;
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Head.Instance.eventEmitter.Handle<EHeadViewType>(Sys_Head.EEvents.OnSelectViewType, OnSelectViewType, toRegister);
            Sys_Head.Instance.eventEmitter.Handle<uint, EHeadViewType>(Sys_Head.EEvents.OnSelectItem, OnSelectItem, toRegister);
            Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnUsingUpdate, OnUsingUpdate, toRegister);
            Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnExpritedUpdate, OnExpritedUpdate, toRegister);
            Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnAddUpdate, OnAddUpdate, toRegister);
        }

        protected override void OnShow()
        {            
            layout.describeView.OnCreateModel();
            //功能开启部分
            //bool isAddpointOpen = Sys_FunctionOpen.Instance.IsOpen(10103, false);
            //layout.leftTabs.CheckHeadFrame(isAddpointOpen);

            if (curType != 0)
            {
                defaultType = curType;
            }
            layout.leftTabs.OnDefaultSelect(defaultType);
        }

        protected override void OnHide()
        {            
            foreach (var data in openPanelDic)
            {
                data.Value.Hide();
            }
            layout.describeView._UnloadShowContent();
            layout.describeView.timer?.Cancel();
        }

        protected override void OnDestroy()
        {
            foreach (var item in openPanelDic)
            {
                UIComponent uIComponent = item.Value;
                item.Value.OnDestroy();
            }
        }

        private void OnSelectViewType(EHeadViewType type)
        {
            foreach (var data in openPanelDic)
            {
                if (data.Key == type)
                {
                    data.Value.Show();
                }
                else
                {
                    data.Value.Hide();
                }
            }
            curType = (int)type;
        }

        private void OnSelectItem(uint id, EHeadViewType type)
        {
            layout.describeView.SetData(id, type);
            if (Sys_Head.Instance.activeInfosCheckDic.ContainsKey(id))
            {
                Sys_Head.Instance.activeInfosCheckDic[id] = true;
            }
            RefreshRedPoint();
        }

        private void OnUsingUpdate()
        {
            layout.describeView.UseHeadFrameUpdate();
        }

        private void OnExpritedUpdate()
        {
            layout.describeView.ExpritedUpdate();
        }

        private void OnAddUpdate()
        {
            layout.describeView.ActiveUpdate();
            RefreshRedPoint();
        }

        private void RefreshRedPoint()
        {
            layout.leftTabs.ShowRedPoint(EHeadViewType.None, false);
            foreach (var item in Sys_Head.Instance.activeInfosCheckDic)
            {
                if (!item.Value)
                {
                    layout.leftTabs.ShowRedPoint(Sys_Head.Instance.GetTypeById(item.Key), true);
                }
            }
        }

        public void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Head);
        }
    }
}
