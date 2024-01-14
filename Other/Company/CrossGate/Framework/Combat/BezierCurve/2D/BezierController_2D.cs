#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierController_2D : MonoBehaviour
{
    public Transform t1;
    public Transform t2;
    public Transform t3;
    public Transform t4;
    public Transform t5;
    
    private void OnDrawGizmos()
    {
        int count = 30;
        float t = 1f / count;
        Vector3[] points = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            float f = t * i;
            points[i] = BezierToolHelper.Calculate4BezierPoint_2D(f, t1.position, t2.position, t3.position, t4.position, t5.position);
        }

        UnityEditor.Handles.DrawPolyLine(points);
    }
}
#endif
