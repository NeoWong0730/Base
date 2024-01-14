using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Logic;

namespace Table
{
    public class ItemIdCount
    {
        public uint id;
        public long count;
        public uint equipPara;

        public CSVItem.Data CSV => CSVItem.Instance.GetConfData(id);
        public long CountInBag => Sys_Bag.Instance.GetItemCount(id);
        public virtual bool Enough => id != 0 && CountInBag >= count;

        public ItemIdCount() { }
        public ItemIdCount(uint id, long count)
        {
            this.id = id;
            this.count = count;
        }
        public ItemIdCount Reset(uint id, long count)
        {
            this.id = id;
            this.count = count;
            return this;
        }

        public static bool IsEnough(IList<ItemIdCount> ls)
        {
            bool ret = true;
            if (ls != null)
            {
                for (int i = 0, length = ls.Count; i < length; ++i)
                {
                    ret &= ls[i].Enough;
                    if (!ret)
                    {
                        break;
                    }
                }
            }
            return ret;
        }
    }

    public class ItemGuidCount : ItemIdCount
    {
        public ulong guid;

        public ItemGuidCount() { }
        public ItemGuidCount(ulong guid, uint id, long count) : base(id, count)
        {
            this.guid = guid;
        }
    }

    public partial class CSVDrop
    {
        private Dictionary<uint, List<CSVDrop.Data>> cacheDatas = new Dictionary<uint, List<CSVDrop.Data>>();

        private bool isCacheData = false;

        public void CacheData()
        {
            cacheDatas.Clear();

            var dictDrop = GetAll();
            foreach (var data in dictDrop)
            {
                if (cacheDatas.ContainsKey(data.drop_id))
                {
                    cacheDatas[data.drop_id].Add(data);
                }
                else
                {
                    cacheDatas.Add(data.drop_id, new List<CSVDrop.Data>() { data });
                }
            }
        }

        public List<ItemIdCount> GetDropItem(uint _dropId)
        {
            if (!isCacheData)
            {
                isCacheData = true;
                CacheData();
            }

            List<ItemIdCount> listDrops = new List<ItemIdCount>();

            List<CSVDrop.Data>  tempList = null;
            if (cacheDatas.TryGetValue(_dropId, out tempList))
            {
                foreach (var data in tempList)
                {
                    if (IsFixCondition(data, _dropId) && data.reward_show != null)
                    {
                        foreach (var drop in data.reward_show)
                        {
                            ItemIdCount temp = new ItemIdCount();

                            try
                            {
                                temp.id = drop[0];
                                temp.count = drop[1];
                                temp.equipPara = data.equip_para;

                                listDrops.Add(temp);
                            }
                            catch (Exception e)
                            {
                                Debug.LogErrorFormat("drop reward_show error Id={0}", _dropId);
                            }
                        }
                    }
                }
            }

            return listDrops;
        }

        public CSVDrop.Data GetDropItemData(uint dropId)
        {
            if (!isCacheData)
            {
                isCacheData = true;
                CacheData();
            }

            List<ItemIdCount> listDrops = new List<ItemIdCount>();
            List<CSVDrop.Data>  tempList = null;
            if (cacheDatas.TryGetValue(dropId, out tempList))
            {
                foreach (var data in tempList)
                {
                    if (IsFixCondition(data, dropId) && data.reward_show != null)
                    {
                        return data;
                    }
                }
            }
            return null;
        }


        private bool IsFixCondition(CSVDrop.Data _dropData, uint _dropId)
        {
            bool isFixId = _dropData.drop_id == _dropId;
            bool isFixHero = IsFixHero(_dropData);
            bool isFixCareer = IsFixCareer(_dropData);
            bool isFixLevel = IsFixLevel(_dropData);
            bool isFixWorldLevel = IsFixWorldLevel(_dropData);

            return isFixId && isFixHero && isFixCareer && isFixLevel && isFixWorldLevel;
        }

        private bool IsFixHero(CSVDrop.Data _dropData)
        {
            bool isFix = false;
            isFix = _dropData.role == null;
            if (!isFix)
            {
                isFix = _dropData.role.Contains(Sys_Role.Instance.HeroId);
            }

            return isFix;
        }

        private bool IsFixCareer(CSVDrop.Data _dropData)
        {
            bool isFix = false;
            isFix = _dropData.career == null;
            if (!isFix)
            {
                isFix = _dropData.career.Contains(Sys_Role.Instance.Role.Career);
            }

            return isFix;
        }

        private bool IsFixLevel(CSVDrop.Data _dropData)
        {
            bool isFix = false;
            isFix = _dropData.level == null;
            if (!isFix && _dropData.level.Count == 2)
            {
                uint minLevel = _dropData.level[0];
                uint maxLevel = _dropData.level[1];

                uint roleLevel = Sys_Role.Instance.Role.Level;

                isFix = roleLevel >= minLevel && roleLevel <= maxLevel;
            }

            return isFix;
        }

        private bool IsFixWorldLevel(CSVDrop.Data _dropData)
        {
            bool isFix = false;
            isFix = _dropData.world_level == null;
            if (!isFix && _dropData.world_level.Count == 2)
            {
                uint minLevel = _dropData.world_level[0];
                uint maxLevel = _dropData.world_level[1];

                //uint roleLevel = Sys_Role.Instance.Role.Level;

                //isFix = roleLevel >= minLevel && roleLevel <= maxLevel;
                isFix = true;
            }

            return isFix;
        }
    }
}


