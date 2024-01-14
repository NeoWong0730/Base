using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SkeletonModelController))]
public class ModelAutoLoad : MonoBehaviour
{
    public HumanPartConfig mHumanPartConfig;

    [HideInInspector] public int nSkeletonSelected = 0;
    [HideInInspector] public int nClothSelected = 0;
    [HideInInspector] public int nHairSelected = 0;
    [HideInInspector] public int nFaceSelected = 0;

    public Material mTestOverrideMaterial;

    // Start is called before the first frame update
    void Start()
    {
        Change();
    }

    public void Change()
    {
        SkeletonModelController model = GetComponent<SkeletonModelController>();

        if (!mHumanPartConfig)
            return;

        if (nSkeletonSelected >= 0 && mHumanPartConfig.mSkeletons != null && mHumanPartConfig.mCloths.Length > nSkeletonSelected)
        {
            model.LoadSkeleton(mHumanPartConfig.mSkeletons[nSkeletonSelected]);
        }

        if (nClothSelected >= 0 && mHumanPartConfig.mCloths != null && mHumanPartConfig.mCloths.Length > nClothSelected)
        {
            model.SetPart(0, mHumanPartConfig.mCloths[nClothSelected]);
        }

        if (nHairSelected >= 0 && mHumanPartConfig.mHairs != null && mHumanPartConfig.mHairs.Length > nHairSelected)
        {
            model.SetPart(1, mHumanPartConfig.mHairs[nHairSelected]);
        }

        if (nFaceSelected >= 0 && mHumanPartConfig.mFaces != null && mHumanPartConfig.mFaces.Length > nFaceSelected)
        {
            model.SetPart(2, mHumanPartConfig.mFaces[nFaceSelected]);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ModelAutoLoad))]
public class HumanModelEditor : Editor
{    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ModelAutoLoad modelAutoLoad = target as ModelAutoLoad;
        if (!modelAutoLoad.mHumanPartConfig)
            return;

        modelAutoLoad.nSkeletonSelected = modelAutoLoad.mHumanPartConfig.mSkeletons == null
            ? 0 : EditorGUILayout.Popup("Skeleton", modelAutoLoad.nSkeletonSelected, modelAutoLoad.mHumanPartConfig.mSkeletons);

        modelAutoLoad.nClothSelected = modelAutoLoad.mHumanPartConfig.mCloths == null
            ? 0 : EditorGUILayout.Popup("Cloth", modelAutoLoad.nClothSelected, modelAutoLoad.mHumanPartConfig.mCloths);

        modelAutoLoad.nHairSelected = modelAutoLoad.mHumanPartConfig.mHairs == null
            ? 0 : EditorGUILayout.Popup("Hair", modelAutoLoad.nHairSelected, modelAutoLoad.mHumanPartConfig.mHairs);

        modelAutoLoad.nFaceSelected = modelAutoLoad.mHumanPartConfig.mFaces == null
            ? 0 : EditorGUILayout.Popup("Face", modelAutoLoad.nFaceSelected, modelAutoLoad.mHumanPartConfig.mFaces);

        if (GUILayout.Button("Change"))
        {
            modelAutoLoad.Change();
        }

        if (GUILayout.Button("Override Material"))
        {
            modelAutoLoad.GetComponent<SkeletonModelController>().SetOverrideMaterial(modelAutoLoad.mTestOverrideMaterial, false);
        }

        if (GUILayout.Button("Cancel Override Material"))
        {
            modelAutoLoad.GetComponent<SkeletonModelController>().SetOverrideMaterial(null);
        }
    }
}

#endif
