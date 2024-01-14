using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Table;
namespace Logic
{
    public class UI_LottoRewardPreview : UIBase
    {
        private Button m_ButtonClose;
        private InfinityGrid[] m_InfinityGrids;
        private CP_ToggleRegistry m_ToggleRegistry;
        private int currentIndex = -1;
        Dictionary<uint, List<uint>> normalReward = new Dictionary<uint, List<uint>>();
        Dictionary<uint, List<uint>> specialReward = new Dictionary<uint, List<uint>>();
        private GameObject[] gridGos = new GameObject[5];
        List<CSVAward.Data>  m_AwardList;
        private ScrollRect m_ScrollRect;
        private bool isNormal;
        protected override void  OnLoaded()
        {
            ParseConfig();
            m_ButtonClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            m_ButtonClose.onClick.AddListener(OnClickClose);
            

            m_ToggleRegistry = transform.Find("Animator/Tips/Toggles").GetComponent<CP_ToggleRegistry>();
            m_ToggleRegistry.onToggleChange = OnToggleChanged;

            int toggleCount = transform.Find("Animator/Tips/Toggles").childCount;
            for (int i = 0; i < toggleCount; ++i)
            {
                CP_Toggle toggle = transform.Find("Animator/Tips/Toggles/Toggle" + i.ToString()).GetComponent<CP_Toggle>();
                toggle.id = i;
            }

            if (m_InfinityGrids == null)
            {
                m_InfinityGrids = new InfinityGrid[5];
                for (int i = 0; i < m_InfinityGrids.Length; ++i)
                {
                    m_InfinityGrids[i] = transform.Find("Animator/Tips/Rect/Rectlist/Image_line" + i.ToString() + "/Scroll View").GetComponent<InfinityGrid>();
                    m_InfinityGrids[i].onCreateCell = OnCreateCell;
                    m_InfinityGrids[i].onCellChange = OnCellChange;
                    gridGos[i] = m_InfinityGrids[i].transform.parent.gameObject;
                }
            }

            m_ScrollRect = transform.Find("Animator/Tips/Rect").GetComponent<ScrollRect>();
        }

        protected override void OnOpen(object arg)
        {            
            isNormal = ((int)arg) == 1;
        }

        protected override void OnShow()
        {            
            if (isNormal)
                m_ToggleRegistry.SwitchTo(0);
            else
                m_ToggleRegistry.SwitchTo(1);
            Refresh(isNormal);
            
        }

        private void ParseConfig()
        {
            var dataList = CSVAwardPreview.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; ++i)
            {
                var item = dataList[i];
                if (item.pool == 1)
                    normalReward.Add(item.quality, item.awardId);
                else if (item.pool == 2)
                    specialReward.Add(item.quality, item.awardId);
            }
        }
        private void Refresh(bool bNormal)
        {
            isNormal = bNormal;
            if (isNormal)
            {
                for(int i = 0; i < m_InfinityGrids.Length; ++i)
                {
                    List<uint> awardList = null;
                    if(normalReward.TryGetValue((uint)(6 - i), out awardList))
                    {
                        if (!gridGos[i].activeSelf)
                            gridGos[i].SetActive(true);
                        if (awardList != null && awardList.Count > 0)
                        {
                            m_InfinityGrids[i].CellCount = awardList.Count;
                            m_InfinityGrids[i].ForceRefreshActiveCell();
                            m_InfinityGrids[i].MoveToIndex(0);
                        }
                        else
                        {
                            m_InfinityGrids[i].CellCount = 0;
                            m_InfinityGrids[i].ForceRefreshActiveCell();
                        }
                    }
                    else
                    {
                        gridGos[i].SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_InfinityGrids.Length; ++i)
                {
                    List<uint> awardList = null;
                    if (specialReward.TryGetValue((uint)(6 - i), out awardList))
                    {
                        if (!gridGos[i].activeSelf)
                            gridGos[i].SetActive(true);
                        if (awardList != null && awardList.Count > 0)
                        {
                            m_InfinityGrids[i].CellCount = awardList.Count;
                            m_InfinityGrids[i].ForceRefreshActiveCell();
                            m_InfinityGrids[i].MoveToIndex(0);
                        }
                        else
                        {
                            m_InfinityGrids[i].CellCount = 0;
                            m_InfinityGrids[i].ForceRefreshActiveCell();
                        }
                    }
                    else
                    {
                        gridGos[i].SetActive(false);
                    }
                }
            }
        }

        private void OnToggleChanged(int current, int old)
        {
            currentIndex = current;
            Refresh(current == 0);
            m_ScrollRect.verticalNormalizedPosition = 1;
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            Grid item = new Grid();
            switch(cell.mRootTransform.parent.parent.parent.parent.name)
            {
                case "Image_line0":
                    item.row = 0;
                    break;
                case "Image_line1":
                    item.row = 1;
                    break;
                case "Image_line2":
                    item.row = 2;
                    break;
                case "Image_line3":
                    item.row = 3;
                    break;
                case "Image_line4":
                    item.row = 4;
                    break;

            }
            item.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(item);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Grid item = cell.mUserData as Grid;
            uint rewardIndex = (uint)index;
            if (currentIndex == 0)
            {
                var data = CSVAward.Instance.GetConfData(normalReward[6 - item.row][index]);
                if (data != null)
                    item.SetData(data.itemId, data.itemNum,data.isExclusive);
                else
                    DebugUtil.LogError("Can't find AwardData , Id = " + normalReward[6 - item.row][index].ToString());
            }
            else
            {
                var data = CSVAward.Instance.GetConfData(specialReward[6 - item.row][index]);
                if (data != null)
                    item.SetData(data.itemId, data.itemNum,data.isExclusive);
                else
                    DebugUtil.LogError("Can't find AwardData , Id = " + specialReward[6 - item.row][index].ToString());
            }
        }
        public void OnClickClose()
        {
            CloseSelf();
        }

        public class Grid
        {
            public uint row;
            private GameObject go;
            private PropItem propItem;
            private GameObject isExclusiveGo;

            public void BindGameObject(GameObject gameObject)
            {
                go = gameObject;
                isExclusiveGo = go.transform.Find("Image_Zhuanshu").gameObject;
                propItem = new PropItem();
                propItem.BindGameObject(go);
            }

            public void SetData(uint itemId, uint itemCount,bool isExclusive)
            {
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                              (_id: itemId,
                              _count: itemCount,
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
                propItem.SetData(new MessageBoxEvt(EUIID.UI_Rewards_Result, showItem));
                if (isExclusiveGo.activeInHierarchy != isExclusive)
                    isExclusiveGo.SetActive(isExclusive);
            }
        }
    }
}
