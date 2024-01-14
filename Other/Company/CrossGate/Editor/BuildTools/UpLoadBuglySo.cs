using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class UpLoadBuglySo
{
    private static string[] soFolders = { "armeabi-v7a" };//"arm64-v8a", "armeabi-v7a", "x86" };
    private static string symbolFile = "symbol";

    //没有libil2cpp.so.debug 文件，不知道怎样产生的！！！！  libil2cpp.so.debug

    private static string soCmdTemplate1 =
        "java -jar buglySymbolAndroid.jar -i #SOPATH#/libil2cpp.so -u -id #ID# -key #KEY# -package #PACKAGE# -version #VERSION#";


    private static string soCmdTemplate =
       "java -jar buglySymbolAndroid.jar -i #SOPATH#/libil2cpp.so -o #OUTPATH# -symbol -u -id #ID# -key #KEY# -package #PACKAGE# -version #VERSION#";


    /// <summary>
    /// 打完包后调用此方法 自动上传符号表文件
    /// </summary>
    [MenuItem("__Tools__/打包相关/自动上传符号表")]
    public static void UploadBuglyso()
    {
        //DeleteSo();
        //CopyBuglySo();
        if (EditCommand(out string arg))
        {
            Debug.Log("EditCommand: " + arg);
            RunCmd(arg, BuglyToolPath());
        }
    }


    private static void CopyBuglySo()
    {
        FileTool.CopyFolder(DeBugSOPath(), BuglyToolPath());
        FileTool.OpenFolder(BuglyToolPath());
    }


    private static void DeleteSo()
    {
        foreach (var folder in soFolders)
        {
            FileTool.DeleteFolder(BuglyToolPath() + "/" + folder);
        }
    }

    private static bool EditCommand(out string arg)
    {
        bool exists = false;
        StringBuilder sb = new StringBuilder();
        foreach (var folder in soFolders)
        {
            string soPath = BuglyToolPath() + "/" + folder;
            string outPath = BuglyToolPath() + "/" + symbolFile;
            if (FileTool.CheckFolder(soPath))
            {
                exists = true;
                sb.Append(soCmdTemplate).Append(" & ");
                sb.Replace("#SOPATH#", soPath);
                sb.Replace("#OUTPATH#", outPath);
                sb.Replace("#ID#", "f2a1cc0a3c");  //这里注意一下要改成自己的id
                sb.Replace("#KEY#", "b8b5e2dc-2fc5-4d17-8a20-184b0b4462e2"); //自己的key
                sb.Replace("#PACKAGE#", Application.identifier);
                sb.Replace("#VERSION#", Application.version);
            }
        }

        sb.Append("exit").Replace("/", @"\");
        arg = sb.ToString();
        return exists;
    }

    public static string DeBugSOPath()
    {
        return Path.GetFullPath(Application.dataPath + "/../Temp/StagingArea/symbols");
    }

    public static string BuglyToolPath()
    {
        return Path.GetFullPath(Application.dataPath + "/../buglytools");
    }
    public static void RunCmd(string arg, string workingDirectory, string exe = "cmd.exe")
    {

        ProcessStartInfo info = new ProcessStartInfo(exe);
        info.Arguments = "/c " + arg;
        info.WorkingDirectory = workingDirectory;
        info.CreateNoWindow = false;
        info.ErrorDialog = true;
        info.UseShellExecute = true;

        if (info.UseShellExecute)
        {
            info.RedirectStandardOutput = false;
            info.RedirectStandardError = false;
            info.RedirectStandardInput = false;
        }
        else
        {
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }
        Debug.Log("RunCmd: " + info.Arguments);

        Process process = Process.Start(info);

        if (!info.UseShellExecute)
        {
            Debug.Log(process.StandardOutput);
            Debug.Log(process.StandardError);
        }

        process.WaitForExit();
        process.Close();
    }
}