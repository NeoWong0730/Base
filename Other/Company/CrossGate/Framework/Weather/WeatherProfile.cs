using UnityEngine;

public class WeatherProfile : ScriptableObject
{
    [HideInInspector]
    public EWeatherType eWeatherType = EWeatherType.Sunny;

    [HideInInspector]
    public Gradient AmbientSkyColor = new Gradient();                   //环境光颜色
    [HideInInspector]
    public Gradient LightColor = new Gradient();                        //主光源颜色
    [HideInInspector]
    public AnimationCurve fLightIntensity = new AnimationCurve();       //主光源强度    
    [HideInInspector]
    public AnimationCurve fShadowStrength = new AnimationCurve();       //阴影强度    

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