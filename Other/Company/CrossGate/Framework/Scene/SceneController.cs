#define SCENE_DISPLAY
#define USE_DYNAMIC_LOAD

using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameObjectCreator
{
    private AsyncOperationHandle<GameObject> assetHandle;
    private Queue<Transform> pool;
    public Transform GetInstance(Transform parent, Vector3 position, Quaternion rotation, string assetPath)
    {
        if (pool != null && pool.Count > 0)
        {
            Transform transform = pool.Dequeue();
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
            pool = new Queue<Transform>();
        }
        pool.Enqueue(gameObject);
    }
    public void Release()
    {
        if (assetHandle.IsValid())
        {
            Addressables.Release<GameObject>(assetHandle);
            assetHandle = default(AsyncOperationHandle<GameObject>);
        }

        if (pool != null)
        {
            while (pool.Count > 0)
            {
                GameObject.DestroyImmediate(pool.Dequeue().gameObject);
            }
        }
    }
}

[DisallowMultipleComponent]
public class SceneController : MonoBehaviour
{
    [SerializeField]
    public int3 gridSize;
    [SerializeField]
    public AABB bounds;
    [SerializeField]
    public int mip;
    public QTree _treeCull;

#if USE_DYNAMIC_LOAD
    public SceneGameObjectData mSceneGameObjectData;        
    private Transform _rootTransfrom;
    private Transform[] _transforms;
    private GameObjectCreator[] _gameObjectCreators;
#endif

    private void Awake()
    {
        _treeCull = new QTree();
#if USE_DYNAMIC_LOAD
        _rootTransfrom = transform;
#endif
    }

    private void Start()
    {
        //_treeCull = new QTree();
        _treeCull.SetSize(bounds, gridSize, mip);
#if USE_DYNAMIC_LOAD
        if (mSceneGameObjectData != null && mSceneGameObjectData.gameObjectTramsforms != null && mSceneGameObjectData.prefabPaths != null)
        {
            _transforms = new Transform[mSceneGameObjectData.gameObjectTramsforms.Length];
            _gameObjectCreators = new GameObjectCreator[mSceneGameObjectData.prefabPaths.Length];
            for (int i = 0; i < _gameObjectCreators.Length; ++i)
            {
                _gameObjectCreators[i] = new GameObjectCreator();
            }
        }
#endif
    }

    private void OnEnable()
    {
        UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;
    }

    private void RenderPipelineManager_beginCameraRendering(UnityEngine.Rendering.ScriptableRenderContext arg1, Camera mainCamera)
    {
        if (mainCamera.CompareTag(Tags.UICamera) || mainCamera.CompareTag(Tags.ShowCamera))
            return;
        Cull(mainCamera);
    }

#if USE_DYNAMIC_LOAD
    private void OnDisable()
    {
        UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;

        if (_transforms == null)
            return;

        for (int dataIndex = 0; dataIndex < _transforms.Length; ++dataIndex)
        {
            if (_transforms[dataIndex] != null)
            {
                GameObjectData gameObjectData = mSceneGameObjectData.gameObjectTramsforms[dataIndex];
                GameObjectCreator gameObjectCreator = _gameObjectCreators[gameObjectData.prefabPathIndex];
                gameObjectCreator.SetInstance(_transforms[dataIndex]);
                _transforms[dataIndex] = null;
            }
        }
    }
#endif

    private void OnDestroy()
    {
#if USE_DYNAMIC_LOAD
        if (_gameObjectCreators != null)
        {
            for (int i = 0; i < _gameObjectCreators.Length; ++i)
            {
                _gameObjectCreators[i].Release();
            }
            _gameObjectCreators = null;
        }
#endif
        if (_treeCull != null)
        {
            _treeCull.Dispose();
            _treeCull = null;
        }
    }
#if USE_DYNAMIC_LOAD
    private int GameObjectVisibility(BlockTree blockTree, int start, int end, int mip, int rlt)
    {
        for (int i = start; i < end; ++i)
        {
            BlockNode node = blockTree.nodes[i];
            Unity.Rendering.FrustumPlanes.IntersectResult result = _treeCull.GetIntersectResult(node.boundIndex, mip);
            if (result == Unity.Rendering.FrustumPlanes.IntersectResult.Out)
            {
                for (int dataIndex = node.dataStart; dataIndex < node.dataEnd; ++dataIndex)
                {
                    if (_transforms[dataIndex] != null)
                    {
                        GameObjectData gameObjectData = mSceneGameObjectData.gameObjectTramsforms[dataIndex];
                        GameObjectCreator gameObjectCreator = _gameObjectCreators[gameObjectData.prefabPathIndex];
                        gameObjectCreator.SetInstance(_transforms[dataIndex]);
                        _transforms[dataIndex] = null;
                    }
                }
                continue;
            }

            if (result == Unity.Rendering.FrustumPlanes.IntersectResult.In || mip == 0)
            {
                if (rlt >= 0)
                    continue;

                for (int dataIndex = node.dataStart; dataIndex < node.dataEnd; ++dataIndex)
                {
                    if (_transforms[dataIndex] == null)
                    {
                        rlt = dataIndex;
                        break;
                    }
                }
            }
            else
            {
                rlt = GameObjectVisibility(blockTree, node.childStart, node.childEnd, mip - 1, rlt);
            }
        }
        return rlt;
    }

#endif

    private void Cull(Camera mainCamera)
    {
        _treeCull.Cull(mainCamera, mainCamera.farClipPlane);
#if USE_DYNAMIC_LOAD
        if (_transforms != null)
        {
            int lodCount = math.min(mSceneGameObjectData.gameObjectIndexTree.Length, RenderExtensionSetting.nSceneMaxLOD + 1);
            for (int i = 0; i < lodCount; ++i)
            {
                BlockTree blockTree = mSceneGameObjectData.gameObjectIndexTree[i];
                int dataIndex = GameObjectVisibility(blockTree, blockTree.nodes.Length - blockTree.rootCount, blockTree.nodes.Length, mip - 1, -1);

                if (dataIndex >= 0)
                {
                    GameObjectData gameObjectData = mSceneGameObjectData.gameObjectTramsforms[dataIndex];
                    GameObjectCreator gameObjectCreator = _gameObjectCreators[gameObjectData.prefabPathIndex];
                    _transforms[dataIndex] = gameObjectCreator.GetInstance(_rootTransfrom, gameObjectData.position, gameObjectData.rotation, mSceneGameObjectData.prefabPaths[gameObjectData.prefabPathIndex]);
                    if (_transforms[dataIndex] != null)
                    {
                        _transforms[dataIndex].localScale = gameObjectData.scale;
                    }
                    break;
                }
            }
        }
#endif
    }

#if UNITY_EDITOR && SCENE_DISPLAY
    public bool bDrawScene = true;
    public bool bDrawGizmos = true;
    public int nDrawMip = 0;

    private void OnDrawGizmos()
    {
        if (bDrawGizmos && _treeCull != null)
        {
            _treeCull.DrawGizmos(nDrawMip);
        }
    }
#endif
}