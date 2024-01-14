using Framework.Table;
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
    /// <summary>成就完成度分类 </summary>
    public enum EAchievementDegreeType
    {
        All = 0,          //全部
        Finished = 1,     //已完成
        Unfinished = 2,   //未完成
        Append = 3,       //新增
    }
    /// <summary>成就等级奖励领取状态 </summary>
    public enum EAchievementRewardState
    {
        Unreceived = 1,//未领取
        Unfinished = 2,//未完成
        Received = 3,//已领取
    }
    /// <summary>成就分享目标类型 </summary>
    public enum EAchievementShareType
    {
        World=ChatType.World,
        Guid= ChatType.Guild,
        Team= ChatType.Team,
        LookForTeam = ChatType.Local,
        Friend=6,
    }
    public class Sys_Achievement : SystemModuleBase<Sys_Achievement>
    {
        #region 系统函数
        public override void Init()
        {
            isLogin = false;
            ProcessEvents(true);
        }
        public override void OnLogin()
        {
            InitData();
        }
        public override void OnLogout()
        {
            SetDefaultData();
        }
        public override void Dispose()
        {
            ProcessEvents(false);
            isLogin = false;
            ClearData();
        }
        #endregion
        #region 数据定义
        public enum EEvents
        {
            OnRefreshReward,            //刷新领取等级奖励状态
            OnRefreshLevelAndExp,       //刷新成就等级经验
            OnRefreshAchievementData,   //刷新成就数据
            OnAchievementAdd,           //获取新成就
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        /// <summary> 成就大类型列表 </summary>
        public List<AchievementMainClassData> achievementTypeList = new List<AchievementMainClassData>();
        Dictionary<uint, List<SubAchievementTypeData>> achievementsubClassDic = new Dictionary<uint, List<SubAchievementTypeData>>();
        /// <summary> 所有成就数据字典(key1大类型 key2子类型) </summary>
        public Dictionary<uint, Dictionary<uint, List<AchievementDataCell>>> allAchievementDataDic = new Dictionary<uint, Dictionary<uint, List<AchievementDataCell>>>();
        /// <summary> 所有成就数据字典(以成就id为key) </summary>
        Dictionary<uint, AchievementDataCell> allAchievementDataByTidDic = new Dictionary<uint, AchievementDataCell>();
        /// <summary> 成就等级奖励列表 </summary>
        public List<AchievementRewardData> achievementRewardList = new List<AchievementRewardData>();

        /// <summary> 当前成就查询结果大类型列表 </summary>
        public List<CSVAchievementType.Data> curSearchResultTypeList = new List<CSVAchievementType.Data>();

        public List<AchShareData> achShareDataList = new List<AchShareData>();
        /// <summary> 战斗中已达成成就暂存列表 </summary>
        List<uint> achievementTidList = new List<uint>();

        /// <summary> 当前选择要请求的成就id列表 </summary>
        public List<uint> curSelectedReqAchIdList = new List<uint>();

        /// <summary> 当前成就等级 </summary>
        public uint curAchievementLevel { get; private set; }
        /// <summary> 当前成就经验 </summary>
        public uint curAchievementExp { get; private set; }
        public bool isCanShow { get; private set; }
        bool isLogin;
        public bool isShare;

        uint checkAchievementLevelId;
        #endregion
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="v"></param>
        private void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAchievement.DataNty, OnDataNty, CmdAchievementDataNty.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdAchievement.SystemDataReq, (ushort)CmdAchievement.SystemDataRes, OnSystemDataRes, CmdAchievementSystemDataRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdAchievement.ReceiveLevelRewardReq, (ushort)CmdAchievement.ReceiveLevelRewardRes, OnReceiveLevelRewardRes, CmdAchievementReceiveLevelRewardRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAchievement.UpdateBaseNty, OnUpdateBaseNty, CmdAchievementUpdateBaseNty.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAchievement.UpdateAchievementNty, OnUpdateAchievementNty, CmdAchievementUpdateAchievementNty.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAchievement.FinishAchievementNty, OnFinishAchievementNty, CmdAchievementFinishAchievementNty.Parser);
                Sys_Fight.Instance.OnEnterFight += OnEnterBattle;
                Sys_Fight.Instance.OnExitFight += OnExitFight;
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdAchievement.DataNty, OnDataNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdAchievement.SystemDataRes, OnSystemDataRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdAchievement.ReceiveLevelRewardRes, OnReceiveLevelRewardRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdAchievement.UpdateBaseNty, OnUpdateBaseNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdAchievement.UpdateAchievementNty, OnUpdateAchievementNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdAchievement.FinishAchievementNty, OnFinishAchievementNty);
                Sys_Fight.Instance.OnEnterFight -= OnEnterBattle;
                Sys_Fight.Instance.OnExitFight -= OnExitFight;
            }
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, EndExit, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, BeginEnter, toRegister);
            Sys_Chat.Instance.eventEmitter.Handle<ChatType>(Sys_Chat.EEvents.MessageAdd, OnMessageAdd, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, toRegister);
        }
        private void OnRefreshChangeData(int arg1, int arg2)
        {
            if (checkAchievementLevelId != 0)
            {
                ulong uid = GetAchievementLevelTitleUid(checkAchievementLevelId);
                if (uid != 0)
                    Sys_Bag.Instance.UseItemByUuid(uid, 1);
                checkAchievementLevelId = 0;
            }
        }
        private void OnMessageAdd(ChatType type)
        {
            if (isShare)
            {
                isShare = false;
                uint languageId = 0;
                switch (type)
                {
                    case ChatType.World:
                        languageId = 2011212;
                        break;
                    case ChatType.Guild:
                        languageId = 2011214;
                        break;
                    case ChatType.Team:
                        languageId = 5880;
                        break;
                    case ChatType.Local:
                        languageId = 2011213;
                        break;
                    default:
                        break;
                }
                if (languageId != 0)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(languageId));
            }
        }
        private void OnEnterBattle(CSVBattleType.Data obj)
        {
            isCanShow = false;
        }
        private void OnExitFight()
        {
            isCanShow = true;
            ShowAchievementTips();
        }
        private void BeginEnter(uint stack, int id)
        {
            EUIID eId = (EUIID)id;
            if (eId == EUIID.UI_FavorabilityClue || eId == EUIID.UI_FavorabilityMusicList || eId == EUIID.UI_FavorabilityDanceList ||
                eId == EUIID.UI_FavorabilitySendGift || eId == EUIID.UI_FavorabilityThanks || eId == EUIID.UI_Dialogue || eId == EUIID.UI_FavorabilityFete
                || eId == EUIID.UI_CutSceneTop)
            {
                isCanShow = false;
            }
        }
        private void EndExit(uint stack, int id)
        {
            EUIID eId = (EUIID)id;
            if (eId == EUIID.UI_FavorabilityClue || eId == EUIID.UI_FavorabilityMusicList || eId == EUIID.UI_FavorabilityDanceList ||
                eId == EUIID.UI_FavorabilitySendGift || eId == EUIID.UI_FavorabilityThanks || eId == EUIID.UI_Dialogue || eId == EUIID.UI_FavorabilityFete
                || eId == EUIID.UI_CutSceneTop)
            {
                isCanShow = true;
                ShowAchievementTips();
            }
        }
        private void ShowAchievementTips()
        {
            if (isLogin)
            {
                if (achievementTidList.Count > 0)
                {
                    for (int i = 0; i < achievementTidList.Count; i++)
                    {
                        AchievementDataCell achData = GetAchievementByTid(achievementTidList[i]);
                        SetTips(achData);
                    }
                    achievementTidList.Clear();
                }
            }
        }
        #region nty、res
        /// <summary>
        /// 上线成就数据
        /// </summary>
        /// <param name="msg"></param>
        private void OnDataNty(NetMsg msg)
        {
            CmdAchievementDataNty ntf = NetMsgUtil.Deserialize<CmdAchievementDataNty>(CmdAchievementDataNty.Parser, msg);
            if (ntf != null)
            {
                if (ntf.Base != null)
                {
                    curAchievementLevel = ntf.Base.Level;
                    curAchievementExp = ntf.Base.Exp;
                }
                if (ntf.Detail != null && ntf.Detail.Groups != null && ntf.Detail.Groups.Count > 0)
                {
                    for (int i = 0; i < ntf.Detail.Groups.Count; i++)
                    {
                        RoleAchievementGroup group = ntf.Detail.Groups[i];
                        if (group.Achievements != null && group.Achievements.Count > 0)
                        {
                            for (int j = 0; j < group.Achievements.Count; j++)
                            {
                                RoleAchievementUnit roleAchievementUnit = group.Achievements[j];
                                AchievementDataCell achData = GetAchievementByTid(roleAchievementUnit.Tid);
                                if (achData != null)
                                {
                                    achData.timestamp = group.Achievements[j].Timestamp;
                                    achData.isSelfAch = achData.timestamp != 0;
                                    //合服历史
                                    if (roleAchievementUnit.History != null && roleAchievementUnit.History.Count > 0)
                                    {
                                        for (int k = 0; k < roleAchievementUnit.History.Count; k++)
                                        {
                                            AchievementDataCell.RoleAchievementHistory historyData = new AchievementDataCell.RoleAchievementHistory();
                                            historyData.serverName = roleAchievementUnit.History[k].Servername.ToStringUtf8();
                                            historyData.serverNameByte = roleAchievementUnit.History[k].Servername;
                                            historyData.timestamp = roleAchievementUnit.History[k].Timestamp;
                                            achData.achHistoryList.Add(historyData);
                                        }
                                    }
                                    if (group.Value != null)
                                    {
                                        if (group.Value.Number != null)
                                            achData.achievementValue = group.Value.Number.Value;
                                        if (group.Value.Set != null)
                                        {
                                            achData.gatherItemCount = group.Value.Set.Values.Count;
                                            for (int k = 0; k < group.Value.Set.Values.Count; k++)
                                            {
                                                achData.CheckGatherItem(group.Value.Set.Values[k]);
                                            }
                                        }
                                    }
                                    SyncAchievementData(achData);
                                }
                                else
                                    DebugUtil.LogError("CSVAchievement not found id：" + group.Achievements[j].Tid);
                            }
                        }
                    }
                }
                if (ntf.Reward != null && ntf.Reward.Rewards != null && ntf.Reward.Rewards.Count > 0)
                {
                    for (int i = 0; i < ntf.Reward.Rewards.Count; i++)
                    {
                        uint reward = ntf.Reward.Rewards[i];
                        int maxtid = Math.Max(achievementRewardList.Count - i * 32, 0);
                        for (int j = 0; j < Math.Min(32, maxtid); ++j)
                        {
                            if (((1 << j) & reward) != 0)
                            {
                                achievementRewardList[j + i * 32].state = EAchievementRewardState.Received;
                            }
                        }
                    }
                }
                HandleRewardData();
            }
        }
        private void OnSystemDataRes(NetMsg msg)
        {
            CmdAchievementSystemDataRes ntf = NetMsgUtil.Deserialize<CmdAchievementSystemDataRes>(CmdAchievementSystemDataRes.Parser, msg);
            if (ntf.Achievements != null && ntf.Achievements.Count>0)
            {
                for (int i = 0; i < ntf.Achievements.Count; i++)
                {
                    SystemAchievementUnit unitData = ntf.Achievements[i];
                    AchievementDataCell achData = GetAchievementByTid(unitData.Tid);
                    if (achData != null)
                    {
                        if (unitData.Info != null)
                        {
                            if (unitData.Info.Shared != null)
                            {
                                SystemAchievementInfoShared shared = unitData.Info.Shared;
                                if (shared.Role != null)
                                {
                                    achData.roleShared = new AchievementDataCell.RoleShared() { roleId = shared.Role.RoleId, roleName = shared.Role.Name.ToStringUtf8() };
                                }
                                if (shared.Family != null)
                                {
                                    achData.familyShared = new AchievementDataCell.FamilyShared() { familyId = shared.Family.FamilyId, familyName = shared.Family.Name.ToStringUtf8() };
                                }
                                if (shared.Multi != null && shared.Multi.Roles!=null && shared.Multi.Roles.Count>0)
                                {
                                    achData.multiShared.Clear();
                                    for (int j = 0; j < shared.Multi.Roles.Count; j++)
                                    {
                                        AchievementDataCell.RoleShared roleShared = new AchievementDataCell.RoleShared();
                                        roleShared.roleId = shared.Multi.Roles[j].RoleId;
                                        if (shared.Multi.Roles[j].RoleId == Sys_Role.Instance.RoleId)
                                            achData.multiIsIncludSelf = true;
                                        roleShared.roleName = shared.Multi.Roles[j].Name.ToStringUtf8();
                                        achData.multiShared.Add(roleShared);
                                    }
                                }
                                achData.timestamp = shared.Timestamp;
                            }
                            if (unitData.Info.Count != null)
                                achData.ratio = ntf.Achievements[i].Info.Count.Ratio;
                        }

                        SyncAchievementData(achData);
                    }
                    else
                        DebugUtil.LogError("CSVAchievement not found id：" + unitData.Tid);
                }
            }
            eventEmitter.Trigger(EEvents.OnRefreshAchievementData,false);
        }
        /// <summary>
        /// 领取成就奖励通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnReceiveLevelRewardRes(NetMsg msg)
        {
            CmdAchievementReceiveLevelRewardRes ntf = NetMsgUtil.Deserialize<CmdAchievementReceiveLevelRewardRes>(CmdAchievementReceiveLevelRewardRes.Parser, msg);
            HandleRewardData(ntf.Tid);
            eventEmitter.Trigger(EEvents.OnRefreshReward);

            ulong uid = GetAchievementLevelTitleUid(ntf.Tid);
            if (uid != 0)
            {
                checkAchievementLevelId = 0;
                Sys_Bag.Instance.UseItemByUuid(uid, 1);
            }
            else
                checkAchievementLevelId = ntf.Tid;
        }
        /// <summary>
        /// 成就等级经验通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnUpdateBaseNty(NetMsg msg)
        {
            CmdAchievementUpdateBaseNty ntf = NetMsgUtil.Deserialize<CmdAchievementUpdateBaseNty>(CmdAchievementUpdateBaseNty.Parser, msg);
            curAchievementLevel = ntf.Base.Level;
            curAchievementExp = ntf.Base.Exp;
            HandleRewardData();
            eventEmitter.Trigger(EEvents.OnRefreshLevelAndExp);
        }
        /// <summary>
        /// 成就进度通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnUpdateAchievementNty(NetMsg msg)
        {
            CmdAchievementUpdateAchievementNty ntf = NetMsgUtil.Deserialize<CmdAchievementUpdateAchievementNty>(CmdAchievementUpdateAchievementNty.Parser, msg);
            if (ntf.Tids != null && ntf.Tids.Count>0)
            {
                for (int i = 0; i < ntf.Tids.Count; i++)
                {
                    AchievementDataCell achData = GetAchievementByTid(ntf.Tids[i]);
                    if (achData != null)
                    {
                        if (ntf.Value != null)
                        {
                            if (ntf.Value.Number != null)
                                achData.achievementValue = ntf.Value.Number.Value;
                            if (ntf.Value.Set != null)
                            {
                                achData.gatherItemCount = ntf.Value.Set.Values.Count;
                                for (int k = 0; k < ntf.Value.Set.Values.Count; k++)
                                {
                                    achData.CheckGatherItem(ntf.Value.Set.Values[k]);
                                }
                            }
                        }
                        SyncAchievementData(achData);
                    }
                    else
                        DebugUtil.LogError("CSVAchievement not found id：" + ntf.Tids[i]);
                }
            }
            eventEmitter.Trigger(EEvents.OnRefreshAchievementData,false);
        }
        /// <summary>
        /// 成就达成通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnFinishAchievementNty(NetMsg msg)
        {
            CmdAchievementFinishAchievementNty ntf = NetMsgUtil.Deserialize<CmdAchievementFinishAchievementNty>(CmdAchievementFinishAchievementNty.Parser, msg);
            if (ntf != null)
            {
                if (isCanShow)
                {
                    for (int i = 0; i < ntf.Tids.Count; i++)
                    {
                        AchievementDataCell achData = GetAchievementByTid(ntf.Tids[i]);
                        if (achData != null)
                        {
                            achData.timestamp = ntf.Timestamp;
                            achData.isSelfAch = achData.timestamp != 0;
                            SyncAchievementData(achData);
                            SetTips(achData);
                        }
                        else
                            DebugUtil.LogError("CSVAchievement not found id：" + ntf.Tids[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < ntf.Tids.Count; i++)
                    {
                        AchievementDataCell achData = GetAchievementByTid(ntf.Tids[i]);
                        if (achData != null)
                        {
                            achData.timestamp = ntf.Timestamp;
                            achData.isSelfAch = achData.timestamp != 0;
                            SyncAchievementData(achData);
                            achievementTidList.Add(ntf.Tids[i]);
                        }
                        else
                            DebugUtil.LogError("CSVAchievement not found id：" + ntf.Tids[i]);
                    }
                }
                eventEmitter.Trigger(EEvents.OnRefreshAchievementData, true);
                eventEmitter.Trigger(EEvents.OnAchievementAdd);
            }
        }
        private void SetTips(AchievementDataCell achData)
        {
            if (!CheckAchievementIsCanShow()) return;
            if (!achData.CheckIsCanShow()) return;
            if (!OptionManager.Instance.GetBoolean(OptionManager.EOptionID.SettingAchievementNotice))
            {
                if (achData.csvAchievementData.Show_Type == 1)//弹窗提示
                    Sys_CommonTip.Instance.TipKnowledge(Sys_Knowledge.ETypes.Achievement, achData.tid);
            }
            CSVErrorCode.Data code = CSVErrorCode.Instance.GetConfData(770001003);
            string content = string.Format(code.words, achData.tid, LanguageHelper.GetAchievementContent(achData.csvAchievementData.Achievement_Title));
            ErrorCodeHelper.PushErrorCode(code.pos, null, content);
        }
        #endregion
        #region req
        public void OnSystemDataReq()
        {
            CmdAchievementSystemDataReq req = new CmdAchievementSystemDataReq();
            req.Tids.AddRange(curSelectedReqAchIdList);
            NetClient.Instance.SendMessage((ushort)CmdAchievement.SystemDataReq, req);
            curSelectedReqAchIdList.Clear();
        }
        public void OnReceiveLevelRewardReq(uint infoId)
        {
            CmdAchievementReceiveLevelRewardReq req = new CmdAchievementReceiveLevelRewardReq();
            req.Tid = infoId;//等级表id
            NetClient.Instance.SendMessage((ushort)CmdAchievement.ReceiveLevelRewardReq, req);
        }
        #endregion
        private void ClearData()
        {
            achShareDataList.Clear();
            achievementTypeList.Clear();
            allAchievementDataByTidDic.Clear();
            allAchievementDataDic.Clear();
            achievementRewardList.Clear();
            achievementsubClassDic.Clear();
            achievementTidList.Clear();
            curSelectedReqAchIdList.Clear();
        }
        private void SetDefaultData()
        {
            curAchievementLevel = 0;
            curAchievementExp = 0;
            isShare = false;
            if (allAchievementDataByTidDic != null && allAchievementDataByTidDic.Count > 0)
            {
                foreach (var item in allAchievementDataByTidDic.Values)
                {
                    item.ClearData();
                    SyncAchievementData(item);
                }
            }
            if (achievementRewardList != null && achievementRewardList.Count > 0)
            {
                foreach (var item in achievementRewardList)
                {
                    item.state = EAchievementRewardState.Unfinished;
                }
            }
            achievementTidList.Clear();
            curSelectedReqAchIdList.Clear();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            curAchievementLevel = 0;
            curAchievementExp = 0;
            isCanShow = true;
            isShare = false;
            checkAchievementLevelId = 0;
            if (!isLogin)
            {
                isLogin = true;
                achShareDataList.Add(new AchShareData() { shareType = EAchievementShareType.Friend, languageId = 2023227 });
                achShareDataList.Add(new AchShareData() { shareType = EAchievementShareType.LookForTeam, languageId = 2021847 });
                achShareDataList.Add(new AchShareData() { shareType = EAchievementShareType.World, languageId = 2021846 });
                achShareDataList.Add(new AchShareData() { shareType = EAchievementShareType.Team, languageId = 2021849 });
                achShareDataList.Add(new AchShareData() { shareType = EAchievementShareType.Guid, languageId = 2021848 });

                var achievementTypeDatas = CSVAchievementType.Instance.GetAll();
                for (int i = 0; i < achievementTypeDatas.Count; i++)
                {
                    AchievementMainClassData data = new AchievementMainClassData();
                    data.tid = achievementTypeDatas[i].id;
                    data.data = achievementTypeDatas[i];
                    achievementTypeList.Add(data);
                }
                achievementTypeList.Sort((a, b) =>
                {
                    return (int)(a.tid - b.tid);
                });

                var achievementDatas = CSVAchievement.Instance.GetAll();
                for (int i = 0; i < achievementDatas.Count; i++)
                {
                    CSVAchievement.Data csvData = achievementDatas[i];
                    AchievementDataCell achData = new AchievementDataCell();
                    achData.tid = csvData.id;
                    achData.isSelfAch = false;
                    achData.csvAchievementData = csvData;
                    achData.roleShared = null;
                    achData.familyShared = null;
                    achData.achievementValue = 0;
                    achData.timestamp = 0;
                    achData.ratio = 0;
                    achData.multiIsIncludSelf = false;
                    achData.gatherItemCount = 0;
                    if (csvData.Trigger_Type == 3)
                    {
                        for (int j = 0; j < csvData.ReachTypeAchievement.Count; j++)
                        {
                            AchievementDataCell.GatherItem item = new AchievementDataCell.GatherItem();
                            item.id = csvData.ReachTypeAchievement[j];
                            item.typeAchievement = csvData.TypeAchievement;
                            item.isGet = false;
                            if (csvData.CollectInfo != null && csvData.CollectInfo.Count - 1 >= j)
                                item.collectInfoId = csvData.CollectInfo[j];
                            achData.gatherItemList.Add(item);
                        }
                    }
                    achData.SetDropItems();
                    //所有成就字典(成就id为key)
                    allAchievementDataByTidDic[csvData.id] = achData;

                    SubAchievementTypeData subData = new SubAchievementTypeData() { subClass = csvData.SubClass, subTitle = csvData.SubClassType };
                    if (achievementsubClassDic.ContainsKey(csvData.MainClass))
                    {
                        SubAchievementTypeData _subData = achievementsubClassDic[csvData.MainClass][achievementsubClassDic[csvData.MainClass].Count - 1];
                        if (_subData.subClass != subData.subClass)
                            achievementsubClassDic[csvData.MainClass].Add(subData);
                        if (csvData.MainClass == GetServerAchievementMainClassData().tid)
                        {
                            if (_subData.subClass != subData.subClass)
                                achievementsubClassDic[GetSheenAchievementMainClassData().tid].Add(subData);
                        }
                    }
                    else
                    {
                        achievementsubClassDic[csvData.MainClass] = new List<SubAchievementTypeData>() { subData };
                        if (csvData.MainClass == GetServerAchievementMainClassData().tid)
                            achievementsubClassDic[GetSheenAchievementMainClassData().tid] = new List<SubAchievementTypeData>() { subData };
                    }
                    //所有成就字典(key1为大类型 key2为子类型)
                    if (allAchievementDataDic.ContainsKey(csvData.MainClass))
                    {
                        if (allAchievementDataDic[csvData.MainClass].ContainsKey(csvData.SubClass))
                            allAchievementDataDic[csvData.MainClass][csvData.SubClass].Add(achData);
                        else
                            allAchievementDataDic[csvData.MainClass][csvData.SubClass] = new List<AchievementDataCell>() { achData };
                        if (csvData.MainClass == GetServerAchievementMainClassData().tid)
                        {
                            if (allAchievementDataDic[GetSheenAchievementMainClassData().tid].ContainsKey(csvData.SubClass))
                                allAchievementDataDic[GetSheenAchievementMainClassData().tid][csvData.SubClass].Add(achData);
                            else
                                allAchievementDataDic[GetSheenAchievementMainClassData().tid][csvData.SubClass] = new List<AchievementDataCell>() { achData };
                        }
                    }
                    else
                    {
                        allAchievementDataDic[csvData.MainClass] = new Dictionary<uint, List<AchievementDataCell>>
                        {
                            [csvData.SubClass] = new List<AchievementDataCell>() { achData }
                        };
                        if (csvData.MainClass == GetServerAchievementMainClassData().tid)
                        {
                            allAchievementDataDic[GetSheenAchievementMainClassData().tid] = new Dictionary<uint, List<AchievementDataCell>>
                            {
                                [csvData.SubClass] = new List<AchievementDataCell>() { achData }
                            };
                        }
                    }
                }

                var achievementLevelDatas = CSVAchievementLevel.Instance.GetAll();
                for (int i = 0; i < achievementLevelDatas.Count; i++)
                {
                    CSVAchievementLevel.Data csvData = achievementLevelDatas[i];
                    AchievementRewardData data = new AchievementRewardData();
                    data.tid = csvData.id;
                    data.csvAchievementLevelData = csvData;
                    data.state = EAchievementRewardState.Unfinished;
                    achievementRewardList.Add(data);
                }
                achievementRewardList.Sort((a, b) => {
                    return (int)(a.tid - b.tid);
                });
            }
            HandleRewardData();
        }
        #region function
        /// <summary>
        /// 获取成就等级对应称号道具地uid
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        private ulong GetAchievementLevelTitleUid(uint tid)
        {
            CSVAchievementLevel.Data data = CSVAchievementLevel.Instance.GetConfData(tid);
            List<ItemIdCount> dropItems = CSVDrop.Instance.GetDropItem(data.Drop_Id);
            if (dropItems != null && dropItems.Count > 0)
            {
                uint titleId = dropItems[0].id;
                if (titleId != 0)
                {
                    List<ulong> uidList = Sys_Bag.Instance.GetUuidsByItemId(titleId);
                    if (uidList.Count > 0)
                    {
                        return uidList[0];
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取服务器成就MainClass数据
        /// </summary>
        public AchievementMainClassData GetServerAchievementMainClassData()
        {
            return achievementTypeList[achievementTypeList.Count - 1];
        }
        /// <summary>
        /// 获取光辉事迹成就MainClass数据
        /// </summary>
        /// <returns></returns>
        public AchievementMainClassData GetSheenAchievementMainClassData()
        {
            return achievementTypeList[achievementTypeList.Count - 2];
        }
        /// <summary>
        /// 判断自己是否符合光辉事迹成就大类的展示
        /// </summary>
        private bool CheckSelfIsConformSheenAch()
        {
            AchievementDataCell dataCell= GetAchievementData(GetServerAchievementMainClassData().tid).Find(o=>o.CheckIsMerge());
            return dataCell != null;
        }
        /// <summary>
        /// 根据和服条件判断自己是否满足 光辉事迹成就 得到不同展示大类
        /// </summary>
        /// <returns></returns>
        public List<AchievementMainClassData> GetAchievementMainClassData()
        {
            List<AchievementMainClassData> dataList = new List<AchievementMainClassData>();
            if (CheckSelfIsConformSheenAch())
            {
                dataList.AddRange(achievementTypeList);
            }
            else
            {
                for (int i = 0; i < achievementTypeList.Count; i++)
                {
                    if (achievementTypeList[i].tid != GetSheenAchievementMainClassData().tid)
                        dataList.Add(achievementTypeList[i]);
                }
            }
            return dataList;
        }
        /// <summary>
        /// 同步列表字典数据
        /// </summary>
        /// <param name="data"></param>
        private void SyncAchievementData(AchievementDataCell data)
        {
            List<AchievementDataCell> curAchDataList = GetAchievementData(data.csvAchievementData.MainClass, data.csvAchievementData.SubClass);
            for (int i = 0; i < curAchDataList.Count; i++)
            {
                if (curAchDataList[i].tid == data.tid)
                {
                    curAchDataList[i] = data;
                    break;
                }
            }
            if (CheckSelfIsConformSheenAch() && data.csvAchievementData.MainClass == GetServerAchievementMainClassData().tid)
            {
                List<AchievementDataCell> sheenAchievementList = GetAchievementData(GetSheenAchievementMainClassData().tid, data.csvAchievementData.SubClass);
                for (int i = 0; i < sheenAchievementList.Count; i++)
                {
                    if (sheenAchievementList[i].tid == data.tid)
                    {
                        sheenAchievementList[i] = data;
                        break;
                    }
                }
            }
        }
        public bool CheckRewardRedPoint()
        {
            bool isShow = false;
            if (CheckAchievementIsCanShow())
            {
                for (int i = 0; i < achievementRewardList.Count; i++)
                {
                    AchievementRewardData data = achievementRewardList[i];
                    if (data.state == EAchievementRewardState.Unreceived)
                    {
                        isShow = true;
                        break;
                    }
                }
            }
            return isShow;
        }
        /// <summary>
        /// 处理成就等级奖励数据
        /// </summary>
        public void HandleRewardData(uint tid = 0)
        {
            if (tid == 0)
            {
                for (int i = 0; i < achievementRewardList.Count; i++)
                {
                    AchievementRewardData data = achievementRewardList[i];
                    if (data.csvAchievementLevelData.Level <= curAchievementLevel && data.csvAchievementLevelData.Drop_Id != 0 && data.state != EAchievementRewardState.Received)
                    {
                        data.state = EAchievementRewardState.Unreceived;
                    }
                }
            }
            else
            {
                for (int i = 0; i < achievementRewardList.Count; i++)
                {
                    AchievementRewardData data = achievementRewardList[i];
                    if (data.tid == tid)
                    {
                        data.state = EAchievementRewardState.Received;
                    }
                }
            }
        }
        /// <summary>
        /// 获取指定类型成就
        /// </summary>
        /// <param name="mainClass">主类型</param>
        /// <param name="subClass">子类型</param>
        /// <param name="degreeType">完成度类型</param>
        /// <param name="sortType">0(不排序) 1(只时间) 2(只id) 3(order内部顺序) 4(id+时间) 5(order+时间)</param>
        /// <param name="isOnlySelf">是否只显示自己的</param>
        /// <param name="isIncludeSheenAch">是否包含光辉成就</param>
        public List<AchievementDataCell> GetAchievementData(uint mainClass = 0, uint subClass = 0, EAchievementDegreeType degreeType = EAchievementDegreeType.All, int sortType = 0, bool isOnlySelf = false,bool isIncludeSheenAch=false)
        {
            List<AchievementDataCell> curAchievementDataList = new List<AchievementDataCell>();
            Dictionary<uint, List<AchievementDataCell>> curDataDic = null;
            List<AchievementDataCell> curDataList = null;
            if (allAchievementDataDic.ContainsKey(mainClass))
                curDataDic = allAchievementDataDic[mainClass];
            if (curDataDic != null && curDataDic.ContainsKey(subClass))
                curDataList = curDataDic[subClass];

            switch (degreeType)
            {
                case EAchievementDegreeType.All:
                    if (mainClass == 0)
                    {
                        foreach (var item in allAchievementDataByTidDic)
                        {
                            if (item.Value.CheckIsCanShow())
                            {
                                if (isOnlySelf)
                                {
                                    if (item.Value.CheckIsSelf())
                                    {
                                        if (subClass == 0)
                                            curAchievementDataList.Add(item.Value);
                                        else
                                        {
                                            if (item.Value.csvAchievementData.SubClass == subClass)
                                                curAchievementDataList.Add(item.Value);
                                        }
                                    }
                                }
                                else
                                {
                                    if (subClass == 0)
                                        curAchievementDataList.Add(item.Value);
                                    else
                                    {
                                        if (item.Value.csvAchievementData.SubClass == subClass)
                                            curAchievementDataList.Add(item.Value);
                                    }
                                }
                                if (isIncludeSheenAch)
                                {
                                    if (item.Value.CheckIsMerge())
                                    {
                                        if (!curAchievementDataList.Contains(item.Value))
                                            curAchievementDataList.Add(item.Value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (curDataDic != null)
                        {
                            if (subClass == 0)
                            {
                                foreach (List<AchievementDataCell> item in curDataDic.Values)
                                {
                                    foreach (var data in item)
                                    {
                                        if (data.CheckIsCanShow())
                                        {
                                            if (mainClass == GetSheenAchievementMainClassData().tid)
                                            {
                                                if (data.CheckIsMerge())
                                                {
                                                    curAchievementDataList.Add(data);
                                                }
                                            }
                                            else
                                            {
                                                if (isOnlySelf)
                                                {
                                                    if (data.CheckIsSelf())
                                                        curAchievementDataList.Add(data);
                                                }
                                                else
                                                    curAchievementDataList.Add(data);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (curDataList != null)
                                {
                                    for (int i = 0; i < curDataList.Count; i++)
                                    {
                                        if (curDataList[i].CheckIsCanShow())
                                        {
                                            if (mainClass == GetSheenAchievementMainClassData().tid)
                                            {
                                                if (curDataList[i].CheckIsMerge())
                                                {
                                                    curAchievementDataList.Add(curDataList[i]);
                                                }
                                            }
                                            else
                                            {
                                                if (isOnlySelf)
                                                {
                                                    if (curDataList[i].CheckIsSelf())
                                                        curAchievementDataList.Add(curDataList[i]);
                                                }
                                                else
                                                    curAchievementDataList.Add(curDataList[i]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case EAchievementDegreeType.Finished:
                    if (mainClass == 0)
                    {
                        foreach (var item in allAchievementDataByTidDic)
                        {
                            if (item.Value.CheckIsCanShow())
                            {
                                if (isOnlySelf)
                                {
                                    if (item.Value.CheckIsSelf())
                                    {
                                        if (subClass == 0)
                                        {
                                            if (item.Value.timestamp != 0)
                                                curAchievementDataList.Add(item.Value);
                                        }
                                        else
                                        {
                                            if (item.Value.csvAchievementData.SubClass == subClass && item.Value.timestamp != 0)
                                                curAchievementDataList.Add(item.Value);
                                        }
                                    }
                                }
                                else
                                {
                                    if (subClass == 0)
                                    {
                                        if (item.Value.timestamp != 0)
                                            curAchievementDataList.Add(item.Value);
                                    }
                                    else
                                    {
                                        if (item.Value.csvAchievementData.SubClass == subClass && item.Value.timestamp != 0)
                                            curAchievementDataList.Add(item.Value);
                                    }
                                }
                                if (isIncludeSheenAch)
                                {
                                    if (item.Value.CheckIsMerge())
                                    {
                                        if (!curAchievementDataList.Contains(item.Value))
                                            curAchievementDataList.Add(item.Value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (curDataDic != null)
                        {
                            if (subClass == 0)
                            {
                                foreach (List<AchievementDataCell> item in curDataDic.Values)
                                {
                                    for (int i = 0; i < item.Count; i++)
                                    {
                                        if (item[i].CheckIsCanShow())
                                        {
                                            if (mainClass == GetSheenAchievementMainClassData().tid)
                                            {
                                                if (item[i].CheckIsMerge())
                                                {
                                                    curAchievementDataList.Add(item[i]);
                                                }
                                            }
                                            else
                                            {
                                                if (item[i].timestamp != 0)
                                                {
                                                    if (isOnlySelf)
                                                    {
                                                        if (item[i].CheckIsSelf())
                                                            curAchievementDataList.Add(item[i]);
                                                    }
                                                    else
                                                        curAchievementDataList.Add(item[i]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (curDataList != null)
                                {
                                    for (int i = 0; i < curDataList.Count; i++)
                                    {
                                        if (curDataList[i].CheckIsCanShow())
                                        {
                                            if (mainClass == GetSheenAchievementMainClassData().tid)
                                            {
                                                if (curDataList[i].CheckIsMerge())
                                                {
                                                    curAchievementDataList.Add(curDataList[i]);
                                                }
                                            }
                                            else
                                            {
                                                if (curDataList[i].timestamp != 0)
                                                {
                                                    if (isOnlySelf)
                                                    {
                                                        if (curDataList[i].CheckIsSelf())
                                                            curAchievementDataList.Add(curDataList[i]);
                                                    }
                                                    else
                                                        curAchievementDataList.Add(curDataList[i]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case EAchievementDegreeType.Unfinished:
                    if (mainClass == 0)
                    {
                        foreach (var item in allAchievementDataByTidDic)
                        {
                            if (item.Value.CheckIsCanShow())
                            {
                                if (isOnlySelf)
                                {
                                    if (item.Value.CheckIsSelf())
                                    {
                                        if (subClass == 0)
                                        {
                                            if (item.Value.timestamp == 0)
                                                curAchievementDataList.Add(item.Value);
                                        }
                                        else
                                        {
                                            if (item.Value.csvAchievementData.SubClass == subClass && item.Value.timestamp == 0)
                                                curAchievementDataList.Add(item.Value);
                                        }
                                    }
                                }
                                else
                                {
                                    if (subClass == 0)
                                    {
                                        if (item.Value.timestamp == 0)
                                            curAchievementDataList.Add(item.Value);
                                    }
                                    else
                                    {
                                        if (item.Value.csvAchievementData.SubClass == subClass && item.Value.timestamp == 0)
                                            curAchievementDataList.Add(item.Value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (curDataDic != null)
                        {
                            if (subClass == 0)
                            {
                                foreach (List<AchievementDataCell> item in curDataDic.Values)
                                {
                                    for (int i = 0; i < item.Count; i++)
                                    {
                                        if (item[i].CheckIsCanShow())
                                        {
                                            if (mainClass != GetSheenAchievementMainClassData().tid)
                                            {
                                                if (item[i].timestamp == 0)
                                                {
                                                    if (isOnlySelf)
                                                    {
                                                        if (item[i].CheckIsSelf())
                                                            curAchievementDataList.Add(item[i]);
                                                    }
                                                    else
                                                        curAchievementDataList.Add(item[i]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (curDataList != null)
                                {
                                    for (int i = 0; i < curDataList.Count; i++)
                                    {
                                        if (curDataList[i].CheckIsCanShow())
                                        {
                                            if (mainClass != GetSheenAchievementMainClassData().tid)
                                            {
                                                if (curDataList[i].timestamp == 0)
                                                {
                                                    if (isOnlySelf)
                                                    {
                                                        if (curDataList[i].CheckIsSelf())
                                                            curAchievementDataList.Add(curDataList[i]);
                                                    }
                                                    else
                                                        curAchievementDataList.Add(curDataList[i]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case EAchievementDegreeType.Append:
                    if (mainClass == 0)
                    {
                        foreach (var item in allAchievementDataByTidDic)
                        {
                            if (item.Value.CheckIsCanShow())
                            {
                                if (isOnlySelf)
                                {
                                    if (item.Value.CheckIsSelf())
                                    {
                                        if (subClass == 0)
                                        {
                                            if (item.Value.csvAchievementData.Is_New == 1)
                                                curAchievementDataList.Add(item.Value);
                                        }
                                        else
                                        {
                                            if (item.Value.csvAchievementData.SubClass == subClass && item.Value.csvAchievementData.Is_New == 1)
                                                curAchievementDataList.Add(item.Value);
                                        }
                                    }
                                }
                                else
                                {
                                    if (subClass == 0)
                                    {
                                        if (item.Value.csvAchievementData.Is_New == 1)
                                            curAchievementDataList.Add(item.Value);
                                    }
                                    else
                                    {
                                        if (item.Value.csvAchievementData.SubClass == subClass && item.Value.csvAchievementData.Is_New == 1)
                                            curAchievementDataList.Add(item.Value);
                                    }
                                }
                                if (isIncludeSheenAch)
                                {
                                    if (item.Value.CheckIsMerge())
                                    {
                                        if (!curAchievementDataList.Contains(item.Value))
                                            curAchievementDataList.Add(item.Value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (curDataDic != null)
                        {
                            if (subClass == 0)
                            {
                                foreach (List<AchievementDataCell> item in curDataDic.Values)
                                {
                                    for (int i = 0; i < item.Count; i++)
                                    {
                                        if (item[i].CheckIsCanShow() && item[i].csvAchievementData.Is_New == 1)
                                        {
                                            if (mainClass == GetSheenAchievementMainClassData().tid)
                                            {
                                                if (item[i].CheckIsMerge())
                                                {
                                                    curAchievementDataList.Add(item[i]);
                                                }
                                            }
                                            else
                                            {
                                                if (isOnlySelf)
                                                {
                                                    if (item[i].CheckIsSelf())
                                                        curAchievementDataList.Add(item[i]);
                                                }
                                                else
                                                    curAchievementDataList.Add(item[i]);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (curDataList != null)
                                {
                                    for (int i = 0; i < curDataList.Count; i++)
                                    {
                                        if (curDataList[i].CheckIsCanShow() && curDataList[i].csvAchievementData.Is_New == 1)
                                        {
                                            if (mainClass == GetSheenAchievementMainClassData().tid)
                                            {
                                                if (curDataList[i].CheckIsMerge())
                                                {
                                                    curAchievementDataList.Add(curDataList[i]);
                                                }
                                            }
                                            else
                                            {
                                                if (isOnlySelf)
                                                {
                                                    if (curDataList[i].CheckIsSelf())
                                                        curAchievementDataList.Add(curDataList[i]);
                                                }
                                                else
                                                    curAchievementDataList.Add(curDataList[i]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            if (sortType == 1)
            {
                InsertionSort(curAchievementDataList);
            }
            else if (sortType == 2)
            {
                curAchievementDataList.Sort((a, b) =>
                {
                    return (int)(a.csvAchievementData.id - b.csvAchievementData.id);
                });
            }
            else if(sortType == 3)
            {
                curAchievementDataList.Sort((a, b) =>
                {
                    return (int)(a.csvAchievementData.Order - b.csvAchievementData.Order);
                });
            }
            else if (sortType == 4)
            {
                curAchievementDataList.Sort((a, b) =>
                {
                    return (int)(a.csvAchievementData.id - b.csvAchievementData.id);
                });
                InsertionSort(curAchievementDataList);
            }
            else if (sortType == 5)
            {
                curAchievementDataList.Sort((a, b) =>
                {
                    return (int)(a.csvAchievementData.Order - b.csvAchievementData.Order);
                });
                InsertionSort(curAchievementDataList);
            }
            return curAchievementDataList;
        }
        public void InsertionSort(List<AchievementDataCell> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                AchievementDataCell curValue = list[i];
                for (int j = i - 1; j >= 0; j--)
                {
                    if (curValue.timestamp > list[j].timestamp)
                    {
                        list[j + 1] = list[j];
                        list[j] = curValue;
                    }
                    else
                        break;
                }
            }
        }
        int searchCount { get; set; }
        public int GetAchievementBySearchCount()
        {
            return searchCount;
        }
        /// <summary>
        /// 获取成就数据by查询内容
        /// </summary>
        /// <param name="content"></param>
        /// <param name="mainClass"></param>
        /// <param name="degreeType"></param>
        /// <returns></returns>
        public Dictionary<uint, Dictionary<EAchievementDegreeType, List<AchievementDataCell>>> GetAchievementBySearch(string content, uint mainClass,EAchievementDegreeType degreeType = EAchievementDegreeType.All)
        {
            searchCount = 0;
            Dictionary<uint, Dictionary<EAchievementDegreeType, List<AchievementDataCell>>> curSubClassDic = new Dictionary<uint, Dictionary<EAchievementDegreeType, List<AchievementDataCell>>>();
            List<AchievementDataCell> dataList = GetAchievementData(mainClass, 0, degreeType, 4);
            for (int i = 0; i < dataList.Count; i++)
            {
                string achName = LanguageHelper.GetAchievementContent(dataList[i].csvAchievementData.Achievement_Title);
                if (achName.Contains(content))
                {
                    AchievementDataCell data = dataList[i];
                    SetAchievementBySearch(data, curSubClassDic, EAchievementDegreeType.All);
                    if (mainClass == GetSheenAchievementMainClassData().tid)
                    {
                        SetAchievementBySearch(data, curSubClassDic, EAchievementDegreeType.Finished);
                    }
                    else
                    {
                        if (data.timestamp != 0)
                            SetAchievementBySearch(data, curSubClassDic, EAchievementDegreeType.Finished);
                        if (data.timestamp == 0)
                            SetAchievementBySearch(data, curSubClassDic, EAchievementDegreeType.Unfinished);
                    }
                    if (data.csvAchievementData.Is_New == 1)
                        SetAchievementBySearch(data, curSubClassDic, EAchievementDegreeType.Append);
                    searchCount++;
                }
            }
            return curSubClassDic;
        }
        private void SetAchievementBySearch(AchievementDataCell data,Dictionary<uint, Dictionary<EAchievementDegreeType, List<AchievementDataCell>>> dic,EAchievementDegreeType type)
        {
            if (dic.ContainsKey(data.csvAchievementData.SubClass))
            {
                if (dic[data.csvAchievementData.SubClass].ContainsKey(type))
                    dic[data.csvAchievementData.SubClass][type].Add(data);
                else
                    dic[data.csvAchievementData.SubClass][type] = new List<AchievementDataCell>() { data };
            }
            else
            {
                dic[data.csvAchievementData.SubClass] = new Dictionary<EAchievementDegreeType, List<AchievementDataCell>>
                {
                    [type] = new List<AchievementDataCell>() { data }
                };
            }
        }
        /// <summary>
        /// 获取成就数据通过tid
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        public AchievementDataCell GetAchievementByTid(uint tid)
        {
            AchievementDataCell data = null;
            if (allAchievementDataByTidDic.ContainsKey(tid))
            {
                data = allAchievementDataByTidDic[tid];
            }
            return data;
        }
        /// <summary>
        /// 获取成就数量
        /// </summary>
        /// <param name="mainClass"></param>
        /// <param name="subClass"></param>
        /// <param name="degreeType"></param>
        /// <param name="isOnlySelf"></param>
        /// <returns></returns>
        public int GetAchievementCount(uint mainClass = 0, uint subClass = 0, EAchievementDegreeType degreeType = EAchievementDegreeType.All, bool isOnlySelf = false)
        {
            List<AchievementDataCell> dataList = GetAchievementData(mainClass, subClass, degreeType, 0, isOnlySelf);
            return dataList.Count;
        }
        /// <summary>
        /// 通过成就等级获得目标数据
        /// </summary>
        /// <param name="type">0(目标经验) 1(目标索引) 2(目标等级描述)</param>
        /// <param name="isCurLevel">true为当前等级，false为下一等级</param>
        /// <returns></returns>
        public int GetAchievementTargetValueByLevel(int type = 0, bool isCurLevel = true)
        {
            uint maxLv = achievementRewardList[achievementRewardList.Count - 1].csvAchievementLevelData.Level;
            uint level = isCurLevel ? curAchievementLevel : curAchievementLevel >= maxLv ? maxLv : curAchievementLevel + 1;
            int value = 0;
            for (int i = 0; i < achievementRewardList.Count; i++)
            {
                CSVAchievementLevel.Data data = achievementRewardList[i].csvAchievementLevelData;
                if (level == data.Level)
                {
                    if (type == 0)
                    {
                        value = (int)data.Need_Points;
                        break;
                    }
                    else if (type == 1)
                    {
                        value = i;
                        break;
                    }
                    else if (type == 2)
                    {
                        value = (int)data.Level_Test;
                        break;
                    }
                }
            }
            return value;
        }
        /// <summary>
        ///获取成成就星数
        /// </summary>
        /// <param name="mainClass">主类型</param>
        /// <param name="subClass">子类型</param>
        /// <param name="degreeType">进度</param>
        /// <param name="isOnlySelf"></param>
        /// <param name="targetType">1(星数) 2(点数) 3(个数)</param>
        /// <param name="starType">星数类型(0(全部)、1(1星)、2(2星)、3(3星)、4(4星))</param>
        /// <param name="isIncludeSheenAch">是否包含光辉成就</param>
        /// <returns></returns>
        public uint GetAchievementStar(uint mainClass = 0, uint subClass = 0, EAchievementDegreeType degreeType = EAchievementDegreeType.All, bool isOnlySelf = false,int targetType = 1, int starType = 0,bool isIncludeSheenAch=false)
        {
            uint value = 0;
            List<AchievementDataCell> dataList = GetAchievementData(mainClass, subClass, degreeType,0,isOnlySelf, isIncludeSheenAch);
            for (int i = 0; i < dataList.Count; i++)
            {
                if (targetType == 1)
                {
                    if (starType == 0)
                    {
                        value += dataList[i].csvAchievementData.Rare;
                    }
                    else
                    {
                        if (starType == dataList[i].csvAchievementData.Rare)
                        {
                            value += dataList[i].csvAchievementData.Rare;
                        }
                    }
                }
                else if (targetType == 2)
                {
                    if (starType == 0)
                    {
                        value += dataList[i].csvAchievementData.Point;
                    }
                    else
                    {
                        if (starType == dataList[i].csvAchievementData.Rare)
                        {
                            value += dataList[i].csvAchievementData.Point;
                        }
                    }
                }
                else if (targetType == 3)
                {
                    if (starType == 0)
                    {
                        value += 1;
                    }
                    else
                    {
                        if (starType == dataList[i].csvAchievementData.Rare)
                        {
                            value += 1;
                        }
                    }
                }
            }
            return value;
        }
        /// <summary>
        /// 获取成就子类型数据列表
        /// </summary>
        /// <param name="mainClass"></param>
        /// <returns></returns>
        public List<SubAchievementTypeData> GetSubAchievementTypeDataList(uint mainClass)
        {
            List<SubAchievementTypeData> dataList = new List<SubAchievementTypeData>();
            if (achievementsubClassDic.ContainsKey(mainClass))
            {
                List<SubAchievementTypeData> subTypeList = achievementsubClassDic[mainClass];
                if (mainClass == GetSheenAchievementMainClassData().tid)
                {
                    List<AchievementDataCell> achievementDataCellList = GetAchievementData(mainClass);
                    for (int i = 0; i < achievementsubClassDic[mainClass].Count; i++)
                    {
                        if (achievementDataCellList.Find(o => o.csvAchievementData.SubClass == achievementsubClassDic[mainClass][i].subClass) != null)
                            dataList.Add(achievementsubClassDic[mainClass][i]);
                    }
                }
                else
                {
                    for (int i = 0; i < subTypeList.Count; i++)
                    {
                        int dataCount = GetAchievementData(mainClass, subTypeList[i].subClass).Count;
                        if (dataCount > 0)
                        {
                            dataList.Add(subTypeList[i]);
                        }
                    }
                }
            }
            if (dataList != null)
            {
                dataList.Sort((a, b) =>
                {
                    return (int)(a.subClass - b.subClass);
                });
            }
            return dataList;
        }
        /// <summary>
        /// 获取单个成就子类型数据
        /// </summary>
        /// <param name="mainClass"></param>
        /// <param name="subClass"></param>
        /// <returns></returns>
        public SubAchievementTypeData GetSubAchievementTypeData(uint mainClass,uint subClass=0)
        {
            SubAchievementTypeData subData = null;
            if (subClass == 0)
            {
                subData = new SubAchievementTypeData() { subClass = 0, subTitle = 5872 };
            }
            else
            {
                List<SubAchievementTypeData> dataList = GetSubAchievementTypeDataList(mainClass);
                for (int i = 0; i < dataList.Count; i++)
                {
                    if (dataList[i].subClass == subClass)
                    {
                        subData= dataList[i];
                        break;
                    }
                }
            }
            return subData;
        }
        public List<Sys_Society.RoleInfo> GetAllFriends()
        {
            List<Sys_Society.RoleInfo> roleInfos = new List<Sys_Society.RoleInfo>();
            foreach (var friend in Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().Values)
            {
                roleInfos.Add(friend);
            }
            roleInfos.Sort((a, b) =>
            {
                if (!a.isOnLine && b.isOnLine)
                {
                    return 1;
                }
                else
                {
                    if (a.lastChatTime > b.lastChatTime)
                        return 1;
                    else
                        return -1;
                }
            });
            return roleInfos;
        }
        /// <summary>
        /// 请求所有服务器成就
        /// </summary>
        public void ReqAllServerAchievement()
        {
            List<AchievementDataCell> list = GetAchievementData(GetServerAchievementMainClassData().tid, 0, EAchievementDegreeType.All);
            curSelectedReqAchIdList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                curSelectedReqAchIdList.Add(list[i].tid);
            }
            OnSystemDataReq();
        }
        public string GetItemNameByTypeAchievement(AchievementDataCell.GatherItem gatherItem)
        {
            string strName = string.Empty;
            if (gatherItem.collectInfoId != 0)
                strName = LanguageHelper.GetAchievementContent(gatherItem.collectInfoId);
            else
            {
                //伙伴
                if (gatherItem.typeAchievement == 2000)
                {
                    CSVPartner.Data data = CSVPartner.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTextContent(data.name);
                }
                //时装
                else if (gatherItem.typeAchievement == 2001)
                {
                    CSVFashionClothes.Data data_1 = CSVFashionClothes.Instance.GetConfData(gatherItem.id);
                    if (data_1 != null)
                        strName = LanguageHelper.GetTextContent(data_1.FashionName);
                    else
                    {
                        CSVFashionAccessory.Data data_2 = CSVFashionAccessory.Instance.GetConfData(gatherItem.id);
                        if (data_2 != null)
                            strName = LanguageHelper.GetTextContent(data_2.AccName);
                        else
                        {
                            CSVFashionWeapon.Data data_3 = CSVFashionWeapon.Instance.GetConfData(gatherItem.id);
                            if (data_3 != null)
                                strName = LanguageHelper.GetTextContent(data_3.WeaponName);
                        }
                    }
                }
                //宠物
                else if (gatherItem.typeAchievement == 2002)
                {
                    CSVPetNew.Data data = CSVPetNew.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTextContent(data.name);
                }
                //宠物系列
                else if (gatherItem.typeAchievement == 2003)
                {
                    CSVGenus.Data data = CSVGenus.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTextContent(data.rale_name);
                }
                //探索点、npc、采集(语言文本直接是成就表的CollectInfo对应id)
                else if (gatherItem.typeAchievement == 2004 || gatherItem.typeAchievement == 2006 || gatherItem.typeAchievement == 2013)
                {
                    CSVNpc.Data data = CSVNpc.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetNpcTextContent(data.name);
                }
                //天气
                else if (gatherItem.typeAchievement == 2005)
                {
                    CSVWeather.Data data = CSVWeather.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTextContent(data.name);
                }
                else if (gatherItem.typeAchievement == 2007)
                {
                    CSVDetect.Data data = CSVDetect.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                    {
                        CSVNpc.Data data_1 = CSVNpc.Instance.GetConfData(data.Favorabilityid);
                        if (data_1 != null)
                            strName = LanguageHelper.GetNpcTextContent(data_1.name);
                    }
                }
                //地图
                else if (gatherItem.typeAchievement == 2008)
                {
                    CSVMapInfo.Data data = CSVMapInfo.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTextContent(data.name);
                }
                //任务
                else if (gatherItem.typeAchievement == 2009)
                {
                    CSVTask.Data data = CSVTask.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTaskTextContent(data.taskName);
                }
                //职业
                else if (gatherItem.typeAchievement == 2010)
                {
                    CSVCareer.Data data = CSVCareer.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTextContent(data.name);
                }
                //道具
                else if (gatherItem.typeAchievement == 2011)
                {
                    CSVItem.Data data = CSVItem.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTextContent(data.name_id);
                }
                //家族兽
                else if (gatherItem.typeAchievement == 2012)
                {
                    CSVFamilyPet.Data data = CSVFamilyPet.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = data.name;
                }
                //经典头目
                else if (gatherItem.typeAchievement == 2014)
                {
                    CSVClassicBoss.Data data_1 = CSVClassicBoss.Instance.GetConfData(gatherItem.id);
                    if (data_1 != null)
                    {
                        CSVNpc.Data data_2 = CSVNpc.Instance.GetConfData(data_1.NPCID);
                        if (data_2 != null)
                            strName = LanguageHelper.GetNpcTextContent(data_2.name);
                    }
                }
                //地域防范事件
                else if (gatherItem.typeAchievement == 2015)
                {
                    CSVAreaProtection.Data data = CSVAreaProtection.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTextContent(data.eventName_id);
                }
                //Boss图鉴
                else if (gatherItem.typeAchievement == 2016 || gatherItem.typeAchievement == 2022)
                {
                    CSVBOSSManual.Data data = CSVBOSSManual.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTextContent(data.BOSS_name);
                }
                //时空幻境结局
                else if (gatherItem.typeAchievement == 2017)
                {
                    CSVGoddessEnd.Data data = CSVGoddessEnd.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTextContent(data.endingName);
                }
                //魔力纪年
                else if (gatherItem.typeAchievement == 2018)
                {
                    CSVChronology.Data data = CSVChronology.Instance.GetConfData(gatherItem.id);
                    if (data != null)
                        strName = LanguageHelper.GetTextContent(data.event_titel);
                }
                //Boss信息
                else if (gatherItem.typeAchievement == 2020)
                {
                    CSVBOSSInformation.Data data_1 = CSVBOSSInformation.Instance.GetConfData(gatherItem.id);
                    if (data_1 != null)
                    {
                        CSVBOSSManual.Data data_2 = CSVBOSSManual.Instance.GetConfData(data_1.bossManual_id);
                        if (data_2 != null)
                            strName = LanguageHelper.GetTextContent(data_2.BOSS_name);
                    }
                }
                //宠物元核套装外观
                else if (gatherItem.typeAchievement == 2021)
                {
                    CSVPetEquipSuitAppearance.Data data_1 = CSVPetEquipSuitAppearance.Instance.GetConfData(gatherItem.id);
                    if (data_1 != null)
                        strName = LanguageHelper.GetTextContent(data_1.name);
                }
                //线索任务
                else if (gatherItem.typeAchievement == 2023)
                {
                    CSVClueTask.Data data_1 = CSVClueTask.Instance.GetConfData(gatherItem.id);
                    if (data_1 != null)
                        strName = LanguageHelper.GetTextContent(data_1.TaskName);
                }
                //击败xx特性的骷髅王
                else if (gatherItem.typeAchievement == 2024)
                {
                    CSVTrialCharacteristic.Data data_1 = CSVTrialCharacteristic.Instance.GetConfData(gatherItem.id);
                    if (data_1 != null)
                        strName = LanguageHelper.GetTextContent(data_1.characteristic1_name);
                }
                //激活xx特性超级技能
                else if (gatherItem.typeAchievement == 2025)
                {
                    CSVTrialSkillBar.Data data_1 = CSVTrialSkillBar.Instance.GetConfData(gatherItem.id);
                    if (data_1 != null)
                    {
                        CSVPassiveSkillInfo.Data data_2 = CSVPassiveSkillInfo.Instance.GetConfData(data_1.superSkill_id);
                        if (data_2 != null)
                            strName = LanguageHelper.GetTextContent(data_2.name);

                    }
                }
            }
            return strName;
        }
        /// <summary>
        /// 检查成就是否需要显示
        /// </summary>
        /// <returns></returns>
        public bool CheckAchievementIsCanShow()
        {
            uint value = uint.Parse(CSVParam.Instance.GetConfData(1398).str_value);
            return value != 1;
        }
        /// <summary>
        /// 打开成就菜单
        /// </summary>
        /// <param name="achId">成就id,id=0时默认打开第一个成就大类</param>
        public void OpenAchievementMenu(uint achId)
        {
            if (achId != 0)
            {
                AchievementDataCell dataCell = GetAchievementByTid(achId);
                if (dataCell == null)
                {
                    DebugUtil.LogError("CSVAchievement not found id :"+ achId);
                    return;
                }
            }
            OpenAchievementMenuParam param = new OpenAchievementMenuParam();
            if (achId != 0)
                param.tid = achId;
            else
            {
                param.tid = 0;
                param.mainCalss = 1;
                param.subCalss = 101;
            }
            UIManager.OpenUI(EUIID.UI_Achievement);
            UIManager.OpenUI(EUIID.UI_Achievement_Menu, false, param);
        }
        #endregion
    }
    public class AchievementMainClassData
    {
        public uint tid;
        public CSVAchievementType.Data data;
    }
    /// <summary>
    /// 成就数据
    /// </summary>
    public class AchievementDataCell
    {
        public class RoleShared
        {
            public ulong roleId;
            public string roleName;
        }
        public class FamilyShared
        {
            public ulong familyId;
            public string familyName;
        }
        public class GatherItem
        {
            public uint id;
            public uint collectInfoId;
            public uint typeAchievement;
            public bool isGet;
        }
        public class RoleAchievementHistory
        {
            public Google.Protobuf.ByteString serverNameByte;
            public string serverName;
            public uint timestamp;
        }
        public uint tid;
        public bool isSelfAch;
        public CSVAchievement.Data csvAchievementData;
        public RoleShared roleShared;
        public FamilyShared familyShared;
        public List<RoleShared> multiShared = new List<RoleShared>();
        public ulong achievementValue;   //触发类成就当前进度
        public List<GatherItem> gatherItemList = new List<GatherItem>(); //收集类成就当前进度(已收集列表id)
        public uint timestamp;   //达成时间
        public uint ratio;       //达成比率(万分比)
        public bool multiIsIncludSelf;
        public int gatherItemCount; //已收集的道具数量
        public List<ItemIdCount> dropItems=new List<ItemIdCount>();//掉落物
        public List<RoleAchievementHistory> achHistoryList = new List<RoleAchievementHistory>();//合服历史记录

        public void CheckGatherItem(uint id)
        {
            if (gatherItemList.Count > 0)
            {
                for (int i = 0; i < gatherItemList.Count; i++)
                {
                    if (gatherItemList[i].id == id)
                        gatherItemList[i].isGet = true;
                }
            }
        }
        public bool CheckIsCanShow()
        {
            bool isCan = false;
            if (csvAchievementData != null)
            {
                if (csvAchievementData.OpenLimiet == 0)
                {
                    if (csvAchievementData.DateLimiet == 0)
                        isCan = true;
                    else
                    {
                        uint time = Framework.TimeManager.ConvertFromZeroTimeZone(csvAchievementData.DateLimiet);
                        if(Framework.TimeManager.GetServerTime() >= time)
                            isCan = true;
                    }
                }
                else
                {
                    bool isOpen = Sys_FunctionOpen.Instance.IsOpen(csvAchievementData.OpenLimiet);
                    if (csvAchievementData.DateLimiet == 0 && isOpen)
                        isCan = true;
                    else
                    {
                        uint time = Framework.TimeManager.ConvertFromZeroTimeZone(csvAchievementData.DateLimiet);
                        if (Framework.TimeManager.GetServerTime() >= time && isOpen)
                            isCan = true;
                    }
                }
                //if (csvAchievementData.OpenLimiet == 0 || (csvAchievementData.OpenLimiet != 0 && Sys_FunctionOpen.Instance.IsOpen(csvAchievementData.OpenLimiet)))
                //    isCan = true;
            }
            return isCan;
        }
        public bool CheckIsSelf()
        {
            if (csvAchievementData.MainClass == Sys_Achievement.Instance.GetServerAchievementMainClassData().tid)
                return isSelfAch;
            return true;
        }
        ////判断该成就是否是自己达成的服务器成就(光辉成就)
        //public bool CheckIsOnlySelfFinish()
        //{
        //    return CheckIsCanShow() && CheckIsSelf() && timestamp != 0;
        //}
        public void SetDropItems()
        {
            dropItems.Clear();
            if (csvAchievementData != null)
            {
                CSVAchievementDrop.Data dropData = CSVAchievementDrop.Instance.GetConfData(csvAchievementData.Rare);
                if (dropData != null)
                {
                    bool isAdd = true;
                    if (dropData.BlackList != null && dropData.BlackList.Count > 0)
                    {
                        for (int i = 0; i < dropData.BlackList.Count; i++)
                        {
                            if (csvAchievementData.MainClass == dropData.BlackList[i])
                            {
                                isAdd = false;
                                break;
                            }
                        }
                    }
                    if (isAdd)
                    {
                        List<ItemIdCount> baseItems = CSVDrop.Instance.GetDropItem(dropData.Drop_Id);
                        if (baseItems != null && baseItems.Count > 0)
                        {
                            dropItems.AddRange(baseItems);
                        }
                    }
                }
                if (csvAchievementData.Drop_Id != 0)
                {
                    List<ItemIdCount> diffItems = CSVDrop.Instance.GetDropItem(csvAchievementData.Drop_Id);
                    if (diffItems != null && diffItems.Count > 0)
                    {
                        dropItems.AddRange(diffItems);
                    }
                }
            }
        }
        /// <summary>
        /// 检查是否合并过
        /// </summary>
        /// <returns></returns>
        public bool CheckIsMerge()
        {
            return achHistoryList != null && achHistoryList.Count > 0;
        }
        public void ClearData()
        {
            isSelfAch = false;
            roleShared = null;
            familyShared = null;
            multiShared.Clear();
            if (gatherItemList.Count > 0)
            {
                for (int i = 0; i < gatherItemList.Count; i++)
                {
                    gatherItemList[i].isGet = false;
                }
            }
            achievementValue = 0;
            timestamp = 0;
            ratio = 0;
            multiIsIncludSelf = false;
            gatherItemCount = 0;
            achHistoryList.Clear();
        }
    }
    /// <summary>
    /// 成就子类型数据
    /// </summary>
    public class SubAchievementTypeData
    {
        public uint subClass;
        public uint subTitle;
    }
    /// <summary>
    /// 成就等级奖励预览数据
    /// </summary>
    public class AchievementRewardData
    {
        public uint tid;
        public CSVAchievementLevel.Data csvAchievementLevelData;
        public EAchievementRewardState state;
    }
    /// <summary>
    /// 成就分享可选类型数据
    /// </summary>
    public class AchShareData
    {
        public EAchievementShareType shareType;
        public uint languageId;
    }
    /// <summary>
    /// 打开成就菜单面板所需参数
    /// </summary>
    public class OpenAchievementMenuParam
    {
        public uint mainCalss;
        public uint subCalss;
        public uint tid;
        public EAchievementDegreeType degreeType;
    }
    public class ClickShowPositionData
    {
        public AchievementDataCell data;
        public RectTransform clickTarget;
        public RectTransform parent;
    }
    public class AchievementRewardItemCell
    {
        PropItem propItem;
        public void Init(Transform tran)
        {
            propItem = new PropItem();
            propItem.BindGameObject(tran.gameObject);
        }
        public void SetData(uint itemId, int itemCount)
        {
            PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                          (_id: itemId,
                          _count: itemCount,
                          _bUseQuailty: true,
                          _bBind: false,
                          _bNew: false,
                          _bUnLock: false,
                          _bSelected: false,
                          _bShowCount: true,
                          _bShowBagCount: false,
                          _bUseClick: true,
                          _onClick: null,
                          _bshowBtnNo: false);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_Achievement_Reward, showItem));
        }
    }
}