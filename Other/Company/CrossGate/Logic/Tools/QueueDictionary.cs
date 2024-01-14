using System.Collections.Generic;

public class QueueDictionary<K, V>
{
    public readonly List<K> list = new List<K>();
    public readonly Dictionary<K, V> dictionary = new Dictionary<K, V>();

    public int Count { get { return list.Count; } }
    public K firstKey { get { return list[0]; } }
    public V FirstValue { get { return dictionary[list[0]]; } }
    public K LastKey { get { return list[Count - 1]; } }
    public V LastValue { get { return dictionary[list[Count - 1]]; } }
    public V this[K key] { get { return dictionary[key]; } }

    public void Enqueue(K k, V v)
    {
        list.Add(k);
        dictionary.Add(k, v);
    }
    public void Dequeue()
    {
        if (list.Count > 0)
        {
            K key = list[0];
            list.RemoveAt(0);
            dictionary.Remove(key);
        }
    }
    public void Remove(K key)
    {
        list.Remove(key);
        dictionary.Remove(key);
    }
    public bool ContainsKey(K key)
    {
        return dictionary.ContainsKey(key);
    }

    public K GetKeyByIndex(int index) { return list[index]; }
    public V GetValueByKey(K key) { return dictionary[key]; }
    public V GetValueByIndex(int index) { return dictionary[list[index]]; }

    public void Clear()
    {
        list.Clear();
        dictionary.Clear();
    }
}
