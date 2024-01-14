using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// iPhone x 工具类
/// </summary>
namespace Framework
{
    public class SafeAreaUtils : MonoBehaviour
    {

#if UNITY_IOS
        [DllImport("_Internal")]
        private static extern void GetSafeArea(out float x, out float y, out float w, out float h);
#endif

        public static Rect Get()
        {
            float x, y, w, h;
#if UNITY_IOS && !UNITY_EDITOR
            GetSafeArea(out x, out y, out w, out h);
#else
            x = 0;
            y = 0;
            w = Screen.width;
            h = Screen.height;
#endif
            return new Rect(x, y, w, h);
        }
    }
}


