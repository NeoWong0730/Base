using UnityEngine;

//#if USE_ADDRESSABLE_ASSET
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//#else
//using Lib.AssetLoader;
//#endif

public class FxLoader : MonoBehaviour
{
    public string path = string.Empty;
    private AsyncOperationHandle<GameObject> assetRequest;

    private void Start()
    {
        AddressablesUtil.InstantiateAsync(ref assetRequest, path, OnCompleted, true, transform);
    }

    private void OnDestroy()
    {
        AddressablesUtil.ReleaseInstance(ref assetRequest, OnCompleted);
    }

    private void OnCompleted(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Result == null)
            return;

        Transform trans = handle.Result.transform;
        trans.localScale = Vector3.one;
        trans.Setlayer(ELayerMask.UI);
    }
}
