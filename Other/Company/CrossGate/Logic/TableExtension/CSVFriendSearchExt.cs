using System.Collections.Generic;

namespace Table
{
    public partial class CSVFriendSearch
    {
        List<CSVFriendSearch.Data>  _ageDatas;

        public List<CSVFriendSearch.Data>  AgeDatas
        {
            get
            {
                if (_ageDatas == null)
                {
                    _ageDatas = new List<CSVFriendSearch.Data>();
                    foreach (CSVFriendSearch.Data data in GetAll())
                    {
                        if (data.FriendSearchClass == 1)
                        {
                            _ageDatas.Add(data);
                        }
                    }
                }

                return _ageDatas;
            }
        }

        List<CSVFriendSearch.Data>  _sexDatas;

        public List<CSVFriendSearch.Data>  SexDatas
        {
            get
            {
                if (_sexDatas == null)
                {
                    _sexDatas = new List<CSVFriendSearch.Data>();
                    foreach (CSVFriendSearch.Data data in GetAll())
                    {
                        if (data.FriendSearchClass == 2)
                        {
                            _sexDatas.Add(data);
                        }
                    }
                }

                return _sexDatas;
            }
        }

        List<CSVFriendSearch.Data>  _interestDatas;

        public List<CSVFriendSearch.Data>  InterestDatas
        {
            get
            {
                if (_interestDatas == null)
                {
                    _interestDatas = new List<CSVFriendSearch.Data>();
                    foreach (CSVFriendSearch.Data data in GetAll())
                    {
                        if (data.FriendSearchClass == 3)
                        {
                            _interestDatas.Add(data);
                        }
                    }
                }

                return _interestDatas;
            }
        }

        List<CSVFriendSearch.Data>  _locationDatas;

        public List<CSVFriendSearch.Data>  LocationDatas
        {
            get
            {
                if (_locationDatas == null)
                {
                    _locationDatas = new List<CSVFriendSearch.Data>();
                    foreach (CSVFriendSearch.Data data in GetAll())
                    {
                        if (data.FriendSearchClass == 4)
                        {
                            _locationDatas.Add(data);
                        }
                    }
                }

                return _locationDatas;
            }
        }

        public int GetAgeDataIndex(uint id)
        {
            for (int index = 0, len = AgeDatas.Count; index < len; index++)
            {
                if (AgeDatas[index].id == id)
                    return index;
            }
            return 0;
        }

        public int GetSexDataIndex(uint id)
        {
            for (int index = 0, len = SexDatas.Count; index < len; index++)
            {
                if (SexDatas[index].id == id)
                    return index;
            }
            return 0;
        }

        public int GetLocationDataIndex(uint id)
        {
            for (int index = 0, len = LocationDatas.Count; index < len; index++)
            {
                if (LocationDatas[index].id == id)
                    return index;
            }
            return 0;
        }
    }
}
