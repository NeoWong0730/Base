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
    private const string selectABModeMenuPath = "__Tools__/打包相关/加载资源(使用AB)";
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


    [MenuItem("__Tools__/打包相关/打包之前测试脚本是否能编译成功")]
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
            Debug.LogError("脚本编译失败");
        }
        else
        {
            Debug.Log("脚本编译成功:" + scriptCompilationResult.assemblies.Count);
        }

        //删除零时文件
        if (Directory.Exists(TempOutputFolder))
        {
            Directory.Delete(TempOutputFolder, true);
        }
    }


    [MenuItem("__Tools__/打包相关/打开Library下aa")]
    public static void openLibrary()
    {
        string path = Application.dataPath.Replace("/Assets", "/") + Addressables.RuntimePath;
        Application.OpenURL(path);
        Debug.Log("打开Library下aa:" + path);
    }


    [MenuItem("__Tools__/打包相关/打开沙盒路径")]
    public static void OpenPersistentPath()
    {
        Application.OpenURL(Application.persistentDataPath);
        Debug.Log("打开沙盒路径:" + Application.persistentDataPath);
    }


    [MenuItem("__Tools__/打包相关/打开缓存路径")]
    public static void OpenCachePath()
    {
        Application.OpenURL(Caching.defaultCache.path);
        Debug.Log("打开缓存路径:" + Caching.defaultCache.path);
    }


    [MenuItem("__Tools__/打包相关/打开项目路径")]
    public static void OpenProjectPath()
    {
        Application.OpenURL(Application.dataPath.Replace("/Assets", "/"));
        Debug.Log("打开项目路径:" + Application.dataPath.Replace("/Assets", "/"));
    }


    [MenuItem("__Tools__/打包相关/打开输出日志路径")]
    public static void OpenConsoleLogPath()
    {
        Application.OpenURL(Application.consoleLogPath);
        Debug.Log("打开输出日志路径:" + Application.consoleLogPath);
    }

}
