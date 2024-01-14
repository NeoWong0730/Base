using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_Equipment : UIBase, UI_Equipment_Layout.IListener, UI_Equipment_Left.IListener
    {
        private UI_Equipment_Layout layout;

        private Dictionary<Sys_Equip.EquipmentOperations, UIParseCommon> dictOpPanel;

        private UIParseCommon curPanel;

        private ItemData curEquip;
        private Sys_Equip.EquipmentOperations _OpType = Sys_Equip.EquipmentOperations.Inlay;

        private Sys_Equip.UIEquipPrama _param;

        protected override void OnLoaded()
        {            
            layout = new UI_Equipment_Layout();
            layout.Parse(gameObject);
            layout.RegisterEvents(this);
            layout.leftList.Register(this);

            dictOpPanel = new Dictionary<Sys_Equip.EquipmentOperations, UIParseCommon>();

            dictOpPanel.Add(Sys_Equip.EquipmentOperations.Inlay, layout.viewInlay);
            dictOpPanel.Add(Sys_Equip.EquipmentOperations.Smelt, layout.viewSmelt);
            dictOpPanel.Add(Sys_Equip.EquipmentOperations.Quenching, layout.viewQuenching);
            dictOpPanel.Add(Sys_Equip.EquipmentOperations.Repair, layout.repair);
            dictOpPanel.Add(Sys_Equip.EquipmentOperations.Make, layout.make);
            dictOpPanel.Add(Sys_Equip.EquipmentOperations.ReMake, layout.reMake);
            dictOpPanel.Add(Sys_Equip.EquipmentOperations.RfreshEffect, layout.refreshEffect);
        }

        protected override void OnOpen(object arg)
        {            
            if (arg != null)
            {
                _param = (Sys_Equip.UIEquipPrama)arg;
                curEquip = _param.curEquip;
                _OpType = _param.opType;
            }
            else
            {
                if (_param != null)
                {
                    curEquip = _param.curEquip;
                    _OpType = _param.opType;
                }
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Equip.Instance.eventEmitter.Handle<Sys_Equip.EquipmentOperations>(Sys_Equip.EEvents.OnOperationType, OnEquipOpType, toRegister);
            Sys_Equip.Instance.eventEmitter.Handle<ulong>(Sys_Equip.EEvents.OnNotifyQuenching, OnNotifyQuenching, toRegister);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNtfRebuildEquip, OnRebuildEquip, toRegister);
        }

        protected override void OnShow()
        {            
            layout.currencyTitle.InitUi();
            UpdateInfo();
        }        

        protected override void OnDestroy()
        {
            layout.leftList.OnDestroy();

            foreach (var data in dictOpPanel)
            {
                data.Value.OnDestroy();
            }

            layout.currencyTitle.Dispose();
        }

        private void UpdateInfo()
        {
            layout.leftTabs.OnDefaultSelect(_OpType);
        }

        private void OnEquipOpType(Sys_Equip.EquipmentOperations _type)
        {
            Sys_Equip.Instance.curOperationType = _type;
            

            foreach (var data in dictOpPanel)
            {
                if (data.Key == _type)
                {
                    curPanel = data.Value;

                    if (!data.Value.gameObject.activeSelf)
                        data.Value.Show();

                    layout.leftList.CalEquipList(_type, _OpType == _type ? curEquip : null);
                }
                else
                {
                    data.Value.Hide();
                }
            }

            _OpType = _type;
        }

        #region interface

        public void OnClickClose()
        {
            UIManager.CloseUI(EUIID.UI_Equipment);
        }

        public void OnSelectEquipment(ulong uId)
        {
            curEquip = Sys_Equip.Instance.GetItemData(uId);
            if (curEquip != null)
            {
                layout.noneTip.SetActive(false);
                curPanel?.UpdateInfo(curEquip);
            }
            else
            {
                layout.noneTip.SetActive(true);

                if (Sys_Equip.Instance.curOperationType == Sys_Equip.EquipmentOperations.Make)
                    layout.SetNoneTip(4193u);
                else
                    layout.SetNoneTip(4194u);

                curPanel?.Hide();
            }
        }
        #endregion

        #region notfy
        private void OnNotifyQuenching(ulong uuId)
        {
            curEquip = null;
            OnEquipOpType(Sys_Equip.EquipmentOperations.Quenching);
        }
        
        private void OnRebuildEquip()
        {
            curEquip = null;
            OnEquipOpType(Sys_Equip.EquipmentOperations.ReMake);
        }
        #endregion
    }
}

