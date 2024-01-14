using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;

namespace Logic
{
    public class UI_KingPetActivity : UI_OperationalActivityBase
    {

        private Button btnGo;

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
        }
        #endregion
        #region func
        private void Parse()
        {
            btnGo = transform.Find("Btn_01").GetComponent<Button>();
            btnGo.onClick.AddListener(OnBtnGoClick);
        }
        private void UpdateView()
        {
            Sys_OperationalActivity.Instance.SetLotteryActivityFirstRedPoint();
        }
        #endregion
        #region event
        private void OnBtnGoClick()
        {
            uint npcInfoId = 1101016u;
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcInfoId, true);
            //UIManager.OpenUI(EUIID.UI_KingPet);
            UIManager.CloseUI(EUIID.UI_OperationalActivity);
        }
        #endregion
    }
}
