using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;


public class AdditionWidget : IDisposable
{
    public string sAssetPath;
    private AsyncOperationHandle<GameObject> mHandle;

    public void Load(string assetPath, Transform parent = null)
    {
        if (string.Equals(assetPath, sAssetPath))
            return;

        AddressablesUtil.Release(ref mHandle, OnModelLoaded);
        sAssetPath = assetPath;

        if (string.IsNullOrWhiteSpace(sAssetPath))
            return;

        AddressablesUtil.InstantiateAsync(ref mHandle, assetPath, OnModelLoaded, false, parent);
    }

    private void OnModelLoaded(AsyncOperationHandle<GameObject> handle)
    {

    }

    public void Dispose()
    {
        AddressablesUtil.ReleaseInstance(ref mHandle, OnModelLoaded);
    }
}
