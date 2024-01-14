using UnityEngine.UI;
using Logic.Core;

namespace Logic
{

    public class UI_BackWalfare : UIBase
    {
        private Button btnClose;

        #region 系统函数
        protected override void OnOpen(object arg)
        {

        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {

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
            btnClose = transform.Find("Animator/Content/Btn_01").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        #endregion
    }
}