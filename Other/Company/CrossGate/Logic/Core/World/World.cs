using Lib.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core
{
    public sealed class World
    {
        public static T AllocActor<T>(ulong uid, Action<T> action = null) where T : Actor, new()
        {            
            T actor = PoolManager.Fetch<T>();
            actor.uID = uid;
            action?.Invoke(actor);
            actor.Construct();
            return actor;
        }        

        public static void CollecActor<T>(ref T actor) where T : Actor, new()
        {
            actor.Dispose();
            PoolManager.Recycle(actor, false);
            actor = null;
        }

        public static void CollecActor(Actor actor)
        {
            actor.Dispose();
            PoolManager.Recycle(actor, false);
        }
#if false
        internal Transform RootTransform = null;
        internal bool bAutoRegisterUpdate = true;

        Dictionary<Type, Dictionary<ulong, Actor>> mActors = new Dictionary<Type, Dictionary<ulong, Actor>>();
        List<IUpdateCmd> mActorUpdates = new List<IUpdateCmd>();        

        Dictionary<Type, List<IUpdateCmd>> mUpdates = new Dictionary<Type, List<IUpdateCmd>>();        

        List<List<IUpdateCmd>> mUpdateList = new List<List<IUpdateCmd>>();        

        public World(string name, bool autoRegisterUpdate = true)
        {
            GameObject root = new GameObject(name);
            GameObject.DontDestroyOnLoad(root);

            RootTransform = root.transform;            
            bAutoRegisterUpdate = autoRegisterUpdate;
        }

        [System.Obsolete("Use AllocActor")]
        /// <summary>
        /// 请使用这个接口///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        public T CreateActor<T>(ulong uid) where T : Actor, new()
        {
            Type type = typeof(T);
            //T actor = ObjectPool<Actor>.Fetch<T>();
            T actor = PoolManager.Fetch<T>();
            actor.mWorld = this;
            actor.uID = uid;

            _AddActor(type, uid, actor);
            return actor;
        }

        private void _AddActor(Type type, ulong uid, Actor actor)
        {
            Dictionary<ulong, Actor> actors = null;
            if (!mActors.TryGetValue(type, out actors))
            {
                actors = new Dictionary<ulong, Actor>();
                mActors[type] = actors;
            }

            if (!actors.ContainsKey(uid))
            {
                actors.Add(uid, actor);
                IUpdateCmd update = actor as IUpdateCmd;
                if (update != null)
                {
                    mActorUpdates.Add(update);
                }                

                actor.Construct();
            }
            else
            {
                DebugUtil.LogErrorFormat("已经拥有相同ID的 {0} {1}", type.ToString(), uid.ToString());
            }
        }

        //[System.Obsolete("Use GameCenter")]
        //public Actor GetActor(Type type, ulong uid)
        //{
        //    Dictionary<ulong, Actor> actors = null;
        //    if (mActors.TryGetValue(type, out actors))
        //    {
        //        Actor actor = null;
        //        if (actors.TryGetValue(uid, out actor))
        //        {
        //            return actor;
        //        }
        //    }
        //    return default;
        //}

        //public Dictionary<ulong, Actor> GetActorsByType(Type type)
        //{
        //    Dictionary<ulong, Actor> actors = null;
        //    if (mActors.TryGetValue(type, out actors))
        //    {
        //        return actors;
        //    }
        //    return default;
        //}

        public void Update()
        {
            for (int i = mActorUpdates.Count - 1; i >= 0; --i)
            {
                IUpdateCmd actorUpdate = mActorUpdates[i];
                if (actorUpdate.IsValid())
                {
                    actorUpdate.Update();
                }
                else
                {
                    mActorUpdates.RemoveAt(i);
                }
            }

            for (int i = 0; i < mUpdateList.Count; ++i)
            {
                List<IUpdateCmd> updates = mUpdateList[i];
                for (int j = updates.Count - 1; j >= 0; --j)
                {
                    IUpdateCmd update = updates[j];
                    if (update.IsValid())
                    {
                        update.Update();
                    }
                    else
                    {
                        updates.RemoveAt(j);
                    }                    
                }
            }
        }        

        public static T AddComponent<T>(Actor actor) where T : Component, new()
        {
            Type type = typeof(T);

            Component component = null;
            T rlt = null;
            if (!actor.mComponents.TryGetValue(type, out component))
            {
                rlt = PoolManager.Fetch<T>();
                rlt.actor = actor;
                actor.mComponents.Add(type, rlt);
                rlt.Construct();

                IUpdateCmd update = rlt as IUpdateCmd;                

                if(update != null)
                {
                    List<IUpdateCmd> updates = null;
                    if (!actor.mWorld.mUpdates.TryGetValue(type, out updates))
                    {
                        updates = new List<IUpdateCmd>(2);
                        actor.mWorld.mUpdates.Add(type, updates);

                        actor.mWorld.mUpdateList.Add(updates);
                    }
                    updates.Add(update);
                }                               
            }
            else
            {
                rlt = component as T;
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("已经存在组件 {0}, 将返回已有组件的句柄", typeof(T));
#endif
            }
            return rlt;
        }

        //[System.Obsolete("Use GameCenter")]
        //public static T GetComponent<T>(Actor actor) where T : Component, new()
        //{
        //    Component component = null;
        //    T rlt = null;
        //    if (actor.mComponents.TryGetValue(typeof(T), out component))
        //    {
        //        rlt = component as T;
        //    }
        //    return rlt;
        //}

        public void RegisterComponentUpdate(Type type)
        {
            List<IUpdateCmd> updates = null;
            if (!mUpdates.TryGetValue(type, out updates))
            {
                updates = new List<IUpdateCmd>();
                mUpdates.Add(type, updates);

                mUpdateList.Add(updates);
            }
        }

        public void Clear()
        {
            var erator = mActors.GetEnumerator();
            while (erator.MoveNext())
            {
                var erator2 = erator.Current.Value.GetEnumerator();
                while (erator2.MoveNext())
                {
                    Actor actor = erator2.Current.Value;
                    actor.Dispose();
                }
                erator.Current.Value.Clear();
            }
            mActors.Clear();

            mActorUpdates.Clear();

            mUpdates.Clear();
            mUpdateList.Clear();            
        }

        public void Dispose()
        {
            Clear();
            if(RootTransform != null)
            GameObject.DestroyImmediate(RootTransform.gameObject);
        }

        public void DestroyActor(Actor actor)
        {
            if (actor == null) { return; }
            Type type = actor.GetType();

            Dictionary<ulong, Actor> actors = null;
            if (mActors.TryGetValue(type, out actors))
            {
                actors.Remove(actor.uID);
                actor.Dispose();
            }
        }

        public void DestroyActor(Type type, ulong uid)
        {
            Dictionary<ulong, Actor> actors = null;
            if (mActors.TryGetValue(type, out actors))
            {
                Actor actor = null;
                if (actors.TryGetValue(uid, out actor))
                {
                    actors.Remove(uid);
                    actor.Dispose();
                }
            }
        }
#endif
    }
}
