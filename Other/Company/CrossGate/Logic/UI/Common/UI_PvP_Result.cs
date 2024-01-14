using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_PvP_Result : UIBase
    {
        int result = 0;
        GameObject Image_Successbg, Image_Failedbg;
        Button btn_close;
        #region 系统函数
        protected override void OnLoaded()
        {
            Image_Successbg = transform.Find("Animator/Image_Successbg").gameObject;
            Image_Failedbg = transform.Find("Animator/Image_Failedbg").gameObject;
            btn_close = transform.Find("Image_Black").GetComponent<Button>();

            btn_close.onClick.AddListener(OnClickClose);
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                result = (int)arg;
            }
        }
        protected override void OnShow()
        {
            SetSuccess();
        }

        protected override void OnHide()
        {
            result = 0;
        }
        #endregion

        public void SetSuccess()
        {
            Image_Successbg.gameObject.SetActive(result == 1);
            Image_Failedbg.gameObject.SetActive(result == 2);
        }

        public void OnClickClose()
        {
            CloseSelf();
        }
    }
}