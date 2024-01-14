using System.Collections;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_Rewards_Result : UIBase
    {
        public class ItemRewardParms
        {
            public List<uint> itemIds = new List<uint>();
            public List<uint> itemCounts = new List<uint>();
        }

        private List<uint> m_ItemIds = new List<uint>();
        private List<uint> m_ItemCounts = new List<uint>();
        private Transform m_InfinityParent;
        private InfinityGrid m_InfinityGrid;
        private Dictionary<GameObject, Grid> ceils = new Dictionary<GameObject, Grid>();
        private Button m_ButtonClose;

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                ItemRewardParms itemRewardParms = arg as ItemRewardParms;
                m_ItemIds = itemRewardParms.itemIds;
                m_ItemCounts = itemRewardParms.itemCounts;
            }
        }


        protected override void OnLoaded()
        {
            m_InfinityParent = transform.Find("Aniamtor/Scroll View");
            m_InfinityGrid = m_InfinityParent.gameObject.GetComponent<InfinityGrid>();
            m_InfinityGrid.onCreateCell += OnCreateCell;
            m_InfinityGrid.onCellChange += OnCellChange;
            m_ButtonClose = transform.Find("Image_BG").GetComponent<Button>();
            m_ButtonClose.onClick.AddListener(OnButtonCloseClicked);
        }

        protected override void OnShow()
        {
            m_InfinityGrid.CellCount = m_ItemIds.Count;
            m_InfinityGrid.ForceRefreshActiveCell();
        }

        private void OnButtonCloseClicked()
        {
            UIManager.CloseUI(EUIID.UI_Rewards_Result);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            Grid entry = new Grid();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(entry);
            ceils.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Grid lifeSkillCeil0 = cell.mUserData as Grid;
            lifeSkillCeil0.SetData(m_ItemIds[index], m_ItemCounts[index]);
        }


        public class Grid
        {
            private GameObject m_Go;
            private PropItem m_PropItem;


            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                m_PropItem = new PropItem();
                m_PropItem.BindGameObject(m_Go);
            }

            public void SetData(uint itemId, uint itemCount)
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
                m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_Rewards_Result, showItem));
            }
        }
    }
}

