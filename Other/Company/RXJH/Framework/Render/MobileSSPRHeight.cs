using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileSSPRHeight : MonoBehaviour
{
    public float fHeight = 0f;
    private void OnEnable()
    {
        RenderExtensionSetting.fHorizontalReflectionPlaneHeightWS = fHeight;
        ++RenderExtensionSetting.bUsageSSPR;
    }

    private void OnDisable()
    {
        --RenderExtensionSetting.bUsageSSPR;
    }
}
