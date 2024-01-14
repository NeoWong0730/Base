using Lib.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Framework
{
    public enum ESceneState
    {
        eNone = 1,
        eSceneLoading = 2,          //场景加载中
        eAssetUnloading = 4,        //资源卸载中
        eSuccess = 8,              //场景加载成功
        eFail = 16,                 //场景加载失败
    }

    public class SceneEntry
    {
        private static string sRootName = "Root";

        public readonly string sName;
        internal Scene mScene;
        internal ESceneState eSceneState = ESceneState.eNone;
        private bool _active = false;
        private bool _need = false;        
        private AsyncOperationHandle<SceneInstance> _assetOperation;
        private GameObject _RootGameObject = null;
        
        public bool IsDone { get { return eSceneState == ESceneState.eSuccess || eSceneState == ESceneState.eFail; } }
        public GameObject Root { get { return _RootGameObject; } }

        public SceneEntry(string name)
        {
            sName = name;
        }

        public bool ContainsState(ESceneState sceneState)
        {
            return (eSceneState & sceneState) != 0;
        }
        public void SetActive(bool active)
        {
            _active = active;
            _need |= _active;

            if (eSceneState == ESceneState.eSuccess)
            {
                _refreshDisplay();
            }           
        }

        internal void Load()
        {
            DebugUtil.LogFormat(ELogType.eScene, "{0}.Load()", sName);
            _need = true;
            _loadScene();
        }

        internal void Unload()
        {
            DebugUtil.LogFormat(ELogType.eScene, "{0}.Unload()", sName);

            _active = false;
            _need = false;

            _unloadAsset();
        }

        private void _loadScene()
        {
            if (ESceneState.eNone != eSceneState)
                return;

            eSceneState = ESceneState.eSceneLoading;
            _assetOperation = Addressables.LoadSceneAsync(AssetPath(), LoadSceneMode.Additive, true);
            _assetOperation.Completed += _sceneLoadOperation_completed;
        }

        private void _sceneLoadOperation_completed(AsyncOperationHandle<SceneInstance> obj)
        {
            obj.Completed -= _sceneLoadOperation_completed;

            if (ESceneState.eSceneLoading != eSceneState)
            {
                return;
            }

            eSceneState = obj.Status == AsyncOperationStatus.Succeeded ? ESceneState.eSuccess : ESceneState.eFail;

            if (_need)
            {
                mScene = obj.Result.Scene;
                _parseScene();
                _refreshDisplay();                
            }
            else
            {
                _unloadAsset();
            }
        }

        private void _unloadAsset()
        {
            if (eSceneState == ESceneState.eSuccess || eSceneState == ESceneState.eFail)
            {
                if (_assetOperation.IsValid())
                {
                    eSceneState = ESceneState.eAssetUnloading;
                    _assetOperation = Addressables.UnloadSceneAsync(_assetOperation);
                    _assetOperation.Completed += _assetUnloadOperation_completed;
                }
                else
                {
                    eSceneState = ESceneState.eNone;
                }
            }
        }

        private void _assetUnloadOperation_completed(AsyncOperationHandle<SceneInstance> obj)
        {
            if (eSceneState == ESceneState.eAssetUnloading)
            {
                eSceneState = ESceneState.eNone;
            }

            if (_need)
            {
                _loadScene();
            }
        }

        private void _refreshDisplay()
        {
            _RootGameObject?.SetActive(_active);
        }

        private void _parseScene()
        {
            if (mScene == null)
                return;

            List<GameObject> rootGameObjects = new List<GameObject>(mScene.rootCount);
            mScene.GetRootGameObjects(rootGameObjects);

            for (int i = 0; i < rootGameObjects.Count; ++i)
            {
                if (string.Equals(rootGameObjects[i].name, sRootName, StringComparison.Ordinal))
                {
                    _RootGameObject = rootGameObjects[i];
                    break;
                }
            }

            //TODO : 暂时写死打开昼夜系统
            if (_RootGameObject != null)
            {
                DayAndNightDynamic dayAndNightDynamic = _RootGameObject.GetComponentInChildren<DayAndNightDynamic>(true);
                if (dayAndNightDynamic != null)
                {
                    dayAndNightDynamic.enabled = true;
                    dayAndNightDynamic.gameObject.SetActive(true);
                }
                else
                {
                    Light mainLight = _RootGameObject.FindChildByName("MainLight", false)?.GetComponent<Light>();
                    if (mainLight)
                    {
                        mainLight.shadowStrength = 0.85f;
                    }
                }
            }
            //====
        }

        private string AssetPath()
        {
            return sName;
            //return string.Format("Scene/{0}.unity", sName);
        }
    }
}