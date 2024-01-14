using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class FoolCreateWorkStreamWindow : EditorWindow
{
    private Vector2 _scrollPos;

    private bool _isNeedWrite;
    protected GUIStyle _textSt;

    private string _workStreamName;
    private string _temporaryEditorFolderPath;
    private string _saveFullWorkStreamDataFolderPath;
    private string _saveRelativeWorkStreamDataFile;
    private string _saveCsFolderPath;
    private int _blockStartIndex = 5001;

    private string _stateCategoryEnumFile;
    private string _workStreamWindowPath;

    private bool _isCanOverride;

    private StringBuilder _stringBuilder = new StringBuilder();

    [MenuItem("Tools/工作流WorkStream/自定义WorkStream")]
    public static void OpenFoolCreateWorkStreamWindow()
    {
        GetWindow<FoolCreateWorkStreamWindow>(false, "自定义WorkStream", true);
    }

    private void OnInspectorUpdate()
    {
        //开启窗口的重绘，不然窗口信息不会刷新
        Repaint();
    }

    private void OnGUI()
    {
        if (_textSt == null) 
        {
            _textSt = new GUIStyle(EditorStyles.label);
            _textSt.fontSize = 12;
            _textSt.normal.textColor = Color.red;
            _textSt.focused.textColor = Color.red;
        }

        _isNeedWrite = false;

        GUILayout.BeginVertical();

        using (GUILayout.ScrollViewScope svs = new GUILayout.ScrollViewScope(_scrollPos)) 
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("命名(文件,脚本,窗口以该名来拼凑，很重要)");
            GUILayout.Space(20f);
            _isCanOverride = GUILayout.Toggle(_isCanOverride, "可以覆盖已有的类型");
            GUILayout.EndHorizontal();
            _workStreamName = GUILayout.TextField(_workStreamName);
            DrawRedTips(string.IsNullOrEmpty(_workStreamName));

            GUILayout.Label("策划保存临时文件的文件夹（可以使用默认的路径）");
            GUILayout.BeginHorizontal();
            _temporaryEditorFolderPath = GUILayout.TextField(_temporaryEditorFolderPath);
            if (string.IsNullOrEmpty(_temporaryEditorFolderPath))
            {
                _temporaryEditorFolderPath = $"Assets/../../Designer_Editor/WorkStreamData";
            }
            if (GUILayout.Button("选择文件夹", GUILayout.Width(100f))) 
            {
                string saveAsFolderPath = EditorUtility.SaveFolderPanel("Select Path", "Assets/", "");
                if (!string.IsNullOrEmpty(saveAsFolderPath))
                {
                    int assetsIndex = saveAsFolderPath.IndexOf("Assets");
                    if (assetsIndex > -1 && (saveAsFolderPath.Length > (assetsIndex + 7)))
                        _temporaryEditorFolderPath = $"Assets/{saveAsFolderPath.Substring(assetsIndex + 7)}";
                    else
                        EditorUtility.DisplayDialog("提示", "请选择Unity项目Assets文件夹下的文件夹", "OK");
                }
            }
            GUILayout.EndHorizontal();
            DrawRedTips(string.IsNullOrEmpty(_temporaryEditorFolderPath));

            GUILayout.Label("生成最终数据的文件夹--游戏中需要读取的数据（可以使用默认的路径）");
            GUILayout.BeginHorizontal();
            _saveFullWorkStreamDataFolderPath = GUILayout.TextField(_saveFullWorkStreamDataFolderPath);
            if (string.IsNullOrEmpty(_saveFullWorkStreamDataFolderPath))
            {
                _saveFullWorkStreamDataFolderPath = $"Assets/Config/WorkStreamData";
                _saveRelativeWorkStreamDataFile = _saveFullWorkStreamDataFolderPath.Substring(_saveFullWorkStreamDataFolderPath.IndexOf("Assets") + 7);
            }
            if (GUILayout.Button("选择文件夹", GUILayout.Width(100f)))
            {
                string saveAsFolderPath = EditorUtility.SaveFolderPanel("Select Path", $"Assets/", "");
                if (!string.IsNullOrEmpty(saveAsFolderPath))
                {
                    int assetsIndex = saveAsFolderPath.IndexOf("Assets");
                    if (assetsIndex > -1 && (saveAsFolderPath.Length > (assetsIndex + 7)))
                    {
                        string folder = saveAsFolderPath.Substring(assetsIndex + 7);
                        _saveFullWorkStreamDataFolderPath = $"Assets/{saveAsFolderPath.Substring(assetsIndex + 7)}";
                        _saveRelativeWorkStreamDataFile = folder;
                    }
                    else
                        EditorUtility.DisplayDialog("提示", "请选择Unity项目Assets文件夹下的文件夹", "OK");
                }
            }
            GUILayout.EndHorizontal();
            DrawRedTips(string.IsNullOrEmpty(_saveFullWorkStreamDataFolderPath));
            _saveRelativeWorkStreamDataFile =  GUILayout.TextField(_saveRelativeWorkStreamDataFile);

            GUILayout.Label($"存放代码的文件夹{(string.IsNullOrWhiteSpace(_workStreamName) ? null : $"（自动生成的[{_workStreamName}]文件夹的父文件夹）")}");
            GUILayout.BeginHorizontal();
            _saveCsFolderPath = GUILayout.TextField(_saveCsFolderPath);
            if (string.IsNullOrEmpty(_saveCsFolderPath))
            {
                _saveCsFolderPath = $"Assets/Scripts/Logic/WorkStream";
            }
            if (GUILayout.Button("选择文件夹", GUILayout.Width(100f)))
            {
                string saveAsFolderPath = EditorUtility.SaveFolderPanel("Select Path", $"Assets/", "");
                if (!string.IsNullOrEmpty(saveAsFolderPath))
                {
                    int assetsIndex = saveAsFolderPath.IndexOf("Assets");
                    if (assetsIndex > -1 && (saveAsFolderPath.Length > (assetsIndex + 7)))
                        _saveCsFolderPath = saveAsFolderPath.Substring(assetsIndex);
                    else
                        EditorUtility.DisplayDialog("提示", "请选择Unity项目Assets文件夹下的文件夹", "OK");
                }
            }
            GUILayout.EndHorizontal();
            DrawRedTips(string.IsNullOrEmpty(_saveCsFolderPath));

            GUILayout.Label("自定义枚举中块从哪个数字开始(默认0-5000为节点，5001-5001+为块)(WorkStream由“块”组成，”块“由”节点“组成)");
            _blockStartIndex = EditorGUILayout.IntField(_blockStartIndex);
            DrawRedTips(_blockStartIndex <= 0);
            
            GUILayout.Space(20f);
            if (GUILayout.Button("生成")) 
            {
                if (_isNeedWrite)
                {
                    EditorUtility.DisplayDialog("提示", "需填写必填项", "OK");
                }
                else 
                {
                    if (!_isCanOverride)
                    {
                        foreach (var item in Enum.GetValues(typeof(StateCategoryEnum)))
                        {
                            if (item.ToString().ToLower() == _workStreamName.ToLower())
                            {
                                EditorUtility.DisplayDialog("提示", $"StateCategoryEnum已经存在{_workStreamName}枚举", "OK");
                                return;
                            }
                        }
                    }
                    
                    StartCreateWorkStream();
                }
            }

            GUILayout.Space(20f);
            if (string.IsNullOrEmpty(_stateCategoryEnumFile) || GUILayout.Button("刷新")) 
            {
                _stateCategoryEnumFile = EditorToolHelp.FindFile(Application.dataPath, "StateCategoryEnum", "*.cs");
                if (string.IsNullOrEmpty(_stateCategoryEnumFile))
                {
                    _stateCategoryEnumFile = $"{Application.dataPath}/WorkStream/StateCategoryEnum.cs";
                }

                _workStreamWindowPath = EditorToolHelp.FindFile(Application.dataPath, "WorkStreamWindow", "*.cs");
                if (string.IsNullOrEmpty(_workStreamWindowPath))
                {
                    _workStreamWindowPath = $"{Application.dataPath}/WorkStream";
                }
                else 
                {
                    _workStreamWindowPath = $"{Path.GetDirectoryName(_workStreamWindowPath)}".Replace("\\", "/");
                }
            }
            GUILayout.Label(_stateCategoryEnumFile);
            GUILayout.Label($"{_workStreamWindowPath}/WS_{_workStreamName}Window");

            _scrollPos = svs.scrollPosition;
        }

        GUILayout.EndVertical();
    }

    private void DrawRedTips(bool isNeedWrite) 
    {
        if (isNeedWrite) 
        {
            GUILayout.Label("*该项必须填*", _textSt);
            _isNeedWrite = true;
        }
    }

    private void StartCreateWorkStream() 
    {
        CreateStateCategoryEnum();
        CreateWorkStream();
        CreateWindow();

        AssetDatabase.Refresh();
    }

    private void CreateStateCategoryEnum() 
    {
        _stringBuilder.Clear();

        List<string> stateCategoryEnumList = new List<string>();
        foreach (var item in Enum.GetValues(typeof(StateCategoryEnum)))
        {
            stateCategoryEnumList.Add(item.ToString());
        }

        bool isExist = false;
        _stringBuilder.Append($"public enum StateCategoryEnum\n{"{"}\n");
        foreach (var item in stateCategoryEnumList)
        {
            _stringBuilder.Append($"\t{item},\n");
            if (item.ToString() == _workStreamName)
                isExist = true;
        }
        if(!isExist)
            _stringBuilder.Append($"\t{_workStreamName}\n");
        _stringBuilder.Append($"{"}"}");

        EditorToolHelp.CreateText(_stateCategoryEnumFile, _stringBuilder.ToString());
    }

    private void CreateWindow() 
    {
        _stringBuilder.Clear();

        string csName = $"WS_{_workStreamName}Window";
        string ppRefreshWindow = $"Open{csName}";
        string wsEnmu = $"{_workStreamName}Enum";

        //WorkStreamWindow
        _stringBuilder.Append(string.Concat(new string[]
        {
            "using System.Collections.Generic;\n",
            "using System.IO;\n",
            "using System.Linq;\n",
            "using UnityEditor;\n",
            "using UnityEngine;\n\n",
            $"public class {csName} : WorkStreamWindow\n{"{"}\n\t",
            $"public static {csName} _window;\n\n\t",
            $"[MenuItem(\"Tools/工作流WorkStream/Open/{_workStreamName}\")]\n\t",
            $"public static void Open{csName}()\n\t{"{"}\n\t\t",
            $"_window = GetWindow<{csName}>(false, \"{csName}\", true);\n\t\t",
            "ShowWorkStream();\n\t\t",
            $"PlayerPrefs.SetInt(\"{ppRefreshWindow}\", 1);\n\t{"}"}\n\n\t",
            $"public static void ShowWorkStream()\n\t{"{"}\n\t\t",
            $"_window.Clear();\n\t\t_window.Show();\n\t\t",
            $"_window.WorkStreamTypeKey = \"{csName}Type\";\n\t\t",
            $"_window.SavePath = \"{_temporaryEditorFolderPath}/{_workStreamName}{"{0}"}/\";\n\t\t",
            $"_window.SaveAllPath = \"{_saveFullWorkStreamDataFolderPath}/{_workStreamName}{"{0}"}/{_workStreamName}.txt\";\n\t\t",
            $"_window.SetHelpToolData(", 
            $"\"{_saveCsFolderPath}/{_workStreamName}/{_workStreamName}Block/\"{","} " +
            $"\"{_saveCsFolderPath}/{_workStreamName}/{_workStreamName}Node/\"{","} " +
            $"\"WS_{_workStreamName}_{"{0}"}_SComponent\"{","} " +
            $"(int)StateCategoryEnum.{_workStreamName}{","} " +
            $"typeof({wsEnmu}){","} " +
            $"{_blockStartIndex});\n\n\t\t",
            $"_window.SetWorkStreamEnum<{wsEnmu}>({_blockStartIndex});\n\n\t\t",
            $"_window.RefreshWorkData();\n\t{"}"}\n\n\t" +
            $"[UnityEditor.Callbacks.DidReloadScripts]\n\t" +
            $"public static void Refresh()\n\t{"{"}\n\t\t" +
            $"bool isOpen = false;\n\t\t" +
            $"if (PlayerPrefs.HasKey(\"{ppRefreshWindow}\"))\n\t\t\t" +
            $"isOpen = PlayerPrefs.GetInt(\"{ppRefreshWindow}\") == 1 ? true : false;\n\n\t\t" +
            $"if (!isOpen)\n\t\t\t" +
            $"return;\n\n\t\t" +
            $"if (_window == null)\n\t\t{"{"}\n\t\t\t" +
            $"Debug.Log(\"已打开Window，再打开\");\n\t\t\t" +
            $"Open{csName}();\n\t\t{"}"}\n\t\telse\n\t\t{"{"}\n\t\t\t" +
            $"Debug.Log(\"已打开Window，重新刷新\");\n\t\t\t" +
            $"ShowWorkStream();\n\t\t{"}"}\n\t{"}"}\n\n\t" +
            $"public void OnDestroy()\n\t{"{"}\n\t\t" +
            $"PlayerPrefs.SetInt(\"{ppRefreshWindow}\", 0);\n\t{"}"}\n\n\t" +
            $"public override void RefreshWorkData()\n\t{"{"}\n\t\t" +
            $"base.RefreshWorkData();\n\t{"}"}\n\n\t" +
            $"public override void RefreshWorkStreamEnum()\n\t{"{"}\n\t\t" +
            $"base.RefreshWorkStreamEnum();\n\n\t\t" +
            $"SetWorkStreamEnum<{wsEnmu}>({_blockStartIndex});\n\t{"}"}\n\n{"}"}",
        }));

        EditorToolHelp.CreateText($"{_workStreamWindowPath}/{csName}.cs", _stringBuilder.ToString());
    }

    private void CreateWorkStream() 
    {
        _stringBuilder.Clear();

        string p = $"{_saveCsFolderPath}/{_workStreamName}";
        if (!Directory.Exists(p)) 
        {
            Directory.CreateDirectory(p);
        }

        //ControllerEntity
        string controllerEntityName = $"WS_{_workStreamName}ControllerEntity";
        _stringBuilder.Append(
            $"using UnityEngine;" +
            $"using System.Collections.Generic;\n\n" +
            $"[StateController((int)StateCategoryEnum.{_workStreamName}, \"{_saveRelativeWorkStreamDataFile}/{_workStreamName}{"{0}"}/{_workStreamName}.txt\")]\n" +
            $"public class {controllerEntityName} : BaseStreamControllerEntity\n{"{"}\n\t" +
            $"public override bool PrepareControllerEntity(WorkStreamManagerEntity workStreamManagerEntity, uint workId, List<WorkBlockData> workBlockDatas, ulong uid = 0)\n\t{"{"}\n\t\t" +
            $"Debug.Log({"$"}\"<color=yellow>{"{"}Id.ToString(){"}"}--{_workStreamName}Controller初始化</color>\");\n\n\t\t" +
            $"if (m_WorkStreamManagerEntity != workStreamManagerEntity)\n\t\t{"{"}\n\t\t\t" +
            $"m_WorkStreamManagerEntity = workStreamManagerEntity;\n\t\t\t" +
            $"OnInit((int)StateCategoryEnum.{_workStreamName});\n\t\t{"}"}\n\n\t\t" +
            $"if (!Prepare{_workStreamName}(workId, workBlockDatas))\n\t\t{"{"}\n\t\t\t" +
            $"OnOver(false);\n\t\t\t" +
            $"return false;\n\t\t{"}"}\n\n\t\t" +
            $"return true;\n\t{"}"}\n\n\t" +
            $"public override void Dispose()\n\t{"{"}\n\t\t" +
            $"Debug.Log({"$"}\"<color=yellow>{"{"}Id.ToString(){"}"}--{_workStreamName}Controller结束</color>\");\n\n\t\t" +
            $"base.Dispose();\n\t{"}"}\n\n\t" +
            $"public bool Prepare{_workStreamName}(uint workId, List<WorkBlockData> workBlockDatas, int blockType = 0)\n\t{"{"}\n\t\t" +
            $"if (workBlockDatas == null || workBlockDatas.Count == 0)\n\t\t\t" +
            $"return false;\n\n\t\t" +
            $"if (m_FirstMachine != null)\n\t\t\t" +
            $"m_FirstMachine.Dispose();\n\n\t\t" +
            $"m_FirstMachine = CreateFirstStateMachineEntity();\n\t\t" +
            $"WorkStreamTranstionComponent workStreamTranstionComponent = m_FirstMachine.AddTranstion<WorkStreamTranstionComponent>();\n\t\t" +
            $"workStreamTranstionComponent.InitWorkBlockDatas(workId, workBlockDatas);\n\t\t" +
            $"m_ControllerBeginAction?.Invoke(this);\n\n\t\t" +
            $"return true;\n\t{"}"}\n\n\t" +
            $"/// <summary>\n\t/// 自定义专属初始化函数，参数根据情况自定义添加\n\t/// 也可以依此拓展自己定义的函数\n\t/// </summary>\n\t" +
            $"public bool DoInit() \n\t{"{"}\n\t\t" +
            $"return true;\n\t{"}"}\n" +
            $"{"}"}"
        );

        EditorToolHelp.CreateText($"{p}/{controllerEntityName}.cs", _stringBuilder.ToString());

        _stringBuilder.Clear();

        //DataComponent
        _stringBuilder.Append(
            $"using System.Collections.Generic;\n" +
            $"using UnityEngine;\n\n" +
            $"public class WS_{_workStreamName}DataComponent : BaseComponent<StateMachineEntity>\n{"{"}\n{"}"}"
        );

        EditorToolHelp.CreateText($"{p}/WS_{_workStreamName}DataComponent.cs", _stringBuilder.ToString());

        _stringBuilder.Clear();

        string wsEnmu = $"{ _workStreamName }Enum";
        //enum
        _stringBuilder.Append($"using System.ComponentModel;\n\n" +
            $"public enum {wsEnmu}\n{"{"}\n\t" +
            $"None = 0,\n\n\t" +
            $"#region 1-{_blockStartIndex - 1}是node节点枚举\n\n\t" +
            $"#endregion\n\n\t" +
            $"#region {_blockStartIndex}+是block枚举\n\n\t" +
            $"#endregion\n" +
            $"{"}"}"
        );

        EditorToolHelp.CreateText($"{p}/{wsEnmu}.cs", _stringBuilder.ToString());

        _stringBuilder.Clear();

        string managerClassName = $"WS_{_workStreamName}ManagerEntity";
        //ManagerEntity
        _stringBuilder.Append(
            $"using System;\n" +
            $"using UnityEngine;\n\n" +
            $"public class {managerClassName} : WorkStreamManagerEntity\n{"{"}\n\t" +
            $"#region 示例\n\t" +
            $"public static {managerClassName} Start{_workStreamName}<T>(uint workId, int attachType = 0, \n\t\t" +
            $"SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream, \n\t\t" +
            $"Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, bool isSelfDestroy = true) where T : BaseStreamControllerEntity\n\t{"{"}\n\t\t" +
            $"{managerClassName} me = CreateWorkStreamManagerEntity<{managerClassName}>(isSelfDestroy);\n\t\t" +
            $"if (me.StartController<T>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction))\n\t\t\t" +
            $"return me;\n\t\t" +
            $"else\n\t\t{"{"}\n\t\t\t" +
            $"me.Dispose();\n\t\t\t" +
            $"return null;\n\t\t{"}"}\n\t{"}"}\n\n\t" +
            $"public static {managerClassName} Start{_workStreamName}02<T>(uint workId, int attachType = 0, \n\t\t" +
            $"SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream, \n\t\t" +
            $"Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, bool isSelfDestroy = true) where T : BaseStreamControllerEntity\n\t{"{"}\n\t\t" +
            $"{managerClassName} me = CreateWorkStreamManagerEntity<{managerClassName}>(isSelfDestroy);\n\t\t" +
            $"T t = me.CreateController<T>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction);\n\t\t" +
            $"if (t != null && t.StartController())\n\t\t\t" +
            $"return me;\n\t\t" +
            $"else\n\t\t{"{"}\n\t\t\t" +
            $"me.Dispose();\n\t\t\t" +
            $"return null;\n\t\t{"}"}\n\t{"}"}\n\n\t" +
            $"public static {managerClassName} Start{_workStreamName}03<T>(uint workId, int attachType = 0, \n\t\t" +
            $"SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream, \n\t\t" +
            $"Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, bool isSelfDestroy = true) where T : BaseStreamControllerEntity\n\t{"{"}\n\t\t" +
            $"{managerClassName} me = CreateWorkStreamManagerEntity<{managerClassName}>(isSelfDestroy);\n\t\t" +
            $"{controllerEntityName} t = me.CreateController<{controllerEntityName}>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction);\n\t\t" +
            $"if (t != null)\n\t\t{"{"}\n\t\t\t" +
            $"if (t.DoInit()) \n\t\t\t{"{"}\n\t\t\t\t" +
            $"if (t.StartController()) \n\t\t\t\t{"{"}\n\t\t\t\t\t" +
            $"return me;\n\t\t\t\t{"}"}\n\t\t\t{"}"}\n\t\t{"}"}\n\n\t\t" +
            $"me.Dispose();\n\t\t" +
            $"return null;\n\t{"}"}\n\n\t" +
            $"#endregion\n" +
            $"{"}"}"
        );

        EditorToolHelp.CreateText($"{p}/{managerClassName}.cs", _stringBuilder.ToString());
    }
}
