using Lib.Core;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using System;

namespace Logic
{
    public class UI_Compete : UIBase
    {
        //role info
        private UI_Compete_RoleInfo left;
        private UI_Compete_RoleInfo right;

        //tips
        private Text textTip;
        private Text textWait;
        private Text textWaitTime;

        //button
        private Button btnCancel;
        private Button btnRefuse;
        private Button btnAccept;

        private Timer timer;
        private float competeTime;

        private bool isCD = false;
        private Timer cdTimer;

        protected override void OnLoaded()
        {
            left = AddComponent<UI_Compete_RoleInfo>(transform.Find("Animator/View_Left_Big/View_Left"));
            right = AddComponent<UI_Compete_RoleInfo>(transform.Find("Animator/View_Right_Big/View_Right"));

            textTip = transform.Find("Animator/Text/Text_Tips").GetComponent<Text>();
            textWait = transform.Find("Animator/Text/Text_Wait").GetComponent<Text>();
            textWaitTime = transform.Find("Animator/Text/Text_Wait/Text_Time").GetComponent<Text>();

            btnCancel = transform.Find("Animator/Btn_01").GetComponent<Button>();
            btnCancel.onClick.AddListener(OnClickCancel);
            btnRefuse = transform.Find("Animator/Btn_Refuse").GetComponent<Button>();
            btnRefuse.onClick.AddListener(OnClickRefuse);
            btnAccept = transform.Find("Animator/Btn_Accept").GetComponent<Button>();
            btnAccept.onClick.AddListener(OnClickAccept);

            textWait.text = LanguageHelper.GetTextContent(4000004);

            CSVParam.Data timeData = CSVParam.Instance.GetConfData(540);
            competeTime = float.Parse(timeData.str_value) / 1000;
        }

        protected override void OnShow()
        {
            UpdatePanel();
        }

        protected override void OnHide()
        {
            timer?.Cancel();

            cdTimer?.Cancel();
            isCD = false;
        }

        private void OnClickCancel()
        {
            if (!isCD)
            {
                Sys_Compete.Instance.OnCancelInviteReq();
                UIManager.CloseUI(EUIID.UI_Compete);

                isCD = true;
                cdTimer = Timer.Register(0.5f, () =>
                {
                    isCD = false;
                });
            }
        }

        private void OnClickRefuse()
        {
            Sys_Compete.Instance.OnInviteOpReq(Packet.CompeteState.CompeteInviteRefuse);

            UIManager.CloseUI(EUIID.UI_Compete);
        }

        private void OnClickAccept()
        {
            Sys_Compete.Instance.OnInviteOpReq(Packet.CompeteState.CompeteInviteAgree);

            UIManager.CloseUI(EUIID.UI_Compete);
        }

        private void UpdatePanel()
        {
            //邀请方在左边，被邀请方在右边
            left.UpdateRole(Sys_Compete.Instance.inviteId, Sys_Compete.Instance.GetInviteNum());
            right.UpdateRole(Sys_Compete.Instance.beInvitedId, Sys_Compete.Instance.GetBeInviteNum());

            timer?.Cancel();
            timer = Timer.Register(competeTime, ()=> {
                timer?.Cancel();

                if (Sys_Compete.Instance.IsInviteRole() || Sys_Compete.Instance.IsBeInvitedRole())
                {
                    Sys_Compete.Instance.OnTimeOutReq();
                }

                UIManager.CloseUI(EUIID.UI_Compete);

            }, (curTime) =>{
                float leftTime = competeTime - curTime;
                string timeStr = string.Format("00:{0:D2}", Mathf.RoundToInt(leftTime));
                textWaitTime.text = timeStr;
            }, false, true);

            textTip.gameObject.SetActive(false);
            btnCancel.gameObject.SetActive(false);
            btnRefuse.gameObject.SetActive(false);
            btnAccept.gameObject.SetActive(false);

            if (Sys_Compete.Instance.IsInviteCamp())
            {
                if (Sys_Compete.Instance.IsInviteRole())
                {
                    btnCancel.gameObject.SetActive(true);
                }
                else
                {
                    textTip.gameObject.SetActive(true);
                    textTip.text = LanguageHelper.GetTextContent(4000005);
                }
            }
            else
            {
                if (Sys_Compete.Instance.IsBeInvitedRole())
                {
                    btnRefuse.gameObject.SetActive(true);
                    btnAccept.gameObject.SetActive(true);
                }
                else
                {
                    textTip.gameObject.SetActive(true);
                    textTip.text = textTip.text = LanguageHelper.GetTextContent(4000006); ;
                }
            }
        }
    }
}
