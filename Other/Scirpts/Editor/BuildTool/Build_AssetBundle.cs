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
    #region ·����ȡ
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

        //���װ�(��Դ�汾<=0)
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
        rlt = FileTool.CopyAsset(Addressables.BuildPath, tempDesABDir, "����Library aa  -> AssetBundle/aa", true, null);

        return rlt;
    }


    public static int ReadLolcalHash(VersionSetting versionSetting)
    {
        //���汾�ص�hash������ͬһ�汾���ȸ�ϵ��(ͨ����һ�ι���Զ��Ŀ¼��hash)
        int result = 0;
        string localHashPath = HotFixBinaryFile + "/LolcalHash.txt";
        string lolcalHash;
        if (File.Exists(localHashPath))
        {
            lolcalHash = File.ReadAllText(localHashPath);
            if (string.IsNullOrEmpty(lolcalHash))
            {
                Debug.LogError("��¼�װ�hashΪ�գ���Ϊbundleδ��������");
                return -5;
            }
            else
            {
                //���ð���Ψһ�ȸ���ʶ��
                string HotFixUniqueIdentifierPath = string.Format("{0}/{1}", HotFixBinaryFile, "HotFixUniqueIdentifier.txt");
                File.WriteAllText(HotFixUniqueIdentifierPath, lolcalHash);

                versionSetting.HotFixUniqueIdentifier = lolcalHash;
                EditorUtility.SetDirty(versionSetting);
            }
        }
        else
        {
            Debug.LogError("�����ļ�LolcalHash.txtʧ�ܣ���Ϊbundleδ��������");
            return -5;
        }
        return result;
    }
}
