using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;

namespace Logic
{
    public class UI_AlipayActivity : UI_OperationalActivityBase
    {
        private Button btnGoto;

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
            UIManager.HitPointShow(EUIID.UI_OperationalActivity, "AlipayActivityShow");
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
            UIManager.HitButton(EUIID.UI_OperationalActivity, "AlipayBtnClick");
            btnGoto = transform.Find("Button").GetComponent<Button>();
            btnGoto.onClick.AddListener(OnBtnGotoClick);
        }

        #endregion
        #region event
        private void OnBtnGotoClick()
        {
            Sys_OperationalActivity.Instance.JunpToAlipayActivityPage();
        }

        #endregion
    }
}
