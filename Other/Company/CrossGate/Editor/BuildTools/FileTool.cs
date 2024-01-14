
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;


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

}
