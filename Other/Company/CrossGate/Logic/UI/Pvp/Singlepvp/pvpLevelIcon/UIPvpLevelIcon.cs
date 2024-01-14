using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    public  class UIPvpLevelIcon
    {
        Dictionary<string, GameObject> m_HadLoadLevelIcon = new Dictionary<string, GameObject>();
        Dictionary<string, AsyncOperationHandle<GameObject>> m_HandDic = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        public GameObject CurLevelIcon { get; private set; }

        public Transform Parent { get; set; }
        public void ShowLevelIcon(string name,Transform parentTransform = null)
        {
            if (m_HadLoadLevelIcon.ContainsKey(name))
            {
                if (CurLevelIcon == m_HadLoadLevelIcon[name])
                    return;

                m_HadLoadLevelIcon[name].SetActive(true);

                if (CurLevelIcon != null)
                    CurLevelIcon.SetActive(false);

                CurLevelIcon = m_HadLoadLevelIcon[name];

                return;
            }
            LoadLevel(name, parentTransform);

        }
        private void LoadLevel(string name, Transform parentTransform = null)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (m_HandDic.ContainsKey(name))
                return;

            AsyncOperationHandle<GameObject> handle = default;
            AddressablesUtil.InstantiateAsync(ref handle, name, LoadCompleter, true, parentTransform == null ? Parent : parentTransform);
            m_HandDic.Add(name, handle);
        }

        private void LoadCompleter(AsyncOperationHandle<GameObject> handle)
        {
            m_HadLoadLevelIcon.Add(handle.DebugName, handle.Result);

            if (CurLevelIcon != null)
                CurLevelIcon.SetActive(false);

            CurLevelIcon = handle.Result;
        }


        public void Destory()
        {
            foreach (var kvp in m_HandDic)
            {
                AsyncOperationHandle<GameObject> handle = kvp.Value;
                AddressablesUtil.ReleaseInstance(ref handle, LoadCompleter);
            }

            m_HandDic.Clear();
            m_HadLoadLevelIcon.Clear();
        }
    }
}
