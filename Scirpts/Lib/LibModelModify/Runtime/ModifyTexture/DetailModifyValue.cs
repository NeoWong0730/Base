using System;
using UnityEngine;

[System.Serializable]
public struct DetailModifyValue : IEquatable<DetailModifyValue>
{
    [SerializeField] public UnityEngine.Color vColor;
    [SerializeField]
    [Range(0, 1)]
    public float fX;
    [SerializeField]
    [Range(0, 1)]
    public float fY;
    [SerializeField]
    [Range(0, 1)]
    public float fScale;
    [SerializeField]
    [Range(0, 1)]
    public float fRotate;    

    public bool Equals(DetailModifyValue other)
    {
        return vColor == other.vColor
            && fX == other.fX
            && fY == other.fY
            && fScale == other.fScale
            && fRotate == other.fRotate;            
    }
}