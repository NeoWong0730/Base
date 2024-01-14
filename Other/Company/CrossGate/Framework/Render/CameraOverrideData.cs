using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class CameraOverrideData : MonoBehaviour
    {
        public bool useWeather = true;
        public bool ignoreWeatherFx = false;

        public bool shadowDistanceOverride = false;
        public float shadowDistance = 0;

        public bool ambientSkyColorOverride = false;
        public Color ambientSkyColor = Color.white;

        public bool maxAdditionalLightsCountOverride = true;

        public UniversalAdditionalCameraData additionalCameraData { get; private set; }
        private void Awake()
        {
            if (TryGetComponent<Camera>(out Camera camera))
            {
                camera.cullingMask &= (~((int)ELayerMask.UI));
                if (useWeather && !ignoreWeatherFx)
                {                    
                    camera.cullingMask |= (int)ELayerMask.WeatherFX;
                }
                else
                {
                    camera.cullingMask &= (~((int)ELayerMask.WeatherFX));
                }

                additionalCameraData = camera.GetUniversalAdditionalCameraData();
            }
        }
    }
}