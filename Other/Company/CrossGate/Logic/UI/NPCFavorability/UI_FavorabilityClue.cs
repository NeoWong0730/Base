using System;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // ����
    public class UI_FavorabilityClue : UIBase {
        public FavorabilityNPC npc;

        public Button btnExit;
        public Button btnClue;
        public Text diseaseDesc;
        public Text npcFavorability;
        public Text playerFavorability;
        public Text totalFavorability;

        public GameObject npcFavorabilityGo;
        public GameObject playerFavorabilityGo;

        public Button btnPlayerFavorabilityInfo;

        public Transform rewardParent;

        public UI_RewardList rewards;

        public enum EReason {
            None,
            LessRemainCount, // ʣ��������㣬
            LessGoods, // ȱ����Ʒ
            LessPlayerFavorability, // ��������
            ReachedMax, // �ѵ������ֵ
        }
        public EReason reason = EReason.None;

        public void OnBtnExitClicked() {
            this.CloseSelf();
        }
        public void OnBtnClueClicked() {
            if (this.reason == EReason.LessPlayerFavorability) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010611));
            }
            else if (this.reason == EReason.LessGoods) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010613));
            }
            else {
                Sys_NPCFavorability.Instance.ReqCmdFavorabilityAddValue((uint)EFavorabilityBahaviourType.Clue, this.npc.id);
            }
        }
        public void OnBtnPlayerFavorabilityInfoClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityTip);
        }

        protected override void OnLoaded() {
            this.btnExit = this.transform.Find("Animator/View_Title10/Btn_Close").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);
            this.btnClue = this.transform.Find("Animator/View_Heal/Button_Fete/Button").GetComponent<Button>();
            this.btnClue.onClick.AddListener(this.OnBtnClueClicked);
            this.btnPlayerFavorabilityInfo = this.transform.Find("Animator/Button_Tips").GetComponent<Button>();
            this.btnPlayerFavorabilityInfo.onClick.AddListener(this.OnBtnPlayerFavorabilityInfoClicked);

            this.npcFavorability = this.transform.Find("Animator/View_Heal/Button_Fete/Text_Add/Text_Number").GetComponent<Text>();
            this.playerFavorability = this.transform.Find("Animator/View_Heal/Button_Fete/Text_Point/Text_Number").GetComponent<Text>();
            this.totalFavorability = this.transform.Find("Animator/Text/Text_Number").GetComponent<Text>();

            this.npcFavorabilityGo = this.transform.Find("Animator/View_Heal/Button_Fete/Text_Add").gameObject;
            this.playerFavorabilityGo = this.transform.Find("Animator/View_Heal/Button_Fete/Text_Point").gameObject;

            this.diseaseDesc = this.transform.Find("Animator/View_Heal/ImageBG/Text").GetComponent<Text>();
            this.rewardParent = this.transform.Find("Animator/View_Heal/Scroll_View/Viewport");
        }
        protected override void OnOpen(object arg) {
            uint id = Convert.ToUInt32(arg);
            Sys_NPCFavorability.Instance.TryGetNpc(id, out this.npc, false);
        }

        protected override void OnShow() {
            this.RefreshAll();
        }
        public void RefreshAll() {
            CSVNPCDisease.Data csvDisease = CSVNPCDisease.Instance.GetConfData(this.npc.healthId);
            if (csvDisease != null) {
                TextHelper.SetText(this.diseaseDesc, csvDisease.Des);
                this.playerFavorabilityGo.SetActive(true);

                CSVNPCCharacter.Data csvCharacter = CSVNPCCharacter.Instance.GetConfData(this.npc.csvNPCFavorability.Character);
                uint r = csvCharacter == null ? 0 : csvCharacter.FavorabilityRatio;
                float value = 1f * csvDisease.FavorabilityValue * r / 10000f;
                CSVNPCMood.Data csvMode = CSVNPCMood.Instance.GetConfData(this.npc.moodId);
                r = csvMode == null ? 0 : csvMode.FavorabilityRatio;
                value = value * r / 10000f;
                value = Mathf.CeilToInt(value);

                this.npcFavorability.text = "+" + value.ToString();
            }
            else {
                TextHelper.SetText(this.diseaseDesc, "");
                this.playerFavorabilityGo.SetActive(false);
            }

            if (this.rewards == null) {
                this.rewards = new UI_RewardList(this.rewardParent, EUIID.UI_FavorabilityClue);
            }
            this.rewards.SetRewardList(this.npc.Drugs);
            this.rewards.Build(true, false, false, false, false, true, true, true, null, false, false, true);

            // ctrl button
            //uint remainActCount = npc.RemainActTime(EFavorabilityBahaviourType.Clue);
            CSVFavorabilityBehavior.Data csvBehaviour = CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.Clue);
            this.playerFavorability.text = "-" + csvBehaviour.CostPoint.ToString();

            this.totalFavorability.text = Sys_NPCFavorability.Instance.Favorability.ToString();

            bool enoughGoods = this.npc.HasFullDrugs;
            bool enoughPlayerFavorability = Sys_NPCFavorability.Instance.IsEnoughFavorability(csvBehaviour.CostPoint);

            this.reason = EReason.None;
            ImageHelper.SetImageGray(this.btnClue, !(enoughGoods && enoughPlayerFavorability), true);
            if (!enoughPlayerFavorability) {
                this.reason = EReason.LessPlayerFavorability;
            }
            else if (!enoughGoods) {
                this.reason = EReason.LessGoods;
            }
        }

        protected override void ProcessEvents(bool toRegister) {
            Sys_NPCFavorability.Instance.eventEmitter.Handle(Sys_NPCFavorability.EEvents.OnPlayerFavorabilityChanged, this.OnPlayerFavorabilityChanged, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint, uint, uint>(Sys_NPCFavorability.EEvents.OnNpcFavorabilityChanged, this.OnNpcFavorabilityChanged, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int,int>(Sys_Bag.EEvents.OnRefreshChangeData, this.OnItemCountChanged, toRegister);
        }
        private void OnPlayerFavorabilityChanged() {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.RefreshAll();
            }
        }
        private void OnNpcFavorabilityChanged(uint npcId, uint stageId, uint from, uint to) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy && npcId == this.npc.id) {
                this.CloseSelf();
            }
        }
        private void OnItemCountChanged(int changeType,int curBoxId) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.RefreshAll();
            }
        }
    }
}