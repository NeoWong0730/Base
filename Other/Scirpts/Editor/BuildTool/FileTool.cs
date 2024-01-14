
using Lib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static BuildTool;
using Debug = UnityEngine.Debug;
using Framework;

public class FileTool
{
    public static bool CheckFolder(string path)
    {
        if (Directory.Exists(path))
        {
            return true;
        }
        UnityEditor.EditorUtility.DisplayDialog("Error", "Path does not exist \n\t" + path, "确认");
        return false;
    }
    public static void OpenFolder(string path)
    {
        if (CheckFolder(path))
        {
            System.Diagnostics.Process.Start(path);
        }

    }

    public static void CopyFolder(Dictionary<string, string> copyDic)
    {
        foreach (KeyValuePair<string, string> path in copyDic)
        {

            if (CheckFolder(path.Key))
            {

                CopyDir(path.Key, path.Value);
                Debug.Log("Copy Success : \n\tFrom:" + path.Key + " \n\tTo:" + path.Value);
            }
        }
        EditorUtility.ClearProgressBar();
    }

    public static void CopyFolder(string fromPath, string toPath)
    {
        CopyDir(fromPath, toPath);
        Debug.Log("Copy Success : \n\tFrom:" + fromPath + " \n\tTo:" + toPath);
        EditorUtility.ClearProgressBar();
    }

    public static void CreateFolder(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        Directory.CreateDirectory(path);
    }

    public static void CreateFolder(string path, bool delete)
    {
        if (delete)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
        }
        else
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }

    public static void DeleteFolder(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    private static void CopyDir(string origin, string target)
    {
#if UNITY_IOS
      
        if (!origin.EndsWith("/"))
        {
            origin += "/";
        }
 
        if (!target.EndsWith("/"))
        {
            target += "/";
        }
#else
        if (!origin.EndsWith("\\", System.StringComparison.Ordinal))
        {
            origin += "\\";
        }

        if (!target.EndsWith("\\", System.StringComparison.Ordinal))
        {
            target += "\\";
        }

#endif
        if (!Directory.Exists(target))
        {
            Directory.CreateDirectory(target);
        }

        DirectoryInfo info = new DirectoryInfo(origin);
        FileInfo[] fileList = info.GetFiles();
        DirectoryInfo[] dirList = info.GetDirectories();
        float index = 0;
        foreach (FileInfo fi in fileList)
        {


            if (fi.Extension == ".zip" || fi.Extension == ".meta" || fi.Extension == ".rar")
            {
                Debug.Log("dont copy :" + fi.FullName);
                continue;
            }
            float progress = (index / (float)fileList.Length);
            EditorUtility.DisplayProgressBar("Copy ", "Copying: " + Path.GetFileName(fi.FullName), progress);
            File.Copy(fi.FullName, target + fi.Name, true);
            index++;
        }

        foreach (DirectoryInfo di in dirList)
        {
            if (di.FullName.Contains(".svn"))
            {
                Debug.Log("Continue SVN " + di.FullName);
                continue;
            }

            CopyDir(di.FullName, target + "\\" + di.Name);
        }
    }

    public static void Copy(string src, string dst, bool overwrite)
    {        
        string title = "拷贝:" + src + " -> " + dst;
        if (EditorUtility.DisplayCancelableProgressBar(title, src + " -> " + dst, 0))
        {
            return;
        }

        FileInfo fileInfo = new FileInfo(src);
        if((fileInfo.Attributes & FileAttributes.Directory) != 0)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(src);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

            src = fileInfo.FullName;
            dst = Path.GetFullPath(dst);

            for (int i = 0; i < fileInfos.Length; ++i)
            {
                string srcPath = fileInfos[i].FullName;
                string dstPath = srcPath.Replace(src, dst);

                if (EditorUtility.DisplayCancelableProgressBar(title, srcPath + " -> " + dstPath, (float)i / fileInfos.Length))
                {
                    break;
                }

                string dir = Path.GetDirectoryName(dstPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.Copy(srcPath, dstPath, overwrite);
            }
        }
        else
        {
            if (fileInfo.Exists)
            {
                string dir = Path.GetDirectoryName(dst);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.Copy(src, dst, overwrite);
            }
        }
        EditorUtility.ClearProgressBar();
    }



    /// <summary>
    /// 拷贝文件
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="destinationPath"></param>
    public static void CopyAndReplaceDirectory(string sourcePath, string destinationPath)
    {
        if (!Directory.Exists(destinationPath))
            Directory.CreateDirectory(destinationPath);

        DirectoryInfo info = new DirectoryInfo(sourcePath);

        foreach (var file in info.GetFileSystemInfos())
        {
            string filePath = Path.Combine(destinationPath, file.Name);
            if (file is System.IO.FileInfo)
                File.Copy(file.FullName, filePath, true);//重写
            else
                CopyAndReplaceDirectory(file.FullName, filePath);
        }
    }


    public static void DirectoryOperate(string path, bool delete = true, bool create = true)
    {
        try
        {
            if (delete && Directory.Exists(path))
                Directory.Delete(path, true);
            if (create && !Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public static void EncryptAsset(string src, string des, string password = null)
    {
        byte[] data = File.ReadAllBytes(src);
        string dir = Path.GetDirectoryName(des);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        Unity.SharpZipLib.Checksum.Crc32 crc32 = new Unity.SharpZipLib.Checksum.Crc32();
        crc32.Reset();
        crc32.Update(data);

        using (FileStream fileStream = File.Create(des))
        using (Unity.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new Unity.SharpZipLib.Zip.ZipOutputStream(fileStream))
        {
            zipOutputStream.SetLevel(6);

            if (!string.IsNullOrWhiteSpace(password))
            {
                zipOutputStream.Password = password;
            }

            Unity.SharpZipLib.Zip.ZipEntry zipEntry = new Unity.SharpZipLib.Zip.ZipEntry(Path.GetFileName(des));

            zipEntry.DateTime = System.DateTime.MinValue;//DateTime.Now;
            zipEntry.Size = data.Length;
            zipEntry.Crc = crc32.Value;

            zipOutputStream.PutNextEntry(zipEntry);
            zipOutputStream.Write(data, 0, data.Length);
            zipOutputStream.CloseEntry();
        }
    }

    public static int EncryptAsset_0(string srcDir, string desDir, string title, bool clearOld, List<string> ignore = null)
    {
        srcDir = Path.GetFullPath(srcDir);
        desDir = Path.GetFullPath(desDir);
        int l = srcDir.Length;
        int rlt = 0;

        if (!Directory.Exists(srcDir))
        {
            Debug.LogErrorFormat("不存在源目录 {0}", srcDir);
            return 2;
        }

        if (clearOld && Directory.Exists(desDir))
        {
            Directory.Delete(desDir, true);
        }

        if (!Directory.Exists(desDir))
        {
            Directory.CreateDirectory(desDir);
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(srcDir);
        FileInfo[] fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

        for (int i = 0; i < fileInfos.Length; ++i)
        {
            FileInfo fileInfo = fileInfos[i];
            if (ignore.Contains(fileInfo.Extension))
            {
                continue;
            }

            string srcPath = fileInfo.FullName.Replace("\\", "/");
            if (EditorUtility.DisplayCancelableProgressBar(title, srcPath, (float)i / fileInfos.Length))
            {
                rlt = 1;
                break;
            }
            string desPath = desDir + srcPath.Remove(0, l);

            //（Config/Video）目录下的资源不加密
            string videoFile = "Assets/Config/Video/";
            if (srcPath.Contains(videoFile))
            {
                string dir = Path.GetDirectoryName(desPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.Copy(srcPath, desPath);
            }
            else
            {
                EncryptAsset(srcPath, desPath);
            }
        }
        EditorUtility.ClearProgressBar();
        return rlt;
    }


    public static int CopyAsset(string srcDir, string desDir, string title, bool clearOld, List<string> ignore = null)
    {
        srcDir = Path.GetFullPath(srcDir);
        desDir = Path.GetFullPath(desDir);
        int l = srcDir.Length;
        int rlt = 0;

        if (!Directory.Exists(srcDir))
        {
            Debug.LogErrorFormat("不存在源目录 {0}", srcDir);
            return 2;
        }

        if (clearOld && Directory.Exists(desDir))
        {
            Directory.Delete(desDir, true);
        }

        if (!Directory.Exists(desDir))
        {
            Directory.CreateDirectory(desDir);
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(srcDir);
        DirectoryInfo[] infos = directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories);

        List<DirectoryInfo> directoryInfos = new List<DirectoryInfo>(infos.Length + 1);
        directoryInfos.Add(directoryInfo);
        directoryInfos.AddRange(infos);

        for (int dirIndex = 0; dirIndex < directoryInfos.Count; ++dirIndex)
        {
            FileInfo[] fileInfos = directoryInfos[dirIndex].GetFiles("*.*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < fileInfos.Length; ++i)
            {
                FileInfo fileInfo = fileInfos[i];
                if (ignore != null && ignore.Contains(fileInfo.Extension))
                {
                    continue;
                }

                string srcPath = fileInfo.FullName;
                if (EditorUtility.DisplayCancelableProgressBar(title, srcPath, (float)i / fileInfos.Length))
                {
                    rlt = 1;
                    break;
                }
                string desPath = desDir + srcPath.Remove(0, l);

                string dir = Path.GetDirectoryName(desPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.Copy(srcPath, desPath);
            }

            if (rlt != 0)
            {
                break;
            }
        }
        EditorUtility.ClearProgressBar();
        return rlt;
    }

    public enum ECopyAssetMode : int
    {
        eCopyAB = 1,
        eCopyConfig = 2,
        eCopyDll = 3,
        eCopyABToAssetBundleBuildPath = 4,
        eCopyCatalog = 5,
        eCopyBundle = 6
    }
    public static int CopyAssetBundleToStream(BuildTarget target, ECopyAssetMode eCopyAssetMode)
    {
        int rlt = 0;
        string platformName = Enum.GetName(typeof(BuildTarget), target);

        switch (eCopyAssetMode)
        {
            case ECopyAssetMode.eCopyAB:
                rlt = CopyAsset(Addressables.BuildPath, Addressables.PlayerBuildDataPath, "拷贝Library aa  -> StreamingAssets/aa", true, null);
                break;
            case ECopyAssetMode.eCopyConfig:
                string assetCsvPublishDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAssetCsvDir);
                string streamingCsvDir = string.Format("{0}/{1}", Application.streamingAssetsPath, AssetPath.sAssetCsvDir);
                rlt = CopyAsset(assetCsvPublishDir, streamingCsvDir, "拷贝Config -> StreamingAssets/config", true, new List<string> { ".manifest" });
                break;
            case ECopyAssetMode.eCopyDll:
                string assetLogicPublishDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAssetLogicDir);
                string streamingLogicDir = string.Format("{0}/{1}", Application.streamingAssetsPath, AssetPath.sAssetLogicDir);
                rlt = CopyAsset(assetLogicPublishDir, streamingLogicDir, "拷贝Logic -> StreamingAssets/logic", true, new List<string> { ".manifest", ".dll" });
                break;
            case ECopyAssetMode.eCopyABToAssetBundleBuildPath:
                string tempDesABDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAddressableDir);
                rlt = CopyAsset(Addressables.BuildPath, tempDesABDir, "拷贝Library aa  -> AssetBundle/aa", true, null);
                break;
            case ECopyAssetMode.eCopyCatalog:
                rlt = CopyAsset(Addressables.BuildPath + "/AddressablesLink", Addressables.PlayerBuildDataPath + "/AddressablesLink", "拷贝Library aa  -> StreamingAsset/aa", true, null);
                File.Copy(Addressables.BuildPath + "/catalog.json", Addressables.PlayerBuildDataPath + "/catalog.json", true);
                File.Copy(Addressables.BuildPath + "/settings.json", Addressables.PlayerBuildDataPath + "/settings.json", true);
                break;
            case ECopyAssetMode.eCopyBundle:
                string assetBundlePublishDir = string.Format("{0}/{1}/{2}/{3}/{4}", Application.dataPath, AssetBundleBuildPath, platformName, AssetPath.sAddressableDir, platformName);
                string streamingBundleDir = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, AssetPath.sAddressableDir, platformName);
                rlt = CopyAsset(assetBundlePublishDir, streamingBundleDir, "拷贝Bundle -> StreamingAssets/aa/Android", true, new List<string> { ".manifest" });
                break;
        }

        AssetDatabase.Refresh();
        return rlt;
    }
}
