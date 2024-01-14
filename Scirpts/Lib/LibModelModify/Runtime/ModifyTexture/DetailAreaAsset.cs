using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct DetailArea
{
    //x = ��Сֵ y = �м�ֵ z = ���ֵ
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
