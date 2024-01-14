using System;
using Logic.Core;
using Table;
using UnityEngine.UI;

namespace Logic {
    // 挑战规则
    public class UI_WorldBossChallengeRule : UIBase {
        public Text numText;
        public Text nameText;

        public CP_StarLevel starLevel;
        public Text descText;
        public Text timeText;
        public Text limitText;
        public Text levelText;

        public Button btn;

        public uint challengeId;
        public CSVChallengeLevel.Data csv;

        protected override void OnLoaded() {
            this.btn = this.transform.Find("Animator/View_Detail/BG").GetComponent<Button>();
            this.btn.onClick.AddListener(this.OnBtnClicked);

            this.nameText = this.transform.Find("Animator/View_Detail/Image_BG/Title0/Text_Title").GetComponent<Text>();
            this.numText = this.transform.Find("Animator/View_Detail/Image_BG/Title0/Text_Title/Text_num").GetComponent<Text>();

            this.starLevel = this.transform.Find("Animator/View_Detail/Image_BG/Title1/Grid_star").GetComponent<CP_StarLevel>();
            this.descText = this.transform.Find("Animator/View_Detail/Image_BG/Content_ChallengeDesc/Text_Des").GetComponent<Text>();

            this.timeText = this.transform.Find("Animator/View_Detail/Image_BG/Content_Time/Text_Des").GetComponent<Text>();
            this.limitText = this.transform.Find("Animator/View_Detail/Image_BG/Content_Fr/Text_Des").GetComponent<Text>();
            this.levelText = this.transform.Find("Animator/View_Detail/Image_BG/Content_LevelLimit/Text_Des").GetComponent<Text>();
        }
        public void OnBtnClicked() {
            this.CloseSelf();
        }

        protected override void OnOpen(object arg) {
            this.challengeId = Convert.ToUInt32(arg);
        }
        protected override void OnShow() {
            this.RefreshAll();
        }
        public void RefreshAll() {
            csv = CSVChallengeLevel.Instance.GetConfData(this.challengeId);
            if (csv != null) {
                TextHelper.SetText(this.nameText, csv.rule_name);
                this.numText.text = csv.rulePage_leve.ToString();
                this.starLevel.Build((int)csv.rulePage_leve, null);

                TextHelper.SetText(this.descText, csv.rulePage_description);
                TextHelper.SetText(this.limitText, csv.rulePage_rewardTimes);
                TextHelper.SetText(this.timeText, csv.rulePage_time); 
                TextHelper.SetText(this.levelText, 4157000003, csv.challengeLevelLimit[0].ToString(), csv.challengeLevelLimit[1].ToString());

                Lib.Core.FrameworkTool.ForceRebuildLayout(this.gameObject);
            }
        }
    }
}


