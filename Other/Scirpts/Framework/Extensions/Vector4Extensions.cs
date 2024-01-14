using UnityEngine;

namespace Framework
{
    public static class Vector4Extensions
    {
        public static float GetX(this ref Vector4 vector)
        {
            return vector.x;
        }

        public static float GetY(this ref Vector4 vector)
        {
            return vector.y;
        }

        public static float GetZ(this ref Vector4 vector)
        {
            return vector.z;
        }

        public static float GetW(this ref Vector4 vector)
        {
            return vector.w;
        }

        public static void SetX(this ref Vector4 vector, float x)
        {
            vector.x = x;
        }

        public static void SetY(this ref Vector4 vector, float y)
        {
            vector.y = y;
        }

        public static void SetZ(this ref Vector4 vector, float z)
        {
            vector.z = z;
        }
        public static void SetW(this ref Vector4 vector, float w)
        {
            vector.w = w;
        }
    }
}