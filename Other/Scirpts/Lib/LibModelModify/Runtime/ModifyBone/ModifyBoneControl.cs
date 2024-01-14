using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EModifyBoneControlType
{
    ScaleX,
    ScaleY,
    ScaleZ,
    X,
    Y,
    Z
}

[System.Serializable]
public class ModifyBoneControl
{
    public EModifyBoneControlType eModifyBoneControlType;
    public Transform mBone;
    public float fMin;
    public float fMid;
    public float fMax;
    public bool useCurve;
    public AnimationCurve mCurve;
}
