using Lib.AssetLoader;
using Lib.Core;
using System.IO;
using UnityEditor;
using UnityEngine;

//#if ILRUNTIME_MODE
//[InitializeOnLoad]--暂时先注释
public class Startup
{
    private const string ScriptAssembliesDir = "Library/ScriptAssemblies";

    static Startup()
    {
        string CodeDir = string.Format("Assets/{0}", AssetPath.sAssetLogicDir);
        if (Directory.Exists(CodeDir))
            Directory.Delete(CodeDir, true);
        Directory.CreateDirectory(CodeDir);

        File.Copy(Path.Combine(ScriptAssembliesDir, AssetPath.sLogicDllName), Path.Combine(CodeDir, AssetPath.sLogicDllName + ".bytes"), true);
        File.Copy(Path.Combine(ScriptAssembliesDir, AssetPath.sLogicPdbName), Path.Combine(CodeDir, AssetPath.sLogicPdbName + ".bytes"), true);
        //DebugUtil.Log(ELogType.eNone, "copy dll pdb success");
        AssetDatabase.Refresh();
    }


    public static void CopyDllFromLibraryAssembly()
    {
        string CodeDir = string.Format("Assets/{0}", AssetPath.sAssetLogicDir);
        if (Directory.Exists(CodeDir))
            Directory.Delete(CodeDir, true);
        Directory.CreateDirectory(CodeDir);

        File.Copy(Path.Combine(ScriptAssembliesDir, AssetPath.sLogicDllName), Path.Combine(CodeDir, AssetPath.sLogicDllName + ".bytes"), true);
        File.Copy(Path.Combine(ScriptAssembliesDir, AssetPath.sLogicPdbName), Path.Combine(CodeDir, AssetPath.sLogicPdbName + ".bytes"), true);
        //DebugUtil.Log(ELogType.eNone, "copy dll pdb success");
        AssetDatabase.Refresh();
    }

    public static void CopyDllFromLogicDll(string platformName)
    {
        string LogicDllDir = string.Format("{0}/{1}/{2}/{3}", Application.dataPath, BuildTool.AssetBundleBuildPath, platformName, AssetPath.sAssetLogicDir);
        string CodeDir = string.Format("Assets/{0}", AssetPath.sAssetLogicDir);
        if (Directory.Exists(CodeDir))
            Directory.Delete(CodeDir, true);
        Directory.CreateDirectory(CodeDir);

        File.Copy(Path.Combine(LogicDllDir, AssetPath.sLogicDllName), Path.Combine(CodeDir, AssetPath.sLogicDllName + ".bytes"), true);
        File.Copy(Path.Combine(LogicDllDir, AssetPath.sLogicPdbName), Path.Combine(CodeDir, AssetPath.sLogicPdbName + ".bytes"), true);
        DebugUtil.Log(ELogType.eNone, "copy dll pdb success");
        AssetDatabase.Refresh();
    }


    public static void CopyDllFromAddressableBuild(string TempOutputFolder, string platformName)
    {
        string CodeDir = string.Format("Assets/{0}", AssetPath.sAssetLogicDir);
        if (Directory.Exists(CodeDir))
            Directory.Delete(CodeDir, true);
        Directory.CreateDirectory(CodeDir);

        File.Copy(Path.Combine(TempOutputFolder, AssetPath.sLogicDllName), Path.Combine(CodeDir, AssetPath.sLogicDllName + ".bytes"), true);
        File.Copy(Path.Combine(TempOutputFolder, AssetPath.sLogicPdbName), Path.Combine(CodeDir, AssetPath.sLogicPdbName + ".bytes"), true);
        DebugUtil.Log(ELogType.eNone, "copy dll pdb success");
        AssetDatabase.Refresh();
    }






}
//#endif
