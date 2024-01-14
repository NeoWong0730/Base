using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic.Core
{
    public class AnimationClipLoader
    {
        AsyncOperationHandle<AnimationClip> _handle;
        System.Action<AnimationClipLoader> _action;

        public string RealName;
        public AnimationClip Result;

        public void Start(string realname, System.Action<AnimationClipLoader> action)
        {
            RealName = realname;
            _action = action;
            _handle = Addressables.LoadAssetAsync<AnimationClip>(realname);
            _handle.Completed += Complete;
        }

        private void Complete(AsyncOperationHandle<AnimationClip> handle)
        {
            Result = _handle.Result;
            _action?.Invoke(this);
        }

        public void Release()
        {
            if (_handle.IsValid())
            {
                _handle.Completed -= Complete;
                AddressablesUtil.Release(ref _handle, Complete);
            }

            Result = null;
            _action = null;
        }
    }

    public class AssetsGroupLoader
    {
        public struct Progress
        {
            public int step;
            public string name;
        }

        private Progress progress;

        public List<string> names { get; private set; } = new List<string>();
        public List<string> descs { get; private set; } = new List<string>();
        public List<AnimationClipLoader> assetRequests = new List<AnimationClipLoader>();
        private int step = 0;

        private Action _groupLoadStart;
        private Action _groupLoadOver;
        private Action<Progress> _groupLoadProgress;

        private int count { get { return names.Count; } }

        public void AddLoadTask(string name, string desc = null)
        {
            if (!string.IsNullOrEmpty(name)) 
            {
                names.Add(name);
                descs.Add(desc);
            }
        }

        public void RemoveLoadTask(string name, string desc = null)
        {
            names.Remove(name);
            descs.Remove(desc);
        }


        public void StartLoad(Action groupLoadStart, Action groupLoadOver, Action<Progress> groupLoadProgress)
        {
            _groupLoadStart = groupLoadStart;
            _groupLoadOver = groupLoadOver;
            _groupLoadProgress = groupLoadProgress;

            if (count > 0)
            {
                _groupLoadStart?.Invoke();
                for (int i = 0, length = names.Count; i < length; ++i)
                {
                    string realName = names[i];
                    AnimationClipLoader aniLoader = new AnimationClipLoader();
                    assetRequests.Add(aniLoader);
                    aniLoader.Start(realName, OnClipLoadEnd);
                }
            }
            else
            {
                _groupLoadOver?.Invoke();
            }
        }

        private void OnClipLoadEnd(AnimationClipLoader loader)
        {
            step++;
            progress.name = loader.RealName;
            progress.step = step;
            _groupLoadProgress?.Invoke(progress);
            if (step == count)
            {
                _groupLoadOver?.Invoke();
            }
        }

        public void UnloadAll()
        {
            for (int i = 0, length = assetRequests.Count; i < length; ++i) 
            {
                assetRequests[i].Release();
            }
            assetRequests.Clear();
            
            names.Clear();
            descs.Clear();
            step = 0;
        }
    }
}
