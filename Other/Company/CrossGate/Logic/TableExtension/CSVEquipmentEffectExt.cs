using System.Collections.Generic;

namespace Table
{
    public partial class CSVEquipmentEffect
    {
        public Data GetDataByEffectId(uint effectId)
        {
            for (int i = 0; i < Count; ++i)
            {
                Data temp = GetByIndex(i);
                if (temp.effect == effectId)
                    return temp;
            }
            
            return null;
        }
    }
}
