using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PreFrameData : MonoBehaviour
{
    public float fWindSpeed = 0;

    private void OnBeginFrameRendering(ScriptableRenderContext arg1, Camera[] arg2)
    {
        float power = Mathf.Sin(Time.time * fWindSpeed);
        Shader.SetGlobalFloat("_WindPower", power);
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginFrameRendering += OnBeginFrameRendering;
    }   

    private void OnDisable()
    {
        RenderPipelineManager.beginFrameRendering -= OnBeginFrameRendering;
    }
}
