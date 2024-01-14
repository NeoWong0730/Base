using Lib.AssetLoader;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PostprocessBuild_PC 
{

    /// <summary>
    /// 构建后置处理-PC
    /// </summary>
    /// <param name="path"></param>
    public static void OnPostprocessBuild_PC(string path)
    {
        string dataFileName = string.Format("{0}_Data", Path.GetFileNameWithoutExtension(path)); 
        string dirPath =string.Format("{0}/{1}/{2}", Path.GetDirectoryName(path), dataFileName, "StreamingAssets");
        string srcAaPath = string.Format("{0}/{1}/{2}", Application.dataPath,  BuildTool.AssetBundleBuildPath, System.Enum.GetName(typeof(BuildTarget), EditorUserBuildSettings.activeBuildTarget));
        FileTool.CopyAndReplaceDirectory(srcAaPath, dirPath);

        BuildSetting mBuildSetting = null;
        string settingPath = string.Format("{0}/{1}", Application.dataPath, "DefaultBuildSetting.asset");
        if (File.Exists(settingPath))
        {
            mBuildSetting = AssetDatabase.LoadAssetAtPath<BuildSetting>("Assets/DefaultBuildSetting.asset");
        }

        if (mBuildSetting.eApkMode == ApkMode.UseSDK)
        {
            //sdk导入到streaming
            string pcSDkPath = string.Format("{0}/KwaiSDKUI", Application.dataPath);
            if (Directory.Exists(pcSDkPath))
            {
                FileTool.CopyAndReplaceDirectory(pcSDkPath, dirPath);
            }
        }

#if UNITY_STANDALONE_WIN && UNITY_EDITOR

        if (mBuildSetting.eApkMode == ApkMode.NoSDK)
            return;
        if (mBuildSetting.eUseACEAnti==ApkProtectModel.NoProtect)
            return;
        Debug.Log("OnPostprocessBuild_PC: " + path);
        string dirBuildPath = Path.GetDirectoryName(path);
        string autoProtectPath = "/../PCAutoProtectClient";
        string AntiCheatExpert= string.Format("{0}/{1}/AceAnti", Application.dataPath, autoProtectPath);
        //ACE相关的防护组件
        FileTool.CopyAndReplaceDirectory(AntiCheatExpert, dirBuildPath);

        string inputElementPath = string.Format("{0}/{1}/{2}", Application.dataPath, autoProtectPath, "InputElement.txt");
        string inputPath= string.Format("{0}/{1}/{2}", Application.dataPath, autoProtectPath, "input");
        string unZipFilePath = string.Format("{0}/{1}/{2}", Application.dataPath, autoProtectPath, "output.zip");
        if (!Directory.Exists(inputPath))
        {
            Directory.CreateDirectory(inputPath);
        }
        if (File.Exists(unZipFilePath))
        {
            File.Delete(unZipFilePath);
        }
        if (!File.Exists(inputElementPath))
        {
            Debug.LogError("没有对应的元素文件：InputElement.txt");
            return;
        }

        string[] lines = File.ReadAllLines(inputElementPath);
        for (int i = 0; i < lines.Length; i++)
        {
            string filePath = lines[i];
            var srcPath = Path.Combine(dirBuildPath, filePath);
            string fileName = Path.GetFileName(srcPath);
            var desPath = Path.Combine(inputPath, fileName);
            File.Copy(srcPath, desPath, true);
        }

        //压缩
        bool zip = ZipAndUnZipFile.ZipFile(inputPath, null, out string errorMsg);
        if (!zip)
        {
            Debug.LogError(errorMsg);
            return;
        }
        System.Diagnostics.Process processcmd = null;
        //加固
        if (zip)
        {
            string cmdInputPath= string.Format("{0}/{1}/{2}", Application.dataPath, autoProtectPath, "cmd.txt");
            string cmdline = null;
            if (File.Exists(cmdInputPath))
            {
                string[] cmd = File.ReadAllLines(cmdInputPath);
                if(cmd.Length>0)
                    cmdline = cmd[0];
            }
            if (string.IsNullOrEmpty(cmdline))
                return;
            processcmd = System.Diagnostics.Process.Start("cmd.exe", "/K " + cmdline);
            processcmd.WaitForExit();
            processcmd.Close();
            processcmd.Dispose();           
        }

        //解压
        bool unZip = ZipAndUnZipFile.UnZipFile(unZipFilePath, dirBuildPath, out errorMsg);
        if (!unZip)
        {
            Debug.LogError(errorMsg);
            return;
        }
        if (processcmd != null)
            processcmd.Kill();
#endif
    }


}
