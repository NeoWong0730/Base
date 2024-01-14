using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

public static class DrawInstance
{
    public delegate void OnDrawInstance(ProfilingSampler profilingSampler, ScriptableRenderContext context, UnityEngine.Rendering.Universal.RenderingData renderingData, UnityEngine.Experimental.Rendering.Universal.RenderQueueType renderQueueType);
    public static OnDrawInstance onDrawInstance;

    internal static bool Has()
    {
        if (onDrawInstance != null)
            return true;
        return false;
    }

    internal static void Execute(ProfilingSampler profilingSampler, ScriptableRenderContext context, UnityEngine.Rendering.Universal.RenderingData renderingData, UnityEngine.Experimental.Rendering.Universal.RenderQueueType renderQueueType)
    {
        onDrawInstance(profilingSampler, context, renderingData, renderQueueType);
    }
}

public class RenderInstancingPass : UnityEngine.Rendering.Universal.ScriptableRenderPass
{
    UnityEngine.Experimental.Rendering.Universal.RenderQueueType renderQueueType;    
    string m_ProfilerTag;
    
    ProfilingSampler mProfilingSampler;

    public RenderInstancingPass(string profilerTag, UnityEngine.Rendering.Universal.RenderPassEvent renderPassEvent, UnityEngine.Experimental.Rendering.Universal.RenderQueueType renderQueueType)
    {
        m_ProfilerTag = profilerTag;
        mProfilingSampler = new ProfilingSampler(m_ProfilerTag);

        this.renderPassEvent = renderPassEvent;
        this.renderQueueType = renderQueueType;                
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        base.Configure(cmd, cameraTextureDescriptor);
    }

    public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
    {        
        DrawInstance.Execute(mProfilingSampler, context, renderingData, renderQueueType);
    }
}
