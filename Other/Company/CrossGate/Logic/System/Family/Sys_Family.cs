using Google.Protobuf;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    /// <summary> 家族系统 </summary>
    public partial class Sys_Family : SystemModuleBase<Sys_Family>
    {
        #region 数据定义
        /// <summary> 家族数据 </summary>
        public partial class FamilyData
        {
            #region 数据结构
            /// <summary>
            /// 家族职位，对应服务器所示下标
            /// </summary>
            public enum EFamilyStatus
            {
                EApprentice = 100,  //学徒
                EMember = 200,      //成员
                EBrachMember = 300, //分会成员
                EBranchLeader = 400,//分会会长
                EElders = 500,      //长老
                EViceLeader = 600,  //副族长
                ELeader = 700,      //族长
            }
            /// <summary>
            /// 建筑物下标，对应服务器所示下标
            /// </summary>
            public enum EBuildingIndex
            {
                Castle = 0,           //城堡(主事厅)
                Bank = 1,             //银行(金库)
                SciencesAcademy = 2,  //科学院(工坊)
                Grange = 3,           //农舍(居舍)
                MilitaryAcademy = 4,  //军事院(修炼场)
                TrainingAnimals = 5,  //驯兽栏(家族兽栏)
                Barracks = 6,         //军营(库房)
            }
            /// <summary>
            /// 家族权限(字段对应配置)
            /// </summary>
            public enum EFamilyAuthority
            {
                IsAppointment,        //是否可任免
                ModifyName,           //修改家族名字
                BuildingUp,           //家族建筑升级
                ModifyDeclaration,    //修改宣言
                GroupMessage,         //发布群发消息
                InitiataMerger,       //发起家族合并
                AcceptMerger,         //接收家族合并
                EstablishBranch,      //建立分会
                RemoveBranch,         //移除分会
                MergeBranch,          //合并分会
                Invitation,           //邀请加入
                ApplicationAcceptance,//申请加入受理
                ModifyApproval,       //修改加入免审批权限
                ModifyApprovalLevel,  //修改加入免审批等级
                Worker,               //成员转正
                IsForbiddenWords,     //可否禁言
                Clear,                //踢人
                BattleEnroll,         //家族战是否可以报名
                FamilyPetName,        //家族兽改名
                FamilyPetNotice,      //编辑“家族兽通知”
                FamilyPetEgg,         //领取家族兽蛋
                FamilyPetTraining,    //设置“训练家族兽”
                FamilyBonus,          //获得奖金
            }
            /// <summary>
            /// 家族技能类型
            /// </summary>
            public enum FamilySkillType
            {
                GetCoinRateUp = 1,   //资金速率提升(财运亨通)
                ReduceBuildCost = 2, //降低建筑维护费用(持筹握算)
                ReduceStudyTime = 3, //减少研究时间(良工巧匠)
                MembershipCap = 4,   //成员上限
                BuildBranchNum = 5,  //可创建分会数
                BranchMemberNum = 6, //分会成员数
                BreakthroughTraining = 7,  //历练突破
            }
            /// <summary>
            /// 家族玩家信息
            /// </summary>
            public class FamilyPlayerInfo
            {
                /// <summary> 家族场景数据 </summary>
                public CmdGuildSceneInfoNtf cmdGuildSceneInfoNtf = new CmdGuildSceneInfoNtf();
                /// <summary> 家族游戏数据 </summary>
                public CmdGuildGameInfoNtf cmdGuildGameInfoNtf = new CmdGuildGameInfoNtf();
                /// <summary> 已申请列表 </summary>
                public List<ulong> applyList = new List<ulong>();
                /// <summary> 家族令牌数量 </summary>
                public uint staminaNum;
            }
            /// <summary>
            /// 家族详细信息
            /// </summary>
            public class FamilyDetailInfo
            {
                /// <summary> 家族信息 </summary>
                public CmdGuildGetGuildInfoAck cmdGuildGetGuildInfoAck = new CmdGuildGetGuildInfoAck();
                /// <summary> 成员信息 </summary>
                public CmdGuildGetMemberInfoAck cmdGuildGetMemberInfoAck = new CmdGuildGetMemberInfoAck();
                /// <summary> 申请列表 </summary>
                public CmdGuildGetGuildApplyMemberAck cmdGuildGetGuildApplyMemberAck = new CmdGuildGetGuildApplyMemberAck();
            }
            /// <summary>
            /// 查询家族信息
            /// </summary>
            public class QueryFamilyInfo
            {
                /// <summary> 家族列表 </summary>
                public List<BriefInfo> familyList = new List<BriefInfo>();
                /// <summary> 查询列表 </summary>
                public List<BriefInfo> queryList = new List<BriefInfo>();
                /// <summary> 查询合并列表 </summary
                public List<BriefInfo> mergeList = new List<BriefInfo>();

                /// <summary>
                /// 获取列表最后一个编号
                /// </summary>
                /// <returns></returns>
                public ulong GetFamilyListLastId()
                {
                    if (familyList.Count <= 0)
                        return 0;

                    return familyList[familyList.Count - 1].GuildId;
                }
                /// <summary>
                /// 清理列表
                /// </summary>
                public void Clear()
                {
                    familyList.Clear();
                    queryList.Clear();
                    mergeList.Clear();
                }
            }
            /// <summary>
            /// 家族建筑信息
            /// </summary>
            public class FamilyBuildInfo
            {
                /// <summary> 建筑技能 </summary>
                public class BuildSkill
                {
                    /// <summary> 建筑类型包含所有等级技能Id </summary>
                    public Dictionary<uint, List<uint>> dict_SkillId = new Dictionary<uint, List<uint>>();
                }
                /// <summary> 全技能列表 </summary>
                public List<BuildSkill> list_BuildSkill = new List<BuildSkill>(); //技能列表
                /// <summary> 建筑等级效果 </summary>
                public uint unlockLimit;        //解锁技能等级上限
                public List<uint> unlockSkillId = new List<uint>();//解锁技能编号
                public uint capitalCeiling;     //资金上限
                public uint dividendCap;        //分红上限
                public uint signInReward;       //每日签到奖励
                public uint dailyDonationLimit; //每日捐献资金增幅上限
                public List<uint> rewardCollectionTrigger = new List<uint>(); //奖励领取触发
                public List<uint> dailyReward = new List<uint>(); //每日捐献奖励
                /// <summary> 技能等级效果 </summary>
                public float getCoinRateUp;     //资金速率提升(万分比)
                public float reduceBuildCost;   //降低建筑维护费用(万分比)
                public float reduceStudyTime;   //减少研究时间(万分比)
                public uint membershipCap;      //成员上限
                public uint buildBranchNum;     //可创建分会数
                public uint branchMemberNum;    //分会成员数
                public uint breakthroughTraining; //历练突破
                /// <summary> 其他效果 </summary>
                public uint maintenanceCost;    //维护费用

                public FamilyBuildInfo()
                {
                    var values = System.Enum.GetValues(typeof(Sys_Family.FamilyData.EBuildingIndex));
                    foreach (var item in values)
                    {
                        list_BuildSkill.Add(new BuildSkill());
                    }

                    var familySkillUps = CSVFamilySkillUp.Instance.GetAll();

                    for (int i = 0, len = familySkillUps.Count; i < len; i++)
                    {
                        CSVFamilySkillUp.Data data = familySkillUps[i];
                        EBuildingIndex eBuildingIndex = (EBuildingIndex)(data.id / 10000);
                        uint skillType = data.id % 10000 / 1000;
                        BuildSkill buildSkill = list_BuildSkill[(int)eBuildingIndex];
                        List<uint> skillIdList = null;
                        if (!buildSkill.dict_SkillId.TryGetValue(skillType, out skillIdList))
                        {
                            skillIdList = new List<uint>();
                            buildSkill.dict_SkillId.Add(skillType, skillIdList);
                        }
                        skillIdList.Add(data.id);
                    }
                }

                public void UpdateData(Google.Protobuf.Collections.RepeatedField<GuildDetailInfo.Types.Building> field)
                {
                    for (int i = 0; i < field.Count; i++)
                    {
                        var Build = field[i];
                        uint id = (uint)(i * 100) + Build.Lvl;
                        CSVFamilyArchitectureEffect.Data cSVFamilyArchitectureEffectData = CSVFamilyArchitectureEffect.Instance.GetConfData(id);
                        if (null == cSVFamilyArchitectureEffectData) continue;
                        /// <summary> 建筑等级效果 </summary>
                        switch ((EBuildingIndex)i)
                        {
                            case EBuildingIndex.Bank:
                                {
                                    capitalCeiling = cSVFamilyArchitectureEffectData.CapitalCeiling;
                                    dividendCap = cSVFamilyArchitectureEffectData.DividendCap;
                                    signInReward = cSVFamilyArchitectureEffectData.SignInReward;
                                    dailyDonationLimit = cSVFamilyArchitectureEffectData.DailyDonationLimit;
                                    rewardCollectionTrigger.Clear();
                                    if (null != cSVFamilyArchitectureEffectData.RewardCollectionTrigger)
                                        rewardCollectionTrigger.AddRange(cSVFamilyArchitectureEffectData.RewardCollectionTrigger);
                                    dailyReward.Clear();
                                    if (null != cSVFamilyArchitectureEffectData.DailyReward)
                                        dailyReward.AddRange(cSVFamilyArchitectureEffectData.DailyReward);
                                }
                                break;
                            case EBuildingIndex.SciencesAcademy:
                                {
                                    unlockSkillId.Clear();
                                    if (null != cSVFamilyArchitectureEffectData.UnlockSkillId)
                                        unlockSkillId.AddRange(cSVFamilyArchitectureEffectData.UnlockSkillId);
                                }
                                break;
                            case EBuildingIndex.Grange:
                                {

                                }
                                break;
                            case EBuildingIndex.MilitaryAcademy:
                                {
                                    unlockLimit = cSVFamilyArchitectureEffectData.UnlockLimit;
                                }
                                break;
                        }
                        /// <summary> 技能等级效果 </summary>
                        foreach (var item in Build.SkillMap)
                        {
                            if (i == item / 10000)
                            {
                                CSVFamilySkillUp.Data cSVFamilySkillUpData = CSVFamilySkillUp.Instance.GetConfData(item);
                                if (null == cSVFamilySkillUpData) continue;
                                switch ((FamilySkillType)cSVFamilySkillUpData.SkillType)
                                {
                                    case FamilySkillType.GetCoinRateUp: { getCoinRateUp = Sys_Family.Instance.familyData.GetSkillValue(cSVFamilySkillUpData, 10000.0f); } break;
                                    case FamilySkillType.ReduceBuildCost: { reduceBuildCost = Sys_Family.Instance.familyData.GetSkillValue(cSVFamilySkillUpData, 10000.0f); } break;
                                    case FamilySkillType.ReduceStudyTime: { reduceStudyTime = Sys_Family.Instance.familyData.GetSkillValue(cSVFamilySkillUpData, 10000.0f); } break;
                                    case FamilySkillType.MembershipCap: { membershipCap = (uint)Sys_Family.Instance.familyData.GetSkillValue(cSVFamilySkillUpData, 10000.0f); } break;
                                    case FamilySkillType.BuildBranchNum: { buildBranchNum = (uint)Sys_Family.Instance.familyData.GetSkillValue(cSVFamilySkillUpData, 10000.0f); } break;
                                    case FamilySkillType.BranchMemberNum: { branchMemberNum = (uint)Sys_Family.Instance.familyData.GetSkillValue(cSVFamilySkillUpData, 10000.0f); } break;
                                    case FamilySkillType.BreakthroughTraining: { breakthroughTraining = (uint)Sys_Family.Instance.familyData.GetSkillValue(cSVFamilySkillUpData, 10000.0f); } break;
                                }
                            }
                        }
                    }
                    /// <summary> 其他效果 </summary>
                    uint buildCost = 0;
                    for (int i = 0; i < field.Count; i++)
                    {
                        uint id = (uint)i * 100 + field[i].Lvl;
                        CSVFamilyArchitecture.Data cSVFamilyArchitectureData = CSVFamilyArchitecture.Instance.GetConfData(id);
                        if (null != cSVFamilyArchitectureData)
                        {
                            buildCost += cSVFamilyArchitectureData.MaintenanceCost;
                        }
                    }
                    uint lv = field.Count > 0 ? field[0].Lvl : 1;
                    uint costRatio = uint.Parse(CSVParam.Instance.GetConfData(758).str_value);
                    maintenanceCost = (uint)Math.Ceiling((costRatio * lv + buildCost) * (1F - reduceBuildCost));
                }
            }
            /// <summary>
            /// 家族酒会信息
            /// </summary>
            public class FamilyPartyInfo
            {
                /// <summary> 流行菜品Id（烹饪表）</summary>
                public uint fashionFoodId = 0;
                /// <summary> 酒会价值 </summary>
                public uint partyValue = 0;
                /// <summary> 今日上缴次数 </summary>
                public uint submitTimes = 0;
                /// <summary> 食材是否领取 </summary>
                public bool isFoodMatGet = false;
                /// <summary> 酒会开始之前的城堡等级(只在酒会过程中使用) </summary>
                public uint castleLvBeforeParty = 0;
                /// <summary> 酒会记录列表 </summary>
                public List<PartyRecord> listPartyRecords = new List<PartyRecord>();
                /// <summary> 上一个提交的菜品ItemId （弹窗用） </summary>
                public uint lastSubmitFoodItemId = 0;
                /// <summary> 城堡等级对应酒会表数据（0星不计入） </summary>
                public Dictionary<uint, CSVFamilyReception.Data> dictPartyDataByCastleLv = new Dictionary<uint, CSVFamilyReception.Data>();
                /// <summary> 城堡星级对应酒会表数据（0星计入） </summary>
                public Dictionary<uint, CSVFamilyReception.Data> dictPartyDataByStarNum = new Dictionary<uint, CSVFamilyReception.Data>();
                /// <summary> 酒会价值升星节点(超过当前值的星级就是当前对应的index) </summary>
                public List<uint> listValueStage = new List<uint>();
                /// <summary> 家族酒会场景外提示是否弹过（一次登录只弹一次） </summary>
                public bool FamilyPartyOutsideHintIsPopup = false;
                /// <summary> 家族酒会场景内前往组队提示是否弹过（一次登录只弹一次） </summary>
                public bool FamilyPartyGetTeamInsideHintIsPopup = false;
                public class PartyRecord
                {
                    public string name;
                    public uint itemId;
                }

                public void UpdateRecordList(List<CmdGuildGetCuisineRecordAck.Types.Info> datas)
                {
                    listPartyRecords.Clear();
                    for (int i = 0; i < datas.Count; i++)
                    {
                        var data = datas[i];
                        PartyRecord record = new PartyRecord
                        {
                            name = data.Name.ToStringUtf8(),
                            itemId = data.ItemId
                        };
                        listPartyRecords.Add(record);
                    }
                }
            }

            /// <summary>
            /// /// 家族拍卖信息
            /// /// </summary>
            public class FamilyAuctionInfo
            {
                private Dictionary<uint, GuildAuction> activeAuctionInfoDic;
                private List<GuildAuctionMyInfo> myAuctionInfo;
                public List<GuildAuctionRecord> auctionRecord;
                /// <summary>
                /// 是否可见拍卖列表-服务器用了判断是否给客户端推送拍卖商品更新
                /// </summary>
                public bool IsWatch { get; set; }

                /// <summary>
                /// 设置活动拍卖数据
                /// </summary>
                /// <param name="data"></param>
                public void SetActiveDicData(GuildAuction data)
                {
                    if (null == activeAuctionInfoDic)
                        activeAuctionInfoDic = new Dictionary<uint, GuildAuction>();
                    activeAuctionInfoDic[data.ActiveId] = data;
                }

                /// <summary>
                /// 移除活动拍卖数据
                /// </summary>
                /// <param name="auctionId"></param>
                public void RemoveActiveDicData(uint auctionId)
                {
                    if (null != activeAuctionInfoDic && activeAuctionInfoDic.ContainsKey(auctionId))
                    {
                        activeAuctionInfoDic.Remove(auctionId);
                    }
                }

                /// <summary>
                /// 活动拍卖数据清空
                /// </summary>
                public void ClearAuctionData()
                {
                    if (null == activeAuctionInfoDic)
                        activeAuctionInfoDic = new Dictionary<uint, GuildAuction>();
                    activeAuctionInfoDic.Clear();
                }

                /// <summary>
                /// 获取拍卖数据通过活动id
                /// </summary>
                /// <param name="activeId">活动id</param>
                /// <returns></returns>
                public GuildAuction GetActiveDicData(uint activeId)
                {
                    if (null == activeAuctionInfoDic)
                        activeAuctionInfoDic = new Dictionary<uint, GuildAuction>();
                    //TestFun(activeId);
                    activeAuctionInfoDic.TryGetValue(activeId, out GuildAuction data);
                    return data;
                }

                /// <summary>
                /// 设置我的拍卖数据
                /// </summary>
                /// <param name="data"></param>
                public void SetMyActiveAuctionDicData(RepeatedField<global::Packet.GuildAuctionMyInfo> data)
                {
                    if (null == myAuctionInfo)
                        myAuctionInfo = new List<GuildAuctionMyInfo>();
                    myAuctionInfo.Clear();
                    myAuctionInfo.AddRange(data);
                }

                /// <summary>
                /// 获取我的拍卖数据
                /// </summary>
                public List<GuildAuctionMyInfo> GetMyActiveAuctionDicData()
                {
                    if (null == myAuctionInfo)
                        myAuctionInfo = new List<GuildAuctionMyInfo>();
                    return myAuctionInfo;
                }

                /// <summary>
                /// 我的个人拍卖记录修改
                /// </summary>
                /// <param name="activeId"></param>
                /// <param name="itemId"></param>
                /// <param name="del"></param>
                public void ChangeMyAuctionInfo(uint activeId, CmdGuildAuctionListUpdateNtf.Types.ItemUpdate itemValue, bool del)
                {
                    if (!del)
                    {
                        List<GuildAuctionMyInfo> myInfos = GetMyActiveAuctionDicData();
                        for (int i = 0; i < myInfos.Count; i++)
                        {
                            GuildAuctionMyInfo temp = myInfos[i];
                            if (temp.ActiveId == activeId && itemValue.Id == temp.Id)
                            {
                                temp.Price = itemValue.Price;
                                break;
                            }
                        }
                    }
                    else
                    {
                        List<GuildAuctionMyInfo> myInfos = GetMyActiveAuctionDicData();
                        for (int i = myInfos.Count - 1; i >= 0; i--)
                        {
                            GuildAuctionMyInfo temp = myInfos[i];
                            if (temp.ActiveId == activeId && itemValue.Id == temp.Id)
                            {
                                myInfos.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }

                /// <summary>
                /// 更新我的拍卖信息
                /// </summary>
                /// <param name="data"></param>
                public void UpdateMyAuctionDataByServerNtf(CmdGuildAuctionMyInfoUpdateNtf data)
                {
                    if (null == data)
                    {
                        DebugUtil.Log(ELogType.eNone, "CmdGuildAuctionListUpdateNtf Is Null");
                        return;
                    }
                    if (null != data.NewMyInfo)//商品添加
                    {
                        AddMyAuctionInfo(data.NewMyInfo);
                        Instance.eventEmitter.Trigger(EEvents.OnAuctionMyInfoAckEnd);
                    }
                    else if (null != data.UpdateMyInfo)//商品变化
                    {
                        ChangeMyAuctionInfo(data.UpdateMyInfo);
                        Instance.eventEmitter.Trigger(EEvents.OnAuctionItemChange);
                    }
                    else if (null != data.DelMyInfo && data.DelMyInfo.Count > 0)
                    {
                        for (int i = 0; i < data.DelMyInfo.Count; i++)
                        {
                            DelMyAuctionInfo(data.DelMyInfo[i]);
                        }
                        Sys_Family.Instance.eventEmitter.Trigger(EEvents.OnAuctionItemRemove);
                    }
                }

                /// <summary>
                /// 我的个人拍卖-增加
                /// </summary>
                /// /// <param name="newMyInfo"></param>
                public void AddMyAuctionInfo(GuildAuctionMyInfo newMyInfo)
                {
                    List<GuildAuctionMyInfo> myInfos = GetMyActiveAuctionDicData();
                    myInfos.Add(newMyInfo);
                }

                /// <summary>
                /// 我的个人拍卖-价格变化
                /// </summary>
                /// <param name="myInfoUpdate"></param>
                public void ChangeMyAuctionInfo(CmdGuildAuctionMyInfoUpdateNtf.Types.MyInfoUpdate myInfoUpdate)
                {
                    List<GuildAuctionMyInfo> myInfos = GetMyActiveAuctionDicData();

                    for (int i = 0; i < myInfos.Count; i++)
                    {
                        GuildAuctionMyInfo temp = myInfos[i];
                        if (temp.ActiveId == myInfoUpdate.ActiveId && myInfoUpdate.Id == temp.Id)
                        {
                            temp.Price = myInfoUpdate.Price;
                            temp.MyPrice = myInfoUpdate.MyPrice;
                            temp.Owned = myInfoUpdate.Owned;
                            break;
                        }
                    }
                }

                /// <summary>
                /// 我的个人拍卖-道具移除被买走（别人或自己），或者活动结束
                /// </summary>
                /// <param name="myInfoUpdate"></param>
                public void DelMyAuctionInfo(CmdGuildAuctionMyInfoUpdateNtf.Types.MyInfoUpdate myInfoUpdate)
                {
                    List<GuildAuctionMyInfo> myInfos = GetMyActiveAuctionDicData();
                    for (int i = myInfos.Count - 1; i >= 0; i--)
                    {
                        GuildAuctionMyInfo temp = myInfos[i];
                        if (temp.ActiveId == myInfoUpdate.ActiveId && myInfoUpdate.Id == temp.Id)
                        {
                            myInfos.RemoveAt(i);
                            break;
                        }
                    }
                }
                /// <summary>
                /// 更新活动数据 道具变更  分功变更等
                /// </summary>
                /// <param name="data"></param>
                public void UpdateAuctionDataByServerNtf(CmdGuildAuctionListUpdateNtf data)
                {
                    if (null == data)
                    {
                        DebugUtil.Log(ELogType.eNone, "CmdGuildAuctionListUpdateNtf Is Null");
                        return;
                    }
                    if (null != data.NewAuction)
                    {
                        SetActiveDicData(data.NewAuction);
                        bool hasSame = false;
                        if (null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
                        {
                            for (int i = 0; i < Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs.Count; i++)
                            {
                                GuildDetailInfo.Types.AuctionBrief tempData = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs[i];
                                if (tempData.ActiveId == data.NewAuction.ActiveId)
                                {
                                    hasSame = true;
                                    break;
                                }
                            }
                            if (!hasSame)
                            {
                                Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs.Add(new GuildDetailInfo.Types.AuctionBrief { ActiveId = data.NewAuction.ActiveId, EndTime = data.NewAuction.EndTime });
                            }
                        }
                        Sys_Family.Instance.needShowAuctionRedPoint = true;
                        RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyActiveRedPoint, null);
                        Instance.eventEmitter.Trigger(EEvents.OnAuctionAckEnd);
                    }
                    else if (null != data.EndActiveId && data.EndActiveId.Value != 0)
                    {
                        if (null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
                        {
                            for (int i = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs.Count - 1; i >= 0; i--)
                            {
                                GuildDetailInfo.Types.AuctionBrief tempData = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs[i];
                                if (tempData.ActiveId == data.EndActiveId.Value)
                                {
                                    Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs.RemoveAt(i);
                                }
                            }
                        }
                        RemoveActiveDicData(data.EndActiveId.Value);
                        Instance.eventEmitter.Trigger(EEvents.OnAuctionAckEnd);
                    }
                    else if (null != data.UpdateItem)
                    {
                        GuildAuction guildAuction = GetActiveDicData(data.ActiveId);
                        if (null != guildAuction)
                        {
                            ChangeAuctionItem(guildAuction, data.UpdateItem, data.Bonus.Value);
                            ChangeMyAuctionInfo(data.ActiveId, data.UpdateItem, false);
                        }
                        else
                        {
                            DebugUtil.Log(ELogType.eNone, $"Not Find id = {data.ActiveId} in GuildAuction's Dictionary");
                        }
                        Sys_Family.Instance.eventEmitter.Trigger(EEvents.OnAuctionItemChange, data.ActiveId);
                    }
                    else
                    {
                        GuildAuction guildAuction = GetActiveDicData(data.ActiveId);
                        if (null != guildAuction)
                        {
                            RemoveAuctionItem(guildAuction, data.DelItem, data.Bonus.Value);
                            ChangeMyAuctionInfo(data.ActiveId, data.DelItem, true);
                        }
                        else
                        {
                            DebugUtil.Log(ELogType.eNone, $"Not Find id = {data.ActiveId} in GuildAuction's Dictionary");
                        }
                        Sys_Family.Instance.eventEmitter.Trigger(EEvents.OnAuctionItemRemove, data.ActiveId, data.DelItem.Id);
                    }
                }

                /// <summary>
                /// 道具出价发生变化
                /// </summary>
                /// <param name="data"> 拍卖数据</param>
                /// <param name="updateDate"> 更新道具数据 </param>
                /// <param name="bonus"> 分红数据 0等于无权限</param>
                private void ChangeAuctionItem(GuildAuction data, CmdGuildAuctionListUpdateNtf.Types.ItemUpdate updateDate, long bonus)
                {
                    if (null == updateDate)
                        return;
                    for (int i = 0; i < data.Items.Count; i++)
                    {
                        GuildAuction.Types.AuctionItem tempItem = data.Items[i];
                        if (updateDate.Id == tempItem.Id)
                        {
                            tempItem.Price = updateDate.Price;
                            if (tempItem.Unowned)
                                tempItem.Unowned = false;
                        }
                    }
                    data.Bonus = bonus;
                }

                /// <summary>
                /// 道具出价发生变化
                /// </summary>
                /// <param name="data"> 拍卖数据</param>
                /// <param name="updateDate"> 更新道具数据 </param>
                /// <param name="bonus"> 分红数据 0等于无权限</param>
                public void ChangeAuctionItem(uint activeid, uint id, uint price)
                {
                    GuildAuction guildAuction = GetActiveDicData(activeid);
                    if (null != guildAuction)
                    {
                        for (int i = 0; i < guildAuction.Items.Count; i++)
                        {
                            GuildAuction.Types.AuctionItem tempItem = guildAuction.Items[i];
                            if (id == tempItem.Id)
                            {
                                tempItem.Price = price;
                                if (tempItem.Unowned)
                                    tempItem.Unowned = false;
                            }
                        }
                    }
                    else
                    {
                        DebugUtil.Log(ELogType.eNone, $"Not Find id = {activeid} in GuildAuction's Dictionary");
                    }
                }

                /// <summary>
                /// 请求一口价返回处理
                /// </summary>
                /// <param name="activeId"></param>
                /// <param name="id"></param>
                /// <param name="infoId"></param>
                public void RemoveOnePriceItem(uint activeId, uint id, uint infoId)
                {
                    GuildAuction guildAuction = GetActiveDicData(activeId);
                    if (null != guildAuction)
                    {

                        for (int i = guildAuction.Items.Count - 1; i >= 0; i--)
                        {
                            GuildAuction.Types.AuctionItem tempItem = guildAuction.Items[i];
                            if (id == tempItem.Id)
                            {
                                guildAuction.Items.RemoveAt(i);
                                break;
                            }
                        }

                    }
                    else
                    {
                        DebugUtil.Log(ELogType.eNone, $"Not Find id = {activeId} in GuildAuction's Dictionary");
                    }
                }

                /// <summary>
                /// 移除一口价道具
                /// </summary>
                /// <param name="data"></param>
                /// <param name="delUpdate"></param>
                /// <param name="bonus"></param>
                private void RemoveAuctionItem(GuildAuction data, CmdGuildAuctionListUpdateNtf.Types.ItemUpdate delUpdate, long bonus)
                {
                    if (null == delUpdate)
                        return;
                    for (int i = data.Items.Count - 1; i >= 0; i--)
                    {
                        GuildAuction.Types.AuctionItem tempItem = data.Items[i];
                        if (delUpdate.Id == tempItem.Id)
                        {
                            data.Items.RemoveAt(i);
                            break;
                        }
                    }
                    data.Bonus = bonus;
                }

                /// <summary>
                /// 返回拍卖道具数据
                /// </summary>
                /// <param name="activeId"></param>
                /// <param name="uid"></param>
                /// <returns></returns>
                public GuildAuction.Types.AuctionItem GetAuctionItem(uint activeId, uint uid)
                {
                    GuildAuction data = GetActiveDicData(activeId);
                    if (null != data)
                    {
                        for (int i = 0; i < data.Items.Count; i++)
                        {
                            GuildAuction.Types.AuctionItem tempItem = data.Items[i];
                            if (tempItem.Id == uid)
                            {
                                return tempItem;
                            }
                        }
                        return null;
                    }
                    else
                    {

                        DebugUtil.Log(ELogType.eNone, $"Not Find id = {activeId} in GuildAuction's Dictionary");
                        return null;
                    }
                }

                /// <summary>
                /// 返回该道具是否我最高出价
                /// </summary>
                /// <param name="activeId"></param>
                /// <param name="uid"></param>
                /// <returns></returns>
                public bool IsMyAuctionItem(uint activeId, uint uid)
                {
                    List<GuildAuctionMyInfo> datas = GetMyActiveAuctionDicData();
                    if (null != datas)
                    {
                        for (int i = 0; i < datas.Count; i++)
                        {
                            GuildAuctionMyInfo tempItem = datas[i];
                            if (tempItem.Id == uid && tempItem.MyPrice >= tempItem.Price)
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    return false;
                }
                /// <summary>
                /// 获得拍卖场次结束时间
                /// </summary>
                /// <param name="auctionId"></param>
                /// <returns></returns>
                public uint GetAuctionEndTime(uint auctionId)
                {
                    GuildAuction data = GetActiveDicData(auctionId);
                    if (null == data)
                        return 0;
                    return data.EndTime;
                }

                /// <summary>
                /// 获得拍卖是否结束
                /// </summary>
                /// <param name="auctionId"></param>
                /// <returns></returns>
                public bool GetAuctionIsEnd(uint auctionId)
                {
                    uint endTime = GetAuctionEndTime(auctionId);
                    if (endTime == 0)
                        return true;
                    if (Sys_Time.Instance.GetServerTime() >= endTime)
                        return true;
                    return false;
                }


            }
            #endregion
            #region 数据定义
            /// <summary> 家族玩家信息 </summary>
            public FamilyPlayerInfo familyPlayerInfo = new FamilyPlayerInfo();
            /// <summary> 家族详细信息 </summary>
            public FamilyDetailInfo familyDetailInfo = new FamilyDetailInfo();
            /// <summary> 查询家族信息 </summary>
            public QueryFamilyInfo queryFamilyInfo = new QueryFamilyInfo();
            /// <summary> 家族建筑信息 </summary>
            public FamilyBuildInfo familyBuildInfo = new FamilyBuildInfo();
            /// <summary> 家族酒会信息 </summary>
            public FamilyPartyInfo familyPartyInfo = new FamilyPartyInfo();
            /// <summary> 家族拍卖信息 </summary>
            public FamilyAuctionInfo familyAuctionInfo = new FamilyAuctionInfo();
            #endregion
            #region 数据处理
            /// <summary>
            /// 是否有家族
            /// </summary>
            public bool isInFamily
            {
                get
                {
                    return familyPlayerInfo.cmdGuildGameInfoNtf.GuildId != 0;
                }
            }
            public ulong GuildId
            {
                get
                {
                    if (null != familyPlayerInfo && null != familyPlayerInfo.cmdGuildGameInfoNtf)
                    {
                        return familyPlayerInfo.cmdGuildGameInfoNtf.GuildId;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            public ulong GuildUId
            {
                get
                {
                    if (null != familyPlayerInfo && null != familyPlayerInfo.cmdGuildGameInfoNtf)
                    {
                        return familyPlayerInfo.cmdGuildGameInfoNtf.GuildUid;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            /// <summary>
            /// 是否家族正在合并中
            /// </summary>
            public bool isMerging
            {
                get
                {
                    if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return false;
                    return familyDetailInfo.cmdGuildGetGuildInfoAck.Info.InitiativeMerge.OtherId != 0;
                }
            }
            /// <summary>
            /// 查询分会信息
            /// </summary>
            /// <param name="BranchId"></param>
            /// <returns></returns>
            public GuildDetailInfo.Types.BranchInfo CheckBranchInfo(uint BranchId)
            {
                var BranchMemberInfos = familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BranchMemberInfo;
                foreach (var BranchMemberInfo in BranchMemberInfos)
                {
                    if (BranchMemberInfo.Id == BranchId)
                        return BranchMemberInfo;
                }
                return null;
            }
            /// <summary>
            /// 查找分会成员列表
            /// </summary>
            /// <param name="BranchId"></param>
            /// <param name="listStatusLimit"></param>
            /// <returns></returns>
            public List<CmdGuildGetMemberInfoAck.Types.MemberInfo> CheckMemberInfos(uint BranchId, List<FamilyData.EFamilyStatus> listStatusLimit)
            {
                List<CmdGuildGetMemberInfoAck.Types.MemberInfo> memberInfos = new List<CmdGuildGetMemberInfoAck.Types.MemberInfo>();
                var members = familyDetailInfo.cmdGuildGetMemberInfoAck.Member;
                foreach (var member in members)
                {
                    uint status = member.Position % 10000;
                    uint branchId = member.Position / 10000;
                    bool isContains = null == listStatusLimit ? true : listStatusLimit.Contains((FamilyData.EFamilyStatus)status);
                    if (branchId == BranchId && isContains)
                    {
                        memberInfos.Add(member);
                    }
                }
                return memberInfos;
            }

            /// <summary>
            /// 获取分会成员数量
            /// </summary>
            /// <param name="BranchId"></param>
            /// <returns>返回分会成员数量</returns>
            public uint GetMemberCount(uint BranchId)
            {
                uint branchMemberCount = 0;
                var members = familyDetailInfo.cmdGuildGetMemberInfoAck.Member;
                foreach (var member in members)
                {
                    uint branchId = member.Position / 10000;
                    if (branchId == BranchId)
                    {
                        branchMemberCount++;
                    }
                }
                return branchMemberCount;
            }
            /// <summary>
            /// 查找会长/分会长
            /// </summary>
            /// <param name="BranchId"></param>
            /// <returns></returns>
            public CmdGuildGetMemberInfoAck.Types.MemberInfo CheckLeader(uint BranchId)
            {
                var members = familyDetailInfo.cmdGuildGetMemberInfoAck.Member;
                CmdGuildGetMemberInfoAck.Types.MemberInfo leader = null;
                foreach (var member in members)
                {
                    if (BranchId == 0)
                    {
                        if (member.Position % 10000 == (int)EFamilyStatus.ELeader)
                        {
                            leader = member;
                            break;
                        }
                    }
                    else if (member.Position / 10000 == BranchId)
                    {
                        if (member.Position % 10000 == (int)EFamilyStatus.EBranchLeader)
                        {
                            leader = member;
                            break;
                        }
                    }
                }
                return leader;
            }
            /// <summary>
            /// 查找自己
            /// </summary>
            /// <returns></returns>
            public CmdGuildGetMemberInfoAck.Types.MemberInfo CheckMe()
            {
                var members = familyDetailInfo.cmdGuildGetMemberInfoAck.Member;
                CmdGuildGetMemberInfoAck.Types.MemberInfo me = null;
                foreach (var member in members)
                {
                    if (member.RoleId == Sys_Role.Instance.RoleId)
                    {
                        me = member;
                        break;
                    }
                }
                return me;
            }
            /// <summary>
            /// 查找成员
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public CmdGuildGetMemberInfoAck.Types.MemberInfo CheckMember(ulong id)
            {
                var members = familyDetailInfo.cmdGuildGetMemberInfoAck.Member;
                CmdGuildGetMemberInfoAck.Types.MemberInfo memberInfo = null;
                foreach (var member in members)
                {
                    if (member.RoleId == id)
                    {
                        memberInfo = member;
                        break;
                    }
                }
                return memberInfo;
            }
            /// <summary>
            /// 我是否获取某个权限
            /// </summary>
            /// <param name="eFamilyAuthority"></param>
            /// <returns></returns>
            public bool GetMyPostAuthority(EFamilyAuthority eFamilyAuthority)
            {
                uint Position = familyDetailInfo.cmdGuildGetGuildInfoAck.MyPosition;
                EFamilyStatus eFamilyStatus = (EFamilyStatus)(Position % 10000);
                return GetPostAuthority(eFamilyStatus, eFamilyAuthority);
            }
            /// <summary>
            /// 获取职位权限
            /// </summary>
            /// <param name="eFamilyStatus"></param>
            /// <param name="eFamilyAuthority"></param>
            /// <returns></returns>
            public bool GetPostAuthority(EFamilyStatus eFamilyStatus, EFamilyAuthority eFamilyAuthority)
            {
                CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData((uint)eFamilyStatus);
                if (null == cSVFamilyPostAuthorityData) return false;
                switch (eFamilyAuthority)
                {
                    case EFamilyAuthority.IsAppointment: return cSVFamilyPostAuthorityData.IsAppointment == 1;
                    case EFamilyAuthority.ModifyName: return cSVFamilyPostAuthorityData.ModifyName == 1;
                    case EFamilyAuthority.BuildingUp: return cSVFamilyPostAuthorityData.BuildingUp == 1;
                    case EFamilyAuthority.ModifyDeclaration: return cSVFamilyPostAuthorityData.ModifyDeclaration == 1;
                    case EFamilyAuthority.GroupMessage: return cSVFamilyPostAuthorityData.GroupMessage == 1;
                    case EFamilyAuthority.InitiataMerger: return cSVFamilyPostAuthorityData.InitiataMerger == 1;
                    case EFamilyAuthority.AcceptMerger: return cSVFamilyPostAuthorityData.AcceptMerger == 1;
                    case EFamilyAuthority.EstablishBranch: return cSVFamilyPostAuthorityData.EstablishBranch == 1;
                    case EFamilyAuthority.RemoveBranch: return cSVFamilyPostAuthorityData.RemoveBranch == 1;
                    case EFamilyAuthority.MergeBranch: return cSVFamilyPostAuthorityData.MergeBranch == 1;
                    case EFamilyAuthority.Invitation: return cSVFamilyPostAuthorityData.Invitation == 1;
                    case EFamilyAuthority.ApplicationAcceptance: return cSVFamilyPostAuthorityData.ApplicationAcceptance == 1;
                    case EFamilyAuthority.ModifyApproval: return cSVFamilyPostAuthorityData.ModifyApproval == 1;
                    case EFamilyAuthority.ModifyApprovalLevel: return cSVFamilyPostAuthorityData.ModifyApprovalLevel == 1;
                    case EFamilyAuthority.Worker: return cSVFamilyPostAuthorityData.Worker == 1;
                    case EFamilyAuthority.IsForbiddenWords: return cSVFamilyPostAuthorityData.IsForbiddenWords == 1;
                    case EFamilyAuthority.Clear: return cSVFamilyPostAuthorityData.Clear == 1;
                    case EFamilyAuthority.BattleEnroll: return cSVFamilyPostAuthorityData.BattleEnroll == 1;
                    case EFamilyAuthority.FamilyPetName: return cSVFamilyPostAuthorityData.FamilyPetName == 1;
                    case EFamilyAuthority.FamilyPetNotice: return cSVFamilyPostAuthorityData.FamilyPetNotice == 1;
                    case EFamilyAuthority.FamilyPetEgg: return cSVFamilyPostAuthorityData.FamilyPetEgg == 1;
                    case EFamilyAuthority.FamilyPetTraining: return cSVFamilyPostAuthorityData.FamilyPetTraining == 1;
                    case EFamilyAuthority.FamilyBonus: return cSVFamilyPostAuthorityData.FamilyBonus == 1;
                    default:
                        return false;

                }
            }

            /// <summary>
            /// 是否处于较高职位
            /// </summary>
            /// <param name="id1"></param>
            /// <param name="id2"></param>
            /// <returns></returns>
            public bool IsHighStatus(ulong id1, ulong id2)
            {
                var memberInfo1 = CheckMember(id1);
                var memberInfo2 = CheckMember(id2);
                if (null == memberInfo1 || null == memberInfo2) return false;
                return memberInfo1.Position % 10000 > memberInfo2.Position % 10000;
            }
            /// <summary>
            /// 设置成员职位
            /// </summary>
            /// <param name="id"></param>
            /// <param name="Position"></param>
            public void SetMemberStatus(ulong id, uint Position)
            {
                var memberInfo = CheckMember(id);
                if (null == memberInfo) return;
                if (Position == 0)//移除
                {
                    familyDetailInfo.cmdGuildGetMemberInfoAck.Member.Remove(memberInfo);
                }
                else //调整职位
                {
                    memberInfo.Position = Position;
                }
            }
            /// <summary>
            /// 计算建筑等级
            /// </summary>
            /// <param name="list"></param>
            /// <returns></returns>
            public uint TotalBuildLevel(List<FamilyData.EBuildingIndex> list)
            {
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return 0;
                var allBuilds = familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings;
                uint level = 0;
                foreach (int index in list)
                {
                    level += allBuilds.Count > index ? allBuilds[index].Lvl : 0;
                }
                return level;
            }
            /// <summary>
            /// 获取家族等级
            /// </summary>
            /// <returns></returns>
            public uint GetGuildLevel()
            {
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
                    return 1;
                return Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings.Count > 0 ? Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings[0].Lvl : 1;
            }
            /// <summary>
            /// 当前建筑是否可以升级
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public bool isUpgradeByBuild(uint id)
            {
                CSVFamilyArchitecture.Data cSVFamilyArchitectureData = CSVFamilyArchitecture.Instance.GetConfData(id);
                if (null == cSVFamilyArchitectureData) return false;
                /// <summary> 有建筑物正在升级 </summary>
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return false;
                if (familyDetailInfo.cmdGuildGetGuildInfoAck.Info.NowUpgrade >= 0) return false;
                /// <summary> 升级货币 </summary>
                if (cSVFamilyArchitectureData.FundsRequired > familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildCoin) return false;
                /// <summary> 维护费用 </summary>
                if (cSVFamilyArchitectureData.MaintenanceCost * 10 > familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildCoin) return false;
                List<EBuildingIndex> list = new List<EBuildingIndex>();
                if (null == cSVFamilyArchitectureData.FrontBuilding)
                {
                    list.Add((EBuildingIndex)0);
                }
                else
                {
                    foreach (var bulid in cSVFamilyArchitectureData.FrontBuilding)
                    {
                        list.Add((EBuildingIndex)bulid);
                    }
                }
                bool isProsperityLvlEngout = Sys_Family.Instance.familyData.GetConstructLevel() >= cSVFamilyArchitectureData.demandProsperityLevel;
                uint totalLevel = TotalBuildLevel(list);
                /// <summary> 前置建筑总和等级 </summary>
                if (totalLevel < cSVFamilyArchitectureData.DemandLevel || !isProsperityLvlEngout) return false;
                /// <summary> 当前建筑是否已满级 </summary>
                Sys_Family.FamilyData.EBuildingIndex eBuildingIndex = (Sys_Family.FamilyData.EBuildingIndex)(id / 100);
                var allBuilds = familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings;
                if (allBuilds.Count > (int)eBuildingIndex)
                {
                    if (cSVFamilyArchitectureData.MaxLevel <= allBuilds[(int)eBuildingIndex].Lvl)
                        return false;
                }
                return true;
            }

            /// <summary>
            /// 当前建筑的等级
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public uint GetLevelByBuildIndex(uint index)
            {
                var allBuilds = familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings;
                if (allBuilds.Count > (int)index)
                {
                    return allBuilds[(int)index].Lvl;
                }
                return 0;
            }

            /// <summary>
            /// 获取职位数量
            /// </summary>
            /// <param name="eFamilyStatus"></param>
            /// <returns></returns>
            public uint GetPostNum(EFamilyStatus eFamilyStatus)
            {
                uint Num = 0;
                CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData((uint)eFamilyStatus);
                if (null == cSVFamilyPostAuthorityData) return Num;
                switch (eFamilyStatus)
                {
                    case EFamilyStatus.EApprentice://学徒
                    case EFamilyStatus.EMember://成员
                        {
                            Num = familyBuildInfo.membershipCap;
                        }
                        break;
                    case EFamilyStatus.EBrachMember://分会成员
                        {
                            Num = familyBuildInfo.branchMemberNum;
                        }
                        break;
                    case EFamilyStatus.EBranchLeader://分会会长
                    case EFamilyStatus.EElders: //长老
                    case EFamilyStatus.EViceLeader://副族长
                    case EFamilyStatus.ELeader: //族长
                        {
                            Num = cSVFamilyPostAuthorityData.PostNum;
                        }
                        break;
                    default:
                        {
                            Num = 0;
                        }
                        break;
                }
                return Num;
            }
            /// <summary>
            /// 是否达到捐献上限
            /// </summary>
            /// <param name="donateId"></param>
            /// <returns></returns>
            public bool IsLimit_Donate(uint donateId)
            {
                var donateList = familyPlayerInfo.cmdGuildSceneInfoNtf.Donate;
                for (int i = 0; i < donateList.Count; i++)
                {
                    var donate = donateList[i];
                    if (donate.DonateId != donateId) continue;
                    CSVFamilyDonate.Data cSVFamilyDonateData = CSVFamilyDonate.Instance.GetConfData(donateId);
                    if (null == cSVFamilyDonateData) continue;
                    if (cSVFamilyDonateData.DonationNum <= donate.Count)
                    {
                        return true;
                    }
                }
                return false;
            }
            /// <summary>
            /// 捐献货币是否足够
            /// </summary>
            /// <param name="donateId"></param>
            /// <returns></returns>
            public bool IsEnough_Donate(uint donateId)
            {
                var donate = Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.Donate;
                uint count = 0;
                for (int i = 0; i < donate.Count; i++)
                {
                    var DonateInfo = donate[i];
                    if (DonateInfo.DonateId == donateId)
                    {
                        count = DonateInfo.Count;
                        break;
                    }
                }
                uint id = count + 1;

                CSVFamilyDonate.Data cSVFamilyDonateData = CSVFamilyDonate.Instance.GetConfData(donateId);
                if (null == cSVFamilyDonateData) return false;
                CSVFamilyDonateTime.Data cSVFamilyDonateTimeData = CSVFamilyDonateTime.Instance.GetConfData(id);
                if (null == cSVFamilyDonateTimeData) return false;

                bool isEnough = false;
                uint donationType = 0;
                uint needCount = 0;
                switch (donateId)
                {
                    case 1: //捐献1
                        donationType = cSVFamilyDonateData.DonationType;
                        needCount = cSVFamilyDonateTimeData.GoldSingleCost;
                        isEnough = Sys_Bag.Instance.GetItemCount((uint)cSVFamilyDonateData.DonationType) >= cSVFamilyDonateTimeData.GoldSingleCost;
                        break;
                    case 2: //捐献2
                        donationType = cSVFamilyDonateData.DonationType;
                        needCount = cSVFamilyDonateTimeData.DiamondsSingleCost;
                        isEnough = Sys_Bag.Instance.GetItemCount((uint)cSVFamilyDonateData.DonationType) >= cSVFamilyDonateTimeData.DiamondsSingleCost;
                        break;
                    default:
                        break;
                }

                if (!isEnough)
                {
                    Sys_Bag.Instance.TryOpenExchangeCoinUI((ECurrencyType)donationType, needCount);
                }
                return isEnough;
            }
            /// <summary>
            /// 是否达到捐献奖励要求
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool IsFinish_DonateReward(int index)
            {
                if (familyBuildInfo.rewardCollectionTrigger.Count < index) return false;
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return false;
                return familyBuildInfo.rewardCollectionTrigger[index] <= familyDetailInfo.cmdGuildGetGuildInfoAck.Info.TodayDonateCoin;
            }
            /// <summary>
            /// 是否已领取捐献奖励
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool IsReceived_DonateReward(int index)
            {
                return familyPlayerInfo.cmdGuildSceneInfoNtf.DonateRewardIndex.Contains((uint)index);
            }
            /// <summary>
            /// 得到技能值
            /// </summary>
            /// <param name="cSVFamilySkillUpData"></param>
            /// <param name="ratio"></param>
            /// <returns></returns>
            public float GetSkillValue(CSVFamilySkillUp.Data cSVFamilySkillUpData, float ratio = 10000.0f)
            {
                float value = 0;
                switch ((FamilySkillType)cSVFamilySkillUpData.SkillType)
                {
                    case FamilySkillType.GetCoinRateUp: { value = (float)cSVFamilySkillUpData.Parameter1 / ratio; } break;
                    case FamilySkillType.ReduceBuildCost: { value = (float)cSVFamilySkillUpData.Parameter1 / ratio; } break;
                    case FamilySkillType.ReduceStudyTime: { value = (float)cSVFamilySkillUpData.Parameter1 / ratio; } break;
                    case FamilySkillType.MembershipCap: { value = cSVFamilySkillUpData.Parameter1; } break;
                    case FamilySkillType.BuildBranchNum: { value = cSVFamilySkillUpData.Parameter2; } break;
                    case FamilySkillType.BranchMemberNum: { value = cSVFamilySkillUpData.Parameter3; } break;
                    case FamilySkillType.BreakthroughTraining: { value = cSVFamilySkillUpData.Parameter2; } break;
                    default: { value = cSVFamilySkillUpData.Parameter1; } break;
                }
                return value;
            }
            /// <summary>
            /// 获取对应类型的技能Id
            /// </summary>
            /// <param name="familySkillType"></param>
            /// <returns></returns>
            public uint GetSkillId(FamilySkillType familySkillType)
            {
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return 0;
                var allBuilds = familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings;

                for (int i = 0; i < allBuilds.Count; i++)
                {
                    var skillMap = allBuilds[i].SkillMap;

                    for (int j = 0; j < skillMap.Count; j++)
                    {
                        uint skillId = skillMap[j];
                        if (skillId / 1000 % 10 == (int)familySkillType)
                        {
                            return skillId;
                        }
                    }
                }
                return 0;
            }
            /// <summary>
            /// 获取申请成员数量
            /// </summary>
            /// <returns></returns>
            public uint GetFamilyApplyMember()
            {
                if (null == familyDetailInfo.cmdGuildGetGuildApplyMemberAck ||
                    null == familyDetailInfo.cmdGuildGetGuildApplyMemberAck.List)
                    return 0;

                return (uint)familyDetailInfo.cmdGuildGetGuildApplyMemberAck.List.Count;
            }

            /// <summary>
            /// 获取行业经验
            /// </summary>
            /// <param name="type"></param>
            /// <returns>返回对应行业经验</returns>
            public uint GetConstructExp(EConstructs type)
            {
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return 0;
                switch (type)
                {
                    case EConstructs.Agriculture:
                        {
                            return familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AgricultureExp;
                        }
                    case EConstructs.Business:
                        {
                            return familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BusinessExp;
                        }
                    case EConstructs.Security:
                        {
                            return familyDetailInfo.cmdGuildGetGuildInfoAck.Info.SecurityExp;
                        }
                    case EConstructs.Religion:
                        {
                            return familyDetailInfo.cmdGuildGetGuildInfoAck.Info.ReligionExp;
                        }
                    case EConstructs.Technology:
                        {
                            return familyDetailInfo.cmdGuildGetGuildInfoAck.Info.TechnologyExp;
                        }
                }
                return 0;
            }

            /// <summary>
            /// 获取繁荣度等级
            /// </summary>
            /// <param name="type"></param>
            /// <returns>繁荣度等级</returns>
            public uint GetConstructLevel()
            {
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return 0;
                return familyDetailInfo.cmdGuildGetGuildInfoAck.Info.ProsperityLvl;
            }

            public bool GetCurrentConstructExpIsMax(EConstructs type)
            {
                uint level = Sys_Family.Instance.familyData.GetConstructLevel();
                CSVFamilyProsperity.Data familyConstructLevelData = CSVFamilyProsperity.Instance.GetConfData(level);
                if (null != familyConstructLevelData)
                {
                    return Sys_Family.Instance.GetClientDataExp(type, familyConstructLevelData) <= Sys_Family.Instance.familyData.GetConstructExp(type);
                }
                return false;
            }

            /// <summary>
            /// 设置家族货币数据包括繁荣度
            /// </summary>
            /// <param name="additem"></param>
            public void SetGuidCurrency(uint itemId, uint itemCount)
            {
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
                switch (itemId)
                {
                    //家族资金
                    case (uint)ECurrencyType.FamilyCoin:
                        familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildCoin += itemCount;
                        break;
                    //个人家族贡献
                    case (uint)ECurrencyType.PersonalContribution:
                        familyPlayerInfo.cmdGuildGameInfoNtf.GuildContribution += itemCount;
                        break;
                    //农业繁荣度
                    case (uint)EFamilyConstruct.FarmingProsperity:
                        familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AgricultureExp += itemCount;
                        break;
                    //商业繁荣度
                    case (uint)EFamilyConstruct.BusinessProsperity:
                        familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BusinessExp += itemCount;
                        break;
                    //治安繁荣度
                    case (uint)EFamilyConstruct.SecurityProsperity:
                        familyDetailInfo.cmdGuildGetGuildInfoAck.Info.SecurityExp += itemCount;
                        break;
                    //宗教繁荣度
                    case (uint)EFamilyConstruct.ReligiousProsperity:
                        familyDetailInfo.cmdGuildGetGuildInfoAck.Info.ReligionExp += itemCount;
                        break;
                    //科技繁荣度
                    case (uint)EFamilyConstruct.TechProsperity:
                        familyDetailInfo.cmdGuildGetGuildInfoAck.Info.TechnologyExp += itemCount;
                        break;
                }
            }

            /// <summary>
            /// 获取家族令牌
            /// </summary>
            /// <param name="additem"></param>
            public long GetGuidStamina()
            {
                return Sys_Bag.Instance.GetItemCount((uint)ECurrencyType.FamilyStamina);
            }

            public uint GetBuildLevel(Sys_Family.FamilyData.EBuildingIndex eBuildType)
            {
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return 0;
                var info = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info;
                var allBuilds = info.AllBuildings;
                GuildDetailInfo.Types.Building building = allBuilds.Count > (int)eBuildType ? allBuilds[(int)eBuildType] : null;
                if (null != building)
                {
                    return building.Lvl;
                }
                else
                {
                    return 0;
                }
            }
            #endregion
        }

        /// <summary> 事件枚举 </summary>
        public enum EEvents
        {
            UpdateFamilyList,         //更新家族列表         
            GetQueryFamilyListRes,    //得到查询结果
            GetMergeFamilyListRes,    //得到合并列表
            UpdateApplyFamilyList,    //更新申请列表
            CreateFamily,             //创建家族
            JoinFamily,               //加入家族
            QuitFamily,               //退出家族
            UpdateFamilyInfo,         //得到家族信息
            UpdateMember,             //更新家族成员
            GetApplyMember,           //获取申请成员
            UpdateApplyMember,        //更新申请成员
            UpdateFamilyName,         //更新家族名称
            UpdateFamilyDeclaration,  //更新家族描述
            UpdateSetApplyInfo,       //更新申请设置
            CreateBranch,             //创建分会成功
            DestroyBranch,            //删除分会成功
            UpdateBranchName,         //更新分会信息
            UdateSignState,           //更新签到状态
            UdateBonusState,          //更新分会状态
            UpdateMemberStatus,       //更新成员职位
            UpdateBuildState,         //更新建筑状态
            UpdateBuildSkillState,    //更新技能状态
            UpgradeBuildSkill,        //技能升级完成
            UpdateDonateData,         //更新捐献数据
            UpdateDonateRewardData,   //更新捐献奖励
            SendMergeResult,          //发送合并结果
            UpdateMergeStatus,        //更新合并状态
            UpdateApplyMergedList,    //更新申请合并
            UpdateMergeResult,        //更新合并结果
            UpdateMergeTarget,        //更新合并目标
            NeedApplyMergedListReq,   //需请求新列表
            ChangeMyNews,             //改变新闻标记
            GuildCurrencyChange,      //家族货币-繁荣度-家族资金-家族个人贡献改变通知
            GuildStaminaChange,       //家族令牌变化
            GuildConstructLvChange,   //家族繁荣读变化
            OnPartyValueNtfUpdate,    //家族酒会价值刷新
            OnPartyRecordListUpdate,  //家族酒会上缴记录列表刷新
            OnPartyDataUpdate,        //家族酒会配置数据刷新(流行菜品/上缴次数/食材领取状态等参数)
            OnPartySubmitSucceed,     //家族酒会上缴菜品成功
            OnAuctionAckEnd,          //当请求拍卖列表成功
            OnAuctionMyInfoAckEnd,    //当请求我的拍卖列表信息成功
            OnAuctionReocordAckEnd,   //当请求拍卖记录成功
            OnAuctionItemChange,      //拍卖道具状态改变
            OnAuctionItemRemove,      //拍卖道具被删除-一口价
            OnAuctionItemStateError,      //拍卖道具状态错误
            OnDeleConsignEntry,          //删除家族委托条目 或者更新自己委托条目(被协助之后需要名字需要更换)
            OnPublishSuccess,           //发布成功
            OnReceiveRedPacket,       //收到家族红包
            UpdateMySystemRedPacket,  //更新自己的系统红包
            UpdateMyTotalRedPacket,   //更新自己发放的红包总金额、总个数
            UpdateShowRedPacket,      //更新大厅展示的八个红包和红包历史数据
            UpdateHistoryRedPacket,   //更新大厅红包历史数据
            OnOpendRedPacketBack,     //抢红包返回
            OnVoiceRecordIsPass,      //语音红包录制是否通过
            OnRefreshRedPacketPoint,  //刷新家族红包红点

            OnGetFamilyPetInfo,        //获取家族兽信息
            OnGetFamilyPetFeedInfo,        //获取家族兽喂养数据
            OnSetTrainInfoEnd,        //设置家族兽训练
            OnFamilyPetFeedEnd,        //喂食家族兽后
            OnFamilyPetTrainScore,        //获取家族兽训练积分
            OnFamilyPetNotice,        //家族兽公告刷新
            OnFamilyPopAdd,             //当喂养或者活动开始结束提示
            OnFamilyRankInfoBack,             //请求排行数据返回

            OnSceneInfoNtf,
            FamilyPetGetUIEvent,  //获取二级弹窗点击确定
            OnFamilyPetTrainStarOrEnd,             //家族兽训练开始或者结束
            OnFamilyPetNoticeVerChange,        //家族兽公告版本变化
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        /// <summary> 家族数据 </summary>
        public FamilyData familyData { get; set; } = new FamilyData();

        /*/// <summary> 建设最后任务id </summary>
        private HashSet<uint> lastTaskIds = null;
        private HashSet<uint> LastTaskIds
        {
            get
            {
                if(null == lastTaskIds)
                {
                    lastTaskIds = new HashSet<uint>();
                    var industryTasks = CSVIndustryTask.Instance.GetAll();
                    for (int i = 0, len = industryTasks.Count; i < len; i++)
                    {
                        var item = industryTasks[i];
                        if (null != item.task_array && item.task_array.Count > 0)
                        {
                            lastTaskIds.Add(item.task_array[item.task_array.Count - 1]);
                        }
                    }
                }
                return lastTaskIds;
            }
        }*/
        #endregion
        #region 系统函数
        public override void Init()
        {
            ProcessEvents(true);
            FamilyDataInit();
        }
        public override void Dispose()
        {
            ProcessEvents(false);
        }
        public override void OnLogin()
        {
            recommendConstructId = 0;
            FamilyCreaturesLogin();
            OnLoginReq();
        }
        public override void OnLogout()
        {
            recommendConstructId = 0;
            showAddFamilyTips = false;
            severNtf = false;
            needShowFraudTip = false;
            FamilyRedPacketLogout();
            FamilyPartyLogout();
            FamilyCreaturesLogout();
        }
        public override void OnSyncFinished()
        {

        }
        #endregion
        #region 初始化
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void ProcessEvents(bool toRegister)
        {
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnLoadOK, OnEnterFamilyMap, toRegister);
            Net_Combat.Instance.eventEmitter.Handle<bool>(Net_Combat.EEvents.OnBattleOver, OnBattleOver, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnReceived, OnReceived, toRegister);
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.CreateReq, (ushort)CmdGuild.CreateAck, OnGuildCreateAck, CmdGuildCreateAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.ApplyReq, (ushort)CmdGuild.ApplyAck, OnGuildApplyAck, CmdGuildApplyAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.ChangeNameReq, (ushort)CmdGuild.ChangeNameAck, OnGuildChangeNameAck, CmdGuildChangeNameAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.ChangeNoticeReq, (ushort)CmdGuild.ChangeNoticeAck, OnGuildChangeNoticeAck, CmdGuildChangeNoticeAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.SigninReq, (ushort)CmdGuild.SigninAck, OnGuildSigninAck, CmdGuildSigninAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GuildNotifyReq, (ushort)CmdGuild.GuildNotifyAck, OnGuildNotifyAck, CmdGuildGuildNotifyAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.GuildNotifyNtf, OnGuildNotifyNtf, CmdGuildGuildNotifyNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.HandleApplyReq, (ushort)CmdGuild.HandleApplyAck, OnGuildHandleApplyAck, CmdGuildHandleApplyAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetBonusReq, (ushort)CmdGuild.GetBonusAck, OnGuildGetBonusAck, CmdGuildGetBonusAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.KickMemberReq, (ushort)CmdGuild.KickMemberAck, OnGuildKickMemberAck, CmdGuildKickMemberAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.KickMemberNtf, OnGuildKickMemberNtf, CmdGuildKickMemberNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.QuitReq, (ushort)CmdGuild.QuitAck, OnGuildQuitAck, CmdGuildQuitAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.OutgoingLeaderReq, (ushort)CmdGuild.OutgoingLeaderAck, OnGuildOutgoingLeaderAck, CmdGuildOutgoingLeaderAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.LeaderChangeNtf, OnGuildLeaderChangeNtf, CmdGuildLeaderChangeNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.UpgradeBuildingReq, (ushort)CmdGuild.UpgradeBuildingAck, OnGuildUpgradeBuildingAck, CmdGuildUpgradeBuildingAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.DonateReq, (ushort)CmdGuild.DonateAck, OnGuildDonateAck, CmdGuildDonateAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetDonateRewardReq, (ushort)CmdGuild.GetDonateRewardAck, OnGuildGetDonateRewardAck, CmdGuildGetDonateRewardAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.MergeReq, (ushort)CmdGuild.MergeAck, OnGuildMergeAck, CmdGuildMergeAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.HandleMergeApplyReq, (ushort)CmdGuild.HandleMergeApplyAck, OnGuildHandleMergeApplyAck, CmdGuildHandleMergeApplyAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.SystemMergeNtf, OnGuildSystemMergeNtf, CmdGuildSystemMergeNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetGuildListReq, (ushort)CmdGuild.GetGuildListAck, OnGuildGetGuildListAck, CmdGuildGetGuildListAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.FindGuildReq, (ushort)CmdGuild.FindGuildAck, OnGuildFindGuildAck, CmdGuildFindGuildAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetGuildInfoReq, (ushort)CmdGuild.GetGuildInfoAck, OnGuildGetGuildInfoAck, CmdGuildGetGuildInfoAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.CreateBranchReq, (ushort)CmdGuild.CreateBranchAck, OnGuildCreateBranchAck, CmdGuildCreateBranchAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.BranchMoveReq, (ushort)CmdGuild.BranchMoveAck, OnGuildBranchMoveAck, CmdGuildBranchMoveAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.UpgradeWorkshopSkillReq, (ushort)CmdGuild.UpgradeWorkshopSkillAck, OnGuildUpgradeWorkshopSkillAck, CmdGuildUpgradeWorkshopSkillAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.DestroyBranchReq, (ushort)CmdGuild.DestroyBranchAck, OnGuildDestroyBranchAck, CmdGuildDestroyBranchAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.ChangePositionReq, (ushort)CmdGuild.ChangePositionAck, OnGuildChangePositionAck, CmdGuildChangePositionAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.HandleApplyNtf, OnGuildHandleApplyNtf, CmdGuildHandleApplyNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.ChangeDstMergeGuildReq, (ushort)CmdGuild.ChangeDstMergeGuildAck, OnGuildChangeDstMergeGuildAck, CmdGuildChangeDstMergeGuildAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.ChangeDstMergeGuildNtf, OnGuildChangeDstMergeGuildNtf, CmdGuildChangeDstMergeGuildNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetMemberInfoReq, (ushort)CmdGuild.GetMemberInfoAck, OnGuildGetMemberInfoAck, CmdGuildGetMemberInfoAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetMyApplyListReq, (ushort)CmdGuild.GetMyApplyListAck, OnGuildGetMyApplyListAck, CmdGuildGetMyApplyListAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.OneKeyApplyReq, (ushort)CmdGuild.OneKeyApplyAck, OnGuildOneKeyApplyAck, CmdGuildOneKeyApplyAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.SceneInfoNtf, OnGuildSceneInfoNtf, CmdGuildSceneInfoNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.GameInfoNtf, OnGuildGameInfoNtf, CmdGuildGameInfoNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.ChangeBranchNameReq, (ushort)CmdGuild.ChangeBranchNameAck, OnGuildChangeBranchNameAck, CmdGuildChangeBranchNameAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetGuildApplyMemberReq, (ushort)CmdGuild.GetGuildApplyMemberAck, OnGuildGetGuildApplyMemberAck, CmdGuildGetGuildApplyMemberAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.SetApplyInfoReq, (ushort)CmdGuild.SetApplyInfoAck, OnGuildSetApplyInfoAck, CmdGuildSetApplyInfoAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.CancleMergeReq, (ushort)CmdGuild.CancleMergeAck, OnGuildCancleMergeAck, CmdGuildCancleMergeAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.ChangeMyNewsReq, (ushort)CmdGuild.ChangeMyNewsAck, OnGuildChangeMyNewsAck, CmdGuildChangeMyNewsAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.BranchMergeReq, (ushort)CmdGuild.BranchMergeAck, OnGuildBranchMergeAck, CmdGuildBranchMergeAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.NewApplyMmberNtf, OnGuildNewApplyMmberNtf, CmdGuildNewApplyMmberNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.SkillUpgradeNtf, OnGuildSkillUpgradeNtf, CmdGuildSkillUpgradeNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.RefuseApplyNtf, OnRefuseApplyNtf, CmdGuildRefuseApplyNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.IdNtf, OnIdNtf, CmdGuildIdNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.TodayDonateNtf, OnTodayDonateNtf, CmdGuildTodayDonateNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.InviteReq, (ushort)CmdGuild.InviteNtf, OnGuildInviteNtf, CmdGuildInviteNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.StaminaNtf, OnGuildStaminaNtf, CmdGuildStaminaNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.ContributionNtf, OnGuildContributionNtf, CmdGuildContributionNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.CurrencyNtf, OnGuildCurrencyNtf, CmdGuildCurrencyNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.ProsperityLvlNtf, OnGuildProsperityLvlNtf, CmdGuildProsperityLvlNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetCuisineInfoReq, (ushort)CmdGuild.GetCuisineInfoAck, OnGuildGetCuisineInfoAck, CmdGuildGetCuisineInfoAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.HandInCuisineReq, (ushort)CmdGuild.HandInCuisineAck, OnGuildHandInCuisineAck, CmdGuildHandInCuisineAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.PartyStarNtf, OnGuildPartyStarNtf, CmdGuildPartyStarNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetCuisineIngredientReq, (ushort)CmdGuild.GetCuisineIngredientAck, OnGetCuisineIngredientAck, CmdGuildGetCuisineIngredientAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetCuisineRecordReq, (ushort)CmdGuild.GetCuisineRecordAck, OnGetCuisineRecordAck, CmdGuildGetCuisineRecordAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.PartyMonsterNtf, OnGuildPartyMonsterNtf, CmdGuildPartyMonsterNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.ChangeNameNtf, OnGuildChangeNameNtf, CmdGuildChangeNameNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.PosChangeNtf, OnGuildPosChangeNtf, CmdGuildPosChangeNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildAuction.ListReq, (ushort)CmdGuildAuction.ListAck, OnGuildAuctionListAck, CmdGuildAuctionListAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildAuction.WatchReq, (ushort)CmdGuildAuction.WatchAck, OnGuildAuctionWatchAck, CmdGuildAuctionWatchAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildAuction.MyInfoReq, (ushort)CmdGuildAuction.MyInfoAck, OnGuildAuctionMyInfoAck, CmdGuildAuctionMyInfoAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuildAuction.ListUpdateNtf, OnGuildAuctionListUpdateNtf, CmdGuildAuctionListUpdateNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildAuction.RecordReq, (ushort)CmdGuildAuction.RecordAck, OnGuildAuctionRecordAck, CmdGuildAuctionRecordAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildAuction.BidReq, (ushort)CmdGuildAuction.BidAck, OnGuildAuctionBidAck, CmdGuildAuctionBidAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildAuction.OnePriceReq, (ushort)CmdGuildAuction.OnePriceAck, OnGuildAuctionOnePriceAck, CmdGuildAuctionOnePriceAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuildAuction.NewNtf, OnGuildAuctionNewNtf, CmdGuildAuctionNewNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildPet.GetInfoReq, (ushort)CmdGuildPet.GetInfoRes, OnGuildPetGetInfoRes, CmdGuildPetGetInfoRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildPet.UpdatePetInfoReq, (ushort)CmdGuildPet.UpdatePetInfoRes, OnGuildPetUpdatePetInfoRes, CmdGuildPetUpdatePetInfoRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildPet.SetTrainingReq, (ushort)CmdGuildPet.SetTrainingRes, OnGuildPetSetTrainingRes, CmdGuildPetSetTrainingRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildPet.GetFeedInfoReq, (ushort)CmdGuildPet.GetFeedInfoRes, OnGuildPetGetFeedInfoRes, CmdGuildPetGetFeedInfoRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildPet.FeedReq, (ushort)CmdGuildPet.FeedRes, OnGuildPetFeedRes, CmdGuildPetFeedRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildPet.ChangeNameReq, (ushort)CmdGuildPet.ChangeNameRes, OnGuildPetChangeNameRes, CmdGuildPetChangeNameRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildPet.ChangeNoticeReq, (ushort)CmdGuildPet.ChangeNoticeRes, OnGuildPetChangeNoticeRes, CmdGuildPetChangeNoticeRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildPet.GetTrainingScoreReq, (ushort)CmdGuildPet.GetTrainingScoreRes, OnGuildPetGetTrainingScoreRes, CmdGuildPetGetTrainingScoreRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildPet.GetGuildPetReq, (ushort)CmdGuildPet.GetGuildPetRes, OnGuildPetGetGuildPetRes, CmdGuildPetGetGuildPetRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildPet.GetNoticeReq, (ushort)CmdGuildPet.GetNoticeRes, OnGuildPetGetNoticeRes, CmdGuildPetGetNoticeRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuildPet.QueryRankReq, (ushort)CmdGuildPet.QueryRankRes, OnGuildPetQueryRankRes, CmdGuildPetQueryRankRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuildPet.FightEndNtf, OnGuildPetFightEndNtf, CmdGuildPetFightEndNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuildAuction.MyInfoUpdateNtf, OnGuildAuctionMyInfoUpdateNtf, CmdGuildAuctionMyInfoUpdateNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.EnterLimitNtf, OnGuildEnterLimitNtf, CmdGuildEnterLimitNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.MergeApplyNtf, OnGuildMergeApplyNtf, CmdGuildMergeApplyNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.CancelMergeNtf, OnGuildCancelMergeNtf, CmdGuildCancelMergeNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.IndustryTaskFinishNtf, OnGuildIndustryTaskFinishNtf, CmdGuildIndustryTaskFinishNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.DataNty, OnDataNty, CmdGuildDataNty.Parser);
            }
            else
            {

                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.CreateAck, OnGuildCreateAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ApplyAck, OnGuildApplyAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ChangeNameAck, OnGuildChangeNameAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ChangeNoticeAck, OnGuildChangeNoticeAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.SigninAck, OnGuildSigninAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GuildNotifyAck, OnGuildNotifyAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GuildNotifyNtf, OnGuildNotifyNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.HandleApplyAck, OnGuildHandleApplyAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetBonusAck, OnGuildGetBonusAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.KickMemberAck, OnGuildKickMemberAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.KickMemberNtf, OnGuildKickMemberNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.QuitAck, OnGuildQuitAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.OutgoingLeaderAck, OnGuildOutgoingLeaderAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.LeaderChangeNtf, OnGuildLeaderChangeNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.UpgradeBuildingAck, OnGuildUpgradeBuildingAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.DonateAck, OnGuildDonateAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetDonateRewardAck, OnGuildGetDonateRewardAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.MergeAck, OnGuildMergeAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.HandleMergeApplyAck, OnGuildHandleMergeApplyAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.SystemMergeNtf, OnGuildSystemMergeNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetGuildListAck, OnGuildGetGuildListAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.FindGuildAck, OnGuildFindGuildAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetGuildInfoAck, OnGuildGetGuildInfoAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.CreateBranchAck, OnGuildCreateBranchAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.BranchMoveAck, OnGuildBranchMoveAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.UpgradeWorkshopSkillAck, OnGuildUpgradeWorkshopSkillAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.DestroyBranchAck, OnGuildDestroyBranchAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ChangePositionAck, OnGuildChangePositionAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.HandleApplyNtf, OnGuildHandleApplyNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ChangeDstMergeGuildAck, OnGuildChangeDstMergeGuildAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ChangeDstMergeGuildNtf, OnGuildChangeDstMergeGuildNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetMemberInfoAck, OnGuildGetMemberInfoAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetMyApplyListAck, OnGuildGetMyApplyListAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.OneKeyApplyAck, OnGuildOneKeyApplyAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.SceneInfoNtf, OnGuildSceneInfoNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GameInfoNtf, OnGuildGameInfoNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ChangeBranchNameAck, OnGuildChangeBranchNameAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetGuildApplyMemberAck, OnGuildGetGuildApplyMemberAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.SetApplyInfoAck, OnGuildSetApplyInfoAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.CancleMergeAck, OnGuildCancleMergeAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ChangeMyNewsAck, OnGuildChangeMyNewsAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.BranchMergeAck, OnGuildBranchMergeAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.NewApplyMmberNtf, OnGuildNewApplyMmberNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.SkillUpgradeNtf, OnGuildSkillUpgradeNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.RefuseApplyNtf, OnRefuseApplyNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.IdNtf, OnIdNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.TodayDonateNtf, OnTodayDonateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.InviteNtf, OnGuildInviteNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.StaminaNtf, OnGuildStaminaNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ContributionNtf, OnGuildContributionNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.CurrencyNtf, OnGuildCurrencyNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ProsperityLvlNtf, OnGuildProsperityLvlNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetCuisineInfoAck, OnGuildGetCuisineInfoAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.HandInCuisineAck, OnGuildHandInCuisineAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.PartyStarNtf, OnGuildPartyStarNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetCuisineIngredientAck, OnGetCuisineIngredientAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetCuisineRecordAck, OnGetCuisineRecordAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.PartyMonsterNtf, OnGuildPartyMonsterNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.ChangeNameNtf, OnGuildChangeNameNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.PosChangeNtf, OnGuildPosChangeNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildAuction.ListAck, OnGuildAuctionListAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildAuction.WatchAck, OnGuildAuctionWatchAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildAuction.MyInfoAck, OnGuildAuctionMyInfoAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildAuction.ListUpdateNtf, OnGuildAuctionListUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildAuction.RecordAck, OnGuildAuctionRecordAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildAuction.BidAck, OnGuildAuctionBidAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildAuction.OnePriceAck, OnGuildAuctionOnePriceAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildAuction.NewNtf, OnGuildAuctionNewNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.GetInfoRes, OnGuildPetGetInfoRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.UpdatePetInfoRes, OnGuildPetUpdatePetInfoRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.SetTrainingRes, OnGuildPetSetTrainingRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.GetFeedInfoRes, OnGuildPetGetFeedInfoRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.FeedRes, OnGuildPetFeedRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.ChangeNameRes, OnGuildPetChangeNameRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.ChangeNoticeRes, OnGuildPetChangeNoticeRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.GetTrainingScoreRes, OnGuildPetGetTrainingScoreRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.GetGuildPetRes, OnGuildPetGetGuildPetRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.GetNoticeRes, OnGuildPetGetNoticeRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.QueryRankRes, OnGuildPetQueryRankRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildPet.FightEndNtf, OnGuildPetFightEndNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildAuction.MyInfoUpdateNtf, OnGuildAuctionMyInfoUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.EnterLimitNtf, OnGuildEnterLimitNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.MergeApplyNtf, OnGuildMergeApplyNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.CancelMergeNtf, OnGuildCancelMergeNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.IndustryTaskFinishNtf, OnGuildIndustryTaskFinishNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.DataNty, OnDataNty);
            }
            ProcessEvents_Consign(toRegister);
            ProcessEvents_RedPacket(toRegister);
        }
        #endregion
        #region 数据处理
        private void FamilyDataInit()
        {
            FamilyPartyDataInit();
            FamilyConsignDataInit();
            FamilyRedPacketInit();
        }
        private void FamilyPartyDataInit()
        {
            familyData.familyPartyInfo.dictPartyDataByCastleLv.Clear();
            familyData.familyPartyInfo.listValueStage.Clear();
            familyData.familyPartyInfo.dictPartyDataByStarNum.Clear();

            var familyReceptions = CSVFamilyReception.Instance.GetAll();
            for (int i = 0, len = familyReceptions.Count; i < len; i++)
            {
                CSVFamilyReception.Data partyData = familyReceptions[i];
                if (familyData.familyPartyInfo.dictPartyDataByCastleLv.TryGetValue(partyData.familyCastleLv, out CSVFamilyReception.Data dictPartyData))
                {
                    if (partyData.receptionValue > dictPartyData.receptionValue)
                    {
                        familyData.familyPartyInfo.dictPartyDataByCastleLv[partyData.familyCastleLv] = partyData;
                    }
                }
                else
                {
                    familyData.familyPartyInfo.dictPartyDataByCastleLv.Add(partyData.familyCastleLv, partyData);
                }
                familyData.familyPartyInfo.dictPartyDataByStarNum.Add(partyData.receptionStar, partyData);
                familyData.familyPartyInfo.listValueStage.Add(partyData.receptionValue);
            }
            familyData.familyPartyInfo.listValueStage.Sort();
        }
        #endregion
        #region 服务器发送消息
        /// <summary>
        /// 创建家族请求
        /// </summary>
        /// <param name="name"></param>
        /// <param name="notice"></param>
        public void SendGuildCreateReq(string name, string notice)
        {
            CmdGuildCreateReq req = new CmdGuildCreateReq();
            req.Name = ByteString.CopyFromUtf8(name);
            req.Notice = ByteString.CopyFromUtf8(notice);
            NetClient.Instance.SendMessage((ushort)CmdGuild.CreateReq, req);
        }
        /// <summary>
        /// 申请家族请求
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="bApply"></param>
        public void SendGuildApplyReq(ulong guildId, bool bApply)
        {
            if (!Sys_Family.Instance.CanJoinFamily(bApply))
            {
                return;
            }
            CmdGuildApplyReq req = new CmdGuildApplyReq();
            req.GuildId = guildId;
            req.BApply = bApply;
            NetClient.Instance.SendMessage((ushort)CmdGuild.ApplyReq, req);
        }
        /// <summary>
        /// 家族改名称请求
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectType"></param>
        public void SendGuildChangeNameReq(string name, uint selectType)
        {
            CmdGuildChangeNameReq req = new CmdGuildChangeNameReq();
            req.Name = ByteString.CopyFromUtf8(name);
            req.ItemType = selectType;
            NetClient.Instance.SendMessage((ushort)CmdGuild.ChangeNameReq, req);
        }
        /// <summary>
        /// 家族改宣言请求
        /// </summary>
        public void SendGuildChangeNoticeReq(string notice)
        {
            CmdGuildChangeNoticeReq req = new CmdGuildChangeNoticeReq();
            req.Notice = ByteString.CopyFromUtf8(notice);
            NetClient.Instance.SendMessage((ushort)CmdGuild.ChangeNoticeReq, req);
        }
        /// <summary>
        /// 家族签到请求
        /// </summary>
        public void SendGuildSigninReq()
        {
            CmdGuildSigninReq req = new CmdGuildSigninReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.SigninReq, req);
        }
        /// <summary>
        /// 家族群发消息请求
        /// </summary>
        /// <param name="msg"></param>
        public void SendGuildNotifyReq(string msg)
        {
            CmdGuildGuildNotifyReq req = new CmdGuildGuildNotifyReq();
            req.Msg = ByteString.CopyFromUtf8(msg);
            NetClient.Instance.SendMessage((ushort)CmdGuild.GuildNotifyReq, req);
        }
        /// <summary>
        /// 处理家族申请请求
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="isAgree"></param>
        public void SendGuildHandleApplyReq(ulong roleId, bool isAgree)
        {
            CmdGuildHandleApplyReq req = new CmdGuildHandleApplyReq();
            req.RoleId = roleId;
            req.IsAgree = isAgree;
            NetClient.Instance.SendMessage((ushort)CmdGuild.HandleApplyReq, req);
        }
        /// <summary>
        /// 家族分红请求
        /// </summary>
        public void SendGuildGetBonusReq()
        {
            CmdGuildGetBonusReq req = new CmdGuildGetBonusReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetBonusReq, req);
        }
        /// <summary>
        /// 家族请离请求
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="msg"></param>
        public void SendGuildKickMemberReq(ulong roleId, string msg)
        {
            CmdGuildKickMemberReq req = new CmdGuildKickMemberReq();
            req.RoleId = roleId;
            req.Msg = ByteString.CopyFromUtf8(msg);
            NetClient.Instance.SendMessage((ushort)CmdGuild.KickMemberReq, req);
        }
        /// <summary>
        /// 家族退出请求
        /// </summary>
        public void SendGuildQuitReq()
        {
            CmdGuildQuitReq req = new CmdGuildQuitReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.QuitReq, req);
        }
        /// <summary>
        /// 家族卸任族长请求
        /// </summary>
        /// <param name="roleId"></param>
        public void SendGuildOutgoingLeaderReq(ulong roleId)
        {
            CmdGuildOutgoingLeaderReq req = new CmdGuildOutgoingLeaderReq();
            req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdGuild.OutgoingLeaderReq, req);
        }
        /// <summary>
        /// 家族升级建筑请求
        /// </summary>
        /// <param name="buildingType"></param>
        public void SendGuildUpgradeBuildingReq(uint buildingType)
        {
            CmdGuildUpgradeBuildingReq req = new CmdGuildUpgradeBuildingReq();
            req.BuildingType = buildingType;
            NetClient.Instance.SendMessage((ushort)CmdGuild.UpgradeBuildingReq, req);
        }
        /// <summary>
        /// 家族捐钱请求
        /// </summary>
        /// <param name="donateId"></param>
        public void SendGuildDonateReq(uint donateId)
        {
            CmdGuildDonateReq req = new CmdGuildDonateReq();
            req.DonateId = donateId;
            NetClient.Instance.SendMessage((ushort)CmdGuild.DonateReq, req);
        }
        /// <summary>
        /// 家族领捐赠奖励请求
        /// </summary>
        /// <param name="index"></param>
        public void SendGuildGetDonateRewardReq(uint index)
        {
            CmdGuildGetDonateRewardReq req = new CmdGuildGetDonateRewardReq();
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetDonateRewardReq, req);
        }
        /// <summary>
        /// 家族合并请求
        /// </summary>
        /// <param name="guildId"> 需要收到请求的 工会id </param>
        /// <param name="dstGuildId"> 合并后存在的 工会id </param>
        public void SendGuildMergeReq(ulong guildId, ulong dstGuildId)
        {
            CmdGuildMergeReq req = new CmdGuildMergeReq();
            req.GuildId = guildId;
            req.DstGuildId = dstGuildId;
            NetClient.Instance.SendMessage((ushort)CmdGuild.MergeReq, req);
        }
        /// <summary>
        /// 家族处理合并请求
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="dstguildId"></param>
        /// <param name="bAgree"></param>
        public void SendGuildHandleMergeApplyReq(ulong guildId, ulong dstguildId, bool bAgree)
        {
            CmdGuildHandleMergeApplyReq req = new CmdGuildHandleMergeApplyReq();
            req.GuildId = guildId;
            req.DstguildId = dstguildId;
            req.BAgree = bAgree;
            NetClient.Instance.SendMessage((ushort)CmdGuild.HandleMergeApplyReq, req);
        }
        /// <summary>
        /// 家族列表请求
        /// </summary>
        /// <param name="guildId"></param>
        public void SendGuildGetGuildListReq(ulong guildId)
        {
            CmdGuildGetGuildListReq req = new CmdGuildGetGuildListReq();
            req.GuildId = guildId;
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetGuildListReq, req);
        }
        /// <summary>
        /// 查找家族请求
        /// </summary>
        /// <param name="text"></param>
        public void SendGuildFindGuildReq(string text)
        {
            CmdGuildFindGuildReq req = new CmdGuildFindGuildReq();
            req.BMulti = false;
            req.Name = ByteString.CopyFromUtf8(text);
            uint id = 0;
            if (uint.TryParse(text, out id))
            {
                req.GuildId.Add(id);
            }
            NetClient.Instance.SendMessage((ushort)CmdGuild.FindGuildReq, req);
        }
        /// <summary>
        /// 查询合并列表
        /// </summary>
        public void SendGuildFindGuildReq(List<ulong> list)
        {
            CmdGuildFindGuildReq req = new CmdGuildFindGuildReq();
            req.BMulti = true;
            req.GuildId.Add(list);
            NetClient.Instance.SendMessage((ushort)CmdGuild.FindGuildReq, req);
        }
        /// <summary>
        /// 获得家族简明信息请求
        /// </summary>
        public void SendGuildGetGuildInfoReq()
        {
            CmdGuildGetGuildInfoReq req = new CmdGuildGetGuildInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetGuildInfoReq, req);
        }
        /// <summary>
        /// 家族创建分会请求
        /// </summary>
        /// <param name="name"></param>
        public void SendGuildCreateBranchReq(string name)
        {
            CmdGuildCreateBranchReq req = new CmdGuildCreateBranchReq();
            req.Name = ByteString.CopyFromUtf8(name);
            NetClient.Instance.SendMessage((ushort)CmdGuild.CreateBranchReq, req);
        }
        /// <summary>
        /// 家族分会主会成员移动请求
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="member"></param>
        /// <param name="branchMember"></param>
        public void SendGuildBranchMoveReq(uint branchId, List<ulong> member, List<ulong> branchMember)
        {
            CmdGuildBranchMoveReq req = new CmdGuildBranchMoveReq();
            req.BranchId = branchId;
            req.Member.Clear();
            req.Member.AddRange(member);
            req.BranchMember.Clear();
            req.BranchMember.AddRange(branchMember);
            NetClient.Instance.SendMessage((ushort)CmdGuild.BranchMoveReq, req);
        }
        /// <summary>
        /// 家族升级工坊技能请求
        /// </summary>
        /// <param name="skillId"></param>
        public void SendGuildUpgradeWorkshopSkillReq(uint skillId)
        {
            CmdGuildUpgradeWorkshopSkillReq req = new CmdGuildUpgradeWorkshopSkillReq();
            req.SkillId = skillId;
            NetClient.Instance.SendMessage((ushort)CmdGuild.UpgradeWorkshopSkillReq, req);
        }
        /// <summary>
        /// 家族移除分会请求
        /// </summary>
        /// <param name="branchid"></param>
        public void SendGuildDestroyBranchReq(uint branchid)
        {
            CmdGuildDestroyBranchReq req = new CmdGuildDestroyBranchReq();
            req.Branchid = branchid;
            NetClient.Instance.SendMessage((ushort)CmdGuild.DestroyBranchReq, req);
        }
        /// <summary>
        /// 家族改变职位请求
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="position"></param>
        public void SendGuildChangePositionReq(ulong roleId, uint position)
        {
            CmdGuildChangePositionReq req = new CmdGuildChangePositionReq();
            req.RoleId = roleId;
            req.Position = position;
            NetClient.Instance.SendMessage((ushort)CmdGuild.ChangePositionReq, req);
        }
        /// <summary>
        /// 改变合并的目标家族
        /// </summary>
        /// <param name="otherGuildId"></param>
        /// <param name="dstGuildId"></param>
        public void SendGuildChangeDstMergeGuildReq(ulong otherGuildId, ulong dstGuildId)
        {
            CmdGuildChangeDstMergeGuildReq req = new CmdGuildChangeDstMergeGuildReq();
            req.OtherGuildId = otherGuildId;
            req.DstGuildId = dstGuildId;
            NetClient.Instance.SendMessage((ushort)CmdGuild.ChangeDstMergeGuildReq, req);
        }
        /// <summary>
        /// 获取成员信息
        /// </summary>
        public void SendGuildGetMemberInfoReq()
        {
            CmdGuildGetMemberInfoReq req = new CmdGuildGetMemberInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetMemberInfoReq, req);
        }
        /// <summary>
        /// 我的申请列表
        /// </summary>
        public void SendGuildGetMyApplyListReq()
        {
            CmdGuildGetMyApplyListReq req = new CmdGuildGetMyApplyListReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetMyApplyListReq, req);
        }
        /// <summary>
        /// 一键申请
        /// </summary>
        public void SendGuildOneKeyApplyReq()
        {
            CmdGuildOneKeyApplyReq req = new CmdGuildOneKeyApplyReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.OneKeyApplyReq, req);
        }
        /// <summary>
        /// 分会改名
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="name"></param>
        public void SendGuildChangeBranchNameReq(uint branchId, string name)
        {
            CmdGuildChangeBranchNameReq req = new CmdGuildChangeBranchNameReq();
            req.BranchId = branchId;
            req.Name = ByteString.CopyFromUtf8(name);
            NetClient.Instance.SendMessage((ushort)CmdGuild.ChangeBranchNameReq, req);
        }
        /// <summary>
        /// 查看公会申请成员
        /// </summary>
        public void SendGuildGetGuildApplyMemberReq()
        {
            CmdGuildGetGuildApplyMemberReq req = new CmdGuildGetGuildApplyMemberReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetGuildApplyMemberReq, req);
        }
        /// <summary>
        /// 设置自动申请信息
        /// </summary>
        public void SendGuildSetApplyInfoReq(bool autoAgree, uint minLv)
        {
            CmdGuildSetApplyInfoReq req = new CmdGuildSetApplyInfoReq();
            req.AutoAgree = autoAgree;
            req.Lvl = minLv;
            NetClient.Instance.SendMessage((ushort)CmdGuild.SetApplyInfoReq, req);
        }
        /// <summary>
        /// 取消家族合并
        /// </summary>
        public void SendGuildCancleMergeReq()
        {
            CmdGuildCancleMergeReq req = new CmdGuildCancleMergeReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.CancleMergeReq, req);
        }
        /// <summary>
        /// 不显示自己的动态
        /// </summary>
        /// <param name="BDisplay"></param>
        public void SendGuildChangeMyNewsReq(bool BDisplay)
        {
            CmdGuildChangeMyNewsReq req = new CmdGuildChangeMyNewsReq();
            req.BDisplay = BDisplay;
            NetClient.Instance.SendMessage((ushort)CmdGuild.ChangeMyNewsReq, req);
        }
        /// <summary>
        /// 分会合并
        /// </summary>
        /// <param name="SrcId"></param>
        /// <param name="DstId"></param>
        public void SendGuildBranchMergeReq(uint SrcId, uint DstId)
        {
            CmdGuildBranchMergeReq req = new CmdGuildBranchMergeReq();
            req.SrcId = SrcId;
            req.DstId = DstId;
            NetClient.Instance.SendMessage((ushort)CmdGuild.BranchMergeReq, req);
        }
        /// <summary>
        /// 邀请玩家入会
        /// </summary>
        /// <param name="roleId"></param>
        public void SendGuildInviteReq(ulong roleId)
        {
            CmdGuildInviteReq req = new CmdGuildInviteReq();
            req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdGuild.InviteReq, req);
        }
        /// <summary>
        /// 回馈邀请请求
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="roleId"></param>
        /// <param name="roleName"></param>
        public void SendGuildInviteRpl(ulong guildId, ulong roleId, string roleName, uint agreeType)
        {
            if (!CanJoinFamily(agreeType == 0))
            {
                return;
            }
            CmdGuildInviteRpl req = new CmdGuildInviteRpl();
            req.GuildId = guildId;
            req.RoleId = roleId;
            req.RoleName = ByteString.CopyFromUtf8(roleName);
            req.AgreeType = agreeType;
            NetClient.Instance.SendMessage((ushort)CmdGuild.InviteRpl, req);
        }


        /// <summary>
        /// 提交道具
        /// </summary>
        /// <param name="pushDatas"></param>
        public void SendGuildHandInItemProsperityReq(List<ItemData> pushDatas)
        {
            CmdGuildHandInItemProsperityReq req = new CmdGuildHandInItemProsperityReq();
            for (int i = 0; i < pushDatas.Count; i++)
            {
                var itemData = pushDatas[i];
                CmdGuildHandInItemProsperityReq.Types.ItemInfo item = new CmdGuildHandInItemProsperityReq.Types.ItemInfo();
                item.ItemUid = itemData.Uuid;
                item.Count = itemData.Count;
                item.InfoId = itemData.Id;
                req.Items.Add(item);
            }
            NetClient.Instance.SendMessage((ushort)CmdGuild.HandInItemProsperityReq, req);
        }

        /// <summary>获取今日菜肴id</summary>
        public void GetCuisineInfoReq()
        {
            CmdGuildGetCuisineInfoReq req = new CmdGuildGetCuisineInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetCuisineInfoReq, req);
        }

        /// <summary> 上交菜品 </summary>
        public void HandInCuisineReq(List<ItemData> items)
        {
            CmdGuildHandInCuisineReq req = new CmdGuildHandInCuisineReq();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var itemInfo = new CmdGuildHandInCuisineReq.Types.ItemInfo { ItemUid = item.Uuid, Count = 1, InfoId = item.Id };
                familyData.familyPartyInfo.lastSubmitFoodItemId = item.Id;
                req.Items.Add(itemInfo);
            }
            NetClient.Instance.SendMessage((ushort)CmdGuild.HandInCuisineReq, req);
        }
        /// <summary>领取菜肴素材</summary>
        public void GetCuisineIngredientReq()
        {
            CmdGuildGetCuisineIngredientReq req = new CmdGuildGetCuisineIngredientReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetCuisineIngredientReq, req);
        }
        /// <summary>获取酒会记录</summary>
        public void GetCuisineRecordReq()
        {
            CmdGuildGetCuisineRecordReq req = new CmdGuildGetCuisineRecordReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetCuisineRecordReq, req);
        }

        /// <summary>
        /// 接取建设任务
        /// </summary>
        /// <param name="type"></param>
        public void GuildAcceptIndustryTaskReq(uint type)
        {
            CmdGuildAcceptIndustryTaskReq req = new CmdGuildAcceptIndustryTaskReq();
            req.IndustryType = type;
            NetClient.Instance.SendMessage((ushort)CmdGuild.AcceptIndustryTaskReq, req);
        }
        #endregion
        #region 服务器接收消息
        /// <summary>
        /// scenesvr 上的信息通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildSceneInfoNtf(NetMsg msg)
        {
            CmdGuildSceneInfoNtf ntf = NetMsgUtil.Deserialize<CmdGuildSceneInfoNtf>(CmdGuildSceneInfoNtf.Parser, msg);
            familyData.familyPlayerInfo.cmdGuildSceneInfoNtf = ntf;
            consignFirstOpen = ntf.ConsignFirstOpenFlag;
            eventEmitter.Trigger<CmdGuildSceneInfoNtf>(EEvents.OnSceneInfoNtf, ntf);
        }
        /// <summary>
        /// gamesvr 上的信息通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildGameInfoNtf(NetMsg msg)
        {
            CmdGuildGameInfoNtf ntf = NetMsgUtil.Deserialize<CmdGuildGameInfoNtf>(CmdGuildGameInfoNtf.Parser, msg);
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf = ntf;
            SendFamilyInfo();
        }
        /// <summary>
        /// 获得家族信息
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildGetGuildInfoAck(NetMsg msg)
        {
            CmdGuildGetGuildInfoAck res = NetMsgUtil.Deserialize<CmdGuildGetGuildInfoAck>(CmdGuildGetGuildInfoAck.Parser, msg);
            if (!res.NoChange)
            {
                familyData.familyDetailInfo.cmdGuildGetGuildInfoAck = res;
                familyData.familyBuildInfo.UpdateData(res.Info.AllBuildings);
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateFamilyInfo);
        }
        /// <summary>
        /// 获取成员信息
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildGetMemberInfoAck(NetMsg msg)
        {
            CmdGuildGetMemberInfoAck res = NetMsgUtil.Deserialize<CmdGuildGetMemberInfoAck>(CmdGuildGetMemberInfoAck.Parser, msg);
            Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetMemberInfoAck = res;
            Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.OnlineMember = res.OnlineMember;
            Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.MemberCount =
              (uint)Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member.Count;
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMember);
        }
        /// <summary>
        /// 查看公会申请成员
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildGetGuildApplyMemberAck(NetMsg msg)
        {
            CmdGuildGetGuildApplyMemberAck res = NetMsgUtil.Deserialize<CmdGuildGetGuildApplyMemberAck>(CmdGuildGetGuildApplyMemberAck.Parser, msg);
            familyData.familyDetailInfo.cmdGuildGetGuildApplyMemberAck = res;
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.GetApplyMember);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyApplyMember, null);
        }
        /// <summary>
        /// 公会申请成员增量
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildNewApplyMmberNtf(NetMsg msg)
        {
            CmdGuildNewApplyMmberNtf ntf = NetMsgUtil.Deserialize<CmdGuildNewApplyMmberNtf>(CmdGuildNewApplyMmberNtf.Parser, msg);
            var applyMemberList = familyData.familyDetailInfo.cmdGuildGetGuildApplyMemberAck.List;
            if (ntf.ClearList)
            {
                applyMemberList.Clear();
            }
            else
            {
                ApplyMember applyMember = null;
                for (int i = 0; i < applyMemberList.Count; i++)
                {
                    var roleInfo = applyMemberList[i];
                    if (roleInfo.RoleId == ntf.RoleInfo.RoleId)
                    {
                        applyMember = roleInfo;
                        break;
                    }
                }

                if (ntf.Add)
                {
                    if (null == applyMember)
                    {
                        applyMemberList.Add(ntf.RoleInfo);
                    }
                }
                else
                {
                    if (null != applyMember)
                    {
                        applyMemberList.Remove(applyMember);
                    }
                }
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateApplyMember);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyApplyMember, null);
        }
        /// <summary>
        /// 家族列表
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildGetGuildListAck(NetMsg msg)
        {
            CmdGuildGetGuildListAck res = NetMsgUtil.Deserialize<CmdGuildGetGuildListAck>(CmdGuildGetGuildListAck.Parser, msg);
            for (int i = 0; i < res.Infos.Count; i++)
            {
                var resInfo = res.Infos[i];
                bool hasSameGuild = false;
                for (int j = 0; j < familyData.queryFamilyInfo.familyList.Count; j++)
                {
                    if(resInfo.GuildId == familyData.queryFamilyInfo.familyList[j].GuildId)
                    {
                        hasSameGuild = true;
                        break;
                    }
                }
                if(!hasSameGuild)
                {
                    familyData.queryFamilyInfo.familyList.Add(resInfo);
                }
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateFamilyList);
        }
        /// <summary>
        /// 查找家族
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildFindGuildAck(NetMsg msg)
        {
            CmdGuildFindGuildAck res = NetMsgUtil.Deserialize<CmdGuildFindGuildAck>(CmdGuildFindGuildAck.Parser, msg);
            if (res.BMulti)
            {
                familyData.queryFamilyInfo.mergeList.Clear();
                familyData.queryFamilyInfo.mergeList.AddRange(res.GuildList);
                Sys_Family.Instance.eventEmitter.Trigger(EEvents.GetMergeFamilyListRes);
            }
            else
            {
                familyData.queryFamilyInfo.queryList.Clear();
                familyData.queryFamilyInfo.queryList.AddRange(res.GuildList);
                Sys_Family.Instance.eventEmitter.Trigger(EEvents.GetQueryFamilyListRes);
                if (res.GuildList.Count <= 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10636));
                }
            }
        }
        bool showAddFamilyTips = false;
        /// <summary>
        /// 我的申请列表
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildGetMyApplyListAck(NetMsg msg)
        {
            CmdGuildGetMyApplyListAck res = NetMsgUtil.Deserialize<CmdGuildGetMyApplyListAck>(CmdGuildGetMyApplyListAck.Parser, msg);
            var list = familyData.familyPlayerInfo.applyList;
            list.Clear();
            for (int i = 0, count = res.Infos.Count; i < count; i++)
            {
                list.Add(res.Infos[i].GuildId);
            }
            if(!showAddFamilyTips)
            {
                showAddFamilyTips = true;
                if (familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildId == 0 && Sys_FunctionOpen.Instance.IsOpen(30401))
                {
                    Logic.Core.UIScheduler.Push(EUIID.UI_Family_JoinTips, null, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
                }
            }
            
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateApplyFamilyList);
        }
        /// <summary>
        /// 申请家族
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildApplyAck(NetMsg msg)
        {
            CmdGuildApplyAck res = NetMsgUtil.Deserialize<CmdGuildApplyAck>(CmdGuildApplyAck.Parser, msg);
            var list = familyData.familyPlayerInfo.applyList;
            if (res.BApply)
            {
                if (!list.Contains(res.GuildId))
                    list.Add(res.GuildId);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10026));
            }
            else
            {
                if (list.Contains(res.GuildId))
                    list.Remove(res.GuildId);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10052));
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateApplyFamilyList);
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event23);
        }
        /// <summary>
        /// 一键申请
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildOneKeyApplyAck(NetMsg msg)
        {
            CmdGuildOneKeyApplyAck res = NetMsgUtil.Deserialize<CmdGuildOneKeyApplyAck>(CmdGuildOneKeyApplyAck.Parser, msg);
            var list = familyData.familyPlayerInfo.applyList;
            foreach (var item in res.GuildList)
            {
                if (!list.Contains(item.GuildId))
                {
                    list.Add(item.GuildId);
                }
            }
            if (list.Count > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10026));
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10890));
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateApplyFamilyList);
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event23);
        }
        /// <summary>
        /// 创建家族
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildCreateAck(NetMsg msg)
        {
            CmdGuildCreateAck res = NetMsgUtil.Deserialize<CmdGuildCreateAck>(CmdGuildCreateAck.Parser, msg);
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildId = res.GuildId;
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildUid = res.GuildUid;
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.CreateFamily);
            needShowFraudTip = true;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10020));
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event23);
        }
        public bool needShowFraudTip = false;
        /// <summary>
        /// 通知申请人处理结果（同意才通知）
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildHandleApplyNtf(NetMsg msg)
        {
            CmdGuildHandleApplyNtf ntf = NetMsgUtil.Deserialize<CmdGuildHandleApplyNtf>(CmdGuildHandleApplyNtf.Parser, msg);
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildId = ntf.GuildId;
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildUid = ntf.GuildUid;
            if (ntf.GuildId == 0) return;
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.JoinFamily);
            needShowFraudTip = true;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10034, Sys_Role.Instance.sRoleName));
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event23);
        }
        /// <summary>
        /// 家族改名称
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildChangeNameAck(NetMsg msg)
        {
            CmdGuildChangeNameAck res = NetMsgUtil.Deserialize<CmdGuildChangeNameAck>(CmdGuildChangeNameAck.Parser, msg);
            if (null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
                familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GName = res.Name;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10621));
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateFamilyName);
        }
        /// <summary>
        /// 家族改宣言
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildChangeNoticeAck(NetMsg msg)
        {
            CmdGuildChangeNoticeAck res = NetMsgUtil.Deserialize<CmdGuildChangeNoticeAck>(CmdGuildChangeNoticeAck.Parser, msg);
            if (null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
                familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.Notice = res.Notice;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10622));
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateFamilyDeclaration);
        }
        /// <summary>
        /// 不显示自己的动态
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildChangeMyNewsAck(NetMsg msg)
        {
            CmdGuildChangeMyNewsAck res = NetMsgUtil.Deserialize<CmdGuildChangeMyNewsAck>(CmdGuildChangeMyNewsAck.Parser, msg);
            familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.DisplayMyNews = res.BDisplay;
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.ChangeMyNews);
        }
        /// <summary>
        /// 设置自动申请信息
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildSetApplyInfoAck(NetMsg msg)
        {
            CmdGuildSetApplyInfoAck res = NetMsgUtil.Deserialize<CmdGuildSetApplyInfoAck>(CmdGuildSetApplyInfoAck.Parser, msg);
            if (null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
            {
                familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AutoAgree = res.AutoAgree;
                familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.MinAutoLvl = res.Lvl;
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateSetApplyInfo);
        }
        /// <summary>
        /// 家族签到
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildSigninAck(NetMsg msg)
        {
            CmdGuildSigninAck res = NetMsgUtil.Deserialize<CmdGuildSigninAck>(CmdGuildSigninAck.Parser, msg);
            familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.BSignIn = true;
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilySign, null);
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UdateSignState);
        }
        /// <summary>
        /// 家族分红
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildGetBonusAck(NetMsg msg)
        {
            CmdGuildGetBonusAck res = NetMsgUtil.Deserialize<CmdGuildGetBonusAck>(CmdGuildGetBonusAck.Parser, msg);
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UdateBonusState);
        }
        /// <summary>
        /// 家族群发消息
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildNotifyAck(NetMsg msg)
        {
            CmdGuildGuildNotifyAck res = NetMsgUtil.Deserialize<CmdGuildGuildNotifyAck>(CmdGuildGuildNotifyAck.Parser, msg);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10998));
        }
        /// <summary>
        /// 家族群发消息
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildNotifyNtf(NetMsg msg)
        {
            CmdGuildGuildNotifyNtf ntf = NetMsgUtil.Deserialize<CmdGuildGuildNotifyNtf>(CmdGuildGuildNotifyNtf.Parser, msg);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10998));
        }
        /// <summary>
        /// 处理家族申请
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildHandleApplyAck(NetMsg msg)
        {
            CmdGuildHandleApplyAck res = NetMsgUtil.Deserialize<CmdGuildHandleApplyAck>(CmdGuildHandleApplyAck.Parser, msg);
            Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.OnlineMember = res.Online;

            if (res.BJoin)//已有家族无法加入
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10052));
            }
            else if (res.IsAgree)//同意加入
            {
                SendGuildGetMemberInfoReq();
            }
        }
        /// <summary>
        /// 家族请离
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildKickMemberAck(NetMsg msg)
        {
            CmdGuildKickMemberAck res = NetMsgUtil.Deserialize<CmdGuildKickMemberAck>(CmdGuildKickMemberAck.Parser, msg);
            Sys_Family.Instance.familyData.SetMemberStatus(res.RoleId, 0);
            Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.OnlineMember = res.Online;
            Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.MemberCount =
                  (uint)Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member.Count;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10047));
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMember);
        }
        /// <summary>
        /// 家族请离
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildKickMemberNtf(NetMsg msg)
        {
            CmdGuildKickMemberNtf ntf = NetMsgUtil.Deserialize<CmdGuildKickMemberNtf>(CmdGuildKickMemberNtf.Parser, msg);
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildId = (uint)ntf.GuildId;
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildUid = 0;
            CheckPlayerIsQuitFamily();
            severNtf = false;
            if ((null != familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info && null != familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos))
                familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos.Clear();
            needShowFraudTip = false;
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.QuitFamily);
        }
        /// <summary>
        /// 家族退出
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildQuitAck(NetMsg msg)
        {
            CmdGuildQuitAck res = NetMsgUtil.Deserialize<CmdGuildQuitAck>(CmdGuildQuitAck.Parser, msg);
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildId = 0;
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildUid = 0;
            CheckPlayerIsQuitFamily();
            severNtf = false;
            if ((null != familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info && null != familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos))
                familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos.Clear();
            needShowFraudTip = false;
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.QuitFamily);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10035, Sys_Role.Instance.sRoleName));
        }
        /// <summary>
        /// 家族卸任族长
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildOutgoingLeaderAck(NetMsg msg)
        {
            CmdGuildOutgoingLeaderAck res = NetMsgUtil.Deserialize<CmdGuildOutgoingLeaderAck>(CmdGuildOutgoingLeaderAck.Parser, msg);
            var members = familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member;
            foreach (var member in members)
            {
                if (member.Position == (uint)Sys_Family.FamilyData.EFamilyStatus.ELeader)
                {
                    severNtf = false;
                    if ((null != familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info && null != familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos))
                        familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos.Clear();
                    member.Position = (uint)Sys_Family.FamilyData.EFamilyStatus.EMember;
                }
                else if (member.RoleId == res.RoleId)
                {
                    member.Position = (uint)Sys_Family.FamilyData.EFamilyStatus.ELeader;
                }
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMember);
        }
        /// <summary>
        /// 家族更改族长
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildLeaderChangeNtf(NetMsg msg)
        {
            CmdGuildLeaderChangeNtf ntf = NetMsgUtil.Deserialize<CmdGuildLeaderChangeNtf>(CmdGuildLeaderChangeNtf.Parser, msg);
            var members = familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member;
            foreach (var member in members)
            {
                if (member.Position == (uint)Sys_Family.FamilyData.EFamilyStatus.ELeader)
                {
                    severNtf = false;
                    member.Position = (uint)Sys_Family.FamilyData.EFamilyStatus.EMember;
                }
                else if (member.RoleId == ntf.RoleId)
                {
                    member.Position = (uint)Sys_Family.FamilyData.EFamilyStatus.ELeader;
                }
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMember);
        }
        /// <summary>
        /// 家族升级建筑
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildUpgradeBuildingAck(NetMsg msg)
        {
            CmdGuildUpgradeBuildingAck res = NetMsgUtil.Deserialize<CmdGuildUpgradeBuildingAck>(CmdGuildUpgradeBuildingAck.Parser, msg);
            uint BuildingType = res.BuildingType;
            if (null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
            {
                var Info = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info;
                Info.NowUpgrade = (int)res.BuildingType;
                Info.UpgradeFinishTime = res.FinishTime;
                familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildCoin = res.GuildCoin;
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateBuildState);
        }
        /// <summary>
        /// 家族升级工坊技能
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildUpgradeWorkshopSkillAck(NetMsg msg)
        {
            CmdGuildUpgradeWorkshopSkillAck res = NetMsgUtil.Deserialize<CmdGuildUpgradeWorkshopSkillAck>(CmdGuildUpgradeWorkshopSkillAck.Parser, msg);
            uint BuildingType = res.SkillId / 10000;
            if (null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
            {
                var AllBuildings = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings;
                if (AllBuildings.Count <= BuildingType) return;
                var building = AllBuildings[(int)BuildingType];
                building.NowUpgrade = (int)res.SkillId;
                building.UpgradeFinishTime = res.FinishTime;
                familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildCoin = res.GuildCoin;
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateBuildSkillState);
        }
        /// <summary>
        /// 家族捐钱
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildDonateAck(NetMsg msg)
        {
            CmdGuildDonateAck res = NetMsgUtil.Deserialize<CmdGuildDonateAck>(CmdGuildDonateAck.Parser, msg);
            if (res.ErrorCode != 0) return;
            var donateInfo = familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.Donate;
            //捐献数据更新
            CmdGuildSceneInfoNtf.Types.DonateInfo donate = null;
            foreach (var item in donateInfo)
            {
                if (item.DonateId == res.DonateId)
                {
                    donate = item;
                    break;
                }
            }
            if (null == donate)
            {
                donate = new CmdGuildSceneInfoNtf.Types.DonateInfo();
                donateInfo.Add(donate);
            }
            donate.DonateId = res.DonateId;
            donate.Count += 1;
            //个人贡献更新
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildContribution += res.Contribution;
            //公会货币更新
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var tempCoin = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildCoin + res.Guildcoin;
            if (Sys_Family.Instance.familyData.familyBuildInfo.capitalCeiling < tempCoin)
                tempCoin = Sys_Family.Instance.familyData.familyBuildInfo.capitalCeiling;
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildCoin = tempCoin;
            //公会每日货币更新
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.TodayDonateCoin = res.TodayGuildCoin;
            //成员中的个人贡献更新
            CmdGuildGetMemberInfoAck.Types.MemberInfo memberInfo = Sys_Family.Instance.familyData.CheckMe();
            if (null != memberInfo)
            {
                memberInfo.Contribution += res.Contribution;
            }
            //捐献记录
            var donateRecords = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.DonateRecords;
            GuildDetailInfo.Types.DonateRecord donateRecord = new GuildDetailInfo.Types.DonateRecord();
            donateRecord.Name = ByteString.CopyFromUtf8(Sys_Role.Instance.sRoleName);
            donateRecord.ItemId = res.DonateId;
            donateRecord.ItemCount = res.ItemCount;
            donateRecord.GuildCoin = res.Guildcoin;
            donateRecords.Add(donateRecord);

            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateDonateData);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyDonate, null);
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event25);
        }
        /// <summary>
        /// 家族领捐赠奖励
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildGetDonateRewardAck(NetMsg msg)
        {
            CmdGuildGetDonateRewardAck res = NetMsgUtil.Deserialize<CmdGuildGetDonateRewardAck>(CmdGuildGetDonateRewardAck.Parser, msg);
            var DonateRewardIndex = familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.DonateRewardIndex;
            if (!DonateRewardIndex.Contains(res.Index))
            {
                DonateRewardIndex.Add(res.Index);
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateDonateRewardData);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyDonateReward, null);
        }
        /// <summary>
        /// 家族创建分会
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildCreateBranchAck(NetMsg msg)
        {
            CmdGuildCreateBranchAck res = NetMsgUtil.Deserialize<CmdGuildCreateBranchAck>(CmdGuildCreateBranchAck.Parser, msg);
            GuildDetailInfo.Types.BranchInfo branchInfo = new GuildDetailInfo.Types.BranchInfo();
            branchInfo.Id = res.BranchId;
            branchInfo.Name = res.Name;
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BranchMemberInfo.Add(branchInfo);
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.CreateBranch);
        }
        /// <summary>
        /// 家族移除分会
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildDestroyBranchAck(NetMsg msg)
        {
            CmdGuildDestroyBranchAck res = NetMsgUtil.Deserialize<CmdGuildDestroyBranchAck>(CmdGuildDestroyBranchAck.Parser, msg);
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var BranchMemberInfos = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BranchMemberInfo;
            GuildDetailInfo.Types.BranchInfo BranchMemberInfo = null;
            foreach (var branchMemberInfo in BranchMemberInfos)
            {
                if (branchMemberInfo.Id == res.Branchid)
                {
                    BranchMemberInfo = branchMemberInfo;
                    break;
                }
            }
            var members = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member;

            for (int i = 0; i < members.Count; i++)
            {
                uint BranchId = members[i].Position / 10000;
                if (res.Branchid == BranchId)
                {
                    members[i].Position = (uint)FamilyData.EFamilyStatus.EMember;
                }
            }
            if (null != BranchMemberInfo)
                BranchMemberInfos.Remove(BranchMemberInfo);
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.DestroyBranch);
        }
        /// <summary>
        /// 家族分会改名
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildChangeBranchNameAck(NetMsg msg)
        {
            CmdGuildChangeBranchNameAck res = NetMsgUtil.Deserialize<CmdGuildChangeBranchNameAck>(CmdGuildChangeBranchNameAck.Parser, msg);
            var BranchMemberInfos = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BranchMemberInfo;
            GuildDetailInfo.Types.BranchInfo BranchMemberInfo = null;
            foreach (var branchMemberInfo in BranchMemberInfos)
            {
                if (branchMemberInfo.Id == res.BranchId)
                {
                    BranchMemberInfo = branchMemberInfo;
                    break;
                }
            }
            if (null != BranchMemberInfo)
                BranchMemberInfo.Name = res.Name;
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateBranchName);
        }
        /// <summary>
        /// 家族分会主会成员移动
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildBranchMoveAck(NetMsg msg)
        {
            CmdGuildBranchMoveAck res = NetMsgUtil.Deserialize<CmdGuildBranchMoveAck>(CmdGuildBranchMoveAck.Parser, msg);
            List<CmdGuildGetMemberInfoAck.Types.MemberInfo> members = new List<CmdGuildGetMemberInfoAck.Types.MemberInfo>();
            members.AddRange(familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member);
            foreach (ulong ID in res.Member)
            {
                Sys_Family.Instance.familyData.SetMemberStatus(ID, (uint)FamilyData.EFamilyStatus.EMember);
            }
            foreach (ulong ID in res.BranchMember)
            {
                Sys_Family.Instance.familyData.SetMemberStatus(ID, res.BranchId * 10000 + (uint)FamilyData.EFamilyStatus.EBrachMember);
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMemberStatus);
        }
        /// <summary>
        /// 分会合并
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildBranchMergeAck(NetMsg msg)
        {
            CmdGuildBranchMergeAck res = NetMsgUtil.Deserialize<CmdGuildBranchMergeAck>(CmdGuildBranchMergeAck.Parser, msg);
            List<CmdGuildGetMemberInfoAck.Types.MemberInfo> members = new List<CmdGuildGetMemberInfoAck.Types.MemberInfo>();
            members.AddRange(familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member);
            foreach (ulong ID in res.RoleId)
            {
                //待确定
                Sys_Family.Instance.familyData.SetMemberStatus(ID, res.DstId * 10000 + (uint)FamilyData.EFamilyStatus.EBrachMember);
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMemberStatus);
        }
        /// <summary>
        /// 家族改变职位
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildChangePositionAck(NetMsg msg)
        {
            CmdGuildChangePositionAck res = NetMsgUtil.Deserialize<CmdGuildChangePositionAck>(CmdGuildChangePositionAck.Parser, msg);
            Sys_Family.Instance.familyData.SetMemberStatus(res.RoleId, res.Position);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10038));
            bool hasGet = Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.FamilyPetEgg);
            if (hasGet)
            {
                CheckCanGetFamilyCreature();
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMemberStatus);
        }
        /// <summary>
        /// 家族合并
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildMergeAck(NetMsg msg)
        {
            CmdGuildMergeAck res = NetMsgUtil.Deserialize<CmdGuildMergeAck>(CmdGuildMergeAck.Parser, msg);
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var InitiativeMerge = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.InitiativeMerge;
            InitiativeMerge.OtherId = res.GuildId;
            InitiativeMerge.DstId = res.DstGuildId;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10079));
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMergeStatus);
        }
        /// <summary>
        /// 取消家族合并
        /// </summary>
        private void OnGuildCancleMergeAck(NetMsg msg)
        {
            CmdGuildCancleMergeAck res = NetMsgUtil.Deserialize<CmdGuildCancleMergeAck>(CmdGuildCancleMergeAck.Parser, msg);
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var InitiativeMerge = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.InitiativeMerge;
            InitiativeMerge.OtherId = 0;
            InitiativeMerge.DstId = 0;
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMergeStatus);
        }
        /// <summary>
        /// 家族处理合并
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildHandleMergeApplyAck(NetMsg msg)
        {
            CmdGuildHandleMergeApplyAck res = NetMsgUtil.Deserialize<CmdGuildHandleMergeApplyAck>(CmdGuildHandleMergeApplyAck.Parser, msg);
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var AllMergeInfos = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos;
            GuildDetailInfo.Types.MergeInfo mergeInfo = null;
            foreach (var info in AllMergeInfos)
            {
                if (info.OtherId == res.GuildId)
                {
                    mergeInfo = info;
                    break;
                }
            }
            if (null != mergeInfo)
                AllMergeInfos.Remove(mergeInfo);
            var list = Sys_Family.Instance.familyData.queryFamilyInfo.mergeList;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].GuildId == res.GuildId)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
            if (AllMergeInfos.Count == 0)
            {
                severNtf = false;
            }
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyMergeApply, null);
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateApplyMergedList);
        }
        /// <summary>
        /// 家族处理合并
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildSystemMergeNtf(NetMsg msg)
        {
            CmdGuildSystemMergeNtf ntf = NetMsgUtil.Deserialize<CmdGuildSystemMergeNtf>(CmdGuildSystemMergeNtf.Parser, msg);
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GuildId = ntf.DstGuildId;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10096));
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMergeResult);
        }
        /// <summary>
        /// 改变合并的目标家族
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildChangeDstMergeGuildAck(NetMsg msg)
        {
            CmdGuildChangeDstMergeGuildAck res = NetMsgUtil.Deserialize<CmdGuildChangeDstMergeGuildAck>(CmdGuildChangeDstMergeGuildAck.Parser, msg);
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var InitiativeMerge = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.InitiativeMerge;
            InitiativeMerge.OtherId = res.OtherGuildId;
            InitiativeMerge.DstId = res.DstGuildId;
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMergeTarget);
        }
        /// <summary>
        /// 改变合并的目标家族
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildChangeDstMergeGuildNtf(NetMsg msg)
        {
            CmdGuildChangeDstMergeGuildNtf ntf = NetMsgUtil.Deserialize<CmdGuildChangeDstMergeGuildNtf>(CmdGuildChangeDstMergeGuildNtf.Parser, msg);
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.NeedApplyMergedListReq);
        }
        /// <summary>
        /// 公会技能升级成功通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildSkillUpgradeNtf(NetMsg msg)
        {
            CmdGuildSkillUpgradeNtf ntf = NetMsgUtil.Deserialize<CmdGuildSkillUpgradeNtf>(CmdGuildSkillUpgradeNtf.Parser, msg);
            uint BuildingType = ntf.NewSkill / 10000;
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var AllBuildings = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllBuildings;
            if (AllBuildings.Count <= BuildingType) return;
            var building = AllBuildings[(int)BuildingType];
            building.NowUpgrade = 0;
            building.UpgradeFinishTime = 0;

            if (building.SkillMap.Contains(ntf.OldSkill))
            {
                building.SkillMap.Remove(ntf.OldSkill);
            }
            if (!building.SkillMap.Contains(ntf.NewSkill))
            {
                building.SkillMap.Add(ntf.NewSkill);
            }
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpgradeBuildSkill);
        }
        /// <summary>
        /// 拒绝申请
        /// </summary>
        /// <param name="msg"></param>
        private void OnRefuseApplyNtf(NetMsg msg)
        {
            CmdGuildRefuseApplyNtf ntf = NetMsgUtil.Deserialize<CmdGuildRefuseApplyNtf>(CmdGuildRefuseApplyNtf.Parser, msg);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10766, ntf.GuildName.ToStringUtf8()));
        }
        /// <summary>
        /// 公会id变0
        /// </summary>
        /// <param name="msg"></param>
        private void OnIdNtf(NetMsg msg)
        {
            CmdGuildIdNtf ntf = NetMsgUtil.Deserialize<CmdGuildIdNtf>(CmdGuildIdNtf.Parser, msg);
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildId = 0;
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildUid = 0;
            CheckPlayerIsQuitFamily();
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.QuitFamily);
        }
        /// <summary>
        /// 公会今日捐赠改变
        /// </summary>
        /// <param name="msg"></param>
        private void OnTodayDonateNtf(NetMsg msg)
        {
            CmdGuildTodayDonateNtf ntf = NetMsgUtil.Deserialize<CmdGuildTodayDonateNtf>(CmdGuildTodayDonateNtf.Parser, msg);
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.TodayDonateCoin = ntf.Money;
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyDonateReward, null);
        }
        /// <summary>
        /// 公会邀请
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildInviteNtf(NetMsg msg)
        {
            CmdGuildInviteNtf ntf = NetMsgUtil.Deserialize<CmdGuildInviteNtf>(CmdGuildInviteNtf.Parser, msg);
            if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.RefusalToGuild))
            {
                //设置了不接受邀请
                Sys_Family.Instance.SendGuildInviteRpl(ntf.GuildId, ntf.RoleId, ntf.RoleName.ToStringUtf8(), 2);
            }
            else
            {
                Sys_MessageBag.MessageContent messageConten = new Sys_MessageBag.MessageContent();
                messageConten.mType = EMessageBagType.Family;
                messageConten.cMess = new Sys_MessageBag.GuildMessage();
                messageConten.invitorId = ntf.RoleId;
                messageConten.invitiorName = ntf.RoleName.ToStringUtf8();
                messageConten.cMess.guildId = ntf.GuildId;
                messageConten.cMess.guildName = ntf.GuildName.ToStringUtf8();
                Sys_MessageBag.Instance.SendMessageInfo(EMessageBagType.Family, messageConten);

                /*PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11010, ntf.RoleName.ToStringUtf8(), ntf.GuildName.ToStringUtf8());
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Family.Instance.SendGuildInviteRpl(ntf.GuildId, ntf.RoleId, ntf.RoleName.ToStringUtf8(), 0); //接受
                });
                PromptBoxParameter.Instance.SetCancel(true, () =>
                {
                    Sys_Family.Instance.SendGuildInviteRpl(ntf.GuildId, ntf.RoleId, ntf.RoleName.ToStringUtf8(), 1); //拒绝
                });
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);*/
            }
        }

        /// <summary>
        /// 令牌通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildStaminaNtf(NetMsg msg)
        {
            CmdGuildStaminaNtf ntf = NetMsgUtil.Deserialize<CmdGuildStaminaNtf>(CmdGuildStaminaNtf.Parser, msg);
            uint current = familyData.familyPlayerInfo.staminaNum;

            //familyData.familyPlayerInfo.staminaNum = ntf.Stamina;

            //令牌改变通知
            eventEmitter.Trigger(EEvents.GuildStaminaChange, current);
        }

        /// <summary>
        /// 贡献度添加通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildContributionNtf(NetMsg msg)
        {
            CmdGuildContributionNtf ntf = NetMsgUtil.Deserialize<CmdGuildContributionNtf>(CmdGuildContributionNtf.Parser, msg);
            familyData.familyPlayerInfo.cmdGuildGameInfoNtf.GuildContribution += ntf.AddContribution;
            //货币改变通知
            eventEmitter.Trigger(EEvents.GuildCurrencyChange);
        }

        /// <summary>
        /// 货币变化通知（繁荣度等）
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildCurrencyNtf(NetMsg msg)
        {
            CmdGuildCurrencyNtf ntf = NetMsgUtil.Deserialize<CmdGuildCurrencyNtf>(CmdGuildCurrencyNtf.Parser, msg);
            for (int i = 0; i < ntf.AddItems.Count; i++)
            {
                familyData.SetGuidCurrency(ntf.AddItems[i].ItemId, ntf.AddItems[i].ItemCount);
            }
            //货币改变通知
            eventEmitter.Trigger(EEvents.GuildCurrencyChange);
        }

        /// <summary>
        /// 繁荣度等级变化，等级变化 经验不继承直接清零
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildProsperityLvlNtf(NetMsg msg)
        {
            CmdGuildProsperityLvlNtf ntf = NetMsgUtil.Deserialize<CmdGuildProsperityLvlNtf>(CmdGuildProsperityLvlNtf.Parser, msg);
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.ProsperityLvl = ntf.Lvl;
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AgricultureExp = 0;
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BusinessExp = 0;
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.SecurityExp = 0;
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.ReligionExp = 0;
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.TechnologyExp = 0;
            //繁荣读等级变化改变通知
            eventEmitter.Trigger(EEvents.GuildConstructLvChange);
        }

        /// <summary> 今日菜肴id回调 </summary>
        private void OnGuildGetCuisineInfoAck(NetMsg msg)
        {
            CmdGuildGetCuisineInfoAck ack = NetMsgUtil.Deserialize<CmdGuildGetCuisineInfoAck>(CmdGuildGetCuisineInfoAck.Parser, msg);
            familyData.familyPartyInfo.fashionFoodId = ack.CuisineId;
            familyData.familyPartyInfo.submitTimes = ack.HandinTime;
            familyData.familyPartyInfo.isFoodMatGet = ack.GotCuisineReward;
            familyData.familyPartyInfo.castleLvBeforeParty = ack.Guildlvl;
            //Debug.Log("家族酒会 流行菜品id:" + ack.CuisineId + "|上缴次数:" + ack.HandinTime + "|食材领取状态:" + ack.GotCuisineReward + "|城堡等级:" + ack.Guildlvl);
            eventEmitter.Trigger(EEvents.OnPartyDataUpdate);
        }
        /// <summary> 上交菜肴回调 </summary>
        private void OnGuildHandInCuisineAck(NetMsg msg)
        {
            CmdGuildHandInCuisineAck ack = NetMsgUtil.Deserialize<CmdGuildHandInCuisineAck>(CmdGuildHandInCuisineAck.Parser, msg);
            familyData.familyPartyInfo.lastSubmitFoodItemId = ack.Itemid;
            PopupFamilyPartyStageView(EFamilyPartyPopupType.PartyUpgrade);
            eventEmitter.Trigger(EEvents.OnPartySubmitSucceed);
            GetCuisineInfoReq();
        }
        private void OnGuildPartyStarNtf(NetMsg msg)
        {
            CmdGuildPartyStarNtf ntf = NetMsgUtil.Deserialize<CmdGuildPartyStarNtf>(CmdGuildPartyStarNtf.Parser, msg);
            familyData.familyPartyInfo.partyValue = ntf.Exp;
            eventEmitter.Trigger(EEvents.OnPartyValueNtfUpdate);
        }
        /// <summary> 领取菜肴素材回调 </summary>
        private void OnGetCuisineIngredientAck(NetMsg msg)
        {
            CmdGuildGetCuisineIngredientAck ack = NetMsgUtil.Deserialize<CmdGuildGetCuisineIngredientAck>(CmdGuildGetCuisineIngredientAck.Parser, msg);
            GetCuisineInfoReq();
        }
        /// <summary> 获取酒会记录回调 </summary>
        private void OnGetCuisineRecordAck(NetMsg msg)
        {
            CmdGuildGetCuisineRecordAck ack = NetMsgUtil.Deserialize<CmdGuildGetCuisineRecordAck>(CmdGuildGetCuisineRecordAck.Parser, msg);
            List<CmdGuildGetCuisineRecordAck.Types.Info> datas = new List<CmdGuildGetCuisineRecordAck.Types.Info>();
            datas.AddRange(ack.AllInfos);
            familyData.familyPartyInfo.UpdateRecordList(datas);
            eventEmitter.Trigger(EEvents.OnPartyRecordListUpdate);
        }
        /// <summary> 酒会怪物入侵开启时间节点推送 </summary>
        private void OnGuildPartyMonsterNtf(NetMsg msg)
        {
            CmdGuildPartyMonsterNtf ntf = NetMsgUtil.Deserialize<CmdGuildPartyMonsterNtf>(CmdGuildPartyMonsterNtf.Parser, msg);
            PopupFamilyPartyStageView(EFamilyPartyPopupType.MonsterInvation);
        }

        /// <summary> 在公会名称改变时-成员接收名称变更 </summary>
        private void OnGuildChangeNameNtf(NetMsg msg)
        {
            CmdGuildChangeNameNtf ntf = NetMsgUtil.Deserialize<CmdGuildChangeNameNtf>(CmdGuildChangeNameNtf.Parser, msg);
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GName = ntf.Name;
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateFamilyName);
        }

        /// <summary> 在职位改变-成员接收职位变更 </summary>
        private void OnGuildPosChangeNtf(NetMsg msg)
        {
            CmdGuildPosChangeNtf ntf = NetMsgUtil.Deserialize<CmdGuildPosChangeNtf>(CmdGuildPosChangeNtf.Parser, msg);
            Sys_Family.Instance.familyData.SetMemberStatus(Sys_Role.Instance.RoleId, ntf.Pos);
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMemberStatus);
        }

        /// <summary> 退出家族之后的下次可进入时间通知 </summary>
        private void OnGuildEnterLimitNtf(NetMsg msg)
        {
            CmdGuildEnterLimitNtf ntf = NetMsgUtil.Deserialize<CmdGuildEnterLimitNtf>(CmdGuildEnterLimitNtf.Parser, msg);
            Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildGameInfoNtf.LastOffGuild = ntf.Limit;
        }

        private bool severNtf = false;
        /// <summary> 合并申请通知-空消息 代表需要检查-是否显示红点 </summary>
        private void OnGuildMergeApplyNtf(NetMsg msg)
        {
            severNtf = true;
            SendGuildGetGuildInfoReq();
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMergeStatus);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyMergeApply, null);
        }

        /// <summary> 别人取消了合并申请，通知被操作家族 </summary>
        private void OnGuildCancelMergeNtf(NetMsg msg)
        {
            CmdGuildCancelMergeNtf ntf = NetMsgUtil.Deserialize<CmdGuildCancelMergeNtf>(CmdGuildCancelMergeNtf.Parser, msg);
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            var AllMergeInfos = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos;
            GuildDetailInfo.Types.MergeInfo mergeInfo = null;
            foreach (var info in AllMergeInfos)
            {
                if (info.OtherId == ntf.ApplyGuildId)
                {
                    mergeInfo = info;
                    break;
                }
            }
            if (null != mergeInfo)
                AllMergeInfos.Remove(mergeInfo);
            var list = Sys_Family.Instance.familyData.queryFamilyInfo.mergeList;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].GuildId == ntf.ApplyGuildId)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
            if (AllMergeInfos.Count == 0)
            {
                severNtf = false;
            }
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyMergeApply, null);
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateApplyMergedList);
        }

        private void OnGuildIndustryTaskFinishNtf(NetMsg msg)
        {
            if (((Sys_Team.Instance.HaveTeam && Sys_Team.Instance.isCaptain()) || !Sys_Team.Instance.HaveTeam))
            {
                TryGetConstructTask(0);
            }
        }
        
        public long jointime { get; private set; }
        private void OnDataNty(NetMsg msg) {
            CmdGuildDataNty ntf = NetMsgUtil.Deserialize<CmdGuildDataNty>(CmdGuildDataNty.Parser, msg);
            if (ntf.Member != null) {
                jointime = ntf.Member.Base.Jointime;
            }
        }

        #endregion
        #region 响应事件

        private void OnReceived(int type, uint taskId, TaskEntry taskEntry)
        {
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain())
                return;
            if (type == 18)
            {
                Sys_Task.Instance.TryDoTask(taskId, true, false, true);
            }
        }
        #endregion
        #region 功能函数
        /// <summary>
        /// 打开家族UI
        /// </summary>
        public void OpenUI_Family(UI_FamilyOpenParam openParam = null)
        {
            bool isInFamily = familyData.isInFamily;
            if (isInFamily)
            {
                if(!UIManager.IsOpen(EUIID.UI_Family))
                    UIManager.OpenUI(EUIID.UI_Family, false, openParam);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_ApplyFamily);
            }
        }
        /// <summary>
        /// 跨天更新签到捐献数据
        /// </summary>
        public void OnUpdateGuildSceneInfo()
        {
            uint UpdateTime = familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.LastUpdate;

            uint curTime = Sys_Time.Instance.GetServerTime();
            uint updateDay = (UpdateTime / 86400);
            uint startTime = (updateDay * 24 + 5) * 3600;
            uint endTime = ((updateDay + 1) * 24 + 5) * 3600;

            if (curTime > endTime)
            {
                familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.LastUpdate = curTime;
                familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.BSignIn = false;
                var Donate = familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.Donate;
                for (int i = 0; i < Donate.Count; i++)
                {
                    Donate[i].Count = 0;
                }
                familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.DonateRewardIndex.Clear();
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
                if (null != Sys_Family.Instance.familyData &&
                    null != Sys_Family.Instance.familyData.familyDetailInfo &&
                    null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck &&
                    null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
                {
                    Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.TodayDonateCoin = 0;
                }
            }

        }
        /// <summary>
        /// 发送家族信息
        /// </summary>
        public void SendFamilyInfo()
        {
            if (familyData.isInFamily)
            {
                showAddFamilyTips = true;
                SendGuildGetGuildInfoReq();        //获取家族信息
                SendGuildGetMemberInfoReq();       //获取成员信息
                SendGuildGetGuildApplyMemberReq(); //获取家族申请成员信息
                if (Constants.FamilyMapId != Sys_Map.Instance.CurMapId)
                {
                    GuildPetGetInfoReq();
                }
            }
            else
            {
                SendGuildGetMyApplyListReq(); //获取我申请家族信息
            }
        }
        /// <summary>
        /// 建设总共升级时间
        /// </summary>
        /// <param name="UpgradeTime"></param>
        /// <returns></returns>
        public float GetBuildUpgradeTotalTime(uint UpgradeTime)
        {
            float ratio = Sys_Family.Instance.familyData.familyBuildInfo.reduceStudyTime;
            return UpgradeTime * (1F - ratio);
        }
        /// <summary>
        /// 建筑剩余升级时间
        /// </summary>
        /// <param name="UpgradeTime"></param>
        /// <returns></returns>
        public float GetBuildUpgradeResidueTime(uint UpgradeTime)
        {
            float time = (float)((int)UpgradeTime - (int)Sys_Time.Instance.GetServerTime());
            return time;
        }
        /// <summary>
        /// 是否有红点
        /// </summary>
        /// <returns></returns>
        public bool IsRedPoint()
        {
            if (!Sys_Family.Instance.familyData.isInFamily)
                return false;

            if (IsRedPoint_FamilyView() ||
                IsRedPoint_BankView() || IsRedPoint_Sign() || IsRedPoint_Active() || HasConsignReward() || HasMergeApply())
                return true;
            return false;
        }
        /// <summary>
        /// 家族界面中
        /// </summary>
        /// <returns></returns>
        public bool IsRedPoint_FamilyView()
        {
            if (IsRedPoint_Apply())
                return true;
            return false;
        }
        /// <summary>
        /// 家族银行界面中
        /// </summary>
        /// <returns></returns>
        public bool IsRedPoint_BankView()
        {
            if (IsRedPoint_Donate() ||
                IsRedPoint_DonateAward())
                return true;
            return false;
        }
        /// <summary>
        /// 是否有申请成员红点
        /// </summary>
        /// <returns></returns>
        public bool IsRedPoint_Apply()
        {
            return Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.ApplicationAcceptance) &&
                     Sys_Family.Instance.familyData.GetFamilyApplyMember() > 0;
        }
        /// <summary>
        /// 是否有捐献红点
        /// </summary>
        /// <returns></returns>
        public bool IsRedPoint_Donate()
        {
            if (null != Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.Donate)
            {
                var donates = Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.Donate;

                for (int i = 0; i < donates.Count; i++)
                {
                    var donate = donates[i];
                    if (donate.Count > 0)
                        return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 是否有捐献奖励红点
        /// </summary>
        /// <returns></returns>
        public bool IsRedPoint_DonateAward()
        {
            if (null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info &&
                null != Sys_Family.Instance.familyData.familyBuildInfo.rewardCollectionTrigger)
            {
                uint todayDonateCoin = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.TodayDonateCoin;
                var collectionlist = Sys_Family.Instance.familyData.familyBuildInfo.rewardCollectionTrigger;
                var indexs = Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.DonateRewardIndex;
                var rewardList = Sys_Family.Instance.familyData.familyBuildInfo.dailyReward;
                for (int i = 0; i < collectionlist.Count; i++)
                {
                    uint point = collectionlist[i];
                    if (point <= todayDonateCoin && !indexs.Contains((uint)i) && i < rewardList.Count)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 是否有签到红点
        /// </summary>
        /// <returns></returns>
        public bool IsRedPoint_Sign()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(30406))//签到
                return false;
            return !familyData.familyPlayerInfo.cmdGuildSceneInfoNtf.BSignIn;
        }
        public bool IsRedPoint_Hall()
        {
            return IsRedPoint_Sign() || HasMergeApply();
        }
        /// <summary>
        /// 是否活动显示红点
        /// </summary>
        /// <returns></returns>
        public bool IsRedPoint_Active()
        {
            bool active = HasAuction();
            active |= HasConsignReward();
            return active;
        }

        /// <summary>
        /// 是否有拍卖
        /// </summary>
        /// <returns></returns>
        public bool HasAuction()
        {
            bool active = false;
            if (null != familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
            {
                for (int i = 0; i < familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs.Count; i++)
                {
                    GuildDetailInfo.Types.AuctionBrief tempData = familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs[i];
                    if (null != familyData.familyAuctionInfo.GetActiveDicData(tempData.ActiveId) && !familyData.familyAuctionInfo.GetAuctionIsEnd(tempData.ActiveId))
                    {
                        active = Sys_Family.Instance.needShowAuctionRedPoint;
                    }
                    else if (Sys_Time.Instance.GetServerTime() < tempData.EndTime)
                    {
                        active = Sys_Family.Instance.needShowAuctionRedPoint;
                    }
                }
            }
            return active;
        }

        public bool HasMergeApply()
        {
            bool canEditor = Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.AcceptMerger);

            if (canEditor)
            {
                if ((severNtf || (null != familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info && null != familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos && familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AllMergeInfos.Count > 0)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取行业表数据
        /// </summary>
        /// <param name="type">行业类型</param>
        /// <param name="data">配置表数据</param>
        /// <returns>获取行业表数据经验</returns>
        public uint GetClientDataExp(EConstructs type, CSVFamilyProsperity.Data data)
        {
            if (null != data)
            {
                switch (type)
                {
                    case EConstructs.Agriculture:
                        {
                            return data.agriculture_exp;
                        }
                    case EConstructs.Business:
                        {
                            return data.business_exp;
                        }
                    case EConstructs.Security:
                        {
                            return data.security_exp;
                        }
                    case EConstructs.Religion:
                        {
                            return data.religion_exp;
                        }
                    case EConstructs.Technology:
                        {
                            return data.technology_exp;
                        }
                }
            }
            return 0;
        }

        /// <summary>
        /// 是否显示主界面上的弹窗
        /// 需要判断是否在特定地图以及是否有未使用道具”令牌“
        /// </summary>
        /// <returns></returns>
        public bool IsNeedShowFamilyConstructWindows()
        {
            bool needShow = (Constants.FamilyMapId == Sys_Map.Instance.CurMapId) && Sys_Family.Instance.familyData.GetGuidStamina() > 0;
            return needShow;
        }

        /// <summary>
        /// 获取家族名称
        /// </summary>
        /// <returns></returns>
        public string GetFamilyName()
        {
            if (Sys_Family.Instance.familyData.isInFamily && null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
            {
                return Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GName.ToStringUtf8();
            }
            //可替换
            return LanguageHelper.GetTextContent(540000028);
        }

        public string GetFamilyHeadName()
        {
            if (Sys_Family.Instance.familyData.isInFamily)
            {
                return Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.LeaderName.ToStringUtf8();
            }
            //可替换
            return LanguageHelper.GetTextContent(540000028);
        }

        public ulong GetFamilyId()
        {
            return familyData.GuildId;
        }
        public ulong GetFamilyUId()
        {
            return familyData.GuildUId;
        }
        /// <summary>
        /// 检测菜单栏中的家族酒会面板是否显示
        /// </summary>
        public bool CheckFamilyPartyMenuViewCanShow()
        {
            return CheckFamilyPartyFunctionOpen() && Constants.FamilyCastleMapId == Sys_Map.Instance.CurMapId;
        }
        /// <summary>
        /// 检测家族酒会功能是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckFamilyPartyFunctionOpen()
        {
            return Sys_FunctionOpen.Instance.IsOpen(30502);
        }
        /// <summary>
        /// 计算获取家族酒会贡献
        /// </summary>
        public float GetFamilyPartyValue(uint itemId)
        {
            float value = 0;
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(itemId);
            if (itemData != null)
            {
                if (Sys_Family.Instance.CheckIsFashionFood(itemId))
                {
                    CSVParam.Data rate = CSVParam.Instance.GetConfData(1034);
                    value = itemData.receptionValue * float.Parse(rate.str_value) / 100;
                }
                else
                {
                    value = itemData.receptionValue;
                }
            }
            return value;
        }
        /// <summary>
        /// 获取家族酒会开启时间戳
        /// </summary>
        /// <returns></returns>
        public ulong GetPartyStartTimestamp()
        {
            var zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            var partyStartTime = zeroTime + uint.Parse(CSVParam.Instance.GetConfData(1031).str_value);
            return partyStartTime;
        }
        /// <summary>
        /// 获取家族酒会结束时间戳
        /// </summary>
        /// <returns></returns>
        public ulong GetPartyEndTimestamp()
        {
            var zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            var partyEndTime = zeroTime + uint.Parse(CSVParam.Instance.GetConfData(1037).str_value);
            return partyEndTime;
        }
        /// <summary>
        /// 检测酒会是否进行中
        /// </summary>
        /// <returns></returns>
        public bool CheckIsPartyTime()
        {
            var nowTime = Sys_Time.Instance.GetServerTime();
            var partyStartTime = GetPartyStartTimestamp();
            var partyEndTime = GetPartyEndTimestamp();
            if (nowTime >= partyStartTime && nowTime < partyEndTime)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取当前时间 酒会进行到哪个阶段 -1非酒会时间 0筹备阶段 1阶段 2阶段
        /// </summary>
        /// <returns></returns>
        public int GetFamilyPartyStage()
        {
            var nowTime = Sys_Time.Instance.GetServerTime();
            var partyStartTime = GetPartyStartTimestamp();
            var partyEndTime = GetPartyEndTimestamp();
            var partySecondStageTime = partyStartTime + uint.Parse(CSVParam.Instance.GetConfData(1036).str_value) * 60;
            var zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            if (nowTime >= zeroTime + 5 * 3600 && nowTime < partyStartTime)
            {
                return 0;
            }
            else if (nowTime >= partyStartTime && nowTime < partySecondStageTime)
            {
                return 1;
            }
            else if (nowTime >= partySecondStageTime && nowTime < partyEndTime)
            {
                return 2;
            }
            return -1;
        }
        /// <summary>
        /// 获取下一阶段开始时间戳 酒会开始/酒会2阶段/酒会结束
        /// </summary>
        /// <returns></returns>
        public ulong GetPartyNextStageStartTimeStamp()
        {
            var nowTime = Sys_Time.Instance.GetServerTime();
            var partyStartTime = GetPartyStartTimestamp();
            var zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            var fiveTime = zeroTime + 5 * 3600;
            if (nowTime < fiveTime)
            {
                return fiveTime;
            }
            if (nowTime < partyStartTime)
            {
                return partyStartTime;
            }
            var partySecondStageTime = partyStartTime + uint.Parse(CSVParam.Instance.GetConfData(1036).str_value) * 60;
            if (nowTime < partySecondStageTime)
            {
                return partySecondStageTime;
            }
            var partyEndTime = GetPartyEndTimestamp();
            if (nowTime < partyEndTime)
            {
                return partyEndTime;
            }
            return fiveTime + 86400;
        }
        /// <summary>
        /// 获取酒会价值最大值
        /// </summary>
        /// <returns></returns>
        public uint GetPartyMaxValue()
        {
            uint CastleLevel = 0;
            if (CheckIsPartyTime() && familyData.familyPartyInfo.castleLvBeforeParty > 0)
            {
                CastleLevel = familyData.familyPartyInfo.castleLvBeforeParty;
            }
            else
            {
                CastleLevel = Sys_Family.Instance.familyData.GetBuildLevel(Sys_Family.FamilyData.EBuildingIndex.Castle);
            }
            if (familyData.familyPartyInfo.dictPartyDataByCastleLv.TryGetValue(CastleLevel, out CSVFamilyReception.Data partyData))
            {
                return partyData.receptionValue;
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "找不到家族城堡等级为 " + CastleLevel + " 所对应的酒会价值上限");
            }
            return 100;
        }
        /// <summary>
        /// 获取酒会升到下一星级需要的价值(输入当前价值)
        /// </summary>
        /// <param name="nowValue"></param>
        /// <returns></returns>
        public uint GetPartyStarUpNeedValue(uint value)
        {
            uint needValue = 0;
            var valueStages = familyData.familyPartyInfo.listValueStage;
            for (int i = 0; i < valueStages.Count - 1; i++)
            {
                if (value >= valueStages[i] && value < valueStages[i + 1])
                {
                    needValue = valueStages[i + 1] - value;
                }
            }
            return needValue;
        }
        /// <summary>
        /// 获取酒会星级
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public uint GetPartyStarNumByPartyValue(uint value)
        {
            var valueStages = familyData.familyPartyInfo.listValueStage;
            for (int i = 0; i < valueStages.Count - 1; i++)
            {
                if (value >= valueStages[i] && value < valueStages[i + 1])
                {
                    return (uint)i;
                }
            }
            return (uint)valueStages.Count - 1;
        }
        /// <summary>
        /// 获取家族酒会数据 (星级对应)
        /// </summary>
        /// <param name="starNum"></param>
        /// <returns></returns>
        public CSVFamilyReception.Data GetFamilyPartyDataByStarNum(uint starNum)
        {
            if (familyData.familyPartyInfo.dictPartyDataByStarNum.TryGetValue(starNum, out CSVFamilyReception.Data partyData))
            {
                return partyData;
            }
            return null;
        }
        /// <summary>
        /// 获取酒会享用次数
        /// </summary>
        /// <returns></returns>
        public uint GetPartyTasteNum()
        {
            uint groupId = 1521;
            if (Sys_CollectItem.Instance.collectItemTimes.TryGetValue(groupId, out uint times))
            {
                return times;
            }
            return 0;
        }
        /// <summary>
        /// 获取背包内可以在酒会上缴的菜品
        /// </summary>
        /// <returns></returns>
        public List<ItemData> GetPartyCanSubmitItems()
        {
            List<ItemData> canSubmitFoods = new List<ItemData>();
            canSubmitFoods.AddRange(Sys_Bag.Instance.GetItemDatasByItemType(1911, new List<Func<ItemData, bool>> { CheckFoodItemCanSubmit }));
            canSubmitFoods.AddRange(Sys_Bag.Instance.GetItemDatasByItemType(1912, new List<Func<ItemData, bool>> { CheckFoodItemCanSubmit }));
            canSubmitFoods.AddRange(Sys_Bag.Instance.GetItemDatasByItemType(1913, new List<Func<ItemData, bool>> { CheckFoodItemCanSubmit }));
            canSubmitFoods.AddRange(Sys_Bag.Instance.GetItemDatasByItemType(1914, new List<Func<ItemData, bool>> { CheckFoodItemCanSubmit }));
            List<ItemData> fashionFoods = new List<ItemData>();
            List<ItemData> unFashionFood = new List<ItemData>();
            for (int i = 0; i < canSubmitFoods.Count; i++)
            {
                if (CheckIsFashionFood(canSubmitFoods[i].Id))
                {
                    fashionFoods.Add(canSubmitFoods[i]);
                }
                else
                {
                    unFashionFood.Add(canSubmitFoods[i]);
                }
            }
            canSubmitFoods.Clear();
            canSubmitFoods.AddRange(fashionFoods);
            canSubmitFoods.AddRange(unFashionFood);
            return canSubmitFoods;
        }
        /// <summary>
        /// 检测菜品是否可以在酒会上缴
        /// </summary>
        /// <returns></returns>
        public bool CheckFoodItemCanSubmit(ItemData item)
        {
            if (item != null && item.cSVItemData != null && item.cSVItemData.isSubmit)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 检测是否是流行菜品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckIsFashionFood(uint id)
        {
            CSVCook.Data data = CSVCook.Instance.GetConfData(familyData.familyPartyInfo.fashionFoodId);
            if (data != null)
            {
                for (int i = 0; i < data.result.Count; i++)
                {
                    if (data.result[i] == id)
                    {
                        return true;
                    }
                }

            }
            return false;
        }
        /// <summary>
        /// 获取流行菜品itemId
        /// </summary>
        /// <returns></returns>
        public uint GetFashionFoodId()
        {
            CSVCook.Data data = CSVCook.Instance.GetConfData(familyData.familyPartyInfo.fashionFoodId);
            if (data != null)
            {
                return data.result[data.result.Count - 1];
            }
            return 0;
        }
        /// <summary>
        /// 酒会阶段开启时的活动弹窗
        /// </summary>
        /// <param name="type"></param>
        public void PopupFamilyPartyStageView(EFamilyPartyPopupType type)
        {
            UIScheduler.Push(EUIID.UI_FamilyParty_Popup, (uint)type, null, true, UIScheduler.popTypes[EUIPopType.WhenLastPopedUIClosed]);
        }
        /// <summary>
        /// 获取家族活动列表(加条件过滤)
        /// </summary>
        /// <returns></returns>
        public List<CSVFamilyActive.Data> GetFamilyActiveDataList()
        {
            List<CSVFamilyActive.Data> activeDatas = new List<CSVFamilyActive.Data>();

            var familyActives = CSVFamilyActive.Instance.GetAll();
            for (int i = 0, len = familyActives.Count; i < len; i++)
            {
                CSVFamilyActive.Data activeData = familyActives[i];
                uint key = activeData.id;
                if (key == 10 && !CheckFamilyPartyFunctionOpen())
                {
                    continue;
                }
                if (key == 70 && !Sys_MerchantFleet.Instance.CheckMerchantFleetIsOpen())
                {
                    continue;
                }
                activeDatas.Add(activeData);
            }
            return activeDatas;
        }
        public List<CSVFamilyFight.Data> GetFamilyFightDataList()
        {
            List<CSVFamilyFight.Data> activeDatas = new List<CSVFamilyFight.Data>();

            var familyFights = CSVFamilyFight.Instance.GetAll();
            for (int i = 0, len = familyFights.Count; i < len; i++)
            {
                CSVFamilyFight.Data csv = familyFights[i];
                if (Sys_FunctionOpen.Instance.IsOpen(csv.funcId))
                {
                    activeDatas.Add(csv);
                }
            }
            return activeDatas;
        }
        /// <summary>
        /// 前往酒会(寻路到城堡管家NPC)
        /// </summary>
        public bool GoToFamilyParty()
        {
            //if (CheckIsPartySubmitTime() || CheckIsPartyTime())
            //{
            uint npcInfoId = 1521001;
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcInfoId, true);
            return true;
            //}
            //else
            //{
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(103801));//活动暂未开启
            //    return false;
            //}
        }
        /// <summary>
        /// 检测并上缴酒会菜品(不可提交的情况弹提示)
        /// </summary>
        public void CheckToSubmitPartyFood(ulong selectFoodUId)
        {
            if (CheckIsPartySubmitTime(true))
            {
                List<ItemData> items = new List<ItemData>();
                items.Add(Sys_Bag.Instance.GetItemDataByUuid(selectFoodUId));
                Sys_Family.Instance.HandInCuisineReq(items);
            }
        }
        /// <summary>
        /// 检测是否是酒会上缴时间
        /// </summary>
        /// <param name="showMsg"></param>
        /// <returns></returns>
        public bool CheckIsPartySubmitTime(bool showMsg = false)
        {
            var nowTime = Sys_Time.Instance.GetServerTime();
            var zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            var partyStartTime = GetPartyStartTimestamp();
            var partySecondStageTime = partyStartTime + uint.Parse(CSVParam.Instance.GetConfData(1036).str_value) * 60;
            if (nowTime < zeroTime + 5 * 3600)
            {
                if (showMsg)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6262));//未到上缴时间
                }
                return false;
            }
            else if (nowTime < partySecondStageTime)
            {
                return true;
            }
            else if (nowTime >= partySecondStageTime)
            {
                if (showMsg)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6257));//第一阶段（上缴菜品）已结束，无法提交菜品
                }
                return false;
            }
            return false;
        }

        private void FamilyPartyLogout()
        {
            Sys_Family.Instance.familyData.familyPartyInfo.FamilyPartyOutsideHintIsPopup = false;
            familyData.familyPartyInfo.FamilyPartyGetTeamInsideHintIsPopup = false;
        }
        /// <summary>
        /// 家族酒会活动开始前往提示框(在活动场景外的提示)
        /// </summary>
        public void PopupFamilyPartyOutsideHint()
        {
            if (!Sys_Fight.Instance.IsFight())
            {
                bool isInFamily = Sys_Family.Instance.familyData.isInFamily;
                bool isShow = familyData.familyPartyInfo.FamilyPartyOutsideHintIsPopup;
                if (!isShow && isInFamily && CheckFamilyPartyFunctionOpen() && Constants.FamilyCastleMapId != Sys_Map.Instance.CurMapId && CheckIsPartyTime())
                {
                    familyData.familyPartyInfo.FamilyPartyOutsideHintIsPopup = true;
                    string content = LanguageHelper.GetTextContent(6300);
                    PromptBoxParameter.Instance.OpenActivityPriorityPromptBox(PromptBoxParameter.EPriorityType.FamilyParty, content, () =>
                    {
                        if (!Sys_Fight.Instance.IsFight())
                        {
                            GoToFamilyParty();
                        }
                    });
                }
            }
        }
        /// <summary>
        /// 家族酒会活动进入地图后的组队提示框(在活动场景内提示)
        /// </summary>
        public void PopupFamilyPartyGetTeamInsideHint()
        {
            bool isInFamily = Sys_Family.Instance.familyData.isInFamily;
            bool isShow = familyData.familyPartyInfo.FamilyPartyGetTeamInsideHintIsPopup;
            bool hasTeam = Sys_Team.Instance.HaveTeam;
            if (!isShow && isInFamily && !hasTeam && CheckFamilyPartyFunctionOpen() && Constants.FamilyCastleMapId == Sys_Map.Instance.CurMapId)
            {
                familyData.familyPartyInfo.FamilyPartyGetTeamInsideHintIsPopup = true;
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    UIManager.OpenUI(EUIID.UI_Team_Fast);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                //PromptBoxParameter.Instance.SetToggleChanged(true, (bool value) =>
                //{

                //});
                //PromptBoxParameter.Instance.SetToggleChecked(true);
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(6301);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
            }
        }

        /// <summary>
        /// 先确定有家族再调用 获取家族人数
        /// </summary>
        /// <returns></returns>
        public uint GetGuildMemberNum()
        {
            return (uint)Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member.Count;
        }

        /// <summary>
        /// 是否在退出家族不可加入时间内
        /// </summary>
        /// <returns></returns>
        public bool CanJoinFamily(bool isAgree = false)
        {
            if (Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildGameInfoNtf == null)
                return true;
            if (Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildGameInfoNtf.LastOffGuild > Sys_Time.Instance.GetServerTime() && isAgree)
            {
                var t = Sys_Family.Instance.familyData.familyPlayerInfo.cmdGuildGameInfoNtf.LastOffGuild - Sys_Time.Instance.GetServerTime();
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12101, LanguageHelper.TimeToString((uint)t, LanguageHelper.TimeFormat.Type_4)));
                return false;
            }
            return true;
        }

        //显示推荐建设任务
        private uint recommendConstructId = 0;
        public bool IsShowConstructRedPoint(EConstructs checkType)
        {
            uint level = Sys_Family.Instance.familyData.GetConstructLevel();
            CSVFamilyProsperity.Data familyConstructLevelData = CSVFamilyProsperity.Instance.GetConfData(level);
            if (null != familyConstructLevelData)
            {
                var checkConfigData = Sys_Family.Instance.GetClientDataExp(checkType, familyConstructLevelData);
                var checkExp = Sys_Family.Instance.familyData.GetConstructExp(checkType);
                if (checkExp >= checkConfigData)//满直接不显示
                {
                    return false;
                }

                var values = System.Enum.GetValues(typeof(EConstructs));
                bool hasMax = false;
                uint canRecommendId = (uint)checkType;
                for (int i = 0, count = values.Length; i < count; i++)
                {
                    EConstructs type = (EConstructs)values.GetValue(i);
                    if (type != checkType)
                    {
                        var configData = Sys_Family.Instance.GetClientDataExp(type, familyConstructLevelData);
                        var exp = Sys_Family.Instance.familyData.GetConstructExp(type);
                        if (exp >= configData)
                        {
                            hasMax = true;
                        }
                        if (checkExp > exp)
                        {
                            canRecommendId = (uint)type;
                        }
                    }
                }
                if (recommendConstructId == 0)
                {
                    recommendConstructId = canRecommendId;
                }
                else
                {
                    var saveExp = Sys_Family.Instance.familyData.GetConstructExp((EConstructs)recommendConstructId);
                    if ((uint)checkType == canRecommendId)
                    {
                        bool change = saveExp > checkExp;
                        if (change)
                        {
                            recommendConstructId = (uint)canRecommendId;
                        }
                    }

                }
                return hasMax && canRecommendId == (uint)checkType && recommendConstructId == (uint)checkType;
            }
            return false;
        }

        /// <summary>
        /// 尝试获取建设任务
        /// </summary>
        public void TryGetConstructTask(uint type)
        {
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3290000012u));
                return;
            }

            var staminNum = Sys_Family.Instance.familyData.GetGuidStamina();
            if (Sys_Ini.Instance.Get<IniElement_Int>(1321, out IniElement_Int value))
            {
                if (type == 0)
                {
                    if (staminNum < value.value)
                    {
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(3290000006); //提示令牌不足
                        PromptBoxParameter.Instance.SetConfirm(true, null, 3290000009);
                        PromptBoxParameter.Instance.SetCancel(true, () =>
                        {
                            UIManager.OpenUI(EUIID.UI_Construct_Continue);
                        }, 3290000008);
                        UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    }
                    else
                    {
                        UIManager.OpenUI(EUIID.UI_Construct_Continue);
                    }
                }
                else
                {
                    if (staminNum < value.value)
                    {
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(3290000006); //提示令牌不足
                        PromptBoxParameter.Instance.SetConfirm(true, null, 3290000009);
                        PromptBoxParameter.Instance.SetCancel(true, () =>
                        {
                            GuildAcceptIndustryTaskReq(type);
                        }, 3290000008);
                        UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    }
                    else
                    {
                        GuildAcceptIndustryTaskReq(type);
                    }
                }
            }
        }

        /// <summary>
        /// 检测玩家是否在被请离家族却打开着家族界面
        /// </summary>
        private void CheckPlayerIsQuitFamily()
        {
            if(UIManager.IsOpen(EUIID.UI_Family))
            {
                UIManager.CloseUI(EUIID.UI_Family);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures))
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_Feed))
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures_Feed);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_Get))
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures_Get);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_SetTime))
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures_SetTime);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_Train))
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures_Train);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_Notice))
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures_Notice);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_Popup))
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures_Popup);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_Reward))
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures_Reward);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_Rank))
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures_Rank);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_Rename))
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures_Rename);
            }
            if (UIManager.IsOpen(EUIID.UI_FamilyCreatures_SetTrain))
            {
                UIManager.CloseUI(EUIID.UI_FamilyCreatures_SetTrain);
            }
        }
        #endregion
    }
}