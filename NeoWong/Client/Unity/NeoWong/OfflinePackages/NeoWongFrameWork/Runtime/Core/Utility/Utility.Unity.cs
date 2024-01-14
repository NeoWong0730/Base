using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

namespace NWFramework
{
    public static partial class Utility
    {
        /// <summary>
        /// Unity相关的实用函数
        /// </summary>
        public static partial class Unity
        {
            private static GameObject _entity;
            private static MainBehaviour _behaviour;

            #region 控制协程Coroutine

            public static Coroutine StartCoroutine(string methodName)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    return null;
                }

                MakeEntity();
                return _behaviour.StartCoroutine(methodName);
            }

            public static Coroutine StartCoroutine(IEnumerator routine)
            {
                if (routine == null)
                {
                    return null;
                }

                MakeEntity();
                return _behaviour.StartCoroutine(routine);
            }

            public static Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    return null;
                }

                MakeEntity();
                return _behaviour.StartCoroutine(methodName, value);
            }

            public static void StopCoroutine(string methodName)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    return;
                }

                if (_entity != null)
                {
                    _behaviour.StopCoroutine(methodName);
                }
            }

            public static void StopCoroutine(IEnumerator routine)
            {
                if (routine == null)
                {
                    return;
                }

                if (_entity != null)
                {
                    _behaviour.StopCoroutine(routine);
                }
            }

            public static void StopCoroutine(Coroutine routine)
            {
                if (routine == null)
                    return;

                if (_entity != null)
                {
                    _behaviour.StopCoroutine(routine);
                    routine = null;
                }
            }

            public static void StopAllCoroutines()
            {
                if (_entity != null)
                {
                    _behaviour.StopAllCoroutines();
                }
            }

            #endregion

            #region UnityUpdate/FixedUpdate/LateUpdate 注入

            /// <summary>
            /// 为给外部提供的 添加帧更新事件
            /// </summary>
            /// <param name="fun"></param>
            public static void AddUpdateListener(UnityAction fun)
            {
                MakeEntity();
                _behaviour.AddUpdateListener(fun);
            }

            /// <summary>
            /// 为给外部提供的 添加物理帧更新事件
            /// </summary>
            /// <param name="fun"></param>
            public static void AddFixedUpdateListener(UnityAction fun)
            {
                MakeEntity();
                _behaviour.AddFixedUpdateListener(fun);
            }

            /// <summary>
            /// 为给外部提供的 添加Late帧更新事件
            /// </summary>
            /// <param name="fun"></param>
            public static void AddLateUpdateListener(UnityAction fun)
            {
                MakeEntity();
                _behaviour.AddLateUpdateListener(fun);
            }

            /// <summary>
            /// 移除帧更新事件
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveUpdateListener(UnityAction fun)
            {
                MakeEntity();
                _behaviour.RemoveUpdateListener(fun);
            }

            /// <summary>
            /// 移除物理帧更新事件
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveFixedUpdateListener(UnityAction fun)
            {
                MakeEntity();
                _behaviour.RemoveFixedUpdateListener(fun);
            }

            /// <summary>
            /// 移除Late帧更新事件
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveLateUpdateListener(UnityAction fun)
            {
                MakeEntity();
                _behaviour.RemoveLateUpdateListener(fun);
            }

            #endregion

            #region  Unity Events 注入

            /// <summary>
            /// 为给外部提供的Destroy注册事件
            /// </summary>
            /// <param name="fun"></param>
            public static void AddDestroyListener(UnityAction fun)
            {
                MakeEntity();
                _behaviour.AddDestroyListener(fun);
            }

            /// <summary>
            /// 为给外部提供的Destroy反注册事件
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveDestroyListener(UnityAction fun)
            {
                MakeEntity();
                _behaviour.RemoveDestroyListener(fun);
            }

            /// <summary>
            /// 为给外部提供的OnDrawGizmos注册事件
            /// </summary>
            /// <param name="fun"></param>
            public static void AddOnDrawGizmosListener(UnityAction fun)
            {
                MakeEntity();
                _behaviour.AddOnDrawGizmosListener(fun);
            }

            /// <summary>
            /// 为给外部提供的OnDrawGizmos反注册事件
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveOnDrawGizmosListener(UnityAction fun)
            {
                MakeEntity();
                _behaviour.RemoveOnDrawGizmosListener(fun);
            }

            /// <summary>
            /// 为给外部提供的OnApplicationPause注册事件
            /// </summary>
            /// <param name="fun"></param>
            public static void AddOnApplicationPauseListener(UnityAction<bool> fun)
            {
                MakeEntity();
                _behaviour.AddOnApplicationPauseListener(fun);
            }

            /// <summary>
            /// 为给外部提供的OnApplicationPause反注册事件
            /// </summary>
            /// <param name="fun"></param>
            public static void RemoveOnApplicationPauseListener(UnityAction<bool> fun)
            {
                MakeEntity();
                _behaviour.RemoveOnApplicationPauseListener(fun);
            }

            #endregion

            private static void MakeEntity()
            {
                if (_entity != null)
                {
                    return;
                }

                _entity = new GameObject("[Unity.Utility]");
                _entity.SetActive(true);
                _entity.transform.SetParent(GameModule.Base.transform);

                UnityEngine.Assertions.Assert.IsFalse(_behaviour);
                _behaviour = _entity.AddComponent<MainBehaviour>();
            }

            /// <summary>
            /// 释放Behaviour生命周期
            /// </summary>
            public static void Shutdown()
            {
                _behaviour?.Release();
                if (_entity != null)
                {
                    Object.Destroy(_entity);
                }
                _entity = null;
            }

            private class MainBehaviour : MonoBehaviour
            {
                private event UnityAction UpdateEvent;
                private event UnityAction FixedUpdateEvent;
                private event UnityAction LateUpdateEvent;
                private event UnityAction DestroyEvent;
                private event UnityAction OnDrawGizmosEvent;
                private event UnityAction<bool> OnApplicationPauseEvent;

                void Update()
                {
                    if (UpdateEvent != null)
                    {
                        UpdateEvent();
                    }
                }

                void FixedUpdate()
                {
                    if (FixedUpdateEvent != null)
                    {
                        FixedUpdateEvent();
                    }
                }

                void LateUpdate()
                {
                    if (LateUpdateEvent != null)
                    {
                        LateUpdateEvent();
                    }
                }

                private void OnDestroy()
                {
                    if (DestroyEvent != null)
                    {
                        DestroyEvent();
                    }
                }

                private void OnDrawGizmos()
                {
                    if (OnDrawGizmosEvent != null)
                    {
                        OnDrawGizmosEvent();
                    }
                }

                private void OnApplicationPause(bool pauseStatus)
                {
                    if (OnApplicationPauseEvent != null)
                    {
                        OnApplicationPauseEvent(pauseStatus);
                    }
                }

                public void AddLateUpdateListener(UnityAction fun)
                {
                    LateUpdateEvent += fun;
                }

                public void RemoveLateUpdateListener(UnityAction fun)
                {
                    LateUpdateEvent -= fun;
                }

                public void AddFixedUpdateListener(UnityAction fun)
                {
                    FixedUpdateEvent += fun;
                }

                public void RemoveFixedUpdateListener(UnityAction fun)
                {
                    FixedUpdateEvent -= fun;
                }

                public void AddUpdateListener(UnityAction fun)
                {
                    UpdateEvent += fun;
                }

                public void RemoveUpdateListener(UnityAction fun)
                {
                    UpdateEvent -= fun;
                }

                public void AddDestroyListener(UnityAction fun)
                {
                    DestroyEvent += fun;
                }

                public void RemoveDestroyListener(UnityAction fun)
                {
                    DestroyEvent -= fun;
                }

                public void AddOnDrawGizmosListener(UnityAction fun)
                {
                    OnDrawGizmosEvent += fun;
                }

                public void RemoveOnDrawGizmosListener(UnityAction fun)
                {
                    OnDrawGizmosEvent -= fun;
                }

                public void AddOnApplicationPauseListener(UnityAction<bool> fun)
                {
                    OnApplicationPauseEvent += fun;
                }

                public void RemoveOnApplicationPauseListener(UnityAction<bool> fun)
                {
                    OnApplicationPauseEvent -= fun;
                }

                public void Release()
                {
                    UpdateEvent = null;
                    FixedUpdateEvent = null;
                    LateUpdateEvent = null;
                    OnDrawGizmosEvent = null;
                    DestroyEvent = null;
                    OnApplicationPauseEvent = null;
                }
            }
        }
    }
}
