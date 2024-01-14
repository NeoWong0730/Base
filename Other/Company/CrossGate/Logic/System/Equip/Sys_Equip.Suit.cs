using System.Collections.Generic;
using Logic.Core;
using Packet;

namespace Logic
{
    public partial class Sys_Equip : SystemModuleBase<Sys_Equip>
    {
        public uint CalSuitNumber(uint suitId)
        {
            uint num = 0;
            List<ItemData> itemList = null;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdEquipment, out itemList))
            {
                for (int i = 0; i < itemList.Count; ++i)
                {
                    if (itemList[i].Equip.SuitTypeId == suitId)
                        num++;
                }
            }

            return num;
        }
    }
}

