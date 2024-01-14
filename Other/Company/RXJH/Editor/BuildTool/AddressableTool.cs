using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Player;
using System.IO;

public static class AddressableTool
{
#if !(UNITY_ANDROID || UNITY_IPHONE)
    private const string selectABModeMenuPath = "__Tools__/������/������Դ(ʹ��AB)";
    [MenuItem(selectABModeMenuPath, priority = 0)]
    internal static void SetLoadLocalBundleMode()
    {
        bool bSelected = Menu.GetChecked(selectABModeMenuPath);
        Menu.SetChecked(selectABModeMenuPath, !bSelected);
        EditorPrefs.SetBool(StartupUseLocalBundle.bUseLocalGenBundle, !bSelected);
        StartupUseLocalBundle.AddSharePlayModeToDataBuilder(EditorPrefs.GetBool(StartupUseLocalBundle.bUseLocalGenBundle));
    }

    [MenuItem(selectABModeMenuPath, true)]
    public static bool SetLoadLocalBundleMode_Check_Menu()
    {
        Menu.SetChecked(selectABModeMenuPath, EditorPrefs.GetBool(StartupUseLocalBundle.bUseLocalGenBundle, true));
        return true;
    }
#endif


    [MenuItem("__Tools__/������/���֮ǰ���Խű��Ƿ��ܱ���ɹ�")]
    internal static void BuildPlayerScriptsOnly()
    {
        BuildTarget targetPlatform = EditorUserBuildSettings.activeBuildTarget;
        var targetGroup = BuildPipeline.GetBuildTargetGroup(targetPlatform);
        ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings()
        {
            target = targetPlatform,
            group = targetGroup,
            options = ScriptCompilationOptions.None
        };

        string TempOutputFolder = Path.GetDirectoryName(Application.dataPath) + "/AppScriptDll";
        if (Directory.Exists(TempOutputFolder))
        {
            Directory.Delete(TempOutputFolder, true);
        }
        Directory.CreateDirectory(TempOutputFolder);


        ScriptCompilationResult scriptCompilationResult = PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, TempOutputFolder);

        if (scriptCompilationResult.assemblies.Count == 0)
        {
            Debug.LogError("�ű�����ʧ��");
        }
        else
        {
            Debug.Log("�ű�����ɹ�:" + scriptCompilationResult.assemblies.Count);
        }

        //ɾ����ʱ�ļ�
        if (Directory.Exists(TempOutputFolder))
        {
            Directory.Delete(TempOutputFolder, true);
        }
    }


    [MenuItem("__Tools__/������/��Library��aa")]
    public static void openLibrary()
    {
        string path = Application.dataPath.Replace("/Assets", "/") + Addressables.RuntimePath;
        Application.OpenURL(path);
        Debug.Log("��Library��aa:" + path);
    }


    [MenuItem("__Tools__/������/��ɳ��·��")]
    public static void OpenPersistentPath()
    {
        Application.OpenURL(Application.persistentDataPath);
        Debug.Log("��ɳ��·��:" + Application.persistentDataPath);
    }


    [MenuItem("__Tools__/������/�򿪻���·��")]
    public static void OpenCachePath()
    {
        Application.OpenURL(Caching.defaultCache.path);
        Debug.Log("�򿪻���·��:" + Caching.defaultCache.path);
    }


    [MenuItem("__Tools__/������/����Ŀ·��")]
    public static void OpenProjectPath()
    {
        Application.OpenURL(Application.dataPath.Replace("/Assets", "/"));
        Debug.Log("����Ŀ·��:" + Application.dataPath.Replace("/Assets", "/"));
    }


    [MenuItem("__Tools__/������/�������־·��")]
    public static void OpenConsoleLogPath()
    {
        Application.OpenURL(Application.consoleLogPath);
        Debug.Log("�������־·��:" + Application.consoleLogPath);
    }

}
