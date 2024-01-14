using Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Framework;

public class Build_AssetBundle 
{
    #region 路径获取
    static string hotfixBinaryFile;
    public static String HotFixBinaryFile
    {
        get
        {
            if (String.IsNullOrEmpty(hotfixBinaryFile))
                hotfixBinaryFile = String.Format("{0}/{1}", Application.dataPath, BuildTool.HotFixBinaryFilePath);
            return hotfixBinaryFile;
        }
    }
    #endregion

    public static int GenAssetBundle(BuildSetting buildSetting, VersionSetting versionSetting)
    {
        int rlt = 0;
        string version = buildSetting.sAssetVersion;
        string minimalVersion = VersionHelper.GetMinimalVersion(version);

        int curVersion = 0;
        int.TryParse(minimalVersion, out curVersion);

        //打首包(资源版本<=0)
        if (curVersion <= 0)
        {
            AddressableBundleGroupSetting.SetAddressableAssetSettings(version);

            AddressableBundleGroupSetting.CleanAddressableCache(buildSetting.bClearCache);
            
            rlt = AddressableBundleGroupSetting.GenLocalAsset_Addressable(buildSetting);
            if (rlt == 0)
                rlt = ReadLolcalHash(versionSetting);
        }
        else
        {
            AddressableBundleGroupSetting.GenHotFixAsset_Addressable(buildSetting);
        }

       
        string tempDesABDir = $"{Directory.GetParent(Application.dataPath)}/AssetBundle/{buildSetting.mBuildTarget}/{AssetPath.sAddressableDir}";
        rlt = FileTool.CopyAsset(Addressables.BuildPath, tempDesABDir, "拷贝Library aa  -> AssetBundle/aa", true, null);

        return rlt;
    }


    public static int ReadLolcalHash(VersionSetting versionSetting)
    {
        //保存本地的hash，设置同一版本的热更系列(通过第一次构建远端目录拿hash)
        int result = 0;
        string localHashPath = HotFixBinaryFile + "/LolcalHash.txt";
        string lolcalHash;
        if (File.Exists(localHashPath))
        {
            lolcalHash = File.ReadAllText(localHashPath);
            if (string.IsNullOrEmpty(lolcalHash))
            {
                Debug.LogError("记录首包hash为空，因为bundle未正常生成");
                return -5;
            }
            else
            {
                //设置包的唯一热更标识符
                string HotFixUniqueIdentifierPath = string.Format("{0}/{1}", HotFixBinaryFile, "HotFixUniqueIdentifier.txt");
                File.WriteAllText(HotFixUniqueIdentifierPath, lolcalHash);

                versionSetting.HotFixUniqueIdentifier = lolcalHash;
                EditorUtility.SetDirty(versionSetting);
            }
        }
        else
        {
            Debug.LogError("生成文件LolcalHash.txt失败，因为bundle未正常生成");
            return -5;
        }
        return result;
    }
}
