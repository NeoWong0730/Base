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
        private const int gMaxFrameRate = 120;
        private const int gMinFrameRate = 5;

        private static int _frameMultiple = 1;
        private static int _frameCount;

        public static int frameCount { get { return _frameCount; } }

        public static int FPS { get; private set; }

        private static bool _bCalculateFPS = false;
        private static int _beforeFrameCount = 0;
        private static float _beforeTimePoint = 0f;

        public static long TimeZoneOffset { get { return _timeZoneOffset; } }
        public static bool IsValidServerTime { get { return _bValidServerTime; } }

        private static long _timeZoneOffset = 0;
        private static bool _bValidServerTime = false;

        private static float _syncClientTime = 0;
        public static uint _syncServerTime = 0;

        private static readonly DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly long epoch = dateTime.Ticks;

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

        public static uint GetServerTime(bool withoutTimeZone = false)
        {
            if (_bValidServerTime)
            {
                float diff = UnityEngine.Time.realtimeSinceStartup - _syncClientTime;
                uint serverTime = _syncServerTime + (uint)UnityEngine.Mathf.RoundToInt(diff);
                return withoutTimeZone ? serverTime - (uint)_timeZoneOffset : serverTime;
            }
            else
            {
                return 0;
            }
        }

        public static void CorrectServerTime(uint serverTime)
        {
            _syncServerTime = serverTime;
            _syncClientTime = UnityEngine.Time.realtimeSinceStartup;
            _bValidServerTime = true;
        }

        public static void CorrectServerTimeZone(int timeZone)
        {
            _timeZoneOffset = timeZone;
        }

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
            }
        }

        public static bool IsSameHourInSameDay(uint seconds1, uint seconds2, uint hour)
        {
            return IsSameDay(seconds1, seconds2, hour * 3600);
        }

        public static DateTime GetDateTime(uint localTimeStamp)
        {
            DateTime dateTime = Consts.START_TIME;
            dateTime = dateTime.AddSeconds(localTimeStamp - _timeZoneOffset).ToLocalTime();
            return dateTime;
        }

        public static uint GetLocalNow()
        {
            return (uint)DateTime.Now.Subtract(Consts.START_TIME).TotalSeconds;
        }

        public static uint ConvertFromZeroTimeZone(long timeWithoutTimeZone)
        {
            return (uint)(timeWithoutTimeZone + _timeZoneOffset);
        }

        public static DateTime ConvertToLocalTime(long timeStampWithTimeZone, long timeZoneIffset)
        {
            timeStampWithTimeZone = timeStampWithTimeZone - timeZoneIffset;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(Consts.START_TIME);
            DateTime targetTime = startTime.AddSeconds(timeStampWithTimeZone);

            return targetTime;
        }

        public static uint GetElapseTime()
        {
            uint curServerTime = GetServerTime();
            if (curServerTime > _syncServerTime)
                return curServerTime - _syncServerTime;
            else
                return 0u;
        }

        public static long ClientNowMillisecond()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000;
        }

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
                realIntervalFrame = 1;

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

        public static void Update()
        {
            UpdateFPS();
            DeltaTimeUpdate();
        }
    }
}
