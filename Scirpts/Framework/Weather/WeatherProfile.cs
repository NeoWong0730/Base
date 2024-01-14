using UnityEngine;

namespace Framework
{
    public class WeatherProfile : ScriptableObject
    {
        [HideInInspector]
        public EWeatherType eWeatherType = EWeatherType.Sunny;

        [HideInInspector]
        public Gradient AmbientSkyColor = new Gradient();                   //��������ɫ
        [HideInInspector]
        public Gradient LightColor = new Gradient();                        //����Դ��ɫ
        [HideInInspector]
        public AnimationCurve fLightIntensity = new AnimationCurve();       //����Դǿ��    
        [HideInInspector]
        public AnimationCurve fShadowStrength = new AnimationCurve();       //��Ӱǿ��    

        [HideInInspector]
        public Vector2 fWindDir = Vector2.zero;
        [HideInInspector]
        public float fWindSpeed = 0f;
        [HideInInspector]
        public float fWindForce = 0f;
        [HideInInspector]
        public float WindDensity = 1f;
#if UNITY_EDITOR
        [HideInInspector]
        public float fWindRotation = 0f;
#endif
    }
}