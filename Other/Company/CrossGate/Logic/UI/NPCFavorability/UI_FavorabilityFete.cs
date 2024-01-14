using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityFete : UIBase {
        public class Tab : UISelectableElement {
            public CP_Toggle toggle;
            public Text tabNameLight;
            public Text tabNameDark;

            public uint tabId = 0;

            protected override void Loaded() {
                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);

                this.tabNameLight = this.transform.Find("Object_Selected/Text").GetComponent<Text>();
                this.tabNameDark = this.transform.Find("Object/Text").GetComponent<Text>();
            }
            public void Refresh(uint tabId) {
                this.tabId = tabId;
                var csv = CSVFavorabilityBanquet.Instance.GetConfData(tabId);
                if (csv != null) {
                    TextHelper.SetText(this.tabNameLight, csv.Name);
                    TextHelper.SetText(this.tabNameDark, csv.Name);
                }
            }
            public void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke((int)this.tabId, true);
                }
            }
            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }
        }

        public Button btnExit;
        public Text remainCount;
        public Text playerFavorability;
        public Text desc;

        public Button btnInvitation;
        public Button btnNavTo;

        public GameObject maxLevel;
        public Text npcFavorability;
        public Text playerDescFavorability;

        public GameObject npcFavorabilityGo;
        public GameObject playerFavorabilityGo;

        public Transform rewardParent;
        public UI_RewardList rewards;

        public Button btnPlayerFavorabilityInfo;

        public GameObject tabProto;
        public UIElementCollector<Tab> tabVds = new UIElementCollector<Tab>();

        public FavorabilityNPC npc;
        public int currentTabId = 0;
        public UI_FavorabilitySendGift.EReason reason;
        public List<uint> tabIds;

        public void OnBtnExitClicked() {
            this.CloseSelf();
        }
        public void OnBtnPlayerFavorabilityInfoClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityTip);
        }
        public void OnBtnInvitationClicked() {
            if (this.reason == UI_FavorabilitySendGift.EReason.LessRemainCount) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010610));
            }
            else if (this.reason == UI_FavorabilitySendGift.EReason.LessPlayerFavorability) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010611));
            }
            else if (this.reason == UI_FavorabilitySendGift.EReason.ReachedMax) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010612));
            }
            else if (this.reason == UI_FavorabilitySendGift.EReason.LessGoods) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010613));
            }
            else {
                Sys_NPCFavorability.Instance.ReqCmdFavorabilityAddValue((uint)EFavorabilityBahaviourType.Fete, this.npc.id, 0, 0, (uint)this.currentTabId);
            }
        }

        public void OnBtnNavToClicked() {
            void OnConform() {
                UIManager.ClearUntilMain();
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(1500003u);
            }
            
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(2010665).words;
            PromptBoxParameter.Instance.SetConfirm(true, OnConform, 2010666);
            PromptBoxParameter.Instance.SetCancel(true, null, 2010667);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        protected override void OnLoaded() {
            this.playerFavorability = this.transform.Find("Animator/Text/Text_Number").GetComponent<Text>();
            this.remainCount = this.transform.Find("Animator/View_Fete/Text_Amount/Text_Number").GetComponent<Text>();
            this.desc = this.transform.Find("Animator/View_Fete/Text_Amount/Text_Des").GetComponent<Text>();

            this.btnExit = this.transform.Find("Animator/View_Title10/Btn_Close").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);

            this.btnPlayerFavorabilityInfo = this.transform.Find("Animator/Button_Tips").GetComponent<Button>();
            this.btnPlayerFavorabilityInfo.onClick.AddListener(this.OnBtnPlayerFavorabilityInfoClicked);

            this.maxLevel = this.transform.Find("Animator/View_Fete/Text_Full").gameObject;
            this.playerDescFavorability = this.transform.Find("Animator/View_Fete/Button_Fete/Text_Point/Text_Number").GetComponent<Text>();
            this.npcFavorability = this.transform.Find("Animator/View_Fete/Button_Fete/Text_Add/Text_Number").GetComponent<Text>();

            this.npcFavorabilityGo = this.transform.Find("Animator/View_Fete/Button_Fete/Text_Add").gameObject;
            this.playerFavorabilityGo = this.transform.Find("Animator/View_Fete/Button_Fete/Text_Point").gameObject;

            this.btnInvitation = this.transform.Find("Animator/View_Fete/Button_Fete/Button").GetComponent<Button>();
            this.btnInvitation.onClick.AddListener(this.OnBtnInvitationClicked);
            
            this.btnNavTo = this.transform.Find("Animator/View_Fete/Button_Shop").GetComponent<Button>();
            this.btnNavTo.onClick.AddListener(this.OnBtnNavToClicked);

            this.rewardParent = this.transform.Find("Animator/View_Fete/Scroll_View/Viewport");
            this.tabProto = this.transform.Find("Animator/View_Fete/ScrollView_Menu/List/Toggle").gameObject;
        }
        protected override void OnOpen(object arg) {
            uint id = Convert.ToUInt32(arg);
            Sys_NPCFavorability.Instance.TryGetNpc(id, out this.npc, false);

            this.tabIds = npc.csvNPCFavorability.BanqueID;
            
            this.currentTabId = 0;
        }
        protected override void OnShow() {
            this.RefreshAll();
        }

        public void RefreshAll() {
            this.tabVds.BuildOrRefresh<uint>(this.tabProto, this.tabProto.transform.parent, this.tabIds, (vd, id, indexOfVdList) => {
                vd.SetUniqueId((int)id);
                vd.SetSelectedAction((innerId, force) => {
                    this.currentTabId = innerId;

                    this.RefreshFavorability();
                    this.RefreshGrid();
                });
                vd.Refresh(id);
            });

            if (this.tabVds.CorrectId(ref this.currentTabId, this.tabIds)) {
                if (this.tabVds.TryGetVdById(this.currentTabId, out var vd)) {
                    vd.SetSelected(true, true);
                }
            }
        }
        public void RefreshFavorability() {
            bool isReachedCurrentStageLimit = this.npc.IsReachedCurrentStageLimit;
            this.maxLevel.gameObject.SetActive(isReachedCurrentStageLimit);
            var csvBanquet = CSVFavorabilityBanquet.Instance.GetConfData((uint)this.currentTabId);
            if (csvBanquet != null) {
                CSVNPCCharacter.Data csvCharacter = CSVNPCCharacter.Instance.GetConfData(this.npc.csvNPCFavorability.Character);
                float value = 1f * csvBanquet.FavorabilityValue * csvCharacter.FavorabilityRatio / 10000f;
                CSVNPCMood.Data csvMode = CSVNPCMood.Instance.GetConfData(this.npc.moodId);
                value = value * csvMode.FavorabilityRatio / 10000f;
                value = Mathf.CeilToInt(value);

                this.npcFavorability.text = "+" + value.ToString();
                TextHelper.SetText(this.desc, csvBanquet.Des);
            }

            bool show = !isReachedCurrentStageLimit && csvBanquet != null;
            this.npcFavorabilityGo.SetActive(show);
            this.playerFavorabilityGo.SetActive(show);
            this.npcFavorability.gameObject.SetActive(show);
            this.playerDescFavorability.gameObject.SetActive(show);

            this.playerFavorability.text = Sys_NPCFavorability.Instance.Favorability.ToString();

            uint remainActCount = this.npc.RemainActTime(EFavorabilityBahaviourType.Fete);
            this.remainCount.text = remainActCount.ToString();
            CSVFavorabilityBehavior.Data csvBehaviour = CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.Fete);
            this.playerDescFavorability.text = "-" + csvBehaviour.CostPoint.ToString();

            var goods = FavorabilityNPC.GetFoods((uint)this.currentTabId);
            bool enoughGoods = ItemIdCount.IsEnough(goods);
            bool enoughPlayerFavorability = Sys_NPCFavorability.Instance.IsEnoughFavorability(csvBehaviour.CostPoint);
            bool hasRemainCount = remainActCount > 0;

            this.reason = UI_FavorabilitySendGift.EReason.None;
            ImageHelper.SetImageGray(this.btnInvitation, !(enoughGoods && enoughPlayerFavorability && hasRemainCount && !isReachedCurrentStageLimit), true);
            if (!hasRemainCount) {
                this.reason = UI_FavorabilitySendGift.EReason.LessRemainCount;
            }
            else if (!enoughPlayerFavorability) {
                this.reason = UI_FavorabilitySendGift.EReason.LessPlayerFavorability;
            }
            else if (isReachedCurrentStageLimit) {
                this.reason = UI_FavorabilitySendGift.EReason.ReachedMax;
            }
            else if (!enoughGoods) {
                this.reason = UI_FavorabilitySendGift.EReason.LessGoods;
            }
        }
        public void RefreshGrid() {
            if (this.rewards == null) {
                this.rewards = new UI_RewardList(this.rewardParent, EUIID.UI_FavorabilityFete);
            }
            var ls = FavorabilityNPC.GetFoods((uint)this.currentTabId);
            this.rewards.SetRewardList(ls);
            this.rewards.Build(true, false, false, false, false, true, true, true, PropItem.OnClickPropItem, false);
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
            if (this.gameObject != null && npcId == this.npc.id && this.gameObject.activeInHierarchy) {
                // this.RefreshAll();
                this.CloseSelf();
            }
        }
        private void OnItemCountChanged(int changeType, int curBoxId) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.RefreshAll();
            }
        }
    }
}