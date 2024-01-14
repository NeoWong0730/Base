using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    /// <summary>
    /// 对粒子特效的加载
    /// </summary>
    class ParticleGroupLoader: AdPrefabLoader
    {
        public Action<string, ParticleSystem> ActionFinal;

        public override void Load(string name, Transform parent)
        {
            base.Load(name, parent);
        }

        protected override void LoadCompleter(AsyncOperationHandle<GameObject> handle)
        {

            if (handle.IsDone && handle.Result != null)
            {
                ParticleSystem particle = handle.Result.GetComponent<ParticleSystem>();

                if (particle == null)
                    particle = handle.Result.GetComponentInChildren<ParticleSystem>();

                ActionFinal?.Invoke(handle.DebugName, particle);
            }

        }

    }
}
