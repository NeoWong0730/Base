using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class DayAndNightProfile : ScriptableObject
    {
        //[NaughtyAttributes.MinMaxSlider(0, 1)]
        public Vector2 lightMapTimeStage;

        public AnimationCurve fDayTimeCurve = new AnimationCurve();     //太阳高度    

        [Range(0, 360)]
        public float nSunDirOffset = 0f;
        public AnimationCurve fSunHeight = new AnimationCurve();        //太阳高度

        [Space()]
        public float fFlashInterval = 5f;                               //闪电播放间隔
        public float fFlashDuration = 1f;                               //闪电持续时间
        public AnimationCurve fFlashIntensity = new AnimationCurve();   //闪电强度变化

        [Space()]
        public float _SnowNoiseScale = 1;
        public float fSnowCoverageTime = 0.1f;                          //积雪速度
        [Range(0, 1)]
        public float fSnowRatio = 0.5f;                                 //积雪覆盖率
        [Range(0, 1)]
        public float fSnowHeight = 0.25f;                               //积雪高度    
        public Color snowColor = Color.white;                           //雪亮部颜色
        public Color snowColorDark = Color.black;                       //雪暗部颜色

        [Space()]
        public float _RippleNoiseScale = 1;
        //[NaughtyAttributes.MinMaxSlider(-1, 2)]
        public Vector2 _RippleMaskLevels = new Vector2(0, 1);
        //[NaughtyAttributes.MinMaxSlider(-1, 2)]
        public Vector2 _PuddleMaskLevels = new Vector2(0, 1);

        [Space()]
        public Texture weatherTexture = null;                           //天气贴图

        [HideInInspector] public List<WeatherProfile> mWeatherProfiles = new List<WeatherProfile>();
    }
}