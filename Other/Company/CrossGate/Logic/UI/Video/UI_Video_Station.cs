using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Packet;

namespace Logic
{
    public class UI_Video_Station_Layout
    {
        public Transform transform;
        public UI_Video_LeftTabs leftTabs;
        public UI_Month_Video monthVideoView;
        public UI_Recently_Video recentlyVideoView;
        public UI_Personal_Center personalCanterView;
        public UI_ProbeView viewProp;
        public Button closeBtn;
        public UI_CurrencyTitle UI_CurrencyTitle;

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            monthVideoView = new UI_Month_Video();
            monthVideoView.Init(this.transform.Find("Animator/Viwe_Video"));
            recentlyVideoView = new UI_Recently_Video();
            recentlyVideoView.Init(this.transform.Find("Animator/Viwe_Recently"));
            personalCanterView = new UI_Personal_Center();
            personalCanterView.Init(this.transform.Find("Animator/Viwe_Personal Center"));
            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            leftTabs = new UI_Video_LeftTabs();
            leftTabs.Init(this.transform.Find("Animator/Menu/Scroll View/Viewport/Content"));
        }
        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnClose_ButtonClicked);
        }

        public interface IListener
        {
            void OnClose_ButtonClicked();
        }
    }

    public class UI_Video_Station : UIBase, UI_Video_Station_Layout.IListener
    {
        private UI_Video_Station_Layout layout = new UI_Video_Station_Layout();
        private Dictionary<EVideoViewType, UIComponent> dictOpPanel;
        private int defaultType;
        private int curEViewType;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            dictOpPanel = new Dictionary<EVideoViewType, UIComponent>();
            dictOpPanel.Add(EVideoViewType.MonthVideoView, layout.monthVideoView);
            dictOpPanel.Add(EVideoViewType.RecentlyVideoView, layout.recentlyVideoView);
            dictOpPanel.Add(EVideoViewType.PersonalCenterView, layout.personalCanterView);
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
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Video.Instance.eventEmitter.Handle<EVideoViewType>(Sys_Video.EEvents.OnSelectViewType, OnSelectViewType, toRegister);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnPlayVideoSuccess, OnPlayVideoSuccess, toRegister);
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnOpenPersonCenter, OnOpenPersonCenter, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnUpdateCrossSrvState, OnUpdateCrossSrvState, toRegister);
        }

        private void OnOpenPersonCenter()
        {
            if (Sys_Video.Instance.unuploadList.Count != 0)
            {
                Sys_Video.Instance.VideoBaseDetailReq(VideoButton.Local);
            }
            if (Sys_Video.Instance.uploadList.Count != 0)
            {
                Sys_Video.Instance.VideoBaseDetailReq(VideoButton.Upload);
            }
            if (Sys_Video.Instance.collectList.Count != 0)
            {
                Sys_Video.Instance.VideoBaseDetailReq(VideoButton.Collect);
            }
        }

        private void OnUpdateCrossSrvState()
        {
            UIManager.CloseUI(EUIID.UI_Video);
        }

        protected override void OnShow()
        {
            if (Sys_Video.Instance.personalCenterExpireTime != 0 && Sys_Time.Instance.GetServerTime() < Sys_Video.Instance.personalCenterExpireTime)
            {
                OnOpenPersonCenter();
            }
            else
            {
                Sys_Video.Instance.OpenPersonCenterReq(Sys_Role.Instance.RoleId);
            }
            layout.UI_CurrencyTitle.InitUi();
            if (curEViewType != 0)
            {
                defaultType = curEViewType;
            }
            layout.leftTabs.OnDefaultSelect(defaultType);
        }

        protected override void OnHide()
        {
            foreach (var data in dictOpPanel)
            {
                data.Value.Hide();
            }
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
        }

        private void OnSelectViewType(EVideoViewType _type)
        {
            foreach (var data in dictOpPanel)
            {
                if (data.Key == _type)
                {
                    data.Value.Show();
                }
                else
                {
                    data.Value.Hide();
                }
            }
            curEViewType = (int)_type;
        }

        private void OnPlayVideoSuccess()
        {
            UIManager.CloseUI(EUIID.UI_Video);
            curEViewType = 0;
        }

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Video);
            curEViewType = 0;
        }
    }
}
