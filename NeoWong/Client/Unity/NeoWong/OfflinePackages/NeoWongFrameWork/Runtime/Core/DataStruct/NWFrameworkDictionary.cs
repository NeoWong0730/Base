using System.Collections.Generic;

namespace NWFramework
{
    /// <summary>
    /// 游戏框架字典类
    /// </summary>
    /// <typeparam name="TKey">指定字典Key的元素类型</typeparam>
    /// <typeparam name="TValue">指定字典Value的元素类型</typeparam>
    public class NWFrameworkDictionary<TKey, TValue>
    {
        protected readonly List<TKey> KeyList = new List<TKey>();
        protected readonly Dictionary<TKey, TValue> Dictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// 存储键的列表
        /// </summary>
        public List<TKey> Keys => KeyList;

        /// <summary>
        /// 存储键的列表长度
        /// </summary>
        public int Count => KeyList.Count;

        /// <summary>
        /// 通过KEY的数组下标获取元素
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns></returns>
        public TValue GetValueByIndex(int index)
        {
            return Dictionary[KeyList[index]];
        }

        /// <summary>
        /// 通过KEY的数组下标设置元素
        /// </summary>
        /// <param name="index">下标</param>
        /// <param name="item">TValue</param>
        public void SetValue(int index, TValue item)
        {
            Dictionary[KeyList[index]] = item;
        }

        /// <summary>
        /// 字典索引器
        /// </summary>
        /// <param name="key">TKey</param>
        public TValue this[TKey key]
        {
            get => Dictionary[key];
            set
            {
                if (!ContainsKey(key))
                {
                    Add(key, value);
                }
                else
                {
                    Dictionary[key] = value;
                }
            }
        }

        public void Clear()
        {
            KeyList.Clear();
            Dictionary.Clear();
        }

        public virtual void Add(TKey key, TValue value)
        {
            KeyList.Add(key);
            Dictionary.Add(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public TKey GetKey(int index)
        {
            return KeyList[index];
        }

        public bool Remove(TKey key)
        {
            return KeyList.Remove(key) && Dictionary.Remove(key);
        }
    }
}