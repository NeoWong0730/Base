using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class GameObjectExtensions
    {
        [Obsolete("GetNeedComponent has been renamed (UnityUpgradable) -> GetOrAddComponent", true)]
        public static T GetNeedComponent<T>(this GameObject go) where T : Component
        {
            if (go == null)
                return null;

            T rlt = null;
            if (!go.TryGetComponent<T>(out rlt))
            {
                rlt = go.AddComponent<T>();
            }
            return rlt;
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go == null)
                return null;

            T rlt = null;
            if (!go.TryGetComponent<T>(out rlt))
            {
                rlt = go.AddComponent<T>();
            }
            return rlt;
        }

        public static void DestoryAllChildren(this GameObject go, List<string> dontDestorys = null, bool immediate = false)
        {
            List<GameObject> toDeleted = new List<GameObject>();
            int length = go.transform.childCount;
            for (int i = 0; i < length; i++)
            {
                var child = go.transform.GetChild(i);
                if (dontDestorys != null)
                {
                    if (!dontDestorys.Contains(child.name))
                    {
                        toDeleted.Add(child.gameObject);
                    }
                }
                else
                {
                    toDeleted.Add(child.gameObject);
                }
            }
            foreach (var g in toDeleted)
            {
                if (immediate)
                {
                    GameObject.DestroyImmediate(g);
                }
                else
                {
                    GameObject.Destroy(g);
                }
            }
        }

        public static GameObject FindChildByName(this GameObject parent, string name, bool logNotFound = true)
        {
            if (parent.transform.TryGetChildByName(name, out Transform rlt, logNotFound))
            {
                return rlt.gameObject;
            }
            else
            {
                return null;
            }
        }

        public static List<GameObject> FindChildernWithTag(this GameObject go, string tag, bool recursion = false)
        {
            List<GameObject> result = new List<GameObject>();
            if (!go)
            {
                return result;
            }

            int length = go.transform.childCount;
            for (int i = 0; i < length; i++)
            {
                var child = go.transform.GetChild(i);
                if (recursion)
                {
                    var cs = child.gameObject.FindChildernWithTag(tag, recursion);
                    result.AddRange(cs);
                }
            }

            if (go.tag == tag)
            {
                result.Add(go);
            }

            return result;
        }

        public static GameObject CloneChild(this GameObject prefab, Transform parent = null)
        {
            GameObject result = null;
            if (prefab != null)
            {
                result = CloneChild(prefab.transform, parent);
            }
            return result;
        }
        public static GameObject CloneChild(this Transform prefab, Transform parent = null)
        {
            GameObject result = null;
            if (prefab != null)
            {
                result = GameObject.Instantiate<GameObject>(prefab.gameObject, parent);
            }
            return result;
        }
    }
}