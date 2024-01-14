using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileOperationHelpter 
{


    public static void DeleteDirctory(string dir)
    {
        if (Directory.Exists(dir))
        {
            try
            {
                DeleteFiles(dir);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }


    /// <summary>
    /// 删除文件夹及子文件内文件
    /// </summary>
    /// <param name="str"></param>
    public static void DeleteFiles(string dir)
    {
        if (!Directory.Exists(dir))
            return;

        DirectoryInfo fatherFolder = new DirectoryInfo(dir);
        //删除当前文件夹内文件
        FileInfo[] files = fatherFolder.GetFiles();
        foreach (FileInfo file in files)
        {
            if (file.Attributes.ToString().IndexOf("ReadOnly") != -1)
                file.Attributes = FileAttributes.Normal;
            File.Delete(file.FullName);
        }
        //递归删除子文件夹内文件
        foreach (DirectoryInfo childFolder in fatherFolder.GetDirectories())
        {
            DeleteFiles(childFolder.FullName);
            Directory.Delete(childFolder.FullName);
        }
    }


}
