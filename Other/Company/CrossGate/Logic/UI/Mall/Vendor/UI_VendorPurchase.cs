using System;
using Logic.Core;
using Packet;
using Table;
using UnityEngine.UI;
using static Logic.PropIconLoader;

namespace Logic {
    // 参考 UI_Partner_LevelUp.cs
    public class UI_VendorPurchase : UIBase {
        public Button btnMax;
        public Button btnBuy;

        public PropItem propItem = new PropItem();
        public ShowItemData itemData = new ShowItemData();

        private InputField input;

        private readonly ItemIdCount idCount = new ItemIdCount();
        private readonly UI_CostItem cost = new UI_CostItem();

        // 购买数量
        private int buyNumber;
        private int max;
        // 商品id
        private ShopItem shopItem;
        private ulong npcUid;
        private CSVShopItem.Data csvShopItem;

        protected override void OnLoaded() {
            this.input = this.transform.Find("Animator/View_BuyDetail/Num/InputField_Number").GetComponent<InputField>();
            this.input.onValueChanged.AddListener(this.OnInputValueChanged);

            Button btn = this.transform.Find("Animator/View_TipsBgNew06/Btn_Close").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnExitClicked);
            this.btnBuy = this.transform.Find("Animator/View_BuyDetail/BtnBuy").GetComponent<Button>();
            this.btnBuy.onClick.AddListener(this.OnBtnPurchaseClicked);

            btn = this.transform.Find("Animator/View_BuyDetail/Num/Button_Sub").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnSubClicked);
            btn = this.transform.Find("Animator/View_BuyDetail/Num/Button_Add").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnAddClicked);

            this.btnMax = this.transform.Find("Animator/View_BuyDetail/Num/Button_Max").GetComponent<Button>();
            this.btnMax.onClick.AddListener(this.OnBtnMaxClicked);

            this.cost.SetGameObject(this.transform.Find("Animator/View_BuyDetail/Price/Cost_Coin").gameObject);
            this.propItem.BindGameObject(this.transform.Find("Animator/View_BuyDetail/Image_ItemBG/PropItem").gameObject);
        }
        private void OnInputValueChanged(string content) {
            this.Validate(content);
        }
        public void OnBtnExitClicked() { this.CloseSelf(); }

        public void OnBtnPurchaseClicked() {
            if (this.idCount.Enough) {
                int remainCount = Sys_Mall.GetCanBuyCount(this.shopItem);
                if (remainCount >= this.buyNumber) {
                    Sys_Mall.Instance.OnBuyReq(this.shopItem.ShopItemId, (uint)this.buyNumber, npcUid);
                    this.CloseSelf();
                }
            }
            else {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetErrorCodeContent(103304));
            }
        }
        public void OnBtnSubClicked() { --this.buyNumber; this.Validate(); }
        public void OnBtnAddClicked() {
            if (this.buyNumber >= this.max) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590000004));
            }

            ++this.buyNumber;
            this.Validate();
        }
        public void OnBtnMaxClicked() {
            if (this.buyNumber >= this.max) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590000004));
            }

            this.buyNumber = this.max;
            this.Validate();
        }

        private void Validate(string content) {
            int.TryParse(content, out int inputNumber);
            this.buyNumber = inputNumber;

            this.Validate();

        }
        private void Validate() {
            if (this.buyNumber < 1) {
                this.buyNumber = 1;
            }
            if (this.buyNumber > this.max) {
                this.buyNumber = this.max;
            }

            this.input.text = this.buyNumber.ToString();

            this.RefreshCost();
        }
        protected override void OnOpen(object arg) {
            Tuple<ShopItem, ulong> tp = arg as Tuple<ShopItem, ulong>;
            if (tp != null) {
                this.shopItem = tp.Item1;
                this.npcUid = tp.Item2;
            }
        }

        protected override void OnOpened() {
            // 默认购买一个
            this.buyNumber = 1;

            this.csvShopItem = CSVShopItem.Instance.GetConfData(this.shopItem.ShopItemId);
            if (this.csvShopItem != null) {
                this.RefreshPropItem(this.csvShopItem.item_id);
                this.max = (int)CSVShopItem.Instance.GetConfData(this.shopItem.ShopItemId).perPurchase_limit_count;
                if (this.max <= 0) {
                    this.max = int.MaxValue;
                }
                this.max = Math.Min(Sys_Mall.GetCanBuyCount(this.shopItem), this.max);

                this.Validate();

                this.btnMax.gameObject.SetActive(this.csvShopItem.perPurchase_limit_count > 0);
            }
        }

        private void RefreshCost() {
            if (this.csvShopItem != null) {
                uint total = (uint)this.buyNumber * this.shopItem.Price;
                this.idCount.Reset(this.csvShopItem.price_type, total);

                uint contentId = this.idCount.Enough ? 2010420u : 2010421u;
                this.cost.Refresh(this.idCount, ItemCostLackType.Custom, LanguageHelper.GetTextContent(contentId, total.ToString()));

                //this.btnBuy.gameObject.SetActive(this.idCount.Enough);
            }
        }
        private void RefreshPropItem(uint itemId) {
            this.itemData.Refresh(itemId, 0, true, false, false, false, false, false, false);
            this.propItem.SetData(this.itemData, EUIID.UI_VendorPurchase);

            TextHelper.SetText(this.propItem.txtName, CSVItem.Instance.GetConfData(itemId).name_id);
        }

        #region 事件处理
        //protected override void ProcessEventsForEnable(bool toRegister) {
        //    Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnBuyScuccess, this.OnBuyScuccess, toRegister);
        //    Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnBuyFail, this.OnBuyFail, toRegister);
        //}

        private void OnBuyScuccess() {
            //this.CloseSelf();
        }
        private void OnBuyFail() {
            //this.CloseSelf();
        }
        #endregion
    }
}