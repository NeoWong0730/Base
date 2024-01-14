using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class DayAndNightDynamic : MonoBehaviour
    {
        [Header("Light Setting")]
        [SerializeField] private Light mSceneLight = null;
        private Transform mSceneLightTransform = null;

        [Header("Light Map")]
        [SerializeField] public Terrain mTerrain = null;
        [SerializeField] public Texture mTerrainLightMap = null;
        [SerializeField] private GameObject[] mNightLights = null;

        [SerializeField] public float fWaterPlaneHeightWS = -1.2f;

        private int nNightLightProcessIndex = 0;
        private int nNightLightCount = 0;

        private bool hasSceneLight = false;

        private bool forceRefreshLight = false;
        private bool bLightOn = true;
        private EDayStage eDayStage = EDayStage.Invalid;
        private ESeasonStage eSeasonStage = ESeasonStage.Invalid;
        private EWeatherType eWeatherType = EWeatherType.Sunny;

        public float fSnowCoverageFactor = 1f;

        [Header("Profile")]
        [Space(10)]
        public DayAndNightProfile mProfile = null;

#if UNITY_EDITOR
        public void SetNightLights(GameObject[] nightLights)
        {
            mNightLights = nightLights;
        }

        public void SetMainLights(Light sceneLight)
        {
            mSceneLight = sceneLight;
        }
#endif

        private void Awake()
        {
            nNightLightCount = mNightLights != null ? mNightLights.Length : 0;

            if (mSceneLight)
            {
                mSceneLightTransform = mSceneLight.transform;
                mSceneLight.cullingMask = (int)(ELayerMask.Build | ELayerMask.Default | ELayerMask.Grass | ELayerMask.SmallObject | ELayerMask.Terrain | ELayerMask.Tree | ELayerMask.Water
                    | ELayerMask.Monster | ELayerMask.Player);
                hasSceneLight = true;
            }
        }

        private void OnEnable()
        {
            //TODO： 不知道是不是bug 当开启SRPBatcher的时候， 关闭bUsageLightProbes 会导致闪烁不止
            //TODO： 在开关的瞬间还是会闪一下 干脆在天气系统关掉
            //UnityEngine.Rendering.Universal.UniversalRenderPipeline.asset.useSRPBatcher = false;
            ++WeatherSystem.gUseWeather;
            //TODO: 开关相机特效 可以特殊处理
            if (WeatherSystem.OnWeatherTypeChange != null)
            {
                WeatherSystem.OnWeatherTypeChange(eWeatherType, WeatherSystem.gWeatherType);
            }

            //每次打开的时候进行灯光的刷新，直接刷新所有灯光
            forceRefreshLight = true;

            if (mProfile != null)
            {
                WeatherSystem.gWaterPlaneHeightWS = fWaterPlaneHeightWS;
                WeatherSystem.gNoiseScale = new Vector4(mProfile._RippleNoiseScale, mProfile._SnowNoiseScale, 1, 1);
                //WeatherSystem.gRipperData = new Vector4(mProfile._RippleMaskLevels.x, mProfile._RippleMaskLevels.y, mProfile._PuddleMaskLevels.x, mProfile._PuddleMaskLevels.y);
            }
        }

        private void OnDisable()
        {
            --WeatherSystem.gUseWeather;
            WeatherSystem.gEmissiveLerp = 0;
            //TODO: 开关相机特效 可以特殊处理
            if (WeatherSystem.OnWeatherTypeChange != null)
            {
                WeatherSystem.OnWeatherTypeChange(eWeatherType, WeatherSystem.gWeatherType);
            }
        }

        void Update()
        {
            if (mProfile == null)
                return;

            WeatherSystem.gWeatherTexture = mProfile.weatherTexture;

            //if (WeatherSystem.gWeatherType != EWeatherType.Thunderstorm && Time.frameCount % 4 != 0)
            //    return;
            //if (WeatherSystem.gSeasonStage != eSeasonStage)
            //{
            //    if (WeatherSystem.OnSeasonStageChange != null)
            //    {
            //        WeatherSystem.OnSeasonStageChange(eSeasonStage, WeatherSystem.gSeasonStage);
            //    }
            //    eSeasonStage = WeatherSystem.gSeasonStage;
            //}
            //
            //if (WeatherSystem.gWeatherType != eWeatherType)
            //{
            //    if (WeatherSystem.OnWeatherTypeChange != null)
            //    {
            //        WeatherSystem.OnWeatherTypeChange(eWeatherType, WeatherSystem.gWeatherType);
            //    }
            //    eWeatherType = WeatherSystem.gWeatherType;
            //}
            //
            //if (WeatherSystem.gDayStage != eDayStage)
            //{
            //    if (WeatherSystem.OnDayNightStageChange != null)
            //    {
            //        WeatherSystem.OnDayNightStageChange(eDayStage, WeatherSystem.gDayStage);
            //    }
            //    eDayStage = WeatherSystem.gDayStage;
            //}
            eSeasonStage = WeatherSystem.gSeasonStage;
            eWeatherType = WeatherSystem.gWeatherType;
            eDayStage = WeatherSystem.gDayStage;
#if DEBUG_MODE
            if (WeatherSystem.gWeatherDebug)
            {
                WeatherSystem.gDayProgress += Time.unscaledDeltaTime * WeatherSystem.fSpeed;
                WeatherSystem.gDayProgress = WeatherSystem.gDayProgress - (int)WeatherSystem.gDayProgress;
            }
#endif
#if UNITY_EDITOR
            WeatherSystem.gNoiseScale = new Vector4(mProfile._RippleNoiseScale, mProfile._SnowNoiseScale, 1, 1);
#endif

            float seasonProgress = WeatherSystem.gSeasonProgress;
            float dayProgress = mProfile.fDayTimeCurve.Evaluate(WeatherSystem.gDayProgress);
            float weatherProgress = WeatherSystem.gWeatherProgress;

            //WeatherSystem.gEmissiveLerp = (dayProgress > mProfile.emissiveTimeStage.x && dayProgress < mProfile.emissiveTimeStage.y) ? 1 : 0;

            bool needLightOn = dayProgress > mProfile.lightMapTimeStage.x && dayProgress < mProfile.lightMapTimeStage.y;
            if (needLightOn != bLightOn)
            {
                //TODO:实时不使用lightMap
                //SetLightMapActive(needLightOn);
                bLightOn = needLightOn;
                nNightLightProcessIndex = 0;
            }
            if (forceRefreshLight)
            {
                ProcessNightLight(needLightOn, nNightLightCount);
                forceRefreshLight = false;
            }
            else
            {
                ProcessNightLight(needLightOn, 10);
            }
            int emissiveLerp = needLightOn && nNightLightProcessIndex == nNightLightCount ? 1 : 0;
            WeatherSystem.gEmissiveLerp = Mathf.Lerp(WeatherSystem.gEmissiveLerp, emissiveLerp, 0.1f);

            WeatherProfile fromWeather = mProfile.mWeatherProfiles[(int)WeatherSystem.gWeatherTypeBefore - 1];
            WeatherProfile toWeather = mProfile.mWeatherProfiles[(int)WeatherSystem.gWeatherType - 1];

            UpdateSnow(weatherProgress);

            float weatherFade = 1;
            if (toWeather.eWeatherType == EWeatherType.Snow)
            {
                weatherFade = Mathf.Clamp01(WeatherSystem.gSnowCoverageRate * 4f);
            }
            else if (fromWeather.eWeatherType == EWeatherType.Snow)
            {
                weatherFade = Mathf.Clamp01((weatherProgress - WeatherSystem.gSnowCoverageRate) * 4);
            }
            else
            {
                weatherFade = Mathf.Clamp01(weatherProgress * 4);
            }

            UpdateRain(weatherFade);

            WeatherSystem.gWindParam = new Vector4(toWeather.fWindDir.x, toWeather.fWindSpeed, toWeather.fWindDir.y, toWeather.fWindForce);
            WeatherSystem.gWindDensity = toWeather.WindDensity;

            UpdateLightEffect(dayProgress, fromWeather, toWeather, weatherFade);
            UpdateFlashing();
        }

        private void UpdateRain(float weatherFade)
        {
            float startValue = WeatherSystem.gWeatherTypeBefore == EWeatherType.Rain || WeatherSystem.gWeatherTypeBefore == EWeatherType.Thunderstorm ? 1 : 0;
            float endValue = WeatherSystem.gWeatherType == EWeatherType.Rain || WeatherSystem.gWeatherType == EWeatherType.Thunderstorm ? 1 : 0;

            if (startValue == endValue)
            {
                WeatherSystem.gRainCoverageRate = endValue;
            }
            else
            {
                WeatherSystem.gRainCoverageRate = Mathf.Lerp(startValue, endValue, weatherFade);
            }

            if (WeatherSystem.gRainCoverageRate > 0f)
            {
                WeatherSystem.gRipperData = new Vector4(mProfile._RippleMaskLevels.x, mProfile._RippleMaskLevels.y * WeatherSystem.gRainCoverageRate * endValue, mProfile._PuddleMaskLevels.x, mProfile._PuddleMaskLevels.y * WeatherSystem.gRainCoverageRate);
            }
        }

        private void UpdateSnow(float weatherProgress)
        {
            if (fSnowCoverageFactor < 0.001f)
            {
                WeatherSystem.gSnowCoverageRate = 0;
                return;
            }

            float startValue = 0;
            if (WeatherSystem.gWeatherTypeBefore == EWeatherType.Snow)
            {
                startValue = 1;
            }

            if (eWeatherType == EWeatherType.Snow)
            {
                if (mProfile.fSnowCoverageTime > 0)
                {
                    WeatherSystem.gSnowCoverageRate = startValue + weatherProgress / mProfile.fSnowCoverageTime;
                    if (WeatherSystem.gSnowCoverageRate > 1)
                    {
                        WeatherSystem.gSnowCoverageRate = 1;
                    }
                }
                else
                {
                    WeatherSystem.gSnowCoverageRate = 1;
                }
            }
            else
            {
                if (mProfile.fSnowCoverageTime > 0)
                {
                    WeatherSystem.gSnowCoverageRate = startValue - weatherProgress / mProfile.fSnowCoverageTime;
                    if (WeatherSystem.gSnowCoverageRate < 0)
                    {
                        WeatherSystem.gSnowCoverageRate = 0;
                    }
                }
                else
                {
                    WeatherSystem.gSnowCoverageRate = 0;
                }
            }

            WeatherSystem.gSnowCoverageRate = WeatherSystem.gSnowCoverageRate * fSnowCoverageFactor;

            if (WeatherSystem.gSnowCoverageRate > 0)
            {
                WeatherSystem.gSnowRatio = mProfile.fSnowRatio;
                WeatherSystem.gSnowHeight = mProfile.fSnowHeight;
                WeatherSystem.gSnowColor = mProfile.snowColor;
                WeatherSystem.gSnowColorDark = mProfile.snowColorDark;
            }

#if UNITY_EDITOR
            WeatherSystem.gWaterPlaneHeightWS = fWaterPlaneHeightWS;
#endif
        }

        private void UpdateFlashing()
        {
            if (!hasSceneLight)
                return;

            if (WeatherSystem.gWeatherType != EWeatherType.Thunderstorm || mProfile.fFlashDuration <= 0)
                return;

            WeatherSystem.gFlashStageTime += Time.deltaTime;
            if (WeatherSystem.gFlashing)
            {
                float v = Mathf.Clamp01(WeatherSystem.gFlashStageTime / mProfile.fFlashDuration);
                float vf = mProfile.fFlashIntensity.Evaluate(v);
                if (mSceneLight.intensity < vf)
                {
                    mSceneLight.intensity = vf;
                    mSceneLight.color = Color.white;
                    mSceneLight.shadowStrength = 1;
                    //RenderSettings.ambientSkyColor = Color.white;
                    WeatherSystem.gAmbientSH = Color.white;
                }

                if (WeatherSystem.gFlashStageTime >= mProfile.fFlashDuration)
                {
                    WeatherSystem.gFlashStageTime = 0;
                    WeatherSystem.gFlashing = false;
                }
            }
            else
            {
                if (WeatherSystem.gFlashStageTime > mProfile.fFlashInterval)
                {
                    WeatherSystem.gFlashStageTime = 0;
                    WeatherSystem.gFlashing = true;

                    //开始打雷
                    WeatherSystem.OnFlashing?.Invoke();
                }
            }
        }

        private void UpdateLightEffect(float fade, WeatherProfile from, WeatherProfile to, float lerp)
        {
            //环境颜色变更
            Color ambientSkyColor = to.AmbientSkyColor.Evaluate(fade);
            if (lerp < 0.9999)
            {
                ambientSkyColor = Color.Lerp(from.AmbientSkyColor.Evaluate(fade), ambientSkyColor, lerp);
            }
            //RenderSettings.ambientSkyColor = ambientSkyColor;
            WeatherSystem.gAmbientSH = ambientSkyColor;

            if (hasSceneLight)
            {
                //太阳角度变更
                float rx = 90 * Mathf.Clamp01(mProfile.fSunHeight.Evaluate(fade));
                float ry = fade * 360f + mProfile.nSunDirOffset;
                Vector3 sunAngle = new Vector3(rx, ry, 0);

                Color LightColor = to.LightColor.Evaluate(fade);
                float shadowStrength = Mathf.Clamp01(to.fShadowStrength.Evaluate(fade));
                float sceneLightIntensity = Mathf.Max(0, to.fLightIntensity.Evaluate(fade));

                if (lerp < 0.9999)
                {
                    LightColor = Color.Lerp(from.LightColor.Evaluate(fade), LightColor, lerp);
                    shadowStrength = mSceneLight.shadowStrength = Mathf.Clamp01(Mathf.Lerp(from.fShadowStrength.Evaluate(fade), shadowStrength, lerp));
                    sceneLightIntensity = Mathf.Max(0, Mathf.Lerp(from.fLightIntensity.Evaluate(fade), sceneLightIntensity, lerp));
                }

                mSceneLightTransform.eulerAngles = sunAngle;
                mSceneLight.color = LightColor;
                mSceneLight.shadowStrength = shadowStrength;
                mSceneLight.intensity = sceneLightIntensity;
            }
        }

        private void ProcessNightLight(bool use, int count)
        {
            int i = 0;
            while (nNightLightProcessIndex < nNightLightCount && i < count)
            {
                mNightLights[nNightLightProcessIndex].SetActive(use);
                ++nNightLightProcessIndex;
                ++i;
            }
        }
    }
}