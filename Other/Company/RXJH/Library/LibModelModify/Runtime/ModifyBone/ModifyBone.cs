using System.Collections.Generic;
using UnityEngine;

public class ModifyBone : MonoBehaviour
{
#if UNITY_EDITOR
    public bool drawGizmos = true;
    public Transform mControlBoneRoot;
    public ModifyBoneRecordAsset mModifyBoneAsset;    
#endif

    public ModifyBoneControlGroup[] mModifyBoneControlGroups;

    private int DrawBones(Transform root, int depth, int maxDepth)
    {
        int childCount = root.childCount;
        if (childCount > 0)
        {
            Gizmos.color = Color.red;// Color.HSVToRGB((float)depth / maxDepth, 1, 1);            
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawSphere(root.position, Mathf.Lerp(0.001f, 0.0005f, depth / maxDepth));

        ++depth;
        int realMaxDepth = depth;
        
        if (childCount > 0)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);

                if(child.childCount > 0)
                {
                    Gizmos.color = Color.red;//Color.HSVToRGB((float)depth / maxDepth, 0.5f, 1);
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                
                Gizmos.DrawLine(root.position, child.position);

                int d = DrawBones(child, depth, maxDepth);
                if(d > realMaxDepth)
                {
                    realMaxDepth = d;
                }
            }
        }

        return realMaxDepth;
    }

    int nBoneMaxDepth = 10;
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (drawGizmos && mControlBoneRoot)
        {
            nBoneMaxDepth = DrawBones(mControlBoneRoot, 0, nBoneMaxDepth);
        }
    }
#endif
    private void OnValidate()
    {
        UpdateBones();
    }

    public void UpdateBones()
    {
        if (mModifyBoneControlGroups == null)
            return;

        for (int i = 0; i < mModifyBoneControlGroups.Length; ++i)
        {
            ModifyBoneControlGroup controlGroup = mModifyBoneControlGroups[i];

            if (controlGroup.mModifyBoneControls == null)
                continue;            

            foreach (var control in controlGroup.mModifyBoneControls)
            {
                Transform bone = control.mBone;
                if (!bone)
                {
                    continue;
                }

                float value = controlGroup.fValue;

                if (control.useCurve && control.mCurve != null)
                {
                    value = control.mCurve.Evaluate(value);
                }

#if UNITY_EDITOR
                if (mModifyBoneAsset)
                {
                    control.fMid = GetSourceValue(mModifyBoneAsset, control.eModifyBoneControlType, bone.name);
                }
#endif
                if (value < 0.5f)
                {
                    value = Mathf.Lerp(control.fMin, control.fMid, value * 2);
                }
                else
                {
                    value = Mathf.Lerp(control.fMid, control.fMax, value * 2 - 1);
                }
                
                switch (control.eModifyBoneControlType)
                {
                    case EModifyBoneControlType.ScaleX:
                        bone.localScale = new Vector3(value, bone.localScale.y, bone.localScale.z);
                        break;
                    case EModifyBoneControlType.ScaleY:
                        bone.localScale = new Vector3(bone.localScale.x, value, bone.localScale.z);
                        break;
                    case EModifyBoneControlType.ScaleZ:
                        bone.localScale = new Vector3(bone.localScale.x, bone.localScale.y, value);
                        break;
                    case EModifyBoneControlType.X:
                        bone.localPosition = new Vector3(value, bone.localPosition.y, bone.localPosition.z);
                        break;
                    case EModifyBoneControlType.Y:
                        bone.localPosition = new Vector3(bone.localPosition.x, value, bone.localPosition.z);
                        break;
                    case EModifyBoneControlType.Z:
                        bone.localPosition = new Vector3(bone.localPosition.x, bone.localPosition.y, value);
                        break;
                }
            }
        }
    }

#if UNITY_EDITOR
    public float GetSourceValue(ModifyBoneRecordAsset modifyBoneRecordAsset, EModifyBoneControlType controlType, string boneName)
    {
        int index = UnityEditor.ArrayUtility.IndexOf(modifyBoneRecordAsset.mModifyBoneNames, boneName);
        if (index >= 0)
        {
            switch (controlType)
            {
                case EModifyBoneControlType.ScaleX:
                    return modifyBoneRecordAsset.mBoneLScales[index].x;
                case EModifyBoneControlType.ScaleY:
                    return modifyBoneRecordAsset.mBoneLScales[index].y;
                case EModifyBoneControlType.ScaleZ:
                    return modifyBoneRecordAsset.mBoneLScales[index].z;
                case EModifyBoneControlType.X:
                    return modifyBoneRecordAsset.mBoneLPositions[index].x;
                case EModifyBoneControlType.Y:
                    return modifyBoneRecordAsset.mBoneLPositions[index].y;
                case EModifyBoneControlType.Z:
                    return modifyBoneRecordAsset.mBoneLPositions[index].z;
                default:
                    break;
            }
        }
        return 0;
    }
#endif
}