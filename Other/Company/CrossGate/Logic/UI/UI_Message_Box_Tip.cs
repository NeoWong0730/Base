using Lib.Core;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using System;

namespace Logic
{
    public class MsgBoxParam
    {
        public string strContent;
        public bool isToggle = false;
        public bool toggleState = false;
        public string strToggleTip;
        public Action<bool> actionToggle;
        public Action<bool> actionBtn;
        public int countTime = 0;
    }

    public class UI_Message_Box_Tip : UIBase
    {
        private Text textContent;
        private Toggle toggle;
        private Text textToggleTip;
        private Button btnCancel;
        private Button btnConfirm;
        private Text textBtnConfirm;

        private MsgBoxParam param;

        private int timeCountDown;
        private Timer timer;

        protected override void OnLoaded()
        {
            textContent = transform.Find("Animator/Text_Tip").GetComponent<Text>();
            toggle = transform.Find("Animator/Toggle").GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnToggleChange);
            textToggleTip = transform.Find("Animator/Toggle/Text").GetComponent<Text>();
            btnCancel = transform.Find("Animator/Button_Cancle").GetComponent<Button>();
            btnCancel.onClick.AddListener(OnBtnCancel);
            btnConfirm = transform.Find("Animator/Button_Sure").GetComponent<Button>();
            btnConfirm.onClick.AddListener(OnBtnConfirm);
            textBtnConfirm = transform.Find("Animator/Button_Sure/Text").GetComponent<Text>();
        }

        protected override void OnOpen(object arg)
        {
            param = null;
            if (arg != null)
                param = (MsgBoxParam)arg;
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

        protected override void OnDestroy() 
        {
        }

        private void OnToggleChange(bool isOn)
        {
            if (param != null && param.actionToggle != null)
                param.actionToggle.Invoke(isOn);
        }

        private void OnBtnCancel()
        {
            if (param != null && param.actionBtn != null)
                param.actionBtn.Invoke(false);
            this.CloseSelf();
        }

        private void OnBtnConfirm()
        {
            if (param != null && param.actionBtn != null)
                param.actionBtn.Invoke(true);
            this.CloseSelf();
        }

        private void UpdateInfo()
        {
            if (param == null)
            {
                DebugUtil.LogError("MsgBoxParam is Null");
                return;
            }

            textContent.text = param.strContent;
            toggle.gameObject.SetActive(param.isToggle);
            textToggleTip.text = param.strToggleTip;
            toggle.isOn = param.toggleState;

            //check timer
            if (param.countTime <= 0)
            {
                textBtnConfirm.text = LanguageHelper.GetTextContent(3830000012);
                return;
            }

            timeCountDown = param.countTime;
            timer?.Cancel();
            timer = Timer.Register(1f, () =>
            {
                timeCountDown--;
                if (timeCountDown <= 0)
                {
                    timer?.Cancel();
                    OnBtnConfirm();
                }

                textBtnConfirm.text = LanguageHelper.GetTextContent(3830000008, timeCountDown.ToString());
            }, null, true);
            textBtnConfirm.text = LanguageHelper.GetTextContent(3830000008, timeCountDown.ToString());
        }
    }
}
