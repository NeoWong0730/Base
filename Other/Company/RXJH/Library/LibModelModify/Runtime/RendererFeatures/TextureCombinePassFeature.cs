using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TextureCombinePassFeature : ScriptableRendererFeature
{
    public ComputeShader m_ComputeShader;

    TextureCombinePass m_ScriptablePass;
    private RenderTargetHandle m_TemporaryRT;
    private RenderTargetHandle m_TemporaryCompressRT;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new TextureCombinePass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //m_TemporaryRT.Init("_TextureCombineRT");
        //m_TemporaryCompressRT.Init("_CompressRT");
        //m_ScriptablePass.m_TemporaryRT = m_TemporaryRT;
        //m_ScriptablePass.m_TemporaryCompressRT = m_TemporaryCompressRT;
        m_ScriptablePass.m_ComputeShader = m_ComputeShader;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


