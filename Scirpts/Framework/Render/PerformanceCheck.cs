using UnityEngine.Rendering;
using UnityEngine;

namespace Framework
{
    public class PerformanceCheck
    {
        GameObject go;
        Material _Material;
        Mesh _Mesh;

        private int _TotalDrawTimes = 0;
        private long _TotalDrawMS = 0;

        private int _FrameCount = 0;
        private int _CheckFrameCount = 20;

        private bool bFinished = false;

        public int nPerformanceScore = 0;

        public void Start()
        {
            RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;

            go = Resources.Load<GameObject>("PeformanTest");
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();

            _Material = meshRenderer.sharedMaterial;
            _Mesh = meshFilter.sharedMesh;
        }

        private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (bFinished)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();

            int drawTimes = 0;
            long ms = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            while (true)
            {
                sw.Start();

                for (int j = 0; j < 100; ++j)
                {
                    cmd.DrawMesh(_Mesh, Matrix4x4.identity, _Material, 0, 0);
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                    ++drawTimes;
                }

                sw.Stop();

                ms += sw.ElapsedMilliseconds;
                if (ms >= 16)
                {
                    break;
                }
            }

            _TotalDrawTimes += drawTimes;
            _TotalDrawMS += ms;

            CommandBufferPool.Release(cmd);

            ++_FrameCount;
            if (_FrameCount >= _CheckFrameCount)
            {
                nPerformanceScore = (int)(1000.0f * _TotalDrawTimes / _TotalDrawMS);
                bFinished = true;
            }
        }

        public void End()
        {
            RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;
            _Material = null;
            _Mesh = null;
            go = null;

            Resources.UnloadUnusedAssets();
        }

        public bool IsFinished()
        {
            return bFinished;
        }
    }
}
