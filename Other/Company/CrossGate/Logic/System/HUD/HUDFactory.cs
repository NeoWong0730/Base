using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using System;

namespace Logic
{
    public interface IHUDComponent : IDisposable
    {

    }

    public static class HUDFactory
    {
        private static Dictionary<Type, Queue<IHUDComponent>> pool = new Dictionary<Type, Queue<IHUDComponent>>();

        public static T Get<T>() where T : IHUDComponent, new()
        {
            Type type = typeof(T);
            Queue<IHUDComponent> objectPool;
            if (!pool.TryGetValue(type, out objectPool))
            {
                objectPool = new Queue<IHUDComponent>();
                pool.Add(type, objectPool);
            }
            IHUDComponent hUDComponent;
            hUDComponent= objectPool.Count > 0? objectPool.Dequeue(): new T();
            return (T)hUDComponent;
        }

        public static void Recycle(IHUDComponent hUDComponent)
        {
            Type type = hUDComponent.GetType();
            Queue<IHUDComponent> objectPool;
            if (!pool.TryGetValue(type, out objectPool))
            {
                objectPool = new Queue<IHUDComponent>();
                pool.Add(type, objectPool);
            }
            objectPool.Enqueue(hUDComponent);
        }
    }
}

