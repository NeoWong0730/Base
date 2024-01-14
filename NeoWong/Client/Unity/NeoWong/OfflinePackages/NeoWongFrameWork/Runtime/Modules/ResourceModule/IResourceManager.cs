using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace NWFramework
{
    /// <summary>
    /// 资源管理器接口
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// 获取当前资源适用的游戏版本号
        /// </summary>
        string ApplicableGameVersion { get; }

        /// <summary>
        /// 获取当前内部资源版本号
        /// </summary>
        int InternalResourceVersion { get; }

        /// <summary>
        /// 同时下载的最大数目
        /// </summary>
        int DownloadingMaxNum { get; set; }

        /// <summary>
        /// 失败重试最大数目
        /// </summary>
        int FailedTryAgain { get; set; }

        /// <summary>
        /// 获取资源只读区路径
        /// </summary>
        string ReadOnlyPath { get; }

        /// <summary>
        /// 获取资源读写区路径
        /// </summary>
        string ReadWritePath { get; }

        /// <summary>
        /// 获取或设置资源包名称
        /// </summary>
        string PackageName { get; set; }

        /// <summary>
        /// 获取或设置运行模式
        /// </summary>
        EPlayMode PlayMode { get; set; }

        /// <summary>
        /// 获取或设置下载文件校验等级
        /// </summary>
        EVerifyLevel VerifyLevel { get; set; }

        /// <summary>
        /// 获取或设置异步系统参数，每帧执行消耗的最大时间切片（单位：毫秒）
        /// </summary>
        long Milliseconds { get; set; }

        /// <summary>
        /// 资源缓存表容量
        /// </summary>
        int ARCTableCapacity { get; set; }

        /// <summary>
        /// 设置资源只读区路径
        /// </summary>
        /// <param name="readOnlyPath">资源只读区路径</param>
        void SetReadOnlyPath(string readOnlyPath);

        /// <summary>
        /// 设置资源读写区路径
        /// </summary>
        /// <param name="readWritePath">资源读写区路径</param>
        void SetReadWritePath(string readWritePath);

        /// <summary>
        /// 初始化接口
        /// </summary>
        void Initialize();

        /// <summary>
        /// 初始化操作
        /// </summary>
        /// <param name="customPackageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns></returns>
        InitializationOperation InitPackage(string customPackageName = "");

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="asset">要卸载的资源</param>
        void UnloadAsset(object asset);

        /// <summary>
        /// 释放游戏物体
        /// </summary>
        /// <param name="gameObject">游戏物体</param>
        /// <param name="forceNoPool">强制不入回收池</param>
        /// <param name="delayTime">延迟时间</param>
        void FreeGameObject(GameObject gameObject, bool forceNoPool = false, float delayTime = 0f);

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        void UnloadUnusedAssets();

        /// <summary>
        /// 强制回收所有资源
        /// </summary>
        void ForceUnloadAllAssets();

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        /// <param name="location">要检查资源的名称</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>检查资源是否存在的结果</returns>
        HasAssetResult HasAsset(string location, string packageName = "");

        /// <summary>
        /// 设置默认资源包
        /// </summary>
        /// <param name="package">资源包</param>
        void SetDefaultPackage(ResourcePackage package);

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>是否需要从远端下载</returns>
        bool IsNeedDownloadFromRemote(string location, string packageName = "");

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>是否需要从远端下载</returns>
        bool IsNeedDownloadFromRemote(AssetInfo assetInfo, string packageName = "");

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="tag">资源标签</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>资源信息列表</returns>
        AssetInfo[] GetAssetInfos(string tag, string packageName = "");

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="tags">资源标签列表</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>资源信息列表</returns>
        AssetInfo[] GetAssetInfos(string[] tags, string packageName = "");

        /// <summary>
        /// 获取资源信息
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>资源信息</returns>
        AssetInfo GetAssetInfo(string location, string packageName = "");

        /// <summary>
        /// 检查资源定位地址是否有效
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>资源定位地址是否有效</returns>
        bool CheckLocationValid(string location, string packageName = "");

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">要加载资源的类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="needInstance">是否需要实例化</param>
        /// <param name="needCache">是否需要缓存</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>资源实例</returns>
        T LoadAsset<T>(string location, bool needInstance, bool needCache = false, string packageName = "")
            where T : UnityEngine.Object;

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">要加载资源的类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父节点位置</param>
        /// <param name="needInstance">是否需要实例化</param>
        /// <param name="needCache">是否需要缓存</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>资源实例</returns>
        T LoadAsset<T>(string location, Transform parent, bool needInstance, bool needCache = false, 
            string packageName = "") where T : UnityEngine.Object;

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">要加载资源的类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="handle">资源操作句柄</param>
        /// <param name="needCache">是否需要缓存</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>资源实例</returns>
        T LoadAsset<T>(string location, out AssetOperationHandle handle, bool needCache = false,
            string packageName = "") where T : UnityEngine.Object;

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">要加载资源的类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="parent">父节点位置</param>
        /// <param name="handle">资源操作句柄</param>
        /// <param name="needCache">是否需要缓存</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns></returns>
        T LoadAsset<T>(string location, Transform parent, out AssetOperationHandle handle, bool needCache = false,
            string packageName = "") where T : UnityEngine.Object;

        /// <summary>
        /// 同步加载资源并获取句柄
        /// </summary>
        /// <typeparam name="T">要加载资源的类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="needCache">是否需要缓存</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>同步加载资源句柄</returns>
        AssetOperationHandle LoadAssetGetOperation<T>(string location, bool needCache = false,
            string packageName = "") where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载资源并获取句柄
        /// </summary>
        /// <typeparam name="T">要加载资源的类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="needCache">是否需要缓存</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>异步加载资源句柄</returns>
        AssetOperationHandle LoadAssetAsyncHandle<T>(string location, bool needCache = false,
            string packageName = "") where T : UnityEngine.Object;

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>同步加载子资源句柄</returns>
        SubAssetsOperationHandle LoadSubAssetsSync<T>(string location, string packageName = "")
            where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>异步加载子资源句柄</returns>
        SubAssetsOperationHandle LoadSubAssetsAsync<T>(string location, string packageName = "")
            where T : UnityEngine.Object;

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>同步加载子资源句柄</returns>
        public SubAssetsOperationHandle LoadSubAssetsSync(AssetInfo assetInfo, string packageName = "");

        /// <summary>
        /// 通过Tag加载资源对象集合
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetTag">资源标识</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>资源对象集合</returns>
        UniTask<LoadAssetsByTagOperation<T>> LoadAssetsByTagAsync<T>(string assetTag, string packageName = "") 
            where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">要加载资源的类型</typeparam>
        /// <param name="location">资源定位地址</param>
        /// <param name="cancellationToken">取消操作Token</param>
        /// <param name="needInstance">是否需要实例化</param>
        /// <param name="needCache">是否需要缓存</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <param name="parent">资源实例父节点</param>
        /// <returns>异步资源实例</returns>
        UniTask<T> LoadAssetAsync<T>(string location, CancellationToken cancellationToken = default,
            bool needInstance = true, bool needCache = false, string packageName = "", Transform parent = null)
            where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载游戏物体
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <param name="cancellationToken">取消操作Token</param>
        /// <param name="needCache">是否需要缓存</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>异步游戏物体实例</returns>
        UniTask<GameObject> LoadGameObjectAsync(string location, CancellationToken cancellationToken = default,
            bool needCache = false, string packageName = "");

        /// <summary>
        /// 异步加载游戏物体
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <param name="parent">父节点位置</param>
        /// <param name="cancellationToken">取消操作Token</param>
        /// <param name="needCache">是否需要缓存</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>异步游戏物体实例</returns>
        UniTask<GameObject> LoadGameObjectAsync(string location, Transform parent,
            CancellationToken cancellationToken = default, bool needCache = false, string packageName = "");

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <param name="cancellationToken">取消操作Token</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>原生文件资源实例</returns>
        UniTask<RawFileOperationHandle> LoadRawAssetAsync(string location,
            CancellationToken cancellationToken = default, string packageName = "");

        /// <summary>
        /// 异步加载子文件
        /// </summary>
        /// <typeparam name="T">资源实例类型</typeparam>
        /// <param name="location">资源定位地址</param>
        /// <param name="assetName">子资源名称</param>
        /// <param name="cancellationToken">取消操作Token</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>原生文件资源实例</returns>
        UniTask<T> LoadSubAssetAsync<T>(string location, string assetName,
            CancellationToken cancellationToken = default, string packageName = "")
            where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载所有子文件
        /// </summary>
        /// <typeparam name="T">资源实例类型</typeparam>
        /// <param name="location">资源定位地址</param>
        /// <param name="cancellationToken">取消操作Token</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>原生文件资源实例</returns>
        UniTask<T[]> LoadAllSubAssetAsync<T>(string location, CancellationToken cancellationToken = default,
            string packageName = "") where T : UnityEngine.Object;

        /// <summary>
        /// 放入预加载对象
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <param name="assetObject">预加载对象</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        public void PushPreLoadAsset(string location, UnityEngine.Object assetObject, string packageName = "");

        /// <summary>
        /// 获取预加载的实例对象
        /// </summary>
        /// <typeparam name="T">资源实例类型</typeparam>
        /// <param name="location">资源定位地址</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>预加载对象</returns>
        public T GetPreLoadAsset<T>(string location, string packageName = "") where T : UnityEngine.Object;
    }
}