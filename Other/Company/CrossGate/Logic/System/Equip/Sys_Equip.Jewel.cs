using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public partial class Sys_Equip : SystemModuleBase<Sys_Equip>
    {
        public uint InlaySlotPos = 0;

        private EJewelType _curJewelType = EJewelType.All;
        public EJewelType CurJewelType{
            get {
                return _curJewelType;
            }
            set {
                _curJewelType = value;
            }
        }

        public int SelectJewelGroupIndex = 0;

        public class JewelGroupData {
            public uint itemId;
            public uint count;
        }

        public void OnQuickComposeReq(ulong equipUId, uint slotPos, List<uint> jewelLvs, List<uint> jewelNums, uint buyNum, uint targetLv)
        {
            CmdItemQuickComposeJewelReq req = new CmdItemQuickComposeJewelReq();
            //req.JewelUUID = jewelUId;
            req.Equipuuid = equipUId;
            req.SlotIndex = slotPos;
            req.Jewellevel.AddRange(jewelLvs);
            req.Num.Add(jewelNums);
            req.Buynum = buyNum;
            req.Tolevel = targetLv;

            NetClient.Instance.SendMessage((ushort)CmdItem.QuickComposeJewelReq, req);
        }

        private void OnQuickComposeRes(NetMsg msg)
        {
            CmdItemQuickComposeJewelRes res = NetMsgUtil.Deserialize<CmdItemQuickComposeJewelRes>(CmdItemQuickComposeJewelRes.Parser, msg);
            eventEmitter.Trigger(EEvents.OnJewelNtfQuickCompose);
            //UIManager.CloseUI(EUIID.UI_Jewel_Upgrade);
            //Debug.LogError("OnQuickComposeRes");
        }

        public void OnJewelLevelAttrSelectReq(int index)
        {
            CmdItemJewelLevelAttrSelectReq req = new CmdItemJewelLevelAttrSelectReq();
            req.Uuid = CurOpEquipUId;
            req.AttrIndex = (uint) index;
            NetClient.Instance.SendMessage((ushort)CmdItem.JewelLevelAttrSelectReq, req);
        }

        private void OnJewelLevelAttrSelectRes(NetMsg msg)
        {
            CmdItemJewelLevelAttrSelectRes res = NetMsgUtil.Deserialize<CmdItemJewelLevelAttrSelectRes>(CmdItemJewelLevelAttrSelectRes.Parser, msg);
            ItemData itemEquip = Sys_Bag.Instance.GetItemDataByUuid(CurOpEquipUId);
            itemEquip.Equip.JewelLevleAttrSelect = res.AttrIndex;
        }

        /// <summary>
        /// 判断装备是否有空格,且有可镶嵌类型
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool IsSlotCanInlay(ItemData item)
        {
            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(item.Id);

            List<uint> jewelInlayedIds = new List<uint>();
            for (int i = 0; i < item.Equip.JewelinfoId.Count; ++i)
            {
                if (item.Equip.JewelinfoId[i] != 0)
                    jewelInlayedIds.Add(item.Equip.JewelinfoId[i]);          
            }

            //如果没有可镶嵌位置,返回false
            if (jewelInlayedIds.Count == equipInfo.jewel_number)
            {
                return false;
            }

            List<uint> jewelTypes = new List<uint>(equipInfo.jewel_type);
            for (int i = 0; i < jewelInlayedIds.Count; ++i)
            {
                CSVJewel.Data jewelInfoData = CSVJewel.Instance.GetConfData(jewelInlayedIds[i]);
                jewelTypes.Remove(jewelInfoData.jewel_type);
            }

            for (int i = 0; i < jewelTypes.Count; ++i)
            {
                List<ulong> jewelList = GetJewelList((EJewelType)jewelTypes[i]);
                for (int j = 0; j < jewelList.Count; ++j)
                {
                    ItemData jewelItem = GetItemData(jewelList[j]);
                    CSVJewel.Data jewelInfoData = CSVJewel.Instance.GetConfData(jewelItem.Id);
                    if (jewelInfoData.level <= equipInfo.jewel_level)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //判单宝石可否镶嵌在装备
        public bool CheckJewelCanInlay(uint jewelId)
        {
            ItemData itemEquip = GetItemData(CurOpEquipUId);
            if (null == itemEquip)
                return false;

            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(itemEquip.Id);

            List<uint> jewelInlayedIds = new List<uint>();
            for (int i = 0; i < itemEquip.Equip.JewelinfoId.Count; ++i)
            {
                if (itemEquip.Equip.JewelinfoId[i] != 0)
                    jewelInlayedIds.Add(itemEquip.Equip.JewelinfoId[i]);
            }

            //如果没有可镶嵌位置,返回false
            if (jewelInlayedIds.Count == equipInfo.jewel_number)
            {
                return false;
            }

            //可镶嵌的类型
            List<uint> jewelTypes = new List<uint>(equipInfo.jewel_type);
            for (int i = 0; i < jewelInlayedIds.Count; ++i)
            {
                CSVJewel.Data jewelInfoData = CSVJewel.Instance.GetConfData(jewelInlayedIds[i]);
                jewelTypes.Remove(jewelInfoData.jewel_type);
            }

            CSVJewel.Data jewelInfo = CSVJewel.Instance.GetConfData(jewelId);
            if (null == jewelInfo)
                return false;

            if (jewelTypes.IndexOf(jewelInfo.jewel_type) >= 0)
                return true;

            return false;
        }

        /// <summary>
        /// 判断是否有高等级宝石
        /// </summary>
        /// <param name="jewelUId"></param>
        /// <returns></returns>
        public bool IsJewelCanReplaceHigh(ItemData equipItem, uint jewelInfoId)
        {
            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(equipItem.Id);
            //ItemData jewelItem = GetItemData(jewelUId);

            CSVJewel.Data jewelInfoData = CSVJewel.Instance.GetConfData(jewelInfoId);
            List<ulong> jewelList = GetJewelList((EJewelType)jewelInfoData.jewel_type);
            for (int i = 0; i < jewelList.Count; ++i)
            {
                ItemData jewelTempItem = GetItemData(jewelList[i]);
                CSVJewel.Data jewelTempInfoData = CSVJewel.Instance.GetConfData(jewelTempItem.Id);
                if (jewelTempInfoData.level > jewelInfoData.level && jewelTempInfoData.level <= equipInfo.jewel_level)
                    return true;
            }

            return false;
        }


        public bool IsJewelBagCanImprove(ItemData equipItem, uint jewelInfoId)
        {
            bool isCompound = false;

            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(equipItem.Id);
            CSVJewel.Data jewelData = CSVJewel.Instance.GetConfData(jewelInfoId);

            if (jewelData != null && equipInfo != null)
            {
                if (jewelData.level >= equipInfo.jewel_level)
                    return false;

                if (jewelData.num != 0) //不是最大等级
                {
                    //同类型同等级宝石列表
                    uint count = 0;

                    List<ItemData> bagList = null;
                    if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out bagList))
                    {
                        for (int i = 0; i < bagList.Count; ++i)
                        {
                            if (bagList[i].cSVItemData.type_id == (uint)EItemType.Jewel)
                            {
                                CSVJewel.Data tempJewel = CSVJewel.Instance.GetConfData(bagList[i].Id);
                                if (tempJewel != null
                                    && tempJewel.jewel_type == jewelData.jewel_type
                                    && tempJewel.level == jewelData.level)
                                {
                                    count += bagList[i].Count;
                                }
                            }
                        }
                    }

                    if (count + 1 >= jewelData.num) //加上自身
                    {
                        isCompound = true;
                    }
                }
            }
            else
            {
                Debug.LogErrorFormat("not found jewel infoId={0}", jewelInfoId);
            }

            return isCompound;
        }

        /// <summary>
        /// 判断宝石是否可以升级
        /// </summary>
        /// <param name="_jewelInfoId"></param>
        /// <returns></returns>
        public bool IsJewelCanImprove(ItemData equipItem, uint jewelInfoId)
        {
            bool isCompound = false;

            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(equipItem.Id);
            CSVJewel.Data jewelData = CSVJewel.Instance.GetConfData(jewelInfoId);

            if (jewelData != null && equipInfo != null)
            {
                //规则，只要没达到最大可镶嵌的最大等级，就可以升级
                return jewelData.level < equipInfo.jewel_level;
                //if (jewelData.level >= equipInfo.jewel_level)
                //    return false;

                //if (jewelData.num != 0) //不是最大等级
                //{
                //    //同类型同等级宝石列表
                //    uint count = 0;

                //    List<ItemData> bagList = null;
                //    if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out bagList))
                //    {
                //        for (int i = 0; i < bagList.Count; ++i)
                //        {
                //            if (bagList[i].cSVItemData.type_id == (uint)EItemType.Jewel)
                //            {
                //                CSVJewel.Data tempJewel = CSVJewel.Instance.GetConfData(bagList[i].Id);
                //                if (tempJewel != null
                //                    && tempJewel.jewel_type == jewelData.jewel_type
                //                    && tempJewel.level == jewelData.level)
                //                {
                //                    count += bagList[i].Count;
                //                }
                //            }
                //        }
                //    }

                //    if (count + 1 >= jewelData.num) //加上自身
                //    {
                //        isCompound = true;
                //    }
                //}
            }
            else
            {
                Debug.LogErrorFormat("not found jewel infoId={0}", jewelInfoId);
            }

            return isCompound;
        }

        public List<ulong> GetJewelList(EJewelType _JewelType)
        {
            List<ulong> IdsList = new List<ulong>();

            List<ItemData> gemList = new List<ItemData>();

            List<ItemData> bagList = null;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out bagList))
            {
                for (int i = 0; i < bagList.Count; ++i)
                {
                    if (bagList[i].cSVItemData.type_id == (uint)EItemType.Jewel)
                    {
                        CSVJewel.Data jewelData = CSVJewel.Instance.GetConfData(bagList[i].Id);
                        if (jewelData != null)
                        {
                            if (_JewelType == EJewelType.All) // 全部类型
                            {
                                gemList.Add(bagList[i]);
                            }
                            else if (_JewelType == (EJewelType)jewelData.jewel_type) //同类型 
                            {
                                gemList.Add(bagList[i]);
                            }
                        }
                    }
                }
            }

            gemList.Sort((var1, var2) => var1.Id.CompareTo(var2.Id));

            for (int i = 0; i < gemList.Count; ++i)
            {
                IdsList.Add(gemList[i].Uuid);
            }

            return IdsList;
        }

        public List<JewelGroupData> GetJewelTotalList(EJewelType _JewelType)
        {
            List<JewelGroupData> jewelList = new List<JewelGroupData>();

            Dictionary<uint, uint> jewelDict = new Dictionary<uint, uint>();

            List<ItemData> bagList = null;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out bagList))
            {
                for (int i = 0; i < bagList.Count; ++i)
                {
                    if (bagList[i].cSVItemData.type_id == (uint)EItemType.Jewel)
                    {
                        CSVJewel.Data jewelData = CSVJewel.Instance.GetConfData(bagList[i].Id);
                        if (jewelData != null)
                        {
                            if (_JewelType == EJewelType.All) // 全部类型
                            {
                                if (jewelDict.ContainsKey(bagList[i].Id))
                                    jewelDict[bagList[i].Id] += bagList[i].Count;
                                else
                                    jewelDict.Add(bagList[i].Id, bagList[i].Count);
                            }
                            else if (_JewelType == (EJewelType)jewelData.jewel_type) //同类型 
                            {
                                if (jewelDict.ContainsKey(bagList[i].Id))
                                    jewelDict[bagList[i].Id] += bagList[i].Count;
                                else
                                    jewelDict.Add(bagList[i].Id, bagList[i].Count);
                            }
                        }
                    }
                }
            }

            foreach (var data in jewelDict)
            {
                JewelGroupData jewel = new JewelGroupData();
                jewel.itemId = data.Key;
                jewel.count = data.Value;

                jewelList.Add(jewel);
            }

            jewelList.Sort((var1, var2) => var1.itemId.CompareTo(var2.itemId));

            return jewelList;
        }

        /// <summary>
        /// 获取宝石组: 宝石升级时，待合成的宝石，需要相同的合成一个组
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public JewelGroupData GetJewelGroupData(uint itemId)
        {
            uint count = 0;

            List<ItemData> bagList = null;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out bagList))
            {
                for (int i = 0; i < bagList.Count; ++i)
                {
                    if (bagList[i].Id == itemId)
                        count += bagList[i].Count;
                }
            }

            if (count != 0)
            {
                JewelGroupData temp = new JewelGroupData();
                temp.itemId = itemId;
                temp.count = count;
                return temp;
            }

            return null;
        }

        /// <summary>
        /// 判断装备是否镶嵌了宝石
        /// </summary>
        /// <param name="itemEquip"></param>
        /// <returns></returns>
        public bool IsInlayJewel(ItemData itemEquip)
        {
            bool isInlay = false;

            if (itemEquip.Equip.JewelinfoId != null)
            {
                for (int i = 0; i < itemEquip.Equip.JewelinfoId.Count; ++i)
                {
                    if (itemEquip.Equip.JewelinfoId[i] == 0u)
                        continue;

                    isInlay = true;
                    break;
                }
            }

            return isInlay;
        }

        /// <summary>
        /// 判断装备替换时，是否有宝石
        /// </summary>
        /// <param name="oldEquip"></param>
        /// <param name="newEquip"></param>
        /// <returns></returns>
        public bool CanReplaceEquipJewels(ItemData oldEquip, ItemData newEquip)
        {
            int oldJewelCount = 0;
            bool isReplace = true;

            //判断新装备是否有宝石
            for (int i = 0; i < newEquip.Equip.JewelinfoId.Count; ++i)
            {
                if (newEquip.Equip.JewelinfoId[i] != 0u)
                {
                    isReplace = false;
                    break;
                }
            }

            if (!isReplace)
                return isReplace;


            CSVEquipment.Data newEquipInfo = CSVEquipment.Instance.GetConfData(newEquip.Id);

            for (int i = 0; i < oldEquip.Equip.JewelinfoId.Count; ++i)
            {
                if (oldEquip.Equip.JewelinfoId[i] == 0u)
                    continue;

                CSVJewel.Data jewelInfo = CSVJewel.Instance.GetConfData(oldEquip.Equip.JewelinfoId[i]);
                if (jewelInfo != null)
                {
                    //宝石等级判断
                    if (jewelInfo.level > newEquipInfo.jewel_level)
                    {
                        isReplace = false;
                        break;
                    }

                    //宝石类型
                    if (!newEquipInfo.jewel_type.Contains(jewelInfo.jewel_type))
                    {
                        isReplace = false;
                        break;
                    }

                    oldJewelCount++;
                }
            }

            //判断旧装备数量是否小于等于新装备孔数
            if (isReplace)
            {
                if (oldJewelCount > newEquipInfo.jewel_number)
                    isReplace = false;
            }

            return isReplace;
        }


        #region 宝石升级
        private uint _totalUpgradeCount = 0; //需要多少1级宝石升级
        private uint _leftUpgradeCount = 0;
        public uint LeftUpgradeCount { get { return _leftUpgradeCount; } }

        private uint _oneLevelId;
        public uint OneLevelId { get { return _oneLevelId; } }

        /// <summary>
        /// 次级宝石数据
        /// </summary>
        public class SubJewelData
        {
            public uint jewelId;
            public uint jewelUseNum;
            public JewelGroupData groupData;
        }
        private List<SubJewelData> listSubJewels = new List<SubJewelData>();

        /// <summary>
        /// 宝石转换为1级宝石数量
        /// </summary>
        /// <param name="jewelId"></param>
        /// <returns></returns>
        public uint TransJewelMinCount(uint jewelId)
        {
            CSVJewel.Data curJewel = CSVJewel.Instance.GetConfData(jewelId);
            if (curJewel.level == 1u)
            {
                return 1;
            }

            uint preJewelId = jewelId - 1u;
            CSVJewel.Data preJewel = CSVJewel.Instance.GetConfData(preJewelId);
            return  preJewel.num * TransJewelMinCount(preJewelId);
        }

        /// <summary>
        /// 计算宝石有多少个次级宝石
        /// </summary>
        /// <param name="jewelId"></param>
        /// <returns></returns>
        public List<uint> CalPreJewelList(uint jewelId)
        {
            List<uint> list = new List<uint>();
            uint mod = jewelId % 10;
            mod = mod == 0 ? 10 : mod;
            for (uint i = 1; i < mod; ++i)
            {
                list.Add(jewelId - i);
            }

            //设置一级宝石
            _oneLevelId = 0u;
            if (list.Count > 0)
                _oneLevelId = list[list.Count - 1];

            InitSubJewels(list);

            return list;
        }

        /// <summary>
        /// 需要多少个1级宝石升级
        /// </summary>
        public void CalNeedUpCount(uint jewelId, uint srcJewelId)
        {
            _leftUpgradeCount = _totalUpgradeCount = TransJewelMinCount(jewelId) - TransJewelMinCount(srcJewelId);
        }

        /// <summary>
        /// 初始化次级宝石数据
        /// </summary>
        /// <param name="subJewels"></param>
        private void InitSubJewels(List<uint> subJewels)
        {
            listSubJewels.Clear();
            for(int i = 0; i < subJewels.Count; ++i)
            {
                SubJewelData data = new SubJewelData();
                data.jewelId = subJewels[i];
                data.groupData = GetJewelGroupData(subJewels[i]);
                data.jewelUseNum = data.groupData != null ? CalUseNum(subJewels[i], data.groupData.count) : 0u;

                listSubJewels.Add(data);
            }
        }

        private uint CalUseNum(uint jewelId, uint totalNum)
        {
            uint useNum = 0;
            if (totalNum == 0)
                return useNum;

            //兑换1级的数量
            uint tempCount = TransJewelMinCount(jewelId);

            //个数
            uint count = _leftUpgradeCount / tempCount;
            count = count > totalNum ? totalNum : count;

            _leftUpgradeCount -= tempCount * count;

            return count;
        }

        /// <summary>
        /// 获得次级宝石数据
        /// </summary>
        /// <param name="jewelId"></param>
        /// <returns></returns>
        public SubJewelData GetSubJewelData(uint jewelId)
        {
            for (int i = 0; i < listSubJewels.Count; ++i)
                if (listSubJewels[i].jewelId == jewelId)
                    return listSubJewels[i];

            return null;
        }

        public List<SubJewelData> GetTotalSubJewelDatas()
        {
            return listSubJewels;
        }

        /// <summary>
        /// 减少leftcount
        /// </summary>
        /// <param name="count"></param>
        public void OnMinusLeftUpgradeCount(uint count)
        {
            _leftUpgradeCount -= count;

            //触发事件 
            eventEmitter.Trigger(EEvents.OnNotifyJewelUpgradeChange);
        }

        /// <summary>
        /// 增加leftcount
        /// </summary>
        /// <param name="count"></param>
        public void OnAddLeftUpgradeCount(uint count)
        {
            _leftUpgradeCount += count;
            //触发事件 
            eventEmitter.Trigger(EEvents.OnNotifyJewelUpgradeChange);
        }
        #endregion
    }
}

