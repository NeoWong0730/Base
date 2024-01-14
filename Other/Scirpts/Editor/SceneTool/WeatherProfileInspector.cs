using Framework;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeatherProfile))]
public class WeatherProfileInspector : Editor
{
    class Styles
    {
        public static readonly GUIContent WeatherType =
            new GUIContent("天气类型", "WeatherProfile.eWeatherType");

        public static readonly GUIContent AmbientSkyColor =
            new GUIContent("环境光颜色", "WeatherProfile.AmbientSkyColor");

        public static readonly GUIContent LightColor =
            new GUIContent("日（月）光颜色", "WeatherProfile.LightColor");

        public static readonly GUIContent LightIntensity =
            new GUIContent("日（月）光强度", "WeatherProfile.LightIntensity");

        public static readonly GUIContent ShadowStrength =
            new GUIContent("阴影强度", "WeatherProfile.ShadowStrength");

        public static readonly GUIContent WindRotation =
            new GUIContent("风向", "WeatherProfile.WindRotation 实际更改 WeatherProfile.fWindDir");

        public static readonly GUIContent WindSpeed =
            new GUIContent("风速", "WeatherProfile.WindSpeed");

        public static readonly GUIContent WindForce =
            new GUIContent("风力", "WeatherProfile.WindForce");

        public static readonly GUIContent WindDensity =
            new GUIContent("波的密度（风）", "WeatherProfile.WindDensity");

        public static GUIStyle BoldLabelSimple;

        static Styles()
        {
            BoldLabelSimple = new GUIStyle(EditorStyles.label);
            BoldLabelSimple.fontStyle = FontStyle.Bold;
        }
    }

    public override void OnInspectorGUI()
    {
        WeatherProfile weatherProfile = target as WeatherProfile;

        weatherProfile.eWeatherType = (EWeatherType)EditorGUILayout.EnumPopup(Styles.WeatherType, weatherProfile.eWeatherType);
        EditorGUILayout.Space();

        weatherProfile.AmbientSkyColor = EditorGUILayout.GradientField(Styles.AmbientSkyColor, weatherProfile.AmbientSkyColor);
        weatherProfile.LightColor = EditorGUILayout.GradientField(Styles.LightColor, weatherProfile.LightColor);
        weatherProfile.fLightIntensity = EditorGUILayout.CurveField(Styles.LightIntensity, weatherProfile.fLightIntensity);
        weatherProfile.fShadowStrength = EditorGUILayout.CurveField(Styles.ShadowStrength, weatherProfile.fShadowStrength);
        EditorGUILayout.Space();

        weatherProfile.fWindRotation = EditorGUILayout.FloatField(Styles.WindRotation, weatherProfile.fWindRotation);
        weatherProfile.fWindSpeed = EditorGUILayout.FloatField(Styles.WindSpeed, weatherProfile.fWindSpeed);
        weatherProfile.fWindForce = EditorGUILayout.FloatField(Styles.WindForce, weatherProfile.fWindForce);
        weatherProfile.WindDensity = EditorGUILayout.FloatField(Styles.WindDensity, weatherProfile.WindDensity);

        Vector3 dir = Quaternion.AngleAxis(weatherProfile.fWindRotation, Vector3.up) * Vector3.right;
        weatherProfile.fWindDir = new Vector2(dir.x, dir.z);
    }
}