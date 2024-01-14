using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Framework
{
    public class AdditionLightMapPass : ScriptableRenderPass
    {
        const string m_ProfilerTag = "Addition Light Map";
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
        static readonly int ID_AdditionLightMap = Shader.PropertyToID("_AdditionLightMap");
        static readonly int ID_LightIndex = Shader.PropertyToID("_LightIndex");
        static readonly int ID_AdditionLightMapInfo = Shader.PropertyToID("_AdditionLightMapInfo");


        RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(ID_AdditionLightMap);
        public Mesh mesh;
        public Material material;
        public Matrix4x4[] matrix4X4s = new Matrix4x4[32];
        //public float[] lightIndexBuffer = new float[16];
        //public MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        public float size = 1;

        public AdditionLightMapPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor rtd = new RenderTextureDescriptor(cameraTextureDescriptor.width >> 3, cameraTextureDescriptor.height >> 3, RenderTextureFormat.RG16, 0, 0);
            rtd.sRGB = false;
            rtd.autoGenerateMips = false;
            //rtd.memoryless = RenderTextureMemoryless.Color;

            cmd.GetTemporaryRT(ID_AdditionLightMap, rtd);
            ConfigureTarget(renderTargetIdentifier);
            ConfigureClear(ClearFlag.All, Color.black);
        }
        /*
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //lightIndexBuffer.Clear();
            Rect rect = new Rect();
            int offset = renderingData.lightData.visibleLights.Length - renderingData.lightData.additionalLightsCount;
            for (int i = 0; i < renderingData.lightData.additionalLightsCount; ++i)
            {
                VisibleLight visibleLight = renderingData.lightData.visibleLights[i + offset];
                Vector4 translation = visibleLight.localToWorldMatrix.GetColumn(3);
                Vector2 posXZ = new Vector2(translation.x, translation.z);
                Matrix4x4 mt = visibleLight.localToWorldMatrix;
                mt.m00 = visibleLight.range * 2;
                mt.m11 = visibleLight.range * 2;
                mt.m22 = visibleLight.range * 2;
                matrix4X4s[i] = mt;

                lightIndexBuffer[i] = (i + 1);

                if (i == 0)
                {
                    rect = new Rect(posXZ - new Vector2(visibleLight.range, visibleLight.range), new Vector2(visibleLight.range, visibleLight.range) * 2);
                }
                else
                {
                    rect.xMin = Mathf.Min(rect.xMin, posXZ.x - visibleLight.range);
                    rect.xMax = Mathf.Max(rect.xMax, posXZ.x + visibleLight.range);
                    rect.yMin = Mathf.Min(rect.yMin, posXZ.y - visibleLight.range);
                    rect.yMax = Mathf.Max(rect.yMax, posXZ.y + visibleLight.range);
                }
            }

            float realSize = Mathf.Max(rect.size.x, rect.size.y) / 2;
            realSize *= size;

            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                ref CameraData cameraData = ref renderingData.cameraData;

                Vector3 pos = new Vector3(rect.center.x, 20, rect.center.y);
                Quaternion rot = Quaternion.Euler(90, 0, 0);
                Vector3 scale = new Vector3(1, 1, -1);

                Matrix4x4 viewMatrix = Matrix4x4.TRS(pos, rot, scale).inverse;

                Matrix4x4 projectionMatrix = Matrix4x4.Ortho(-realSize, realSize, -realSize, realSize, 1, 1000);
                projectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix, false);

                RenderingUtils.SetViewAndProjectionMatrices(cmd, viewMatrix, projectionMatrix, false);

                materialPropertyBlock.SetFloatArray(ID_LightIndex, lightIndexBuffer);
                cmd.DrawMeshInstanced(mesh, 0, material, 0, matrix4X4s, renderingData.lightData.additionalLightsCount, materialPropertyBlock);

                RenderingUtils.SetViewAndProjectionMatrices(cmd, cameraData.GetViewMatrix(), cameraData.GetGPUProjectionMatrix(), false);
            }        
            cmd.SetGlobalTexture(ID_AdditionLightMap, renderTargetIdentifier);
            cmd.SetGlobalVector(ID_AdditionLightMapInfo, new Vector4(rect.center.x - realSize, 0, rect.center.y - realSize, realSize * 2));
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
        */

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            int offset = renderingData.lightData.visibleLights.Length - renderingData.lightData.additionalLightsCount;
            for (int i = 0; i < renderingData.lightData.additionalLightsCount; ++i)
            {
                VisibleLight visibleLight = renderingData.lightData.visibleLights[i + offset];
                Matrix4x4 mt = visibleLight.localToWorldMatrix;
                float scale = 2f * size;
                mt.m00 = visibleLight.range * scale;
                mt.m11 = visibleLight.range * scale;
                mt.m22 = visibleLight.range * scale;
                matrix4X4s[i] = mt;

                //lightIndexBuffer[i] = (i + 1);
            }

            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                //cmd.SetRenderTarget(renderTargetIdentifier,
                //    RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare,
                //    RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
                //
                //cmd.ClearRenderTarget(true, true, Color.black);

                //materialPropertyBlock.SetFloatArray(ID_LightIndex, lightIndexBuffer);
                //cmd.DrawMeshInstanced(mesh, 0, material, 0, matrix4X4s, renderingData.lightData.additionalLightsCount, materialPropertyBlock);
                cmd.DrawMeshInstanced(mesh, 0, material, 0, matrix4X4s, renderingData.lightData.additionalLightsCount);
            }

            cmd.SetGlobalTexture(ID_AdditionLightMap, renderTargetIdentifier);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(ID_AdditionLightMap);
        }
    }
}