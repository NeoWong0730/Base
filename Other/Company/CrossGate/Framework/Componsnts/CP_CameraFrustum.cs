using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 相机视锥体
[RequireComponent(typeof(Camera))]
public class CP_CameraFrustum : MonoBehaviour
{
#if UNITY_EDITOR
    public new Camera camera = null;

    private void Awake()
    {
        camera = camera ?? gameObject.GetComponent<Camera>() ?? Camera.main;
    }

    private void OnDrawGizmos()
    {
        if (camera != null)
        {
            UnityEditor.CameraEditorUtils.DrawFrustumGizmo(camera);
        }
    }
#endif
}