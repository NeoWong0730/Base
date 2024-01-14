using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Lib.AssetLoader;
using System.IO;

public class Build_Config 
{
    public static int GenConfig(BuildTarget target)
    {
        string inputDir = $"{Application.dataPath}/{AssetPath.sAssetCsvDir}";
        string outputDir = $"{Directory.GetParent(Application.dataPath)}/AssetBundle/{target}/{AssetPath.sAssetCsvDir}";
        int rlt = FileTool.EncryptAsset_0(inputDir,outputDir, "移动并压缩Config到输出资源目录",true,new List<string> { ".meta"});
        return rlt;
    }

}
