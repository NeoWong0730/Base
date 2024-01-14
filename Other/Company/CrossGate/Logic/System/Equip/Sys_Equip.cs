using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using Table;
using Google.Protobuf.Collections;

namespace Logic
{
    public partial class Sys_Equip : SystemModuleBase<Sys_Equip>
    {
        public class MakeEquipmentResult
        {
            public ItemData preEquip;
            public uint paperId;
        }

        public List<ulong> EquipListUIds = new List<ulong>();

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public ulong CurOpEquipUId = 0L;

        private bool IsJewelComposeTen = false;

        private float DurabilityPercent = 0f;
        public uint QualityLimit;
        private uint SecureLockQuality;
        private uint SecureLockLevel;

        private ulong requestDeleteItem = 0;

        public enum EquipmentOperations
        {
            Inlay = 1,
            Smelt = 2,
            Quenching = 3,
            Repair = 4,
            Make = 5,
            ReMake = 6,
            RfreshEffect = 7,
            Max = 8
        }

        public EquipmentOperations curOperationType = EquipmentOperations.Inlay;

        public class UIEquipPrama
        {
            public ItemData curEquip;
            public EquipmentOperations opType;
        }
        
        public enum EEvents
        {
            OnShowMsg,  //显示详情界面
            OnCompareBasicAttrValue, //基础属性对比
            OnNtfEquiped,
            //OnNtfDelEquipment,

            OnOperationType,            //装备操作类型
            OnSelectInlayEquipment,     //选择操作的装备
            OnJewelNtfInlay,            //宝石镶嵌
            OnJewelNtfUnload,            //宝石卸下
            OnJewelNtfQuickCompose,
            OnJewelCompose,             //合成宝石
            OnNotifySmelt,           //装备熔炼
            OnNotifyRevertSmelt,     //装备熔炼还原
            OnNotifyQuenching,       //装备粹炼
            OnNotifyRepair,         //装备修理
            OnNotifyMake,           //打造套装
            OnNotifyMakeUse,        //替换打造套装属性
            OnNtfDecomposeItem,     //分解装备通知
            OnNotifyJewelUpgradeChange,
            OnNtfFieldUpgradeMat,   //选择部位升级材料
            OnNtfBodyUpgrade,       //装备部位升级通知
            OnNtfBodyUpgradeSkillRefresh,  //装备部位升级被动技能重置
            OnNtfRebuildEquip,      //装备重铸通知
            OnNtfRefreshEffect,     //装备洗练通知
        }

        public override void Init()
        {
            base.Init();

            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.EquipReq, (ushort)CmdItem.EquipRes, OnEquipRes, CmdItemEquipRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.UnloadEquipmentReq, (ushort)CmdItem.UnloadEquipmentRes, OnUnloadEquipRes, CmdItemUnloadEquipmentRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.InlayJewelEquipmentReq, (ushort)CmdItem.InlayJewelEquipmentRes, OnInlayJewelRes, CmdItemInlayJewelEquipmentRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.UnloadJewelReq, (ushort)CmdItem.UnloadJewelRes, OnUnloadJewelRes, CmdItemUnloadJewelRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.QuickComposeJewelReq, (ushort)CmdItem.QuickComposeJewelRes, OnQuickComposeRes, CmdItemQuickComposeJewelRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.ComposeJewelReq, (ushort)CmdItem.ComposeJewelRes, OnComposeJewelRes, CmdItemComposeJewelRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.SmeltEquipmentReq, (ushort)CmdItem.SmeltEquipmentRes, OnEquipmentSmeltRes, CmdItemSmeltEquipmentRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.RevertSmeltReq, (ushort)CmdItem.RevertSmeltRes, OnEquipmentRevertSmeltRes, CmdItemRevertSmeltRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.ExtractEquipmentReq, (ushort)CmdItem.ExtractEquipmentRes, OnEquipmentQuenchingRes, CmdItemExtractEquipmentRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.RepairEquipmentReq, (ushort)CmdItem.RepairEquipmentRes, OnEquipmentRepairRes, CmdItemRepairEquipmentRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.EnchantEquipmentReq, (ushort)CmdItem.EnchantEquipmentRes, OnEquipmentEnchantRes, CmdItemEnchantEquipmentRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdItem.BroadcastWeaponChangeNtf, OnBroadcastWeaponChangeNtf, CmdItemBroadcastWeaponChangeNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.BuildEquipmentReq, (ushort)CmdItem.BuildEquipmentRes, OnEquipmentBuildRes, CmdItemBuildEquipmentRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.BuildEquipmentUseReq, (ushort)CmdItem.BuildEquipmentUseRes, OnBuildReplaceAttrRes, CmdItemBuildEquipmentUseRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.DecomposeEquipmentReq, (ushort)CmdItem.DecomposeEquipmentRes, OnEquipmentDecomposeRes, CmdItemDecomposeEquipmentRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.JewelLevelAttrSelectReq, (ushort)CmdItem.JewelLevelAttrSelectRes, OnJewelLevelAttrSelectRes, CmdItemJewelLevelAttrSelectRes.Parser);
            OnInitSlotUpgradeNtf();
            OnInitRemakeNtf();
            OnInitRefreshEffectNtf();

            DurabilityPercent = float.Parse(CSVParam.Instance.GetConfData(215).str_value);
            QualityLimit = uint.Parse(CSVParam.Instance.GetConfData(216).str_value);

            SecureLockQuality = uint.Parse(CSVParam.Instance.GetConfData(1072).str_value);
            SecureLockLevel = uint.Parse(CSVParam.Instance.GetConfData(1073).str_value);
        }

        public override void OnLogin()
        {
            base.OnLogin();
            IsCloseSmeltBoxTip = false;
        }

        public override void OnLogout()
        {
            base.OnLogout();
            slotLeves.Clear();
            slotExps.Clear();
            effects.Clear();
        }

        #region NetMsg
        public void OnEquipReq(uint slot, ItemData itemEquip, bool isTransJewel = false)
        {
            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(itemEquip.cSVItemData.id);
            if (equipInfo != null)
            {
                uint limitLevel = equipInfo.equipment_level;
                for (int i = 0; i < itemEquip.Equip.EffectAttr.Count; ++i)
                {
                    AttributeRow row = itemEquip.Equip.EffectAttr[i].Attr2;
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(row.Id);
                    if (skillInfo != null && skillInfo.attr != null)
                    {
                        if (skillInfo.attr[0] != null)
                        {
                            if (skillInfo.attr[0][0] == 999) //特殊限制,减去限制等级,策划说写死
                            {
                                limitLevel -= (uint)skillInfo.attr[0][1];
                                break;
                            }
                        }
                    }
                }

                //check 装备等级
                if (Sys_Role.Instance.Role.Level < limitLevel)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4099));
                    return;
                }

                //check 职业限制
                if (equipInfo.career_condition != null
                    && !equipInfo.career_condition.Contains(Sys_Role.Instance.Role.Career))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4182));
                    return;
                }

                //盾牌需要判断主武器是否是双手武器
                //if (equipInfo.equipment_type == 28u)
                //{
                //    ItemData weaponItem = Sys_Equip.Instance.SameEquipment((uint)EquipmentSlot.EquipSlotWeapon1);
                //    if (weaponItem != null)
                //    {
                //        CSVEquipment.Data weaponData = CSVEquipment.Instance.GetConfData(weaponItem.Id);
                //        if(weaponData != null && weaponData.doublehand)
                //        {
                //            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4186));
                //            return;
                //        }
                //    }
                //}

                CmdItemEquipReq req = new CmdItemEquipReq();
                req.EquipSlot = slot;
                req.Uuid = itemEquip.Uuid;
                req.IsTransJewel = isTransJewel;
                NetClient.Instance.SendMessage((ushort)CmdItem.EquipReq, req);
                requestDeleteItem = itemEquip.Uuid;
            }
        }

        private void OnEquipRes(NetMsg msg)
        {
            CmdItemEquipRes res = NetMsgUtil.Deserialize<CmdItemEquipRes>(CmdItemEquipRes.Parser, msg);
            UIManager.CloseUI(EUIID.UI_TipsEquipment);
            Sys_Bag.Instance.eventEmitter.Trigger(Sys_Bag.EEvents.OnClearMainBagSelect);
            eventEmitter.Trigger(EEvents.OnNtfEquiped);
            Sys_LivingSkill.Instance.eventEmitter.Trigger<ulong>(Sys_LivingSkill.EEvents.OnEquip, requestDeleteItem);
            requestDeleteItem = 0;
            Sys_Hint.Instance.PushEffectInNextFight();
        }

        public void UnloadEquipReq(ulong uuId)
        {
            CmdItemUnloadEquipmentReq req = new CmdItemUnloadEquipmentReq();
            req.Uuid = uuId;
            NetClient.Instance.SendMessage((ushort)CmdItem.UnloadEquipmentReq, req);
        }

        private void OnUnloadEquipRes(NetMsg msg)
        {
            CmdItemUnloadEquipmentRes res = NetMsgUtil.Deserialize<CmdItemUnloadEquipmentRes>(CmdItemUnloadEquipmentRes.Parser, msg);
            UIManager.CloseUI(EUIID.UI_TipsEquipment);

            Sys_Hint.Instance.PushEffectInNextFight();
        }

        public void UnloadJewelReq(ulong equipUId, uint _unloadPos)
        {
            CmdItemUnloadJewelReq req = new CmdItemUnloadJewelReq();
            req.EquipUUID = equipUId;
            req.JewelAttrIndex = _unloadPos;
            NetClient.Instance.SendMessage((ushort)CmdItem.UnloadJewelReq, req);
        }

        private void OnUnloadJewelRes(NetMsg msg)
        {
            eventEmitter.Trigger(EEvents.OnJewelNtfUnload);

            Sys_Hint.Instance.PushEffectInNextFight();
        }

        public void InlayJewelReq(uint jewelInfoId)
        {
            CSVJewel.Data jewelInfo = CSVJewel.Instance.GetConfData(jewelInfoId);
            if (jewelInfo != null)
            {
                ItemData itemEquip = GetItemData(CurOpEquipUId);

                CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(itemEquip.Id);
                if (jewelInfo.level > equipInfo.jewel_level)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4088));
                    return;
                }
            }

            CmdItemInlayJewelEquipmentReq req = new CmdItemInlayJewelEquipmentReq();
            req.EquipmentUUID = CurOpEquipUId;
            //req.JewelUUID = jewelUId;
            req.JewelinfoId = jewelInfoId;
            req.JewelAttrIndex = InlaySlotPos;
            NetClient.Instance.SendMessage((ushort)CmdItem.InlayJewelEquipmentReq, req);
        }

        private void OnInlayJewelRes(NetMsg msg)
        {
            //CmdItemInlayJewelEquipmentRes res = NetMsgUtil.Deserialize<CmdItemInlayJewelEquipmentRes>(CmdItemInlayJewelEquipmentRes.Parser, msg);
            eventEmitter.Trigger(EEvents.OnJewelNtfInlay);
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.WearGem);
            Sys_Hint.Instance.PushEffectInNextFight();
            eventEmitter.Trigger<ulong>(EEvents.OnJewelNtfInlay, CurOpEquipUId);
        }

        public void JewelCompoundReq(uint _itemId, uint _count = 1)
        {
            CmdItemComposeJewelReq req = new CmdItemComposeJewelReq();
            req.FromItemID = _itemId;
            req.Count = _count;
            NetClient.Instance.SendMessage((ushort)CmdItem.ComposeJewelReq, req);

            IsJewelComposeTen = _count > 1;
        }

        private void OnComposeJewelRes(NetMsg msg)
        {
            eventEmitter.Trigger(EEvents.OnJewelCompose, IsJewelComposeTen);
        }

        //打造套装临时存储
        //private ItemData makePreEquip;
        //private uint paperId;
        public void OnEquipmentBuildReq(ItemData equipItem, ItemData paperItem = null)
        {
            //makePreEquip = equipItem;
            //paperId = paperItem.cSVItemData.id;

            CmdItemBuildEquipmentReq req = new CmdItemBuildEquipmentReq();
            req.Equipuuid = equipItem.Uuid;
            if (paperItem != null)
                req.CostItemuuid = paperItem.Uuid;
            NetClient.Instance.SendMessage((ushort)CmdItem.BuildEquipmentReq, req);
        }

        private void OnEquipmentBuildRes(NetMsg msg)
        {
            CmdItemBuildEquipmentRes res = NetMsgUtil.Deserialize<CmdItemBuildEquipmentRes>(CmdItemBuildEquipmentRes.Parser, msg);

            //MakeEquipmentResult result = new MakeEquipmentResult();
            //result.preEquip = makePreEquip;
            //result.paperId = paperId;

            //UIManager.OpenUI(EUIID.UI_Make_Success, false, result);
            eventEmitter.Trigger(EEvents.OnNotifyMake);
        }

        #region 锻造替换属性
        public void OnBuildReplaceAttrReq(ulong uuId)
        {
            CmdItemBuildEquipmentUseReq req = new CmdItemBuildEquipmentUseReq();
            req.EquipUUID = uuId;
            NetClient.Instance.SendMessage((ushort)CmdItem.BuildEquipmentUseReq, req);
        }

        private void OnBuildReplaceAttrRes(NetMsg msg)
        {
            CmdItemBuildEquipmentUseRes res = NetMsgUtil.Deserialize<CmdItemBuildEquipmentUseRes>(CmdItemBuildEquipmentUseRes.Parser, msg);
            //和锻造一样刷新UI
            eventEmitter.Trigger(EEvents.OnNotifyMake);

            eventEmitter.Trigger(EEvents.OnNotifyMakeUse);
        }
        #endregion

        private void OnBroadcastWeaponChangeNtf(NetMsg msg)
        {
            CmdItemBroadcastWeaponChangeNtf res = NetMsgUtil.Deserialize<CmdItemBroadcastWeaponChangeNtf>(CmdItemBroadcastWeaponChangeNtf.Parser, msg);
            //Hero hero = GameCenter.mainWorld.GetActor(Hero.Type, res.RoleID) as Hero;
            Hero hero = GameCenter.GetSceneHero(res.RoleID);
            if (hero != null)
            {
                if (hero.weaponComponent != null)
                {
                    hero.weaponComponent.CurWeaponID = res.ItemID;
                    hero.ChangeModel();
                }
            }
            else
            {
                Debug.LogErrorFormat("FashionDyeStateNtf not found hero = {0}", res.RoleID);
            }
        }

        public void OnEquipmentDecomposeReq(ulong equipUId)
        {
            CmdItemDecomposeEquipmentReq req = new CmdItemDecomposeEquipmentReq();
            req.EquipUUID = equipUId;
            requestDeleteItem = equipUId;
            NetClient.Instance.SendMessage((ushort)CmdItem.DecomposeEquipmentReq, req);
        }

        private void OnEquipmentDecomposeRes(NetMsg msg)
        {
            //CmdItemDecomposeEquipmentRes res = NetMsgUtil.Deserialize<CmdItemDecomposeEquipmentRes>(CmdItemDecomposeEquipmentRes.Parser, msg);
            UIManager.CloseUI(EUIID.UI_TipsEquipment);
            eventEmitter.Trigger<ulong>(EEvents.OnNtfDecomposeItem, requestDeleteItem);
            requestDeleteItem = 0;
        }



        #endregion

        public bool IsSelectedEquipment(ulong UId)
        {
            if (CurOpEquipUId == UId)
            {
                return true;
            }
            return false;
        }

        public bool IsEquiped(ItemData itemdata)
        {
            List<ItemData> listItems = null;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdEquipment, out listItems))
            {
                for (int i = 0; i < listItems.Count; ++i)
                {
                    if (listItems[i].Uuid == itemdata.Uuid)
                        return true;
                }
            }

            return false;
        }

        public void SortEquipments(EquipmentOperations _type)
        {
            EquipListUIds.Clear();

            List<ItemData> tempList = new List<ItemData>();

            List<ItemData> bodyList = new List<ItemData>();
            if (_type != EquipmentOperations.ReMake)
            {
                if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdEquipment, out bodyList))
                {
                    //评分排序
                    bodyList.Sort((var1, var2) => var2.Equip.Score.CompareTo(var1.Equip.Score));
                }

                if (_type != EquipmentOperations.Quenching) //除粹炼外，已装备显示在前
                {
                    tempList.AddRange(bodyList);
                }
            }

            List<ItemData> itemList;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out itemList))
            {
                List<ItemData> equiplist = new List<ItemData>();
                for (int i = 0; i < itemList.Count; ++i)
                {
                    if (itemList[i].cSVItemData.type_id == (uint)EItemType.Equipment)
                    {
                        equiplist.Add(itemList[i]);
                    }
                }

                //评分排序
                equiplist.Sort((var1, var2) => var2.Equip.Score.CompareTo(var1.Equip.Score));

                tempList.AddRange(equiplist);
            }

            if (_type == EquipmentOperations.Quenching) //粹炼，已装备显示在后
            {
                tempList.AddRange(bodyList);
            }

            for (int i = 0; i < tempList.Count; ++i)
            {
                //熔炼，淬炼，套装打造，锻铸，洗练,需要检测
                if (_type == EquipmentOperations.Smelt
                    || _type == EquipmentOperations.Quenching
                    || _type == EquipmentOperations.Make
                    || _type == EquipmentOperations.ReMake
                    || _type == EquipmentOperations.RfreshEffect)
                {
                    if (!tempList[i].bBind && CheckQuality(tempList[i]))
                    {
                        if (_type == EquipmentOperations.Make) //锻铸 有额外条件
                        {
                            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(tempList[i].Id);
                            if (equipInfo != null && equipInfo.suit_item_base != null)
                                EquipListUIds.Add(tempList[i].Uuid);
                        }
                        else if (_type == EquipmentOperations.ReMake || _type == EquipmentOperations.RfreshEffect)
                        {
                            if (tempList[i].Equip.Color >= 4)
                                EquipListUIds.Add(tempList[i].Uuid);
                        }
                        else
                        {
                            EquipListUIds.Add(tempList[i].Uuid);
                        }
                    }
                }
                else
                {
                    EquipListUIds.Add(tempList[i].Uuid);
                }
            }
        }

        private bool CheckQuality(ItemData item)
        {
            return item.Quality >= (uint)EEquipmentQuality.Purple;
        }

        public ItemData GetItemData(ulong uuId)
        {
            //已装备查找
            List<ItemData> itemList;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdEquipment, out itemList))
            {
                for (int i = 0; i < itemList.Count; ++i)
                {
                    if (itemList[i].Uuid == uuId)
                        return itemList[i];
                }
            }

            //背包查找
            itemList = null;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out itemList))
            {
                for (int i = 0; i < itemList.Count; ++i)
                {
                    if (itemList[i].Uuid == uuId)
                        return itemList[i];
                }
            }

            //restore 查找
            itemList = null;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdRestore, out itemList))
            {
                for (int i = 0; i < itemList.Count; ++i)
                {
                    if (itemList[i].Uuid == uuId)
                        return itemList[i];
                }
            }

            return null;
        }

        public bool IsTipEquied(ItemData newEquip, ref uint slot, ref bool replace)
        {
            if (!IsFixedEquipRule(newEquip))
                return false;

            CSVEquipment.Data newEquipInfo = CSVEquipment.Instance.GetConfData(newEquip.Id);
            List<uint> slotIds = newEquipInfo.slot_id;
            //未穿戴装备
            for (int i = 0; i < slotIds.Count; ++i)
            {
                if (SameEquipment(slotIds[i]) == null)
                {
                    slot = slotIds[i];
                    replace = false;
                    return true;
                }
            }

            //已穿戴装备对比
            for (int i = 0; i < slotIds.Count; ++i)
            {
                ItemData equipItem = SameEquipment(slotIds[i]);
                if (equipItem != null)
                {
                    CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(equipItem.Id);
                    if (equipInfo.equipment_level <= newEquipInfo.equipment_level
                        && equipInfo.equipment_type == newEquipInfo.equipment_type)
                    {
                        if (newEquip.Equip.Score > equipItem.Equip.Score)
                        {
                            slot = slotIds[i];
                            replace = true;
                            return true;
                        }
                    }
                }
            }

            replace = false;
            return false;
        }

        public List<ItemData> GetEequips(uint infoId, uint quality, List<System.Func<ItemData, bool>> filter)
        {
            //背包查找
            List<ItemData> itemList = new List<ItemData>();
            List<ItemData> boxList;
            bool filterValid = (filter != null && filter.Count > 0);
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out boxList))
            {
                for (int i = 0; i < boxList.Count; ++i)
                {
                    if (boxList[i].Id == infoId)
                    {
                        if (boxList[i].Equip != null && boxList[i].Equip.Color >= quality)
                        {
                            if (!filterValid) {
                                itemList.Add(boxList[i]);
                            }
                            else {
                                bool ret = true;
                                for (int j = 0, length = filter.Count; j < length; ++j) {
                                    ret &= filter[j].Invoke(boxList[i]);
                                }

                                if (ret) {
                                    itemList.Add(boxList[i]);
                                }
                            }
                        }
                    }
                }
            }

            return itemList;
        }

        /// <summary>
        /// 用于背包装备箭头显示
        /// </summary>
        /// <param name="itemEquip"></param>
        /// <returns></returns>
        public bool IsShowArrow(ItemData itemEquip)
        {
            if (itemEquip == null)
                return false;
            if (itemEquip.cSVItemData == null)
                return false;

            if (itemEquip.cSVItemData.type_id != (uint)EItemType.Equipment)
                return false;

            CSVEquipment.Data newEquipInfo = CSVEquipment.Instance.GetConfData(itemEquip.Id);
            List<uint> slotIds = newEquipInfo.slot_id;
            //未穿戴装备
            for (int i = 0; i < slotIds.Count; ++i)
            {
                if (SameEquipment(slotIds[i]) == null)
                {
                    return true;
                }
            }

            //已穿戴装备对比
            for (int i = 0; i < slotIds.Count; ++i)
            {
                ItemData equipItem = SameEquipment(slotIds[i]);
                if (equipItem != null)
                {
                    CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(equipItem.Id);
                    if (equipInfo.equipment_level <= newEquipInfo.equipment_level
                        && equipInfo.equipment_type == newEquipInfo.equipment_type)
                    {
                        if (itemEquip.Equip.Score > equipItem.Equip.Score)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 装备提示，穿戴规则限制
        /// </summary>
        /// <param name="itemEquip"></param>
        /// <returns></returns>
        private bool IsFixedEquipRule(ItemData itemEquip)
        {
            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(itemEquip.Id);
            if (equipInfo != null)
            {
                //check 装备等级
                if (Sys_Role.Instance.Role.Level < equipInfo.equipment_level)
                {
                    return false;
                }

                //check 职业限制
                if (equipInfo.career_condition != null
                    && !equipInfo.career_condition.Contains(Sys_Role.Instance.Role.Career))
                {
                    return false;
                }

                //主武器如果是双手武器需要判断服务器是否已经穿戴
                if (equipInfo.slot_id != null && equipInfo.slot_id[0] == 1u)
                {
                    if (equipInfo.doublehand)
                    {
                        ItemData weaponItem = Sys_Equip.Instance.SameEquipment((uint)EquipmentSlot.EquipSlotWeapon2);
                        if (weaponItem != null)
                        {
                            return false;
                        }
                    }
                }

                //副武器需要判断主武器是否是双手武器
                if (equipInfo.slot_id != null && equipInfo.slot_id[0] == 2u)
                {
                    ItemData weaponItem = Sys_Equip.Instance.SameEquipment((uint)EquipmentSlot.EquipSlotWeapon1);
                    if (weaponItem != null)
                    {
                        CSVEquipment.Data weaponData = CSVEquipment.Instance.GetConfData(weaponItem.Id);
                        if (weaponData != null && weaponData.doublehand)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 装备修理提示
        /// </summary>
        /// <returns></returns>
        public bool IsNeedRepair()
        {
            //已穿戴装备是否需要修理
            List<ItemData> itemList;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdEquipment, out itemList))
            {
                for (int i = 0; i < itemList.Count; ++i)
                {
                    float percent = itemList[i].Equip.DurabilityData.CurrentDurability * 1.0f / itemList[i].Equip.DurabilityData.MaxDurability;
                    percent *= 100;
                    if (percent <= DurabilityPercent)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public string GetEquipmentName(ItemData itemEquip)
        {
            if (itemEquip.Equip.SuitTypeId != 0)
            {
                CSVSuit.Data suitData = CSVSuit.Instance.GetSuitData(itemEquip.Equip.SuitTypeId);
                return LanguageHelper.GetTextContent(4094, LanguageHelper.GetTextContent(suitData.suit_name), LanguageHelper.GetTextContent(itemEquip.cSVItemData.name_id));
            }
            else
            {
                return LanguageHelper.GetTextContent(itemEquip.cSVItemData.name_id);
            }
        }

        public uint GetCurWeapon()
        {
            ItemData weaponItem = Sys_Equip.Instance.SameEquipment((uint)EquipmentSlot.EquipSlotWeapon1);
            if (weaponItem != null)
            {
                return weaponItem.Id;
            }
            else
            {
                return Constants.UMARMEDID;
            }
        }

        public bool IsSecureLock(ItemData itemData, bool jump = true)
        {
            if (null == itemData)
                return false;

            return IsSecureLock(itemData.Id, itemData.Quality, jump);
        }
        
        public bool IsSecureLock(uint equipId, uint itemQuality, bool jump = true)
        {
            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(equipId);
            if (itemQuality >= SecureLockQuality
                && equipData.TransLevel() >= SecureLockLevel
                && Sys_SecureLock.Instance.lockState)
            {
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(106208));
                if (jump)
                    Sys_SecureLock.Instance.JumpToSecureLock();
                return true;
            }

            return false;
        }

     
    }
}

