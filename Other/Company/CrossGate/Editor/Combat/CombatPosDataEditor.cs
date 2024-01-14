using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CombatPosDataEditor : Editor
{
    public class PosEditorData
    {
        public float PosX;
        public float PosY;
        public float PosZ;
        public float AngleX;
        public float AngleY;
        public float AngleZ;
    }

    private readonly static string _SavePosDataPath = "Assets/Config/CombatData/Pos/";

    [MenuItem("Tools/Combat/Create Pos Editor")]
    public static void CreatePos()
    {
        Object[] objs = Selection.objects;
        if (objs == null || objs.Length <= 0)
            return;

        List<string> strs = new List<string>();
        StringBuilder stringBuilder = new StringBuilder();
        
        foreach (var obj in objs)
        {
            strs.Clear();
            stringBuilder.Clear();

            GameObject select = obj as GameObject;
            if (select == null)
                continue;

            //string selectName = select.name;
            //string[] strNames = selectName.Split('_');
            //if (strNames.Length <= 0 || !uint.TryParse(strNames[strNames.Length - 1], out uint posType))
            //{
            //    testName = selectName;
            //    posType = 0;
            //}

            Transform selectTrans = select.transform;
            for (int i = 0; i < selectTrans.childCount; i++)
            {
                Transform child = selectTrans.GetChild(i);
                if (child == null)
                    continue;

                if (!uint.TryParse(child.name, out uint posId))
                    continue;

                //if (string.IsNullOrWhiteSpace(testName) && posType != (posId / 1000000u))
                //    continue;

                stringBuilder.Clear();
                stringBuilder.Append($"{posId.ToString()}|");
                stringBuilder.Append($"{(child.localPosition.x).ToString()}:");
                stringBuilder.Append($"{(child.localPosition.y).ToString()}:");
                stringBuilder.Append($"{(child.localPosition.z).ToString()}:");
                stringBuilder.Append($"{(child.localEulerAngles.x).ToString()}:");
                stringBuilder.Append($"{(child.localEulerAngles.y).ToString()}:");
                stringBuilder.Append($"{(child.localEulerAngles.z).ToString()}");

                strs.Add(stringBuilder.ToString());
            }

            if (strs.Count == 0)
                continue;

            if (!Directory.Exists(_SavePosDataPath))
                Directory.CreateDirectory(_SavePosDataPath);

            EditorToolHelp.ExportTxtFileEx($"{_SavePosDataPath}CombatPosData_{select.name}.txt", strs);
        }
        
        AssetDatabase.Refresh();
    }
}
