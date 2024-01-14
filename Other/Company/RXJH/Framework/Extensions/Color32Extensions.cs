using UnityEngine;

namespace Framework
{
    public static class Color32Extensions
    {
        public static uint ToUInt32(this Color32 color)
        {
            unsafe
            {
                return *(uint*)&color;
            }
        }

        public static Color32 FromUInt32(uint v)
        {
            unsafe
            {
                return *(Color32*)&v;
            }
        }
    }
}