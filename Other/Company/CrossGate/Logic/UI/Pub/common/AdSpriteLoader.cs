using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic
{
    public class AdSpriteLoader
    {
        protected Dictionary<string, AsyncOperationHandle<Texture>> m_HandDic = new Dictionary<string, AsyncOperationHandle<Texture>>();
        public Action<string, Texture> ActionCompleter;

        protected Dictionary<string, List<RawImage>> m_WaitImageDic = new Dictionary<string, List<RawImage>>();
        public void SetImage(string name, RawImage image)
        {
            if (image == null || string.IsNullOrEmpty(name))
                return;

            Texture sprite = GetSprite(name);

            if (sprite != null)
            {
                image.texture = sprite;
                return;
            }


            Load(name);

            if (m_WaitImageDic.ContainsKey(name) == false)
                m_WaitImageDic.Add(name, new List<RawImage>());

            m_WaitImageDic[name].Add(image);
        }
        public virtual Texture GetSprite(string name)
        {
            if (m_HandDic.ContainsKey(name))
                return m_HandDic[name].Result;

            return null;
        }
        public virtual void Load(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (m_HandDic.ContainsKey(name))
                return;

            var mHandle = Addressables.LoadAssetAsync<Texture>(name);

            m_HandDic.Add(name, mHandle);

            mHandle.Completed += LoadCompleter;
        }

        protected virtual void LoadCompleter(AsyncOperationHandle<Texture> handle)
        {
            if (handle.IsDone)
                ActionCompleter?.Invoke(handle.DebugName, handle.Result);

            foreach (var kvp in m_HandDic)
            {
                if (kvp.Value.Equals(handle))
                {
                    if (m_WaitImageDic.ContainsKey(kvp.Key))
                    {
                       var imagelist =  m_WaitImageDic[kvp.Key];

                        int count = imagelist.Count;

                        for (int i = 0; i < count; i++)
                        {
                            imagelist[i].texture = handle.Result;
                        }

                        m_WaitImageDic.Remove(kvp.Key);
                    }
                }
            }
        }

        public virtual void OnDestory()
        {
            foreach (var kvp in m_HandDic)
            {
                Addressables.Release(kvp.Value);
            }

            m_HandDic.Clear();
        }
    }

}
