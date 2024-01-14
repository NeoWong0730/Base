using Logic;
using System.Collections.Generic;

namespace Table
{
    public partial class CSVPetMountStrengthen
    {
        public CSVPetMountStrengthen.Data GetMountPetIntensifyData(uint level, uint type)
        {
            int count = Count;
            for (int index = 0; index < count; index++)
            {
                var mountStrengthenData = GetByIndex(index);

                if (mountStrengthenData.type == type && mountStrengthenData.level == level)
                {
                    return mountStrengthenData;
                }
            }
            return null;
        }
    }
}
