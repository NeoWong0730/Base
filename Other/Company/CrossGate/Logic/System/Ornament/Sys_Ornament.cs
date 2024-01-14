using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Packet;
using Net;
using Table;
using Lib.Core;
using Logic;

namespace Logic
{
    public class Sys_Ornament : SystemModuleBase<Sys_Ornament>
    {
        /// <summary>
        /// 特殊词条ID
        /// </summary>
        public readonly uint SpecialAttrInfoId = 10999;

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        /// <summary> 饰品类型表主键 </summary>
        public List<uint> Types { get; private set; } = new List<uint>();
        //public List<uint> OrnamentIds { get; private set; } = new List<uint>();

        public Dictionary<uint, Dictionary<uint, List<uint>>> dictOrnament = new Dictionary<uint, Dictionary<uint, List<uint>>>();

        public List<uint> CanRecastIds { get; private set; } = new List<uint>();
        /// <summary> 升级基础对象的UID </summary>
        public ulong UpgradeTargetUuid = 0;
        /// <summary> 上一个重铸的UID </summary>
        public ulong LastRecastUuid = 0;
        /// <summary> 重铸中（防止操作过快） </summary>
        public bool isRecasting = false;
        /// <summary> 升级二次确认的本次登录不再提示 </summary>
        public bool UnShowUpgradeHint { get; private set; } = false;
        /// <summary> 重铸二次确认的本次登录不再提示 </summary>
        public bool UnShowRecastHint { get; private set; } = false;
        public enum EEvents : int
        {
            OnShowMsg,  //显示详情界面
            OnCompareBasicAttrValue, //基础属性对比
            OnUpgradeTargetSeleted, //选择界面点击确认选择
            OnUpgradeResBack,        //收到升级res回调
            OnUpgradeAllResBack,    //收到全部升级res回调
            OnRecastResBack,        //收到重铸res回调
            OnRecastLockResBack,    //收到重铸锁定res回调
        }
        #region 系统函数
        public override void Init()
        {
            InitCSVData();
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdItem.OrnamentFitRes, OnOrnamentFitRes, CmdItemOrnamentFitRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdItem.OrnamentLvUpRes, OnOrnamentLvUpRes, CmdItemOrnamentLvUpRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdItem.OrnamentOneKeyLvUpRes, OrnamentAllLvUpRes, CmdItemOrnamentOneKeyLvUpRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdItem.OrnamentRebuildRes, OnOrnamentRebuildRes, CmdItemOrnamentRebuildRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdItem.OrnamentDecomposeRes, OnOrnamentDecomposeRes, CmdItemOrnamentDecomposeRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdItem.OrnamentLockRes, OnOrnamentLockRes, CmdItemOrnamentLockRes.Parser);
        }
        public override void OnLogin()
        {
        }
        public override void OnLogout()
        {
            UnShowUpgradeHint = false;
            UnShowRecastHint = false;
        }
        #endregion

        #region net
        /// <summary> 装备、卸下饰品 </summary>
        public void OrnamentFitReq(uint slot, ulong uuid = 0)
        {
            CmdItemOrnamentFitReq req = new CmdItemOrnamentFitReq();
            req.OrnamentSlot = slot;
            req.Uuid = uuid;
            NetClient.Instance.SendMessage((ushort)CmdItem.OrnamentFitReq, req);
        }
        private void OnOrnamentFitRes(NetMsg msg)
        {
        }
        /// <summary> 升级单件饰品 </summary>
        public void OrnamentLvUpReq(ulong uuid)
        {
            CmdItemOrnamentLvUpReq req = new CmdItemOrnamentLvUpReq();
            req.Uuid = uuid;
            NetClient.Instance.SendMessage((ushort)CmdItem.OrnamentLvUpReq, req);
        }
        private void OnOrnamentLvUpRes(NetMsg msg)
        {
            CmdItemOrnamentLvUpRes res = NetMsgUtil.Deserialize<CmdItemOrnamentLvUpRes>(CmdItemOrnamentLvUpRes.Parser, msg);
            if (res.Success)
            {
                ItemData item = Sys_Bag.Instance.GetItemDataByUuid(res.NewItemUuid);
                if (item != null && item.ornament != null && (item.ornament.ExtAttr.Count > 0 || item.ornament.ExtSkill.Count > 0))
                {
                    OrnamentResultPrama param = new OrnamentResultPrama
                    {
                        type = (uint)EOrnamentPageType.Upgrade,
                        itemUuid = res.NewItemUuid,
                    };
                    UIManager.OpenUI(EUIID.UI_Ornament_Result, false, param);
                }
            }
            eventEmitter.Trigger<ulong, bool>(EEvents.OnUpgradeResBack, res.NewItemUuid, res.Success);
        }
        /// <summary> 单种饰品全部升级 </summary>
        public void OrnamentAllLvUpReq(uint infoId)
        {
            CmdItemOrnamentOneKeyLvUpReq req = new CmdItemOrnamentOneKeyLvUpReq();
            req.InfoId = infoId;
            NetClient.Instance.SendMessage((ushort)CmdItem.OrnamentOneKeyLvUpReq, req);
        }
        private void OrnamentAllLvUpRes(NetMsg msg)
        {
            CmdItemOrnamentOneKeyLvUpRes res = NetMsgUtil.Deserialize<CmdItemOrnamentOneKeyLvUpRes>(CmdItemOrnamentOneKeyLvUpRes.Parser, msg);
            eventEmitter.Trigger<uint, bool>(EEvents.OnUpgradeAllResBack, res.InfoId, res.HasNewItem);
        }
        public void OrnamentRebuildReq(ulong uuid)
        {
            CmdItemOrnamentRebuildReq req = new CmdItemOrnamentRebuildReq();
            req.Uuid = uuid;
            LastRecastUuid = uuid;
            var itemData = Sys_Bag.Instance.GetItemDataByUuid(uuid);
            if (itemData != null && itemData.ornament != null)
            {
                var lockList = itemData.ornament.LockList;
                req.LockList.AddRange(lockList);
            }
            NetClient.Instance.SendMessage((ushort)CmdItem.OrnamentRebuildReq, req);
        }
        private void OnOrnamentRebuildRes(NetMsg msg)
        {
            OrnamentResultPrama param = new OrnamentResultPrama
            {
                type = (uint)EOrnamentPageType.Recast,
                itemUuid = LastRecastUuid,
            };
            UIManager.OpenUI(EUIID.UI_Ornament_Result, false, param);
            eventEmitter.Trigger(EEvents.OnRecastResBack);
        }
        /// <summary> 分解饰品 </summary>
        public void OrnamentDecomposeReq(ulong uuid)
        {
            CmdItemOrnamentDecomposeReq req = new CmdItemOrnamentDecomposeReq();
            req.Uuid = uuid;
            NetClient.Instance.SendMessage((ushort)CmdItem.OrnamentDecomposeReq, req);
        }
        private void OnOrnamentDecomposeRes(NetMsg msg)
        {
        }
        /// <summary> 锁属性 </summary>
        public void OrnamentLockReq(ulong uuid, uint infoId, bool isLock)
        {
            CmdItemOrnamentLockReq req = new CmdItemOrnamentLockReq();
            req.Uuid = uuid;
            req.InfoId = infoId;
            req.Lock = isLock;

            NetClient.Instance.SendMessage((ushort)CmdItem.OrnamentLockReq, req);
        }
        private void OnOrnamentLockRes(NetMsg msg)
        {
            CmdItemOrnamentLockRes res = NetMsgUtil.Deserialize<CmdItemOrnamentLockRes>(CmdItemOrnamentLockRes.Parser, msg);
            //客户端手动覆盖一下数据
            var itemData = Sys_Bag.Instance.GetItemDataByUuid(res.Uuid);
            if (itemData != null && itemData.ornament != null)
            {
                if (res.Lock)
                {
                    itemData.ornament.LockList.Add(res.InfoId);
                }
                else
                {
                    itemData.ornament.LockList.Remove(res.InfoId);
                }
            }
            //DebugOrnamnetLockState(res.Uuid, res.InfoId, res.Lock);
            eventEmitter.Trigger<uint, bool>(EEvents.OnRecastLockResBack, res.InfoId, res.Lock);
        }
        #endregion

        #region function
        /// <summary> 检测功能是否开启 </summary>
        public bool CheckIsOpen(bool showMsg = false)
        {
            if (CSVCheckseq.Instance.GetConfData(10901).IsValid())
            {
                return true;
            }
            if (showMsg)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000569));//角色等级不足Lv.30，无法进行饰品升级
            }
            return false;
        }
        /// <summary>
        /// 检测重铸功能是否开启
        /// </summary>
        public bool CheckRecastIsOpen(bool showMsg = false)
        {
            if (CSVCheckseq.Instance.GetConfData(10911).IsValid())
            {
                return true;
            }
            if (showMsg)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000575));//角色等级Lv.50开启饰品重铸功能
            }
            return false;
        }
        private void InitCSVData()
        {
            Types.AddRange(CSVOrnamentsType.Instance.GetKeys());

            //List<uint> OrnamentIds = new List<uint>();
            //OrnamentIds.AddRange(CSVOrnamentsUpgrade.Instance.GetKeys());

            var ornaments = CSVOrnamentsUpgrade.Instance.GetAll();
            for (int i = 0; i < ornaments.Count; i++)
            {
                //uint id = OrnamentIds[i];
                //CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(id);

                CSVOrnamentsUpgrade.Data ornament = ornaments[i];
                uint id = ornament.id;

                if (!dictOrnament.TryGetValue(ornament.type, out Dictionary<uint, List<uint>> types))
                {
                    types = new Dictionary<uint, List<uint>>();
                    dictOrnament[ornament.type] = types;
                }

                if (!types.TryGetValue(ornament.lv, out List<uint> lvs))
                {
                    lvs = new List<uint>();
                    types[ornament.lv] = lvs;
                }

                lvs.Add(ornament.id);

                if (ornament.reforge_cost != null && ornament.reforge_cost[0] != null)
                {
                    CanRecastIds.Add(id);
                }

                //if (!dictOrnament.ContainsKey(ornament.type))
                //{
                //    dictOrnament[ornament.type] = new Dictionary<uint, List<uint>>();
                //}
                //if (!dictOrnament[ornament.type].ContainsKey(ornament.lv))
                //{
                //    dictOrnament[ornament.type][ornament.lv] = new List<uint>();
                //}
                //dictOrnament[ornament.type][ornament.lv].Add(ornament.id);
                //if (ornament.reforge_cost != null && ornament.reforge_cost[0] != null)
                //{
                //    CanRecastIds.Add(id);
                //}
            }
        }
        public ItemData GetEquipedOrnament(uint ornamentSlot)
        {
            List<ItemData> listItems = null;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdOrnament, out listItems))
            {
                for (int i = 0; i < listItems.Count; ++i)
                {
                    if (listItems[i].Position == ornamentSlot)
                    {
                        return listItems[i];
                    }
                }
            }

            return null;
        }
        public bool IsEquiped(ItemData itemdata)
        {
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdOrnament, out List<ItemData> listItems))
            {
                for (int i = 0; i < listItems.Count; ++i)
                {
                    if (listItems[i].Uuid == itemdata.Uuid)
                        return true;
                }
            }
            return false;
        }
        public bool IsShowCompare(ItemData _item, ref ItemData _resultItem)
        {
            bool isCompare = false;

            CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(_item.Id);
            if (ornamentData != null)
            {
                ItemData result = GetEquipedOrnament(ornamentData.type);
                if (result != null)
                {
                    _resultItem = result;
                    isCompare = true;
                }
            }

            return isCompare;
        }

        public bool IsShowCompare(uint infoId, ref ItemData _resultItem)
        {
            bool isCompare = false;

            CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(infoId);
            if (ornamentData != null)
            {
                ItemData result = GetEquipedOrnament(ornamentData.type);
                if (result != null)
                {
                    _resultItem = result;
                    isCompare = true;
                }
            }

            return isCompare;
        }

        /// <summary> 返回对应type和lv的饰品id  </summary>
        public uint GetOrnamentIdByTypeAndLv(uint type, uint lv)
        {
            if (dictOrnament.TryGetValue(type, out Dictionary<uint, List<uint>> dict))
            {
                if (dict.TryGetValue(lv, out List<uint> ids))
                {
                    return ids[0];
                }
            }
            return 0;
        }
        /// <summary> 获取背包里对应id的饰品列表，不包括穿戴身上的 </summary>
        public List<ItemData> GetItemListByOrnamentId(uint _id)
        {
            List<ItemData> resList = new List<ItemData>();
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out List<ItemData> listItems))
            {
                for (int i = 0; i < listItems.Count; i++)
                {
                    if (listItems[i].Id == _id)
                    {
                        resList.Add(listItems[i]);
                    }
                }
            }
            return resList;
        }
        public bool CheckOrnamentCanUpgrade(uint _ornamentId, bool needSelect, bool showMsg = false)
        {
            CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(_ornamentId);
            var costItems = ornamentData.upgrade_cost_item;
            if (costItems != null)
            {
                for (int i = 0; i < costItems.Count; i++)
                {
                    long hasNum = Sys_Bag.Instance.GetItemCount(costItems[i][0]);
                    if (hasNum < costItems[i][1])
                    {
                        if (showMsg)
                        {
                            //消耗道具不足
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000559));//琉璃石不足，无法升级
                        }
                        return false;
                    }
                }
            }
            if (needSelect)
            {
                if (UpgradeTargetUuid == 0)
                {
                    if (showMsg)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000555));//请先选择用于升级的饰品
                    }
                    return false;
                }
            }
            else
            {
                uint needNum = 1;
                var costOrnament = ornamentData.upgrade_cost_equip;
                if (costOrnament != null)
                {
                    needNum += costOrnament[1];
                }
                long hasNum = Sys_Bag.Instance.GetItemCount(_ornamentId);
                if (hasNum < needNum)
                {
                    if (showMsg)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000554));//材料饰品不足
                    }
                    return false;
                }
            }
            return true;
        }
        /// <summary> 获取身上以及背包内可重铸的饰品 </summary>
        public List<ulong> GetCanRecastItemList()
        {
            List<ulong> items = new List<ulong>();
            //身上的
            for (int i = 0; i < Types.Count; i++)
            {
                var itemData = GetEquipedOrnament(Types[i]);
                if (itemData != null && CanRecastIds.Contains(itemData.Id))
                {
                    items.Add(itemData.Uuid);
                }
            }
            //背包里的
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out List<ItemData> listItems))
            {
                for (int i = 0; i < listItems.Count; i++)
                {
                    if (CanRecastIds.Contains(listItems[i].Id))
                    {
                        items.Add(listItems[i].Uuid);
                    }
                }
            }
            return items;
        }
        /// <summary>
        /// 检测饰品是否可以重铸
        /// </summary>
        public bool CheckCanRecast(uint itemId)
        {
            return CanRecastIds.Contains(itemId);
        }
        /// <summary> 检测饰品是否需要弹出快捷穿戴提示 </summary>
        public bool CheckNeedUseItem(ItemData itemData)
        {
            if (itemData != null && itemData.ornament != null && CheckCanEquipByLv(itemData))
            {
                CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(itemData.Id);

                var equipedOrnament = GetEquipedOrnament(ornamentData.type);
                if (equipedOrnament != null)
                {
                    if (itemData.ornament.Score > equipedOrnament.ornament.Score)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary> 检测穿戴等级是否足够 </summary>
        public bool CheckCanEquipByLv(ItemData itemData, bool showMsg = false)
        {
            CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(itemData.Id);
            if (ornament != null)
            {
                //如果有特殊属性
                if (itemData.ornament != null)
                {
                    var skillList = itemData.ornament.ExtSkill;
                    for (int i = 0; i < skillList.Count; i++)
                    {
                        if (Sys_Ornament.Instance.CheckIsSpecialInfoId(skillList[i].InfoId))
                        {
                            return true;
                        }
                    }
                }

                bool canEquip = Sys_Role.Instance.Role.Level >= ornament.equipment_level;
                if (showMsg && !canEquip)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4099));//人物等级太低无法穿戴高级装备
                }
                return canEquip;
            }
            return false;
        }
        /// <summary> 获取穿戴等级 </summary>
        public uint GetCanEquipLv(ItemData itemData)
        {
            CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(itemData.Id);
            if (ornament != null)
            {
                //如果有特殊属性
                if (itemData.ornament != null)
                {
                    var skillList = itemData.ornament.ExtSkill;
                    for (int i = 0; i < skillList.Count; i++)
                    {
                        if (Sys_Ornament.Instance.CheckIsSpecialInfoId(skillList[i].InfoId))
                        {
                            return 1;
                        }
                    }
                }
                return ornament.equipment_level;
            }
            return 0;
        }
        /// <summary> 检测是否可以分解 </summary>
        public bool CheckCanResolve(ItemData itemData, bool showMsg = false)
        {
            CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(itemData.Id);
            if (ornament != null)
            {
                string str = CSVParam.Instance.GetConfData(959).str_value;
                string[] strs = str.Split('|');
                uint.TryParse(strs[0], out uint needRoleLv);
                uint.TryParse(strs[1], out uint needOrnamentLv);
                if (ornament.lv >= needOrnamentLv)
                {
                    if (Sys_Role.Instance.Role.Level >= needRoleLv)
                    {
                        return true;
                    }
                    else
                    {
                        if (showMsg)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000563));//角色等级不足Lv.50，无法分解饰品;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary> 获取特殊技能值的文本显示 NowPassiveSkillId:当前被动技能表id </summary>
        public string GetPassiveSkillShowString(uint ornamentSkillid, CSVPassiveSkillInfo.Data NowPassiveSkillData)
        {
            if (CheckIsSpecialInfoId(ornamentSkillid))
            {
                return LanguageHelper.GetTextContent(NowPassiveSkillData.desc);
            }
            uint NowPassiveSkillId = NowPassiveSkillData.passive_skill_id[0];
            CSVOrnamentsSkill.Data ornamentSkill = CSVOrnamentsSkill.Instance.GetConfData(ornamentSkillid);
            CSVPassiveSkill.Data nowPassiveSkillData = CSVPassiveSkill.Instance.GetConfData(NowPassiveSkillId);
            if (ornamentSkill != null && nowPassiveSkillData != null && nowPassiveSkillData.passive_effective.Count > 1)
            {
                uint maxSkillId = ornamentSkill.skill_id * 1000 + ornamentSkill.max_level;
                CSVPassiveSkillInfo.Data MaxPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(maxSkillId);
                CSVBuff.Data nowBuffData = CSVBuff.Instance.GetConfData(nowPassiveSkillData.passive_effective[1]);
                if (MaxPassiveSkillInfoData != null && nowBuffData != null && nowBuffData.effective_attr.Count > 0)
                {
                    CSVPassiveSkill.Data MaxPassiveSkillData = CSVPassiveSkill.Instance.GetConfData(MaxPassiveSkillInfoData.passive_skill_id[0]);
                    CSVBuff.Data maxBuffData = CSVBuff.Instance.GetConfData(MaxPassiveSkillData.passive_effective[1]);
                    return ((float)nowBuffData.effective_attr[0][1] / 100).ToString() + "%/" + ((float)maxBuffData.effective_attr[0][1] / 100).ToString() + "%";
                }
            }
            return "";
        }
        /// <summary> 获取额外属性值文本 ornamentAttrid饰品的额外属性表id </summary>
        public string GetExtAttrShowString(uint nowValue, uint ornamentAttrid)
        {
            CSVOrnamentsAttributes.Data ornamentAttrData = CSVOrnamentsAttributes.Instance.GetConfData(ornamentAttrid);
            if (ornamentAttrData != null)
            {
                var attrData = CSVAttr.Instance.GetConfData(ornamentAttrData.attr_id);
                if (attrData.show_type == 2)
                {
                    return (nowValue / 10000f).ToString("P2") + "/" + (ornamentAttrData.max_attr / 10000f).ToString("P2");
                }
                else
                {
                    return nowValue.ToString() + "/" + ornamentAttrData.max_attr.ToString();
                }
            }
            return "";
        }
        /// <summary> 根据评分获得品质 </summary>
        public uint GetQualityByScore(uint id, uint score)
        {
            CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(id);
            if (ornament != null && score > 0)
            {
                var scoreList = ornament.score_sec;
                uint minScore = 0;
                uint maxScore = 0;
                for (int i = 0; i < scoreList.Count; i++)
                {
                    minScore = maxScore;
                    maxScore = scoreList[i];
                    if (score > minScore && score <= maxScore)
                    {
                        return (uint)i + 1;
                    }
                }
                if (maxScore > 0 && score > maxScore)
                {
                    return 5;//橙色
                }
            }
            return 1;
        }
        /// <summary> 检测是否是极品属性 </summary>
        public bool CheckExtAttrIsBest(uint id, uint value)
        {
            CSVOrnamentsAttributes.Data attrData = CSVOrnamentsAttributes.Instance.GetConfData(id);
            uint prama = uint.Parse(CSVParam.Instance.GetConfData(1005).str_value);
            if (attrData != null)
            {
                return (float)value / (float)attrData.max_attr * attrData.score >= prama;
            }
            return false;
        }
        /// <summary> 检测是否是极品技能 </summary>
        public bool CheckExtSkillIsBest(uint skillId)
        {
            CSVPassiveSkillInfo.Data skillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
            uint prama = uint.Parse(CSVParam.Instance.GetConfData(1005).str_value);
            if (skillInfoData != null)
            {
                return skillInfoData.score >= prama;
            }
            return false;
        }

        /// <summary>
        /// 根据饰品类型获取饰品类型名称 1:项链 2：耳环 3：戒指
        /// </summary>
        public string GetOrnamentTypeName(uint typeId)
        {
            string str = "";
            if (typeId == 1)
            {
                str = LanguageHelper.GetTextContent(4809);//项链
            }
            else if (typeId == 2)
            {
                str = LanguageHelper.GetTextContent(4808);//耳环
            }
            else if (typeId == 3)
            {
                str = LanguageHelper.GetTextContent(4810);//戒指
            }
            return str;
        }
        /// <summary>
        /// 弹饰品升级的二次确认弹窗
        /// </summary>
        public void PopupOrnamentUpgradeConfirmHintView(System.Action action)
        {
            bool changeValueState = false;
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                UnShowUpgradeHint = changeValueState;
                action?.Invoke();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            PromptBoxParameter.Instance.SetToggleChanged(true, (bool value) =>
            {
                changeValueState = value;
            });
            PromptBoxParameter.Instance.SetToggleChecked(false);
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000567);
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
        }

        /// <summary>
        /// 弹饰品重铸的二次确认弹窗
        /// </summary>
        public void PopupOrnamentRecastConfirmHintView(System.Action action)
        {
            bool changeValueState = false;
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                UnShowRecastHint = changeValueState;
                action?.Invoke();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            PromptBoxParameter.Instance.SetToggleChanged(true, (bool value) =>
            {
                changeValueState = value;
            });
            PromptBoxParameter.Instance.SetToggleChecked(false);
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000568);
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
        }

        /// <summary>
        /// 弹饰品重铸锁定的二次确认弹窗
        /// </summary>
        public void PopupOrnamentRecastLockConfirmHintView(System.Action action)
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                action?.Invoke();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000572);//锁定的额外属性越多，对应的重铸消耗就越多，是否进行锁定？
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
        }

        /// <summary>
        /// 获取对应类型下的饰品红点
        /// </summary>
        public bool GetTypeRedPoint(uint type)
        {
            //遍历该type下所有等级
            CSVOrnamentsType.Data typeData = CSVOrnamentsType.Instance.GetConfData(type);
            uint maxLv = typeData.maxlevel;
            for (int i = 1; i < maxLv; i++)
            {
                if (GetLevelRedPoint(type, (uint)i))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取
        /// </summary>
        public bool GetLevelRedPoint(uint type, uint level)
        {
            uint ornamentId = GetOrnamentIdByTypeAndLv(type, level);
            CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(ornamentId);
            CSVOrnamentsUpgrade.Data nextOrnament = CSVOrnamentsUpgrade.Instance.GetConfData(ornamentData.nextlevelid);
            if (nextOrnament.equipment_level > Sys_Role.Instance.Role.Level)
            {
                return false;
            }
            var costItems = ornamentData.upgrade_cost_item;
            if (costItems != null)
            {
                //判断消耗材料是否足够
                for (int i = 0; i < costItems.Count; i++)
                {
                    long hasItemNum = Sys_Bag.Instance.GetItemCount(costItems[i][0]);
                    if (hasItemNum < costItems[i][1])
                    {
                        return false;
                    }
                }
            }
            //判断饰品材料是否够
            uint needNum = 1;
            var costOrnament = ornamentData.upgrade_cost_equip;
            if (costOrnament != null)
            {
                needNum += costOrnament[1];
            }
            long hasNum = Sys_Bag.Instance.GetItemCount(ornamentId);
            if (hasNum >= needNum)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 升级饰品选择红点
        /// </summary>
        public bool GetSelectRedPoint(uint ornamentId)
        {
            //需要满足升级条件 GetLevelRedPoint
            CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(ornamentId);
            if (ornamentData != null)
            {
                if (UpgradeTargetUuid <= 0 && GetLevelRedPoint(ornamentData.type, ornamentData.lv))
                {
                    long hasNum = Sys_Bag.Instance.GetItemCount(ornamentId);
                    if (hasNum >= 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 获取升级按钮红点
        /// </summary>
        public bool GetUpgradeBtnRedPoint(uint ornamentId)
        {
            //需要满足升级条件 GetLevelRedPoint
            CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(ornamentId);
            if (ornamentData != null)
            {
                if (ornamentData.extra_attr_num > 0 && UpgradeTargetUuid < 0)
                {
                    return false;
                }
                if (GetLevelRedPoint(ornamentData.type, ornamentData.lv))
                {
                    long hasNum = Sys_Bag.Instance.GetItemCount(ornamentId);
                    if (hasNum >= 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 检测是否时特殊InfoID
        /// </summary>
        public bool CheckIsSpecialInfoId(uint infoId)
        {
            return infoId == SpecialAttrInfoId;
        }

        /// <summary>
        /// 客户端手动修正锁定条目(防止出现强制锁定条目不存在于locklist中)
        /// </summary>
        public void FixedOrnamentLockList(ItemData itemData)
        {
            if (itemData != null && itemData.ornament != null)
            {
                var lockList = itemData.ornament.LockList;
                if (!lockList.Contains(SpecialAttrInfoId))
                {
                    var extSkill = itemData.ornament.ExtSkill;
                    for (int i = 0; i < extSkill.Count; i++)
                    {
                        if (CheckIsSpecialInfoId(extSkill[i].InfoId))
                        {
                            lockList.Add(SpecialAttrInfoId);
                        }
                    }
                }
            }
        }

        private void DebugOrnamnetLockState(ulong uuid, uint infoId, bool isLock)
        {
            var itemData = Sys_Bag.Instance.GetItemDataByUuid(uuid);
            var str = "";
            if (itemData != null && itemData.ornament != null)
            {
                str += "\nlockList ";
                var lockList = itemData.ornament.LockList;
                for (int i = 0; i < lockList.Count; i++)
                {
                    str += (lockList[i] + "|");
                }
                str += "\nattrList ";
                var attrList = itemData.ornament.ExtAttr;
                for (int i = 0; i < attrList.Count; i++)
                {
                    str += (attrList[i].InfoId + "|");
                }
                str += "\nattrList ";
                var skillList = itemData.ornament.ExtSkill;
                for (int i = 0; i < skillList.Count; i++)
                {
                    str += (skillList[i].InfoId + "|");
                }
            }
            Debug.Log("饰品锁定 uid " + uuid + " infoId " + infoId + " isLock " + isLock + str);
        }
        #endregion

        #region event
        #endregion
    }
}

