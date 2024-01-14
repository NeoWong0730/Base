using Lib.Core;
using Logic.Core;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;

namespace Logic
{
    public class UI_LuckyDraw_Result : UIBase
    {
        private Button m_CloseButton;
        private Button m_AgainButton;
        private Text m_Tips;

        private Button m_LuckyDraw_1;
        private Text m_LuckyDrawCount_1;
        private Image m_LuckyDrawIcon_1;
        private Text m_LuckyDrawText;

        private Toggle m_AutoBuy;

        private InfinityGrid m_InfinityGrid;
        private Dictionary<GameObject, Grid> m_Grids = new Dictionary<GameObject, Grid>();

        private CSVFashionActivity.Data m_CSVFashionActivityData;
        private CSVItem.Data m_CSVItemData_FashionCoin;

        protected override void OnInit()
        {
            m_CSVFashionActivityData = CSVFashionActivity.Instance.GetConfData(Sys_Fashion.Instance.activeId);
            m_CSVItemData_FashionCoin = CSVItem.Instance.GetConfData(m_CSVFashionActivityData.FashionCoin);
        }

        protected override void OnLoaded()
        {
            m_CloseButton = transform.Find("Animator/Btn_Back").GetComponent<Button>();
            m_AgainButton = transform.Find("Animator/Btn_GoOn").GetComponent<Button>();
            m_Tips = transform.Find("Animator/Text_Tips").GetComponent<Text>();
            m_AutoBuy = transform.Find("Animator/Toggle_AutoBuy").GetComponent<Toggle>();
            m_LuckyDraw_1 = transform.Find("Animator/Btn_GoOn").GetComponent<Button>();
            m_LuckyDrawIcon_1 = transform.Find("Animator/Cost/Image_Icon").GetComponent<Image>();
            m_LuckyDrawCount_1 = transform.Find("Animator/Cost/Text_Value").GetComponent<Text>();
            m_LuckyDrawText = transform.Find("Animator/Btn_GoOn/Text_01").GetComponent<Text>();
            m_InfinityGrid = transform.Find("Animator/Scroll View").GetComponent<InfinityGrid>();

            m_InfinityGrid.onCreateCell = OnCreateCell;
            m_InfinityGrid.onCellChange = OnCellChange;

            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            m_AgainButton.onClick.AddListener(OnAgainButtonClicked);

            m_AutoBuy.onValueChanged.AddListener(OnToggleChanged);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnRefreshDrawLuckyResult, OnRefreshDrawLuckyResult, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnExchangeDraw, OnExchangeDraw, toRegister);
        }

        private void OnToggleChanged(bool arg)
        {
            if (arg != Sys_Fashion.Instance.autoBuyDraw)
            {
                Sys_Fashion.Instance.AutoBuyDrawReq(arg ? 1 : 0);
            }
        }

        private void OnRefreshDrawLuckyResult()
        {
            m_Tips.gameObject.SetActive(true);
            UpdateInfo();
        }

        private void OnExchangeDraw()
        {
            UpdateItemCount();
        }

        private void UpdateInfo()
        {
            m_InfinityGrid.CellCount = Sys_Fashion.Instance.dropItems.Count;
            m_InfinityGrid.ForceRefreshActiveCell();

            //tips
            uint count = Sys_Fashion.Instance.dropCurrency.Count;
            //string name = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(Sys_Fashion.Instance.dropCurrency.Id).name_id);
            TextHelper.SetText(m_Tips, LanguageHelper.GetTextContent(590002116, count.ToString()));

            //buttonState
            if (Sys_Fashion.Instance.lastDrawTimes == 1)
            {
                TextHelper.SetText(m_LuckyDrawText, 590002122);
            }
            else if (Sys_Fashion.Instance.lastDrawTimes == 10)
            {
                TextHelper.SetText(m_LuckyDrawText, 590002123);
            }
            UpdateItemCount();
            ImageHelper.SetIcon(m_LuckyDrawIcon_1, m_CSVItemData_FashionCoin.icon_id);

            //toggleState
            m_AutoBuy.isOn = Sys_Fashion.Instance.autoBuyDraw;
        }

        private void UpdateItemCount()
        {
            long fashionPointCount = Sys_Bag.Instance.GetItemCount(m_CSVFashionActivityData.FashionCoin);
            if (Sys_Fashion.Instance.lastDrawTimes == 1)
            {
                if (fashionPointCount < 1)
                {
                    TextHelper.SetText(m_LuckyDrawCount_1, 2007202, 1.ToString());
                }
                else
                {
                    TextHelper.SetText(m_LuckyDrawCount_1, 2011419, 1.ToString());
                }
            }
            else if (Sys_Fashion.Instance.lastDrawTimes == 10)
            {
                if (fashionPointCount < 10)
                {
                    TextHelper.SetText(m_LuckyDrawCount_1, 2007202, 10.ToString());
                }
                else
                {
                    TextHelper.SetText(m_LuckyDrawCount_1, 2011419, 10.ToString());
                }
            }
        }

        private void OnAgainButtonClicked()
        {
            if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(205))
            {
                string content = LanguageHelper.GetTextContent(2009583);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }

            if (Sys_Fashion.Instance.lastDrawTimes == 1)
            {
                long fashionPointCount = Sys_Bag.Instance.GetItemCount(m_CSVFashionActivityData.FashionCoin);
                if (fashionPointCount >= 1)     //是否抽一次
                {
                    //“该操作将消耗{0}个{活动道具}，进行{0}次奖池抽取。”
                    string content = LanguageHelper.GetTextContent(590002100, 1.ToString(),
                        LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id), 1.ToString());
                    PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                    () =>
                    {
                        PlayFx(1);
                    });
                }
                else if (fashionPointCount < 1)  //是否用魔币兑换抽奖道具
                {
                    uint needDiamondCount = m_CSVFashionActivityData.Value;
                    if (!m_AutoBuy.isOn)//没有勾选 就直接弹是否用魔币兑换道具界面
                    {
                        //“您的{活动道具}不足，是否使用{0}魔币购买{1}个{活动道具}”
                        string content = LanguageHelper.GetTextContent(590002101, LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id),
                       needDiamondCount.ToString(), 1.ToString(), LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id));
                        PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                        () =>
                        {
                            if (needDiamondCount > Sys_Bag.Instance.GetItemCount(1))
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002121));//魔币不足
                            }
                            else
                            {
                                Sys_Fashion.Instance.FashionExchangeDrawItemReq(1);//魔币兑换道具 
                            }
                        });
                    }
                    else
                    {
                        //“该操作将消耗{0}个{活动道具}、{1}魔币 进行{2}次奖池抽取。”
                        string content = LanguageHelper.GetTextContent(590002102, 0.ToString(), LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id),
                     needDiamondCount.ToString(), 1.ToString());
                        PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                       () =>
                       {
                           if (needDiamondCount > Sys_Bag.Instance.GetItemCount(1))
                           {
                               Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002121));//魔币不足
                           }
                           else
                           {
                               PlayFx(1);
                           }
                       });
                    }
                }
            }

            else if (Sys_Fashion.Instance.lastDrawTimes == 10)
            {
                long fashionPointCount = Sys_Bag.Instance.GetItemCount(m_CSVFashionActivityData.FashionCoin);
                if (fashionPointCount >= 10)     //是否抽10次
                {
                    //“该操作将消耗{0}个{活动道具}，进行{0}次奖池抽取。”
                    string content = LanguageHelper.GetTextContent(590002100, 10.ToString(),
                        LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id), 10.ToString());
                    PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                    () =>
                    {
                        PlayFx(10);
                    });
                }
                else if (fashionPointCount < 10)  //是否用魔币兑换抽奖道具
                {
                    uint needDiamondCount = (uint)(10 - fashionPointCount) * m_CSVFashionActivityData.Value;
                    if (!m_AutoBuy.isOn)//没有勾选 就直接弹是否用魔币兑换道具界面
                    {
                        //“您的{活动道具}不足，是否使用{0}魔币购买{1}个{活动道具}”
                        string content = LanguageHelper.GetTextContent(590002101, LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id),
                      needDiamondCount.ToString(), (10 - fashionPointCount).ToString(), LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id));
                        PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                        () =>
                        {
                            if (needDiamondCount > Sys_Bag.Instance.GetItemCount(1))
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002121));//魔币不足
                            }
                            else
                            {
                                Sys_Fashion.Instance.FashionExchangeDrawItemReq(10);///魔币兑换道具 
                            }
                        });
                    }
                    else
                    {
                        //“该操作将消耗{0}个{活动道具}、{1}魔币 进行{2}次奖池抽取。”
                        string content = LanguageHelper.GetTextContent(590002102, fashionPointCount.ToString(), LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id),
                     needDiamondCount.ToString(), 10.ToString());
                        PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                       () =>
                       {
                           if (needDiamondCount > Sys_Bag.Instance.GetItemCount(1))
                           {
                               Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002121));//魔币不足
                           }
                           else
                           {
                               PlayFx(10);
                           }
                       });
                    }
                }
            }
        }

        private void PlayFx(uint _drawTime)
        {
            UIManager.CloseUI(EUIID.UI_LuckyDraw_Result);

            Sys_Fashion.Instance.StartLuckyDrawFromRes();
        }


        private void OnCreateCell(InfinityGridCell cell)
        {
            Grid grid = new Grid();
            grid.BindGameObject(cell.mRootTransform.gameObject);

            cell.BindUserData(grid);
            m_Grids.Add(cell.mRootTransform.gameObject, grid);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Grid grid = cell.mUserData as Grid;
            grid.SetData(Sys_Fashion.Instance.dropItems[index]);
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_LuckyDraw_Result);
        }

        public class Grid
        {
            private GameObject m_Go;
            private Item m_Item;

            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;
            }

            public void SetData(Item item)
            {
                m_Item = item;

                PropItem propItem = new PropItem();
                propItem.BindGameObject(m_Go);
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                    (_id: m_Item.Id,
                    _count: m_Item.Count,
                    _bUseQuailty: true,
                    _bBind: false,
                    _bNew: false,
                    _bUnLock: false,
                    _bSelected: false,
                    _bShowCount: true,
                    _bShowBagCount: false,
                    _bUseClick: true,
                    _onClick: null,
                    _bshowBtnNo: false);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_LuckyDraw_Result, showItem));
            }
        }
    }
}


