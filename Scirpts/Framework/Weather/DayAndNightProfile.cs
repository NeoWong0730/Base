using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class DayAndNightProfile : ScriptableObject
    {
        //[NaughtyAttributes.MinMaxSlider(0, 1)]
        public Vector2 lightMapTimeStage;

        public AnimationCurve fDayTimeCurve = new AnimationCurve();     //̫���߶�    

        [Range(0, 360)]
        public float nSunDirOffset = 0f;
        public AnimationCurve fSunHeight = new AnimationCurve();        //̫���߶�

        [Space()]
        public float fFlashInterval = 5f;                               //���粥�ż��
        public float fFlashDuration = 1f;                               //�������ʱ��
        public AnimationCurve fFlashIntensity = new AnimationCurve();   //����ǿ�ȱ仯

        [Space()]
        public float _SnowNoiseScale = 1;
        public float fSnowCoverageTime = 0.1f;                          //��ѩ�ٶ�
        [Range(0, 1)]
        public float fSnowRatio = 0.5f;                                 //��ѩ������
        [Range(0, 1)]
        public float fSnowHeight = 0.25f;                               //��ѩ�߶�    
        public Color snowColor = Color.white;                           //ѩ������ɫ
        public Color snowColorDark = Color.black;                       //ѩ������ɫ

        [Space()]
        public float _RippleNoiseScale = 1;
        //[NaughtyAttributes.MinMaxSlider(-1, 2)]
        public Vector2 _RippleMaskLevels = new Vector2(0, 1);
        //[NaughtyAttributes.MinMaxSlider(-1, 2)]
        public Vector2 _PuddleMaskLevels = new Vector2(0, 1);

        [Space()]
        public Texture weatherTexture = null;                           //������ͼ

        [HideInInspector] public List<WeatherProfile> mWeatherProfiles = new List<WeatherProfile>();
    }
}