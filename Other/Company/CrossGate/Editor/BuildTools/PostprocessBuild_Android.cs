using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

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
            CopyExportMainAssetsToSDKAS(path);
            GradlewBatBuild(mBuildSetting);
        }
    }


    public static void CopyExportMainAssetsToSDKAS(string exportPath)
    {
        Debug.Log("CopyExportMainAssetsToSDKAS: " + exportPath);

        string SdkASProject = Application.dataPath + "/../../Android";

        //1.copy Bundle + Config + Dll + bin
        EditorUtility.DisplayProgressBar("Copy Bundle + Config + Dll + bin -> SDKProject", "正在拷贝中", 0.1f);
        string dstAssetPath = string.Format("{0}/{1}/{2}/{3}/{4}", SdkASProject, "unityLibrary", "src", "main", "assets");
        FileTool.DeleteFolder(dstAssetPath);
        string srcAssetPath = string.Format("{0}/{1}/{2}/{3}/{4}", exportPath, "unityLibrary", "src", "main", "assets");
        FileTool.CopyAndReplaceDirectory(srcAssetPath, dstAssetPath);


        //第三方的so库 GME语音的 MTP的已经放入AS工程

        //替换jniLibs目录下的v7a,v8a 应该是只需要替换 libil2cpp.so + lib_burst_generated.so + libunity.so
        EditorUtility.DisplayProgressBar("Copy Bundle -> SDKProject", "正在拷贝中", 0.5f);
        string srcJniLibsPath = string.Format("{0}/{1}/{2}/{3}/{4}", exportPath, "unityLibrary", "src", "main", "jniLibs");
        string dstJniLibsPath = string.Format("{0}/{1}/{2}/{3}/{4}", SdkASProject, "unityLibrary", "src", "main", "jniLibs");
        FileTool.CopyAndReplaceDirectory(srcJniLibsPath, dstJniLibsPath);

        EditorUtility.ClearProgressBar();

    }

    private static void QuDaoPackageLibsMove(bool needMove)
    {
        string libPath = string.Format("{0}/../../Android/{1}/{2}", Application.dataPath, "unityLibrary", "libs");
        string copyPath = string.Format("{0}/../../Android/tempFile", Application.dataPath);

        if (needMove)
        {
            if (!Directory.Exists(copyPath))
                Directory.CreateDirectory(copyPath);

            List<string> tempArr = new List<string>();
            //渠道包 基础sdk 需要删除以下lib库中的文件 
            tempArr.Add("/alipaySdk-15.8.01.20210112203525.jar");
            tempArr.Add("/basewx-2.1.7.aar");
            tempArr.Add("/kwaisdk-2.1.7.aar");
            tempArr.Add("/kwaisdk_qq-2.1.7.aar");
            tempArr.Add("/kwaisdk_wechat-2.1.7.aar");
            tempArr.Add("/open_sdk_lite-0.0.2.jar");
            tempArr.Add("/wechat-sdk-android-without-mta-6.7.9.jar");

            //移动文件
            foreach (var item in tempArr)
            {
                File.Copy(libPath + item, copyPath + item, true);//重写
            }

            foreach (var item in tempArr)
            {
                File.Delete(libPath + item);
            }
        }
        else
        {
            if (Directory.Exists(copyPath))
            {
                FileTool.CopyAndReplaceDirectory(copyPath, libPath);
                Directory.Delete(copyPath, true);
                AssetDatabase.Refresh();
            }
        }
    }



    //[MenuItem("Tools/SmallTools/GradlewBatBuild")]
    static void GradlewBatBuild(BuildSetting buildSetting)
    {
        int.TryParse(buildSetting.sVersionCode, out int versionCode);
        string versionName = buildSetting.sAppVersion;
        string assetVersion = buildSetting.sVersion;
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
                QuDaoPackageLibsMove(true);
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



        if (packageName.Equals("qudao"))
        {
            QuDaoPackageLibsMove(false);
        }
        Debug.Log("打包apk 结束！！！");
    }

}
