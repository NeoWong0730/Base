using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Scciety_FriendGroupOperation_Layout
    {
        public GameObject root;

        public Button closeButton;
        public Button settingButton;
        public Button dismissButton;

        public void Init(GameObject obj)
        {
            root = obj;

            closeButton = root.FindChildByName("close").GetComponent<Button>();
            settingButton = root.FindChildByName("SettingButton").GetComponent<Button>();
            dismissButton = root.FindChildByName("DismissButton").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            settingButton.onClick.AddListener(listener.OnClickSettingButton);
            dismissButton.onClick.AddListener(listener.OnClickDismissButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickSettingButton();

            void OnClickDismissButton();
        }
    }

    public class UI_Scciety_FriendGroupOperation : UIBase, UI_Scciety_FriendGroupOperation_Layout.IListener
    {
        private UI_Scciety_FriendGroupOperation_Layout layout = new UI_Scciety_FriendGroupOperation_Layout();

        public Sys_Society.FriendGroupInfo friendGroupInfo;

        protected override void OnLoaded()
        {
            layout.Init(gameObject);
            layout.RegisterEvents(this); 
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<string>(Sys_Society.EEvents.OnDelFriendGroupSuccess, OnDelFriendGroupSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<string>(Sys_Society.EEvents.OnCreateOrSettingFriendGroupSuccess, OnCreateOrSettingFriendGroupSuccess, toRegister);
        }

        protected override void OnOpen(object arg)
        {
            friendGroupInfo = arg as Sys_Society.FriendGroupInfo;
        }

        public void OnClickCloseButton()
        {
            CloseSelf();
        }

        public void OnClickSettingButton()
        {
            UIManager.OpenUI(EUIID.UI_Society_FriendGroupSetting, false, friendGroupInfo);
        }

        public void OnClickDismissButton()
        {
            PromptBoxParameter.Instance.Clear();
            //TODO
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13032);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Society.Instance.ReqDelFriendGroup(friendGroupInfo);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            PromptBoxParameter.Instance.SetCountdown(3f, PromptBoxParameter.ECountdown.Cancel);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        /// <summary>
        /// 删除好友分组成功回调///
        /// </summary>
        /// <param name="friendGroupInfo">删除的好友分组</param>
        void OnDelFriendGroupSuccess(string name)
        {
            if (friendGroupInfo.name == name)
            {
                CloseSelf();
            }
        }

        void OnCreateOrSettingFriendGroupSuccess(string name)
        {
            CloseSelf();
        }
    }
}
