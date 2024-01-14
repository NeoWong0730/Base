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
        
        public List<uint> slotLeves = new List<uint>();
        public List<uint> slotExps = new List<uint>();
        public List<uint> effects = new List<uint>();
        

        public bool IsSlotUpgradeTip = true;
        public uint slotLowMatTimes = 0;
        public uint slotLowMatNextTime = 0;
        
        private void OnInitSlotUpgradeNtf()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.BodyEnhanceReq, (ushort)CmdItem.BodyEnhanceRes, OnBodyEnhnaceRes, CmdItemBodyEnhanceRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.BodyEnhanceEffectRefreashReq, (ushort)CmdItem.BodyEnhanceEffectRefreashRes, OnBodyEnhanceEffectRefreashRes, CmdItemBodyEnhanceEffectRefreashRes.Parser);
        }
        
        public void OnNotifySlotData(PackageChangeNotify ntf)
        {
            if (ntf.SlotEnhanceLevel != null && ntf.SlotEnhanceLevel.Count > 0)
            {
                slotLeves.Clear();
                slotLeves.AddRange(ntf.SlotEnhanceLevel);
            }

            if (ntf.SlotEnhanceExp != null && ntf.SlotEnhanceExp.Count > 0)
            {
                slotExps.Clear();
                slotExps.AddRange(ntf.SlotEnhanceExp);
            }

            if (ntf.BodyEffect != null &&  ntf.BodyEffect.Count > 0)
            {
                effects.Clear();
                effects.AddRange(ntf.BodyEffect);
                
                eventEmitter.Trigger(EEvents.OnNtfBodyUpgrade);
            }
        }
        
        public void OnBodyEnhanceReq(uint slot, ulong itemId)
        {
            CmdItemBodyEnhanceReq req = new CmdItemBodyEnhanceReq();
            req.Slot = slot;
            req.ItemId = itemId;
            NetClient.Instance.SendMessage((ushort)CmdItem.BodyEnhanceReq, req);
        }

        private void OnBodyEnhnaceRes(NetMsg msg)
        {
            CmdItemBodyEnhanceRes res = NetMsgUtil.Deserialize<CmdItemBodyEnhanceRes>(CmdItemBodyEnhanceRes.Parser, msg);
            
            slotLowMatTimes = 100 - res.SlotUpgradeUseLowTimes;
            slotLowMatNextTime = res.SlotUpgradeUseLowRefreshLeftTime;
            
            CheckSlotNextTime();
            
            if (res.Slot != 0)
            {
                int index = res.Slot > 1 ? (int)res.Slot - 1 : (int)res.Slot;
                index--;//数据索引
                if (index < slotLeves.Count && index < slotExps.Count)
                {
                    slotLeves[index] = res.Level;
                    slotExps[index] = res.Exp;
                }
            
                eventEmitter.Trigger(EEvents.OnNtfBodyUpgrade);
            }
        }

        public void CheckSlotNextTime()
        {
            if (Sys_Time.Instance.GetServerTime() > slotLowMatNextTime)
            {
                slotLowMatTimes = 100; //超过时间，重置剩余次数
            }
        }

        public void OnBodyEnhanceEffectRefreshReq(uint id)
        {
            CmdItemBodyEnhanceEffectRefreashReq req = new CmdItemBodyEnhanceEffectRefreashReq();
            req.Id = id;
            NetClient.Instance.SendMessage((ushort)CmdItem.BodyEnhanceEffectRefreashReq, req);
        }

        private void OnBodyEnhanceEffectRefreashRes(NetMsg msg)
        {
            CmdItemBodyEnhanceEffectRefreashRes res = NetMsgUtil.Deserialize<CmdItemBodyEnhanceEffectRefreashRes>(CmdItemBodyEnhanceEffectRefreashRes.Parser, msg);
            int index = (int)res.Id - 1;
            if (index < effects.Count)
                effects[index] = res.EffectId;

            eventEmitter.Trigger(EEvents.OnNtfBodyUpgradeSkillRefresh);
        }
        
        public uint GetSlotLevel(int index)
        {
            int tempIndex = index - 1;
            if (tempIndex < slotLeves.Count)
                return slotLeves[tempIndex] == 0 ? 1 : slotLeves[tempIndex];
            return 0;
        }

        public uint GetSlotExp(int index)
        {
            int tempIndex = index - 1;
            if (tempIndex < slotExps.Count)
                return slotExps[tempIndex];
            return 0;
        }

        public uint GetSlotEffect(int index)
        {
            if (index < effects.Count)
                return effects[index];
            return 0;
        }

        public uint GetSlotsMinLevel()
        {
            uint lev = slotLeves.Count > 0 ? slotLeves[0] : 1;
            for (int i = 0; i < slotLeves.Count; ++i)
            {
                if (slotLeves[i] < lev)
                    lev = slotLeves[i];
            }
            
            return lev;
        }

        public CSVSlotUpgrade.Data GetEquipSlotData(uint slot, uint level)
        {
            int count = CSVSlotUpgrade.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                CSVSlotUpgrade.Data data = CSVSlotUpgrade.Instance.GetByIndex(i);
                if (data.slot_id == slot && data.lev == level)
                    return data;
            }
            
            return null;
        }

        public uint GetEquipRebuildExpendId(uint lev, uint type, uint score)
        {
            List<CSVRebuildExpend.Data> listDatas = new List<CSVRebuildExpend.Data>();
            int count = CSVRebuildExpend.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                CSVRebuildExpend.Data temp = CSVRebuildExpend.Instance.GetByIndex(i);
                if (temp.equip_lv == lev && temp.equip_type == type)
                {
                    listDatas.Add(temp);
                }
            }

            int length = listDatas.Count;
            for (int i = length - 1; i >= 0; --i)
            {
                if (score >= listDatas[i].score)
                {
                    return listDatas[i].id;
                }
            }

            return 0;
        }

        public List<uint> GetEffects(uint groupId)
        {
            List<uint> list = new List<uint>();
            int count = CSVEquipmentEffect.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                CSVEquipmentEffect.Data data = CSVEquipmentEffect.Instance.GetByIndex(i);
                if (data.group_id == groupId)
                    list.Add(data.effect);
            }

            return list;
        }
    }
}

