using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_HundredPeopelAwakenTip : UIBase {
        public Text recommend;
        public Text current;
        
        public Text desc;
        public Text buffDesc;

        protected override void OnLoaded() {
            Button btn = transform.Find("Image_Close").GetComponent<Button>();
            btn.onClick.AddListener(OnBtnExitClicked);

            this.current = transform.Find("Animator/Image/Text4").GetComponent<Text>();
            this.recommend = transform.Find("Animator/Image/Text1").GetComponent<Text>();
            
            desc = transform.Find("Animator/Image/Text2").GetComponent<Text>();
            buffDesc = transform.Find("Animator/Image/Text3").GetComponent<Text>();
        }

        protected override void OnOpened() {
            uint id = Sys_HundredPeopleArea.Instance.GetCurrentInstanceId();
            CSVInstanceDaily.Data cSVInstanceDailyData = CSVInstanceDaily.Instance.GetConfData(id);
            if (null == cSVInstanceDailyData) return;

            uint curLevel = Sys_TravellerAwakening.Instance.awakeLevel;
            CSVTravellerAwakening.Data csvCurrentAwken = CSVTravellerAwakening.Instance.GetConfData(curLevel);
            string titleContent = "";
            if (csvCurrentAwken != null) {
                titleContent = LanguageHelper.GetTextContent(1006200, LanguageHelper.GetTextContent(csvCurrentAwken.NameId));
                
                CSVBuff.Data csvBuff = CSVBuff.Instance.GetConfData(csvCurrentAwken.BuffID);
                if (csvBuff != null) {
                    TextHelper.SetText(this.buffDesc, csvBuff.desc);
                }
            }

            CSVTravellerAwakening.Data csvAwken = CSVTravellerAwakening.Instance.GetConfData(cSVInstanceDailyData.Awakeningid);
            if (csvAwken != null) {
                string recommendContent = LanguageHelper.GetTextContent(1006204, LanguageHelper.GetTextContent(csvAwken.NameId));
                if (curLevel >= csvAwken.id) {
                    // 绿色
                    TextHelper.SetText(this.current, 1006197, titleContent);
                    TextHelper.SetText(this.recommend, recommendContent);
                    TextHelper.SetText(this.desc, 1006201);
                }
                else {
                    // 红色
                    TextHelper.SetText(this.current, 1006196, titleContent);
                    TextHelper.SetText(this.recommend, recommendContent);
                    TextHelper.SetText(this.desc, 1006202);
                }
            }
        }

        public void OnBtnExitClicked() {
            this.CloseSelf();
        }
    }
}