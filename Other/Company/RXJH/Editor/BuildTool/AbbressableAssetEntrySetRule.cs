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
    SceneBase,//����Scene
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

                //1. ���� �� �� ��ַ ======��
                if (!Enum.TryParse(groupName, false, out AddressableGroupName addressableGroupName))
                {
                    //Debug.LogError($"δ������{groupName}���������ÿ�Ѱַ����Դ·����{assetPath}");
                    //return;
                }

                switch (addressableGroupName)
                {
                    case AddressableGroupName.AnimatorController:
                        if (!(assetPath.EndsWith(".overrideController") || assetPath.EndsWith(".controller")))
                        {
                            Debug.Log($"��Դ���ô���{assetPath}, ���ļ�����ֻ����.controller .overrideController�ļ�");
                            return;
                        }
                        break;
                    case AddressableGroupName.Atlas:
                        if (!assetPath.EndsWith(".spriteatlas"))
                        {
                            Debug.Log($"��Դ���ô���{assetPath}, ���ļ�����ֻ����.spriteatlas");
                            return;
                        }
                        break;
                    case AddressableGroupName.FaceDetail:
                        if (!assetPath.EndsWith(".asset"))
                        {
                            Debug.Log($"��Դ���ô���{assetPath}, ���ļ�����ֻ����.spriteatlas");
                            return;
                        }
                        break;
                    case AddressableGroupName.Audio:
                        if (!assetPath.EndsWith(".wav"))
                        {
                            Debug.Log($"��Դ���ô���{assetPath}, ���ļ�����ֻ����.wav");
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
                            Debug.Log($"��Դ���ô���{assetPath}, ���ļ�����ֻ����.mat");
                            return;
                        }
                        break;
                    case AddressableGroupName.Prefab:
                        if (!assetPath.EndsWith(".prefab"))
                        {
                            Debug.Log($"��Դ���ô���{assetPath}, ���ļ�����ֻ����.prefab�ļ�");
                            return;
                        }
                        break;
                    case AddressableGroupName.Scene:
                        if (!assetPath.EndsWith(".unity"))
                        {
                            Debug.Log($"��Դ���ô���{assetPath}, ���ļ�����ֻ����.unity�ļ�");
                            return;
                        }
                        break;
                    case AddressableGroupName.SceneBase:
                        if (!assetPath.EndsWith(".unity"))
                        {
                            Debug.Log($"��Դ���ô���{assetPath}, ���ļ�����ֻ����.unity�ļ�");
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
                            Debug.Log($"��Դ���ô���{assetPath}, ���ļ�����ֻ����.png�ļ�");
                            return;
                        }
                        break;
                    case AddressableGroupName.UI:
                        if (!assetPath.EndsWith(".prefab"))
                        {
                            Debug.Log($"��Դ���ô���{assetPath}, ���ļ�����ֻ����.prefab�ļ�");
                            return;
                        }
                        break;
                    default:
                        
                        break;
                }

                //2. ����ַ�Ϸ��� ====��
                if (!FrameworkTool.IsAllUTF8(address))
                {
                    Debug.LogErrorFormat("�ļ�·��������������ַ� {0}", address);
                }


                //3. ��������ͬ����Դ ====��
                if (CheckAssetEntrySameName(assetPath, address))
                    return;

                //4. ��ȡGroup ====��
                AddressableAssetGroup assetGroup = settings.FindGroup(groupName);
                if (assetGroup == null)
                {
                    assetGroup = settings.CreateGroup(groupName, false, false, false, settings.DefaultGroup.Schemas);
                    BundledAssetGroupSchema bundledAssetGroupSchema = assetGroup.GetSchema<BundledAssetGroupSchema>();
                    bundledAssetGroupSchema.UseAssetBundleCrc = false;//�ر�bundle Crc У��
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

                //4. ����AssetEntry ===��
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

        //��Ҫ�������е�group, address����Ŀ�ĵ�ַ������ResourceManagerϵͳ�б���Ϊ����,����Ҫ��address(�ļ���)Ψһ�������޷���ȷ������Դ��
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
                    Debug.LogError($"������ͬaddress(�ļ���):{filename},����Դ:{assetPath},��һ����Դ:{item.AssetPath}");
                    isSame = true;
                }
            }
        }
        return isSame;
    }

}
