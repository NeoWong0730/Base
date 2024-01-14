using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;

namespace Logic
{
    public class UI_RankActivity : UI_OperationalActivityBase
    {
        private Button btnGoRank;
        private Button btnGoActivityInfo;

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
            btnGoRank = transform.Find("Btn_01").GetComponent<Button>();
            btnGoRank.onClick.AddListener(OnBtnGoRankClick);
            btnGoActivityInfo = transform.Find("Btn_02").GetComponent<Button>();
            btnGoActivityInfo.onClick.AddListener(OnBtnGoActivityInfoClick);
        }
        private void UpdateView()
        {
            Sys_OperationalActivity.Instance.SetRankActivityFirstRedPoint();
        }
        #endregion
        #region event
        private void OnBtnGoRankClick()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(50901))
            {
                UIManager.OpenUI(EUIID.UI_Rank);
                UIManager.CloseUI(EUIID.UI_OperationalActivity);
                UIManager.HitButton(EUIID.UI_OperationalActivity, "GoRank", EOperationalActivity.RankActivity.ToString());
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000530));
            }
        }

        private void OnBtnGoActivityInfoClick()
        {
            Sys_OperationalActivity.Instance.JunpToRankActivityWebPage();
            UIManager.HitButton(EUIID.UI_OperationalActivity, "GoActivity", EOperationalActivity.RankActivity.ToString());
        }

        #endregion
    }
}
