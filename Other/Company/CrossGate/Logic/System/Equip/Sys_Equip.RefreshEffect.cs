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
        private void OnInitRefreshEffectNtf()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.ReFreshEquipEffectReq, (ushort)CmdItem.ReFreshEquipEffectRes, OnRefreshEffectRes, CmdItemReFreshEquipEffectRes.Parser);
            //EventDispatcher.Instance.AddEventListener((ushort)CmdItem.refre, (ushort)CmdItem.ReFreshEquipEffectRes, OnRefreshEffectRes, CmdItemReFreshEquipEffectRes.Parser);
        }

        public void OnRefreshEffectReq(ulong uId, bool reset = false)
        {
            CmdItemReFreshEquipEffectReq req = new CmdItemReFreshEquipEffectReq();
            req.Uuid = uId;
            req.IsReset = reset;
            NetClient.Instance.SendMessage((ushort)CmdItem.ReFreshEquipEffectReq, req);
        }

        private void OnRefreshEffectRes(NetMsg msg)
        {
            CmdItemReFreshEquipEffectRes res = NetMsgUtil.Deserialize<CmdItemReFreshEquipEffectRes>(CmdItemReFreshEquipEffectRes.Parser, msg);
            ItemData item = Sys_Bag.Instance.GetItemDataByUuid(res.Item.Uuid);
            item.UpdateEquip(res.Item.Equipment);
            eventEmitter.Trigger(EEvents.OnNtfRefreshEffect);
            //Debug.LogError("CmdItemReFreshEquipEffectRes");
        }
    }
}

