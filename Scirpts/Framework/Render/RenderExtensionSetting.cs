using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework
{
    public static class RenderExtensionSetting
    {
        public static int gMaxSceneLOD { get { return 2; } }
        public static bool bUsageLightMap = true;
        public static bool bUsageLightProbes = true;

        public static bool bUsageOcclusionTransparent { get; set; } = true;
        public static bool bUsageOutLine { get; set; } = true;
        public static bool bUsageGaussianBlur { get; set; } = false;
        public static bool bUsageCould { get; set; } = true;
        public static bool bUsageGrass { get; set; } = true;
        public static int nSceneMaxLOD { get; set; } = gMaxSceneLOD;
        public static bool UseDepthTexture { get; set; } = false;
        public static int bUsageSSPR = 0;
        public static float fHorizontalReflectionPlaneHeightWS = 0f;

        public static bool SupportsDepthCopy()
        {
            bool supportsTextureCopy = SystemInfo.copyTextureSupport != CopyTextureSupport.None;
            bool supportsDepthTarget = RenderingUtils.SupportsRenderTextureFormat(RenderTextureFormat.Depth);
            return supportsDepthTarget || supportsTextureCopy;
        }

        public static void SetGrassInteractive(bool v)
        {
            if (v)
            {
                Shader.EnableKeyword(Consts._INTERACTIVE_ON_Keyword);
            }
            else
            {
                Shader.DisableKeyword(Consts._INTERACTIVE_ON_Keyword);
            }
        }

        public static ScreenSpaceProcess.EScreenSpaceProcessUsage eScreenSpaceProcessUsage
        {
            get
            {
                ScreenSpaceProcess.EScreenSpaceProcessUsage processUsage = ScreenSpaceProcess.EScreenSpaceProcessUsage.None;
                if (bUsageGaussianBlur)
                {
                    processUsage |= ScreenSpaceProcess.EScreenSpaceProcessUsage.GaussianBlur;
                }
                else
                {
                    if (bUsageOcclusionTransparent)
                    {
                        processUsage |= ScreenSpaceProcess.EScreenSpaceProcessUsage.OcclusionTransparent;
                    }
                    if (bUsageOutLine)
                    {
                        processUsage |= ScreenSpaceProcess.EScreenSpaceProcessUsage.OutLine;
                    }
                }

                if (bUsageCould)
                {
                    processUsage |= ScreenSpaceProcess.EScreenSpaceProcessUsage.Could;
                }

                return processUsage;
            }
        }

    }
}