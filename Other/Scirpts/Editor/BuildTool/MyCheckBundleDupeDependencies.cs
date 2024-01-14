using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.AnalyzeRules;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public class MyCheckBundleDupeDependencies : CheckBundleDupeDependencies
{

    public override void FixIssues(AddressableAssetSettings settings)
    {
        //为什么要分析2次，修正问题2次才能最终解决问题 ？？？？？

        HashSet<GUID> implicitAssets = new HashSet<GUID>();

        bool hashasImplicitShader = false;
        foreach (var asset in m_ImplicitAssets)
        {
            string assetGuid = asset.ToString();
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

            //剔除属于图集的,不解决图集的重复依赖问题
            if (assetPath.StartsWith("Assets/Projects/Image", System.StringComparison.Ordinal))
                continue;

            //shader 特殊处理，合并到一起
            if (assetPath.EndsWith(".shader", System.StringComparison.Ordinal)
                || assetPath.EndsWith(".shadergraph", System.StringComparison.Ordinal)
                || assetPath.EndsWith(".shadervariants", System.StringComparison.Ordinal))
            {
                var groupShader = CreateOrGetNonStaticGroup(settings, "Shader");
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(assetGuid, groupShader, false, false);
                entry.address = assetGuid;
                entry.SetLabel("shader", true, true);

                hashasImplicitShader = true;
                Debug.LogFormat("有Shader未收集全 {0}", assetPath);
                continue;
            }

            //标记有重复依赖的资源 排除图集引用
            //将其他重复依赖资源 加入hashset 交由基类原有逻辑处理
            implicitAssets.Add(asset);
        }

        m_ImplicitAssets.Clear();

        //当implicitAssets.Count > 0 会调用 settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
        if (implicitAssets.Count > 0)
        {
            Debug.LogError("有未收集全的资源");

            m_ImplicitAssets = implicitAssets;
            base.FixIssues(settings);
        }
        else if (hashasImplicitShader)
        {
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
        }

        //需要重新设置(默认BundleName格式需要更改) 
        var groupDefault = CreateOrGetNonStaticGroup(settings, "Default Local Group");
    }


    private AddressableAssetGroup CreateOrGetNonStaticGroup(AddressableAssetSettings settings, string groupName)
    {
        var group = settings.FindGroup(findGroup => findGroup != null && findGroup.Name == groupName);
        if (group == null || groupName == "Default Local Group")
        {
            if(group ==null)
                group = settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));

            //非静态
            group.GetSchema<ContentUpdateGroupSchema>().StaticContent = false;
            //设置远端模
            BundledAssetGroupSchema groupSchema = group.GetSchema<BundledAssetGroupSchema>();
            groupSchema.UseAssetBundleCrc = false;
            groupSchema.UseAssetBundleCache = true;
            groupSchema.UseAssetBundleCrcForCachedBundles = true;
            groupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.FileNameHash;
            groupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
            groupSchema.InternalIdNamingMode = BundledAssetGroupSchema.AssetNamingMode.FullPath;
            groupSchema.IncludeGUIDInCatalog = false;
            groupSchema.RetryCount = AddressableBundleGroupSetting.WebRequestRetryCount;
            groupSchema.Timeout = AddressableBundleGroupSetting.WebRequestTimeOut;
            groupSchema.AssetBundledCacheClearBehavior = BundledAssetGroupSchema.CacheClearBehavior.ClearWhenSpaceIsNeededInCache;
            groupSchema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
            groupSchema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);
        }

        return group;
    }

}
