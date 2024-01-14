using System;
using UnityEngine.SceneManagement;
using YooAsset;

namespace NWFramework
{
    /// <summary>
    /// 场景管理模块
    /// </summary>
    public sealed class SceneModule : Module
    {
        private ISceneModule _sceneModule;

        private void Start()
        {
            RootModule baseComponent = ModuleSystem.GetModule<RootModule>();
            if (baseComponent == null)
            {
                Log.Fatal("Root module is invalid.");
                return;
            }

            _sceneModule = ModuleImpSystem.GetModule<SceneModuleImp>();
            if (_sceneModule == null)
            {
                Log.Fatal("SceneModule is invalid.");
                return;
            }
        }

        /// <summary>
        /// 当前主场景名称
        /// </summary>
        public string CurrentMainSceneName => _sceneModule.CurrentMainSceneName;

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
        /// <returns></returns>
        public SceneOperationHandle LoadScene(string location, LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false, int priority = 100, Action<SceneOperationHandle> callBack = null,
            bool gcCollect = true, Action<float> progressCallBack = null)
        {
            return _sceneModule.LoadScene(location, sceneMode, suspendLoad, priority, callBack, gcCollect, progressCallBack);
        }

        /// <summary>
        /// 加载子场景
        /// </summary>
        /// <param name="location">场景的定位地址/param>
        /// <param name="suspendLoad">加载完毕时是否主动挂起</param>
        /// <param name="priority">优先级</param>
        /// <param name="callBack">加载回调</param>
        /// <param name="gcCollect">加载主场景是否回收垃圾</param>
        /// <param name="progressCallBack">加载进度回调</param>
        /// <returns></returns>
        public SceneOperationHandle LoadSubScene(string location, bool suspendLoad = false, int priority = 100,
            Action<SceneOperationHandle> callBack = null, bool gcCollect = true, 
            Action<float> progressCallBack = null)
        {
            return _sceneModule.LoadScene(location, LoadSceneMode.Additive, suspendLoad, priority, callBack, gcCollect, progressCallBack);
        }

        /// <summary>
        /// 激活场景（当同时存在多个场景时用于切换激活场景）
        /// </summary>
        /// <param name="location">场景资源定位地址</param>
        /// <returns>是否操作成功</returns>
        public bool ActivateScene(string location)
        {
            return _sceneModule.ActivateScene(location);
        }

        /// <summary>
        /// 解除场景加载挂起操作
        /// </summary>
        /// <param name="location">场景资源定位地址</param>
        /// <returns>是否操作成功</returns>
        public bool UnSuspend(string location)
        {
            return _sceneModule.UnSuspend(location);
        }

        /// <summary>
        /// 是否为主场景
        /// </summary>
        /// <param name="location">场景资源定位地址</param>
        /// <returns>是否主场景</returns>
        public bool IsMainScene(string location)
        {
            return _sceneModule.IsMainScene(location);
        }

        /// <summary>
        /// 异步卸载子场景
        /// </summary>
        /// <param name="location">场景资源定位地址</param>
        /// <returns>场景卸载异步操作类</returns>
        public UnloadSceneOperation UnloadAsync(string location)
        {
            return _sceneModule.UnloadAsync(location);
        }

        /// <summary>
        /// 是否包含场景
        /// </summary>
        /// <param name="location">场景资源定位地址</param>
        /// <returns>是否包含场景</returns>
        public bool IsContainScene(string location)
        {
            return _sceneModule.IsContainScene(location);
        }
    }
}