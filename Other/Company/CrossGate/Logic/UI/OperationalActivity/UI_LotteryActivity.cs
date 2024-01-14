using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;

namespace Logic
{
    public class UI_LotteryActivity : UI_OperationalActivityBase
    {
        private Button btnGoCharge;
        private Button btnGoLottery;

        #region 系统函数
        protected override void Loaded()
        {
        }
        protected override void InitBeforOnShow()
        {
            Parse();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            UpdateView();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            //Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateLotteryActivityData, OnUpdateLotteryActivityData, toRegister);
        }
        #endregion
        #region func
        private void Parse()
        {
            btnGoCharge = transform.Find("Btn_01").GetComponent<Button>();
            btnGoCharge.onClick.AddListener(OnBtnGoChargeClick);
            btnGoLottery = transform.Find("Btn_02").GetComponent<Button>();
            btnGoLottery.onClick.AddListener(OnBtnGoLotteryClick);
        }
        private void UpdateView()
        {
            Sys_OperationalActivity.Instance.SetLotteryActivityFirstRedPoint();
        }
        #endregion
        #region event
        private void OnBtnGoChargeClick()
        {
            //跳转 商城-充值 界面
            MallPrama mallPrama = new MallPrama
            {
                mallId = 101,
                shopId = 1001,
                isCharge = true
            };
            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
            UIManager.HitButton(EUIID.UI_OperationalActivity, "GoCharge", EOperationalActivity.LotteryActivity.ToString());
        }

        private void OnBtnGoLotteryClick()
        {
            //uint npcInfoId = 1101016u;
            //ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcInfoId, true);
            if (Sys_OperationalActivity.Instance.CheckLotteryActivityIsOpen())
            {
                UIManager.OpenUI(EUIID.UI_Lotto);
                UIManager.CloseUI(EUIID.UI_OperationalActivity);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021134));
            }
            UIManager.HitButton(EUIID.UI_OperationalActivity, "GoLottery", EOperationalActivity.LotteryActivity.ToString());
        }
        //private void OnUpdateLotteryActivityData()
        //{
        //    UpdateView();
        //}
        #endregion
    }
}
