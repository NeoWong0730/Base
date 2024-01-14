using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Author: Administrator created 2018/05/28 19:54:37 Monday on pc: USER-BVF9IJRS7E.
// Copyright@DefaultCompany`s SlugPrj. All rights reserved.

public class MenuItem_App : Editor
{
#if UNITY_STANDALONE
    static BuildTargetGroup eBuildTargetGroup = BuildTargetGroup.Standalone;
#elif UNITY_ANDROID
    static BuildTargetGroup eBuildTargetGroup = BuildTargetGroup.Android;
#elif UNITY_IOS
    static BuildTargetGroup eBuildTargetGroup = BuildTargetGroup.iOS;
#endif

    [MenuItem("__App__/打开主场景")]
    public static void OpenMainScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            string mainScenePath = "Assets/ResourcesAB/GameStart.unity";
            EditorSceneManager.OpenScene(mainScenePath);
        }
    }

    [MenuItem("__App__/清空设置")]
    public static void ClearSetting()
    {
        PlayerPrefs.DeleteKey("1");
        PlayerPrefs.DeleteKey("SimplifyDisplayActive");
        PlayerPrefs.DeleteKey("SystemChannelShow");
    }

    [MenuItem("__App__/设置面板")]
    public static void OpenSetting()
    {
        EditorWindow.GetWindow<AppSetting>();
    }

    private static void AddScriptingDefineSymbols(BuildTargetGroup buildTargetGroup, string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return;

        string sDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        string[] defines = sDefines.Split(';');
        List<string> defs = defines.ToList<string>();

        if (!sDefines.Contains(symbol))
        {
            defs.Add(symbol);
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", defs));
    }

    private static void RemoveScriptingDefineSymbols(BuildTargetGroup buildTargetGroup, string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return;

        string sDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        string[] defines = sDefines.Split(';');
        List<string> defs = new List<string>();

        for (int i = 0; i < defines.Length; ++i)
        {
            if (!string.Equals(defines[i], symbol, StringComparison.Ordinal))
            {
                defs.Add(defines[i]);
            }
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", defs));
    }

    [MenuItem("__Tools__/添加选中场景")]
    public static void AddScene()
    {
        List<EditorBuildSettingsScene> sceneList = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        Dictionary<string, EditorBuildSettingsScene> sceneDict = new Dictionary<string, EditorBuildSettingsScene>();
        sceneList.ForEach((scene) =>
        {
            sceneDict.Add(scene.path, scene);
        });

        UnityEngine.Object[] objects = Selection.objects;
        foreach (var obj in objects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if ((obj is SceneAsset) && (!sceneDict.ContainsKey(assetPath)))
            {
                sceneList.Add(new EditorBuildSettingsScene(assetPath, true));
            }
        }

        EditorBuildSettings.scenes = sceneList.ToArray();
    }

    [MenuItem("__Tools__/处理UI")]
    public static void UIProcesser()
    {
        EditorWindow.GetWindow<UIProcesser>();
    }
    
    [MenuItem("__Tools__/获取动画时长")]
    public static void GetAnimation()
    {
        Table.CSVCharacter.Load();
        Table.CSVAction.Load();
        Table.CSVActionState.Load();
        Logic.Core.AssetsGroupLoader assetsGroupLoader = new Logic.Core.AssetsGroupLoader();

        List<uint> weapon_typess = new List<uint> { 1, 2, 3, 4, 5, 6, 7, 8, 28 };
        var csvDict = Table.CSVCharacter.Instance.GetAll();
        List<string> actionStates = new List<string>();
        for (int i = 0; i < weapon_typess.Count; i++)
        {
            foreach (var kvp in csvDict)
            {
                var paths = GetAnimationPaths(kvp.id, weapon_typess[i], ref actionStates);
                if (paths != null)
                {
                    for (int j = 0; j < paths.Count; j++)
                    {
                        assetsGroupLoader.AddLoadTask(paths[j], actionStates[j]);
                    }
                }
            }
        }
        
        assetsGroupLoader.StartLoad(null, () =>
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            string path = Application.dataPath + "/../AnimationTimeLength.txt";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            
            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int i = 0; i < assetsGroupLoader.descs.Count; i++)
                {
                    AnimationClip clip = assetsGroupLoader.assetRequests[i].Result as AnimationClip;
                    stringBuilder.AppendLine(string.Format("{0}  时间长度：{1}  帧数 {2}", assetsGroupLoader.descs[i], clip.length, clip.length * clip.frameRate * 2));
                }
                
                writer.Write(stringBuilder.ToString());
            }
        }, null);
    }
    
    public static List<string> GetAnimationPaths(uint curCharID, uint equipment_type, ref List<string> actionStates)
    {
        actionStates.Clear();
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        List<string> paths = new List<string>();
        uint animationKey = curCharID * Logic.Constants.CHARPARAM + equipment_type * Logic.Constants.WEAPONTYPEPARAM + 0;
        Table.CSVAction.Data actionData = Table.CSVAction.Instance.GetConfData(animationKey);
        if (actionData != null)
        {
            var actionStates2 = Table.CSVActionState.Instance.GetAll();
            Type type = actionData.GetType();
            foreach (var actionState in actionStates2)
            {
                string dir = type.GetField(actionState.action_state).GetValue(actionData).ToString();
                if (!String.IsNullOrWhiteSpace(dir))
                {
                    stringBuilder.Clear();
                    string finalPath = stringBuilder.Append(actionData.dirPath).Append("/").Append(dir).Append(Logic.Constants.ANIMSUFFIX).ToString();
                    paths.Add(finalPath);
                    actionStates.Add("HeroId: " + actionData.hero_id + "   WeaponType: " + actionData.weapon_type + "  AnimationState: " + actionState.action_state);
                }
            } 
        }
        return paths;
    }

    // 快速获取hierichy中gameobject的路径
    [MenuItem("GameObject/CopytHierichyPath(Ctrl G) %G", priority = 0)]
    public static void GetHierichyPath() {
        GameObject go = Selection.activeGameObject;
        if (go != null) {
            Undo.RecordObject(go, "Ctrl Z");

            string path = string.Empty;
            path = FullHierarchyPath(go);
            GUIUtility.systemCopyBuffer = path; // 将string复制到系统剪切板，然后外部使用Ctrl_V从剪切板中剪切出来，方便使用
        }
    }
    public static string FullHierarchyPath(GameObject gameObject) {
        if (gameObject == null)
            return null;

        List<string> paths = new List<string>();
        var t = gameObject.transform;
        while (t != null) {
            paths.Add(t.name);
            t = t.parent;
        }
        string result = "";
        for (int i = paths.Count - 2; i >= 0; --i) {
            result = result + paths[i];
            if (i != 0) {
                result = result + "/";
            }
        }

        return result;
    }
}