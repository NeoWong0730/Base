using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Lib;
using Framework;

public class SceneBatchRenderer : MonoBehaviour
{
    public RenderData[] mRenderDatas;
    public Matrix4x4[] mMatrix4X4s;
    public AABB[] mBounds;
    public string sRenderDataPath;

    private BatchRendererGroup _batchRendererGroup;
    private NativeArray<AABB> _bounds;
    private NativeArray<AABB> _groupBounds;
    private NativeArray<int> _lods;

    private JobHandle _cullingJobDependency;    

    private void OnEnable()
    {
        if (mRenderDatas == null || mMatrix4X4s == null || mBounds == null)
        {
            Debug.LogErrorFormat("SceneBatchRenderer data missing {0}", gameObject.scene.name);
            return;
        }            

        _batchRendererGroup = new BatchRendererGroup(OnPerformCulling);

        int offset = 0;
        
        _lods = new NativeArray<int>(mRenderDatas.Length, Allocator.Persistent);
        _groupBounds = new NativeArray<AABB>(mRenderDatas.Length, Allocator.Persistent);
        _bounds = new NativeArray<AABB>(mBounds, Allocator.Persistent);
        //_textures = new List<Texture>();

        RenderData renderData;
        for (int i = 0; i < mRenderDatas.Length; ++i)
        {
            renderData = mRenderDatas[i];
            if (renderData.material == null || renderData.mesh == null)
            {
                Debug.LogErrorFormat("SceneBatchRenderer material or mesh missing, {0} Index = {1}", gameObject.scene.name, i.ToString());
                break;
            }

#if UNITY_EDITOR
            string[] texNames = renderData.material.GetTexturePropertyNames();
#endif
            int[] texNameIDs = renderData.material.GetTexturePropertyNameIDs();
            for (int j = 0; j < texNameIDs.Length; ++j)
            {
                Texture texture = renderData.material.GetTexture(texNameIDs[j]);
#if UNITY_EDITOR
                if (!texture)
                {
                    DebugUtil.LogFormat(ELogType.eScene, "{0} is not has texture {1}", renderData.material.name, texNames[j]);
                    continue;
                }
#endif
                Texture2D texture2D = texture as Texture2D;
                if (!texture2D)
                {
                    DebugUtil.LogFormat(ELogType.eScene, "{0} is not texture2D", texture.name);
                    continue;
                }

                texture2D.requestedMipmapLevel = 0;
            }

            int index = _batchRendererGroup.AddBatch(renderData.mesh, renderData.subMeshIndex, renderData.material, renderData.layer, renderData.shadowCastingMode, renderData.receiveShadows, false, renderData.bounds, renderData.instanceCount, null, null);
            NativeArray<Matrix4x4> matrix4X4s = _batchRendererGroup.GetBatchMatrices(index);
            NativeArray<Matrix4x4>.Copy(mMatrix4X4s, offset, matrix4X4s, 0, renderData.instanceCount);
            offset += renderData.instanceCount;

            _lods[i] = renderData.lod;
            AABB _groupBound = new AABB();
            _groupBound.Center = renderData.bounds.center;
            _groupBound.Extents = renderData.bounds.extents;
            _groupBounds[i] = _groupBound;
        }

        

#if UNITY_EDITOR
        _results = new NativeArray<byte>(mBounds.Length, Allocator.Persistent);
#endif
    }

    private void OnDisable()
    {
        _cullingJobDependency.Complete();

        if (_batchRendererGroup != null)
        {
            _batchRendererGroup.Dispose();
            _batchRendererGroup = null;
        }

        if (_bounds.IsCreated)
        {
            _bounds.Dispose();
        }

        if(_groupBounds.IsCreated)
        {
            _groupBounds.Dispose();
        }

        if(_lods.IsCreated)
        {
            _lods.Dispose();
        }        

#if UNITY_EDITOR
        if(_results.IsCreated)
        {
            _results.Dispose();
        }
#endif        
    }    

    private JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext)
    {
#if UNITY_EDITOR
        if (Time.frameCount == currentFrame)
        {
            ++currentIndex;
        }
        else
        {
            currentFrame = Time.frameCount;
            currentIndex = 0;
        }
#endif

        int batchCount = cullingContext.batchVisibility.Length;
        if (batchCount == 0)
            return new JobHandle();

        //NativeArray<float4> planes = new NativeArray<float4>(cullingContext.cullingPlanes.Length, Allocator.TempJob);
        var planes = Unity.Rendering.FrustumPlanes.BuildSOAPlanePackets(cullingContext.cullingPlanes, Allocator.TempJob);

        BatchCulling batchCulling = new BatchCulling()
        {
            planes = planes,
            batchVisibilities = cullingContext.batchVisibility,
            visibleIndices = cullingContext.visibleIndices,
            bounds = _bounds,
            groupBounds = _groupBounds,
            lods = _lods,
            maxLod = RenderExtensionSetting.nSceneMaxLOD,
            isValid = cullingContext.lodParameters.isOrthographic == false,
#if UNITY_EDITOR
            results = _results,
            needResults = indexCamera == currentIndex,
#endif
        };
        JobHandle handle = batchCulling.Schedule<BatchCulling>(cullingContext.batchVisibility.Length, 32, _cullingJobDependency);
        _cullingJobDependency = JobHandle.CombineDependencies(handle, _cullingJobDependency);        
        return handle;
    }

#if UNITY_EDITOR
    public bool bDrawGizmos = false;
    public bool bDrawCullResult = true;
    public int indexCamera = 0;

    private int currentIndex = 0;
    private int currentFrame = 0;
    NativeArray<byte> _results;

    private void OnDrawGizmos()
    {
        if (!bDrawGizmos)
            return;
        for (int i = 0; i < mBounds.Length; ++i)
        {
            if (!bDrawCullResult || _results[i] > 0)
            {
                Gizmos.DrawWireCube(mBounds[i].Center, mBounds[i].Size);
            }
        }
    }
#endif
}

[BurstCompile]
struct BatchCulling : IJobParallelFor
{
    //[ReadOnly] public bool draw;
    [DeallocateOnJobCompletion][ReadOnly] public NativeArray<Unity.Rendering.FrustumPlanes.PlanePacket4> planes;
    [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<AABB> bounds;
    [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<AABB> groupBounds;
    [NativeDisableParallelForRestriction] public NativeArray<BatchVisibility> batchVisibilities;
    [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<int> visibleIndices;
    [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<int> lods;    
    [NativeDisableParallelForRestriction] [ReadOnly] public int maxLod;
    [NativeDisableParallelForRestriction] [ReadOnly] public bool isValid;

#if UNITY_EDITOR
    [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<byte> results;
    [NativeDisableParallelForRestriction] [ReadOnly] public bool needResults;
#endif

    public void Execute(int index)
    {
        BatchVisibility batchVisibility = batchVisibilities[index];
        int visibleCount = 0;

        //有效的 当前支持的LOD 在组的包围盒内部
        if (isValid && lods[index] <= maxLod && Unity.Rendering.FrustumPlanes.Intersect2NoPartial(planes, groupBounds[index]) != Unity.Rendering.FrustumPlanes.IntersectResult.Out)
        {
            int instancesCount = batchVisibility.instancesCount;

            if(instancesCount == 1)
            {
                visibleIndices[batchVisibility.offset] = 0;
                ++visibleCount;

#if UNITY_EDITOR
                if (needResults)
                {
                    results[batchVisibility.offset] = 1;
                }
#endif
            }
            else
            {
                for (int i = 0; i < instancesCount; ++i)
                {
#if UNITY_EDITOR
                    if (needResults)
                    {
                        results[batchVisibility.offset + i] = 0;
                    }
#endif

                    if (Unity.Rendering.FrustumPlanes.Intersect2NoPartial(planes, bounds[batchVisibility.offset + i]) != Unity.Rendering.FrustumPlanes.IntersectResult.Out)
                    {
                        visibleIndices[batchVisibility.offset + visibleCount] = i;
                        ++visibleCount;

#if UNITY_EDITOR
                        if (needResults)
                        {
                            results[batchVisibility.offset + i] = 1;
                        }
#endif
                    }
                }
            }
        }
#if UNITY_EDITOR
        else if(needResults)
        {
            int instancesCount = batchVisibility.instancesCount;
            for (int i = 0; i < instancesCount; ++i)
            {
                results[batchVisibility.offset + i] = 0;
            }
        }
#endif
        batchVisibility.visibleCount = visibleCount;
        batchVisibilities[index] = batchVisibility;
    }
}
