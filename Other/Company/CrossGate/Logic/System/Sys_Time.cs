using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;

namespace Logic
{
    /// 服务器时间
    public class Sys_Time : SystemModuleBase<Sys_Time>
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents {
            OnTimeNtf, // 事件更新
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.ServerTimeNtf, OnServerTimeNtf, CmdRoleServerTimeNtf.Parser);
        }

        private void OnServerTimeNtf(NetMsg msg)
        {
            CmdRoleServerTimeNtf res = NetMsgUtil.Deserialize<CmdRoleServerTimeNtf>(CmdRoleServerTimeNtf.Parser, msg);
            uint oldTime = GetServerTime();
            uint newTime = res.ServerTime;
            TimeManager.CorrectServerTime(newTime);

            DebugUtil.LogFormat(ELogType.eTask, "OnServerTimeNtf");
            eventEmitter.Trigger<uint, uint>(EEvents.OnTimeNtf, oldTime, newTime);
        }

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <param name="withoutTimeZone">是否剔除时区信息</param>
        /// <returns></returns>
        public uint GetServerTime(bool withoutTimeZone = false)
        {
            return TimeManager.GetServerTime(withoutTimeZone);
        }

        /// <summary>
        /// 转换到当地时区的时间
        /// </summary>
        /// <param name="timeStampWithTimeZone">服务器时区时间</param>
        /// <returns></returns>
        public static DateTime ConvertToLocalTime(long timeStampWithTimeZone)
        {
            return TimeManager.ConvertToLocalTime(timeStampWithTimeZone, TimeManager.TimeZoneOffset);
        }

        /// <summary>
        /// 开放周数，服务器开始那周为第一周（单周）
        /// </summary>
        /// <returns></returns>
        //public static int WeekOfService()
        //{
        //    uint openday = Sys_Role.Instance.openServiceDay;
        //    DateTime nowtime = GetDateTime(TimeManager.GetServerTime());
        //    var startDate = nowtime - TimeSpan.FromDays(openday);
        //    var  value0 = (int) (startDate.DayOfWeek);
        //    value0 = value0 % 7 == 0 ? (value0 + 7) : value0;
        //    int offset = (openday - (7 - value0)) % 7 == 0 ? 0 : 1;
        //    int week = 1 + (int)(((openday - (7 - value0))) / 7) + offset;
        //    return week;
        //}     


        /// <summary>
        /// 获取当天0点时间戳
        /// </summary>
        /// <returns></returns>
        public ulong GetDayZeroTimestamp(bool withoutTimeZone = false)
        {
            var serverTime = GetServerTime(withoutTimeZone);
            long TimeOffset = withoutTimeZone ? TimeManager.TimeZoneOffset : 0;
            return (ulong)(serverTime - (serverTime + TimeOffset) % 86400);
        }

        public static bool IsServerSameDay5(long xServerSecondsWithTimeZone, long yServerSecondsWithTimeZone, long offset = 0) {
            return IsServerSameDay(xServerSecondsWithTimeZone - 5 * 3600, yServerSecondsWithTimeZone - 5 * 3600, offset);
        }

        public static bool IsServerSameDay(long xServerSecondsWithTimeZone, long yServerSecondsWithTimeZone, long offset = 0)
        {
            if (xServerSecondsWithTimeZone == yServerSecondsWithTimeZone)
            {
                return true;
            }
            else {
                DateTime date1 = ConvertToDatetime(xServerSecondsWithTimeZone - offset);
                DateTime date2 = ConvertToDatetime(yServerSecondsWithTimeZone - offset);
                return (date1.Year == date2.Year && date1.DayOfYear == date2.DayOfYear);
                // UnityEngine.Debug.LogError(seconds1 + " " +  seconds2 + " " + date1.Year + " " + date2.Year + "    " + " " + date1.DayOfYear + " " + date2.DayOfYear);
            }
        }
        
        public static DateTime ConvertToDatetime(long timeStampWithTimeZone)
        {
            DateTime targetTime = Consts.START_TIME.AddSeconds(timeStampWithTimeZone);
            return targetTime;
        }
    }
}
