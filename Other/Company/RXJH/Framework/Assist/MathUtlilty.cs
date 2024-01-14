using System.Collections.Generic;
using UnityEngine;

namespace Lib.Core
{
    public static class MathUtlilty
    {        
        public static Vector3 GetIntersectWithLineAndPlane(Vector3 startPos, Vector3 _dir, Vector3 pointInPlane, Vector3 planeNormal)
        {
            float d = Vector3.Dot(pointInPlane, planeNormal);
            float magnitude = (d - Vector3.Dot(startPos, planeNormal)) / (Vector3.Dot(_dir, planeNormal));
            Vector3 _pos = startPos + _dir * magnitude;
            return _pos;
        }

        public static float Distance(Transform trans1, Transform trans2)
        {
            if (trans1 == null || trans2 == null)
                return float.MaxValue;

            return Vector3.Distance(trans1.position, trans2.position);
        }

        public static float SafeDistance(Transform trans1, Transform trans2)
        {
            return Vector3.Distance(trans1.position, trans2.position);
        }

        public static bool SafeDistanceLess(Transform trans1, Transform trans2, float dis)
        {
            return Vector3.SqrMagnitude(trans1.position - trans2.position) < dis * dis;
        }

        public static bool SafeDistanceLessEqual(Transform trans1, Transform trans2, float dis)
        {
            return Vector3.SqrMagnitude(trans1.position - trans2.position) <= dis * dis;
        }

        public static bool SafeDistanceEqual(Transform trans1, Transform trans2, float dis)
        {
            return Vector3.SqrMagnitude(trans1.position - trans2.position) == dis * dis;
        }

        public static bool SafeDistanceLess(Vector3 pos1, Vector3 pos2, float dis)
        {
            return Vector3.SqrMagnitude(pos1 - pos2) < dis * dis;
        }

        public static bool SafeDistanceLessEqual(Vector3 pos1, Vector3 pos2, float dis)
        {
            return Vector3.SqrMagnitude(pos1 - pos2) <= dis * dis;
        }

        public static bool SafeDistanceEqual(Vector3 pos1, Vector3 pos2, float dis)
        {
            return Vector3.SqrMagnitude(pos1 - pos2) == dis * dis;
        }
    }
}

