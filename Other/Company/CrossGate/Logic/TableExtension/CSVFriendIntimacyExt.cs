using System.Collections.Generic;

namespace Table
{
    public partial class CSVFriendIntimacy
    {
        List<CSVFriendIntimacy.Data>  _sortDatas;

        public List<CSVFriendIntimacy.Data>  SortDatas
        {
            get
            {
                if (_sortDatas == null)
                {
                    _sortDatas = new List<CSVFriendIntimacy.Data>();
                    foreach (CSVFriendIntimacy.Data data in GetAll())
                    {
                        _sortDatas.Add(data);
                    }

                    _sortDatas.Sort((a, b) =>
                    {
                        if (a.IntimacyLvl > b.IntimacyLvl)
                            return -1;
                        else
                            return 1;
                    });
                }

                return _sortDatas;
            }
        }

        /// <summary>
        /// 根据当前的亲密度获取当前亲密档位///
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public CSVFriendIntimacy.Data GetDataByIntimacyValue(uint num)
        {
            for (int index = 0, len = SortDatas.Count; index < len; index++)
            {
                if (num >= SortDatas[index].IntimacyNeed)
                    return SortDatas[index];
            }

            return SortDatas[SortDatas.Count - 1];
        }
    }
}
