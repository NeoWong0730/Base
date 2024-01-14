using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class EditorToolHelp
{
    public static void DragMouseEvent(ref Vector2 dragPos, ref Vector2 offsetPos, 
        System.Action entryAction, System.Action stayAction, System.Action outAction, int buttonIndex, bool isOver = false)
    {
        Vector2 mousePos = Event.current.mousePosition;

        if (isOver || Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseLeaveWindow)
        {
            offsetPos = Vector2.zero;
            outAction?.Invoke();
        }

        if (isOver)
            return;
        
        if (Event.current.type == EventType.MouseDown)
        {
            if (Event.current.button == buttonIndex)
            {
                dragPos = mousePos;
                entryAction?.Invoke();
            }
        }
        
        if (Event.current.type == EventType.MouseDrag)
        {
            if (Event.current.button == buttonIndex)
            {
                Vector2 op = mousePos - dragPos;
                
                if (Mathf.Abs(op.x) < 3 && Mathf.Abs(op.y) < 3)
                {
                    offsetPos = Vector2.zero;
                }
                else
                {
                    offsetPos = op;
                    dragPos = mousePos;

                    stayAction?.Invoke();
                }
            }
        }
    }

    public static void DrawNodeCurve(Rect start, Rect end, bool isSelected = false)
    {
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        DrawNodeCurve(start, endPos, isSelected);
    }

    public static void DrawNodeCurve(Rect start, Vector3 endPos, bool isSelected = false)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        if (isSelected)
        {
            Color shadowCol = Color.red;
            shadowCol.a = 0.2f;
            for (int i = 0; i < 3; i++) // Draw a shadow
            {
                shadowCol.a -= i * 0.01f;
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 8);
            }
        }
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 5f);

        var oldColor = Handles.color;
        Handles.color = Color.yellow;
        //Vector3 upPos = Quaternion.Euler(0f, 0f, 45f) * Vector3.left;
        //Vector3 downPos = Quaternion.Euler(0f, 0f, -45f) * Vector3.left;
        //for (int i = 0; i < 10; i++)
        //{
        //    Vector3 vpos = endPos + new Vector3(-i, 0f, 0f);
        //    Handles.DrawLine(vpos, upPos * (20f - i) + vpos);
        //    Handles.DrawLine(vpos, downPos * (20f - i) + vpos);
        //}
        Handles.DrawSolidDisc(endPos, Vector3.forward, 10f);
        Handles.color = oldColor;
    }

    public static void DrawShadowCurve(Rect start, Rect end, Color color, float width)
    {
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        
        for (int i = 0; i < 3; i++) // Draw a shadow
        {
            color.a -= i * 0.01f;
            Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, (i + 1) * width);
        }

        var oldColor = Handles.color;
        Handles.color = color;
        Handles.DrawSolidDisc(endPos, Vector3.forward, 13f);
        Handles.color = oldColor;
    }

    public static void DrawCurve(float startx, float starty,
        float rightx, float righty,
        float leftx, float lefty,
        float endx, float endy,
        Color color, float width)
    {
        Vector3 endPos = new Vector3(endx, endy, 0);
        Vector3 startPos = new Vector3(startx, starty, 0);
        Vector3 startTan = new Vector3(rightx, righty);
        Vector3 endTan = new Vector3(leftx, lefty);

        Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, width);
    }

    public static Vector2 GetBezierVal(float t, float startx, float starty,
        float rightx, float righty,
        float leftx, float lefty,
        float endx, float endy)
    {
        Vector3 endPos = new Vector3(endx, endy, 0);
        Vector3 startPos = new Vector3(startx, starty, 0);
        Vector3 startTan = new Vector3(rightx, righty);
        Vector3 endTan = new Vector3(leftx, lefty);

        return CombatHelp.Calculate3BezierPoint_2D(t, startPos, startTan, endTan, endPos);
    }

    public static float DistancePointBezier(Vector3 point,
                                    float startx, float starty,
                                    float rightx, float righty,
                                    float leftx, float lefty,
                                    float endx, float endy)
    {
        Vector3 endPos = new Vector3(endx, endy, 0);
        Vector3 startPos = new Vector3(startx, starty, 0);
        Vector3 startTan = new Vector3(rightx, righty);
        Vector3 endTan = new Vector3(leftx, lefty);

        return HandleUtility.DistancePointBezier(point, startPos, endPos, startTan, endTan);
    }

    public static Editor GetOrCreateEditorFortarget(UnityEngine.Object target)
    {
        if (target == null)
        {
            throw new System.ArgumentNullException("Tried to create editor for object that is null");
        }

        var result = Editor.CreateEditor(target);
        return result;
    }
    
    public static void ExportTxtFileEx(string filepath, List<string> strs)
    {
        using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
        {
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            foreach (var str in strs)
            {
                sw.WriteLine(str);
            }
            sw.Close();
        }
    }

    public static void ExportByteFileEx(string filepath, List<string> strs)
    {
        using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
        {
            BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8);

            bw.Write(strs.Count);
            foreach (var str in strs)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(str);
                int count = byteArray.Length;
                bw.Write(count);
                bw.Write(byteArray);
            }

            bw.Close();
        }
    }

    public static string GetPropertyValueByType(SerializedProperty sp, string typeName, bool isEnum = false)
    {
        if (typeName == "Int64")
        {
            return sp.longValue.ToString();
        }
        else if (typeName == "UInt64")
        {
            return sp.longValue.ToString();
        }
        else if (typeName == "Int32")
        {
            return sp.intValue.ToString();
        }
        else if (typeName == "UInt32")
        {
            return sp.intValue.ToString();
        }
        else if (typeName == "Single")
        {
            return sp.floatValue.ToString();
        }
        else if (typeName == "Double")
        {
            return sp.doubleValue.ToString();
        }
        else if (typeName == "String")
        {
            return sp.stringValue.ToString();
        }
        else if (typeName == "Boolean")
        {
            return sp.boolValue ? "1" : "0";
        }
        else if (isEnum)
        {
            return sp.intValue.ToString();
        }
        else if (typeName == "SVector2")
        {
            var xSP = sp.serializedObject.FindProperty($"{sp.propertyPath}.x");
            var ySP = sp.serializedObject.FindProperty($"{sp.propertyPath}.y");
            return $"{xSP.floatValue.ToString()},{ySP.floatValue.ToString()}";
        }

        return string.Empty;
    }

    public static void SetPropertyValueByType(SerializedProperty sp, string typeName, string val, bool isEnum = false)
    {
        if (typeName == "Int64" || typeName == "UInt64")
        {
            sp.longValue = long.Parse(val);
        }
        else if (typeName == "Int32" || typeName == "UInt32")
        {
            sp.intValue = int.Parse(val);
        }
        else if (typeName == "Single")
        {
            sp.floatValue = float.Parse(val);
        }
        else if (typeName == "Double")
        {
            sp.doubleValue = double.Parse(val);
        }
        else if (typeName == "String")
        {
            sp.stringValue = val;
        }
        else if (typeName == "Boolean")
        {
            sp.boolValue = val == "0" ? false : true;
        }
        else if (isEnum)
        {
            sp.intValue = int.Parse(val);
        }
        else if (typeName == "SVector2")
        {
            var xSP = sp.serializedObject.FindProperty($"{sp.propertyPath}.x");
            var ySP = sp.serializedObject.FindProperty($"{sp.propertyPath}.y");
            string[] ss = val.Split(',');
            xSP.floatValue = float.Parse(ss[0]);
            ySP.floatValue = float.Parse(ss[1]);
        }
    }

    public static string CompareFieldStrs(string referObj, string compareObj, Type type)
    {
        string str = string.Empty;
        var ts = type.GetFields();
        for (int i = 0; i < ts.Length; i++)
        {
            if (i == 0)
                str += "\n\t\treturn ";

            FieldInfo t = ts[i];
            string typeName = t.FieldType.Name;
            if (typeName == "Int64" || typeName == "UInt64" ||
                typeName == "Int32" || typeName == "UInt32" ||
                typeName == "Single" || typeName == "Double")
            {
                str += $"({compareObj}.{t.Name} >= {referObj}.{t.Name})";
            }
            else if (typeName == "String" || typeName == "Boolean" || t.FieldType.BaseType == typeof(Enum))
            {
                str += $"({compareObj}.{t.Name} == {referObj}.{t.Name})";
            }
            else if (typeName == "SVector2")
            {
                str += $"({compareObj}.{t.Name}.x >= {referObj}.{t.Name}.x  && {compareObj}.{t.Name}.y <= {referObj}.{t.Name}.y)";
            }

            if (i + 1 < ts.Length)
            {
                str += " && \n\t\t\t";
            }
            else
            {
                str += ";";
            }
        }

        if (string.IsNullOrEmpty(str))
            str = "return true;";

        return str;
    }

    public static bool IsBaseType(Type type)
    {
        string typeName = type.Name;
        switch (typeName)
        {
            case "Int64":
            case "UInt64":
            case "Int32":
            case "UInt32":
            case "Single":
            case "Double":
            case "String":
            case "Boolean":
                return true;
        }

        return false;
    }

    public static bool IsJustShowType(Type type)
    {
        string typeName = type.Name;
        if (IsBaseType(type))
            return true;

        if (type.IsEnum)
            return true;

        if (typeName == "Type")
            return true;

        return false;
    }

    public static bool InterceptNamespaces(Type type, string[] interceptStrs)
    {
        foreach (var str in interceptStrs)
        {
            if (type.Namespace == str)
                return true;
        }

        return false;
    }

    public static int SetIntPopup(Rect rect, List<string> strs, int sv)
    {
        string[] clipNames = strs.ToArray();
        int[] ovs = new int[clipNames.Length];
        for (int i = 0; i < ovs.Length; i++)
        {
            ovs[i] = i;
        }
        return EditorGUI.IntPopup(rect, sv, clipNames, ovs);
    }

    public static float Get2Decimals_3(float f)
    {
        int i = (int)(f * 1000f);

        return i * 0.001f;
    }

    public static void ParseTxtInLine(string filePath, Action<string> action)
    {
        StreamReader sr = new StreamReader(filePath, Encoding.UTF8);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            action?.Invoke(line);
        }
        sr.Close();
        sr.Dispose();
    }

    public static void CreateText(string path, string content)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        //文件写入
        FileStream stream;
        if (!File.Exists(path))
        {
            stream = File.Create(path);
        }
        else
        {
            stream = File.Open(path, FileMode.Truncate);
        }

        StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
        writer.Write(content);
        writer.Close();
    }

    public static string FindFile(string path, string csName, string searchPattern)
    {
        var files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (Path.GetFileNameWithoutExtension(file) == csName)
            {
                return file.Replace("\\", "/");
            }
        }

        return null;
    }
}
