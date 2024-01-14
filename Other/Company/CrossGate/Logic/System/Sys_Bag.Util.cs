using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using Lib.Core;
using UnityEngine;
using System;
using Framework;

namespace Logic
{
    public partial class Sys_Bag : SystemModuleBase<Sys_Bag>
    {
        public List<uint> meat = new List<uint>();
        public List<uint> seaFood = new List<uint>();
        public List<uint> fruit = new List<uint>();
        public List<uint> ingredients = new List<uint>();

        public Dictionary<int, bool> fullState = new Dictionary<int, bool>();

        #region Utils

        private void InitParms()
        {
            m_GiftBoxIds.Clear();
            Sys_Ini.Instance.Get<IniElement_IntArray>(963, out IniElement_IntArray array);
            int[] boxIds = array.value;
            for (int i = 0; i < boxIds.Length; i++)
            {
                m_GiftBoxIds.Add(boxIds[i]);
            }

            m_ForbidItemGetReason.Clear();
            Sys_Ini.Instance.Get<IniElement_IntArray>(1060, out IniElement_IntArray array2);
            int[] reasons = array2.value;
            for (int i = 0; i < reasons.Length; i++)
            {
                m_ForbidItemGetReason.Add((uint)reasons[i]);
            }

            meat.Clear();
            seaFood.Clear();
            fruit.Clear();
            ingredients.Clear();
            foreach (var item in CSVItem.Instance.GetAll())
            {
                CSVItem.Data cSVItemData = item;
                CSVFoodAtlas.Data cSVFoodAtlasData = CSVFoodAtlas.Instance.GetConfData(cSVItemData.id);
                if (cSVFoodAtlasData == null)
                {
                    continue;
                }
                if (cSVItemData.type_id == (uint)EItemType.Meat)
                {
                    meat.Add(cSVItemData.id);
                }
                if (cSVItemData.type_id == (uint)EItemType.SeaFood)
                {
                    seaFood.Add(cSVItemData.id);
                }
                if (cSVItemData.type_id == (uint)EItemType.Fruit)
                {
                    fruit.Add(cSVItemData.id);
                }
                if (cSVItemData.type_id == (uint)EItemType.Ingredients)
                {
                    ingredients.Add(cSVItemData.id);
                }
            }
        }

        public void SetCurrentInteractiveNPC()
        {
            if (Sys_Interactive.CurInteractiveNPC != null && Sys_Interactive.CurInteractiveNPC.cSVNpcData != null)
            {
                curInteractiveNPC = Sys_Interactive.CurInteractiveNPC.cSVNpcData.id;
            }
        }

        public void CheckTempBag()
        {
            bool canTempBagShow = GetBagVaildGridCountByBoxId(BoxIDEnum.BoxIdTemporary) > 0;
            eventEmitter.Trigger<bool>(EEvents.OnShowOrHideMenuTempBagIcon, canTempBagShow);
        }

        public int GetBagVaildGridCountByBoxId(BoxIDEnum boxid)
        {
            int _boxid = (int)boxid;
            if (BagItems.ContainsKey(_boxid))
            {
                List<ItemData> itemDatas = BagItems[_boxid];
                return itemDatas.Count;
            }
            return 0;
        }

        private List<ItemData> _items = new List<ItemData>();

        private Dictionary<uint, long> itemCounts = new Dictionary<uint, long>();

        private int[] UpdateItemIDs = new int[] { 1, 2, 3, 4, 7 };
        private void UpdateItemCount()
        {
            _items.Clear();
            itemCounts.Clear();
            int updateLength = UpdateItemIDs.Length;
            for (int i = 0; i < updateLength; ++i)
            {
                if (BagItems.ContainsKey(UpdateItemIDs[i]))
                {
                    List<ItemData> itemDatas = BagItems[UpdateItemIDs[i]];
                    if (itemDatas != null)
                    {
                        for (int n = 0; n < itemDatas.Count; ++n)
                        {
                            ItemData data = itemDatas[n];

                            if (!itemCounts.TryGetValue(data.Id, out long count))
                            {
                                itemCounts[data.Id] = 0;
                            }
                            count += data.Count;
                            itemCounts[data.Id] = count;
                        }
                    }
                }
            }
        }

        public long GetItemCount(uint itemId)
        {
            if (itemId == 4)
                return (long)Sys_Role.Instance.Role.Exp;

            if (itemId >= 0 && itemId < currency.Length)
                return currency[(int)(itemId)];

            itemCounts.TryGetValue(itemId, out long count);
            return count;
        }

        public long GetCurrencyCount(uint itemId)
        {
            return currency[(int)(itemId)];
        }


        public ItemData GetItemDataByUuid(ulong uuid)
        {
            foreach (var item in BagItems.Values)
            {
                foreach (var kv in item)
                {
                    ItemData data = kv;
                    if (data.Uuid == uuid)
                    {
                        return data;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 好感度赠礼
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ItemData GetGiftItemDatas(ulong uuid)
        {
            foreach (var item in BagItems)
            {
                if (m_GiftBoxIds.Contains(item.Key))
                {
                    foreach (var kv in item.Value)
                    {
                        if (kv.Uuid == uuid)
                        {
                            return kv;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 家族兽喂兽
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public ItemData GetFamilyItemDatas(ulong uuid)
        {
            foreach (var item in BagItems)
            {
                if (m_FamilyBoxIds.Contains(item.Key))
                {
                    foreach (var kv in item.Value)
                    {
                        if (kv.Uuid == uuid)
                        {
                            return kv;
                        }
                    }
                }
            }
            return null;
        }

        public int GetItemCountByUuid(ulong uuid)
        {
            int count = 0;
            foreach (var item in BagItems.Values)
            {
                foreach (var kv in item)
                {
                    ItemData data = kv;
                    if (data.Uuid == uuid)
                    {
                        count = (int)data.Count;
                    }
                }
            }
            return count;
        }

        public List<ulong> GetUuidsByItemId(uint itemId)
        {
            List<ulong> uuids = new List<ulong>();
            foreach (var item in BagItems.Values)
            {
                foreach (var kv in item)
                {
                    ItemData data = kv;
                    if (data.Id == itemId)
                    {
                        uuids.Add(data.Uuid);
                    }
                }
            }
            return uuids;
        }

        public int GetCountByItemType(uint itemType, int fixedBoxId)
        {
            int count = 0;
            List<ItemData> itemDatas = BagItems[fixedBoxId];
            for (int i = 0; i < itemDatas.Count; i++)
            {
                if (itemDatas[i].cSVItemData.type_id != itemType)
                {
                    continue;
                }
                count += (int)itemDatas[i].Count;
            }
            return count;
        }

        public List<ItemData> GetItemDatasByItemType(uint itemType, IList<Func<ItemData, bool>> filters = null,
            BoxIDEnum ignoreType1 = BoxIDEnum.BoxIdTemporary, BoxIDEnum ignoreType2 = BoxIDEnum.BoxIdBank)
        {
            List<ItemData> itemDatas = new List<ItemData>();
            Dictionary<int, List<ItemData>>.Enumerator enumerator = BagItems.GetEnumerator();
            while (enumerator.MoveNext())
            {
                List<ItemData> datas = enumerator.Current.Value;
                foreach (var kv in datas)
                {
                    if (kv.cSVItemData.type_id != itemType)
                    {
                        continue;
                    }
                    if (kv.BoxId == (int)ignoreType1 || kv.BoxId == (int)ignoreType2)
                    {
                        continue;
                    }
                    bool can = true;
                    if (null != filters)
                    {
                        for (int i = 0; i < filters.Count; i++)
                        {
                            if (!can)
                            {
                                break;
                            }
                            can &= filters[i].Invoke(kv);
                        }
                    }
                    if (can)
                    {
                        itemDatas.Add(kv);
                    }
                }
            }
            return itemDatas;
        }

        /// <summary>
        /// reqireType 1:好感度 2:家族
        /// </summary>
        /// <param name="reqireType"></param>
        /// <param name="infoIds"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public List<ItemData> GetItemDatasByItemInfoIds(int reqireType, IList<uint> infoIds, IList<Func<ItemData, bool>> filters = null)
        {
            List<ulong> uuids = new List<ulong>();
            List<ItemData> res = new List<ItemData>();

            for (int i = 0; i < infoIds.Count; i++)
            {
                if (InfoItems.ContainsKey(infoIds[i]))
                {
                    HashSet<ulong> ids = InfoItems[infoIds[i]];
                    foreach (var item in ids)
                    {
                        uuids.Add(item);
                    }
                }
            }
            foreach (var item in uuids)
            {
                ItemData itemData = null;
                if (reqireType == 1)
                {
                    itemData = GetGiftItemDatas(item);
                }
                else if (reqireType == 2)
                {
                    itemData = GetFamilyItemDatas(item);
                }
                if (itemData == null)
                    continue;
                res.Add(itemData);
            }
            for (int i = res.Count - 1; i >= 0; --i)
            {
                bool reached = true;
                for (int j = 0; j < filters.Count; j++)
                {
                    if (!reached)
                    {
                        break;
                    }
                    reached &= filters[j].Invoke(res[i]);
                }
                if (!reached)
                {
                    res.RemoveAt(i);
                }
            }
            return res;
        }

        public int GetItemMaxCountExceptSafeBox(uint itemId)
        {
            int count = 0;
            List<ItemData> itemDatas_filter = new List<ItemData>();
            foreach (var item in BagItems)
            {
                if (item.Key == 5 || item.Key == 6)
                {
                    continue;
                }
                itemDatas_filter.AddRange(item.Value);
            }
            foreach (var item in itemDatas_filter)
            {
                if (item.Id == itemId)
                {
                    count += (int)item.Count;
                }
            }
            return count;
        }

        public int GetCurMainBagBoxLevelUpCost()
        {
            List<List<uint>> str = CSVBoxType.Instance.GetConfData((uint)(curBoxId)).uarray2_value;
            int index = (int)Sys_Bag.Instance.Grids[(uint)curBoxId];
            return (int)str[index + 1][1];
        }

        public int GetCurSafeBoxLevelUpCost()
        {
            List<List<uint>> str = CSVBoxType.Instance.GetConfData(5).uarray2_value;
            int index = (int)Sys_Bag.Instance.Grids[5];
            return (int)str[index + 1][1];
        }

        public int GetSafeBoxTabCount()
        {
            List<List<uint>> str = CSVBoxType.Instance.GetConfData(5).uarray2_value;
            return str.Count;
        }

        public int GetBoxMaxCeilCount(uint boxid)
        {
            List<List<uint>> str = CSVBoxType.Instance.GetConfData(boxid).uarray2_value;
            int index = (int)Sys_Bag.Instance.Grids[boxid];
            return (int)str[index][0];
        }

        public List<ItemData> GetBattleUseItem(uint battle_use)
        {
            List<uint> itemTbs_Normal = CombatManager.Instance.m_BattleTypeTb.normal_medic;
            
            List<uint> itemTbs_Special = CombatManager.Instance.m_BattleTypeTb.special_medic;

            bool repeat = false;
            
            for (int i = 0; i < itemTbs_Normal.Count; i++)
            {
                if (itemTbs_Special.Contains(itemTbs_Normal[i]))
                {
                    repeat = true;
                }
            }

            for (int i = 0; i < itemTbs_Special.Count; i++)
            {
                if (itemTbs_Normal.Contains(itemTbs_Special[i]))
                {
                    repeat = true;
                }
            }

            if (repeat)
            {
                DebugUtil.LogErrorFormat("战斗内道具使用--道具类型有重复");
            }
            
            List<ItemData> itemDatas = new List<ItemData>();
            if (battle_use == 1)
            {
                if (null != itemTbs_Normal)
                {
                    foreach (var item in itemTbs_Normal)
                    {
                        itemDatas.AddRange(GetItemDatasByItemType(item));
                    }
                }
            }
            else if (battle_use == 2)
            {
                if (null != itemTbs_Special)
                {
                    foreach (var item in itemTbs_Special)
                    {
                        itemDatas.AddRange(GetItemDatasByItemType(item));
                    }
                }
            }
            return itemDatas;
        }

        public uint GetBagEmptyGrid(BoxIDEnum _boxid)
        {
            uint count = 0;
            uint boxId = (uint)_boxid;
            int maxIndex = CSVBoxType.Instance.GetConfData(boxId).uarray2_value.Count;
            uint allGridCounts = CSVBoxType.Instance.GetConfData(boxId).uarray2_value[maxIndex - 1][0];
            List<ItemData> datas = Sys_Bag.Instance.BagItems[(int)boxId];
            count = allGridCounts - (uint)datas.Count;
            return count;
        }

        public string GetValueFormat(long value)
        {
            string res = string.Empty;
            if (value >= 1000000000000)
            {
                int v1 = (int)(value / 1000000000000);
                res = string.Format(CSVLanguage.Instance.GetConfData(1000949).words, v1.ToString());
            }
            else if (value >= 10000000000)
            {
                int v2 = (int)(value / 100000000);
                res = string.Format(CSVLanguage.Instance.GetConfData(1000948).words, v2.ToString());
            }
            else if (value >= 100000000)
            {
                decimal v3 = Math.Round(((decimal)value / (decimal)100000000), 9);
                string _res = v3.ToString(4);
                res = string.Format(CSVLanguage.Instance.GetConfData(1000948).words, _res);
            }
            else if (value >= 10000000)
            {
                int v5 = (int)(value / 10000);
                res = string.Format(CSVLanguage.Instance.GetConfData(1000946).words, v5.ToString());
            }
            else
            {
                res = string.Format(value.ToString());
            }
            return res;
        }

        #endregion

        #region 使用提示逻辑
        private void PushUseItem(ItemData item)
        {
            if (GetItemMapUseState(item) == 0)
            {
                return;
            }
            if (item.cSVItemData.type_id == (int)EItemType.Crystal)
            {
                listUseItems.Add(item.Uuid);
            }
            else if (item.cSVItemData.type_id != (int)EItemType.Equipment)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    listUseItems.Add(item.Uuid);
                }
            }
            else
            {
                if (!listUseItems.Contains(item.Uuid))
                {
                    listUseItems.Add(item.Uuid);
                }
            }
        }

        private void DelUseItem(ItemData item)
        {
            if (listUseItems.Contains(item.Uuid))
            {
                listUseItems.Remove(item.Uuid);
            }
        }

        public ItemData GetUseItem(ref uint clearSame)
        {
            if (listUseItems.Count > 0)
            {
                ItemData temp = GetItemDataByUuid(listUseItems[0]);

                if (temp == null)
                {
                    listUseItems.RemoveAt(0);
                    return null;
                }
                if (temp.cSVItemData == null)
                {
                    return null;
                }
                if (temp.cSVItemData.type_id == (int)EItemType.Equipment)
                {
                    listUseItems.RemoveAt(0);
                }
                else if (temp.cSVItemData.type_id == (int)EItemType.Ornament)
                {
                    CSVOrnamentsUpgrade.Data tempOrnamentData = CSVOrnamentsUpgrade.Instance.GetConfData(temp.Id);
                    List<int> deleteList = new List<int> { 0 };
                    for (int i = 1; i < listUseItems.Count; i++)
                    {
                        var curItemData = GetItemDataByUuid(listUseItems[i]);
                        if (curItemData.cSVItemData.type_id == (int)EItemType.Ornament)
                        {
                            CSVOrnamentsUpgrade.Data curOrnamentData = CSVOrnamentsUpgrade.Instance.GetConfData(curItemData.Id);
                            if (tempOrnamentData.type == curOrnamentData.type)
                            {
                                deleteList.Add(i);
                                if (temp.ornament.Score <= curItemData.ornament.Score)
                                {
                                    temp = curItemData;
                                    tempOrnamentData = curOrnamentData;
                                }
                            }
                        }
                    }
                    for (int i = deleteList.Count - 1; i >= 0; i--)
                    {
                        listUseItems.RemoveAt(deleteList[i]);
                    }
                }
                else
                {
                    if (clearSame == 1)
                    {
                        for (int i = listUseItems.Count - 1; i >= 0; i--)
                        {
                            if (listUseItems[i] == temp.Uuid)
                            {
                                listUseItems.RemoveAt(i);
                            }
                        }
                        clearSame = 0;
                        return null;
                    }
                    else
                    {
                        listUseItems.RemoveAt(0);
                    }
                }
                return temp;
            }
            else if (clearSame == 1)
            {
                clearSame = 0;
                return null;
            }

            return null;
        }

        private ItemData GetFirstCrystalCanEquip()
        {
            if (BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out List<ItemData> itemDatas))
            {
                for (int i = 0; i < itemDatas.Count; i++)
                {
                    if (itemDatas[i].cSVItemData.type_id == (int)EItemType.Crystal)
                    {
                        return itemDatas[i];
                    }
                }
            }
            return null;
        }


        public void CheckAnyMainBagFull()
        {
            int visualableGridCount = 0;
            bAnyMainBagFull = false;
            for (int i = 1; i < 5; i++)
            {
                List<List<uint>> str = CSVBoxType.Instance.GetConfData((uint)i).uarray2_value;
                if (Grids.ContainsKey((uint)i))
                {
                    int index = (int)Grids[(uint)i];
                    if (index >= str.Count)
                    {
                        index = str.Count - 1;
                    }
                    visualableGridCount = (int)str[index][0];

                    if (!fullState.TryGetValue(i, out bool full))
                    {
                        fullState.Add(i, false);
                    }

                    if (visualableGridCount == GetBagVaildGridCountByBoxId((BoxIDEnum)i))
                    {
                        eventEmitter.Trigger<int, bool>(EEvents.OnUpdateBagFullState, i, true);
                        bAnyMainBagFull = true;
                        fullState[i] = true;
                    }
                    else if (visualableGridCount > GetBagVaildGridCountByBoxId((BoxIDEnum)i))
                    {
                        eventEmitter.Trigger<int, bool>(EEvents.OnUpdateBagFullState, i, false);
                        fullState[i] = false;
                    }
                }
            }
        }

        public bool BoxFull(BoxIDEnum boxIDEnum)
        {
            if ((uint)boxIDEnum > 4)
            {
                return false;
            }
            int visualableGridCount = 0;
            List<List<uint>> str = CSVBoxType.Instance.GetConfData((uint)boxIDEnum).uarray2_value;
            int index = (int)Sys_Bag.Instance.Grids[(uint)boxIDEnum];
            visualableGridCount = (int)str[index][0];
            return visualableGridCount == GetBagVaildGridCountByBoxId((BoxIDEnum)boxIDEnum);
        }

        //非装备 普通道具
        public bool UseItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return false;
            }
            if (Sys_Role.Instance.Role.Level < itemData.cSVItemData.use_lv)
            {
                string content = string.Format(CSVLanguage.Instance.GetConfData(1000967).words, itemData.cSVItemData.use_lv.ToString());
                Sys_Hint.Instance.PushContent_Normal(content);
                return false;
            }
            if (itemData.cSVItemData.FunctionOpenId != 0)
            {
                if (!Sys_FunctionOpen.Instance.IsOpen(itemData.cSVItemData.FunctionOpenId, true))
                    return false;
            }

            return ItemUse.ParseItem(itemData);
        }

        private void OnCompletedFunctionOpen(Sys_FunctionOpen.FunctionOpenData functionOpenData)
        {
            uint funId = functionOpenData.id;
            Dictionary<int, List<ItemData>>.Enumerator enumerator = BagItems.GetEnumerator();
            while (enumerator.MoveNext())
            {
                List<ItemData> datas = enumerator.Current.Value;
                for (int i = 0, length = datas.Count; i < length; ++i)
                {
                    ItemData itemData = datas[i];
                    if (itemData.cSVItemData.type_id == (int)EItemType.Equipment)
                        continue;
                    if (itemData.cSVItemData.quick_use == 1)
                    {
                        if (Sys_Role.Instance.Role.Level >= itemData.cSVItemData.use_lv)
                        {
                            if (itemData.cSVItemData.FunctionOpenId == funId && Sys_FunctionOpen.Instance.IsOpen(itemData.cSVItemData.FunctionOpenId))
                            {
                                PushUseItem(itemData);
                            }
                        }
                    }
                }
            }
        }

        private void OnPlayerLevelChanged()
        {
            Dictionary<int, List<ItemData>>.Enumerator enumerator = BagItems.GetEnumerator();
            while (enumerator.MoveNext())
            {
                List<ItemData> datas = enumerator.Current.Value;
                for (int i = 0, length = datas.Count; i < length; ++i)
                {
                    ItemData itemData = datas[i];
                    if (itemData.cSVItemData.type_id == (int)EItemType.Equipment)
                        continue;
                    if (itemData.cSVItemData.quick_use == 1)
                    {
                        if (Sys_Role.Instance.Role.Level == itemData.cSVItemData.use_lv)
                        {
                            if (itemData.cSVItemData.FunctionOpenId == 0)
                            {
                                PushUseItem(itemData);
                            }
                            else
                            {
                                if (Sys_FunctionOpen.Instance.IsOpen(itemData.cSVItemData.FunctionOpenId))
                                {
                                    PushUseItem(itemData);
                                }
                            }
                        }
                    }
                }
            }
        }

        //public void CheckUseItem()
        //{
        //    Dictionary<int, List<ItemData>>.Enumerator enumerator = BagItems.GetEnumerator();
        //    while (enumerator.MoveNext())
        //    {
        //        List<ItemData> datas = enumerator.Current.Value;
        //        for (int i = 0, length = datas.Count; i < length; ++i)
        //        {
        //            ItemData itemData = datas[i];
        //            if (itemData.cSVItemData.type_id == (int)EItemType.Equipment)
        //                continue;
        //            if (itemData.cSVItemData.quick_use == 1)
        //            {
        //                if (Sys_Role.Instance.Role.Level >= itemData.cSVItemData.use_lv)
        //                {
        //                    if (itemData.cSVItemData.FunctionOpenId == 0)
        //                    {
        //                        PushUseItem(itemData);
        //                    }
        //                    else
        //                    {
        //                        if (Sys_FunctionOpen.Instance.IsOpen(itemData.cSVItemData.FunctionOpenId))
        //                        {
        //                            PushUseItem(itemData);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion

        public bool CurrencyFrozened(uint currencyId)
        {
            return m_FrozenCurrencyInfos.ContainsKey(currencyId);
        }

        public ulong GetAllFrozenNum(uint currencyId)
        {
            ulong res = 0;
            if (m_FrozenCurrencyInfos.TryGetValue(currencyId, out List<FrozenCurrencyInfo> infos))
            {
                for (int i = 0, count = infos.Count; i < count; i++)
                {
                    res += infos[i].Num;
                }
            }
            return res;
        }

        //type:0 表示查询冻结最大值   1：最小值
        public uint GetFrozenDay(uint currencyId, int type)
        {
            uint freezeTime = 0;
            if (m_FrozenCurrencyInfos.TryGetValue(currencyId, out List<FrozenCurrencyInfo> infos))
            {
                freezeTime = infos[0].UnfreezeTime;
                for (int i = 0, count = infos.Count; i < count; i++)
                {
                    if (type == 0)
                    {
                        freezeTime = (uint)Mathf.Max((int)freezeTime, (int)infos[i].UnfreezeTime);
                    }
                    else if (type == 1)
                    {
                        freezeTime = (uint)Mathf.Min((int)freezeTime, (int)infos[i].UnfreezeTime);
                    }
                }
            }
            uint currentTime = Sys_Time.Instance.GetServerTime();
            if (currentTime <= freezeTime)
            {
                uint remainTime = freezeTime - currentTime;
                return remainTime % 86400u == 0 ? remainTime / 86400u : remainTime / 86400u + 1;
            }
            return 0;
        }

        public ulong GetUnFrozenNum(uint currencyId)
        {
            uint freezeTime = 0;
            ulong num = 0;
            if (m_FrozenCurrencyInfos.TryGetValue(currencyId, out List<FrozenCurrencyInfo> infos))
            {
                freezeTime = infos[0].UnfreezeTime;
                for (int i = 0, count = infos.Count; i < count; i++)
                {
                    freezeTime = (uint)Mathf.Min((int)freezeTime, (int)infos[i].UnfreezeTime);
                }
                for (int i = 0, count = infos.Count; i < count; i++)
                {
                    if (Sys_Time.IsServerSameDay(infos[i].UnfreezeTime, freezeTime))
                    {
                        num += infos[i].Num;
                    }
                }
            }
            return num;
        }

        /// <summary>
        /// needCount 为 需要的个数
        /// </summary>
        /// <param name="eCurrencyType"></param>
        /// <param name="needCount"></param>
        public void TryOpenExchangeCoinUI(ECurrencyType eCurrencyType, long needCount)
        {
            ExchangeCoinParm exchangeCoinParm = new ExchangeCoinParm();
            if (eCurrencyType == ECurrencyType.GoldCoin)//金币
            {
                long shortCount = needCount - Sys_Bag.Instance.GetItemCount(2);
                long needFromCount = 0;
                if (shortCount % 100 == 0)
                {
                    needFromCount = shortCount / 100;
                }
                else
                {
                    needFromCount = shortCount / 100 + 1;
                }
                if (Sys_Bag.Instance.GetItemCount(1) < needFromCount)
                {
                    if (UIManager.IsOpen(EUIID.UI_Mall))
                    {
                        Sys_Mall.Instance.eventEmitter.Trigger(Sys_Mall.EEvents.OnTelCharge);
                    }
                    else
                    {
                        if (!Sys_FunctionOpen.Instance.IsOpen(40105, true))
                        {
                            return;
                        }
                        MallPrama param = new MallPrama();
                        param.mallId = 101u;
                        param.isCharge = true;
                        UIManager.OpenUI(EUIID.UI_Mall, false, param);
                    }
                }
                else
                {
                    exchangeCoinParm.ExchangeType = (uint)eCurrencyType;
                    exchangeCoinParm.needCount = shortCount;
                    UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, exchangeCoinParm);
                }
            }
            else if (eCurrencyType == ECurrencyType.SilverCoin)
            {
                long shortCount = needCount - Sys_Bag.Instance.GetItemCount(3);
                long needFromCount1 = 0;
                long needFromCount2 = 0;
                if (shortCount % 100 == 0)
                {
                    needFromCount1 = shortCount / 100;
                    needFromCount2 = shortCount / 10000;
                }
                else
                {
                    needFromCount1 = shortCount / 100 + 1;
                    needFromCount2 = shortCount / 10000 + 1;
                }
                if (Sys_Bag.Instance.GetItemCount(2) < needFromCount1 && Sys_Bag.Instance.GetItemCount(1) < needFromCount2)
                {
                    if (UIManager.IsOpen(EUIID.UI_Mall))
                    {
                        Sys_Mall.Instance.eventEmitter.Trigger(Sys_Mall.EEvents.OnTelCharge);
                    }
                    else
                    {
                        if (!Sys_FunctionOpen.Instance.IsOpen(40105, true))
                        {
                            return;
                        }
                        MallPrama param = new MallPrama();
                        param.mallId = 101u;
                        param.isCharge = true;

                        UIManager.OpenUI(EUIID.UI_Mall, false, param);
                    }
                }
                else
                {
                    exchangeCoinParm.ExchangeType = (uint)eCurrencyType;
                    exchangeCoinParm.needCount = shortCount;
                    UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, exchangeCoinParm);
                }
            }
            else if (eCurrencyType == ECurrencyType.Diamonds)
            {
                if (UIManager.IsOpen(EUIID.UI_Mall))
                {
                    Sys_Mall.Instance.eventEmitter.Trigger(Sys_Mall.EEvents.OnTelCharge);
                }
                else
                {
                    if (!Sys_FunctionOpen.Instance.IsOpen(40105, true))
                    {
                        return;
                    }
                    MallPrama param = new MallPrama();
                    param.mallId = 101u;
                    param.isCharge = true;

                    UIManager.OpenUI(EUIID.UI_Mall, false, param);
                }
            }
        }

        /// <summary>
        /// 获取数量限制道具当前已使用数量
        /// </summary>
        /// <param name="itemId"></param>
        public uint GetDayLimitItemUsedCount(uint itemId)
        {
            if (dayLimitItemUsedDic.ContainsKey(itemId))
                return dayLimitItemUsedDic[itemId];
            return 0;
        }


        /// <summary>
        /// 0: 右测按钮全隐藏    1:右侧只显示使用(替换按钮)  2:右侧按钮显隐走原来逻辑
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        public int GetItemMapUseState(ItemData itemData)
        {
            if (itemData.cSVItemData == null)
            {
                return 0;
            }
            if (itemData.cSVItemData.banMap != null && itemData.cSVItemData.banMap.Contains(Sys_Map.Instance.CurMapId))
            {
                if (itemData.cSVItemData.useMap == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 2;
            }
        }

        public void ProcessCeilGridClick(CeilGrid bagCeilGrid, bool isShowOpBtn = true)
        {
            uint typeId = bagCeilGrid.mItemData.cSVItemData.type_id;
            //装备道具,单独处理
            if (typeId == (uint)EItemType.Equipment)
            {
                EquipTipsData tipData = new EquipTipsData();
                tipData.equip = bagCeilGrid.mItemData;
                tipData.isShowOpBtn = isShowOpBtn;
                if (bagCeilGrid.eSource == CeilGrid.ESource.e_SafeBox)
                    tipData.isShowLock = false;
                UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
            }
            else if (typeId == (uint)EItemType.Crystal)
            {
                CrystalTipsData crystalTipsData = new CrystalTipsData();
                crystalTipsData.itemData = bagCeilGrid.mItemData;
                crystalTipsData.bShowOp = isShowOpBtn;
                crystalTipsData.bShowCompare = true;
                crystalTipsData.bShowDrop = true;
                crystalTipsData.bShowSale = true;
                UIManager.OpenUI(EUIID.UI_Tips_ElementalCrystal, false, crystalTipsData);
            }
            else if (typeId == (uint)EItemType.Ornament)
            {
                OrnamentTipsData tipData = new OrnamentTipsData();
                tipData.equip = bagCeilGrid.mItemData;
                tipData.isShowOpBtn = isShowOpBtn;
                if (bagCeilGrid.eSource == CeilGrid.ESource.e_SafeBox)
                    tipData.isShowLock = false;
                tipData.sourceUiId = EUIID.UI_Bag;
                UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
            }
            else if (typeId == (uint)EItemType.PetEquipment)
            {
                PetEquipTipsData petEquipTipsData = new PetEquipTipsData();
                petEquipTipsData.openUI = EUIID.UI_Bag;
                petEquipTipsData.petEquip = bagCeilGrid.mItemData;
                petEquipTipsData.isCompare = false;
                petEquipTipsData.isShowOpBtn = isShowOpBtn;
                if (bagCeilGrid.eSource == CeilGrid.ESource.e_SafeBox)
                    petEquipTipsData.isShowLock = false;
                UIManager.OpenUI(EUIID.UI_Tips_PetMagicCore, false, petEquipTipsData);
            }
            else
            {
                //Sys_Bag.Instance.enablePropMessagePenetrationClick = true;
                //if (UIManager.IsOpen(EUIID.UI_Prop_Message))
                //{
                //    PropMessageParam propParam = new PropMessageParam();
                //    propParam.itemData = bagCeilGrid.mItemData;
                //    propParam.needShowInfo = true;
                //    propParam.needShowMarket = true;
                //    Sys_Bag.Instance.eventEmitter.Trigger<PropMessageParam>(Sys_Bag.EEvents.OnUpdatePropItemMessageParm, propParam);
                //}
                //else
                //{
                //    PropMessageParam propParam = new PropMessageParam();
                //    propParam.itemData = bagCeilGrid.mItemData;
                //    propParam.needShowInfo = true;
                //    propParam.needShowMarket = true;
                //    UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
                //}
                PropMessageParam propParam = new PropMessageParam();
                propParam.itemData = bagCeilGrid.mItemData;
                propParam.needShowInfo = true;
                propParam.needShowMarket = true;
                propParam.showOpButton = isShowOpBtn;
                propParam.sourceUiId = EUIID.UI_Bag;
                UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
            }
        }

        /// <summary>
        /// 变身卡根据是否可用做排序处理
        /// </summary>
        public List<ItemData> GetSortTransfigurationCard(List<ItemData> itemList)
        {
            var canUseList = new List<ItemData>();
            var unUseList = new List<ItemData>();
            for (int i = 0; i < itemList.Count; i++)
            {
                var curItem = itemList[i];
                if (Sys_Transfiguration.Instance.GetCurUseShapeShiftData().CanUseChangeCard(curItem.Id))
                {
                    bool flag = false;
                    for (int j = 0; j < canUseList.Count; j++)
                    {
                        var curCanUseItem = canUseList[j];
                        if(curItem.cSVItemData.id == curCanUseItem.cSVItemData.id)
                        {
                            flag = true;
                            canUseList.Insert(j + 1, curItem);
                            break;
                        }
                    }
                    if (!flag)
                    {
                        canUseList.Add(curItem);
                    }
                }
                else
                {
                    unUseList.Add(curItem);
                }
            }
            canUseList.AddRange(unUseList);
            return canUseList;
        }
    }
}



