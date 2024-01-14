using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_SellDetail_OutOfDate : UIBase
    {
        private class LimitMulti
        {
            private Transform transform;

            private Text m_TextNum;
            private Text m_TextPrice;
            private Button m_BtnPriceSub;
            private Button m_BtnPriceAdd;
            private Text m_TextPriceCompare;

            private Text m_TextTotalPrice;

            private Text m_TextBoothPrice;
            private Button m_BtnBoothInfo;

            private Button m_BtnOffSale;
            private Button m_BtnReSale;

            private TradeBrief m_Brief;
            private uint m_Num;
            private uint m_Price;
            private uint recommendPrice = 0;

            private int percentMax = Sys_Trade.Instance.PriceFloatedLimit();
            private int percentMin = -Sys_Trade.Instance.PriceFloatedLimit();
            private int percentDelta = Sys_Trade.Instance.PriceDelta();
            private int percent = 0;

            public void Init(Transform trans)
            {
                transform = trans;

                InputField inputNum = transform.Find("VIew_Line/Image0/InputField").GetComponent<InputField>();
                inputNum.enabled = false;

                m_TextNum = transform.Find("VIew_Line/Image0/InputField/Label_Num").GetComponent<Text>();

                Button btnPrice = transform.Find("VIew_Line/Image1/InputField").GetComponent<Button>();
                m_TextPrice = transform.Find("VIew_Line/Image1/InputField/Text").GetComponent<Text>();

                m_BtnPriceSub = transform.Find("VIew_Line/Image1/Button_Sub").GetComponent<Button>();
                UI_LongPressButton SubButtonPrice = m_BtnPriceSub.gameObject.AddComponent<UI_LongPressButton>();
                //SubButtonPrice.interval = 0.3f;
                //SubButtonPrice.bPressAcc = true;
                SubButtonPrice.onRelease.AddListener(OnClickBtnPriceSub);
                //SubButtonPrice.OnPressAcc.AddListener(OnClickBtnPriceSub);

                m_BtnPriceAdd = transform.Find("VIew_Line/Image1/Button_Add").GetComponent<Button>();
                UI_LongPressButton AddButtonPrice = m_BtnPriceAdd.gameObject.AddComponent<UI_LongPressButton>();
                //AddButtonPrice.interval = 0.3f;
                //AddButtonPrice.bPressAcc = true;
                AddButtonPrice.onRelease.AddListener(OnClickBtnPriceAdd);

                m_TextPriceCompare = transform.Find("VIew_Line/Image1/Text").GetComponent<Text>();

                m_TextTotalPrice = transform.Find("VIew_Line/Image2/Label_Num").GetComponent<Text>();

                m_TextBoothPrice = transform.Find("VIew_Line/Image3/Label_Num").GetComponent<Text>();
                m_BtnBoothInfo = transform.Find("VIew_Line/Image3/Button").GetComponent<Button>();
                m_BtnBoothInfo.onClick.AddListener(OnClickBoothInfo);

                m_BtnOffSale = transform.Find("Button/Btn_01").GetComponent<Button>();
                m_BtnOffSale.onClick.AddListener(OnOffSale);
                m_BtnReSale = transform.Find("Button/Btn_02").GetComponent<Button>();
                m_BtnReSale.onClick.AddListener(OnReSale);
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

                CalCost();
            }

            private void OnClickBtnPriceAdd()
            {
                if (percent < percentMax)
                {
                    percent += percentDelta;
                }

                CalCost();
            }

            private void CalCost()
            {
                ShowPrice();
                CalPriceCompare();
                CalTotalPrice();
                CalBoothPrice();
            }

            private void ShowPrice()
            {
                m_Price = (uint)(recommendPrice * (1 + percent / 100f));
                m_TextPrice.text = m_Price.ToString();
            }

            private void CalPriceCompare()
            {
                m_TextPriceCompare.text = Sys_Trade.Instance.CalPriceCompare(percent);
            }

            private void CalBoothPrice()
            {
                uint boothPrice = Sys_Trade.Instance.CalBoothPrice(m_Price);
                boothPrice = Sys_Trade.Instance.GetBoothPrice(boothPrice, m_Brief.BCross);
                m_TextBoothPrice.text = boothPrice.ToString();
            }

            private void CalTotalPrice()
            {
                uint totalPrice = m_Price * m_Num;
                m_TextTotalPrice.text = totalPrice.ToString();
            }

            private void OnClickBoothInfo()
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, Sys_Trade.Instance.GetBoothPriceTip());
            }

            private void OnOffSale()
            {
                Sys_Trade.Instance.OnOffSaleReq(m_Brief.GoodsUid);
            }

            private void OnReSale()
            {
                if (m_Brief.IsDenied)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011255u));
                else
                    Sys_Trade.Instance.OnReSaleReq(m_Brief.BCross, m_Brief.GoodsUid, m_Price);
            }

            public void Update(TradeBrief brief, uint recPrice)
            {
                m_Brief = brief;

                m_Num = brief.Count;
                m_Price = brief.Price;
                recommendPrice = recPrice;
                
                m_TextNum.text = m_Num.ToString();
                percent = 0;

                CalCost();
            }
        }
 
        private class LimitSingle
        {
            private Transform transform;

            private Text m_TextPrice;
            private Button m_BtnPriceSub;
            private Button m_BtnPriceAdd;
            private Button m_BtnPriceMax;
            private Text m_TextPriceCompare;

            private Text m_TextBoothPrice;
            private Button m_BtnBoothInfo;

            private Button m_BtnOffSale;
            private Button m_BtnReSale;

            private TradeBrief m_Brief;
            private uint m_Price;
            private uint recommendPrice = 0;

            private int percentMax = Sys_Trade.Instance.PriceFloatedLimit();
            private int percentMin = -Sys_Trade.Instance.PriceFloatedLimit();
            private int percentDelta = Sys_Trade.Instance.PriceDelta();
            private int percent = 0;

            public void Init(Transform trans)
            {
                transform = trans;

                Button btnPrice = transform.Find("VIew_Line/Image0/InputField").GetComponent<Button>();
                btnPrice.enabled = false;
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

                m_TextBoothPrice = transform.Find("VIew_Line/Image1/Label_Num").GetComponent<Text>();
                m_BtnBoothInfo = transform.Find("VIew_Line/Image1/Button").GetComponent<Button>();
                m_BtnBoothInfo.onClick.AddListener(OnClickBoothInfo);

                m_BtnOffSale = transform.Find("Button/Btn_01").GetComponent<Button>();
                m_BtnOffSale.onClick.AddListener(OnOffSale);
                m_BtnReSale = transform.Find("Button/Btn_02").GetComponent<Button>();
                m_BtnReSale.onClick.AddListener(OnReSale);
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

                CalCost();
            }

            private void OnClickBtnPriceAdd()
            {
                if (percent < percentMax)
                {
                    percent += percentDelta;
                }

                CalCost();
            }

            private void OnClickBtnPriceMax()
            {
                percent = percentMax;

                CalCost();
            }

            private void CalCost()
            {
                ShowPrice();
                CalPriceCompare();
                CalBoothPrice();
            }

            private void ShowPrice()
            {
                m_Price = (uint)(recommendPrice * (1 + percent / 100f));
                m_TextPrice.text = m_Price.ToString();
            }

            private void CalPriceCompare()
            {
                m_TextPriceCompare.text = Sys_Trade.Instance.CalPriceCompare(percent);
            }

            private void CalBoothPrice()
            {
                uint boothPrice = Sys_Trade.Instance.CalBoothPrice(m_Price);
                boothPrice = Sys_Trade.Instance.GetBoothPrice(boothPrice, m_Brief.BCross);
                m_TextBoothPrice.text = boothPrice.ToString();
            }

            private void OnClickBoothInfo()
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, Sys_Trade.Instance.GetBoothPriceTip());
            }

            private void OnOffSale()
            {
                Sys_Trade.Instance.OnOffSaleReq(m_Brief.GoodsUid);
            }

            private void OnReSale()
            {
                if (m_Brief.IsDenied)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011255u));
                else
                    Sys_Trade.Instance.OnReSaleReq(m_Brief.BCross, m_Brief.GoodsUid, m_Price);
            }

            public void Update(TradeBrief brief, uint recPrice)
            {
                m_Brief = brief;

                m_Price = brief.Price;
                recommendPrice = recPrice;

                percent = 0;

                CalCost();
            }
        }

        private class FreeCommon
        {
            private Transform transform;

            private UI_Common_Num m_InputPrice;
            private Text m_TextUnit;

            private Text m_TextBoothPrice;
            private Button m_BtnBoothInfo;

            private Button m_BtnOffSale;
            private Button m_BtnReSale;

            private TradeBrief m_Brief;
            private uint m_Price;

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

                m_BtnOffSale = transform.Find("Button/Btn_01").GetComponent<Button>();
                m_BtnOffSale.onClick.AddListener(OnOffSale);
                m_BtnReSale = transform.Find("Button/Btn_02").GetComponent<Button>();
                m_BtnReSale.onClick.AddListener(OnReSale);
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
                m_Price = num;
                CalPriceUnit();
                CalBoothPrice();
            }

            private void OnInputPriceEnd(uint num)
            {
                m_Price = num;
                CalPriceUnit();
                CalBoothPrice();

                //价格有最小提示
                uint minPrice = Sys_Trade.Instance.GetFreePricingMin(m_Brief.BCross);
                if (m_Price < minPrice)
                {
                    uint lanId = m_Brief.BCross ? 2011285u: 2011156;
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(lanId, minPrice.ToString()));
                }
            }

            private void OnClickBoothInfo()
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, Sys_Trade.Instance.GetBoothPriceTip());
            }

            private void CalPriceUnit()
            {
                m_TextUnit.text = Sys_Trade.Instance.CalUnit(m_Price);
            }

            private void CalBoothPrice()
            {
                //摊位费
                uint boothPrice = Sys_Trade.Instance.CalBoothPrice(m_Price);
                boothPrice = Sys_Trade.Instance.GetBoothPrice(boothPrice, m_Brief.BCross);
                m_TextBoothPrice.text = boothPrice.ToString();
            }

            private void OnOffSale()
            {
                Sys_Trade.Instance.OnOffSaleReq(m_Brief.GoodsUid);
            }

            private void OnReSale()
            {
                if (m_Brief.IsDenied)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011255u));
                    return;
                }

                //摊位费不足，需要弹出货币兑换
                uint boothPrice = Sys_Trade.Instance.CalBoothPrice(m_Brief.Count * m_Price);
                boothPrice = Sys_Trade.Instance.GetBoothPrice(boothPrice, m_Brief.BCross);
                if (boothPrice > Sys_Bag.Instance.GetItemCount((uint)ECurrencyType.SilverCoin))
                {
                    ExchangeCoinParm exchangeCoinParm = new ExchangeCoinParm();
                    exchangeCoinParm.ExchangeType = (uint)ECurrencyType.SilverCoin;
                    exchangeCoinParm.needCount = 0;
                    UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, exchangeCoinParm);
                    //UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, (uint)ECurrencyType.SilverCoin);
                    return;
                }

                //需要弹二次确认框
                Sys_Trade.SaleConfirmParam param = new Sys_Trade.SaleConfirmParam();
                param.BReSale = true;
                param.Brief = new Packet.TradeBrief();
                param.Brief.BCross = m_Brief.BCross;
                param.Brief.InfoId = m_Brief.InfoId;
                param.Brief.Count = m_Brief.Count;
                param.Brief.GoodsUid = m_Brief.GoodsUid;
                param.Brief.Price = m_Price;
                param.Brief.Color = m_Brief.Color;

                UIManager.CloseUI(EUIID.UI_Trade_SellDetail_OutOfDate);
                UIManager.OpenUI(EUIID.UI_Trade_Sure_Tip, false, param);
            }

            public void UpdateInfo(TradeBrief brief)
            {
                m_Brief = brief;

                m_Price = brief.Price;
                CalPriceUnit();
                CalBoothPrice();
                m_InputPrice.SetData(m_Price);
            }
        }

        private class FreeAssign
        {
            private Transform transform;

            private Text m_TextTargetName;
            private Text m_TextTargetPrice;
            private Text m_TextHotPrice;

            private Button m_BtnOffSale;

            private TradeBrief m_Brief;
            
            public void Init(Transform trans)
            {
                transform = trans;

                m_TextTargetName = transform.Find("VIew_Line/Image0/Label_Num").GetComponent<Text>();
                //m_LineName = transform.Find("VIew_Line/Image0/Line");
                m_TextTargetPrice = transform.Find("VIew_Line/Image1/Price/Label_Num").GetComponent<Text>();
                //m_LinePrice = transform.Find("VIew_Line/Image1/Line");
                m_TextHotPrice = transform.Find("VIew_Line/Image2/Price/Label_Num").GetComponent<Text>();

                m_BtnOffSale = transform.Find("Button/Btn_01").GetComponent<Button>();
                m_BtnOffSale.onClick.AddListener(OnOffSale);
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnOffSale()
            {
                Sys_Trade.Instance.OnOffSaleReq(m_Brief.GoodsUid);
            }

            public void UpdateInfo(TradeBrief brief)
            {
                m_Brief = brief;
                m_TextTargetName.text = brief.TargetName.ToStringUtf8();
                m_TextTargetPrice.text = brief.TargetPrice.ToString();
                m_TextHotPrice.text = brief.Price.ToString();
            }
        }

        private Button m_BtnClose;

        private PropSpecialItem m_PropItem;

        private Text m_Name;
        private Text m_Des;

        private Text m_TextType;
        private UI_Trade_SellDetail_Type m_SellDetailType;
        private UI_Trade_SellDetail_OutOfDate_List m_List;
        private GameObject goEmpty;

        private LimitMulti m_LimitMulti;
        private LimitSingle m_LimitSingle;
        private FreeCommon m_FreeCommon;
        private FreeAssign m_FreeAssign;

        //private Button m_OffSale;

        private enum OpType
        {
            None,
            LimitSingle,
            LimitMulti,
            FreeCommon,
            FreeAssign,
        }

        private OpType m_OpType = OpType.None;

        private TradeBrief m_Brief;
        private CSVCommodity.Data m_GoodData;
        private CmdTradeComparePriceRes m_ComparePriceData;

        //protected virtual void OnInit() { }
        protected override void OnOpen(object arg)
        {
            m_Brief = null;
            if (arg != null)
                m_Brief = (TradeBrief)arg;

            m_GoodData = null;
            if (m_Brief != null)
                m_GoodData = CSVCommodity.Instance.GetConfData(m_Brief.InfoId);
        }

        protected override void OnLoaded()
        {
            m_PropItem = new PropSpecialItem();
            m_PropItem.BindGameObject(transform.Find("Animator/Image_Right/Top/PropItem").gameObject);

            m_Name = transform.Find("Animator/Image_Right/Top/Text_Name").GetComponent<Text>();
            m_Des = transform.Find("Animator/Image_Right/Top/Text_Dex").GetComponent<Text>();

            m_BtnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(() => { this.CloseSelf(); });

            m_TextType = transform.Find("Animator/Image_LeftBG/Text").GetComponent<Text>();

            m_SellDetailType = new UI_Trade_SellDetail_Type();
            m_SellDetailType.Init(transform.Find("Animator/Image_LeftBG/Toggles"));

            m_List = new UI_Trade_SellDetail_OutOfDate_List();
            m_List.Init(transform.Find("Animator/Image_LeftBG/Rect"));

            goEmpty = transform.Find("Animator/Image_LeftBG/Empty").gameObject;

            m_LimitMulti = new LimitMulti();
            m_LimitMulti.Init(transform.Find("Animator/Image_Right/State0"));

            m_LimitSingle = new LimitSingle();
            m_LimitSingle.Init(transform.Find("Animator/Image_Right/State1"));

            m_FreeCommon = new FreeCommon();
            m_FreeCommon.Init(transform.Find("Animator/Image_Right/State2"));

            m_FreeAssign = new FreeAssign();
            m_FreeAssign.Init(transform.Find("Animator/Image_Right/State3"));

            //m_OffSale = transform.Find("Animator/Image_Right/Button/Btn_01").GetComponent<Button>();
            //m_OffSale.onClick.AddListener(OnOffSale);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        //protected override void OnDestroy()
        //{
        //    //m_Currency?.Dispose();
        //}

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Trade.Instance.eventEmitter.Handle<Sys_Trade.SellDetailType>(Sys_Trade.EEvents.OnSellDetailType, OnSellDeitalType, toRegister);
            Sys_Trade.Instance.eventEmitter.Handle<CmdTradeComparePriceRes>(Sys_Trade.EEvents.OnComparePriceNtf, OnComparePrice, toRegister);
            //Sys_TerrorSeries.Instance.eventEmitter.Handle<int>(Sys_TerrorSeries.EEvents.OnSelectLine, OnSelectLine, toRegister);
            //Sys_TerrorSeries.Instance.eventEmitter.Handle(Sys_TerrorSeries.EEvents.OnUpdateTaskData, OnUpdateTaskData, toRegister);
        }

        private void OnSellDeitalType(Sys_Trade.SellDetailType type)
        {
            if (type == Sys_Trade.SellDetailType.Selling)
            {
                goEmpty.SetActive(m_ComparePriceData.Goods.Count == 0);
                m_List.OnUpdateListInfos(new List<TradeBrief>(m_ComparePriceData.Goods));
            }
            else if (type == Sys_Trade.SellDetailType.Publicity)
            {
                goEmpty.SetActive(m_ComparePriceData.PublicGoods.Count == 0);
                m_List.OnUpdateListInfos(new List<TradeBrief>(m_ComparePriceData.PublicGoods));
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
                    m_LimitMulti.Update(m_Brief, res.RecommendPrice);
                    m_List.OnUpdateListInfos(new List<TradeBrief>(res.Goods));
                    goEmpty.SetActive(m_ComparePriceData.Goods.Count == 0);
                    break;
                case OpType.LimitSingle:
                    m_LimitSingle.Show();
                    m_LimitSingle.Update(m_Brief, res.RecommendPrice);
                    m_List.OnUpdateListInfos(new List<TradeBrief>(res.Goods));
                    goEmpty.SetActive(m_ComparePriceData.Goods.Count == 0);
                    break;
                case OpType.FreeCommon:
                    m_FreeCommon.Show();
                    m_FreeCommon.UpdateInfo(m_Brief);
                    m_List.OnUpdateListInfos(new List<TradeBrief>(res.Goods));
                    m_SellDetailType.Show();
                    m_SellDetailType.OnSelect(Sys_Trade.SellDetailType.Selling);
                    m_TextType.gameObject.SetActive(false);
                    break;
                case OpType.FreeAssign:
                    m_FreeAssign.Show();
                    m_FreeAssign.UpdateInfo(m_Brief);
                    m_SellDetailType.Show();
                    m_SellDetailType.OnSelect(Sys_Trade.SellDetailType.Selling);
                    m_TextType.gameObject.SetActive(false);
                    break;
            }
        }

        private void UpdateInfo()
        {
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(m_Brief.InfoId);

            ItemData item = new ItemData(99, m_Brief.GoodsUid, m_Brief.InfoId, m_Brief.Count, 0, false, false, null, null, 0, null);
            item.SetQuality(m_Brief.Color);

            PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(item, false, false);
            showItem.SetTradeEnd(true);
            showItem.SetCross(m_Brief.BCross);
            showItem.SetLevel(m_Brief.Petlv);
            m_PropItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_SellDetail_Publicity, showItem));

            m_Name.text = LanguageHelper.GetTextContent(itemData.name_id);
            m_Des.text = LanguageHelper.GetTextContent(itemData.describe_id);

            m_TextType.gameObject.SetActive(true);
            m_SellDetailType.Hide();

            m_LimitSingle.Hide();
            m_LimitMulti.Hide();
            m_FreeCommon.Hide();
            m_FreeAssign.Hide();

            m_OpType = OpType.None;

            if (Sys_Trade.Instance.IsAssign(m_Brief))
            {
                m_OpType = OpType.FreeAssign;
            }
            else
            {
                if (m_GoodData.pricing_type == 1u) //自由定价
                {
                    m_OpType = OpType.FreeCommon;
                }
                else
                {
                    //批量
                    if (m_GoodData.bulk_sale > 1)
                        m_OpType = OpType.LimitMulti;
                    else
                        m_OpType = OpType.LimitSingle;
                }
            }

            Sys_Trade.Instance.OnComparePriceReq(m_Brief.BCross, m_GoodData.id);
        }
    }
}


