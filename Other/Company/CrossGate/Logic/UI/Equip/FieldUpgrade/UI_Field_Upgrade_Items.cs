using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Field_Upgrade_Items : UIBase
    {
        private class SelectCell
        {
            private Transform transform;
            private Button btn;
            private PropItem prop;
            private Text textName;
            private Text textLevel;

            private ItemData itemPaper;
            private Action<ItemData> action;

            public void Init(Transform trans)
            {
                transform = trans;
                
                btn = transform.GetComponent<Button>();
                btn.onClick.AddListener(OnClick);

                prop = new PropItem();
                prop.BindGameObject(transform.Find("PropItem").gameObject);
                prop.btnNone.image.raycastTarget = false;

                textName = transform.Find("Text_Name").GetComponent<Text>();
                textLevel = transform.Find("Text").GetComponent<Text>();
            }

            private void OnClick()
            {
                action?.Invoke(itemPaper);
            }

            public void AddListener(Action<ItemData> _action)
            {
                action = _action;
            }

            public void UpdateInfo(ItemData item)
            {
                itemPaper = item;

                PropIconLoader.ShowItemData costData = new PropIconLoader.ShowItemData(itemPaper.Id, 1, true, false, false, false, false, 
                    false, true, true, OnClickEquip, false, false);
                costData.SetQuality(itemPaper.Quality);
                costData.SetMarketEnd(itemPaper.bMarketEnd);
                prop.SetData(new MessageBoxEvt( EUIID.UI_EquipSlot_Upgrade_Items, costData));

                textName.text = LanguageHelper.GetTextContent(item.cSVItemData.name_id);
                textLevel.text = "";
            }

            private void OnClickEquip(PropItem item)
            {
                EquipTipsData tipData = new EquipTipsData();
                tipData.equip = itemPaper;
                tipData.isCompare = false;
                tipData.isShowOpBtn = false;
                
                UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
            }
        }
        
        private Button btnAdd;
        //private Transform transScrollView;
        //private InfinityGridLayoutGroup infinity;
        // private Dictionary<GameObject, SelectCell> itemCeilGrids = new Dictionary<GameObject, SelectCell>();
        private Text txtTitle;
        // private int infinityCount;
        private InfinityGrid _infinityGrid;
        private List<ItemData> listItems = new List<ItemData>();
        private uint Slot = 0;

        protected override void OnLoaded()
        {
            Lib.Core.EventTrigger.AddEventListener(transform.Find("Blank (1)").gameObject, EventTriggerType.PointerClick, (eventData) =>
            {
                this.CloseSelf();
            });
            
            btnAdd = transform.Find("Animator/Add").GetComponent<Button>();
            btnAdd.onClick.AddListener(OnClickAdd);
            txtTitle = transform.Find("Animator/Text").GetComponent<Text>();
            //transScrollView = transform.Find("Animator/Scroll_View/Grid");
            _infinityGrid = transform.Find("Animator/Scroll_View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        protected override void OnOpen(object arg)
        {
            Slot = (uint) arg;
        }

        protected override void OnShow()
        {
            SetItemView();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
 
        }
        
        
        protected override void OnClose()
        {
            
        }

        private void OnClickAdd()
        {
            PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);
            iItemData.id = 100401;
            var boxEvt = new MessageBoxEvt(EUIID.UI_EquipSlot_Upgrade_Items, iItemData);

            boxEvt.b_ForceShowScource = true;
            boxEvt.b_ShowItemInfo = false;
            UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
        }
        
        private void OnCreateCell(InfinityGridCell cell)
        {
            SelectCell entry = new SelectCell();
            entry.Init(cell.mRootTransform);
            entry.AddListener(OnItemClick);

            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            SelectCell entry = cell.mUserData as SelectCell;
            //entry.SetIndex(index);
            entry.UpdateInfo(listItems[index]);
        }

        // private void IntItem()
        // {
        //     for (int i = 0; i < transScrollView.childCount; i++)
        //     {
        //         GameObject go = transScrollView.GetChild(i).gameObject;
        //         SelectCell itemCeil = new SelectCell();
        //         itemCeil.Init(go.transform);
        //         itemCeil.AddListener(OnItemClick);
        //         itemCeilGrids.Add(go, itemCeil);
        //     }
        // }

        // private void UpdateChildrenCallback(int index, Transform trans)
        // {
        //     if (index < 0 || index >= infinityCount)
        //         return;
        //     if (itemCeilGrids.ContainsKey(trans.gameObject))
        //     {
        //         SelectCell itemCeil = itemCeilGrids[trans.gameObject];
        //         itemCeil.UpdateInfo(listItems[index]);
        //     }
        // }

        private void SetItemView()
        {
            uint slotIndex = Slot > 1 ? Slot - 1 : Slot;
            uint curLv = Sys_Equip.Instance.GetSlotLevel((int)slotIndex);
            CSVSlotUpgrade.Data curData = Sys_Equip.Instance.GetEquipSlotData((uint)Slot, curLv);
            uint minLv = curData.materia_lev[0];
            uint maxLv = curData.materia_lev[1];
            uint transMin = minLv % 10;
            transMin = transMin == 0 ? minLv / 10 + 1 : transMin;
            uint transMax = maxLv % 10;
            transMax = transMax == 0 ? maxLv / 10 + 1 : transMax;
            txtTitle.text = LanguageHelper.GetTextContent(4236, transMin.ToString(), transMax.ToString());
            
            listItems.Clear();
            List<ItemData>  items = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.Equipment);
            for (int i = 0; i < items.Count; ++i)
            {
                CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(items[i].Id);
                bool isFix = equipData.equipment_level >= minLv && equipData.equipment_level <= maxLv;
                if (isFix && !Sys_Equip.Instance.IsEquiped(items[i]))
                    listItems.Add(items[i]);
            }
            
            _infinityGrid.CellCount = listItems.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }

        public void OnItemClick(ItemData itemData)
        {
            Sys_Equip.Instance.eventEmitter.Trigger<ulong>(Sys_Equip.EEvents.OnNtfFieldUpgradeMat, itemData.Uuid);
            UIManager.CloseUI(EUIID.UI_EquipSlot_Upgrade_Items);
        }

        public void OnGetAwayBtnClicked()
        {
            uint id = 0;
            if (id == 3014)
            {
                CSVPetNewParam.Data cSVPetParameterData = CSVPetNewParam.Instance.GetConfData(35u);
                if (null != cSVPetParameterData)
                {
                    Sys_Trade.Instance.FindCategory(cSVPetParameterData.value);
                    UIManager.CloseUI(EUIID.UI_EquipSlot_Upgrade_Items);
                }
            }
            else
            {
                if (Sys_FamilyResBattle.Instance.InFamilyBattle)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3230000059));
                    return;
                }
                else if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                {
                    Sys_Hint.Instance.PushForbidOprationInFight();  //战斗内提示：当前处于战斗中，无法进行该操作
                    return;
                }

                if (id == 3015)
                {
                    PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

                    iItemData.id = 100400;

                    var boxEvt = new MessageBoxEvt(EUIID.UI_Pet_Message, iItemData);

                    boxEvt.b_ForceShowScource = true;
                    boxEvt.b_ShowItemInfo = false;
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                }
                else if (id == 3010)
                {
                    //uint petNewParamId = 31;
                    PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

                    iItemData.id = 100499;

                    var boxEvt = new MessageBoxEvt(EUIID.UI_Pet_Message, iItemData);

                    boxEvt.b_ForceShowScource = true;
                    boxEvt.b_ShowItemInfo = false;
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                }
                else if (id == 3035)
                {
                    //uint petNewParamId = 31;
                    PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

                    iItemData.id = 100500;

                    var boxEvt = new MessageBoxEvt(EUIID.UI_Pet_Message, iItemData);

                    boxEvt.b_ForceShowScource = true;
                    boxEvt.b_ShowItemInfo = false;
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                }
            }
        }
        
    }
}

