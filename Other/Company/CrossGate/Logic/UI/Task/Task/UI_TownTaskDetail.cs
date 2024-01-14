using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_TownTaskDetail : UIBase, UI_TownTaskDetail.Layout.IListener {
        public class Tab : UISelectableElement {
            public Transform contentGo;
            public Text taskType;
            public Text taskGoal; // taskLimit
            public GameObject hasDone;

            public GameObject limitGo;
            public Text limitDesc;
            public Image bg;

            public CP_Toggle toggle;

            private void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke(this.index, true);
                }
            }

            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }

            protected override void Loaded() {
                this.taskType = this.transform.Find("Content/Text").GetComponent<Text>();
                this.taskGoal = this.transform.Find("Content/Text_Des").GetComponent<Text>();
                this.contentGo = this.transform.Find("Content");

                this.hasDone = this.transform.Find("Image_Finish").gameObject;
                this.limitGo = this.transform.Find("Lock").gameObject;
                this.limitDesc = this.transform.Find("Lock/Text").GetComponent<Text>();
                this.bg = this.transform.Find("Content/Image").GetComponent<Image>();

                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);
            }

            public Sys_TownTask.Task task;
            public int index;
            public bool isEquip = false;

            public void Refresh(Sys_TownTask.Task task, int index) {
                this.task = task;
                this.index = index;


                var csvNpc = CSVNpc.Instance.GetConfData(task.csv.FavorabilityNpc);
                string npcName = "";
                if (csvNpc != null) {
                    npcName = LanguageHelper.GetNpcTextContent(csvNpc.name);
                }

                isEquip = false;
                var csvType = CSVTownType.Instance.GetConfData(task.csvLibrary.TaskType);
                if (csvType != null) {
                    isEquip = csvType.forEquip;
                    TextHelper.SetText(this.taskType, csvType.TypeDesc);
                }

                uint quality = task.csvLibrary.TownTaskLv; // 低中高
                uint iconId = 60212;
                uint styleId = 155;
                if (quality == 2) {
                    iconId = 60211;
                    styleId = 154;
                }
                else if (quality == 3) {
                    iconId = 60210;
                    styleId = 153;
                }

                ImageHelper.SetIcon(bg, iconId);
                var csvStyle = CSVWordStyle.Instance.GetConfData(styleId);

                CSVItem.Data csvItem = null;
                if (isEquip) {
                    csvItem = CSVItem.Instance.GetConfData(task.csvLibrary.TaskConsumeEqpt);
                }
                else {
                    csvItem = CSVItem.Instance.GetConfData(task.csvLibrary.TaskConsumeItem);
                }

                string itemName = "";
                if (csvItem != null) {
                    itemName = LanguageHelper.GetTextContent(csvItem.name_id);

#if DEBUG_MODE
                    itemName += (csvItem.id + " " + task.id + " " + task.libraryId);
#endif
                }

                if (isEquip) {
                    string itemQuality = LanguageHelper.GetTextContent(1662000000 - 1 + task.csvLibrary.ConsumeEqptQuality);
                    string s = LanguageHelper.GetTextContent(1660000009, npcName, itemQuality, itemName);
                    TextHelper.SetText(this.taskGoal, s, csvStyle);
                }
                else {
                    string s = LanguageHelper.GetTextContent(1660000004, npcName, task.csvLibrary.ConsumeItemNum.ToString(), itemName);
                    TextHelper.SetText(this.taskGoal, s, csvStyle);
                }

                task.CanSimpleDo(out var reason);
                this.limitGo.SetActive(false);
                this.hasDone.SetActive(false);
                if (reason == Sys_TownTask.Task.EReason.HasFinish) {
                    this.hasDone.SetActive(true);
                    ImageHelper.SetImageGray(contentGo, true, true);
                }
                else if (reason == Sys_TownTask.Task.EReason.InvalidFavorityLevel) {
                    this.limitGo.SetActive(true);
                    string favorityStageName = LanguageHelper.GetTextContent(CSVFavorabilityStageName.Instance.GetConfData(task.csvLibrary.NeedFavorabilityLv).name);
                    TextHelper.SetText(this.limitDesc, 1660000005, npcName, favorityStageName);
                    ImageHelper.SetImageGray(contentGo, true, true);
                }
                else {
                    ImageHelper.SetImageGray(contentGo, false, true);
                }
            }
        }

        public class Layout : UILayoutBase {
            public Text title;
            public Text favorityValue;
            public Button btnFavorityHelp;

            // public Text cdDesc;
            public CDText cdText;
            public Button btnRefresh;
            public UI_CostItem costRefresh = new UI_CostItem();
            public ItemIdCount idCountRefresh = new ItemGuidCount();
            public Button btnReward;
            public Transform redDot;
            public Text contributeLevel;
            public Text lrLevel;
            public Slider expSlider;
            public GameObject hasMax;

            // 左侧
            public List<Transform> tabs = new List<Transform>(8);

            // 右侧
            public Text limtDesc;
            public Text costFavority;
            public Text taskReward;
            public Button btnSubmit;
            public Text btnSubmitText;

            public GameObject emptyScroll;
            public InfinityGrid infinityGrid;

            public void Parse(GameObject root) {
                this.Init(root);

                this.title = this.transform.Find("Animator/View_Title01/Text_Title").GetComponent<Text>();
                this.favorityValue = this.transform.Find("Animator/View_Top/Favority/Text").GetComponent<Text>();
                this.btnFavorityHelp = this.transform.Find("Animator/View_Top/Favority/Button_Tips").GetComponent<Button>();

                // this.cdDesc = this.transform.Find("").GetComponent<Text>();
                this.cdText = this.transform.Find("Animator/View_Top/Image_Time/Tex1").GetComponent<CDText>();
                hasMax = this.transform.Find("Animator/View_Top/Text_Full").gameObject;

                var t = this.transform.Find("Animator/View_Top/Image_Cost/Cost");
                this.costRefresh.SetGameObject(t.gameObject);
                this.btnRefresh = this.transform.Find("Animator/View_Top/Image_Cost/Button_Go").GetComponent<Button>();

                this.btnReward = this.transform.Find("Animator/View_Top/Gift").GetComponent<Button>();
                this.redDot = this.transform.Find("Animator/View_Top/Gift/RedDot");
                this.contributeLevel = this.transform.Find("Animator/View_Top/Text").GetComponent<Text>();
                this.lrLevel = this.transform.Find("Animator/View_Top/Text_Percent").GetComponent<Text>();
                this.expSlider = this.transform.Find("Animator/View_Top/Slider_Eg").GetComponent<Slider>();

                // 左侧
                t = this.transform.Find("Animator/RawImage/View_Left");
                this.tabs.Clear();
                for (int i = 0, length = t.childCount; i < length; ++i) {
                    var child = t.GetChild(i);
                    this.tabs.Add(child);
                }

                // this.tabs.Sort((l, r) => {
                //     int.TryParse(l.name, out int lNum);
                //     int.TryParse(r.name, out int rNum);
                //     return lNum - rNum;
                // });

                // 右侧
                this.limtDesc = this.transform.Find("Animator/RawImage/View_Right/Text_Task/Text_Des").GetComponent<Text>();
                this.costFavority = this.transform.Find("Animator/RawImage/View_Right/Cost/Text/Cost").GetComponent<Text>();

                this.taskReward = this.transform.Find("Animator/RawImage/View_Right/Award/Image/Image_Coin/Text_Cost").GetComponent<Text>();
                this.btnSubmit = this.transform.Find("Animator/RawImage/View_Right/Submit/Button_Go").GetComponent<Button>();
                this.btnSubmitText = this.transform.Find("Animator/RawImage/View_Right/Submit/Button_Go/Text_01").GetComponent<Text>();

                this.infinityGrid = this.transform.Find("Animator/RawImage/View_Right/Submit/Scroll_View").GetComponent<InfinityGrid>();
                this.emptyScroll = this.transform.Find("Animator/RawImage/View_Right/Submit/EmptyScroll").gameObject;
            }

            public void RegisterEvents(IListener listener) {
                this.btnFavorityHelp.onClick.AddListener(listener.OnBtnHelpClicked);
                this.btnRefresh.onClick.AddListener(listener.OnBtnRefreshClicked);
                this.btnReward.onClick.AddListener(listener.OnBtnRewardClicked);
                this.btnSubmit.onClick.AddListener(listener.OnBtnSubmitClicked);

                this.infinityGrid.onCreateCell += listener.OnCreateCell;
                this.infinityGrid.onCellChange += listener.OnCellChange;
                this.cdText.onTimeRefresh = listener.OnTimeRefresh;
            }

            public interface IListener {
                void OnBtnHelpClicked();
                void OnBtnRefreshClicked();
                void OnBtnRewardClicked();
                void OnBtnSubmitClicked();

                void OnCreateCell(InfinityGridCell cell);
                void OnCellChange(InfinityGridCell cell, int index);
                void OnTimeRefresh(Text text, float time, bool isEnd);
            }
        }

        public Layout layout = new Layout();

        public uint townId;
        public Sys_TownTask.Town town;
        public List<Tab> vds = new List<Tab>();
        public int selectTaskIndex = 0;

        public readonly List<ItemData> defaultList = new List<ItemData>(1) {
            new ItemData(0, 0, 2, 0, 0, false, false, null, null, 0),
        };

        public COW<PropIconLoader.ShowItemData> items = new COW<PropIconLoader.ShowItemData>();
        public int selectedItemIndex = 0;

        protected override void OnLoaded() {
            this.layout.Parse(this.gameObject);
            this.layout.RegisterEvents(this);
        }

        protected override void OnDestroy() {
            this.vds.Clear();
            _timer?.Cancel();
        }

        protected override void OnOpen(object arg) {
            if (arg is Tuple<uint, object> tp) {
                this.townId = Convert.ToUInt32(tp.Item2);
                Sys_TownTask.Instance.towns.TryGetValue(this.townId, out town);
            }
        }

        protected override void OnShow() {
            if (town == null) {
                return;
            }

            RefreshAll();
            if (this.vds.Count > 0) {
                this.vds[this.selectTaskIndex].SetSelected(true, true);
            }
        }

        protected override void OnOpened() {
            if (town == null) {
                return;
            }

            string s = LanguageHelper.GetTextContent(this.town.csvFavority.PlaceName);
            TextHelper.SetText(this.layout.title, 1660000003, s);

            vds.Clear();
            for (int i = 0, length = layout.tabs.Count; i < length; ++i) {
                var vd = new Tab();
                vd.SetSelectedAction(this.OnSelect);
                vd.Init(this.layout.tabs[i]);
                this.vds.Add(vd);
            }
        }

        private void RefreshAll() {
            RefreshFavority();

            this.RefreshLevel();
            this.RefreshTitle();
            this.RefreshLeft();
        }

        #region 事件监听

        protected override void ProcessEvents(bool toRegister) {
            // 好感度变化
            // 城镇npc激活变化
            // 可做条件变化
            // 任务完成变化
            // 好感度等级变化
            Sys_NPCFavorability.Instance.eventEmitter.Handle(Sys_NPCFavorability.EEvents.OnPlayerFavorabilityChanged, this.OnPlayerFavorabilityChanged, toRegister);
            Sys_TownTask.Instance.eventEmitter.Handle<uint, uint, uint>(Sys_TownTask.EEvents.OnCommitRes, this.OnCommitRes, toRegister);
            Sys_TownTask.Instance.eventEmitter.Handle<uint>(Sys_TownTask.EEvents.OnRefreshTaskRes, this.OnRefreshTaskRes, toRegister);
            Sys_TownTask.Instance.eventEmitter.Handle<uint, uint>(Sys_TownTask.EEvents.OnTakeAwardRes, this.OnTakeAwardRes, toRegister);
            Sys_TownTask.Instance.eventEmitter.Handle(Sys_TownTask.EEvents.OnInfoNtf, this.OnInfoNtf, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, this.OnItemCountChanged, toRegister);
        }

        private void OnPlayerFavorabilityChanged() {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.RefreshFavority();
            }
        }

        private void OnCommitRes(uint townId, uint bigId, uint libraryId) {
            if (townId != this.townId) {
                // 防止UI关闭，换了一个城镇然后再次打开该UI
                return;
            }

            this.RefreshTitle();
            this.RefreshLevel();
            this.RefreshLeft();

            TryResetRight(selectTaskIndex);
            this.RefreshRight();
        }

        private void OnRefreshTaskRes(uint townId) {
            if (townId != this.townId) {
                // 防止UI关闭，换了一个城镇然后再次打开该UI
                return;
            }

            this.RefreshTitle();
            this.RefreshLeft();

            TryResetRight(selectTaskIndex);
            this.RefreshRight();
        }

        private void OnTakeAwardRes(uint townId, uint changedCount) {
            if (townId == this.townId) {
                this.RefreshLevel();
            }
        }

        private void OnInfoNtf() {
            // 重新获取town
            Sys_TownTask.Instance.towns.TryGetValue(this.townId, out town);

            this.RefreshTitle();
            this.RefreshLeft();

            TryResetRight(selectTaskIndex);
            this.RefreshRight();
        }

        private void OnItemCountChanged(int changeType, int curBoxId) {
            this.RefreshTitle();
        }

        #endregion

        public void RefreshFavority() {
            TextHelper.SetText(this.layout.favorityValue, 12240, Sys_NPCFavorability.Instance.Favorability.ToString());
        }

        public void OnBtnHelpClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityTip);
        }

        public void OnBtnRefreshClicked() {
            if (!this.layout.idCountRefresh.Enough) {
                // 货币不足
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1660000010));
                return;
            }

            Sys_TownTask.Instance.RefreshTaskReq(this.townId);
        }

        public void OnBtnRewardClicked() {
            UIManager.OpenUI(EUIID.UI_TownTaskReward, false, this.townId);
        }

        private Lib.Core.Timer _timer;
        public void OnBtnSubmitClicked() {
            // 只有未完成 才可以点击进来
            if (submitReason == Sys_TownTask.Task.EReason.Nil) {
                var vd = vds[this.selectTaskIndex];
                if (vd.isEquip) {
                    // 选中的装备guid
                    void OnConform() {
                        _timer?.Cancel();
                        _timer = Timer.RegisterOrReuse(ref _timer, 0.1f, () => {
                            var selectItemData = items[selectedItemIndex];
                            ulong guid = selectItemData.guid;
                            
                            ItemData equip = Sys_Bag.Instance.GetItemDataByUuid(guid);
                            if (equip != null && equip.IsLocked)
                            {
                                PromptBoxParameter.Instance.Clear();
                                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, LanguageHelper.GetTextContent(equip.cSVItemData.name_id), LanguageHelper.GetTextContent(equip.cSVItemData.name_id));
                                PromptBoxParameter.Instance.SetConfirm(true, () =>
                                {
                                    Sys_Bag.Instance.OnItemLockReq(guid, false);
                                });
                                PromptBoxParameter.Instance.SetCancel(true, null);
                                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                                return;
                            }
                            
                            if (selectItemData.Quality >= Sys_Equip.Instance.QualityLimit &&
                                Sys_Equip.Instance.IsSecureLock(selectItemData.id, selectItemData.Quality)) {
                                return;
                            }
                            else {
                                Sys_TownTask.Instance.CommitReq(this.townId, vd.task.id, guid);
                            }
                        });
                    }

                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(1660000013).words;
                    PromptBoxParameter.Instance.SetConfirm(true, OnConform);
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
                else {
                    if (vd.task != null) {
                        Sys_TownTask.Instance.CommitReq(this.townId, vd.task.id);
                    }
                }
            }
            else if (submitReason == Sys_TownTask.Task.EReason.ReachMaxLevel) {
                // 等级已满
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1660000012));
            }
            else if (submitReason == Sys_TownTask.Task.EReason.LackItem) {
                // 道具不足
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1663000001));
            }
            else if (submitReason == Sys_TownTask.Task.EReason.LackFavority) {
                // 好感度不足
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1663000002));
            }
            else if (submitReason == Sys_TownTask.Task.EReason.InvalidFavorityLevel) {
                // 好感度等级不足
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1663000003));
            }
            else if (submitReason == Sys_TownTask.Task.EReason.InvalidPlayerLevel) {
                // 玩家等级不在范围内
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1663000004));
            }
            else if (submitReason == Sys_TownTask.Task.EReason.NotSelectTarget) {
                // 没有选中的道具或者装备
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1663000005));
            }
        }

        public void OnCreateCell(InfinityGridCell cell) {
            PropItem entry = new PropItem();
            var go = cell.mRootTransform.gameObject;
            entry.BindGameObject(go);
            cell.BindUserData(entry);
        }

        public void OnCellChange(InfinityGridCell cell, int index) {
            var entry = cell.mUserData as PropItem;
            this.RefreshOneItem(index, entry, cell.mRootTransform);
        }

        private void RefreshOneItem(int index, PropItem entry, Transform trans) {
            if (0 <= index && index < this.items.RealCount) {
                var cellData = this.items[index];
                cellData.bUseClick = true;
                cellData.bUseTips = false;
                cellData.bUseQuailty = true;
                cellData.bShowBtnNo = false;

                entry.SetData(cellData, EUIID.UI_TownTaskDetail);
                entry.BtnBoneShow(countInBag <= 0);
                var vd = this.vds[this.selectTaskIndex];
                if (vd.isEquip) {
                    entry.txtNumber.gameObject.SetActive(false);
                }
                else {
                    entry.RefreshLRCount(cellData.CountInBag, cellData.count);
                }

                bool sel = selectedItemIndex == index;
                entry.SetSelected(sel);

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

        private PropItem lastSelectedGiftPropItem;

        private void OnCellClicked(PropItem propItem, int index) {
            this.lastSelectedGiftPropItem?.SetSelected(false);
            this.selectedItemIndex = index;

            if (this.items[index].bagData.cSVItemData.type_id == (uint) EItemType.Equipment) {
                if (countInBag <= 0) {
                    PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(100495, 1, true, false, false, false, false, false, false);
                    var boxEvt = new MessageBoxEvt(EUIID.UI_Pet_Message, iItemData);
                    boxEvt.b_ForceShowScource = true;
                    boxEvt.b_ShowItemInfo = false;
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                }
                else {
                    EquipTipsData tipData = new EquipTipsData();
                    tipData.equip = this.items[index].bagData;
                    tipData.isCompare = false;
                    tipData.isShowOpBtn = false;
                    UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
                }
            }
            else {
                PropMessageParam propParam = new PropMessageParam();
                var itemData = propParam.itemData = this.items[index].bagData;
                propParam.needShowInfo = false;
                propParam.needShowMarket = false;
                propParam.sourceUiId = EUIID.UI_TownTaskDetail;

                var args = new PropIconLoader.ShowItemData(itemData.Id,
                    itemData.Count, true, itemData.bBind, itemData.bNew, false,
                    false);
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_TownTaskDetail, args));
            }

            propItem.SetSelected(true);
            this.lastSelectedGiftPropItem = propItem;

            RefreshRight();
        }

        private bool haveChanged = false;

        public void OnTimeRefresh(Text text, float time, bool isEnd) {
            if (isEnd) {
                TextHelper.SetText(text, "00:00:00");
            }
            else {
                // if (time < 60 && !haveChanged) {
                //     haveChanged = true;
                //
                //     text.fontSize = text.fontSize + 2;
                //     text.color = Color.red;
                // }

                var t = Mathf.Round(time);
                var s = LanguageHelper.TimeToString((uint) t, LanguageHelper.TimeFormat.Type_1);
                TextHelper.SetText(text, s);
            }
        }

        private void TryResetRight(int index) {
            var vd = this.vds[index];
            if (vd.isEquip) {
                this.selectedItemIndex = -1;
            }
            else {
                this.selectedItemIndex = 0;
            }
        }

        public void OnSelect(int index, bool _) {
            if (selectTaskIndex != index) {
                // reset
                TryResetRight(index);
            }

            this.selectTaskIndex = index;
            this.RefreshRight();
        }

        public void RefreshTitle() {
            // 刷新按钮的消耗货币
            uint id = 2;
            int count = 1000;
            if (Sys_Ini.Instance.Get<IniElement_IntArray>(1455, out IniElement_IntArray outer) && outer.value != null && outer.value.Length >= 2) {
                id = (uint) outer.value[0];
                count = outer.value[1];
            }

            this.layout.idCountRefresh.Reset(id, count);
            this.layout.costRefresh.Refresh(this.layout.idCountRefresh, ItemCostLackType.Normal);

            layout.hasMax.SetActive(town.IsReachedFull);

            long diff = Sys_TownTask.Instance.nextRefreshTime - Sys_Time.Instance.GetServerTime();
            this.layout.cdText.Begin(diff);
        }

        public void RefreshLevel() {
            TextHelper.SetText(this.layout.contributeLevel, 1660000002, this.town.contributeLevel.ToString());
            this.layout.redDot.gameObject.SetActive(this.town.CanAnyGot);

            var csv = CSVTownContributeLv.Instance.GetConfData(this.town.contributeLevelId);
            if (csv != null) {
                string s = $"{town.contributeExpInLevel.ToString()}/{csv.TowntUpgradeExp.ToString()}";
                TextHelper.SetText(this.layout.lrLevel, s);
                this.layout.expSlider.value = 1f * this.town.contributeExpInLevel / csv.TowntUpgradeExp;
            }
            else {
                // error
                this.layout.lrLevel.text = "";
                this.layout.expSlider.value = 1f;
            }
        }

        public void RefreshLeft() {
            int min = Mathf.Min(this.town.tasks.Count, this.vds.Count);
            for (int i = 0, length = vds.Count; i < length; ++i) {
                var vd = vds[i];
                if (i < min) {
                    vd.Show();
                }
                else {
                    vd.Hide();
                }
            }

            for (int i = 0, length = min; i < length; ++i) {
                this.vds[i].Refresh(this.town.tasks[i], i);
            }
        }

        public Sys_TownTask.Task.EReason submitReason = Sys_TownTask.Task.EReason.Nil;
        public uint countInBag = 0;

        public void RefreshRight() {
            var vd = this.vds[this.selectTaskIndex];
            var task = vd.task;

            if (task == null) {
                return;
            }

            this.layout.limtDesc.text = vd.taskGoal.text;
            TextHelper.SetText(this.layout.costFavority, task.csvLibrary.ConsumePoints.ToString());
            TextHelper.SetText(layout.taskReward, task.csvLibrary.TaskReward.ToString());

            PropIconLoader.ShowItemData OnCreate(int index) {
                var r = new PropIconLoader.ShowItemData(1, 1, true, false, false, false, false);
                return r;
            }

            void OnRefresh(int index, ItemData item, PropIconLoader.ShowItemData prop) {
                prop.guid = item.Uuid;
                prop.id = item.Id;
                prop.count = item.Count;

                prop.SetQuality(item.Quality);
                prop.bagData = item;
                prop.EquipPara = item.EquipParam;
            }

            uint csvNeedId = task.csvLibrary.TaskConsumeItem;
            List<ItemData> ds = null;
            if (vd.isEquip) {
                ds = Sys_Equip.Instance.GetEequips(task.csvLibrary.TaskConsumeEqpt, task.csvLibrary.ConsumeEqptQuality, FavorabilityNPC.ItemsFilters);
                countInBag = (uint) ds.Count;
                csvNeedId = task.csvLibrary.TaskConsumeEqpt;
            }
            else {
                countInBag = (uint) Sys_Bag.Instance.GetItemCount(csvNeedId);
            }

            long needCount = 0;

            void NotExistInBag() {
                items.TryCopy<ItemData>(defaultList.Count, defaultList, OnCreate, OnRefresh);
                long c = task.csvLibrary.ConsumeItemNum;
                items[0].id = csvNeedId;
                items[0].count = c;

                var quality = defaultList[0].Quality;
                var t = defaultList[0].SetData(0, 0, csvNeedId, (uint) c, 0, false, false, null, null, 0);
                items[0].SetQuality(quality);
                items[0].bagData = t;
            }

            if (countInBag <= 0) {
                if (vd.isEquip) {
                    defaultList[0].SetQuality(task.csvLibrary.ConsumeEqptQuality);
                }
                else {
                    defaultList[0].SetQuality(CSVItem.Instance.GetConfData(csvNeedId).quality);
                }

                NotExistInBag();
                needCount = 1;
            }
            else {
                if (vd.isEquip) {
                    // 需要保证返回值有序，否则记录的index就会有问题
                    items.TryCopy<ItemData>(ds.Count, ds, OnCreate, OnRefresh);

                    needCount = items.RealCount;
                }
                else {
                    defaultList[0].SetQuality(CSVItem.Instance.GetConfData(csvNeedId).quality);
                    NotExistInBag();

                    needCount = 1;
                }
            }

            this.layout.emptyScroll.SetActive(false);
            this.layout.infinityGrid.gameObject.SetActive(true);

            // 属性提交的道具
            this.layout.infinityGrid.CellCount = (int) needCount;
            this.layout.infinityGrid.ForceRefreshActiveCell();

            void CantSubmit() {
                TextHelper.SetText(this.layout.btnSubmitText, 1660000008);
                this.layout.btnSubmit.enabled = true;
                ImageHelper.SetImageGray(this.layout.btnSubmit, true, true);
            }

            task.CanDo(out this.submitReason);
            if (this.submitReason == Sys_TownTask.Task.EReason.HasFinish) {
                TextHelper.SetText(this.layout.btnSubmitText, 1660000007);
                ButtonHelper.Enable(this.layout.btnSubmit, false);
            }
            else if (this.submitReason == Sys_TownTask.Task.EReason.ReachMaxLevel) {
                CantSubmit();
            }
            else if (this.submitReason == Sys_TownTask.Task.EReason.Nil) {
                bool hasSelectItem = (0 <= selectedItemIndex && selectedItemIndex < items.Count);
                if (hasSelectItem) {
                    TextHelper.SetText(this.layout.btnSubmitText, 1660000006);
                    ButtonHelper.Enable(this.layout.btnSubmit, true);
                }
                else {
                    this.submitReason = Sys_TownTask.Task.EReason.NotSelectTarget;
                    CantSubmit();
                }
            }
            else {
                CantSubmit();
            }
        }
    }
}