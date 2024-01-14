using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_SellDetail_Publicity : UIBase
    {
        private class FreeCommon 
        {
            private Transform transform;
            private GameObject gameObject;

            private Text m_TextLeftTime;
            private Text m_TextPrice;
  
            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                m_TextLeftTime = transform.Find("VIew_Line/Image0/Label_Time").GetComponent<Text>();
                m_TextPrice = transform.Find("VIew_Line/Image1/Label_Num").GetComponent<Text>();
            }

            public void Show()
            {
                gameObject.SetActive(true);
            }

            public void Hide()
            {
                gameObject.SetActive(false);
            }

            public void UpdateInfo(TradeBrief brief)
            {
                m_TextLeftTime.text = LanguageHelper.TimeToString(brief.OnsaleTime - Sys_Time.Instance.GetServerTime(), LanguageHelper.TimeFormat.Type_4);
                m_TextPrice.text = brief.Price.ToString();
            }
        }

        private class FreeAssign
        {
            private Transform transform;
            private GameObject gameObject;

            private Text m_TextLeftTime;
            private Text m_TextTargetName;
            private Text m_TextTargetPrice;
            private Text m_TextHotPrice;

            public void Init(Transform trans)
            {
                transform = trans;
                gameObject = transform.gameObject;

                m_TextLeftTime = transform.Find("VIew_Line/Image0/Label_Time").GetComponent<Text>();
                m_TextTargetName = transform.Find("VIew_Line/Image1/Label_Num").GetComponent<Text>();
                m_TextTargetPrice = transform.Find("VIew_Line/Image2/Price/Label_Num").GetComponent<Text>();
                m_TextHotPrice = transform.Find("VIew_Line/Image3/Price/Label_Num").GetComponent<Text>();
            }

            public void Show()
            {
                gameObject.SetActive(true);
            }

            public void Hide()
            {
                gameObject.SetActive(false);
            }

            public void UpdateInfo(TradeBrief brief)
            {
                m_TextLeftTime.text = LanguageHelper.TimeToString(brief.OnsaleTime - Sys_Time.Instance.GetServerTime(), LanguageHelper.TimeFormat.Type_4);
                m_TextTargetName.text = brief.TargetName.ToStringUtf8();
                m_TextTargetPrice.text = brief.TargetPrice.ToString();
                m_TextHotPrice.text = brief.Price.ToString();
            }
        }

        private Button m_BtnClose;
        
        private PropSpecialItem m_PropItem;

        private Text m_Name;
        private Text m_Des;

        private Image m_ImgTip;
        private Text m_TextWatchTimes;

        private Text m_TextType;
        private UI_Trade_SellDetail_Type m_SellDetailType;
        private UI_Trade_SellDetail_Publicity_List m_List;
        private GameObject goEmpty;

        private FreeCommon m_FreeCommon;
        private FreeAssign m_FreeAssign;

        private Button m_OffSale;

        private enum OpType
        {
            None,
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

            m_ImgTip = transform.Find("Animator/Image_Right/Top/Tips/Image0").GetComponent<Image>();
            m_TextWatchTimes = transform.Find("Animator/Image_Right/Top/Text_Person").GetComponent<Text>();

            m_BtnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(() =>{ this.CloseSelf(); });

            m_TextType = transform.Find("Animator/Image_LeftBG/Text").GetComponent<Text>();

            m_SellDetailType = new UI_Trade_SellDetail_Type();
            m_SellDetailType.Init(transform.Find("Animator/Image_LeftBG/Toggles"));

            m_List = new UI_Trade_SellDetail_Publicity_List();
            m_List.Init(transform.Find("Animator/Image_LeftBG/Rect"));

            goEmpty = transform.Find("Animator/Image_LeftBG/Empty").gameObject;

            m_FreeCommon = new FreeCommon();
            m_FreeCommon.Init(transform.Find("Animator/Image_Right/State0"));

            m_FreeAssign = new FreeAssign();
            m_FreeAssign.Init(transform.Find("Animator/Image_Right/State1"));

            m_OffSale = transform.Find("Animator/Image_Right/Button/Btn_01").GetComponent<Button>();
            m_OffSale.onClick.AddListener(OnOffSale);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        //protected override void OnDestroy()
        //{
        //    //m_Currency?.Dispose();
        //}

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Trade.Instance.eventEmitter.Handle<Sys_Trade.SellDetailType>(Sys_Trade.EEvents.OnSellDetailType, OnSellDeitalType, toRegister);
            Sys_Trade.Instance.eventEmitter.Handle<CmdTradeComparePriceRes>(Sys_Trade.EEvents.OnComparePriceNtf, OnComparePrice, toRegister);
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
        }

        private void OnComparePrice(CmdTradeComparePriceRes res)
        {
            m_ComparePriceData = res;
            switch (m_OpType)
            {
                case OpType.FreeCommon:
                    m_FreeCommon.Show();
                    m_FreeCommon.UpdateInfo(m_Brief);
                    m_List.OnUpdateListInfos(new List<TradeBrief>(res.Goods));
                    m_SellDetailType.Show();
                    m_SellDetailType.OnSelect(Sys_Trade.SellDetailType.Publicity);
                    m_TextType.gameObject.SetActive(false);
                    break;
                case OpType.FreeAssign:
                    m_FreeAssign.Show();
                    m_FreeAssign.UpdateInfo(m_Brief);
                    m_SellDetailType.Show();
                    m_SellDetailType.OnSelect(Sys_Trade.SellDetailType.Publicity);
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

            //Tip
            m_ImgTip.enabled = true;
            ImageHelper.SetIcon(m_ImgTip, 993412);

            //关注
            CSVCommodity.Data data = CSVCommodity.Instance.GetConfData(m_Brief.InfoId);
            if (data.attention)
            {
                m_TextWatchTimes.gameObject.SetActive(true);
                m_TextWatchTimes.text = LanguageHelper.GetTextContent(2011081, m_Brief.WatchTimes.ToString());
            }
            else
            {
                m_TextWatchTimes.gameObject.SetActive(false);
            }

            m_TextType.gameObject.SetActive(true);
            m_SellDetailType.Hide();

            m_FreeCommon.Hide();
            m_FreeAssign.Hide();

            m_OpType = OpType.None;

            if (Sys_Trade.Instance.IsAssign(m_Brief))
            {
                m_OpType = OpType.FreeAssign;
            }
            else
            {
                m_OpType = OpType.FreeCommon;
            }

            Sys_Trade.Instance.OnComparePriceReq(m_Brief.BCross, m_GoodData.id);
        }

        private void OnOffSale()
        {
            Sys_Trade.Instance.OnOffSaleReq(m_Brief.GoodsUid);
        }
    }
}


