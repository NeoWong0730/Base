using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using UnityEditor.Animations;
using Table;
using System.Reflection;
using System.Collections.Generic;
using Framework;
using Logic;

public class AnimationTools : MonoBehaviour
{
    [MenuItem("Assets/AnimationTools/拷贝单个文件")]
    static void CopyClipFile()
    {
        var arr = Selection.assetGUIDs;
        string dp = AssetDatabase.GUIDToAssetPath(arr[0]);

        FileInfo fileInfo = new FileInfo(dp);
        try
        {
            string assetPath = fileInfo.FullName.Substring(Application.dataPath.Length - 6);
            string[] filterStrs = assetPath.Split('_');
            if (filterStrs.Length >= 2)
            {
                if (filterStrs[filterStrs.Length - 1].StartsWith("Mesh") || filterStrs[filterStrs.Length - 1].StartsWith("mesh"))
                {
                    return;
                }
            }
            //string tempPath = assetPath.Substring(21);

            AnimationClip asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
            RemoveAnimationCurve(asset);
            CompressAnimationClip(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (asset != null)
            {
                AnimationClip newClip = new AnimationClip();
                EditorUtility.CopySerialized(asset, newClip);

                string targetPath = "Assets/Art/AnimationClip/" + fileInfo.Name;
                targetPath = targetPath.Replace(@"\Action", "");
                targetPath = targetPath.Replace(@"\Clothes", "");
                targetPath = targetPath.Replace(".FBX", ".anim");

                //string[] strs = tempPath.Split('\\');
                //string dictStr = targetPath.Substring(7, targetPath.Length - strs[strs.Length - 1].Length - 9);

                //string finalDictPath = Application.dataPath + "/" + dictStr;
                //if (!Directory.Exists(finalDictPath))
                //{
                //    Directory.CreateDirectory(finalDictPath);
                //}
                AssetDatabase.CreateAsset(newClip, targetPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("fileInfo: " + fileInfo.FullName);
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/AnimationTools/拷贝整个目录")]
    static void CopyClipDir()
    {
        var arr = Selection.assetGUIDs;
        string dp = AssetDatabase.GUIDToAssetPath(arr[0]);
        DirectoryInfo directoryInfo = new DirectoryInfo(dp);
        FileInfo[] fileInfos = directoryInfo.GetFiles("*.FBX", SearchOption.AllDirectories);

        foreach (FileInfo fileInfo in fileInfos)
        {
            try
            {
                string assetPath = fileInfo.FullName.Substring(Application.dataPath.Length - 6);
                string[] filterStrs = assetPath.Split('_');
                if (filterStrs.Length >= 2)
                {
                    if (filterStrs[filterStrs.Length - 1].StartsWith("Mesh") || filterStrs[filterStrs.Length - 1].StartsWith("mesh"))
                    {
                        continue;
                    }
                }
                //string tempPath = assetPath.Substring(21);

                AnimationClip asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                RemoveAnimationCurve(asset);
                CompressAnimationClip(asset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                if (asset != null)
                {
                    AnimationClip newClip = new AnimationClip();
                    EditorUtility.CopySerialized(asset, newClip);

                    string targetPath = "Assets/Art/AnimationClip/" + fileInfo.Name;
                    targetPath = targetPath.Replace(@"\Action", "");
                    targetPath = targetPath.Replace(@"\Clothes", "");
                    targetPath = targetPath.Replace(".FBX", ".anim");

                    //string[] strs = tempPath.Split('\\');
                    //string dictStr = targetPath.Substring(7, targetPath.Length - strs[strs.Length - 1].Length - 9);

                    //string finalDictPath = Application.dataPath + "/" + dictStr;
                    //if (!Directory.Exists(finalDictPath))
                    //{
                    //    Directory.CreateDirectory(finalDictPath);
                    //}

                    AssetDatabase.CreateAsset(newClip, targetPath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("fileInfo: " + fileInfo.FullName);
            }
        }

        AssetDatabase.Refresh();

    }

    static string _Name_Scale = "m_LocalScale";

    static void RemoveAnimationCurve(AnimationClip animationClip)
    {
        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(animationClip);
        for (int i = 0; i < curveBindings.Length; ++i)
        {
            EditorCurveBinding curveBinding = curveBindings[i];
            AnimationCurve curveData = AnimationUtility.GetEditorCurve(animationClip, curveBinding);

            if (curveBinding.propertyName.StartsWith(_Name_Scale))
            {
                bool vaild = false;
                for (int j = 0; j < curveData.length; ++j)
                {
                    Keyframe keyframe = curveData[j];
                    if (keyframe.value >= 0.99999 && keyframe.value <= 1.00001f)
                    {
                    }
                    else
                    {
                        vaild = true;
                        break;
                    }
                }

                if (!vaild)
                {
                    AnimationUtility.SetEditorCurve(animationClip, curveBinding, null);
                    Debug.LogFormat("删除了{0}的曲线{1} {2}", animationClip.name, i.ToString(), curveBinding.propertyName);
                }
            }
        }
    }

    static void CompressAnimationClip(AnimationClip _clip)
    {
        AnimationClipCurveData[] tCurveArr = AnimationUtility.GetAllCurves(_clip);
        Keyframe tKey;
        Keyframe[] tKeyFrameArr;
        for (int i = 0; i < tCurveArr.Length; ++i)
        {
            AnimationClipCurveData tCurveData = tCurveArr[i];
            if (tCurveData.curve == null || tCurveData.curve.keys == null)
            {
                continue;
            }
            tKeyFrameArr = tCurveData.curve.keys;
            for (int j = 0; j < tKeyFrameArr.Length; j++)
            {
                tKey = tKeyFrameArr[j];
                tKey.value = float.Parse(tKey.value.ToString("f3"));    //#.###
                tKey.inTangent = float.Parse(tKey.inTangent.ToString("f3"));
                tKey.outTangent = float.Parse(tKey.outTangent.ToString("f3"));
                tKey.inWeight = float.Parse(tKey.inWeight.ToString("f3"));
                tKey.outWeight = float.Parse(tKey.outWeight.ToString("f3"));
                tKeyFrameArr[j] = tKey;
            }
            tCurveData.curve.keys = tKeyFrameArr;
            _clip.SetCurve(tCurveData.path, tCurveData.type, tCurveData.propertyName, tCurveData.curve);
        }
    }

    [MenuItem("Assets/AnimationTools/生成所有AnimatorOverrideController")]
    static void CreateAllAnimatorOverrideControllers()
    {
        var arr = Selection.assetGUIDs;
        string dp = AssetDatabase.GUIDToAssetPath(arr[0]);
        FileInfo fileInfo = new FileInfo(dp);
        string assetPath = fileInfo.FullName.Substring(Application.dataPath.Length - 6);
        string tempPath = assetPath.Substring(21);

        AnimatorController baseController = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
        AnimationClip[] baseAnimationClips = baseController.animationClips;

        if (CSVBaseAction.Instance == null)
            CSVBaseAction.Load();

        Dictionary<string, FieldInfo> fieldInfoDicts = new Dictionary<string, FieldInfo>();
        Type type = typeof(CSVBaseAction.Data);
        FieldInfo[] fieldInfos = type.GetFields();
        for (int index =0, len = fieldInfos.Length; index < len; index++)
        {
            fieldInfoDicts[fieldInfos[index].Name] = fieldInfos[index];
        }
       
        var baseActionDatas = CSVBaseAction.Instance.GetAll();

        foreach (var baseActionData in baseActionDatas)
        {
            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(baseController);
            animatorOverrideController.name = $"aoc_{baseActionData.id}";

            List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            AnimationClipPair[] animationClipPair = animatorOverrideController.clips;
            for (int index = 0, len = animationClipPair.Length; index < len; index++)
            {
                string baseClipName = animationClipPair[index].originalClip.name;
                string[] strs = baseClipName.Split('_');
                string stateName = strs[strs.Length - 1];

                if (fieldInfoDicts.ContainsKey(stateName))
                {
                    string overrideClipName = fieldInfoDicts[stateName].GetValue(baseActionData).ToString();

                    if (!string.IsNullOrEmpty(overrideClipName))
                    {
                        var animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"Assets/Art/AnimationClip/{overrideClipName}.anim");
                        if (animationClip != null)
                        {
                            keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClipPair[index].originalClip, animationClip));
                        }
                        else
                        {
                            keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClipPair[index].originalClip, animationClipPair[index].originalClip));
                            Debug.Log($"Can not find clip: Assets/Art/AnimationClip/{overrideClipName}.anim");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"baseActionData{baseActionData.id} doesn't contain state: {stateName}");
                }
            }
            animatorOverrideController.ApplyOverrides(keyValuePairs);

            string targetPath = $"Assets/ResourcesAB/AnimatorController/{animatorOverrideController.name}.overrideController";
            AssetDatabase.CreateAsset(animatorOverrideController, targetPath);

            EditorUtility.SetDirty(animatorOverrideController);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/AnimationTools/生成所有AnimationClipListData")]
    static void CreateAllAnimationClipListDatas()
    {
        //ScriptableObjectUtility.CreateAsset<AnimationClipListData>();

        if (CSVSkillAction.Instance == null)
            CSVSkillAction.Load();

        var skillActionDatas = CSVSkillAction.Instance.GetAll();

        foreach (var skillActionData in skillActionDatas)
        {
            var asset = ScriptableObject.CreateInstance<AnimationClipListData>();
            ProjectWindowUtil.CreateAsset(asset, $"animdatas_{skillActionData.id}.asset");

            EditorUtility.SetDirty(asset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/AnimationTools/加载所有AnimationClip数据")]
    static void LoadAllAnimationClipData()
    {
        if (CSVSkillAction.Instance == null)
            CSVSkillAction.Load();

        if (CSVActionState.Instance == null)
            CSVActionState.Load();

        Dictionary<string, CSVActionState.Data> actionStatesDict = new Dictionary<string, CSVActionState.Data>();

        var actionStates = CSVActionState.Instance.GetAll();
        foreach (var actionState in actionStates)
        {
            actionStatesDict[actionState.action_state] = actionState;
        }

        var arr = Selection.assetGUIDs;
        string dp = AssetDatabase.GUIDToAssetPath(arr[0]);
        DirectoryInfo directoryInfo = new DirectoryInfo(dp);
        FileInfo[] fileInfos = directoryInfo.GetFiles("*.asset", SearchOption.AllDirectories);

        Dictionary<string, FieldInfo> fieldInfoDicts = new Dictionary<string, FieldInfo>();
        Type type = typeof(CSVSkillAction.Data);
        FieldInfo[] fieldInfos = type.GetFields();

        foreach (FileInfo fileInfo in fileInfos)
        {
            string assetPath = fileInfo.FullName.Substring(Application.dataPath.Length - 6);
            AnimationClipListData animationClipListData = AssetDatabase.LoadAssetAtPath<AnimationClipListData>(assetPath);
            Debug.Log(assetPath);
            string[] strs = animationClipListData.name.Split('_');
            animationClipListData.id = uint.Parse(strs[strs.Length - 1]);

            animationClipListData.animationClipDatasDict.Clear();
            animationClipListData.animationClipDatas.Clear();

            for (int index = 0, len = fieldInfos.Length; index < len; index++)
            {
                if (fieldInfos[index].Name != "id" && 
                    fieldInfos[index].Name != "action_id" && 
                    fieldInfos[index].Name != "weapon_type" && 
                    fieldInfos[index].Name != "weapon_action_id" &&
                    fieldInfos[index].Name != "path")
                {               
                    string clipName = fieldInfos[index].GetValue(CSVSkillAction.Instance.GetConfData(animationClipListData.id)).ToString();
                    if (!string.IsNullOrEmpty(clipName))
                    {
                        AnimationClipData animationClipData = new AnimationClipData();
                        animationClipData.name = fieldInfos[index].Name;
                        animationClipData.layer = actionStatesDict[fieldInfos[index].Name].action_Layer;

                        var animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"Assets/Art/AnimationClip/{clipName}.anim"); ;
                        if (animationClip != null)
                        {
                            animationClipData.clip = animationClip;
                        }
                        else
                        {
                            Debug.Log($"Can not find clip: Assets/Art/AnimationClip/{clipName}.anim");
                        }
                        animationClipListData.animationClipDatas.Add(animationClipData);
                    }              
                }
            }

            EditorUtility.SetDirty(animationClipListData);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
