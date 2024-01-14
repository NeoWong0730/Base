using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;

namespace Logic
{
    public class EquipRemakeParam
    {
        public uint rebuildId;
        public ItemData paramEquip;
    }
    
    public class UI_Equipment_ReMake : UIParseCommon
    {

        public class EquipIconRoot
        {
            private Transform transform;
            private CP_Toggle toggle;

            private EquipItem uiEquip;
            private Text txtName;
            private Text txtLev;
            
            private Action<uint> _action;
            private uint rebuildId;
            private ItemData paramEquip;
            public void Init(Transform trans)
            {
                transform = trans;
                toggle = transform.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
                
                uiEquip = new EquipItem();
                uiEquip.Bind(transform.Find("EquipItem").gameObject);
                uiEquip.Layout.btnItem.onClick.AddListener(OnClickEquip);
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                txtLev = transform.Find("Text_LevelNum").GetComponent<Text>();
            }

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    _action?.Invoke(rebuildId);
                }
            }

            private void OnClickEquip()
            {
                EquipRemakeParam param = new EquipRemakeParam(){rebuildId = this.rebuildId, paramEquip = this.paramEquip};
                UIManager.OpenUI(EUIID.UI_Equipment_Remake_Preview, false, param);
            }

            public void Register(Action<uint> action)
            {
                _action = action;
            }

            public void UpdateInfo(uint id, uint color)
            {
                rebuildId = id;
                CSVEquipmentRebuild.Data data = CSVEquipmentRebuild.Instance.GetConfData(rebuildId);
                CSVEquipment.Data equip = CSVEquipment.Instance.GetConfData(data.aim_type_id);
                CSVItem.Data item = CSVItem.Instance.GetConfData(data.aim_type_id);
                if (item != null)
                {
                    txtName.text = LanguageHelper.GetTextContent(item.name_id);
                    txtLev.text = LanguageHelper.GetTextContent(1000002, equip.TransLevel().ToString());
                
                    paramEquip = new ItemData(0, 0, data.aim_type_id, (uint)1, 0, false, false, null, null, 0); 
                    paramEquip.SetQuality(color);
                    uiEquip.SetData(paramEquip);
                }
            }

            public void OnSelect(bool isSelect)
            {
                toggle.SetSelected(isSelect, true);
            }
        }

        private InfinityGrid _infinityGrid;
        private PropItem propCost;
        private Button btnReMake;
        
        private List<uint> listRemakeIds = new List<uint>();
        private int selectIndex = 0;
        private ulong uId;
        private uint toInfoId;

        private uint color;
        //private uint costId;

        protected override void Parse()
        {
            base.Parse();

            _infinityGrid = transform.Find("View_Right/Scroll_Equip (1)").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            propCost = new PropItem(); 
            propCost.BindGameObject(transform.Find("View_Right/Cost/Grid/PropItem").gameObject);

            btnReMake = transform.Find("View_Right/Cost/Button").GetComponent<Button>();
            btnReMake.onClick.AddListener(OnClickRemake);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void OnDestroy()
        {
            
        }
        
        private void OnCreateCell(InfinityGridCell cell)
        {
            EquipIconRoot entry = new EquipIconRoot();
            entry.Init(cell.mRootTransform);
            entry.OnSelect(false);
            entry.Register(OnSelectRebuidId);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            EquipIconRoot entry = cell.mUserData as EquipIconRoot;
            entry.UpdateInfo(listRemakeIds[index], color);
            entry.OnSelect(index == selectIndex);
        }

        private void OnClickRemake()
        {
            Sys_Equip.Instance.OnRebuildEquipReq(uId, toInfoId);
        }

        private void OnSelectRebuidId(uint rebuildId)
        {
            int index = listRemakeIds.IndexOf(rebuildId);
            if (index != selectIndex)
                selectIndex = index;
            CSVEquipmentRebuild.Data temp = CSVEquipmentRebuild.Instance.GetConfData(rebuildId);
            toInfoId = temp.aim_type_id;
        }

        public override void UpdateInfo(ItemData item)
        {
            uId = item.Uuid;
            color = item.Equip.Color;
            listRemakeIds.Clear();
            int count = CSVEquipmentRebuild.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                CSVEquipmentRebuild.Data data = CSVEquipmentRebuild.Instance.GetByIndex(i);
                if (data.type_id == item.Id)
                {
                    listRemakeIds.Add(data.id);
                }
            }

            selectIndex = 0;
            _infinityGrid.CellCount = listRemakeIds.Count;
            _infinityGrid.ForceRefreshActiveCell();
            
            //cost
            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(item.Id);
            if (equipData != null)
            {
                uint expendId = Sys_Equip.Instance.GetEquipRebuildExpendId(equipData.equipment_level, equipData.equipment_type, (uint)item.Equip.Score);
                CSVRebuildExpend.Data temp = CSVRebuildExpend.Instance.GetConfData(expendId);
                uint costId = temp.reborn_cost[0];
                uint costNum = temp.reborn_cost[1];
              
                PropIconLoader.ShowItemData showitem = new PropIconLoader.ShowItemData(costId, costNum, true, false, false, false, false, true, true, false);
                propCost.SetData(showitem, EUIID.UI_Equipment);
            }
        }
    }
}


