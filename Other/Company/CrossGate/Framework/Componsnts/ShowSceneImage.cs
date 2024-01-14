using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ShowSceneImage : ShowSceneSetting
{
    public string sAssetPath;
    private RawImage mRawImage;
    private AsyncOperationHandle<GameObject> mHandle;
    private RenderTexture mRenderTexture;
    private Camera mCamera;
    public int nAntialiasing = 0;

    private void OnEnable()
    {
        mRawImage = GetComponent<RawImage>();        
        AddressablesUtil.InstantiateAsync(ref mHandle, sAssetPath, OnLoaded, true);
    }

    private void OnLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status != AsyncOperationStatus.Succeeded)
            return;

        DontDestroyOnLoad(obj.Result);

        Transform trans = obj.Result.transform;
        mCamera = trans.Find("Camera").GetComponent<Camera>();

        int width = vResolution.x;
        int height = vResolution.y;
        float scale = fScale;

        float realScale = Mathf.Clamp(scale, 0.125f, 8f);

        width = width > 0 ? width : Screen.width;
        height = height > 0 ? height : Screen.height;

        width = (int)(width * realScale);
        height = (int)(height * realScale);

        ReleaseTemporary();

        int antialiasing = nAntialiasing;
        if (antialiasing == 0)
        {
            int antiAliasingLevel = RenderExtensionSetting.nSceneMaxLOD;
            antialiasing = 1 << antiAliasingLevel;
        }
        mRenderTexture = RenderTexture.GetTemporary(width, height, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, antialiasing, RenderTextureMemoryless.MSAA);

        mCamera.targetTexture = mRenderTexture;
        mRawImage.texture = mRenderTexture;
        mRawImage.color = Color.white;
    }

    public void ReleaseTemporary()
    {
        if (mRenderTexture != null)
        {
            if (mCamera != null)
            {
                mCamera.targetTexture = null;
                mCamera = null;
            }                
            
            RenderTexture.ReleaseTemporary(mRenderTexture);
            mRenderTexture = null;
            mRawImage.texture = null;
            mRawImage.color = Color.clear;
        }
    }

    private void OnDisable()
    {
        ReleaseTemporary();
        AddressablesUtil.ReleaseInstance(ref mHandle, OnLoaded);
    }
}