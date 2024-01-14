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
    public class UI_Fashion_Point : UIBase
    {
        private Button m_CloseButton;
        private Button m_ImageClose;
        private InfinityGrid m_InfinityGrid;
        private Dictionary<GameObject, PointGrid> m_Grids = new Dictionary<GameObject, PointGrid>();
        //private List<uint> m_Points = new List<uint>();

        protected override void OnInit()
        {
            //for (int i = 0; i < CSVFashionScroe.Instance.Count; i++)
            //{
            //    m_Points.Add(CSVFashionScroe.Instance[i].id);
            //}
        }

        protected override void OnLoaded()
        {
            m_CloseButton = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            m_ImageClose = transform.Find("Close").GetComponent<Button>();
            m_InfinityGrid = transform.Find("Animator/Scroll View").GetComponent<InfinityGrid>();

            m_InfinityGrid.onCreateCell = OnCreateCell_Collect;
            m_InfinityGrid.onCellChange = OnCellChange_Collect;
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            m_ImageClose.onClick.AddListener(OnImageCloseClicked);
        }

        protected override void OnShow()
        {
            //m_InfinityGrid.CellCount = m_Points.Count;
            m_InfinityGrid.CellCount = CSVFashionScroe.Instance.Count;
            m_InfinityGrid.ForceRefreshActiveCell();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnGetFashionReward, OnGetFashionReward, toRegister);
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Fashion_Point);
        }

        private void OnImageCloseClicked()
        {
            UIManager.CloseUI(EUIID.UI_Fashion_Point);
        }

        private void OnGetFashionReward(uint pointId)
        {
            foreach (var item in m_Grids)
            {
                PointGrid pointGrid = item.Value;
                if (pointGrid.PointId == pointId)
                {
                    pointGrid.OnGetFashionReward();
                    break;
                }
            }
        }

        private void OnCreateCell_Collect(InfinityGridCell cell)
        {
            PointGrid pointGrid = new PointGrid();
            pointGrid.BindGameObject(cell.mRootTransform.gameObject);

            cell.BindUserData(pointGrid);
            m_Grids.Add(cell.mRootTransform.gameObject, pointGrid);
        }

        private void OnCellChange_Collect(InfinityGridCell cell, int index)
        {
            PointGrid consignGrid = cell.mUserData as PointGrid;
            consignGrid.SetData(CSVFashionScroe.Instance.GetByIndex(index));
        }

        public class PointGrid
        {
            private GameObject m_Go;
            private Text m_Point;
            private Button m_StateButton;
            private GameObject m_GetGo;
            private Text m_ButtonText;
            private Transform m_ItemParent;
            private Dictionary<GameObject, PropItem> m_PropItems = new Dictionary<GameObject, PropItem>();
            private CSVFashionScroe.Data m_CSVFashionScroeData;
            public uint PointId
            {
                get;
                private set;
            }

            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                m_Point = m_Go.transform.Find("Text_Point/Text_Value").GetComponent<Text>();
                m_StateButton = m_Go.transform.Find("State/Btn_01").GetComponent<Button>();
                m_GetGo = m_Go.transform.Find("State/Image").gameObject;
                m_ButtonText = m_Go.transform.Find("State/Btn_01/Text_01").GetComponent<Text>();
                m_ItemParent = m_Go.transform.Find("Scroll View/Viewport/Content");

                m_StateButton.onClick.AddListener(OnStateButtonClicked);
            }

            public void SetData(CSVFashionScroe.Data data)//(uint pointId)
            {
                //PointId = pointId;
                //m_CSVFashionScroeData = CSVFashionScroe.Instance.GetConfData(PointId);
                
                m_CSVFashionScroeData = data;
                PointId = m_CSVFashionScroeData.id;

                Refresh();
            }

            public void OnGetFashionReward()
            {
                m_GetGo.SetActive(true);
                m_StateButton.gameObject.SetActive(false);
            }

            private void Refresh()
            {
                TextHelper.SetText(m_Point, m_CSVFashionScroeData.score.ToString());
                if (Sys_Fashion.Instance.rewardsGet.IndexOf(PointId) > -1)
                {
                    //TextHelper.SetText(m_ButtonText, LanguageHelper.GetTextContent(590002202));
                    //已领取
                    m_GetGo.SetActive(true);
                    m_StateButton.gameObject.SetActive(false);
                    //ButtonHelper.Enable(m_StateButton, false);
                }
                else if (Sys_Fashion.Instance.fashionPoint >= m_CSVFashionScroeData.score)
                {
                    TextHelper.SetText(m_ButtonText, LanguageHelper.GetTextContent(4701));
                    //可领取   领取 亮
                    ButtonHelper.Enable(m_StateButton, true);
                    m_GetGo.SetActive(false);
                }
                else
                {
                    TextHelper.SetText(m_ButtonText, LanguageHelper.GetTextContent(4701));
                    m_GetGo.SetActive(false);
                    //未完成   领取 置灰
                    ButtonHelper.Enable(m_StateButton, false);  
                }

                List<ItemIdCount> itemIdCounts = CSVDrop.Instance.GetDropItem(m_CSVFashionScroeData.reward);


                FrameworkTool.CreateChildList(m_ItemParent, itemIdCounts.Count);
                for (int i = 0; i < itemIdCounts.Count; i++)
                {
                    GameObject gameObject = m_ItemParent.GetChild(i).gameObject;
                    if (!m_PropItems.TryGetValue(gameObject, out PropItem propItem))
                    {
                        propItem = new PropItem();
                        m_PropItems.Add(gameObject, propItem);
                    }
                    propItem.BindGameObject(gameObject);
                    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                        (_id: itemIdCounts[i].id,
                        _count: itemIdCounts[i].count,
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
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_Fashion_Point, showItem));
                }
            }

            private void OnStateButtonClicked()
            {
                //领取
                Sys_Fashion.Instance.FashionGetTotalValueAwardReq(PointId);
            }
        }
    }
}


