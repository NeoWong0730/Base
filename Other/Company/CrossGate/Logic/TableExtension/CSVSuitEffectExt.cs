using System.Collections.Generic;

namespace Table
{
    public partial class CSVSuitEffect
    {
        public CSVSuitEffect.Data GetSuitEffectData(uint suitId, uint num)
        {
            for (int i = 0; i < Count; ++i)
            {
                CSVSuitEffect.Data data = CSVSuitEffect.Instance.GetByIndex(i);
                if (data.suit_id == suitId && data.num == num)
                {
                    return data;
                }
            }

            return null;
        }

        public List<CSVSuitEffect.Data>  GetSuitEffectList(uint suitId, uint career)
        {
            List<CSVSuitEffect.Data>  listTemp = new List<CSVSuitEffect.Data>();
            for (int i = 0; i < Count; ++i)
            {
                CSVSuitEffect.Data data = CSVSuitEffect.Instance.GetByIndex(i);
                if (data.suit_id == suitId && data.career.IndexOf(career) >= 0)
                {
                    listTemp.Add(data);
                }
            }

            return listTemp;
        }
    }
}
