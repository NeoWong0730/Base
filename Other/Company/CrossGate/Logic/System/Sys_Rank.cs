using Logic.Core;
using Lib.Core;
using Packet;
using Net;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public class RankSetting
    {
        private ulong setState;
        private uint time;
        public void SetSettingData(ulong setState,uint time)
        {
            this.setState = setState;
            this.time = time;
        }

        public ulong GetSettingData()
        {
            return setState;
        }

        public uint GetnextSetTime()
        {
            return time;
        }

        /// <summary>
        /// 通过位比较获得状态
        /// </summary>
        /// <param name="subTypeId">子类型id</param>
        /// <returns>true 参选打勾</returns>
        public bool GetSubTypeState(uint subTypeId)
        {
            var ranklistsetDatas = CSVRanklistset.Instance.GetAll();
            for (int i = 0, len = ranklistsetDatas.Count; i < len; i++)
            {
                CSVRanklistset.Data data = ranklistsetDatas[i];
                if (data.Rankid == subTypeId)
                {
                    int bit = (int)data.id - 1;
                    return GetBitvalue(bit);
                }
            }
            return false;
        }

        public bool GetStatebyBit(int bit)
        {
            return GetBitvalue(bit);
        }

        private bool GetBitvalue(int index)
        {
            if ((ulong)index > 64)
                return false;
            ulong value = (setState & (1ul << index));
            return value > 0;
        }

        public void SetBitvalue(uint subTypeId)
        {
            var ranklistsetDatas = CSVRanklistset.Instance.GetAll();
            for (int i = 0, len = ranklistsetDatas.Count; i < len; i++)
            {
                CSVRanklistset.Data data = ranklistsetDatas[i];
                if (data.Rankid == subTypeId)
                {
                    int bit = (int)data.id - 1;
                    if (bit >= 64)
                    {
                        return;
                    }
                    setState = setState | (1ul << bit);
                }
            }
        }

        public void SetBitValueZeo(uint subTypeId)
        {
            var ranklistsetDatas = CSVRanklistset.Instance.GetAll();
            for (int i = 0, len = ranklistsetDatas.Count; i < len; i++)
            {
                CSVRanklistset.Data data = ranklistsetDatas[i];
                if (data.Rankid == subTypeId)
                {
                    int bit = (int)data.id - 1;
                    if (bit >= 64)
                    {
                        return;
                    }
                    setState = setState & (~(1ul << bit));
                }
            }
        }

        public bool IsHasSameValue(ulong setState)
        {
            if (this.setState == setState)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class RankDataBase
    {
        private uint type;
        private uint subType;
        private List<RankUnitData> otherDatas;

        public void SetRankData(CmdRankQueryRes res)
        {
            type = res.Type;
            subType = res.SubType;
            otherDatas = new List<RankUnitData>(res.Units);
            if (otherDatas.Count > 1)
            {
                otherDatas.Sort(Comp);
            }
        }

        public RankUnitData GetSelfData()
        {
            List<RankUnitData> selfDataList = new List<RankUnitData>();
            for (int i = 0; i < otherDatas.Count; i++)
            {
                if (otherDatas[i] != null)
                {
                    switch ((RankType)type)
                    {
                        case RankType.Role:
                            if (otherDatas[i].RoleData != null && otherDatas[i].RoleData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.Career:
                            if (otherDatas[i].CareerData != null && otherDatas[i].CareerData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.Equip:
                            if (otherDatas[i].EquipData != null && otherDatas[i].EquipData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.Attr:
                            if (otherDatas[i].AttrData != null && otherDatas[i].AttrData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.Arena:
                            if (otherDatas[i].ArenaData != null && otherDatas[i].ArenaData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.Growth:
                            if (otherDatas[i].GrowthData != null && otherDatas[i].GrowthData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.Survival:
                            if (otherDatas[i].SurvivalData != null && otherDatas[i].SurvivalData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.RedEnv:
                            if (otherDatas[i].RedEnvData != null && otherDatas[i].RedEnvData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.Leisure:
                            if (otherDatas[i].LeisureData != null && otherDatas[i].LeisureData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.Lovely:
                            if (otherDatas[i].LovelyData != null && otherDatas[i].LovelyData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.Guild:
                            if (otherDatas[i].GuildData != null && otherDatas[i].GuildData.GuildId == Sys_Family.Instance.familyData.GuildId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.Achievement:
                            if (otherDatas[i].AchievementData != null && otherDatas[i].AchievementData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.TrialGate:
                            if (otherDatas[i].TrialGateData != null && otherDatas[i].TrialGateData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.TianTi:
                            if (otherDatas[i].TiantiData != null && otherDatas[i].TiantiData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.BossTower:
                            if (otherDatas[i].BossTowerData != null && otherDatas[i].BossTowerData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                return otherDatas[i];
                            }
                            break;
                        case RankType.Max:
                            return null;
                        default:
                            return null;
                    }
                }
            }
            return null;
        }

        public RankUnitData GetRankDataByIndex(int index)
        {
            if (null == otherDatas)
                return null;

            int count = otherDatas.Count;
            if (index < 0 || index >= count)
                return null;

            return otherDatas[index];
        }

        private int Comp(RankUnitData a, RankUnitData b)
        {
            if (a.Rank > b.Rank)
            {
                return 1;
            }
            else if (a.Rank < b.Rank)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public int GetOtherDataCount()
        {
            if (null != otherDatas)
            {
                return otherDatas.Count;
            }
            else
            {
                return 0;
            }
        }
    }
    /// <summary>
    /// 排行榜打开到指定页签
    /// </summary>
    public class OpenUIRankParam
    {
        public uint initType;
        public uint initSubType;
    }
    public class Sys_Rank : SystemModuleBase<Sys_Rank>
    {
        private bool _sendMessage = true;
        RankSetting rankSet = null;
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        Dictionary<uint, uint> typeCdDics = new Dictionary<uint, uint>();
        Dictionary<uint, RankDataBase> rankDics = new Dictionary<uint, RankDataBase>();
        public uint initType = 0u; // 默认页签id
        public uint InitType
        {
            get
            {
                if (initType == 0)
                {
                    CSVParam.Data item = CSVParam.Instance.GetConfData(905);
                    string[] values = item.str_value.Split('|');
                    if (null != item)
                    {
                        initType = uint.Parse(values[0]);
                    }
                }
                return initType;
            }
            set
            {
                initType = value;
            }
        }
        public uint initTypeSub = 0u;// 默认子页签id
        public uint InitTypeSub
        {
            get
            {
                if (initTypeSub == 0)
                {
                    CSVParam.Data item = CSVParam.Instance.GetConfData(905);
                    string[] values = item.str_value.Split('|');
                    if (null != item)
                    {
                        initTypeSub = uint.Parse(values[1]);
                    }
                }
                return initTypeSub;
            }
            set
            {
                initTypeSub = value;
            }
        }

        public enum EEvents
        {
            GetRankRes, // 显示请求的排行榜数据
            RankQueryRes,
            RankNextTimeReset//重置排行榜下次请求的时间
        }

        public override void Init()
        {
            AddListeners();
        }

        public override void OnLogin()
        {
            rankSet = null;
            SetSendState(true);
            base.OnLogin();
        }

        public override void OnLogout()
        {
            rankSet = null;
            typeCdDics.Clear();
            rankDics.Clear();
        }

        private void AddListeners()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdRank.QueryReq, (ushort)CmdRank.QueryRes, OnRankQueryRes, CmdRankQueryRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdRank.UnitDescReq, (ushort)CmdRank.UnitDescRes, OnRankUnitDescRes, CmdRankUnitDescRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdRank.GetSetStateReq, (ushort)CmdRank.GetSetStateRes, OnRankGetSetStateRes, CmdRankGetSetStateRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdRank.SetStateReq, (ushort)CmdRank.SetStateRes, OnRankSetStateRes, CmdRankSetStateRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdRank.BatUnitDescReq, (ushort)CmdRank.BatUnitDescRes, OnBatUnitDescRes, CmdRankBatUnitDescRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRank.CacheExpireNtf, OnRankCacheExpireNtf, CmdRankCacheExpireNtf.Parser);
            //断线重连上时
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, true);
        }

        private void OnReconnectResult(bool result)
        {
            SetSendState(true);
        }

        /// <summary>
        /// 请求排行榜数据
        /// </summary>
        /// <param name="rankType">排行大类</param>
        /// <param name="subType">排行榜子类</param>
        /// <param name="rankGroupType">子类分组——旅人，勇者，宗师，总榜</param>
        public void RankQueryReq(uint rankType, uint subType, uint rankGroupType = 0)
        {
            if (IsCanRankData(SetType((uint)rankType, subType, rankGroupType)))
            {
                CmdRankQueryReq req = new CmdRankQueryReq();
                req.Type = rankType;
                req.SubType = subType;
                req.GroupType = rankGroupType;
                NetClient.Instance.SendMessage((ushort)CmdRank.QueryReq, req);
            }
            else
            {
                uint key = SetType((uint)rankType, subType, rankGroupType);
                eventEmitter.Trigger(EEvents.GetRankRes, key);
            }
        }

        private void OnRankQueryRes(NetMsg msg)
        {
            CmdRankQueryRes res = NetMsgUtil.Deserialize<CmdRankQueryRes>(CmdRankQueryRes.Parser, msg);

            if (res.Notmain)
            {
                eventEmitter.Trigger<CmdRankQueryRes>(EEvents.RankQueryRes, res);
                return;
            }
            uint rankType = res.Type;
            uint subType = res.SubType;
            uint rankGroupType = res.GroupType;
            List<RankUnitData> rankUnitDatas = new List<RankUnitData>(res.Units);

            RankDataBase rankDataBase = new RankDataBase();
            rankDataBase.SetRankData(res);
            uint key = SetType(rankType, subType, rankGroupType);

            if (rankDics.ContainsKey(key))
            {
                rankDics[key] = rankDataBase;
                typeCdDics[key] = res.NextReqTime;
            }
            else
            {
                rankDics.Add(key, rankDataBase);
                typeCdDics.Add(key, res.NextReqTime);
            }
            eventEmitter.Trigger(EEvents.GetRankRes, key);
        }

        /// <summary>
        /// 查询详细信息
        /// </summary>
        /// <param name="rankType">排行大类</param>
        /// <param name="subType">排行榜子类</param>
        /// <param name="roleId">玩家id</param>
        /// <param name="petUid">宠物uid 当且仅当子类型为宠物</param>
        public void RankUnitDescReq(uint rankType, uint subType, ulong roleId, uint petUid = 0, uint equipId = 0)
        {
            if (_sendMessage)
            {
                CmdRankUnitDescReq req = new CmdRankUnitDescReq();
                req.Type = rankType;
                req.SubType = subType;
                req.RoleId = roleId;
                req.PetUid = petUid;
                req.Itemtid = equipId;
                NetClient.Instance.SendMessage((ushort)CmdRank.UnitDescReq, req);
                SetSendState(false);
            }
        }

        public bool OpenTeamPlayerView(RankDescRole rankDescRole)
        {
            if (rankDescRole.RoleId != Sys_Role.Instance.RoleId)
            {
                CmdSocialGetBriefInfoAck info = new CmdSocialGetBriefInfoAck();
                info.HeroId = rankDescRole.HeroId;
                info.RoleId = rankDescRole.RoleId;
                info.Name = rankDescRole.Name;
                info.Level = rankDescRole.Level;
                info.Occ = rankDescRole.Career;
                info.CareerRank = rankDescRole.CareerRank;
                info.GuildName = rankDescRole.GuildName;
                info.RoleHead = rankDescRole.Photo;
                info.RoleHeadFrame = rankDescRole.PhotoFrame;
                CSVCharacter.Data characterData = CSVCharacter.Instance.GetConfData(rankDescRole.HeroId);
                if (null != characterData)
                {
                    info.HeadIcon = characterData.headid;
                }
                Sys_Role_Info.InfoParmas infoParmas = new Sys_Role_Info.InfoParmas();
                infoParmas.Clear();
                infoParmas.eType = Sys_Role_Info.EType.Chat;
                infoParmas.mInfo = info;
                UIManager.OpenUI(EUIID.UI_Team_Player, false, infoParmas);
                return true;
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(560000002));
                return false;
            }
        }

        private void OnRankUnitDescRes(NetMsg msg)
        {
            CmdRankUnitDescRes res = NetMsgUtil.Deserialize<CmdRankUnitDescRes>(CmdRankUnitDescRes.Parser, msg);
            if (null != res)
            {
                uint rankType = res.Type;
                uint subType = res.SubType;
                if (rankType == (uint)RankType.TrialGate || rankType == (uint)RankType.BossTower)
                {
                    RankDescRole rankDescRole = res.RoleInfo;
                    if (null == rankDescRole || (null != rankDescRole && rankDescRole.RoleId == 0))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(560000001));
                    }
                    else
                    {
                        OpenTeamPlayerView(rankDescRole);
                    }
                }
                else
                {
                    uint id = rankType * 100 + subType;
                    CSVRanklistmain.Data mainData = CSVRanklistmain.Instance.GetConfData(id);
                    if (null != mainData)
                    {
                        if (UIManager.IsOpen(EUIID.UI_Rank))
                        {
                            uint showType = mainData.Descshowtype;
                            if (showType == 3)//交互面板
                            {
                                RankDescRole rankDescRole = res.RoleInfo;
                                if (null == rankDescRole || (null != rankDescRole && rankDescRole.RoleId == 0))
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(560000001));
                                }
                                else
                                {
                                    OpenTeamPlayerView(rankDescRole);
                                }
                            }
                            else if (showType == 5)//宠物面板
                            {
                                RankDescPet rankDescPet = res.PetInfo;
                                if (null == rankDescPet || (null != rankDescPet && null == rankDescPet.Pet)
                                    || (null != rankDescPet && null != rankDescPet.Pet && rankDescPet.Pet.SimpleInfo.PetId == 0))
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(560000001));
                                }
                                else
                                {
                                    ClientPet clientPet = new ClientPet(rankDescPet.Pet, false);
                                    //ItemData item = new ItemData(99, rankDescPet.Pet.Uid, rankDescPet.Pet.SimpleInfo.PetId, 1, 0, false, false, null, null, 0, rankDescPet.Pet)
                                    UIManager.OpenUI(EUIID.UI_Pet_Details, false, clientPet);
                                }
                            }
                            else if (showType == 6)//装备
                            {
                                RankDescEquip rankDescEquip = res.EquipInfo;
                                if (null == rankDescEquip || ((null != rankDescEquip && null == rankDescEquip.Item)
                                    || (null != rankDescEquip && null != rankDescEquip.Item && rankDescEquip.Item.Id == 0)))
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(560000001));
                                }
                                else
                                {
                                    EquipTipsData tipData = new EquipTipsData();
                                    ItemData itemData = new ItemData(99, rankDescEquip.Item.Uuid, rankDescEquip.Item.Id, rankDescEquip.Item.Count, rankDescEquip.Item.Position, false, false, rankDescEquip.Item.Equipment, rankDescEquip.Item.Essence, rankDescEquip.Item.Marketendtime);
                                    tipData.equip = itemData;
                                    tipData.isCompare = true;
                                    tipData.isShowOpBtn = false;
                                    tipData.isShowLock = false;
                                    UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
                                }
                            }
                            else
                            {
                                RankDescRole rankDescRole = res.RoleInfo;
                                if (null == rankDescRole || (null != rankDescRole && rankDescRole.RoleId == 0))
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(560000001));
                                }
                                else
                                {
                                    UI_Rank_DetailParam rankDetail = new UI_Rank_DetailParam();
                                    if (showType == 1)//综合评分
                                    {
                                        rankDetail.showType = ERankDetailType.Player;
                                    }
                                    else if (showType == 2)//多项评分
                                    {
                                        rankDetail.showType = ERankDetailType.Charactor;
                                    }
                                    else if (showType == 4)//PVP面板
                                    {
                                        rankDetail.showType = ERankDetailType.PVP;
                                        rankDetail.rankDescArena = res.ArenaInfo;
                                    }
                                    else if (showType == 7)//天梯面板
                                    {
                                        rankDetail.showType = ERankDetailType.LadderPvp;
                                        rankDetail.randDesTianti = res.TiantiInfo;
                                    }
                                    rankDetail.rankDescRole = res.RoleInfo;
                                    UIManager.OpenUI(EUIID.UI_Rank_Detail01, false, rankDetail);
                                }
                            }
                        }
                    }
                    else
                    {
                        DebugUtil.Log(ELogType.eNone, $"CSVRankMain.Data not find id = {id}");
                    }
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(560000001));
            }
            SetSendState(true);
        }

        /// <summary>
        /// 获取榜单设置状态
        /// </summary>
        public void RankGetSetStateReq()
        {
            if (null == rankSet)
            {
                CmdRankGetSetStateReq req = new CmdRankGetSetStateReq();
                NetClient.Instance.SendMessage((ushort)CmdRank.GetSetStateReq, req);
            }
        }

        private void OnRankGetSetStateRes(NetMsg msg)
        {
            CmdRankGetSetStateRes res = NetMsgUtil.Deserialize<CmdRankGetSetStateRes>(CmdRankGetSetStateRes.Parser, msg);
            SetRankSettingData(res.SetState,res.NextSetTime);
        }

        /// <summary>
        /// 设置榜单上榜状态
        /// </summary>
        /// <param name="equipFlag">装备状态 位变量</param>
        /// <param name="attrFalg">属性状态 位变量</param>
        /// <param name="petFlag">宠物状态布尔值</param>
        public void RankSetStateReq(ulong setState)
        {
            if (null != rankSet && !rankSet.IsHasSameValue(setState))
            {
                CmdRankSetStateReq req = new CmdRankSetStateReq();
                req.SetState = setState;
                NetClient.Instance.SendMessage((ushort)CmdRank.SetStateReq, req);
            }
        }

        private void OnRankSetStateRes(NetMsg msg)
        {
            CmdRankSetStateRes res = NetMsgUtil.Deserialize<CmdRankSetStateRes>(CmdRankSetStateRes.Parser, msg);
            bool success = res.Success; // 设置成功与否 避免服务器设置失败
            if (success)
            {
                SetRankSettingData(res.SetState,res.NextSetTime);
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "RankSet is fail"); 
            }
        }

        /// <summary>
        ///批量获取指定榜指定名次区间的单元详情
        /// </summary>
        /// <param name="type">大类</param>
        /// <param name="subType">子类</param>
        /// <param name="groupType">分组类型</param>
        /// <param name="start">开始名次</param>
        /// <param name="end">结束名次</param>
        /// 
        public void BatUnitDescReq(uint type, uint subType, uint groupType, uint start, uint end)
        {
            CmdRankBatUnitDescReq req = new CmdRankBatUnitDescReq();
            req.Type = type;
            req.SubType = subType;
            req.GroupType = groupType;
            req.Start = start;
            req.End = end;
            NetClient.Instance.SendMessage((ushort)CmdRank.BatUnitDescReq, req);
        }

        private void OnBatUnitDescRes(NetMsg msg)
        {
            CmdRankBatUnitDescRes res = NetMsgUtil.Deserialize<CmdRankBatUnitDescRes>(CmdRankBatUnitDescRes.Parser, msg);
            if (res==null || res.Desc == null || res.Desc.Units == null)
            {
                return;
            }
            Sys_Reputation.Instance.rankDesRoleList.Clear();
            for (int i = 0; i < res.Desc.Units.Count; ++i)
            {
                if (res.Desc.Units[i] != null)
                {
                    Sys_Reputation.Instance.rankDesRoleList.Add(res.Desc.Units[i].RoleInfo);
                }
            }
        }

        private void OnRankCacheExpireNtf(NetMsg msg)
        {
            typeCdDics.Clear();

            eventEmitter.Trigger(EEvents.RankNextTimeReset);
        }

        #region Util
        /// <summary>
        /// 设置状态缓存数据
        /// </summary>
        /// <param name="setState">位变量 子类型-1 为对应的设置标记</param>
        private void SetRankSettingData(ulong setState, uint time)
        {
            if (null != rankSet)
            {
                rankSet.SetSettingData(setState, time);
            }
            else
            {
                rankSet = new RankSetting();
                rankSet.SetSettingData(setState, time);
            }
        }

        public RankSetting GetRankSetting()
        {
            return rankSet;
        }

        /// <summary>
        /// 返回组合type 大类型 子类型 分组
        /// </summary>
        /// <param name="num">组合过的type 数字</param>
        /// <returns>下标 0 1 2 代表 大类型 子类型 分组</returns>
        public uint[] GetTypes(uint num)
        {
            uint[] types = new uint[3];
            if (num > 10000u)
            {
                types[0] = num / 10000u;
                types[1] = num % 10000u / 100u;
                types[2] = num % 10000u % 100u;
            }
            else
            {
                for (int i = 0; i < types.Length - 1; i++)
                {
                    types[i] = 0u;
                }
                types[2] = 4u;
            }
            return types;
        }

        public static uint SetType(uint type, uint subType, uint groupType = 0)
        {
            return type * 10000u + subType * 100u + groupType;
        }

        /// <summary>
        /// 通过子类型表格id 计算 对应的 小类型
        /// </summary>
        /// <param name="configId"></param>
        /// <returns>返回子类型</returns>
        public uint GetSubTypeByTypeSubConfigId(uint configId)
        {
            return 0;
        }

        public bool IsCanRankData(uint timeKey)
        {
            if (typeCdDics.TryGetValue(timeKey, out uint timeS))
            {
                if (timeS <= Sys_Time.Instance.GetServerTime())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取排序后的所有大类型
        /// </summary>
        /// <returns>排序后的大类型的所有数据列表</returns>
        public List<CSVRanklistsort.Data>  GetAllRankType()
        {
            //List<CSVRanklistsort.Data>  tyList = new List<CSVRanklistsort.Data>(CSVRanklistsort.Instance.Count);
            //for (int i = 0; i < CSVRanklistsort.Instance.Count; i++)
            //{
            //    tyList.Add(CSVRanklistsort.Instance[i]);
            //}
            List<CSVRanklistsort.Data>  tyList = new List<CSVRanklistsort.Data>(CSVRanklistsort.Instance.GetAll());
            if (tyList.Count > 1)
            {
                tyList.Sort(TypeComp);
            }
            return tyList;
        }

        public List<CSVRanklistmain.Data>  GetAllRankTypeSubByType(uint typeId)
        {
            List<CSVRanklistmain.Data>  goalDatas = new List<CSVRanklistmain.Data>();

            var ranklistmainDatas = CSVRanklistmain.Instance.GetAll();
            for (int i = 0, len = ranklistmainDatas.Count; i < len; i++)
            {
                CSVRanklistmain.Data tempData = ranklistmainDatas[i];
                if (typeId == tempData.RankType)
                {
                    goalDatas.Add(tempData);
                }
            }
            return goalDatas;
        }

        private int TypeComp(CSVRanklistsort.Data a, CSVRanklistsort.Data b)
        {
            if (a.Sort > b.Sort)
            {
                return 1;
            }
            else if (a.Sort < b.Sort)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取分段数量
        /// </summary>
        /// <returns>返回的值为总榜+具体分布数量 0</returns>
        public int GetRankGroupType()
        {
            CSVParam.Data item = CSVParam.Instance.GetConfData(701);
            if (null != item)
            {
                string[] values = item.str_value.Split('|');
                return values.Length;
            }
            return 0;
        }

        public RankDataBase GetRankDataBaseByKey(uint key)
        {
            if (rankDics == null)
                return null;
            RankDataBase rankDataBase;
            rankDics.TryGetValue(key, out rankDataBase);
            return rankDataBase;
        }

        public uint GetRankIcon(int rankNum)
        {
            CSVParam.Data item = CSVParam.Instance.GetConfData(902u);
            if (null != item)
            {
                int index = rankNum - 1;
                string[] values = item.str_value.Split('|');
                if (index < 0 || index >= values.Length)
                {
                    return 0;
                }
                else
                {
                    return uint.Parse(values[index]);
                }
            }
            return 0;
        }

        private Dictionary<uint, List<uint>> _rankGroup = null;
        public List<uint> rankGroupList = new List<uint>();
        public Dictionary<uint, List<uint>> RankGroup
        {
            get
            {
                if (this._rankGroup == null)
                {
                    this._rankGroup = new Dictionary<uint, List<uint>>();

                    var ranklistmainDatas = CSVRanklistmain.Instance.GetAll();
                    for (int i = 0, len = ranklistmainDatas.Count; i < len; i++)
                    {
                        CSVRanklistmain.Data rankMainData = ranklistmainDatas[i];
                        if (rankMainData.Canset)
                        {
                            if (!this._rankGroup.TryGetValue(rankMainData.RankType, out List<uint> ls))
                            {
                                ls = new List<uint>();
                                this._rankGroup.Add(rankMainData.RankType, ls);
                                this.rankGroupList.Add(rankMainData.RankType);
                            }
                            ls.Add(rankMainData.id);
                        }
                    }
                }
                return this._rankGroup;
            }
        }

        public void SetSendState(bool state)
        {
            _sendMessage = state;
        }

        #endregion
    }
}