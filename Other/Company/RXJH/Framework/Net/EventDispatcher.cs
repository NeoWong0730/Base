using System;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using Lib.Core;

namespace Net
{
    public delegate void EventListenerDelegate(NetMsg msg);

    public struct NetEventHandle
    {
        public MessageParser Parser;
        public EventListenerDelegate Func;
    }

    public class EventDispatcher
    {
        private EventDispatcher() { }
        //TODO 历史遗留问题 这个静态类就行
        public static EventDispatcher Instance { get; } = new EventDispatcher();

        private readonly Dictionary<UInt16, NetEventHandle> eventListeners = new Dictionary<UInt16, NetEventHandle>();

        public void AddEventListener(ushort first, ushort reqSecond, ushort resSecond, EventListenerDelegate listener, MessageParser parser)
        {
            AddEventListener(NetMsgUtil.CalMsgCode(first, reqSecond), NetMsgUtil.CalMsgCode(first, resSecond), listener, parser);
        }
        
        private void AddEventListener(ushort request, ushort response, EventListenerDelegate listener, MessageParser parser)
        {
            if (listener == null)
                return;

#if DEBUG_MODE
            if (this.eventListeners.ContainsKey(response))
            {
                DebugUtil.LogErrorFormat("MsgId: {0} is repeated regist!", response);
            }
#endif
            this.eventListeners[response] = new NetEventHandle() { Parser = parser, Func = listener };

        }

        public void RemoveEventListener(ushort first, ushort second, EventListenerDelegate listener) {
            RemoveEventListener(NetMsgUtil.CalMsgCode(first, second), listener);
        }

        public void RemoveEventListener(UInt16 eventType, EventListenerDelegate listener)
        {
            NetEventHandle func;
            if (this.eventListeners.TryGetValue(eventType, out func))
            {
                if (func.Func != listener)
                {
                    DebugUtil.LogErrorFormat("EventDispatcher RemoveEventListener eventID ={0} Fail, because func not Equals !", eventType);
                }
                else
                {
                    this.eventListeners.Remove(eventType);
                }
            }
        }
        public void Clear()
        {
            this.eventListeners.Clear();
        }

        public bool TryGetEventHandle(ushort evtId, out NetEventHandle handler)
        {
            return eventListeners.TryGetValue(evtId, out handler);
        }

#if DEBUG_MODE
        public HashSet<int> allowLogs = new HashSet<int>();
        public bool allowAll = true;
        public bool FilterLog(int evtId)
        {
            if (allowAll || allowLogs.Contains(evtId))
            {
                return true;
            }
            return false;
        }
#endif
    }
}