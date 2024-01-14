using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Framework
{
    public class ScreenSpaceProcess : ScriptableRendererFeature
    {
        [System.Flags]
        public enum EScreenSpaceProcessUsage
        { 
            None = 0,
            GaussianBlur = 1,
            OutLine = 2,
            OcclusionTransparent = 4,
            Could = 8,
        }

        [System.Serializable]
        public struct ScreenSpaceProcessSetting
        {
            public EScreenSpaceProcessUsage eScreenSpaceProcessUsage;
            public RenderPassEvent eRenderPassEvent;
            public int nRenderPassEventOffset;
            public bool bGrabTexture;
        }

        public Material mMaterial;

        [SerializeField]
        public ScreenSpaceProcessSetting processSetting;
        ScreenSpaceProcessPass mScreenSpaceProcessPass;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if ((processSetting.eScreenSpaceProcessUsage & RenderExtensionSetting.eScreenSpaceProcessUsage) != EScreenSpaceProcessUsage.None)
            {
                mScreenSpaceProcessPass.SetUp(renderer);
                renderer.EnqueuePass(mScreenSpaceProcessPass);
            }
        }

        public override void Create()
        {
            mScreenSpaceProcessPass = new ScreenSpaceProcessPass(name, mMaterial, processSetting);
        }
    }
}