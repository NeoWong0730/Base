using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using Rotorz.Games.Collections;
using System.Text;
using System;
using System.IO;
using System.Reflection;

namespace Quick.Code
{
    public class GenerateCodeWindow : EditorWindow
    {
        [MenuItem("__Tools__/QuickGenCode")]
        public static void OpenWindow()
        {
            if (codeWindow == null)
                codeWindow = EditorWindow.GetWindow(typeof(GenerateCodeWindow)) as GenerateCodeWindow;

            Texture2D icon = (Texture2D)EditorGUIUtility.Load(iconPath);
            codeWindow.titleContent = new GUIContent("QuickCode", icon);
            codeWindow.Show();

        }

        private static string iconPath = "Assets/QuickCode/Editor/Images/icon.png";
        private static GenerateCodeWindow codeWindow = null;
        private SerializedObject serializedObj;

        //选择的根游戏体
        private GameObject root;
        //ui控件列表
        private List<Component> uiWidgets = new List<Component>();
        /// <summary>
        /// 控件白名单
        /// </summary>
        private List<Type> whiteWidgetLis = new List<Type>()
        {
            typeof(Button),
            typeof(Dropdown),
            typeof(InputField),
            typeof(Slider),
            typeof(Text),
            typeof(CP_ToggleRegistry),
            typeof(Toggle),
            typeof(InfinityGrid),
            typeof(InfinityIrregularGrid),
            typeof(ScrollRect),
            typeof(Scrollbar),
            typeof(ToggleGroup),
            typeof(Image),
            typeof(RawImage),            
            typeof(RectTransform),            
            //typeof(MonoBehaviour),                       
        };
        //ui游戏对象列表
        private List<GameObject> uiObjects = new List<GameObject>();
        //视图宽度一半
        private float halfViewWidth;
        //视图高度一半
        private float halfViewHeight;

        private Vector2 scrollWidgetPos;
        private Vector2 scrollObjectPos;
        private Vector2 scrollTextPos;

        private int selectedBar = 0;
        private bool isMono = false;

        private bool bFindHasRule = true;

        #region 代码生成
        private StringBuilder codeStateText;
        private StringBuilder codeEventText;
        private StringBuilder codeEventFuncText;
        private StringBuilder codeOutEventText;
        private StringBuilder codeAssignText;
        private StringBuilder codeCallInitText;
        private StringBuilder codeAllText;

        //缓存所有变量名和对应控件对象，对重名作处理
        private Dictionary<string, object> variableNameDic = new Dictionary<string, object>();
        //变量编号
        private int variableNum;
        //需要注册事件的控件,可通过toggle选择
        private Dictionary<string, bool> selectedEventWidgets = new Dictionary<string, bool>();
        //UI 类名
        private string className;
        //生成脚本的类型
        private Type scriptType;
        #endregion

        #region 代码格式分类
        private string regionStartFmt { get { return selectedBar == 0 ? CodeConfig.regionStartFmt : CodeConfig.regionStartFmtLua; } }
        private string regionEnd { get { return selectedBar == 0 ? CodeConfig.regionEnd : CodeConfig.regionEndLua; } }
        private string statementRegion { get { return CodeConfig.statementRegion; } }
        private string eventRegion { get { return selectedBar == 0 ? CodeConfig.eventRegion : CodeConfig.eventRegionLua; } }
        private string assignRegion { get { return selectedBar == 0 ? CodeConfig.assignRegion : CodeConfig.assignRegionLua; } }
        private string methodStartFmt { get { return selectedBar == 0 ? CodeConfig.methodStartFmt : CodeConfig.methodStartFmtLua; } }
        private string methodOverrideEventStartFmt { get { return selectedBar == 0 ? CodeConfig.methodOverrideEventStartFmt : CodeConfig.methodStartFmtLua; } }
        private string methodEnd { get { return selectedBar == 0 ? CodeConfig.methodEnd : CodeConfig.methodEndLua; } }
        private string assignCodeFmt { get { return selectedBar == 0 ? CodeConfig.assignCodeFmt : CodeConfig.assignCodeFmtLua; } }
        private string assignGameObjectCodeFmt { get { return selectedBar == 0 ? CodeConfig.assignGameObjectCodeFmt : CodeConfig.assignGameObjectCodeFmtLua; } }
        private string assignRootCodeFmt { get { return selectedBar == 0 ? CodeConfig.assignRootCodeFmt : CodeConfig.assignRootCodeFmtLua; } }
        private string onClickSerilCode { get { return selectedBar == 0 ? CodeConfig.onClickSerilCode : CodeConfig.onClickSerilCodeLua; } }
        private string onValueChangeSerilCode { get { return selectedBar == 0 ? CodeConfig.onValueChangeSerilCode : CodeConfig.onValueChangeSerilCodeLua; } }
        private string btnCallbackSerilCode { get { return selectedBar == 0 ? CodeConfig.btnCallbackSerilCode : CodeConfig.btnCallbackSerilCodeLua; } }
        private string eventCallbackSerilCode { get { return selectedBar == 0 ? CodeConfig.eventCallbackSerilCode : CodeConfig.eventCallbackSerilCodeLua; } }
        #endregion

        void OnEnable()
        {
            serializedObj = new SerializedObject(this);
        }

        void OnGUI()
        {
            serializedObj.Update();

            if (codeWindow == null)
            {
                codeWindow = GetWindow<GenerateCodeWindow>();
            }
            halfViewWidth = EditorGUIUtility.currentViewWidth / 2f;
            halfViewHeight = codeWindow.position.height / 2f;

            using (new EditorGUILayout.HorizontalScope())
            {
                //左半部分
                using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope(GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f)))
                {
                    GUI.backgroundColor = Color.white;
                    Rect rect = vScope.rect;
                    rect.height = codeWindow.position.height;
                    GUI.Box(rect, "");

                    DrawSelectUI();
                    DrawFindWidget();
                    DrawWidgetList();
                    DrawCustomObjectList();
                }
                //右半部分
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f)))
                {
                    DrawCodeGenTitle();
                    DrawCodeGenToolBar();
                }
            }

            serializedObj.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制 选择要分析的UI
        /// </summary>
        private void DrawSelectUI()
        {
            EditorGUILayout.Space();
            using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
            {
                GUI.backgroundColor = Color.white;
                Rect rect = hScope.rect;
                rect.height = EditorGUIUtility.singleLineHeight;
                GUI.Box(rect, "");

                EditorGUILayout.LabelField("选择待处理UI:", GUILayout.Width(halfViewWidth / 4f));
                GameObject lastRoot = root;
                root = EditorGUILayout.ObjectField(root, typeof(GameObject), true) as GameObject;

                if (lastRoot != null && lastRoot != root)
                {
                    uiWidgets.Clear();
                    uiObjects.Clear();
                }
            }
        }
        bool selectText;
        /// <summary>
        /// 绘制 查找UI控件
        /// </summary>
        private void DrawFindWidget()
        {
            EditorGUILayout.Space();
            using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
            {
                GUI.backgroundColor = Color.white;
                Rect rect = hScope.rect;
                rect.height = EditorGUIUtility.singleLineHeight;
                GUI.Box(rect, "");

                bFindHasRule = EditorGUILayout.Toggle("根据命名规则筛选", bFindHasRule);
                if (GUILayout.Button("查找UI控件", GUILayout.Width(halfViewWidth / 2f)))
                {
                    if (root == null)
                    {
                        Debug.LogWarning("请先选择一个UI物体!");
                        return;
                    }

                    if(bFindHasRule)
                    {
                        RecursiveUI_Rule(root.transform, DoParse_Rule);
                    }
                    else
                    {
                        RecursiveUI(root.transform, DoParse);
                    }                                        
                }

                if (GUILayout.Button("清除控件"))
                {
                    uiWidgets.Clear();
                }
                if (GUILayout.Button("清除其他"))
                {
                    uiObjects.Clear();
                }

            }
        }

        private void DoParse(Transform tran)
        {
            Component[] widgets = tran.GetComponents<Component>();
            for (int i = 0; i < widgets.Length; i++)
            {
                var widget = widgets[i];
                if (widget != null && !uiWidgets.Contains(widget))
                {
                    if (whiteWidgetLis.Contains(widget.GetType()))
                    {
                        uiWidgets.Add(widget);
                    }
                    else
                    {
                        Debug.Log(string.Format("<color=green>忽略控件:{0} widgetName:{1}</color>", widget.GetType(), widget.name));
                        continue;
                    }
                }
            }
        }

        private void DoParse_Rule(Transform tran)
        {
            if(tran.name.StartsWith("@"))
            {
                uiWidgets.Add(tran);
            }
            else if(tran.name.StartsWith("_"))
            {
                for(int i = 0; i < whiteWidgetLis.Count; ++i)
                {
                    Component widget = tran.GetComponent(whiteWidgetLis[i]);
                    if(widget != null)
                    {
                        uiWidgets.Add(widget);
                        break;
                    }
                }                
            }            
        }

        /// <summary>
        /// 绘制 控件列表
        /// </summary>
        private void DrawWidgetList()
        {
            EditorGUILayout.Space();

            ReorderableListGUI.Title("UI控件");
            scrollWidgetPos = EditorGUILayout.BeginScrollView(scrollWidgetPos);
            ReorderableListGUI.ListField<Component>(uiWidgets, DrawWidget);

            EditorGUILayout.EndScrollView();
        }

        private Component DrawWidget(Rect position, Component item)
        {
            item = (Component)EditorGUI.ObjectField(position, item, typeof(Component), true);
            return item;
        }

        /// <summary>
        /// 绘制 其他ui gameobject,比如某些节点要控制下层的隐藏显示
        /// </summary>
        private void DrawCustomObjectList()
        {
            EditorGUILayout.Space();

            ReorderableListGUI.Title("其他UI对象");
            scrollObjectPos = EditorGUILayout.BeginScrollView(scrollObjectPos);
            ReorderableListGUI.ListField<GameObject>(uiObjects, DrawCustomObject);
            EditorGUILayout.EndScrollView();
        }

        private GameObject DrawCustomObject(Rect position, GameObject item)
        {
            item = (GameObject)EditorGUI.ObjectField(position, item, typeof(GameObject), true);
            return item;
        }

        private void DrawCodeGenTitle()
        {
            EditorGUILayout.Space();
            using (var hScope = new EditorGUILayout.HorizontalScope(GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.backgroundColor = Color.white;
                Rect rect = hScope.rect;
                GUI.Box(rect, "");

                EditorGUILayout.LabelField("代码生成:");
            }
        }

        private void DrawCodeGenToolBar()
        {
            EditorGUILayout.Space();

            selectedBar = GUILayout.Toolbar(selectedBar, new string[] { "C#", "Lua" });

            switch (selectedBar)
            {
                case 0:
                    DrawCsPage();
                    break;
                case 1:
                    DrawLuaPage();
                    break;
                default:
                    break;
            }
        }

        private void DrawCsPage()
        {
            EditorGUILayout.Space();
            //isMono = GUILayout.Toggle(isMono, "继承CP_MonoBehaviourBase");
            EditorGUILayout.Space();
            if (GUILayout.Button("变量声明", GUILayout.Width(halfViewWidth / 3f)))
            {
                BuildStatementCode();
            }

            EditorGUILayout.Space();
            using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope())
            {
                GUI.backgroundColor = Color.white;
                GUI.Box(vScope.rect, "");

                EditorGUILayout.LabelField("选择需要注册事件回调的控件:");
                DrawEventWidget();

                EditorGUILayout.Space();
                if (GUILayout.Button("注册事件", GUILayout.Width(halfViewWidth / 3f)))
                {
                    BuildEventCode();
                }
            }

            EditorGUILayout.Space();
            using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("查找赋值"))
                {
                    BuildAssignmentCode();
                    //SelfComponmentInit();
                }

                if (GUILayout.Button("复制代码"))
                {
                    TextEditor p = new TextEditor();
                    codeAllText = new StringBuilder(codeStateText.ToString());
                    codeAllText.Append(codeAssignText);
                    codeAllText.Append(codeCallInitText);
                    codeAllText.Append(codeEventText);
                    codeAllText.Append(codeOutEventText);
                    p.text = codeAllText.ToString();
                    p.OnFocus();
                    p.Copy();

                    EditorUtility.DisplayDialog("提示", "代码复制成功", "OK");
                }
                if (GUILayout.Button("生成脚本"))
                {
                    CreateCsUIScript();
                }
            }

            EditorGUILayout.Space();
            using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
            {
                if (isMono)
                {
                    //if (GUILayout.Button("挂载脚本组件"))
                    //{
                    //    AddScriptComponent();
                    //}
                    //if (GUILayout.Button("绑定UI(无需查找赋值)"))
                    //{
                    //    BindSerializeWidget();
                    //}
                }
            }
            DrawPreviewText();
        }

        private void DrawLuaPage()
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("查找赋值", GUILayout.Width(halfViewWidth / 3f)))
            {
                BuildStatementCode();
                BuildAssignmentCode();
            }

            EditorGUILayout.Space();
            using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope())
            {
                GUI.backgroundColor = Color.white;
                GUI.Box(vScope.rect, "");

                EditorGUILayout.LabelField("选择需要注册事件回调的控件:");
                DrawEventWidget();

                EditorGUILayout.Space();
                if (GUILayout.Button("注册事件", GUILayout.Width(halfViewWidth / 3f)))
                {
                    BuildEventCode();
                }
            }

            EditorGUILayout.Space();
            using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("复制代码"))
                {
                    TextEditor p = new TextEditor();
                    codeAllText = new StringBuilder();
                    codeAllText.Append(codeAssignText);
                    codeAllText.Append(codeCallInitText);
                    codeAllText.Append(codeEventText);
                    codeAllText.Append(codeOutEventText);
                    p.text = codeAllText.ToString();
                    p.OnFocus();
                    p.Copy();

                    EditorUtility.DisplayDialog("提示", "代码复制成功", "OK");
                }
                if (GUILayout.Button("生成脚本"))
                {
                    CreateLuaUIScript();
                }
            }

            DrawPreviewText();
        }

        /// <summary>
        /// 遍历UI
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="callback">回调</param>
        public void RecursiveUI(Transform parent, UnityAction<Transform> callback)
        {
            if (callback != null)
                callback(parent);

            if (parent.childCount >= 0)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    Transform child = parent.GetChild(i);

                    RecursiveUI(child, callback);
                }
            }
        }

        public void RecursiveUI_Rule(Transform parent, UnityAction<Transform> callback)
        {
            if (callback != null)
                callback(parent);

            if (parent.childCount >= 0)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    Transform child = parent.GetChild(i);
                    if (child.name.StartsWith("@"))
                    {
                        if (callback != null)
                            callback(child);
                        continue;
                    }
                    RecursiveUI_Rule(child, callback);
                }
            }
        }

        private string BuildStatementCode()
        {
            variableNum = 0;
            variableNameDic.Clear();

            codeStateText = null;
            codeStateText = new StringBuilder();

            codeStateText.Append(CodeConfig.statementRegion);
            //非mono类声明一个transform
//             if (!isMono)
//             {
//                 codeStateText.Append(CodeConfig.stateTransform);
//             }

            codeStateText.AppendLine("\tpublic GameObject mRoot { get; private set; }");
            codeStateText.AppendLine("\tpublic Transform mTrans { get; private set; }");

            //控件列表
            for (int i = 0; i < uiWidgets.Count; i++)
            {
                if (uiWidgets[i] == null) continue;

                Type type = uiWidgets[i].GetType();
                if (type == null)
                {
                    Debug.LogError("BuildUICode type error !");
                    return "";
                }

                string typeName = type.Name;                
                string widgetName = uiWidgets[i].name;
                if (widgetName.StartsWith("_") || widgetName.StartsWith("@"))
                {
                    widgetName = widgetName.Remove(0, 1);
                }

                string variableName = string.Format("{0}_{1}", widgetName, typeName);
                variableName = variableName.Replace(' ', '_');   //命名有空格的情况       

                //重名处理
                ++variableNum;
                if (variableNameDic.ContainsKey(variableName))
                {
                    variableName += variableNum;
                }
                variableNameDic.Add(variableName, uiWidgets[i]);

                if (isMono)
                {
                    codeStateText.AppendFormat(CodeConfig.serilStateCodeFmt, typeName, variableName);
                }
                else
                {
                    codeStateText.AppendFormat(CodeConfig.stateCodeFmt, typeName, variableName);
                }
            }
            //其他对象列表，目前都是GameObject
            for (int i = 0; i < uiObjects.Count; i++)
            {
                if (uiObjects[i] == null) continue;

                Type type = uiObjects[i].GetType();
                if (type == null)
                {
                    Debug.LogError("BuildUICode type error !");
                    return "";
                }

                string typeName = type.Name;
                string variableName = string.Format("go_{0}", uiObjects[i].name);
                variableName = variableName.Replace(' ', '_');   //命名有空格的情况
                //重名处理
                ++variableNum;
                if (variableNameDic.ContainsKey(variableName))
                {
                    variableName += variableNum;
                }
                variableNameDic.Add(variableName, uiObjects[i]);

                if (isMono)
                {
                    codeStateText.AppendFormat(CodeConfig.serilStateCodeFmt, typeName, variableName);
                }
                else
                {
                    codeStateText.AppendFormat(CodeConfig.stateCodeFmt, typeName, variableName);
                }
            }

            codeStateText.Append(CodeConfig.regionEnd);
            //Debug.Log(codeStateText);
            return codeStateText.ToString();
        }

        private void DrawEventWidget()
        {
            using (EditorGUILayout.HorizontalScope hScope = new EditorGUILayout.HorizontalScope())
            {
                //筛选当前UI的事件控件
                foreach (var elem in Enum.GetValues(typeof(CodeConfig.EventWidgetType)))
                {
                    for (int i = 0; i < uiWidgets.Count; i++)
                    {
                        if (uiWidgets[i] == null) continue;

                        Type type = uiWidgets[i].GetType();
                        if (type == null)
                        {
                            Debug.LogError("BuildUICode type error !");
                            continue;
                        }

                        if (type.Name == elem.ToString() && !selectedEventWidgets.ContainsKey(type.Name))
                        {
                            selectedEventWidgets.Add(type.Name, true);
                        }
                    }
                }

                //绘制toggle,注意不能遍历dic的同时赋值
                List<string> list = new List<string>(selectedEventWidgets.Keys);
                foreach (string wedagetName in list)
                {
                    selectedEventWidgets[wedagetName] = EditorGUILayout.ToggleLeft(wedagetName, selectedEventWidgets[wedagetName],
                        GUILayout.Width(halfViewWidth / 8f));
                }
            }
        }

        /// <summary>
        /// 构建注册控件事件的代码
        /// </summary>
        /// <returns></returns>
        private string BuildEventCode()
        {
            if (codeEventText == null)
            {
                codeEventText = new StringBuilder();
            }

            if (codeEventFuncText == null)
            {
                codeEventFuncText = new StringBuilder();
            }
            codeEventText.Clear();
            codeEventFuncText.Clear();

            codeEventText.AppendFormat(methodStartFmt, "RegisterEvents(IListener listener)");

            codeEventFuncText.AppendFormat(CodeConfig.interfaceMonoStart, "IListener");

            for (int i = 0; i < uiWidgets.Count; i++)
            {
                if (uiWidgets[i] == null) continue;

                //剔除不是事件或者是事件但未勾选toggle的控件
                string typeName = uiWidgets[i].GetType().Name;
                if (!selectedEventWidgets.ContainsKey(typeName) || !selectedEventWidgets[typeName])
                {
                    continue;
                }

                foreach (var vName in variableNameDic.Keys)
                {
                    if (uiWidgets[i].Equals(variableNameDic[vName]))
                    {
                        string variableName = vName;
                        if (!string.IsNullOrEmpty(variableName))
                        {
                            CodeConfig.ComponentInfo info = null;
                            if (CodeConfig.eventCBParamDic.TryGetValue(typeName, out info))
                            {
                                string methodName = variableName.Substring(variableName.IndexOf('_') + 1);
                                string addEventStr = string.Format(info.sListenerFormat, variableName, methodName);
                                codeEventText.AppendLine(addEventStr);

                                string funStr = string.Format(info.sFuncFormat, methodName);

                                codeEventFuncText.AppendLine(funStr);
                            }
                        }
                        break;
                    }
                }
            }

            string codeStr = codeEventText.ToString();
            codeEventText.AppendLine(methodEnd);
            codeEventFuncText.AppendLine(methodEnd);

            codeEventText.Replace("//TODO:Implementation Here", "");
            return codeEventText.ToString();
        }

        /// <summary>
        /// 当前操作生成的代码预览
        /// </summary>
        private void DrawPreviewText()
        {
            EditorGUILayout.Space();

            using (var ver = new EditorGUILayout.VerticalScope())
            {
                GUI.backgroundColor = Color.white;
                GUI.Box(ver.rect, "");

                EditorGUILayout.HelpBox("代码预览:", MessageType.None);
                using (var scr = new EditorGUILayout.ScrollViewScope(scrollTextPos))
                {
                    scrollTextPos = scr.scrollPosition;

                    if (codeStateText != null && !string.IsNullOrEmpty(codeStateText.ToString()) && selectedBar == 0)
                    {
                        //GUILayout.TextArea(codeStateText.ToString());
                        GUILayout.Label(codeStateText.ToString());
                    }

                    if (codeAssignText != null && !string.IsNullOrEmpty(codeAssignText.ToString()))
                    {
                        GUILayout.Label(codeAssignText.ToString());
                    }

                    if (codeCallInitText != null && !string.IsNullOrEmpty(codeCallInitText.ToString()))
                    {
                        GUILayout.Label(codeCallInitText.ToString());
                    }
                    if (codeEventText != null && !string.IsNullOrEmpty(codeEventText.ToString()))
                    {
                        //GUILayout.TextArea(codeEventText.ToString());
                        GUILayout.Label(codeEventText.ToString());
                    }
                    if (codeEventFuncText != null && codeEventFuncText.Length > 0)
                    {
                        //GUILayout.TextArea(codeEventText.ToString());
                        GUILayout.Label(codeEventFuncText.ToString());
                    }
                    if (codeOutEventText != null && !string.IsNullOrEmpty(codeOutEventText.ToString()))
                    {
                        GUILayout.Label(codeOutEventText.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 生成C# UI脚本
        /// </summary>
        private void CreateCsUIScript()
        {
            string path = EditorPrefs.GetString("create_script_folder", "");
            path = EditorUtility.SaveFilePanel("Create Script", path, root.name + "_Layout.cs", "cs");
            if (string.IsNullOrEmpty(path)) return;

            int index = path.LastIndexOf('/');
            className = path.Substring(index + 1, path.LastIndexOf('.') - index - 1);

            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.Append(string.Format(CodeConfig.codeAnnotation, System.Environment.UserName, System.DateTime.Now.ToLocalTime(), "UI Componments Data Binding"));
            scriptBuilder.Append(CodeConfig.usingNamespace);

            scriptBuilder.AppendLine("namespace Logic");
            scriptBuilder.AppendLine("{");

            if (isMono)
            {
                scriptBuilder.AppendFormat(CodeConfig.classMonoStart, className);
            }
            else
            {                
                scriptBuilder.AppendFormat(CodeConfig.classStart, className);
            }
            scriptBuilder.Append(codeStateText);
            scriptBuilder.Append(codeAssignText);
            scriptBuilder.Append(codeCallInitText);
            scriptBuilder.Append(codeEventText);
            scriptBuilder.Append(codeEventFuncText);
            scriptBuilder.Append(codeOutEventText);
            scriptBuilder.AppendLine(CodeConfig.classEnd);
            scriptBuilder.AppendLine("}");

            File.WriteAllText(path, scriptBuilder.ToString(), new UTF8Encoding(false));
            AssetDatabase.Refresh();

            Debug.LogFormat("<color=yellow>脚本生成成功,生成路径为:{0}</color>", path);
            EditorPrefs.SetString("create_script_folder", path);
        }

        /// <summary>
        /// 生成Lua UI脚本
        /// </summary>
        private void CreateLuaUIScript()
        {
            string path = EditorPrefs.GetString("create_script_folder_lua", "");
            path = EditorUtility.SaveFilePanel("Create lua Script", path, root.name + ".lua", "lua");
            if (string.IsNullOrEmpty(path)) return;

            int index = path.LastIndexOf('/');
            className = path.Substring(index + 1, path.LastIndexOf('.') - index - 1);

            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.Append(CodeConfig.codeAnnotationLua);
            scriptBuilder.Append(CodeConfig.requireCode);
            scriptBuilder.AppendFormat(CodeConfig.classStartLua, className, className);
            scriptBuilder.AppendFormat(CodeConfig.classCtorLua, className);
            scriptBuilder.Append(codeAssignText);
            scriptBuilder.Append(codeEventText);
            scriptBuilder.Append(codeOutEventText);
            scriptBuilder.AppendFormat(CodeConfig.classEndLua, className);

            File.WriteAllText(path, scriptBuilder.ToString().Replace("Class", className), new UTF8Encoding(false));
            AssetDatabase.Refresh();

            Debug.Log("脚本生成成功,生成路径为:" + path);
            EditorPrefs.SetString("create_script_folder_lua", path);
        }

        /// <summary>
        /// 在根物体上挂载生成的脚本(必须继承monobehavior)
        /// </summary>
        private void AddScriptComponent()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("警告", "请等待编辑器完成编译再执行此操作", "OK");
                return;
            }

            if (root == null || string.IsNullOrEmpty(className))
            {
                EditorUtility.DisplayDialog("警告", "请先按顺序生成UI脚本再执行此操作", "OK");
                return;
            }

            //通过Assembly-CSharp程序集挂载脚本
            Assembly[] AssbyCustmList = System.AppDomain.CurrentDomain.GetAssemblies();
            Assembly asCSharp = null;
            for (int i = 0; i < AssbyCustmList.Length; i++)
            {
                string assbyName = AssbyCustmList[i].GetName().Name;
                if (assbyName == "Assembly-CSharp")
                {
                    asCSharp = AssbyCustmList[i];
                    break;
                }
            }

            scriptType = asCSharp.GetType(className);
            if (scriptType == null)
            {
                EditorUtility.DisplayDialog("警告", "挂载失败，请先检查脚本是否正确生成", "OK");
                return;
            }

            var target = root.GetComponent(scriptType);
            if (target == null)
            {
                target = root.AddComponent(scriptType);
            }
        }
        /// <summary>
        /// 给UI变量绑定实例
        /// </summary>
        private void BindSerializeWidget()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("警告", "请等待编辑器完成编译再执行此操作", "OK");
                return;
            }

            if (root == null || string.IsNullOrEmpty(className))
            {
                EditorUtility.DisplayDialog("警告", "请先按顺序生成UI脚本再执行此操作", "OK");
                return;
            }

            if (scriptType == null)
            {
                EditorUtility.DisplayDialog("警告", "请先按顺序挂载UI脚本再执行此操作", "OK");
                return;
            }

            object target = null;
            target = root.GetComponent(scriptType);
            if (target == null)
            {
                target = root.AddComponent(scriptType);
            }

            //奇怪，资源刷新以后variableNameDic被清空了，先再获取一遍
            BuildStatementCode();

            foreach (var name in variableNameDic.Keys)
            {
                if (string.IsNullOrEmpty(name)) continue;
                scriptType.InvokeMember(name,
                                BindingFlags.SetField |
                                BindingFlags.Instance |
                                BindingFlags.NonPublic,
                                null, target, new object[] { variableNameDic[name] }, null, null, null);
            }
        }


        private void BuildAssignmentCode()
        {
            codeAssignText = new StringBuilder();
            codeAssignText.AppendFormat(methodStartFmt, "Parse(GameObject root)");
            codeAssignText.AppendLine("\t\tmRoot = root;");
            codeAssignText.AppendLine("\t\tmTrans = root.transform;");

            var allPath = GetChildrenPaths(root);

            if (variableNameDic == null)
            {
                return;
            }

            //格式：变量名 = transform.Find("").Getcomponent<>();
            foreach (var name in variableNameDic.Keys)
            {
                var obj = variableNameDic[name];
                if (obj == null) continue;

                string path = "";
                bool isRootComponent = false;
                foreach (var tran in allPath.Keys)
                {
                    if (tran == null) continue;

                    Component behav = obj as Component;
                    if (behav != null)
                    {
                        //判断是否挂在根上，根上不需要路径
                        isRootComponent = behav.gameObject == root;
                        if (isRootComponent) break;

                        if (behav.gameObject == tran.gameObject)
                        {
                            path = allPath[tran];
                            break;
                        }
                    }
                    else
                    {
                        if (tran.gameObject == (UnityEngine.Object)obj)
                        {
                            path = allPath[tran];
                            break;
                        }
                    }
                }

                if (obj is GameObject)
                {
                    codeAssignText.AppendFormat(assignGameObjectCodeFmt, name, path);
                }
                else
                {
                    if (isRootComponent)
                    {
                        codeAssignText.AppendFormat(assignRootCodeFmt, name, obj.GetType().Name);
                    }
                    else
                    {
                        codeAssignText.AppendFormat(assignCodeFmt, name, path, obj.GetType().Name);
                    }
                }
            }

            codeAssignText.AppendLine(methodEnd);
            codeAssignText.AppendLine();
        }

        private void SelfComponmentInit()
        {
            codeCallInitText = new StringBuilder();
            codeCallInitText.Append(CodeConfig.initComponentCode);
        }


        private Dictionary<Transform, string> GetChildrenPaths(GameObject rootGo)
        {
            Dictionary<Transform, string> pathDic = new Dictionary<Transform, string>();
            string path = string.Empty;
            Transform[] tfArray = rootGo.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < tfArray.Length; i++)
            {
                Transform node = tfArray[i];

                string str = node.name;
                while (node.parent != null && node.gameObject != rootGo && node.parent.gameObject != rootGo)
                {
                    str = string.Format("{0}/{1}", node.parent.name, str);
                    node = node.parent;
                }
                path += string.Format("{0}\n", str);

                if (!pathDic.ContainsKey(tfArray[i]))
                {
                    pathDic.Add(tfArray[i], str);
                }
            }
            //Debug.Log(path);

            return pathDic;
        }

    }
}
