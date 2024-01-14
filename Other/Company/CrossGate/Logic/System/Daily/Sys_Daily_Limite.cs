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
        private uint m_ClearTime;
        public uint ClearNociteTime { get { return m_ClearTime; } }

        private uint mUpdataTimePoint = 0;
        private uint mUpdataTodayTimePoint = 0;

        public ulong mTodayZeroTimePoint { get; private set; } = 0;

        public DateTime mTodayDay; //今天零点
        public DateTime mTodayDay5; //今天5点
        public DateTime mYesterDay5;//昨天5点

        /// <summary>
        /// 获取当前的时间以秒为单位，从当天零点开始
        /// </summary>
        /// <returns></returns>
        private uint GetNowTime()
        {
            var nowtimedate = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());

            int nowSecond = nowtimedate.Hour * 3600 + nowtimedate.Minute * 60 + nowtimedate.Second;//当天时间 以秒为单位

            return (uint)nowSecond;
        }

        private void UpdataTodayTime()
        {
            mUpdataTimePoint = Sys_Time.Instance.GetServerTime();

            mUpdataTodayTimePoint = GetNowTime();

            mTodayZeroTimePoint = Sys_Time.Instance.GetDayZeroTimestamp();

            mTodayDay = Sys_Time.ConvertToDatetime((long)mTodayZeroTimePoint);
            mTodayDay5 = Sys_Time.ConvertToDatetime((long)(mTodayZeroTimePoint + 3600*5));
            mYesterDay5 = Sys_Time.ConvertToDatetime((long)(mTodayZeroTimePoint - 68400));
        }

        private void StopAutoUpdataTime()
        {
            mUpdataTodayTimePoint = 0;
            mTodayZeroTimePoint = 0;
        }


        public uint GetTodayTimeSceond()
        {
            uint offset = Sys_Time.Instance.GetServerTime() - mUpdataTimePoint;

            uint result = mUpdataTodayTimePoint + offset;

            uint daytime = 24 * 3600;

            var next = result  - (daytime);

            if (next >= 0)
            {
                result = result - ((uint)(result/(daytime*1f)))*daytime;
            }
                
            return result;
        }

    }

    #region limite daily time
    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        public class DailyWithLimiteTime
        {
            public int OpenTime;

            public int OpenMinutes;

            public uint DailyID;
        }

        public enum ELimitDailyState
        {
            None,
            WillStart,
            TodayStart,
            Opening,
            Close,
            Over,
        }

        /// <summary>
        /// 酒吧功能是否可以使用
        /// </summary>
        /// <returns></returns>
        public bool IsPubDailyReady()
        {
            GetPubTimes();

            var data = CSVDailyActivity.Instance.GetConfData(60);

            bool dailyIsOk = data.ActiveType == 2 ? (LimitDailyState(60) == ELimitDailyState.Opening) : (isTodayDaily(60));


            bool hadFresh = mBarCmd.HasFreshed;

            return (dailyIsOk) && (!hadFresh);
        }
        

        public ELimitDailyState LimitDailyState(uint id)
        {
            return GetLimitDailyState(id);
        }
        public ELimitDailyState GetLimitDailyState(uint id)
        {
            DailyFunc dailyFunc = null;

            if (m_DailyFuncDic.TryGetValue(id, out dailyFunc) == false)
                return ELimitDailyState.None;

            if (dailyFunc.DailyType != EDailyType.Limite)
                return ELimitDailyState.None;

            if (dailyFunc.isUnLock() == false || dailyFunc.IsInOpenDay() == false)
                return ELimitDailyState.Close;

            var opentime = dailyFunc.GetOpenTimeData();

            if (opentime == null)
                return ELimitDailyState.Over;

            var nowtime = Sys_Time.Instance.GetServerTime();

            var todayhourtime = nowtime % 86400;

            if (todayhourtime >= opentime.OpenTime && todayhourtime < opentime.EndTime)
                return ELimitDailyState.Opening;

            if (todayhourtime < opentime.OpenTime && (opentime.OpenTime - (int)todayhourtime) <= dailyFunc.Data.NoticeLong)
                return ELimitDailyState.WillStart;

            return ELimitDailyState.TodayStart;

        }
      
    }
    #endregion 
}
