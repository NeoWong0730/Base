using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lib.Core;

namespace Logic
{
    public class UI_Equipment_Left : UIParseCommon
    {
        //private ScrollRect _scrollRect;
        //private ToggleGroup toggleGroup;
        //private InfinityGridLayoutGroup gridGroup;
        //private GridLayoutGroup layoutGroup;
        private Dictionary<GameObject, UI_EquipIconRoot> dicEquipments = new Dictionary<GameObject, UI_EquipIconRoot>();
        //private int visualGridCount;
        private InfinityGrid _infinityGrid;

        private Lib.Core.CoroutineHandler handler;

        private IListener listener;

        protected override void Parse()
        {
            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
            //_scrollRect = transform.GetComponent<ScrollRect>();
            //toggleGroup = transform.Find("Viewport/Content").gameObject.GetComponent<ToggleGroup>();
            //gridGroup = transform.Find("Viewport/Content").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            //gridGroup.minAmount = 7;
            //gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            //layoutGroup = gridGroup.GetComponent<GridLayoutGroup>();

            //for (int i = 0; i < gridGroup.transform.childCount; ++i)
            //{
            //    GameObject go = gridGroup.transform.GetChild(i).gameObject;

            //    UI_EquipIconRoot iconRoot = new UI_EquipIconRoot();
            //    iconRoot.Init(go.transform);
            //    iconRoot.AddListener(OnSelectEquipment);
            //    dicEquipments.Add(go, iconRoot);
            //}

            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfInlay, OnNtfInlay, true);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfUnload, OnNtfUnload, true);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfQuickCompose, OnJewelNtfQuickCompose, true);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifySmelt, OnNtfSmelt, true);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifyRevertSmelt, OnNtfSmelt, true);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifyMakeUse, OnNtfMakeUse, true);
            Sys_Equip.Instance.eventEmitter.Handle<bool>(Sys_Equip.EEvents.OnNotifyRepair, OnNtfRepair, true);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNtfRefreshEffect, OnNtfRefreshEffect, true);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void OnDestroy()
        {
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfInlay, OnNtfInlay, false);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfUnload, OnNtfUnload, false);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfQuickCompose, OnJewelNtfQuickCompose, false);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifySmelt, OnNtfSmelt, false);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifyRevertSmelt, OnNtfSmelt, false);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifyMakeUse, OnNtfMakeUse, false);
            Sys_Equip.Instance.eventEmitter.Handle<bool>(Sys_Equip.EEvents.OnNotifyRepair, OnNtfRepair, false);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNtfRefreshEffect, OnNtfRefreshEffect, false);
        }

        //private void UpdateChildrenCallback(int index, Transform trans)
        //{
        //    if (index < 0 || index >= visualGridCount)
        //        return;

        //    if (dicEquipments.ContainsKey(trans.gameObject))
        //    {
        //        UI_EquipIconRoot iconRoot = dicEquipments[trans.gameObject];
        //        iconRoot.UpdateEquipInfo(Sys_Equip.Instance.EquipListUIds[index]);
        //    }
        //}

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_EquipIconRoot entry = new UI_EquipIconRoot();
            entry.Init(cell.mRootTransform);
            entry.AddListener(OnSelectEquipment);

            cell.BindUserData(entry);

            dicEquipments.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_EquipIconRoot entry = cell.mUserData as UI_EquipIconRoot;
            entry.UpdateEquipInfo(Sys_Equip.Instance.EquipListUIds[index]);
            //entry.SetData(hornType == 0 ? Sys_Chat.Instance.mSingleServerHornDatas[index] : Sys_Chat.Instance.mFullServerHornDatas[index]);
        }

        public void CalEquipList(Sys_Equip.EquipmentOperations opType, ItemData item)
        {
            //cal sort equipments
            Sys_Equip.Instance.SortEquipments(opType);

            //设置选中的装备
            if (item != null)
            {
                Sys_Equip.Instance.CurOpEquipUId = item.Uuid;

                if (Sys_Equip.Instance.EquipListUIds.Count == 0)
                {
                    //Sys_Equip.Instance.CurOpEquip = null;
                    OnSelectEquipment(0);
                }
            }
            else
            {
                if (Sys_Equip.Instance.EquipListUIds.Count > 0)
                {
                    Sys_Equip.Instance.CurOpEquipUId = Sys_Equip.Instance.EquipListUIds[0];
                }
                else
                {
                    Sys_Equip.Instance.CurOpEquipUId = 0L;
                    OnSelectEquipment(0);
                }
            }

            //_infinityGrid.Clear();
            _infinityGrid.CellCount = Sys_Equip.Instance.EquipListUIds.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
            //visualGridCount = Sys_Equip.Instance.EquipListUIds.Count;
            //gridGroup.SetAmount(visualGridCount);


            if (handler != null)
            {
                CoroutineManager.Instance.Stop(handler);
                handler = null;
            }

            handler = CoroutineManager.Instance.StartHandler(CheckNeedScroll());
            //CheckNeedScroll();
        }

        private IEnumerator CheckNeedScroll()
        {
            yield return new WaitForSeconds(0.3f);

            if (Sys_Equip.Instance.CurOpEquipUId != 0L)
            {
                int cellIndex = 0;
                for (int i = 0; i < Sys_Equip.Instance.EquipListUIds.Count; ++i)
                {
                    if (Sys_Equip.Instance.EquipListUIds[i] == Sys_Equip.Instance.CurOpEquipUId)
                    {
                        cellIndex = i;
                        break;
                    }
                        
                }

                if (cellIndex > 4)
                {
                    _infinityGrid.MoveToIndex(cellIndex);
                    //Debug.LogError(cellIndex);
                    //float time = (cellIndex / 5 * 0.2f);
                    //time = time > 0.5f ? 0.5f : time;
                    //gridGroup.MoveToCellIndex(cellIndex, time);
                }
            }
        }

        private void OnSelectEquipment(ulong uId)
        {
            listener?.OnSelectEquipment(uId);
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        private void OnNtfInlay()
        {
            foreach (var data in dicEquipments)
            {
                data.Value.Refresh();
            }
        }

        private void OnNtfUnload()
        {
            foreach (var data in dicEquipments)
            {
                data.Value.Refresh();
            }
        }

        private void OnJewelNtfQuickCompose()
        {
            foreach (var data in dicEquipments)
            {
                data.Value.Refresh();
            }
        }

        private void OnNtfSmelt()
        {
            foreach (var data in dicEquipments)
            {
                data.Value.Refresh();
            }
        }

        private void OnNtfMakeUse()
        {
            foreach (var data in dicEquipments)
            {
                data.Value.Refresh();
            }
        }

        private void OnNtfRepair(bool strength)
        {
            foreach (var data in dicEquipments)
            {
                data.Value.Refresh();
            }
        }
        
        private void OnNtfRefreshEffect()
        {
            foreach (var data in dicEquipments)
            {
                data.Value.Refresh();
            }
        }

        public interface IListener
        {
            void OnSelectEquipment(ulong uId);
        }
    }
}


