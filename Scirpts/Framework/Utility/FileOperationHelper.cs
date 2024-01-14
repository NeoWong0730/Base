using System;
using System.IO;
using UnityEngine;
using Lib;

public static class FileOperationHelper
{
    public static void DeleteDirectory(string dir)
    {
        if (Directory.Exists(dir))
        {
            try
            {
                DeleteFiles(dir);
            }
            catch (Exception e)
            {
                DebugUtil.LogException(e);
            }
        }
    }

    public static void DeleteFiles(string dir)
    {
        if (!Directory.Exists(dir))
            return;

        DirectoryInfo fatherFolder = new DirectoryInfo(dir);
        FileInfo[] files = fatherFolder.GetFiles();
        foreach (FileInfo file in files)
        {
            if (file.Attributes.ToString().IndexOf("ReadOnly") != -1)
                file.Attributes = FileAttributes.Normal;
            File.Delete(file.FullName);
        }

        foreach (DirectoryInfo childFolder in fatherFolder.GetDirectories())
        {
            DeleteFiles(childFolder.FullName);
            Directory.Delete(childFolder.FullName);
        }
    }
}
