using UnityEngine;
using UnityEngine.UI;

//#if USE_ADDRESSABLE_ASSET
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
//#else
//using Lib.AssetLoader;
//#endif

[RequireComponent(typeof(RawImage))]
public class RawImageLoader : MonoBehaviour
{
    private RawImage _rawImage;
    private AsyncOperationHandle<Texture> _handle;
    public string _sAssetPath;
    public bool _bAutoSetNativeSize;

    public RawImage rawImage { get { return _rawImage; } }

    public void Set(string path, bool autoSetNativeSize = false)
    {
        if (string.Equals(_sAssetPath, path, StringComparison.Ordinal))
        {
            return;
        }

        _sAssetPath = path;
        _bAutoSetNativeSize = autoSetNativeSize;

        _Release();

        if (isActiveAndEnabled)
        {
            _Load();
        }
    }

    private void Awake()
    {
        _rawImage = GetComponent<RawImage>();
        _Release();
    }

    private void OnEnable()
    {
        _Load();
    }

    private void OnDisable()
    {
        _Release();
    }

    private void _Load()
    {
        if (!string.IsNullOrEmpty(_sAssetPath))
        {
            AddressablesUtil.LoadAssetAsync<Texture>(ref _handle, _sAssetPath, OnCompleted);
        }
    }

    private void _Release()
    {
        if (_rawImage)
        {
            _rawImage.texture = null;
            _rawImage.enabled = false;
        }
        AddressablesUtil.Release<Texture>(ref _handle, OnCompleted);
    }

    private void OnCompleted(AsyncOperationHandle<Texture> handle)
    {
        _rawImage.texture = handle.Result;
        if (_bAutoSetNativeSize)
        {
            _rawImage.SetNativeSize();
        }
        _rawImage.enabled = true;
    }
}
