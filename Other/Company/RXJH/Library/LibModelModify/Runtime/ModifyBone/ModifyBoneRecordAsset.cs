using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyBoneRecordAsset : ScriptableObject
{
    public string mRootBone;
    public string[] mModifyBoneNames;
    public Vector3[] mBoneLPositions;
    public Vector3[] mBoneLEulerAngles;
    public Vector3[] mBoneLScales;
}