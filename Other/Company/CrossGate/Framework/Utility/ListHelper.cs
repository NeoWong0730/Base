using System;
using System.Collections.Generic;
using System.Linq;

public static class ListHelper {
    // 数组的复制
    public static bool CopyTo<T>(this IList<T> from, ref List<T> to) {
        if (from != null) {
            if (to == null) {
                to = new List<T>(from.Count);
            }
            int fromCount = from.Count;
            int toCount = to.Count;

            int i = 0;
            if (fromCount >= toCount) {
                for (; i < toCount; ++i) {
                    to[i] = from[i];
                }
                for (; i < fromCount; ++i) {
                    to.Add(from[i]);
                }
            }
            else {
                for (; i < fromCount; ++i) {
                    to[i] = from[i];
                }

                to.RemoveRange(fromCount, toCount - fromCount);
            }

            return true;
        }
        return false;
    }

    public static bool CopyKeyTo<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> from, ref List<TKey> to) {
        if (from != null) {
            if (to == null) {
                to = new List<TKey>(from.Count);
            }
            int fromCount = from.Count;
            int toCount = to.Count;

            int i = 0;
            if (fromCount >= toCount) {
                foreach (var kvp in from) {
                    if (i < toCount) {
                        to[i++] = kvp.Key;
                    }
                    else {
                        to.Add(kvp.Key);
                    }
                }
            }
            else {
                foreach (var kvp in from) {
                    to[i++] = kvp.Key;
                }

                to.RemoveRange(fromCount, toCount - fromCount);
            }

            return true;
        }
        return false;
    }

    public static bool CopyValueTo<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> from, ref List<TValue> to) {
        if (from != null) {
            if (to == null) {
                to = new List<TValue>(from.Count);
            }
            int fromCount = from.Count;
            int toCount = to.Count;

            int i = 0;
            if (fromCount >= toCount) {
                foreach (var kvp in from) {
                    if (i < toCount) {
                        to[i++] = kvp.Value;
                    }
                    else {
                        to.Add(kvp.Value);
                    }
                }
            }
            else {
                foreach (var kvp in from) {
                    to[i++] = kvp.Value;
                }

                to.RemoveRange(fromCount, toCount - fromCount);
            }

            return true;
        }
        return false;
    }

    public static bool BuildList(ref List<int> to, int fromIndex, int endIndex) {
        if (to != null) {
            int fromCount = endIndex - fromIndex + 1;
            if (to == null) {
                to = new List<int>(fromCount);
            }
            int toCount = to.Count;

            int i = 0;
            if (fromCount >= toCount) {
                for (; i < toCount; ++i) {
                    to[i] = fromIndex++;
                }
                for (i = fromIndex; i <= endIndex; ++i) {
                    to.Add(i);
                }
            }
            else {
                for (; i < fromCount; ++i) {
                    to[i] = fromIndex++;
                }

                to.RemoveRange(fromCount, toCount - fromCount);
            }

            return true;
        }
        return false;
    }

    public static void Print<T>(this IList<T> list) {
        if (list == null) {
            return;
        }

        int length = list.Count;
        UnityEngine.Debug.LogFormat("Length: {0} Elems: {1}", length, list);
        for (int i = 0; i < length; ++i) {
            UnityEngine.Debug.LogFormat(" " + list[i]);
        }
    }

    // list中值为target的个数
    public static int Count<T>(this List<T> list, T target) {
        int count = 0;
        for (int i = 0, length = list.Count; i < length; ++i) {
            EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
            if (EqualityComparer<T>.Default.Equals(list[i], target)) {
                ++count;
            }
        }

        return count;
    }
    
    // 插入排序，稳定排序
    public static void SortStable(IList<uint> list, Func<uint, uint, bool> compare)
    {
        int j;
        for (int i = 1; i < list.Count; ++i)
        {
            var curr = list[i];

            j = i - 1;
            for (; j >= 0 && compare(curr, list[j]); --j)
                list[j + 1] = list[j];

            list[j + 1] = curr;
        }
    }
}