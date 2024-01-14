using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public enum EMaterialControlType
{
    None = 0,
    FromInstance = 1,
    FromAssetPath = 2,
    DestroyOnOverride = 4,
    UseBaseMap = 8,
    UseNormalMap = 16,
}

public interface IUserDataChange
{   
    void OnInit(object userData);
    void OnUserDataChange(object userData);
    void OnUninit(object userData);
}

[Serializable]
public class ModelPartLoader : IDisposable
{
    public SkeletonModelController humanModel;

    public ModelPart mModelPart;
    private AsyncOperationHandle<GameObject> mHandle;
    public string sAssetPath;
    public object mUserData;
    public IUserDataChange mUserDataChange;

    public void Load(string assetPath, Transform parent = null)
    {
        if (!mHandle.IsValid())
        {
            sAssetPath = string.Empty;
        }

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
        mUserDataChange = handle.Result.GetComponent<IUserDataChange>();
        if (mUserDataChange != null)
        {
            mUserDataChange.OnInit(mUserData);
        }

        mModelPart = handle.Result.GetComponent<ModelPart>();

        if(humanModel.mSkeleton != null)
        {
            mModelPart.BindingSkeleton(humanModel.mSkeleton, humanModel.mAnimancer);            
        }
        
        if (humanModel.eMaterialControlType != EMaterialControlType.None)
        {
            mModelPart.SetOverrideMaterial(-1, humanModel.mOverrideMaterial);
        }
    }

    public void Dispose()
    {
        mModelPart?.CancelOverrideMaterial(-1);
        if (mUserDataChange != null)
        {
            mUserDataChange.OnUninit(mUserData);
        }
        humanModel = null;
        mUserDataChange = null;
        mUserData = null;
        sAssetPath = string.Empty;
        AddressablesUtil.ReleaseInstance(ref mHandle, OnModelLoaded);
    }

    public void ApplyUserDataChange()
    {
        if (mUserDataChange != null)
        {
            mUserDataChange.OnUserDataChange(mUserData);
        }
    }
}
