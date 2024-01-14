using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;


namespace Logic
{
    public class UI_FamilyBoss_Sure : UIBase
    {

        private Text textTip;
        private Button btnSure;
        private Timer timer;

        protected override void OnLoaded()
        {
            btnSure = transform.Find("Animator/Button_Sure").GetComponent<Button>();
            btnSure.onClick.AddListener(OnClickClose);

            textTip = transform.Find("Animator/Text_Tip").GetComponent<Text>();
        }

        protected override void OnDestroy()
        {

        }
        protected override void OnOpen(object arg)
        {
            
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {
            timer?.Cancel();
            timer = null;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            //Sys_FamilyBoss.Instance.eventEmitter.Handle<uint, uint>(Sys_FamilyBoss.EEvents.OnBossSimpleInfo, this.OnBossSimpleInfo, toRegister);
        }

        private void OnClickClose()
        {
            CloseSelf();
            if (Sys_Time.Instance.GetServerTime() >= Sys_FamilyBoss.Instance.NextTime)
            {
                Sys_FamilyBoss.Instance.OnGuildBossInfoReq();
            }
        }

        private void UpdateInfo()
        {
            if (Sys_Time.Instance.GetServerTime() < Sys_FamilyBoss.Instance.NextTime)
            {
                uint leftTime = Sys_FamilyBoss.Instance.NextTime - Sys_Time.Instance.GetServerTime();
                timer?.Cancel();
                timer = Timer.Register(1f, () =>
                {
                    leftTime--;
                    if (leftTime <= 0)
                    {
                        leftTime = 0;
                        timer?.Cancel();
                        timer = null;
                    }
                    textTip.text = LanguageHelper.GetTextContent(3910010318, LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_1));

                }, null, true);
                textTip.text = LanguageHelper.GetTextContent(3910010318, LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_1));
            }
            else
            {
                CloseSelf();
            }
        }
    }
}