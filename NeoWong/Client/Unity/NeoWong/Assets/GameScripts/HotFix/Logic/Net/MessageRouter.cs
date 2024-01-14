using System;
using System.Collections.Generic;
using System.Reflection;
using NWFramework;

namespace HotFix
{
    class Msg
    {
        public IConnection conn;
        public Google.Protobuf.IMessage message;
    }

    /// <summary>
    /// 消息分发器///
    /// </summary>
    public class MessageRouter : Singleton<MessageRouter>
    {
        bool running = false;

        public bool Running { get { return running; } }

        private Queue<Msg> messageQueue = new Queue<Msg>();

        public delegate void MessageHandler<T>(IConnection conn, T msg);

        private Dictionary<string, Delegate> delegateMap = new Dictionary<string, Delegate>();

        public void Subscribe<T>(MessageHandler<T> handler) where T : Google.Protobuf.IMessage
        {
            string type = typeof(T).FullName;
            if (!delegateMap.ContainsKey(type))
            {
                delegateMap[type] = null;
            }
            delegateMap[type] = (MessageHandler<T>)delegateMap[type] + handler;
        }

        public void Off<T>(MessageHandler<T> handler) where T : Google.Protobuf.IMessage
        {
            string key = typeof(T).FullName;
            if (!delegateMap.ContainsKey(key))
            {
                delegateMap[key] = null;
            }
            delegateMap[key] = (MessageHandler<T>)delegateMap[key] - handler;
        }

        void Fire<T>(Connection conn, T msg)
        {
            string type = typeof(T).FullName;
            if (delegateMap.ContainsKey(type))
            {
                MessageHandler<T> handler = (MessageHandler<T>)delegateMap[type];

                try
                {
                    handler?.Invoke(conn, msg);
                }
                catch (Exception e)
                {
                    Log.Error($"MessageRouter.Fire Error: {e.StackTrace}");
                }
            }
        }

        public void AddMessage(IConnection _conn, Google.Protobuf.IMessage _message)
        {
            messageQueue.Enqueue(new Msg() { conn = _conn, message = _message });
        }

        public void Start()
        {
            if (running)
                return;

            running = true;
        }

        public void Stop()
        {
            running = false;
            messageQueue.Clear();
        }

        public void Update()
        {
            if (!running)
                return;

            try
            {
                if (messageQueue.Count > 0)
                {
                    Msg msg = messageQueue.Dequeue();
                    Google.Protobuf.IMessage package = msg.message;
                    if (package != null)
                    {
                        ExecuteMessage(msg.conn, package);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.StackTrace);
            }
        }

        private void ExecuteMessage(IConnection conn, Google.Protobuf.IMessage message)
        {
            var fireMethod = this.GetType().GetMethod("Fire", BindingFlags.NonPublic | BindingFlags.Instance);
            var met = fireMethod.MakeGenericMethod(message.GetType());
            met.Invoke(this, new object[] { conn, message });

            var t = message.GetType();
            foreach (var p in t.GetProperties())
            {
                if ("Parser" == p.Name || "Descriptor" == p.Name)
                    continue;

                var value = p.GetValue(message);
                if (value != null)
                {
                    if (typeof(Google.Protobuf.IMessage).IsAssignableFrom(value.GetType()))
                    {
                        ExecuteMessage(conn, (Google.Protobuf.IMessage)value);
                    }
                }
            }
        }
    }
}
