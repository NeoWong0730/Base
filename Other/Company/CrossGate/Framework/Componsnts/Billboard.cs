using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Billboard : MonoBehaviour
{
    private static int count = 0;    
    private static Quaternion quaternion;    

    private static void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {        
        quaternion = arg2.transform.rotation;        
    }

    public bool justY = false;

    private void OnEnable()
    {
        ++count;
        if (count == 1)
        {
            RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;            
        }
    }    

    private void OnDisable()
    {
        --count;
        if (count == 0)
        {
            RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;
        }
    }    

    private void OnWillRenderObject()
    {        
        if(justY)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.y = quaternion.eulerAngles.y;
            transform.eulerAngles = eulerAngles;
        }
        else
        {
            transform.rotation = quaternion;
        }        
    }    
}
