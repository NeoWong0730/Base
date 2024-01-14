using System;
using System.Collections.Generic;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;
using static Logic.PropIconLoader;

namespace Logic {
    public class UI_VendorList : UIBase, UI_VendorList.Tab.IListener {
        public class Tab : UIComponent {
            public interface IListener {
                void OnClicked(ShopItem shopItem);
            }

            public PropItem propItem = new PropItem();
            public ShowItemData itemData = new ShowItemData();

            public new Text name;
            public Text number;

            private readonly ItemIdCount idCount = new ItemIdCount();
            public UI_CostItem cost = new UI_CostItem();

            private IListener listener;
            private ShopItem shopItem;

            protected override void Loaded() {
                this.propItem.BindGameObject(this.transform.Find("PropItem").gameObject);

                Button btn = this.transform.GetComponent<Button>();
                btn.onClick.AddListener(this.OnBtnClicked);

                this.name = this.transform.Find("Text_Name").GetComponent<Text>();
                this.number = this.transform.Find("Text_Num/Text").GetComponent<Text>();
                this.cost.SetGameObject(this.transform.Find("Cost_Coin").gameObject);
            }
            public override void OnDestroy() {
                base.OnDestroy();
            }

            private void OnBtnClicked() {
                this.listener?.OnClicked(this.shopItem);
            }

            public void Refresh(ShopItem shopItem, IListener listener) {
                this.shopItem = shopItem;
                this.listener = listener;

                CSVShopItem.Data csvShopItem = CSVShopItem.Instance.GetConfData(shopItem.ShopItemId);
                if (csvShopItem != null) {
                    CSVItem.Data csvItem = CSVItem.Instance.GetConfData(csvShopItem.item_id);
                    if (csvItem != null) {
                        TextHelper.SetText(this.name, csvItem.name_id);
                        if (shopItem.SelfNum != 0 && shopItem.GlobalNum == 0) {
                            this.number.text = (shopItem.SelfNum - (long)shopItem.SelfCount).ToString();
                        }
                        else if (shopItem.GlobalNum != 0 && shopItem.SelfNum == 0) {
                            this.number.text = (shopItem.GlobalNum - (long)shopItem.GlobalCount).ToString();
                        }

                        this.idCount.Reset(csvShopItem.price_type, shopItem.Price);
                        this.cost.Refresh(this.idCount, ItemCostLackType.Custom, shopItem.Price.ToString());

                        this.itemData.Refresh(csvItem.id, 0, true, false, false, false, false, false, false);
                        this.propItem.SetData(this.itemData, EUIID.UI_VendorList);
                    }
                }
            }
        }

        public UI_CurrencyTitle currencyTitle;

        public Button btnExit;
        public Text npcName;

        public Text page;
        public CP_PageSwitcher switcher;

        public GameObject emptyGo;
        public GameObject notEmptyGo;

        public GameObject proto;
        public Transform protoParent;
        public COWVd<Tab> tabs = new COWVd<Tab>();

        private uint npcId;
        private uint shopId;
        private ulong npcUid;

        private List<ShopItem> allShopItemList;
        private List<ShopItem> pageShopItemList;

        protected override void OnLoaded() {
            this.currencyTitle = new UI_CurrencyTitle(this.transform.Find("Animator/UI_Property").gameObject);
            this.npcName = this.transform.Find("Animator/Detail/TextName").GetComponent<Text>();
            this.btnExit = this.transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);

            this.page = this.transform.Find("Animator/Detail/Page/TextPage").GetComponent<Text>();
            this.switcher = this.transform.Find("Animator/Detail/Page").GetComponent<CP_PageSwitcher>();
            this.switcher.onExec += this.OnSwitch;

            this.emptyGo = this.transform.Find("Animator/Detail/Empty")?.gameObject;
            this.notEmptyGo = this.transform.Find("Animator/Detail/Scroll View")?.gameObject;

            this.proto = this.transform.Find("Animator/Detail/Scroll View/Viewport/Content/Proto").gameObject;
            this.protoParent = this.transform.Find("Animator/Detail/Scroll View/Viewport/Content");
        }
        protected override void OnDestroy() {
            this.tabs.ForEach((vd) => {
                vd.OnDestroy();
            });
            this.tabs.Clear();
            this.currencyTitle?.Dispose();
        }
        public void OnBtnExitClicked() {
            this.CloseSelf();
        }
        protected override void OnOpen(object arg) {
            Tuple<uint, uint, ulong, object> tp = arg as Tuple<uint, uint, ulong, object>;
            if (tp != null) {
                this.npcId = tp.Item2;
                this.npcUid = tp.Item3;
                this.shopId = Convert.ToUInt32(tp.Item4);
            }
        }
        protected override void OnOpened() {
            this.currencyTitle.SetData(CSVShop.Instance.GetConfData(this.shopId).show_currency);

            uint nameId = CSVNpc.Instance.GetConfData(this.npcId).name;
            TextHelper.SetText(this.npcName, 590000002, LanguageHelper.GetNpcTextContent(nameId));

            Sys_Mall.Instance.OnItemRecordReq(this.shopId, this.npcUid);
            this.allShopItemList = EmptyList<ShopItem>.Value;
        }
        protected override void OnShow() {
            this.RefreshAll();
        }

        private void OnSwitch(int index, int startIndex, int rangeCount) {
            this.pageShopItemList = this.allShopItemList.GetRange(startIndex, rangeCount);

            if (this.switcher.PageCount <= 1) {
                this.page.gameObject.SetActive(false);
            }
            else {
                this.page.gameObject.SetActive(true);
                TextHelper.SetText(this.page, string.Format("{0}/{1}", (index + 1).ToString(), this.switcher.PageCount));
            }

            this.RefreshPage(index);
        }

        public void RefreshAll() {
            bool Filter(ShopItem item) {
                return Sys_Mall.GetCanBuyCount(item) > 0;
            }

            this.allShopItemList = this.allShopItemList.FindAll(Filter);

            if (this.allShopItemList.Count > 0) {
                this.emptyGo?.SetActive(false);
                this.notEmptyGo?.SetActive(true);
                this.switcher.gameObject.SetActive(true);

                this.switcher.SetCount(this.allShopItemList.Count);
                if (!this.switcher.SetCurrentIndex(this.switcher.currentPageIndex)) {
                    this.switcher.SetCurrentIndex(0);
                }
                this.switcher.Exec();
            }
            else {
                this.emptyGo?.SetActive(true);
                this.notEmptyGo?.SetActive(false);
                this.switcher.gameObject.SetActive(false);
            }
        }
        public void RefreshPage(int index) {
            void OnVdRefresh(Tab vd, int idx) {
                vd.Refresh(this.pageShopItemList[idx], this);
            }

            this.tabs.TryBuildOrRefresh(this.proto, this.protoParent, this.pageShopItemList.Count, OnVdRefresh);
        }

        #region
        protected override void ProcessEvents(bool toRegister) {
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnBuyScuccess, this.OnBuyScuccess, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnRefreshShopData, this.OnRefreshShopData, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle<uint>(Sys_Mall.EEvents.OnFillShopData, this.OnFillShopData, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnBuyFail, this.OnBuyFail, toRegister);
        }

        private void OnBuyFail() {
            Sys_Mall.Instance.OnItemRecordReq(this.shopId, this.npcUid);
        }
        private void OnBuyScuccess() {
            this.allShopItemList = Sys_Mall.Instance.GetShopItems(this.shopId);
            this.RefreshAll();
        }
        private void OnRefreshShopData() {
            this.allShopItemList = Sys_Mall.Instance.GetShopItems(this.shopId);
            this.RefreshAll();
        }
        private void OnFillShopData(uint shopId) {
            if (shopId == this.shopId) {
                this.allShopItemList = Sys_Mall.Instance.GetShopItems(this.shopId);
                this.RefreshAll();
            }
        }

        public void OnClicked(ShopItem shopItem) {
            UIManager.OpenUI(EUIID.UI_VendorPurchase, false, new Tuple<ShopItem, ulong>(shopItem, this.npcUid));
        }
        #endregion
    }
}