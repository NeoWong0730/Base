using System;
using System.Collections.Generic;

namespace Logic
{
    public static class ZeroArray {
        private static Dictionary<Type, Object> zeroArrayDict = new Dictionary<Type, Object>();

        public static int Count {
            get {
                return zeroArrayDict.Count;
            }
        }

        public static T[] GetZeroArray<T>()
        {
            T[] rlt = null;
            Type type = typeof(T);
            if (zeroArrayDict.TryGetValue(type, out var v))
            {
                rlt = v as T[];
            }
            else
            {
                rlt = new T[0];
                zeroArrayDict.Add(type, rlt);
            }
            return rlt;
        }
    }
}
