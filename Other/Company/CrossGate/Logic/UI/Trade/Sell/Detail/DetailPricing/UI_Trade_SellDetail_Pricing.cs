using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_SellDetail_Pricing : UIBase
    {
        private class LimitPricingSingle
        {
            private Transform transform;

            private Button m_BtnPrice;
            private Text m_TextPrice;
            private Button m_BtnPriceSub;
            private Button m_BtnPriceAdd;
            private Button m_BtnPriceMax;
            private Text m_TextPriceCompare;

            private Text m_TextBoothPrice;
            private Button m_BtnBoothInfo;

            private uint recommendPrice;
            private uint m_Price = 0u;

            private int percentMax = Sys_Trade.Instance.PriceFloatedLimit();
            private int percentMin = -Sys_Trade.Instance.PriceFloatedLimit();
            private int percentDelta = Sys_Trade.Instance.PriceDelta();
            private int percent = 0;

            public uint Price { get { return m_Price;  } }

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnPrice = transform.Find("VIew_Line/Image0/InputField").GetComponent<Button>();
                m_BtnPrice.enabled = false;

                m_TextPrice = transform.Find("VIew_Line/Image0/InputField/Text").GetComponent<Text>();

                m_BtnPriceSub = transform.Find("VIew_Line/Image0/Button_Sub").GetComponent<Button>();
                UI_LongPressButton SubButtonPrice = m_BtnPriceSub.gameObject.AddComponent<UI_LongPressButton>();
                //SubButtonPrice.interval = 0.3f;
                //SubButtonPrice.bPressAcc = true;
                SubButtonPrice.onRelease.AddListener(OnClickBtnPriceSub);
                //SubButtonPrice.OnPressAcc.AddListener(OnClickBtnPriceSub);

                m_BtnPriceAdd = transform.Find("VIew_Line/Image0/Button_Add").GetComponent<Button>();
                UI_LongPressButton AddButtonPrice = m_BtnPriceAdd.gameObject.AddComponent<UI_LongPressButton>();
                //AddButtonPrice.interval = 0.3f;
                //AddButtonPrice.bPressAcc = true;
                AddButtonPrice.onRelease.AddListener(OnClickBtnPriceAdd);

                m_BtnPriceMax = transform.Find("VIew_Line/Image0/Button_Max").GetComponent<Button>();
                m_BtnPriceMax.onClick.AddListener(OnClickBtnPriceMax);

                m_TextPriceCompare = transform.Find("VIew_Line/Image0/Text").GetComponent<Text>();

                //摊位
                m_TextBoothPrice = transform.Find("VIew_Line/Image1/Label_Num").GetComponent<Text>();
                m_BtnBoothInfo = transform.Find("VIew_Line/Image1/Button").GetComponent<Button>();
                m_BtnBoothInfo.onClick.AddListener(OnClickBoothInfo);
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnClickBtnPriceSub()
            {
                if (percent > percentMin)
                {
                    percent -= percentDelta;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011154));
                }

                CalPriceShow();
                CalBoothPrice();
                CalPriceCompare();
            }

            private void OnClickBtnPriceAdd()
            {
                if (percent < percentMax)
                {
                    percent += percentDelta;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011155));
                }

                CalPriceShow();
                CalBoothPrice();
                CalPriceCompare();
            }

            private void OnClickBtnPriceMax()
            {
                percent = percentMax;

                CalPriceShow();
                CalBoothPrice();
                CalPriceCompare();
            }

            private void OnClickBoothInfo()
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, Sys_Trade.Instance.GetBoothPriceTip());
            }

            private void CalPriceShow()
            {
                ulong nPrice = (ulong)recommendPrice * (100 + (ulong)percent) / 100;
                m_Price = (uint)nPrice;
                m_TextPrice.text = m_Price.ToString();
            }

            private void CalBoothPrice()
            {
                //摊位费
                uint boothPrice = Sys_Trade.Instance.CalBoothPrice(m_Price);
                boothPrice = Sys_Trade.Instance.GetBoothPrice(boothPrice, false);
                m_TextBoothPrice.text = boothPrice.ToString();
            }

            private void CalPriceCompare()
            {
                m_TextPriceCompare.text = Sys_Trade.Instance.CalPriceCompare(percent);
            }

            public void Update(CSVCommodity.Data goodData, uint recPrice)
            {
                //单价
                recommendPrice = recPrice;
                m_Price = recommendPrice;

                percent = 0;

                m_TextPrice.text = m_Price.ToString();

                //摊位费
                CalBoothPrice();

                CalPriceCompare();
            }
        }

        private class LimitPricingMulti
        {
            private Transform transform;

            private UI_Common_Num m_InputNum;
            private Button m_BtnNumSub;
            private Button m_BtnNumAdd;
            private Button m_BtnNumMax;

            private Button m_BtnPrice;
            private Text m_TextPrice;
            private Button m_BtnPriceSub;
            private Button m_BtnPriceAdd;
            private Text m_TextPriceCompare;

            private Text m_TextTotalPrice;

            private Text m_TextBoothPrice;
            private Button m_BtnBoothInfo;

            private uint m_Num;
            private uint m_Price;

            private uint minNum = 0u;
            private uint maxNum = 0u;

            private uint recommendPrice = 0u;

            private int percentMax = Sys_Trade.Instance.PriceFloatedLimit();
            private int percentMin = -Sys_Trade.Instance.PriceFloatedLimit();
            private int percentDelta = Sys_Trade.Instance.PriceDelta();
            private int percent = 0;

            public uint Num { get { return m_Num; } }
            public uint Price { get { return m_Price; } }

            public void Init(Transform trans)
            {
                transform = trans;

                //数量
                m_InputNum = new UI_Common_Num();
                m_InputNum.Init(transform.Find("VIew_Line/Image0/InputField"));
                m_InputNum.RegEnd(OnInputNumEnd);

                m_BtnNumSub = transform.Find("VIew_Line/Image0/Button_Sub").GetComponent<Button>();
                UI_LongPressButton SubButtonNum = m_BtnNumSub.gameObject.AddComponent<UI_LongPressButton>();
                //SubButtonNum.interval = 0.3f;
                //SubButtonNum.bPressAcc = true;
                SubButtonNum.onRelease.AddListener(OnClickBtnNumSub);
                //SubButtonNum.OnPressAcc.AddListener(OnClickBtnNumSub);


                m_BtnNumAdd = transform.Find("VIew_Line/Image0/Button_Add").GetComponent<Button>();
                UI_LongPressButton AddButtonNum = m_BtnNumAdd.gameObject.AddComponent<UI_LongPressButton>();
                //AddButtonNum.interval = 0.3f;
                //AddButtonNum.bPressAcc = true;
                AddButtonNum.onRelease.AddListener(OnClickBtnNumAdd);
                //AddButtonNum.OnPressAcc.AddListener(OnClickBtnNumAdd);

                m_BtnNumMax = transform.Find("VIew_Line/Image0/Button_Max").GetComponent<Button>();
                m_BtnNumMax.onClick.AddListener(OnClickBtnNumMax);

                //单价
                m_BtnPrice = transform.Find("VIew_Line/Image1/InputField").GetComponent<Button>();
                m_BtnPrice.enabled = false;
                m_TextPrice = transform.Find("VIew_Line/Image1/InputField/Text").GetComponent<Text>();

                m_BtnPriceSub = transform.Find("VIew_Line/Image1/Button_Sub").GetComponent<Button>();
                UI_LongPressButton SubButtonPrice = m_BtnPriceSub.gameObject.AddComponent<UI_LongPressButton>();
                SubButtonPrice.interval = 0.3f;
                //SubButtonPrice.bPressAcc = true;
                SubButtonPrice.onRelease.AddListener(OnClickBtnPriceSub);
                //SubButtonPrice.OnPressAcc.AddListener(OnClickBtnPriceSub);

                m_BtnPriceAdd = transform.Find("VIew_Line/Image1/Button_Add").GetComponent<Button>();
                UI_LongPressButton AddButtonPrice = m_BtnPriceAdd.gameObject.AddComponent<UI_LongPressButton>();
                AddButtonPrice.interval = 0.3f;
                //AddButtonPrice.bPressAcc = true;
                AddButtonPrice.onRelease.AddListener(OnClickBtnPriceAdd);
                //AddButtonPrice.OnPressAcc.AddListener(OnClickBtnPriceAdd);

                //和推荐价格对比
                m_TextPriceCompare = transform.Find("VIew_Line/Image1/Text").GetComponent<Text>();

                //总价
                m_TextTotalPrice = transform.Find("VIew_Line/Image2/Label_Num").GetComponent<Text>();
                //摊位
                m_TextBoothPrice = transform.Find("VIew_Line/Image3/Label_Num").GetComponent<Text>();
                m_BtnBoothInfo = transform.Find("VIew_Line/Image3/Button").GetComponent<Button>();
                m_BtnBoothInfo.onClick.AddListener(OnClickBoothInfo);
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnInputNumEnd(uint num)
            {
                m_Num = num > maxNum ? maxNum : num;
                m_Num = m_Num < minNum ? minNum : m_Num;
                m_InputNum.SetData(m_Num);
                CalPrice();
            }

            private void OnClickBtnNumSub()
            {
                if (m_Num > minNum)
                {
                    m_Num--;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011242u));
                }

                m_InputNum.SetData(m_Num);
                CalPrice();
            }

            private void OnClickBtnNumAdd()
            {
                if (m_Num < maxNum)
                {
                    m_Num++;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011243u));
                }

                m_InputNum.SetData(m_Num);
                CalPrice();
            }

            private void OnClickBtnNumMax()
            {
                m_Num = maxNum;
                m_InputNum.SetData(m_Num);
                CalPrice();
            }

            private void OnClickBtnPriceSub()
            {
                if (percent > percentMin)
                {
                    percent -= percentDelta;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011154));
                }

                CalPrice();
                CalPriceCompare();
            }

            private void OnClickBtnPriceAdd()
            {
                if (percent < percentMax)
                {
                    percent += percentDelta;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011155));
                }

                CalPrice();
                CalPriceCompare();
            }

            private void OnClickBoothInfo()
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, Sys_Trade.Instance.GetBoothPriceTip());
            }

            private void CalPrice()
            {
                ulong nPrice = (ulong)recommendPrice * (100 + (ulong)percent) / 100;
                //Debug.LogError(fPrice.ToString());
                //int nPrice = Mathf.RoundToInt(fPrice);
                //Debug.LogError(nPrice.ToString());
                //string tempPrice = string.Format("{0}", recommendPrice * (1 + percent * 0.01f)); // 热更
                m_Price = (uint)nPrice;
                m_TextPrice.text = m_Price.ToString();

                //Debug.LogErrorFormat("price = {0}, recPrice = {1}, percent = {2}", m_Price, recommendPrice, percent);
                CalTotalPrice();
                CalBoothPrice();
            }

            private void CalTotalPrice()
            {
                uint totalPrice = m_Num * m_Price;
                m_TextTotalPrice.text = totalPrice.ToString();
            }

            private void CalBoothPrice()
            {
                //摊位费
                uint boothPrice = Sys_Trade.Instance.CalBoothPrice(m_Num * m_Price);
                boothPrice = Sys_Trade.Instance.GetBoothPrice(boothPrice, false);
                m_TextBoothPrice.text = boothPrice.ToString();
            }

            private void CalPriceCompare()
            {
                m_TextPriceCompare.text = Sys_Trade.Instance.CalPriceCompare(percent);
            }

            public void Update(ItemData item, CSVCommodity.Data goodData, uint recPrice)
            {
                //数量
                minNum = 1u;
                maxNum = item.Count < goodData.bulk_sale ? item.Count : goodData.bulk_sale;
                m_Num = maxNum;
                m_InputNum.SetData(m_Num);

                //单价
                recommendPrice = recPrice;
                m_Price = recommendPrice;

                percent = 0;

                CalPrice();

                CalPriceCompare();
            }
        }

        private class FreePricing
        {
            private Transform transform;

            private UI_Common_Num m_InputPrice;
            private Text m_TextUnit;

            private Text m_TextBoothPrice;
            private Button m_BtnBoothInfo;

            public uint Price;

            public void Init(Transform trans)
            {
                transform = trans;

                m_InputPrice = new UI_Common_Num();
                m_InputPrice.Init(transform.Find("VIew_Line/Image0/InputField"));
                m_InputPrice.RegChange(OnInputPriceChange);
                m_InputPrice.RegEnd(OnInputPriceEnd);

                m_TextUnit = transform.Find("VIew_Line/Image0/Text").GetComponent<Text>();

                //摊位
                m_TextBoothPrice = transform.Find("VIew_Line/Image1/Label_Num").GetComponent<Text>();
                m_BtnBoothInfo = transform.Find("VIew_Line/Image1/Button").GetComponent<Button>();
                m_BtnBoothInfo.onClick.AddListener(OnClickBoothInfo);
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnInputPriceChange(uint num)
            {
                Price = num;
                CalPriceUnit();
                CalBoothPrice();
            }

            private void OnInputPriceEnd(uint num)
            {
                Price = num;
                CalPriceUnit();
                CalBoothPrice();

                //价格有最小提示
                if (Price < Sys_Trade.Instance.GetFreePricingMin())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011156, Sys_Trade.Instance.GetFreePricingMin().ToString()));
                }
            }

            private void OnClickBoothInfo()
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, Sys_Trade.Instance.GetBoothPriceTip());
            }

            private void CalPriceUnit()
            {
                m_TextUnit.text = Sys_Trade.Instance.CalUnit(Price);
            }

            private void CalBoothPrice()
            {
                //摊位费
                uint boothPrice = Sys_Trade.Instance.CalBoothPrice(Price);
                boothPrice = Sys_Trade.Instance.GetBoothPrice(boothPrice, false);
                m_TextBoothPrice.text = boothPrice.ToString();
            }

            public void UpdateInfo()
            {
                //单价
                Price = 0u;
                m_InputPrice.Reset();
                m_InputPrice.SetDefault(LanguageHelper.GetTextContent(2011108));

                //
                CalPriceUnit();
                //摊位费
                CalBoothPrice();
            }
        }

        private Button m_BtnClose;

        private Text m_TextType;
        private UI_Trade_SellDetail_Type m_SellDetailType;
        private UI_Trade_SellDetail_Pricing_List m_List;
        private GameObject goEmpty;

        private PropSpecialItem m_PropItem;

        private Text m_Name;
        private Text m_Des;

        private LimitPricingSingle m_LimitSingle;
        private LimitPricingMulti m_LimitMulti;
        private FreePricing m_FreePricing;

        private Button m_BtnSale;

        private Sys_Trade.TradeItemInfo m_itemInfo;
        private ItemData _saleItem;
        private CSVCommodity.Data m_GoodData;

        private enum OpType
        {
            None,
            LimitSingle,
            LimitMulti,
            Free,
        }

        private OpType m_OpType = OpType.None;

        private CmdTradeComparePriceRes m_ComparePriceData;

        protected override void OnOpen(object arg)
        {
            //_saleItem = null;
            //m_GoodData = null;

            if (arg != null)
            {
                m_itemInfo = (Sys_Trade.TradeItemInfo)arg;
            }
        }

        protected override void OnLoaded()
        {
            m_PropItem = new PropSpecialItem();
            m_PropItem.BindGameObject(transform.Find("Animator/Image_Right/Top/PropItem").gameObject);
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(m_PropItem.btnImage.gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (ret) => { OnClick(); });


            m_Name = transform.Find("Animator/Image_Right/Top/Text_Name").GetComponent<Text>();
            m_Des = transform.Find("Animator/Image_Right/Top/Text_Dex").GetComponent<Text>();

            m_BtnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(() =>{ this.CloseSelf(); });

            m_TextType = transform.Find("Animator/Image_LeftBG/Text").GetComponent<Text>();

            m_SellDetailType = new UI_Trade_SellDetail_Type();
            m_SellDetailType.Init(transform.Find("Animator/Image_LeftBG/Toggles"));

            m_List = new UI_Trade_SellDetail_Pricing_List();
            m_List.Init(transform.Find("Animator/Image_LeftBG/Rect"));

            goEmpty = transform.Find("Animator/Image_LeftBG/Empty").gameObject;

            m_LimitSingle = new LimitPricingSingle();
            m_LimitSingle.Init(transform.Find("Animator/Image_Right/State1"));

            m_LimitMulti = new LimitPricingMulti();
            m_LimitMulti.Init(transform.Find("Animator/Image_Right/State0"));

            m_FreePricing = new FreePricing();
            m_FreePricing.Init(transform.Find("Animator/Image_Right/State2"));

            m_BtnSale = transform.Find("Animator/Image_Right/Button/Btn_01").GetComponent<Button>();
            m_BtnSale.onClick.AddListener(OnSaleClick);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Trade.Instance.eventEmitter.Handle<Sys_Trade.SellDetailType>(Sys_Trade.EEvents.OnSellDetailType, OnSellDeitalType, toRegister);
            Sys_Trade.Instance.eventEmitter.Handle<CmdTradeComparePriceRes>(Sys_Trade.EEvents.OnComparePriceNtf, OnComparePrice, toRegister);
        }

        private void OnClickPropItem(PropSpecialItem propItem)
        {
            //ItemData itemData = Sys_Trade.Instance.GetItemDataByTradeItemInfo(m_itemInfo);
            //PropIconLoader.ShowItemData temp = new PropIconLoader.ShowItemData(itemData.Id, itemData.Count, true, false, false, false, false, false, false, false);
            //MessageBoxEvt msgEvt = new MessageBoxEvt(EUIID.UI_Trade_SellDetail_Pricing, temp);

            //UIManager.OpenUI(EUIID.UI_Message_Box, false, msgEvt);
        }

        private void OnClick()
        {
            _saleItem = Sys_Trade.Instance.GetItemDataByTradeItemInfo(m_itemInfo);
            if (_saleItem != null)
            {
                if (_saleItem.cSVItemData.type_id == (int)EItemType.Equipment)
                {
                    EquipTipsData tipData = new EquipTipsData();
                    tipData.equip = _saleItem;
                    tipData.isCompare = true;
                    tipData.isShowOpBtn = false;

                    UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
                }
                else if (_saleItem.cSVItemData.type_id == (int)EItemType.Pet)
                {
                    ClientPet clientPet = new ClientPet(_saleItem.Pet);
                    UIManager.OpenUI(EUIID.UI_Pet_Details, false, clientPet);
                }
                else if (_saleItem.cSVItemData.type_id == (int)EItemType.Ornament)
                {
                    OrnamentTipsData tipData = new OrnamentTipsData();
                    tipData.equip = _saleItem;
                    tipData.isCompare = false;
                    tipData.sourceUiId = EUIID.UI_Trade_SellDetail_Pricing;
                    UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
                }
                else if (_saleItem.cSVItemData.type_id == (int)EItemType.PetEquipment)
                {
                    PetEquipTipsData petEquipTipsData = new PetEquipTipsData();
                    petEquipTipsData.openUI = EUIID.UI_Trade_SellDetail_Pricing;
                    petEquipTipsData.petEquip = _saleItem;
                    petEquipTipsData.isCompare = false;
                    petEquipTipsData.isShowOpBtn = true;
                    UIManager.OpenUI(EUIID.UI_Tips_PetMagicCore, false, petEquipTipsData);
                }
                else
                {
                    PropIconLoader.ShowItemData temp = new PropIconLoader.ShowItemData(_saleItem.Id, _saleItem.Count, true, false, false, false, false, false, false, false);
                    MessageBoxEvt msgEvt = new MessageBoxEvt(EUIID.UI_Trade_Sell, temp);

                    UIManager.OpenUI(EUIID.UI_Message_Box, false, msgEvt);
                }
            }
        }

        private void OnSaleClick()
        {
            bool bCross = false;
            uint infoId = m_GoodData.id;
            uint salePrice = 0u;
            uint count = 0u;

            switch (m_OpType)
            {
                case OpType.LimitMulti:
                    salePrice = m_LimitMulti.Price;
                    count = m_LimitMulti.Num;
                    break;
                case OpType.LimitSingle:
                    salePrice = m_LimitSingle.Price;
                    count = 1u;
                    break;
                case OpType.Free:
                    salePrice = m_FreePricing.Price;
                    count = 1u;
                    break;
            }

            if (m_OpType == OpType.Free)
            {
                if (salePrice <= 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011157));
                    return;
                }

                if (salePrice < Sys_Trade.Instance.GetFreePricingMin())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011156, Sys_Trade.Instance.GetFreePricingMin().ToString()));
                    return;
                }
            }
            else
            {
                if (salePrice == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011157));
                    return;
                }
            }

            //摊位费不足，需要弹出货币兑换
            uint boothPrice = Sys_Trade.Instance.CalBoothPrice(count * (uint)salePrice);
            boothPrice = Sys_Trade.Instance.GetBoothPrice(boothPrice, false);
            if (boothPrice > Sys_Bag.Instance.GetItemCount((uint)ECurrencyType.SilverCoin))
            {
                ExchangeCoinParm exchangeCoinParm = new ExchangeCoinParm();
                exchangeCoinParm.ExchangeType = (uint)ECurrencyType.SilverCoin;
                exchangeCoinParm.needCount = 0;
                UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, exchangeCoinParm);
                //UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, (uint)ECurrencyType.SilverCoin);
                return;
            }

            if (_saleItem.cSVItemData.type_id == (int) EItemType.Equipment)
            {
                if (_saleItem.IsLocked)
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, LanguageHelper.GetTextContent(_saleItem.cSVItemData.name_id), LanguageHelper.GetTextContent(_saleItem.cSVItemData.name_id));
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        Sys_Bag.Instance.OnItemLockReq(_saleItem.Uuid, false);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    return;
                }
            }
            else if (_saleItem.cSVItemData.type_id == (int) EItemType.Pet)
            {
                if (_saleItem.Pet != null && _saleItem.Pet.Islocked)
                {
                    ClientPet clientPet = new ClientPet(_saleItem.Pet);
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, clientPet.GetPetNmae(), clientPet.GetPetNmae());
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                    
                        Sys_Pet.Instance.OnPetLockReq(clientPet.GetPetUid(), false);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    return;
                }
            }
            else if (_saleItem.cSVItemData.type_id == (int) EItemType.PetEquipment)
            {
                if (_saleItem.IsLocked)
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, LanguageHelper.GetTextContent(_saleItem.cSVItemData.name_id), LanguageHelper.GetTextContent(_saleItem.cSVItemData.name_id));
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        Sys_Bag.Instance.OnItemLockReq(_saleItem.Uuid, false);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    return;
                }
            }
            else if (_saleItem.cSVItemData.type_id == (int) EItemType.Ornament)
            {
                if (_saleItem.IsLocked)
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, LanguageHelper.GetTextContent(_saleItem.cSVItemData.name_id), LanguageHelper.GetTextContent(_saleItem.cSVItemData.name_id));
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        Sys_Bag.Instance.OnItemLockReq(_saleItem.Uuid, false);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    return;
                }
            }

            Sys_Trade.Instance.OnSaleReq(bCross, infoId, m_itemInfo.uID, (uint)salePrice, count);
        }

        private void OnSellDeitalType(Sys_Trade.SellDetailType type)
        {
            if (type == Sys_Trade.SellDetailType.Selling)
            {
                m_List.Hide();
                goEmpty.SetActive(m_ComparePriceData.Goods.Count == 0);
                if(m_ComparePriceData.Goods.Count > 0)
                {
                    m_List.Show();
                    m_List.OnUpdateListInfos(new List<TradeBrief>(m_ComparePriceData.Goods));
                }
                
            }
            else if (type == Sys_Trade.SellDetailType.Publicity)
            {
                m_List.Hide();
                goEmpty.SetActive(m_ComparePriceData.PublicGoods.Count == 0);

                if (m_ComparePriceData.PublicGoods.Count > 0)
                {
                    m_List.Show();
                    m_List.OnUpdateListInfos(new List<TradeBrief>(m_ComparePriceData.PublicGoods));
                }
            }
            else
            {

            }
            //m_List?.OnListInfo(type);
        }

        private void OnComparePrice(CmdTradeComparePriceRes res)
        {
            m_ComparePriceData = res;
            switch (m_OpType)
            {
                case OpType.LimitMulti:
                    m_LimitMulti.Show();
                    m_LimitMulti.Update(_saleItem, m_GoodData, res.RecommendPrice);
                    m_List.Show();
                    m_List.OnUpdateListInfos(new List<TradeBrief>(res.Goods));
                    goEmpty.SetActive(m_ComparePriceData.Goods.Count == 0);
                    break;
                case OpType.LimitSingle:
                    m_LimitSingle.Show();
                    m_LimitSingle.Update(m_GoodData, res.RecommendPrice);
                    m_List.Show();
                    m_List.OnUpdateListInfos(new List<TradeBrief>(res.Goods));
                    goEmpty.SetActive(m_ComparePriceData.Goods.Count == 0);
                    break;
                case OpType.Free:
                    m_FreePricing.Show();
                    m_FreePricing.UpdateInfo();
                    m_SellDetailType.Show();
                    m_SellDetailType.OnSelect(Sys_Trade.SellDetailType.Selling);
                    m_TextType.gameObject.SetActive(false);
                    break;
            }
        }

        private void UpdateInfo()
        {
            _saleItem = Sys_Trade.Instance.GetItemDataByTradeItemInfo(m_itemInfo);
            if (_saleItem != null)
                m_GoodData = CSVCommodity.Instance.GetConfData(_saleItem.Id);

            if (_saleItem != null)
            {
                CSVItem.Data itemData = CSVItem.Instance.GetConfData(_saleItem.Id);
                bool isPet = itemData.type_id == (int)EItemType.Pet;

                PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(_saleItem, false, false,false);
                showItem.SetTradeEnd(true);
                showItem.AddClickAction(OnClickPropItem);
                if (isPet)
                    showItem.SetLevel(_saleItem.Pet.SimpleInfo.Level);
                m_PropItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_Sell, showItem));

                m_Name.text = LanguageHelper.GetTextContent(itemData.name_id);
                //m_Des.text = LanguageHelper.GetTextContent(itemData.describe_id);
            }

            m_List.Hide();
            goEmpty.SetActive(true);

            m_TextType.gameObject.SetActive(true);
            m_SellDetailType.Hide();

            m_LimitSingle.Hide();
            m_LimitMulti.Hide();
            m_FreePricing.Hide();

            m_OpType = OpType.None;

            //限制定价
            if (m_GoodData.pricing_type == 0)
            {
                //批量
                if (m_GoodData.bulk_sale > 1)
                    m_OpType = OpType.LimitMulti;
                else
                    m_OpType = OpType.LimitSingle;
            }
            else
            {
                m_OpType = OpType.Free;
            }

            Sys_Trade.Instance.OnComparePriceReq(Sys_Trade.Instance.CurSellServerType == Sys_Trade.ServerType.Cross, m_GoodData.id);
        }
    }
}


