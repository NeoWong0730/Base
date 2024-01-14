using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifyBoneControlGroup
{
    public string sName;
    [Range(0, 1)]
    public float fValue;
    public ModifyBoneControl[] mModifyBoneControls;
}
