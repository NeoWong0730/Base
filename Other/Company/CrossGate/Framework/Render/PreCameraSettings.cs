using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework
{
    public class PreCameraSettings : MonoBehaviour
    {
        private string passName = "PreCamera";
        private string passClearName = "PreCameraClear";        

        public Transform weatherFxRoot;
        public GameObject rainFX;
        public GameObject snowFX;
        
        private bool bPlayRainFX;
        private bool bPlaySnowFX;

        private float shadowDistanceBackUp = 0;
        private int maxAdditionalLightsCountBackUp = 0;

        private bool supportsDepthCopy;

        private void Start()
        {            
            supportsDepthCopy = RenderExtensionSetting.SupportsDepthCopy();
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;

            if(rainFX != null)
            {
                rainFX.SetActive(bPlayRainFX);
            }
            if(snowFX != null)
            {
                snowFX.SetActive(bPlaySnowFX);
            }            
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
#if UNITY_EDITOR
            CameraOverrideData cameraOverrideData;
            if ((camera.cameraType == CameraType.SceneView && Camera.main != null && Camera.main.TryGetComponent(out cameraOverrideData))
                || camera.TryGetComponent(out cameraOverrideData))
#else
            if (camera.TryGetComponent(out CameraOverrideData cameraOverrideData))
#endif
            {
                bool useWeather = WeatherSystem.gUseWeather > 0 && cameraOverrideData.useWeather;
                //天气特效设置
                _RerfeshFX(useWeather, camera);

                CommandBuffer cmd = CommandBufferPool.Get(passName);

                //TODO:useWeather指代是世界场景
                if (!cameraOverrideData.ignoreWeatherFx && cameraOverrideData.useWeather && supportsDepthCopy && RenderExtensionSetting.UseDepthTexture)
                {
                    cameraOverrideData.additionalCameraData.requiresColorOption = CameraOverrideOption.On;
                    cameraOverrideData.additionalCameraData.requiresDepthOption = CameraOverrideOption.On;
                    cmd.EnableShaderKeyword(Consts._DEPTHMAP_ON_Keyword);
                    cmd.EnableShaderKeyword(Consts._OPAQUE_TEXTURE_ON_Keyword);
                }
                else
                {
                    cameraOverrideData.additionalCameraData.requiresColorOption = CameraOverrideOption.Off;
                    cameraOverrideData.additionalCameraData.requiresDepthOption = CameraOverrideOption.Off;
                    cmd.DisableShaderKeyword(Consts._DEPTHMAP_ON_Keyword);
                    cmd.DisableShaderKeyword(Consts._OPAQUE_TEXTURE_ON_Keyword);
                }

                //阴影距离设置
                if (cameraOverrideData.shadowDistanceOverride)
                {
                    shadowDistanceBackUp = UniversalRenderPipeline.asset.shadowDistance;
                    UniversalRenderPipeline.asset.shadowDistance = cameraOverrideData.shadowDistance;
                }

                //点光源数量设置
                if (cameraOverrideData.maxAdditionalLightsCountOverride)
                {
                    maxAdditionalLightsCountBackUp = UniversalRenderPipeline.asset.maxAdditionalLightsCount;
                    UniversalRenderPipeline.asset.maxAdditionalLightsCount = 0;
                }

                //环境光的设置
                if (cameraOverrideData.ambientSkyColorOverride)
                {
                    cmd.SetGlobalColor(Consts._AmbientSH_ID, cameraOverrideData.ambientSkyColor);
                }
                else if (useWeather)
                {
                    cmd.SetGlobalColor(Consts._AmbientSH_ID, WeatherSystem.gAmbientSH);
                }
                else
                {
                    SphericalHarmonicsL2 ambientSH;
                    LightProbes.GetInterpolatedProbe(Vector3.zero, null, out ambientSH);
                    cmd.SetGlobalColor(Consts._AmbientSH_ID, new Color(ambientSH[0, 0], ambientSH[1, 0], ambientSH[2, 0], 1));
                }

                //天气相关的数据设置
                if (useWeather)
                {
                    cmd.SetGlobalVector(Consts.ID_RipperData, WeatherSystem.gRipperData);
                    cmd.SetGlobalVector(Consts.ID_NoiseScale, WeatherSystem.gNoiseScale);
                    cmd.SetGlobalVector(Consts.ID_WindParam, WeatherSystem.gWindParam);
                    cmd.SetGlobalFloat(Consts.ID_WindDensity, WeatherSystem.gWindDensity);

                    if (WeatherSystem.gWeatherTexture != null)
                    {
                        //if (WeatherSystem.gWeatherType == EWeatherType.Rain || WeatherSystem.gWeatherType == EWeatherType.Thunderstorm || WeatherSystem.gSnowCoverageRate > 0)
                        //{
                        //    cmd.SetGlobalTexture(Consts.ID_WeatherTexture, WeatherSystem.gWeatherTexture);
                        //}
                        cmd.SetGlobalTexture(Consts.ID_WeatherTexture, WeatherSystem.gWeatherTexture);
                    }

                    if (WeatherSystem.gSnowCoverageRate > 0)
                    {
                        cmd.EnableShaderKeyword(Consts._SNOW_ON_Keyword);
                        
                        cmd.SetGlobalColor(Consts.ID_SnowColor, WeatherSystem.gSnowColor);
                        cmd.SetGlobalColor(Consts.ID_SnowColorDark, WeatherSystem.gSnowColorDark);                        
                    }
                    else if (WeatherSystem.gRainCoverageRate > 0)
                    {
                        cmd.EnableShaderKeyword(Consts._RIPPLE_ON_Keyword);
                    }

                    //cmd.SetGlobalFloat(Consts.ID_SnowFade, WeatherSystem.gSnowCoverageRate);
                    //cmd.SetGlobalFloat(Consts.ID_WaterHeight, RenderExtensionSetting.fWaterPlaneHeightWS);                    
                    //cmd.SetGlobalFloat(Consts.ID_SnowRatio, WeatherSystem.gSnowRatio);
                    //cmd.SetGlobalFloat(Consts.ID_SnowHeight, WeatherSystem.gSnowHeight);

                    cmd.SetGlobalVector(Consts.ID_SnowData, new Vector4(WeatherSystem.gSnowCoverageRate, WeatherSystem.gWaterPlaneHeightWS, WeatherSystem.gSnowRatio, WeatherSystem.gSnowHeight));
                    cmd.SetGlobalFloat(Consts.ID_NightSchedule, WeatherSystem.gEmissiveLerp);
                }
                else
                {
                    cmd.SetGlobalVector(Consts.ID_SnowData, Vector4.zero);
                    cmd.SetGlobalFloat(Consts.ID_NightSchedule, 0);
                }

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }
        }

        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
#if UNITY_EDITOR
            CameraOverrideData cameraOverrideData;
            if ((camera.cameraType == CameraType.SceneView && Camera.main != null && Camera.main.TryGetComponent(out cameraOverrideData))
                || camera.TryGetComponent(out cameraOverrideData))
#else
            if (camera.TryGetComponent(out CameraOverrideData cameraOverrideData))
#endif
            {
                CommandBuffer cmd = CommandBufferPool.Get(passClearName);
                cmd.DisableShaderKeyword(Consts._SNOW_ON_Keyword);
                cmd.DisableShaderKeyword(Consts._RIPPLE_ON_Keyword);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);

                if (cameraOverrideData.shadowDistanceOverride)
                {
                    UniversalRenderPipeline.asset.shadowDistance = shadowDistanceBackUp;
                }

                if (cameraOverrideData.maxAdditionalLightsCountOverride)
                {
                    UniversalRenderPipeline.asset.maxAdditionalLightsCount = maxAdditionalLightsCountBackUp;
                }
            }
        }

        private void _RerfeshFX(bool useWeather, Camera camera)
        {
            bool needRainFX = false;
            bool needSnowFX = false;

            if (useWeather)
            {
                needRainFX = WeatherSystem.gWeatherType == EWeatherType.Rain || WeatherSystem.gWeatherType == EWeatherType.Thunderstorm;
                needSnowFX = WeatherSystem.gWeatherType == EWeatherType.Snow;
            }

            if (needRainFX != bPlayRainFX)
            {
                if(rainFX != null)
                {
                    rainFX.SetActive(needRainFX);
                }                
                bPlayRainFX = needRainFX;
            }

            if (needSnowFX != bPlaySnowFX)
            {
                if(snowFX != null)
                {
                    snowFX.SetActive(needSnowFX);
                }                
                bPlaySnowFX = needSnowFX;
            }

            if (weatherFxRoot != null && (bPlayRainFX || bPlaySnowFX))
            {
                Transform trans = camera.transform;
                weatherFxRoot.SetPositionAndRotation(trans.position, trans.rotation);
            }
        }
    }
}