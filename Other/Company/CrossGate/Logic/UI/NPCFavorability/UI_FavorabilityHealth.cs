using System;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityHealth : UIBase {
        public FavorabilityNPC npc;

        public Button btnExit;
        public Text diseaseTitle;
        public Text diseaseDesc;
        public Transform rewardParent;

        public UI_RewardList rewards;

        public void OnBtnExitClicked() {
            this.CloseSelf();
        }

        protected override void OnLoaded() {
            this.btnExit = this.transform.Find("Close").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);

            this.diseaseTitle = this.transform.Find("ZoneHealth/View_Health/Image_Title/Text_Title").GetComponent<Text>();
            this.diseaseDesc = this.transform.Find("ZoneHealth/View_Health/Text_Des").GetComponent<Text>();
            this.rewardParent = this.transform.Find("ZoneHealth/View_Health/Scroll_View/Viewport");
        }

        protected override void OnOpen(object arg) {
            uint id = Convert.ToUInt32(arg);
            Sys_NPCFavorability.Instance.TryGetNpc(id, out this.npc, false);
        }

        protected override void OnShow() {
            if (this.npc.isSickness) {
                CSVNPCDisease.Data csvDisease = CSVNPCDisease.Instance.GetConfData(this.npc.healthId);
                if (csvDisease != null) {
                    TextHelper.SetText(this.diseaseTitle, csvDisease.Name);
                    TextHelper.SetText(this.diseaseDesc, csvDisease.Des);
                }
            }
            else {
                // 2是健康
                CSVNPCHealth.Data csvHealth = CSVNPCHealth.Instance.GetConfData(2);
                if (csvHealth != null) {
                    TextHelper.SetText(this.diseaseTitle, csvHealth.Name);
                    TextHelper.SetText(this.diseaseDesc, csvHealth.Des);
                }
            }

            if (this.rewards == null) {
                this.rewards = new UI_RewardList(this.rewardParent, EUIID.UI_FavorabilityHealth);
            }

            this.rewards.SetRewardList(this.npc.Drugs);
            this.rewards.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);
        }
    }
}