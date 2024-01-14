using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Packet;
using System;

namespace Logic
{

    public class ItemData : IDisposable
    {
        public CSVItem.Data cSVItemData { get; private set; }

        public int BoxId { get; private set; }

        public ulong Uuid { get; private set; }

        public uint Id { get; private set; }

        public uint Position { get; private set; }

        public uint Count { get; private set; }

        public bool bNew { get; set; }

        public bool bBind { get; private set; }

        public Equipment Equip { get; private set; }

        public PetUnit Pet { get; private set; }

        public Essence essence { get; private set; }

        public Crystal crystal { get; private set; }

        public Ornament ornament { get; private set; }

        public PetEquip petEquip { get; private set; }
        public int maxDurability
        {
            get
            {
                if (cSVItemData.type_id == (int)EItemType.Crystal)
                {
                    CSVElementAttributes.Data cSVElementAttributesData = CSVElementAttributes.Instance.GetConfData(Id);
                    if (cSVElementAttributesData != null)
                    {
                        return (int)cSVElementAttributesData.durable;
                    }
                    else
                    {
                        return -1;
                    }
                }
                return -1;
            }
        }

        public int MarketendTime { get; private set; }   //-1 代表永久禁售 0代表没限制 其他代表截至时间戳(单位：天)

        public MarketendTimer marketendTimer;

        public bool bMarketEnd
        {
            get
            {
                if (marketendTimer == null)
                {
                    return false;
                }
                else
                {
                    return marketendTimer.marketend;
                }
            }
        }

        public uint Quality { get; private set; }

        public uint SrcUIId = 0u; //来源UI,ID

        public uint EquipParam { get; set; }

        public uint outTime { get; set; }
        
        public bool IsLocked { get; set; }

        public ItemData(int _boxid, ulong _uuid, uint _id, uint _count, uint _position, bool _new, bool _bound, Equipment _equip,
            Essence _essence, int _marketendtime, PetUnit _petUnit = null, Crystal _crystal = null, Ornament _ornament = null, PetEquip _petEquip = null)
        {
            BoxId = _boxid;
            Uuid = _uuid;
            Id = _id;
            Count = _count;
            Position = _position;
            bNew = _new;
            bBind = _bound;
            Equip = _equip;
            essence = _essence;
            crystal = _crystal;
            ornament = _ornament;
            Pet = _petUnit;
            petEquip = _petEquip;
            cSVItemData = CSVItem.Instance.GetConfData(_id);
            MarketendTime = _marketendtime;
            marketendTimer = new MarketendTimer();
            CalItemMaretState();
            CalItemQuality();
        }

        public ItemData() { }

        public ItemData SetData(int _boxid, ulong _uuid, uint _id, uint _count, uint _position, bool _new, bool _bound, Equipment _equip,
            Essence _essence, int _marketendtime, PetUnit _petUnit = null, Crystal _crystal = null, Ornament _ornament = null, PetEquip _petEquip = null, bool isLocked = false)
        {
            BoxId = _boxid;
            Uuid = _uuid;
            Id = _id;
            Count = _count;
            Position = _position;
            bNew = _new;
            bBind = _bound;
            Equip = _equip;
            essence = _essence;
            crystal = _crystal;
            ornament = _ornament;
            Pet = _petUnit;
            petEquip = _petEquip;
            IsLocked = isLocked;
            cSVItemData = CSVItem.Instance.GetConfData(_id);
            MarketendTime = _marketendtime;
            marketendTimer = new MarketendTimer();
            CalItemMaretState();
            CalItemQuality();
            return this;
        }

        public void SetData(ItemData data)
        {
            BoxId = data.BoxId;
            Uuid = data.Uuid;
            Id = data.Id;
            Count = data.Count;
            Position = data.Position;
            bNew = data.bNew;
            bBind = data.bBind;
            Equip = data.Equip;
            essence = data.essence;
            crystal = data.crystal;
            ornament = data.ornament;
            Pet = data.Pet;
            petEquip = data.petEquip;
            cSVItemData = data.cSVItemData;

            MarketendTime = data.MarketendTime;

            marketendTimer = new MarketendTimer();
            CalItemMaretState();
            CalItemQuality();

        }
        public void SetCount(uint count)
        {
            Count = count;
        }

        public void SetQuality(uint quality)
        {
            Quality = quality;
        }

        public void UpdateEquip(Equipment equip)
        {
            Equip = equip;
        }

        private void CalItemMaretState()
        {
            if (bBind)
            {
                MarketendTime = 0;
                marketendTimer.foreverMarket = false;
            }
            if (MarketendTime == -1)
            {
                marketendTimer.marketend = false;
                marketendTimer.foreverMarket = true;
            }
            else if (MarketendTime == 0)
            {
                marketendTimer.marketend = true;
                marketendTimer.foreverMarket = false;
            }
            else
            {
                marketendTimer.foreverMarket = false;
                marketendTimer.marketendTime = (uint)MarketendTime * 86400;
                marketendTimer.marketendDateTime = Sys_Time.ConvertToLocalTime(MarketendTime * 86400);
                marketendTimer.UpdateReMainMarkendTime();
            }
        }

        private void CalItemQuality()
        {
            uint typeId = cSVItemData.type_id;
            if (typeId == (int)EItemType.Equipment)
            {
                if (Equip != null)
                    Quality = (uint)Equip.Color;
                else
                    Quality = cSVItemData.quality;
            }
            else if (typeId == (int)EItemType.Ornament)
            {
                if (ornament != null)
                {
                    Quality = Sys_Ornament.Instance.GetQualityByScore(Id, ornament.Score);
                }
                else
                {
                    Quality = cSVItemData.quality;
                }
            }
            else if (typeId == (int)EItemType.PetEquipment)
            {
                if (petEquip != null)
                {
                    Quality = petEquip.Color;
                }
            }
            else
            {
                Quality = cSVItemData.quality;
            }
        }

        public void Dispose()
        {
            //Reset();
        }

        public void Reset()
        {
            BoxId = 0;
            Uuid = 0;
            Id = 0;
            Count = 0;
            Position = 0;
            bNew = false;
            bBind = false;
            Equip = null;
            essence = null;
            crystal = null;
            Pet = null;
            petEquip = null;
            cSVItemData = null;
            MarketendTime = 0;
            marketendTimer = null;
            Quality = 0;
            SrcUIId = 0;
            outTime = 0;
        }
    }

    public class MarketendTimer
    {
        public bool foreverMarket;      //是否永久禁售
        public bool marketend;          //禁售期是否结束
        public uint marketendTime;      //禁售期结束的时间戳(s)
        public uint remainTime;         //剩余时间(s)
        public DateTime marketendDateTime;

        public void UpdateReMainMarkendTime()
        {
            if (foreverMarket)//永久禁售 就不需要继续计算了
            {
                marketend = false;
                return;
            }
            uint time = Sys_Time.Instance.GetServerTime();
            marketend = time >= marketendTime;
            if (!marketend)
            {
                remainTime = marketendTime - time;
            }
            else
            {
                remainTime = 0;
            }
        }

        public string GetMarkendTimeFormat()
        {
            UpdateReMainMarkendTime();
            string str = string.Format(CSVLanguage.Instance.GetConfData(1009001).words, marketendDateTime.Year.ToString(), marketendDateTime.Month.ToString(), marketendDateTime.Day.ToString());
            return str;
        }
    }

}
