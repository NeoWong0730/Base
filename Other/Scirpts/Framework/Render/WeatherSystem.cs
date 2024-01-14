using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Framework
{
    public enum EDayStage
    {
        Invalid = -1,
        Day = 0,
        Night = 1,
    }

    public enum ESeasonStage
    {
        Invalid = -1,
        Spring = 0,
        Summer = 1,
        Fall = 2,
        Winter = 3,
    }

    public enum EWeatherType
    {
        Sunny = 1,
        Rain = 2,
        Snow = 3,
        Thunderstorm = 4,
    }

    public static class WeatherSystem
    {
        public static Action<EDayStage, EDayStage> OnDayNightStageChange;//from, to
        public static Action<ESeasonStage, ESeasonStage> OnSeasonStageChange;//from, to
        public static Action<EWeatherType, EWeatherType> OnWeatherTypeChange;//from, to
        public static Action OnFlashing;

        public static Color gAmbientSH { get; set; }
        public static int gUseWeather { get; set; } = 0;                                  //是否存在天气系统    
        public static float gEmissiveLerp { get; set; } = 0;                              //夜晚自发光

        public static ESeasonStage gSeasonStage { get; set; } = ESeasonStage.Invalid;
        public static EDayStage gDayStage { get; set; } = EDayStage.Invalid;
        public static EWeatherType gWeatherType { get; set; } = EWeatherType.Thunderstorm;
        public static EWeatherType gWeatherTypeBefore { get; set; } = EWeatherType.Sunny;

        public static float gSeasonProgress { get; set; } = 0;
        //public static float gSeasonStart = 0;
        //public static float gSeasonDuration = 86400;

        public static float gDayProgress { get; set; } = 0.8f;
        //public static float gDayStart = 0;
        //public static float gDayDuration = 1800;

        public static float gWeatherProgress { get; set; } = 0;
        //public static float gWeatherStart = 0;
        //public static float gWeatherDuration = 900;

        public static float gCurSeasonProgress { get; set; } = 0;

        public static float gSnowCoverageRate { get; set; } = 0f; //当前雪覆盖率
        public static float gRainCoverageRate { get; set; } = 0f; //当前雪覆盖率

        public static bool gFlashing { get; set; } = false;       //当前正在打雷
        public static float gFlashStageTime { get; set; }        //当前打雷播放时间 或 等待打雷的时间
        public static float gSnowRatio = 0.5f;
        public static float gSnowHeight = 0.25f;
        public static Texture gWeatherTexture = null;
        public static Color gSnowColor = Color.white;
        public static Color gSnowColorDark = Color.black;

        public static Vector4 gRipperData = Vector4.zero;
        public static Vector4 gNoiseScale = Vector4.one;

        public static Vector4 gWindParam = Vector4.zero;        //风的参数 xz风向 y风速 w强度
        public static float gWindDensity = 1;                   //风的参数 xz风向 y风速 w强度

        public static float gWaterPlaneHeightWS { get; set; } = 0f; //海平面高度

        //#if DEBUG_MODE
        public static bool gWeatherDebug { get; set; } = false;
        public static bool gOneSeasonWeatherDebug { get; set; } = false;
        public static float fSpeed { get; set; } = 0;
        //#endif
    }
}
