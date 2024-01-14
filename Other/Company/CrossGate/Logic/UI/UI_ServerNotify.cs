using UnityEngine.UI;
using Logic.Core;
using System;
using Packet;
using Framework;

namespace Logic
{
    public class UI_ServerNotify : UIBase
    {
        public Button btnSure;
        public Text textContent;

        public int reason;
        public string appendContent;

        protected override void OnLoaded()
        {
            btnSure = transform.Find("Animator/Button_Sure").GetComponent<Button>();
            textContent = transform.Find("Animator/Text_Tip").GetComponent<Text>();

            btnSure.onClick.AddListener(OnBtnClicked);
        }
        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                var tp = arg as Tuple<int, string>;
                if (tp != null)
                {
                    reason = tp.Item1;
                    appendContent = tp.Item2;
                }
            }
        }

        protected override void OnOpened()
        {
            if (reason != 0)
            {
                uint langId = (uint)(1000070 + reason);
                string reasonContent = string.Format("{0} {1}", LanguageHelper.GetTextContent(langId), appendContent);

                textContent.text = reasonContent;
            }
        }

        private void OnBtnClicked()
        {
            RoleOffReason offReason = (RoleOffReason)reason;
            switch (offReason)
            {
                case RoleOffReason.EnterOther:
                case RoleOffReason.Gm:
                case RoleOffReason.Ban:
                case RoleOffReason.GmBeforeLoginAccount:
                    Sys_Net.Instance.OnPauseReconnect();
                    Sys_Role.Instance.ExitGameReq();
                    SDKManager.SDKLogout();
                    UIManager.CloseUI(EUIID.UI_Reconnection);
                    UIManager.CloseUI(EUIID.UI_ServerNotify);
                    break;
                case RoleOffReason.GmAllExitApp:
                case RoleOffReason.ProtoVerWrong:
                case RoleOffReason.GmBeforeHotUpdate:
                    Sys_Role.Instance.SDKAccountReportExtraData(SDKManager.SDKReportState.EXIT);
                    AppManager.Quit();
                    break;
                default:
                    //case RoleOffReason.Kicked:
                    //case RoleOffReason.Stop:
                    //case RoleOffReason.ReconnKicked:
                    //case RoleOffReason.GmBeforeSelectService:
                    Sys_Net.Instance.OnPauseReconnect();
                    Sys_Role.Instance.ExitGameReq();
                    UIManager.CloseUI(EUIID.UI_Reconnection);
                    UIManager.CloseUI(EUIID.UI_ServerNotify);
                    break;
            }
        }
    }
}
