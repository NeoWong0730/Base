using System;
using System.Collections.Generic;

namespace Framework
{
    public static class ListExtensions
    {
        public static void AddOnce<T>(this IList<T> ts, T item)
        {
            if (ts.Contains(item))
                return;
            ts.Add(item);
        }

        public static void TryRemove<T>(this IList<T> ts, T item)
        {
            if (!ts.Contains(item))
                return;
            ts.Remove(item);
        }

        public static bool EqualList<T>(this IList<T> sourceCollection, IList<T> targetCollection, Func<T, T, bool> carpare)
        {
            if (sourceCollection == null || targetCollection == null)
            {
                return false;
            }
            if (object.ReferenceEquals(sourceCollection, targetCollection))
            {
                return true;
            }
            if (sourceCollection.Count != targetCollection.Count)
            {
                return false;
            }
            for (int i = sourceCollection.Count - 1; i >= 0; --i)
            {
                for (int j = targetCollection.Count - 1; j >= 0; --j)
                {
                    if (carpare(sourceCollection[i], targetCollection[j]))
                    {
                        targetCollection.RemoveAt(j);
                        sourceCollection.RemoveAt(i);
                        break;
                    }
                }
            }
            if (targetCollection.Count == 0)
            {
                return true;
            }
            return false;
        }
    }
}