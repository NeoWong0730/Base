using Lib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public enum AddressableGroupName
{
    None =0,
    AnimationData,
    AnimatorController,
    Atlas,
    AvatarMask,
    BehaviorTree,
    FaceDetail,
    Audio,
    Emoji,
    Font,
    Material,
    Prefab,
    Scene,
    SceneBase,//所属Scene
    Shader,
    Texture,
    UI,
}

public class AbbressableAssetEntrySetRule
{
    private static string sResourceABDir = "Assets/ResourcesAB/";
    public static void SetAddressableAssetEntryName(string assetPath)
    {
        if (!AssetDatabase.IsValidFolder(assetPath))
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (assetPath.StartsWith(sResourceABDir, System.StringComparison.Ordinal))
            {
                int index = assetPath.IndexOf('/', sResourceABDir.Length);
                if (index < 0)
                {
                    return;
                }

                string groupName = assetPath.Substring(sResourceABDir.Length, index - sResourceABDir.Length);
                string address = Path.GetFileNameWithoutExtension(assetPath);

                //1. 设置 组 和 地址 ======》
                if (!Enum.TryParse(groupName, false, out AddressableGroupName addressableGroupName))
                {
                    //Debug.LogError($"未经处理{groupName}，不会设置可寻址，资源路径：{assetPath}");
                    //return;
                }

                switch (addressableGroupName)
                {
                    case AddressableGroupName.AnimatorController:
                        if (!(assetPath.EndsWith(".overrideController") || assetPath.EndsWith(".controller")))
                        {
                            Debug.Log($"资源放置错误：{assetPath}, 此文件夹下只放置.controller .overrideController文件");
                            return;
                        }
                        break;
                    case AddressableGroupName.Atlas:
                        if (!assetPath.EndsWith(".spriteatlas"))
                        {
                            Debug.Log($"资源放置错误：{assetPath}, 此文件夹下只放置.spriteatlas");
                            return;
                        }
                        break;
                    case AddressableGroupName.FaceDetail:
                        if (!assetPath.EndsWith(".asset"))
                        {
                            Debug.Log($"资源放置错误：{assetPath}, 此文件夹下只放置.spriteatlas");
                            return;
                        }
                        break;
                    case AddressableGroupName.Audio:
                        if (!assetPath.EndsWith(".wav"))
                        {
                            Debug.Log($"资源放置错误：{assetPath}, 此文件夹下只放置.wav");
                            return;
                        }
                        break;
                    case AddressableGroupName.Emoji:
                        if (groupName == "Emoji" && !assetPath.EndsWith(".asset"))
                        {
                            return;
                        }
                        break;
                    case AddressableGroupName.Font:
                        if (!(assetPath.EndsWith(".ttf") || assetPath.EndsWith(".fontsettings") || assetPath.EndsWith(".TTF")))
                        {
                            return;
                        }
                        break;
                    case AddressableGroupName.Material:
                        if (!assetPath.EndsWith(".mat"))
                        {
                            Debug.Log($"资源放置错误：{assetPath}, 此文件夹下只放置.mat");
                            return;
                        }
                        break;
                    case AddressableGroupName.Prefab:
                        if (!assetPath.EndsWith(".prefab"))
                        {
                            Debug.Log($"资源放置错误：{assetPath}, 此文件夹下只放置.prefab文件");
                            return;
                        }
                        break;
                    case AddressableGroupName.Scene:
                        if (!assetPath.EndsWith(".unity"))
                        {
                            Debug.Log($"资源放置错误：{assetPath}, 此文件夹下只放置.unity文件");
                            return;
                        }
                        break;
                    case AddressableGroupName.SceneBase:
                        if (!assetPath.EndsWith(".unity"))
                        {
                            Debug.Log($"资源放置错误：{assetPath}, 此文件夹下只放置.unity文件");
                            return;
                        }
                        break;
                    case AddressableGroupName.Shader:
                        if (!(assetPath.EndsWith(".shadervariants") || assetPath.EndsWith(".shader") || assetPath.EndsWith(".shadergraph")))
                        {
                            return;
                        }
                        break;
                    case AddressableGroupName.Texture:
                        if (!assetPath.EndsWith(".png"))
                        {
                            Debug.Log($"资源放置错误：{assetPath}, 此文件夹下只放置.png文件");
                            return;
                        }
                        break;
                    case AddressableGroupName.UI:
                        if (!assetPath.EndsWith(".prefab"))
                        {
                            Debug.Log($"资源放置错误：{assetPath}, 此文件夹下只放置.prefab文件");
                            return;
                        }
                        break;
                    default:
                        
                        break;
                }

                //2. 检查地址合法性 ====》
                if (!FrameworkTool.IsAllUTF8(address))
                {
                    Debug.LogErrorFormat("文件路径包含不合理的字符 {0}", address);
                }


                //3. 查找有无同名资源 ====》
                if (CheckAssetEntrySameName(assetPath, address))
                    return;

                //4. 获取Group ====》
                AddressableAssetGroup assetGroup = settings.FindGroup(groupName);
                if (assetGroup == null)
                {
                    assetGroup = settings.CreateGroup(groupName, false, false, false, settings.DefaultGroup.Schemas);
                    BundledAssetGroupSchema bundledAssetGroupSchema = assetGroup.GetSchema<BundledAssetGroupSchema>();
                    bundledAssetGroupSchema.UseAssetBundleCrc = false;//关闭bundle Crc 校验
                    bundledAssetGroupSchema.UseAssetBundleCache = true;
                    bundledAssetGroupSchema.UseAssetBundleCrcForCachedBundles = true;
                    bundledAssetGroupSchema.IncludeGUIDInCatalog = false;
                    bundledAssetGroupSchema.RetryCount = AddressableBundleGroupSetting.WebRequestRetryCount;
                    bundledAssetGroupSchema.Timeout = AddressableBundleGroupSetting.WebRequestTimeOut;
                    bundledAssetGroupSchema.AssetBundledCacheClearBehavior = BundledAssetGroupSchema.CacheClearBehavior.ClearWhenSpaceIsNeededInCache;
                    bundledAssetGroupSchema.InternalIdNamingMode = BundledAssetGroupSchema.AssetNamingMode.FullPath;

                    assetGroup.GetSchema<ContentUpdateGroupSchema>().StaticContent = false;
                    bundledAssetGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.FileNameHash;
                    bundledAssetGroupSchema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
                    bundledAssetGroupSchema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);

                    if (addressableGroupName == AddressableGroupName.Shader)
                    {
                        bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel;
                    }
                    else
                    {
                        bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
                    }
                }

                //4. 加入AssetEntry ===》
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, assetGroup);
                entry.address = address;
                if (addressableGroupName == AddressableGroupName.Shader)
                {
                    entry.SetLabel("shader", true, true);
                }
            }
        }
    }

    public static bool CheckAssetEntrySameName(string assetPath, string filename)
    {
        bool isSame = false;
        filename = filename.ToLower();
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

        //需要遍历所有的group, address：条目的地址，它在ResourceManager系统中被视为主键,必须要求address(文件名)唯一，否则无法正确加载资源。
        foreach (var group in settings.groups)
        {
            if (group.name.Equals("Default Local Group", StringComparison.Ordinal) || group.name.Equals("Built In Data", StringComparison.Ordinal))
                continue;

            if (group.m_SerializeEntries == null)
                continue;

            foreach (var item in group.m_SerializeEntries)
            {
                if (item.AssetPath.Equals(assetPath, StringComparison.Ordinal))
                    continue;
                if (item.address.Equals(filename, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.LogError($"存在相同address(文件名):{filename},该资源:{assetPath},另一个资源:{item.AssetPath}");
                    isSame = true;
                }
            }
        }
        return isSame;
    }

}
