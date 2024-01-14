using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationClipAnalysis
{
    [MenuItem("__Tools__/生成动作预加载代码")]
    public static void GenAnimationClipProload()
    {
#if false
        string outPath = "/Scripts/Logic/AniClipProload.cs";
#else
        string outPath = "/Config/AniClipProload.txt";
#endif

        Table.CSVAction.Load(true);

        HashSet<string> clipSet = new HashSet<string>();
        foreach (var data in Table.CSVAction.Instance.GetAll())
        {
            if (!data.role)
                continue;

            if (!string.IsNullOrEmpty(data.action_idle))
            {
                string filePath = string.Format("{0}/{1}.{2}", data.dirPath, data.action_idle, "anim");
                if (!clipSet.Contains(filePath))
                {
                    clipSet.Add(filePath);
                }
            }

            if (!string.IsNullOrEmpty(data.action_run))
            {
                string filePath = string.Format("{0}/{1}.{2}", data.dirPath, data.action_run, "anim");
                if (!clipSet.Contains(filePath))
                {
                    clipSet.Add(filePath);
                }
            }
        }

        Table.CSVAction.Unload();

        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
#if false
        stringBuilder.Append("using Framework;\n");
        stringBuilder.Append("using UnityEngine;\n");

        stringBuilder.Append("public static class AnimationClipProload\n");
        stringBuilder.Append("{\n");
        stringBuilder.Append("\tpublic static void Preload(AssetsPreload assetsPreload)\n");
        stringBuilder.Append("\t{\n");

        foreach (var clipAddr in clipSet)
        {
            stringBuilder.Append("\t\tassetsPreload.Preload<AnimationClip>(\"");
            stringBuilder.Append(clipAddr);
            stringBuilder.Append("\");\n");
        }

        stringBuilder.Append("\t}\n");
        stringBuilder.Append("}\n");
#else
        foreach (var clipAddr in clipSet)
        {
            stringBuilder.AppendLine(clipAddr);
        }
#endif
        string content = stringBuilder.ToString();

        Debug.Log(content);
        
        System.IO.File.WriteAllText(Application.dataPath + outPath, content);
    }
}
