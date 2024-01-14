using System;
using System.Collections.Generic;

namespace Lib.Core
{
    // 枚举作为key的box 
    public class FastEnumIntEqualityComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : struct
    {
        public bool Equals(TEnum x, TEnum y)
        {
            return EnumInt32ToInt.Convert(x) == EnumInt32ToInt.Convert(y);
        }

        public int GetHashCode(TEnum obj)
        {
            return EnumInt32ToInt.Convert(obj);
        }
    }

    /// <summary>
    /// 消息分发
    /// </summary>
    /// <typeparam name="TEnum">枚举类型 支持定义多种枚举类型</typeparam>
    public class EventEmitter<TEnum> where TEnum : struct
    {
        private Dictionary<TEnum, List<Delegate>> messageTable = null;        

        private bool AddDelegate(TEnum eventType, Delegate handler)
        {
            if (handler == null)
                return false;

            if (messageTable == null)
            {                
                messageTable = new Dictionary<TEnum, List<Delegate>>(new FastEnumIntEqualityComparer<TEnum>());
            }

            List<Delegate> srcHandler = null;
            if (!messageTable.TryGetValue(eventType, out srcHandler) || srcHandler == null)
            {
                messageTable[eventType] = new List<Delegate>(8) { handler };
            }
            else
            {
                //UnityEngine.Debug.AssertFormat(srcHandler[0].GetType() == handler.GetType(), "{0} Add Delegate Type Not Match with reason: {1} != {2}", eventType.ToString(), srcHandler[0].GetType().ToString(), handler.GetType().ToString());

                if (srcHandler.Contains(handler))
                    DebugUtil.LogErrorFormat("{0} handler is existed", handler.ToString());
                else
                    srcHandler.Add(handler);
                //srcHandler.Contains(handler);
                //Delegate tmp = Delegate.Combine(srcHandler);
                //tmp = Delegate.Combine(tmp, handler);
                //messageTable[eventType] = tmp.GetInvocationList();
            }

            return true;
        }
        private bool RemoveDelegate(TEnum eventType, Delegate handler)
        {
            if (handler == null)
                return false;

            if (messageTable == null)
                return false;

            List<Delegate> srcHandler = null;
            if (messageTable.TryGetValue(eventType, out srcHandler) && srcHandler != null)
            {
                srcHandler.Remove(handler);
                if (srcHandler.Count == 0)
                    messageTable.Remove(eventType);
                //Delegate tmp = Delegate.Combine(srcHandler);
                //tmp = Delegate.Remove(tmp, handler);
                //if (tmp == null)
                //{
                //    messageTable.Remove(eventType);
                //}
                //else
                //{
                //    messageTable[eventType] = tmp.GetInvocationList();
                //}
            }

            return true;
        }

        public void Handle(TEnum eventType, Action handler, bool bAdd)
        {
            if (bAdd)
            {
                AddDelegate(eventType, handler);
            }
            else
            {
                RemoveDelegate(eventType, handler);
            }
        }
        public void Handle<TArg0>(TEnum eventType, Action<TArg0> handler, bool bAdd)
        {
            if (bAdd)
            {
                AddDelegate(eventType, handler);
            }
            else
            {
                RemoveDelegate(eventType, handler);
            }
        }
        public void Handle<TArg0, TArg1>(TEnum eventType, Action<TArg0, TArg1> handler, bool bAdd)
        {
            if (bAdd)
            {
                AddDelegate(eventType, handler);
            }
            else
            {
                RemoveDelegate(eventType, handler);
            }
        }
        public void Handle<TArg0, TArg1, TArg2>(TEnum eventType, Action<TArg0, TArg1, TArg2> handler, bool bAdd)
        {
            if (bAdd)
            {
                AddDelegate(eventType, handler);
            }
            else
            {
                RemoveDelegate(eventType, handler);
            }
        }
        public void Handle<TArg0, TArg1, TArg2, TArg3>(TEnum eventType, Action<TArg0, TArg1, TArg2, TArg3> handler, bool bAdd)
        {
            if (bAdd)
            {
                AddDelegate(eventType, handler);
            }
            else
            {
                RemoveDelegate(eventType, handler);
            }
        }
        public void Trigger(TEnum eventType)
        {
            if (messageTable == null)
                return;

            List<Delegate> handler = null;
            if (messageTable.TryGetValue(eventType, out handler))
            {
                for (int i = 0; i < handler.Count; ++i)
                {
                    try
                    {
                        (handler[i] as Action)?.Invoke();
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogException(e);
                        DebugUtil.LogErrorFormat("EventEmitter ({0}) 处理失败", eventType.ToString());

                        RemoveDelegate(eventType, handler[i]);
                        --i;
                    }
                }
            }
        }
        public void Trigger<TArg0>(TEnum eventType, TArg0 arg)
        {
            if (messageTable == null)
                return;

            List<Delegate> handler = null;
            if (messageTable.TryGetValue(eventType, out handler))
            {
                for (int i = 0; i < handler.Count; ++i)
                {
                    try
                    {
                        (handler[i] as Action<TArg0>)?.Invoke(arg);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogException(e);
                        DebugUtil.LogErrorFormat("EventEmitter ({0}) 处理失败", eventType.ToString());

                        RemoveDelegate(eventType, handler[i]);
                        --i;
                    }
                }
            }
        }
        public void Trigger<TArg0, TArg1>(TEnum eventType, TArg0 arg, TArg1 arg1)
        {
            if (messageTable == null)
                return;

            List<Delegate> handler = null;
            if (messageTable.TryGetValue(eventType, out handler))
            {
                for (int i = 0; i < handler.Count; ++i)
                {
                    try
                    {
                        (handler[i] as Action<TArg0, TArg1>)?.Invoke(arg, arg1);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogException(e);
                        DebugUtil.LogErrorFormat("EventEmitter ({0}) 处理失败", eventType.ToString());

                        RemoveDelegate(eventType, handler[i]);
                        --i;
                    }
                }
            }
        }
        public void Trigger<TArg0, TArg1, TArg2>(TEnum eventType, TArg0 arg, TArg1 arg1, TArg2 arg2)
        {
            if (messageTable == null)
                return;

            List<Delegate> handler = null;
            if (messageTable.TryGetValue(eventType, out handler))
            {
                for (int i = 0; i < handler.Count; ++i)
                {
                    try
                    {
                        (handler[i] as Action<TArg0, TArg1, TArg2>)?.Invoke(arg, arg1, arg2);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogException(e);
                        DebugUtil.LogErrorFormat("EventEmitter ({0}) 处理失败", eventType.ToString());

                        RemoveDelegate(eventType, handler[i]);
                        --i;
                    }
                }
            }
        }
        public void Trigger<TArg0, TArg1, TArg2, TArg3>(TEnum eventType, TArg0 arg, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            if (messageTable == null)
                return;

            List<Delegate> handler = null;
            if (messageTable.TryGetValue(eventType, out handler))
            {
                for (int i = 0; i < handler.Count; ++i)
                {
                    try
                    {
                        (handler[i] as Action<TArg0, TArg1, TArg2, TArg3>)?.Invoke(arg, arg1, arg2, arg3);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogException(e);
                        DebugUtil.LogErrorFormat("EventEmitter ({0}) 处理失败", eventType.ToString());

                        RemoveDelegate(eventType, handler[i]);
                        --i;
                    }
                }
            }
        }
    }
}
