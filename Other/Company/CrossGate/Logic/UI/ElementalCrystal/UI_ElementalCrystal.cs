using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;

namespace Logic
{
    public partial class UI_ElementalCrystal : UIBase
    {
        private CP_ToggleRegistry m_CP_ToggleRegistry_Lable;        //主页签切换
        private int m_CurSelectLable;
        private int curSelectLable
        {
            get { return m_CurSelectLable; }
            set
            {
                if (m_CurSelectLable != value)
                {
                    m_CurSelectLable = value;
                }
            }
        }
        private GameObject m_Page1;
        private GameObject m_Page2;
        private Button m_TradeExchange;
        private Button m_ButtonShop;
        private uint m_ItemId;
        private uint m_ShopId;
        private uint m_MallId;

        protected override void OnInit()
        {
            m_CurSelectLable = 0;
            string[] ids = CSVParam.Instance.GetConfData(914).str_value.Split('|');
            m_MallId = uint.Parse(ids[0]);
            m_ShopId = uint.Parse(ids[1]);
            m_ItemId = uint.Parse(ids[2]);
        }

        protected override void OnLoaded()
        {
            m_Page1 = transform.Find("Animator/Page01").gameObject;
            m_Page2 = transform.Find("Animator/Page02").gameObject;
            m_TradeExchange = transform.Find("Animator/Page01/View_Bottom/Button03").GetComponent<Button>();
            m_ButtonShop = transform.Find("Animator/Page01/View_Bottom/Button02").GetComponent<Button>();
            m_CP_ToggleRegistry_Lable = transform.Find("Animator/Toggle_Tab").GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegistry_Lable.onToggleChange = OnLableChanged;
            RegisterLeft();
            RegisterMid();
            RegisterRight();
            m_CloseButton = transform.Find("Animator/RawImage/Btn_Close").GetComponent<Button>();
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            m_TradeExchange.onClick.AddListener(OnTradeButtonClicked);
            m_ButtonShop.onClick.AddListener(OnShopButtonClicked);
        }

        protected override void OnShow()
        {
            m_CP_ToggleRegistry_Lable.SwitchTo(m_CurSelectLable);
        }

        private void OnLableChanged(int curToggle, int old)
        {
            curSelectLable = curToggle;
            if (curSelectLable == 0)
            {
                m_Page1.SetActive(true);
                m_Page2.SetActive(false);

            }
            else
            {
                m_CurSelectMagic = 0;
                m_Page1.SetActive(false);
                m_Page2.SetActive(true);
                ConstructLeftData();
                RefreshLeft();
                ConstructRightData();
                RefreshRight();
                RefreshMid();
                Simulate();
            }
        }

        private void OnExchangeButtonClicked()
        {
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(1154005);
            UIManager.CloseUI(EUIID.UI_ElementalCrystal);
            if (UIManager.IsOpen(EUIID.UI_Bag))
            {
                UIManager.CloseUI(EUIID.UI_Bag);
                Sys_Bag.Instance.useItemReq = false;
            }
            if (UIManager.IsOpen(EUIID.UI_Element))
            {
                UIManager.CloseUI(EUIID.UI_Element);
            }
            if (UIManager.IsOpen(EUIID.UI_Pet_Message))
            {
                UIManager.CloseUI(EUIID.UI_Pet_Message);
            }
            if (UIManager.IsOpen(EUIID.UI_Pet_BookReview))
            {
                UIManager.CloseUI(EUIID.UI_Pet_BookReview);
            }
        }

        private void OnShopButtonClicked()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(40105, true))
            {
                return;
            }
            MallPrama mallPrama = new MallPrama();
            mallPrama.itemId = m_ItemId;
            mallPrama.mallId = m_MallId;
            mallPrama.shopId = m_ShopId;
            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
        }

        private void OnTradeButtonClicked()
        {
            Sys_Trade.Instance.FindCategory(127);
        }
    }
}


