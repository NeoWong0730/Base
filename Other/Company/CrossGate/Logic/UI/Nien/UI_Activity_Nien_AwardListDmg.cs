using System.Collections;
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
    public class UI_Activity_Nien_AwardListDmg
    {

        public class CellData
        {
            public uint dmg;
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
                txtRank.text = LanguageHelper.GetTextContent(2016305, cell.dmg.ToString());
                
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
            int count = CSVNienDamageAccumulate.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                CSVNienDamageAccumulate.Data curData = CSVNienDamageAccumulate.Instance.GetByIndex(i);
                if (curData.Activity_Id == activityId)
                {
                    CellData data = new CellData();
                    data.dmg = curData.Damage;
                    data.dropId = curData.Reward;

                    listDatas.Add(data);
                }
            }

            _infinityGrid.CellCount = listDatas.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


