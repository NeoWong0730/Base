using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class CustomDataLayoutEditor
{
    public class ShowObjInfoEditor
    {
        public uint OrderIndexInParent;
        public uint LayerIndex;
        public uint OrderIndex;
        public bool IsFoldout;
    }

    public BindingFlags m_BindingFlags;

    private string[] _interceptNamespaces = new string[] { "System.Reflection", "UnityEngine" };
    private List<ShowObjInfoEditor> _infoList = new List<ShowObjInfoEditor>();
    
    public CustomDataLayoutEditor()
    {
        m_BindingFlags = BindingFlags.Instance | BindingFlags.Public;
    }

    public CustomDataLayoutEditor(BindingFlags bindingFlags)
    {
        m_BindingFlags = bindingFlags;
    }

    public void StartDrawBlock(object obj, string fileName)
    {
        StartDrawBlock(obj, fileName, m_BindingFlags);
    }

    public void StartDrawBlock(object obj, string fileName, BindingFlags bindingFlags)
    {
        if (obj == null)
            return;

        Type objType = obj.GetType();

        EditorGUILayout.BeginVertical(GUI.skin.label);

        uint index = 0;
        foreach (FieldInfo fi in objType.GetFields(bindingFlags))
        {
            try
            {
                SetChildLayout01(fi.FieldType, fi.GetValue(obj), fi.Name, 0, 0, ref index);
            }
            catch { }
        }

        foreach (PropertyInfo pi in objType.GetProperties(bindingFlags))
        {
            try
            {
                SetChildLayout01(pi.PropertyType, pi.GetValue(obj), pi.Name, 0, 0, ref index);
            }
            catch { }
        }

        EditorGUILayout.Separator();

        EditorGUILayout.EndVertical();
    }

    public void DrawBlock(object obj, string fileName, uint orderIndexInParent, uint layerIndex, uint orderIndex)
    {
        if (obj == null)
            return;

        Type objType = obj.GetType();
        if (EditorToolHelp.IsJustShowType(objType))
        {
            Debug.LogError($"为:{objType.ToString()}");
            return;
        }

        EditorGUILayout.BeginVertical((layerIndex % 2 == 0) ? GUI.skin.button : GUI.skin.box);

        ShowObjInfoEditor showObjInfoEditor = GetShowObjInfoEditor(orderIndexInParent, layerIndex, orderIndex);

        showObjInfoEditor.IsFoldout = EditorGUILayout.Foldout(showObjInfoEditor.IsFoldout, $"{(string.IsNullOrEmpty(fileName) ? $"{obj.GetType().Name.ToString()}" : $"{fileName}（{obj.GetType().Name.ToString()}）")}({orderIndexInParent.ToString()}|{layerIndex.ToString()}|{orderIndex.ToString()})");
        if (showObjInfoEditor.IsFoldout)
        {
            uint index = 0u;

            if (objType.IsGenericType)
            {
                Type genericType = objType.GetGenericTypeDefinition();
                if (genericType == typeof(List<>))
                {
                    for (int i = 0, listCount = ((IList)obj).Count; i < listCount; i++)
                    {
                        object listChildObj = ((IList)obj)[i];
                        SetChildLayout01(listChildObj.GetType(), listChildObj, $"Item{i.ToString()}", orderIndex, layerIndex + 1, ref index);
                    }
                }
                else if (genericType == typeof(Dictionary<,>))
                {
                    int dicCount = ((IDictionary)obj).Count;
                    foreach (DictionaryEntry kv in (IDictionary)obj)
                    {
                        SetChildLayout01(kv.Value.GetType(), kv.Value, $"{kv.Key.ToString()}", orderIndex, layerIndex + 1, ref index);
                    }
                }
            }
            else if (objType.IsClass)
            {
                foreach (FieldInfo fi in objType.GetFields(m_BindingFlags))
                {
                    try
                    {
                        SetChildLayout01(fi.FieldType, fi.GetValue(obj), fi.Name, orderIndex, layerIndex + 1, ref index);
                    }
                    catch { }
                }

                foreach (PropertyInfo pi in objType.GetProperties(m_BindingFlags))
                {
                    try
                    {
                        SetChildLayout01(pi.PropertyType, pi.GetValue(obj), pi.Name, orderIndex, layerIndex + 1, ref index);
                    }
                    catch { }
                }
            }
        }

        EditorGUILayout.Separator();

        EditorGUILayout.EndVertical();
    }

    private void SetChildLayout01(Type childType, object childObj, string childFileName, uint orderIndexInParent, uint layerIndex, ref uint orderIndex)
    {
        if (childObj == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(childFileName);
            EditorGUILayout.TextArea(null);
            EditorGUILayout.EndHorizontal();
        }
        else if (EditorToolHelp.IsBaseType(childType) || childType.Name == "Type")
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(childFileName);
            EditorGUILayout.TextArea(childObj.ToString());
            EditorGUILayout.EndHorizontal();
        }
        else if (childType.IsEnum)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(childFileName);
            EditorGUILayout.EnumPopup((Enum)childObj);
            EditorGUILayout.EndHorizontal();
        }
        else if (childType == typeof(GameObject) || childType == typeof(Transform))
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(childFileName);
            EditorGUILayout.ObjectField((UnityEngine.Object)childObj, childType, true);
            EditorGUILayout.EndHorizontal();
        }
        else if (childType.IsClass)
        {
            if (EditorToolHelp.InterceptNamespaces(childType, _interceptNamespaces))
                return;

            EditorGUILayout.Separator();
            DrawBlock(childObj, childFileName, orderIndexInParent, layerIndex, ++orderIndex);
        }
        else if (childType.IsGenericType)
        {
            EditorGUILayout.Separator();
            DrawBlock(childObj, childFileName, orderIndexInParent, layerIndex, ++orderIndex);
        }
    }

    private ShowObjInfoEditor GetShowObjInfoEditor(uint orderIndexInParent, uint layerIndex, uint orderIndex)
    {
        for (int i = 0, count = _infoList.Count; i < count; i++)
        {
            ShowObjInfoEditor soie = _infoList[i];
            if (soie.OrderIndexInParent == orderIndexInParent && soie.LayerIndex == layerIndex && soie.OrderIndex == orderIndex)
                return soie;
        }

        ShowObjInfoEditor showObjInfoEditor = new ShowObjInfoEditor();
        showObjInfoEditor.OrderIndexInParent = orderIndexInParent;
        showObjInfoEditor.LayerIndex = layerIndex;
        showObjInfoEditor.OrderIndex = orderIndex;

        _infoList.Add(showObjInfoEditor);

        return showObjInfoEditor;
    }

    
}
