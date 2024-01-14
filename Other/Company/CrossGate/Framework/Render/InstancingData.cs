using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct DetailChunckData
{
    public int boundIndex;
    public int offset;
    public int count;
}

[System.Serializable]
public struct DetailLayerDate
{
    public DetailChunckData[] chunckDatas;
}

public class InstancingData : ScriptableObject
{    
    public DetailLayerDate[] layerDatas;
    public Matrix4x4[] matrices;
}

[System.Serializable]
public struct InstancingRenderData
{
    public Mesh mesh;
    public Material material;

    public float3 rotate;
    public float3 scale;
#if UNITY_EDITOR
    [Range(0, 1)]
    public float threshold;
    public int[] alphaMasks;
#endif

    [Header("x = minScale y = maxScale z = seed")]
    public float3 randomScale;

    [Header("风吹最大弯曲的角度")]
    [Range(0, 360)]
    public float windingMax;

    [Range(0, 360)]
    [Header("碰撞最大弯曲的角度")]
    public float collideWindingMax;
}
