using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public enum EDailyType
    {
        Daily, //日常
        Week,//周常
        Limite,//限时
        Holiday,//节日
        NotActive,//未开启
    }

    public class DailyOpenData
    {
        public uint ID;

        public int OpenTime;

        public int EndTime;

    }


    public class DailyNotice
    {
        public int StartNoticeTime;
        public int EndNoticeTime;

        public uint ID;

        /// <summary>
        /// 提示类型 0 开始前, 1 开始时
        /// </summary>
        public uint Type = 0;
    }

    public class DailyFunc
    {
        public uint DailyID { get; set; }

        public EDailyType DailyType { get; private set; }

        public CSVDailyActivity.Data Data { get; protected set; }

        protected List<DailyOpenData> m_OpenDatas = new List<DailyOpenData>();

        protected List<DailyNotice> m_Notice = new List<DailyNotice>();

        public bool IsNoticing { get; private set; }

        public bool IsOpenning { get; private set; }

      //  public bool IsNew { get; private set; }
        public virtual void Init()
        {
            Data = CSVDailyActivity.Instance.GetConfData(DailyID);

            UpdataDailyType();

            if (DailyType == EDailyType.Limite)
            {
                ReadNoticDatas();
                ReadOpenDatas();
            }

        }

        public virtual bool InitTimeWork()
        {
            if (DailyType != EDailyType.Limite || isUnLock() == false || IsInOpenDay() == false)
                return false;
            bool result = InitOpenTimeWork();
            bool result0 = InitNoticeTimeWork();

            //DebugUtil.LogError("Init Time Work  : " + Data.id.ToString());

            return result | result0;
        }
  
        /// <summary>
        /// 初始化开启时间 
        /// </summary>
        private void ReadOpenDatas()
        {
            if (Data.OpeningTime == null)
                return;

            int count = Data.OpeningTime.Count;

            for (int i = 0; i < count; i++)
            {
                var opentimeArray = Data.OpeningTime[i];

                if (opentimeArray == null || opentimeArray.Count != 2)
                {
                    DebugUtil.LogError("日常的开启时间填表错误，限时活动必须设置开启时间，请修正 ");
                    continue;
                }

                var start = opentimeArray[0] * 3600 + opentimeArray[1] * 60;
                var endtime = start + (int)Data.Duration;

                DailyOpenData opendata = new DailyOpenData() { 
                    OpenTime = start,
                    EndTime = endtime
                };

                m_OpenDatas.Add(opendata);

                if (Data.Playing > 0)
                {
                    var notice = new DailyNotice()
                    {
                        ID = Data.id,
                        StartNoticeTime = start + 1,
                        EndNoticeTime = start + (int)Data.Playing,
                        Type = 1

                    };

                    m_Notice.Add(notice);
                  
                }


            }

        }

        private void ReadNoticDatas()
        {
            if (Data.NoticeTime == null)
                return;

            int count = Data.NoticeTime.Count;

            for (int n = 0; n < count; n++)
            {
                int startNoticeTime = Data.NoticeTime[n][0] * 3600 + Data.NoticeTime[n][1] * 60;
                int endNoticeTime = startNoticeTime + (int)Data.NoticeLong;  

                var notice = new DailyNotice()
                {
                    ID = Data.id,
                    StartNoticeTime = startNoticeTime,
                    EndNoticeTime = endNoticeTime,
                    Type = 0

                };

                m_Notice.Add(notice);
            }

        }


        protected bool InitOpenTimeWork()
        {
            int opendatacount = m_OpenDatas.Count;

            var nowtimetoday = Sys_Time.Instance.GetServerTime() % 86400;

            bool result = false;

            var TodayZeroTimePoint = Sys_Daily.Instance.mTodayZeroTimePoint;

            for (int i = 0; i < opendatacount; i++)
            {
                if (nowtimetoday < m_OpenDatas[i].OpenTime)
                {
                    Sys_Daily.Instance.PushTimeWork((uint)((uint)TodayZeroTimePoint + m_OpenDatas[i].OpenTime), i, Data.id, Sys_Daily.EDailyTimerWorkType.Open);
                    result = true;
                }

                if (nowtimetoday < m_OpenDatas[i].EndTime)
                {
                    Sys_Daily.Instance.PushTimeWork((uint)((uint)TodayZeroTimePoint + m_OpenDatas[i].EndTime), i, Data.id, Sys_Daily.EDailyTimerWorkType.Close);
                    result = true;


                    if (Data.id == 101)
                        Sys_Daily.Instance.PushTimeWork((uint)((uint)TodayZeroTimePoint + (uint)m_OpenDatas[i].EndTime + 2), i, Data.id, Sys_Daily.EDailyTimerWorkType.SendDataToServer);
                }
                    
                if (nowtimetoday >= m_OpenDatas[i].OpenTime && nowtimetoday < m_OpenDatas[i].EndTime)
                    OnOpenTime((uint)i);
            }

            return result;
        }

        protected bool InitNoticeTimeWork()
        {
            int count = m_Notice.Count;

            var nowtimetoday = Sys_Time.Instance.GetServerTime() % 86400;

            bool result = false;

            var TodayZeroTimePoint = Sys_Daily.Instance.mTodayZeroTimePoint;


            for (int i = 0; i < count; i++)
            {

                if (m_Notice[i].StartNoticeTime + (uint)TodayZeroTimePoint <= Sys_Daily.Instance.ClearNociteTime)
                    continue;

                if (nowtimetoday < m_Notice[i].StartNoticeTime)
                {
                    Sys_Daily.Instance.PushTimeWork((uint)m_Notice[i].StartNoticeTime + (uint)TodayZeroTimePoint, i, Data.id, Sys_Daily.EDailyTimerWorkType.PreTips);
                    result = true;
                }

                if (nowtimetoday < m_Notice[i].EndNoticeTime)
                {
                    Sys_Daily.Instance.PushTimeWork((uint)m_Notice[i].EndNoticeTime + (uint)TodayZeroTimePoint, i, Data.id, Sys_Daily.EDailyTimerWorkType.PreTipsClose);
                    result = true;
                }

                if (nowtimetoday >= m_Notice[i].StartNoticeTime && nowtimetoday < m_Notice[i].EndNoticeTime)
                    OnNotice((uint)i);
            }

            return result;
        }
        /// <summary>
        /// 活动类型 
        /// </summary>
        protected void UpdataDailyType()
        {
            switch (Data.ActiveType)
            {
                case 1:
                    DailyType = EDailyType.Daily;
                    break;
                case 2:
                    DailyType = EDailyType.Limite;
                    break;
                case 4:
                    DailyType = EDailyType.Holiday;
                    break;
                case 5:
                    DailyType = EDailyType.Week;
                    break;
            }
        }

        /// <summary>
        /// 已经使用的次数
        /// </summary>
        /// <returns></returns>
        public virtual uint GetTimes()
        {
            return Sys_Daily.Instance.GetDailyTimes(DailyID);
        }

        public virtual bool HaveReward()
        {
            return false;
        }

        public DailyNotice GetDailyNotice(int index)
        {
            if (index >= m_Notice.Count)
                return null;

            return m_Notice[index];
        }

        /// <summary>
        /// 获取有效的活动时间，活动时间可能还未到
        /// </summary>
        /// <returns></returns>
        public virtual uint GetOpenTime()
        {
            var nowtime = Sys_Time.Instance.GetServerTime();

            int count = m_OpenDatas.Count;

            var todayhourtime = nowtime % 86400;

            for (int i = 0; i < count; i++)
            {
                if (todayhourtime < (uint)m_OpenDatas[i].EndTime)
                {
                    return (uint)m_OpenDatas[i].OpenTime;
                }

            }

            return 0;
        }

        /// <summary>
        /// 获取当前或者最近的一个活动时间
        /// </summary>
        /// <returns></returns>
        public DailyOpenData GetOpenTimeData()
        {
            var nowtime = Sys_Time.Instance.GetServerTime();

            var todayhourtime = nowtime % 86400;

            int count = m_OpenDatas.Count;

            for (int i = 0; i < count; i++)
            {
                if (todayhourtime < (uint)m_OpenDatas[i].EndTime)
                {
                    return m_OpenDatas[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 活动是否在开启的日期
        /// </summary>
        /// <returns></returns>
        public virtual bool IsInOpenDay()
        {
            if (Data.ActiveType != 2)
                return true;

            if (Data.HideLevel > 0 && Sys_Role.Instance.Role.Level >= Data.HideLevel)
                return false;

            var starttime = Sys_Daily.Instance.getDailyStartTime(Data.ResetTime);

            bool isDoubleWeek = (starttime.Day / 7) % 2 == 0;

            if (Data.OpeningMode1 == 2 && !isDoubleWeek) // 双周
                return false;

            if (Data.OpeningMode1 == 1 && isDoubleWeek)//单周
                return false;

            var value0 = (int)(starttime.DayOfWeek);

            value0 = value0 % 7 == 0 ? (value0 + 7) : value0;

            if (Sys_Daily.DayOfWeekCompate(Data, value0) == false)//周几开启
                return false;

            return true;
        }

        /// <summary>
        /// 获取正在进行的活动
        /// </summary>
        /// <returns></returns>
        public virtual bool IsInOpenTime()
        {
            var nowtime = Sys_Time.Instance.GetServerTime();

            int count = m_OpenDatas.Count;

            var todayhourtime = nowtime % 86400;

            bool result = false;

            for (int i = 0; i < count; i++)
            {
                if (todayhourtime >= (uint)m_OpenDatas[i].OpenTime && todayhourtime < (uint)m_OpenDatas[i].EndTime)
                {
                    result = true;
                    break;
                }

            }
            return result;
        }

        public void UnLock()
        {
            
        }

        /// <summary>
        /// 活动是否解锁
        /// </summary>
        /// <returns></returns>
        public virtual bool isUnLock()
        {
            bool result = Sys_FunctionOpen.Instance.IsOpen(Data.FunctionOpenid);

            if (!result)
                return false;

            return Data.HideLevel == 0 || (Data.HideLevel > Sys_Role.Instance.Role.Level);
        }

        /// <summary>
        /// 该活动是否是激活且在活动时间范围内
        /// </summary>
        /// <returns></returns>
        public bool isVaild()
        {
            if (isUnLock() && IsInOpenDay() && IsInOpenTime())
                return true;

            return false;
        }


        public virtual bool OnJoin()
        {

            if (Data.Npcid != 0)
            {
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(Data.Npcid);
            }
            else if (Data.Uiid != 0)
            {
                UIManager.CloseUI(EUIID.UI_DailyActivites);

                if (Data.UiidSonId > 0)
                    UIManager.OpenUI((EUIID)Data.Uiid,false, Data.UiidSonId);
                else
                    UIManager.OpenUI((EUIID)Data.Uiid);
                return false;
            }

            return true;
        }

        public virtual void OnNotice(uint index)
        {
            uint key = Data.id << 16 | index;

            Sys_Daily.Instance.AddNotice(key);

            IsNoticing = true;

            SetNewNoticeTips(true);
            //DebugUtil.LogError("start notice " + Data.id + "time :" + m_Notice[(int)index].StartNoticeTime.ToString());
        }

        public virtual void OnNoticeEnd(uint index)
        {
            uint key = Data.id << 16 | index;

            Sys_Daily.Instance.RemoveNotice(key);

            IsNoticing = false;

           SetNewNoticeTips(false);
           // DebugUtil.LogError("end notice " + Data.id + "time :" + m_Notice[(int)index].EndNoticeTime.ToString());
        }

        public virtual void OnOpenTime(uint index)
        {
            IsOpenning = true;
            SetNewOpenTips(true);   
        }

        public virtual void OnEndTime(uint index)
        {
            IsOpenning = false;
            SetNewOpenTips(false);
        }


        public bool HaveNewUnlockTips()
        {
           var value = Sys_Daily.Instance.GetDailyNewTips(DailyID);

            if (value == null)
                return false;

            return value.isFuncTips;
        }

        public bool HaveNewOpenTips()
        {
            var value = Sys_Daily.Instance.GetDailyNewTips(DailyID);

            if (value == null)
                return false;

            return value.isLimiteTips;
        }

        public bool HaveNewNotice()
        {
            var value = Sys_Daily.Instance.GetDailyNewTips(DailyID);

            if (value == null)
                return false;

            return value.isNoctice;
        }

        public void SetNewUnlockTips(bool active)
        {
            Sys_Daily.Instance.SetNewTipsNewFunc(Data.id, active);
        }

        public void SetNewOpenTips(bool active)
        {
            Sys_Daily.Instance.SetNewTipsNewLimite(DailyID,active);    
        }

        public void SetNewNoticeTips(bool active)
        {
            Sys_Daily.Instance.SetNewTipsNewNotice(DailyID,active);

        }
        public void DisableAllTips()
        {
            Sys_Daily.Instance.CloseNewTips(DailyID);
        }

        /// <summary>
        /// 判断 挑战活动，每日活动 活动是否要置灰
        /// </summary>
        /// <returns></returns>
        public virtual bool HadUsedTimes()
        {
            if (Data.ActiveType != 1 && Data.ActiveType != 5 && Data.ActiveType != 6)
                return false;

            int acstate = -1;
            int timesstate = -1;

            if (Sys_Daily.Instance.GetDailyMaxActivityNum(DailyID) > 0)
            {
                bool ismaxac = Sys_Daily.Instance.isDailyMaxActivityNum(DailyID);

                acstate = ismaxac ? 1 : 0;
            }

            if (Data.limite > 0)
            {
                bool ismaxtimes = Sys_Daily.Instance.IsDailyTimesMax(DailyID);
                timesstate = ismaxtimes ? 1 : 0;
            }

            bool valu0isMax = false;

            if ((acstate == 1 && timesstate == 1) || (acstate < 0 && timesstate == 1) || (timesstate < 0 && acstate == 1))
                valu0isMax = true;

            if (valu0isMax && DailyID == 140)
            {
                var jsdata = Sys_JSBattle.Instance.GetRoleVictoryArenaDaily();

                if (jsdata != null && (jsdata.LeftBuyTimes > 0 || jsdata.LeftChallengeTimes > 0))
                {
                    valu0isMax = false;
                }
            }
            else if (valu0isMax && DailyID == 120)
            {
                var value = Sys_Hangup.Instance.GetTired();
                var value0 = Sys_Hangup.Instance.cmdHangUpDataNtf.WorkingHourPoint;
                if (value < 100 || value0 > 0)
                    valu0isMax = false;
            }

            return valu0isMax;
        }
    }
}
