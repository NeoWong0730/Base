using UnityEngine;

namespace Framework
{
    public static class Vector2Extensions
    {
        public static float GetX(this ref Vector2 vector)
        {
            return vector.x;
        }

        public static float GetY(this ref Vector2 vector)
        {
            return vector.y;
        }

        public static void SetX(this ref Vector2 vector, float x)
        {
            vector.x = x;
        }

        public static void SetY(this ref Vector2 vector, float y)
        {
            vector.y = y;
        }
    }
}