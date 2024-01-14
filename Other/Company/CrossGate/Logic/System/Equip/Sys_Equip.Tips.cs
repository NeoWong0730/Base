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
        //Tips
        //public List<ItemData> compareList = new List<ItemData>();
        //public ItemData curCompareItem = null;

        //public void CalSameItems(List<uint> slotIds)
        //{
        //    compareList.Clear();
        //    curCompareItem = null;

        //    List<ItemData> listItems = null;
        //    if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdEquipment, out listItems))
        //    {
        //        foreach (ItemData item in listItems)
        //        {
        //            if (slotIds.Contains(item.Position))
        //            {
        //                compareList.Add(item);
        //            }
        //        }
        //    }
        //}

        public bool SameCountsEquipment(List<uint> slotIds)
        {
            int tempCount = 0;
            if (slotIds != null)
            {
                for (int i = 0; i < slotIds.Count; ++i)
                {
                    if (SameEquipment(slotIds[i]) != null)
                    {
                        tempCount++;
                    }
                }
            }
            else
            {
                return false;
            }

            return tempCount == slotIds.Count;
        }

        public ItemData SameEquipment(uint equipSlot)
        {
            List<ItemData> listItems = null;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdEquipment, out listItems))
            {
                for (int i = 0; i < listItems.Count; ++i)
                {
                    if (listItems[i].Position == equipSlot)
                    {
                        //Debug.LogError(listItems[i].Id);
                        return listItems[i];
                    }
                }
            }

            return null;
        }

        public bool IsShowCompare(uint infoId, ref ItemData _resultItem)
        {
            bool isCompare = false;

            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(infoId);
            if (equipData != null)
            {
                for (int i = 0; i < equipData.slot_id.Count; ++i)
                {
                    ItemData result = SameEquipment(equipData.slot_id[i]);
                    if (result != null)
                    {
                        _resultItem = result;
                        isCompare = true;
                        break;
                    }
                }
                
            }

            return isCompare;
        }
    }
}

