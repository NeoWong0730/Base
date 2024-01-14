using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Lib;

public static class AddressablesUtil
{
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
                DebugUtil.LogWarningFormat("Canceling UnLoaded Resource Handle and not Unregister Callback action", handle.DebugName);
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
                DebugUtil.LogWarningFormat("Canceling UnLoaded Resource Handle and not Unregister Callback action", handle.DebugName);
            }
            Addressables.Release(handle);
            handle = default(AsyncOperationHandle<TObject>);
        }
    }

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
                DebugUtil.LogWarningFormat("Canceling UnLoaded Resource Handle and not Unregister Callback action", handle.DebugName);
            }
            Addressables.Release(handle);
            handle = default(AsyncOperationHandle);
        }
    }

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
                DebugUtil.LogWarningFormat("Canceling UnLoaded Resource Handle and not Unregister Callback action", handle.DebugName);
            }
            bool rlt = Addressables.ReleaseInstance(handle);
            handle = default(AsyncOperationHandle);
            return rlt;
        }
        return false;
    }

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
                DebugUtil.LogWarningFormat("Canceling UnLoaded Resource Handle and not Unregister Callback action", handle.DebugName);
            }
            bool rlt = Addressables.ReleaseInstance(handle);
            handle = default(AsyncOperationHandle<GameObject>);
            return rlt;
        }
        return false;
    }

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

    public static void InstantiateAsync(ref AsyncOperationHandle<GameObject> handle, object key, System.Action<AsyncOperationHandle<GameObject>> action, bool callbackNowIfIsDone, InstantiationParameters instantiationParameters, bool trackHandle = false)
    {
        ReleaseInstance(ref handle, action);
        handle = Addressables.InstantiateAsync(key, instantiationParameters, trackHandle);
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
