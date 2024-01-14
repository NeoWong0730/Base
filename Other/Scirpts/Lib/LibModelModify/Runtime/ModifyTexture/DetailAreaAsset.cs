using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct DetailArea
{
    //x = 最小值 y = 中间值 z = 最大值
    public float3 xRange;
    public float3 yRange;
    public float3 fScaleRange;
    public float3 fRotateRange;
    public bool2 vMirror;
}

[CreateAssetMenu(fileName = "DetailAreaAsset.asset", menuName = "ScriptableObjects/DetailAreaAsset", order = 1)]
public class DetailAreaAsset : ScriptableObject
{
    public DetailArea[] mDetailAreas;
}
