using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Lib;

namespace Framework
{
    public class AssetsPreload
    {
        private const int nStep_None = 0;
        private const int nStep_LoadAssets = 1;
        private const int nStep_LoadScenes = 2;
        private const int nStep_LoadFinish = 3;

        private List<AsyncOperationHandle> mAssetRequests = new List<AsyncOperationHandle>();

        private string _loadSceneName;
        private LoadSceneMode _loadSceneMode;

        private int _sceneLoadedCount = 0;
        private int _assetLoadedCount = 0;

        private int _step = 0;
        
        public float fProgress { get; private set; }

        public void Preload<T>(string name)
        {
            if (_step != 0)
            {
                DebugUtil.LogErrorFormat("Can not load new asset while loading started: {0}", name);
                return;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(name);
            mAssetRequests.Add(handle);
        }

        public void PreOpenScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (_step != 0)
            {
                DebugUtil.LogErrorFormat("Can not load new asset while loading started: {0}", sceneName);
                return;
            }

            _loadSceneName = sceneName;
            _loadSceneMode = loadSceneMode;
        }

        public void StartLoad()
        {
            if (_step != 0)
            {
                DebugUtil.LogErrorFormat("Can not load multiTimes");
                return;
            }

            _step = nStep_LoadAssets;
            fProgress = 0;
        }

        public bool CheckLoaded()
        {
            return _step == nStep_LoadFinish;
        }

        public void OnLoading()
        {
            if (_step == nStep_LoadAssets)
            {
                _assetLoadedCount = RectifyLoadedCount();
                if (_assetLoadedCount >= mAssetRequests.Count)
                {
                    _step = nStep_LoadScenes;
                }
            }
            else if (_step == nStep_LoadScenes)
            {
                if (_sceneLoadedCount < 1 && !string.IsNullOrWhiteSpace(_loadSceneName))
                {
                    ESceneState sceneState = SceneManager.GetSceneState(_loadSceneName);
                    if (sceneState == ESceneState.eNone)
                    {
                        SceneManager.LoadSceneAsync(_loadSceneName, _loadSceneMode, _loadSceneMode == LoadSceneMode.Single);
                    }
                    else if (sceneState == ESceneState.eSuccess)
                    {
                        ++_sceneLoadedCount;
                    }
                }
                else
                {
                    _step = nStep_LoadFinish;
                }
            }

            if (!string.IsNullOrWhiteSpace(_loadSceneName))
            {
                fProgress = (float)(_assetLoadedCount + _sceneLoadedCount) / (float)(mAssetRequests.Count + 1);
            }
            else
            {
                fProgress = (float)_assetLoadedCount / (float)mAssetRequests.Count;
            }
        }

        private int RectifyLoadedCount()
        {
            int count = 0;
            for (int i = 0; i < mAssetRequests.Count; ++i)
            {
                if (mAssetRequests[i].IsDone)
                {
                    ++count;
                }
            }
            return count;
        }

        public void UnLoadAll()
        {
            for (int i = 0; i < mAssetRequests.Count; ++i)
            {
                AsyncOperationHandle handle = mAssetRequests[i];
                Addressables.Release(handle);
            }
            mAssetRequests.Clear();

            fProgress = 0;
            _assetLoadedCount = 0;
            _sceneLoadedCount = 0;
            _loadSceneName = null;
            _step = nStep_None;
        }
    }
}
