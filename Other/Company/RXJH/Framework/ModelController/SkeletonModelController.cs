using Animancer;
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SkeletonModelController : MonoBehaviour, IModel, IMaterialOverride, IAnimationPlayable
{
    private string sSkeletonAssetPath;
    private AsyncOperationHandle<GameObject> mSkeletonHandle;

    /// <summary>
    /// 骨架信息包括骨骼绑点
    /// </summary>
    public Skeleton mSkeleton;

    /// <summary>
    /// 和骨架一起加载的完整模型
    /// </summary>
    public ModelFull mModelFull;

    /// <summary>
    /// 模型各个固定部位
    /// </summary>
    public ModelPartLoader[] mModelPart = new ModelPartLoader[4];

    /// <summary>
    /// 绑定资源加载管理
    /// </summary>
    public List<AdditionWidget> mAdditionWidgets = new List<AdditionWidget>();

    private string sOverrideMaterialPath;
    private AsyncOperationHandle<Material> mOverrideMaterialHandle;
    internal EMaterialControlType eMaterialControlType;
    internal Material mOverrideMaterial;

    /// <summary>
    /// 动画播放
    /// </summary>
    public Animancer.HybridAnimancerComponent mAnimancer;

    /// <summary>
    /// 当前模型缓存的动画片段
    /// 可以和动画相关的封装到一起
    /// </summary>
    [HideInInspector]
    public AnimationClipListData animationClipListData;

    public AsyncOperationHandle<AnimationClipListData> mAnimationClipListDataHandle;
    public AsyncOperationHandle<AvatarMask> mAvatarMaskHandler;

    private AnimancerLayer baseLayer;
    private AnimancerLayer leftArmLayer;
    private AnimancerLayer rightArmLayer;
    private AnimancerLayer upperBodyLayer;
    private AnimancerLayer fullBobyLayer;

    public enum EAnimancerLayer
    {
        BaseLayer = 0,  //基础层，由AnimatorController控制
        LeftArm = 1,    //左手
        RightArm = 2,   //右手
        UpperBody = 3,  //上身
        FullBody = 4,   //全身
    }



    public Transform GetTransform() { return transform; }

    public void SetPartCount(int count)
    {
        if (mModelPart.Length != count)
        {
            Array.Resize(ref mModelPart, count);
        }
    }

    public void SetPart(int index, string assetPath)
    {
        if (index < 0 || index >= mModelPart.Length)
            return;

        if (mModelPart[index] == null)
        {
            mModelPart[index] = new ModelPartLoader();            
        }
        mModelPart[index].humanModel = this;

        if (mSkeleton)
        {
            mModelPart[index].Load(assetPath, mSkeleton.mRoot);
        }
        else
        {
            mModelPart[index].sAssetPath = assetPath;
        }
    }

    public ModelPartLoader GetPart(int index)
    {
        if (index < 0 || index >= mModelPart.Length)
            return null;

        return mModelPart[index];
    }

    public bool LoadSkeleton(string assetPath)
    {
        if (string.Equals(sSkeletonAssetPath, assetPath))
            return true;

        AddressablesUtil.ReleaseInstance(ref mSkeletonHandle, null);
        sSkeletonAssetPath = assetPath;
        AddressablesUtil.InstantiateAsync(ref mSkeletonHandle, assetPath, OnSkeletonLoaded, true, transform);

        return mSkeletonHandle.IsDone;
    }

    private void OnSkeletonLoaded(AsyncOperationHandle<GameObject> handle)
    {
        GameObject go = handle.Result;
        mAnimancer = go.GetComponent<Animancer.HybridAnimancerComponent>();
        InitAnimancer();
        mSkeleton = go.GetComponent<Skeleton>();
        mModelFull = go.GetComponent<ModelFull>();

        if (eMaterialControlType != EMaterialControlType.None && mModelFull)
        {
            mModelFull.SetOverrideMaterial(-1, mOverrideMaterial);
        }

        if(mModelPart != null)
        {
            for (int i = 0; i < mModelPart.Length; ++i)
            {
                mModelPart[i].Load(mModelPart[i].sAssetPath, mSkeleton.mRoot);
            }
        }
    }

    public AdditionWidget AddWidget(int slotID, string assetPath)
    {
        Transform slot = null;

        if (slotID == -1) //根节点
        {
            slot = GetTransform();
        }
        else if (slotID == -2) //世界节点
        {

        }
        else
        {
            slot = mSkeleton.GetSlotByID(slotID);
        }

        AdditionWidget additionWidget = new AdditionWidget();
        mAdditionWidgets.Add(additionWidget);

        additionWidget.Load(assetPath, slot);

        return additionWidget;
    }

    public bool RemoveWidget(AdditionWidget widget)
    {
        int index = mAdditionWidgets.IndexOf(widget);
        if (index < 0)
        {
            return false;
        }

        widget.Dispose();
        mAdditionWidgets.Remove(widget);
        return true;
    }

    public void OnLODChange(int from, int to)
    {

    }

    private void OnDestroy()
    {
        for (int i = 0; i < mModelPart.Length; ++i)
        {
            mModelPart[i].Dispose();
        }

        if (mAnimationClipListDataHandle.IsValid())
        {
            AddressablesUtil.Release<AnimationClipListData>(ref mAnimationClipListDataHandle, LoadAnimationClipListDataCallBack);
        }
    }

    #region Animation

    public void LoadAnimationClips(uint id)
    {
        if (mAnimationClipListDataHandle.IsValid())
        {
            AddressablesUtil.Release<AnimationClipListData>(ref mAnimationClipListDataHandle, LoadAnimationClipListDataCallBack);
        }

        AddressablesUtil.LoadAssetAsync<AnimationClipListData>(ref mAnimationClipListDataHandle, $"animdatas_{id}", LoadAnimationClipListDataCallBack, false);
    }

    void LoadAnimationClipListDataCallBack(AsyncOperationHandle<AnimationClipListData> handle)
    {
        animationClipListData = handle.Result;

        animationClipListData.animationClipDatasDict.Clear();
        for (int index = 0, len = animationClipListData.animationClipDatas.Count; index < len; index++)
        {
            animationClipListData.animationClipDatasDict[animationClipListData.animationClipDatas[index].name] = animationClipListData.animationClipDatas[index];
        }
    }

    void InitAnimancer()
    {
        baseLayer = mAnimancer.Layers[0];
        baseLayer.SetDebugName("Base Layer");
        AddressablesUtil.LoadAssetAsync<AvatarMask>(ref mAvatarMaskHandler, "fullbody", (mAvatarMaskHandler) =>
        {
            baseLayer.SetMask(mAvatarMaskHandler.Result);
        }, false);

        leftArmLayer = mAnimancer.Layers[1];
        leftArmLayer.SetDebugName("LeftArm Layer");
        AddressablesUtil.LoadAssetAsync<AvatarMask>(ref mAvatarMaskHandler, "leftarm", (mAvatarMaskHandler) =>
        {
            leftArmLayer.SetMask(mAvatarMaskHandler.Result);
        }, false);

        rightArmLayer = mAnimancer.Layers[2];
        rightArmLayer.SetDebugName("RightArmLayer");
        AddressablesUtil.LoadAssetAsync<AvatarMask>(ref mAvatarMaskHandler, "rightarm", (mAvatarMaskHandler) =>
        {
            rightArmLayer.SetMask(mAvatarMaskHandler.Result);
        }, false);

        upperBodyLayer = mAnimancer.Layers[3];
        upperBodyLayer.SetDebugName("UpperBodyLayer");
        AddressablesUtil.LoadAssetAsync<AvatarMask>(ref mAvatarMaskHandler, "upperbody", (mAvatarMaskHandler) =>
        {
            upperBodyLayer.SetMask(mAvatarMaskHandler.Result);
        }, false);

        fullBobyLayer = mAnimancer.Layers[4];
        fullBobyLayer.SetDebugName("FullBodyLayer");
        AddressablesUtil.LoadAssetAsync<AvatarMask>(ref mAvatarMaskHandler, "fullbody", (mAvatarMaskHandler) =>
        {
            fullBobyLayer.SetMask(mAvatarMaskHandler.Result);
        }, false);

        LoadOverrideController("aoc_100110000");
    }

    public void PlayAnimation(string stateName, EAnimancerLayer layer = EAnimancerLayer.FullBody)
    {
        if (animationClipListData == null || !animationClipListData.animationClipDatasDict.ContainsKey(stateName))
            return;

        AnimationClip animationClip = animationClipListData.animationClipDatasDict[stateName].clip;
        PlayAnimation(animationClip, layer);
    }

    public void PlayAnimation(AnimationClip clip, EAnimancerLayer layer = EAnimancerLayer.FullBody)
    {
        switch (layer)
        {
            case EAnimancerLayer.LeftArm:
                baseLayer.Play(clip);
                break;
            case EAnimancerLayer.RightArm:
                rightArmLayer.Play(clip);
                break;
            case EAnimancerLayer.UpperBody:
                upperBodyLayer.Play(clip);
                break;
            case EAnimancerLayer.FullBody:
                fullBobyLayer.Play(clip);
                break;
            case EAnimancerLayer.BaseLayer:
                Debug.LogError($"You can not play base layer animation with this function, pls use PlayAnimatorControllerState");
                break;
        }
    }

    public void StopLayer(EAnimancerLayer layer = EAnimancerLayer.FullBody)
    {
        switch (layer)
        {
            case EAnimancerLayer.LeftArm:
                leftArmLayer.Stop();
                break;
            case EAnimancerLayer.RightArm:
                rightArmLayer.Stop();
                break;
            case EAnimancerLayer.UpperBody:
                upperBodyLayer.Stop();
                break;
            case EAnimancerLayer.FullBody:
                fullBobyLayer.Stop();
                break;
            case EAnimancerLayer.BaseLayer:
                Debug.LogError($"You need not stop base layer animation, it is running all the time");
                break;
        }
    }

    public void PlayAnimatorControllerState(string stateName, float fadeTime = 0.1f)
    {
        controllerState.CrossFadeInFixedTime(stateName, fadeTime);
    }

    public void SetBool(int id, bool value)
    {
        controllerState?.SetBool(id, value);
    }

    public void SetBool(string name, bool value)
    {
        controllerState?.SetBool(name, value);
    }

    public void SetFloat(int id, float value)
    {
        controllerState?.SetFloat(id, value);
    }

    public void SetFloat(string name, float value)
    {
        controllerState?.SetFloat(name, value);
    }

    public float SetFloat(string name, float value, float dampTime, float deltaTime, float maxSpeed = float.PositiveInfinity)
    {
        if (controllerState != null)
            return controllerState.SetFloat(name, value, dampTime, deltaTime, maxSpeed);

        return value;
    }

    public float SetFloat(int id, float value, float dampTime, float deltaTime, float maxSpeed = float.PositiveInfinity)
    {
        if (controllerState != null)
            return controllerState.SetFloat(id, value, dampTime, deltaTime, maxSpeed);

        return value;
    }

    public void SetInteger(int id, int value)
    {
        controllerState?.SetInteger(id, value);
    }

    public void SetInteger(string name, int value)
    {
        controllerState?.SetInteger(name, value);
    }

    public void SetTrigger(int id)
    {
        controllerState?.SetTrigger(id);
    }

    public void SetTrigger(string name)
    {
        controllerState?.SetTrigger(name);
    }

    public void ResetTrigger(int id)
    {
        controllerState?.ResetTrigger(id);
    }

    public void ResetTrigger(string name)
    {
        controllerState?.ResetTrigger(name);
    }

    private string sOverrideControllerAssetPath;
    private AsyncOperationHandle<AnimatorOverrideController> mOverrideControllerHandle;

    public bool LoadOverrideController(string assetPath)
    {
        if (string.Equals(sOverrideControllerAssetPath, assetPath))
            return true;

        if (mOverrideControllerHandle.IsValid())
        {
            AddressablesUtil.Release<AnimatorOverrideController>(ref mOverrideControllerHandle, OnOverrideControllerLoaded);
        }
        sOverrideControllerAssetPath = assetPath;
        AddressablesUtil.LoadAssetAsync<AnimatorOverrideController>(ref mOverrideControllerHandle, assetPath, OnOverrideControllerLoaded, true);
        return mOverrideControllerHandle.IsDone;
    }

    ControllerState controllerState;

    void OnOverrideControllerLoaded(AsyncOperationHandle<AnimatorOverrideController> handle)
    {
        controllerState = new ControllerState(handle.Result);
        baseLayer.Play(controllerState);
        mAnimancer.runtimeAnimatorController = handle.Result;
    }

    #endregion Animation

    #region Material

    private void DestroyOverrideMaterial()
    {
        if (eMaterialControlType.HasFlag(EMaterialControlType.FromInstance))
        {
            if (eMaterialControlType.HasFlag(EMaterialControlType.DestroyOnOverride))
            {
                UnityEngine.Object.Destroy(mOverrideMaterial);
            }
        }
        else if (eMaterialControlType == EMaterialControlType.FromAssetPath)
        {
            AddressablesUtil.Release(ref mOverrideMaterialHandle, OnMaterialLoaded);
            sOverrideMaterialPath = string.Empty;
        }
    }

    /// <summary>
    /// 模型材质覆盖
    /// </summary>
    /// <param name="material">材质</param>
    /// <param name="autoDestroy">自动删除</param>
    public void SetOverrideMaterial(Material material, bool autoDestroy)
    {
        if (mOverrideMaterial == material && eMaterialControlType == EMaterialControlType.FromInstance)
            return;

        DestroyOverrideMaterial();

        mOverrideMaterial = material;
        if (mOverrideMaterial == null)
        {
            eMaterialControlType = EMaterialControlType.None;
            for(int i = 0; i< mModelPart.Length; ++i)
            {
                if (mModelPart[i].mModelPart != null)
                {
                    mModelPart[i].mModelPart.CancelOverrideMaterial(-1);
                }
            }

            if (mModelFull)
            {
                mModelFull.SetOverrideMaterial(-1, mOverrideMaterial);
            }
        }
        else
        {
            eMaterialControlType = EMaterialControlType.FromInstance;
            if (autoDestroy)
            {
                eMaterialControlType |= EMaterialControlType.DestroyOnOverride;
            }

            for (int i = 0; i < mModelPart.Length; ++i)
            {
                if (mModelPart[i].mModelPart != null)
                {
                    mModelPart[i].mModelPart.SetOverrideMaterial(-1, mOverrideMaterial);
                }
            }

            if (mModelFull)
            {
                mModelFull.SetOverrideMaterial(-1, mOverrideMaterial);
            }
        }
    }

    /// <summary>
    /// 模型材质覆盖
    /// </summary>
    /// <param name="materialPath">材质资源路径</param>
    public void SetOverrideMaterial(string materialPath)
    {
        if (string.Equals(sOverrideMaterialPath, materialPath) && eMaterialControlType == EMaterialControlType.FromAssetPath)
            return;

        DestroyOverrideMaterial();

        sOverrideMaterialPath = materialPath;
        if (string.IsNullOrWhiteSpace(sOverrideMaterialPath))
        {
            eMaterialControlType = EMaterialControlType.None;
            mOverrideMaterial = null;
            for (int i = 0; i < mModelPart.Length; ++i)
            {
                if (mModelPart[i].mModelPart != null)
                {
                    mModelPart[i].mModelPart.CancelOverrideMaterial(-1);
                }
            }

            if (mModelFull)
            {
                mModelFull.CancelOverrideMaterial(-1);
            }
        }
        else
        {
            eMaterialControlType = EMaterialControlType.FromAssetPath;
            AddressablesUtil.LoadAssetAsync<Material>(ref mOverrideMaterialHandle, sOverrideMaterialPath, OnMaterialLoaded, true);
            if (!mOverrideMaterialHandle.IsDone)
            {
                mOverrideMaterial = null;
                for (int i = 0; i < mModelPart.Length; ++i)
                {
                    if (mModelPart[i].mModelPart != null)
                    {
                        mModelPart[i].mModelPart.CancelOverrideMaterial(-1);
                    }
                }

                if (mModelFull)
                {
                    mModelFull.CancelOverrideMaterial(-1);
                }
            }
        }
    }

    private void OnMaterialLoaded(AsyncOperationHandle<Material> handle)
    {
        mOverrideMaterial = handle.Result;
        for (int i = 0; i < mModelPart.Length; ++i)
        {
            if (mModelPart[i].mModelPart != null)
            {
                mModelPart[i].mModelPart.SetOverrideMaterial(-1, mOverrideMaterial);
            }
        }

        if (mModelFull)
        {
            mModelFull.SetOverrideMaterial(-1, mOverrideMaterial);
        }
    }
    #endregion Material
}