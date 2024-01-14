using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPartConfig : ScriptableObject
{
    [SerializeField]
    public string[] mSkeletons;
    [SerializeField]
    public string[] mFaces;
    [SerializeField]
    public string[] mCloths;
    [SerializeField]
    public string[] mHairs;
}