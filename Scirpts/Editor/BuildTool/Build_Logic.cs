using Framework;
using Lib;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

public class Build_Logic
{
    public static int GenLogic(BuildTarget target, BuildOptions buildOptions, bool useDebugMode)
    {
        int rlt = 0;

        string outputDir = $"{Directory.GetParent(Application.dataPath)}/AssetBundle/{target}/{AssetPath.sAssetLogicDir}";
        FileTool.CreateFolder(outputDir);


        //gen logic
        var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
        ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings()
        {
            target = target,
            group = targetGroup,
            options = ScriptCompilationOptions.None
        };
        string TempOutputFolder = Path.GetDirectoryName(Application.dataPath) + "/AppScriptDll";
        FileTool.CreateFolder(TempOutputFolder);
        EditorUtility.DisplayCancelableProgressBar("生成Logic.dll", AssetPath.sLogicDllName, 0.2f);
        ScriptCompilationResult scriptCompilationResult = PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, TempOutputFolder);
        AssetDatabase.Refresh();


        //encrypt logic
        string dllDestPath = Path.Combine(outputDir, AssetPath.sLogicDllName);
        File.Copy(Path.Combine(TempOutputFolder, AssetPath.sLogicDllName), dllDestPath + ".bytes");
        AssetDatabase.Refresh();
        EditorUtility.DisplayCancelableProgressBar("加密Logic.dll", AssetPath.sLogicDllName, 0.5f);
        FileTool.EncryptAsset(dllDestPath + ".bytes", dllDestPath + ".bytes", Consts.sLogicPassword);


        //debug 模式才会开启远程pdb调试模式
        if (useDebugMode)
        {
            //copy pdb
            string pdbOriPath = Path.Combine(TempOutputFolder, AssetPath.sLogicPdbName);
            string pdbDestPath = Path.Combine(outputDir, AssetPath.sLogicPdbName);
            File.Copy(pdbOriPath, pdbDestPath + ".bytes", true);
            AssetDatabase.Refresh();

            //encrypt pdb
            EditorUtility.DisplayCancelableProgressBar("加密pdb", AssetPath.sLogicPdbName, 0.8f);
            FileTool.EncryptAsset(pdbDestPath + ".bytes", pdbDestPath + ".bytes", Consts.sLogicPassword);
        }

        //删除零时文件
        FileTool.DeleteFolder(TempOutputFolder);

        EditorUtility.ClearProgressBar();
        return rlt;

    }
}
