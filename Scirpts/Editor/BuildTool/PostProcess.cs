using System;
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;


public class PostProcess : IPreprocessBuildWithReport, IPostprocessBuildWithReport, IPostGenerateGradleAndroidProject, IUnityLinkerProcessor
{
    /// <summary>
    /// 构建前置处理
    /// </summary>
    /// <param name="target"></param>
    /// <param name="path"></param>
    public void OnPreprocessBuild(BuildReport report)
    {
        switch (report.summary.platform)
        {
            case BuildTarget.Android:
                PostprocessBuild_Android.OnPreprocessBuild_Android(report.summary.outputPath);
                break;
            case BuildTarget.iOS:
                PostprocessBuild_iOS.OnPreprocessBuild_iOS(report.summary.outputPath);
                break;
        }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        switch (report.summary.platform)
        {
            case BuildTarget.Android:
                PostprocessBuild_Android.OnPostprocessBuild_Android(report.summary.outputPath);
                break;
            case BuildTarget.iOS:
                PostprocessBuild_iOS.OnPostprocessBuild_iOS(report.summary.outputPath);
                break;
            case BuildTarget.StandaloneWindows64:
                PostprocessBuild_PC.OnPostprocessBuild_PC(report.summary.outputPath);
                break;
        }
    }


    int IOrderedCallback.callbackOrder => 0;
    void IPostGenerateGradleAndroidProject.OnPostGenerateGradleAndroidProject(string path)
    {
        Debug.Log("OnPostGenerateGradleAndroidProject");

        string platform = Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget);

        //需要区分是否是分包
        BuildSetting mBuildSetting = null;
        string settingPath = string.Format("{0}/{1}", Application.dataPath, "DefaultBuildSetting.asset");
        if (File.Exists(settingPath))
        {
            mBuildSetting = AssetDatabase.LoadAssetAtPath<BuildSetting>("Assets/DefaultBuildSetting.asset");
        }

        if (mBuildSetting.bUseSplitPack)
        {
            //copy Bundle
            string srcBundlePath = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, BuildTool.AssetBundleBuildPath, platform, "aa", "Android");
            string dstAaPath = string.Format("{0}/{1}/{2}/{3}/{4}", path, "src", "main", "assets", "aa");
            FileTool.CreateFolder(dstAaPath);
            string dstAaAndroidPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", path, "src", "main", "assets", "aa", "Android");
            FileTool.CreateFolder(dstAaAndroidPath);

            DirectoryInfo dir = new DirectoryInfo(srcBundlePath);

            long TotalSize = 0;
            long MegaByte900 = 1048576 * 900;

            FileInfo[] fileInfos = dir.GetFiles();
            for (int i = 0; i < fileInfos.Length; ++i)
            {
                if (TotalSize > MegaByte900)
                {

                }
                else
                {
                    FileInfo fileInfo = fileInfos[i];
                    string desTemp = string.Format("{0}/{1}", dstAaAndroidPath, fileInfo.Name);
                    File.Copy(fileInfo.FullName, desTemp, true);
                    TotalSize += fileInfo.Length;
                }
            }


            //copy catalog.json
            string srcS0CatalogPath = string.Format("{0}/{1}/{2}/aa/catalog.json", Application.dataPath, BuildTool.AssetBundleBuildPath, BuildTool.AssetBundleS0Name);
            string desCatalogPath = string.Format("{0}/{1}", dstAaPath, "catalog.json");
            File.Copy(srcS0CatalogPath, desCatalogPath);


            // setting.json
            string oriSettingPath = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, BuildTool.AssetBundleBuildPath, platform, "aa", "settings.json");
            string dessettingPath = string.Format("{0}/{1}", dstAaPath, "settings.json");
            File.Copy(oriSettingPath, dessettingPath, true);


            //copy Config
            string dstConfigPath = string.Format("{0}/{1}/{2}/{3}/{4}", path, "src", "main", "assets", "Config");
            FileTool.DeleteFolder(dstConfigPath);
            string srcConfigPath = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, BuildTool.AssetBundleBuildPath, platform, "Config");
            FileTool.CopyAndReplaceDirectory(srcConfigPath, dstConfigPath);

            //copy Dll
            string dstLogicDllPath = string.Format("{0}/{1}/{2}/{3}/{4}", path, "src", "main", "assets", "LogicDll");
            FileTool.DeleteFolder(dstLogicDllPath);
            string srcLogicDllPath = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, BuildTool.AssetBundleBuildPath, platform, "LogicDll");
            FileTool.CopyAndReplaceDirectory(srcLogicDllPath, dstLogicDllPath);

        }
        else
        {
            //1.copy res to apk
            string srcAaPath = string.Format("{0}/{1}/{2}", Application.dataPath, BuildTool.AssetBundleBuildPath, platform);
            string dstAaPath = string.Format("{0}/{1}/{2}/{3}", path, "src", "main", "assets");
            FileTool.CopyAndReplaceDirectory(srcAaPath, dstAaPath);
        }


        EditorUtility.ClearProgressBar();
    }


    string IUnityLinkerProcessor.GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
    {
        Debug.Log("GenerateAdditionalLinkXmlFile");

        string buildTarget = Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget);
        string LinkXmlPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", Application.dataPath, "../AssetBundle", buildTarget, "aa", "AddressablesLink", "link.xml");
        return LinkXmlPath;
    }

}

