using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_PointMall_ShopItem : UIComponent
    {
        private CP_Toggle toggle;
        private Image _ImgIcon;
        private Text textName;

        private GameObject costOriginal;
        private Text textOriginal;
        private Image iconOriginal;
        private Text textOriginalValue;

        private GameObject costNow;
        private Image iconNow;
        private Text textNowValue;

        private Image imgMarkDiscount; //折扣
        private Text textDiscountNum;
        private Image imgMarkLimitTime; //限时
        private Image imgSellOut;       //限购售完

        private Text textCondition;

        private ShopItem mShopItem;
        private uint mShopItemId;
        private bool isPriceChange;
        protected override void Loaded()
        {
            toggle = gameObject.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener(OnClickToggle);

            _ImgIcon = transform.Find("Image_ICON").GetComponent<Image>();

            textName = transform.Find("Text_Name").GetComponent<Text>();

            costOriginal = transform.Find("Cost/Cost_Original").gameObject;
            textOriginal = costOriginal.transform.Find("Text_Original").GetComponent<Text>();
            iconOriginal = textOriginal.transform.Find("Image_Icon").GetComponent<Image>();
            textOriginalValue = iconOriginal.transform.Find("Text").GetComponent<Text>();

            costNow = transform.Find("Cost/Cost_Now").gameObject;
            iconNow = costNow.transform.Find("Image_Icon").GetComponent<Image>();
            textNowValue = iconNow.transform.Find("Text").GetComponent<Text>();

            imgMarkDiscount = transform.Find("MarkGrid/Image_Mark01").GetComponent<Image>();
            textDiscountNum = imgMarkDiscount.transform.Find("Text/Text_Num").GetComponent<Text>();
            imgMarkLimitTime = transform.Find("MarkGrid/Image_Mark02").GetComponent<Image>();

            imgSellOut = transform.Find("Image_Sellout").GetComponent<Image>();

            ProcessEvents(true);
        }

        public override void OnDestroy()
        {
            ProcessEvents(false);
        }

        private void ProcessEvents(bool toRegister)
        {
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnRefreshShopData, OnRefreshShopData, toRegister);
            //Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChnange, toRegister);
        }

        private void OnClickToggle(bool isOn)
        {
            if (isOn)
            {
                Sys_Mall.Instance.OnSelectShopItem(mShopItemId);
            }
        }

        public void OnSelect(bool isOn)
        {
            toggle.SetSelected(isOn, true);
        }

        public void UpdateInfo(ShopItem shopItem, bool priceChange = false)
        {
            mShopItem = shopItem;
            mShopItemId = shopItem.ShopItemId;
            isPriceChange = priceChange;
            CSVShopItem.Data shopData = CSVShopItem.Instance.GetConfData(mShopItemId);
            if (shopData == null)
            {
                Debug.LogErrorFormat("id {0} 商品未配置", mShopItemId);
                return;
            }
            if (shopData.shop_id == (int)EPointShopType.Aid)//援助商店的话需要导师等级判断
            {
                textCondition = transform.Find("Text_Condition").GetComponent<Text>();
                if (shopData.tutor_level > Sys_Attr.Instance.tutorLevel)
                {
                    textCondition.gameObject.SetActive(true);
                    textCondition.text = LanguageHelper.GetTextContent(8114, shopData.tutor_level.ToString());
                }
                else
                {
                    textCondition.gameObject.SetActive(false);
                }
            }
            CSVItem.Data itemInfo = CSVItem.Instance.GetConfData(shopData.item_id);
            if (itemInfo == null)
            {
                Debug.LogErrorFormat("id {0} 道具表未配置", shopData.item_id);
                return;
            }

            ImageHelper.SetIcon(_ImgIcon, itemInfo.icon_id);

            textName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(shopData.item_id).name_id);

            costOriginal.SetActive(!priceChange && shopData.price_before != shopItem.Price);

            CSVItem.Data priceTypeData = CSVItem.Instance.GetConfData(shopData.price_type);
            if (priceTypeData != null)
            {
                ImageHelper.SetIcon(iconOriginal, CSVItem.Instance.GetConfData(shopData.price_type).small_icon_id);
                ImageHelper.SetIcon(iconNow, CSVItem.Instance.GetConfData(shopData.price_type).small_icon_id);
            }
            else
            {
                Debug.LogErrorFormat("CSVItem 没有配置  id = {0}", shopData.price_type);
            }

            textOriginalValue.text = shopData.price_before.ToString();
            textNowValue.text = shopItem.Price.ToString();

            toggle.SetSelected(mShopItemId == Sys_Mall.Instance.SelectShopItemId, true);

            imgMarkDiscount.gameObject.SetActive(false);
            imgMarkLimitTime.gameObject.SetActive(false);
            imgSellOut.gameObject.SetActive(false);

            if (!priceChange && shopData.price_before != shopItem.Price)
            {
                float discount = shopItem.Price * 1.0f / shopData.price_before;
                discount *= 10;
                imgMarkDiscount.gameObject.SetActive(true);
                textDiscountNum.text = string.Format("{0:0.0}", discount);
            }

            bool isSellOut = false;
            if (shopData.limit_type != 0)
            {
                isSellOut = Sys_Mall.Instance.IsSellOut(mShopItemId);
                imgMarkLimitTime.gameObject.SetActive(true);

                Text textLan = imgMarkLimitTime.transform.Find("Text").GetComponent<Text>();

                bool isSelfType = shopData.personal_limit != 0;
                if (isSelfType)
                {
                    textLan.text = LanguageHelper.GetTextContent(2009802, Sys_Mall.Instance.CalSelfLeftNum(mShopItemId).ToString());
                }

                bool isAllServerType = shopData.limit_type == 3 || shopData.limit_type == 4; //每日或每周全服限购类型
                if (isAllServerType)
                {
                    int leftNum = Sys_Mall.Instance.CalAllServerLeftNum(mShopItemId);
                    textLan.text = LanguageHelper.GetTextContent(2009802, leftNum.ToString());
                }
            }

            imgSellOut.gameObject.SetActive(isSellOut);
            ImageHelper.SetImageGray(_ImgIcon, isSellOut);
            ImageHelper.SetImageGray(iconOriginal, isSellOut);
            ImageHelper.SetImageGray(iconNow, isSellOut);

        }

        private void OnItemClick(PropItem propItem)
        {
            toggle.SetSelected(true, true);
        }

        private void OnRefreshShopData()
        {
            if (mShopItemId != 0)
            {
                ShopItem shopitem = Sys_Mall.Instance.GetNewData(mShopItemId);
                if (shopitem != null)
                    UpdateInfo(shopitem, isPriceChange);
            }
        }

        private void OnCurrencyChnange()
        {
            if (mShopItemId != 0)
            {
                ShopItem shopitem = Sys_Mall.Instance.GetNewData(mShopItemId);
                if (shopitem != null)
                    UpdateInfo(shopitem, isPriceChange);
            }
        }
    }
}