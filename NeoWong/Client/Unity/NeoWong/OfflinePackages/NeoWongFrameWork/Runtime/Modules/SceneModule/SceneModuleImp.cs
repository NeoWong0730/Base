using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using YooAsset;

namespace NWFramework
{
    /// <summary>
    /// 场景管理器
    /// </summary>
    internal class SceneModuleImp : ModuleImp, ISceneModule
    {
        private string _currentMainSceneName = string.Empty;

        private SceneOperationHandle _currentMainScene;

        private readonly Dictionary<string, SceneOperationHandle> _subScenes = new Dictionary<string, SceneOperationHandle>();

        /// <summary>
        /// 当前主场景名称
        /// </summary>
        public string CurrentMainSceneName => _currentMainSceneName;

        internal override void Shutdown()
        {
            var iter = _subScenes.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                SceneOperationHandle subScene = iter.Current;
                if (subScene != null)
                {
                    subScene.UnloadAsync();
                }
            }
            iter.Dispose();
            _subScenes.Clear();
            _currentMainSceneName = string.Empty;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">加载完毕时是否主动挂起</param>
        /// <param name="priority">优先级</param>
        /// <param name="callBack">加载回调</param>
        /// <param name="gcCollect">加载主场景是否回收垃圾</param>
        /// <param name="progressCallBack">加载进度回调</param>
        /// <returns>SceneOperationHandle</returns>
        public SceneOperationHandle LoadScene(string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100,
            Action<SceneOperationHandle> callBack = null,
            bool gcCollect = true,
            Action<float> progressCallBack = null)
        {
            if (sceneMode == LoadSceneMode.Additive)
            {
                if (_subScenes.TryGetValue(location, out SceneOperationHandle subScene))
                {
                    Log.Warning($"Could not load subScene while already loaded. Scene: {location}");
                    return subScene;
                }
                subScene = YooAssets.LoadSceneAsync(location, sceneMode, suspendLoad, priority);

                if (callBack != null)
                {
                    subScene.Completed += callBack;
                }

                if (progressCallBack != null)
                {
                    InvokeProgress(subScene, progressCallBack).Forget();
                }
                _subScenes.Add(location, subScene);

                return subScene;
            }
            else
            {
                if (_currentMainScene is { IsDone: false })
                {
                    Log.Warning($"Could not load MainScene while loading. CurrentMainScene: {_currentMainSceneName}.");
                    return null;
                }

                _currentMainSceneName = location;

                _currentMainScene = YooAssets.LoadSceneAsync(location, sceneMode, suspendLoad, priority);

                if (callBack != null)
                {
                    _currentMainScene.Completed += callBack;
                }

                if (progressCallBack != null)
                {
                    InvokeProgress(_currentMainScene, progressCallBack).Forget();
                }
   
                GameModule.Resource.ForceUnloadUnusedAssets(gcCollect);

                return _currentMainScene;
            }
        }

        private async UniTaskVoid InvokeProgress(SceneOperationHandle sceneOperationHandle, Action<float> progress)
        {
            if (sceneOperationHandle == null)
            {
                return;
            }

            while (!sceneOperationHandle.IsDone)
            {
                await UniTask.Yield();

                progress?.Invoke(sceneOperationHandle.Progress);
            }
        }

        /// <summary>
        /// 激活场景（当同时存在多个场景时用于切换激活场景）
        /// </summary>
        /// <param name="location">场景资源定位地址</param>
        /// <returns>是否操作成功</returns>
        public bool ActivateScene(string location)
        {
            if (_currentMainSceneName.Equals(location))
            {
                if (_currentMainScene != null)
                {
                    return _currentMainScene.ActivateScene();
                }
                return false;
            }
            _subScenes.TryGetValue(location, out SceneOperationHandle subScene);
            if (subScene != null)
            {
                return subScene.ActivateScene();
            }
            Log.Warning($"IsMainScene invalid location:{location}");
            return false;
        }

        /// <summary>
        /// 解除场景加载挂起操作
        /// </summary>
        /// <param name="location">场景资源定位地址</param>
        /// <returns>是否操作成功</returns>
        public bool UnSuspend(string location)
        {
            if (_currentMainSceneName.Equals(location))
            {
                if (_currentMainScene != null)
                {
                    return _currentMainScene.UnSuspend();
                }
                return false;
            }
            _subScenes.TryGetValue(location, out SceneOperationHandle subScene);
            if (subScene != null)
            {
                return subScene.UnSuspend();
            }
            Log.Warning($"IsMainScene invalid location:{location}");
            return false;
        }

        /// <summary>
        /// 是否为主场景
        /// </summary>
        /// <param name="location">场景资源定位地址</param>
        /// <returns>是否主场景</returns>
        public bool IsMainScene(string location)
        {
            if (_currentMainSceneName.Equals(location))
            {
                if (_currentMainScene != null)
                {
                    return _currentMainScene.IsMainScene();
                }
                return true;
            }
            _subScenes.TryGetValue(location, out SceneOperationHandle subScene);
            if (subScene != null)
            {
                return subScene.IsMainScene();
            }
            Log.Warning($"IsMainScene invalid location:{location}");
            return false;
        }

        /// <summary>
        /// 异步卸载子场景
        /// </summary>
        /// <param name="location">场景资源定位地址</param>
        /// <returns>场景卸载异步操作类</returns>
        public UnloadSceneOperation UnloadAsync(string location)
        {
            _subScenes.TryGetValue(location, out SceneOperationHandle subScene);
            if (subScene != null)
            {
                if (subScene.SceneObject == default)
                {
                    Log.Error($"Could not unload Scene while not loaded. Scene: {location}");
                    return null;
                }
                _subScenes.Remove(location);
                return subScene.UnloadAsync();
            }
            Log.Warning($"UnloadAsync invalid location:{location}");
            return null;
        }

        /// <summary>
        /// 是否包含场景
        /// </summary>
        /// <param name="location">场景资源定位地址</param>
        /// <returns>是否包含场景</returns>
        public bool IsContainScene(string location)
        {
            if (_currentMainSceneName.Equals(location))
            {
                return true;
            }
            return _subScenes.TryGetValue(location, out var _);
        }
    }

}