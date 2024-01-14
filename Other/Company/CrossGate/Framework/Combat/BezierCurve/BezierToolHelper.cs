using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BezierToolHelper
{
    #region 二维贝塞尔曲线
    //二阶贝塞尔曲线
    //(1-t)^2P0 + 2(1-t)tP1 + t^2*P2
    public static Vector2 Calculate2BezierPoint_2D(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1f - t;

        return u * u * p0 + 2 * t * u * p1 + t * t * p2;
    }

    //三阶贝塞尔曲线
    //(1-t)^3P0 + 3(1-t)^2tP1 + 3(1-t)t^2P2 + t^3*P3
    public static Vector2 Calculate3BezierPoint_2D(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float u = 1f - t;
        float u2 = u * u;
        float t2 = t * t;

        return u2 * u * p0 + 3 * t * u2 * p1 + 3 * t2 * u * p2 + t2 * t * p3;
    }

    //四阶贝塞尔曲线
    //(1-t)^4P0 + 4(1-t)^3tP1 + 6(1-t)^2t^2P2 + 4(1-t)t^3P3 + t^4*P4
    public static Vector2 Calculate4BezierPoint_2D(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float u = 1f - t;
        float u2 = u * u;
        float t2 = t * t;

        return u2 * u2 * p0 + 4 * u2 * u * t * p1 + 6 * u2 * t2 * p2 + 4 * u * t2 * t * p3 + t2 * t2 * p4;
    }
    #endregion


    #region 三维贝塞尔曲线
    //二阶贝塞尔曲线
    public static Vector3 Calculate2BezierPoint_3D(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1f - t;

        return u * u * p0 + 2 * t * u * p1 + t * t * p2;
    }

    //三阶贝塞尔曲线
    public static Vector3 Calculate3BezierPoint_3D(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1f - t;
        float u2 = u * u;
        float t2 = t * t;

        return u2 * u * p0 + 3 * t * u2 * p1 + 3 * t2 * u * p2 + t2 * t * p3;
    }

    //四阶贝塞尔曲线
    public static Vector3 Calculate4BezierPoint_3D(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        float u = 1f - t;
        float u2 = u * u;
        float t2 = t * t;

        return u2 * u2 * p0 + 4 * u2 * u * t * p1 + 6 * u2 * t2 * p2 + 4 * u * t2 * t * p3 + t2 * t2 * p4;
    }
    #endregion
}
