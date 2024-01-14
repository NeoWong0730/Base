using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class RenderInstancing : UnityEngine.Rendering.Universal.ScriptableRendererFeature
{
    [SerializeField]
    public RenderPassEvent eRenderPassEvent;
    public RenderQueueType renderQueueType;

    RenderInstancingPass renderObjectsPass;
    public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
    {
        if (SystemInfo.supportsInstancing && DrawInstance.Has())
        {
            renderer.EnqueuePass(renderObjectsPass);
        }
    }

    public override void Create()
    {
        renderObjectsPass = new RenderInstancingPass(name, eRenderPassEvent, renderQueueType);
    }
}
