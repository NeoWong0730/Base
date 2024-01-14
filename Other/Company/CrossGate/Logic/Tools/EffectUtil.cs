using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Lib.Core;

namespace Logic
{
    /// <summary>
    /// 特效工具类(大世界使用)
    /// </summary>
    public class EffectUtil : Singleton<EffectUtil>
    {
        /// <summary>
        /// 特效标签
        /// </summary>
        [Flags]
        public enum EEffectTag
        {
            None = 1,
            Collection = 2,
            Inquiry = 4,
            InteractiveShow = 8,
            EscortOut = 16,
            EscortIn = 32,
            CollectBirth = 64,
            CollectLock = 128,
            CollectUnLock = 256,
            FullPet = 512,
            OnMount = 1024,
            OffMount = 2048,
            TrackOut = 4096,
            TrackIn = 8192,
            Escort = 16384,
            Recover = 32768,
            BattleProtect = Recover * 2,
            Build = BattleProtect * 2,
            MagicSoul = Build * 2,
        }

        public class EffectBase
        {
            public GameObject EffectGo
            {
                get;
                private set;
            }

            public ulong ActorUID
            {
                get;
                set;
            }

            public EEffectTag EffectTag
            {
                get;
                set;
            }

            public Lib.Core.Timer DestoryTimer
            {
                get;
                private set;
            }

            public Transform ParentRoot
            {
                get;
                private set;
            }

            public EffectBase SetEffectGo(GameObject go)
            {
                EffectGo = go;
                return this;
            }

            public EffectBase SetEffetTag(EEffectTag tag)
            {
                EffectTag = tag;
                return this;
            }

            public EffectBase SetDestroyTimer(float time = 0f)
            {
                if (time != 0f)
                {
                    DestoryTimer?.Cancel();
                    DestoryTimer = Lib.Core.Timer.Register(time, () =>
                    {
                        EffectUtil.Instance.UnloadEffectByTag(ActorUID, EffectTag);
                    });
                }
                return this;
            }

            public EffectBase SetParent(Transform transform)
            {
                ParentRoot = transform;
                if (EffectGo != null)
                {
                    EffectGo.transform.SetParent(transform, false);                   
                }
                return this;
            }

            public EffectBase SetLayer(ELayerMask eLayerMask)
            {
                if (EffectGo != null)
                {
                    EffectGo.transform.Setlayer(eLayerMask);
                }
                return this;
            }

            public EffectBase SetYOffset(float yOffset)
            {
                if (EffectGo != null)
                {
                    EffectGo.transform.localPosition = new Vector3(EffectGo.transform.localPosition.x, EffectGo.transform.localPosition.y + yOffset, EffectGo.transform.localPosition.z);
                }
                return this;
            }

            public EffectBase SetScale(float scaleX, float scaleZ)
            {
                if (EffectGo != null)
                {
                    EffectGo.transform.localScale = new Vector3(scaleX, 1f, scaleZ);
                }

                return this;
            }

            public void Dispose()
            {
                GameObject.DestroyImmediate(EffectGo);
                EffectGo = null;
                EffectTag = EEffectTag.None;
                ActorUID = 0;
                DestoryTimer?.Cancel();
                DestoryTimer = null;

                PoolManager.Recycle(this);
            }

            public void UpdateRotation(Vector3 targetPos, Vector3 offset)
            {
                //EffectGo.transform.LookAt(targetPos);
                //EffectGo.transform.localEulerAngles += offset;
                EffectLookAt effectLookAt = EffectGo.GetNeedComponent<EffectLookAt>();
                effectLookAt.targetPos = targetPos;
                effectLookAt.offset = offset;
            }
        }

        /// <summary>
        /// 缓存资源引用///
        /// </summary>
        private Dictionary<string, AsyncOperationHandle<GameObject>> effectHandlers = new Dictionary<string, AsyncOperationHandle<GameObject>>();

        /// <summary>
        /// 特效集合///
        /// </summary>
        private Dictionary<ulong, List<EffectBase>> effectBases = new Dictionary<ulong, List<EffectBase>>();

        /// <summary>
        /// 加载一个特效(大世界)
        /// </summary>
        /// <param name="actorUID">Actor的UID</param>
        /// <param name="effectPath">特效路径</param>
        /// <param name="transform">挂点</param>
        /// <param name="tag">特效标签</param>
        /// <param name="destroyTime">销毁时间</param>
        public void LoadEffect(ulong actorUID, string effectPath, Transform transform, EEffectTag tag, float destroyTime = 0f, float scaleRatioX = 1f, float scaleRatioZ = 1f, ELayerMask eLayerMask = ELayerMask.Default, float yOffset = 0f)
        {
            if (transform == null)
                return;

            EffectBase effectBase = PoolManager.Fetch(typeof(EffectBase)) as EffectBase;
            effectBase.ActorUID = actorUID;
            AsyncOperationHandle<GameObject> handler;
            if (effectHandlers.TryGetValue(effectPath, out handler))
            {
                if (handler.IsDone && handler.Result != null)
                {
                    GameObject go = GameObject.Instantiate(handler.Result);
                    effectBase.SetEffectGo(go).SetEffetTag(tag).SetDestroyTimer(destroyTime).SetScale(scaleRatioX, scaleRatioZ).SetParent(transform).SetLayer(eLayerMask).SetYOffset(yOffset);
                }
                else
                {
                    Download(effectBase, effectPath, transform, tag, destroyTime, scaleRatioX, scaleRatioZ, eLayerMask, yOffset);
                }
            }
            else
            {
                Download(effectBase, effectPath, transform, tag, destroyTime, scaleRatioX, scaleRatioZ, eLayerMask, yOffset);
            }           
            if (effectBases.ContainsKey(actorUID))
            {
                effectBases[actorUID].Add(effectBase);
            }
            else
            {
                List<EffectBase> effects = new List<EffectBase>();
                effects.Add(effectBase);
                effectBases.Add(actorUID, effects);
            }
        }

        void Download(EffectBase effectBase, string effectPath, Transform transform, EEffectTag tag, float destroyTime = 0f, float scaleRatioX = 1f, float scaleRatioZ = 1f, ELayerMask eLayerMask = ELayerMask.Default, float yOffset = 0f)
        {
            AsyncOperationHandle<GameObject> newHandler = Addressables.LoadAssetAsync<GameObject>(effectPath);
            newHandler.Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                GameObject go = GameObject.Instantiate(handle.Result);
                effectBase.SetEffectGo(go).SetEffetTag(tag).SetDestroyTimer(destroyTime).SetScale(scaleRatioX, scaleRatioZ).SetParent(transform).SetLayer(eLayerMask).SetYOffset(yOffset);
                if (effectHandlers.ContainsKey(effectPath))
                {
                    AsyncOperationHandle<GameObject> oldHandler = effectHandlers[effectPath];
                    if (oldHandler.IsValid())
                        AddressablesUtil.Release(ref oldHandler, null);
                }
                effectHandlers[effectPath] = newHandler;
            };
        }

        /// <summary>
        /// 按标签销毁特效///
        /// </summary>
        /// <param name="actorUID">Actor的UID</param>
        /// <param name="tag">特效标签</param>
        public void UnloadEffectByTag(ulong actorUID, EEffectTag tag)
        {
            if (effectBases.ContainsKey(actorUID))
            {
                for (int index = effectBases[actorUID].Count - 1; index >= 0; --index)
                {
                    if ((tag & effectBases[actorUID][index].EffectTag) > 0)
                    {
                        effectBases[actorUID][index].Dispose();
                        effectBases[actorUID].RemoveAt(index);
                        break;
                    }                    
                }
            }
        }

        public void Dispose()
        {
            var handlerTor = effectHandlers.GetEnumerator();
            while (handlerTor.MoveNext())
            {
                AsyncOperationHandle<GameObject> handler = handlerTor.Current.Value;
                if (handler.IsValid())
                    AddressablesUtil.Release(ref handler, null);
            }
            effectHandlers.Clear();

            foreach (var effects in effectBases.Values)
            {
                foreach (var effect in effects)
                {
                    effect.Dispose();
                }
            }
            effectBases.Clear();
        }

        public void UpdateEffectRotation(ulong actorUID, EEffectTag tag, Vector3 targetPos, Vector3 offset)
        {
            if (effectBases.ContainsKey(actorUID))
            {
                for (int index = 0, len = effectBases[actorUID].Count; index < len; index++)
                {
                    if ((tag & effectBases[actorUID][index].EffectTag) > 0)
                    {
                        effectBases[actorUID][index].UpdateRotation(targetPos, offset);
                    }
                }
            }
        }
    }
}
