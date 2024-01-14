//#define USE_TRANSFORM_Job

using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class TansformWorldToScreen : MonoBehaviour
{
    public class WorldToScreenData
    {
        internal bool state;
        public Transform from;
        public RectTransform to;
        public Vector3 positionOffset { get; set; }
        public Vector2 anchoredOffset { get; set; }
    }

    private Queue<WorldToScreenData> _pool = new Queue<WorldToScreenData>();
    private List<WorldToScreenData> _worldToScreenDatas = new List<WorldToScreenData>();
#if USE_TRANSFORM_Job
    private NativeArray<float4> _postions;
#endif
    public WorldToScreenData Request()
    {
        WorldToScreenData worldToScreenData;
        if (_pool.Count > 0)
        {
            worldToScreenData = _pool.Dequeue();
        }
        else
        {
            worldToScreenData = new WorldToScreenData();
            DebugUtil.LogFormat(ELogType.eNone, _worldToScreenDatas.Count + 1 >= 128, "_worldToScreenDatas.Count = {0}", _worldToScreenDatas.Count + 1);
        }

        _worldToScreenDatas.Add(worldToScreenData);
        worldToScreenData.state = true;

        return worldToScreenData;
    }

    public void Release(ref WorldToScreenData worldToScreenData)
    {
        if (worldToScreenData == null)
            return;

        if (worldToScreenData.state)
        {
            worldToScreenData.state = false;
            _pool.Enqueue(worldToScreenData);
        }
        worldToScreenData.from = null;
        worldToScreenData.to = null;

        worldToScreenData = null;
    }

    private void OnDestroy()
    {
        _worldToScreenDatas.Clear();
        _pool.Clear();
#if USE_TRANSFORM_Job
        if (_postions.IsCreated)
        {
            _postions.Dispose();
        }
#endif
    }

#if USE_TRANSFORM_Job
    [BurstCompile]
    private void LateUpdate()
    {
        int count = _worldToScreenDatas.Count;
        for (int i = count - 1; i >= 0; --i)
        {
            WorldToScreenData worldToScreenData = _worldToScreenDatas[i];
            if (worldToScreenData.state == false || worldToScreenData.from == null || worldToScreenData.to == null)
            {
                _worldToScreenDatas.RemoveAt(i);
            }
        }
        
        int capacity = 1;
        if (_postions.IsCreated && _postions.Length < count)
        {
            capacity = _postions.Length;
            _postions.Dispose();
        }

        count = _worldToScreenDatas.Count;
        if (!_postions.IsCreated)
        {
            while (capacity < count)
            {
                capacity <<= 1;
            }
            _postions = new NativeArray<float4>(capacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        }

        for (int i = 0; i < count; ++i)
        {
            float3 pos = _worldToScreenDatas[i].from.position + _worldToScreenDatas[i].positionOffset;
            _postions[i] = new float4(pos, 1);
        }

        float4x4 VP = math.mul(MatrixUtil.CreateFrom(CameraManager.mCamera.projectionMatrix), MatrixUtil.CreateFrom(CameraManager.mCamera.worldToCameraMatrix));
        float4x4 VP_I = math.inverse(math.mul(MatrixUtil.CreateFrom(CameraManager.mUICamera.projectionMatrix), MatrixUtil.CreateFrom(CameraManager.mUICamera.worldToCameraMatrix)));

        WorldToScreenJob job = new WorldToScreenJob()
        {
            worldToViewport = VP,
            viewportToWorld = VP_I,
            postions = _postions,
        };

        JobHandle jobHandle = job.Schedule<WorldToScreenJob>(count, 32);
        jobHandle.Complete();

        for (int i = 0; i < count; ++i)
        {
            if (_postions[i].w != 0)
            {
                _worldToScreenDatas[i].to.position = _postions[i].xyz;
                _worldToScreenDatas[i].to.anchoredPosition += _worldToScreenDatas[i].anchoredOffset;
            }
        }
    }

    [BurstCompile]
    struct WorldToScreenJob : IJobParallelFor
    {
        [ReadOnly] public float4x4 worldToViewport;
        [ReadOnly] public float4x4 viewportToWorld;
        public NativeArray<float4> postions;
        public void Execute(int index)
        {
            if (postions[index].w == 0)
                return;

            float4 rlt = postions[index];
            rlt = math.mul(worldToViewport, postions[index]);
            //转换到NDC坐标空间 [-1,1]
            if (rlt.w == 0)
            {
                rlt.xyz = 0;
            }
            else
            {
                rlt.xy = rlt.xy / rlt.w;
                rlt.z = 0;
            }
            rlt.w = 1;
            postions[index] = math.mul(viewportToWorld, rlt);
        }
    }
#else
    private void LateUpdate()
    {
        Camera fromCamera = CameraManager.mCamera;
        Camera toCamera = CameraManager.mUICamera;

        if (!fromCamera || !toCamera)
            return;

        int count = _worldToScreenDatas.Count;
        for (int i = count - 1; i >= 0; --i)
        {
            WorldToScreenData worldToScreenData = _worldToScreenDatas[i];
            if (worldToScreenData.state == false || worldToScreenData.from == null || worldToScreenData.to == null)
            {
                _worldToScreenDatas.RemoveAt(i);
            }
            else
            {
                float3 pos = worldToScreenData.from.position + worldToScreenData.positionOffset;
                pos = fromCamera.WorldToViewportPoint(pos);
                pos = toCamera.ViewportToWorldPoint(pos);
                worldToScreenData.to.position = pos;
                worldToScreenData.to.anchoredPosition += worldToScreenData.anchoredOffset;
            }
        }
    }
#endif
}