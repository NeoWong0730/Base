using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using Table;
using System.Collections.Generic;
using System;
using Framework;

namespace Logic
{
    public class CutSceneWeatherData
    {
        public bool isStart;   //cutscene控制天气开始和结束
        public uint weatherId;   //cutscene天气id 0不控制  1晴天 2下雨 3下雪 4雷雨
        public uint dayOrNightId;     //cutscene日夜 0 不控制 1 白天 2夜晚
    }

    public class Sys_Weather : SystemModuleBase<Sys_Weather>, ISystemModuleUpdate
    {
        private List<uint> weatherList = new List<uint>();
        private List<int> removeWeatherList = new List<int>();

        private uint starttime;   
        private uint usetime;
        private uint curLeftTime;
        private int index;

        public bool isDay;
        private bool isCutSceneWeatherStart;

        public uint curWeather;
        public uint beforeWeather;
        public uint nextWeather;
        public uint curSeason;
        private uint removeMapDefaultWeather;

        private uint dayNightTime;
        private uint allSeasonsTime;
        private uint weatherTime;

        private DateTime specialstartdt;
        private DateTime specialenddt;

        private int tempIndex;
        private bool tempIsDay;
        private bool isRefreshData;
        private int lastFrameCount = 0;


        //环境声音
        private AudioEntry mAudioEntry;
        private AudioEntry mThunderAudioEntry;
        private uint currentAudioID;

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnWeatherChange,
            OnDayNightChange,
            OnSeasonChange,
            OnRemoveWeatherMap,
            OnCutSceneWeather,
        }

        public override void Init()
        {
            base.Init();
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMap.WeatherNtf, OnWeather, CmdMapWeatherNtf.Parser);

            dayNightTime = CSVWeatherTime.Instance.GetConfData(1).time;
            allSeasonsTime = CSVWeatherTime.Instance.GetConfData(2).time;
            weatherTime = CSVWeatherTime.Instance.GetConfData(3).time;

            eventEmitter.Handle<CutSceneWeatherData>(EEvents.OnCutSceneWeather, OnCutSceneWeather, true);
            eventEmitter.Handle(EEvents.OnRemoveWeatherMap, OnRemoveWeatherMap, true);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, true);
            WeatherSystem.OnFlashing += OnFlashing;
        }

        public override void Dispose()
        {
            eventEmitter.Handle<CutSceneWeatherData>(EEvents.OnCutSceneWeather, OnCutSceneWeather, false);
            eventEmitter.Handle(EEvents.OnRemoveWeatherMap, OnRemoveWeatherMap, false);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, false);

        }

        public override void OnLogin()
        {
            starttime = Framework.TimeManager.ConvertFromZeroTimeZone(CSVWeatherTime.Instance.GetConfData(4).time);
        }

        public override void OnLogout()
        {
            curWeather = 0;
            beforeWeather = 0;
            nextWeather = 0;
            usetime = 0;
            tempIndex = 0;
            isRefreshData = false;
            tempIsDay = false;
            isCutSceneWeatherStart = false;
            isDay = false;
            removeWeatherList.Clear();
            weatherList.Clear();
        }

        #region Function
        private void OnWeather(NetMsg msg)
        {
            CmdMapWeatherNtf ntf = NetMsgUtil.Deserialize<CmdMapWeatherNtf>(CmdMapWeatherNtf.Parser, msg);    
            weatherList.Clear();
            for (int i=0;i< ntf.Weathers.Count;i++)    //这一个季节的天气 + 下一个季节的第一个
            {
                weatherList.Add(ntf.Weathers[i]);
                DebugUtil.LogFormat(ELogType.eWeather, ntf.Weathers[i].ToString());
            }
            isRefreshData = true;
            eventEmitter.Trigger(EEvents.OnSeasonChange);
        }

        private void OnRemoveWeatherMap()
        {
            removeWeatherList.Clear();
            
            CSVMapInfo.Data infoData = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
            if (infoData==null|| infoData.remove_weather == null)
            {
                removeMapDefaultWeather = 0;
                return;
            }
            removeMapDefaultWeather = infoData.weather;
            for (int i=0; i< infoData.remove_weather.Count; ++i)
            {
                removeWeatherList.Add(infoData.remove_weather[i]);
            }
            isRefreshData = true;
        }

        private void OnCutSceneWeather(CutSceneWeatherData data)
        {
            isCutSceneWeatherStart = data.isStart;
            if (data.isStart)
            {
                if (data.weatherId != 0)
                {
                    WeatherSystem.gWeatherTypeBefore = GetEWeatherType(curWeather);
                    WeatherSystem.gWeatherType = (EWeatherType)data.weatherId;
                }
                if (data.dayOrNightId != 0)
                {
                    WeatherSystem.gDayStage = (EDayStage)(data.dayOrNightId-1);
                }
            }
            else
            {
                WeatherSystem.gWeatherType = GetEWeatherType(curWeather);
                WeatherSystem.gWeatherTypeBefore = GetEWeatherType(beforeWeather);
                WeatherSystem.gDayStage = isDay ? EDayStage.Day : EDayStage.Night;
            }
        }

        private void OnTimeNtf(uint time01,uint time02)
        {
            starttime = Framework.TimeManager.ConvertFromZeroTimeZone(CSVWeatherTime.Instance.GetConfData(4).time);
        }


        private uint GetCurLeftTime()
        {
            uint resultTime = 0;
            if (lastFrameCount != UnityEngine.Time.frameCount)
            {
                lastFrameCount = UnityEngine.Time.frameCount;
#if DEBUG_MODE
                if (WeatherSystem.gOneSeasonWeatherDebug)
                {
                    usetime = (uint)(WeatherSystem.gCurSeasonProgress * allSeasonsTime);
                }
                else
                {
                    usetime = Sys_Time.Instance.GetServerTime() - starttime;
                }
#else
            usetime = Sys_Time.Instance.GetServerTime() - starttime;
#endif
                uint curweatherleftTime = usetime % allSeasonsTime;
                resultTime = curweatherleftTime;
            }
            return resultTime;
        }

        public void GetCurWeather()
        {
            if (weatherList.Count > index )
            {
                curWeather = weatherList[index];
            }
            else
            {
                curWeather = 1;
            }
            uint special = isSpecialWeather();
            if (special != 0)
            {
                curWeather =CSVWeatherSpecial.Instance.GetConfData(special).weather;
            }
            if (removeWeatherList.Count != 0 && removeWeatherList.Contains((int)curWeather))
            {
                curWeather = removeMapDefaultWeather;
                DebugUtil.LogFormat(ELogType.eWeather, "enter mapid:" + Sys_Map.Instance.CurMapId.ToString() + "   server weatherid: " + weatherList[index].ToString() + "    default weatherid" + removeMapDefaultWeather.ToString());
            }
        }

        public void GetNextWeather()
        {
            if (weatherList.Count > index + 1)
            {
                nextWeather = weatherList[index + 1];
            }
            else
            {
                nextWeather = 1;
                DebugUtil.LogFormat(ELogType.eWeather, "当前天气列表不存在 " + (index + 1).ToString());
            }
            uint special = isSpecialWeatherBeforeOrNext(false);
            if (special != 0)
            {
                nextWeather = CSVWeatherSpecial.Instance.GetConfData(special).weather;
            }
            if (removeWeatherList.Count != 0 && removeWeatherList.Contains((int)nextWeather))
            {
                nextWeather = removeMapDefaultWeather;
            }
        }

        private void GetBeforeWeather()
        {
            if (index == 0)
            {
                beforeWeather = curWeather == 3 ? (uint)3 : (uint)1;
            }
            else
            {
                if (weatherList.Count > index - 1)
                {
                    beforeWeather = weatherList[index - 1];
                }
                else
                {
                    DebugUtil.LogFormat(ELogType.eWeather, "当前天气列表不存在 " + (index - 1).ToString());
                }
            }
            uint special = isSpecialWeatherBeforeOrNext(true);
            if (special != 0)
            {
                beforeWeather = CSVWeatherSpecial.Instance.GetConfData(special).weather;
            }
            if (removeWeatherList.Count != 0 && removeWeatherList.Contains((int)beforeWeather))
            {
                beforeWeather = removeMapDefaultWeather;
            }
        }

        public uint isSpecialWeather()
        {
            uint specialWeatherId = 0;
            DateTime nowdt = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            foreach (var data in CSVWeatherSpecial.Instance.GetAll())
            {
                if (data.time[0] == 0)
                {
                    specialstartdt = new DateTime(nowdt.Year, (int)data.time[1], (int)data.time[2], (int)data.time[3], (int)data.time[4], (int)data.time[5]);
                }
                else
                {
                    specialstartdt = new DateTime((int)data.time[0], (int)data.time[1], (int)data.time[2], (int)data.time[3], (int)data.time[4], (int)data.time[5]);
                }
                specialenddt = specialstartdt.AddSeconds(data.last_time);
                if (nowdt >= specialstartdt && nowdt < specialenddt)
                {
                    specialWeatherId = data.id;
                    break;
                }
            }
            return specialWeatherId;
        }

        public uint isSpecialWeatherBeforeOrNext(bool isbefore)
        {
            uint specialWeatherId = 0;
            DateTime nowdt = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            foreach (var data in CSVWeatherSpecial.Instance.GetAll())
            {
                if (data.time[0] == 0)
                {
                    specialstartdt = new DateTime(nowdt.Year, (int)data.time[1], (int)data.time[2], (int)data.time[3], (int)data.time[4], (int)data.time[5]);
                }
                else
                {
                    specialstartdt = new DateTime((int)data.time[0], (int)data.time[1], (int)data.time[2], (int)data.time[3], (int)data.time[4], (int)data.time[5]);
                }
                specialenddt = specialstartdt.AddSeconds(data.last_time);
                if (isbefore)
                {
                    if (nowdt < specialenddt.AddSeconds(data.last_time)&& nowdt>= specialenddt)
                    {
                        specialWeatherId = data.id;
                        break;
                    }
                }
                else
                {
                    if (nowdt.AddSeconds(weatherTime) >= specialstartdt && nowdt.AddSeconds(weatherTime) < specialenddt)
                    {
                        specialWeatherId = data.id;
                        break;
                    }
                }
          
            }
            return specialWeatherId;
        }

        public uint GetNextDayOrNightTime()
        {
           return dayNightTime - (curLeftTime % dayNightTime);
        }

        public EWeatherType GetEWeatherType(uint weatherid)
        {
            if (weatherid == 1)
            {
                return EWeatherType.Sunny;
            }
            else if (weatherid == 2)
            {
                return EWeatherType.Rain;
            }
            else if (weatherid == 3)
            {
                return EWeatherType.Snow;
            }
            else if (weatherid == 4)
            {
                return EWeatherType.Thunderstorm;
            }
            else
            {
                return EWeatherType.Sunny;
            }
        }

        public ESeasonStage GetESeasonStage()
        {
            uint time = (Sys_Time.Instance.GetServerTime() - starttime) % (allSeasonsTime * 4) / allSeasonsTime;
            if (time ==0)
            {
                return ESeasonStage.Spring;
            }
            else if (time==1)
            {
                return ESeasonStage.Summer;
            }
            else if (time==2)
            {
                return ESeasonStage.Fall;
            }
            else if (time ==3)
            {
                return ESeasonStage.Winter;
            }
            else
            {
                return ESeasonStage.Invalid;
            }
        }
#endregion

        public void OnUpdate()
        {
            UpdateTime();
            UpdateAudio();
        }

        private void UpdateTime()
        {
#if DEBUG_MODE
            if (WeatherSystem.gWeatherDebug)
                return;
#endif
            curLeftTime = GetCurLeftTime();
            tempIndex = (int)(curLeftTime / weatherTime);
            tempIsDay = curLeftTime / dayNightTime % 2 == 0;

            //天气变换 重置天气数据
            if (tempIndex != index|| isRefreshData)
            {
                index = tempIndex;
                if (!isCutSceneWeatherStart)
                {
                    GetBeforeWeather();
                    GetCurWeather();
                    GetNextWeather();
                    WeatherSystem.gWeatherType = GetEWeatherType(curWeather);
                    WeatherSystem.gWeatherTypeBefore = GetEWeatherType(beforeWeather);
                    DebugUtil.LogFormat(ELogType.eWeather, "OnWeatherChange" + WeatherSystem.gWeatherType.ToString());
                }
                eventEmitter.Trigger(EEvents.OnWeatherChange);            
            }

            //昼夜变换 重置昼夜数据
            if (tempIsDay != isDay || isRefreshData)
            {
                isDay = tempIsDay;
                WeatherSystem.gDayStage = isDay ? EDayStage.Day : EDayStage.Night;
                eventEmitter.Trigger(EEvents.OnDayNightChange);
            }
            WeatherSystem.gWeatherProgress = (float)curLeftTime % weatherTime / weatherTime;
            WeatherSystem.gDayProgress = (float)curLeftTime % (dayNightTime * 2) / (dayNightTime * 2);
            WeatherSystem.gSeasonProgress = (float)curLeftTime % (allSeasonsTime * 4) / (allSeasonsTime * 4);
            WeatherSystem.gCurSeasonProgress = (float)curLeftTime % allSeasonsTime / allSeasonsTime;
            isRefreshData = false;
        }

        private void UpdateAudio()
        {
            uint audioID = 0;

            if (WeatherSystem.gUseWeather != 0)
            {                
                switch (WeatherSystem.gWeatherType)
                {                    
                    case EWeatherType.Rain:
                        audioID = Constants.CSVID_RainAudio;
                        break;
                    case EWeatherType.Snow:
                        audioID = Constants.CSVID_SnowAudio;
                        break;
                    case EWeatherType.Thunderstorm:
                        audioID = Constants.CSVID_ThunderstormAudio;
                        break;
                    default:
                        break;
                }
            }

            //环境声音
            if (audioID != currentAudioID)
            {
                AudioUtil.StopAudio(mAudioEntry);
                mAudioEntry = null;
                AudioUtil.StopAudio(mThunderAudioEntry);
                mThunderAudioEntry = null;

                currentAudioID = audioID;
                if (currentAudioID > 0)
                {
                    DebugUtil.LogFormat(ELogType.eWeather, "播放天气音效 {0}", audioID);
                    mAudioEntry = AudioUtil.PlayAudioEx(audioID, true, true);
                }
            }
        }

        private void OnFlashing()
        {
            DebugUtil.LogFormat(ELogType.eWeather, "播放打雷音效");
            AudioUtil.StopAudio(mThunderAudioEntry);
            mThunderAudioEntry = AudioUtil.PlayAudioEx(Constants.CSVID_FlashingAudio, false, true);
        }
    }
}
