using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using System.Text;
using System.IO;
#endif

namespace Framework
{
    public class UIMember : MonoBehaviour
    {
#if UNITY_EDITOR
        public string[] mNames;
#endif
        public Component[] mValues;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(UIMember))]
    public class UIMemberEditor : Editor
    {
        public class UIMemberInfo
        {
            public string sName;
            public Component mValue;
            public bool hasRepeat;
        }

        private ReorderableList mReorderableList;
        private UIMember uIMember;
        private List<UIMemberInfo> memberInfos;

        private void OnEnable()
        {
            uIMember = target as UIMember;

            if (uIMember.mNames == null)
            {
                memberInfos = new List<UIMemberInfo>();
            }
            else
            {
                memberInfos = new List<UIMemberInfo>(uIMember.mNames.Length);

                for (int i = 0; i < uIMember.mNames.Length; ++i)
                {
                    UIMemberInfo memberInfo = new UIMemberInfo();
                    memberInfos.Add(memberInfo);
                    memberInfo.sName = uIMember.mNames[i];
                    memberInfo.mValue = uIMember.mValues[i];
                }
            }

            mReorderableList = new ReorderableList(memberInfos, typeof(UIMemberInfo));
            mReorderableList.drawElementCallback = ElementCallbackDelegate;
            //mReorderableList.onChangedCallback = ChangedCallbackDelegate;        
        }

        private void ElementCallbackDelegate(Rect rect, int index, bool isActive, bool isFocused)
        {
            memberInfos[index].sName = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width / 2, rect.height), memberInfos[index].sName);
            memberInfos[index].mValue = (Component)EditorGUI.ObjectField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, rect.height), memberInfos[index].mValue, typeof(Component), true);
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            mReorderableList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                uIMember.mNames = new string[memberInfos.Count];
                uIMember.mValues = new Component[memberInfos.Count];

                for (int i = 0; i < memberInfos.Count; ++i)
                {
                    uIMember.mNames[i] = memberInfos[i].sName;
                    uIMember.mValues[i] = memberInfos[i].mValue;
                }
                //EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Generation cs code"))
            {
                if (!CheckRepeat())
                    return;

                string path = EditorUtility.SaveFilePanel("Save", Application.dataPath + "/Scripts/Logic", "UILayout", "cs");
                if (string.IsNullOrWhiteSpace(path))
                    return;

                GenerationCsCode(path);
            }
        }

        private bool CheckRepeat()
        {
            return true;
        }

        private void GenerationCsCode(string path)
        {
            StringBuilder sb = new StringBuilder();

            string name = Path.GetFileNameWithoutExtension(path);
            sb.AppendLine($"namespace Logic");
            sb.AppendLine("{");
            sb.AppendLine($"\tpublic class {name}");
            sb.AppendLine("\t{");

            sb.AppendLine("\t\tprotected UIMember mMember;");

            sb.AppendLine("\t\tpublic bool Bind(UnityEngine.GameObject go)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\treturn go.TryGetComponent<UIMember>(out mMember);");
            sb.AppendLine("\t\t}");

            for (int i = 0; i < uIMember.mNames.Length; ++i)
            {
                sb.AppendLine($"\t\tpublic {uIMember.mValues[i].GetType().FullName} {uIMember.mNames[i]} {{ get {{ return ({uIMember.mValues[i].GetType().FullName})mMember.mValues[{i}]; }} }}");
            }


            sb.AppendLine("\t}");
            sb.AppendLine("}");

            File.WriteAllText(path, sb.ToString());
        }
    }

#endif
}