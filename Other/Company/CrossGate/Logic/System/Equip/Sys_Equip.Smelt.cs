using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public partial class Sys_Equip : SystemModuleBase<Sys_Equip>
    {

        public bool IsCloseSmeltBoxTip = false;

        public void OnEquipmentSmeltReq(ulong equipUId)
        {
            CmdItemSmeltEquipmentReq req = new CmdItemSmeltEquipmentReq();
            req.Uuid = equipUId;
            NetClient.Instance.SendMessage((ushort)CmdItem.SmeltEquipmentReq, req);
        }

        private void OnEquipmentSmeltRes(NetMsg msg)
        {
            eventEmitter.Trigger(EEvents.OnNotifySmelt);
        }

        public void OnEquipmentRevertSmeltReq(ulong equipUId)
        {
            CmdItemRevertSmeltReq req = new CmdItemRevertSmeltReq();
            req.Uuid = equipUId;
            NetClient.Instance.SendMessage((ushort)CmdItem.RevertSmeltReq, req);
        }

        private void OnEquipmentRevertSmeltRes(NetMsg msg)
        {
            eventEmitter.Trigger(EEvents.OnNotifyRevertSmelt);
        }
    }
}

