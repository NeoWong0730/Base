using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;


namespace Logic
{
    public class UI_FamilyBoss_RankingReward_Right
    {
        public class Cell
        {
            private Transform transform;

            private Text m_textRank;
            private GameObject m_ItemTemplate;

            public void Init(Transform trans)
            {
                transform = trans;

                m_textRank = transform.Find("Image_bg/Text").GetComponent<Text>();
                m_ItemTemplate = transform.Find("Object/PropItem").gameObject;
                m_ItemTemplate.gameObject.SetActive(false);
            }

            public void UpdateInfo(Sys_FamilyBoss.RankRewardShow rewardShow)
            {
                if (rewardShow.arrRank[0] == rewardShow.arrRank[1])
                    m_textRank.text = rewardShow.arrRank[0].ToString();
                else 
                    m_textRank.text = string.Format("{0}-{1}", rewardShow.arrRank[0], rewardShow.arrRank[1]);

                Lib.Core.FrameworkTool.DestroyChildren(m_ItemTemplate.transform.parent.gameObject, m_ItemTemplate.name);
                List<ItemIdCount> list = CSVDrop.Instance.GetDropItem(rewardShow.dropId);
                foreach(var data in list)
                {
                    CSVDrop.Data dropData = CSVDrop.Instance.GetDropItemData(rewardShow.dropId);

                    GameObject go = GameObject.Instantiate(m_ItemTemplate);
                    go.transform.SetParent(m_ItemTemplate.transform.parent);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.gameObject.SetActive(true);

                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(go);

                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(data.id, data.count, true, false, false, false, false, true, false, true);
                    itemData.equipPara = dropData.equip_para;
                    propItem.SetData(itemData, EUIID.UI_FamilyBoss_RankingReward);
                }
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        //private Lib.Core.CoroutineHandler handler;
        //private Dictionary<GameObject, UI_Mall_ShopItem> dicCells = new Dictionary<GameObject, UI_Mall_ShopItem>();
        //private int visualGridCount;
        private List<Sys_FamilyBoss.RankRewardShow> m_ListShow;

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            Cell entry = new Cell();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            //dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Cell entry = cell.mUserData as Cell;
            entry.UpdateInfo(m_ListShow[index]);
        }

        public void OnRewardType(int index)
        {
            m_ListShow = Sys_FamilyBoss.Instance.GetRankRewardShow(index);
            _infinityGrid.CellCount = m_ListShow.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
        }
    }
}