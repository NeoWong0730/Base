using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityTip : UIBase {
        public Button btnExit;
        public Text diseaseTitle;

        public void OnBtnExitClicked() {
            this.CloseSelf();
        }

        protected override void OnLoaded() {
            this.btnExit = this.transform.Find("Black").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);

            this.diseaseTitle = this.transform.Find("Animator/View_Rule/Image_Bg/Text_Tips").GetComponent<Text>();
        }

        protected override void OnShow() {
            string content = LanguageHelper.GetTextContent(2010629, (CSVFavorabilityStamina.Instance.GetConfData(1).Time * 1f / 60f).ToString());
            TextHelper.SetText(this.diseaseTitle, content);

            FrameworkTool.ForceRebuildLayout(this.gameObject);
        }
    }
}