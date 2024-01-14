using System.Collections.Generic;

namespace NWFramework
{
    /// <summary>
    /// 通用的属性存储
    /// </summary>
    public class TypeAttributeStore
    {
        private Dictionary<string, object> _dict = new Dictionary<string, object>();

        public void Set<T>(T value)
        {
            string key = typeof(T).FullName;
            _dict[key] = value;
        }

        public T Get<T>()
        {
            string key = typeof(T).FullName;
            if (_dict.ContainsKey(key))
            {
                return (T)_dict[key];
            }
            return default(T);
        }
    }
}
