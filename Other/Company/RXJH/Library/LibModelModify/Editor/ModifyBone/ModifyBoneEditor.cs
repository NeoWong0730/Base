using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModifyBone))]
public class ModifyBoneEditor : Editor
{
    Transform mSelected = null;
    
    private void DrawModifyBones(Transform root)
    {
        int childCount = root.childCount;
        if (root.childCount > 0)
        {
            for (int i = 0; i < childCount; ++i)
            {
                DrawModifyBones(root.GetChild(i));
            }
        }
        else
        {
            if (Handles.Button(root.position, Camera.current.transform.rotation, 0.005f, 0.005f, Handles.RectangleHandleCap))
            {
                mSelected = root;
                Selection.activeGameObject = root.gameObject;
                Event.current.Use();
            }
        }
    }

    private void OnSceneGUI()
    {        
        ModifyBone meshControl = target as ModifyBone;
        Handles.color = Color.blue;
        if (meshControl.drawGizmos && meshControl.mControlBoneRoot != null)
        {
            DrawModifyBones(meshControl.mControlBoneRoot);

            if (mSelected != null)
            {
                if (mSelected.gameObject != Selection.activeGameObject)
                {
                    mSelected = null;
                    return;
                }

                Vector3 localPosition = mSelected.localPosition;
                Vector3 localEulerAngles = UnityEditor.TransformUtils.GetInspectorRotation(mSelected);
                Vector3 localScale = mSelected.localScale;

                Handles.BeginGUI();
                GUILayout.BeginArea(new Rect(64, 0, 400, 200));
                EditorGUILayout.LabelField(mSelected.name);
                localPosition = EditorGUILayout.Vector3Field("Position", localPosition);
                localEulerAngles = EditorGUILayout.Vector3Field("Rotation", localEulerAngles);
                localScale = EditorGUILayout.Vector3Field("Scale", localScale);
                GUILayout.EndArea();
                Handles.EndGUI();

                if (mSelected.localPosition != localPosition || UnityEditor.TransformUtils.GetInspectorRotation(mSelected) != localEulerAngles || mSelected.localScale != localScale)
                {
                    Undo.RecordObject(mSelected, "transform");
                    mSelected.localPosition = localPosition;
                    UnityEditor.TransformUtils.SetInspectorRotation(mSelected, localEulerAngles);
                    mSelected.localScale = localScale;
                }
            }
        }
    }

    public List<Transform> CollectControlBones()
    {
        ModifyBone modifyBone = target as ModifyBone;
        if (modifyBone.mControlBoneRoot == null)
        {
            if (modifyBone.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer skinnedMeshRenderer))
            {
                if (skinnedMeshRenderer.rootBone != null)
                {
                    modifyBone.mControlBoneRoot = skinnedMeshRenderer.rootBone.Find("Dummy_face");
                }
            }

            if (modifyBone.mControlBoneRoot == null)
            {
                modifyBone.mControlBoneRoot = modifyBone.transform;
            }
        }

        Transform boneRoot = modifyBone.mControlBoneRoot;
        if (boneRoot == null)
            return null;

        Transform[] transforms = boneRoot.GetComponentsInChildren<Transform>();
        List<Transform> ts = new List<Transform>(transforms.Length);
        foreach (var t in transforms)
        {
            if (t.childCount <= 0)
                ts.Add(t);
        }

        return ts;
    }

    public void RecodeBonesValue(ModifyBoneRecordAsset modifyBoneAsset, Transform root, List<Transform> bones)
    {
        modifyBoneAsset.mRootBone = root.name;
        modifyBoneAsset.mModifyBoneNames = new string[bones.Count];
        modifyBoneAsset.mBoneLPositions = new Vector3[bones.Count];
        modifyBoneAsset.mBoneLEulerAngles = new Vector3[bones.Count];
        modifyBoneAsset.mBoneLScales = new Vector3[bones.Count];

        for (int i = 0; i < bones.Count; ++i)
        {
            modifyBoneAsset.mModifyBoneNames[i] = bones[i].name;
            modifyBoneAsset.mBoneLPositions[i] = bones[i].localPosition;
            modifyBoneAsset.mBoneLEulerAngles[i] = bones[i].localEulerAngles;
            modifyBoneAsset.mBoneLScales[i] = bones[i].localScale;
        }

        EditorUtility.SetDirty(modifyBoneAsset);
        AssetDatabase.SaveAssetIfDirty(modifyBoneAsset);
    }

    private void GenerationCSCode(string path)
    {
        ModifyBone meshControl = target as ModifyBone;

        string className = System.IO.Path.GetFileNameWithoutExtension(path);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("namespace Logic");
        sb.AppendLine("{");

        sb.AppendLine($"\tpublic enum {className}");
        sb.AppendLine("\t{");

        sb.AppendLine($"\t\tInvalid = -1,");

        for (int i = 0; i < meshControl.mModifyBoneControlGroups.Length; ++i)
        {
            sb.AppendLine($"\t\t{meshControl.mModifyBoneControlGroups[i].sName} = {i},");
        }

        sb.AppendLine($"\t\tCount = {meshControl.mModifyBoneControlGroups.Length}");

        sb.AppendLine("\t}");

        sb.AppendLine("}");

        System.IO.File.WriteAllText(path, sb.ToString());
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ModifyBone modifyBone = (target as ModifyBone);

        if(modifyBone.mModifyBoneAsset == null)
        {
            if (GUILayout.Button("Export Modify Bone Record"))
            {
                ModifyBoneRecordAsset modifyBoneAsset = ScriptableObject.CreateInstance<ModifyBoneRecordAsset>();
                List<Transform> bones = CollectControlBones();

                if (bones != null)
                {
                    RecodeBonesValue(modifyBoneAsset, modifyBone.mControlBoneRoot, bones);
                }

                modifyBone.mModifyBoneAsset = modifyBoneAsset;

                string path = EditorUtility.SaveFilePanel("Save", Application.dataPath + "Art/Human/BaseModel/Famale", "famale_bone_record", "asset");

                path = path.Replace(Application.dataPath, "Assets/");

                AssetDatabase.CreateAsset(modifyBoneAsset, path);

                EditorUtility.SetDirty(target);
            }
        }
        
        if (GUILayout.Button("Gen Enum"))
        {
            string path = EditorUtility.SaveFilePanel("Save", Application.dataPath + "Scripts/Logic/Level/LevelPlay/Data", "EFaceModifyType", "cs");
            if (string.IsNullOrWhiteSpace(path))
                return;

            GenerationCSCode(path);
        }
        /*
        if (GUILayout.Button("Export Config"))
        {
            string path = EditorUtility.SaveFilePanel("Save", Application.dataPath + "Scripts/Logic/Level/LevelPlay/Data", "EFaceModifyType", "cs");
            if (string.IsNullOrWhiteSpace(path))
                return;

            GenerationCSCode(path);
        }
        */
    }
}
