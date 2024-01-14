using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using Logic.Core;
using UnityEngine.UI;

namespace Logic
{
    public class UI_SevenDaysTargetPopup : UIBase
    {
        private Button btnClose;
        private Button btnGo;

        #region 系统函数
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            Sys_OperationalActivity.Instance.SetSevenDaysTargetIsPopuped();
        }
        protected override void OnHide()
        {
        }
        protected override void OnDestroy()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            btnGo = transform.Find("Animator/Button").GetComponent<Button>();
            btnGo.onClick.AddListener(OnBtnGoClick);
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnBtnGoClick()
        {
            UIManager.OpenUI(EUIID.UI_SevenDaysTarget);
            this.CloseSelf();
        }
        #endregion
    }
}
