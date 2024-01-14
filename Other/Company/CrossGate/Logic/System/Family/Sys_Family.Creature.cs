using Google.Protobuf;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.Json;
using Table;
using UnityEngine;
using UnityEngine.UI;
using static Packet.CmdGuildPetQueryRankRes.Types;

namespace Logic
{

    public class FamilyCreatureEntry
    {
        private uint familyCreatureId;
        private uint stage;
        public string Name
        {
            get
            {
                return !creature.Name.IsEmpty ? creature.Name.ToStringUtf8() : LanguageHelper.GetTextContent(Sys_Family.Instance.GetNameLangIdByType(cSV.food_Type));
            }
        }

        public bool IsFull
        {
            get
            {
                return CSVFamilyPet.Instance.GetConfData(ConfigId + 1) == null;
            }
        }

        public bool IsTrain
        {
            get
            {
                GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
                if (null == guildPetTraining)
                {
                    return false;
                }
                else
                {
                    return guildPetTraining.TrainingStage / 10 == familyCreatureId;
                }

            }
        }

        public CSVFamilyPet.Data cSV
        {
            get
            {
                return CSVFamilyPet.Instance.GetConfData(ConfigId);
            }
        }

        public float CurrentMoodRatio
        {
            get
            {
                for (int i = 0; i < CSVFamilyPetMood.Instance.Count; i++)
                {
                    CSVFamilyPetMood.Data cSVFamilyPetMoodData = CSVFamilyPetMood.Instance.GetByIndex(i);
                    if (null != cSVFamilyPetMoodData.Range && cSVFamilyPetMoodData.Range.Count >= 2)
                    {
                        if (cSVFamilyPetMoodData.Range[0] <= creature.Mood && creature.Mood <= cSVFamilyPetMoodData.Range[1])
                        {
                            return cSVFamilyPetMoodData.addGrowthRatio / 10000.0f;
                        }
                    }
                }
                return 0f;
            }
        }

        public float CurrentHealthRatio
        {
            get
            {
                for (int i = 0; i < CSVFamilyPetHealth.Instance.Count; i++)
                {
                    CSVFamilyPetHealth.Data healthData = CSVFamilyPetHealth.Instance.GetByIndex(i);
                    if (null != healthData.Range && healthData.Range.Count >= 2)
                    {
                        if (healthData.Range[0] <= creature.Mood && creature.Mood <= healthData.Range[1])
                        {
                            return healthData.addGrowthRatio / 10000.0f;
                        }
                    }
                }
                return 0f;
            }
        }

        public GuildPetUnit creature;
        /// <summary>
        /// 家族兽的属性id
        /// </summary>
        public uint FmilyCreatureId { get => familyCreatureId; set => familyCreatureId = value; }
        /// <summary>
        /// 家族兽的阶id
        /// </summary>
        public uint ConfigId { get => familyCreatureId * 10 + stage; }
        public void Reset(GuildPetUnit guildPetUnit)
        {
            creature = guildPetUnit;
            familyCreatureId = guildPetUnit.Id;
            stage = guildPetUnit.Stage;
        }

        public string GetCreatureOld()
        {
            uint dTime = Sys_Time.Instance.GetServerTime() - creature.ActiveTime;
            uint day = dTime / 86400; //获得天数

            uint month = day * 3;
            uint year = month / 12;
            month = month % 12;
            string str = string.Empty;
            if (year > 0)
            {
                if (month == 0)
                {
                    str = LanguageHelper.GetTextContent(2023362, year.ToString());
                }
                else
                {
                    str = LanguageHelper.GetTextContent(2023363, year.ToString(), month.ToString());
                }
            }
            else
            {
                if (month > 0)
                {
                    str = LanguageHelper.GetTextContent(2023361, month.ToString());
                }
                else
                {
                    str = LanguageHelper.GetTextContent(2023360);
                }
            }
            return str;
        }

        public enum EFoodStackType
        {
            UnStack = 0,  // 不堆叠
            Stack = 1, // 堆叠
        }
        private readonly Dictionary<uint, PropIconLoader.ShowItemDataExt> itemDict = new Dictionary<uint, PropIconLoader.ShowItemDataExt>();

        private List<uint> _sendGifts = null;
        public List<uint> SendGifts
        {
            get
            {
                if (this._sendGifts == null)
                {
                    //this._sendGifts = new List<uint>(CSVFamilyPetFood.Instance.Count);
                    //for (int i = 0; i < CSVFamilyPetFood.Instance.Count; i++)
                    //{
                    //    _sendGifts.Add(CSVFamilyPetFood.Instance[i].id);
                    //}
                    this._sendGifts = new List<uint>(CSVFamilyPetFood.Instance.GetKeys());
                }
                return this._sendGifts;
            }
        }

        private readonly List<Func<ItemData, bool>> GiftsFilters = new List<Func<ItemData, bool>> {
            (item) => {
                return !item.bBind;
            },
            item => {
                return item.BoxId != (int)BoxIDEnum.BoxIdEquipment;
            }
        };

        // 所有道具
        public List<PropIconLoader.ShowItemDataExt> GiftsInBag
        {
            get
            {
                var ls = Sys_Bag.Instance.GetItemDatasByItemInfoIds(2, this.SendGifts, this.GiftsFilters);
                /*for (int i = 0; i < this.SendGifts.Count; i++)
                {
                    ls.AddRange(Sys_Bag.Instance.GetItemDatasByItemType(this.SendGifts[i], this.GiftsFilters, BoxIDEnum.BoxIdBank));
                }*/
                this.itemDict.Clear();
                List<PropIconLoader.ShowItemDataExt> gifts = new List<PropIconLoader.ShowItemDataExt>();
                for (int i = 0, length = ls.Count; i < length; ++i)
                {
                    uint id = ls[i].Id;
                    var csv = CSVFamilyPetFood.Instance.GetConfData(id);
                    if (csv != null)
                    {
                        if ((EFoodStackType)csv.stack == EFoodStackType.UnStack)
                        {
                            PropIconLoader.ShowItemDataExt item = new PropIconLoader.ShowItemDataExt(ls[i].Uuid, id, ls[i].Count, csv.num, false);
                            item.bagData = ls[i];
                            // client强制设置quality
                            item.SetQuality(ls[i].Quality);

                            gifts.Add(item);
                        }
                        else
                        {
                            if (!this.itemDict.TryGetValue(id, out PropIconLoader.ShowItemDataExt item))
                            {
                                item = new PropIconLoader.ShowItemDataExt(ls[i].Uuid, id, 0, csv.num, true);
                                this.itemDict.Add(id, item);
                            }
                            item.count += ls[i].Count;
                            item.bagData = ls[i];
                        }
                    }
                }
                foreach (var kvp in this.itemDict)
                {
                    gifts.Add(kvp.Value);
                }
                this.itemDict.Clear();
                return gifts;
            }
        }

        public List<PropIconLoader.ShowItemDataExt> FilterByGiftType(uint giftTypes)
        {
            bool Predicate(PropIconLoader.ShowItemDataExt item)
            {
                var csv = CSVFamilyPetFood.Instance.GetConfData(item.id);
                if (csv != null)
                {
                    bool ret = false;
                    ret |= (csv.food_Type == giftTypes);
                    if (ret) { return true; }
                }
                return false;
            }
            List<PropIconLoader.ShowItemDataExt> ls = GiftsInBag.FindAll(Predicate);
            PropIconLoader.ShowItemDataExt none = new PropIconLoader.ShowItemDataExt();
            none.id = 0;
            ls.Add(none);
            return ls;
        }
    }

    /// <summary> 家族系统-家族兽 </summary>
    public partial class Sys_Family : SystemModuleBase<Sys_Family>
    {
        float grothTypeRatio = 0;
        /// <summary>
        /// 成长值-属性匹配系数
        /// </summary>
        public float GrothTypeRatio
        {
            get
            {
                if (this.grothTypeRatio == 0)
                {
                    grothTypeRatio = Convert.ToSingle(CSVParam.Instance.GetConfData(1201).str_value) / 10000f;
                }
                return grothTypeRatio;
            }
        }

        float moodLoveRatio = 0;
        /// <summary>
        /// 心情值-道具喜爱系数
        /// </summary>
        public float MoodLoveRatio
        {
            get
            {
                if (this.moodLoveRatio == 0)
                {
                    moodLoveRatio = Convert.ToSingle(CSVParam.Instance.GetConfData(1202).str_value) / 10000f;
                }
                return moodLoveRatio;
            }
        }

        float moodTypeRatio = 0;
        /// <summary>
        /// 心情值-属性匹配系数
        /// </summary>
        public float MoodTypeRatio
        {
            get
            {
                if (this.moodTypeRatio == 0)
                {
                    moodTypeRatio = Convert.ToSingle(CSVParam.Instance.GetConfData(1203).str_value) / 10000f;
                }
                return moodTypeRatio;
            }
        }

        float healthTypeRatio = 0;
        /// <summary>
        /// 健康值值-属性匹配系数
        /// </summary>
        public float HealthTypeRatio
        {
            get
            {
                if (this.healthTypeRatio == 0)
                {
                    healthTypeRatio = Convert.ToSingle(CSVParam.Instance.GetConfData(1204).str_value) / 10000f;
                }
                return healthTypeRatio;
            }
        }

        float feedExpRatio = 0;

        public float FeedExpRatio
        {
            get
            {
                if (this.feedExpRatio == 0)
                {
                    feedExpRatio = Convert.ToSingle(CSVParam.Instance.GetConfData(1232).str_value);
                }
                return feedExpRatio;
            }
        }

        public List<FamilyCreatureEntry> familyCreatureEntries;

        private GuildPetTraining trainInfo;
        public uint totalScore; // 家族总训练积分
        public uint score; // 个人积分
        public uint FightCount = 0;
        public bool showActiveOpen = true;

        private void FamilyCreaturesLogin()
        {
            openFamilyUiAway = 0;
            isFamilyCreatureNeedShowGetTips = true;
            showPopInfoTimer?.Cancel();
            trainShowTimer?.Cancel();
            familyCreatureEntries?.Clear();
            totalScore = 0;
            score = 0;
            trainInfo = null;
            showActiveOpen = true;
            showTrainTip = true;
            ClearCreatureDB();
            LoadFamilyCreatureDB();
            LoadFamilyCreatureNoticeDB();
        }

        private void FamilyCreaturesLogout()
        {
            openFamilyUiAway = 0;
            isFamilyCreatureNeedShowGetTips = true;
            showPopInfoTimer?.Cancel();
            trainShowTimer?.Cancel();
            familyCreatureEntries?.Clear();
            totalScore = 0;
            score = 0;
            trainInfo = null;
            showActiveOpen = true;
            showTrainTip = true;
        }
        /// <summary>
        /// 获取家族兽信息
        /// </summary>
        public void GuildPetGetInfoReq()
        {
            CmdGuildPetGetInfoReq req = new CmdGuildPetGetInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.GetInfoReq, req);
        }

        private void OnGuildPetGetInfoRes(NetMsg msg)
        {
            CmdGuildPetGetInfoRes res = NetMsgUtil.Deserialize<CmdGuildPetGetInfoRes>(CmdGuildPetGetInfoRes.Parser, msg);
            if (null == familyCreatureEntries)
            {
                familyCreatureEntries = new List<FamilyCreatureEntry>(res.GuildPet.Count);
            }
            familyCreatureEntries?.Clear();
            for (int i = 0; i < res.GuildPet.Count; i++)
            {
                bool isNew = true;
                for (int j = 0; j < familyCreatureEntries.Count; j++)
                {
                    if (familyCreatureEntries[j].FmilyCreatureId == res.GuildPet[i].Id)
                    {
                        isNew = false;
                        familyCreatureEntries[j].Reset(res.GuildPet[i]);
                        break;
                    }
                }
                if (isNew)
                {
                    FamilyCreatureEntry temp = new FamilyCreatureEntry();
                    temp.Reset(res.GuildPet[i]);
                    familyCreatureEntries.Add(temp);
                }
            }

            trainInfo = res.TrainingInfo;
            if (showActiveOpen && Constants.FamilyMapId == Sys_Map.Instance.CurMapId)
            {
                if (IsInTrainTime())
                {
                    showActiveOpen = false;
                    Sys_Family.Instance.eventEmitter.Trigger(EEvents.OnFamilyPetTrainStarOrEnd);
                    PopupParam param = new PopupParam();
                    param.type = 1;
                    PopFamilyPetInfo(param);
                    showPopInfoTimer?.Cancel();
                    showPopInfoTimer = Timer.Register((uint)GetFamilyCreatureTrainTime(), () =>
                    {
                        Sys_Family.Instance.eventEmitter.Trigger(EEvents.OnFamilyPetTrainStarOrEnd);
                        PopupParam param2 = new PopupParam();
                        param2.type = 2;
                        PopFamilyPetInfo(param2);
                    });
                }
                else if (IsFamilyCreaturesOpenDate())
                {
                    Sys_Ini.Instance.Get<IniElement_Int>(1264, out IniElement_Int durationTime);
                    ///当前时间
                    var currentTime = Sys_Time.Instance.GetServerTime();
                    //当日的0点时间戳
                    ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
                    var setTime = zeroTime + trainInfo.StartTime;
                    if (currentTime < setTime)
                    {
                        showPopInfoTimer?.Cancel();
                        showPopInfoTimer = Timer.Register(setTime - currentTime, () =>
                        {
                            showActiveOpen = false;
                            Sys_Family.Instance.eventEmitter.Trigger(EEvents.OnFamilyPetTrainStarOrEnd);
                            PopupParam param3 = new PopupParam();
                            param3.type = 1;
                            PopFamilyPetInfo(param3);
                            showPopInfoTimer?.Cancel();
                            showPopInfoTimer = Timer.Register(durationTime.value, () =>
                            {
                                Sys_Family.Instance.eventEmitter.Trigger(EEvents.OnFamilyPetTrainStarOrEnd);
                                PopupParam param4 = new PopupParam();
                                param4.type = 2;
                                PopFamilyPetInfo(param4);
                            });

                        });
                    }
                }
            }
            bool hasGet = Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.FamilyPetEgg);
            if (hasGet && isFamilyCreatureNeedShowGetTips)
            {
                CheckCanGetFamilyCreature();
                isFamilyCreatureNeedShowGetTips = false;
            }

            if (IsInTrainTime())
            {
                CheckCanTrainCreature();
            }
            else
            {
                SetTrainTimer();
            }
            familyNoticVer = res.NoticeVer;
            eventEmitter.Trigger(EEvents.OnGetFamilyPetInfo);
        }

        private Timer showPopInfoTimer;


        /// <summary>
        /// 获取喂食信息
        /// </summary>
        /// <param name="id"></param>
        public void GuildPetGetFeedInfoReq()
        {
            CmdGuildPetGetFeedInfoReq req = new CmdGuildPetGetFeedInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.GetFeedInfoReq, req);
        }

        public uint hungerCount = 0;
        public uint feedCount = 0;
        /// <summary>
        /// 下次刷新时间
        /// </summary>
        private uint dailyRefreshTime;
        private uint dailyFeedCount;
        public uint DailyFeedCount
        {
            get
            {
                var currentTime = Sys_Time.Instance.GetServerTime();
                if (currentTime > dailyRefreshTime)
                {
                    dailyFeedCount = 0;
                    //当日的0点时间戳
                    ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
                    dailyRefreshTime = (uint)zeroTime + 86400;
                }
                return dailyFeedCount;
            }
        }

        private void OnGuildPetGetFeedInfoRes(NetMsg msg)
        {
            CmdGuildPetGetFeedInfoRes res = NetMsgUtil.Deserialize<CmdGuildPetGetFeedInfoRes>(CmdGuildPetGetFeedInfoRes.Parser, msg);
            hungerCount = res.HungerCount;
            feedCount = res.FeedCount;
            dailyFeedCount = res.DailyFeedCount;
            //当日的0点时间戳
            ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            dailyRefreshTime = (uint)zeroTime + 86400;
            eventEmitter.Trigger(EEvents.OnGetFamilyPetFeedInfo);
        }

        /// <summary>
        /// 获取单个家族兽信息
        /// </summary>
        /// <param name="id"></param>
        public void GuildPetUpdatePetInfoReq(uint id)
        {
            CmdGuildPetUpdatePetInfoReq req = new CmdGuildPetUpdatePetInfoReq();
            req.Id = id;
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.UpdatePetInfoReq, req);
        }

        private void OnGuildPetUpdatePetInfoRes(NetMsg msg)
        {
            CmdGuildPetUpdatePetInfoRes res = NetMsgUtil.Deserialize<CmdGuildPetUpdatePetInfoRes>(CmdGuildPetUpdatePetInfoRes.Parser, msg);
            if (null == familyCreatureEntries)
            {
                familyCreatureEntries = new List<FamilyCreatureEntry>(4);
            }
            bool hasData = false;

            for (int i = 0; i < familyCreatureEntries.Count; i++)
            {
                FamilyCreatureEntry clientData = familyCreatureEntries[i];
                if (clientData.FmilyCreatureId == res.GuildPet.Id)
                {
                    hasData = true;
                    clientData.Reset(res.GuildPet);
                    break;
                }
            }
            if (!hasData)
            {
                FamilyCreatureEntry temp = new FamilyCreatureEntry();
                temp.Reset(res.GuildPet);
                familyCreatureEntries.Add(temp);
            }
            eventEmitter.Trigger(EEvents.OnGetFamilyPetInfo);
        }

        /// <summary>
        /// 设置家族兽训练信息
        /// </summary>
        public void GuildPetSetTrainingReq(GuildPetTraining info)
        {
            CmdGuildPetSetTrainingReq req = new CmdGuildPetSetTrainingReq();
            req.TrainingInfo = info;
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.SetTrainingReq, req);
        }

        private void OnGuildPetSetTrainingRes(NetMsg msg)
        {
            CmdGuildPetSetTrainingRes res = NetMsgUtil.Deserialize<CmdGuildPetSetTrainingRes>(CmdGuildPetSetTrainingRes.Parser, msg);
            trainInfo = res.TrainingInfo;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023421));
            eventEmitter.Trigger(EEvents.OnSetTrainInfoEnd);
        }

        /// <summary>
        /// 领取家族兽
        /// </summary>
        public void GuildPetGetGuildPetReq(uint id)
        {
            CmdGuildPetGetGuildPetReq req = new CmdGuildPetGetGuildPetReq();
            req.Id = id;
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.GetGuildPetReq, req);
        }

        private void OnGuildPetGetGuildPetRes(NetMsg msg)
        {
            CmdGuildPetGetGuildPetRes res = NetMsgUtil.Deserialize<CmdGuildPetGetGuildPetRes>(CmdGuildPetGetGuildPetRes.Parser, msg);
            if (null == familyCreatureEntries)
            {
                familyCreatureEntries = new List<FamilyCreatureEntry>(4);
            }
            FamilyCreatureEntry temp = new FamilyCreatureEntry();
            temp.Reset(res.GuildPet);
            familyCreatureEntries.Add(temp);
            if (trainInfo == null)
            {
                trainInfo = new GuildPetTraining();
                trainInfo.TrainingStage = res.GuildPet.Id * 10 + res.GuildPet.Stage;
                trainInfo.StartTime = Convert.ToUInt32(CSVParam.Instance.GetConfData(1261).str_value);
                trainInfo.BSet = false;
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_Get))
            {
                if (familyCreatureEntries.Count == 1)
                {
                    UIManager.CloseUI(EUIID.UI_FamilyCreatures_Get);
                    UIManager.OpenUI(EUIID.UI_FamilyCreatures);
                }
                else
                {
                    UIManager.CloseUI(EUIID.UI_FamilyCreatures_Get);
                }
            }
            eventEmitter.Trigger(EEvents.OnGetFamilyPetInfo);
        }


        /// <summary>
        /// 喂食家族兽
        /// </summary>
        public void GuildPetFeedReq(uint id, uint itemId)
        {
            CmdGuildPetFeedReq req = new CmdGuildPetFeedReq();
            req.Id = id;
            req.ItemId = itemId;
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.FeedReq, req);
        }

        private void OnGuildPetFeedRes(NetMsg msg)
        {
            CmdGuildPetFeedRes res = NetMsgUtil.Deserialize<CmdGuildPetFeedRes>(CmdGuildPetFeedRes.Parser, msg);
            bool stageUp = false;
            for (int i = 0; i < familyCreatureEntries.Count; i++)
            {
                var unit = familyCreatureEntries[i];
                if (unit.FmilyCreatureId == res.GuildPet.Id)
                {
                    stageUp = unit.creature.Stage < res.GuildPet.Stage;
                    unit.creature.Stage = res.GuildPet.Stage;
                    unit.creature.Mood = res.GuildPet.Mood;
                    unit.creature.Health = res.GuildPet.Health;
                    unit.creature.Growth = res.GuildPet.Growth;
                    unit.creature.DailyGrowth = res.GuildPet.DailyGrowth;
                    break;
                }
            }

            bool ishunger = hungerCount > 0;
            hungerCount = ishunger ? (hungerCount - 1) : 0;
            feedCount = feedCount - 1;

            var currentTime = Sys_Time.Instance.GetServerTime();
            if (currentTime > dailyRefreshTime)
            {
                GuildPetGetFeedInfoReq();
            }
            else
            {
                dailyFeedCount += 1;
            }

            PopupParam param = new PopupParam();
            param.type = 3;
            param.isHunger = ishunger;
            param.growth2Value = res.Addgrowth;
            param.mood2Value = res.Addmood;
            param.Health2Value = res.Addhealth;
            param.itemId = res.ItemId;

            PopFamilyPetInfo(param);

            eventEmitter.Trigger(EEvents.OnFamilyPetFeedEnd);
        }

        /// <summary>
        /// 获取训练积分
        /// </summary>
        public void GuildPetGetTrainingScoreReq()
        {
            CmdGuildPetGetTrainingScoreReq req = new CmdGuildPetGetTrainingScoreReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.GetTrainingScoreReq, req);
        }

        private void OnGuildPetGetTrainingScoreRes(NetMsg msg)
        {
            CmdGuildPetGetTrainingScoreRes res = NetMsgUtil.Deserialize<CmdGuildPetGetTrainingScoreRes>(CmdGuildPetGetTrainingScoreRes.Parser, msg);
            this.totalScore = res.TotalScore;
            this.score = res.Score;
            this.FightCount = res.GuildPetFightCount;
            eventEmitter.Trigger(EEvents.OnFamilyPetTrainScore);
        }

        /// <summary>
        /// 训练
        /// </summary>
        public void GuildPetEnterBattleReq()
        {
            CmdGuildPetEnterBattleReq req = new CmdGuildPetEnterBattleReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.EnterBattleReq, req);
        }

        /// <summary>
        /// 改家族兽名字
        /// </summary>
        /// <param name="id"></param>
        public void GuildPetChangeNameReq(uint id, string name)
        {
            CmdGuildPetChangeNameReq req = new CmdGuildPetChangeNameReq();
            req.Id = id;
            req.Name = FrameworkTool.ConvertToGoogleByteString(name);
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.ChangeNameReq, req);
        }

        private void OnGuildPetChangeNameRes(NetMsg msg)
        {
            CmdGuildPetChangeNameRes res = NetMsgUtil.Deserialize<CmdGuildPetChangeNameRes>(CmdGuildPetChangeNameRes.Parser, msg);
            for (int i = 0; i < familyCreatureEntries.Count; i++)
            {
                var unit = familyCreatureEntries[i];
                if (unit.FmilyCreatureId == res.Id)
                {
                    unit.creature.Name = res.Name;
                    break;
                }
            }
            eventEmitter.Trigger(EEvents.OnGetFamilyPetInfo);
        }

        /// <summary>
        /// 改家家族兽通知
        /// </summary>
        /// <param name="id"></param>
        public void GuildPetChangeNoticeReq(string notice)
        {
            CmdGuildPetChangeNoticeReq req = new CmdGuildPetChangeNoticeReq();
            req.Notice = FrameworkTool.ConvertToGoogleByteString(notice);
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.ChangeNoticeReq, req);
        }

        public string familyNotic = "";
        public uint familyNoticVer;
        private void OnGuildPetChangeNoticeRes(NetMsg msg)
        {
            CmdGuildPetChangeNoticeRes res = NetMsgUtil.Deserialize<CmdGuildPetChangeNoticeRes>(CmdGuildPetChangeNoticeRes.Parser, msg);
            familyNotic = res.Notice.ToStringUtf8();
            familyNoticVer = res.NoticeVer;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023331));
            eventEmitter.Trigger(EEvents.OnFamilyPetNotice);
        }

        /// <summary>
        /// 获取家族公告
        /// </summary>
        public void GuildPetGetNoticeReq()
        {
            CmdGuildPetGetNoticeReq req = new CmdGuildPetGetNoticeReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.GetNoticeReq, req);
        }

        private void OnGuildPetGetNoticeRes(NetMsg msg)
        {
            CmdGuildPetGetNoticeRes res = NetMsgUtil.Deserialize<CmdGuildPetGetNoticeRes>(CmdGuildPetGetNoticeRes.Parser, msg);
            familyNotic = res.Notice.ToStringUtf8();
            familyNoticVer = res.NoticeVer;
            eventEmitter.Trigger(EEvents.OnFamilyPetNotice);
        }

        public List<RankInfo> rankInfos = new List<RankInfo>();
        public void GuildPetQueryRankReq()
        {
            CmdGuildPetQueryRankReq req = new CmdGuildPetQueryRankReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildPet.QueryRankReq, req);
        }

        private void OnGuildPetQueryRankRes(NetMsg msg)
        {
            CmdGuildPetQueryRankRes res = NetMsgUtil.Deserialize<CmdGuildPetQueryRankRes>(CmdGuildPetQueryRankRes.Parser, msg);
            if (null == rankInfos)
            {
                rankInfos = new List<RankInfo>(res.RankInfo.Count);
            }
            rankInfos.Clear();
            rankInfos.AddRange(res.RankInfo);
            SetIsTranStarIsUnlockRewardState();
            eventEmitter.Trigger(EEvents.OnFamilyRankInfoBack);
        }
        /// <summary> 战斗结果 </summary>
        public CmdGuildPetFightEndNtf cmdGuildPetFightEndNtf;
        /// <summary> 战斗结束 </summary>
        private bool IsBattleOver = false;
        /// <summary>
        /// 战斗结算
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildPetFightEndNtf(NetMsg msg)
        {
            CmdGuildPetFightEndNtf res = NetMsgUtil.Deserialize<CmdGuildPetFightEndNtf>(CmdGuildPetFightEndNtf.Parser, msg);
            cmdGuildPetFightEndNtf = res;
            FightCount = res.GuildPetFightCount;
            OpenFamilyTrainResultView();
        }

        /// <summary>
        /// 战斗结束
        /// </summary>
        /// <param name="value"></param>
        private void OnBattleOver(bool value)
        {
            IsBattleOver = value;
            OpenFamilyTrainResultView();
        }

        /// <summary>
        /// 打开训练结算界面
        /// </summary>
        public void OpenFamilyTrainResultView()
        {
            if (null != cmdGuildPetFightEndNtf && IsBattleOver)
            {
                UIManager.OpenUI(EUIID.UI_FamilyCreatures_Result, false, cmdGuildPetFightEndNtf);
                cmdGuildPetFightEndNtf = null;
            }
        }

        /// <summary>
        /// 获取训练信息
        /// </summary>
        /// <returns></returns>
        public GuildPetTraining GetTrainInfo()
        {
            if (null == trainInfo)
            {
                trainInfo = new GuildPetTraining();
            }

            return trainInfo;
        }

        /// <summary>
        /// 获取家族兽通过index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public FamilyCreatureEntry GetFamilyCreatureByIndex(int index)
        {
            if (null != familyCreatureEntries && index >= 0 && index < familyCreatureEntries.Count)
            {
                return familyCreatureEntries[index];
            }
            return null;
        }

        /// <summary>
        /// 获取家族兽通过阶段id
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns></returns>
        public FamilyCreatureEntry GetFamilyCreatureByStageID(uint stageId)
        {
            if (null != familyCreatureEntries)
            {
                for (int i = 0; i < familyCreatureEntries.Count; i++)
                {
                    if (familyCreatureEntries[i].ConfigId == stageId)
                    {
                        return familyCreatureEntries[i];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 水风火地 - 获取名称语言id
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public uint GetNameLangIdByType(uint type)
        {
            uint id;
            switch (type)
            {
                case 1:
                    id = 2023301;
                    break;
                case 2:
                    id = 2023302;
                    break;
                case 3:
                    id = 2023303;
                    break;
                case 4:
                    id = 2023304;
                    break;
                default:
                    id = 0;
                    break;
            }
            return id;
        }

        /// <summary>
        /// 获取是否有下一个
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool GetWillLockFamilyCreatureByIndex(int index)
        {
            int preIndex = index - 1;
            if (null != familyCreatureEntries && preIndex >= 0 && preIndex < familyCreatureEntries.Count)
            {
                return familyCreatureEntries[preIndex].cSV.familyPet_id == 0;
            }
            return false;
        }

        public string CreatureStateStr()
        {
            if (CreatureState())
                return LanguageHelper.GetTextContent(2023313);
            return LanguageHelper.GetTextContent(2023312);
        }

        public bool CreatureState()
        {
            ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            uint currentTime = Sys_Time.Instance.GetServerTime();
            if (currentTime - zeroTime < 18000)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取阶段语言id
        /// </summary>
        /// <param name="stage"></param>
        /// <returns></returns>
        public uint CreatureState(uint stage)
        {
            return 2023530 + stage;
        }

        /// <summary>
        /// 获取系别名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public uint GetTypeName(uint type)
        {
            if (type == 1)
            {
                return 2023306;
            }
            else if (type == 2)
            {
                return 2023307;
            }
            else if (type == 3)
            {
                return 2023305;
            }
            else if (type == 4)
            {
                return 2023308;
            }
            return 2023306;
        }

        public bool GetTypeIsGet(uint type)
        {
            if (null == familyCreatureEntries)
                return false;
            for (int i = 0; i < familyCreatureEntries.Count; i++)
            {
                if (familyCreatureEntries[i].cSV.food_Type == type)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 当前阶段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsHasCreatureState(uint id)
        {
            if (null == familyCreatureEntries)
                return false;
            for (int i = 0; i < familyCreatureEntries.Count; i++)
            {
                if (familyCreatureEntries[i].cSV.id == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 当前阶段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasFamilyCreaturesType(uint type)
        {
            if (null == familyCreatureEntries)
                return false;
            for (int i = 0; i < familyCreatureEntries.Count; i++)
            {
                if (familyCreatureEntries[i].FmilyCreatureId == type)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否是当前训练的兽
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsCurrentTrainCreature(uint id)
        {
            if (null == trainInfo)
                return false;
            for (int i = 0; i < familyCreatureEntries.Count; i++)
            {
                if (trainInfo.TrainingStage / 10 == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 通过总id 获取家族兽
        /// </summary>
        /// <param name="typeId">Type表的id</param>
        /// <returns></returns>
        public FamilyCreatureEntry GetFamilyCreatureByType(uint typeId)
        {
            if (null != familyCreatureEntries)
            {
                for (int i = 0; i < familyCreatureEntries.Count; i++)
                {
                    if (familyCreatureEntries[i].FmilyCreatureId == typeId)
                    {
                        return familyCreatureEntries[i];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 设置喂养数值
        /// </summary>
        /// <param name="text"></param>
        /// <param name="allValue"></param>
        /// <param name="configValue"></param>
        public void SetTextData(Text text, float allValue, float configValue)
        {
            if (allValue > configValue)
            {
                text.text = LanguageHelper.GetTextContent(2023832, allValue.ToString(), (allValue - configValue).ToString());
            }
            else
            {
                text.text = allValue.ToString();
            }
        }

        //活动最低配置时间秒时
        private uint minTime = 0;
        public uint MinTime
        {
            get
            {
                if (minTime == 0)
                {
                    minTime = Convert.ToUInt32(CSVParam.Instance.GetConfData(1265).str_value);
                }
                return minTime;
            }
        }
        /// <summary>
        /// 获取喂食时间倒计时
        /// </summary>
        /// <returns></returns>
        public string GetFeedTimerStr()
        {
            ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            var sleepTime = zeroTime + MinTime;
            var endTime = zeroTime + 86399;
            var currentTime = Sys_Time.Instance.GetServerTime();
            if (currentTime < sleepTime)
            {
                return "";
            }
            else
            {
                return LanguageHelper.TimeToString((uint)(endTime - currentTime), LanguageHelper.TimeFormat.Type_1);
            }
        }

        /// <summary>
        /// 家族兽是否活动中
        /// </summary>
        /// <returns></returns>
        public bool FamilyCreatureIsSleep()
        {
            //当日的0点时间戳
            ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            var sleepTime = zeroTime + MinTime;
            var endTime = zeroTime + 86399;
            var currentTime = Sys_Time.Instance.GetServerTime();
            return currentTime < sleepTime;
        }

        /// <summary>
        /// 返回家族兽训练时间字符
        /// </summary>
        /// <returns></returns>
        public string FamilyCreatureTrainTime()
        {
            if (IsFamilyCreaturesOpenDate())
            {
                var time = GetFamilyCreatureTrainTime();
                if (time != 0)
                {
                    return LanguageHelper.TimeToString((uint)time, LanguageHelper.TimeFormat.Type_1);
                }
                else
                {
                    return "00:00:00";
                }
            }
            else
            {
                return "00:00:00";
            }
        }

        public ulong GetFamilyCreatureTrainTime()
        {
            Sys_Ini.Instance.Get<IniElement_Int>(1264, out IniElement_Int durationTime);
            ///当前时间
            var currentTime = Sys_Time.Instance.GetServerTime();
            //当日的0点时间戳
            ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            var setTime = zeroTime + trainInfo.StartTime;
            var end = 0ul;
            if (setTime > currentTime && currentTime >= setTime - (uint)CreatureReadyTime)
            {
                end = setTime - currentTime;
                return end;
            }
            else if ((setTime + (ulong)durationTime.value) > currentTime && currentTime >= setTime)
            {
                end = setTime + (ulong)durationTime.value - currentTime;
                return end;
            }
            return 0;
        }

        private int creatureReadyTime = -1;
        public int CreatureReadyTime
        {
            get
            {
                if(creatureReadyTime == -1)
                {
                    CSVDailyActivity.Data data = CSVDailyActivity.Instance.GetConfData(115u);
                    if(null != data)
                    {
                        creatureReadyTime = (int)data.NoticeLong;
                    }
                }
                return creatureReadyTime;
            }
        }

        /// <summary>
        /// 是否是当前的开放训练时间
        /// </summary>
        /// <returns></returns>
        public bool IsInTrainTime()
        {
            if (IsFamilyCreaturesOpenDate() && trainInfo != null)
            {
                ///持续时间
                Sys_Ini.Instance.Get<IniElement_Int>(1264, out IniElement_Int durationTime);
                ///当前时间
                var currentTime = Sys_Time.Instance.GetServerTime();
                //当日的0点时间戳
                ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
                var setTime = zeroTime + trainInfo.StartTime;
                if ((setTime + (ulong)durationTime.value) > currentTime && currentTime >= setTime)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否在训练准备时间和训练中
        /// </summary>
        /// <returns></returns>
        public bool IsOnReadyTrainTime()
        {
            if (IsFamilyCreaturesOpenDate() && trainInfo != null)
            {
                ///持续时间
                Sys_Ini.Instance.Get<IniElement_Int>(1264, out IniElement_Int durationTime);
                ///当前时间
                var currentTime = Sys_Time.Instance.GetServerTime();
                //当日的0点时间戳
                ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
                var setTime = zeroTime + trainInfo.StartTime;
                if ((setTime + (ulong)durationTime.value) > currentTime && currentTime >= (setTime - (uint)CreatureReadyTime))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsOnReadyTime()
        {
            if (IsFamilyCreaturesOpenDate() && trainInfo != null)
            {
                ///持续时间
                Sys_Ini.Instance.Get<IniElement_Int>(1264, out IniElement_Int durationTime);
                ///当前时间
                var currentTime = Sys_Time.Instance.GetServerTime();
                //当日的0点时间戳
                ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
                var setTime = zeroTime + trainInfo.StartTime;
                if ((setTime > currentTime && currentTime >= (setTime - (uint)CreatureReadyTime)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 显示训练按钮
        /// </summary>
        /// <returns></returns>
        public bool ShowTrainInfo()
        {
            if (trainInfo != null)
            {
                ///持续时间
                Sys_Ini.Instance.Get<IniElement_Int>(1264, out IniElement_Int durationTime);
                ///当前时间
                var currentTime = Sys_Time.Instance.GetServerTime();
                //当日的0点时间戳
                ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
                var setTime = zeroTime + trainInfo.StartTime;
                var refreshTime = zeroTime + MinTime;
                if ((currentTime >= (setTime - (uint)CreatureReadyTime) && IsFamilyCreaturesOpenDate()) || (IsFamilyCreaturesOpenDate() && currentTime < refreshTime))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 返回是否家族兽训练开启日
        /// </summary>
        /// <returns></returns>
        public bool IsFamilyCreaturesOpenDate(uint type = 2)
        {
            var starttime = Sys_Daily.Instance.getDailyStartTime(type);

            bool isDoubleWeek = (starttime.Day / 7) % 2 == 0;

            var value0 = (int)(starttime.DayOfWeek);

            value0 = value0 % 7 == 0 ? (value0 + 7) : value0;

            if (DayOfWeekCompate(value0) == false)//周几开启
                return false;

            return true;
        }


        /// <summary>
        /// 返回是否家族兽训练开启日
        /// </summary>
        /// <returns></returns>
        public bool IsFamilyCreaturesOpenNextDate()
        {
            var starttime = Sys_Daily.Instance.getDailyStartTime(2);

            bool isDoubleWeek = (starttime.Day / 7) % 2 == 0;

            var value0 = (int)(starttime.DayOfWeek);

            value0 = value0 % 7 == 0 ? (value0 + 7) : value0;

            if (DayOfWeekNextCompate(value0) == false)//周几开启
                return false;

            return true;
        }
        /// <summary>
        /// 是否是家族兽开放的日子
        /// </summary>
        public bool DayOfWeekCompate(int day)
        {
            List<uint> data;
            CSVParam.Data cSVParamData = CSVParam.Instance.GetConfData(1260);
            data = ReadHelper.ReadArray_ReadUInt(cSVParamData.str_value, '|');
            if (data == null || data.Count == 0)
                return true;

            bool result = false;

            int count = data.Count;

            for (int i = 0; i < count; i++)
            {
                var value = data[i];

                if (value == 0 || value == day)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 是否开放之后的第二天
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public bool DayOfWeekNextCompate(int day)
        {
            List<uint> data;
            CSVParam.Data cSVParamData = CSVParam.Instance.GetConfData(1260);
            data = ReadHelper.ReadArray_ReadUInt(cSVParamData.str_value, '|');
            if (data == null || data.Count == 0)
                return true;

            bool result = false;

            int count = data.Count;

            for (int i = 0; i < count; i++)
            {
                var value = data[i] + 1;

                if (value == 0 || value == day)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取是否有家族兽
        /// </summary>
        /// <returns></returns>
        public bool HasFamilyCreature()
        {
            if (familyCreatureEntries != null)
            {
                return familyCreatureEntries.Count > 0;
            }
            return false;
        }

        private void PopFamilyPetInfo(PopupParam param)
        {
            if (param.type == 2)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2023847); //训练已结束，感谢大家踊跃参与....
                PromptBoxParameter.Instance.SetConfirm(true, null);
                PromptBoxParameter.Instance.SetCancel(false, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_Popup))
            {
                eventEmitter.Trigger(EEvents.OnFamilyPopAdd, param);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_FamilyCreatures_Popup, false, param);
            }
        }

        /// <summary>
        /// 当进入家族领地的时候发送请求数据
        /// </summary>
        public void OnEnterFamilyMap()
        {
            if (Constants.FamilyMapId == Sys_Map.Instance.CurMapId)
            {
                GuildPetGetInfoReq();
            }
            CheckCanTrainCreature();
        }

        public void OpenFamilyCreatureUI()
        {
            if (!Sys_Family.Instance.familyData.isInFamily)
            {
                return;
            }

            if (Sys_Family.Instance.HasFamilyCreature())
            {
                if (openFamilyUiAway > 0)
                {
                    openFamilyUiAway = 0;
                    UIManager.OpenUI(EUIID.UI_FamilyCreatures_Get, false, new Tuple<uint, object>(0, 0));
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_FamilyCreatures);
                }
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_FamilyCreatures_Get, false, new Tuple<uint, object>(0, 0));
            }
        }

        /// <summary>
        /// 上线标记-是否可以提示获取家族兽
        /// </summary>
        public bool isFamilyCreatureNeedShowGetTips = true;
        public uint openFamilyUiAway = 0;
        /// <summary>
        /// 检查是否需要打开领取家族兽界面-需有权限玩家才可调用
        /// </summary>
        public void CheckCanGetFamilyCreature()
        {
            bool hasFamilyCreatures = HasFamilyCreature();

            if (!hasFamilyCreatures)
            {
                PromptBoxParameter.Instance.OpenActivityPriorityPromptBox(PromptBoxParameter.EPriorityType.FamilyPetEgg, LanguageHelper.GetTextContent(2023522), () =>
                {
                    UI_FamilyOpenParam openParam = new UI_FamilyOpenParam()
                    {
                        familyMenuEnum = (int)UI_Family.EFamilyMenu.Activity, // 0 - 3 大厅 家族成员 家族建筑， 家族活动
                        activeId = 60 // 家族活动表id 
                    };
                    Sys_Family.Instance.OpenUI_Family(openParam);
                    eventEmitter.Trigger(EEvents.FamilyPetGetUIEvent);
                });
            }
            else
            {
                int familyCreatureCount = familyCreatureEntries.Count;
                bool isMax = familyCreatureEntries.Count >= 4;
                if (!isMax && GetWillLockFamilyCreatureByIndex(familyCreatureCount))
                {
                    PromptBoxParameter.Instance.OpenActivityPriorityPromptBox(PromptBoxParameter.EPriorityType.FamilyPetEgg, LanguageHelper.GetTextContent(2023522), () =>
                     {
                         openFamilyUiAway = 1;
                         UI_FamilyOpenParam openParam = new UI_FamilyOpenParam()
                         {
                             familyMenuEnum = (int)UI_Family.EFamilyMenu.Activity, // 0 - 3 大厅 家族成员 家族建筑， 家族活动
                             activeId = 60 // 家族活动表id 
                         };
                         Sys_Family.Instance.OpenUI_Family(openParam);
                     });
                    eventEmitter.Trigger(EEvents.FamilyPetGetUIEvent);
                }
            }
        }
        /// <summary>
        /// 是否提示训练
        /// </summary>
        public bool showTrainTip = true;
        private Timer trainShowTimer;
        /// <summary>
        /// 检查是否开始活动开启
        /// </summary>
        public void CheckCanTrainCreature()
        {
            bool hasFamilyCreatures = HasFamilyCreature();

            if (showTrainTip && hasFamilyCreatures && IsInTrainTime())
            {
                CSVMapInfo.Data mapData = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
                if (mapData != null && !mapData.DisablePromptBox)
                {
                    showTrainTip = false;
                    PromptBoxParameter.Instance.OpenActivityPriorityPromptBox(PromptBoxParameter.EPriorityType.FamilyPetTraining, LanguageHelper.GetTextContent(2023756), () =>
                    {
                        GotoTrainCreature();
                    });
                }
            }
        }

        private void SetTrainTimer()
        {
            Sys_Ini.Instance.Get<IniElement_Int>(1264, out IniElement_Int durationTime);
            ///当前时间
            var currentTime = Sys_Time.Instance.GetServerTime();
            //当日的0点时间戳
            ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            var setTime = zeroTime + trainInfo.StartTime;
            if (currentTime < setTime)
            {
                trainShowTimer?.Cancel();
                trainShowTimer = Timer.Register(setTime - currentTime, () =>
                {
                    trainShowTimer?.Cancel();
                    trainShowTimer = Timer.Register(zeroTime + 86400 + trainInfo.StartTime, () =>
                    {
                        GuildPetGetInfoReq();
                    });
                    GuildPetGetInfoReq();
                });
            }
            else
            {
                trainShowTimer?.Cancel();
                trainShowTimer = Timer.Register(zeroTime + 86400 + trainInfo.StartTime, () =>
                {
                    GuildPetGetInfoReq();
                });
            }
        }

        public void GotoTrainCreature()
        {
            GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
            if (null == guildPetTraining)
                return;
            FamilyCreatureEntry trainFamilyCreatures = Sys_Family.Instance.GetFamilyCreatureByType(guildPetTraining.TrainingStage / 10);
            if (null == trainFamilyCreatures)
                return;
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(trainFamilyCreatures.cSV.id);
            UIManager.CloseUI(EUIID.UI_Family);
            UIManager.CloseUI(EUIID.UI_FamilyCreatures);
        }

        public uint GetRankRewardStar()
        {
            if (Sys_Ini.Instance.Get<IniElement_Int>(1262, out IniElement_Int starParam))
            {
                return (uint)starParam.value;
            }
            return 0;
        }

        public void SetIsTranStarIsUnlockRewardState()
        {
            GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
            uint StarL = GetRankRewardStar();
            if (null != guildPetTraining)
            {
                for (int i = 1; i < 99; i++)
                {
                    CSVFamilyPetTrainingStarReward.Data cSVFamilyPetTrainingStarRewardData = CSVFamilyPetTrainingStarReward.Instance.GetConfData((uint)(guildPetTraining.TrainingStage * 10 + i));
                    if (null != cSVFamilyPetTrainingStarRewardData)
                    {
                        if (totalScore >= cSVFamilyPetTrainingStarRewardData.trainingIntegralCondition && cSVFamilyPetTrainingStarRewardData.trainingStar >= StarL)
                        {
                            isCanGetTrainStarReward = true;
                            return;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            isCanGetTrainStarReward = false;
        }

        public bool isCanGetTrainStarReward = false;

        public class CreatureDifficulty
        {
            public ulong familyId;
            public uint familyCreatrueStage;
        }
        public readonly string creatureDifficultyFileName = "CreatureDifficulty";
        public CreatureDifficulty creatureDifficulty = new CreatureDifficulty();
        public void CheckDifficultyNeedUpdate()
        {
            bool hasEditorAuthority = Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.FamilyPetTraining);
            if (!hasEditorAuthority || !IsFamilyCreaturesOpenDate())
            {
                return;
            }
            bool hasCreature = HasFamilyCreature();
            if (!hasCreature)
                return;
            GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
            if (null == guildPetTraining)
                return;

            var newCreature = familyCreatureEntries[familyCreatureEntries.Count - 1];

            CSVFamilyPet.Data trainFamilyPetData = CSVFamilyPet.Instance.GetConfData(guildPetTraining.TrainingStage);
            if (null == trainFamilyPetData || null == newCreature.cSV)
                return;
            //最新家族兽的阶段 也是阶段表id
            var currentStage = newCreature.cSV.stage;
            //训练家族兽的阶段 也是阶段表id
            var trainStage = trainFamilyPetData.stage;
            //本地存在的家族id
            var saveFamilyId = creatureDifficulty.familyId;
            //本地存在的阶段id
            var saveFamilyCreatureStage = creatureDifficulty.familyCreatrueStage;
            var currentFamilyId = GetFamilyId();
            if (currentFamilyId != saveFamilyId)
            {
                if (currentStage == trainStage)
                {
                    SaveFamilyCreatureDB(currentFamilyId, currentStage, false);
                }
                else
                {
                    SaveFamilyCreatureDB(currentFamilyId, currentStage, true);
                }
            }
            else
            {
                if (saveFamilyCreatureStage != currentStage)
                {
                    if (familyCreatureEntries.Count > 1)
                    {
                        SaveFamilyCreatureDB(currentFamilyId, currentStage, true);
                    }
                    else
                    {
                        if (newCreature.cSV.stage != 1)
                        {
                            SaveFamilyCreatureDB(currentFamilyId, currentStage, true);
                        }
                    }

                }
            }
        }

        private void ParseJson(JsonObject jsonvalue)
        {
            JsonHeler.DeserializeObject(jsonvalue, creatureDifficulty);
        }

        private void LoadFamilyCreatureDB()
        {
            var jsonValue = FileStore.ReadJson(creatureDifficultyFileName);
            if (jsonValue == null)
                return;
            ParseJson(jsonValue);
        }

        public void SaveFamilyCreatureDB(ulong FamilyId, uint stageId, bool showTips)
        {
            if (showTips)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2023426); //已解锁更高的训练难度是否跳转到“设置训练”界面？
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    if (Sys_Family.Instance.ShowTrainInfo())
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023425u));
                        return;
                    }
                    if (!IsFamilyCreaturesOpenDate())
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023415));
                        return;
                    }
                    if (!UIManager.IsOpen(EUIID.UI_FamilyCreatures_SetTrain))
                    {
                        UIManager.OpenUI(EUIID.UI_FamilyCreatures_SetTrain);
                    }
                }, 2023427u);
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            creatureDifficulty.familyId = FamilyId;
            creatureDifficulty.familyCreatrueStage = stageId;
            FileStore.WriteJson(creatureDifficultyFileName, creatureDifficulty);
        }

        #region 家族兽公告红点版本

        public class CreatureNoticeRed
        {
            public ulong familyId;
            public uint noticeVer;
        }
        public readonly string CreatureNoticeRedFileName = "CreatureNoticeRed";
        public CreatureNoticeRed creatureNoticeRed = new CreatureNoticeRed();

        public bool IsShowNoticeRedPoint()
        {
            var currentFamilyId = GetFamilyId();
            return (currentFamilyId == creatureNoticeRed.familyId && familyNoticVer != creatureNoticeRed.noticeVer) || currentFamilyId != creatureNoticeRed.familyId;
        }

        public void CheckCreatureNoticeRedPoint()
        {
            var currentFamilyId = GetFamilyId();
            if ((currentFamilyId == creatureNoticeRed.familyId && familyNoticVer != creatureNoticeRed.noticeVer)
                || currentFamilyId != creatureNoticeRed.familyId)
            {
                SaveFamilyCreatureNoticeDB(currentFamilyId, familyNoticVer);
                eventEmitter.Trigger(EEvents.OnFamilyPetNoticeVerChange); 
            }
        }

        private void ParseNoticeJson(JsonObject jsonvalue)
        {
            JsonHeler.DeserializeObject(jsonvalue, creatureNoticeRed);
        }

        private void LoadFamilyCreatureNoticeDB()
        {
            var jsonValue = FileStore.ReadJson(CreatureNoticeRedFileName);
            if (jsonValue == null)
                return;
            ParseNoticeJson(jsonValue);
        }

        public void SaveFamilyCreatureNoticeDB(ulong FamilyId, uint ver)
        {
            creatureNoticeRed.familyId = FamilyId;
            creatureNoticeRed.noticeVer = ver;
            FileStore.WriteJson(CreatureNoticeRedFileName, creatureNoticeRed);
        }

        private void ClearCreatureDB()
        {
            creatureDifficulty.familyId = 0;
            creatureDifficulty.familyCreatrueStage = 0;

            creatureNoticeRed.familyId = 0;
            creatureNoticeRed.noticeVer = 0;
        }
        #endregion

        public bool IsNeedShowMenuTrainBtn()
        {
            return IsInTrainTime() && Constants.FamilyMapId == Sys_Map.Instance.CurMapId;
        }
    }
}
