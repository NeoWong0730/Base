using Lib.AssetLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;

public class PostprocessBuild_Android 
{
    /// <summary>
    /// 构建前置处理-Android
    /// </summary>
    /// <param name="path"></param>
    public static void OnPreprocessBuild_Android(string path)
    {
        Debug.Log("OnPreprocessBuild_Android:"+ path);

        /*        PostProcess.DeleteFileOrDirectory(Application.dataPath + "/Plugins/Android");
                AssetDatabase.Refresh();

                AndroidPostProcess.SetAndroidFile(path);
                if (BuildUtils.IsSelectSDKChannel(SDKChannel.TowerUnion))
                    AndroidPostProcess.SetTowerUnion(path);
                if (BuildUtils.IsSelectSDKOthers(SDKOthers.Bugly))
                    AndroidPostProcess.SetBugly(path);

                AssetDatabase.Refresh();*/

    }


    /// <summary>
    /// 构建后置处理-Android
    /// </summary>
    /// <param name="path"></param>
    public static void OnPostprocessBuild_Android(string path)
    {
        Debug.Log("OnPostprocessBuild_Android" + path);

        BuildSetting mBuildSetting = null;
        string settingPath = string.Format("{0}/{1}", Application.dataPath, "DefaultBuildSetting.asset");
        if (File.Exists(settingPath))
        {
            mBuildSetting = AssetDatabase.LoadAssetAtPath<BuildSetting>("Assets/DefaultBuildSetting.asset");
        }
        if (mBuildSetting.eApkMode == ApkMode.UseSDK)
        {
            CopyExportMainAssetsToSDKAS(mBuildSetting, path);
            GradlewBatBuild(mBuildSetting);
        }
    }


    /// <summary>
    /// 拷贝导出的Android工程（不带sdk）的主要资源，到sdk工程中
    /// </summary>
    /// <param name="mBuildSetting"></param>
    /// <param name="exportPath"></param>
    public static void CopyExportMainAssetsToSDKAS(BuildSetting mBuildSetting, string exportPath)
    {
        Debug.Log("CopyExportMainAssetsToSDKAS: " + exportPath);

        string SdkASProject = Application.dataPath + "/../../Android";

        //1.copy Bundle
        string dstAaPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", SdkASProject, "unityLibrary", "src", "main", "assets", "aa");
        FileTool.CreateFolder(dstAaPath);

        if (mBuildSetting.bUseSplitPack)
        {
            EditorUtility.DisplayProgressBar("Copy Bundle -> SDKProject", "正在拷贝中", 0.1f);

            string dstAaAndroidPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}", SdkASProject, "unityLibrary", "src", "main", "assets", "aa", "Android");
            FileTool.CreateFolder(dstAaAndroidPath);

            #region 分包处理 按照策划分包列表，进行copy

            //copy Bundle
            string platform = Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget);
            string srcBundlePath = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, BuildTool.AssetBundleBuildPath, platform, "aa", "Android");
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
            #endregion
        }
        else
        {
            EditorUtility.DisplayProgressBar("Copy Bundle -> SDKProject", "正在拷贝中", 0.1f);
            string platformName = Enum.GetName(typeof(BuildTarget), mBuildSetting.mBuildTarget);
            string srcAaPath = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, BuildTool.AssetBundleBuildPath, platformName, "aa");
            FileTool.CopyAndReplaceDirectory(srcAaPath, dstAaPath);
        }


        //2.copy Config
        EditorUtility.DisplayProgressBar("Copy Config -> SDKProject", "正在拷贝中", 0.3f);
        string dstConfigPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", SdkASProject, "unityLibrary", "src", "main", "assets", "Config");
        FileTool.DeleteFolder(dstConfigPath);
        string srcConfigPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", exportPath, "unityLibrary", "src", "main", "assets", "Config");
        FileTool.CopyAndReplaceDirectory(srcConfigPath, dstConfigPath);


        //3.copy Dll
        EditorUtility.DisplayProgressBar("Copy Dll -> SDKProject", "正在拷贝中", 0.6f);
        string dstLogicDllPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", SdkASProject, "unityLibrary", "src", "main", "assets", "LogicDll");
        FileTool.DeleteFolder(dstLogicDllPath);
        string srcLogicDllPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", exportPath, "unityLibrary", "src", "main", "assets", "LogicDll");
        FileTool.CopyAndReplaceDirectory(srcLogicDllPath, dstLogicDllPath);


        //4.android copy Bin文件
        EditorUtility.DisplayProgressBar("Copy Bin -> SDKProject ", "正在拷贝中", 0.8f);
        string dstBinPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", SdkASProject, "unityLibrary", "src", "main", "assets", "bin");
        FileTool.DeleteFolder(dstBinPath);
        string srcBinPath = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", exportPath, "unityLibrary", "src", "main", "assets", "bin");
        FileTool.CopyAndReplaceDirectory(srcBinPath, dstBinPath);


        //5.android So库,第三方so库处理 替换jniLibs目录下的v7a,v8a
        EditorUtility.DisplayProgressBar("拷贝到AndroidStudio工程", "正在拷贝中", 1.0f);
        string srcJniLibsPath = string.Format("{0}/{1}/{2}/{3}/{4}", exportPath, "unityLibrary", "src", "main", "jniLibs");
        string dstJniLibsPath = string.Format("{0}/{1}/{2}/{3}/{4}", SdkASProject, "unityLibrary", "src", "main", "jniLibs");
        FileTool.CopyAndReplaceDirectory(srcJniLibsPath, dstJniLibsPath);

        EditorUtility.ClearProgressBar();
    }

  



    //[MenuItem("Tools/SmallTools/GradlewBatBuild")]
    static void GradlewBatBuild(BuildSetting buildSetting)
    {
        int.TryParse(buildSetting.sVersionCode, out int versionCode);
        string versionName = buildSetting.sAppVersion;
        string assetVersion = buildSetting.sAssetVersion;
        string packageName = "ceshi";
        switch (buildSetting.sChannelId)
        {
            case 0:
            case 1:
                packageName = "ceshi";
                break;
            case 2:
                packageName = "test2";
                break;
            case 3:
                packageName = "test4";
                break;
            case 4:
                packageName = "falancheng";
                break;
            case 5:
                packageName = "kuaishou";
                break;
            case 6:
                packageName = "qudao";
                break;
            case 7:
                packageName = "kuaishou-noEncrypt";
                break;
            case 8:
                packageName = "kuaishou-QRcode";
                break;
        }

        string path = Application.dataPath + "/../../Android/launcher/release";
        try
        {
            string instructorParams = string.Format("clean assembleRelease -PVERSION_CODE={0} -PVERSION_NAME={1} -PVERSION_CHANNEL={2} -PASSET_VERSION={3}", versionCode, versionName, packageName, assetVersion);
            Debug.Log(instructorParams);
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.WorkingDirectory = Application.dataPath + "/../../Android/"; //@"D:\Projects\crossgate_android\client_as\";
            proc.StartInfo.FileName = "gradlew.bat";
            proc.StartInfo.Arguments = instructorParams;

            //关闭Shell的使用 
            //proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.CreateNoWindow = true;
            //proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;//这里设置DOS窗口不显示，经实践可行
            proc.Start();
            proc.WaitForExit();
            proc.Close();
            proc.Dispose();

            //explorer.exe是 Windows 程序管理器或者文件资源管理器
            proc = System.Diagnostics.Process.Start("explorer.exe", path);
            proc.WaitForExit();
            proc.Close();
            proc.Dispose();
        }
        catch (System.Exception ex)
        {
            Debug.LogErrorFormat("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
        }


        Debug.Log("打包apk 结束！！！");
    }

}
