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
    public class CrystalTipsData
    {
        public ItemData itemData;
        public bool bShowOp;
        public bool bShowCompare;
        public bool bShowDrop;
        public bool bShowSale;
    }

    public class UI_Tips_ElementalCrystal : UIBase
    {
        private CrystalTipsData m_CrystalTipsData;
        private GameObject m_Tips1Go;
        private GameObject m_Tips2Go;
        private Button m_PutDownButton;
        private Button m_PutOnButton;
        private Button m_SellButton;
        private Button m_DropButton;
        private Button m_SaleButton;
        private GameObject m_ClickClose;
        private Tips m_Tips1;
        private Tips m_Tips2;
        private GameObject m_ButtonRoot;



        protected override void OnOpen(object arg)
        {
            m_CrystalTipsData = arg as CrystalTipsData;
        }

        protected override void OnLoaded()
        {
            ParseTips();
            m_PutDownButton = transform.Find("View_Tips/Button_Root/Button_PutDown").GetComponent<Button>();
            m_PutOnButton = transform.Find("View_Tips/Button_Root/Button_PutOn").GetComponent<Button>();
            m_SellButton = transform.Find("View_Tips/Button_Root/Button_Sell").GetComponent<Button>();
            m_DropButton = transform.Find("View_Tips/Button_Root/Button_Drop").GetComponent<Button>();
            m_SaleButton = transform.Find("View_Tips/Button_Root/Button_OnSale").GetComponent<Button>();
            m_ButtonRoot = transform.Find("View_Tips/Button_Root").gameObject;
            m_ClickClose = transform.Find("Close").gameObject;
            m_PutDownButton.onClick.AddListener(OnPutDownClicked);
            m_PutOnButton.onClick.AddListener(OnPutOnClicked);
            m_SellButton.onClick.AddListener(OnSellClicked);
            m_DropButton.onClick.AddListener(OnDropClicked);
            m_SaleButton.onClick.AddListener(OnSaleClicked);
            Lib.Core.EventTrigger eventTrigger = Lib.Core.EventTrigger.Get(m_ClickClose);
            eventTrigger.AddEventListener(EventTriggerType.PointerClick, OnCloseClicked);
        }

        private void ParseTips()
        {
            m_Tips1Go = transform.Find("View_Tips/Tips01").gameObject;
            m_Tips1 = new Tips();
            m_Tips1.BindGameObject(m_Tips1Go);

            m_Tips2Go = transform.Find("View_Tips/Tips02").gameObject;
            m_Tips2 = new Tips();
            m_Tips2.BindGameObject(m_Tips2Go);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (m_CrystalTipsData.bShowSale)
            {
                m_SaleButton.gameObject.SetActive(m_CrystalTipsData.itemData.cSVItemData.on_sale);
            }
            else
            {
                m_SaleButton.gameObject.SetActive(false);
            }
            if (!m_CrystalTipsData.bShowOp)
            {
                m_ButtonRoot.SetActive(false);
                m_Tips2.SetData(m_CrystalTipsData.itemData);
            }
            else
            {
                m_ButtonRoot.SetActive(true);
                if (m_CrystalTipsData.itemData.BoxId == (uint)BoxIDEnum.BoxIdCrystal)
                {
                    m_PutDownButton.gameObject.SetActive(true);
                    m_PutOnButton.gameObject.SetActive(false);
                }
                else
                {
                    m_PutDownButton.gameObject.SetActive(false);
                    m_PutOnButton.gameObject.SetActive(true);
                }
                m_Tips2.SetData(m_CrystalTipsData.itemData);
            }
            if (!m_CrystalTipsData.bShowCompare)
            {
                m_Tips1Go.SetActive(false);
            }
            else if (Sys_ElementalCrystal.Instance.bEquiped)
            {
                m_Tips1Go.SetActive(true);
                m_Tips1.SetData(Sys_ElementalCrystal.Instance.GetEquipedCrystal());
            }
            else
            {
                m_Tips1Go.SetActive(false);
            }
            m_DropButton.gameObject.SetActive(m_CrystalTipsData.bShowDrop);
            m_SellButton.gameObject.SetActive(m_CrystalTipsData.itemData.cSVItemData.sell_price > 0 && m_CrystalTipsData.bShowCompare);
        }

        private void OnPutDownClicked()
        {
            Sys_Bag.Instance.EquipCrystalReq(0);
            UIManager.CloseUI(EUIID.UI_Tips_ElementalCrystal);
        }

        private void OnPutOnClicked()
        {
            if (m_CrystalTipsData.itemData.marketendTimer.foreverMarket)
            {
                Sys_Bag.Instance.EquipCrystalReq(m_CrystalTipsData.itemData.Uuid);
                UIManager.CloseUI(EUIID.UI_Tips_ElementalCrystal);
            }
            else if (m_CrystalTipsData.itemData.cSVItemData.on_sale)
            {
                PromptBoxParameter.Instance.Clear();
                string content1 = CSVLanguage.Instance.GetConfData(680000513).words;
                PromptBoxParameter.Instance.content = content1;
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Bag.Instance.EquipCrystalReq(m_CrystalTipsData.itemData.Uuid);
                    UIManager.CloseUI(EUIID.UI_Tips_ElementalCrystal);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                Sys_Bag.Instance.EquipCrystalReq(m_CrystalTipsData.itemData.Uuid);
                UIManager.CloseUI(EUIID.UI_Tips_ElementalCrystal);
            }
        }

        private void OnSellClicked()
        {
            Sys_Bag.Instance.SaleItem(m_CrystalTipsData.itemData.Uuid, 1);
            UIManager.CloseUI(EUIID.UI_Tips_ElementalCrystal);
        }

        private void OnDropClicked()
        {
            if (m_CrystalTipsData.itemData.cSVItemData.DoubleCheck == 0)
            {
                Sys_Bag.Instance.OnDisCardItemReq(m_CrystalTipsData.itemData.Uuid);
                UIManager.CloseUI(EUIID.UI_Tips_ElementalCrystal);
            }
            else
            {
                PromptBoxParameter.Instance.Clear();
                string content1 = CSVLanguage.Instance.GetConfData(m_CrystalTipsData.itemData.cSVItemData.DoubleCheckID).words;
                PromptBoxParameter.Instance.content = content1;
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    UIManager.CloseUI(EUIID.UI_Tips_ElementalCrystal);
                    Sys_Bag.Instance.OnDisCardItemReq(m_CrystalTipsData.itemData.Uuid);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        private void OnSaleClicked()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(40201, true))
                return;

            m_CrystalTipsData.itemData.marketendTimer.UpdateReMainMarkendTime();
            if (m_CrystalTipsData.itemData.bBind)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011185));
            }
            else if (m_CrystalTipsData.itemData.marketendTimer.foreverMarket)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011281));
            }
            else if (m_CrystalTipsData.itemData.marketendTimer.remainTime > 0)
            {
                uint left = m_CrystalTipsData.itemData.marketendTimer.remainTime % 86400;
                uint day = m_CrystalTipsData.itemData.marketendTimer.remainTime / 86400;
                if (left != 0)
                    day = day < 1 ? 1 : day + 1;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011150, day.ToString()));
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011150, LanguageHelper.TimeToString(mItemData.marketendTimer.remainTime, LanguageHelper.TimeFormat.Type_4)));
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011150, day.ToString()));
            }
            else
            {
                Sys_Trade.Instance.SaleItem(m_CrystalTipsData.itemData);
                UIManager.CloseUI(EUIID.UI_Tips_ElementalCrystal);
            }
        }

        private void OnCloseClicked(BaseEventData baseEventData)
        {
            UIManager.CloseUI(EUIID.UI_Tips_ElementalCrystal);
        }

        public class Tips
        {
            private GameObject m_Go;
            private PropItem m_PropItem;
            private Text m_Name;
            private Text m_ItemType;
            private Text m_ItemLevel;
            private GameObject m_Equiped;
            private Text m_Durability;
            private ItemData m_ItemData;
            private RawImage m_QualityBg;
            private Transform m_AttrParent;
            private GameObject m_LockGo;
            private Text m_Lockdate;
            private CSVElementAttributes.Data m_CSVElementAttributesData;

            public void BindGameObject(GameObject gameObject)
            {
                this.m_Go = gameObject;

                m_ItemType = m_Go.transform.Find("Background_Root/IconRoot/Text_Type").GetComponent<Text>();
                m_ItemLevel = m_Go.transform.Find("Background_Root/IconRoot/Text_Level").GetComponent<Text>();
                m_LockGo = m_Go.transform.Find("Background_Root/View_Tip/Image_Lock").gameObject;
                m_Durability = m_Go.transform.Find("Background_Root/View_Tip/Image_Score/Durability_Number").GetComponent<Text>();
                m_Lockdate = m_Go.transform.Find("Background_Root/View_Tip/Image_Lock/Text_Lockdate").GetComponent<Text>();
                m_Name = m_Go.transform.Find("Background_Root/IconRoot/Text_Name").GetComponent<Text>();
                m_QualityBg = m_Go.transform.Find("Background_Root/IconRoot/Image_Quality").GetComponent<RawImage>();
                m_Equiped = m_Go.transform.Find("Background_Root/IconRoot/Image_Equip").gameObject;
                m_AttrParent = m_Go.transform.Find("Background_Root/GameObject/View_Basic_Prop");
                m_PropItem = new PropItem();
                m_PropItem.BindGameObject(m_Go.transform.Find("Background_Root/IconRoot/PropItem").gameObject);
            }

            public void SetData(ItemData itemData)
            {
                m_ItemData = itemData;
                m_CSVElementAttributesData = CSVElementAttributes.Instance.GetConfData(m_ItemData.Id);
                Refresh();
            }

            private void Refresh()
            {
                TextHelper.SetQuailtyText(m_Name, (uint)m_ItemData.cSVItemData.quality, CSVLanguage.Instance.GetConfData(m_ItemData.cSVItemData.name_id).words);
                TextHelper.SetText(m_Durability, string.Format("{0}/{1}", m_ItemData.crystal.Durability, m_CSVElementAttributesData.durable));
                m_Equiped.SetActive(m_ItemData.BoxId == (uint)BoxIDEnum.BoxIdCrystal);
                ImageHelper.SetBgQuality(m_QualityBg, (uint)m_ItemData.cSVItemData.quality);
                TextHelper.SetText(m_ItemType, string.Format(CSVLanguage.Instance.GetConfData(2007413).words, CSVLanguage.Instance.GetConfData(m_ItemData.cSVItemData.type_name).words));
                TextHelper.SetText(m_ItemLevel, string.Format(CSVLanguage.Instance.GetConfData(2007412).words, m_ItemData.cSVItemData.lv));
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                                (_id: m_ItemData.Id,
                                _count: 0,
                                _bUseQuailty: true,
                                _bBind: false,
                                _bNew: false,
                                _bUnLock: false,
                                _bSelected: false,
                                _bShowCount: false,
                                _bShowBagCount: false,
                                _bUseClick: false,
                                _onClick: null,
                                _bshowBtnNo: false);
                m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_ElementalCrystal_Exchange, showItem));

                CSVElementAttributes.Data cSVElementAttributesData = CSVElementAttributes.Instance.GetConfData(m_ItemData.Id);
                if (null != cSVElementAttributesData)
                {
                    int needCount = cSVElementAttributesData.elementAttributes.Count;
                    FrameworkTool.CreateChildList(m_AttrParent, needCount, 1);
                    for (int i = 1; i <= needCount; i++)
                    {
                        GameObject gameObject = m_AttrParent.GetChild(i).gameObject;
                        Text attrName = gameObject.transform.Find("Basis_Property/Basis_Property01").GetComponent<Text>();
                        Text attrNum = gameObject.transform.Find("Basis_Property/Number").GetComponent<Text>();
                        uint attrId = cSVElementAttributesData.elementAttributes[i - 1][0];
                        uint attrValue = cSVElementAttributesData.elementAttributes[i - 1][1];
                        TextHelper.SetText(attrName, CSVAttr.Instance.GetConfData(attrId).name);
                        TextHelper.SetText(attrNum, attrValue.ToString());
                    }
                    FrameworkTool.ForceRebuildLayout(m_AttrParent.gameObject);
                }

                UpdateMarketEndTime();
            }

            private void UpdateMarketEndTime()
            {
                if (!m_ItemData.cSVItemData.on_sale)
                {
                    m_LockGo.SetActive(false);
                }
                else
                {
                    if (!m_ItemData.bMarketEnd)
                    {
                        m_LockGo.SetActive(true);
                        if (m_ItemData.MarketendTime == -1)
                        {
                            TextHelper.SetText(m_Lockdate, 1009002);
                        }
                        else
                        {
                            m_Lockdate.text = m_ItemData.marketendTimer.GetMarkendTimeFormat();
                        }
                    }
                    else
                    {
                        m_LockGo.SetActive(false);
                    }
                    FrameworkTool.ForceRebuildLayout(m_LockGo);
                }
            }
        }
    }
}


