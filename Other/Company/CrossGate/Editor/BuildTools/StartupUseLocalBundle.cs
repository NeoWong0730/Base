using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using System.IO;
using UnityEditor.AddressableAssets.Build;

[InitializeOnLoad]
public class StartupUseLocalBundle
{
    public enum PlayModeScript
    {
        //0-use Asset Database(fastest)  1-Simulate Group  2-Use Existing Build 3.Default Build Script  4.Use Share Build (requires share bundles)
        Fastest = 0,
        Simulate = 1,
        Default = 2,
        NewBuild = 3,//只有Build时才用
        Share = 4,
    }

    public const string bUseLocalGenBundle = "bUseLocalGenBundle";
    static StartupUseLocalBundle()
    {
#if !(UNITY_ANDROID || UNITY_IPHONE)
        AddSharePlayModeToDataBuilder(EditorPrefs.GetBool(StartupUseLocalBundle.bUseLocalGenBundle,true));
#else
        AddSharePlayModeToDataBuilder(false);
#endif
    }

    public static void AddSharePlayModeToDataBuilder(bool forceAB)
    {
        //1.获取 AddressableSetting
        AddressableAssetSettings aasettings = AddressableAssetSettingsDefaultObject.GetSettings(true);
        if (aasettings == null)
        {
            Debug.Log("AddressableAssetSettings == null");
            return;
        }

        //2.有没有 BuildScriptSharePlayMode Asset资源，没有则创建(使用aa内部封装的DataBuilderFolder)
        string assetSharePath = aasettings.DataBuilderFolder + "/" + typeof(BuildScriptSharePlayMode).Name + ".asset";
        if (!File.Exists(assetSharePath))
        {
            var script = ScriptableObject.CreateInstance<BuildScriptSharePlayMode>();
            AssetDatabase.CreateAsset(script, assetSharePath);
            AssetDatabase.Refresh();
        }

        //3.根据bundle的设置格式，设置当前运行模式
        if (forceAB)
        {
            //Debug.Log("使用内网bundle资源");
            //3.1判断有没有加入 AddressableAssetSettingsEditor 面板，没有则加入DataBuilders
            bool haveSharePlayMode = false;
            IDataBuilder dataBuilder = null;
            for (int i = 0; i < aasettings.DataBuilders.Count; i++)
            {
                dataBuilder = aasettings.GetDataBuilder(i);
                //BUG  dataBuilder只有脚本修改编译的后才会置成null，如果只是单纯的删除DataBuilder里的asset资源，即使 dataBuilder == null，也不会走这里！！！
                if (dataBuilder != null)
                {
                    if (dataBuilder.Name == "Use Share Build (requires share bundles)")
                    {
                        haveSharePlayMode = true;
                    }
                }
                else
                {
                    aasettings.RemoveDataBuilder(i);
                }
            }
            if (!haveSharePlayMode)
            {
                ScriptableObject builder = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetSharePath.Substring(assetSharePath.IndexOf("Assets\\")));
                if (!typeof(IDataBuilder).IsAssignableFrom(builder.GetType()))
                {
                    Debug.LogWarningFormat("Asset at {0} does not implement the IDataBuilder interface.", assetSharePath);
                    return;
                }
                aasettings.AddDataBuilder(builder as IDataBuilder);
                AssetDatabase.Refresh();
            }

            // 3.2设置Play Mode Script运行的模式
            AddressableAssetSettingsDefaultObject.Settings.ActivePlayModeDataBuilderIndex = (int)PlayModeScript.Share;
        }
        else
        {
            //不使用AB模式，需要还原默认的 本地加载路径
            var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
            string LocalBuildPathStr = "[UnityEngine.AddressableAssets.Addressables.BuildPath]/[BuildTarget]";
            m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kLocalBuildPath, LocalBuildPathStr);

            string LocalLoadPathStr = "{UnityEngine.AddressableAssets.Addressables.RuntimePath}/[BuildTarget]";
            m_Settings.profileSettings.SetValue(m_Settings.activeProfileId, AddressableAssetSettings.kLocalLoadPath, LocalLoadPathStr);

            if (m_Settings.ActivePlayModeDataBuilderIndex == (int)PlayModeScript.Simulate)
            {
                //Debug.Log("使用simulate模式"); 
            }
            else if (m_Settings.ActivePlayModeDataBuilderIndex == (int)PlayModeScript.Default)
            { 
                //Debug.Log("使用default模式");
            }   
            else
            {
                m_Settings.ActivePlayModeDataBuilderIndex = (int)PlayModeScript.Fastest;
                //Debug.Log("使用fast模式");
            }
        }
    }

}
