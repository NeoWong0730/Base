using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
public static class FindScriptBeWhoReference 
{
    public static List<string> extionList = new List<string>()
    {
        ".prefab",
        ".unity",
        ".mat",
        ".fbx",
        ".controller",
        ".rendertexture",
        ".cubemap",
        ".modulemap",
        ".asset",
        ".playable"
    };

    [MenuItem("Assets/查找脚本被谁引用")]
    public static void FindScript()
    {
        var selected = Selection.activeObject;
        if (selected == null)
        {
            Debug.LogError("没有选中脚本！！！");
            return ;
        }
        Debug.Log(selected.GetType());
        
        if (!(selected is MonoScript))
        {
            Debug.LogError("选中的不是脚本，无法搜索被谁引用着！！！");
            return;
        }
            
        string selectedPath = AssetDatabase.GetAssetPath(selected);

        string TempOutputFolder = Application.dataPath + "/LogRecord";
        if (!Directory.Exists(TempOutputFolder))
        {
            Directory.CreateDirectory(TempOutputFolder);
            AssetDatabase.Refresh();
        }

        string bytesPath = Application.dataPath + "/LogRecord/AssetRelationByte.bytes";
        if (!File.Exists(bytesPath))
        {
            Dictionary<string, List<string>> tempDic = new Dictionary<string, List<string>>();
            DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath);
            FileInfo[] fs = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            for (int i = 0; i < fs.Length; i++)
            {
                FileInfo fileInfo = fs[i];
                string fullName = fileInfo.FullName;
                string ext = Path.GetExtension(fullName).ToLower();
                if (ext == ".meta")
                {
                    continue;
                }

                fullName = fullName.Replace("\\", "/");
                int index = fullName.IndexOf("Assets");
                string assetPath = fullName.Substring(index);

                //暂时先过滤这么多
                if (assetPath.StartsWith("Assets/StreamingAssets") ||
                    assetPath.StartsWith("Assets/Designer_Editor") ||
                    assetPath.StartsWith("Assets/AddressableAssetsData") ||
                    assetPath.StartsWith("Assets/Projects/Image") ||
                    assetPath.StartsWith("Assets/Config"))
                    continue;

                if (!extionList.Contains(ext))
                {
                    continue;
                }


                EditorUtility.DisplayProgressBar(i + "+构建关系中+" + fs.Length, fileInfo.Name, (float)i / fs.Length);
                
                string[] deps = AssetDatabase.GetDependencies(assetPath);
                for (int j = 0; j < deps.Length; j++)
                {
                    string tempPath = deps[j];
                    if (tempPath.StartsWith("Assets/StreamingAssets", System.StringComparison.Ordinal) ||
                        tempPath.StartsWith("Assets/Designer_Editor", System.StringComparison.Ordinal) ||
                        tempPath.StartsWith("Assets/AddressableAssetsData", System.StringComparison.Ordinal) ||
                        tempPath.StartsWith("Assets/Projects/Image", System.StringComparison.Ordinal) ||
                        tempPath.StartsWith("Assets/Config", System.StringComparison.Ordinal))
                        continue;
                    if (tempPath.EndsWith(".shader", System.StringComparison.Ordinal))
                        continue;

                    if (!tempDic.ContainsKey(tempPath))
                    {
                        List<string> tempList = new List<string>();
                        tempDic[tempPath] = tempList;
                    }
                    tempDic[tempPath].Add(assetPath);
                }
            }

            EditorUtility.ClearProgressBar();

            if (tempDic.ContainsKey(selectedPath))
            {
                foreach (var item in tempDic[selectedPath])
                {
                    Debug.LogError(selectedPath + "  被引用： " + item);
                }
            }
            else
            {
                Debug.LogError("没有东西引用：" + selectedPath);
            }
            
            //写入
            WriteBinary(bytesPath, tempDic);

            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            foreach (var item in tempDic)
            {
                stringBuilder.AppendFormat("{0}\t被依赖的个数:{1}\n", item.Key, item.Value.Count);
                if (item.Value != null)
                {
                    foreach (var dep in item.Value)
                    {
                        stringBuilder.AppendFormat("\t{0} \n", dep);
                    }
                }
                stringBuilder.AppendLine("------------------------------------------------------------------------------------");
            }
            System.IO.File.WriteAllText(UnityEngine.Application.dataPath + "/LogRecord/AssetRelationText.txt", stringBuilder.ToString());

        }
        else
        {
            var binary = ReadBinary(bytesPath);
            Dictionary<string, List<string>> tempDic = binary as Dictionary<string, List<string>>;

            if (tempDic.ContainsKey(selectedPath))
            {
                foreach (var item in tempDic[selectedPath])
                {
                    Debug.LogError(selectedPath + "  被引用： " + item);
                }
            }
            else
            {
                Debug.LogError("没有东西引用：" + selectedPath);
            }
        }
    }





    public static void WriteBinary(string path, object graph)
    {
        Stream fStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        BinaryFormatter binFormat = new BinaryFormatter();
        binFormat.Serialize(fStream, graph);
        fStream.Close();
        AssetDatabase.Refresh();
    }

    static object ReadBinary(string path)
    {
        Stream fStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryFormatter binFormat = new BinaryFormatter();
        return binFormat.Deserialize(fStream);
    }


}
