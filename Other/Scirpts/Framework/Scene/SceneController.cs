using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;

[DisallowMultipleComponent]
public class SceneController : MonoBehaviour
{
    [Serializable]
    public struct AssetInfo
    {
        public string assetPath;

        private AsyncOperationHandle<GameObject> assetHandle;
        private List<Transform> pool;

        public Transform GetInstance(Transform parent, Vector3 position, Quaternion rotation)
        {
            if (pool != null && pool.Count > 0)
            {
                Transform transform = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);

                transform.SetPositionAndRotation(position, rotation);
                return transform;
            }
            else if (assetHandle.IsValid())
            {
                if (assetHandle.IsDone)
                {
                    Transform transform = GameObject.Instantiate<GameObject>(assetHandle.Result, position, rotation, parent).transform;
                    return transform;
                }
            }
            else
            {
                AddressablesUtil.LoadAssetAsync<GameObject>(ref assetHandle, assetPath, null);
            }
            return null;
        }

        public void SetInstance(Transform gameObject)
        {
            if (pool == null)
            {
                pool = new List<Transform>();
            }
            pool.Add(gameObject);
        }

        public void Release()
        {
            if (pool != null)
            {
                for (int i = 0; i < pool.Count; ++i)
                {
                    GameObject.DestroyImmediate(pool[i].gameObject);
                }
                pool.Clear();
            }

            if (assetHandle.IsValid())
            {
                AddressablesUtil.Release<GameObject>(ref assetHandle, null);
                assetHandle = default(AsyncOperationHandle<GameObject>);
            }
        }
    }

    [BurstCompile]
    struct CullJob : IJobParallelFor
    {
        [ReadOnly] public int mCameraCount;
        [ReadOnly] public NativeArray<float3> mCameraPoses;
        [ReadOnly] public NativeArray<int> mCameraLayers;

        [ReadOnly] public NativeArray<int> mLayers;
        [ReadOnly] public NativeArray<float4> mPositionAndRadiuses;

        [WriteOnly] public NativeArray<bool> mResults;

        public void Execute(int index)
        {
            mResults[index] = false;

            float3 position = mPositionAndRadiuses[index].xyz;
            float radius = mPositionAndRadiuses[index].w;
            int layer = mLayers[index];

            for (int i = 0; i < mCameraCount; ++i)
            {
                if ((mCameraLayers[i] & layer) != 0
                    && math.distancesq(mCameraPoses[i], position) < radius * radius)
                {
                    mResults[index] = true;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 取所有物体mask的最大集合
    /// </summary>
    [SerializeField] public int nLayerMask;

    /// <summary>
    /// 根节点
    /// </summary>
    [SerializeField] public Transform mRootTransfrom;

    /// <summary>
    /// 所有的资源信息
    /// </summary>
    [SerializeField] public AssetInfo[] mAssetInfos;

    /// <summary>
    /// 实例对应的资源index
    /// </summary>
    [SerializeField] public int[] mAssetIndices;

    [SerializeField] public int[] mLayers;
    [SerializeField] public float4[] mPositionAndRadiuses;
    [SerializeField] public Quaternion[] mRotations;
    [SerializeField] public Vector3[] mScales;

    /// <summary>
    /// 已经创建的实例
    /// </summary>
    private Transform[] mTransforms;

    private NativeArray<int> _layers;
    private NativeArray<float4> _positionAndRadiuses;

    private NativeArray<bool> _results;

    private NativeArray<float3> _cameraPos;
    private NativeArray<int> _cameraLayer;

    private bool isValid = false;

    private void Start()
    {
        if (mPositionAndRadiuses == null || mLayers == null || mPositionAndRadiuses.Length != mLayers.Length)
            return;

        isValid = true;

        _results = new NativeArray<bool>(mPositionAndRadiuses.Length, Allocator.Persistent);

        _layers = new NativeArray<int>(_layers, Allocator.Persistent);
        _positionAndRadiuses = new NativeArray<float4>(mPositionAndRadiuses, Allocator.Persistent);

        mLayers = null;
        mPositionAndRadiuses = null;
    }

    private void OnEnable()
    {
        if (isValid)
        {
            UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering += RenderPipelineManager_beginFrameRendering;
        }
    }    

    private void OnDisable()
    {
        if (isValid)
        {
            UnityEngine.Rendering.RenderPipelineManager.beginFrameRendering -= RenderPipelineManager_beginFrameRendering;
        }
    }

    private void OnDestroy()
    {
        if(_layers.IsCreated)
        {
            _layers.Dispose();
        }

        if (_positionAndRadiuses.IsCreated)
        {
            _positionAndRadiuses.Dispose();
        }

        if(_results.IsCreated)
        {
            _results.Dispose();
        }

        if (_cameraPos.IsCreated)
        {
            _cameraPos.Dispose();
        }

        if(_cameraLayer.IsCreated)
        {
            _cameraLayer.Dispose();
        }
    }

    private void RenderPipelineManager_beginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
    {
        if (!_cameraPos.IsCreated)
        {
            _cameraPos = new NativeArray<float3>(cameras.Length, Allocator.Persistent);
            _cameraLayer = new NativeArray<int>(cameras.Length, Allocator.Persistent);
        }
        else if (_cameraPos.Length < cameras.Length)
        {
            _cameraPos.Dispose();
            _cameraPos = new NativeArray<float3>(cameras.Length, Allocator.Persistent);

            _cameraLayer.Dispose();
            _cameraLayer = new NativeArray<int>(cameras.Length, Allocator.Persistent);
        }

        int count = 0;

        for (int i = 0; i < cameras.Length; ++i)
        {
            Camera cam = cameras[i];
            if (0 == (cam.cullingMask & nLayerMask))
                continue;

            _cameraLayer[count] = cam.cullingMask;
            _cameraPos[count] = cam.transform.position;

            ++count;
        }

        CullJob job = new CullJob()
        {
            mCameraCount = count,
            mCameraLayers = _cameraLayer,
            mCameraPoses = _cameraPos,
            mLayers = _layers,
            mPositionAndRadiuses = _positionAndRadiuses,
            mResults = _results,
        };

        JobHandle jobHandle = job.Schedule<CullJob>(_results.Length, 32);
        jobHandle.Complete();
    }    
}