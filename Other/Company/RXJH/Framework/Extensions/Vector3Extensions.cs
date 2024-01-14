using UnityEngine;

namespace Framework
{
    public static class Vector3Extensions
    {
        public static float GetX(this ref Vector3 vector)
        {
            return vector.x;
        }

        public static float GetY(this ref Vector3 vector)
        {
            return vector.y;
        }

        public static float GetZ(this ref Vector3 vector)
        {
            return vector.z;
        }

        public static void SetX(this ref Vector3 vector, float x)
        {
            vector.x = x;
        }

        public static void SetY(this ref Vector3 vector, float y)
        {
            vector.y = y;
        }

        public static void SetZ(this ref Vector3 vector, float z)
        {
            vector.z = z;
        }
    }
}