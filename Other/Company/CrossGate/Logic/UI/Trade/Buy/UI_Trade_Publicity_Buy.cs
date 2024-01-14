using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Publicity_Buy : UIBase
    {
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
                }
                else
                {
                    m_NameLine.SetActive(true);
                    m_TargetPriceLine.SetActive(true);

                    ImageHelper.SetImageGray(imgTargetName, true, true);
                    ImageHelper.SetImageGray(imgTargetPrice, true, true);
                }
            }
        }

        private class BuyPublicity
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

        private enum BuyOpType
        {
            None,
            Assign,
            Publicity,
        }
        private BuyOpType m_OpType = BuyOpType.None;

        private Button m_BtnClose;
        private PropSpecialItem m_PropItem;

        private BuyAssign m_BuyAssign;
        private BuyPublicity m_BuyPublicity;

        private Text m_TextLeftTime;

        private Button m_BtnBuy;
        private Text m_TextBtnBuy;
        private Button m_BtnCancel;

        private TradeBrief m_Brief;
        private CSVCommodity.Data m_GoodData;

        private Lib.Core.Timer m_Timer;
        private bool IsAdvanceBuy = false;

        private bool IsBuyed = false;

        protected override void OnOpen(object arg)
        {
            IsBuyed = false;
            m_Brief = null;
            if (arg != null)
                m_Brief = (TradeBrief)arg;

            if (m_Brief != null)
                m_GoodData = CSVCommodity.Instance.GetConfData(m_Brief.InfoId);
        }

        protected override void OnLoaded()
        {
            m_BtnClose = transform.Find("Animator/View_TipsBg02_Smallest/Btn_Close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(OnClickClose);

            m_PropItem = new PropSpecialItem();
            m_PropItem.BindGameObject(transform.Find("Animator/View_BuyDetail/Image_ItemBG/PropItem").gameObject);

            m_BuyAssign = new BuyAssign();
            m_BuyAssign.Init(transform.Find("Animator/View_BuyDetail/Line0"));

            m_BuyPublicity = new BuyPublicity();
            m_BuyPublicity.Init(transform.Find("Animator/View_BuyDetail/Line3"));

            m_TextLeftTime = transform.Find("Animator/View_BuyDetail/Text/Text0").GetComponent<Text>();

            m_BtnBuy = transform.Find("Animator/View_BuyDetail/Btn_01").GetComponent<Button>();
            m_BtnBuy.onClick.AddListener(OnClickBuy);
            m_TextBtnBuy = transform.Find("Animator/View_BuyDetail/Btn_01/Text_01").GetComponent<Text>();

            m_BtnCancel = transform.Find("Animator/View_BuyDetail/Btn_02").GetComponent<Button>();
            m_BtnCancel.onClick.AddListener(OnClickCancel);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {
            m_Timer?.Cancel();
            m_Timer = null;
        }        

        //protected override void ProcessEvents(bool toRegister)
        //{
        //    //Sys_TerrorSeries.Instance.eventEmitter.Handle<int>(Sys_TerrorSeries.EEvents.OnSelectLine, OnSelectLine, toRegister);
        //    //Sys_TerrorSeries.Instance.eventEmitter.Handle(Sys_TerrorSeries.EEvents.OnUpdateTaskData, OnUpdateTaskData, toRegister);
        //}
        
        private void OnClickClose()
        {
            uint price = m_Brief.TargetId == Sys_Role.Instance.RoleId ? m_Brief.TargetPrice : m_Brief.Price;
            if (!IsAdvanceBuy)
                Sys_Trade.Instance.OnOfferPriceReq(m_Brief.BCross, m_Brief.InfoId, m_Brief.GoodsUid, m_Brief.Price, false);
            this.CloseSelf();
        }

        private void OnClickBuy()
        {
            if (!Sys_Trade.Instance.CheckGoodBuyLevel(m_GoodData))
                return;

            uint price = m_Brief.TargetId == Sys_Role.Instance.RoleId ? m_Brief.TargetPrice : m_Brief.Price;

            if (!CheckPriceNeed(price))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(104417));
                return;
            }

            if (IsAdvanceBuy)
            {
                //判断特权卡有没有激活
                bool isActive = Sys_OperationalActivity.Instance.CheckSpecialCardIsActive(2);
                if (!isActive)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011909));
                    this.CloseSelf();
                    return;
                }
                
                if (m_Brief.OnsaleTime > Sys_Time.Instance.GetServerTime())
                {
                    uint tempTime = m_Brief.OnsaleTime - Sys_Time.Instance.GetServerTime();
                    if (tempTime > Sys_Trade.Instance.PublicityRemainTime())
                    {
                        uint time = Sys_Trade.Instance.AdvanceBuyTimes();
                        if (time > 0)
                        {
                            Sys_Trade.Instance.OnAdvanceOffPriceReq(m_Brief.GoodsUid, price);
                            this.CloseSelf();
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011273));
                            this.CloseSelf();
                        }
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011275));
                        this.CloseSelf();
                    }
                }
                else 
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011275));
                    this.CloseSelf();
                }
            }
            else
            {
                Sys_Trade.Instance.OnOfferPriceReq(m_Brief.BCross, m_Brief.InfoId, m_Brief.GoodsUid, price, true);
                m_BtnBuy.gameObject.SetActive(false);
                m_BtnCancel.gameObject.SetActive(true);

                IsBuyed = true;
            }
        }

        private void OnClickCancel()
        {
            uint price = m_Brief.TargetId == Sys_Role.Instance.RoleId ? m_Brief.TargetPrice : m_Brief.Price;

            Sys_Trade.Instance.OnOfferPriceReq(m_Brief.BCross, m_Brief.InfoId, m_Brief.GoodsUid, price, false);

            m_BtnCancel.gameObject.SetActive(false);
            m_BtnBuy.gameObject.SetActive(true);
        }

        private bool CheckPriceNeed(uint price)
        {
            long count =  Sys_Bag.Instance.GetItemCount(2u);
            return count >= price;
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
            m_PropItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_Publicity_Buy, showItem));
            m_PropItem.txtName.text = LanguageHelper.GetTextContent(item.cSVItemData.name_id);

            m_BtnCancel.gameObject.SetActive(false);
            m_BtnBuy.gameObject.SetActive(true);

            if (IsBuyed)
            {
                m_BtnBuy.gameObject.SetActive(false);
                m_BtnCancel.gameObject.SetActive(true);
            }
            else
            {
                m_BuyAssign.Hide();
                m_BuyPublicity.Hide();
            }

            bool isAssign = Sys_Trade.Instance.IsAssign(m_Brief);
            if (isAssign) //指定
            {
                m_OpType = BuyOpType.Assign;
                m_BuyAssign.Show();
                m_BuyAssign.UpdateInfo(m_Brief);
            }
            else
            {
                m_OpType = BuyOpType.Publicity;
                m_BuyPublicity.Show();
                m_BuyPublicity.UpdateInfo(m_Brief);
            }

            uint tempTime = m_Brief.OnsaleTime - Sys_Time.Instance.GetServerTime();
            IsAdvanceBuy = tempTime > Sys_Trade.Instance.PublicityRemainTime();
            if (!IsAdvanceBuy)
            {
                //leftTime
                uint leftTime = 0;
                if (m_Brief.OnsaleTime > Sys_Time.Instance.GetServerTime())
                    leftTime = m_Brief.OnsaleTime - Sys_Time.Instance.GetServerTime();
                ShowTime(leftTime);

                m_Timer?.Cancel();
                m_Timer = Lib.Core.Timer.Register(leftTime, () => {
                    m_Timer?.Cancel();
                    leftTime = 0u;
                    ShowTime(leftTime);
                    this.CloseSelf();
                }, (dt) => {
                    float temp = leftTime - dt;
                    if (temp < 0f)
                        temp = 0f;
                    ShowTime((uint)Mathf.RoundToInt(temp));
                });

                m_TextBtnBuy.text = LanguageHelper.GetTextContent(2011283); //购买
            }
            else
            {
                m_TextBtnBuy.text = LanguageHelper.GetTextContent(2011268); //预购
                //判断特权卡有没有激活
                bool isActive = Sys_OperationalActivity.Instance.CheckSpecialCardIsActive(2);
                if (isActive)
                    m_TextLeftTime.text = LanguageHelper.GetTextContent(2011269, Sys_Trade.Instance.AdvanceBuyTimes().ToString());
                else
                    m_TextLeftTime.text = LanguageHelper.GetTextContent(2011908);
            }
        }

        private void ShowTime(uint leftTime)
        {
            m_TextLeftTime.text = LanguageHelper.GetTextContent(2011284, LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_1));
        }
    }
}


