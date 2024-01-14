using System;
using System.Collections.Generic;
using Lib;

namespace Framework
{
    public class FastEnumIntEqualityCompare<TEnum> : IEqualityComparer<TEnum> where TEnum : struct
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

    public class EventEmitter<TEnum> where TEnum : struct
    {
        private Dictionary<TEnum, List<Delegate>> messageTable = null;

        private bool AddDelege(TEnum eventType, Delegate handler)
        {
            if (handler == null)
                return false;

            if (messageTable == null)
            {
                messageTable = new Dictionary<TEnum, List<Delegate>>(new FastEnumIntEqualityCompare<TEnum>());
            }

            List<Delegate> srcHandler = null;
            if (!messageTable.TryGetValue(eventType, out srcHandler) || srcHandler == null)
            {
                messageTable[eventType] = new List<Delegate>(8) { handler };
            }
            else
            {
                if (srcHandler.Contains(handler))
                    DebugUtil.LogErrorFormat("{0} handler is existed", handler.ToString());
                else
                    srcHandler.Add(handler);
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
            }

            return true;
        }

        public void Handle(TEnum eventType, Action handler, bool bAdd)
        {
            if (bAdd)
                AddDelege(eventType, handler);
            else
                RemoveDelegate(eventType, handler);
        }

        public void Handle<T1>(TEnum eventType, Action<T1> handler, bool bAdd)
        {
            if (bAdd)
                AddDelege(eventType, handler);
            else
                RemoveDelegate(eventType, handler);
        }

        public void Handle<T1, T2>(TEnum eventType, Action<T1, T2> handler, bool bAdd)
        {
            if (bAdd)
                AddDelege(eventType, handler);
            else
                RemoveDelegate(eventType, handler);
        }

        public void Handle<T1, T2, T3>(TEnum eventType, Action<T1, T2, T3> handler, bool bAdd)
        {
            if (bAdd)
                AddDelege(eventType, handler);
            else
                RemoveDelegate(eventType, handler);
        }

        public void Handle<T1, T2, T3, T4>(TEnum eventType, Action<T1, T2, T3, T4> handler, bool bAdd)
        {
            if (bAdd)
                AddDelege(eventType, handler);
            else
                RemoveDelegate(eventType, handler);
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
                        DebugUtil.LogErrorFormat("EventEmitter({0}) execute failed", eventType.ToString());

                        RemoveDelegate(eventType, handler[i]);
                        --i;
                    }
                }
            }
        }

        public void Trigger<T1>(TEnum eventType, T1 arg1)
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
                        (handler[i] as Action<T1>)?.Invoke(arg1);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogException(e);
                        DebugUtil.LogErrorFormat("EventEmitter({0}) execute failed", eventType.ToString());

                        RemoveDelegate(eventType, handler[i]);
                        --i;
                    }
                }
            }
        }

        public void Trigger<T1, T2>(TEnum eventType, T1 arg1, T2 arg2)
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
                        (handler[i] as Action<T1, T2>)?.Invoke(arg1, arg2);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogException(e);
                        DebugUtil.LogErrorFormat("EventEmitter({0}) execute failed", eventType.ToString());

                        RemoveDelegate(eventType, handler[i]);
                        --i;
                    }
                }
            }
        }

        public void Trigger<T1, T2, T3>(TEnum eventType, T1 arg1, T2 arg2, T3 arg3)
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
                        (handler[i] as Action<T1, T2, T3>)?.Invoke(arg1, arg2, arg3);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogException(e);
                        DebugUtil.LogErrorFormat("EventEmitter({0}) execute failed", eventType.ToString());

                        RemoveDelegate(eventType, handler[i]);
                        --i;
                    }
                }
            }
        }

        public void Trigger<T1, T2, T3, T4>(TEnum eventType, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
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
                        (handler[i] as Action<T1, T2, T3, T4>)?.Invoke(arg1, arg2, arg3, arg4);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogException(e);
                        DebugUtil.LogErrorFormat("EventEmitter({0}) execute failed", eventType.ToString());

                        RemoveDelegate(eventType, handler[i]);
                        --i;
                    }
                }
            }
        }
    }
}
