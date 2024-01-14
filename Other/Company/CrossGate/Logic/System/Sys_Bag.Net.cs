using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using Lib.Core;
using UnityEngine;
using static Packet.CmdItemOptionalGiftPackReq.Types;
using System;
using Google.Protobuf.Collections;

namespace Logic
{
    public class ItemChangeData
    {
        public long count;
        public uint quality;
    }

    public enum EBagViewType
    {
        None = 0,
        ViewBag = 1,    //背包
        ViewTransform = 2,     //变身 
        Max = 3,
    }

    public partial class Sys_Bag : SystemModuleBase<Sys_Bag>
    {
        public Dictionary<int, List<ItemData>> BagItems = new Dictionary<int, List<ItemData>>();//主背包

        private Dictionary<uint, HashSet<ulong>> InfoItems = new Dictionary<uint, HashSet<ulong>>();//key: infoId  valu:该infoId对应的list<itemdata>

        public List<ItemData> changeItems = new List<ItemData>(16);

        public Dictionary<uint, uint> Grids = new Dictionary<uint, uint>();//存储背包类型和对应的格子等级

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public int curBoxId = 1;  //主背包页签   

        public int curSafeBoxTabId = 0;  //银行页签

        public Queue<uint> AddItemQueue = new Queue<uint>();

        private List<ulong> listUseItems = new List<ulong>();

        public Dictionary<uint, ItemChangeData> AddItemList = new Dictionary<uint, ItemChangeData>();

        private long[] currency = new long[(int)ECurrencyType.Max];

        public bool bAnyMainBagFull = false;   // 主背包任一页签是否满了

        private bool tidyBag = false;

        private List<int> m_GiftBoxIds = new List<int>();

        private List<int> m_FamilyBoxIds = new List<int>() { 1, 2, 3 };

        private Dictionary<uint, List<FrozenCurrencyInfo>> m_FrozenCurrencyInfos = new Dictionary<uint, List<FrozenCurrencyInfo>>();

        private bool m_UseItemReq;//用于处理玩家快速点击使用导致的显示bug(在必要的地方已添加保护)

        private Timer m_UseItemTimer;
        public bool useItemReq
        {
            get
            {
                return m_UseItemReq;
            }
            set
            {
                if (m_UseItemReq != value)
                {
                    m_UseItemReq = value;
                    if (m_UseItemReq)
                    {
                        m_UseItemTimer?.Cancel();
                        m_UseItemTimer = Timer.Register(0.5f, () => { m_UseItemReq = false; });
                    }
                }
            }
        }

        private List<uint> m_ForbidItemGetReason = new List<uint>();

        public bool enablePropMessagePenetrationClick;

        public uint curInteractiveNPC;
        /// <summary>数量限制道具已使用数量 </summary>
        Dictionary<uint, uint> dayLimitItemUsedDic = new Dictionary<uint, uint>();

        public enum EEvents
        {
            OnRefreshMainBagData,             //更新ui界面页签总数据
            OnUnLuckBox,                      //解锁格子
            OnRefreshChangeData,              //更新改变的数据
            OnRefreshTemporaryBagData,        //更新临时背包界面
            OnCurrencyChanged,                //货币更新
            OnRefreshSafeBoxBagData,          //更新银行界面  
            OnUpdateSafeBoxTab,               //更新银行解锁页签
            OnChageEquiped,                   //更新身上装备
            OnChangeCrystal,                  //更新元素水晶
            OnChangeOrnament,                 //更新身上饰品
            OnShowOrHideMenuTempBagIcon,      //显示或者隐藏临时背包按钮
            OnPlayMenuBagAnim,                //播放主界面背包动画
            OnClearMainBagSelect,             //清除主背包所有选中特效
            OnClearCopyBagSelect,             //清除银行背包所有选中特效 
            OnClearSafeBoxSelect,             //清除银行所有选中特效
            OnUseItemSuccessInBattle,         //更新战斗内道具使用
            OnBattleRoundStartNtf,            //战斗内回合开始
            OnSelectMob_UseItem,              //战斗内使用物品 选中单位
            OnResetMainBattleData,            //重置战斗内物品使用次数  
            OnRefreshBagFull,                 //刷新背包任一页签是否满了
            OnDeleteItem,                     //删除道具

            OnUseItemRes,// 道具使用后服务器返回
            OnGetItem,      //获得新道具
            OnCallItemSource,//道具反查

            ComposeSpecilEvent,//合成界面反查特殊事件
            ComposeSuccess,//合成后服务器返回

            OnNtfDelItem,
            OnNtfDelSameItem,

            OnFrozenCurrency,

            OnUpdatePropItemMessageParm,
            OnUpdateBagFullState,

            OnAllSale,//一键出售
            OnAllDel,// 一键分解

            OnSelectBagViewType,
            
            OnItemLockedChange,
            OnItemPetEquipLockedChange,
        }


        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.PullPackageDataReq, (ushort)CmdItem.PullPackageDataRes, OnPullPackageDataRes, PackageChangeNotify.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.CleanItemNewIconReq, (ushort)CmdItem.CleanItemNewIconRes, OnClearNewIconRes, CmdItemCleanItemNewIconRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.UnlockBoxLevelReq, (ushort)CmdItem.UnlockBoxLevelRes, OnUnLockGridRes, CmdItemUnlockBoxLevelRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.UseItemReq, (ushort)CmdItem.UseItemRes, OnUseItemRes, CmdItemUseItemRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.DecomposeItemReq, (ushort)CmdItem.DecomposeItemRes, OnDecomposeItemRes, CmdItemDecomposeItemRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.DisCardItemReq, (ushort)CmdItem.DisCardItemRes, OnDisCardItemRes, CmdItemDisCardItemRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.ComposeItemReq, (ushort)CmdItem.ComposeItemRes, OnComposeItemRes, CmdItemComposeItemRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.EquipCrystalReq, (ushort)CmdItem.EquipCrystalAck, EquipCrystalRes, CmdItemEquipCrystalAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.ExchangeCrystalReq, (ushort)CmdItem.ExchangeCrystalAck, ExchangeCrystalRes, CmdItemExchangeCrystalAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.OptionalGiftPackReq, (ushort)CmdItem.OptionalGiftPackRes, OnItemOptionalGiftRes, CmdItemOptionalGiftPackRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.EnergyExChangeGoldReq, (ushort)CmdItem.EnergyExChangeGoldRes, OnEnergyExChangeGoldRes, CmdItemOptionalGiftPackRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdItem.FreezeCurrencyNtf, OnFreezeCurrencyNtf, CmdItemFreezeCurrencyNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdItem.UnfreezeCurrencyNtf, OnUnfreezeCurrencyNtf, CmdItemUnfreezeCurrencyNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdItem.ForceUnfreezeCurrencyNtf, OnForceUnfreezeCurrencyNtf, CmdItemForceUnfreezeCurrencyNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.BatchSellReq, (ushort)CmdItem.BatchSellRes, OnBatchSellRes, CmdItemBatchSellRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.BatchDeComposeReq, (ushort)CmdItem.BatchDeComposeRes, OnBatchDeComposeRes, CmdItemBatchDeComposeRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdItem.DayLimitUpdateNtf, OnDayLimitUpdateNtf, CmdItemDayLimitUpdateNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdItem.LockItemReq, (ushort)CmdItem.LockItemRes, OnItemLockRes, CmdItemLockItemRes.Parser);

            Sys_CutScene.Instance.eventEmitter.Handle<uint, uint>(Sys_CutScene.EEvents.OnStart, OnCutSceneStart, true);
            Sys_CutScene.Instance.eventEmitter.Handle<uint, uint>(Sys_CutScene.EEvents.OnRealEnd, OnCutSceneEnd, true);
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, true);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnCompletedFunctionOpen, true);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnPlayerLevelChanged, true);
            InitParms();
        }
        public override void OnLogin()
        {
            for (int i = 0; i < currency.Length; i++)
            {
                currency[i] = 0;
            }
            AddItemQueue.Clear();
            //PullPackageDataReq();
        }

        public override void OnLogout()
        {
            listUseItems.Clear();
        }

        public override void Dispose()
        {
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, false);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnCompletedFunctionOpen, false);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnPlayerLevelChanged, false);
        }

        private void OnReconnectResult(bool result)
        {
            if (result)
            {
                //PullPackageDataReq();
            }
        }

        private void OnCutSceneStart(uint arg1, uint arg2)
        {
            UIManager.CloseUI(EUIID.UI_PerForm, false, false);
        }

        private void OnCutSceneEnd(uint arg1, uint arg2)
        {
            UIManager.OpenUI(EUIID.UI_PerForm);
        }

        #region Net

        //玩家登陆拉取背包数据请求
        public void PullPackageDataReq()
        {
            CmdItemPullPackageDataReq cmdItemPullPackageDataReq = new CmdItemPullPackageDataReq();
            NetClient.Instance.SendMessage((ushort)CmdItem.PullPackageDataReq, cmdItemPullPackageDataReq);
        }

        //玩家登陆拉取背包数据返回结果
        private void OnPullPackageDataRes(NetMsg netMsg)
        {
            OnBagPackDataChanged(netMsg);
        }

        //整理背包请求
        public void TidyBagPackReq(uint boxId)
        {
            CmdItemSortPackageDataReq cmdItemSortPackageDataReq = new CmdItemSortPackageDataReq();
            cmdItemSortPackageDataReq.BoxID = boxId;
            NetClient.Instance.SendMessage((ushort)CmdItem.SortPackageDataReq, cmdItemSortPackageDataReq);
            tidyBag = true;
        }


        private void OnBagPackDataChanged(NetMsg netMsg)
        {
            PackageChangeNotify res = NetMsgUtil.Deserialize<PackageChangeNotify>(PackageChangeNotify.Parser, netMsg);

            #region 上线登陆 || 整理背包
            if (res.BoxArea.Count > 0)
            {
                foreach (var item in res.BoxArea)
                {
                    //每次计算页签拷贝数据的时候需要清空一下
                    changeItems.Clear();
                    List<ItemData> itemDatas = new List<ItemData>();
                    Item[] itemArray = new Item[item.Item.Count];
                    item.Item.CopyTo(itemArray, 0);
                    List<Item> tempList = new List<Item>(itemArray);
                    tempList.Sort((x, y) => x.Position.CompareTo(y.Position));
                    if (BagItems.ContainsKey(item.BoxID))
                    {
                        List<ItemData> oldItemDatas = BagItems[item.BoxID];
                        for (int i = 0; i < oldItemDatas.Count; i++)
                        {
                            PoolManager.Recycle(oldItemDatas[i]);
                        }
                        BagItems.Remove(item.BoxID);
                    }
                    foreach (var kv in tempList)
                    {
                        ItemData itemData = PoolManager.Fetch(typeof(ItemData)) as ItemData;
                        itemData.SetData(item.BoxID, kv.Uuid, kv.Id, kv.Count, kv.Position, kv.ShowNewIcon, kv.Bind, kv.Equipment, kv.Essence, kv.Marketendtime, null, kv.Crystal, kv.Ornament, kv.PetEquip, kv.Islocked);
                        itemData.outTime = kv.OutTime;
                        if (!InfoItems.TryGetValue(kv.Id, out HashSet<ulong> items))
                        {
                            items = new HashSet<ulong>();
                            InfoItems.Add(kv.Id, items);
                        }
                        items.Add(itemData.Uuid);
                        itemDatas.Add(itemData);
                        changeItems.Add(itemData);
                    }
                    BagItems.Add(item.BoxID, itemDatas);
                    uint boxLevel = 0;
                    if (!Grids.TryGetValue((uint)item.BoxID, out boxLevel))
                    {
                        Grids.Add((uint)item.BoxID, (uint)item.BoxLevel);
                    }
                    Grids[(uint)item.BoxID] = (uint)item.BoxLevel;

                    eventEmitter.Trigger<int, int>(EEvents.OnRefreshChangeData, 0, item.BoxID);

                    //加入提示对列

                    if (!tidyBag)
                    {
                        Dictionary<uint, ItemData> dictOrnaments = new Dictionary<uint, ItemData>();
                        foreach (ItemData temp in itemDatas)
                        {
                            uint typeId = temp.cSVItemData.type_id;
                            if (typeId == (int)EItemType.Equipment)
                            {
                                if (item.BoxID == (int)BoxIDEnum.BoxIdNormal)
                                    PushUseItem(temp);
                            }
                            else if (typeId == (int)EItemType.Ornament)
                            {
                                if (item.BoxID == (int)BoxIDEnum.BoxIdNormal)
                                {
                                    CSVOrnamentsUpgrade.Data ornament = CSVOrnamentsUpgrade.Instance.GetConfData(temp.Id);
                                    if (dictOrnaments.TryGetValue(ornament.type, out ItemData value))
                                    {
                                        if (temp.ornament.Score > value.ornament.Score)
                                        {
                                            dictOrnaments[ornament.type] = temp;
                                        }
                                    }
                                    else
                                    {
                                        dictOrnaments.Add(ornament.type, temp);
                                    }
                                }
                            }
                        }
                        foreach (var temp in dictOrnaments)
                        {
                            PushUseItem(temp.Value);
                        }
                        //keep equipfirst
                        for (int i = 0; i < itemDatas.Count; i++)
                        {
                            if (itemDatas[i].cSVItemData.type_id == (int)EItemType.Crystal && itemDatas[i].BoxId == (int)BoxIDEnum.BoxIdNormal)
                            {
                                PushUseItem(itemDatas[i]);
                                break;
                            }
                        }
                    }
                }
                UpdateItemCount();
                tidyBag = false;
            }
            #endregion

            bool changeEquiped = false;
            bool changeCrystal = false;
            bool changeOrnament = false;
            bool usePetSeal = false;

            #region 道具变更
            if (res.Change.Count > 0)
            {
                changeItems.Clear();

                bool isSendItemsChange = false;

                List<ItemData> needPushPool = new List<ItemData>(8);

                foreach (var item in res.Change)
                {
                    if (!BagItems.ContainsKey((int)item.BoxId))
                    {
                        DebugUtil.LogFormat(ELogType.eBag, "背包不存在{0}页签", item.BoxId);
                        return;
                    }
                    //判断是否是身上装备变化
                    if ((item.BoxId == (uint)BoxIDEnum.BoxIdEquipment))
                    {
                        changeEquiped = true;
                    }
                    if ((item.BoxId == (uint)BoxIDEnum.BoxIdCrystal))
                    {
                        changeCrystal = true;
                    }
                    if ((item.BoxId == (uint)BoxIDEnum.BoxIdOrnament))
                    {
                        changeOrnament = true;
                    }

                    List<ItemData> itemDatas = BagItems[(int)item.BoxId];

                    //道具被创建
                    if (item.ChangeType == 2)
                    {
                        ItemData itemData = PoolManager.Fetch(typeof(ItemData)) as ItemData;
                        itemData.SetData((int)item.BoxId, item.Item.Uuid, item.Item.Id, item.Item.Count, item.Item.Position, item.Item.ShowNewIcon,
                            item.Item.Bind, item.Item.Equipment, item.Item.Essence, item.Item.Marketendtime, null, item.Item.Crystal, item.Item.Ornament, item.Item.PetEquip, item.Item.Islocked);
                        itemData.outTime = item.Item.OutTime;
                        itemDatas.Add(itemData);
                        itemDatas.Sort((x, y) => x.Position.CompareTo(y.Position));
                        changeItems.Add(itemData);

                        if (!InfoItems.TryGetValue(item.Item.Id, out HashSet<ulong> items))
                        {
                            items = new HashSet<ulong>();
                            InfoItems.Add(item.Item.Id, items);
                        }
                        items.Add(itemData.Uuid);

                        if (item.BoxId == (uint)BoxIDEnum.BoxIdTemporary)
                        {
                            uint src_boxid = itemData.cSVItemData.box_id;
                            string tabName = CSVBoxType.Instance.GetConfData(src_boxid).tab_name;
                            string str = CSVLanguage.Instance.GetConfData(uint.Parse(tabName)).words;
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000921, str));
                        }
                        else if (item.BoxId == (uint)BoxIDEnum.BoxIdCrystal)
                        {
                            if (Sys_ElementalCrystal.Instance.durabilityType == 1)
                            {
                                Sys_ElementalCrystal.Instance.eventEmitter.Trigger(Sys_ElementalCrystal.EEvents.OnSetActiveMenuCrystal, false);
                            }
                        }
                        uint typeId = itemData.cSVItemData.type_id;
                        if (typeId == (int)EItemType.Equipment)
                        {
                            if (item.BoxId == (int)BoxIDEnum.BoxIdNormal)
                            {
                                PushUseItem(itemData);
                            }
                        }
                        else if (typeId == (int)EItemType.Crystal)
                        {
                            if (item.BoxId == (int)BoxIDEnum.BoxIdNormal && !Sys_ElementalCrystal.Instance.bEquiped || Sys_ElementalCrystal.Instance.curEquipCrystalDurability == 0)
                            {
                                PushUseItem(itemData);
                            }
                        }
                        else if (typeId == (int)EItemType.Ornament)
                        {
                            if (item.BoxId == (int)BoxIDEnum.BoxIdNormal && Sys_Ornament.Instance.CheckNeedUseItem(itemData))
                            {
                                PushUseItem(itemData);
                            }
                        }
                        else if (itemData.cSVItemData.quick_use == 1 && itemData.BoxId != (uint)BoxIDEnum.BoxIdTemporary && item.BoxId != (uint)BoxIDEnum.BoxIdBank)
                        {
                            if (Sys_Role.Instance.Role.Level >= itemData.cSVItemData.use_lv)
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

                        //装备可以被卸下,卸下的时候不算新 (装备不可以堆叠 只需要在道具被创建的时候判断)
                        if (item.Item.ShowNewIcon && item.BoxId >= 1 && item.BoxId <= 4)
                        {
                            //additemlist
                            if (!AddItemList.TryGetValue(itemData.Id, out ItemChangeData itemChangeData))
                            {
                                itemChangeData = new ItemChangeData();
                                AddItemList.Add(itemData.Id, itemChangeData);
                            }
                            uint quality = itemData.Quality;
                            itemChangeData.count += itemData.Count;
                            if ((typeId == (int)EItemType.Equipment || typeId == (int)EItemType.Pet) && itemChangeData.count > 1)
                            {
                                quality = 1;
                            }
                            itemChangeData.quality = quality;
                        }

                        isSendItemsChange = true;
                        // eventEmitter.Trigger<int, int>(EEvents.OnRefreshChangeData, 1, curBoxId);
                    }
                    //道具被删除
                    else if (item.ChangeType == 3)
                    {
                        ItemData itemData = null;
                        for (int i = itemDatas.Count - 1; i >= 0; --i)
                        {
                            if (itemDatas[i].Uuid == item.Item.Uuid)
                            {
                                itemData = itemDatas[i];

                                if (InfoItems.TryGetValue(item.Item.Id, out HashSet<ulong> items))
                                {
                                    items.Remove(itemData.Uuid);
                                }
                                itemData.SetCount(0);
                                itemDatas.RemoveAt(i);
                                break;
                            }
                        }
                        if (itemData == null)
                        {
                            DebugUtil.LogErrorFormat("没有找到要删除的item");
                        }
                        else
                        {
                            if (itemData.cSVItemData.type_id == 3018)//宠物封印
                            {
                                usePetSeal = true;
                            }
                            if (item.BoxId == (uint)BoxIDEnum.BoxIdBank)
                            {
                                eventEmitter.Trigger(EEvents.OnClearSafeBoxSelect);
                            }
                            else if (item.BoxId == (uint)BoxIDEnum.BoxIdCrystal)
                            {
                                if (Sys_ElementalCrystal.Instance.durabilityType == 1)
                                {
                                    Sys_ElementalCrystal.Instance.eventEmitter.Trigger(Sys_ElementalCrystal.EEvents.OnSetActiveMenuCrystal, false);
                                }
                            }
                            //删除的装备移出队列
                            if (itemData.cSVItemData.type_id == (uint)EItemType.Equipment)
                            {
                                DelUseItem(itemData);
                            }
                            else
                            {
                                for (int i = listUseItems.Count - 1; i >= 0; i--)
                                {
                                    if (listUseItems[i] == itemData.Uuid)
                                    {
                                        listUseItems.RemoveAt(i);
                                    }
                                }
                            }
                            changeItems.Add(itemData);

                            /*if (itemData.cSVItemData.type_id == 1601) // 杂物- 钓竿-标枪
                            {
                                Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent2, EMagicAchievement.Event53, itemData.Id);
                            }*/

                            eventEmitter.Trigger<ItemData>(EEvents.OnNtfDelItem, itemData);
                            eventEmitter.Trigger<ulong>(EEvents.OnDeleteItem, itemData.Uuid);
                            isSendItemsChange = true;
                            // eventEmitter.Trigger<int, int>(EEvents.OnRefreshChangeData, 1, curBoxId);
                            //PoolManager.Recycle(itemData);
                            needPushPool.Add(itemData);
                        }
                    }
                    //道具被更新
                    else if (item.ChangeType == 4)
                    {
                        for (int i = itemDatas.Count - 1; i >= 0; --i)
                        {
                            if (itemDatas[i].Uuid == item.Item.Uuid)
                            {
                                ItemData newItemData = PoolManager.Fetch(typeof(ItemData)) as ItemData;
                                newItemData.SetData((int)item.BoxId, item.Item.Uuid, item.Item.Id, item.Item.Count, item.Item.Position, item.Item.ShowNewIcon,
                                    item.Item.Bind, item.Item.Equipment, item.Item.Essence, item.Item.Marketendtime, null, item.Item.Crystal, item.Item.Ornament, item.Item.PetEquip, item.Item.Islocked);
                                newItemData.outTime = item.Item.OutTime;
                                uint oldCount = itemDatas[i].Count;
                                uint newCount = newItemData.Count;

                                /*if (oldCount > newCount)
                                {
                                    if (itemDatas[i].cSVItemData.type_id == 1601) // 杂物- 钓竿-标枪
                                    {
                                        Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent2, EMagicAchievement.Event53, itemDatas[i].Id);
                                    }
                                }*/

                                //additemlist
                                if ((item.BoxId >= 1 && item.BoxId <= 4)|| item.BoxId== (uint)BoxIDEnum.BoxIdShapeshift)
                                {
                                    if (newCount > oldCount)
                                    {
                                        long changeCount = newCount - oldCount;
                                        if (!AddItemList.TryGetValue(newItemData.Id, out ItemChangeData itemChangeData))
                                        {
                                            itemChangeData = new ItemChangeData();
                                            AddItemList.Add(newItemData.Id, itemChangeData);
                                        }
                                        uint quality = newItemData.Quality;
                                        uint type_id = newItemData.cSVItemData.type_id;
                                        itemChangeData.count += changeCount;
                                        if (type_id == (int)EItemType.Equipment || type_id == (int)EItemType.Pet && newItemData.Count > 1)
                                        {
                                            quality = 1;
                                        }
                                        itemChangeData.quality = quality;
                                    }
                                }

                                //useItem Logic
                                if (newItemData.cSVItemData.type_id != (uint)EItemType.Equipment && newItemData.cSVItemData.quick_use == 1 &&
                                    newItemData.BoxId != (uint)BoxIDEnum.BoxIdTemporary && newItemData.BoxId != (uint)BoxIDEnum.BoxIdBank)
                                {
                                    if (Sys_Role.Instance.Role.Level >= newItemData.cSVItemData.use_lv && GetItemMapUseState(newItemData) != 0)
                                    {
                                        if (newItemData.cSVItemData.FunctionOpenId == 0)
                                        {
                                            if (newCount > oldCount)
                                            {
                                                for (uint k = 0, _count = newCount - oldCount; k < _count; k++)
                                                {
                                                    listUseItems.Add(item.Item.Uuid);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Sys_FunctionOpen.Instance.IsOpen(newItemData.cSVItemData.FunctionOpenId))
                                            {
                                                if (newCount > oldCount)
                                                {
                                                    for (uint k = 0, _count = newCount - oldCount; k < _count; k++)
                                                    {
                                                        listUseItems.Add(item.Item.Uuid);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (newItemData.cSVItemData.type_id == (uint)EItemType.Ornament)
                                {
                                    if (newCount > oldCount && Sys_Ornament.Instance.CheckNeedUseItem(itemDatas[i]))
                                    {
                                        listUseItems.Add(item.Item.Uuid);
                                    }
                                }
                                //crystal
                                if (changeCrystal)
                                {
                                    if (itemDatas[i].crystal.Durability >= (uint)(itemDatas[i].maxDurability * 0.1f) && newItemData.crystal.Durability < (uint)(itemDatas[i].maxDurability * 0.1f))
                                    {
                                        Sys_ElementalCrystal.Instance.durabilityType = 0;
                                        Sys_ElementalCrystal.Instance.eventEmitter.Trigger(Sys_ElementalCrystal.EEvents.OnSetActiveMenuCrystal, true);
                                    }
                                    if (newItemData.crystal.Durability == 0)
                                    {
                                        Sys_ElementalCrystal.Instance.durabilityType = 1;
                                        ItemData itemData = GetFirstCrystalCanEquip();
                                        if (itemData != null)
                                        {
                                            PushUseItem(itemData);
                                        }
                                    }
                                }
                                if (itemDatas[i].cSVItemData.type_id == (uint)EItemType.PetEquipment)
                                {
                                    Sys_Pet.Instance.ShowPetEquipSmeltAttrChange(item.Item);
                                }
                                PoolManager.Recycle(itemDatas[i]);
                                itemDatas[i] = newItemData;
                                changeItems.Add(newItemData);
                                isSendItemsChange = true;
                                if (item.BoxId == (uint)BoxIDEnum.BoxIdTemporary)
                                {
                                    uint src_boxid = newItemData.cSVItemData.box_id;
                                    string tabName = CSVBoxType.Instance.GetConfData(src_boxid).tab_name;
                                    string str = CSVLanguage.Instance.GetConfData(uint.Parse(tabName)).words;
                                    string content = string.Format(LanguageHelper.GetTextContent(1000921), str);
                                    Sys_Hint.Instance.PushContent_Normal(content);
                                }
                            }
                        }

                    }
                }

                UpdateItemCount();

                if (isSendItemsChange)
                {
                    int count = needPushPool.Count;

                    eventEmitter.Trigger<int, int>(EEvents.OnRefreshChangeData, 1, curBoxId);

                    for (int i = 0; i < count; i++)
                        PoolManager.Recycle(needPushPool[i]);

                    needPushPool.Clear();
                }

            }
            #endregion

            #region 货币
            if (res.Currencys.Count > 0)
            {
                for (int i = 0; i < res.Currencys.Count; i++)
                {
                    if (i >= currency.Length)
                    {
                        DebugUtil.LogErrorFormat("不存在货币 id={0}", i);
                    }
                    else
                    {
                        currency[i] = res.Currencys[i];
                    }
                }
            }
            //货币变更
            if (res.Currencychange.Count > 0)
            {
                for (int i = 0; i < res.Currencychange.Count; i++)
                {
                    var data = res.Currencychange[i];
                    uint id = data.ItemID;
                    long value = GetItemCount(data.ItemID) + data.ChangeCount;

                    currency[(int)data.ItemID] = value;

                    /*if (ECurrencyType.Vitality == (ECurrencyType)data.ItemID && data.ChangeCount < 0)
                    {
                        Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.DrainFatigue);
                    }*/
                    eventEmitter.Trigger<uint, long>(EEvents.OnCurrencyChanged, id, value);
                    if (data.ChangeCount > 0)
                    {
                        //if (!AddItemList.ContainsKey(data.ItemID))
                        //{
                        //    AddItemList.Add(data.ItemID, data.ChangeCount);
                        //}

                        if (!AddItemList.TryGetValue(data.ItemID, out ItemChangeData itemChangeData))
                        {
                            itemChangeData = new ItemChangeData();
                            AddItemList.Add(data.ItemID, itemChangeData);
                        }
                        itemChangeData.count += data.ChangeCount;
                        itemChangeData.quality = 1;
                    }
                    else
                    {
                        if (data.ItemID == 5)  //活力消耗
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021711, Mathf.Abs(data.ChangeCount).ToString(), value.ToString()));
                        }
                    }
                }
                eventEmitter.Trigger<int, int>(EEvents.OnRefreshChangeData, 1, curBoxId);
            }
            #endregion

            #region  获得新道具提示
            for (int i = 0; i < res.ChangeCount.Count; i++)
            {
                if (res.ChangeCount[i].ChangeCount > 0)
                {
                    if (res.Reason == (uint)LifeSkillActiveReason.BuildEquipment || res.Reason == (uint)LifeSkillActiveReason.BuildMedicine || m_ForbidItemGetReason.Contains(res.Reason))
                    {
                        continue;
                    }
                    //if (!AddItemList.ContainsKey(res.ChangeCount[i].ItemID))
                    //{
                    //    AddItemList.Add(res.ChangeCount[i].ItemID, res.ChangeCount[i].ChangeCount);
                    //}
                }
            }
            foreach (var item in AddItemList)
            {
                if (res.Reason != (uint)LuckyDrawActiveReason.Draw && res.Reason != (uint)SignActiveReason.DailySignDraw && res.Reason != (uint)LuckyDrawActiveReason.MountDraw)
                {
                    AddItemQueue.Enqueue(item.Key);
                }
                long count = AddItemList[item.Key].count;
                uint quality = AddItemList[item.Key].quality;
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(item.Key);
                if (null != cSVItemData)
                {
                    CSVLanguage.Data cSVLanguageData = CSVLanguage.Instance.GetConfData(cSVItemData.name_id);
                    if (null != cSVLanguageData)
                    {
                        string Name = cSVLanguageData.words;

                        string content = LanguageHelper.GetTextContent(1000935, Constants.gHintColors_Items[quality - 1], Name, count.ToString());

                        uint lanId = 0;
                        if (res.Reason == (uint)PetActiveReason.AbandonPet)       //贩卖宠物
                        {
                            lanId = 400000002;
                        }
                        else if (res.Reason == (uint)PetActiveReason.CatchAutoAbandon)  //自动贩卖
                        {
                            lanId = 400000003;
                        }
                        else
                        {
                            lanId = 400000001;
                        }
                        string content_Chat = LanguageHelper.GetTextContent(lanId, Constants.gChatColors_Items[quality - 1], Name, count.ToString());
                        if (res.Reason != (uint)LuckyDrawActiveReason.Draw && res.Reason != (uint)SignActiveReason.DailySignDraw && res.Reason != (uint)LuckyDrawActiveReason.MountDraw)
                        {
                            Sys_Hint.Instance.PushContent_GetReward(content, item.Key);
                            Sys_Chat.Instance.PushMessage(ChatType.Person, null, content_Chat, Sys_Chat.EMessageProcess.None);
                        }
                        eventEmitter.Trigger<string>(EEvents.OnGetItem, content);
                    }
                }
            }
            AddItemList.Clear();
            #endregion


            #region Frozen

            if (res.Frozen != null && res.Frozen.Frozens != null)
            {
                m_FrozenCurrencyInfos.Clear();
                for (int i = 0, count = res.Frozen.Frozens.Count; i < count; i++)
                {
                    List<FrozenCurrencyInfo> frozenCurrencyInfos = new List<FrozenCurrencyInfo>();
                    uint currencyId = res.Frozen.Frozens[i].CurrencyId;
                    for (int j = 0, n = res.Frozen.Frozens[i].Infos.Count; j < n; j++)
                    {
                        frozenCurrencyInfos.Add(res.Frozen.Frozens[i].Infos[j]);
                    }
                    m_FrozenCurrencyInfos.Add(currencyId, frozenCurrencyInfos);
                }
            }

            #endregion

            CheckTempBag();
            CheckAnyMainBagFull();

            #region EventProcess

            //发送更新身上装备通知
            if (changeEquiped)
            {
                eventEmitter.Trigger(EEvents.OnChageEquiped);
            }
            //发送更新身上水晶装备通知
            if (changeCrystal)
            {
                eventEmitter.Trigger(EEvents.OnChangeCrystal);
            }
            //发送更新身上饰品通知
            if (changeOrnament)
            {
                eventEmitter.Trigger(EEvents.OnChangeOrnament);
            }
            if (res.Reason == (uint)ItemActiveReason.UseItemBattle)
            {
                if (usePetSeal)
                {
                    if (GetCountByItemType(3018, 1) == 0)//宠物封印卡
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11788));
                    }
                }
            }
            eventEmitter.Trigger(EEvents.ComposeSuccess);
            eventEmitter.Trigger<int>(EEvents.OnRefreshMainBagData, curBoxId);
            eventEmitter.Trigger(EEvents.OnRefreshTemporaryBagData);
            eventEmitter.Trigger<int>(EEvents.OnRefreshSafeBoxBagData, curSafeBoxTabId);
            Sys_Fashion.Instance.eventEmitter.Trigger(Sys_Fashion.EEvents.OnUpdatePropRoot);
            Sys_Fashion.Instance.eventEmitter.Trigger(Sys_Fashion.EEvents.OnUpdateUnLockButton);
            Sys_Fashion.Instance.eventEmitter.Trigger(Sys_Fashion.EEvents.OnUpdateDyePropRoot);
            Sys_Fashion.Instance.eventEmitter.Trigger(Sys_Fashion.EEvents.OnCheckDyeColorDirty);
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChangeItemCount);
            Sys_Bag.Instance.eventEmitter.Trigger<bool>(EEvents.OnRefreshBagFull, bAnyMainBagFull);
            Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnUpdateNumber);
            useItemReq = false;
            #endregion
            
            //装备部位
            Sys_Equip.Instance.OnNotifySlotData(res);
        }

        //清除新图标标志
        public void ClearNewIconReq(int curTab)
        {
            if (!BagItems.ContainsKey(curTab))
            {
                return;
            }
            List<ItemData> itemDatas = BagItems[curTab];
            bool needReq = false;
            int index = itemDatas.Count - 1;
            while (index >= 0)
            {
                if (itemDatas[index].bNew)
                {
                    needReq = true;
                    break;
                }
                --index;
            }
            if (needReq)
            {
                CmdItemCleanItemNewIconReq cmdItemCleanItemNewIconReq = new CmdItemCleanItemNewIconReq();
                cmdItemCleanItemNewIconReq.BoxID = (uint)curTab;
                NetClient.Instance.SendMessage((ushort)CmdItem.CleanItemNewIconReq, cmdItemCleanItemNewIconReq);
            }
        }

        public void OnClearNewIconRes(NetMsg netMsg)
        {
            CmdItemCleanItemNewIconRes res = NetMsgUtil.Deserialize<CmdItemCleanItemNewIconRes>(CmdItemCleanItemNewIconRes.Parser, netMsg);
            if (!BagItems.ContainsKey((int)res.BoxID))
            {
                DebugUtil.LogErrorFormat("背包中不存在{0}页签", res.BoxID);
                return;
            }
            List<ItemData> itemDatas = BagItems[(int)res.BoxID];
            foreach (ItemData item in itemDatas)
            {
                item.bNew = false;
            }
        }


        public void UnLockGridReq(uint boxid, uint level)
        {
            if (boxid == 5)
            {
                if (GetCurSafeBoxLevelUpCost() > GetItemCount(2))
                {
                    long needCount = GetCurSafeBoxLevelUpCost();
                    Sys_Bag.Instance.TryOpenExchangeCoinUI(ECurrencyType.GoldCoin, needCount);
                    //string content = string.Format(CSVLanguage.Instance.GetConfData(1000934).words, CSVLanguage.Instance.GetConfData(1000907).words);
                    //Sys_Hint.Instance.PushContent_Normal(content);
                    return;
                }
            }
            else
            {
                if (GetCurMainBagBoxLevelUpCost() > GetItemCount(2))
                {
                    long needCount = GetCurMainBagBoxLevelUpCost();
                    Sys_Bag.Instance.TryOpenExchangeCoinUI(ECurrencyType.GoldCoin, needCount);
                    //string content = string.Format(CSVLanguage.Instance.GetConfData(1000934).words, CSVLanguage.Instance.GetConfData(1000907).words);
                    //Sys_Hint.Instance.PushContent_Normal(content);
                    return;
                }
            }
            CmdItemUnlockBoxLevelReq cmdItemUnlockBoxLevelReq = new CmdItemUnlockBoxLevelReq();
            cmdItemUnlockBoxLevelReq.BoxID = boxid;
            cmdItemUnlockBoxLevelReq.ToLevel = level;
            NetClient.Instance.SendMessage((ushort)CmdItem.UnlockBoxLevelReq, cmdItemUnlockBoxLevelReq);
        }

        private void OnUnLockGridRes(NetMsg netMsg)
        {
            CmdItemUnlockBoxLevelRes res = NetMsgUtil.Deserialize<CmdItemUnlockBoxLevelRes>(CmdItemUnlockBoxLevelRes.Parser, netMsg);
            if (!Grids.ContainsKey(res.BoxID))
            {
                DebugUtil.LogErrorFormat("不存在id为{0}的boxid", res.BoxID);
                return;
            }
            Grids[res.BoxID] = res.BoxLevel;
            eventEmitter.Trigger<int>(EEvents.OnRefreshMainBagData, curBoxId);
            eventEmitter.Trigger<int>(EEvents.OnUnLuckBox, curBoxId);
            eventEmitter.Trigger(EEvents.OnUpdateSafeBoxTab);
            CheckAnyMainBagFull();
            Sys_Bag.Instance.eventEmitter.Trigger<bool>(EEvents.OnRefreshBagFull, bAnyMainBagFull);
        }

        #region UseItem
        public void UseItemByUuid(ulong uuid, uint count)
        {
            CmdItemUseItemReq cmdItemUseItemReq = new CmdItemUseItemReq();
            cmdItemUseItemReq.Uuid = uuid;
            cmdItemUseItemReq.Count = count;
            NetClient.Instance.SendMessage((ushort)CmdItem.UseItemReq, cmdItemUseItemReq);
        }

        // 使用道具，在 UseItemByUuid 的基础上加入特殊道具附加逻辑的入口
        public void UsetItem(uint itemID, ulong uuid, uint count)
        {
            var data = CSVItem.Instance.GetConfData(itemID);

            if (data != null && data.type_id == 1405)//大地鼠彩票
            {
                UIManager.OpenUI(EUIID.UI_Pub, false, new UIPubParmas() { UUID = uuid, Count = count });
            }
            else
            {
                if (data != null && data.type_id == 1702)
                {
                    if (!Sys_FunctionOpen.Instance.IsOpen(10501, false))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10574));
                        return;
                    }
                }
                UseItemByUuid(uuid, count);
            }
        }

        //背包内使用道具返回
        private void OnUseItemRes(NetMsg netMsg)
        {
            useItemReq = false;
            CmdItemUseItemRes cmdItemUseItemRes = NetMsgUtil.Deserialize<CmdItemUseItemRes>(CmdItemUseItemRes.Parser, netMsg);
            List<uint> itemIds = new List<uint>();                      //返回的道具id
            List<uint> itemNums = new List<uint>();                     //返回的道具id对应的数量  两个list长度一致
            uint useitemInfoId = cmdItemUseItemRes.InfoId;
            foreach (var item in cmdItemUseItemRes.ItemIds)
            {
                itemIds.Add(item);
            }
            foreach (var item in cmdItemUseItemRes.ItemNums)
            {
                itemNums.Add(item);
            }
            eventEmitter.Trigger<uint, List<uint>, List<uint>>(EEvents.OnUseItemRes, useitemInfoId, itemIds, itemNums);

            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(useitemInfoId);
            if (cSVItemData != null)
            {
                if (cSVItemData.fun_parameter == "drop1")
                {
                    UI_Rewards_Result.ItemRewardParms itemRewardParms = new UI_Rewards_Result.ItemRewardParms();
                    for (int i = 0; i < cmdItemUseItemRes.ItemIds.Count; i++)
                    {
                        itemRewardParms.itemIds.Add(cmdItemUseItemRes.ItemIds[i]);
                    }
                    for (int i = 0; i < cmdItemUseItemRes.ItemNums.Count; i++)
                    {
                        itemRewardParms.itemCounts.Add(cmdItemUseItemRes.ItemNums[i]);
                    }
                    UIManager.OpenUI(EUIID.UI_Rewards_Result, false, itemRewardParms);
                }
                else if (cSVItemData.fun_parameter == "FxGift")
                {
                    int m_euid = (int)cSVItemData.fun_value[1];
                    UI_Rewards_GetNew.ItemFxBagParms itemFxParms = new UI_Rewards_GetNew.ItemFxBagParms();
                    for (int i = 0; i < cmdItemUseItemRes.ItemIds.Count; i++)
                    {
                        itemFxParms.itemIds.Add(cmdItemUseItemRes.ItemIds[i]);
                    }
                    for (int i = 0; i < cmdItemUseItemRes.ItemNums.Count; i++)
                    {
                        itemFxParms.itemCounts.Add(cmdItemUseItemRes.ItemNums[i]);
                    }
                    UIManager.OpenUI(m_euid, false, itemFxParms);
                }
                
            }
        }

        public void UseItemById(uint id, uint count)
        {
            CmdItemUseItemByItemIDReq cmdItemUseItemByItemIDReq = new CmdItemUseItemByItemIDReq();
            cmdItemUseItemByItemIDReq.Id = id;
            cmdItemUseItemByItemIDReq.Count = count;
            NetClient.Instance.SendMessage((ushort)CmdItem.UseItemByItemIdreq, cmdItemUseItemByItemIDReq);
        }

        #endregion

        //道具鉴定
        public void RefreshItemReq(ulong uID)
        {
            CmdItemRefreshItemReq req = new CmdItemRefreshItemReq();
            req.Uuid = uID;
            NetClient.Instance.SendMessage((ushort)CmdItem.RefreshItemReq, req);
        }

        public void SaleItem(ulong uuid, uint count)
        {
            CmdItemSellItemReq cmdItemSellItemReq = new CmdItemSellItemReq();
            cmdItemSellItemReq.Uuid = uuid;
            cmdItemSellItemReq.Count = count;
            NetClient.Instance.SendMessage((ushort)CmdItem.SellItemReq, cmdItemSellItemReq);
        }

        public void MergeTemporaryBoxReq()
        {
            CmdItemMergeTemporaryBoxReq cmdItemMergeTemporaryBoxReq = new CmdItemMergeTemporaryBoxReq();
            NetClient.Instance.SendMessage((ushort)CmdItem.MergeTemporaryBoxReq, cmdItemMergeTemporaryBoxReq);
        }

        public void TransformItemReq(ulong uuid, uint toBox, uint pos)
        {
            CmdItemTransformItemReq cmdItemTransformItemReq = new CmdItemTransformItemReq();
            cmdItemTransformItemReq.Uuid = uuid;
            cmdItemTransformItemReq.ToBoxID = toBox;
            cmdItemTransformItemReq.Position = pos;
            cmdItemTransformItemReq.NpcUId = curInteractiveNPC;
            NetClient.Instance.SendMessage((ushort)CmdItem.TransformItemReq, cmdItemTransformItemReq);
        }

        private void TransformItemRes(NetMsg netMsg)
        {

        }


        public void ItemExchangeCurrencyReq(int exchangeType, int fromType, long Consumecount, long changeCount)
        {
            CmdItemExchangeCurrencyReq cmdItemExchangeCurrencyReq = new CmdItemExchangeCurrencyReq();
            if (exchangeType == 2)//金币
            {
                cmdItemExchangeCurrencyReq.ChangeDiamond = -Consumecount;
                cmdItemExchangeCurrencyReq.ChangeGold = changeCount;
            }
            else if (exchangeType == 3)//银币
            {
                if (fromType == 1) //用金币兑换
                {
                    cmdItemExchangeCurrencyReq.ChangeGold = -Consumecount;
                    cmdItemExchangeCurrencyReq.ChangeSliver = changeCount;
                }
                else if (fromType == 0) //用魔币兑换
                {
                    cmdItemExchangeCurrencyReq.ChangeDiamond = -Consumecount;
                    cmdItemExchangeCurrencyReq.ChangeSliver = changeCount;
                }
            }
            NetClient.Instance.SendMessage((ushort)CmdItem.ExchangeCurrencyReq, cmdItemExchangeCurrencyReq);
        }


        public void OnDecomposeItemReq(ulong uuid, uint num)
        {
            CmdItemDecomposeItemReq cmdItemDecomposeItemReq = new CmdItemDecomposeItemReq();
            cmdItemDecomposeItemReq.ItemUUID = uuid;
            cmdItemDecomposeItemReq.Num = num;
            NetClient.Instance.SendMessage((ushort)CmdItem.DecomposeItemReq, cmdItemDecomposeItemReq);
        }

        private void OnDecomposeItemRes(NetMsg netMsg)
        {
        }

        public void OnDisCardItemReq(ulong uuid)
        {
            CmdItemDisCardItemReq cmdItemDisCardItemReq = new CmdItemDisCardItemReq();
            cmdItemDisCardItemReq.ItemUUID = uuid;
            NetClient.Instance.SendMessage((ushort)CmdItem.DisCardItemReq, cmdItemDisCardItemReq);
        }

        private void OnDisCardItemRes(NetMsg netMsg)
        {
        }

        public void OnComposeItemReq(uint composeId, uint num)
        {
            CmdItemComposeItemReq cmdItemComposeItemReq = new CmdItemComposeItemReq();
            cmdItemComposeItemReq.ComposeId = composeId;
            cmdItemComposeItemReq.Num = num;
            NetClient.Instance.SendMessage((ushort)CmdItem.ComposeItemReq, cmdItemComposeItemReq);
        }

        private void OnComposeItemRes(NetMsg netMsg)
        {
        }

        #region Crystal
        //水晶装备
        public void EquipCrystalReq(ulong uuid)
        {
            CmdItemEquipCrystalReq cmdItemEquipCrystalReq = new CmdItemEquipCrystalReq();
            cmdItemEquipCrystalReq.Uuid = uuid;
            NetClient.Instance.SendMessage((ushort)CmdItem.EquipCrystalReq, cmdItemEquipCrystalReq);
        }

        private void EquipCrystalRes(NetMsg netMsg)
        {
            CmdItemEquipCrystalAck cmdItemEquipCrystalAck = NetMsgUtil.Deserialize<CmdItemEquipCrystalAck>(CmdItemEquipCrystalAck.Parser, netMsg);
        }

        //count1范围1到100
        public void ExchangeCrystalReq(uint infoId1, uint count1, uint infoId2)
        {
            CmdItemExchangeCrystalReq cmdItemExchangeCrystalReq = new CmdItemExchangeCrystalReq();
            cmdItemExchangeCrystalReq.InfoId1 = infoId1;
            cmdItemExchangeCrystalReq.Count1 = count1;
            cmdItemExchangeCrystalReq.InfoId2 = infoId2;
            NetClient.Instance.SendMessage((ushort)CmdItem.ExchangeCrystalReq, cmdItemExchangeCrystalReq);
        }

        private void ExchangeCrystalRes(NetMsg netMsg)
        {
            CmdItemExchangeCrystalAck cmdItemEquipCrystalAck = NetMsgUtil.Deserialize<CmdItemExchangeCrystalAck>(CmdItemExchangeCrystalAck.Parser, netMsg);
            Sys_ElementalCrystal.Instance.eventEmitter.Trigger(Sys_ElementalCrystal.EEvents.OnRefreshExchangeProp);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000508));
        }

        #endregion

        #region OptionalGift

        public void OptionalGiftPackReq(ulong uuid, Dictionary<uint, uint> _singleOptional)
        {
            CmdItemOptionalGiftPackReq cmdItemOptionalGiftPackReq = new CmdItemOptionalGiftPackReq();
            cmdItemOptionalGiftPackReq.Uuid = uuid;
            foreach (var item in _singleOptional)
            {
                singleOptional singleOptional = new singleOptional();
                singleOptional.Index = item.Key;
                singleOptional.Count = item.Value;
                cmdItemOptionalGiftPackReq.OptionalInfo.Add(singleOptional);
            }
            NetClient.Instance.SendMessage((ushort)CmdItem.OptionalGiftPackReq, cmdItemOptionalGiftPackReq);
        }

        private void OnItemOptionalGiftRes(NetMsg netMsg)
        {
            CmdItemOptionalGiftPackRes cmdItemOptionalGiftPackRes = NetMsgUtil.Deserialize<CmdItemOptionalGiftPackRes>(CmdItemOptionalGiftPackRes.Parser, netMsg);
            UI_Rewards_Result.ItemRewardParms itemRewardParms = new UI_Rewards_Result.ItemRewardParms();
            for (int i = 0; i < cmdItemOptionalGiftPackRes.ItemIds.Count; i++)
            {
                itemRewardParms.itemIds.Add(cmdItemOptionalGiftPackRes.ItemIds[i]);
            }
            for (int i = 0; i < cmdItemOptionalGiftPackRes.ItemNums.Count; i++)
            {
                itemRewardParms.itemCounts.Add(cmdItemOptionalGiftPackRes.ItemNums[i]);
            }
            UIManager.OpenUI(EUIID.UI_Rewards_Result, false, itemRewardParms);
        }

        #endregion

        #region FreezeCurrency
        public void EnergyExChangeGoldReq()
        {
            CmdItemEnchantEquipmentReq cmdItemEnchantEquipmentReq = new CmdItemEnchantEquipmentReq();
            NetClient.Instance.SendMessage((ushort)CmdItem.EnergyExChangeGoldReq, cmdItemEnchantEquipmentReq);
        }

        private void OnEnergyExChangeGoldRes(NetMsg netMsg)
        {
            CmdItemEnergyExChangeGoldRes cmdItemEnergyExChangeGoldRes = NetMsgUtil.Deserialize<CmdItemEnergyExChangeGoldRes>(CmdItemEnergyExChangeGoldRes.Parser, netMsg);
        }

        private void OnFreezeCurrencyNtf(NetMsg netMsg)
        {
            CmdItemFreezeCurrencyNtf ntf = NetMsgUtil.Deserialize<CmdItemFreezeCurrencyNtf>(CmdItemFreezeCurrencyNtf.Parser, netMsg);
            if (m_FrozenCurrencyInfos.TryGetValue(ntf.CurrencyId, out List<FrozenCurrencyInfo> infos))
            {
                infos.Add(ntf.Info);
            }
            else
            {
                List<FrozenCurrencyInfo> frozenCurrencyInfos = new List<FrozenCurrencyInfo>();
                frozenCurrencyInfos.Add(ntf.Info);
                m_FrozenCurrencyInfos.Add(ntf.CurrencyId, frozenCurrencyInfos);
            }
            eventEmitter.Trigger(EEvents.OnFrozenCurrency);
        }

        private void OnUnfreezeCurrencyNtf(NetMsg netMsg)
        {
            CmdItemUnfreezeCurrencyNtf ntf = NetMsgUtil.Deserialize<CmdItemUnfreezeCurrencyNtf>(CmdItemUnfreezeCurrencyNtf.Parser, netMsg);
            List<uint> remove = new List<uint>();
            foreach (var item in m_FrozenCurrencyInfos)
            {
                List<FrozenCurrencyInfo> infos = item.Value;
                for (int i = infos.Count - 1; i >= 0; --i)
                {
                    if (infos[i].UnfreezeTime <= ntf.UnfreezeTime)
                    {
                        infos.RemoveAt(i);
                        if (infos.Count == 0)
                        {
                            remove.Add(item.Key);
                        }
                    }
                }
            }
            for (int i = 0, count = remove.Count; i < count; i++)
                m_FrozenCurrencyInfos.Remove(remove[i]);

            eventEmitter.Trigger(EEvents.OnFrozenCurrency);
        }

        private void OnForceUnfreezeCurrencyNtf(NetMsg netMsg)
        {
            CmdItemForceUnfreezeCurrencyNtf ntf = NetMsgUtil.Deserialize<CmdItemForceUnfreezeCurrencyNtf>(CmdItemForceUnfreezeCurrencyNtf.Parser, netMsg);
            m_FrozenCurrencyInfos.Remove(ntf.CurrencyId);
            eventEmitter.Trigger(EEvents.OnFrozenCurrency);
        }

        #endregion

        #endregion

        #region BatchClear

        public List<ItemData> itemsCanSale = new List<ItemData>();

        public List<ItemData> itemsCanDel = new List<ItemData>();

        public RepeatedField<uint> itemIds;

        public RepeatedField<uint> itemNums;

        public void ClearBagReq()
        {
            itemsCanDel = GetAllBagItemsCanDel();
            itemsCanSale = GetAllBagItemsCanSale();
            if (itemsCanDel.Count == 0 && itemsCanSale.Count == 0)
            {
                string content = LanguageHelper.GetTextContent(1012013);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            UIManager.OpenUI(EUIID.UI_Bag_Clear);
        }
        //出售列表中不能包含可以分解的
        public List<ItemData> GetAllBagItemsCanSale()
        {
            List<ItemData> itemDatas = new List<ItemData>();
            List<ItemData> item_1 = BagItems[1];
            List<ItemData> item_3 = BagItems[3];
            for (int i = 0; i < item_1.Count; i++)
            {
                ItemData itemData = item_1[i];
                if (itemsCanDel.Contains(itemData))
                {
                    continue;
                }
                if (itemData.cSVItemData.Recommend_Sale == 1)
                {
                    //不是玩家打造的装备
                    if (itemData.cSVItemData.type_id == (uint)EItemType.Equipment)
                    {
                        if (itemData.Equip != null && itemData.Equip.BuildType == 0u)
                        {
                            itemDatas.Add(item_1[i]);
                        }
                    }
                    else
                    {
                        itemDatas.Add(item_1[i]);
                    }
                }
            }
            for (int i = 0; i < item_3.Count; i++)
            {
                ItemData itemData = item_3[i];
                if (itemsCanDel.Contains(itemData))
                {
                    continue;
                }
                if (itemData.cSVItemData.Recommend_Sale == 1)
                {
                    //不是玩家打造的装备
                    if (itemData.cSVItemData.type_id == (uint)EItemType.Equipment && itemData.Equip != null && itemData.Equip.BuildType == 0u)
                    {
                        itemDatas.Add(item_3[i]);
                    }
                    else
                    {
                        itemDatas.Add(item_3[i]);
                    }
                }
            }
            return itemDatas;
        }

        public List<ItemData> GetAllBagItemsCanDel()
        {
            List<ItemData> itemDatas = new List<ItemData>();
            List<ItemData> item_1 = BagItems[1];
            List<ItemData> item_3 = BagItems[3];
            for (int i = 0; i < item_1.Count; i++)
            {
                ItemData itemData = item_1[i];
                bool isEquiped = Sys_Equip.Instance.IsEquiped(itemData);
                bool isBind = itemData.bBind;
                bool isMake = itemData.Equip != null && itemData.Equip.BuildType != 0u;

                if (!isEquiped && !isBind && isMake && (itemData.Quality == (uint)EQualityType.White || itemData.Quality == (uint)(uint)EQualityType.Green
                    || itemData.Quality == (uint)(uint)EQualityType.Blue))
                {
                    itemDatas.Add(itemData);
                }
                if (itemData.cSVItemData.Recommend_Undo > 0)
                {
                    itemDatas.Add(itemData);
                }
            }
            for (int i = 0; i < item_3.Count; i++)
            {
                ItemData itemData = item_3[i];
                bool isEquiped = Sys_Equip.Instance.IsEquiped(itemData);
                bool isBind = itemData.bBind;
                bool isMake = itemData.Equip != null && itemData.Equip.BuildType != 0u;

                if (!isEquiped && !isBind && isMake && (itemData.Quality == (uint)EQualityType.White || itemData.Quality == (uint)(uint)EQualityType.Green
                    || itemData.Quality == (uint)(uint)EQualityType.Blue))
                {
                    itemDatas.Add(itemData);
                }
                if (itemData.cSVItemData.Recommend_Undo > 0)
                {
                    itemDatas.Add(itemData);
                }
            }
            return itemDatas;
        }

        public void BatchSellReq(List<ulong> uuIDs)
        {
            CmdItemBatchSellReq req = new CmdItemBatchSellReq();
            for (int i = 0; i < uuIDs.Count; i++)
            {
                req.Uuid.Add(uuIDs[i]);
            }
            NetClient.Instance.SendMessage((ushort)CmdItem.BatchSellReq, req);
        }

        private void OnBatchSellRes(NetMsg netMsg)
        {
            CmdItemBatchSellRes res = NetMsgUtil.Deserialize<CmdItemBatchSellRes>(CmdItemBatchSellRes.Parser, netMsg);
            eventEmitter.Trigger(EEvents.OnAllSale, res.Addsilvers);
        }

        public void BatchDeComposeReq(List<ulong> uuIDs)
        {
            CmdItemBatchDeComposeReq req = new CmdItemBatchDeComposeReq();
            for (int i = 0; i < uuIDs.Count; i++)
            {
                req.Uuid.Add(uuIDs[i]);
            }
            NetClient.Instance.SendMessage((ushort)CmdItem.BatchDeComposeReq, req);
        }

        private void OnBatchDeComposeRes(NetMsg netMsg)
        {
            CmdItemBatchDeComposeRes res = NetMsgUtil.Deserialize<CmdItemBatchDeComposeRes>(CmdItemBatchDeComposeRes.Parser, netMsg);
            itemIds = res.Itemid;
            itemNums = res.ItemNum;
            eventEmitter.Trigger(EEvents.OnAllDel);
        }
        private void OnDayLimitUpdateNtf(NetMsg msg)
        {
            CmdItemDayLimitUpdateNtf res = NetMsgUtil.Deserialize<CmdItemDayLimitUpdateNtf>(CmdItemDayLimitUpdateNtf.Parser, msg);
            //全部更新
            if (res.IsUpdateAll)
            {
                dayLimitItemUsedDic.Clear();
            }
            if (res.Infos != null && res.Infos.Count > 0)
            {
                for (int i = 0; i < res.Infos.Count; i++)
                {
                    dayLimitItemUsedDic[res.Infos[i].InfoId] = res.Infos[i].UseCount;
                }
            }
        }

        public void OnItemLockReq(ulong uId, bool islock, uint petUid = 0)
        {
            CmdItemLockItemReq req = new CmdItemLockItemReq();
            req.Uuid = uId;
            req.Islocked = islock;
            req.Petuid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdItem.LockItemReq, req);
        }

        private void OnItemLockRes(NetMsg msg)
        {
            CmdItemLockItemRes res = NetMsgUtil.Deserialize<CmdItemLockItemRes>(CmdItemLockItemRes.Parser, msg);
            if (res.Petuid != 0)
            {
                ClientPet pet = Sys_Pet.Instance.GetPetByUId(res.Petuid);
                Packet.Item data = null;
                List<Packet.Item> items = pet.GetPetEquipItems();
                if (items != null)
                {
                    for (int i = 0; i < items.Count; ++i)
                    {
                        if (items[i].Uuid == res.Uuid)
                        {
                            data = items[i];
                            data.Islocked = res.Islocked;
                            break;
                        }
                    }
                }

                if (data != null)
                {
                    uint lanId = res.Islocked ? 15246u : 15247u;
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(lanId));
                    eventEmitter.Trigger(EEvents.OnItemPetEquipLockedChange, res.Petuid);
                }
                return;
            }
            
            ItemData item = Sys_Bag.Instance.GetItemDataByUuid(res.Uuid);
            if (item != null)
            {
                uint lanId = 0;
                if (item.cSVItemData.type_id == (int) EItemType.Equipment)
                {
                    lanId = res.Islocked ? 15242u : 15243u;
                }
                else if (item.cSVItemData.type_id == (int) EItemType.PetEquipment)
                {
                    lanId = res.Islocked ? 15246u : 15247u;
                }
                else if (item.cSVItemData.type_id == (int) EItemType.Ornament)
                {
                    lanId = res.Islocked ? 15244u : 15245u;
                }
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(lanId));
                
                item.IsLocked = res.Islocked;
                if (res.Petuid != 0)
                {
                    eventEmitter.Trigger(EEvents.OnItemPetEquipLockedChange, res.Petuid);
                }
                else
                {
                    eventEmitter.Trigger(EEvents.OnItemLockedChange);
                }
            }
            else
            {
                DebugUtil.LogError("背包里找不到 道具：" + res.Uuid);
            }
        }
        #endregion
    }
}



