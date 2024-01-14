using System.Collections.Generic;

namespace Table
{
    public partial class CSVItem
    {
        List<CSVItem.Data>  _giftDatas;

        public List<CSVItem.Data>  GiftDatas
        {
            get
            {
                if (_giftDatas == null)
                {
                    _giftDatas = new List<CSVItem.Data>();
                    foreach (CSVItem.Data data in GetAll())
                    {
                        if (data.PresentType == 1)
                        {
                            _giftDatas.Add(data);
                        }
                    }
                }

                return _giftDatas;
            }
        }

        List<CSVItem.Data>  _commonItemDatas;

        public List<CSVItem.Data>  CommonItemDatas
        {
            get
            {
                if (_commonItemDatas == null)
                {
                    _commonItemDatas = new List<CSVItem.Data>();
                    foreach (CSVItem.Data data in GetAll())
                    {
                        if (data.PresentType == 2)
                        {
                            _commonItemDatas.Add(data);
                        }
                    }
                }

                return _commonItemDatas;
            }
        }

        List<CSVItem.Data>  _unCommonItemDatas;

        public List<CSVItem.Data>  UnCommonItemDatas
        {
            get
            {
                if (_unCommonItemDatas == null)
                {
                    _unCommonItemDatas = new List<CSVItem.Data>();
                    foreach (CSVItem.Data data in GetAll())
                    {
                        if (data.PresentType == 3)
                        {
                            _unCommonItemDatas.Add(data);
                        }
                    }
                }

                return _unCommonItemDatas;
            }
        }

        public int GetGiftIndexByID(uint id)
        {
            for (int index = 0, len = GiftDatas.Count; index < len; index++)
            {
                if (GiftDatas[index].id == id)
                    return index;
            }
            return -1;
        }

        public int GetCommonIndexByID(uint id)
        {
            for (int index = 0, len = CommonItemDatas.Count; index < len; index++)
            {
                if (CommonItemDatas[index].id == id)
                    return index;
            }
            return -1;
        }

        public int GetUnCommonIndexByID(uint id)
        {
            for (int index = 0, len = UnCommonItemDatas.Count; index < len; index++)
            {
                if (UnCommonItemDatas[index].id == id)
                    return index;
            }
            return -1;
        }
    }
}