using System;
using static Unity.Mathematics.math;
using UnityEngine;

namespace Unity.Mathematics
{
    public static class MatrixUtil
    {
        public static void CopyFrom(this float4x4 target, Matrix4x4 matrix4X4)
        {
            target.c0 = new float4(matrix4X4.m00, matrix4X4.m10, matrix4X4.m20, matrix4X4.m30);
            target.c1 = new float4(matrix4X4.m01, matrix4X4.m11, matrix4X4.m21, matrix4X4.m31);
            target.c2 = new float4(matrix4X4.m02, matrix4X4.m12, matrix4X4.m22, matrix4X4.m32);
            target.c3 = new float4(matrix4X4.m03, matrix4X4.m13, matrix4X4.m23, matrix4X4.m33);
        }

        public static float4x4 CreateFrom(Matrix4x4 matrix4X4)
        {
            float4x4 m = new float4x4(
                matrix4X4.m00, matrix4X4.m01, matrix4X4.m02, matrix4X4.m03,
                matrix4X4.m10, matrix4X4.m11, matrix4X4.m12, matrix4X4.m13,
                matrix4X4.m20, matrix4X4.m21, matrix4X4.m22, matrix4X4.m23,
                matrix4X4.m30, matrix4X4.m31, matrix4X4.m32, matrix4X4.m33
                );
            return m;
        }
    }
}