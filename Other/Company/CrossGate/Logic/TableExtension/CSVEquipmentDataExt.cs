using Logic;
using System.Collections.Generic;
using Lib.Core;

namespace Table
{
    /// <summary>
    /// NPC表扩展///
    /// </summary>
    public static class CSVEquipmentDataExt
    {
        public static int TransLevel(this CSVEquipment.Data data)
        {
            int level = (int)data.equipment_level % 10;
            level = level == 0 ? (int)data.equipment_level / 10 + 1 : level;
            return level;
        }
    }
}
