using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Panel_Publicity
    {
        private Transform transform;

        private Button m_BtnSearch;

        private UI_Trade_Panel_Publicity_Server m_Server;
        private UI_Trade_Panel_Publicity_Category m_Category;
        private UI_Trade_Panel_Publicity_SubCategory m_ViewSubCategory;
        private UI_Trade_Panel_Publicity_View m_ViewBuy;
        private UI_Trade_Panel_Publicity_WatchItems m_ViewWatchItem;
        
        public void Init(Transform trans)
        {
            transform = trans;

            m_BtnSearch = transform.Find("View_Left/Button").GetComponent<Button>();

            m_Server = new UI_Trade_Panel_Publicity_Server();
            m_Server.Init(transform.Find("Toggles_Left"));

            m_Category = new UI_Trade_Panel_Publicity_Category();
            m_Category.Init(transform.Find("View_Left"));

            m_ViewSubCategory = new UI_Trade_Panel_Publicity_SubCategory();
            m_ViewSubCategory.Init(transform.Find("View_sort"));

            m_ViewBuy = new UI_Trade_Panel_Publicity_View();
            m_ViewBuy.Init(transform.Find("View_Buy"));

            m_ViewWatchItem = new UI_Trade_Panel_Publicity_WatchItems();
            m_ViewWatchItem.Init(transform.Find("View_Collection"));

            m_BtnSearch.onClick.AddListener(OnClickSearch);
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

        public void ProcessEvents(bool register)
        {
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnViewPublicityCategory, OnSelectCategory, register);
            Sys_Trade.Instance.eventEmitter.Handle<uint>(Sys_Trade.EEvents.OnViewPublicitySubCategory, OnSelectSubCategory, register);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnViewPublicityBackToSubCategory, OnBackToSubCategory, register);

            m_Category.ProcessEvents(register);
        }

        public  void OnDestroy()
        {
            ProcessEvents(false);

            m_ViewSubCategory.OnDestroy();
            m_ViewBuy.OnDestroy();
            m_ViewWatchItem.OnDestroy();
        }

        private void OnClickSearch()
        {
            UIManager.OpenUI(EUIID.UI_Trade_Search, false, Sys_Trade.Instance.CurPublicityServerType == Sys_Trade.ServerType.Cross);
        }

        public void UpdateInfo(Sys_Trade.ServerType serverType)
        {
            m_Server.OnSelectServer(serverType);
        }

        private void OnSelectCategory(uint category)
        {
            m_ViewSubCategory.Hide();
            m_ViewBuy.Hide();
            m_ViewWatchItem.Hide();

            if (category == 100u || category == 200u) //关注特殊处理
            {
                //Debug.LogError("关注特殊处理");
                m_ViewWatchItem.Show();

                m_ViewWatchItem.OnReqWatchList();
            }
            else
            {
                m_ViewSubCategory.Show();

                m_ViewSubCategory.UpdateInfo();
            }
        }

        private void OnSelectSubCategory(uint subCategory)
        {
            m_ViewSubCategory.Hide();
            m_ViewBuy.Show();
            m_ViewBuy.UpdateInfo();
        }

        private void OnBackToSubCategory()
        {
            m_ViewBuy.Hide();
            m_ViewSubCategory.Show();
            Sys_Trade.Instance.CurPublicitySubCategory = 0;
        }
    }
}


