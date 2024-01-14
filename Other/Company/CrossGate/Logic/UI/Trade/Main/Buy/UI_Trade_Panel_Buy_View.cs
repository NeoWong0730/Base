using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Trade_Panel_Buy_View : UI_Trade_Panel_Buy_View_Top.IListner, UI_Trade_Panel_Buy_View_Bottom.IListner
    {
        private Transform transform;

        private GameObject m_Empty;
        private UI_Trade_Panel_Buy_View_Top m_Top;
        private UI_Trade_Panel_Buy_View_Item m_ViewItem;
        private UI_Trade_Panel_Buy_View_Bottom m_Bottom;

        private Transform _transPublicityTip;
        private Button _btnPublicityTip;

        private uint m_CurPage;
        private uint m_MaxPage;

        public void Init(Transform trans)
        {
            transform = trans;
  
            m_Empty = transform.Find("Image_BG/Empty").gameObject;

            m_Top = new UI_Trade_Panel_Buy_View_Top();
            m_Top.Init(transform.Find("GameObject/View_Top"));
            m_Top.Register(this);

            m_ViewItem = new UI_Trade_Panel_Buy_View_Item();
            m_ViewItem.Init(transform.Find("GameObject/Rect"));

            m_Bottom = new UI_Trade_Panel_Buy_View_Bottom();
            m_Bottom.Init(transform.Find("GameObject/View_Bottom"));
            m_Bottom.Register(this);
            m_Bottom.EnableRelation(false);

            _transPublicityTip = transform.Find("Text_Tip");
            _transPublicityTip.gameObject.SetActive(false);
            _btnPublicityTip = transform.Find("Text_Tip/Image").GetComponent<Button>();
            _btnPublicityTip.onClick.AddListener(OnClickPublicityTip);
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);

            ProcessEvents(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);

            ProcessEvents(false);
        }

        public void OnDestroy()
        {
            ProcessEvents(false);
            m_ViewItem.OnDestroy();
        }

        public void ProcessEvents(bool register)
        {
            //Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSelectServer, OnSelectServer, register);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnBuyNtf, OnBuyNtf, register);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnTradeListNtf, OnTradeListRes, register);
        }

        private void OnClickPublicityTip()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2011200) });
        }

        public void UpdateInfo()
        {
            m_Top.UpdateInfo();
            m_CurPage = 1u;

            m_ViewItem.Hide();

            OnListReq();

            _transPublicityTip.gameObject.SetActive(Sys_Trade.Instance.CurPageType == Sys_Trade.PageType.Publicity);
        }

        //private void OnSelectServer()
        //{
        //    OnListReq();
        //}

        private void OnBuyNtf()
        {
            OnListReq(m_CurPage);
        }

        public void OnTradeListRes()
        {
            if (Sys_Trade.Instance.CurPageType == Sys_Trade.PageType.Buy)
            {
                m_ViewItem.Show();
                m_ViewItem.UpdateInfo();
     
                m_Empty.SetActive(Sys_Trade.Instance.IsEmptyGood());
            
                Sys_Trade.Instance.GetGoodPage(ref m_CurPage, ref m_MaxPage);
                System.Text.StringBuilder builder = StringBuilderPool.GetTemporary();
                builder.AppendFormat("{0}/{1}", m_CurPage, m_MaxPage);
                m_Bottom.SetPage(StringBuilderPool.ReleaseTemporaryAndToString(builder));
            }
        }

        private void OnListReq(uint curPage = 1u)
        {
            if (Sys_Trade.Instance.SearchParam.isSearch && Sys_Trade.Instance.SearchParam.showType != TradeShowType.Publicity)
            {
                TradeShowType showType = Sys_Trade.Instance.SearchParam.showType;//0在售， 1公示, 2议价
                TradeSearchEquipParam equipParam = Sys_Trade.Instance.SearchParam.equipParam;
                TradeSearchPetParam petParam = Sys_Trade.Instance.SearchParam.petParam;
                TradeSearchPetEquipParam petEquipParam = Sys_Trade.Instance.SearchParam.coreParam;
                TradeSearchOrnamentParam oraParam = Sys_Trade.Instance.SearchParam.ornamentParam;
                TradeSearchType searchType = Sys_Trade.Instance.SearchParam.searchType;
                uint infoId = Sys_Trade.Instance.SearchParam.infoId;
                bool bCross = Sys_Trade.Instance.CurBuyServerType == Sys_Trade.ServerType.Cross;
                ulong goodsUId = Sys_Trade.Instance.SearchParam.goodsUId;

                Sys_Trade.Instance.OnTradeListReq(bCross, (uint)showType, m_Top.IsDownRank, curPage, searchType, infoId, Sys_Trade.Instance.CurBuySubCategory, m_Top.SubClassId, equipParam, petParam, petEquipParam, oraParam, goodsUId);

                //Sys_Trade.Instance.SearchParam.Reset();
            }
            else
            {
                TradeShowType showType = TradeShowType.OnSale; 
                if (m_Top.BarginIndex == 0) //0全部，1非议价，2议价
                    showType = TradeShowType.OnSaleAndDiscuss;
                else if (m_Top.BarginIndex == 1)
                    showType = TradeShowType.OnSale;
                else if (m_Top.BarginIndex == 2)
                    showType = TradeShowType.Discuss;

                bool bCross = Sys_Trade.Instance.CurBuyServerType == Sys_Trade.ServerType.Cross;

                Sys_Trade.Instance.OnTradeListReq(bCross, (uint)showType, m_Top.IsDownRank, curPage, TradeSearchType.Category, 0u, Sys_Trade.Instance.CurBuySubCategory, m_Top.SubClassId);
            }
        }

        #region IListner
        public void OnChangeSubClass()
        {
            if (Sys_Trade.Instance.SearchParam.isSearch && Sys_Trade.Instance.SearchParam.showType != TradeShowType.Publicity)
                Sys_Trade.Instance.SearchParam.Reset();

            //Debug.LogError("OnChangeSubClass");
            OnListReq();
        }

        public void OnChangeBargin()
        {
            //if (Sys_Trade.Instance.SearchParam.isSearch && Sys_Trade.Instance.SearchParam.showType != TradeShowType.Publicity)
            //    Sys_Trade.Instance.SearchParam.Reset();

            OnListReq();
        }

        public void OnChangePriceSort()
        {
            //if (Sys_Trade.Instance.SearchParam.isSearch && Sys_Trade.Instance.SearchParam.showType != TradeShowType.Publicity)
            //    Sys_Trade.Instance.SearchParam.Reset();

            OnListReq();
        }

        public void OnPageLeft()
        {
            uint tempPage = m_CurPage;
            if (tempPage < 2)
            {
                tempPage = 1;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011265));
            }
            else
            {
                tempPage--;
            }

            if (tempPage != m_CurPage)
            {
                m_CurPage = tempPage;
                OnListReq(m_CurPage);
            }
        }

        public void OnPageRight()
        {
            uint tempPage = m_CurPage;
            tempPage++;
            if (tempPage > m_MaxPage)
            {
                tempPage = m_MaxPage;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011266));
            }

            if (tempPage != m_CurPage)
            {
                m_CurPage = tempPage;
                OnListReq(m_CurPage);
            }
        }

        public void OnChangePage(uint num)
        {
            num = num <= 1 ? 1 : num;
            num = num >= m_MaxPage ? m_MaxPage : num;
            m_CurPage = num;
            OnListReq(m_CurPage);
        }
        #endregion
    }
}


