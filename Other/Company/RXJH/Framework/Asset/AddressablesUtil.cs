using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public static class AddressablesUtil
{
    /// <summary>
    /// Release the operation and its associated resources.
    /// </summary>
    /// <typeparam name="TObject">The type of the AsyncOperationHandle being released</typeparam>
    /// <param name="handle">The operation handle to release.</param>
    public static void Release<TObject>(ref AsyncOperationHandle<TObject> handle, System.Action<AsyncOperationHandle<TObject>> action)
    {
        if (handle.IsValid())
        {
            if (action != null)
            {
                handle.Completed -= action;
            }
            else
            {
                Lib.Core.DebugUtil.LogWarningFormat("正在取消未加载完成的资源句柄{0},并且未注销完成回调", handle.DebugName);
            }
            Addressables.Release<TObject>(handle);
            handle = default(AsyncOperationHandle<TObject>);
        }
    }
    public static void Release<TObject>(ref AsyncOperationHandle handle, System.Action<AsyncOperationHandle<TObject>> action)
    {
        if (handle.IsValid())
        {
            if (action != null)
            {
                handle.Convert<TObject>().Completed -= action;
            }
            else
            {
                Lib.Core.DebugUtil.LogWarningFormat("正在取消未加载完成的资源句柄{0},并且未注销完成回调", handle.DebugName);
            }
            Addressables.Release(handle);
            handle = default(AsyncOperationHandle<TObject>);
        }
    }
    /// <summary>
    /// Release the operation and its associated resources.
    /// </summary>
    /// <param name="handle">The operation handle to release.</param>
    public static void Release(ref AsyncOperationHandle handle, System.Action<AsyncOperationHandle> action)
    {
        if (handle.IsValid())
        {
            if (action != null)
            {
                handle.Completed -= action;
            }
            else
            {
                Lib.Core.DebugUtil.LogWarningFormat("正在取消未加载完成的资源句柄{0},并且未注销完成回调", handle.DebugName);
            }
            Addressables.Release(handle);
            handle = default(AsyncOperationHandle);
        }
    }
    /// <summary>
    /// Releases and destroys an object that was created via Addressables.InstantiateAsync. 
    /// </summary>
    /// <param name="handle">The handle to the game object to destroy, that was returned by InstantiateAsync.</param>
    /// <returns>Returns true if the instance was successfully released.</returns>
    public static bool ReleaseInstance(ref AsyncOperationHandle handle, System.Action<AsyncOperationHandle> action)
    {
        if (handle.IsValid())
        {
            if (action != null)
            {
                handle.Completed -= action;
            }
            else
            {
                Lib.Core.DebugUtil.LogWarningFormat("正在取消未加载完成的资源句柄{0},并且未注销完成回调", handle.DebugName);
            }
            bool rlt = Addressables.ReleaseInstance(handle);
            handle = default(AsyncOperationHandle);
            return rlt;
        }
        return false;
    }
    /// <summary>
    /// Releases and destroys an object that was created via Addressables.InstantiateAsync. 
    /// </summary>
    /// <param name="handle">The handle to the game object to destroy, that was returned by InstantiateAsync.</param>
    /// <returns>Returns true if the instance was successfully released.</returns>
    public static bool ReleaseInstance(ref AsyncOperationHandle<GameObject> handle, System.Action<AsyncOperationHandle<GameObject>> action)
    {
        if (handle.IsValid())
        {
            if (action != null)
            {
                handle.Completed -= action;
            }
            else
            {
                Lib.Core.DebugUtil.LogWarningFormat("正在取消未加载完成的资源句柄{0},并且未注销完成回调", handle.DebugName);
            }
            bool rlt = Addressables.ReleaseInstance(handle);
            handle = default(AsyncOperationHandle<GameObject>);
            return rlt;
        }
        return false;
    }

    /// <summary>
    /// 资源加载接口
    /// </summary>
    /// <typeparam name="TObject">资源类型</typeparam>
    /// <param name="handle">资源句柄</param>
    /// <param name="key">资源路径</param>
    /// <param name="action">加载完成回调 如果需要卸载的回调和新资源完成的回调不一致 请先手动调 Release 卸载原有资源和回调</param>
    /// <param name="callbackNowIfIsDone">true 如果当前的资源已经是加载完成的资源 直接调用加载完成的回调</param>
    public static void LoadAssetAsync<TObject>(ref AsyncOperationHandle<TObject> handle, object key, System.Action<AsyncOperationHandle<TObject>> action, bool callbackNowIfIsDone = true)
    {
        Release(ref handle, action);
        handle = Addressables.LoadAssetAsync<TObject>(key);
        if (action != null)
        {
            if (callbackNowIfIsDone && handle.IsDone)
            {
                action(handle);
            }
            else
            {
                handle.Completed += action;
            }
        }
    }

    /// <summary>
    /// 资源加载接口
    /// </summary>
    /// <typeparam name="TObject">资源类型</typeparam>
    /// <param name="handle">资源句柄</param>
    /// <param name="key">资源路径</param>
    /// <param name="action">加载完成回调 如果需要卸载的回调和新资源完成的回调不一致 请先手动调 Release 卸载原有资源和回调</param>
    /// <param name="callbackNowIfIsDone">true 如果当前的资源已经是加载完成的资源 直接调用加载完成的回调</param>
    public static void InstantiateAsync(ref AsyncOperationHandle<GameObject> handle, object key, System.Action<AsyncOperationHandle<GameObject>> action, bool callbackNowIfIsDone = true, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = false)
    {
        ReleaseInstance(ref handle, action);
        handle = Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
        if (action != null)
        {
            if (callbackNowIfIsDone && handle.IsDone)
            {
                action(handle);
            }
            else
            {
                handle.Completed += action;
            }
        }
    }

    public static void InstantiateAsync(ref AsyncOperationHandle<GameObject> handle, object key, System.Action<AsyncOperationHandle<GameObject>> action, bool callbackNowIfIsDone, InstantiationParameters instantiateParameters, bool trackHandle = false)
    {
        ReleaseInstance(ref handle, action);
        handle = Addressables.InstantiateAsync(key, instantiateParameters, trackHandle);
        if (action != null)
        {
            if (callbackNowIfIsDone && handle.IsDone)
            {
                action(handle);
            }
            else
            {
                handle.Completed += action;
            }
        }        
    }
}
