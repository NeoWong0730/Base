using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using YooAsset.Editor;

namespace NWFramework.Editor
{
    /// <summary>
    /// 打包工具类
    /// </summary>
    public static class ReleaseTools
    {
        [MenuItem("NWFramework/BuildDll/BuildAndCopyDlls", false, 30)]
        public static void BuildAndCopyDlls()
        {
#if Hotfix_HyBridCLR
            BuildDLLCommand.HYBridCLR_BuildAndCopyDlls(BuildTarget.StandaloneWindows64);
#elif Hotfix_ILRuntime

#elif Hotfix_XLua

#else

#endif
        }

        [MenuItem("NWFramework/Build/一键打包Windows", false, 30)]
        public static void AutomationBuildWindows()
        {
#if Hotfix_HyBridCLR
            BuildDLLCommand.HYBridCLR_BuildAndCopyDlls(BuildTarget.StandaloneWindows64);
#elif Hotfix_ILRuntime

#elif Hotfix_XLua

#else

#endif
            AssetDatabase.Refresh();
            BuildInternal(BuildTarget.StandaloneWindows64, Application.dataPath + "/../Builds/Windows", GetBuildPackageVersion());
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, $"{Application.dataPath}/../Builds/Windows/{GetBuildPackageVersion()}Windows.exe");
        }

        [MenuItem("NWFramework/Build/一键打包Android", false, 30)]
        public static void AutomationBuildAndroid() 
        {
#if Hotfix_HyBridCLR
            BuildDLLCommand.HYBridCLR_BuildAndCopyDlls(BuildTarget.Android);
#elif Hotfix_ILRuntime

#elif Hotfix_XLua

#else

#endif
            AssetDatabase.Refresh();
            BuildInternal(BuildTarget.Android, Application.dataPath + "/../Builds/Android", GetBuildPackageVersion());
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.Android, BuildTarget.Android, $"{Application.dataPath}/../Builds/Android/{GetBuildPackageVersion()}Android.apk");
        }

        [MenuItem("NWFramework/Build/一键打包IOS", false, 30)]
        public static void AutomationBuildIOS() 
        {
#if Hotfix_HyBridCLR
            BuildDLLCommand.HYBridCLR_BuildAndCopyDlls(BuildTarget.iOS);
#elif Hotfix_ILRuntime

#elif Hotfix_XLua

#else

#endif
            AssetDatabase.Refresh();
            BuildInternal(BuildTarget.iOS, Application.dataPath + "/../Builds/iOS", GetBuildPackageVersion());
            AssetDatabase.Refresh();
            BuildImp(BuildTargetGroup.iOS, BuildTarget.iOS, $"{Application.dataPath}/../Builds/iOS/XCode_Project");
        }

        private static string GetBuildPackageVersion()
        {
            int totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
        }

        private static void BuildInternal(BuildTarget buildTarget, string outputRoot, string packageVersion = "1.0")
        {
            Debug.Log($"开始构建 : {buildTarget}");

            BuildParameters.SBPBuildParameters sbpBuildParameters = new BuildParameters.SBPBuildParameters();
            sbpBuildParameters.WriteLinkXML = true;

            //构建参数
            BuildParameters buildParameters = new BuildParameters()
            {
                StreamingAssetsRoot = AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot(),
                BuildOutputRoot = outputRoot,
                BuildTarget = buildTarget,
                BuildPipeline = EBuildPipeline.ScriptableBuildPipeline,
                BuildMode = EBuildMode.IncrementalBuild,
                PackageName = "DefaultPackage",
                PackageVersion = packageVersion,
                VerifyBuildingResult = true,
                SharedPackRule = new ZeroRedundancySharedPackRule(),
                CompressOption = ECompressOption.LZ4,
                OutputNameStyle = EOutputNameStyle.BundleName_HashName,
                CopyBuildinFileOption = ECopyBuildinFileOption.ClearAndCopyByTags,
                CopyBuildinFileTags = "Meta",
                SBPParameters = sbpBuildParameters,
            };

            //执行构建

            AssetBundleBuilder builder = new AssetBundleBuilder();
            var buildResult = builder.Run(buildParameters);
            if (buildResult.Success)
            {
                Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
            }
            else
            {
                Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
            }
        }

        private static void BuildImp(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, string locationPathName)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, BuildTarget.StandaloneWindows64);
            AssetDatabase.Refresh();

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = new[] { "Assets/Scenes/main.unity" },
                locationPathName = locationPathName,
                targetGroup = buildTargetGroup,
                target = buildTarget,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;
            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"Build success: {summary.totalSize / 1024 / 1024} MB");
            }
            else
            {
                Debug.Log($"Build Failed" + summary.result);
            }
        }
    }
}