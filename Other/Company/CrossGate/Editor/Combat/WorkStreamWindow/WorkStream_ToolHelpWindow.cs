using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public partial class WorkStreamWindow
{
    #region HelpTool
    public string m_CreateCSFileBlockPath;
    public string m_CreateCSFileNodePath;
    public string m_CreateCSFileName;
    public int m_StateCategory;
    public Type m_DefineStateType;
    public int m_BlockStartIndex;

    private string _commonCSWorkStreamName = "WS_Common_{0}__SComponent";
    private string _commonNodePath = "Assets/Scripts/Logic/WorkStream/CommonWorkStream/CommonNode/";
    private string _commonBlockPath = "Assets/Scripts/Logic/WorkStream/CommonWorkStream/CommonBlock/";

    private static StringBuilder _stringBuilder = new StringBuilder();

    private string[] _workTypeStrs = new string[] { "Block", "Node" };

    private List<int> _existWorkEnums = new List<int>();
    private List<int> _noExistWorkEnums = new List<int>();

    private Vector2 _helpToolScrollPos;
    
    protected string _workStreamEnumContentNotePath = "Assets/../../Designer_Editor/WorkStreamData/WorkStreamEnumContentNote";

    private int _selectToolMenu;
    private string[] _toolHelpMenuStrs = new string[] { "生成节点文件", "节点内容注释", "操作WorkStream内容", "GM命令" };

    private void DrawHelpToolLeft() 
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(5);
        _selectToolMenu = GUILayout.SelectionGrid(_selectToolMenu, _toolHelpMenuStrs, 1);
        EditorGUILayout.EndVertical();
    }

    private void DrawHelpToolRight()
    {
        EditorGUILayout.BeginVertical();
        switch (_selectToolMenu) 
        {
            case 0:
                DrawWorkStreamEnumCS();
                break;

            case 1:
                //ParseWorkStreamEnumContentNote();
                DrawWorkStreamNodeContentNote();
                break;

            case 2:
                OperationWorkStreamContent();
                break;

            case 3:
                DoGM();
                break;
        }
        EditorGUILayout.EndVertical();
    }

    #region 生成节点文件
    private void DrawWorkStreamEnumCS()
    {
        _existWorkEnums.Clear();
        _noExistWorkEnums.Clear();
        Type[] types = typeof(StateBaseComponent).Assembly.GetTypes();
        foreach (var type in types)
        {
            object[] attrs = type.GetCustomAttributes(typeof(StateComponentAttribute), false);
            if (attrs.Length == 0)
                continue;

            foreach (var attr in attrs)
            {
                StateComponentAttribute sca = (StateComponentAttribute)attr;

                int stateCategory = sca.m_StateCategory;
                if (m_StateCategory != stateCategory)
                    continue;

                int defineState = sca.m_DefineState;

                _existWorkEnums.Add(defineState);
            }
        }

        Rect rect = new Rect(_dragAreaEditor.Width + _dragAreaEditor.StartX, _dragAreaEditor.StartY, Screen.width - (_dragAreaEditor.Width + _dragAreaEditor.StartX), Screen.height);

        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(Screen.height - 30f)))
        {
            using (EditorGUILayout.ScrollViewScope svs = new EditorGUILayout.ScrollViewScope(_helpToolScrollPos))
            {
                EditorGUILayout.Space(10f);

                if (m_DefineStateType == null)
                {
                    Close();
                    return;
                }

                foreach (var item in Enum.GetValues(m_DefineStateType))
                {
                    string enumName = item.ToString();
                    int enumInt = (int)item;
                    if (enumInt == 0)
                        continue;

                    FieldInfo fieldInfo = m_DefineStateType.GetField(enumName);
                    if (fieldInfo == null)
                        continue;

                    object[] attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attrs == null || attrs.Length <= 0)
                        continue;

                    bool isExist = false;
                    for (int i = 0; i < _existWorkEnums.Count; i++)
                    {
                        if (_existWorkEnums[i] == enumInt)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (isExist)
                        continue;

                    EditorGUILayout.Space(5f);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.Popup(enumInt < m_BlockStartIndex ? 1 : 0, _workTypeStrs, _popupSt02, GUILayout.Width(150f));

                        int blockIndex = GetIndexByEnum(enumInt, enumInt < m_BlockStartIndex ? WorkNodeEnumDic : WorkBlockEnumDic);
                        EditorGUILayout.Popup(blockIndex, enumInt < m_BlockStartIndex ? WorkNodeEnumStrs : WorkBlockEnumStrs, blockIndex == 0 ? _popupSt : _popupSt02);

                        if (GUILayout.Button("生成独有代码", GUILayout.Width(100f)))
                        {
                            _stringBuilder.Clear();

                            CreateStateBaseComponentCS(enumInt);
                        }

                        if (GUILayout.Button("生成公用代码", GUILayout.Width(100f)))
                        {
                            _stringBuilder.Clear();

                            CreateStateBaseComponentCS(enumInt, true);
                        }

                        _noExistWorkEnums.Add(enumInt);
                    }
                }

                EditorGUILayout.Space(10f);

                if (_noExistWorkEnums.Count > 0)
                {
                    if (GUILayout.Button("一键生成代码", GUILayout.Width(100f)))
                    {
                        for (int i = 0; i < _noExistWorkEnums.Count; i++)
                        {
                            _stringBuilder.Clear();

                            CreateStateBaseComponentCS(_noExistWorkEnums[i]);
                        }
                    }
                }

                EditorGUILayout.Space(40f);

                _helpToolScrollPos = svs.scrollPosition;
            }
        }
    }

    private void CreateStateBaseComponentCS(int workEnum, bool isCommon = false)
    {
        string className = string.Format(isCommon ? _commonCSWorkStreamName : m_CreateCSFileName, Enum.GetName(m_DefineStateType, workEnum));

        _stringBuilder.Append(string.Concat(new string[]
        {
            $"[StateComponent((int)StateCategoryEnum.{((StateCategoryEnum)m_StateCategory).ToString()}, (int){m_DefineStateType.Name}.{Enum.GetName(m_DefineStateType, workEnum)})]\n",
            $"public class {className} : StateBaseComponent\n",
            "{\n",
            "\tpublic override void Init(string str)\n\t{\n\t\t",
            "m_CurUseEntity.TranstionMultiStates(this);",
            "\n\t}\n}",
        }));

        EditorToolHelp.CreateText($"{(workEnum < m_BlockStartIndex ? (isCommon ? _commonNodePath : m_CreateCSFileNodePath) : (isCommon ? _commonBlockPath : m_CreateCSFileBlockPath))}{className}.cs", _stringBuilder.ToString());

        AssetDatabase.Refresh();
    }
    #endregion

    #region 节点内容注释
    private Vector2 _noteScrollV2;
    private void DrawWorkStreamNodeContentNote()
    {
        if (GUILayout.Button("Save"))
        {
            SaveWorkStreamEnumContentNote();
        }

        EditorGUILayout.BeginVertical();
        using (EditorGUILayout.ScrollViewScope svs = new EditorGUILayout.ScrollViewScope(_noteScrollV2))
        {
            foreach (var item in Enum.GetValues(m_DefineStateType))
            {
                string enumName = item.ToString();
                int enumInt = (int)item;
                if (enumInt == 0 || enumInt >= m_BlockStartIndex)
                    continue;

                FieldInfo fieldInfo = m_DefineStateType.GetField(enumName);
                if (fieldInfo == null)
                    continue;

                object[] attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs == null || attrs.Length <= 0)
                    continue;

                _nodeEnumParamsNoteDic.TryGetValue(enumInt, out string noteStr);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{((DescriptionAttribute)attrs[0]).Description}[{enumInt.ToString()}]", GUILayout.Width(400f));
                _nodeEnumParamsNoteDic[enumInt] = EditorGUILayout.TextField($"{noteStr}");
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(30f);

            _noteScrollV2 = svs.scrollPosition;
        }
        EditorGUILayout.EndVertical();
    }

    private void SaveWorkStreamEnumContentNote()
    {
        if (!Directory.Exists(_workStreamEnumContentNotePath))
            Directory.CreateDirectory(_workStreamEnumContentNotePath);

        string filePath = $"{_workStreamEnumContentNotePath}/{WorkStreamTypeKey}Notes.txt";

        List<string> strs = new List<string>();
        foreach (var kv in _nodeEnumParamsNoteDic)
        {
            strs.Add($"{kv.Key.ToString()}#{kv.Value}");
        }

        EditorToolHelp.ExportTxtFileEx(filePath, strs);

        AssetDatabase.Refresh();
    }
    #endregion

    #region 操作WorkStream内容
    private int _selectOperation;
    private string[] _operationTypeStrs = new string[] { "插入节点", "插入块数据", "替换功能", "删除节点", "查询功能" };
    private TextAsset _textAsset;
    private string _operationWorkIdsStr;
    private HashSet<uint> _fileWorkIdSet = new HashSet<uint>();
    private HashSet<uint> _textWorkIdSet = new HashSet<uint>();
    private bool _isAllWorkId;
    private void OperationWorkStreamContent()
    {
        EditorGUILayout.BeginVertical();
        _textAsset = EditorGUILayout.ObjectField(_textAsset, typeof(TextAsset), true, null) as TextAsset;
        var operationWorkIdsStr = EditorGUILayout.TextArea(_operationWorkIdsStr, GUILayout.Height(150));
        if (_operationWorkIdsStr != operationWorkIdsStr) 
        {
            _operationWorkIdsStr = operationWorkIdsStr;
            _isAllWorkId = _operationWorkIdsStr.ToLower().Contains("all");
        }

        EditorGUILayout.Space(20);

        _selectOperation = EditorGUILayout.Popup(_selectOperation, _operationTypeStrs);

        EditorGUILayout.Space(10);

        switch (_selectOperation) 
        {
            case 0:
                InsertNodeGUI();
                break;

            case 1:
                InsertBlockGUI();
                break;

            case 2:
                ReplaceNodeGUI();
                break;

            case 3:
                DelNodeGUI();
                break;

            case 4:
                CheckNodeGUI();
                break;
        }

        EditorGUILayout.EndVertical();
    }

    private class ReplaceInfo
    {
        public int _operationReplaceBlockEnum;
        public int _operationReplaceNodeEnum;
        public string _replaceNodeContent;
        public bool _isSpecificNodeContent;
        public string _specificNodeContent;
        public int _replaceNodeType;
        
        public int _needReplaceToEnum;
    }
    
    private string[] _replaceNodeTypeStrs = new string[] { "内容替换", "节点替换" };
    private ReplaceInfo _replaceInfo = new ReplaceInfo();
    private void ReplaceNodeGUI() 
    {
        _replaceInfo._replaceNodeType = EditorGUILayout.Popup(_replaceInfo._replaceNodeType, _replaceNodeTypeStrs, GUILayout.Width(150));

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        if (_replaceInfo._replaceNodeType == 1)
        {
            EditorGUILayout.LabelField("处理节点：", GUILayout.Width(80));
        }

        int specificBlockIndex = GetIndexByEnum(_replaceInfo._operationReplaceBlockEnum, WorkBlockEnumDic);
        specificBlockIndex = EditorGUILayout.Popup(specificBlockIndex, WorkBlockEnumStrs, _popupSt03);
        _replaceInfo._operationReplaceBlockEnum = WorkBlockEnumDic[specificBlockIndex];

        int nodeIndex = GetIndexByEnum(_replaceInfo._operationReplaceNodeEnum, WorkNodeEnumDic);
        nodeIndex = CustomPopup(nodeIndex, WorkNodeEnumStrs, nodeIndex == 0 ? _popupSt : _popupSt03,
                delegate (int selectIndex) {
                    _replaceInfo._operationReplaceNodeEnum = WorkNodeEnumDic[selectIndex];
                });

        _replaceInfo._isSpecificNodeContent = EditorGUILayout.Toggle(_replaceInfo._isSpecificNodeContent, GUILayout.Width(20));
        if (_replaceInfo._isSpecificNodeContent)
            _replaceInfo._specificNodeContent = EditorGUILayout.TextField(_replaceInfo._specificNodeContent, GUILayout.Width(250));
        else
            EditorGUILayout.LabelField("即将替换所有的值", GUILayout.Width(250));

        if (_replaceInfo._replaceNodeType == 0) 
        {
            EditorGUILayout.LabelField(_replaceInfo._isSpecificNodeContent ? "替换" : null, GUILayout.Width(30));
            _replaceInfo._replaceNodeContent = EditorGUILayout.TextField(_replaceInfo._replaceNodeContent, GUILayout.Width(250));
        }
        
        EditorGUILayout.EndHorizontal();

        if (_replaceInfo._replaceNodeType == 1) 
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("替换为：", GUILayout.Width(80));

            int replaceNodeIndex = GetIndexByEnum(_replaceInfo._needReplaceToEnum, WorkNodeEnumDic);
            replaceNodeIndex = CustomPopup(replaceNodeIndex, WorkNodeEnumStrs, replaceNodeIndex == 0 ? _popupSt : _popupSt03,
                    delegate (int selectIndex) {
                        _replaceInfo._needReplaceToEnum = WorkNodeEnumDic[selectIndex];
                    });

            _replaceInfo._replaceNodeContent = EditorGUILayout.TextField(_replaceInfo._replaceNodeContent, GUILayout.Width(250));

            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.Space(10);
        if (GUILayout.Button("替换"))
        {
            MakeWorkIdDatas((uint workId, WorkBlockData workBlockData) =>
            {
                ReplaceNodeData(workBlockData, workBlockData.TopWorkNodeData, _replaceInfo, _replaceInfo._replaceNodeType == 1);
            });
        }
    }

    private class InsertInfo
    {
        public int _specificInsertBlockType;
        public int _operationInsertNodeEnum;
        public bool _isSpecificInsertNodeContent;
        public bool _isSpecificFuzzyNodeContent;
        public string _operationInsertNodeContent;
        public int _insertGroup = -1;
        public int _insertNodeEnum;
        public string _insertContent;
        public int _insertBlockEnum;
        public int _insertAddType;
        public bool _isUseCopy;
        public int _CopyType;
    }
    private string[] _insertTypeStrs = new string[] { "插在 Down 面", "插在 Up 面" };
    private string[] _insertTypeStrs02 = new string[] { "插在 Down 面", "插在 Up 面", "插入为子节点" };
    private string[] _insertCopyTypeStrs = new string[] { "插入复制的该数据以及子节点数据", "只插入复制的该条数据" };
    private InsertInfo _insertInfo = new InsertInfo();
    private void InsertNodeGUI() 
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("参照节点：", GUILayout.Width(60));

        int specificBlockIndex = GetIndexByEnum(_insertInfo._specificInsertBlockType, WorkBlockEnumDic);
        specificBlockIndex = EditorGUILayout.Popup(specificBlockIndex, WorkBlockEnumStrs, _popupSt03);
        _insertInfo._specificInsertBlockType = WorkBlockEnumDic[specificBlockIndex];

        int nodeIndex = GetIndexByEnum(_insertInfo._operationInsertNodeEnum, WorkNodeEnumDic);
        nodeIndex = CustomPopup(nodeIndex, WorkNodeEnumStrs, nodeIndex == 0 ? _popupSt : _popupSt03,
                delegate (int selectIndex) {
                    _insertInfo._operationInsertNodeEnum = WorkNodeEnumDic[selectIndex];
                });
        _insertInfo._isSpecificInsertNodeContent = EditorGUILayout.Toggle(_insertInfo._isSpecificInsertNodeContent, GUILayout.Width(20));
        if (_insertInfo._isSpecificInsertNodeContent)
        {
            _insertInfo._operationInsertNodeContent = EditorGUILayout.TextField(_insertInfo._operationInsertNodeContent);
            _insertInfo._isSpecificFuzzyNodeContent = GUILayout.Toggle(_insertInfo._isSpecificFuzzyNodeContent, "模糊匹配", GUILayout.Width(80));
        }
        else
            EditorGUILayout.LabelField("即将对所有该参照类型进行插入");

        if (_nodeOperateFunc.CopyWNData != null)
            _insertInfo._isUseCopy = GUILayout.Toggle(_insertInfo._isUseCopy, "使用拷贝数据", GUILayout.Width(100));
        else
            _insertInfo._isUseCopy = false;

        if (!_insertInfo._isUseCopy && _insertInfo._insertAddType > 1)
            _insertInfo._insertAddType = 0;

        _insertInfo._insertAddType = EditorGUILayout.Popup(_insertInfo._insertAddType, _insertInfo._isUseCopy ? _insertTypeStrs02 : _insertTypeStrs);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        if (_insertInfo._isUseCopy)
        {
            _insertInfo._CopyType = EditorGUILayout.Popup(_insertInfo._CopyType, _insertCopyTypeStrs, GUILayout.Width(200f));
            EditorGUILayout.LabelField($"组：{_nodeOperateFunc.CopyWNData.GroupIndex}");
            nodeIndex = GetIndexByEnum(_nodeOperateFunc.CopyWNData.NodeType, WorkNodeEnumDic);
            EditorGUILayout.LabelField($"节点类型：{WorkNodeEnumStrs[nodeIndex]}");
            EditorGUILayout.LabelField($"节点内容：{_nodeOperateFunc.CopyWNData.NodeContent}");
            specificBlockIndex = GetIndexByEnum(_nodeOperateFunc.CopyWNData.SkipWorkBlockType, WorkBlockEnumDic);
            EditorGUILayout.LabelField($"节点跳转：{WorkBlockEnumStrs[specificBlockIndex]}");
        }
        else
        {
            EditorGUILayout.LabelField("插入的节点：", GUILayout.Width(80));

            _insertInfo._insertGroup = EditorGUILayout.IntField(_insertInfo._insertGroup, GUILayout.Width(30));

            nodeIndex = GetIndexByEnum(_insertInfo._insertNodeEnum, WorkNodeEnumDic);
            nodeIndex = CustomPopup(nodeIndex, WorkNodeEnumStrs, nodeIndex == 0 ? _popupSt : _popupSt03,
                    delegate (int selectIndex) {
                        _insertInfo._insertNodeEnum = WorkNodeEnumDic[selectIndex];
                    });

            _insertInfo._insertContent = EditorGUILayout.TextField(_insertInfo._insertContent);

            int blockIndex = GetIndexByEnum(_insertInfo._insertBlockEnum, WorkBlockEnumDic);
            blockIndex = EditorGUILayout.Popup(blockIndex, WorkBlockEnumStrs, _popupSt03);
            _insertInfo._insertBlockEnum = WorkBlockEnumDic[blockIndex];
        }
        
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(15);
        if (GUILayout.Button("插入"))
        {
            if (_insertInfo._operationInsertNodeEnum == 0 || (!_insertInfo._isUseCopy && _insertInfo._insertNodeEnum == 0))
                return;

            MakeWorkIdDatas((uint workId, WorkBlockData workBlockData) =>
            {
                InsertNodeData(workId, workBlockData, workBlockData.TopWorkNodeData, null, _insertInfo);
            });
        }
    }

    private class InsertBlcokInfo
    {

    }
    InsertBlcokInfo _insertBlcokInfo = new InsertBlcokInfo();
    private void InsertBlockGUI()
    {
        if (_copyWorkBlockData == null)
            return;

        if (GUILayout.Button("插入"))
        {
            int blockIndex = GetIndexByEnum(_copyWorkBlockData.CurWorkBlockType, WorkBlockEnumDic);
            string blockName = WorkBlockEnumStrs[blockIndex];

            MakeWorkIdDatas(null, true, (uint workId) =>
            {
                List<WorkBlockData> workBlokDataList = new List<WorkBlockData>();
                if (_dataPathDic.ContainsKey(workId))
                    ParseTxtToData(SavePath, workId, workBlokDataList, false);

                for (int wbdIndex = 0, wbdCount = workBlokDataList.Count; wbdIndex < wbdCount; wbdIndex++)
                {
                    if (workBlokDataList[wbdIndex].CurWorkBlockType == _copyWorkBlockData.CurWorkBlockType)
                        return;
                }

                var blockData = FileDataOperationManager.DeepCopyObj<WorkBlockData>(_copyWorkBlockData);
                if (blockData != null)
                {
                    workBlokDataList.Add(blockData);
                    SaveToTxt(SavePath, workId, workBlokDataList);

                    Debug.Log($"在{workId.ToString()}插入数据块：【{blockName}】");
                }
            });
        }
    }
    
    private string[] _locationDelTypeStrs = new string[] { "在 Down 面", "在 Up 面" };
    private class DelNodeInfo
    {
        public bool haveDelLocation;
        public int locationNodeEnum;
        public string locationNodeContent;
        public int locationDelType;
        public int delSpecificBlockType;
        public int delNodeEnum;
        public bool delIsSpecificNodeContent;
        public string delNodeContent;
    }
    private DelNodeInfo _delNodeInfo;
    private void DelNodeGUI()
    {
        if (_delNodeInfo == null)
            _delNodeInfo = new DelNodeInfo();

        EditorGUILayout.BeginHorizontal();
        _delNodeInfo.haveDelLocation = EditorGUILayout.Toggle(_delNodeInfo.haveDelLocation, GUILayout.Width(20));
        if (_delNodeInfo.haveDelLocation)
        {
            int locationNodeIndex = GetIndexByEnum(_delNodeInfo.locationNodeEnum, WorkNodeEnumDic);
            locationNodeIndex = CustomPopup(locationNodeIndex, WorkNodeEnumStrs, locationNodeIndex == 0 ? _popupSt : _popupSt03,
                    delegate (int selectIndex) {
                        _delNodeInfo.locationNodeEnum = WorkNodeEnumDic[selectIndex];
                    });

            _delNodeInfo.locationNodeContent = EditorGUILayout.TextField(_delNodeInfo.locationNodeContent, GUILayout.Width(250));

            _delNodeInfo.locationDelType = EditorGUILayout.Popup(_delNodeInfo.locationDelType, _locationDelTypeStrs);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();

        int specificBlockIndex = GetIndexByEnum(_delNodeInfo.delSpecificBlockType, WorkBlockEnumDic);
        specificBlockIndex = EditorGUILayout.Popup(specificBlockIndex, WorkBlockEnumStrs, _popupSt03);
        _delNodeInfo.delSpecificBlockType = WorkBlockEnumDic[specificBlockIndex];

        int nodeIndex = GetIndexByEnum(_delNodeInfo.delNodeEnum, WorkNodeEnumDic);
        nodeIndex = CustomPopup(nodeIndex, WorkNodeEnumStrs, nodeIndex == 0 ? _popupSt : _popupSt03,
                delegate (int selectIndex) {
                    _delNodeInfo.delNodeEnum = WorkNodeEnumDic[selectIndex];
                });

        _delNodeInfo.delIsSpecificNodeContent = EditorGUILayout.Toggle(_delNodeInfo.delIsSpecificNodeContent, GUILayout.Width(20));
        if (_delNodeInfo.delIsSpecificNodeContent)
            _delNodeInfo.delNodeContent = EditorGUILayout.TextField(_delNodeInfo.delNodeContent, GUILayout.Width(250));
        else
            EditorGUILayout.LabelField("只删除类型", GUILayout.Width(250));

        if (GUILayout.Button("删除", GUILayout.Width(150)))
        {
            MakeWorkIdDatas((uint workId, WorkBlockData workBlockData) =>
            {
                DelNodeData(workBlockData, workBlockData.TopWorkNodeData, null, -1, -1, _delNodeInfo, false);
            });
        }

        EditorGUILayout.EndHorizontal();
    }

    private int _checkNodeEnum;
    private bool _checkIsSpecificNodeContent;
    private bool _checkIsContainNodeContent;
    private string _checkNodeContent;
    private HashSet<int> _useSet = new HashSet<int>();
    private void CheckNodeGUI()
    {
        //if (GUILayout.Button("查询所有节点情况", GUILayout.Width(150)))
        //{
        //    _useSet.Clear();

        //    MakeWorkIdDatas((uint workId, WorkBlockData workBlockData) =>
        //    {
        //        CheckUseNodeTypeData(workId, workBlockData.TopWorkNodeData, (uint cwId, WorkNodeData wnd) =>
        //        {
        //            _useSet.Add(wnd.NodeType);
        //        });
        //    }, false);

        //    foreach (var kv in WorkNodeEnumDic)
        //    {
        //        if (!_useSet.Contains(kv.Value))
        //        {
        //            Debug.Log($"{WorkNodeEnumStrs[kv.Key]}【{kv.Value}】节点未曾使用");
        //        }
        //    }
        //}

        //EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        int nodeIndex = GetIndexByEnum(_checkNodeEnum, WorkNodeEnumDic);
        nodeIndex = CustomPopup(nodeIndex, WorkNodeEnumStrs, nodeIndex == 0 ? _popupSt : _popupSt03,
                delegate (int selectIndex) {
                    _checkNodeEnum = WorkNodeEnumDic[selectIndex];
                });

        _checkIsSpecificNodeContent = EditorGUILayout.Toggle(_checkIsSpecificNodeContent, GUILayout.Width(20));
        if (_checkIsSpecificNodeContent)
        {
            _checkNodeContent = EditorGUILayout.TextField(_checkNodeContent, GUILayout.Width(250));
            _checkIsContainNodeContent = EditorGUILayout.Toggle(_checkIsContainNodeContent, GUILayout.Width(20));
            EditorGUILayout.LabelField("包含内容即可", GUILayout.Width(80));
        }
        else
            EditorGUILayout.LabelField("只查询类型", GUILayout.Width(250));

        if (GUILayout.Button("查询", GUILayout.Width(150)))
        {
            MakeWorkIdDatas((uint workId, WorkBlockData workBlockData) =>
            {
                CheckNodeData(workId, workBlockData.TopWorkNodeData, _checkNodeEnum,
                    _checkIsSpecificNodeContent, _checkIsContainNodeContent, _checkNodeContent);
            }, false);
        }

        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region Common Logic
    public void SetHelpToolData(string fileBlockPath, string fileNodePath, string fileName, int stateCategory, Type defineStateType, int blockStartIndex)
    {
        m_CreateCSFileBlockPath = fileBlockPath;
        m_CreateCSFileNodePath = fileNodePath;
        m_CreateCSFileName = fileName;
        m_StateCategory = stateCategory;
        m_DefineStateType = defineStateType;
        m_BlockStartIndex = blockStartIndex;
    }

    protected void ParseWorkStreamEnumContentNote()
    {
        if (!Directory.Exists(_workStreamEnumContentNotePath))
            return;

        string filePath = $"{_workStreamEnumContentNotePath}/{WorkStreamTypeKey}Notes.txt";
        if (!File.Exists(filePath))
            return;

        _nodeEnumParamsNoteDic.Clear();

        EditorToolHelp.ParseTxtInLine(filePath, (string line) =>
        {
            if (!string.IsNullOrEmpty(line))
            {
                string[] strs0 = line.Split('#');
                if (strs0 != null && strs0.Length > 1)
                {
                    _nodeEnumParamsNoteDic[int.Parse(strs0[0])] = strs0[1];
                }
            }
        });
    }

    private void MakeWorkIdDatas(Action<uint, WorkBlockData> action, bool isNeedWrite = true, Action<uint> doWorkIdAction = null) 
    {
        if (_isAllWorkId)
        {
            foreach (var info in m_WorkMenuList)
            {
                if (info == null)
                    continue;

                UpdateNodeData(SavePath, info.WorkId, action, isNeedWrite);

                doWorkIdAction?.Invoke(info.WorkId);
            }
        }
        else 
        {
            _fileWorkIdSet.Clear();
            if (_textAsset != null)
            {
                string[] fileWorkIdsStr = _textAsset.text.Replace("\n", "").Replace("\t", "").Split('|');
                foreach (var item in fileWorkIdsStr)
                {
                    if (uint.TryParse(item, out uint workId))
                        _fileWorkIdSet.Add(workId);
                }
            }

            _textWorkIdSet.Clear();
            if (!string.IsNullOrWhiteSpace(_operationWorkIdsStr))
            {
                string[] workIdsStr = _operationWorkIdsStr.Replace("\n", "").Replace("\t", "").Split('|');
                foreach (var workIdStr in workIdsStr)
                {
                    if (uint.TryParse(workIdStr, out uint workId))
                        _textWorkIdSet.Add(workId);
                }
            }

            foreach (var workId in _fileWorkIdSet)
            {
                UpdateNodeData(SavePath, workId, action, isNeedWrite);

                doWorkIdAction?.Invoke(workId);
            }

            foreach (var workId in _textWorkIdSet)
            {
                UpdateNodeData(SavePath, workId, action, isNeedWrite);

                doWorkIdAction?.Invoke(workId);
            }
        }
        
        AssetDatabase.Refresh();
    }

    private void UpdateNodeData(string filePath, uint updateWorkId, Action<uint, WorkBlockData> action, bool isNeedWrite = true) 
    {
        if (!Directory.Exists(filePath))
        {
            Debug.LogError($"不存在文件夹路径{filePath}");
            return;
        }

        string path = $"{filePath}{updateWorkId.ToString()}.txt";

        if (!File.Exists(path))
        {
            Debug.LogError($"文件夹路径{filePath}不存在文件{updateWorkId.ToString()}.txt");
            return;
        }

        byte[] datas = File.ReadAllBytes(path);
        FileDataOperationReadEntity fileDataOperationReadEntity = new FileDataOperationReadEntity();
        WorkStreamData workStreamData = fileDataOperationReadEntity.DoRead(new MemoryStream(datas), (BinaryReader br) =>
        {
            return WorkStreamData_ConfigTool.Load(br);
        },
        (Stream rs, BinaryReader br) =>
        {
            uint workId = br.ReadUInt32();
            if (updateWorkId != workId)
            {
                Debug.LogError($"选择{updateWorkId.ToString()}，但加载{workId.ToString()}");
                return;
            }
        });

        if (workStreamData.AttackWorkBlockDatas != null) 
        {
            foreach (WorkBlockData workBlockData in workStreamData.AttackWorkBlockDatas)
            {
                action?.Invoke(updateWorkId, workBlockData);
            }
        }

        if (workStreamData.TargetWorkBlockDatas != null)
        {
            foreach (WorkBlockData workBlockData in workStreamData.TargetWorkBlockDatas)
            {
                action?.Invoke(updateWorkId, workBlockData);
            }
        }

        if (isNeedWrite)
        {
            FileDataOperationWriteEntity fileDataOperationWriteEntity = new FileDataOperationWriteEntity();
            fileDataOperationWriteEntity.DoWriter(path, workStreamData, (BinaryWriter bw) =>
            {
                bw.Write(updateWorkId);
            }, true);
        }
    }

    private void ReplaceNodeData(WorkBlockData workBlockData, WorkNodeData workNodeData, ReplaceInfo replaceInfo, bool isNeedReplaceNodeEnum) 
    {
        if (replaceInfo._operationReplaceBlockEnum > 0 && workBlockData.CurWorkBlockType != replaceInfo._operationReplaceBlockEnum)
            return;

        if (workNodeData.NodeType == _replaceInfo._operationReplaceNodeEnum)
        {
            if (_replaceInfo._isSpecificNodeContent)
            {
                if (workNodeData.NodeContent == _replaceInfo._specificNodeContent)
                {
                    if (isNeedReplaceNodeEnum)
                        workNodeData.NodeType = _replaceInfo._needReplaceToEnum;
                    workNodeData.NodeContent = _replaceInfo._replaceNodeContent;
                }
            }
            else
            {
                if (isNeedReplaceNodeEnum)
                    workNodeData.NodeType = _replaceInfo._needReplaceToEnum;
                workNodeData.NodeContent = _replaceInfo._replaceNodeContent;
            }
        }

        if (workNodeData.TransitionWorkGroupList == null)
            return;

        foreach (WorkGroupNodeData workGroupNodeData in workNodeData.TransitionWorkGroupList)
        {
            if (workGroupNodeData.NodeList == null)
                return;

            foreach (var nodeData in workGroupNodeData.NodeList)
            {
                ReplaceNodeData(workBlockData, nodeData, _replaceInfo, isNeedReplaceNodeEnum);
            }
        }
    }

    private void InsertNodeData(uint workId, WorkBlockData workBlockData, WorkNodeData checkWorkNodeData, 
        WorkNodeData parent, InsertInfo insertInfo)
    {
        if (insertInfo._specificInsertBlockType > 0 && workBlockData.CurWorkBlockType != insertInfo._specificInsertBlockType)
            return;

        WorkNodeData referNode = null;
        if (checkWorkNodeData.NodeType == insertInfo._operationInsertNodeEnum)
        {
            if (insertInfo._isSpecificInsertNodeContent)
            {
                if (string.IsNullOrWhiteSpace(checkWorkNodeData.NodeContent) && string.IsNullOrWhiteSpace(insertInfo._operationInsertNodeContent))
                {
                    referNode = checkWorkNodeData;
                }
                else if (_insertInfo._isSpecificFuzzyNodeContent)
                {
                    if (!string.IsNullOrWhiteSpace(checkWorkNodeData.NodeContent) && 
                        checkWorkNodeData.NodeContent.Contains(insertInfo._operationInsertNodeContent))
                        referNode = checkWorkNodeData;
                }
                else
                {
                    if (checkWorkNodeData.NodeContent == insertInfo._operationInsertNodeContent)
                        referNode = checkWorkNodeData;
                }
            }
            else
                referNode = checkWorkNodeData;

            if (referNode != null) 
            {
                if (insertInfo._isUseCopy)
                {
                    AddNodeByCopyNodeDatas(workBlockData, referNode, _nodeOperateFunc.CopyWNData, 
                        _insertInfo._CopyType == 0 ? 2 : 1, insertInfo._insertAddType);
                }
                else 
                {
                    Debug.Log($"workId:{workId}中在{checkWorkNodeData.NodeType}:[{checkWorkNodeData.NodeContent}]插入数据");

                    WorkNodeData addNode = AddBrotherNode(workBlockData, parent, referNode, 
                        (sbyte)insertInfo._insertGroup, insertInfo._insertAddType == 0);
                    addNode.NodeType = insertInfo._insertNodeEnum;
                    addNode.NodeContent = insertInfo._insertContent;
                    addNode.SkipWorkBlockType = insertInfo._insertBlockEnum;

                    if (parent != null)
                    {
                        if (parent.TransitionWorkGroupList != null)
                            parent.TransitionWorkGroupList.Sort(GroupSort);
                    }
                }
            }
        }

        if (checkWorkNodeData.TransitionWorkGroupList == null)
            return;

        for (int twgIndex = checkWorkNodeData.TransitionWorkGroupList.Count - 1; twgIndex > -1; --twgIndex)
        {
            WorkGroupNodeData workGroupNodeData = checkWorkNodeData.TransitionWorkGroupList[twgIndex];
            if (workGroupNodeData.NodeList == null)
                return;

            for (int nodeIndex = workGroupNodeData.NodeList.Count - 1; nodeIndex > -1; --nodeIndex)
            {
                var nodeData = workGroupNodeData.NodeList[nodeIndex];
                InsertNodeData(workId, workBlockData, nodeData, checkWorkNodeData, insertInfo);
            }
        }
    }

    private void DelNodeData(WorkBlockData workBlockData, WorkNodeData workNodeData, WorkNodeData parent,
        int parentTransIndex, int parentGroupNodeIndex,
        DelNodeInfo delNodeInfo, bool isNeedCheck)
    {
        if (delNodeInfo.delSpecificBlockType > 0 && workBlockData.CurWorkBlockType != delNodeInfo.delSpecificBlockType)
            return;

        if (isNeedCheck || !delNodeInfo.haveDelLocation)
        {
            if (workNodeData.NodeType == delNodeInfo.delNodeEnum)
            {
                if (delNodeInfo.delIsSpecificNodeContent)
                {
                    if (workNodeData.NodeContent == delNodeInfo.delNodeContent)
                    {
                        RemoveNode(parent, parentTransIndex, parentGroupNodeIndex);
                    }
                }
                else
                {
                    RemoveNode(parent, parentTransIndex, parentGroupNodeIndex);
                }
            }
        }
        
        if (workNodeData.TransitionWorkGroupList == null)
            return;

        for (int twgIndex = workNodeData.TransitionWorkGroupList.Count - 1; twgIndex > -1; --twgIndex)
        {
            WorkGroupNodeData workGroupNodeData = workNodeData.TransitionWorkGroupList[twgIndex];
            if (workGroupNodeData.NodeList == null)
                return;

            for (int nodeIndex = workGroupNodeData.NodeList.Count - 1; nodeIndex > -1; --nodeIndex)
            {
                bool isNeedCheckLocation = true;
                if (delNodeInfo.haveDelLocation)
                {
                    isNeedCheckLocation = false;
                    WorkNodeData checkLocationNode = null;
                    if (delNodeInfo.locationDelType == 0)
                    {
                        if (nodeIndex > 0)
                            checkLocationNode = workGroupNodeData.NodeList[nodeIndex - 1];
                    }
                    else
                    {
                        if (nodeIndex < workGroupNodeData.NodeList.Count - 1)
                            checkLocationNode = workGroupNodeData.NodeList[nodeIndex + 1];
                    }

                    if (checkLocationNode != null)
                    {
                        isNeedCheckLocation = checkLocationNode.NodeType == delNodeInfo.locationNodeEnum &&
                            checkLocationNode.NodeContent == delNodeInfo.locationNodeContent;
                    }
                }

                var nodeData = workGroupNodeData.NodeList[nodeIndex];
                DelNodeData(workBlockData, nodeData, workNodeData, twgIndex, nodeIndex,
                    delNodeInfo, isNeedCheckLocation);
            }
        }
    }

    private void CheckNodeData(uint checkWorkId, WorkNodeData workNodeData, int checkNodeEnum,
        bool checkIsSpecificNodeContent, bool checkIsContainNodeContent, string checkNodeContent)
    {
        if (workNodeData.NodeType == checkNodeEnum)
        {
            if (checkIsSpecificNodeContent)
            {
                if ((!checkIsContainNodeContent && ((string.IsNullOrWhiteSpace(checkNodeContent) && string.IsNullOrWhiteSpace(workNodeData.NodeContent)) || workNodeData.NodeContent == checkNodeContent)) || 
                    (checkIsContainNodeContent && !string.IsNullOrWhiteSpace(checkNodeContent) && !string.IsNullOrWhiteSpace(workNodeData.NodeContent) && workNodeData.NodeContent.Contains(checkNodeContent)))
                {
                    Debug.Log($"workId：{checkWorkId}中使用了类型：{WorkNodeEnumStrs[GetIndexByEnum(checkNodeEnum, WorkNodeEnumDic)]}[{checkNodeEnum.ToString()}]，内容为：{workNodeData.NodeContent}    查询内容：{checkNodeContent}的节点");
                    return;
                }
            }
            else
            {
                Debug.Log($"workId：{checkWorkId}中使用了类型：{WorkNodeEnumStrs[GetIndexByEnum(checkNodeEnum, WorkNodeEnumDic)]}[{checkNodeEnum.ToString()}]的节点，内容为：{workNodeData.NodeContent} ");
                return;
            }
        }

        if (workNodeData.TransitionWorkGroupList == null)
            return;

        for (int twgIndex = workNodeData.TransitionWorkGroupList.Count - 1; twgIndex > -1; --twgIndex)
        {
            WorkGroupNodeData workGroupNodeData = workNodeData.TransitionWorkGroupList[twgIndex];
            if (workGroupNodeData.NodeList == null)
                return;

            for (int nodeIndex = workGroupNodeData.NodeList.Count - 1; nodeIndex > -1; --nodeIndex)
            {
                var nodeData = workGroupNodeData.NodeList[nodeIndex];
                CheckNodeData(checkWorkId, nodeData, checkNodeEnum, checkIsSpecificNodeContent, checkIsContainNodeContent, checkNodeContent);
            }
        }
    }
    
    private void CheckUseNodeTypeData(uint checkWorkId, WorkNodeData workNodeData, Action<uint, WorkNodeData> action = null)
    {
        action?.Invoke(checkWorkId, workNodeData);

        if (workNodeData.TransitionWorkGroupList == null)
            return;

        for (int twgIndex = workNodeData.TransitionWorkGroupList.Count - 1; twgIndex > -1; --twgIndex)
        {
            WorkGroupNodeData workGroupNodeData = workNodeData.TransitionWorkGroupList[twgIndex];
            if (workGroupNodeData.NodeList == null)
                return;

            for (int nodeIndex = workGroupNodeData.NodeList.Count - 1; nodeIndex > -1; --nodeIndex)
            {
                var nodeData = workGroupNodeData.NodeList[nodeIndex];
                CheckUseNodeTypeData(checkWorkId, nodeData, action);
            }
        }
    }
    #endregion

    #endregion
}
