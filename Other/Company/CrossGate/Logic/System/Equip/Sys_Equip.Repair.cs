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
        private bool IsRepairStrongth = false;

        public void OnEquipmentRepairReq(ulong equipUId, uint repairType)
        {
            CmdItemRepairEquipmentReq req = new CmdItemRepairEquipmentReq();
            req.Uuid = equipUId;
            req.RepairType = repairType;
            NetClient.Instance.SendMessage((ushort)CmdItem.RepairEquipmentReq, req);

            IsRepairStrongth = repairType == 1; //0,普通修理,1强化修理
        }

        private void OnEquipmentRepairRes(NetMsg msg)
        {
            CmdItemRepairEquipmentRes res = NetMsgUtil.Deserialize<CmdItemRepairEquipmentRes>(CmdItemRepairEquipmentRes.Parser, msg);
            eventEmitter.Trigger(EEvents.OnNotifyRepair, IsRepairStrongth);
        }
    }
}

