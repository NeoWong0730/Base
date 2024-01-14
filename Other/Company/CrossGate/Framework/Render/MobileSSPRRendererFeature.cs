//see README here: https://github.com/ColinLeung-NiloCat/UnityURP-MobileScreenSpacePlanarReflection

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MobileSSPRRendererFeature : ScriptableRendererFeature
{
    //public static MobileSSPRRendererFeature instance; //for example scene to call, user should add 1 and not more than 1 MobileSSPRRendererFeature anyway so it is safe to use static ref
    [System.Serializable]
    public class PassSettings
    {
        [Header("Settings")]
        public bool ShouldRenderSSPR = true;
        //public float HorizontalReflectionPlaneHeightWS = 0.01f; //default higher than ground a bit, to avoid ZFighting if user placed a ground plane at y=0
        [Range(0.01f, 1f)]
        public float FadeOutScreenBorderWidthVerticle = 0.25f;
        [Range(0.01f, 1f)]
        public float FadeOutScreenBorderWidthHorizontal = 0.35f;
        [Range(0, 8f)]
        public float ScreenLRStretchIntensity = 4;
        [Range(-1f, 1f)]
        public float ScreenLRStretchThreshold = 0.7f;
        [ColorUsage(true, true)]
        public Color TintColor = Color.white;

        //////////////////////////////////////////////////////////////////////////////////
        [Header("Performance Settings")]
        [Range(128, 1024)]
        [Tooltip("set to 512 or below for better performance, if visual quality lost is acceptable")]
        public int RT_height = 512;
        [Tooltip("can set to false for better performance, if visual quality lost is acceptable")]
        public bool UseHDR = true;
        [Tooltip("can set to false for better performance, if visual quality lost is acceptable")]
        public bool ApplyFillHoleFix = true;
        [Tooltip("can set to false for better performance, if flickering is acceptable")]
        public bool ShouldRemoveFlickerFinalControl = true;

        //////////////////////////////////////////////////////////////////////////////////
        [Header("Danger Zone")]
        [Tooltip("You should always turn this on, unless you want to debug")]
        public bool EnablePerPlatformAutoSafeGuard = true;

        public ComputeShader mComputeShader;
    }
    public PassSettings Settings = new PassSettings();

    public class CustomRenderPass : ScriptableRenderPass
    {
        const string m_ProfilerTag = "Screen Space Planar Reflection";
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);

        readonly string _SSPR_ON_Keyword = "_SSPR_ON";

        readonly int PID_RTSize = Shader.PropertyToID("_RTSize");
        readonly int PID_HorizontalPlaneHeightWS = Shader.PropertyToID("_HorizontalPlaneHeightWS");
        readonly int PID_FadeOutScreenBorderWidthVerticle = Shader.PropertyToID("_FadeOutScreenBorderWidthVerticle");
        readonly int PID_FadeOutScreenBorderWidthHorizontal = Shader.PropertyToID("_FadeOutScreenBorderWidthHorizontal");
        readonly int PID_CameraDirection = Shader.PropertyToID("_CameraDirection");
        readonly int PID_ScreenLRStretchIntensity = Shader.PropertyToID("_ScreenLRStretchIntensity");
        readonly int PID_ScreenLRStretchThreshold = Shader.PropertyToID("_ScreenLRStretchThreshold");
        readonly int PID_FinalTintColor = Shader.PropertyToID("_FinalTintColor");

        readonly string PID_CameraOpaqueTexture = "_CameraOpaqueTexture";
        readonly string PID_CameraDepthTexture = "_CameraDepthTexture";

        readonly int PID_PlanarReflectionTexture = Shader.PropertyToID("_PlanarReflectionTexture");
        readonly int PID_HashTexture = Shader.PropertyToID("_HashTexture");
        readonly int PID_PosWSyTexture = Shader.PropertyToID("_PosWSyTexture");

        readonly int PID_PlanarReflectionRWT = Shader.PropertyToID("ColorRT");
        readonly int PID_HashRWT = Shader.PropertyToID("HashRT");
        readonly int PID_PosWSyRWT = Shader.PropertyToID("PosWSyRT");

        //reflection plane renderer's material's shader must use this LightMode
        ShaderTagId STID_SSRP = new ShaderTagId("MobileSSPR");

        string KernelName_MobilePathSinglePassColorRTDirectResolve = "MobilePathSinglePassColorRTDirectResolve";
        string KernelName_NonMobilePathClear = "NonMobilePathClear";
        string KernelName_NonMobilePathRenderHashRT = "NonMobilePathRenderHashRT";
        string KernelName_NonMobilePathResolveColorRT = "NonMobilePathResolveColorRT";
        string KernelName_FillHoles = "FillHoles";

        //must match compute shader's [numthread(x)]
        const int SHADER_NUMTHREAD_X = 8;
        //must match compute shader's [numthread(y)]
        const int SHADER_NUMTHREAD_Y = 8;

        PassSettings settings;
        ComputeShader cs;
        public CustomRenderPass(PassSettings settings)
        {
            this.settings = settings;

            //(ComputeShader)Resources.Load("MobileSSPRComputeShader");
            cs = settings.mComputeShader;
        }

        int GetRTHeight()
        {
            return Mathf.CeilToInt(settings.RT_height / (float)SHADER_NUMTHREAD_Y) * SHADER_NUMTHREAD_Y;
        }

        int GetRTWidth()
        {
            float aspect = (float)Screen.width / Screen.height;
            return Mathf.CeilToInt(GetRTHeight() * aspect / (float)SHADER_NUMTHREAD_X) * SHADER_NUMTHREAD_X;
        }

        /// <summary>
        /// If user enabled PerPlatformAutoSafeGuard, this function will return true if we should use mobile path
        /// </summary>
        bool ShouldUseSinglePassUnsafeAllowFlickeringDirectResolve()
        {
            if (settings.EnablePerPlatformAutoSafeGuard)
            {
                //if RInt RT is not supported, use mobile path
                if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RInt))
                    return true;

                //tested Metal(even on a Mac) can't use InterlockedMin().
                //so if metal, use mobile path
                if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
                    return true;
#if UNITY_EDITOR
                //PC(DirectX) can use RenderTextureFormat.RInt + InterlockedMin() without any problem, use Non-Mobile path.
                //Non-Mobile path will NOT produce any flickering
                if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12)
                    return false;
#elif UNITY_ANDROID
                //- samsung galaxy A70(Adreno612) will fail if use RenderTextureFormat.RInt + InterlockedMin() in compute shader
                //- but Lenovo S5(Adreno506) is correct, WTF???
                //because behavior is different between android devices, we assume all android are not safe to use RenderTextureFormat.RInt + InterlockedMin() in compute shader
                //so android always go mobile path
                return true;
#endif
            }

            //let user decide if we still don't know the correct answer
            return !settings.ShouldRemoveFlickerFinalControl;
        }
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor rtd = new RenderTextureDescriptor(GetRTWidth(), GetRTHeight(), RenderTextureFormat.Default, 0, 0);

            rtd.sRGB = false; //don't need gamma correction when sampling these RTs, it is linear data already because it will be filled by screen's linear data
            rtd.enableRandomWrite = true; //using RWTexture2D in compute shader need to turn on this

            //color RT
            bool shouldUseHDRColorRT = settings.UseHDR;
            if (cameraTextureDescriptor.colorFormat == RenderTextureFormat.ARGB32)
                shouldUseHDRColorRT = false;// if there are no HDR info to reflect anyway, no need a HDR colorRT
            rtd.colorFormat = shouldUseHDRColorRT ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32; //we need alpha! (usually LDR is enough, ignore HDR is acceptable for reflection)
            cmd.GetTemporaryRT(PID_PlanarReflectionTexture, rtd);

            //PackedData RT
            if (ShouldUseSinglePassUnsafeAllowFlickeringDirectResolve())
            {
                //use unsafe method if mobile
                //posWSy RT (will use this RT for posWSy compare test, just like the concept of regular depth buffer)
                rtd.colorFormat = RenderTextureFormat.RFloat;
                cmd.GetTemporaryRT(PID_PosWSyTexture, rtd);
            }
            else
            {
                //use 100% correct method if console/PC
                rtd.colorFormat = RenderTextureFormat.RInt;
                cmd.GetTemporaryRT(PID_HashTexture, rtd);
            }
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                int dispatchThreadGroupXCount = GetRTWidth() / SHADER_NUMTHREAD_X; //divide by shader's numthreads.x
                int dispatchThreadGroupYCount = GetRTHeight() / SHADER_NUMTHREAD_Y; //divide by shader's numthreads.y
                int dispatchThreadGroupZCount = 1; //divide by shader's numthreads.z

                if (settings.ShouldRenderSSPR)
                {
                    cmd.SetComputeVectorParam(cs, PID_RTSize, new Vector2(GetRTWidth(), GetRTHeight()));
                    cmd.SetComputeFloatParam(cs, PID_HorizontalPlaneHeightWS, RenderExtensionSetting.fHorizontalReflectionPlaneHeightWS);
                    cmd.SetComputeFloatParam(cs, PID_FadeOutScreenBorderWidthVerticle, settings.FadeOutScreenBorderWidthVerticle);
                    cmd.SetComputeFloatParam(cs, PID_FadeOutScreenBorderWidthHorizontal, settings.FadeOutScreenBorderWidthHorizontal);
                    cmd.SetComputeVectorParam(cs, PID_CameraDirection, renderingData.cameraData.camera.transform.forward);
                    cmd.SetComputeFloatParam(cs, PID_ScreenLRStretchIntensity, settings.ScreenLRStretchIntensity);
                    cmd.SetComputeFloatParam(cs, PID_ScreenLRStretchThreshold, settings.ScreenLRStretchThreshold);
                    cmd.SetComputeVectorParam(cs, PID_FinalTintColor, settings.TintColor);

                    //we found that on metal, UNITY_MATRIX_VP is not correct, so we will pass our own VP matrix to compute shader
                    //Camera camera = renderingData.cameraData.camera;
                    //Matrix4x4 VP = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true) * camera.worldToCameraMatrix;
                    //cb.SetComputeMatrixParam(cs, "_VPMatrix", VP);

                    if (ShouldUseSinglePassUnsafeAllowFlickeringDirectResolve())
                    {
                        ////////////////////////////////////////////////
                        //Mobile Path (Android GLES / Metal)
                        ////////////////////////////////////////////////

                        //kernel MobilePathsinglePassColorRTDirectResolve
                        int kernel_MobilePathSinglePassColorRTDirectResolve = cs.FindKernel(KernelName_MobilePathSinglePassColorRTDirectResolve);
                        cmd.SetComputeTextureParam(cs, kernel_MobilePathSinglePassColorRTDirectResolve, PID_PlanarReflectionRWT, new RenderTargetIdentifier(PID_PlanarReflectionTexture));
                        cmd.SetComputeTextureParam(cs, kernel_MobilePathSinglePassColorRTDirectResolve, PID_PosWSyRWT, new RenderTargetIdentifier(PID_PosWSyTexture));
                        cmd.SetComputeTextureParam(cs, kernel_MobilePathSinglePassColorRTDirectResolve, PID_CameraOpaqueTexture, new RenderTargetIdentifier(PID_CameraOpaqueTexture));
                        cmd.SetComputeTextureParam(cs, kernel_MobilePathSinglePassColorRTDirectResolve, PID_CameraDepthTexture, new RenderTargetIdentifier(PID_CameraDepthTexture));
                        cmd.DispatchCompute(cs, kernel_MobilePathSinglePassColorRTDirectResolve, dispatchThreadGroupXCount, dispatchThreadGroupYCount, dispatchThreadGroupZCount);

                    }
                    else
                    {
                        ////////////////////////////////////////////////
                        //Non-Mobile Path (PC/console)
                        ////////////////////////////////////////////////
                        
                        //kernel NonMobilePathClear
                        int kernel_NonMobilePathClear = cs.FindKernel(KernelName_NonMobilePathClear);
                        cmd.SetComputeTextureParam(cs, kernel_NonMobilePathClear, PID_HashRWT, new RenderTargetIdentifier(PID_HashTexture));
                        cmd.SetComputeTextureParam(cs, kernel_NonMobilePathClear, PID_PlanarReflectionRWT, new RenderTargetIdentifier(PID_PlanarReflectionTexture));
                        cmd.DispatchCompute(cs, kernel_NonMobilePathClear, dispatchThreadGroupXCount, dispatchThreadGroupYCount, dispatchThreadGroupZCount);

                        //kernel NonMobilePathRenderHashRT
                        int kernel_NonMobilePathRenderHashRT = cs.FindKernel(KernelName_NonMobilePathRenderHashRT);
                        cmd.SetComputeTextureParam(cs, kernel_NonMobilePathRenderHashRT, PID_HashRWT, new RenderTargetIdentifier(PID_HashTexture));
                        cmd.SetComputeTextureParam(cs, kernel_NonMobilePathRenderHashRT, PID_CameraDepthTexture, new RenderTargetIdentifier(PID_CameraDepthTexture));

                        cmd.DispatchCompute(cs, kernel_NonMobilePathRenderHashRT, dispatchThreadGroupXCount, dispatchThreadGroupYCount, dispatchThreadGroupZCount);

                        //resolve to ColorRT
                        int kernel_NonMobilePathResolveColorRT = cs.FindKernel(KernelName_NonMobilePathResolveColorRT);
                        cmd.SetComputeTextureParam(cs, kernel_NonMobilePathResolveColorRT, PID_CameraOpaqueTexture, new RenderTargetIdentifier(PID_CameraOpaqueTexture));
                        cmd.SetComputeTextureParam(cs, kernel_NonMobilePathResolveColorRT, PID_PlanarReflectionRWT, new RenderTargetIdentifier(PID_PlanarReflectionTexture));
                        cmd.SetComputeTextureParam(cs, kernel_NonMobilePathResolveColorRT, PID_HashRWT, new RenderTargetIdentifier(PID_HashTexture));
                        cmd.DispatchCompute(cs, kernel_NonMobilePathResolveColorRT, dispatchThreadGroupXCount, dispatchThreadGroupYCount, dispatchThreadGroupZCount);
                    }
                    
                    //optional shared pass to improve result only: fill RT hole
                    if (settings.ApplyFillHoleFix)
                    {
                        int kernel_FillHoles = cs.FindKernel(KernelName_FillHoles);
                        cmd.SetComputeTextureParam(cs, kernel_FillHoles, PID_PlanarReflectionRWT, new RenderTargetIdentifier(PID_PlanarReflectionTexture));
                        //cb.SetComputeTextureParam(cs, kernel_FillHoles, "PackedDataRT", _SSPR_PackedDataRT_rti);
                        cmd.DispatchCompute(cs, kernel_FillHoles, Mathf.CeilToInt(dispatchThreadGroupXCount / 2f), Mathf.CeilToInt(dispatchThreadGroupYCount / 2f), dispatchThreadGroupZCount);
                    }

                    //send out to global, for user's shader to sample reflection result RT (_MobileSSPR_ColorRT)
                    //where _MobileSSPR_ColorRT's rgb is reflection color, a is reflection usage 0~1 for user's shader to lerp with fallback reflection probe's rgb
                    cmd.SetGlobalTexture(PID_PlanarReflectionTexture, new RenderTargetIdentifier(PID_PlanarReflectionTexture));
                    cmd.EnableShaderKeyword(_SSPR_ON_Keyword);
                }
                else
                {
                    //allow user to skip SSPR related code if disabled
                    cmd.DisableShaderKeyword(_SSPR_ON_Keyword);
                }
            }
            //CoreUtils.SetKeyword(cmd, ShaderKeywordStrings.MainLightShadows, renderingData.shadowData.supportsMainLightShadows);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            //======================================================================
            //draw objects(e.g. reflective wet ground plane) with lightmode "MobileSSPR", which will sample _MobileSSPR_ColorRT
            DrawingSettings drawingSettings = CreateDrawingSettings(STID_SSRP, ref renderingData, SortingCriteria.CommonOpaque);
            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);
            
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(PID_PlanarReflectionTexture);

            if (ShouldUseSinglePassUnsafeAllowFlickeringDirectResolve())
                cmd.ReleaseTemporaryRT(PID_PosWSyTexture);
            else
                cmd.ReleaseTemporaryRT(PID_HashTexture);
        }
    }

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(Settings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents - 1;//we must wait _CameraOpaqueTexture & _CameraDepthTexture is usable
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (RenderExtensionSetting.bUsageSSPR > 0)
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}


