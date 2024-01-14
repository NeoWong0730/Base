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
        public void OnEquipmentQuenchingReq(ulong equipUId)
        {
            CmdItemExtractEquipmentReq req = new CmdItemExtractEquipmentReq();
            req.Uuid = equipUId;
            NetClient.Instance.SendMessage((ushort)CmdItem.ExtractEquipmentReq, req);
        }

        private void OnEquipmentQuenchingRes(NetMsg msg)
        {
            CmdItemExtractEquipmentRes res = NetMsgUtil.Deserialize<CmdItemExtractEquipmentRes>(CmdItemExtractEquipmentRes.Parser, msg);
            eventEmitter.Trigger(EEvents.OnNotifyQuenching, res.Uuid);

            UIManager.OpenUI(EUIID.UI_Quenching_Success, false, res.Uuid);
        }
    }
}

