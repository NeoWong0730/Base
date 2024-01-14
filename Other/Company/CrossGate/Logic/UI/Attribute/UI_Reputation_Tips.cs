using Logic.Core;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Reputation_Tips : UIBase
    {
        private Button closeBtn;

        protected override void OnLoaded()
        {
            closeBtn = transform.Find("View_BG").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseBtnClicked);
        }

        private void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Reputation_Tips);
        }
    }
}
