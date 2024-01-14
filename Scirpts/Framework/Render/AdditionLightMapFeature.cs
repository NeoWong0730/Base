using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Framework
{
    public class AdditionLightMapFeature : ScriptableRendererFeature
    {
        public Mesh mesh;
        public Material material;
        public float size;

        AdditionLightMapPass additionLightMapPass;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.lightData.additionalLightsCount > 0 && SystemInfo.supportsInstancing)
            {
                additionLightMapPass.size = size;
                renderer.EnqueuePass(additionLightMapPass);
            }
        }

        public override void Create()
        {
            additionLightMapPass = new AdditionLightMapPass();
            additionLightMapPass.mesh = mesh;
            additionLightMapPass.material = material;
        }
    }
}