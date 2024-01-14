using Lib.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    public class VirtualGameObject : IDisposable
    {
        public enum EState
        {
            Empty,
            Loading,
            Loaded,
            Failed,
        }

        public int id = 0;
        public EState eState { get; private set; }

        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }
        public VirtualGameObject parent { get; private set; }
        
        private List<VirtualGameObject> children = new List<VirtualGameObject>();
        public string sAttachSlotPath { get; private set; }
        public bool activeSelf { get; private set; } = true;

        private bool bHideIfParentNotLoaded = true;

        private AsyncOperationHandle<GameObject> mHandle;
        public string sAssetPath { get; private set; }

        public Action<VirtualGameObject> onLoaded;

        Vector3 _localPosition = Vector3.zero;
        Quaternion _localRotation = Quaternion.identity;
        Vector3 _localScale = Vector3.one;

        public Vector3 localPosition
        {
            get { return _localPosition; }
            set
            {
                if (_localPosition.Equals(value))
                    return;
                _localPosition = value;
                if (gameObject != null)
                {
                    transform.localPosition = _localPosition;
                }
            }
        }
        public Quaternion localRotation
        {
            get { return _localRotation; }
            set
            {
                if (_localRotation.Equals(value))
                    return;
                _localRotation = value;
                if (gameObject != null)
                {
                    transform.localRotation = _localRotation;
                }
            }
        }
        public Vector3 localScale
        {
            get { return _localScale; }
            set
            {
                if (_localScale.Equals(value))
                    return;
                _localScale = value;
                if (gameObject != null)
                {
                    transform.localScale = _localScale;
                }
            }
        }

        private bool hideIfParentNotLoaded
        {
            get
            {
                return bHideIfParentNotLoaded;
            }
            set
            {
                if (bHideIfParentNotLoaded != value)
                {
                    bHideIfParentNotLoaded = value;
                    _SetActive();
                }
            }
        }

        public void SetActive(bool bActive)
        {
            if (activeSelf != bActive)
            {
                activeSelf = bActive;
                _SetActive();
            }
        }
        public void SetGameObject(GameObject go, bool ignoreParent = false)
        {
            onLoaded = null;

            if (gameObject == go)
                return;

            _CleanGameObject();

            if (go == null)
            {
                return;
            }

            _OnAssetLoaded(go, ignoreParent);

            eState = EState.Loaded;
        }
        public void LoadAsset(string assetPath, Action<VirtualGameObject> func)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                _CleanGameObject();
                return;
            }
            
            if (string.Equals(sAssetPath, assetPath, StringComparison.Ordinal))
                return;

            _CleanGameObject();

            eState = EState.Loading;
            onLoaded = func;

            sAssetPath = assetPath;
            AddressablesUtil.InstantiateAsync(ref mHandle, sAssetPath, OnAssetLoaded,false);
        }

        protected virtual void _OnAssetProcess(GameObject go)
        {

        }

        private void _OnAssetLoaded(GameObject go, bool ignoreParent)
        {
            gameObject = go;
            transform = go?.transform;

            _OnAssetProcess(go);

            if (!ignoreParent)
            {
                _SetParent();
            }

            int childrenCount = children == null ? 0 : children.Count;
            for (int i = 0; i < childrenCount; ++i)
            {
                children[i]._SetParent();
            }
        }
        
        private void OnAssetLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Failed)
            {
                eState = EState.Failed;
            }
            else
            {
                _OnAssetLoaded(handle.Result, false);
                eState = EState.Loaded;
            }
            onLoaded?.Invoke(this);
            onLoaded = null;
        }        

        public void Clear()
        {
            _CleanGameObject();
            onLoaded = null;
        }

        protected virtual void _CleanGameObject()
        {
            if (eState == EState.Empty)
                return;

            eState = EState.Empty;

            //标记gameObject 为空
            GameObject temp = gameObject;
            gameObject = null;
            transform = null;

            //重置子节点的parent
            int childrenCount = children == null ? 0 : children.Count;
            for (int i = 0; i < childrenCount; ++i)
            {
                children[i]._SetParent();
            }

            //删除gameObject            
            AddressablesUtil.ReleaseInstance(ref mHandle, OnAssetLoaded);

            if (temp != null)
            {
                GameObject.DestroyImmediate(temp);
            }
            sAssetPath = null;
        }

        public virtual void Dispose()
        {
            SetParent(null, null);
            int childrenCount = children == null ? 0 : children.Count;

            if (childrenCount > 0)
            {
                for (int i = 0; i < childrenCount; ++i)
                {
                    children[i].sAttachSlotPath = null;
                    children[i].parent = null;
                    children[i]._SetParent();
                }
                //TODO 低内存时可以 释放掉
                children.Clear();
            }

            _CleanGameObject();

            onLoaded = null;
            id = 0;
            activeSelf = true;
            bHideIfParentNotLoaded = true;

            _localPosition = Vector3.zero;
            _localRotation = Quaternion.identity;
            _localScale = Vector3.one;
            //_Dispose();
        }
        public void SetParent(VirtualGameObject virtualGameObject, string attachSlotPath)
        {
            attachSlotPath = virtualGameObject == null ? null : attachSlotPath;
            if (virtualGameObject == parent && string.Equals(attachSlotPath, sAttachSlotPath, StringComparison.Ordinal))
                return;

            parent?._RemoveChild(this);
            parent = virtualGameObject;
            parent?._AddChild(this);

            sAttachSlotPath = attachSlotPath;
            _SetParent();
        }
        private void _RemoveChild(VirtualGameObject virtualGameObject)
        {
            for (int i = 0; i < children.Count; ++i)
            {
                if (children[i] == virtualGameObject)
                {
                    children.RemoveAt(i);
                    return;
                }
            }
        }
        private void _AddChild(VirtualGameObject virtualGameObject)
        {
            children.Add(virtualGameObject);
        }
        private void _Dispose()
        {
            int childrenCount = children == null ? 0 : children.Count;

            if (childrenCount > 0)
            {
                for (int i = 0; i < childrenCount; ++i)
                {
                    children[i]._Dispose();
                }
                //TODO 低内存时可以 释放掉
                children.Clear();
            }

            _CleanGameObject();
            parent = null;
            sAttachSlotPath = null;
            onLoaded = null;
        }
        private void _SetParent()
        {
            if (gameObject == null)
                return;

            Transform parentTransform = null;

            if (parent != null)
            {
                if (!string.IsNullOrWhiteSpace(sAttachSlotPath))
                {
                    if (parent.gameObject != null)
                    {
                        parent.transform.TryGetChildByName(sAttachSlotPath, out parentTransform);
                    }
                }
                else
                {
                    parentTransform = parent.transform;
                }
            }

            transform.SetParent(parentTransform, false);
            transform.localPosition = _localPosition;
            transform.localRotation = _localRotation;
            transform.localScale = _localScale;

            _SetActive();
        }
        private void _SetActive()
        {
            bool active = activeSelf && (bHideIfParentNotLoaded == false || parent == null || parent.gameObject != null);
            gameObject?.SetActive(active);
        }
    }
}