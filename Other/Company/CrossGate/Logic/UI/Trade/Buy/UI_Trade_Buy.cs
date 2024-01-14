using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Buy : UIBase
    {
        #region BuySingle
        private class BuySingle
        {
            private Transform transform;

            private Text m_TextNum;
            private Text m_TextPrice;

            public void Init(Transform trans)
            {
                transform = trans;

                m_TextNum = transform.Find("VIew_Line/Num/Image_BG/Label_Num").GetComponent<Text>();
                m_TextPrice = transform.Find("VIew_Line/Total/Image_BG/Label_Num").GetComponent<Text>();
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            public void UpdateInfo(TradeBrief brief)
            {
                m_TextNum.text = brief.Count.ToString();
                m_TextPrice.text = brief.Price.ToString();
            }
        }
        #endregion

        #region BuyMulti
        private class BuyMulti
        {
            private Transform transform;

            private Text m_TextPrice;

            private UI_Common_Num m_InputNum;

            private Button m_BtnSubNum;
            private Button m_BtnAddNum;
            private Button m_BtnMaxNum;

            //private Text m_TextSaleNum;
            private Text m_TextTotalPrice;

            private uint m_Price;
            private uint m_Num;
            private uint numMax;
            public uint Num { get { return m_Num; } }

            public void Init(Transform trans)
            {
                transform = trans;

                //m_TextNum = transform.Find("VIew_Line/Num/Label_Num").GetComponent<Text>();
                m_TextPrice = transform.Find("VIew_Line/Single/Label_Num").GetComponent<Text>();

                m_InputNum = new UI_Common_Num();
                m_InputNum.Init(transform.Find("VIew_Line/Num0/Image_Number/InputField_Number"));
                m_InputNum.RegEnd(OnInputEndNum);

                m_BtnSubNum = transform.Find("VIew_Line/Num0/Image_Number/Button_Sub").GetComponent<Button>();
                UI_LongPressButton SubButtonNum = m_BtnSubNum.gameObject.AddComponent<UI_LongPressButton>();
                //SubButtonNum.interval = 0.3f;
                //SubButtonNum.bPressAcc = true;
                SubButtonNum.onRelease.AddListener(OnClickBtnNumSub);
                //SubButtonNum.OnPressAcc.AddListener(OnClickBtnNumSub);

                m_BtnAddNum = transform.Find("VIew_Line/Num0/Image_Number/Button_Add").GetComponent<Button>();
                UI_LongPressButton AddButtonNum = m_BtnAddNum.gameObject.AddComponent<UI_LongPressButton>();
                //AddButtonNum.interval = 0.3f;
                //AddButtonNum.bPressAcc = true;
                AddButtonNum.onRelease.AddListener(OnClickBtnNumAdd);
                //AddButtonNum.OnPressAcc.AddListener(OnClickBtnNumAdd);

                m_BtnMaxNum = transform.Find("VIew_Line/Num0/Image_Number/Button_Max").GetComponent<Button>();
                m_BtnMaxNum.onClick.AddListener(OnClickBtnNumMax);

                m_TextTotalPrice = transform.Find("VIew_Line/Total/Image_BG/Label_Num").GetComponent<Text>();
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnInputEndNum(uint num)
            {
                m_Num = num;
                if (m_Num > numMax)
                    m_Num = numMax;

                if (m_Num < 1u)
                    m_Num = 1u;

                m_InputNum.SetData(m_Num);
                CalTotalPrice();
            }

            private void OnClickBtnNumSub()
            {
                if (m_Num > 1)
                {
                    m_Num--;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011242u));
                }

                m_InputNum.SetData(m_Num);
                CalTotalPrice();
            }

            private void OnClickBtnNumAdd()
            {
                if (m_Num < numMax)
                {
                    m_Num++;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011243u));
                }

                m_InputNum.SetData(m_Num);
                CalTotalPrice();
            }

            private void OnClickBtnNumMax()
            {
                m_Num = numMax;
                m_InputNum.SetData(m_Num);
                CalTotalPrice();
            }

            private void CalTotalPrice()
            {
                uint totalPrice = m_Num * m_Price;

                m_TextTotalPrice.text = totalPrice.ToString();
            }

            public void UpdateInfo(TradeBrief brief)
            {
                numMax = brief.Count;
                m_Price = brief.Price;
                m_TextPrice.text = m_Price.ToString();

                m_Num = 1u;
                m_InputNum.SetData(m_Num);

                CalTotalPrice();
            }
        }
        #endregion

        #region BuyAssign
        private class BuyAssign
        {
            private Transform transform;

            private Image imgTargetName;
            private Text m_TextTargetName;
            private GameObject m_NameLine;

            private Image imgTargetPrice;
            private Text m_TextTargetPrice;
            private GameObject m_TargetPriceLine;

            private Image imgPrice;
            private Text m_TextPrice;
            private GameObject m_PriceLine;

            private Text m_TextInfo;
            private Text m_TextLeftTime;
            
            public void Init(Transform trans)
            {
                transform = trans;

                imgTargetName = transform.Find("VIew_Line/Person/Image_BG").GetComponent<Image>();
                m_TextTargetName = transform.Find("VIew_Line/Person/Image_BG/Label_Name").GetComponent<Text>();
                m_NameLine = transform.Find("VIew_Line/Person/Image_Line").gameObject;

                imgTargetPrice = transform.Find("VIew_Line/Appoint/Image_BG").GetComponent<Image>();
                m_TextTargetPrice = transform.Find("VIew_Line/Appoint/Image_BG/Label_Num").GetComponent<Text>();
                m_TargetPriceLine = transform.Find("VIew_Line/Appoint/Image_Line").gameObject;

                imgPrice = transform.Find("VIew_Line/Auction/Image_BG").GetComponent<Image>();
                m_TextPrice = transform.Find("VIew_Line/Auction/Image_BG/Label_Num").GetComponent<Text>();
                m_PriceLine = transform.Find("VIew_Line/Auction/Image_Line").gameObject;

                m_TextInfo = transform.Find("Text/Text0").GetComponent<Text>();
                m_TextLeftTime = transform.Find("Text/Text1").GetComponent<Text>();
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            public void UpdateInfo(TradeBrief brief)
            {
                m_TextTargetName.text = brief.TargetName.ToStringUtf8();
                m_TextTargetPrice.text = brief.TargetPrice.ToString();
                m_TextPrice.text = brief.Price.ToString();

                m_NameLine.SetActive(false);
                m_TargetPriceLine.SetActive(false);
                m_PriceLine.SetActive(false);

                bool assignSelf = brief.TargetId == Sys_Role.Instance.RoleId;
                bool isAssignTime = Sys_Trade.Instance.IsAssignTime(brief);
                if (isAssignTime)
                {
                    m_PriceLine.SetActive(assignSelf);
                    m_NameLine.SetActive(!assignSelf);
                    m_TargetPriceLine.SetActive(!assignSelf);

                    ImageHelper.SetImageGray(imgTargetName, !assignSelf, true);
                    ImageHelper.SetImageGray(imgTargetPrice, !assignSelf, true);
                    ImageHelper.SetImageGray(imgPrice, assignSelf, true);

                    uint leftTime = 0u;
                    if (brief.TargetTime > Sys_Time.Instance.GetServerTime())
                        leftTime = brief.TargetTime - Sys_Time.Instance.GetServerTime();

                    m_TextLeftTime.text = LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_2);
                    m_TextInfo.text = LanguageHelper.GetTextContent(2011015);
                }
                else
                {
                    m_NameLine.SetActive(true);
                    m_TargetPriceLine.SetActive(true);

                    ImageHelper.SetImageGray(imgTargetName, true, true);
                    ImageHelper.SetImageGray(imgTargetPrice, true, true);

                    m_TextLeftTime.text = LanguageHelper.GetTextContent(2011231, brief.Price.ToString());
                    m_TextInfo.text = LanguageHelper.GetTextContent(2011230);
                }
            }
        }
        #endregion

        #region BuyPublicity
        private class BuyPublicity
        {
            private Transform transform;

            private Text m_TextNum;
            private Text m_TextPrice;

            private Text m_TextLeftTime;

            public void Init(Transform trans)
            {
                transform = trans;

                m_TextNum = transform.Find("VIew_Line/Num/Image_BG/Label_Num").GetComponent<Text>();
                m_TextPrice = transform.Find("VIew_Line/Total/Image_BG/Label_Num").GetComponent<Text>();

                m_TextLeftTime = transform.Find("Text/Text1").GetComponent<Text>();
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            public void UpdateInfo(TradeBrief brief)
            {
                m_TextNum.text = brief.Count.ToString();
                m_TextPrice.text = brief.Price.ToString();

                uint endTime = brief.OnsaleTime;
                m_TextLeftTime.text = LanguageHelper.TimeToString(endTime - Framework.TimeManager.GetServerTime(), LanguageHelper.TimeFormat.Type_4);
            }
        }
        #endregion

        private enum BuyOpType
        {
            None,
            Single,
            Multi,
            Assign,
            Publicity,
        }
        private BuyOpType m_OpType = BuyOpType.None;

        private Button m_BtnClose;
        private PropSpecialItem m_PropItem;

        private BuySingle m_BuySingle;
        private BuyMulti m_BuyMulti;
        private BuyAssign m_BuyAssign;
        private BuyPublicity m_BuyPublicity;

        private Button m_BtnBuy;

        private TradeBrief m_Brief;
        private CSVCommodity.Data m_GoodData;

        protected override void OnOpen(object arg)
        {
            m_Brief = null;
            if (arg != null)
                m_Brief = (TradeBrief)arg;

            if (m_Brief != null)
                m_GoodData = CSVCommodity.Instance.GetConfData(m_Brief.InfoId);
        }

        protected override void OnLoaded()
        {
            m_BtnClose = transform.Find("Animator/View_TipsBg02_Smallest/Btn_Close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(() => { this.CloseSelf();});

            m_PropItem = new PropSpecialItem();
            m_PropItem.BindGameObject(transform.Find("Animator/View_BuyDetail/Image_ItemBG/PropItem").gameObject);

            m_BuySingle = new BuySingle();
            m_BuySingle.Init(transform.Find("Animator/View_BuyDetail/Line2"));

            m_BuyMulti = new BuyMulti();
            m_BuyMulti.Init(transform.Find("Animator/View_BuyDetail/Line1"));

            m_BuyAssign = new BuyAssign();
            m_BuyAssign.Init(transform.Find("Animator/View_BuyDetail/Line0"));

            m_BuyPublicity = new BuyPublicity();
            m_BuyPublicity.Init(transform.Find("Animator/View_BuyDetail/Line3"));

            m_BtnBuy = transform.Find("Animator/View_BuyDetail/Btn_01").GetComponent<Button>();
            m_BtnBuy.onClick.AddListener(OnClickBuy);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }        

        //protected override void ProcessEvents(bool toRegister)
        //{
        //    //Sys_TerrorSeries.Instance.eventEmitter.Handle<int>(Sys_TerrorSeries.EEvents.OnSelectLine, OnSelectLine, toRegister);
        //    //Sys_TerrorSeries.Instance.eventEmitter.Handle(Sys_TerrorSeries.EEvents.OnUpdateTaskData, OnUpdateTaskData, toRegister);
        //}

        private void OnClickBuy()
        {
            uint price = m_Brief.Price;
            if (m_Brief.TargetId == Sys_Role.Instance.RoleId)
            {
                if (Sys_Time.Instance.GetServerTime() < m_Brief.TargetTime)
                    price = m_Brief.TargetPrice;
            }
            uint count = m_Brief.Count;
            switch (m_OpType)
            {
                case BuyOpType.Single:
                    break;
                case BuyOpType.Multi:
                    count = m_BuyMulti.Num;
                    break;
                case BuyOpType.Assign:
                    //price = m_Brief.psa;
                    break;
                case BuyOpType.Publicity:
                    break;
            }

            CSVCommodity.Data goodData = CSVCommodity.Instance.GetConfData(m_Brief.InfoId);
            if (!Sys_Trade.Instance.CheckGoodBuyLevel(goodData))
                return;
            Sys_Trade.Instance.OnBuyReq(m_Brief.BCross, m_Brief.InfoId, m_Brief.GoodsUid, price, count);
        }

        private void UpdateInfo()
        {
            if (null == m_Brief)
            {
                Debug.LogErrorFormat("m_Brief is Null !!!");
                return;
            }

            if (null == m_GoodData)
            {
                Debug.LogErrorFormat("m_GoodData is Null !!!");
                return;
            }

            ItemData item = new ItemData(99, m_Brief.GoodsUid, m_Brief.InfoId, m_Brief.Count, 0, false, false, null, null, 0, null);
            item.SetQuality(m_Brief.Color);

            PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(item, false, false);
            showItem.SetTradeEnd(true);
            showItem.SetCross(m_Brief.BCross);
            showItem.SetLevel(m_Brief.Petlv);
            m_PropItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_Buy, showItem));
            m_PropItem.txtName.text = LanguageHelper.GetTextContent(item.cSVItemData.name_id);

            m_BuySingle.Hide();
            m_BuyMulti.Hide();
            m_BuyAssign.Hide();
            m_BuyPublicity.Hide();

            bool isAssign = Sys_Trade.Instance.IsAssign(m_Brief);
            if (isAssign) //指定
            {
                m_OpType = BuyOpType.Assign;
                m_BuyAssign.Show();
                m_BuyAssign.UpdateInfo(m_Brief);
            }
            else
            {
                bool isPublicity = Sys_Trade.Instance.IsPublicity(m_Brief);
                if (isPublicity)
                {
                    m_OpType = BuyOpType.Publicity;
                    m_BuyPublicity.Show();
                    m_BuyPublicity.UpdateInfo(m_Brief);
                }
                else
                {
                    if (m_GoodData.bulk_sale > 1)
                    {
                        m_OpType = BuyOpType.Multi;
                        m_BuyMulti.Show();
                        m_BuyMulti.UpdateInfo(m_Brief);
                    }
                    else
                    {
                        m_OpType = BuyOpType.Single;
                        m_BuySingle.Show();
                        m_BuySingle.UpdateInfo(m_Brief);
                    }
                }
            }
        }
       
    }
}


