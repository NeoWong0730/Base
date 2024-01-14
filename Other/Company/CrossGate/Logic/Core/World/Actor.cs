using System;
using System.Collections.Generic;

namespace Logic.Core
{
    public abstract class Actor
    {
        public static T Create<T>(ulong uid) where T : Actor, new()
        {
            T actor = PoolManager.Fetch<T>();
            actor.uID = uid;
            actor.Construct();
            return actor;
        }

        public static void Destroy(ref Actor actor)
        {
            actor.Dispose();
            PoolManager.Recycle(actor, false);
            actor = null;
        }

        public World mWorld { get; internal set; }

        public ulong uID { get; internal set; }
        protected bool bValid = true;

        internal Dictionary<Type, Component> mComponents;
        
        public bool IsValid() { return bValid; }

        public virtual bool UnValidDelete()
        {
            return true;
        }

        internal Actor() { }
        internal void Construct()
        {
            mComponents = new Dictionary<Type, Component>();
            OnConstruct();
            bValid = true;
        }

        internal void Dispose()
        {
            bValid = false;
            OnDispose();
            Dictionary<Type, Component>.Enumerator enumerator = mComponents.GetEnumerator();

            Component component = null;
            while (enumerator.MoveNext())
            {
                component = enumerator.Current.Value;
                component.Dispose();                
            }
            mComponents.Clear();

            mWorld = null;
            uID = 0;
            component = null;
            //PoolManager.Recycle(this, false);
        }

        protected virtual void OnConstruct() { }
        protected virtual void OnDispose() { }                
    }
}
