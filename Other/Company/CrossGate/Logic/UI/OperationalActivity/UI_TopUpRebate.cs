using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;

namespace Logic
{
    /// <summary> 充值返利 </summary>
    public class UI_TopUpRebate : UI_OperationalActivityBase
    {
        Button btn;
        Text btnText;
        Text timeText;
        protected override void InitBeforOnShow()
        {
            btn = transform.Find("Button").GetComponent<Button>();
            btnText = btn.transform.Find("Text").GetComponent<Text>();
            timeText = transform.Find("Text1").GetComponent<Text>();
            btn.onClick.AddListener(OnClickGet);

            Sys_OperationalActivity.Instance.actionBtnState -= OnUpdateRebateData;
            Sys_OperationalActivity.Instance.actionBtnState += OnUpdateRebateData;
        }
        public override void Show()
        {
            base.Show();
            SetBtnState();
            RefreshTimer();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Sys_OperationalActivity.Instance.actionBtnState -= OnUpdateRebateData;
        }
        public void OnUpdateRebateData()
        {
            SetBtnState();
        }
        private void SetBtnState()
        {
            if (Sys_OperationalActivity.Instance.isGetCashCoupon)
            {
                btnText.text = CSVLanguage.Instance.GetConfData(10158).words;
                btn.interactable = false;
            }
            else
            {

                btnText.text = CSVLanguage.Instance.GetConfData(2022721).words;
                btn.interactable = true;
            }
        }
        private void OnClickGet()
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(591000651).words;
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_OperationalActivity.Instance.ChargeRebateGetReq();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
        private void RefreshTimer()
        {
            DateTime time = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(Sys_OperationalActivity.Instance.aliveTime));
            timeText.text = time.ToString("yyyy-MM-dd HH:mm");
        }
    }
}