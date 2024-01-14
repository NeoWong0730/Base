using Framework;
using Lib.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public enum EPetModelParts : int
    {
        None = -1,
        Main = 0,
        Weapon = 1,
        Count,
    }

    //Main Part 一定要是0
    public class DisplayControl<TEnum> where TEnum : struct // : IDisposable
    {
        public static DisplayControl<TEnum> Create(int partCount)
        {
            //DisplayControl<TEnum> displayControl = new DisplayControl<TEnum>();
            //displayControl.mParts = new VirtualGameObject[partCount];
            DisplayControl<TEnum> displayControl = PoolManager.Fetch(typeof(DisplayControl<TEnum>)) as DisplayControl<TEnum>;
            if (displayControl.mParts == null || displayControl.mParts.Length != partCount)
            {
                displayControl.mParts = new VirtualGameObject[partCount];
            }
            return displayControl;
        }

        public static void Destory(ref DisplayControl<TEnum> control)
        {
            if (control != null)
            {
                try
                {
                    control.Dispose();
                    PoolManager.Recycle(control, false);
                    control = null;
                }
                catch(Exception e)
                {
                    control = null;
                    DebugUtil.LogException(e);
                    DebugUtil.LogError("Destory DisplayControl<TEnum> Exception");
                }                                
            }
        }

        public Action<int> onLoaded;

        private ELayerMask _layerMask = ELayerMask.Default;
        public ELayerMask eLayerMask
        {
            get
            {
                return _layerMask;
            }
            set
            {
                if (_layerMask != value)
                {
                    _layerMask = value;
                    for (int i = 0; i < mParts.Length; ++i)
                    {
                        if (mParts[i] != null && mParts[i].gameObject != null)
                        {
                            mParts[i].gameObject.transform.Setlayer(_layerMask);
                        }
                    }
                }
            }
        }

        protected AnimationControl _Animation = new AnimationControl();
        public AnimationControl mAnimation
        {
            get
            {
                if (bMainPartFinished)
                {
                    return _Animation;
                }
                return null;
            }
        }

        protected VirtualGameObject[] mParts;

        protected HashSet<VirtualGameObject> mOthers;

        //protected DisplayControl() { }

        public bool IsAllPartsFinished()
        {
            if (mParts == null)
                return true;
            for (int i = 0; i < mParts.Length; ++i)
            {
                if (mParts[i] != null && mParts[i].eState == VirtualGameObject.EState.Loading)
                    return false;
            }
            return true;
        }

        public bool IsPartFinished(TEnum id)
        {
            int index = EnumInt32ToInt.Convert<TEnum>(id);
            return IsPartFinished(index);
        }

        public bool IsPartFinished(int id)
        {
            if (mParts == null)
                return false;

            if (id >= 0 && id < mParts.Length && mParts[id] != null)
            {
                return mParts[id].eState != VirtualGameObject.EState.Loading;
            }

            return false;
        }

        public bool bMainPartFinished { get { return IsPartFinished(0); } }        

        public void LoadMainModel(TEnum part, string assetPath, TEnum parentPart, string socketName, bool ignoreParent = false)
        {            
            VirtualGameObject VGO = GetOrCreatePart(part);
            if (VGO == null)
                return;

            if (string.IsNullOrWhiteSpace(assetPath))
            {
                if (VGO.eState!=VirtualGameObject.EState.Empty)
                {
                    if (onLoaded!=null)
                    {
                        onLoaded(VGO.id);
                    }
                    VGO.Clear();
                }
            }
            VGO.LoadAsset(assetPath, OnVirtualGameObjectLoaded);

            if (!ignoreParent)
            {
                VirtualGameObject parentVGO = GetPart(parentPart);
                VGO.SetParent(parentVGO, socketName);
            }
        }

        public VirtualGameObject LoadAddition(TEnum part, string assetPath, TEnum parentPart, string socketName)
        {
            VirtualGameObject VGO = PoolManager.Fetch<VirtualGameObject>();
            VGO.id = EnumInt32ToInt.Convert(part);
            VGO.LoadAsset(assetPath, OnVirtualGameObjectLoaded);

            VirtualGameObject parentVGO = GetPart(parentPart);
            VGO.SetParent(parentVGO, socketName);

            if (mOthers == null)
                mOthers = new HashSet<VirtualGameObject>();

            mOthers.Add(VGO);

            return VGO;
        }

        public void Remove(VirtualGameObject VGO)
        {
            if (mOthers == null || VGO == null)
                return;

            if (mOthers.Remove(VGO))
            {
                PoolManager.Recycle(VGO);
            }
            else
            {
                VirtualGameObject main = GetPart(0);
                if (main != null)
                {
                    DebugUtil.LogErrorFormat("移除失败 : 物体{0} 不在 {1} 中", VGO.sAssetPath, main.sAssetPath);
                }
                else
                {
                    DebugUtil.LogErrorFormat("移除失败 : 物体{0} 不在 xx 中", VGO.sAssetPath);
                }
            }
        }

        protected virtual void OnVirtualGameObjectLoaded(VirtualGameObject virtualGameObject)
        {
            if (virtualGameObject.gameObject != null)
            {
                virtualGameObject.gameObject.transform.Setlayer(_layerMask);
                //virtualGameObject.gameObject.SetActive(false);
            }

            if (virtualGameObject.id == 0)
            {
                //todo 挂在第二层
                if (_Animation != null)
                    _Animation.SetOwner(virtualGameObject.transform.GetChild(0).gameObject);
                
            }

            if (onLoaded != null)
            {
                onLoaded(virtualGameObject.id);
            }
        }

        internal VirtualGameObject GetOrCreatePart(TEnum part)
        {
            int child = EnumInt32ToInt.Convert(part);
            if (child >= 0 && child < mParts.Length)
            {
                if(mParts[child] == null)
                {
                    mParts[child] = CreateVGo();
                    mParts[child].id = child;
                }
                return mParts[child];
            }
            return null;
        }

        public VirtualGameObject GetPart(TEnum part)
        {
            int child = EnumInt32ToInt.Convert(part);
            return GetPart(child);
        }

        public VirtualGameObject GetPart(int part)
        {
            if (part >= 0 && part < mParts.Length)
            {
                return mParts[part];
            }
            return null;
        }

        public VirtualGameObject GetOtherWeapon()
        {
            if (mOthers != null)
            {
                foreach (var other in mOthers)
                {
                    return other;
                }
            }
            
            return null;
        }

        public virtual void Dispose()
        {
            onLoaded = null;
            _layerMask = ELayerMask.Default;

            _Animation.ClearAnimations();
            for (int i = 0; i < mParts.Length; ++i)
            {
                PoolManager.Recycle(mParts[i]);
                mParts[i] = null;
            }

            if (mOthers != null)
            {
                var enumerator = mOthers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PoolManager.Recycle(enumerator.Current);
                }
                mOthers.Clear();
            }
        }

        protected virtual VirtualGameObject CreateVGo()
        {
            return PoolManager.Fetch<VirtualGameObject>();
        }
    }
}