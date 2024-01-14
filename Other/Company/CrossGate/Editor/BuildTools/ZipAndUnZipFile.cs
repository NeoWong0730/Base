using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ZipAndUnZipFile
{
    #region 加压
    /// <summary>
    /// 压缩文件（不压缩文件夹）
    /// </summary>
    public static bool ZipFile(string dirPath, string zipFilePath, out string err)
    {
        err = "";
        if (dirPath == string.Empty)
        {
            err = "要压缩的文件夹不能为空！";
            return false;
        }
        if (!Directory.Exists(dirPath))
        {
            err = "要压缩的文件夹不存在！";
            return false;
        }
        //压缩文件名为空时使用文件夹名＋.zip
        if (string.IsNullOrEmpty(zipFilePath))
        {
            if (dirPath.EndsWith("//"))
            {
                dirPath = dirPath.Substring(0, dirPath.Length - 1);
            }
            zipFilePath = dirPath + ".zip";
        }
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }
        try
        {
            string[] filenames = Directory.GetFiles(dirPath);
            using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
            {
                s.SetLevel(9);
                byte[] buffer = new byte[4096];
                foreach (string file in filenames)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                    entry.DateTime = System.DateTime.Now;
                    s.PutNextEntry(entry);
                    using (FileStream fs = File.OpenRead(file))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            s.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                s.Finish();
                s.Close();
            }
        }
        catch (System.Exception ex)
        {
            err = ex.Message;
            return false;
        }
        return true;
    }
    #endregion

    #region 解压
    /// <summary>
    /// zip格式的文件。
    /// </summary>
    public static bool UnZipFile(string zipFilePath, string unZipDir, out string err)
    {
        err = "";
        if (zipFilePath == string.Empty)
        {
            err = "压缩文件不能为空！";
            return false;
        }
        if (!File.Exists(zipFilePath))
        {
            err = "压缩文件不存在！";
            return false;
        }
        if (string.IsNullOrEmpty(unZipDir))
            unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
        if (!unZipDir.EndsWith("//"))
            unZipDir += "//";
        if (!Directory.Exists(unZipDir))
            Directory.CreateDirectory(unZipDir);

        try
        {
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(unZipDir + directoryName);
                    }
                    if (!directoryName.EndsWith("//"))
                        directoryName += "//";
                    if (fileName != System.String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                    streamWriter.Write(data, 0, size);
                                else
                                    break;
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            err = ex.Message;
            return false;
        }
        return true;
    }
    #endregion
}

