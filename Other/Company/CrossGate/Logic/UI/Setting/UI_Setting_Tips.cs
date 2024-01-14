using Logic.Core;
using UnityEngine.UI;

namespace Logic
{
    public class UISetingTipParam
    {
        public uint contentLanId;
    }
    public class UI_Setting_Tips : UIBase
    {
        private Button closeBtn;
        private Text content;

        private UISetingTipParam param;

        protected override void OnLoaded()
        {            
            closeBtn = transform.Find("Close").GetComponent<Button>();
            content = transform.Find("Animator/ZoneHealth/View_Health/Text_Des").GetComponent<Text>();

            closeBtn.onClick.AddListener(OnCloseBtnClick);
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                param = (UISetingTipParam)arg;
            }
        }

        protected override void OnShow()
        {
            if (param == null)
            {
                return;
            }
            content.text = LanguageHelper.GetTextContent(param.contentLanId);
        }

        protected override void OnHide()
        {
            param = null;
        }

        private void OnCloseBtnClick()
        {
            UIManager.CloseUI(EUIID.UI_Setting_Tips);
        }
    }
}
