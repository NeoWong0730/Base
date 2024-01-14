using Framework;
using Lib.Core;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SceneInstanceRender : MonoBehaviour
{
    [SerializeField]
    public string sInstanceDataPath;
    private AsyncOperationHandle<SceneInstanceData> _instanceDataHandle;
    private SceneInstanceData _instanceData;

    public int nStripLevel = 0;    
    public float CollideSize = 0.6f;
    public static Vector3 CollidePosition;
    public int mip;

    public static readonly int kCountPerDrawCall = 1023;
    public static readonly int kMaxMatrix = 8192;

    private int _maxMatrix = kCountPerDrawCall;// kCountPerDrawCall;
    //一次渲染只使用一次ComputeBuffer
    private bool _ComputeBufferUsed = false;
    private Matrix4x4[] tempMatrix4X4;

    private ComputeBuffer matrixBuffer;
    //private ComputeBuffer argBuffer;
    //private uint[] args = new uint[5] { 0u, 0u, 0u, 0u, 0u };

    public QTree mQTree;
    [SerializeField]
    public int3 gridSize;
    [SerializeField]
    public AABB bounds;

    private void Awake()
    {
        mQTree = new QTree();
        mQTree.SetSize(bounds, gridSize, mip);
    }

    private void OnEnable()
    {
        if (!SystemInfo.supportsInstancing)
        {
            DebugUtil.Log(ELogType.eNone, "supportsInstancing false");
            return;
        }        

        if (SystemInfo.supportsComputeShaders && false)// && UnityEngine.Rendering.Universal.RenderingUtils.useStructuredBuffer)
        {
            _maxMatrix = kMaxMatrix;
            //_maxMatrix = kCountPerDrawCall;
        }
        else
        {
            _maxMatrix = kCountPerDrawCall;
        }

        if (_instanceData == null && !string.IsNullOrWhiteSpace(sInstanceDataPath))
        {
            AddressablesUtil.LoadAssetAsync<SceneInstanceData>(ref _instanceDataHandle, sInstanceDataPath, _instanceDataHandle_Completed);
        }

        if (Application.isPlaying)
        {
            DrawInstance.onDrawInstance += _onDrawInstance;
        }
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;

        tempMatrix4X4 = new Matrix4x4[_maxMatrix];
        if(_maxMatrix > kCountPerDrawCall)
        {
            matrixBuffer = new ComputeBuffer(kMaxMatrix, 64);
            //argBuffer = new ComputeBuffer(1, 5 * 4, ComputeBufferType.IndirectArguments);
        }        
    }

    private void _instanceDataHandle_Completed(AsyncOperationHandle<SceneInstanceData> obj)
    {
        _instanceData = obj.Result;
    }

    private void OnDisable()
    {
        if (!SystemInfo.supportsInstancing)
        {
            return;
        }

        if (Application.isPlaying)
        {
            if (DrawInstance.onDrawInstance != null)
            {
                DrawInstance.onDrawInstance -= _onDrawInstance;
            }
        }

        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;

        if (matrixBuffer != null)
        {
            matrixBuffer.Dispose();
            matrixBuffer = null;
        }

        //if (argBuffer != null)
        //{
        //    argBuffer.Dispose();
        //    argBuffer = null;
        //}
    }

    private void OnDestroy()
    {
        if (_instanceDataHandle.IsValid())
        {
            AddressablesUtil.Release<SceneInstanceData>(ref _instanceDataHandle, _instanceDataHandle_Completed);
            _instanceData = null;
        }

        if (mQTree != null)
        {
            mQTree.Dispose();
            mQTree = null;
        }
    }

    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (!RenderExtensionSetting.bUsageGrass)
            return;

        if (camera.cameraType == CameraType.SceneView || camera.CompareTag(Tags.UICamera) || camera.CompareTag(Tags.ShowCamera))
            return;

        mQTree.AsyncCull(camera, camera.farClipPlane);
    }
    private void _onDrawInstance(ProfilingSampler profilingSampler, ScriptableRenderContext context, UnityEngine.Rendering.Universal.RenderingData renderingData, RenderQueueType renderQueueType)
    {
        if (!RenderExtensionSetting.bUsageGrass)
            return;

        ExecuteDrawMeshInstanced(profilingSampler, context, renderingData.cameraData.camera, _instanceData, renderQueueType);
    }
    public void ExecuteDrawMeshInstanced(ProfilingSampler profilingSampler, ScriptableRenderContext context, Camera camera, SceneInstanceData instanceData, RenderQueueType renderQueueType)
    {
        if (_instanceData == null)
            return;

        if (nStripLevel == 4)
            return;

        if (!LayerMaskUtil.ContainLayer(camera.cullingMask, (int)ELayerMask.Grass) || mQTree == null)
            return;

        mQTree.Complete();        

        _ComputeBufferUsed = false;

        //绘制视锥体内的
        CommandBuffer cmd = CommandBufferPool.Get(profilingSampler.name);
        cmd.Clear();
        cmd.SetGlobalVector(Consts._CollideInfo_ID, new float4(CollidePosition, CollideSize));
        context.ExecuteCommandBuffer(cmd);

        for (int layer = 0; layer < instanceData.instanceIndexTree.Length; ++layer)
        {
            if (instanceData.lod[layer] > RenderExtensionSetting.nSceneMaxLOD)
            {
                continue;
            }

            Mesh mesh = instanceData.meshPath[layer];
            Material material = instanceData.materialPath[layer];

            if (material == null || mesh == null)
            {
                continue;
            }

            if (!((material.renderQueue <= 2450 && renderQueueType == RenderQueueType.Opaque) || (material.renderQueue > 2450 && renderQueueType == RenderQueueType.Transparent)))
            {
                continue;
            }

            BlockTree blockTree = instanceData.instanceIndexTree[layer];                 
            int arrayCount = Draw(blockTree.nodes, instanceData.instanceMatrices, blockTree.nodes.Length - blockTree.rootCount, blockTree.nodes.Length, mip - 1, 0, mesh, material, context, cmd);
            DrawMeshInstanced(context, cmd, mesh, 0, material, -1, tempMatrix4X4, arrayCount);            
        }
        
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    private int Draw(BlockNode[] nodes, Matrix4x4[] matrices, int start, int end, int mip, int count, Mesh mesh, Material material, ScriptableRenderContext context, CommandBuffer cmd)
    {
        for (int i = start; i < end; ++i)
        {
            BlockNode node = nodes[i];
            Unity.Rendering.FrustumPlanes.IntersectResult result = mQTree.GetIntersectResult(node.boundIndex, mip);
            if (result == Unity.Rendering.FrustumPlanes.IntersectResult.Out)
            {
                continue;
            }

            if (result == Unity.Rendering.FrustumPlanes.IntersectResult.In || mip == 0)
            {
                int sourceIndex = node.dataStart;

                while (sourceIndex < node.dataEnd)
                {
                    int maxLength = _ComputeBufferUsed ? kCountPerDrawCall : _maxMatrix;

                    int length = math.min(node.dataEnd - sourceIndex, maxLength - count);
                    Array.Copy(matrices, sourceIndex, tempMatrix4X4, count, length);
                    sourceIndex += length;
                    count += length;

                    if (count >= maxLength)
                    {
                        DrawMeshInstanced(context, cmd, mesh, 0, material, -1, tempMatrix4X4, count);
                        count = 0;
                    }
                }
            }
            else
            {
                count = Draw(nodes, matrices, node.childStart, node.childEnd, mip - 1, count, mesh, material, context, cmd);
            }
        }

        return count;
    }

    private void DrawMeshInstanced(ScriptableRenderContext context, CommandBuffer cmd, Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count)
    {
        if (count > kCountPerDrawCall)
        {
            cmd.Clear();
            _ComputeBufferUsed = true;

            matrixBuffer.SetData(tempMatrix4X4, 0, 0, count);
            //matrixBuffer.SetCounterValue((uint)count);
            //material.SetBuffer(Consts._MatrixBuffer_ID, matrixBuffer);
            cmd.SetGlobalBuffer(Consts._MatrixBuffer_ID, matrixBuffer);
            //args[0] = mesh.GetIndexCount(0);
            //args[1] = (uint)count;
            //args[2] = mesh.GetIndexStart(0);
            //args[3] = mesh.GetBaseVertex(0);
            //argBuffer.SetData(args);
            cmd.DrawMeshInstancedProcedural(mesh, 0, material, 0, count);
            context.ExecuteCommandBuffer(cmd);
        }
        else if (count > 1)
        {
            cmd.Clear();
            cmd.DrawMeshInstanced(mesh, 0, material, 0, tempMatrix4X4, count);
            context.ExecuteCommandBuffer(cmd);
        }
        else if (count > 0)
        {
            cmd.Clear();
            cmd.DrawMesh(mesh, tempMatrix4X4[0], material, 0, 0);
            context.ExecuteCommandBuffer(cmd);
        }
    }

#if UNITY_EDITOR
    public bool bDrawScene = true;
    public bool bDrawGizmos = true;
    public int nDrawMip = 0;

    private void OnDrawGizmos()
    {
        if (bDrawGizmos && mQTree != null)
        {
            mQTree.DrawGizmos(nDrawMip);
        }
    }
#endif
}
