using System;

namespace Logic.Core
{
    public abstract class Component
    {
        public Actor actor { get; internal set; }

        bool bValid = true;
        public bool IsValid() { return bValid; }

        internal void Dispose()
        {
            bValid = false;
            OnDispose();

            actor = null;
            //PoolManager.Recycle(this, false);
        }

        internal void Construct()
        {
            bValid = true;
            OnConstruct();
        }

        protected virtual void OnConstruct() { }
        protected virtual void OnDispose() { }
    }
}
