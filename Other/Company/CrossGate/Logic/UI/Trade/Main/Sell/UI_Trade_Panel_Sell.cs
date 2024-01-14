using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Panel_Sell
    {
        private Transform transform;

        private UI_Trade_Panel_Sell_Category m_Category;
        private UI_Trade_Panel_Sell_Item_List m_ItemList;
        private GameObject m_EmptyListGo;
        //private UI_Trade_Panel_Sell_Pet_List m_PetList;
        private UI_Trade_Panel_Sell_Booth m_Booth;


        private Sys_Trade.SellCategory m_SellType;

        public void Init(Transform trans)
        {
            transform = trans;

            m_Category = new UI_Trade_Panel_Sell_Category();
            m_Category.Init(transform.Find("View_Left/Toggles"));

            m_ItemList = new UI_Trade_Panel_Sell_Item_List();
            m_ItemList.Init(transform.Find("View_Left/Scroll_View_Prop"));

            //m_PetList = new UI_Trade_Panel_Sell_Pet_List();
            //m_PetList.Init(transform.Find("View_Left/Scroll_View_Pet"));

            m_EmptyListGo = transform.Find("View_Left/Empty").gameObject;

            m_Booth = new UI_Trade_Panel_Sell_Booth();
            m_Booth.Init(transform.Find("View_Booth"));
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);

            Sys_Trade.Instance.eventEmitter.Handle<Sys_Trade.SellCategory>(Sys_Trade.EEvents.OnSellCategory, OnSelectCategory, true);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSaleBoothUpdateNtf, OnSaleBoothUpdate, true);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSaleSuccessNtf, OnSaleSuccess, true);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnOffSaleSuccessNtf, OnOffSaleSuccess, true);

            m_Category?.OnSelectCategory(Sys_Trade.SellCategory.Item);
            m_Booth?.UpdateInfo();
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);

            Sys_Trade.Instance.eventEmitter.Handle<Sys_Trade.SellCategory>(Sys_Trade.EEvents.OnSellCategory, OnSelectCategory, false);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSaleBoothUpdateNtf, OnSaleBoothUpdate, false);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSaleSuccessNtf, OnSaleSuccess, false);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnOffSaleSuccessNtf, OnOffSaleSuccess, false);
        }

        private void OnSelectCategory(Sys_Trade.SellCategory sellCategory)
        {
            m_SellType = sellCategory;
            CheckSellList(m_SellType);
        }

        private void OnSaleBoothUpdate()
        {
            m_Booth?.UpdateInfo();
        }

        private void OnSaleSuccess()
        {
            CheckSellList(m_SellType);
        }

        private void OnOffSaleSuccess()
        {
            CheckSellList(m_SellType);
        }

        private void CheckSellList(Sys_Trade.SellCategory sellType)
        {
            List<Sys_Trade.TradeItemInfo> tempList = new List<Sys_Trade.TradeItemInfo>();
            if (sellType == Sys_Trade.SellCategory.Item)
                tempList = Sys_Trade.Instance.GetSellItemList();
            else if (sellType == Sys_Trade.SellCategory.Pet)
                tempList = Sys_Trade.Instance.GetSellPetList();

            if (tempList.Count > 0)
            {
                m_ItemList.Show();
                m_ItemList.UpdateList(tempList);
                m_EmptyListGo.SetActive(false);
            }
            else
            {
                m_ItemList.Hide();
                m_EmptyListGo.SetActive(true);
            }
        }
    }
}


