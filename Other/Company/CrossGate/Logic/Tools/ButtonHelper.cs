using Lib.Core;
using UnityEngine.UI;

namespace Logic
{
    public static class ButtonHelper
    {
        public static void Enable(Button button, bool toEnable)
        {
            button.enabled = toEnable;
            ImageHelper.SetImageGray(button, !toEnable, true);
        }

        public static void AddButtonCtrl(Button button)
        {
            ButtonCtrl ctrl = button?.gameObject.GetNeedComponent<ButtonCtrl>();
            if (null != ctrl)
                ctrl.button = button;
        }
    }
}
