using Logic.Core;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Vitality_Tips : UIBase
    {
        private Text messgae;
        private Button closeBtn;

        protected override void OnLoaded()
        {
            messgae = transform.Find("Animator/View_Tips/Text_Describe").GetComponent<Text>();
            closeBtn = transform.Find("Blank").GetComponent<Button>();
            closeBtn.onClick.AddListener(OncloseBtnClicked);
        }

        private void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Vitality_Tips);
        }        
    }
}
