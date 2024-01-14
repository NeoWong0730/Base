using System;
using System.Collections.Generic;

namespace Logic {
    /// <summary>
    /// 优化：抗辩/斜变 支持 所有引用类型为List<Object>
    /// </summary>
    public static class ZeroList {
        private static Dictionary<Type, Object> zeroListDict = new Dictionary<Type, Object>();

        public static int Count {
            get {
                return zeroListDict.Count;
            }
        }

        public static List<T> GetZeroList<T>() {
            List<T> rlt = null;
            Type type = typeof(T);
            if (zeroListDict.TryGetValue(type, out var v)) {
                rlt = v as List<T>;
            }
            else {
                rlt = new List<T>(0);
                zeroListDict.Add(type, rlt);
            }
            return rlt;
        }
    }

    public static class EmptyList<T> {
        // 不要外部进行add等操作
        public static readonly List<T> Value = new List<T>(0);
    }

    public static class EmptyArray<T> {
        public static readonly T[] Value = new T[0];
    }
}
