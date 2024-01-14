using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace NWFramework
{
    /// <summary>
    /// 资源管理模块
    /// </summary>
    [UpdateModule]
    internal partial class ResourceManager : ModuleImp, IResourceManager
    {
        #region Propreties

        /// <summary>
        /// 默认资源包名称
        /// </summary>
        public string PackageName { get; set; } = "DefaultPackage";

        /// <summary>
        /// 默认资源包。
        /// </summary>
        public ResourcePackage DefaultPackage { private set; get; }

        /// <summary>
        /// 获取当前资源适用的游戏版本号
        /// </summary>
        public string ApplicableGameVersion => _applicableGameVersion;

        private string _applicableGameVersion;

        /// <summary>
        /// 获取当前内部资源版本号
        /// </summary>
        public int InternalResourceVersion => _internalResourceVersion;

        private int _internalResourceVersion;

        /// <summary>
        /// 同时下载的最大数目
        /// </summary>
        public int DownloadingMaxNum { get; set; }

        /// <summary>
        /// 失败重试最大数目
        /// </summary>
        public int FailedTryAgain { get; set; }

        /// <summary>
        /// 获取资源只读区路径
        /// </summary>
        public string ReadOnlyPath => _readOnlyPath;

        private string _readOnlyPath;

        /// <summary>
        /// 获取资源读写区路径
        /// </summary>
        public string ReadWritePath => _readWritePath;

        private string _readWritePath;

        /// <summary>
        /// 资源系统运行模式
        /// </summary>
        public EPlayMode PlayMode { get; set; }

        /// <summary>
        /// 下载文件校验等级
        /// </summary>
        public EVerifyLevel VerifyLevel { get; set; }

        /// <summary>
        /// 设置异步系统参数，每帧执行消耗的最大时间切片（单位：毫秒）
        /// </summary>
        public long Milliseconds { get; set; }

        /// <summary>
        /// 获取游戏框架模块优先级
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行</remarks>
        internal override int Priority => 4;

        /// <summary>
        /// 资源缓存表容量
        /// </summary>
        public int ARCTableCapacity { get; set; }

        /// <summary>
        /// 是否开启对象池
        /// </summary>
        public static bool EnableGoPool { get; set; } = true;

        private readonly Dictionary<string, AssetInfo> _assetInfoMap = new Dictionary<string, AssetInfo>();

        private static readonly Type _typeOfGameObject = typeof(GameObject);

        #endregion

        #region 生命周期

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            ResourcePool.Instance.OnUpdate();
        }

        internal override void Shutdown()
        {
            _assetInfoMap.Clear();

            ReleasePreLoadAssets(isShutDown: true);
#if !UNITY_WEBGL
            YooAssets.Destroy();
#endif
            ResourcePool.Instance.OnDestroy();
        }

        private void ReleaseAllHandle()
        {
            var iter = _releaseMaps.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                AssetOperationHandle handle = iter.Current;
                if (handle is { IsValid: true })
                {
                    handle.Dispose();
                    handle = null;
                }
            }

            iter.Dispose();
            _releaseMaps.Clear();

            iter = _operationHandlesMaps.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                AssetOperationHandle handle = iter.Current;
                if (handle is { IsValid: true })
                {
                    handle.Dispose();
                    handle = null;
                }
            }

            iter.Dispose();
            _operationHandlesMaps.Clear();

            _arcCacheTable =
                new ArcCacheTable<string, AssetOperationHandle>(ARCTableCapacity, OnAddAsset, OnRemoveAsset);
        }

        #endregion

        #region 设置接口

        /// <summary>
        /// 设置资源只读区路径
        /// </summary>
        /// <param name="readOnlyPath">资源只读区路径</param>
        public void SetReadOnlyPath(string readOnlyPath)
        {
            if (string.IsNullOrEmpty(readOnlyPath))
            {
                throw new NWFrameworkException("Read-only path is invalid.");
            }

            _readOnlyPath = readOnlyPath;
        }

        /// <summary>
        /// 设置资源读写区路径
        /// </summary>
        /// <param name="readWritePath">资源读写区路径</param>
        public void SetReadWritePath(string readWritePath)
        {
            if (string.IsNullOrEmpty(readWritePath))
            {
                throw new NWFrameworkException("Read-write path is invalid.");
            }

            _readWritePath = readWritePath;
        }

        #endregion

        private Dictionary<string, AssetOperationHandle> _releaseMaps;

        private Dictionary<string, AssetOperationHandle> _operationHandlesMaps;

        private ArcCacheTable<string, AssetOperationHandle> _arcCacheTable;


        private void OnAddAsset(string location, AssetOperationHandle handle)
        {
            _operationHandlesMaps[location] = handle;
            if (_releaseMaps.ContainsKey(location))
            {
                _releaseMaps.Remove(location);
            }
        }

        private void OnRemoveAsset(string location, AssetOperationHandle handle)
        {
            if (_operationHandlesMaps.ContainsKey(location))
            {
                _operationHandlesMaps.Remove(location);
            }

            _releaseMaps[location] = handle;
            GameModule.Resource.UnloadUnusedAssets(performGCCollect: false);
        }

        /// <summary>
        /// 从缓存中获取同步资源句柄
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <param name="needCache">是否需要缓存</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>资源句柄</returns>
        private AssetOperationHandle GetHandleSync<T>(string location, bool needCache = false, string packageName = "")
            where T : Object
        {
            if (!needCache)
            {
                if (string.IsNullOrEmpty(packageName))
                {
                    return YooAssets.LoadAssetSync<T>(location);
                }

                var package = YooAssets.GetPackage(packageName);
                return package.LoadAssetSync<T>(location);
            }

            // 缓存key
            var cacheKey = string.IsNullOrEmpty(packageName) || packageName.Equals(PackageName)
                ? location
                : $"{packageName}/{location}";

            AssetOperationHandle handle = null;
            // 尝试从从ARC缓存表取出对象
            handle = _arcCacheTable.GetCache(cacheKey);

            if (handle == null)
            {
                if (string.IsNullOrEmpty(packageName))
                {
                    handle = YooAssets.LoadAssetSync<T>(location);
                }
                else
                {
                    var package = YooAssets.GetPackage(packageName);
                    handle = package.LoadAssetSync<T>(location);
                }
            }

            // 对象推入ARC缓存表
            _arcCacheTable.PutCache(cacheKey, handle);
            return handle;
        }

        /// <summary>
        /// 从缓存中获取异步资源句柄
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <param name="needCache">是否需要缓存</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>资源句柄</returns>
        private AssetOperationHandle GetHandleAsync<T>(string location, bool needCache = false, string packageName = "")
            where T : Object
        {
            if (!needCache)
            {
                if (string.IsNullOrEmpty(packageName))
                {
                    return YooAssets.LoadAssetAsync<T>(location);
                }

                var package = YooAssets.GetPackage(packageName);
                return package.LoadAssetAsync<T>(location);
            }

            // 缓存key
            var cacheKey = string.IsNullOrEmpty(packageName) || packageName.Equals(PackageName)
                ? location
                : $"{packageName}/{location}";

            AssetOperationHandle handle = null;
            // 尝试从从ARC缓存表取出对象
            handle = _arcCacheTable.GetCache(cacheKey);

            if (handle == null)
            {
                if (string.IsNullOrEmpty(packageName))
                {
                    handle = YooAssets.LoadAssetAsync<T>(location);
                }
                else
                {
                    var package = YooAssets.GetPackage(packageName);
                    handle = package.LoadAssetAsync<T>(location);
                }
            }

            // 对象推入ARC缓存表
            _arcCacheTable.PutCache(cacheKey, handle);
            return handle;
        }

        /// <summary>
        /// 初始化资源模块
        /// </summary>
        public void Initialize()
        {
            // 初始化资源系统
            YooAssets.Initialize(new AssetsLogger());
            YooAssets.SetOperationSystemMaxTimeSlice(Milliseconds);
            YooAssets.SetCacheSystemCachedFileVerifyLevel(VerifyLevel);

            // 创建默认的资源包
            string packageName = PackageName;
            var defaultPackage = YooAssets.TryGetPackage(packageName);
            if (defaultPackage == null)
            {
                defaultPackage = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(defaultPackage);
            }

            ResourcePool.Instance.OnAwake();

            _releaseMaps ??= new Dictionary<string, AssetOperationHandle>(ARCTableCapacity);
            _operationHandlesMaps ??= new Dictionary<string, AssetOperationHandle>(ARCTableCapacity);
            _arcCacheTable ??=
                new ArcCacheTable<string, AssetOperationHandle>(ARCTableCapacity, OnAddAsset, OnRemoveAsset);
        }

        /// <summary>
        /// 初始化资源包裹
        /// </summary>
        /// <returns>初始化资源包裹操作句柄</returns>
        public InitializationOperation InitPackage(string customPackageName = "")
        {
            // 创建默认的资源包
            var targetPackageName = string.IsNullOrEmpty(customPackageName) || customPackageName.Equals(PackageName)
                ? PackageName
                : customPackageName;
            var package = YooAssets.TryGetPackage(targetPackageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(targetPackageName);
            }

            // 设置默认资源包
            if (targetPackageName.Equals(PackageName))
            {
                YooAssets.SetDefaultPackage(package);
                DefaultPackage = package;
            }

#if UNITY_EDITOR
            //编辑器模式使用
            EPlayMode playMode = (EPlayMode)UnityEditor.EditorPrefs.GetInt("EditorResourceMode");
            Log.Warning($"编辑器模式使用:{playMode}");
#else
            //运行时使用。
            EPlayMode playMode = PlayMode;
#endif

            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                var createParameters = new EditorSimulateModeParameters();
                createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(targetPackageName);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 单机运行模式
            if (playMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                createParameters.DecryptionServices = new GameDecryptionServices();
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // 联机运行模式
            if (playMode == EPlayMode.HostPlayMode)
            {
                var createParameters = new HostPlayModeParameters();
                createParameters.DecryptionServices = new GameDecryptionServices();
                createParameters.BuildinQueryServices = new BuiltinQueryServices();
                createParameters.DeliveryQueryServices = new DefaultDeliveryQueryServices();
                createParameters.RemoteServices = new RemoteServices(targetPackageName);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // WebGL运行模式
            if (playMode == EPlayMode.WebPlayMode)
            {
                YooAssets.SetCacheSystemDisableCacheOnWebGL();
                var createParameters = new WebPlayModeParameters();
                createParameters.DecryptionServices = new GameDecryptionServices();
                createParameters.BuildinQueryServices = new WebGLBuiltinQueryServices();
                createParameters.RemoteServices = new RemoteServices(targetPackageName);
                // WebGL运行模式下，直接使用远程热更资源
                createParameters.BuildinRootDirectory = SettingsUtils.FrameworkGlobalSettings.HostServerURL;
                createParameters.SandboxRootDirectory = SettingsUtils.FrameworkGlobalSettings.HostServerURL;
                initializationOperation = package.InitializeAsync(createParameters);
            }

            return initializationOperation;
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="asset">要卸载的资源实例</param>
        /// <exception cref="NWFrameworkException">游戏框架异常类 - 未实现</exception>
        public void UnloadAsset(object asset)
        {
            throw new NWFrameworkException("System.NotImplementedException.");
        }

        /// <summary>
        /// 释放游戏物体
        /// </summary>
        /// <param name="gameObject">游戏物体</param>
        /// <param name="forceNoPool">强制不入回收池</param>
        /// <param name="delayTime">延迟时间</param>
        public void FreeGameObject(GameObject gameObject, bool forceNoPool = false, float delayTime = 0f)
        {
            if (Application.isPlaying)
            {
                if (EnableGoPool && ResourcePool.Instance.IsNeedRecycle(gameObject, out GoProperty property, forceNoPool))
                {
                    if (delayTime > 0f)
                    {
                        ResourcePool.Instance.DelayDestroy(gameObject, property, delayTime);
                        return;
                    }

                    ResourcePool.Instance.AddCacheGo(property.ResPath, gameObject, property);
                }
                else
                {
                    if (delayTime > 0f)
                    {
                        Object.Destroy(gameObject, delayTime);
                        return;
                    }

                    Object.Destroy(gameObject);
                }
            }
            else
            {
                if (delayTime > 0f)
                {
                    Object.Destroy(gameObject, delayTime);
                    return;
                }

                Object.Destroy(gameObject);
            }
        }

        private void AddRecycleGoProperty(string location, GameObject go, Vector3 initScale)
        {
            bool flag = ResourceCacheMgr.Instance.IsNeedCache(location, out int _, out int maxPoolCnt);
            if (!(EnableGoPool & flag) || maxPoolCnt <= 0)
            {
                return;
            }

            ResourcePool.Instance.AddNewRecycleProperty(go, location, initScale);
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        public void UnloadUnusedAssets()
        {
            var iter = _releaseMaps.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                AssetOperationHandle handle = iter.Current;
                if (handle is { IsValid: true })
                {
                    handle.Dispose();
                    handle = null;
                }
            }

            iter.Dispose();
            _releaseMaps.Clear();

            if (DefaultPackage == null)
            {
                throw new NWFrameworkException("Package is invalid.");
            }

            DefaultPackage.UnloadUnusedAssets();
        }

        /// <summary>
        /// 强制回收所有资源
        /// </summary>
        public void ForceUnloadAllAssets()
        {
#if UNITY_WEBGL
			return;
#else
            if (DefaultPackage == null)
            {
                throw new NWFrameworkException("Package is invalid.");
            }

            DefaultPackage.ForceUnloadAllAssets();
#endif
        }


        public HasAssetResult HasAsset(string location, string packageName = "")
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new NWFrameworkException("Asset name is invalid.");
            }

            AssetInfo assetInfo = GetAssetInfo(location, packageName);

            if (!CheckLocationValid(location))
            {
                return HasAssetResult.Valid;
            }

            if (assetInfo == null)
            {
                return HasAssetResult.NotExist;
            }

            if (IsNeedDownloadFromRemote(assetInfo))
            {
                return HasAssetResult.AssetOnline;
            }

            return HasAssetResult.AssetOnDisk;
        }

        /// <summary>
        /// 设置默认的资源包
        /// </summary>
        public void SetDefaultPackage(ResourcePackage package)
        {
            YooAssets.SetDefaultPackage(package);
            DefaultPackage = package;
        }

        #region 资源信息

        public bool IsNeedDownloadFromRemote(string location, string packageName = "")
        {
            if (string.IsNullOrEmpty(packageName))
            {
                return YooAssets.IsNeedDownloadFromRemote(location);
            }
            else
            {
                var package = YooAssets.GetPackage(packageName);
                return package.IsNeedDownloadFromRemote(location);
            }
        }


        public bool IsNeedDownloadFromRemote(AssetInfo assetInfo, string packageName = "")
        {
            if (string.IsNullOrEmpty(packageName))
            {
                return YooAssets.IsNeedDownloadFromRemote(assetInfo);
            }
            else
            {
                var package = YooAssets.GetPackage(packageName);
                return package.IsNeedDownloadFromRemote(assetInfo);
            }
        }


        public AssetInfo[] GetAssetInfos(string tag, string packageName = "")
        {
            if (string.IsNullOrEmpty(packageName))
            {
                return YooAssets.GetAssetInfos(tag);
            }
            else
            {
                var package = YooAssets.GetPackage(packageName);
                return package.GetAssetInfos(tag);
            }
        }


        public AssetInfo[] GetAssetInfos(string[] tags, string packageName = "")
        {
            if (string.IsNullOrEmpty(packageName))
            {
                return YooAssets.GetAssetInfos(tags);
            }
            else
            {
                var package = YooAssets.GetPackage(packageName);
                return package.GetAssetInfos(tags);
            }
        }


        public AssetInfo GetAssetInfo(string location, string packageName = "")
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new NWFrameworkException("Asset name is invalid.");
            }

            if (string.IsNullOrEmpty(packageName))
            {
                if (_assetInfoMap.TryGetValue(location, out AssetInfo assetInfo))
                {
                    return assetInfo;
                }

                assetInfo = YooAssets.GetAssetInfo(location);
                _assetInfoMap[location] = assetInfo;
                return assetInfo;
            }
            else
            {
                string key = $"{packageName}/{location}";
                if (_assetInfoMap.TryGetValue(key, out AssetInfo assetInfo))
                {
                    return assetInfo;
                }

                var package = YooAssets.GetPackage(packageName);
                if (package == null)
                {
                    throw new NWFrameworkException($"The package does not exist. Package Name :{packageName}");
                }

                assetInfo = package.GetAssetInfo(location);
                _assetInfoMap[key] = assetInfo;
                return assetInfo;
            }
        }


        public bool CheckLocationValid(string location, string packageName = "")
        {
            if (string.IsNullOrEmpty(packageName))
            {
                return YooAssets.CheckLocationValid(location);
            }
            else
            {
                var package = YooAssets.GetPackage(packageName);
                return package.CheckLocationValid(location);
            }
        }

        #endregion

        public T LoadAsset<T>(string location, bool needInstance = true, bool needCache = false, string packageName = "") where T : Object
        {
            return LoadAsset<T>(location, parent: null, needInstance, needCache, packageName);
        }

        public T LoadAsset<T>(string location, Transform parent, bool needInstance = true, bool needCache = false, string packageName = "")
            where T : Object
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("Asset name is invalid.");
                return default;
            }

            Type assetType = typeof(T);

            if (EnableGoPool && assetType == _typeOfGameObject)
            {
                GameObject go = ResourcePool.Instance.AllocCacheGoByLocation(location, parentTrans: parent);

                if (go != null)
                {
                    return go as T;
                }
            }

            AssetOperationHandle handle = GetHandleSync<T>(location, needCache, packageName: packageName);

            AssetInfo assetInfo = GetAssetInfo(location, packageName);
            if (EnableGoPool && handle.AssetObject != null)
            {
                if (ResourceCacheMgr.Instance.IsNeedCache(assetInfo.AssetPath, out var cacheTime))
                {
                    ResourceCacheMgr.Instance.AddCache(assetInfo.AssetPath, handle.AssetObject, cacheTime);
                }
            }

            if (assetType == _typeOfGameObject)
            {
                if (needInstance)
                {
                    GameObject gameObject = handle.InstantiateSync(parent);
                    if (!needCache)
                    {
                        AssetReference.BindAssetReference(gameObject, handle, location, packageName: packageName);
                    }

                    if (EnableGoPool)
                    {
                        AddRecycleGoProperty(assetInfo.AssetPath, gameObject, gameObject.transform.localScale);
                    }

                    return gameObject as T;
                }
            }

            T ret = handle.AssetObject as T;
            if (!needCache)
            {
                handle.Dispose();
            }

            return ret;
        }

        public T LoadAsset<T>(string location, out AssetOperationHandle handle, bool needCache = false,
            string packageName = "") where T : Object
        {
            return LoadAsset<T>(location, null, out handle, needCache, packageName);
        }

        public T LoadAsset<T>(string location, Transform parent, out AssetOperationHandle handle,
            bool needCache = false, string packageName = "") where T : Object
        {
            handle = GetHandleSync<T>(location, needCache, packageName: packageName);

            if (string.IsNullOrEmpty(location))
            {
                Log.Error("Asset name is invalid.");
                return default;
            }

            if (typeof(T) == _typeOfGameObject)
            {
                GameObject gameObject = handle.InstantiateSync(parent);
                if (!needCache)
                {
                    AssetReference.BindAssetReference(gameObject, handle, location, packageName: packageName);
                }

                return gameObject as T;
            }

            T ret = handle.AssetObject as T;
            if (!needCache)
            {
                handle.Dispose();
            }

            return ret;
        }

        public AssetOperationHandle LoadAssetGetOperation<T>(string location, bool needCache = false,
            string packageName = "") where T : Object
        {
            return GetHandleSync<T>(location, needCache, packageName: packageName);
        }

        public AssetOperationHandle LoadAssetAsyncHandle<T>(string location, bool needCache = false,
            string packageName = "") where T : Object
        {
            return GetHandleAsync<T>(location, needCache, packageName: packageName);
        }

        public SubAssetsOperationHandle LoadSubAssetsSync<TObject>(string location, string packageName = "")
            where TObject : Object
        {
            if (string.IsNullOrEmpty(packageName))
            {
                return YooAssets.LoadSubAssetsSync<TObject>(location: location);
            }

            var package = YooAssets.GetPackage(packageName);
            return package.LoadSubAssetsSync<TObject>(location);
        }

        public SubAssetsOperationHandle LoadSubAssetsAsync<TObject>(string location, string packageName = "")
            where TObject : Object
        {
            if (string.IsNullOrEmpty(packageName))
            {
                return YooAssets.LoadSubAssetsAsync<TObject>(location: location);
            }

            var package = YooAssets.GetPackage(packageName);
            return package.LoadSubAssetsAsync<TObject>(location: location);
        }

        public SubAssetsOperationHandle LoadSubAssetsSync(AssetInfo assetInfo, string packageName = "")
        {
            if (string.IsNullOrEmpty(packageName))
            {
                return YooAssets.LoadSubAssetsSync(assetInfo);
            }

            var package = YooAssets.GetPackage(packageName);
            return package.LoadSubAssetsSync(assetInfo);
        }

        public async UniTask<LoadAssetsByTagOperation<T>> LoadAssetsByTagAsync<T>(string assetTag, string packageName = "")
            where T : UnityEngine.Object
        {
            LoadAssetsByTagOperation<T> operation = new LoadAssetsByTagOperation<T>(assetTag, packageName);
            YooAssets.StartOperation(operation);
            await operation.ToUniTask();
            return operation;
        }

        public async UniTask<T> LoadAssetAsync<T>(string location, CancellationToken cancellationToken = default,
            bool needInstance = true, bool needCache = false, string packageName = "", Transform parent = null) where T : Object
        {
            if (string.IsNullOrEmpty(location))
            {
                Log.Error("Asset name is invalid.");
                return default;
            }

            Type assetType = typeof(T);

            if (EnableGoPool && assetType == _typeOfGameObject)
            {
                GameObject go = ResourcePool.Instance.AllocCacheGoByLocation(location, parentTrans: parent);

                if (go != null)
                {
                    return go as T;
                }
            }

            AssetOperationHandle handle = LoadAssetAsyncHandle<T>(location, needCache, packageName: packageName);

            bool cancelOrFailed = await handle.ToUniTask().AttachExternalCancellation(cancellationToken)
                .SuppressCancellationThrow();

            if (cancelOrFailed)
            {
                return null;
            }

            AssetInfo assetInfo = GetAssetInfo(location, packageName);
            if (EnableGoPool && handle.AssetObject != null)
            {
                if (ResourceCacheMgr.Instance.IsNeedCache(assetInfo.AssetPath, out var cacheTime))
                {
                    ResourceCacheMgr.Instance.AddCache(assetInfo.AssetPath, handle.AssetObject, cacheTime);
                }
            }

            if (typeof(T) == _typeOfGameObject)
            {
                if (needInstance)
                {
                    GameObject gameObject = handle.InstantiateSync(parent);
                    if (!needCache)
                    {
                        AssetReference.BindAssetReference(gameObject, handle, location, packageName: packageName);
                    }

                    if (EnableGoPool)
                    {
                        AddRecycleGoProperty(assetInfo.AssetPath, gameObject, gameObject.transform.localScale);
                    }

                    return gameObject as T;
                }
            }

            T ret = handle.AssetObject as T;
            if (!needCache)
            {
                handle.Dispose();
            }

            return ret;
        }

        public async UniTask<GameObject> LoadGameObjectAsync(string location,
            CancellationToken cancellationToken = default, bool needCache = false, string packageName = "")
        {
            return await LoadGameObjectAsync(location, null, cancellationToken, needCache, packageName);
        }

        public async UniTask<GameObject> LoadGameObjectAsync(string location, Transform parent,
            CancellationToken cancellationToken = default, bool needCache = false, string packageName = "")
        {
            if (EnableGoPool)
            {
                GameObject go = ResourcePool.Instance.AllocCacheGoByLocation(location, parentTrans: parent);

                if (go != null)
                {
                    return go;
                }
            }

            GameObject gameObject = await LoadAssetAsync<GameObject>(location, cancellationToken, needInstance: true, needCache: needCache, packageName: packageName, parent: parent);

            return gameObject;
        }

        public async UniTask<RawFileOperationHandle> LoadRawAssetAsync(string location,
            CancellationToken cancellationToken = default, string packageName = "")
        {
            RawFileOperationHandle handle;
            if (string.IsNullOrEmpty(packageName))
            {
                handle = YooAssets.LoadRawFileAsync(location);
            }
            else
            {
                var package = YooAssets.GetPackage(packageName);
                handle = package.LoadRawFileAsync(location);
            }

            bool cancelOrFailed = await handle.ToUniTask().AttachExternalCancellation(cancellationToken)
                .SuppressCancellationThrow();

            return cancelOrFailed ? null : handle;
        }

        public async UniTask<T> LoadSubAssetAsync<T>(string location, string assetName,
            CancellationToken cancellationToken = default, string packageName = "") where T : Object
        {
            var assetInfo = GetAssetInfo(location, packageName: packageName);
            if (assetInfo == null)
            {
                Log.Fatal($"AssetsInfo is null");
                return null;
            }

            SubAssetsOperationHandle handle;
            if (string.IsNullOrEmpty(packageName))
            {
                handle = YooAssets.LoadSubAssetsAsync(assetInfo);
            }
            else
            {
                var package = YooAssets.GetPackage(packageName);
                handle = package.LoadSubAssetsAsync(assetInfo);
            }

            bool cancelOrFailed = await handle.ToUniTask().AttachExternalCancellation(cancellationToken)
                .SuppressCancellationThrow();

            handle.Dispose();

            return cancelOrFailed ? null : handle.GetSubAssetObject<T>(assetName);
        }

        public async UniTask<T[]> LoadAllSubAssetAsync<T>(string location,
            CancellationToken cancellationToken = default, string packageName = "") where T : Object
        {
            var assetInfo = GetAssetInfo(location, packageName: packageName);
            if (assetInfo == null)
            {
                Log.Fatal($"AssetsInfo is null");
                return null;
            }

            SubAssetsOperationHandle handle;
            if (string.IsNullOrEmpty(packageName))
            {
                handle = YooAssets.LoadSubAssetsAsync(assetInfo);
            }
            else
            {
                var package = YooAssets.GetPackage(packageName);
                handle = package.LoadSubAssetsAsync(assetInfo);
            }

            bool cancelOrFailed = await handle.ToUniTask().AttachExternalCancellation(cancellationToken)
                .SuppressCancellationThrow();

            handle.Dispose();

            return cancelOrFailed ? null : handle.GetSubAssetObjects<T>();
        }

        #region 预加载

        private readonly Dictionary<string, Object> _preLoadMaps = new Dictionary<string, Object>();

        public void PushPreLoadAsset(string location, Object assetObject, string packageName = "")
        {
            var cacheKey = string.IsNullOrEmpty(packageName) || packageName.Equals(PackageName)
                ? location
                : $"{packageName}/{location}";
            if (_preLoadMaps.ContainsKey(cacheKey))
            {
                return;
            }

            _preLoadMaps.Add(cacheKey, assetObject);
        }

        public T GetPreLoadAsset<T>(string location, string packageName = "") where T : Object
        {
            var cacheKey = string.IsNullOrEmpty(packageName) || packageName.Equals(PackageName)
                ? location
                : $"{packageName}/{location}";
            if (_preLoadMaps.TryGetValue(cacheKey, out Object assetObject))
            {
                return assetObject as T;
            }

            return default;
        }

        private void ReleasePreLoadAssets(bool isShutDown = false)
        {
            if (!isShutDown)
            {
                using var iter = _preLoadMaps.GetEnumerator();
                while (iter.MoveNext())
                {
                    var assetObject = iter.Current.Value;
                    if (assetObject != null)
                    {
                        Object.Destroy(assetObject);
                    }
                }
            }

            _preLoadMaps.Clear();
        }

        #endregion
    }
}