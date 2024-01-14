using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        public enum EEvents
        {
            RewardAck,
            ActiveValueChange,
            NewLimiteDaily,//新的限时日常开启
            RemoveLimiteDaily,
            NewNotice,
            RemoveNotice,
            NewTipsChange,
            DailyReward,//每个活动奖励的变化

        }
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityVal.GetRewardAck, Notify_RewardAck, CmdActivityValGetRewardAck.Parser);//领奖
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityVal.InfoAck, Notify_InfoAck, CmdActivityValInfoAck.Parser);//信息
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityVal.ChangeNtf, Notify_ChangeNtf, CmdActivityValChangeNtf.Parser);//活跃度改变

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityVal.DailyNtf, Notify_DailyNtf, CmdActivityValDailyNtf.Parser);//活跃度改变
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityVal.DailyUpdateNtf, Notify_DailyUpdateNtf, CmdActivityValDailyUpdateNtf.Parser);//活跃度改变

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityVal.MonsterKillNtf, Notify_DailyMonsterKill, CmdActivityValMonsterKillNtf.Parser);//怪物击杀数量改变

            
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTask.CmdBarAllEventNtf, Notify_BarAllEventNtf, CmdBarAllEventNtf.Parser);//活跃度改变

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityVal.ClientClearNtf, Notify_ClearNtf, CmdActivityValClientClearNtf.Parser);//活跃度改变


            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, Notify_FuncOpen, true);
            Sys_FunctionOpen.Instance.eventEmitter.Handle(Sys_FunctionOpen.EEvents.UnLockAllFuntion, Notify_AllFuncOpen, true);

            Sys_FunctionOpen.Instance.eventEmitter.Handle(Sys_FunctionOpen.EEvents.InitFinish, Notify_FuncOpenInit, true);

            ParseConfig();

            ReadKilledMonster();
        }

        public override void OnLogin()
        {
            UpdataTodayTime();

            Apply_InfoReq();

            LoadNewTipsDB();

            ReGetAllWork();    
        }

        public override void OnLogout()
        {
            StopAutoUpdataTime();
            ClearWorkList();

            SaveNewTipsDB();
        }

        public override void Dispose()
        {
            DisposeConfig();
        }
    }


    #region local data
    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {

        private CmdActivityValInfoAck mData = new CmdActivityValInfoAck();

        /// <summary>
        /// 获得限时活动 列表
        /// </summary>
        public IList<uint> Recommend { get { return mData.RecommendActivity; } }

        /// <summary>
        /// 已领取奖励
        /// </summary>
        public IList<uint> Reward { get { return mData.GotReward; } }


        private DateTime m_DateTime = DateTime.MaxValue;
        /// <summary>
        /// 服务器数据刷新时间
        /// </summary>
        /// 
        public DateTime dateTime { get { return m_DateTime; } }

        /// <summary>
        /// 一键清除限时活动的时间戳
        /// </summary>

        public CmdActivityValClientClearNtf ClearLimitData { get; private set; } = null;
        /// <summary>
        /// 所有活动活跃度
        /// </summary>
        public IList<CmdActivityValInfoAck.Types.ActivityItem> Activity { get { return mData.ValItem; } }


        private Dictionary<uint, uint> mDicDailyActivty = new Dictionary<uint, uint>();


      
        //private bool isFristGetClearMsg = true;

        public bool HadGetReward(uint id)
        {
            return mData.GotReward.Contains(id);
        }
        /// <summary>
        /// 总共活跃度，将Activity所有项相加
        /// </summary>
        /// 
        public uint m_totalActivity = 0;
        public uint TotalActivity
        {
            get { return m_totalActivity; }
            private set
            {
                if (m_totalActivity == value)
                    return;
                m_totalActivity = value;

                eventEmitter.Trigger(EEvents.ActiveValueChange);
            }
        }

        /// <summary>
        /// 获得活跃度，根据日常ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint getDailyAcitvity(uint id)
        {
            uint value = 0;

            mDicDailyActivty.TryGetValue(id, out value);

            return value;
        }

        /// <summary>
        /// 获取当前活动的次数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint getDailyCurTimes(uint id)
        {
            return GetNowDailyUsedTimes(id);
        }


        public uint getDailyTotalTimes(uint id)
        {
            return GetDailyTotalTimes(id);
        }
        /// <summary>
        /// 更新修改的数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        private void ValChange(uint id, uint value)
        {
            bool result = false;

            foreach (var item in mData.ValItem)
            {
                if (item.Id == id)
                {
                    item.Id = value;

                    result = true;

                    mDicDailyActivty[id] = value;

                    break;
                }
            }

            if (!result)
            {
                mData.ValItem.Add(new CmdActivityValInfoAck.Types.ActivityItem() { Id = id, Val = value });

                if (mDicDailyActivty.ContainsKey(id))
                    mDicDailyActivty[id] = value;
                else
                    mDicDailyActivty.Add(id, value);
            }

            CalulateTotalActivity();
        }

        private void CalulateTotalActivity()
        {
            uint totalValue = 0;
            foreach (var item in mDicDailyActivty)
            {
                totalValue += item.Value;
            }

            TotalActivity = totalValue;
        }
        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="data"></param>
        private void SetValInfoData(CmdActivityValInfoAck data)
        {
            mData = data;

            mDicDailyActivty.Clear();

            uint totalValue = 0;
            foreach (var item in Activity)
            {
                mDicDailyActivty.Add(item.Id, item.Val);

                totalValue += item.Val;
            }

            TotalActivity = totalValue;

            SetRefreshServerTime(data.RefreshTime);
        }

        private void SetRefreshServerTime(uint time)
        {
           // var startTime = Sys_Time.ConvertToLocalTime(time);

            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            m_DateTime = startTime.AddSeconds(time);

            SetNextRefreshTime(time);
        }

        /// <summary>
        /// 是否为推荐活动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public bool IsRecommendDaily(uint id)
        {
            return mData.RecommendActivity.Contains(id);
        }

    }
    #endregion


    #region pub 酒吧事件
    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        private CmdBarAllEventNtf mBarCmd = new CmdBarAllEventNtf();
        private void Notify_BarAllEventNtf(NetMsg msg)
        {
            CmdBarAllEventNtf info = NetMsgUtil.Deserialize<CmdBarAllEventNtf>(CmdBarAllEventNtf.Parser, msg);

            mBarCmd = info;
        }

        public uint GetPubTimes()
        {
            if (mBarCmd.ExpireDay <= Sys_Time.Instance.GetServerTime())
            {
                mBarCmd.UsedTimes = 0;
                mBarCmd.HasFreshed = false;
            }

            return mBarCmd.UsedTimes;
        }
    }

    #endregion
    #region daily common moudel

    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        private Dictionary<uint, DailyActivity> mDicActivityCom = new Dictionary<uint, DailyActivity>();


        private void AddDailyActivityComData(IList<DailyActivity> dailyActivity)
        {
            foreach (var item in dailyActivity)
            {
                SetDailComdata(item);
            }
        }

        private void SetDailComdata(DailyActivity item)
        {
            if (mDicActivityCom.ContainsKey(item.PlayType) == false)
                mDicActivityCom.Add(item.PlayType, item);
            else
                mDicActivityCom[item.PlayType] = item;
        }

        /// <summary>
        /// 当前活动次数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint GetNowDailyUsedTimes(uint id)
        {
            //DailyActivity item = null;
            //mDicActivityCom.TryGetValue(id, out item);

            return GetDailyTimes(id);
        }

        /// <summary>
        /// 活动最大次数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint GetDailyMaxTimes(uint id)
        {
            var item = CSVDailyActivity.Instance.GetConfData(id);

            return item == null ? 0 : item.limite;
        }

        /// <summary>
        /// 获取日常参数 服务器数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DailyActivity GetDailyActivityCom(uint id)
        {
            DailyActivity item = null;
            mDicActivityCom.TryGetValue(id, out item);

            return item;
        }
    }
    #endregion

    #region from server
    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        private void Notify_RewardAck(NetMsg msg)
        {
            CmdActivityValGetRewardAck info = NetMsgUtil.Deserialize<CmdActivityValGetRewardAck>(CmdActivityValGetRewardAck.Parser, msg);

            if (mData.GotReward.Contains(info.Id) == false)
            {
                mData.GotReward.Add(info.Id);

                eventEmitter.Trigger<uint>(EEvents.RewardAck, info.Id);
            }


        }

        private void Notify_InfoAck(NetMsg msg)
        {
            CmdActivityValInfoAck info = NetMsgUtil.Deserialize<CmdActivityValInfoAck>(CmdActivityValInfoAck.Parser, msg);

            SetValInfoData(info);

            //UpdataTodayTime();
        }

        private void Notify_ChangeNtf(NetMsg msg)
        {
            CmdActivityValChangeNtf info = NetMsgUtil.Deserialize<CmdActivityValChangeNtf>(CmdActivityValChangeNtf.Parser, msg);

            ValChange(info.Id, info.Val);
        }

        private void Notify_DailyNtf(NetMsg msg)
        {
            CmdActivityValDailyNtf info = NetMsgUtil.Deserialize<CmdActivityValDailyNtf>(CmdActivityValDailyNtf.Parser, msg);

            AddDailyActivityComData(info.Activities);
        }

        private void Notify_DailyUpdateNtf(NetMsg msg)
        {
            CmdActivityValDailyUpdateNtf info = NetMsgUtil.Deserialize<CmdActivityValDailyUpdateNtf>(CmdActivityValDailyUpdateNtf.Parser, msg);

            SetDailComdata(info.Activity);
        }


        private void Notify_DailyMonsterKill(NetMsg msg)
        {
            CmdActivityValMonsterKillNtf info = NetMsgUtil.Deserialize<CmdActivityValMonsterKillNtf>(CmdActivityValMonsterKillNtf.Parser, msg);

            KilledMonsterCount = info.Count;

        }

        private void Notify_ClearNtf(NetMsg msg)
        {
            CmdActivityValClientClearNtf info = NetMsgUtil.Deserialize<CmdActivityValClientClearNtf>(CmdActivityValClientClearNtf.Parser, msg);

            ClearLimitData = info;

            m_ClearTime = info.ClearTime;

            ClearNotice();

        }
        private void Notify_AllFuncOpen()
        {
            ReGetAllWork();
        }
        private void Notify_FuncOpen(Sys_FunctionOpen.FunctionOpenData data)
        {
            if (data == null)
                return;

            if (data.id == DailyFuncOpenID)
            {
                ReGetAllWork();
                return;
            }

            if( m_FuncOpen.ContainsKey(data.id) == false)
                return;

            var dailyID = m_FuncOpen[data.id];

            GetWorkDaily(dailyID);

            SetNewTipsNewFunc(dailyID, true);
        }

        private void Notify_FuncOpenInit()
        {
            ReGetAllWork();
        }

    }

    #endregion

    #region send server
    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        /// <summary>
        /// 领取奖励
        /// </summary>
        /// <param name="id"></param>
        public void Apply_Reward(uint id)
        {
            CmdActivityValGetRewardReq info = new CmdActivityValGetRewardReq() { Id = id };

            NetClient.Instance.SendMessage((ushort)CmdActivityVal.GetRewardReq, info);
        }
        /// <summary>
        /// 获取全部信息
        /// </summary>
        public void Apply_InfoReq()
        {
            CmdActivityValInfoReq info = new CmdActivityValInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdActivityVal.InfoReq, info);

        }

    
        public void Apply_ClearLimite()
        {
            CmdActivityValClientClearReq info = new CmdActivityValClientClearReq();

            NetClient.Instance.SendMessage((ushort)CmdActivityVal.ClientClearReq, info);

        }
    }

    #endregion


    #region client logic
    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        public bool JoinDaily(uint id,bool showtips = true)
        {
            if (Sys_Hint.Instance.PushForbidOprationInFight())
            {
                return false;
            }

            if (isDailyReady(id,showtips) == false)
                return false;

            var data = CSVDailyActivity.Instance.GetConfData(id);

            if (data.IsFamilyActive && Sys_Family.Instance.familyData.isInFamily == false)
            {
                Sys_Family.Instance.OpenUI_Family();

                return false;
            }

            if (m_DailyFuncDic.TryGetValue(id, out DailyFunc value) == false)
            {
                return false;
            }

           
            return value.OnJoin();
        }

        public void GotoDailyNpc(uint id)
        {
            var data = CSVDailyActivity.Instance.GetConfData(id);

            if (data == null)
                return;

            if (data.Npcid == 0)
            {
                JoinDaily(id);
                return;
            }

            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(data.Npcid);
        }

        public bool GotoActivity(uint id)
        {
            if (Sys_Hint.Instance.PushForbidOprationInFight())
            {
                return false;
            }

            var data = CSVDailyActivity.Instance.GetConfData(id);
            if (data == null)
                return false;

            if (data.IsFamilyActive && Sys_Family.Instance.familyData.isInFamily == false)
            {
                Sys_Family.Instance.OpenUI_Family();

                return false;
            }

            if (m_DailyFuncDic.TryGetValue(id, out DailyFunc value) == false)
            {
                return false;
            }

            return value.OnJoin();
        }
        public bool isDailyReady(uint id, bool bTips = true)
        {
            var data = CSVDailyActivity.Instance.GetConfData(id);

            if (data == null)
                return false;

            if (Sys_FunctionOpen.Instance.IsOpen(data.FunctionOpenid, false) == false)
            {
                if (bTips)
                {
                    string tipstring = LanguageHelper.GetTextContent(data.OpeningImpose, DailyDataHelper.GetOpenConditionString(data.FunctionOpenid));
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(data.OpeningImpose,tipstring));
                }

                return false;
            }


            if (isTodayDaily(id) == false)
            {
                if (bTips)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010257));
                return false;
            }

            if (data.ActiveType == 2)
            {
                var state = LimitDailyState(id);

                if (bTips)
                    LimiDailyStateTips(state);

                if (state == ELimitDailyState.Over || state == ELimitDailyState.WillStart || state == ELimitDailyState.TodayStart)
                    return false;
                
            }

            if (data.limite > 0 && getDailyCurTimes(id) >= data.limite)
            {
                if (bTips)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010261));
                return false;
            }


            return true;
        }

        private void LimiDailyStateTips(ELimitDailyState state)
        {
            uint tipsLangueId = 0;
            switch (state)
            {
                case ELimitDailyState.Over:
                    tipsLangueId = 2010258u;
                    break;
                case ELimitDailyState.WillStart:
                    tipsLangueId = 2010259u;
                    break;
                case ELimitDailyState.TodayStart:
                    tipsLangueId = 2010260u;
                    break;
            }

            if (tipsLangueId > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(tipsLangueId));
            }
        }
        public int getDayOfWeek()
        {
            var date = Sys_Time.ConvertToDatetime(Sys_Time.Instance.GetServerTime());

            var day = date.DayOfWeek;

            int tempday = Convert.ToInt32(day);

            tempday = tempday - 1;

            tempday = tempday < 0 ? (tempday + 8) : (tempday + 1);

            return tempday;
        }


        public uint GetDailyMaxActivityNum(uint id)
        {
            var data = CSVDailyActivity.Instance.GetConfData(id);

            if (data == null)
                return 0;

            if (data.ActivityNumMax == null || data.ActivityNumMax.Count == 0)
                return 0;

            int maxNumcount = data.ActivityNumMax.Count;

            if (maxNumcount == 1)
                return data.ActivityNumMax[0];

             int count = data.Play_Lv.Count;

            int index = count - 1;

            for (int i = 1; i < count; i++)
            {
                if (Sys_Role.Instance.Role.Level >= data.Play_Lv[i - 1] && Sys_Role.Instance.Role.Level < data.Play_Lv[i])
                {
                    index = i - 1;
                    break;
                }    

            }

            if (index > maxNumcount - 1)
                index = maxNumcount - 1;
           
            return data.ActivityNumMax[index];
        }

        /// <summary>
        /// 活动每次可获得的活跃度
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint GetDailyActivityNum(uint id)
        {
            var data = CSVDailyActivity.Instance.GetConfData(id);

            if (data == null)
                return 0;

            if (data.ActivityNum == null || data.ActivityNum.Count == 0)
                return 0;

            int maxNumcount = data.ActivityNum.Count;

            if (maxNumcount == 1)
                return data.ActivityNum[0];

            int count = data.Play_Lv.Count;

            int index = count - 1;

            for (int i = 1; i < count; i++)
            {
                if (Sys_Role.Instance.Role.Level >= data.Play_Lv[i - 1] && Sys_Role.Instance.Role.Level < data.Play_Lv[i])
                {
                    index = i - 1;
                    break;
                }

            }

            if (index > maxNumcount - 1)
                index = maxNumcount - 1;

            return data.ActivityNum[index];
        }
        public bool isDailyMaxActivityNum(uint id)
        {
            var data = CSVDailyActivity.Instance.GetConfData(id);

            if (data == null)
                return false;

            var maxnum = GetDailyMaxActivityNum(id);

            if (maxnum == 0)
                return false;

            var curActivity = getDailyAcitvity(id);

            if (curActivity >= GetDailyMaxActivityNum(id))
                return true;

            return false;
        }
        public bool SkipToDailyForMainTask()
        {
            if (UIManager.IsOpen(EUIID.UI_DailyActivites))
                return false;

            //var dataDic = CSVDailyActivity.Instance.GetDictData();
            //var dataList = dataDic.Values.ToList();

            //var dataList = new List<CSVDailyActivity.Data>(CSVDailyActivity.Instance.GetAll());

            //dataList.Sort((x, y) =>
            //{

            //    if (x.MainTaksRestrict > y.MainTaksRestrict)
            //        return 1;

            //    if (x.MainTaksRestrict == y.MainTaksRestrict)
            //        return 0;

            //    return -1;

            //});

            //uint findID = 0;
            //CSVDailyActivity.Data dailydata = null;
            //foreach (var data in dataList)
            //{
            //    if (isDailyReady(data.id, false) && !isDailyMaxActivityNum(data.id))
            //    {
            //        findID = data.id;
            //        dailydata = data;
            //        break;
            //    }
            //}

            //if (findID == 0 || (dailydata != null && dailydata.MainTaksRestrict == 0))
            //    findID = 50;

            UIManager.OpenUI(EUIID.UI_DailyActivites, false, new UIDailyActivitesParmas() { SkipToID = 0 });

            return true;
        }


        public bool HaveReward()
        {
            uint curDailyActivty = Sys_Daily.Instance.TotalActivity;

            int count = CSVDailyActivityReward.Instance.Count;

            for (int i = 0; i < count; i++)
            {
                var item = CSVDailyActivityReward.Instance.GetByIndex(i);
                if (curDailyActivty >= item.Activity && Sys_Daily.Instance.HadGetReward(item.id) == false)
                {
                    return true;
                }
            }

            return false;
        }
    }
    #endregion

    /// <summary>
    /// 魔物讨伐，由于魔物讨伐没有相关模块只记录数据，所以讲数据记在日常模块中
    /// </summary>
    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        public uint KilledMonsterCount { get; private set; } = 0;

        public uint KilledMonsterMaxCount { get; private set; } = 0;

        private void ReadKilledMonster()
        {
            var paramdata = CSVParam.Instance.GetConfData(1055);

            if (paramdata != null)
                KilledMonsterMaxCount = uint.Parse(paramdata.str_value);
        }

        private int GetKilledMosterTiems()
        {
           uint result = KilledMonsterCount / KilledMonsterMaxCount;

            if (result > 1)
                result = 1;

            return (int)result;
        }
    }
}
