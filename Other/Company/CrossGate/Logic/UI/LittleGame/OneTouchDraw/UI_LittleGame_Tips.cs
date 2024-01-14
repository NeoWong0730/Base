using Logic.Core;
using UnityEngine.UI;

namespace Logic
{
    public class UI_LittleGame_Tips : UIBase
    {
        public uint lanId;
        public Text text;

        public Button buttonClose;

        protected override void OnLoaded()
        {
            text = transform.Find("Animator/Tips/Text_Describe").GetComponent<Text>();
            buttonClose = transform.Find("Black").GetComponent<Button>();
            buttonClose.onClick.AddListener(OnBtnCloseClicked);
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                lanId = (uint)arg;
            }
        }
        protected override void OnShow()
        {
            TextHelper.SetText(text, lanId);
        }

        private void OnBtnCloseClicked()
        {
            CloseSelf();
        }
    }
}