using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // 好感度：感谢信
    public class UI_FavorabilityThanks : UIBase {
        public Button btnExit;
        public Text title;
        public Text content;
        public Text totalFavorability;

        public Transform protoParent;
        public Button btn;
        public Button btnPlayerFavorabilityInfo;

        private void OnBtnClicked() {
            Sys_NPCFavorability.Instance.ReqCmdFavorabilityStageUp(this.npc.id);
        }

        public void OnBtnExitClicked() {
            this.CloseSelf();
        }

        public void OnBtnPlayerFavorabilityInfoClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityTip);
        }

        protected override void OnLoaded() {
            this.btnExit = this.transform.Find("Animator/View_Title10/Btn_Close").GetComponent<Button>();
            this.btn = this.transform.Find("Animator/View/Button_Fete/Button").GetComponent<Button>();

            this.btnPlayerFavorabilityInfo = this.transform.Find("Animator/Button_Tips").GetComponent<Button>();
            this.btnPlayerFavorabilityInfo.onClick.AddListener(this.OnBtnPlayerFavorabilityInfoClicked);

            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);
            this.btn.onClick.AddListener(this.OnBtnClicked);

            this.title = this.transform.Find("Animator/View/Image/Text").GetComponent<Text>();
            this.content = this.transform.Find("Animator/View/ImageBG/Text").GetComponent<Text>();
            this.protoParent = this.transform.Find("Animator/View/Scroll_View/Viewport");

            this.totalFavorability = this.transform.Find("Animator/Text/Text_Number").GetComponent<Text>();
        }

        public UI_RewardList rewardList;

        public FavorabilityNPC npc;
        public List<ItemIdCount> rewardLs;

        protected override void OnOpen(object arg) {
            uint id = Convert.ToUInt32(arg);
            Sys_NPCFavorability.Instance.TryGetNpc(id, out this.npc, false);

            uint dropId = this.npc.csvNPCFavorabilityStage.Reward;
            this.rewardLs = CSVDrop.Instance.GetDropItem(dropId);
        }

        protected override void OnShow() {
            if (this.npc.csvNPCFavorabilityStage.RewardType == (uint) EFavorabilityLetterRewardType.Drop) {
                if (this.rewardList == null) {
                    this.rewardList = new UI_RewardList(this.protoParent, EUIID.UI_FavorabilityThanks);
                }

                this.rewardList.SetRewardList(this.rewardLs);
                this.rewardList.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);
                this.rewardList.Show(true);
            }
            else {
                this.rewardList?.Show(false);
            }

            this.totalFavorability.text = Sys_NPCFavorability.Instance.Favorability.ToString();

            CSVNpc.Data csvNpc = CSVNpc.Instance.GetConfData(this.npc.id);
            if (csvNpc != null) {
                TextHelper.SetText(this.title, 2010614, LanguageHelper.GetNpcTextContent(csvNpc.name));
            }
            else {
                TextHelper.SetText(this.title, "匿名");
            }

            this.btn.gameObject.SetActive(true);
            var csvTask = CSVTask.Instance.GetConfData(this.npc.csvNPCFavorabilityStage.WishTask);
            if (csvTask != null) {
                TextHelper.SetText(this.content, this.npc.csvNPCFavorabilityStage.LetterLan, Sys_Role.Instance.sRoleName, LanguageHelper.GetTaskTextContent(csvTask.taskName));
            }
            else {
                TextHelper.SetText(this.content, "已接受");
                this.btn.gameObject.SetActive(false);
            }
        }

        protected override void ProcessEvents(bool toRegister) {
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint, uint>(Sys_NPCFavorability.EEvents.OnFavorabilityStageChanged, this.OnFavorabilityStageChanged, toRegister);
        }

        private void OnFavorabilityStageChanged(uint npcId, uint oldStageId, uint newStageId) {
            if (this.gameObject != null && npcId == this.npc.id) {
                this.CloseSelf();
            }
        }
    }
}