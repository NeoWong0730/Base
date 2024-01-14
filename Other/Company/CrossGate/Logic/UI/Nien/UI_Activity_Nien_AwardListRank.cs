﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Framework;
using System;
using System.Diagnostics.Tracing;
using Lib.Core;

namespace Logic
{
    public class UI_Activity_Nien_AwardListRank
    {
        public class CellData
        {
            public string rank;
            public uint dropId;
        }
        
        public class AwardCell
        {
            private Transform transform;

            private Text txtRank;
            private Transform itemTemplate;
            
            public void Init(Transform trans)
            {
                transform = trans;
                txtRank = transform.Find("Text").GetComponent<Text>();
                itemTemplate = transform.Find("Scroll_View/Viewport/Item");
                itemTemplate.gameObject.SetActive(false);
            }

            public void SetData(CellData cell)
            {
                txtRank.text = LanguageHelper.GetTextContent(2016303, cell.rank);
                
                Lib.Core.FrameworkTool.DestroyChildren(itemTemplate.parent.gameObject, itemTemplate.name);
                List<ItemIdCount> list = CSVDrop.Instance.GetDropItem(cell.dropId);
                foreach(var data in list)
                {
                    CSVDrop.Data dropData = CSVDrop.Instance.GetDropItemData(cell.dropId);

                    GameObject go = GameObject.Instantiate(itemTemplate.gameObject);
                    go.transform.SetParent(itemTemplate.parent);
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
        private List<CellData> listDatas = new List<CellData>();
        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnChangeCell;
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
            AwardCell award = new AwardCell();
            award.Init(cell.mRootTransform);
            cell.BindUserData(award);
        }

        private void OnChangeCell(InfinityGridCell cell, int index)
        {
            AwardCell award = cell.mUserData as AwardCell;
            award.SetData(listDatas[index]);
        }

        public void UpdateInfo(uint activityId)
        {
            listDatas.Clear();
            int count = CSVNienPresonalRank.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                CSVNienPresonalRank.Data curData = CSVNienPresonalRank.Instance.GetByIndex(i);
                if (curData.Activity_Id == activityId)
                {
                    CSVNienPresonalRank.Data preData = curData.Presonal_Rank != 1 ? CSVNienPresonalRank.Instance.GetByIndex(i - 1): null;

                    CellData data = new CellData();
                    if (preData != null)
                    {
                        uint diff = curData.Presonal_Rank - preData.Presonal_Rank;
                        if (diff > 1)
                        {
                            data.rank = string.Format("{0}-{1}", preData.Presonal_Rank + 1, curData.Presonal_Rank);
                        }
                        else
                        {
                            data.rank = string.Format("{0}",  curData.Presonal_Rank);
                        }

                        data.dropId = curData.Reward;
                    }
                    else
                    {
                        data.rank = string.Format("{0}",  curData.Presonal_Rank);
                        data.dropId = curData.Reward;
                    }
                    
                    listDatas.Add(data);
                }
            }

            _infinityGrid.CellCount = listDatas.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


