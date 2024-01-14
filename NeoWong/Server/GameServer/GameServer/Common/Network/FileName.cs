using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Serilog;

namespace Common.Network
{
    class Msg
    {
        public Connection conn;
        public Google.Protobuf.IMessage message;
    }

    /// <summary>
    /// 消息分发器///
    /// </summary>
    public class MessageRouter : Singleton<MessageRouter>
    {
        int threadCount = 1;
        int workerCount = 0;
        bool running = false;

        public bool Running { get { return running; } }

        AutoResetEvent threadEvent = new AutoResetEvent(true);

        private Queue<Msg> messageQueue = new Queue<Msg>();

        public delegate void MessageHandler<T>(Connection conn, T msg);

        private Dictionary<string, Delegate> delegateMap = new Dictionary<string, Delegate>();

        public void Subscribe<T>(MessageHandler<T> handler) where T : Google.Protobuf.IMessage
        {
            string type = typeof(T).FullName;
            if (!delegateMap.ContainsKey(type))
            {
                delegateMap[type] = null;
            }
            delegateMap[type] = (MessageHandler<T>)delegateMap[type] + handler;
            Log.Debug(type + ":" + delegateMap[type].GetInvocationList().Length);
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

        public void AddMessage(Connection _conn, Google.Protobuf.IMessage _message)
        {
            lock (messageQueue)
            {
                messageQueue.Enqueue(new Msg() { conn = _conn, message = _message });
            }
            threadEvent.Set();
        }

        public void Start(int _threadCount)
        {
            if (running) return;
            running = true;
            threadCount = Math.Min(Math.Max(_threadCount, 1), 200);
            ThreadPool.SetMinThreads(threadCount + 20, threadCount + 20);
            for (int i = 0; i < threadCount; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(MessageWork));
            }

            while (workerCount < threadCount)
            {
                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            running = false;
            messageQueue.Clear();
            while (workerCount > 0)
            {
                threadEvent.Set();
            }
            Thread.Sleep(100);
        }

        private void MessageWork(object state)
        {
            Log.Information("Worker thread start");
            try
            {
                workerCount = Interlocked.Increment(ref workerCount);
                while (running)
                {
                    if (messageQueue.Count == 0)
                    {
                        threadEvent.WaitOne();
                        continue;
                    }
                    Msg msg = null;
                    lock (messageQueue)
                    {
                        if (messageQueue.Count == 0) continue;
                        msg = messageQueue.Dequeue();
                    }
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
            finally
            {
                workerCount = Interlocked.Decrement(ref workerCount);
            }
            Log.Information("Worker thread end");
        }

        private void ExecuteMessage(Connection conn, Google.Protobuf.IMessage message)
        {
            var fireMethod = this.GetType().GetMethod("Fire", BindingFlags.NonPublic | BindingFlags.Instance);
            var met = fireMethod.MakeGenericMethod(message.GetType());
            met.Invoke(this, new object[] { conn, message });

            var t = message.GetType();
            foreach (var p in t.GetProperties())
            {
                //过滤属性
                if ("Parser" == p.Name || "Descriptor" == p.Name) continue;
                var value = p.GetValue(message);
                if (value != null)
                {
                    if (typeof(Google.Protobuf.IMessage).IsAssignableFrom(value.GetType()))
                    {
                        //继续递归
                        ExecuteMessage(conn, (Google.Protobuf.IMessage)value);
                    }
                }
            }
        }
    }
}
