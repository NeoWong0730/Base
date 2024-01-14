using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class DetailAsset : ScriptableObject
{
    public Material mMaterial;
    public float2 vTextureSize;
    public bool useColor;
    public int AreaID;
}