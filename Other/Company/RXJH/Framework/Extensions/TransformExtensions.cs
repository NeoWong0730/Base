using Lib.Core;
using UnityEngine;

namespace Framework
{
    public static class TransformExtensions
    {
        public static void Setlayer(this Transform trans, int layer, bool direct = false)
        {
            if (trans != null)
            {
                trans.gameObject.layer = layer;
                if (!direct)
                {
                    int count = trans.childCount;
                    for (int i = 0; i < count; ++i)
                    {
                        Setlayer(trans.GetChild(i), layer);
                    }
                }
            }
        }

        public static void SetTag(this Transform trans, string tag, bool direct = false)
        {
            if (trans != null)
            {
                trans.gameObject.tag = tag;
                if (!direct)
                {
                    int count = trans.childCount;
                    for (int i = 0; i < count; ++i)
                    {
                        SetTag(trans.GetChild(i), tag);
                    }
                }
            }
        }
        public static bool TryGetChildByName(this Transform parent, string name, out Transform result, bool logNotFound = true)
        {
            result = parent.Find(name);
            if (result != null)
            {
                return true;
            }
            else
            {
                int count = parent.childCount;
                for (int i = 0; i < count; ++i)
                {
                    if (parent.GetChild(i).TryGetChildByName(name, out result, false))
                    {
                        return true;
                    }
                }
            }

            if (logNotFound)
            {
                DebugUtil.LogError(parent.name + " can't find " + name);
            }

            return false;
        }
    }
}