using System;
using UnityEngine;

namespace Framework
{
    public enum EUpdateInterval
    {
        EveryFrame = 0,
        IntervalFrame = 1,
        IntervalTime = 2,
    }

    public static class TimeManager
    {
        #region FrameSplit Variable
        private const int gMaxFrameRate = 120;
        private const int gMinFrameRate = 5;

        private static int _frameMultiple = 1;
        private static int _frameCount;

        public static int frameCount { get { return _frameCount; } }
        #endregion

        #region FPS Variable
        public static int FPS { get; private set; }

        private static bool _bCalculateFPS = false;
        private static int _beforeFrameCount = 0;
        private static float _beforeTimePoint = 0f;
        #endregion

        #region ServerTime Variable
        // 服务器时区与UTC标准时区之间的差值秒数
        public static long TimeZoneOffset { get { return _timeZoneOffset; } }
        public static bool IsValidServerTime { get { return _bValidServerTime; } }

        private static long _timeZoneOffset = 0;
        private static bool _bValidServerTime = false;

        private static float _syncClientTime = 0;
        public static uint _syncServeTime = 0;

        private static readonly DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly long epoch = dateTime.Ticks;
        #endregion

        #region FPS Function
        public static void StartUpdate()
        {
            _beforeFrameCount = Time.frameCount;
            _beforeTimePoint = Time.realtimeSinceStartup;
        }

        public static void StartFPSCalculate()
        {
            _bCalculateFPS = true;
        }
        public static void StopFPSCalculate()
        {
            _bCalculateFPS = false;
        }
        public static void UpdateFPS()
        {
            if (_bCalculateFPS)
            {
                float realtimeSinceStartup = Time.realtimeSinceStartup;
                int frameCount = Time.frameCount;

                float timeSpace = realtimeSinceStartup - _beforeTimePoint;
                if (timeSpace >= 0.5f)
                {
                    float fps = (frameCount - _beforeFrameCount) / timeSpace;
                    FPS = (int)fps;
                    if (fps > FPS)
                    {
                        FPS += 1;
                    }
                    _beforeFrameCount = frameCount;
                    _beforeTimePoint = realtimeSinceStartup;
                }
            }
        }
        #endregion

        #region ServerTime Function
        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <param name="withoutTimeZone">是否剔除时区信息</param>
        /// <returns></returns>
        public static uint GetServerTime(bool withoutTimeZone = false)
        {
            if (_bValidServerTime)
            {
                float diff = UnityEngine.Time.realtimeSinceStartup - _syncClientTime;
                uint serverTime = _syncServeTime + (uint)UnityEngine.Mathf.RoundToInt(diff);
                return withoutTimeZone ? serverTime - (uint)_timeZoneOffset : serverTime;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 修正服务器时间
        /// </summary>
        /// <param name="serverTime"></param>
        public static void CorrectServerTime(uint serverTime)
        {
            _syncServeTime = serverTime;
            _syncClientTime = UnityEngine.Time.realtimeSinceStartup;
            _bValidServerTime = true;
        }

        /// <summary>
        /// 修正服务器时间的时区偏移
        /// </summary>
        public static void CorrectServerTimeZone(int timeZone)
        {
            _timeZoneOffset = timeZone;
        }

        /// <summary>
        /// 判断是否为同一天
        /// </summary>
        /// <param name="seconds1"></param>
        /// <param name="seconds2"></param>
        /// <returns></returns>
        public static bool IsSameDay(uint seconds1, uint seconds2, uint offset = 0)
        {
            if (seconds1 == seconds2)
            {
                return true;
            }
            else
            {
                DateTime date1 = ConvertToLocalTime(seconds1 - offset, _timeZoneOffset);
                DateTime date2 = ConvertToLocalTime(seconds2 - offset, _timeZoneOffset);
                return (date1.Year == date2.Year && date1.DayOfYear == date2.DayOfYear);
                // UnityEngine.Debug.LogError(seconds1 + " " +  seconds2 + " " + date1.Year + " " + date2.Year + "    " + " " + date1.DayOfYear + " " + date2.DayOfYear);
            }
        }
        
        // 每日5点
        public static bool IsSameDay5(uint seconds1, uint seconds2)
        {
            return IsSameDay(seconds1, seconds2, 5 * 3600);
        }
        
        /// <summary>
        /// localTimeStamp是北京时间的时间戳
        /// </summary>
        /// <param name="localTimeStamp"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(uint localTimeStamp)
        {
            DateTime dateTime = START_TIME;
            dateTime = dateTime.AddSeconds(localTimeStamp - _timeZoneOffset).ToLocalTime();
            return dateTime;
        }

        public static readonly DateTime START_TIME = new DateTime(1970, 1, 1);

        public static uint GetLocalNow()
        {
            return (uint)DateTime.Now.Subtract(START_TIME).TotalSeconds;
        }

        /// <summary>
        /// 将0时区的时间 转换为当地时间
        /// </summary>
        /// <param name="timeWithoutTimeZone">将0时区的时间</param>
        /// <returns></returns>
        public static uint ConvertFromZeroTimeZone(long timeWithoutTimeZone)
        {
            return (uint)(timeWithoutTimeZone + _timeZoneOffset);
        }

        /// <summary>
        /// 转换时间到当地时间
        /// </summary>
        /// <param name="timeStampWithTimeZone">包含时区的时间</param>
        /// <param name="timeZoneOffse">时区偏移</param>
        /// <returns></returns>
        public static DateTime ConvertToLocalTime(long timeStampWithTimeZone, long timeZoneOffse)
        {
            timeStampWithTimeZone = timeStampWithTimeZone - timeZoneOffse;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(Consts.START_TIME);
            DateTime targetTime = startTime.AddSeconds(timeStampWithTimeZone);

            return targetTime;
        }

        /// <summary>
        /// 游戏开始流逝时间(按时间戳计算)
        /// </summary>
        /// <returns></returns>
        public static uint GetElapseTime()
        {
            uint curServerTime = GetServerTime();
            if (curServerTime > _syncServeTime)
                return curServerTime - _syncServeTime;
            else
                return 0u;
        }
        
        public static long ClientNowMillisecond()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000;
        }
        #endregion

        #region FrameSplit Function
        public static void DeltaTimeUpdate()
        {
#if USE_SPLIT_FRAME
            if (Application.targetFrameRate < 1)
            {
                _frameMultiple = gMaxFrameRate / 60;
            }
            else
            {
                _frameMultiple = gMaxFrameRate / Application.targetFrameRate;
            }

            if (_frameMultiple < 1)
            {
                _frameMultiple = 1;
            }
            _frameCount = Time.frameCount;
#endif
        }

        public static bool CanExecute(int offsetFrame, int intervalFrame)
        {
#if USE_SPLIT_FRAME
            int realOffsetFrame = offsetFrame / _frameMultiple;
            int realIntervalFrame = intervalFrame / _frameMultiple;

            if (realIntervalFrame < 1)
            {
                realIntervalFrame = 1;
            }

            return (_frameCount - realOffsetFrame) % realIntervalFrame == 0;
#else
            return true;
#endif
        }

        public static int CalRealOffsetFrame(int offsetFrame)
        {
            return offsetFrame / _frameMultiple;
        }

        public static int CalRealIntervalFrame(int intervalFrame)
        {
            int realIntervalFrame = intervalFrame / _frameMultiple;

            if (realIntervalFrame < 1)
            {
                realIntervalFrame = 1;
            }

            return realIntervalFrame;
        }

        #endregion

        public static void Update()
        {            
            UpdateFPS();
            DeltaTimeUpdate();
        }
    }
}