using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#region Editor内容

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UIComponentBinder.BindComponent))]
public class BindItemDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        using (new EditorGUI.PropertyScope(position, label, property)) {
            EditorGUIUtility.labelWidth = 120;
            position.height = EditorGUIUtility.singleLineHeight;

            Rect componentRect = new Rect(position) {
                x = position.x + 30,
                width = 130
            };
            var component = property.FindPropertyRelative("component");
            EditorGUI.PropertyField(componentRect, component, GUIContent.none);

            Rect listenRect = new Rect(componentRect) {
                x = componentRect.x + 135,
                width = 30
            };
            var listen = property.FindPropertyRelative("toListen");
            EditorGUI.PropertyField(listenRect, listen, GUIContent.none);

            Rect nameRect = new Rect(listenRect) {
                x = listenRect.x + 20,
                width = 130
            };
            var name = property.FindPropertyRelative("name");
            name.stringValue = EditorGUI.TextField(nameRect, "", name.stringValue);
        }
    }
}

[CustomEditor(typeof(UIComponentBinder))]
public class UIComponentBinderInspector : Editor {
    private UIComponentBinder owner;

    private SerializedProperty fieldStyle;
    private ReorderableList recorderableList;

    private void OnEnable() {
        owner = target as UIComponentBinder;

        fieldStyle = serializedObject.FindProperty("fieldStyle");

        var prop = serializedObject.FindProperty("bindComponents");
        recorderableList = new ReorderableList(serializedObject, prop);
        recorderableList.elementHeight = 20;
        recorderableList.drawElementCallback = (rect, index, active, focused) => {
            var element = prop.GetArrayElementAtIndex(index);

            Rect itemRect = new Rect(rect) {
                x = rect.x,
                width = 20
            };
            EditorGUI.LabelField(itemRect, string.Format("[{0}]", index.ToString()));

            rect.height -= 4;
            rect.y += 2;
            EditorGUI.PropertyField(rect, element);
        };

        recorderableList.onSelectCallback += rlist => { GUI.backgroundColor = Color.blue; };

        recorderableList.drawHeaderCallback = rect => {
            var oldColor = GUI.color;
            GUI.color = Color.green;
            EditorGUI.LabelField(rect, string.Format("[--> Index | {0} | {1} | {2} <--]", /*prop.displayName, */"Component", "Listen", "Name"));
            GUI.color = oldColor;
        };
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(fieldStyle);
        recorderableList.DoLayoutList();
        if (GUILayout.Button("AUTO NAME")) {
            owner.AutoName();
        }

        if (GUILayout.Button("CHECK")) {
            owner.Check();
        }

        if (GUILayout.Button("COPY")) {
            if (owner.Check()) {
                string code = owner.Copy("Layout", false);
                GUIUtility.systemCopyBuffer = code;
                Debug.LogError(code);
            }
        }

        if (GUILayout.Button("GENERATE")) {
            if (owner.Check()) {
                string path = EditorUtility.SaveFilePanel("SaveFile", Application.dataPath + "/Scripts/Logic/System/UI", "XXX_Layout", "cs");
                if (string.IsNullOrWhiteSpace(path)) {
                    return;
                }

                string clsName = Path.GetFileNameWithoutExtension(path);
                if (!clsName.Contains("_Layout")) {
                    clsName += "_Layout";
                }

                string code = owner.Copy(clsName, true);
                GUIUtility.systemCopyBuffer = code;
                File.WriteAllText(path, code);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif

#endregion

[DisallowMultipleComponent]
public class UIComponentBinder : MonoBehaviour {
    [Serializable]
    public class BindComponent {
        public Component component;

#if UNITY_EDITOR
        // 将name只在editor序列化，否则会占用运行时内存
        public string name;
        public bool toListen = false;

        public string componentType => type?.ToString();

        public Type type {
            get {
                if (component != null) {
                    return component.GetType();
                }

                return null;
            }
        }

        public string GetComponentPath(Component end) {
            return GetPath(component, end);
        }

#endif
    }

    public List<BindComponent> bindComponents = new List<BindComponent>();

#if UNITY_EDITOR
    public enum EFieldStyle {
        Field,
        Property,
    }

    public EFieldStyle fieldStyle = EFieldStyle.Field;
    public const string TAB = "    ";

    // tuple形式，方便后续动态的add，进行拓展
    public static readonly Dictionary<Type, IList<ValueTuple<string, string>>> LISTEN_DESCS = new Dictionary<Type, IList<ValueTuple<string, string>>>() {
        {
            typeof(Button), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnBtnClicked_{0}();", "this.{0}.onClick.AddListener(listener.OnBtnClicked_{1});"),
            }
        }, {
            typeof(Toggle), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnValueChanged_{0}(bool status);", "this.{0}.onValueChanged.AddListener(listener.OnValueChanged_{1});"),
            }
        }, {
            typeof(Slider), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnValueChanged_{0}(float currentValue);", "this.{0}.onValueChanged.AddListener(listener.OnValueChanged_{1});"),
            }
        }, {
            typeof(InfinityGrid), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnCreateCell_{0}(InfinityGridCell cell);", "this.{0}.onCreateCell += listener.OnCreateCell_{1};"),
                new ValueTuple<string, string>("void OnCellChange_{0}(InfinityGridCell cell, int index);", "this.{0}.onCellChange += listener.OnCellChange_{1};"),
            }
        }, {
            typeof(CDText), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnTimeRefresh_{0}(TMPro.TextMeshProUGUI text, float time, bool isEnd);", "this.{0}.onTimeRefresh += listener.OnTimeRefresh_{1};"),
            }
        }, {
            typeof(GiftStatus), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnClicked_{0}(EGiftStatus status);", "this.{0}.onClicked += listener.OnClicked_{1};"),
            }
        }, {
            typeof(UICenterOnChild), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnCenter_{0}(int index, UnityEngine.Transform t);", "this.{0}.onCenter += listener.OnCenter_{1};"),
                new ValueTuple<string, string>("void OnTransform_{0}(bool hOrV, int index, UnityEngine.Transform t, float toMiddle, UnityEngine.Vector3 srCenterOnCentent);",
                    "this.{0}.onTransform += listener.OnTransform_{1};"),
            }
        }, {
            typeof(PageSwitcher), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnPageSwitch_{0}(int pageIndex, int startIndex, int range);", "this.{0}.onExec += listener.OnPageSwitch_{1};"),
            }
        }, {
            typeof(LRArrowSwitcher), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnPageSwitch_{0}(int currentIndex, uint id);", "this.{0}.onExec += listener.OnPageSwitch_{1};"),
            }
        }, {
            typeof(SliderFTMLerp), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnCompleted_{0}();", "this.{0}.onCompleted += listener.OnCompleted_{1};"),
                new ValueTuple<string, string>("void OnChanged_{0}(float to, float toSubFrom, float max, float rate);", "this.{0}.onChanged += listener.OnChanged_{1};"),
            }
        }, {
            typeof(ToggleEx), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnValueChanged_{0}(bool status, bool interaction/*交互点击true，否则false*/);", "this.{0}.onValueChanged.AddListener(listener.OnValueChanged_{1});"),
            }
        }, {
            typeof(ToggleRegistry), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnToggleChange_{0}(int newId, int oldId, bool interaction/*交互点击true，否则false*/);", "this.{0}.onToggleChange += listener.OnToggleChange_{1};"),
            }
        }, {
            typeof(TMP_InputField), new List<ValueTuple<string, string>>() {
                new ValueTuple<string, string>("void OnValueChanged_{0}(string input);", "this.{0}.onValueChanged.AddListener(listener.OnValueChanged_{1});"),
            }
        }
    };

    // 是否需要生成listener
    public bool NeedListener() {
        bool need = false;
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var item = bindComponents[i];
            if (item.component != null && item.toListen) {
                var type = item.type;
                if (type != null && LISTEN_DESCS.TryGetValue(type, out var descs)) {
                    need = true;
                    break;
                }
            }
        }

        return need;
    }

    // https://github.com/scriban/scriban
    private string CSharpCopy(string clsName, bool includeNamespace) {
        string AppendTab(int level, bool include = false, string Tab = UIComponentBinder.TAB) {
            StringBuilder rlt = new StringBuilder();
            if (level <= 0) {
                return "";
            }

            for (int i = 0; i < (include ? level + 1 : level); ++i) {
                rlt.Append(Tab);
            }

            return rlt.ToString();
        }

        StringBuilder sb = new StringBuilder();
        if (includeNamespace) {
            sb.AppendLine("// Generated by UIComponentBinder.cs, you can't modifity it manualy!");
            sb.AppendLine("using System;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using Logic.Core;");
            sb.AppendLine("using TMPro;");

            sb.AppendLine();
            sb.AppendLine("namespace Logic.UI {");
        }

        sb.AppendLine(includeNamespace ? TAB + "[Serializable]" : "[Serializable]");
        string prefix = "public partial class " + $"{clsName}" + " : UILayoutBase {";
        sb.AppendLine(includeNamespace ? TAB + prefix : prefix);
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var item = bindComponents[i];
            sb.Append(AppendTab(1, includeNamespace));
            sb.AppendFormat("public {0} {1}", item.componentType, item.name);

            if (fieldStyle == EFieldStyle.Property) {
                sb.Append(" { get; private set; } = null;");
            }
            else if (fieldStyle == EFieldStyle.Field) {
                sb.Append(" = null;");
            }
            sb.AppendFormat(" // [{0}] Path: \"{1}\"", i.ToString(), item.GetComponentPath(this));
            
            sb.AppendLine();
        }

        sb.AppendLine();

        sb.Append(AppendTab(1, includeNamespace));
        sb.AppendLine("protected override void Loaded() {");
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var item = bindComponents[i];
            sb.Append(AppendTab(2, includeNamespace));
            sb.AppendFormat("this.{0} = binder.Find<{1}>({2});", item.name, item.componentType, i.ToString());
            sb.AppendLine();
        }

        sb.Append(AppendTab(1, includeNamespace));
        sb.AppendLine("}");

        sb.AppendLine();
        sb.AppendLine("#if UNITY_EDITOR");
        sb.Append(AppendTab(1));
        sb.AppendLine("[__TAB__]protected override void FindByPath(UnityEngine.Transform transform, bool check = false) {");

        sb.Append(AppendTab(2));
        sb.AppendLine("[__TAB__]if (!check) {");
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var item = bindComponents[i];
            sb.Append(AppendTab(3));
            var path = item.GetComponentPath(this);
            if (path == null) {
                sb.AppendFormat("[__TAB__]this.{0} = transform.GetComponent<{1}>();", item.name, item.componentType);
            }
            else {
                sb.AppendFormat("[__TAB__]this.{0} = transform.Find(\"{1}\").GetComponent<{2}>();", item.name, path, item.componentType);
            }

            sb.AppendLine();
        }

        sb.Append(AppendTab(2));
        sb.AppendLine("[__TAB__]}");

        sb.Append(AppendTab(2));
        sb.AppendLine("[__TAB__]else {");
        sb.Append(AppendTab(3));
        sb.AppendLine("[__TAB__]UnityEngine.Transform _t_ = null;");
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var item = bindComponents[i];
            sb.Append(AppendTab(3));
            var path = item.GetComponentPath(this);
            if (path == null) {
                sb.AppendFormat("[__TAB__]this.{0} = transform.GetComponent<{1}>();", item.name, item.componentType);
            }
            else {
                sb.AppendFormat("[__TAB__]_t_ = transform.Find(\"{0}\");", path);
                sb.AppendLine();
                sb.Append(AppendTab(3));
                sb.AppendFormat("[__TAB__]this.{0} = _t_ != null ? _t_.GetComponent<{1}>() : null;", item.name, item.componentType);
            }

            sb.AppendLine();
        }

        sb.Append(AppendTab(2));
        sb.AppendLine("[__TAB__]}");

        sb.Append(AppendTab(1));
        sb.AppendLine("[__TAB__]}");
        sb.AppendLine("#endif");

        if (NeedListener()) {
            sb.AppendLine();
            sb.Append(AppendTab(1, includeNamespace));

            sb.AppendLine(@"public interface IListener {");
            for (int i = 0, length = bindComponents.Count; i < length; ++i) {
                var item = bindComponents[i];
                if (item.component != null && item.toListen) {
                    var type = item.type;
                    if (type != null && LISTEN_DESCS.TryGetValue(type, out var descs)) {
                        foreach (var desc in descs) {
                            sb.Append(AppendTab(2, includeNamespace));
                            sb.AppendFormat(desc.Item1, item.name);
                            sb.AppendLine();
                        }
                    }
                }
            }

            sb.Append(AppendTab(1, includeNamespace));
            sb.AppendLine("}");
            sb.AppendLine();

            sb.Append(AppendTab(1, includeNamespace));
            sb.AppendLine("public void BindEvents(IListener listener, bool toListen = true) {");
            for (int i = 0, length = bindComponents.Count; i < length; ++i) {
                var item = bindComponents[i];
                if (item.component != null && item.toListen) {
                    var type = item.type;
                    if (type != null && LISTEN_DESCS.TryGetValue(type, out var descs)) {
                        foreach (var desc in descs) {
                            sb.Append(AppendTab(2, includeNamespace));
                            sb.AppendFormat(desc.Item2, item.name, item.name);
                            sb.AppendLine();
                        }
                    }
                }
            }

            sb.Append(AppendTab(1, includeNamespace));
            sb.AppendLine("}");
        }

        sb.AppendLine(includeNamespace ? TAB + "}" : "}");

        if (includeNamespace) {
            sb.AppendLine("}");
        }
        
        if (includeNamespace) {
            sb = sb.Replace("[__TAB__]", TAB);
        }
        else {
            sb = sb.Replace("[__TAB__]", "");
        }

        return sb.ToString();
    }

    public string Copy(string clsName, bool includeNamespace) {
        return CSharpCopy(clsName, includeNamespace);
    }

    public void AutoName() {
        string FirstCharUpper(string target) {
            if (!string.IsNullOrWhiteSpace(target) && target.Length > 1) {
                return target[0].ToString().ToUpper() + target.Substring(1);
            }

            return null;
        }

        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var cp = bindComponents[i];
            if (cp != null && cp.component != null) {
                var targetName = cp.component.name;

                // 名字规则
                // 去除空格
                targetName = targetName?.Replace(" ", "");
                // 首字母大写
                targetName = FirstCharUpper(targetName);
                // 添加前缀
                var prefix = cp.componentType.ToLower();
                prefix = prefix.Split(new char[] { '.' })[^1];
                targetName = prefix + targetName;

                cp.name = targetName;
            }
        }
    }

    // 重名检测
    // 命名合理性检测
    // 组件null检测
    public bool Check() {
        bool rlt = true;
        HashSet<string> hashset = new HashSet<string>();
        for (int i = 0, length = bindComponents.Count; i < length; ++i) {
            var name = bindComponents[i].name;
            if (string.IsNullOrEmpty(name)) {
                Debug.LogErrorFormat("index: {0} has empty name", i.ToString());
                rlt = false;
            }
            else {
                name = name.Trim();
                // https://stackoverflow.com/questions/6372318/c-sharp-string-starts-with-a-number-regex
                if (Regex.IsMatch(name, @"^\d")) {
                    Debug.LogErrorFormat("index: {0} start with number", i.ToString());
                }
                else {
                    var component = bindComponents[i].component;
                    if (component == null) {
                        Debug.LogErrorFormat("index: {0} component is null", i.ToString());
                        rlt = false;
                    }
                    else {
                        if (!hashset.Contains(name)) {
                            hashset.Add(name);
                        }
                        else {
                            Debug.LogErrorFormat("index: {0} has same name with already item", i.ToString());
                            rlt = false;
                        }
                    }
                }
            }
        }

        return rlt;
    }
#endif

    public T Find<T>(int index) where T : Component {
        if (index < 0 || index >= bindComponents.Count) {
            Debug.LogErrorFormat("Index: {0} is out of range", index.ToString());
            return null;
        }

        T component = bindComponents[index].component as T;
        if (component == null) {
            Debug.LogErrorFormat("Index: {0} has invalid component {1}", index.ToString(), typeof(T));
            return null;
        }

        return component;
    }

    public static string GetPath(Component component, Component end = null) {
        if (component == null) {
            return null;
        }

        string totalPath = null;
        List<string> paths = new List<string>();
        if (end == null) {
            var cp = component;
            while (cp.transform.parent != null) {
                paths.Add(cp.transform.name);
                cp = cp.transform.parent;
            }
        }
        else {
            var cp = component;
            while (cp.gameObject != end.gameObject) {
                paths.Add(cp.transform.name);
                cp = cp.transform.parent;
            }
        }

        if (paths.Count > 1) {
            paths.Reverse();
            totalPath = string.Join("/", paths);
        }
        else if (paths.Count > 0) {
            totalPath = paths[0];
        }

        return totalPath;
    }
}