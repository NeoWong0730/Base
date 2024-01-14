using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // 送礼
    public class UI_FavorabilitySendGift : UIBase {
        public class Tab : UISelectableElement {
            public CP_Toggle toggle;
            public Text tabNameLight;
            public Text tabNameDark;

            public int tabIndex = 0;

            protected override void Loaded() {
                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);

                this.tabNameLight = this.transform.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
                this.tabNameDark = this.transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            }
            public void Refresh(int tabIndex) {
                this.tabIndex = tabIndex;
                if (tabIndex == 1) {
                    TextHelper.SetText(this.tabNameLight, 2010605);
                    TextHelper.SetText(this.tabNameDark, 2010605);
                }
                else {
                    TextHelper.SetText(this.tabNameLight, 2010606);
                    TextHelper.SetText(this.tabNameDark, 2010606);
                }
            }
            public void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke(this.tabIndex, true);
                }
            }
            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }
        }
        public FavorabilityNPC npc;

        public Button btnExit;
        public Button btnSendGift;
        public Button btnFilterGift;
        public Button btnLikeDesc;

        public Transform likeProto;

        public InfinityGrid infinity;

        public GameObject tabProto;
        public UIElementCollector<Tab> tabVds = new UIElementCollector<Tab>();
        public PropItem propItem;
        public PropIconLoader.ShowItemDataExt selectedCellData = new PropIconLoader.ShowItemDataExt();
        public Animator selectedParent;

        public Text npcFavorability;
        public Text playerFavorability;
        public Text maxLevel;
        public Text remainCountText;

        public GameObject npcFavorabilityGo;
        public GameObject playerFavorabilityGo;

        public Text totalFavorability;
        public Button btnPlayerFavorabilityInfo;

        public int currentTabId = 0;
        public List<PropIconLoader.ShowItemDataExt> cellDatas = new List<PropIconLoader.ShowItemDataExt>();
        public List<uint> filters = new List<uint>();
        public int selectedGiftPropItemId = -1;
        public int selectedGiftPropItemIndex = -1;
        public MessageBoxEvt boxEvent = new MessageBoxEvt();
        public Text sendGiftTotalTimes;

        public COWVd<UI_FavorabilityNPCShow.Gift> gifts = new COWVd<UI_FavorabilityNPCShow.Gift>();

        public enum EReason {
            None,
            NoSelected, // 没有选中物品
            LessRemainCount, // 剩余次数不足，
            LessGoods, // 缺少物品
            LessPlayerFavorability, // 体力不足
            ReachedMax, // 已到达最大值
        }
        public EReason reason = EReason.None;

        public void OnBtnExitClicked() {
            this.CloseSelf();
        }
        public void OnBtnPlayerFavorabilityInfoClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityTip);
        }
        public void OnBtnSendGiftClicked() {
            //this.RefreshAll();
            //return;

            // 赠礼
            if (this.selectedGiftPropItemId == -1 || this.selectedGiftPropItemIndex == -1) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010609));
            }
            else if (this.reason == EReason.LessRemainCount) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010610));
            }
            else if (this.reason == EReason.LessPlayerFavorability) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010611));
            }
            else if (this.reason == EReason.ReachedMax) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010612));
            }
            else if (this.reason == EReason.LessGoods) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010613));
            }
            else {
                var cellData = this.cellDatas[this.selectedGiftPropItemIndex];
                PropIconLoader.ShowItemDataExt ext = cellData as PropIconLoader.ShowItemDataExt;
                if (ext != null) {
                    
                    void _DoEquip(PropIconLoader.ShowItemDataExt data) {
                        if (data.CSV.type_id == (uint) EItemType.Equipment && data.Quality >= 4) {
                            void OnConform() {
                                _Do(data);
                            }

                            PromptBoxParameter.Instance.Clear();
                            PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(2010661).words;
                            PromptBoxParameter.Instance.SetConfirm(true, OnConform, 2010663);
                            PromptBoxParameter.Instance.SetCancel(true, null, 2010662);
                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        }
                        else {
                            _Do(data);
                        }
                    }

                    void _Do(PropIconLoader.ShowItemDataExt data) {
                        if (data.useStack) {
                            Sys_NPCFavorability.Instance.ReqCmdFavorabilityAddValue((uint)EFavorabilityBahaviourType.SendGift, this.npc.id, data.id);
                        }
                        else {
                            Sys_NPCFavorability.Instance.ReqCmdFavorabilityAddValue((uint)EFavorabilityBahaviourType.SendGift, this.npc.id, 0, data.guid);
                        }
                    }

                    _DoEquip(ext);
                }
            }
        }

        public class SendGiftArg {
            public HashSet<uint> hash = null;
            public Action<HashSet<uint>> callback = null;

            public SendGiftArg(Action<HashSet<uint>> callback) {
                this.callback = callback;
            }

            public void Invoke() {
                this.callback?.Invoke(this.hash);
            }
        }

        public SendGiftArg sendGiftArg;
        public void OnBtnFilterGiftClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityFilterGifts, false, new SendGiftArg(this.OnFilterGift));
        }

        public void OnBtnLikeDescClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityNpcLikeDesc, false, this.npc.id);
        }

        private void OnFilterGift(HashSet<uint> arg) {
            this.filters = new List<uint>(arg);
            this.selectedGiftPropItemId = -1;
            this.selectedGiftPropItemIndex = -1;
            this.RefreshAll();
        }

        protected override void OnLoaded() {
            this.tabProto = this.transform.Find("Animator/View_Give/Menu/Proto").gameObject;
            this.selectedParent = this.transform.Find("Animator/Selected").GetComponent<Animator>();

            this.btnExit = this.transform.Find("Animator/View_Title10/Btn_Close").GetComponent<Button>();
            this.btnSendGift = this.transform.Find("Animator/View_Give/Button_Fete/Button").GetComponent<Button>();
            this.btnFilterGift = this.transform.Find("Animator/View_Give/Button_Screen").GetComponent<Button>();
            this.btnLikeDesc = this.transform.Find("Animator/View_Give/Object_Type/Button").GetComponent<Button>();
            this.infinity = this.transform.Find("Animator/View_Give/Scroll_View").GetComponent<InfinityGrid>();

            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);
            this.btnSendGift.onClick.AddListener(this.OnBtnSendGiftClicked);
            this.btnFilterGift.onClick.AddListener(this.OnBtnFilterGiftClicked);
            this.btnLikeDesc.onClick.AddListener(this.OnBtnLikeDescClicked);

            this.npcFavorability = this.transform.Find("Animator/View_Give/Button_Fete/Text_Add/Text_Number").GetComponent<Text>();
            this.playerFavorability = this.transform.Find("Animator/View_Give/Button_Fete/Text_Point/Text_Number").GetComponent<Text>();

            this.npcFavorabilityGo = this.transform.Find("Animator/View_Give/Button_Fete/Text_Add").gameObject;
            this.playerFavorabilityGo = this.transform.Find("Animator/View_Give/Button_Fete/Text_Point").gameObject;

            this.maxLevel = this.transform.Find("Animator/View_Give/Text_Full").GetComponent<Text>();
            this.remainCountText = this.transform.Find("Animator/View_Give/Text_Amount/Text").GetComponent<Text>();

            this.totalFavorability = this.transform.Find("Animator/Text/Text_Number").GetComponent<Text>();

            this.likeProto = this.transform.Find("Animator/View_Give/Object_Type/Scroll_View/Viewport/Item");
            this.sendGiftTotalTimes = this.transform.Find("Animator/Text_All").GetComponent<Text>();

            this.btnPlayerFavorabilityInfo = this.transform.Find("Animator/Button_Tips").GetComponent<Button>();
            this.btnPlayerFavorabilityInfo.onClick.AddListener(this.OnBtnPlayerFavorabilityInfoClicked);

            this.infinity.onCreateCell += this.OnCreateCell;
            this.infinity.onCellChange += this.OnCellChange;
        }

        private void OnCreateCell(InfinityGridCell cell) {
            PropItem entry = new PropItem();
            var go = cell.mRootTransform.gameObject;
            entry.BindGameObject(go);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index) {
            var entry = cell.mUserData as PropItem;
            this.UpdateChildrenCallback(index, entry, cell.mRootTransform);
        }

        protected override void OnOpen(object arg) {
            uint id = Convert.ToUInt32(arg);
            Sys_NPCFavorability.Instance.TryGetNpc(id, out this.npc, false);
            
            this.selectedGiftPropItemId = -1;
            this.selectedGiftPropItemIndex = -1;
            this.currentTabId = 0;
        }

        protected override void OnShow() {
            this.RefreshAll();
        }

        public void RefreshFavorability() {
            // 是否到达当前阶段最大值
            bool isReachedCurrentStageLimit = this.npc.IsReachedCurrentStageLimit;
            this.maxLevel.gameObject.SetActive(isReachedCurrentStageLimit);
            if (this.selectedGiftPropItemId == -1) {
                this.npcFavorability.gameObject.SetActive(false);
                this.playerFavorability.gameObject.SetActive(false);

                this.playerFavorabilityGo.SetActive(false);
                this.npcFavorabilityGo.SetActive(false);
            }
            else {
                var cellData =  this.cellDatas[this.selectedGiftPropItemIndex];
                var csvGift = CSVFavorabilityGift.Instance.GetConfData(cellData.id);
                if (csvGift != null) {
                    bool isLike = this.npc.IsLikedGift(cellData.id);
                    uint baseFavorability = isLike ? csvGift.FavouriteFavorabilityValue : csvGift.IncreaseFavorabilityValue;
                    CSVNPCCharacter.Data csvCharacter = CSVNPCCharacter.Instance.GetConfData(this.npc.csvNPCFavorability.Character);
                    float value = 1f * baseFavorability * csvCharacter.FavorabilityRatio / 10000f;
                    CSVNPCMood.Data csvMode = CSVNPCMood.Instance.GetConfData(this.npc.moodId);
                    value = value * csvMode.FavorabilityRatio / 10000f;
                    value = Mathf.CeilToInt(value);

                    this.npcFavorability.text = "+" + value.ToString();
                }

                bool show = !isReachedCurrentStageLimit && csvGift != null;
                this.playerFavorabilityGo.SetActive(show);
                this.npcFavorabilityGo.SetActive(show);
                this.npcFavorability.gameObject.SetActive(show);
                this.playerFavorability.gameObject.SetActive(show);
            }

            this.totalFavorability.text = Sys_NPCFavorability.Instance.Favorability.ToString();

            uint remainActCount = this.npc.RemainActTime(EFavorabilityBahaviourType.SendGift);
            this.remainCountText.text = remainActCount.ToString();
            CSVFavorabilityBehavior.Data csvBehaviour = CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.SendGift);
            this.playerFavorability.text = "-" + csvBehaviour.CostPoint.ToString();

            bool enoughGoods = false;
            if (this.selectedGiftPropItemId != -1 && this.selectedGiftPropItemIndex != -1) {
                var cellData =  this.cellDatas[this.selectedGiftPropItemIndex];
                enoughGoods = this.npc.HasFullGifts(cellData, (cellData as PropIconLoader.ShowItemDataExt).neededCount);
            }
            bool enoughPlayerFavorability = Sys_NPCFavorability.Instance.IsEnoughFavorability(csvBehaviour.CostPoint);
            bool hasRemainCount = remainActCount > 0;

            this.reason = EReason.None;
            bool valid = (enoughGoods && enoughPlayerFavorability && hasRemainCount && !isReachedCurrentStageLimit);
            ImageHelper.SetImageGray(this.btnSendGift, !valid, true);
            
            this.propItem?.SetActive(false);
            if (valid) {
                var cellData = this.cellDatas[this.selectedGiftPropItemIndex];
                if (this.propItem == null) {
                    this.propItem = PropIconLoader.GetAsset(selectedCellData, this.selectedParent.transform);
                    (propItem.transform as RectTransform).anchoredPosition = Vector3.zero;
                }

                this.selectedCellData.Refresh(cellData.id, cellData.count, true, cellData.bBind, cellData.bNew, cellData.bUnLock, cellData.bSelected, cellData.bShowCount, cellData.bShowBagCount, false, null, cellData.bShowBtnNo, false);
                this.selectedCellData.SetQuality(cellData.Quality);
                this.propItem?.SetData(this.selectedCellData, EUIID.UI_FavorabilitySendGift);
                
                this.propItem?.SetActive(true);
                this.selectedParent.Play("Selected", -1, 0);
            }

            if (!hasRemainCount) {
                this.reason = EReason.LessRemainCount;
            }
            else if (!enoughPlayerFavorability) {
                this.reason = EReason.LessPlayerFavorability;
            }
            else if (isReachedCurrentStageLimit) {
                this.reason = EReason.ReachedMax;
            }
            else if (!enoughGoods) {
                this.reason = EReason.LessGoods;
            }
        }
        public void RefreshLikes() {
            this.gifts.TryBuildOrRefresh(this.likeProto.gameObject, this.likeProto.parent, this.npc.likesGiftTypes.Count, (vd, vdIndex) => {
                vd.Refresh(this.npc.id, this.npc.likesGiftTypes[vdIndex]);
            });
        }

        private readonly List<int> tabIds = new List<int>() { 1, 2 };
        public void RefreshAll() {
            var ids = this.tabIds;
            this.tabVds.BuildOrRefresh<int>(this.tabProto, this.tabProto.transform.parent, ids, (vd, id, indexOfVdList) => {
                vd.SetUniqueId(id);
                vd.SetSelectedAction((innerId, force) => {
                    if (this.currentTabId != innerId) {
                        this.selectedGiftPropItemId = -1;
                        this.selectedGiftPropItemIndex = -1;
                    }

                    this.currentTabId = innerId;
                    this.GetCellDatas();

                    var leftIndex =  this.cellDatas.FindIndex((ele) => {
                        return ele.id == this.selectedGiftPropItemId;
                    });
                    var rightIndex =  this.cellDatas.FindLastIndex((ele) => {
                        return ele.id == this.selectedGiftPropItemId;
                    });
                    
                    if (leftIndex == -1) {
                        // 送完了
                        this.selectedGiftPropItemId = -1;
                        this.selectedGiftPropItemIndex = -1;
                    }
                    else {
                        if(rightIndex != leftIndex) {
                            // 多个相同id的item
                            this.selectedGiftPropItemId = -1;
                            this.selectedGiftPropItemIndex = -1;
                        }
                        else {
                            // 没送完，但是位置有可能有变动
                            this.selectedGiftPropItemIndex = leftIndex;
                        }
                    }

                    this.RefreshFavorability();
                    this.RefreshGrid();

                    if (this.selectedGiftPropItemId == -1) {
                        this.infinity.MoveToIndex(0);
                    }
                });
                vd.Refresh(id);
            });
            // 默认选中Tab
            if (ids.Count > 0) {
                if (this.currentTabId <= 0 || !ids.Contains(this.currentTabId)) {
                    this.currentTabId = ids[0];
                }
                this.tabVds[this.currentTabId - 1].SetSelected(true, true);
            }

            this.RefreshLikes();
            
            var usedTotal = Logic.FavorabilityNPC.UsedTotalActTimes(EFavorabilityBahaviourType.SendGift);
            CSVFavorabilityBehavior.Data csvBe = CSVFavorabilityBehavior.Instance.GetConfData((uint)EFavorabilityBahaviourType.SendGift);
            var total = csvBe != null ? csvBe.DailyTotal : usedTotal;
            var leftTimes = (long) total - (long) usedTotal;
            TextHelper.SetText(this.sendGiftTotalTimes, 2010660, leftTimes.ToString());
        }

        public void UpdateChildrenCallback(int index, PropItem entry, Transform trans) {
            if (0 <= index && index < this.cellDatas.Count) {
                var cellData = this.cellDatas[index];
                cellData.bUseClick = true;
                cellData.bUseTips = false;
                cellData.bUseQuailty = true;
                entry.SetData(cellData, EUIID.UI_FavorabilitySendGift);

                entry.imgLike.SetActive(this.npc.IsLikedGift(cellData.id));
                bool sel = cellData.id == this.selectedGiftPropItemId && this.selectedGiftPropItemIndex == index;
                if (sel) {
                    this.lastSelectedGiftPropItem = entry;
                }

                entry.SetSelected(sel);
                // 刷新个数
                entry.RefreshLRCount(cellData.count, cellData.neededCount);

                cellData.onclick = (propItem) => {
                    int idx = index;
                    this.OnCellClicked(propItem, idx);
                };
            }
            else {
                if (entry == null) {
                    return;
                }

                if (entry.ItemData == null) {
                    entry.SetEmpty(true);
                }
                else {
                    var quality = entry.ItemData.Quality;
                    entry.SetEmpty(true);
                    entry.ItemData.SetQuality(quality);
                }
            }
        }

        private PropItem lastSelectedGiftPropItem = null;
        private void OnCellClicked(PropItem propItem, int index) {
            this.lastSelectedGiftPropItem?.SetSelected(false);
            this.selectedGiftPropItemId = (int)propItem.ItemData.id;
            this.selectedGiftPropItemIndex = index;

            this.boxEvent.Reset(EUIID.UI_FavorabilitySendGift, propItem.ItemData);

            if (this.cellDatas[index].bagData.cSVItemData.type_id == (uint)EItemType.Equipment) {
                EquipTipsData tipData = new EquipTipsData();
                tipData.equip = this.cellDatas[index].bagData;
                tipData.isCompare = false;
                tipData.isShowOpBtn = false;

                UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
            }
            else {
                PropMessageParam propParam = new PropMessageParam();
                propParam.itemData = this.cellDatas[index].bagData;
                propParam.needShowInfo = false;
                propParam.needShowMarket = false;
                propParam.sourceUiId = EUIID.UI_FavorabilitySendGift;
                UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
            }

            propItem.SetSelected(true);
            this.lastSelectedGiftPropItem = propItem;

            this.RefreshFavorability();
        }

        private void GetCellDatas() {
            float GetIncreaseFavorability(uint giftId, bool isLike = true) {
                var csvGift = CSVFavorabilityGift.Instance.GetConfData(giftId);
                uint baseFavorability = isLike ? csvGift.FavouriteFavorabilityValue : csvGift.IncreaseFavorabilityValue;
                CSVNPCCharacter.Data csvCharacter = CSVNPCCharacter.Instance.GetConfData(this.npc.csvNPCFavorability.Character);
                float value = 1f * baseFavorability * csvCharacter.FavorabilityRatio / 10000f;
                CSVNPCMood.Data csvMode = CSVNPCMood.Instance.GetConfData(this.npc.moodId);
                value = value * csvMode.FavorabilityRatio / 10000f;
                value = Mathf.CeilToInt(value);
                return value;
            }

            // todo: 效率可能存在问题
            if (this.currentTabId == 1) {
                this.cellDatas = this.npc.GiftsInBag;

                this.cellDatas.Sort((left, right) => {
                    bool isLeftLike = this.npc.IsLikedGift(left.id);
                    bool isRightLike = this.npc.IsLikedGift(right.id);
                    float leftValue = GetIncreaseFavorability(left.id, isLeftLike);
                    float rightValue = GetIncreaseFavorability(right.id, isRightLike);
                    return (int)(rightValue - leftValue);
                });
            }
            else {
                this.cellDatas = this.npc.LikeGiftsInBag;

                this.cellDatas.Sort((left, right) => {
                    float leftValue = GetIncreaseFavorability(left.id);
                    float rightValue = GetIncreaseFavorability(right.id);
                    return (int)(rightValue - leftValue);
                });
            }

            this.cellDatas = FavorabilityNPC.FilterByGiftType(this.cellDatas, this.filters);
        }
        private void RefreshGrid() {
            this.infinity.CellCount = Math.Max(this.cellDatas.Count, 20);
            this.infinity.ForceRefreshActiveCell();
        }
        protected override void ProcessEvents(bool toRegister) {
            Sys_NPCFavorability.Instance.eventEmitter.Handle(Sys_NPCFavorability.EEvents.OnPlayerFavorabilityChanged, this.OnPlayerFavorabilityChanged, toRegister);
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint, uint, uint>(Sys_NPCFavorability.EEvents.OnNpcFavorabilityChanged, this.OnNpcFavorabilityChanged, toRegister);
        }
        private void OnPlayerFavorabilityChanged() {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.lastSelectedGiftPropItem?.SetSelected(false);
                this.RefreshAll();
            }
        }
        private void OnNpcFavorabilityChanged(uint npcId, uint stageId, uint from, uint to) {
            if (this.gameObject != null && this.gameObject.activeInHierarchy && npcId == this.npc.id) {
                this.lastSelectedGiftPropItem?.SetSelected(false);
                this.RefreshAll();
                //this.CloseSelf();
            }
        }
    }
}