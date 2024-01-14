using Logic;
using System.Collections.Generic;

namespace Table
{
    public partial class CSVGreen
    {
        public CSVGreen.Data GetGreenData(uint libId, uint attrId)
        {
            int count = CSVGreen.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                CSVGreen.Data data = Instance.GetByIndex(i);
                if (data.group_id == libId 
                    && data.attr_id == attrId)
                {
                    return data;
                }
            }

            return null;
        }
    }
}
