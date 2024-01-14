using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Packet;
using Table;
using static Packet.NpcFavorabilityItem.Types;

namespace Logic {
    public enum EFavorabilityBahaviourType {
        Clue = 1,
        Talk = 2,
        Dance = 3,
        Play = 4,
        SendGift = 5,
        Fete = 6,
    }

    // 消耗好感点， 增加与特定npc的好感度
    public class FavorabilityNPC {
        public uint id { get; private set; }
        private CSVNPCFavorability.Data _csvNPCFavorability;
        public CSVNPCFavorability.Data csvNPCFavorability {
            get {
                if (this._csvNPCFavorability == null) {
                    this._csvNPCFavorability = CSVNPCFavorability.Instance.GetConfData(this.id);
                }
                return this._csvNPCFavorability;
            }
        }

        public CSVNPCFavorabilityStage.Data csvNPCFavorabilityStage {
            get {
                return CSVNPCFavorabilityStage.Instance.GetConfData(this.favorabilityStageId);
            }
        }
        private CSVNpc.Data _csvNPC;
        public CSVNpc.Data csvNPC {
            get {
                if (this._csvNPC == null) {
                    this._csvNPC = CSVNpc.Instance.GetConfData(this.id);
                }
                return this._csvNPC;
            }
        }

        public static uint GetFavorabilityStageId(uint id, uint stage/*1 ~ 5*/) {
            // 好感id * 10 + 好感阶段
            return id * 10 + stage;
        }

        // 好感度
        public uint favorability { get; private set; }
        // 好感阶段,server通知修改
        public uint favorabilityStage { get; private set; } = 0;
        public uint favorabilityStageId {
            get {
                return GetFavorabilityStageId(this.id, this.favorabilityStage);
            }
        }

        public bool IsTaskSubmited {
            get {
                return Sys_Task.Instance.IsSubmited(this.csvNPCFavorabilityStage.WishTask);
            }
        }

        // 是否是新解锁的npc
        public bool IsNew { get; set; } = false;

        // 是否探索过
        public bool isExplored { get; private set; }
        public uint healthValue { get; private set; }

        public bool IsReachedCurrentStageLimit {
            get {
                return this.favorability >= this.csvNPCFavorabilityStage.FavorabilityValueMax;
            }
        }
        public bool IsNPCReachedMax {
            get {
                // 如果下一个stage表格没填写，则表示当前是最大等级
                return CSVNPCFavorabilityStage.Instance.GetConfData(this.favorabilityStageId + 1) == null;
            }
        }

        public uint zoneId {
            get {
                return this.csvNPCFavorability.Place;
            }
        }

        // 疾病id
        public uint healthId { get; private set; }
        // 心情id
        public uint moodId { get; private set; }
        // 心情值
        public uint moodValue { get; private set; }

        // actId: count
        public Dictionary<uint, uint> acts { get; private set; } = new Dictionary<uint, uint>();
        public bool IsOpen(EFavorabilityBahaviourType type) {
            CSVFavorabilityBehavior.Data csv = CSVFavorabilityBehavior.Instance.GetConfData((uint)type);
            if (csv != null) {
                return Sys_FunctionOpen.Instance.IsOpen(csv.FunctionOpenid, false);
            }
            return false;
        }
        public static uint UsedTotalActTimes(EFavorabilityBahaviourType bahaviourType) {
            uint times = 0;
            foreach (var kvp in Sys_NPCFavorability.Instance.npcs) {
                if (kvp.Value.acts.TryGetValue((uint)bahaviourType, out uint t)) {
                    times += t;
                }
            }
            return times;
        }
        public uint RemainActTime(EFavorabilityBahaviourType bahaviourType) {
            uint remain = 0;
            CSVFavorabilityBehavior.Data csv = CSVFavorabilityBehavior.Instance.GetConfData((uint)bahaviourType);
            if (csv != null) {
                this.acts.TryGetValue((uint)bahaviourType, out uint behaviourCount);

                uint npcRemain = 0;
                uint totalRemain = 0;
                if (csv.DailyTotal != 0) {
                    totalRemain = csv.DailyTotal - UsedTotalActTimes(bahaviourType);
                    if (csv.DailyTimes != 0) {
                        npcRemain = csv.DailyTimes - behaviourCount;
                    }
                    else {    // 无穷
                        npcRemain = uint.MaxValue;
                    }
                }
                else { // 无穷
                    totalRemain = uint.MaxValue;
                    if (csv.DailyTimes != 0) {
                        npcRemain = csv.DailyTimes - behaviourCount;
                    }
                    else {    // 无穷
                        npcRemain = uint.MaxValue;
                    }
                }

                remain = Math.Min(npcRemain, totalRemain);
            }
            return remain;
        }

        // 喜欢的物品的解锁
        public List<uint> unlockGiftTypes { get; private set; } = new List<uint>();
        // 是否某个礼物类型已经被解锁
        public bool IsGiftTypeUnlock(uint giftType) {
            return this.unlockGiftTypes.Contains(giftType);
        }

        private List<uint> _likesGiftTypes = null;
        public List<uint> likesGiftTypes {
            get {
                if (this._likesGiftTypes == null) {
                    this._likesGiftTypes = new List<uint>();
                    if (this.csvNPCFavorability.GiftTpye1 != 0)
                        this._likesGiftTypes.Add(this.csvNPCFavorability.GiftTpye1);
                    if (this.csvNPCFavorability.GiftTpye2 != 0)
                        this._likesGiftTypes.Add(this.csvNPCFavorability.GiftTpye2);
                    if (this.csvNPCFavorability.GiftTpye3 != 0)
                        this._likesGiftTypes.Add(this.csvNPCFavorability.GiftTpye3);
                }
                return this._likesGiftTypes;
            }
        }

        // 是否为喜欢的礼物
        public bool IsLikedGift(uint itemId) {
            bool isLike = false;
            CSVFavorabilityGift.Data csvGift = CSVFavorabilityGift.Instance.GetConfData(itemId);
            if (csvGift != null) {
                isLike = this.unlockGiftTypes.Contains(csvGift.Gift_Type);
            }
            return isLike;
        }

        public bool isSickness {
            get {
                return this.healthId != 0;
            }
        }

        // 治病药品
        // 药品不受 道具绑定规则 的限制
        public List<ItemIdCount> Drugs {
            get {
                List<ItemIdCount> drugs = new List<ItemIdCount>();
                var csv = CSVNPCDisease.Instance.GetConfData(this.healthId);
                if (csv != null) {
                    if (csv.ItemID1 != 0) {
                        drugs.Add(new ItemIdCount(csv.ItemID1, csv.Num1));
                    }
                    if (csv.ItemID2 != 0) {
                        drugs.Add(new ItemIdCount(csv.ItemID2, csv.Num2));
                    }
                    if (csv.ItemID3 != 0) {
                        drugs.Add(new ItemIdCount(csv.ItemID3, csv.Num3));
                    }
                    if (csv.ItemID4 != 0) {
                        drugs.Add(new ItemIdCount(csv.ItemID4, csv.Num4));
                    }
                }
                return drugs;
            }
        }

        private static Dictionary<uint, List<ItemIdCount>> _foods = new Dictionary<uint, List<ItemIdCount>>();
        public static List<ItemIdCount> GetFoods(uint id) {
            if (!_foods.TryGetValue(id, out List<ItemIdCount> foods)) {
                foods = new List<ItemIdCount>();
                _foods.Add(id, foods);

                CSVFavorabilityBanquet.Data csv = CSVFavorabilityBanquet.Instance.GetConfData(id);
                if (csv != null) {
                    if (csv.ItemID1 != 0) {
                        foods.Add(new ItemIdCount(csv.ItemID1, csv.Num1));
                    }
                    if (csv.ItemID2 != 0) {
                        foods.Add(new ItemIdCount(csv.ItemID2, csv.Num2));
                    }
                    if (csv.ItemID3 != 0) {
                        foods.Add(new ItemIdCount(csv.ItemID3, csv.Num3));
                    }
                    if (csv.ItemID4 != 0) {
                        foods.Add(new ItemIdCount(csv.ItemID4, csv.Num4));
                    }
                    if (csv.ItemID5 != 0) {
                        foods.Add(new ItemIdCount(csv.ItemID5, csv.Num5));
                    }
                    if (csv.ItemID6 != 0) {
                        foods.Add(new ItemIdCount(csv.ItemID6, csv.Num6));
                    }
                    if (csv.ItemID7 != 0) {
                        foods.Add(new ItemIdCount(csv.ItemID7, csv.Num7));
                    }
                    if (csv.ItemID8 != 0) {
                        foods.Add(new ItemIdCount(csv.ItemID8, csv.Num8));
                    }
                    if (csv.ItemID9 != 0) {
                        foods.Add(new ItemIdCount(csv.ItemID9, csv.Num9));
                    }
                    if (csv.ItemID10 != 0) {
                        foods.Add(new ItemIdCount(csv.ItemID10, csv.Num10));
                    }
                }
            }
            return foods;
        }

        public bool HasFullDrugs {
            get {
                bool has = true;
                var drugs = this.Drugs;
                for (int i = 0, length = this.Drugs.Count; i < length; ++i) {
                    has &= drugs[i].Enough;
                }
                return has;
            }
        }
        public bool HasFullGifts(PropIconLoader.ShowItemDataExt propItem, uint neededCount) {
            return propItem.CountInBag >= neededCount;
        }

        public enum EGiftStackType {
            UnStack = 0,  // 不堆叠
            Stack = 1, // 堆叠
        }
        private readonly Dictionary<uint, PropIconLoader.ShowItemDataExt> itemDict = new Dictionary<uint, PropIconLoader.ShowItemDataExt>();

        private List<uint> _sendGifts = null;
        public List<uint> SendGifts {
            get {
                if (this._sendGifts == null) {
                    //var dict = CSVFavorabilityGift.Instance.GetDictData();
                    this._sendGifts = new List<uint>(CSVFavorabilityGift.Instance.GetKeys());
                }
                return this._sendGifts;
            }
        }
        public static readonly List<Func<ItemData, bool>> GiftsFilters = new List<Func<ItemData, bool>> {
            (item) => {
                return !item.bBind;
            },
            item => {
                return item.BoxId != (int)BoxIDEnum.BoxIdEquipment;
            },
            item => {
                if (item.cSVItemData.type_id != (uint) EItemType.Equipment) {
                    return true;
                }
                else {
                    if (Sys_SecureLock.Instance.lockState) {
                        if (Sys_Equip.Instance.IsSecureLock(item, false)) {
                            return false;
                        }
                    }

                    if (!Sys_Equip.Instance.IsInlayJewel(item)) {
                        return true;
                    }

                    return false;
                }
            },
        };

        public static readonly List<Func<ItemData, bool>> ItemsFilters = new List<Func<ItemData, bool>> {
            (item) => {
                return !item.bBind;
            },
            item => {
                return item.BoxId != (int)BoxIDEnum.BoxIdEquipment;
            },
            item => {
                if (item.cSVItemData.type_id != (uint) EItemType.Equipment) {
                    return true;
                }
                else {
                    if (!Sys_Equip.Instance.IsInlayJewel(item)) {
                        return true;
                    }

                    return false;
                }
            },
        };
        // 送礼物品
        public List<PropIconLoader.ShowItemDataExt> GiftsInBag {
            get {
                var ls = Sys_Bag.Instance.GetItemDatasByItemInfoIds(1, this.SendGifts, GiftsFilters);
                this.itemDict.Clear();
                List<PropIconLoader.ShowItemDataExt> gifts = new List<PropIconLoader.ShowItemDataExt>();
                for (int i = 0, length = ls.Count; i < length; ++i) {
                    uint id = ls[i].Id;
                    var csv = CSVFavorabilityGift.Instance.GetConfData(id);
                    if (csv != null) {
                        if ((EGiftStackType)csv.Stack == EGiftStackType.UnStack) {
                            PropIconLoader.ShowItemDataExt item = new PropIconLoader.ShowItemDataExt(ls[i].Uuid, id, ls[i].Count, csv.Num, false);
                            item.bagData = ls[i];
                            // client强制设置quality
                            item.SetQuality(ls[i].Quality);

                            gifts.Add(item);
                        }
                        else {
                            if (!this.itemDict.TryGetValue(id, out PropIconLoader.ShowItemDataExt item)) {
                                item = new PropIconLoader.ShowItemDataExt(ls[i].Uuid, id, 0, csv.Num, true);
                                this.itemDict.Add(id, item);
                            }
                            item.count += ls[i].Count;
                            item.bagData = ls[i];
                        }
                    }
                }
                foreach (var kvp in this.itemDict) {
                    gifts.Add(kvp.Value);
                }
                this.itemDict.Clear();
                return gifts;
            }
        }
        public List<PropIconLoader.ShowItemDataExt> LikeGiftsInBag {
            get {
                bool Filter(PropIconLoader.ShowItemDataExt item) {
                    var csv = CSVFavorabilityGift.Instance.GetConfData(item.id);
                    return csv != null && this.unlockGiftTypes.Contains(csv.Gift_Type);
                }
                List<PropIconLoader.ShowItemDataExt> likes = this.GiftsInBag.FindAll(Filter);
                return likes;
            }
        }
        public static List<PropIconLoader.ShowItemDataExt> FilterByGiftType(List<PropIconLoader.ShowItemDataExt> origin, IList<uint> giftTypes) {
            if (giftTypes == null || giftTypes.Count <= 0) { return origin; }

            bool Predicate(PropIconLoader.ShowItemDataExt item) {
                var csv = CSVFavorabilityGift.Instance.GetConfData(item.id);
                if (csv != null) {
                    bool ret = false;
                    for (int i = 0, length = giftTypes.Count; i < length; ++i) {
                        ret |= (csv.Gift_Type == giftTypes[i]);
                        if (ret) { return true; }
                    }
                }
                return false;
            }
            List<PropIconLoader.ShowItemDataExt> ls = origin.FindAll(Predicate);
            return ls;
        }

        public FavorabilityNPC(uint id) {
            this.id = id;
        }

        public bool CanInteractive(uint totalRelation) {
            if (!this.isExplored) {
                return false;
            }
            if (this.IsReachedCurrentStageLimit) {
                return false;
            }

            return true;
        }

        // server刷新
        public FavorabilityNPC Refresh(NpcFavorabilityItem serverNpc) {
            return this.RefreshExplored(true).RefreshHealth(serverNpc.HealthValue, serverNpc.SickId).RefreshFavorability(serverNpc.Favorability).
                    RefreshFavorabilityStage(serverNpc.FavorabilityStage).RefreshMood(serverNpc.MoodValue, serverNpc.MoodId).
                    RefreshUnlock(serverNpc.UnlockedItem).RefreshInteractive(serverNpc.ActTimes);
        }
        public FavorabilityNPC RefreshExplored(bool isExplored) {
            this.isExplored = isExplored;
            return this;
        }
        public FavorabilityNPC RefreshMood(uint mood, uint moodId) {
            this.moodValue = mood;
            this.moodId = moodId;
            return this;
        }
        public FavorabilityNPC RefreshHealth(uint health, uint sickId) {
            this.healthValue = health;
            this.healthId = sickId;
            return this;
        }
        public FavorabilityNPC RefreshFavorability(uint favorability) {
            this.favorability = favorability;
            return this;
        }
        public FavorabilityNPC RefreshFavorabilityStage(uint FavorabilityStage) {
            this.favorabilityStage = FavorabilityStage;
            return this;
        }
        public FavorabilityNPC RefreshInteractive(IList<ActTime> actList) {
            this.acts.Clear();
            for (int i = 0, length = actList.Count; i < length; ++i) {
                this.acts.Add(actList[i].Actid, actList[i].Count);
            }
            return this;
        }
        public FavorabilityNPC RefreshUnlock(IList<uint> unlocks) {
            this.unlockGiftTypes.Clear();
            if (unlocks != null) {
                for (int i = 0, length = unlocks.Count; i < length; ++i) {
                    this.unlockGiftTypes.Add(unlocks[i]);
                }
            }
            return this;
        }
        public FavorabilityNPC RefreshUnlock(uint unlock) {
            if (!this.unlockGiftTypes.Contains(unlock)) {
                this.unlockGiftTypes.Add(unlock);
            }
            return this;
        }
    }
    public class ZoneFavorabilityNPC {
        public uint zoneId { get; private set; }
        public Dictionary<uint, FavorabilityNPC> npcs { get; private set; } = new Dictionary<uint, FavorabilityNPC>();
        public bool gotRewads { get; set; } = false;
        public int Count { get { return this.npcs.Count; } }
        // public ulong unlockTime;

        // 属于当前zone的npc的总个数
        private int _count = -1;
        public int TotalCount {
            get {
                if (this._count < 0) {
                    this._count = GetTotalCount(this.zoneId);
                }
                return this._count;
            }
        }

        public static int GetTotalCount(uint zoneId) {
            int count = 0;
            foreach (var kvp in CSVNPCFavorability.Instance.GetAll()) {
                if (kvp.Place == zoneId) {
                    ++count;
                }
            }
            return count;
        }
        public static void GetIds(uint zoneId, bool active, List<uint> npcIds) {
            foreach (var kvp in CSVNPCFavorability.Instance.GetAll()) {
                if (kvp.Place == zoneId) {
                    if (active) {
                        if (Sys_NPCFavorability.Instance.TryGetNpc(kvp.id, out var npc)) {
                            npcIds.Add(kvp.id);
                        }
                    }
                    else {
                        npcIds.Add(kvp.id);
                    }
                }
            }
        }

        public int ReachedMaxCount {
            get {
                int count = 0;
                foreach (var kvp in this.npcs) {
                    count += kvp.Value.IsNPCReachedMax ? 1 : 0;
                }
                return count;
            }
        }
        // 是否该区域所有npc全部到达最大的好感度上限
        public bool IsAllNpcMax {
            get {
                bool ret = true;
                if (this.TotalCount == this.Count) {
                    foreach (var kvp in this.npcs) {
                        ret &= kvp.Value.IsNPCReachedMax;
                        if (!ret) { break; }
                    }
                }
                return ret;
            }
        }

        private List<ItemIdCount> _rewards;
        public List<ItemIdCount> rewards {
            get {
                if (this._rewards == null) {
                    this._rewards = GetRewards(this.zoneId);
                }
                return this._rewards;
            }
        }
        public static List<ItemIdCount> GetRewards(uint zoneId) {
            List<ItemIdCount> ls = new List<ItemIdCount>();
            var csv = CSVFavorabilityPlaceReward.Instance.GetConfData(zoneId);
            if (csv != null) {
                ls = CSVDrop.Instance.GetDropItem(csv.Reward);
            }
            return ls;
        }
        public CSVFavorabilityPlaceReward.Data csv {
            get {
                return CSVFavorabilityPlaceReward.Instance.GetConfData(this.zoneId);
            }
        }

        public ZoneFavorabilityNPC(uint zoneId) {
            this.zoneId = zoneId;
        }

        public void Add(FavorabilityNPC npc) {
            if (npc != null && !this.npcs.ContainsKey(npc.id)) {
                this.npcs.Add(npc.id, npc);
            }

        }
        public void Clear() {
            this.npcs.Clear();
            this.gotRewads = false;
        }
    }

    public partial class Sys_NPCFavorability : SystemModuleBase<Sys_NPCFavorability> {
        // 上次体力恢复时间
        public uint lastFavorabilityRecoveryTime { get; private set; }
        private Timer favorabilityTimer;
        private uint _favorability = 0;
        public uint Favorability {
            get {
                return this._favorability;
            }
            set {
                if (this._favorability != value) {
                    this._favorability = value;
                    this.eventEmitter.Trigger(EEvents.OnPlayerFavorabilityChanged);
                }

                this.CorrectFavorability();
                this.favorabilityTimer?.Cancel();

                uint now = Sys_Time.Instance.GetServerTime();
                float remain = this.NextFavorabilityRefreshTime - now;
                //UnityEngine.Debug.LogError("now: " + now + " lastFavorabilityRecoveryTime: " + lastFavorabilityRecoveryTime + " remain: " + remain);
                this.favorabilityTimer = Timer.Register(remain, () => {
                    ++this._favorability;
                    this.CorrectFavorability();
                    this.eventEmitter.Trigger(EEvents.OnPlayerFavorabilityChanged);
                    this.lastFavorabilityRecoveryTime = Sys_Time.Instance.GetServerTime();

                    //UnityEngine.Debug.LogError(" lastFavorabilityRecoveryTime: " + lastFavorabilityRecoveryTime);

                    this.favorabilityTimer?.Cancel();
                    this.favorabilityTimer = Timer.Register(this.FavorabilityRecoveryTime, () => {
                        ++this._favorability;
                        this.CorrectFavorability();
                        this.eventEmitter.Trigger(EEvents.OnPlayerFavorabilityChanged);
                        this.lastFavorabilityRecoveryTime = Sys_Time.Instance.GetServerTime();

                        //UnityEngine.Debug.LogError(now + " lastFavorabilityRecoveryTime: " + lastFavorabilityRecoveryTime);
                    }, null, true, true);
                }, null, false, true);
            }
        }
        // 体力是否充足
        public bool IsFullStamina {
            get {
                return this._favorability >= CSVFavorabilityStamina.Instance.GetConfData(1).MaxFavorabilityStamina;
            }
        }
        public bool AnyNpcUnlocked {
            get {
                return Sys_NPCFavorability.Instance.unluckNpcCount > 0;
            }
        }
        public bool AnyNpcReachedMax {
            get {
                foreach (var kvp in npcs) {
                    if (kvp.Value.IsNPCReachedMax) {
                        return true;
                    }
                }

                return false;
            }
        }
        public bool IsEnoughFavorability(uint toUse) {
            return this._favorability >= toUse;
        }
        public void CorrectFavorability() {
            if (this._favorability >= CSVFavorabilityStamina.Instance.GetConfData(1).MaxFavorabilityStamina) {
                this._favorability = CSVFavorabilityStamina.Instance.GetConfData(1).MaxFavorabilityStamina;
            }
        }
        public uint NextFavorabilityRefreshTime {
            get {
                return this.lastFavorabilityRecoveryTime + this.FavorabilityRecoveryTime;
            }
        }
        public uint FavorabilityRecoveryTime {
            get {
                return CSVFavorabilityStamina.Instance.GetConfData(1).Time;
            }
        }

        public Dictionary<uint, FavorabilityNPC> npcs { get; private set; } = new Dictionary<uint, FavorabilityNPC>();
        // mapid:npcId:NPC
        public Dictionary<uint, ZoneFavorabilityNPC> zoneNpcs { get; private set; } = new Dictionary<uint, ZoneFavorabilityNPC>();
        public int unluckNpcCount {
            get {
                return this.npcs.Count;
            }
        }

        public uint selectedZoneId;

        // 第一个含有解锁npc的区域
        public uint FirstZoneId {
            get {
                uint zoneId = 0;
                foreach (var kvp in this.zoneNpcs) {
                    if (kvp.Value.Count > 0) {
                        zoneId = kvp.Key;
                        break;
                    }
                }
                return zoneId;
            }
        }

        // 所有区域所有npc全部max
        public bool IsAllZoneMax {
            get {
                foreach (var kvp in this.zoneNpcs) {
                    if(!kvp.Value.IsAllNpcMax) {
                        return false;
                    }
                }
                return true;
            }
        }

        private Timer npcTimer;
        public uint NextNPCRefreshTime {
            get {
                return this._lastNPCRefreshTime + 86400;
            }
        }
        private uint _lastNPCRefreshTime;
        public uint lastNPCRefreshTime {
            get {
                return this._lastNPCRefreshTime;
            }
            private set {
                this._lastNPCRefreshTime = value;
                this.npcTimer?.Cancel();

                uint now = Sys_Time.Instance.GetServerTime();
                uint remain = this.NextNPCRefreshTime - now;
                DebugUtil.LogFormat(ELogType.eRelation, "now: " + now + " _lastNPCRefreshTime: " + this._lastNPCRefreshTime + " remain: " + remain);
                this.npcTimer = Timer.Register(remain, () => {
                    this._lastNPCRefreshTime = Sys_Time.Instance.GetServerTime();
                    this.ReqCmdFavorabilityInfo();

                    //UnityEngine.Debug.LogError(" _lastNPCRefreshTime: " + _lastNPCRefreshTime);

                    this.npcTimer?.Cancel();
                    this.npcTimer = Timer.Register(86400, () => {
                        this._lastNPCRefreshTime = Sys_Time.Instance.GetServerTime();
                        this.ReqCmdFavorabilityInfo();
                    }, null, true, true);
                }, null, false, true);
            }
        }

        private Dictionary<uint, List<uint>> _giftGroup = null;
        public List<uint> giftGroupList = new List<uint>();
        public Dictionary<uint, List<uint>> giftGroup {
            get {
                if (this._giftGroup == null) {
                    this._giftGroup = new Dictionary<uint, List<uint>>();
                    foreach (var kvp in CSVFavorabilityGiftType.Instance.GetAll()) {
                        if (!this._giftGroup.TryGetValue(kvp.type, out List<uint> ls)) {
                            ls = new List<uint>();
                            this._giftGroup.Add(kvp.type, ls);
                            this.giftGroupList.Add(kvp.type);
                        }
                        ls.Add(kvp.id);
                    }
                }
                return this._giftGroup;
            }
        }

        public void Clear() {
            this.npcs.Clear();
            foreach (var kvp in this.zoneNpcs) {
                kvp.Value.Clear();
            }
            this.zoneNpcs.Clear();
        }

        public bool TryGetNpc(uint id, out FavorabilityNPC npc, bool createIf = false) {
            if (!this.npcs.TryGetValue(id, out npc)) {
                if (createIf) {
                    npc = new FavorabilityNPC(id);
                }
                return false;
            }
            return true;
        }
        public bool GetZoneNpcs(uint zoneId, out ZoneFavorabilityNPC npc, bool createIf = false) {
            if (!this.zoneNpcs.TryGetValue(zoneId, out npc)) {
                if (createIf) {
                    npc = new ZoneFavorabilityNPC(zoneId);
                }
                return false;
            }
            return true;
        }

        public uint GetExistedTask() {
            uint id = 0;
            if (Sys_Task.Instance.receivedTasks.TryGetValue((int)ETaskCategory.NPCFavorability, out var dict) && dict.Count > 0) {
                foreach (var kvp in dict) {
                    id = kvp.Key;
                    break;
                }
            }
            return id;
        }

        // 拜访功能组点击事件
        public void OnBtnVisitClicked(uint npcId) {
            this.OpenDialogue(npcId, true, true);
        }
        public bool IsNPCReachedMax(uint npcId) {
            bool reached = false;
            if (this.TryGetNpc(npcId, out var npc, false)) {
                reached = npc.IsNPCReachedMax;
            }
            return reached;
        }
        // 获取npc好感阶段
        public bool TryGetNpcStage(uint npcId, out int stage) {
            stage = 0;
            if (this.TryGetNpc(npcId, out FavorabilityNPC npc, false)) {
                stage = (int)npc.favorabilityStage;
                return true;
            }
            return false;
        }
        // 获取npc好感度
        public uint GetNpcFavorability(uint npcId) {
            uint result = 0;
            if (this.TryGetNpc(npcId, out FavorabilityNPC npc, false)) {
                result = npc.favorability;
            }
            return result;
        }

        public void OpenDialogue(uint npcId, bool withMenu = false, bool needShowDislogueContent = true) {
            this.GetNpcStatus(npcId, out uint dialogueId, out ENPCFavorabilityType npcType);
            if (dialogueId != 0) {
                CSVDialogue.Data csvDialogue = CSVDialogue.Instance.GetConfData(dialogueId);
                List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(csvDialogue);

                ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                //ResetDialogueDataEventData resetDialogueDataEventData = Logic.Core.ObjectPool<ResetDialogueDataEventData>.Fetch(typeof(ResetDialogueDataEventData));
                resetDialogueDataEventData.Init(datas, null, csvDialogue);
                this.OpenDialogue(resetDialogueDataEventData);

                // 打开对话内容显示
                Sys_Dialogue.Instance.eventEmitter.Trigger<bool>(Sys_Dialogue.EEvents.OnShowContent, needShowDislogueContent);
            }

            if (withMenu) {
                UIManager.OpenUI(EUIID.UI_FavorabilityMenu, false, npcId);
            }
        }

        public void OpenDialogue(ResetDialogueDataEventData resetDialogueDataEventData) {
            Sys_Dialogue.Instance.currentResetDialogueDataEventData = resetDialogueDataEventData;
            if (!UIManager.IsOpen(EUIID.UI_FavorabilityDialogue)) {
                UIManager.OpenUI(EUIID.UI_FavorabilityDialogue, true, resetDialogueDataEventData);
            }
            else {
                Sys_Dialogue.Instance.eventEmitter.Trigger<ResetDialogueDataEventData>(Sys_Dialogue.EEvents.OnResetDialogueData, resetDialogueDataEventData);
            }
        }

        public void GetNpcStatus(uint npcId, out uint dialogueId, out ENPCFavorabilityType npcType) {
            dialogueId = 0;
            npcType = ENPCFavorabilityType.Normal;
            if (Sys_NPCFavorability.Instance.TryGetNpc(npcId, out var npc, false)) {
                if (npc.IsNPCReachedMax) {
                    dialogueId = 0;
                    npcType = ENPCFavorabilityType.Max;
                }
                else {
                    if (npc.IsReachedCurrentStageLimit) {
                        if (npc.IsTaskSubmited) {
                            dialogueId = npc.csvNPCFavorabilityStage.LetterDia;
                            npcType = ENPCFavorabilityType.Letter;
                        }
                        else {
                            dialogueId = npc.csvNPCFavorabilityStage.WishTaskDia;
                            npcType = ENPCFavorabilityType.WishTask;
                        }
                    }
                    else {
                        if (npc.isSickness) {
                            dialogueId = npc.csvNPCFavorabilityStage.SickDia;
                            npcType = ENPCFavorabilityType.SickNess;
                        }
                        else {
                            dialogueId = npc.csvNPCFavorabilityStage.InteractiveDia;
                            npcType = ENPCFavorabilityType.Normal;
                        }
                    }
                }
            }

            // for test
            //npcType = ENPCFavorabilityType.Normal;
        }

        public void CloseAllUI(bool include = false) {
            if (include) {
                if (UIManager.IsOpen(EUIID.UI_FavorabilityMain)) {
                    UIManager.CloseUI(EUIID.UI_FavorabilityMain);
                }
                if (UIManager.IsOpen(EUIID.UI_FavorabilityNPCShow)) {
                    UIManager.CloseUI(EUIID.UI_FavorabilityNPCShow);
                }
            }
            if (UIManager.IsOpen(EUIID.UI_FavorabilityFete)) {
                UIManager.CloseUI(EUIID.UI_FavorabilityFete);
            }
            if (UIManager.IsOpen(EUIID.UI_FavorabilityMenu)) {
                UIManager.CloseUI(EUIID.UI_FavorabilityMenu);
            }
            if (UIManager.IsOpen(EUIID.UI_FavorabilityDanceList)) {
                UIManager.CloseUI(EUIID.UI_FavorabilityDanceList);
            }
            if (UIManager.IsOpen(EUIID.UI_FavorabilityMusicList)) {
                UIManager.CloseUI(EUIID.UI_FavorabilityMusicList);
            }
            if (UIManager.IsOpen(EUIID.UI_FavorabilityFilterGifts)) {
                UIManager.CloseUI(EUIID.UI_FavorabilityFilterGifts);
            }
            if (UIManager.IsOpen(EUIID.UI_FavorabilityHealth)) {
                UIManager.CloseUI(EUIID.UI_FavorabilityHealth);
            }
            if (UIManager.IsOpen(EUIID.UI_FavorabilitySendGift)) {
                UIManager.CloseUI(EUIID.UI_FavorabilitySendGift);
            }
            if (UIManager.IsOpen(EUIID.UI_FavorabilityThanks)) {
                UIManager.CloseUI(EUIID.UI_FavorabilityThanks);
            }
            if (UIManager.IsOpen(EUIID.UI_FavorabilityClue)) {
                UIManager.CloseUI(EUIID.UI_FavorabilityClue);
            }
            if (UIManager.IsOpen(EUIID.UI_FavorabilityDialogue)) {
                UIManager.CloseUI(EUIID.UI_FavorabilityDialogue);
            }
        }
    }
}