using System;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;




public class PostProcess : IPreprocessBuildWithReport, IPostprocessBuildWithReport , IPostGenerateGradleAndroidProject, IUnityLinkerProcessor
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

        string buildTarget = Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget);

        //1.copy res to apk
        string srcAaPath = string.Format("{0}/{1}/{2}", Application.dataPath, BuildTool.AssetBundleBuildPath, buildTarget);
        string dstAaPath = string.Format("{0}/{1}/{2}/{3}", path, "src", "main", "assets");
        FileTool.CopyAndReplaceDirectory(srcAaPath, dstAaPath);

        EditorUtility.ClearProgressBar();
    }


    string IUnityLinkerProcessor.GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
    {
        Debug.Log("GenerateAdditionalLinkXmlFile");

        string buildTarget = Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget);
        string LinkXmlPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", Application.dataPath, "../AssetBundle", buildTarget, "aa", "AddressablesLink", "link.xml");
        return LinkXmlPath;
    }

    void IUnityLinkerProcessor.OnBeforeRun(BuildReport report, UnityLinkerBuildPipelineData data)
    {
        Debug.Log("OnBeforeRun");
    }

    void IUnityLinkerProcessor.OnAfterRun(BuildReport report, UnityLinkerBuildPipelineData data)
    {
        Debug.Log("OnAfterRun");
    }

}

