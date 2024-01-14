using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Trade_Record : UIBase
    {
        private Button m_BtnClose;

        private UI_Trade_Record_Type m_Type;
        private UI_Trade_Record_Treasure_Type m_TreasureType;
        private UI_Trade_Record_List_Buy m_BuyList;
        private UI_Trade_Record_List_Sale m_SaleList;

        private Text m_TextName;
        private Text m_TextPrice;
        private Text m_TextTip;


        private uint m_RecordType = 0u;
        private uint m_RecordTreasureType = 0;
        private bool isFirst = false; //避免初始页签切换，执行两遍逻辑更新.页签独立

        protected override void OnOpen(object arg)
        {
           
        }

        protected override void OnLoaded()
        {
            //m_Currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

            m_BtnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(() =>{ this.CloseSelf(); });

            m_Type = new UI_Trade_Record_Type();
            m_Type.Init(transform.Find("Animator/View_Detail/Toggles"));

            m_TreasureType = new UI_Trade_Record_Treasure_Type();
            m_TreasureType.Init(transform.Find("Animator/View_Detail/Toggles2"));

            m_BuyList = new UI_Trade_Record_List_Buy();
            m_BuyList.Init(transform.Find("Animator/View_Detail/Scroll View0"));

            m_SaleList = new UI_Trade_Record_List_Sale();
            m_SaleList.Init(transform.Find("Animator/View_Detail/Scroll View1"));

            m_TextName = transform.Find("Animator/View_Detail/title/Text_Name").GetComponent<Text>();
            m_TextPrice = transform.Find("Animator/View_Detail/title/Text_Price").GetComponent<Text>();

            m_TextTip = transform.Find("Animator/View_Detail/Text_Explain").GetComponent<Text>();
            m_TextTip.text = LanguageHelper.GetTextContent(2011117, Sys_Trade.Instance.GetTradeRecordCount().ToString());
        }
        //protected virtual void OnUpdate() { }
        //protected virtual void OnLateUpdate() { }

        protected override void OnShow()
        {
            UpdateInfo();
        }
        
        protected override void OnDestroy()
        {
            m_BuyList?.Hide();
            m_SaleList?.Hide();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnRecordType, OnRecordType, toRegister);
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnRecordTreasureType, OnRecordTreasureType, toRegister);
        }

        private void UpdateInfo()
        {
            isFirst = false;
            m_RecordType = 0u;
            m_RecordTreasureType = 0;
            m_Type.OnSelect(m_RecordType);
            m_TreasureType.OnSelect(m_RecordTreasureType);
        }

        private void OnRecordType(uint recordType)
        {
            m_RecordType = recordType;
            m_BuyList.Hide();
            m_SaleList.Hide();

            if (recordType == 0u) //出售
            {
                m_TextName.text = LanguageHelper.GetTextContent(2011118);
                m_TextPrice.text = LanguageHelper.GetTextContent(2011119);

                m_SaleList.Show();
                if (m_RecordTreasureType == 0)
                    m_SaleList.OnUpdateListInfos(Sys_Trade.Instance.GetSaleRecordTreasure());
                else
                    m_SaleList.OnUpdateListInfos(Sys_Trade.Instance.GetSaleRecordNormal());
            }
            else if (recordType == 1u) //购买
            {
                m_TextName.text = LanguageHelper.GetTextContent(2011215);
                m_TextPrice.text = LanguageHelper.GetTextContent(2011216);

                m_BuyList.Show();
                if (m_RecordTreasureType == 0)
                    m_BuyList.OnUpdateListInfos(Sys_Trade.Instance.GetBuyRecordTreasure());
                else
                    m_BuyList.OnUpdateListInfos(Sys_Trade.Instance.GetBuyRecordNormal());
            }
        }

        private void OnRecordTreasureType(uint treasureType)
        {
            if (!isFirst)
            {
                isFirst = true;
                return;
            }

            m_RecordTreasureType = treasureType;
            if (m_RecordType == 0u) //出售
            {
                if (m_RecordTreasureType == 0)
                    m_SaleList.OnUpdateListInfos(Sys_Trade.Instance.GetSaleRecordTreasure());
                else
                    m_SaleList.OnUpdateListInfos(Sys_Trade.Instance.GetSaleRecordNormal());
            }
            else if (m_RecordType == 1u) //购买
            {
                if (m_RecordTreasureType == 0)
                    m_BuyList.OnUpdateListInfos(Sys_Trade.Instance.GetBuyRecordTreasure());
                else
                    m_BuyList.OnUpdateListInfos(Sys_Trade.Instance.GetBuyRecordNormal());
            }
        }
    }
}


