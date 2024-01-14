using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using static BezierController_3D;

public class CreateBezierDataEditor : Editor
{
    private readonly static string _BezierPrefabPath = "Assets/Designer_Editor/CombatData/BezierCurve/";

    private readonly static string _SaveBezierDataPath = "Assets/Config/CombatData/BezierCurve/";

    [MenuItem("Tools/Combat/Create Bezier Editor")]
    public static void CreateBezier()
    {
        List<string> strs = new List<string>();
        StringBuilder stringBuilder = new StringBuilder();

        string[] paths = Directory.GetFiles(_BezierPrefabPath);
        foreach (var path in paths)
        {
            if (Path.GetExtension(path) != ".prefab")
                continue;

            BezierController_3D bc3D = AssetDatabase.LoadAssetAtPath<BezierController_3D>(path);
            if (bc3D == null)
            {
                Debug.LogError($"{path}未挂载BezierController_3D脚本");
                continue;
            }

            stringBuilder.Clear();

            string idStr = bc3D.gameObject.name.Trim();
            uint id;
            if (!uint.TryParse(idStr, out id))
            {
                Debug.LogError($"CreateBezier获取数据中prefab：{idStr}命名不规范");
                continue;
            }

            stringBuilder.Append($"{idStr}@{bc3D.m_Speed.ToString()}&");
            Transform originTrans = null;
            for (int i = 0; i < bc3D.EditorPointTrans.Count; i++)
            {
                EditorTransClass editorTransClass = bc3D.EditorPointTrans[i];

                if (i == 0)
                    originTrans = editorTransClass.m_Trans;

                if (originTrans == null)
                {
                    Debug.LogError("没有原点");
                    return;
                }

                stringBuilder.Append($"{i.ToString()}*");

                if (editorTransClass.m_LeftHelpTrans != null)
                {
                    Vector3 disV3 = editorTransClass.m_LeftHelpTrans.position - originTrans.position;
                    stringBuilder.Append($"{EditorToolHelp.Get2Decimals_3(disV3.x).ToString()}:");
                    stringBuilder.Append($"{EditorToolHelp.Get2Decimals_3(disV3.y).ToString()}:");
                    stringBuilder.Append($"{EditorToolHelp.Get2Decimals_3(disV3.z).ToString()}");
                }
                stringBuilder.Append("|");
                if (editorTransClass.m_Trans != null && i > 0)
                {
                    Vector3 disV3 = editorTransClass.m_Trans.position - originTrans.position;
                    stringBuilder.Append($"{EditorToolHelp.Get2Decimals_3(disV3.x).ToString()}:");
                    stringBuilder.Append($"{EditorToolHelp.Get2Decimals_3(disV3.y).ToString()}:");
                    stringBuilder.Append($"{EditorToolHelp.Get2Decimals_3(disV3.z).ToString()}");
                }
                stringBuilder.Append("|");
                if (editorTransClass.m_RightHelpTrans != null)
                {
                    Vector3 disV3 = editorTransClass.m_RightHelpTrans.position - originTrans.position;
                    stringBuilder.Append($"{EditorToolHelp.Get2Decimals_3(disV3.x).ToString()}:");
                    stringBuilder.Append($"{EditorToolHelp.Get2Decimals_3(disV3.y).ToString()}:");
                    stringBuilder.Append($"{EditorToolHelp.Get2Decimals_3(disV3.z).ToString()}");
                }

                if (i < bc3D.EditorPointTrans.Count - 1)
                {
                    stringBuilder.Append("#");
                }
            }

            strs.Add(stringBuilder.ToString());
        }

        if (!Directory.Exists(_SaveBezierDataPath))
            Directory.CreateDirectory(_SaveBezierDataPath);

        EditorToolHelp.ExportTxtFileEx($"{_SaveBezierDataPath}CombatBezierData.txt", strs);

        AssetDatabase.Refresh();
    }
}
