using Logic;
using System.Collections.Generic;

namespace Table
{
    public partial class CSVSuit
    {
        public CSVSuit.Data GetSuitData(uint suitId, uint slotId = 0u)
        {
            for (int i = 0; i < Count; ++i)
            {
                CSVSuit.Data data = CSVSuit.Instance.GetByIndex(i);
                if (data.suit_id == suitId)
                {
                    if (slotId == 0u)
                        return data;
                    else if (slotId == data.slot_id)
                        return data;
                }
            }

            return null;
        }
    }
}
