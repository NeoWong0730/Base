using System;
using System.Collections.Generic;
using System.Globalization;
using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_ActivitySubscribe : UI_OperationalActivityBase {
        public class Tab : UISelectableElement {
            public CP_Toggle toggle;
            public Text tabNameLight;
            public Text tabNameDark;

            public uint tabId = 0;

            protected override void Loaded() {
                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);

                this.tabNameLight = this.transform.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
                this.tabNameDark = this.transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            }

            public void Refresh(uint tabId) {
                this.tabId = tabId;
                // var csv = CSVTreasureRaider.Instance.GetConfData(tabId);
                // if (csv != null) {
                // TextHelper.SetText(this.tabNameLight, csv.Name);
                // TextHelper.SetText(this.tabNameDark, csv.Name);
                // }
            }

            public void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke((int) this.tabId, true);
                }
            }

            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }
        }

        public class Reward : UIComponent {
            public Transform left;
            public Transform right;
            public Transform infoNode;

            public new Text name;
            public Image icon;

            protected override void Loaded() {
                left = transform.Find("Left");
                right = transform.Find("Right");
                infoNode = transform.Find("Item");
                icon = transform.Find("Item/Image_Icon").GetComponent<Image>();
                name = transform.Find("Item/Text").GetComponent<Text>();
                
                var btn = transform.Find("Item").GetComponent<Button>();
                btn.onClick.AddListener(OnBtnClicked);
            }

            public ItemIdCount idCount;
            public void Refresh(ItemIdCount idCount, int index) {
                this.idCount = idCount;
                
                bool idOdd = index % 2 == 0;
                if (idOdd) {
                    infoNode.SetParent(left);
                }
                else {
                    infoNode.SetParent(right);
                }

                var csv = CSVItem.Instance.GetConfData(idCount.id);
                if (csv != null) {
                    ImageHelper.SetIcon(this.icon, csv.icon_id);
                    TextHelper.SetText(this.name, csv.name_id);
                }
                else {
                    // error
                }
            }

            private void OnBtnClicked() {
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_OperationalActivity, new PropIconLoader.ShowItemData(
                    idCount.id,  idCount.count, true, false, false, false, false, true, false, false, null, false, false )));
            }
        }

        public uint selectedId = 0;
        public Sys_ActivitySubscribe.Item selectItem;
        public UIElementCollector<Tab> tabVds = new UIElementCollector<Tab>();
        public COWVd<Reward> rewardsVds = new COWVd<Reward>();

        public Transform upNode;
        public UI_RewardList upReward;
        public Transform rewardProto;

        public Button btnBuy;
        public Image buyIcon;
        public Text buyCount;
        public Text buyDesc;
        public ButtonList btnList;

        public Text Text_1;
        public Text Text_2;
        public Text Text_3;
        public Text Text_4;

        #region 逻辑事件处理

        protected override void ProcessEventsForEnable(bool toRegister) {
            Sys_ActivitySubscribe.Instance.eventEmitter.Handle(Sys_ActivitySubscribe.EEvents.OnReceiveAll, OnReceiveAll, toRegister);
            Sys_ActivitySubscribe.Instance.eventEmitter.Handle<uint>(Sys_ActivitySubscribe.EEvents.OnBuy, OnBuy, toRegister);
            Sys_Charge.Instance.eventEmitter.Handle<uint>(Sys_Charge.EEvents.OnChargedNotify, OnChargedNotify, toRegister);
        }

        private void OnReceiveAll() {
            RefreshAll();
        }

        // 游戏逻辑购买成功/失败
        private void OnBuy(uint id) {
            if (selectedId == id) {
                if (this.tabVds.TryGetVdById((int) id, out var vd)) {
                    vd.SetSelected(true, true);
                }
            }
        }

        // sdk购买成功notify
        private void OnChargedNotify(uint id) {
            RefreshAll();
        }

        #endregion

        protected override void InitBeforOnShow() {
            upNode = transform.Find("View_Award/Award1/Scroll View/Viewport");
            rewardProto = transform.Find("View_Award/Award2/Scroll View/Viewport/Reward/Item");

            buyDesc = transform.Find("View_Award/Text_Tips").GetComponent<Text>();
            buyIcon = transform.Find("View_Award/Btn_Purchase/Image_Icon").GetComponent<Image>();
            buyCount = transform.Find("View_Award/Btn_Purchase/Text_01").GetComponent<Text>();

            Text_1 = transform.Find("Text_1").GetComponent<Text>();
            Text_2 = transform.Find("Text_2").GetComponent<Text>();
            Text_3 = transform.Find("Text_3").GetComponent<Text>();
            Text_4 = transform.Find("Text_4").GetComponent<Text>();

            btnBuy = transform.Find("View_Award/Btn_Purchase").GetComponent<Button>();
            btnBuy.onClick.AddListener(OnBtnBuyClicked);
            
            btnList = transform.GetComponent<ButtonList>();
            btnList.onClick += OnButtonClick;

            var t = transform.Find("View_Award/TabList/1").gameObject;
            tabVds.Clear();
            tabVds.TryAdd(t);
            t = transform.Find("View_Award/TabList/2").gameObject;
            tabVds.TryAdd(t);
            t = transform.Find("View_Award/TabList/3").gameObject;
            tabVds.TryAdd(t);
            selectedId = Sys_ActivitySubscribe.Instance.firstOpenItem;
        }

        private void OnButtonClick(int a, int b, int c, int d)
        {
            switch (a)
            {
                case 0:
                {
                    uint ruleID = (uint)b;
                    CSVUIRule.Data csvRule = Table.CSVUIRule.Instance.GetConfData(ruleID);
                    UIManager.OpenUI(EUIID.UI_HelpRule, false, csvRule.ruleIds);
                }
                    break;
                default:
                    break;
            }
        }

        private void OnBtnBuyClicked() {
            if (reason == EReason.lackOfItem) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2015114));
            }
            else if (reason == EReason.NotInTimeRangeOrHasBuied) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2015115));
            }
            else {
                if (selectItem.csv.priceType == 0) {
                    // sdk充值
                    Sys_Charge.Instance.OnChargeReq(selectItem.csv.priceID);
                }
                else {
                    Sys_ActivitySubscribe.Instance.ReqBuy(selectedId);
                }
            }
        }

        public override void Show() {
            base.Show();

            Sys_ActivitySubscribe.Instance.SetRedPoint(false);
            RefreshAll();
        }

        private List<uint> tabIds;

        private void RefreshAll() {
            tabIds = new List<uint>(Sys_ActivitySubscribe.Instance.items.Keys);
            tabIds.Sort((l, r) => {
                var csvL = CSVTreasureRaider.Instance.GetConfData(l);
                var csvR = CSVTreasureRaider.Instance.GetConfData(r);
                if (csvL != null && csvR != null) {
                    return csvL.type.CompareTo(csvR.type);
                }
                else {
                    return 0;
                }
            });
            // 构建和刷新
            this.tabVds.TryRefresh(this.tabIds.Count, OnTabRefresh);

            // 默认选中页签
            if (this.tabVds.TryGetVdById((int) this.selectedId, out var vd)) {
                vd.SetSelected(true, true);
            }
            else if(tabVds.RealCount > 0){
                tabVds[0].SetSelected(true, true);
            }
        }

        private void OnTabSelected(int id, bool force) {
            this.selectedId = (uint) id;
            this.RefreshGrid();
        }

        private void OnTabRefresh(Tab vd, int index) {
            uint id = tabIds[index];
            vd.SetUniqueId((int) id);
            vd.SetSelectedAction(OnTabSelected);
            vd.Refresh(id);
        }

        public enum EReason {
            Nil,
            lackOfItem,
            NotInTimeRangeOrHasBuied,
        }

        public EReason reason = EReason.Nil;

        public List<ItemIdCount> rewards = new List<ItemIdCount>();

        private void OnRefreshReward(Reward r, int index) {
            r.Refresh(rewards[index], index);
        }

        private void RefreshGrid() {
            if (Sys_ActivitySubscribe.Instance.items.TryGetValue(selectedId, out selectItem)) {
                var beginTime = Sys_Time.ConvertToDatetime(selectItem.csv.startTime + TimeManager.TimeZoneOffset);
                var endTime = Sys_Time.ConvertToDatetime(selectItem.csv.endTime + TimeManager.TimeZoneOffset);
                var finalTime = Sys_Time.ConvertToDatetime(selectItem.csv.prizeTime + TimeManager.TimeZoneOffset);
                TextHelper.SetText(Text_1, 2015102, beginTime.ToString(GameMain.beijingCulture), endTime.ToString(GameMain.beijingCulture));
                TextHelper.SetText(Text_2, 2015103, finalTime.ToString(GameMain.beijingCulture));
                // int diff = (int) ((long) selectItem.csv.prizeTime - (long) selectItem.csv.endTime);
                // var day = (int) (diff / 86400);
                TextHelper.SetText(Text_3, 2015105/*, day.ToString()*/);
                TextHelper.SetText(Text_4, 2015104/*, day.ToString()*/);

                if (selectItem.csv.priceType == 0) {
                    buyIcon.gameObject.SetActive(false);
                    // todo ￥
                    TextHelper.SetText(buyCount, 2015113, selectItem.csv.price.ToString());
                }
                else {
                    buyIcon.gameObject.SetActive(true);
                    ImageHelper.SetIcon(buyIcon, CSVItem.Instance.GetConfData(selectItem.csv.priceType).icon_id);

                    TextHelper.SetText(buyCount, selectItem.csv.price.ToString());
                }

                var csvFound = CSVItem.Instance.GetConfData(selectItem.csv.refundType);
                string foundName = LanguageHelper.GetTextContent(csvFound.name_id);
                // todo 返还
                TextHelper.SetText(buyDesc, 2015112, selectItem.csv.refund.ToString(), foundName);

                bool canBuy = !selectItem.hasBuyed && selectItem.IsOpening;
                reason = EReason.Nil;
                if (!canBuy) {
                    reason = EReason.NotInTimeRangeOrHasBuied;
                }
                else {
                    if (selectItem.csv.priceType != 0) {
                        long CountInBag = Sys_Bag.Instance.GetItemCount(selectItem.csv.priceType);
                        bool isItemEnough = CountInBag >= selectItem.csv.price;
                        canBuy &= (isItemEnough);

                        if (!isItemEnough) {
                            reason = EReason.lackOfItem;
                        }
                    }
                }

                ImageHelper.SetImageGray(btnBuy, !canBuy, true);
                // ButtonHelper.Enable(btnBuy, canBuy);

                if (upReward == null) {
                    upReward = new UI_RewardList(upNode, EUIID.UI_OperationalActivity);
                }

                this.upReward.SetRewardList(CSVDrop.Instance.GetDropItem(selectItem.csv.gameReward));
                this.upReward?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

                rewards = CSVDrop.Instance.GetDropItem(selectItem.csv.inKindReward);
                rewardsVds.TryBuildOrRefresh(rewardProto.gameObject, rewardProto.parent, rewards.Count, OnRefreshReward);
            }
            else {
                ButtonHelper.Enable(btnBuy, false);
            }
        }
    }

    public class Sys_ActivitySubscribe : SystemModuleBase<Sys_ActivitySubscribe> {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents {
            OnBuy,
            OnReceiveAll, // ui打开的时候，断线了同时时间过期了

            OnRefreshRedPoint,

            OnActivityBegin,
            OnActivityEnd,
        }

        public override void Init() {
            EventDispatcher.Instance.AddEventListener(0, (ushort) PKCompete.TreasureDrawInfoNtf, this.OnReceiveAll, TreasureDrawInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) PKCompete.TreasureDrawBuyReq, (ushort) PKCompete.TreasureDrawBuyRes, this.OnBuy, TreasureDrawBuyRes.Parser);
        }

        public override void OnLogin() {
            timerBegin?.Cancel();
            timerEnd?.Cancel();
            
            canShowRedPoint = true;
            items.Clear();
            uint groupId = 1001;
            if (Sys_Ini.Instance.Get<IniElement_Int>(1530, out var v)) {
                groupId = (uint) v.value;
            }

            var allLines = CSVTreasureRaider.Instance.GetAll();
            for (int i = 0, length = allLines.Count; i < length; ++i) {
                var line = allLines[i];
                if (line.group == groupId) {
                    var it = new Item() {
                        id = line.id,
                        hasBuyed = false
                    };
                    items.Add(line.id, it);
                }
            }
        }

        public override void OnLogout() {
            timerBegin?.Cancel();
            timerEnd?.Cancel();
        }

        private void OnReceiveAll(NetMsg msg) {
            TreasureDrawInfoNtf response = NetMsgUtil.Deserialize<TreasureDrawInfoNtf>(TreasureDrawInfoNtf.Parser, msg);
            for (int i = 0, length = response.HasBuyId.Count; i < length; ++i) {
                if (items.TryGetValue(response.HasBuyId[i], out var item)) {
                    item.hasBuyed = true;
                }
            }
            
            timerBegin?.Cancel();
            timerEnd?.Cancel();
            // 没有任何开启的item,需要设立timer以备运行时开启
            if (firstOpenItem == 0) {
                long now = Sys_Time.Instance.GetServerTime(true);
                foreach (var kvp in items) {
                    long diff = kvp.Value.csv.startTime - now;
                    if (diff > 0) {
                        timerBegin = Timer.RegisterOrReuse(ref timerBegin, diff, () => {
                            eventEmitter.Trigger(EEvents.OnActivityBegin);
                        }, null, false, true);
                        
                        diff += (kvp.Value.csv.endTime - kvp.Value.csv.startTime);
                        timerEnd = Timer.RegisterOrReuse(ref timerEnd, diff, () => {
                            eventEmitter.Trigger(EEvents.OnActivityEnd);
                        }, null, false, true);
                        break;
                    }
                }
            }
            else {
                var item = items[firstOpenItem];
                long now = Sys_Time.Instance.GetServerTime(true);
                long diff = item.csv.endTime - now;
                if (diff > 0) {
                    timerEnd = Timer.RegisterOrReuse(ref timerEnd, diff, () => {
                        eventEmitter.Trigger(EEvents.OnActivityEnd);
                    }, null, false, true);
                }
            }

            eventEmitter.Trigger(EEvents.OnReceiveAll);
        }

        public void ReqBuy(uint id) {
            TreasureDrawBuyReq req = new TreasureDrawBuyReq();
            req.DrawId = id;
            NetClient.Instance.SendMessage((ushort) PKCompete.TreasureDrawBuyReq, req);
        }

        private void OnBuy(NetMsg msg) {
            TreasureDrawBuyRes response = NetMsgUtil.Deserialize<TreasureDrawBuyRes>(TreasureDrawBuyRes.Parser, msg);
            uint id = response.DrawId;
            if (items.TryGetValue(id, out var item)) {
                item.hasBuyed = true;
                
                eventEmitter.Trigger<uint>(EEvents.OnBuy, id);
            }
        }

        public class Item {
            public uint id;

            public CSVTreasureRaider.Data csv {
                get { return CSVTreasureRaider.Instance.GetConfData(id); }
            }

            // serverData
            // 已经购买过
            public bool hasBuyed;

            public bool IsOpening {
                get {
                    // 是否在时间区间之内
                    long now = Sys_Time.Instance.GetServerTime(true);
                    return csv.startTime <= now && now <= csv.endTime;
                }
            }

            public bool redPoint {
                get { return IsOpening && !hasBuyed; }
            }
        }

        public Dictionary<uint, Item> items = new Dictionary<uint, Item>();
        public Timer timerBegin;
        public Timer timerEnd;

        // 第一个开启的itemId
        public uint firstOpenItem {
            get {
                foreach (var kvp in items) {
                    if (kvp.Value.IsOpening) {
                        return kvp.Key;
                    }
                }

                return 0;
            }
        }

        // 点击UI的时候设置为false，切换账号的时候设置为true
        public bool canShowRedPoint { get; private set; }

        public void SetRedPoint(bool newFlag) {
            canShowRedPoint = newFlag;
            eventEmitter.Trigger(EEvents.OnRefreshRedPoint);
        }

        public bool redPoint {
            get { return canShowRedPoint && CheckIsOpen(); }
        }

        public bool CheckIsOpen() {
            // 功能开启 并且时间区间合适
            bool isFunctionOpen = firstOpenItem != 0;
            isFunctionOpen &= Sys_FunctionOpen.Instance.IsOpen(50916);
            isFunctionOpen &= (Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(121));
            return isFunctionOpen;
        }
    }
}