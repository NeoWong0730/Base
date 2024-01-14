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
    public class UI_Equipment_Remake_Preview : UIBase
    {
        private UI_Equipment_Remake_Preview_Layout layout;

        private EquipRemakeParam param;

        protected override void OnLoaded()
        {            
            layout = new UI_Equipment_Remake_Preview_Layout();
            layout.Init(transform);
        }

        protected override void OnOpen(object arg)
        {            
            param = null;
            if (arg != null)
                param = (EquipRemakeParam)arg;
        }

        //protected override void ProcessEventsForEnable(bool toRegister)
        //{            
        //    //Sys_Equip.Instance.eventEmitter.Handle<Sys_Equip.EquipmentOperations>(Sys_Equip.EEvents.OnOperationType, OnEquipOpType, toRegister);
        //    //Sys_Equip.Instance.eventEmitter.Handle<ulong>(Sys_Equip.EEvents.OnNotifyQuenching, OnNotifyQuenching, toRegister);
        //}

        protected override void OnShow()
        {
            layout?.UpdateInfo(param);
        }
    }
}

