using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    /// <summary>
    /// 用于 管理局部的 资源加载 。目前用在同一个节点下根据不同的条件挂载不同的特效,在不需要时进行统一卸载
    /// </summary>
    public class AdPrefabLoader
    {
        protected Dictionary<string, AsyncOperationHandle<GameObject>> m_HandDic = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        public Action<string, GameObject> ActionCompleter;

        public virtual GameObject GetGameObject(string name)
        {
            if (m_HandDic.ContainsKey(name))
                return m_HandDic[name].Result;

            return null;
        }
        public virtual void Load(string name, Transform parent)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (m_HandDic.ContainsKey(name))
                return;

            AsyncOperationHandle<GameObject> mHandle = default;
            AddressablesUtil.InstantiateAsync(ref mHandle, name, LoadCompleter, true, parent);
            m_HandDic.Add(name, mHandle);
        }

        protected virtual void LoadCompleter(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.IsDone)
                ActionCompleter?.Invoke(handle.DebugName, handle.Result);
        }

        public virtual void OnDestory()
        {
            foreach (var kvp in m_HandDic)
            {
                AsyncOperationHandle<GameObject> handle = kvp.Value;
                AddressablesUtil.ReleaseInstance(ref handle, LoadCompleter);
            }

            m_HandDic.Clear();
        }
    }

}
