using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditorInternal;

[CustomEditor(typeof(ModifyTexture))]
public class ModifyTextureEditor : Editor
{
    SerializedProperty mDetailDatasProp;
    SerializedProperty mDetailModifyValuesProp;
    SerializedProperty mDetailAreaArrayProp;
    SerializedObject mDetailAreaAssetSO;
    ReorderableList mReorderableList;

    static readonly GUIContent mDetailAreaContent = new GUIContent("Area");

    private void OnEnable()
    {
        ModifyTexture modifyTexture = target as ModifyTexture;
        if (modifyTexture.detailAreaAsset != null)
        {
            mDetailAreaAssetSO = new SerializedObject(modifyTexture.detailAreaAsset);
            mDetailAreaArrayProp = mDetailAreaAssetSO.FindProperty("mDetailAreas");
        }
        else
        {
            mDetailAreaAssetSO = null;
            mDetailAreaArrayProp = null;
        }

        mDetailDatasProp = serializedObject.FindProperty("detailDatas");
        mDetailModifyValuesProp = serializedObject.FindProperty("detailModifyValues");

        mReorderableList = new ReorderableList(serializedObject, mDetailDatasProp, true, true, true, true);
        mReorderableList.drawElementCallback = onDrawElementCallback;
        mReorderableList.elementHeightCallback = onElementHeightCallback;
    }

    private float onElementHeightCallback(int index)
    {
        if (mReorderableList.IsSelected(index))
        {
            SerializedProperty detailAssetProp = mDetailDatasProp.GetArrayElementAtIndex(index);
            DetailAsset detailAsset = detailAssetProp.objectReferenceValue as DetailAsset;
            
            SerializedProperty detailAreaArray = mDetailAreaArrayProp != null && detailAsset != null ? mDetailAreaArrayProp.GetArrayElementAtIndex(detailAsset.AreaID) : null;
            if (detailAreaArray != null)
            {
                return EditorGUIUtility.singleLineHeight * 19f;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight * 13f;
            }
        }
        return EditorGUIUtility.singleLineHeight * 6;
    }

    private void onDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty detailDataElement = mReorderableList.serializedProperty.GetArrayElementAtIndex(index);

        float texSize = EditorGUIUtility.singleLineHeight * 4;

        Rect rect0 = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rect0, detailDataElement);

        DetailAsset detailAsset = detailDataElement.objectReferenceValue as DetailAsset;
        if (detailAsset)
        {
            rect0 = new Rect(rect.x + texSize, rect.y, rect.width - texSize, EditorGUIUtility.singleLineHeight);

            //rect0.y += EditorGUIUtility.singleLineHeight;
            //EditorGUI.ObjectField(rect0, detailAsset.mMaterial, typeof(Material), false);

            rect0.y += EditorGUIUtility.singleLineHeight * 1.5f;
            EditorGUI.LabelField(rect0, $"Size = ({detailAsset.vTextureSize.x}, {detailAsset.vTextureSize.y})");

            rect0.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect0, $"Use Color = {detailAsset.useColor}");

            rect0.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect0, $"Area ID = {detailAsset.AreaID}");

            Texture tex = null;
            if (detailAsset != null && detailAsset.mMaterial != null)
            {
                tex = detailAsset.mMaterial.mainTexture;
            }
            rect0 = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, texSize, texSize);
            EditorGUI.DrawTextureTransparent(rect0, tex);
        }

        rect0 = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 6, rect.width, EditorGUIUtility.singleLineHeight);

        if (mReorderableList.IsSelected(index))
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 6, rect.width, EditorGUIUtility.singleLineHeight * 5f), Color.black);

            SerializedProperty detailModifyValueElment = mDetailModifyValuesProp.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(rect0, detailModifyValueElment.FindPropertyRelative("vColor"), true);
            rect0.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect0, detailModifyValueElment.FindPropertyRelative("fX"), true);
            rect0.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect0, detailModifyValueElment.FindPropertyRelative("fY"), true);
            rect0.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect0, detailModifyValueElment.FindPropertyRelative("fScale"), true);
            rect0.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect0, detailModifyValueElment.FindPropertyRelative("fRotate"), true);
            rect0.y += EditorGUIUtility.singleLineHeight;
            if (GUI.Button(new Rect(rect0.x, rect0.y, rect.width * 0.25f, EditorGUIUtility.singleLineHeight), "°×É«"))
            {
                SerializedProperty colorProp = detailModifyValueElment.FindPropertyRelative("vColor");
                if (colorProp.colorValue != Color.white)
                {
                    colorProp.colorValue = Color.white;
                }
                else
                {
                    colorProp.colorValue = Color.clear;
                }
            }
            if (GUI.Button(new Rect(rect0.x + rect.width * 0.25f, rect0.y, rect.width * 0.25f, EditorGUIUtility.singleLineHeight), "×ó"))
            {
                detailModifyValueElment.FindPropertyRelative("fX").floatValue = 0;
                detailModifyValueElment.FindPropertyRelative("fY").floatValue = 0;
                detailModifyValueElment.FindPropertyRelative("fScale").floatValue = 0;
                detailModifyValueElment.FindPropertyRelative("fRotate").floatValue = 0;
            }
            if (GUI.Button(new Rect(rect0.x + rect.width * 0.5f, rect0.y, rect.width * 0.25f, EditorGUIUtility.singleLineHeight), "ÖÐ"))
            {
                detailModifyValueElment.FindPropertyRelative("fX").floatValue = 0.5f;
                detailModifyValueElment.FindPropertyRelative("fY").floatValue = 0.5f;
                detailModifyValueElment.FindPropertyRelative("fScale").floatValue = 0.5f;
                detailModifyValueElment.FindPropertyRelative("fRotate").floatValue = 0.5f;
            }
            if (GUI.Button(new Rect(rect0.x + rect.width * 0.75f, rect0.y, rect.width * 0.25f, EditorGUIUtility.singleLineHeight), "ÓÒ"))
            {
                detailModifyValueElment.FindPropertyRelative("fX").floatValue = 1.0f;
                detailModifyValueElment.FindPropertyRelative("fY").floatValue = 1.0f;
                detailModifyValueElment.FindPropertyRelative("fScale").floatValue = 1.0f;
                detailModifyValueElment.FindPropertyRelative("fRotate").floatValue = 1.0f;
            }

            rect0.y += EditorGUIUtility.singleLineHeight;
            if(mDetailAreaArrayProp != null && detailAsset != null)
            {
                SerializedProperty detailAreaProp = mDetailAreaArrayProp.GetArrayElementAtIndex(detailAsset.AreaID);
                if (detailAreaProp != null)
                {
                    EditorGUI.PropertyField(rect0, detailAreaProp, mDetailAreaContent, true);
                }
                else
                {

                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        ModifyTexture modifyTexture = target as ModifyTexture;

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            modifyTexture.SetDirty();
        }

        EditorGUI.BeginChangeCheck();
        modifyTexture.detailAreaAsset = EditorGUILayout.ObjectField(modifyTexture.detailAreaAsset, typeof(DetailAreaAsset), true) as DetailAreaAsset;
        if (EditorGUI.EndChangeCheck())
        {
            if (modifyTexture.detailAreaAsset != null)
            {
                mDetailAreaAssetSO = new SerializedObject(modifyTexture.detailAreaAsset);
                mDetailAreaArrayProp = mDetailAreaAssetSO.FindProperty("mDetailAreas");
            }
            else
            {
                mDetailAreaAssetSO = null;
                mDetailAreaArrayProp = null;
            }
            modifyTexture.SetDirty();
        }

        mReorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();

        int detailDatasCount = modifyTexture.detailDatas == null ? 0 : modifyTexture.detailDatas.Length;
        int detailModifyValuesCount = modifyTexture.detailModifyValues == null ? 0 : modifyTexture.detailModifyValues.Length;

        if (detailDatasCount != detailModifyValuesCount)
        {
            if (modifyTexture.detailModifyValues == null)
            {
                modifyTexture.detailModifyValues = new DetailModifyValue[detailDatasCount];
            }
            else
            {
                Array.Resize(ref modifyTexture.detailModifyValues, detailDatasCount);
            }
        }

        if (mDetailAreaAssetSO != null && mDetailAreaAssetSO.hasModifiedProperties)
        {
            mDetailAreaAssetSO.ApplyModifiedProperties();
            modifyTexture.SetDirty();
        }

        if (GUILayout.Button("Save"))
        {
            string path = EditorUtility.SaveFilePanel("Save", Application.dataPath + "Assets/Settings/FaceData", "FaceTextureData", "bin");
            if (string.IsNullOrWhiteSpace(path))
                return;

            SaveFaceData(path, modifyTexture);
        }

        if (GUILayout.Button("Load"))
        {
            string path = EditorUtility.OpenFilePanel("Load", Application.dataPath + "Assets/Settings/FaceData", "bin");
            if (string.IsNullOrWhiteSpace(path))
                return;

            LoadFaceData(path, modifyTexture);
        }

        //if (mDetailAreaArrayProp != null)
        //{            
        //    ListPropertyField(mDetailAreaArrayProp);
        //
        //    if (mDetailAreaAssetSO.hasModifiedProperties)
        //    {                
        //        mDetailAreaAssetSO.ApplyModifiedProperties();
        //        modifyTexture.SetDirty();
        //    }
        //}
    }

    private void ListPropertyField(SerializedProperty serializedProperty)
    {
        if (EditorGUILayout.PropertyField(serializedProperty, true))
        {
            ++EditorGUI.indentLevel;
            for (int i = 0; i < serializedProperty.arraySize; ++i)
            {
                var element = serializedProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(element);
            }
            --EditorGUI.indentLevel;
        }
    }

    private void SaveFaceData(string path, ModifyTexture modifyTexture)
    {
        if (modifyTexture.detailDatas == null)
            return;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < modifyTexture.detailDatas.Length; ++i)
        {
            string detailPath = AssetDatabase.GetAssetPath(modifyTexture.detailDatas[i]);
            sb.AppendLine(detailPath);
            DetailModifyValue detailModifyValue = modifyTexture.detailModifyValues[i];
            
            sb.AppendLine($"{ColorUtility.ToHtmlStringRGBA(detailModifyValue.vColor)}|{detailModifyValue.fX}|{detailModifyValue.fY}|{detailModifyValue.fRotate}|{detailModifyValue.fScale}");
        }
        System.IO.File.WriteAllText(path, sb.ToString());
    }

    private void LoadFaceData(string path, ModifyTexture modifyTexture)
    {
        System.IO.FileStream stream = System.IO.File.OpenRead(path);
        System.IO.StreamReader streamReader = new System.IO.StreamReader(stream);

        List<DetailAsset> detailAssets = new List<DetailAsset>();
        List<DetailModifyValue> detailModifyValues = new List<DetailModifyValue>();

        string line;
        while (!string.IsNullOrWhiteSpace(line = streamReader.ReadLine()))
        {
            detailAssets.Add(AssetDatabase.LoadAssetAtPath<DetailAsset>(line));
            DetailModifyValue detailModifyValue = new DetailModifyValue();
            line = streamReader.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] ss = line.Split('|');
                if(ss.Length == 5)
                {
                    ColorUtility.TryParseHtmlString(ss[0], out Color color);
                    detailModifyValue.vColor = color;
                    float.TryParse(ss[1], out float x);
                    detailModifyValue.fX = x;
                    float.TryParse(ss[2], out float y);
                    detailModifyValue.fY = y;
                    float.TryParse(ss[3], out float rotate);
                    detailModifyValue.fRotate = rotate;
                    float.TryParse(ss[4], out float scale);
                    detailModifyValue.fScale = scale;
                }
            }
            detailModifyValues.Add(detailModifyValue);
        }

        modifyTexture.detailDatas = detailAssets.ToArray();
        modifyTexture.detailModifyValues = detailModifyValues.ToArray();

        streamReader.Close();
        stream.Close();
    }
}
