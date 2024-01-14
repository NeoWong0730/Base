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
    public class UI_ElementalCrystal_Exchange : UIBase
    {
        private Transform m_ParentRight;
        private Text m_RightAllotPoint;
        private List<Element> m_ElementsRight = new List<Element>();
        private int[] m_RightDatas = new int[4];
        private int m_AllotRight;
        private List<int> m_RightValues = new List<int>();
        private List<int> m_RightNullValues = new List<int>();
        private Image m_CloseBg;
        private Transform m_PropTrans;
        private List<PropItem> m_PropItems = new List<PropItem>();
        private Button m_ExchangeButton;
        private Button m_InfoButton;
        private Button m_CloseButton;

        protected override void OnLoaded()
        {
            RegisterRight();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_ElementalCrystal.Instance.eventEmitter.Handle(Sys_ElementalCrystal.EEvents.OnRefreshExchangeProp, RefreshPropItems, toRegister);
        }

        protected override void OnShow()
        {
            ConstructRightData();
            RefreshRight();
        }

        private void RegisterRight()
        {
            m_CloseButton = transform.Find("Animator/View_Bg/Btn_Close").GetComponent<Button>();
            m_PropTrans = transform.Find("Animator/View_Right/Content");
            m_ExchangeButton = transform.Find("Animator/View_Right/Btn_01").GetComponent<Button>();
            m_InfoButton = transform.Find("Animator/View_Circle/Image_Title/Button").GetComponent<Button>();
            m_CloseBg = transform.Find("Black").GetComponent<Image>();
            m_ParentRight = transform.Find("Animator/View_Right/Items");
            m_RightAllotPoint = transform.Find("Animator/View_Right/Text").GetComponent<Text>();
            for (int i = 0; i < m_RightDatas.Length; i++)
            {
                GameObject gameObject = m_ParentRight.GetChild(i).gameObject;
                Element element = new Element();
                element.BindGameObject(gameObject);
                element.AddEvent(OnElementAddClicked, OnElementSubClicked);
                element.SetIndex(i);
                m_ElementsRight.Add(element);

                GameObject propGo = m_PropTrans.GetChild(i).gameObject;
                PropItem propItem = new PropItem();
                propItem.BindGameObject(propGo);
                m_PropItems.Add(propItem);
            }
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(m_CloseBg.gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnCloseClicked);
            m_ExchangeButton.onClick.AddListener(OnExchangeButtonClicked);
            m_InfoButton.onClick.AddListener(OnInfoButtonClicked);
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void ConstructRightData()
        {
            for (int i = 0; i < m_RightDatas.Length; i++)
            {
                m_RightDatas[i] = 0;
            }
            for (int i = 0; i < m_RightDatas.Length; i++)
            {
                m_ElementsRight[i].value = m_RightDatas[i];
            }
        }

        private void RefreshRight()
        {
            m_RightValues.Clear();
            m_RightNullValues.Clear();
            m_AllotRight = 10;
            for (int i = 0; i < m_ElementsRight.Count; i++)
            {
                m_AllotRight -= m_ElementsRight[i].value;
                if (m_ElementsRight[i].value > 0)
                {
                    m_RightValues.Add(i);
                }
                else
                {
                    m_RightNullValues.Add(i);
                }
            }
            if (m_RightValues.Count == 2)
            {
                for (int i = 0; i < m_RightNullValues.Count; i++)
                {
                    m_ElementsRight[m_RightNullValues[i]].ShowOrHideAddButton(false);
                    m_ElementsRight[m_RightNullValues[i]].ShowOrHideSubButton(false);
                }
            }
            else if (m_RightValues.Count == 1)
            {
                int index = m_RightValues[0];
                int tempIndex = index;
                if (index == 0)
                {
                    tempIndex = 4;
                }
                int lastIndex = (tempIndex - 1) % 4;
                int nextIndex = (tempIndex + 1) % 4;
                for (int i = 0; i < m_ElementsRight.Count; i++)
                {
                    if (i == index)
                    {
                        continue;
                    }
                    m_ElementsRight[i].ShowOrHideAddButton(i == lastIndex || i == nextIndex);
                }
            }
            else
            {
                for (int i = 0; i < m_ElementsRight.Count; i++)
                {
                    m_ElementsRight[i].ShowOrHideAddButton(true);
                }
            }
            RefreshRightAllot();
            RefreshPropItems();
        }

        private void RefreshPropItems()
        {
            uint itemId = 0;
            for (int i = 0; i < m_PropTrans.childCount; i++)
            {
                if (i == 0)
                {
                    itemId = Sys_ElementalCrystal.LAND_DEBRIS;
                }
                else if (i == 1)
                {
                    itemId = Sys_ElementalCrystal.WATER_DEBRIS;
                }
                else if (i == 2)
                {
                    itemId = Sys_ElementalCrystal.FIRE_DEBRIS;
                }
                else if (i == 3)
                {
                    itemId = Sys_ElementalCrystal.WIND_DEBRIS;
                }
                uint needCount = (uint)m_ElementsRight[i].value * 10;
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                                (_id: itemId,
                                _count: needCount,
                                _bUseQuailty: true,
                                _bBind: false,
                                _bNew: false,
                                _bUnLock: false,
                                _bSelected: false,
                                _bShowCount: true,
                                _bShowBagCount: true,
                                _bUseClick: true,
                                _onClick: null,
                                _bshowBtnNo: true);
                m_PropItems[i].SetData(new MessageBoxEvt(EUIID.UI_ElementalCrystal_Exchange, showItem));
            }
        }

        private void OnElementAddClicked(Element element)
        {
            for (int i = 0; i < m_RightValues.Count; i++)
            {
                if (m_RightValues[i] != element.index)
                {
                    if (m_AllotRight == 0)
                    {
                        m_ElementsRight[m_RightValues[i]].value -= 1;
                    }
                }
            }
            RefreshRight();
        }

        private void OnElementSubClicked(Element element)
        {
            RefreshRight();
        }

        private void RefreshRightAllot()
        {
            TextHelper.SetText(m_RightAllotPoint, m_AllotRight.ToString());
        }

        private void OnExchangeButtonClicked()
        {
            if (m_AllotRight > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000503));
                return;
            }
            List<uint> items = new List<uint>();
            uint itemId = 0;
            uint count = 0;
            bool getFirstCount = false;
            bool enough = true;
            for (int i = 0; i < m_ElementsRight.Count; i++)
            {
                if (m_ElementsRight[i].value > 0)
                {
                    itemId = Sys_ElementalCrystal.crystalBaseDebrisId + (uint)(i + 1);
                    items.Add(itemId);
                    uint needCount = (uint)m_ElementsRight[i].value * 10;
                    uint bagCount = (uint)Sys_Bag.Instance.GetItemCount(itemId);
                    if (needCount > bagCount)
                    {
                        enough = false;
                        break;
                    }
                    if (!getFirstCount)
                    {
                        count = needCount;
                        getFirstCount = true;
                    }
                }
            }
            if (!enough)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000509, 
                    CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(itemId).name_id).words));
                return;
            }
            if (items.Count == 1)
            {
                Sys_Bag.Instance.ExchangeCrystalReq(items[0], count, 0);
            }
            else if (items.Count == 2)
            {
                Sys_Bag.Instance.ExchangeCrystalReq(items[0], count, items[1]);
            }
          
            ConstructRightData();
            RefreshRight();
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_ElementalCrystal_Exchange);
        }

        private void OnInfoButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_ElementalCrystal);
        }

        private void OnCloseClicked(BaseEventData baseEventData)
        {
            UIManager.CloseUI(EUIID.UI_ElementalCrystal_Exchange);
        }
    }
}


