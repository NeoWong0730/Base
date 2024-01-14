using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework
{
    public class DetailAssetLoader : IDisposable
    {
        public int nIndex;
        public string sAssetPath;
        public AsyncOperationHandle<DetailAsset> mHandle;
        private Action<DetailAssetLoader> onLoaded;

        public DetailAsset GetAsset()
        {
            return mHandle.IsValid() && mHandle.IsDone ? mHandle.Result : null;
        }

        public bool Load(string assetPath, Action<DetailAssetLoader> onLoadedCallback)
        {
            if (string.Equals(assetPath, sAssetPath))
                return true;

            AddressablesUtil.Release(ref mHandle, OnAssetLoaded);
            sAssetPath = assetPath;

            if (string.IsNullOrWhiteSpace(sAssetPath))
                return true;

            onLoaded = onLoadedCallback;
            AddressablesUtil.LoadAssetAsync(ref mHandle, assetPath, OnAssetLoaded, true);

            return false;
        }

        private void OnAssetLoaded(AsyncOperationHandle<DetailAsset> handle)
        {
            onLoaded(this);
        }

        public void Dispose()
        {
            sAssetPath = string.Empty;
            AddressablesUtil.Release(ref mHandle, OnAssetLoaded);
        }
    }

    [RequireComponent(typeof(ModifyMaterial))]
    [RequireComponent(typeof(ModifyBone))]
    [RequireComponent(typeof(ModifyTexture))]
    public class FaceModify : MonoBehaviour, IUserDataChange
    {
        [SerializeField]
        private Material mFaceMaterial;
        private Material mFaceMaterialInstance;
        private Material mEyeMaterial;
        private AsyncOperationHandle<Material> mEyeMaterialHandle;

        public ModifyBone mModifyBone;
        public ModifyMaterial mModifyMaterial;
        public ModifyTexture mModifyTexture;
        public DetailAssetLoader[] mDetailAssetLoader;        

        private void Awake()
        {
            if (mFaceMaterial)
            {
                mFaceMaterialInstance = Instantiate(mFaceMaterial);
                mModifyMaterial.SetMaterial(mFaceMaterialInstance);
                mModifyTexture.mTargetMaterial = mFaceMaterialInstance;
            }

            if (mEyeMaterial)
            {
                mModifyMaterial.SetMaterial(1, mEyeMaterial);
            }
        }

        private void OnDestroy()
        {
            mModifyMaterial.SetMaterial(mFaceMaterial);
            if (mFaceMaterialInstance)
            {
                DestroyImmediate(mFaceMaterialInstance);
            }
            mModifyTexture.mTargetMaterial = null;

            if (mEyeMaterialHandle.IsValid())
            {
                if (mEyeMaterial)
                {
                    mModifyMaterial.SetMaterial(1, mEyeMaterial);
                }
                AddressablesUtil.Release(ref mEyeMaterialHandle, OnEyeMaterialLoaded);
            }
        }

        public void OnInit(object userData)
        {

        }

        public void OnUninit(object userData)
        {

        }

        public void OnUserDataChange(object userData)
        {
            FaceModifyData faceModifyData = userData as FaceModifyData;
            if (faceModifyData == null)
                return;

            if (faceModifyData.hasBoneChange)
            {
                for (int i = 0; i < faceModifyData.mBoneModifyValues.Length; ++i)
                {
                    mModifyBone.mModifyBoneControlGroups[i].fValue = faceModifyData.mBoneModifyValues[i];
                }
                mModifyBone.UpdateBones();
                faceModifyData.hasBoneChange = false;
            }

            if (faceModifyData.hasTextureChange)
            {
                for (int i = 0; i < faceModifyData.mTextureModifyValues.Length; ++i)
                {
                    SetDetailValue(i, faceModifyData.mTextureModifyValues[i]);
                    if (mDetailAssetLoader[i].Load(faceModifyData.mTextureAssetPaths[i], OnDetailAssetLoaded))
                    {
                        SetDetailAsset(i);
                    }
                }
                faceModifyData.hasBoneChange = false;
            }

            if (faceModifyData.hasEyeChange)
            {
                AddressablesUtil.LoadAssetAsync(ref mEyeMaterialHandle, faceModifyData.sEyeMaterialAssetPath, OnEyeMaterialLoaded);
            }
        }

        private void OnDetailAssetLoaded(DetailAssetLoader loader)
        {
            SetDetailAsset(loader.nIndex);
        }

        public void SetDetailAsset(int i)
        {
            if (mModifyTexture.detailDatas[i] != mDetailAssetLoader[i].GetAsset())
            {
                mModifyTexture.detailDatas[i] = mDetailAssetLoader[i].GetAsset();
                mModifyTexture.SetDirty();
            }
        }

        public void SetDetailValue(int i, DetailModifyValue value)
        {
            if (!mModifyTexture.detailModifyValues[i].Equals(value))
            {
                mModifyTexture.detailModifyValues[i] = value;
                mModifyTexture.SetDirty();
            }
        }

        private void OnEyeMaterialLoaded(AsyncOperationHandle<Material> handle)
        {
            if (mEyeMaterialHandle.Result)
            {
                mModifyMaterial.SetMaterial(1, mEyeMaterialHandle.Result);
            }
        }
    }
}