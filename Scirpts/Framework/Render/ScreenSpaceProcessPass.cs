using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework
{
    public class ScreenSpaceProcessPass : ScriptableRenderPass
    {
        private static RenderTargetHandle _GrabTexture = new RenderTargetHandle { id = Shader.PropertyToID("_GrabTexture") };
        private Material mMaterial;
        private ScreenSpaceProcess.ScreenSpaceProcessSetting mProcessSetting;
        private RenderTargetIdentifier m_Source;

        string m_ProfilerTag;
        ProfilingSampler m_ProfilingSampler;

        public ScreenSpaceProcessPass(string profiler, Material material, ScreenSpaceProcess.ScreenSpaceProcessSetting processSetting)
        {
            m_ProfilerTag = profiler;
            m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
            renderPassEvent = processSetting.eRenderPassEvent + processSetting.nRenderPassEventOffset;
            mProcessSetting = processSetting;
            mMaterial = material;
        }

        public void SetUp(ScriptableRenderer renderer)
        {
            m_Source = renderer.cameraColorTarget;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (mProcessSetting.bGrabTexture)
            {
                RenderTextureDescriptor descriptor = cameraTextureDescriptor;
                descriptor.msaaSamples = 1;
                descriptor.depthBufferBits = 0;
                descriptor.width /= 4;
                descriptor.height /= 4;

                cmd.GetTemporaryRT(_GrabTexture.id, descriptor, FilterMode.Bilinear);
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                if (mProcessSetting.bGrabTexture)
                {
                    Blit(cmd, m_Source, _GrabTexture.Identifier(), mMaterial);
                    Blit(cmd, _GrabTexture.Identifier(), m_Source);
                    cmd.ReleaseTemporaryRT(_GrabTexture.id);
                }
                else
                {
                    cmd.Blit(null, BuiltinRenderTextureType.CurrentActive, mMaterial);
                }
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }
}