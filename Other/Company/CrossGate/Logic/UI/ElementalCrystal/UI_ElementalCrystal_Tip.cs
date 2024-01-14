using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;
using Packet;

namespace Logic
{
    public class UI_ElementalCrystal_Tip : UIBase
    {
        private Button m_CloseButton;
        private Button m_TradeButton;
        private Button m_ShopButton;
        private Button m_OkButton;
        private Text m_Content;
        private uint m_ItemId;
        private uint m_ShopId;
        private uint m_MallId;
        private uint itemId;
        private uint m_Type;        //0:不足   1：消失

        protected override void OnOpen(object arg)
        {
            Tuple<uint, uint> tuple = arg as Tuple<uint, uint>;
            if (tuple != null)
            {
                itemId = tuple.Item1;
                m_Type = tuple.Item2;
            }
        }

        protected override void OnInit()
        {
            string[] ids = CSVParam.Instance.GetConfData(914).str_value.Split('|');
            m_MallId = uint.Parse(ids[0]);
            m_ShopId = uint.Parse(ids[1]);
            m_ItemId = uint.Parse(ids[2]);
        }

        protected override void OnLoaded()
        {
            m_CloseButton = transform.Find("Animator/View_BG/Btn_Close").GetComponent<Button>();
            m_TradeButton = transform.Find("Animator/Detail/Button03").GetComponent<Button>();
            m_ShopButton = transform.Find("Animator/Detail/Button02").GetComponent<Button>();
            m_OkButton = transform.Find("Animator/Detail/Btn_01").GetComponent<Button>();
            m_Content = transform.Find("Animator/Detail/Text").GetComponent<Text>();
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            m_TradeButton.onClick.AddListener(OnTradeButtonClicked);
            m_ShopButton.onClick.AddListener(OnShopButtonClicked);
            m_OkButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (m_Type == 0)
            {
                TextHelper.SetText(m_Content, LanguageHelper.GetTextContent(680000510, CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(itemId).name_id).words));
            }
            else
            {
                TextHelper.SetText(m_Content, LanguageHelper.GetTextContent(680000511, CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(itemId).name_id).words));
            }
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_ElementalCrystal_Tip);
        }

        private void OnConfirmButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_ElementalCrystal_Tip);
        }

        private void OnExchangeButtonClicked()
        {
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(1154005);
            UIManager.CloseUI(EUIID.UI_ElementalCrystal_Tip);
            if (UIManager.IsOpen(EUIID.UI_Bag))
            {
                UIManager.CloseUI(EUIID.UI_Bag);
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


