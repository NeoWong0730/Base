using UnityEngine;

namespace Framework
{
    public static class TimelineManager
    {
        public const string rootName = "Root_Timeline";
        public static Transform root { get; private set; }

        public static void Init()
        {
            if (root == null)
            {
                root = new GameObject(rootName)?.transform;
                GameObject.DontDestroyOnLoad(root);
            }
        }
    }
}
