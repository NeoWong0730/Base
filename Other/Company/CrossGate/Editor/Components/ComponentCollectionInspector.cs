using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComponentCollection))]
public class ComponentCollectionInspector : Editor
{
    ComponentCollection componentCollection;

    void OnEnable()
    {        
        componentCollection = target as ComponentCollection;
    }

    public override void OnInspectorGUI()
    {
        List<Component> components = componentCollection.components;
        List<string> fieldNames = componentCollection.fieldNames;
        
        if (components != null)
        {
            if (GUILayout.Button("+"))
            {
                components.Add(null);
                fieldNames.Add(null);
            }

            for (int i = components.Count - 1; i >= 0; --i)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-"))
                {
                    components.RemoveAt(i);
                    fieldNames.RemoveAt(i);
                }
                else
                {
                    System.Type type = components[i] != null ? components[i].GetType() : typeof(Component);

                    fieldNames[i] = EditorGUILayout.TextField(fieldNames[i]);
                    components[i] = EditorGUILayout.ObjectField(components[i], type, true) as Component;                    

                    if (string.IsNullOrWhiteSpace(fieldNames[i]))
                    {
                        if (components[i] != null)
                        {
                            fieldNames[i] = components[i].name + "_" + components[i].GetType().Name;
                        }
                    }
                    else
                    {
                        fieldNames[i] = fieldNames[i].Replace(" ", "_");
                    }
                }
                EditorGUILayout.EndHorizontal();

                if(components[i] == null)
                {
                    string tmp = EditorGUILayout.TextField(string.Empty);
                    if (!string.IsNullOrWhiteSpace(tmp))
                    {
                        Transform transform = componentCollection.transform.Find(tmp);
                        if (transform != null)
                        {
                            Component[] cs = transform.GetComponents<Component>();
                            Component c = null;

                            for (int k = 0; k < cs.Length; ++k)
                            {
                                c = cs[k];

                                if (c is CanvasRenderer)
                                {
                                    continue;
                                }

                                if (!(c is RectTransform))
                                {
                                    break;
                                }
                            }

                            components[i] = c;
                        }
                    }
                }                
            }

            if (GUILayout.Button("GenLayoutCode"))
            {
                GenLayoutCode(components, fieldNames, componentCollection.gameObject.name + "_Layout");
            }
        }
    }    
    
    private void GenLayoutCode(List<Component> components, List<string> fieldNames, string className)
    {
        string fieldFormat = "\t\tpublic {0} {1} {{ get {{ return components[{2}] as {0}; }} }} //{3} \n";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        sb.AppendLine("//Auto Generated From ComponentCollectionInspector");
        sb.AppendLine("namespace Logic.UILayout");
        sb.AppendLine("{");
        sb.Append("\tpublic class ");
        sb.Append(className);
        sb.AppendLine("\n\t{");

        sb.AppendLine("\t\tprivate System.Collections.Generic.List<UnityEngine.Component> components;");        
        sb.AppendLine("\t\tpublic UnityEngine.Transform mTrans { get; private set; }");
        sb.AppendLine("\t\tpublic UnityEngine.GameObject mRoot { get { return mTrans.gameObject; } }");

        for (int i = components.Count - 1; i >= 0; --i)
        {
            string node = MenuItem_App.FullHierarchyPath(components[i].gameObject);
            sb.AppendFormat(fieldFormat, components[i].GetType().FullName, fieldNames[i], i.ToString(), node);
        }

        sb.AppendLine("\t\tpublic virtual void Parse(UnityEngine.GameObject root)");
        sb.AppendLine("\t\t{");        
        sb.AppendLine("\t\t\tmTrans = root.transform;");        
        sb.AppendLine("\t\t\tif (root.TryGetComponent<ComponentCollection>(out ComponentCollection componentCollection))");
        sb.AppendLine("\t\t\t{");
        sb.AppendLine("\t\t\t\tcomponents = componentCollection.components;");
        sb.AppendLine("\t\t\t}");
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t}");
        sb.AppendLine("}");

        string path = "Assets/Scripts/Logic/UILayout/" + className + ".cs";
        System.IO.File.WriteAllText(path, sb.ToString());
        sb.Clear();
    }
}
