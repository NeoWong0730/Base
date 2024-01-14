using System.Collections.Generic;
using UnityEngine;
using Lib;

namespace Framework
{
    public class ObjectPool<TValue>
    {
        private int limit;
        private Stack<TValue> pool;
        private System.Func<TValue> createAction;
        private System.Action<TValue> destroyAction;
        public ObjectPool(int limit, System.Func<TValue> createAction = null, System.Action<TValue> destroyAction = null)
        {
            this.limit = limit;
            this.createAction = createAction;
            this.destroyAction = destroyAction;
            pool = new Stack<TValue>(limit);
        }
        public TValue Get()
        {
            TValue ret = default(TValue);
            if (pool.Count > 0)
            {
                ret = pool.Pop();
            }
            else
            {
                if (createAction != null)
                {
                    ret = createAction.Invoke();
                }
            }
            return ret;
        }
        public bool Push(TValue target)
        {
            bool ret = false;
            if (target != null)
            {
                if (pool.Count < limit)
                {
                    ret = true;
                    pool.Push(target);
                }
                else
                {
                    destroyAction?.Invoke(target);
                }
            }
            return ret;
        }
        public void Dispose()
        {
            createAction = null;
            destroyAction = null;
            while (pool.Count > 0)
            {
                TValue target = pool.Pop();
                destroyAction?.Invoke(target);
            }
            pool.Clear();
        }
    }

    public class GameObjectPool
    {
        private int limit;
        private Stack<GameObject> _pool;
        private GameObject _template;
        public int count
        {
            get
            {
                return _pool.Count;
            }
        }

        public GameObjectPool(int limit, GameObject template)
        {
            this.limit = limit;
            _pool = new Stack<GameObject>(limit);
            _template = template;
        }
        public GameObject Get(GameObject parent)
        {
            GameObject ret;
            if (_pool.Count > 0)
            {
                ret = _pool.Pop();
            }
            else
            {
                ret = GameObject.Instantiate(_template, parent.transform);
            }
            return ret;
        }
        public GameObject Get(Transform parent)
        {
            GameObject ret;
            if (_pool.Count > 0)
            {
                ret = _pool.Pop();
            }
            else
            {
                if (_template == null)
                {
                    DebugUtil.LogErrorFormat("GameObjectPool.Get() ===> the object you want to instantiate is null==>_template=null");
                    return null;
                }
                ret = GameObject.Instantiate(_template, parent);
            }
            return ret;
        }

        public void Recovery(GameObject target)
        {
            if (target != null)
            {
                if (_pool.Count < limit)
                {
                    _pool.Push(target);
                }
                else
                {
                    GameObject.Destroy(target);
                }
            }
        }

        public void Dispose()
        {
            _template = null;
            while (_pool.Count > 0)
            {
                GameObject target = _pool.Pop();
                GameObject.DestroyImmediate(target);
            }
        }

        public void Clear()
        {
            _pool.Clear();
        }
    }
}
