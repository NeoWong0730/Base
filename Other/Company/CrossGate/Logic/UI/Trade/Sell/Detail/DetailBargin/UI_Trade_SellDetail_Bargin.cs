using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_SellDetail_Bargin : UIBase
    {
        private Button m_BtnClose;
       
        private PropItem m_PropItem;
        private Text m_Name;
        private Text m_Des;

        private Transform m_TransOutOfDate;
        private Transform m_TransTime;
        private Text m_TextTime;

        private Image m_ImgWatch;
        private Text m_TextWatch;

        private Button m_OffSale;

        private TradeBrief m_Brief;
        private CSVCommodity.Data m_GoodData;
        

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
            m_BtnClose = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(() => { this.CloseSelf(); });

            m_PropItem = new PropItem();
            m_PropItem.BindGameObject(transform.Find("Animator/Detail/Image_BG/PropItem").gameObject);
            m_Name = transform.Find("Animator/Detail/Image_BG/Text_Name").GetComponent<Text>();
            m_Des = transform.Find("Animator/Detail/Image_BG/Text_Dex").GetComponent<Text>();

            m_TransOutOfDate = transform.Find("Animator/Detail/Time/Label_Overdue");
            m_TransTime = transform.Find("Animator/Detail/Time/Intime");
            m_TextTime = transform.Find("Animator/Detail/Time/Intime/Label_Time").GetComponent<Text>();

            m_ImgWatch = transform.Find("Animator/Detail/Image_BG/Image_heart/Image").GetComponent<Image>();
            m_TextWatch = transform.Find("Animator/Detail/Image_BG/Text_Num").GetComponent<Text>();

            m_OffSale = transform.Find("Animator/Detail/Btn_01").GetComponent<Button>();
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

        private void UpdateInfo()
        {
            if (m_GoodData != null)
            {
                CSVItem.Data item = CSVItem.Instance.GetConfData(m_GoodData.id);

                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData(item.id, m_Brief.Count, true, false, false, false, false, false, false, true);
                showItem.SetQuality(m_Brief.Color);
                showItem.SetMarketEnd(true);
                m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_Trade_Sell, showItem));

                m_Name.text = LanguageHelper.GetTextContent(item.name_id);
                //m_Lv.text = item.lv.ToString();
                m_Des.text = LanguageHelper.GetTextContent(item.describe_id);
            }

            //关注
            if (m_GoodData.attention)
            {
                m_ImgWatch.fillAmount = m_Brief.WatchTimes / 10f;
                m_TextWatch.gameObject.SetActive(true);
                m_TextWatch.text = m_Brief.WatchTimes.ToString();
            }
            else
            {
                m_TextWatch.gameObject.SetActive(false);
                m_ImgWatch.fillAmount = 0f;
            }


            //剩余时间
            bool isPublicity = Sys_Trade.Instance.IsPublicity(m_Brief);
            bool isOutOfDate = Sys_Trade.Instance.IsOutOfDate(m_Brief);

            m_TransOutOfDate.gameObject.SetActive(isOutOfDate);
            m_TransTime.gameObject.SetActive(!isOutOfDate);
            if (!isOutOfDate)
            {
                if (isPublicity)
                {
                    //uint saleTime = Sys_Trade.Instance.SaleTime(m_GoodData.treasure == 1);
                    uint endTime = m_Brief.OnsaleTime;
                    m_TextTime.text = LanguageHelper.TimeToString(endTime - Framework.TimeManager.GetServerTime(), LanguageHelper.TimeFormat.Type_4);
                }
                else
                {
                    uint saleTime = Sys_Trade.Instance.SaleTime(m_GoodData.treasure);
                    saleTime *= 86400u;
                    uint endTime = saleTime + m_Brief.OnsaleTime;
                    m_TextTime.text = LanguageHelper.TimeToString(endTime - Framework.TimeManager.GetServerTime(), LanguageHelper.TimeFormat.Type_4);
                }
            }
        }

        private void OnOffSale()
        {
            Sys_Trade.Instance.OnOffSaleReq(m_Brief.GoodsUid);
        }
    }
}


