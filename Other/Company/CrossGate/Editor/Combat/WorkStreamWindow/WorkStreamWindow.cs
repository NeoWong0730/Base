using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text;
using System;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

public class WorkStreamMenuInfoEditor
{
    public uint WorkId;
    public string WorkStreamDes;
}

public partial class WorkStreamWindow : EditorWindow
{
    public class NodeOperateFunc
    {
        public bool IsOperate;
        public Rect OpRect;
        public WorkBlockData WBlockData;
        public WorkNodeData WNodeData;
        public WorkNodeData CopyWNData;
        public int CopyType;
    }

    public class NodeWidgetHelpFunc
    {
        public bool IsHelp;
        public bool IsShow;
        public WorkNodeData WNodeData;
        public int WidgetType;
        public float StayTime;
        public Rect ShowRect;
    }

    public string WorkStreamTypeKey;
    protected int _workStreamType;
    private string _savePath;
    public string SavePath
    {
        get
        {
            return string.Format(_savePath, _workStreamType == 0 ? string.Empty : _workStreamType.ToString());
        }
        set
        {
            _savePath = value;
        }
    }
    private string _saveAllPath;
    public string SaveAllPath
    {
        get
        {
            return string.Format(_saveAllPath, _workStreamType == 0 ? string.Empty : _workStreamType.ToString());
        }
        set
        {
            _saveAllPath = value;
        }
    }

    public string CopyIdsTxtFile;

    public string[] AttachEnumStrs = new string[] { "施放者", "目标" };
    public string[] WorkBlockEnumStrs = new string[] { "Block" };
    public Dictionary<int, int> WorkBlockEnumDic = new Dictionary<int, int>() { { 0, 0 } };
    public string[] WorkNodeEnumStrs = new string[] { "Node" };
    public Dictionary<int, int> WorkNodeEnumDic = new Dictionary<int, int> { { 0, 0 } };

    /// <summary>
    /// 收集节点的枚举值
    /// </summary>
    public int m_CollectNodeType = 4999;

    private string[] _showTypeStrs = new string[] { "拓扑图式", "堆积木式" };

    private string[] _leftWorkMenuStrs = new string[] { "工作流数据", "HelpTool" };
    private int _selectLeftWorkMenu;

    protected Dictionary<uint, string> _dataPathDic = new Dictionary<uint, string>();

    private DragAreaEditor _dragAreaEditor = new DragAreaEditor(0f, 0f, 280f);

    protected static GUIStyle _popupSt;
    protected static GUIStyle _popupSt02;
    protected static GUIStyle _popupSt03;
    protected static GUIStyle _labelSt;
    protected static GUIStyle _labelSt02;
    protected static GUIStyle _textSt;
    protected static GUIStyle _buttonSt;
    protected static GUIStyle _buttonSt02;
    protected static GUIStyle _toggleSt;

    protected static readonly string _popupTypePrefsStr = "WorkStreamWindow_PopupType";

    protected int _popupType;
    protected int PopupType
    {
        get
        {
            if (_popupType == 0)
            {
                if (!PlayerPrefs.HasKey(_popupTypePrefsStr))
                {
                    _popupType = 1;
                }
                else
                {
                    _popupType = PlayerPrefs.GetInt(_popupTypePrefsStr);
                }
            }

            return _popupType;
        }
        set
        {
            if (value <= 0)
                return;

            if (_popupType != value)
            {
                _popupType = value;
                PlayerPrefs.SetInt(_popupTypePrefsStr, value);
            }
        }
    }

    private Regex _regex = new Regex(@"^[\*0-9]$");
    private string _textWorkIdStr;
    private char[] _textWorkIdCharArray;
    private uint _textWorkId;
    private uint _checkWorkId;
    private bool _isFuzzyCheckId;
    private Vector2 _workMenuScrollPos;
    public uint m_SelectWorkId;

    private bool _useLeftDownSpace;

    public List<WorkBlockData> m_WorkBlockDataList = new List<WorkBlockData>();
    public List<WorkStreamMenuInfoEditor> m_WorkMenuList = new List<WorkStreamMenuInfoEditor>();

    protected bool _isShowCollectContent;
    protected List<uint> _collectIdList = new List<uint>();
    protected Dictionary<uint, List<WorkBlockData>> _collectWorkBlockDic;

    private WorkBlockData _copyWorkBlockData;

    private bool _isHeapStyle = true;

    private float _blockHeight;
    private Dictionary<WorkBlockData, float> _blockHeightDic = new Dictionary<WorkBlockData, float>();
    private sbyte _maxLayerIndex;
    private sbyte _selectLayerIndex = -1;
    private sbyte _selectGroupIndex;
    private uint _selectParentId;
    private Vector2 _workBlocksScrollPos;
    private Vector2 _workBlocksViewPos;
    private readonly float _blockBaseHeight = 200f;
    private readonly float _blockLeftWidth = 220f;

    protected float _heapStyleNodeTypeWidth = 200f;
    protected float _heapStyleNodeContentWidth = 320f;
    private readonly float _heapNodeHeight = 30f;
    private static float _heapNodeContentHeight = 20f;
    private readonly float _heapNodeWidth = 1000f;
    private readonly float _heapNodeContentWidth = 990f;

    private float _topoNodeHeight = 110f;
    private float _topoNodeContentHeight = 80f;
    private float _topoNodeWidth = 200f;
    private float _topoNodeContentWidth = 160f;
    private Dictionary<WorkNodeData, int> _topoNodeFissionWeightDic = new Dictionary<WorkNodeData, int>();

    private Texture2D _workMenuBgTexture;
    private Texture2D WorkMenuBGTexture
    {
        get
        {
            if (_workMenuBgTexture == null)
            {
                _workMenuBgTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                _workMenuBgTexture.SetPixel(0, 0, new Color(0.35f, 0.35f, 0.35f));
                _workMenuBgTexture.Apply();
            }
            return _workMenuBgTexture;
        }
    }
    private Texture2D _rightBgTexture;
    private Texture2D RightBGTexture
    {
        get
        {
            if (_rightBgTexture == null)
            {
                _rightBgTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                _rightBgTexture.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f));
                _rightBgTexture.Apply();
            }
            return _rightBgTexture;
        }
    }

    private bool _debugLog;
    private bool _showConcurrentColor = true;
    private Dictionary<ushort, Color> _concurrentNodeColorDic = new Dictionary<ushort, Color>();

    protected uint FrameCount;

    private bool _showHelpTips;
    private Rect _helpTipsRect;
    private int _helpTipsNodeType;
    private string _helpTipsContentStr;
    protected Dictionary<int, string> _nodeEnumParamsNoteDic = new Dictionary<int, string>();

    protected NodeOperateFunc _nodeOperateFunc = new NodeOperateFunc();

    private NodeWidgetHelpFunc _nodeWidgetHelpFunc = new NodeWidgetHelpFunc();

    #region 生命周期
    private void Awake()
    {
        Clear();
    }

    private void Update()
    {
        ++FrameCount;

        if (_useLeftDownSpace)
            UpdateDynamicShowFrame();

        OnUpdate();

        if (_useLeftDownSpace)
            UpdateShowDynamicRunNode();
    }

    public virtual void OnUpdate()
    {

    }

    private void OnGUI()
    {
        SetGUIStyle();

        DoCustomGUI();

        EditorGUILayout.BeginHorizontal();
        DrawLeft();
        Handles.DrawLine(new Vector3(_dragAreaEditor.Width - 0.5f, 0f), new Vector3(_dragAreaEditor.Width - 0.5f, Screen.height));
        DrawRight();
        EditorGUILayout.EndHorizontal();
    }

    public virtual void DoCustomGUI()
    {

    }

    void OnInspectorUpdate()
    {
        //开启窗口的重绘，不然窗口信息不会刷新
        Repaint();
    }
    #endregion

    public void Clear()
    {
        _textWorkIdStr = null;
        _textWorkIdCharArray = null;
        _textWorkId = 0;
        _checkWorkId = 0;
        _isFuzzyCheckId = false;
        m_SelectWorkId = 0;
        m_WorkMenuList.Clear();

        ClearShowDynamicNodeData();

        ClearOldWorkState();
    }

    public void ClearOldWorkState()
    {
        m_WorkBlockDataList.Clear();
        _blockHeight = 0;
        _blockHeightDic.Clear();
        _maxLayerIndex = 0;
        _selectLayerIndex = -1;
        _topoNodeFissionWeightDic.Clear();
    }

    public static void SetGUIStyle()
    {
        if (_popupSt != null)
            return;

        _popupSt = new GUIStyle(EditorStyles.popup);
        _popupSt.fontSize = 14;
        _popupSt.fixedHeight = _heapNodeContentHeight;
        _popupSt.fontStyle = FontStyle.Bold;
        _popupSt.normal.textColor = Color.red;
        _popupSt.focused.textColor = Color.red;

        _popupSt02 = new GUIStyle(EditorStyles.popup);
        _popupSt02.fontSize = 14;
        _popupSt02.fixedHeight = _heapNodeContentHeight;
        _popupSt02.fontStyle = FontStyle.Bold;
        _popupSt02.normal.textColor = new Color(0.2f, 0.8f, 0.2f, 1f);
        _popupSt02.focused.textColor = new Color(0.2f, 0.8f, 0.2f, 1f);

        _popupSt03 = new GUIStyle(EditorStyles.popup);
        _popupSt03.fontSize = 14;
        _popupSt03.fixedHeight = _heapNodeContentHeight;
        _popupSt03.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        _popupSt03.focused.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        _labelSt = new GUIStyle(EditorStyles.label);
        _labelSt.wordWrap = true;
        _labelSt.fontSize = 14;

        _textSt = new GUIStyle(EditorStyles.textField);
        _textSt.fontSize = 14;
        _textSt.fixedHeight = _heapNodeContentHeight;

        _labelSt02 = new GUIStyle(EditorStyles.label);
        _labelSt02.wordWrap = true;
        _labelSt02.alignment = TextAnchor.UpperLeft;
        _labelSt02.fontSize = 15;

        _buttonSt = new GUIStyle(EditorStyles.miniButton);
        _buttonSt.fontSize = 18;
        _buttonSt.fixedHeight = _heapNodeContentHeight;
        _buttonSt.fontStyle = FontStyle.Bold;
        _buttonSt.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        _buttonSt.focused.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);

        _buttonSt02 = new GUIStyle(EditorStyles.miniButton);
        _buttonSt02.fontSize = 18;
        _buttonSt02.fixedHeight = _heapNodeContentHeight;
        _buttonSt02.fontStyle = FontStyle.Bold;
        _buttonSt02.normal.textColor = Color.magenta;
        _buttonSt02.focused.textColor = Color.magenta;

        _toggleSt = new GUIStyle(EditorStyles.toggle);
        _toggleSt.fontSize = 18;
        _toggleSt.fixedHeight = _heapNodeContentHeight;
    }

    public virtual void RefreshWorkData()
    {
        if (Directory.Exists(SavePath))
        {
            _dataPathDic.Clear();

            string[] paths = Directory.GetFiles(SavePath);
            foreach (var path in paths)
            {
                if (Path.GetExtension(path).Contains(".meta"))
                    continue;

                uint workId = uint.Parse(Path.GetFileNameWithoutExtension(path));
                _dataPathDic.Add(workId, path);
            }
        }

        if (PlayerPrefs.HasKey(WorkStreamTypeKey))
            _workStreamType = PlayerPrefs.GetInt(WorkStreamTypeKey);
    }

    #region DrawUI
    private void DrawLeft()
    {
        _dragAreaEditor.BeginVerticalDragArea();

        int selectLeftWorkMenu = GUILayout.SelectionGrid(_selectLeftWorkMenu, _leftWorkMenuStrs, 2);
        if (_selectLeftWorkMenu != selectLeftWorkMenu)
        {
            _selectLeftWorkMenu = selectLeftWorkMenu;
            if (_selectLeftWorkMenu != 0)
                _selectToolMenu = 0;
        }
        if (_selectLeftWorkMenu == 0)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("查询:", GUILayout.Width(30f));
                    string textWorkIdStr = EditorGUILayout.TextField(_textWorkIdStr, GUILayout.Height(20f));
                    if (string.IsNullOrEmpty(textWorkIdStr))
                    {
                        _textWorkId = 0u;
                        _textWorkIdStr = null;
                        _textWorkIdCharArray = null;
                    }
                    else
                    {
                        if (uint.TryParse(textWorkIdStr, out uint textWorkId))
                        {
                            _textWorkId = textWorkId;
                            _textWorkIdStr = _textWorkId.ToString();
                            _textWorkIdCharArray = null;
                        }
                        else
                        {
                            _textWorkId = 0u;
                            _textWorkIdStr = textWorkIdStr;
                            //if (_regex.IsMatch(textWorkIdStr))
                            //    _textWorkIdStr = textWorkIdStr;
                            //else
                            //    _textWorkIdStr = null;
                        }
                    }

                    if (_textWorkId == 0u)
                        _checkWorkId = 0u;

                    if (GUILayout.Button("查询"))
                    {
                        _checkWorkId = _textWorkId;
                        _isFuzzyCheckId = false;

                        SetWorkIdArray();
                    }

                    if (GUILayout.Button("查询2"))
                    {
                        _checkWorkId = _textWorkId;
                        _isFuzzyCheckId = true;

                        SetWorkIdArray();
                    }
                }

                using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height((Screen.height - 70f) * (_useLeftDownSpace ? 0.5f : 1f))))
                {
                    using (EditorGUILayout.ScrollViewScope svs = new EditorGUILayout.ScrollViewScope(_workMenuScrollPos))
                    {
                        GUIStyle st = new GUIStyle(EditorStyles.miniButtonMid);
                        st.normal.background = WorkMenuBGTexture;
                        st.fontSize = 16;
                        st.fixedHeight = 20;
                        st.padding.bottom = 2;
                        st.fontStyle = FontStyle.Bold;

                        foreach (WorkStreamMenuInfoEditor workInfo in m_WorkMenuList)
                        {
                            DrawWorkMenu(workInfo.WorkId, workInfo.WorkStreamDes, st);
                        }

                        _workMenuScrollPos = svs.scrollPosition;
                    }
                }

                if (_useLeftDownSpace)
                {
                    DrawLeftDownSpaceToDynamicShow((Screen.height - 70f) * 0.5f);
                }
            }
        }
        else
        {
            DrawHelpToolLeft();
        }

        _dragAreaEditor.EndVerticalDragArea(Repaint);
    }

    private void SetWorkIdArray()
    {
        if (_textWorkId > 0u || string.IsNullOrEmpty(_textWorkIdStr))
        {
            _textWorkIdCharArray = null;
        }
        else
        {
            bool isCanUseWorkId = true;
            _textWorkIdCharArray = _textWorkIdStr.ToCharArray();
            if (_textWorkIdCharArray != null)
            {
                foreach (var workIdChar in _textWorkIdCharArray)
                {
                    if (!uint.TryParse(workIdChar.ToString(), out uint workIdCharuInt) && workIdChar != '*')
                    {
                        isCanUseWorkId = false;
                        break;
                    }
                }
            }
            if (!isCanUseWorkId)
                _textWorkIdCharArray = null;
        }
    }

    private void DrawWorkMenu(uint workId, string workDes, GUIStyle st)
    {
        string workIdStr = workId.ToString();

        if ((_checkWorkId == 0u || (!_isFuzzyCheckId && !workIdStr.Contains(_checkWorkId.ToString())) || (_isFuzzyCheckId && _checkWorkId != workId)) &&
            (_textWorkIdCharArray == null || _textWorkIdCharArray.Length <= 0))
            return;

        if (_textWorkIdCharArray != null && _textWorkIdCharArray.Length > 0)
        {
            int workIdCharLen = _textWorkIdCharArray.Length;
            int workIdBitTimes = (int)Mathf.Pow(10, workIdCharLen - 1);
            int workIdMaxBitVal = (int)workId / workIdBitTimes;
            if (workIdMaxBitVal <= 0 || workIdMaxBitVal >= 10)
                return;

            for (int i = 0; i < workIdCharLen; i++)
            {
                char workIdChar = _textWorkIdCharArray[i];
                if (uint.TryParse(workIdChar.ToString(), out uint workIdCharId))
                {
                    uint workIdCharBitLen = (uint)Mathf.Pow(10, workIdCharLen - 1 - i);
                    uint curBitVal = ((workId % (workIdCharBitLen * 10)) - (workId % workIdCharBitLen)) / workIdCharBitLen;
                    if (curBitVal != workIdCharId)
                        return;
                }
            }
        }
        
        if (_dataPathDic.ContainsKey(workId))
        {
            workIdStr = $"[已配置]{workId.ToString()}";
            st.normal.textColor = new Color(0.16f, 0.61f, 1f);
        }
        else
        {
            st.normal.textColor = Color.white;
        }

        workIdStr += $"{(string.IsNullOrEmpty(workDes) ? null : $" ({workDes})")}";

        if (workId == m_SelectWorkId)
        {
            st.normal.textColor = Color.yellow;
        }

        if (GUILayout.Button(workIdStr, st))
        {
            ClickWorkMenuBtn(workId);
        }
        GUILayout.Space(3f);
    }

    private void ClickWorkMenuBtn(uint btnWorkId, bool isForce = false)
    {
        if (isForce || m_SelectWorkId != btnWorkId)
        {
            ClearOldWorkState();

            m_SelectWorkId = btnWorkId;

            if (_dataPathDic.ContainsKey(m_SelectWorkId))
                ParseTxtToData(SavePath, m_SelectWorkId, m_WorkBlockDataList);

            _isShowCollectContent = false;
        }
    }

    private void LocationShowWorkId(uint workId)
    {
        _checkWorkId = _textWorkId = workId;
        _textWorkIdStr = workId.ToString();
        _textWorkIdCharArray = null;
        _isFuzzyCheckId = true;

        ClickWorkMenuBtn(workId);
    }

    private void DrawRight()
    {
        if (_selectLeftWorkMenu == 0)
        {
            DrawWorkStreamRight();
        }
        else
        {
            DrawHelpToolRight();
        }
    }

    private void DrawWorkStreamRight()
    {
        _showHelpTips = false;

        Rect rect = new Rect(_dragAreaEditor.Width + _dragAreaEditor.StartX, _dragAreaEditor.StartY, Screen.width - (_dragAreaEditor.Width + _dragAreaEditor.StartX), Screen.height);

        using (new EditorGUILayout.VerticalScope())
        {
            using (new EditorGUILayout.HorizontalScope())
            {

                string showFilePath = string.Empty;
                if (m_SelectWorkId > 0u)
                    _dataPathDic.TryGetValue(m_SelectWorkId, out showFilePath);
                EditorGUILayout.TextArea(showFilePath, _textSt);

                if (GUILayout.Button("刷新", _buttonSt, GUILayout.Width(60f)))
                {
                    ClickWorkMenuBtn(m_SelectWorkId, true);
                }

                GUILayout.Space(5.5f);

                int showTypeSelect = EditorGUILayout.Popup(_isHeapStyle ? 1 : 0, _showTypeStrs, _popupSt02, GUILayout.Width(100f));
                _isHeapStyle = showTypeSelect == 1;

                if (GUILayout.Button("添加工作块", _buttonSt, GUILayout.Width(98f)))
                {
                    if (m_SelectWorkId > 0u)
                    {
                        WorkBlockData workBlockData = new WorkBlockData();
                        workBlockData.TopWorkNodeData = new WorkNodeData();
                        workBlockData.TopWorkNodeData.Id = 1;
                        workBlockData.TopWorkNodeData.LayerIndex = -1;
                        workBlockData.TopWorkNodeData.IsMainLine = true;

                        m_WorkBlockDataList.Add(workBlockData);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "没有选择WorkId", "OK");
                    }
                }

                GUILayout.Space(5.5f);

#if ILRUNTIME_MODE
                GUILayout.Button("Save", _buttonSt02, GUILayout.Width(60f));
#else
                if (GUILayout.Button("Save", _buttonSt, GUILayout.Width(60f)))
                {
                    if (m_SelectWorkId > 0u)
                    {
                        if (EditorUtility.DisplayDialog("提示", $"是否保存WorkId : {m_SelectWorkId.ToString()}", "Save", "Cancel"))
                        {
                            SaveToTxt(SavePath, m_SelectWorkId, m_WorkBlockDataList);

                            if (_collectWorkBlockDic != null)
                            {
                                foreach (var kv in _collectWorkBlockDic)
                                {
                                    if (kv.Key == 0u || kv.Value == null)
                                        continue;

                                    SaveToTxt(SavePath, kv.Key, kv.Value);
                                }
                            }
                            return;
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "没有选择WorkId", "OK");
                    }
                }
#endif

                GUILayout.Space(5.5f);

                if (GUILayout.Button("另存为", _buttonSt, GUILayout.Width(75f)))
                {
                    if (m_SelectWorkId > 0u)
                    {
                        string saveAsFilePath = EditorUtility.SaveFilePanel("另存为(uint的名字)", "Assets/../../Design_Editor/", string.Empty, "txt");
                        if (!string.IsNullOrEmpty(saveAsFilePath) && uint.TryParse(Path.GetFileNameWithoutExtension(saveAsFilePath), out uint saveWorkId))
                        {
                            SaveToTxt($"{Path.GetDirectoryName(saveAsFilePath).Replace("\\", "/")}/", saveWorkId, m_WorkBlockDataList);
                            RefreshWorkData();
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "没有选择WorkId", "OK");
                    }
                }

                GUILayout.Space(5.5f);

#if ILRUNTIME_MODE
                GUILayout.Button("生成最终文件", _buttonSt02, GUILayout.Width(120f));
#else
                if (GUILayout.Button("生成最终文件", _buttonSt, GUILayout.Width(120f)))
                {
                    if (EditorUtility.DisplayDialog("提示", $"生成最终文件供游戏开启时读取", "SaveAll", "Cancel"))
                    {
                        SaveAll();
                    }
                }
#endif

                bool debugLog = GUILayout.Toggle(_debugLog, "Debug信息", GUILayout.Width(100f));
                if (debugLog != _debugLog)
                {
                    _debugLog = debugLog;

                    RefreshWorkStreamEnum();
                }

                bool showConcurrentColor = GUILayout.Toggle(_showConcurrentColor, "展示并行颜色", GUILayout.Width(100f));
                if (showConcurrentColor != _showConcurrentColor)
                {
                    _showConcurrentColor = showConcurrentColor;
                }

                bool popupTypeToggle = GUILayout.Toggle(PopupType > 1, "新popup", GUILayout.Width(100f));
                PopupType = popupTypeToggle ? 2 : 1;

                if (Application.isPlaying)
                {
                    if (GUILayout.Button("刷新数据", _buttonSt, GUILayout.Width(80f)))
                    {
                        WorkStreamConfigManager.Instance.RefreshAll();
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawRightSecondColumn();
            }
        }

        Rect scrollPosRect = new Rect(rect.x, rect.y + 55f, rect.width, rect.height - 75f);
        Rect scrollViewRect = new Rect(0f, 0f, _workBlocksViewPos.x, _workBlocksViewPos.y);
        _workBlocksScrollPos = GUI.BeginScrollView(scrollPosRect, _workBlocksScrollPos, scrollViewRect);

        _maxLayerIndex = 0;
        float viewHeight = 0f;
        for (int i = 0; i < m_WorkBlockDataList.Count; i++)
        {
            viewHeight += DrawWorkBlock(scrollViewRect, m_WorkBlockDataList, i);

            viewHeight += 20f;

            scrollViewRect.y = viewHeight;
        }

#region 处理显示收集的节点
        if (_isShowCollectContent)
        {
            if (_collectWorkBlockDic == null)
            {
                _collectWorkBlockDic = new Dictionary<uint, List<WorkBlockData>>();
                for (int i = 0; i < _collectIdList.Count; i++)
                {
                    var collectId = _collectIdList[i];

                    List<WorkBlockData> wbdList = new List<WorkBlockData>();
                    if (_dataPathDic.ContainsKey(collectId))
                        ParseTxtToData(SavePath, collectId, wbdList, false);

                    _collectWorkBlockDic.Add(collectId, wbdList);
                }
            }

            foreach (var kv in _collectWorkBlockDic)
            {
                for (int j = 0; j < kv.Value.Count; j++)
                {
                    viewHeight += DrawWorkBlock(scrollViewRect, kv.Value, j, kv.Key);

                    viewHeight += 20f;

                    scrollViewRect.y = viewHeight;
                }
            }
        }
#endregion

        if (viewHeight < rect.height - 75f)
            _workBlocksViewPos.y = rect.height - 75f;
        else
            _workBlocksViewPos.y = viewHeight;

        float viewWidth = 0f;
        if (_isHeapStyle)
            viewWidth = _blockLeftWidth + _maxLayerIndex * 20f + _heapNodeWidth;
        else
        {
            float blockRightWidth = _maxLayerIndex * _topoNodeWidth + _topoNodeWidth;
            viewWidth = _blockLeftWidth + (blockRightWidth < _heapNodeWidth ? _heapNodeWidth : blockRightWidth);
        }
        if (_workBlocksViewPos.x <= viewWidth)
            _workBlocksViewPos.x = viewWidth;
        else
        {
            _workBlocksViewPos.x = rect.width - 10f;
        }

        GUI.EndScrollView();
    }

    protected virtual void DrawRightSecondColumn() { }

    private float DrawWorkBlock(Rect rect, List<WorkBlockData> workBlockDataList, int i, uint workId = 0u)
    {
        WorkBlockData workBlockData = workBlockDataList[i];

        _blockHeight = 0f;
        _blockHeightDic.TryGetValue(workBlockData, out _blockHeight);

        if (_blockHeight < _blockBaseHeight)
            _blockHeight = _blockBaseHeight;

        GUI.Box(new Rect(rect.x + 5f, rect.y, _workBlocksViewPos.x, _blockHeight), string.Empty, _isHeapStyle ? GUI.skin.box : GUI.skin.button);

        if (workId == 0u)
        {
            if (GUI.Button(new Rect(rect.x + 10f, rect.y + 5f, 25f, 25f), "X", _buttonSt02))
            {
                workBlockDataList.Remove(workBlockData);
                return _blockHeight;
            }

            if (i > 0)
            {
                if (GUI.Button(new Rect(rect.x + 45f, rect.y + 5f, 45f, 25f), "上移", _buttonSt))
                {
                    workBlockDataList[i] = workBlockDataList[i - 1];
                    workBlockDataList[i - 1] = workBlockData;
                    return _blockHeight;
                }
            }

            if (i < workBlockDataList.Count - 1)
            {
                if (GUI.Button(new Rect(rect.x + 100f, rect.y + 5f, 45f, 25f), "下移", _buttonSt))
                {
                    workBlockDataList[i] = workBlockDataList[i + 1];
                    workBlockDataList[i + 1] = workBlockData;
                    return _blockHeight;
                }
            }

            if (GUI.Button(new Rect(rect.x + _blockLeftWidth - 45f, rect.y + 5f, 25f, 25f), "+", _buttonSt))
            {
                AddChildNode(workBlockData, workBlockData.TopWorkNodeData, -1);
            }
        }
        else
        {
            EditorGUI.LabelField(new Rect(rect.x + 10f, rect.y + 5f, 200f, 25f), $"{workId.ToString()}", _labelSt);
        }

        EditorGUI.LabelField(new Rect(rect.x + 10f, rect.y + 50f, _blockLeftWidth * 0.6f, 20f), "当前:", _labelSt);

        int blockIndex = GetIndexByEnum(workBlockData.CurWorkBlockType, WorkBlockEnumDic);
        blockIndex = EditorGUI.Popup(new Rect(rect.x + 50f, rect.y + 50f, _blockLeftWidth * 0.7f, 20f), blockIndex,
            WorkBlockEnumStrs, blockIndex == 0 ? _popupSt : _popupSt02);
        workBlockData.CurWorkBlockType = WorkBlockEnumDic[blockIndex];
        workBlockData.TopWorkNodeData.NodeType = workBlockData.CurWorkBlockType;

        EditorGUI.LabelField(new Rect(rect.x + 10f, rect.y + 80f, _blockLeftWidth * 0.6f, 20f), workBlockData.TopWorkNodeData.Id.ToString(), _labelSt);

        workBlockData.AttachType = (byte)EditorGUI.Popup(new Rect(rect.x + 50f, rect.y + 80f, _blockLeftWidth * 0.5f, 20f), workBlockData.AttachType,
            AttachEnumStrs, _popupSt02);

        EditorGUI.LabelField(new Rect(rect.x + 10f, rect.y + 110f, _blockLeftWidth * 0.6f, 20f), "跳转:", _labelSt);

        blockIndex = GetIndexByEnum(workBlockData.NextBlockType, WorkBlockEnumDic);
        blockIndex = EditorGUI.Popup(new Rect(rect.x + 50f, rect.y + 110f, _blockLeftWidth * 0.7f, 20f), blockIndex,
            WorkBlockEnumStrs, _popupSt02);
        workBlockData.NextBlockType = WorkBlockEnumDic[blockIndex];

        if (GUI.Button(new Rect(rect.x + 10f, rect.y + 140f, 50f, 25f), "复制", _buttonSt))
        {
            _copyWorkBlockData = workBlockData;
        }
        if (_copyWorkBlockData != null && GUI.Button(new Rect(rect.x + 70f, rect.y + 140f, 50f, 25f), "粘贴", _buttonSt))
        {
            var blockData = FileDataOperationManager.DeepCopyObj<WorkBlockData>(_copyWorkBlockData);
            if (blockData != null)
                m_WorkBlockDataList.Add(blockData);
        }

#region 鼠标辅助作用
        if (Event.current.type == EventType.MouseDown)
        {
            if (Event.current.button == 1)
            {
                Vector2 mousePos = Event.current.mousePosition;
                if (mousePos.x > rect.x && mousePos.x < rect.x + _blockLeftWidth - 10f && mousePos.y > rect.y && mousePos.y < rect.y + _blockHeight - 1f)
                {
                    _nodeOperateFunc.IsOperate = true;
                    _nodeOperateFunc.OpRect = new Rect(mousePos, Vector2.one * 200f);
                    _nodeOperateFunc.WBlockData = workBlockData;
                    _nodeOperateFunc.WNodeData = workBlockData.TopWorkNodeData;
                    Event.current.Use();
                }
            }
            else if (Event.current.button == 2)
            {
                _nodeOperateFunc.IsOperate = false;
                Event.current.Use();
            }
        }
#endregion

        Handles.color = Color.black;
        Handles.DrawDottedLine(new Vector3(_blockLeftWidth - 10f, rect.y + 2f, 0f), new Vector3(_blockLeftWidth - 10f, rect.y + _blockHeight - 1f, 0f), 1f);

        if (_isHeapStyle)
        {
            _blockHeight = 5f;
            DrawWorkNode_HeapStyle(workBlockData, workBlockData.TopWorkNodeData, rect.y, _blockHeight, false);
            if (_blockHeight < _blockBaseHeight)
                _blockHeight = _blockBaseHeight;
        }
        else
        {
            _blockHeight = GetTopoBlockTotalHeight(workBlockData);
            DrawWorkNode_TopoStyle(workBlockData, workBlockData.TopWorkNodeData, rect.y + 10f + _blockHeight * 0.5f - _topoNodeHeight * 0.5f, _blockHeight, false);
        }

        if (_showHelpTips)
        {
            if (_nodeEnumParamsNoteDic.TryGetValue(_helpTipsNodeType, out string htcs) && !string.IsNullOrEmpty(htcs))
                _helpTipsContentStr = $"{htcs} ";
            else
                _helpTipsContentStr = "*没填说明";
            _helpTipsRect.width = _helpTipsContentStr.Length * _labelSt.fontSize;
            GUI.Box(_helpTipsRect, string.Empty);
            EditorGUI.LabelField(_helpTipsRect, _helpTipsContentStr, _labelSt);
        }

        if (_nodeOperateFunc.IsOperate)
        {
            _nodeOperateFunc.OpRect.width = 200f;
            _nodeOperateFunc.OpRect.height = 200f;

            float x = _nodeOperateFunc.OpRect.x + 10f;
            float y = _nodeOperateFunc.OpRect.y;
            float w = 180f;
            float h = 30f;

            GUI.Box(_nodeOperateFunc.OpRect, string.Empty, GUI.skin.button);

            GUI.Label(new Rect(x, y - 5f, w, h), "*按鼠标滚轮隐藏弹出框*");

            y += 25f;
            if (_nodeOperateFunc.WBlockData.TopWorkNodeData != _nodeOperateFunc.WNodeData)
            {
                if (GUI.Button(new Rect(x, y, w, h), "只复制该数据"))
                {
                    _nodeOperateFunc.CopyWNData = _nodeOperateFunc.WNodeData;
                    _nodeOperateFunc.CopyType = 1;
                }

                y += 5f + h;
                if (GUI.Button(new Rect(x, y, w, h), "复制该数据以及子节点数据"))
                {
                    _nodeOperateFunc.CopyWNData = _nodeOperateFunc.WNodeData;
                    _nodeOperateFunc.CopyType = 2;
                }

                y += 5f + h;
            }

            if (_nodeOperateFunc.CopyWNData == null)
            {
                GUI.Label(new Rect(x, y, w, h), "没有数据粘贴", _labelSt);
            }
            else
            {
                if (GUI.Button(new Rect(x, y, w, h), $"粘贴数据NodeId:{_nodeOperateFunc.CopyWNData.Id}作为子节点"))
                {
                    AddNodeByCopyNodeDatas(_nodeOperateFunc.WBlockData, _nodeOperateFunc.WNodeData, _nodeOperateFunc.CopyWNData, _nodeOperateFunc.CopyType);
                }
                if (_nodeOperateFunc.WBlockData.TopWorkNodeData != _nodeOperateFunc.WNodeData)
                {
                    y += 5f + h;
                    if (GUI.Button(new Rect(x, y, w, h), $"粘贴到该节点【下面】"))
                    {
                        AddNodeByCopyNodeDatas(_nodeOperateFunc.WBlockData, _nodeOperateFunc.WNodeData, _nodeOperateFunc.CopyWNData, _nodeOperateFunc.CopyType, 0);
                    }
                    y += 5f + h;
                    if (GUI.Button(new Rect(x, y, w, h), $"粘贴到该节点【上面】"))
                    {
                        AddNodeByCopyNodeDatas(_nodeOperateFunc.WBlockData, _nodeOperateFunc.WNodeData, _nodeOperateFunc.CopyWNData, _nodeOperateFunc.CopyType, 1);
                    }
                }
            }

            CustomDrawNodeOperateFunc(_nodeOperateFunc, _nodeOperateFunc.OpRect);
        }

        //if (_nodeWidgetHelpFunc.IsHelp)
        //{
        //    if (_nodeWidgetHelpFunc.IsShow)
        //    {
        //        GUI.Box(_nodeWidgetHelpFunc.ShowRect, string.Empty, GUI.skin.button);

        //        string helpStr = string.Empty;
        //        if (_nodeWidgetHelpFunc.WidgetType == 1)
        //        {
        //            helpStr = "=-1默认一定执行，其他值为选择组进行执行（标注：组数范围为-128到127)";
        //        }
        //        else if (_nodeWidgetHelpFunc.WidgetType == 2)
        //        {
        //            helpStr = "在同一组中是否并发执行";
        //        }
        //        GUI.Label(_nodeWidgetHelpFunc.ShowRect, helpStr, _labelSt02);
        //    }
        //}

        _blockHeightDic[workBlockData] = _blockHeight;

        return _blockHeight;
    }

    protected virtual void CustomDrawNodeOperateFunc(NodeOperateFunc nodeOperateFunc, Rect rect) { }
    
#region 拓扑图式画UI展示
    private void DrawWorkNode_TopoStyle(WorkBlockData workBlockData, WorkNodeData workNodeData, float startY, float height, bool isDrawNode, WorkNodeData parent = null, int parentTransIndex = -1, int parentGroupNodeIndex = -1, float parentStartX = 0, float parentStartY = 0)
    {
        float x = 0f;
        if (isDrawNode)
        {
            if (_maxLayerIndex < workNodeData.LayerIndex)
                _maxLayerIndex = workNodeData.LayerIndex;

            x = _blockLeftWidth + workNodeData.LayerIndex * _topoNodeWidth;
            float y = startY;

            if (workNodeData.LayerIndex == _selectLayerIndex && workNodeData.GroupIndex == _selectGroupIndex && (parent != null && parent.Id == _selectParentId))
            {
                Handles.color = Color.red;
                Handles.DrawPolyLine(new Vector3[] {new Vector3(x, y, 0f),
                                                    new Vector3(x + _topoNodeContentWidth + 1f, y, 0f),
                                                    new Vector3(x + _topoNodeContentWidth + 1, y + _topoNodeContentHeight + 1f, 0f),
                                                    new Vector3(x, y + _topoNodeContentHeight + 1f, 0f),
                                                    new Vector3(x, y, 0f)});
            }

            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 1)
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    if (mousePos.x > x && mousePos.x < x + _topoNodeWidth && mousePos.y > y && mousePos.y < y + _topoNodeHeight)
                    {
                        if (Event.current.alt)
                        {
                            _selectLayerIndex = workNodeData.LayerIndex;
                            _selectGroupIndex = workNodeData.GroupIndex;
                            _selectParentId = parent == null ? 0u : parent.Id;
                        }
                        else
                        {
                            _nodeOperateFunc.IsOperate = true;
                            _nodeOperateFunc.OpRect = new Rect(mousePos, Vector2.one * 200f);
                            _nodeOperateFunc.WBlockData = workBlockData;
                            _nodeOperateFunc.WNodeData = workNodeData;
                        }
                        Event.current.Use();
                    }
                }
                else if (Event.current.button == 2)
                {
                    _nodeOperateFunc.IsOperate = false;
                    _selectLayerIndex = -1;
                    Event.current.Use();
                }
            }

            if (_showConcurrentColor)
            {
                WorkNodeData concurrentNode = GetNodeConcurrentNode(workBlockData, parent, workNodeData);
                if (concurrentNode != null)
                {
                    if (!_concurrentNodeColorDic.TryGetValue(concurrentNode.Id, out Color color))
                    {
                        color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0.7f);
                        _concurrentNodeColorDic[concurrentNode.Id] = color;
                    }

                    EditorGUI.DrawRect(new Rect(x - 4f, y - 4f, _topoNodeContentWidth + 8f, _topoNodeContentHeight + 8f), color);
                }
            }

            if (_useLeftDownSpace)
                ShowDynamicRunNode(workBlockData, workNodeData, x - 3f, y - 3f, _topoNodeContentWidth + 6f, _topoNodeContentHeight + 6f);

            GUI.Box(new Rect(x, y, _topoNodeContentWidth, _topoNodeContentHeight), string.Empty, GUI.skin.box);

            float lineX = x;
            string groupIndexStr = EditorGUI.TextField(new Rect(lineX, y, 30f, _heapNodeContentHeight), workNodeData.GroupIndex.ToString(), _textSt);
            if (sbyte.TryParse(groupIndexStr, out sbyte groupIndex) && groupIndex != workNodeData.GroupIndex)
            {
                ChangeGroupIndex(workBlockData, parent, workNodeData, groupIndex, parentTransIndex, parentGroupNodeIndex);

                return;
            }
            NodeWidgetHelpBox(new Rect(lineX, y, 30f, _heapNodeContentHeight), workNodeData, _nodeWidgetHelpFunc, 1);

            lineX += 35f;
            bool isConcurrent = EditorGUI.Toggle(new Rect(lineX, y, 20f, _heapNodeContentHeight), workNodeData.IsConcurrent, _toggleSt);
            if (isConcurrent != workNodeData.IsConcurrent)
            {
                ChangeConcurrent(workNodeData, parent, isConcurrent);
            }
            NodeWidgetHelpBox(new Rect(lineX, y, 20f, _heapNodeContentHeight), workNodeData, _nodeWidgetHelpFunc, 2);

            lineX += 25f;
            if (GUI.Button(new Rect(lineX, y, 20, _heapNodeContentHeight), "+", _buttonSt))
            {
                AddChildNode(workBlockData, workNodeData, -1);
                return;
            }

            if (GUI.Button(new Rect(x + _topoNodeContentWidth, y, 20, _heapNodeContentHeight), "X", _buttonSt02))
            {
                RemoveNode(parent, parentTransIndex, parentGroupNodeIndex);
                return;
            }

            float helpx = x + _topoNodeContentWidth;
            float helpy = y + _heapNodeContentHeight;
            GUI.Box(new Rect(helpx, helpy, 20, _heapNodeContentHeight), "?", _buttonSt);
            if (Event.current.mousePosition.x > helpx && Event.current.mousePosition.x < helpx + 20 &&
                Event.current.mousePosition.y > helpy && Event.current.mousePosition.y < helpy + _heapNodeContentHeight)
            {
                _showHelpTips = true;
                _helpTipsNodeType = workNodeData.NodeType;
                _helpTipsContentStr = string.Empty;
                _helpTipsRect = new Rect(helpx + 20f, helpy, _helpTipsContentStr.Length * _labelSt.fontSize, _heapNodeContentHeight);
            }

            if (parent != null)
            {
                lineX += 25f;
                if (GUI.Button(new Rect(lineX, y, 20, _heapNodeContentHeight), "B", _buttonSt))
                {
                    AddBrotherNode(workBlockData, parent, workNodeData);
                    return;
                }

                List<WorkNodeData> nodeList = parent.TransitionWorkGroupList[parentTransIndex].NodeList;
                int upNodeIndex = parentGroupNodeIndex - 1;

                if (upNodeIndex > -1)
                {
                    lineX += 25f;
                    if (GUI.Button(new Rect(lineX, y, 20, _heapNodeContentHeight), "上", _buttonSt))
                    {
                        MoveNodeInGroup(workNodeData, parent, parentTransIndex, parentGroupNodeIndex, -1);

                        return;
                    }
                }

                int downNodeIndex = parentGroupNodeIndex + 1;
                if (downNodeIndex < nodeList.Count)
                {
                    lineX += 25f;
                    if (GUI.Button(new Rect(lineX, y, 20, _heapNodeContentHeight), "下", _buttonSt))
                    {
                        MoveNodeInGroup(workNodeData, parent, parentTransIndex, parentGroupNodeIndex, 1);

                        return;
                    }
                }
            }

            y += _heapNodeContentHeight;
            int nodeIndex = GetIndexByEnum(workNodeData.NodeType, WorkNodeEnumDic);
            nodeIndex = CustomPopup(new Rect(x, y, _topoNodeContentWidth, _heapNodeContentHeight),
                nodeIndex, WorkNodeEnumStrs, nodeIndex == 0 ? _popupSt : _popupSt02,
                PopupType < 2 ? (Action<int>)null : delegate (int selectIndex) {
                    workNodeData.NodeType = WorkNodeEnumDic[selectIndex];
                });
            workNodeData.NodeType = WorkNodeEnumDic[nodeIndex];

            y += _heapNodeContentHeight;
            workNodeData.NodeContent = EditorGUI.TextField(new Rect(x, y, _topoNodeContentWidth, _heapNodeContentHeight), workNodeData.NodeContent, _textSt);

            y += _heapNodeContentHeight;
            int blockIndex = GetIndexByEnum(workNodeData.SkipWorkBlockType, WorkBlockEnumDic);
            blockIndex = EditorGUI.Popup(new Rect(x, y, _topoNodeContentWidth, _heapNodeContentHeight), blockIndex, WorkBlockEnumStrs, _popupSt02);
            workNodeData.SkipWorkBlockType = WorkBlockEnumDic[blockIndex];

            if (_debugLog)
            {
                EditorGUI.LabelField(new Rect(x + _topoNodeContentWidth, startY + _heapNodeContentHeight, _heapNodeContentHeight * 2, 20f), $"{workNodeData.Id.ToString()}|{(workNodeData.IsMainLine ? "T" : "F")}", _labelSt);
                EditorGUI.LabelField(new Rect(x + _topoNodeContentWidth, startY + _heapNodeContentHeight * 2, _heapNodeContentHeight * 2, 20f), $"{workNodeData.LayerIndex.ToString()}|{workNodeData.InParentGroupNodeListIndex.ToString()}", _labelSt);
            }

            //EditorGUI.LabelField(new Rect(x, startY + _topoNodeContentHeight - _heapNodeContentHeight, _topoNodeContentWidth, _heapNodeContentHeight), 
            //    $"({workNodeData.Id.ToString()}|{(workNodeData.IsMainLine ? "T" : "F")}){workNodeData.LayerIndex.ToString()}|{workNodeData.InParentGroupNodeListIndex.ToString()}", _labelSt);

            if (workNodeData.LayerIndex > 0)
            {
                EditorToolHelp.DrawNodeCurve(new Rect(parentStartX, parentStartY, _topoNodeContentWidth, _topoNodeContentHeight),
                    new Rect(x, startY, 0f, _topoNodeContentHeight), false);
            }
        }

        if (workNodeData.TransitionWorkGroupList != null)
        {
            if (workNodeData.TransitionWorkGroupList.Count == 0)
            {
                workNodeData.TransitionWorkGroupList = null;
                return;
            }
            else
            {
                _topoNodeFissionWeightDic.TryGetValue(workNodeData, out int weight);
                int weightCount = 0;
                float heightPer = height / weight;

                for (int m = 0; m < workNodeData.TransitionWorkGroupList.Count; m++)
                {
                    WorkGroupNodeData groupData = workNodeData.TransitionWorkGroupList[m];
                    if (groupData.NodeList == null || groupData.NodeList.Count == 0)
                    {
                        workNodeData.TransitionWorkGroupList.RemoveAt(m);
                        return;
                    }

                    for (int n = 0; n < groupData.NodeList.Count; n++)
                    {
                        WorkNodeData child = groupData.NodeList[n];

                        if (!_topoNodeFissionWeightDic.TryGetValue(child, out int childWeight))
                            continue;

                        float childHeight = heightPer * childWeight;
                        float childY = startY - height * 0.5f + heightPer * (weightCount + childWeight * 0.5f);

                        weightCount += childWeight;

                        DrawWorkNode_TopoStyle(workBlockData, child, childY, childHeight, true, workNodeData, m, n, x, startY);

                        if (n == 0 && m > 0)
                        {
                            float cx = _blockLeftWidth + child.LayerIndex * _topoNodeWidth;
                            var oldColor = Handles.color;
                            Handles.color = new Color(0.3f, 0.2f, 1f);
                            Handles.DrawLine(new Vector3(cx, childY - 5f, 0f), new Vector3(cx + _topoNodeContentWidth + 15f, childY - 5f, 0f));
                            Handles.DrawLine(new Vector3(cx, childY - 4.5f, 0f), new Vector3(cx + _topoNodeContentWidth + 15f, childY - 4.5f, 0f));
                            Handles.DrawLine(new Vector3(cx, childY - 4f, 0f), new Vector3(cx + _topoNodeContentWidth + 15f, childY - 4f, 0f));
                            Handles.color = oldColor;
                        }
                    }
                }
            }
        }
    }

    private float GetTopoBlockTotalHeight(WorkBlockData workBlockData)
    {
        if (workBlockData.TopWorkNodeData.TransitionWorkGroupList == null)
            return _blockBaseHeight;

        int count = 1;
        _topoNodeFissionWeightDic.Clear();
        FissionNode(workBlockData.TopWorkNodeData, ref count);

        float height = count * _topoNodeHeight;

        return height < _blockBaseHeight ? _blockBaseHeight : height;
    }
#endregion

#region 堆积木式画UI展示
    private void DrawWorkNode_HeapStyle(WorkBlockData workBlockData, WorkNodeData workNodeData, float startY, float height, bool isDrawNode, WorkNodeData parent = null, int parentTransIndex = -1, int parentGroupNodeIndex = -1)
    {
        if (isDrawNode)
        {
            if (_maxLayerIndex < workNodeData.LayerIndex)
                _maxLayerIndex = workNodeData.LayerIndex;

            float startX = workNodeData.LayerIndex * 20f + _blockLeftWidth;
            float x = startX;
            float y = startY + height;

            if (workNodeData.LayerIndex == _selectLayerIndex && workNodeData.GroupIndex == _selectGroupIndex && (parent != null && parent.Id == _selectParentId))
            {
                Handles.color = Color.red;
                Handles.DrawPolyLine(new Vector3[] {new Vector3(_blockLeftWidth, y, 0f),
                                                    new Vector3(startX + _heapNodeContentWidth, y, 0f),
                                                    new Vector3(startX + _heapNodeContentWidth, y + _heapNodeContentHeight, 0f),
                                                    new Vector3(_blockLeftWidth, y + _heapNodeContentHeight, 0f),
                                                    new Vector3(_blockLeftWidth, y, 0f)});
            }

            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 1)
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    if (mousePos.x > _blockLeftWidth && mousePos.x < _blockLeftWidth + _heapNodeWidth && mousePos.y > y && mousePos.y < y + _heapNodeHeight)
                    {
                        if (Event.current.alt)
                        {
                            _selectLayerIndex = workNodeData.LayerIndex;
                            _selectGroupIndex = workNodeData.GroupIndex;
                            _selectParentId = parent == null ? 0u : parent.Id;
                        }
                        else
                        {
                            _nodeOperateFunc.IsOperate = true;
                            _nodeOperateFunc.OpRect = new Rect(mousePos, Vector2.one * 200f);
                            _nodeOperateFunc.WBlockData = workBlockData;
                            _nodeOperateFunc.WNodeData = workNodeData;
                        }
                        Event.current.Use();
                    }
                }
                else if (Event.current.button == 2)
                {
                    _nodeOperateFunc.IsOperate = false;
                    _selectLayerIndex = -1;
                    Event.current.Use();
                }
            }

            if (_showConcurrentColor)
            {
                WorkNodeData concurrentNode = GetNodeConcurrentNode(workBlockData, parent, workNodeData);
                if (concurrentNode != null)
                {
                    if (!_concurrentNodeColorDic.TryGetValue(concurrentNode.Id, out Color color))
                    {
                        color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0.5f);
                        _concurrentNodeColorDic[concurrentNode.Id] = color;
                    }

                    EditorGUI.DrawRect(new Rect(x - 3f, y - 3f, _heapNodeContentWidth + 6f, _heapNodeContentHeight + 6f), color);
                }
            }

            if (_useLeftDownSpace)
                ShowDynamicRunNode(workBlockData, workNodeData, x - 3f, y - 3f, _heapNodeContentWidth + 6f, _heapNodeContentHeight + 6f);

            GUI.Box(new Rect(x, y, _heapNodeContentWidth, _heapNodeContentHeight), string.Empty, GUI.skin.box);

            string groupIndexStr = EditorGUI.TextField(new Rect(x, y, 30f, _heapNodeContentHeight), workNodeData.GroupIndex.ToString(), _textSt);
            if (sbyte.TryParse(groupIndexStr, out sbyte groupIndex) && groupIndex != workNodeData.GroupIndex)
            {
                ChangeGroupIndex(workBlockData, parent, workNodeData, groupIndex, parentTransIndex, parentGroupNodeIndex);

                return;
            }
            NodeWidgetHelpBox(new Rect(x, y, 30f, _heapNodeContentHeight), workNodeData, _nodeWidgetHelpFunc, 1);

            x += 35f;
            bool isConcurrent = EditorGUI.Toggle(new Rect(x, y, 20f, _heapNodeContentHeight), workNodeData.IsConcurrent, _toggleSt);
            if (isConcurrent != workNodeData.IsConcurrent)
            {
                ChangeConcurrent(workNodeData, parent, isConcurrent);
            }
            NodeWidgetHelpBox(new Rect(x, y, 20f, _heapNodeContentHeight), workNodeData, _nodeWidgetHelpFunc, 2);

            x += 25f;
            int nodeIndex = GetIndexByEnum(workNodeData.NodeType, WorkNodeEnumDic);
            nodeIndex = CustomPopup(new Rect(x, y, _heapStyleNodeTypeWidth, _heapNodeContentHeight),
                nodeIndex, WorkNodeEnumStrs, nodeIndex == 0 ? _popupSt : _popupSt03,
                PopupType < 2 ? (Action<int>)null : delegate (int selectIndex) {
                    workNodeData.NodeType = WorkNodeEnumDic[selectIndex];
                });
            workNodeData.NodeType = WorkNodeEnumDic[nodeIndex];

            x += _heapStyleNodeTypeWidth + 5f;
            workNodeData.NodeContent = EditorGUI.TextField(new Rect(x, y, _heapStyleNodeContentWidth, _heapNodeContentHeight), workNodeData.NodeContent, _textSt);

            x += _heapStyleNodeContentWidth + 5f;
            int blockIndex = GetIndexByEnum(workNodeData.SkipWorkBlockType, WorkBlockEnumDic);
            blockIndex = EditorGUI.Popup(new Rect(x, y, 120f, _heapNodeContentHeight), blockIndex, WorkBlockEnumStrs, _popupSt03);
            workNodeData.SkipWorkBlockType = WorkBlockEnumDic[blockIndex];

            x += 125f;
            if (GUI.Button(new Rect(x, y, 20, _heapNodeContentHeight), "+", _buttonSt))
            {
                AddChildNode(workBlockData, workNodeData, -1);
            }

            x += 25f;
            if (GUI.Button(new Rect(x, y, 20, _heapNodeContentHeight), "X", _buttonSt))
            {
                RemoveNode(parent, parentTransIndex, parentGroupNodeIndex);
                return;
            }

            if (parent != null)
            {
                x += 25f;
                if (GUI.Button(new Rect(x, y, 20, _heapNodeContentHeight), "B", _buttonSt))
                {
                    AddBrotherNode(workBlockData, parent, workNodeData);
                }

                List<WorkNodeData> nodeList = parent.TransitionWorkGroupList[parentTransIndex].NodeList;
                int upNodeIndex = parentGroupNodeIndex - 1;

                if (upNodeIndex > -1)
                {
                    x += 25f;
                    if (GUI.Button(new Rect(x, y, 20, _heapNodeContentHeight), "上", _buttonSt))
                    {
                        MoveNodeInGroup(workNodeData, parent, parentTransIndex, parentGroupNodeIndex, -1);

                        return;
                    }
                }

                int downNodeIndex = parentGroupNodeIndex + 1;
                if (downNodeIndex < nodeList.Count)
                {
                    x += 25f;
                    if (GUI.Button(new Rect(x, y, 20, _heapNodeContentHeight), "下", _buttonSt))
                    {
                        MoveNodeInGroup(workNodeData, parent, parentTransIndex, parentGroupNodeIndex, 1);

                        return;
                    }
                }
            }

            x += 25f;
            GUI.Box(new Rect(x, y, 20, _heapNodeContentHeight), "?", _buttonSt);
            if (Event.current.mousePosition.x > x && Event.current.mousePosition.x < x + 20 &&
                Event.current.mousePosition.y > y && Event.current.mousePosition.y < y + _heapNodeContentHeight)
            {
                _showHelpTips = true;
                _helpTipsNodeType = workNodeData.NodeType;
                _helpTipsContentStr = string.Empty;
                _helpTipsRect = new Rect(x + 25f, y, _helpTipsContentStr.Length * _labelSt.fontSize, _heapNodeContentHeight);
            }

            if (_debugLog)
                EditorGUI.LabelField(new Rect(startX + _heapNodeWidth - 100f, y, 100, _heapNodeContentHeight), $"({workNodeData.Id.ToString()}|{(workNodeData.IsMainLine ? "T" : "F")}){workNodeData.LayerIndex.ToString()}|{workNodeData.InParentGroupNodeListIndex.ToString()}", _labelSt);

            _blockHeight += _heapNodeHeight;
        }

        if (workNodeData.TransitionWorkGroupList != null)
        {
            if (workNodeData.TransitionWorkGroupList.Count == 0)
            {
                workNodeData.TransitionWorkGroupList = null;
                return;
            }
            else
            {
                for (int m = 0; m < workNodeData.TransitionWorkGroupList.Count; m++)
                {
                    WorkGroupNodeData groupData = workNodeData.TransitionWorkGroupList[m];
                    if (groupData.NodeList == null || groupData.NodeList.Count == 0)
                    {
                        workNodeData.TransitionWorkGroupList.RemoveAt(m);
                        return;
                    }

                    for (int n = 0; n < groupData.NodeList.Count; n++)
                    {
                        WorkNodeData child = groupData.NodeList[n];
                        DrawWorkNode_HeapStyle(workBlockData, child, startY, _blockHeight, true, workNodeData, m, n);

                        if ((n + 1 == groupData.NodeList.Count) && (m + 1 < workNodeData.TransitionWorkGroupList.Count))
                        {
                            float cx = workNodeData.LayerIndex * 20f + _blockLeftWidth;
                            var oldColor = Handles.color;
                            Handles.color = new Color(1f, 0.1f, 0f);
                            //Handles.DrawLine(new Vector3(cx, childY + childHeight - 5f, 0f), new Vector3(cx + _topoNodeContentWidth, childY + childHeight - 5f, 0f));
                            //Handles.DrawLine(new Vector3(cx, childY + childHeight - 4.5f, 0f), new Vector3(cx + _topoNodeContentWidth, childY + childHeight - 4.5f, 0f));
                            //Handles.DrawLine(new Vector3(cx, childY + childHeight - 4f, 0f), new Vector3(cx + _topoNodeContentWidth, childY + childHeight - 4f, 0f));
                            Handles.color = oldColor;
                        }
                    }
                }
            }
        }
    }
#endregion

    private void NodeWidgetHelpBox(Rect rect, WorkNodeData workNodeData, NodeWidgetHelpFunc nodeWidgetHelpFunc, int widgetType)
    {
        //if (Event.current.type == EventType.MouseDown)
        {
            Vector2 mousePos = Event.current.mousePosition;
            if (mousePos.x > rect.x && mousePos.x < rect.x + rect.width && mousePos.y > rect.y && mousePos.y < rect.y + rect.height)
            {
                if (nodeWidgetHelpFunc.IsHelp)
                {
                    if (nodeWidgetHelpFunc.WNodeData != workNodeData || nodeWidgetHelpFunc.WidgetType != widgetType)
                    {
                        nodeWidgetHelpFunc.StayTime = 0f;
                        nodeWidgetHelpFunc.IsShow = false;
                    }
                    else
                    {
                        nodeWidgetHelpFunc.StayTime += Time.deltaTime;
                    }

                    if (nodeWidgetHelpFunc.StayTime > 1f)
                    {
                        nodeWidgetHelpFunc.IsShow = true;
                    }
                }

                nodeWidgetHelpFunc.WNodeData = workNodeData;
                nodeWidgetHelpFunc.WidgetType = widgetType;
                nodeWidgetHelpFunc.IsHelp = true;
                nodeWidgetHelpFunc.ShowRect = new Rect(rect.x + rect.width + 2f, rect.y, 200f, 200f);
            }
            else
            {
                if (nodeWidgetHelpFunc.IsHelp)
                {
                    if (nodeWidgetHelpFunc.WNodeData == workNodeData && nodeWidgetHelpFunc.WidgetType == widgetType)
                    {
                        nodeWidgetHelpFunc.StayTime = 0f;
                        nodeWidgetHelpFunc.IsHelp = false;
                        nodeWidgetHelpFunc.IsShow = false;
                    }
                }
            }

            //Event.current.Use();
        }
    }
    
#endregion

#region 数据文件处理
    protected void ParseTxtToData(string filePath, uint selectWorkId, List<WorkBlockData> workBlockDatas, bool compareSelect = true, int fileDataOperationType = -1)
    {
        workBlockDatas.Clear();

        if (!Directory.Exists(filePath))
        {
            Debug.LogError($"不存在文件夹路径{filePath}");
            return;
        }

        string path = $"{filePath}{selectWorkId.ToString()}.txt";

        if (!File.Exists(path))
        {
            Debug.LogError($"文件夹路径{filePath}不存在文件{selectWorkId.ToString()}.txt");
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
            if (compareSelect && m_SelectWorkId != workId)
            {
                Debug.LogError($"选择{m_SelectWorkId.ToString()}，但加载{workId.ToString()}");
                return;
            }
        });

        SetTxtToData(workBlockDatas, workStreamData);
    }
    
    protected void SetTxtToData(List<WorkBlockData> workBlockDatas, WorkStreamData workStreamData)
    {
        if (workStreamData.AttackWorkBlockDatas != null)
        {
            foreach (var workBlockData in workStreamData.AttackWorkBlockDatas)
            {
                workBlockDatas.Add(workBlockData);
            }
        }
        if (workStreamData.TargetWorkBlockDatas != null)
        {
            foreach (var workBlockData in workStreamData.TargetWorkBlockDatas)
            {
                workBlockDatas.Add(workBlockData);
            }
        }
    }

    protected void SaveToTxt(string filePath, uint selectWorkId, List<WorkBlockData> workBlockDatas)
    {
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);

        string path = $"{filePath}{selectWorkId.ToString()}.txt";

        WorkStreamData workStreamData = new WorkStreamData();
        for (int i = 0; i < workBlockDatas.Count; i++)
        {
            WorkBlockData workBlockData = workBlockDatas[i];
            if (workBlockData == null || workBlockData.TopWorkNodeData.TransitionWorkGroupList == null || workBlockData.TopWorkNodeData.TransitionWorkGroupList.Count == 0)
                continue;

            if (workBlockData.AttachType == 0)
            {
                if (workStreamData.AttackWorkBlockDatas == null)
                    workStreamData.AttackWorkBlockDatas = new List<WorkBlockData>();

                workStreamData.AttackWorkBlockDatas.Add(workBlockData);
            }
            else
            {
                if (workStreamData.TargetWorkBlockDatas == null)
                    workStreamData.TargetWorkBlockDatas = new List<WorkBlockData>();

                workStreamData.TargetWorkBlockDatas.Add(workBlockData);
            }
        }
        if (workStreamData.AttackWorkBlockDatas == null && workStreamData.TargetWorkBlockDatas == null)
        {
            Debug.LogError($"{selectWorkId}选择save没有数据，无法SaveToTxt");
            return;
        }

        FileDataOperationWriteEntity fileDataOperationWriteEntity = new FileDataOperationWriteEntity();
        fileDataOperationWriteEntity.DoWriter(path, workStreamData, (BinaryWriter bw) =>
        {
            bw.Write(selectWorkId);
        }, true);
        
        AssetDatabase.Refresh();

        if (File.Exists(path))
        {
            _dataPathDic[selectWorkId] = path;
        }
    }

    /// <summary>
    /// 生成最终数据文件接口
    /// </summary>
    protected virtual void SaveAll()
    {
        SaveConfigDataToFile(_dataPathDic, SaveAllPath);

        AssetDatabase.Refresh();
    }

    protected void SaveConfigDataToFile(Dictionary<uint, string> dataPathDic, string saveToFilePath)
    {
        if (dataPathDic.Count > 0)
        {
            string dir = Path.GetDirectoryName(saveToFilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            FileDataOperationReadEntity reader = new FileDataOperationReadEntity();

            FileDataOperationWriteEntity fileDataOperationWriteEntity = new FileDataOperationWriteEntity();

            long startPos = 0;
            long endPos = 0;

            fileDataOperationWriteEntity.StartWriter(saveToFilePath, (MemoryStream ms, BinaryWriter binaryWriter) =>
            {
                binaryWriter.Write(dataPathDic.Count);
            });

            int workStreamCount = 0;
            foreach (var kv in dataPathDic)
            {
                uint workId = 0u;
                WorkStreamData workStreamData = reader.DoRead(new MemoryStream(File.ReadAllBytes(kv.Value)), (BinaryReader br) =>
                {
                    return WorkStreamData_ConfigTool.Load(br);
                },
                (Stream rs, BinaryReader br) =>
                {
                    workId = br.ReadUInt32();
                });

                if (kv.Key != workId)
                {
                    Debug.LogError($"文件名是：{kv.Key}，但数据的WorkId是：{workId} ，不匹配，生成数据不对，必须核查，路径：{kv.Value}");
                    continue;
                }

                if (workId == 0u || workStreamData == null)
                {
                    Debug.LogError($"数据的WorkId是：{workId} ，内容数据：{workStreamData}，生成数据不对，必须核查，路径：{kv.Value}");
                    continue;
                }

                ++workStreamCount;

                //下次写入数据的位置+workId+WorkStreamData
                fileDataOperationWriteEntity.AddWriterData(workStreamData, (MemoryStream ms, BinaryWriter binaryWriter) =>
                {
                    startPos = ms.Position;
                    binaryWriter.Write(0u);
                    binaryWriter.Write(workId);
                },
                (MemoryStream ms, BinaryWriter binaryWriter) =>
                {
                    endPos = ms.Position;
                    ms.Seek(startPos, SeekOrigin.Begin);
                    binaryWriter.Write((uint)endPos);
                    ms.Seek(endPos, SeekOrigin.Begin);
                });
            }
            
            fileDataOperationWriteEntity.EndWrite((MemoryStream ms, BinaryWriter binaryWriter) =>
            {
                if (workStreamCount != dataPathDic.Count)
                {
                    Debug.Log($"实际写入文件数：{workStreamCount}，应有文件数：{dataPathDic.Count}");

                    ms.Seek(0, SeekOrigin.Begin);
                    binaryWriter.Write(workStreamCount);
                    ms.Seek(0, SeekOrigin.End);
                }
            }, true);
        }
    }
    
    protected void CopyCombatWorkStreamDatas(string destSaveAsPath)
    {
        List<uint> srcList = new List<uint>();
        List<uint> destList = new List<uint>();
        EditorToolHelp.ParseTxtInLine(CopyIdsTxtFile, (string line) =>
        {
            CopyCombatWorkStreamDatas(line, srcList, destList, destSaveAsPath);
        });
    }

    private void CopyCombatWorkStreamDatas(string line, List<uint> srcList, List<uint> destList, string destSaveAsPath)
    {
        if (!string.IsNullOrEmpty(line))
        {
            string[] strs = line.Split(':');
            if (strs != null && strs.Length == 2)
            {
                if (!string.IsNullOrEmpty(strs[0]) && !string.IsNullOrEmpty(strs[1]))
                {
                    int bType = int.Parse(strs[0]);

                    bool isSrcType = bType == 3 || bType == 1;
                    bool isDestType = bType == 4 || bType == 2;

                    if (isSrcType)
                        srcList.Clear();
                    else if (isDestType)
                        destList.Clear();

                    string[] strs1 = strs[1].Split('|');
                    if (strs1 != null && strs1.Length > 0)
                    {
                        foreach (var workStreamIdStr in strs1)
                        {
                            if (string.IsNullOrEmpty(workStreamIdStr))
                                continue;

                            uint workStreamId = uint.Parse(workStreamIdStr);
                            if (workStreamId == 0u)
                                continue;

                            if (isSrcType)
                                srcList.Add(workStreamId);
                            else if (isDestType)
                                destList.Add(workStreamId);
                        }
                    }

                    if (isDestType)
                    {
                        if (bType == 4)
                        {
                            if (destList.Count == 0 || srcList.Count != destList.Count)
                            {
                                Debug.LogError($"Type : {bType.ToString()} -- 这行数据有问题: {line}");
                            }
                            else
                            {
                                for (int i = 0; i < srcList.Count; i++)
                                {
                                    uint srcId = srcList[i];
                                    uint destId = destList[i];

                                    CopySrcTxtToDestTxt(srcId, destId, destSaveAsPath);
                                }
                            }
                        }
                        else if (bType == 2)
                        {
                            if (srcList.Count == 0 || destList.Count == 0)
                            {
                                Debug.LogError($"Type : {bType.ToString()} -- 这行数据有问题: {line}");
                            }
                            else
                            {
                                uint srcId = srcList[0];
                                for (int i = 0; i < destList.Count; i++)
                                {
                                    uint destId = destList[i];

                                    CopySrcTxtToDestTxt(srcId, destId, destSaveAsPath);
                                }
                            }
                        }

                        srcList.Clear();
                        destList.Clear();
                    }
                }
            }
        }
    }

    private void CopySrcTxtToDestTxt(uint srcId, uint destId, string destSaveAsPath)
    {
        if (srcId == 0u || destId == 0u || string.IsNullOrEmpty(destSaveAsPath))
            return;

        if (!_dataPathDic.TryGetValue(srcId, out string srcFilePath))
        {
            Debug.LogError($"不存在SrcId : {srcId.ToString()}");
            return;
        }

        List<WorkBlockData> workBlockDatas = new List<WorkBlockData>();
        ParseTxtToData(SavePath, srcId, workBlockDatas, false);
        SaveToTxt(destSaveAsPath, destId, workBlockDatas);
    }
#endregion

#region 数据处理
    private void ChangeGroupIndex(WorkBlockData workBlockData, WorkNodeData parent, WorkNodeData workNodeData, sbyte newGroupIndex, int parentTransIndex, int parentGroupNodeIndex)
    {
        workNodeData.GroupIndex = newGroupIndex;
        if (parent != null)
        {
            RemoveNode(parent, parentTransIndex, parentGroupNodeIndex);

            AddNode(workBlockData, parent, workNodeData, newGroupIndex);

            if (parent.TransitionWorkGroupList != null)
                parent.TransitionWorkGroupList.Sort(GroupSort);
        }
    }

    private void ChangeConcurrent(WorkNodeData workNodeData, WorkNodeData parent, bool newConcurrent)
    {
        workNodeData.IsConcurrent = newConcurrent;
        ResetWorkLayerMainLine(parent);
    }

    private void RemoveNode(WorkNodeData parent, int parentTransIndex, int parentGroupNodeIndex)
    {
        if (parent == null || parent.TransitionWorkGroupList == null || parent.TransitionWorkGroupList.Count == 0)
            return;

        WorkGroupNodeData workGroupNodeData = parent.TransitionWorkGroupList[parentTransIndex];

        workGroupNodeData.NodeList.RemoveAt(parentGroupNodeIndex);

        if (workGroupNodeData.NodeList.Count == 0)
        {
            if (parent.TransitionWorkGroupList != null)
            {
                parent.TransitionWorkGroupList.RemoveAt(parentTransIndex);
                //if (parent.TransitionWorkGroupList.Count == 0)
                //    parent.TransitionWorkGroupList = null;
            }
        }
        else
        {
            for (int i = 0; i < workGroupNodeData.NodeList.Count; i++)
            {
                workGroupNodeData.NodeList[i].InParentGroupNodeListIndex = (short)i;
            }
        }

        ResetWorkLayerMainLine(parent);
    }

    public WorkNodeData AddChildNode(WorkBlockData workBlockData, WorkNodeData parentWorkNodeData, sbyte groupIndex)
    {
        WorkNodeData createNode = new WorkNodeData();
        createNode.Id = workBlockData.TopWorkNodeData.Id;
        workBlockData.TopWorkNodeData.Id += 1;

        AddNode(workBlockData, parentWorkNodeData, createNode, groupIndex);
		return createNode;
    }

    private void AddBrotherNode(WorkBlockData workBlockData, WorkNodeData parentWorkNodeData, WorkNodeData brotherWorkNodeData, bool isAddNext = true)
    {
        WorkNodeData createNode = new WorkNodeData();
        createNode.Id = workBlockData.TopWorkNodeData.Id;
        workBlockData.TopWorkNodeData.Id += 1;

        AddNode(workBlockData, parentWorkNodeData, createNode, brotherWorkNodeData.GroupIndex, brotherWorkNodeData, isAddNext);
    }

    private WorkNodeData AddBrotherNode(WorkBlockData workBlockData, WorkNodeData parentWorkNodeData, WorkNodeData brotherWorkNodeData, sbyte groupIndex, bool isAddNext = true)
    {
        WorkNodeData createNode = new WorkNodeData();
        createNode.Id = workBlockData.TopWorkNodeData.Id;
        workBlockData.TopWorkNodeData.Id += 1;

        AddNode(workBlockData, parentWorkNodeData, createNode, groupIndex, brotherWorkNodeData, isAddNext);

        return createNode;
    }

    /// <summary>
    /// AddCopyNodeType=0作为兄弟节点插入到parentWorkNodeData节点下面；
    /// =1作为兄弟节点插入到上面；
    /// =2作为子节点插入
    /// </summary>
    private void AddNodeByCopyNodeDatas(WorkBlockData workBlockData, WorkNodeData specifyWorkNodeData, 
        WorkNodeData copyNodeData, int copyType, int AddCopyNodeType = 2)
    {
        WorkNodeData parentNode;
        WorkNodeData brotherNode = null;
        if (AddCopyNodeType == 0 || AddCopyNodeType == 1)
        {
            parentNode = GetParent(workBlockData.TopWorkNodeData, specifyWorkNodeData);
            brotherNode = specifyWorkNodeData;
        }
        else
            parentNode = specifyWorkNodeData;

        WorkNodeData createNode = CreateCopyNodeData(workBlockData, parentNode, copyNodeData);

        if (copyType == 2)
        {
            if (copyNodeData.TransitionWorkGroupList != null && copyNodeData.TransitionWorkGroupList.Count > 0)
            {
                foreach (WorkGroupNodeData childGroupData in copyNodeData.TransitionWorkGroupList)
                {
                    for (int cgnIndex = 0; cgnIndex < childGroupData.NodeList.Count; cgnIndex++)
                    {
                        var cwnd = childGroupData.NodeList[cgnIndex];

                        CopyChildNode(workBlockData, createNode, cwnd, copyType);
                    }
                }
            }
        }

        AddNode(workBlockData, parentNode, createNode, copyNodeData.GroupIndex, brotherNode, AddCopyNodeType == 0);
    }

    private void CopyChildNode(WorkBlockData workBlockData, WorkNodeData parentWorkNodeData, 
        WorkNodeData copyNodeData, int copyType)
    {
        WorkNodeData createNode = CreateCopyNodeData(workBlockData, parentWorkNodeData, copyNodeData);

        AddNode(workBlockData, parentWorkNodeData, createNode, copyNodeData.GroupIndex);

        if (copyType == 2)
        {
            if (copyNodeData.TransitionWorkGroupList != null && copyNodeData.TransitionWorkGroupList.Count > 0)
            {
                foreach (WorkGroupNodeData childGroupData in copyNodeData.TransitionWorkGroupList)
                {
                    for (int cgnIndex = 0; cgnIndex < childGroupData.NodeList.Count; cgnIndex++)
                    {
                        var cwnd = childGroupData.NodeList[cgnIndex];

                        CopyChildNode(workBlockData, createNode, cwnd, copyType);
                    }
                }
            }
        }
    }

    private WorkNodeData CreateCopyNodeData(WorkBlockData workBlockData, WorkNodeData parentWorkNodeData, WorkNodeData copyNodeData)
    {
        WorkNodeData createNode = new WorkNodeData();
        createNode.Id = workBlockData.TopWorkNodeData.Id;
        workBlockData.TopWorkNodeData.Id += 1;

        createNode.NodeType = copyNodeData.NodeType;
        createNode.NodeContent = copyNodeData.NodeContent;
        createNode.SkipWorkBlockType = copyNodeData.SkipWorkBlockType;

        createNode.IsConcurrent = copyNodeData.IsConcurrent;

        createNode.GroupIndex = copyNodeData.GroupIndex;
        createNode.LayerIndex = (sbyte)(parentWorkNodeData.LayerIndex + 1);
        createNode.Parent = parentWorkNodeData;

        return createNode;
    }

    private void AddNode(WorkBlockData workBlockData, WorkNodeData parentWorkNodeData, WorkNodeData createNode, 
        sbyte groupIndex, WorkNodeData brotherNode = null, bool isAddNext = true)
    {
        bool isExist = false;
        createNode.GroupIndex = groupIndex;
        createNode.LayerIndex = (sbyte)(parentWorkNodeData.LayerIndex + 1);
        createNode.Parent = parentWorkNodeData;
        if (parentWorkNodeData.TransitionWorkGroupList != null)
        {
            foreach (WorkGroupNodeData childGroupData in parentWorkNodeData.TransitionWorkGroupList)
            {
                if (childGroupData.GroupIndex == groupIndex)
                {
                    if (childGroupData.NodeList == null)
                        childGroupData.NodeList = new List<WorkNodeData>();
                    if (brotherNode == null)
                    {
                        childGroupData.NodeList.Add(createNode);
                        createNode.InParentGroupNodeListIndex = (short)(childGroupData.NodeList.Count - 1);
                    }
                    else
                    {
                        bool isInsert = false;
                        for (int cgnIndex = 0; cgnIndex < childGroupData.NodeList.Count; cgnIndex++)
                        {
                            if (!isInsert && childGroupData.NodeList[cgnIndex] == brotherNode)
                            {
                                if (cgnIndex != brotherNode.InParentGroupNodeListIndex)
                                {
                                    Debug.LogError($"该node有问题，ID : {brotherNode.Id.ToString()}");
                                }

                                if (isAddNext)
                                {
                                    childGroupData.NodeList.Insert(cgnIndex + 1, createNode);
                                    createNode.InParentGroupNodeListIndex = (short)(cgnIndex + 1);
                                }
                                else
                                {
                                    childGroupData.NodeList.Insert(cgnIndex, createNode);
                                    createNode.InParentGroupNodeListIndex = (short)cgnIndex;
                                    isInsert = true;
                                }
                            }
                            else if(childGroupData.NodeList[cgnIndex].InParentGroupNodeListIndex != cgnIndex)
                            {
                                childGroupData.NodeList[cgnIndex].InParentGroupNodeListIndex = (short)cgnIndex;
                            }
                        }
                    }

                    isExist = true;
                    break;
                }
            }
        }
        else
        {
            parentWorkNodeData.TransitionWorkGroupList = new List<WorkGroupNodeData>();
        }

        if (!isExist)
        {
            WorkGroupNodeData createGroup = new WorkGroupNodeData();
            createGroup.GroupIndex = groupIndex;
            parentWorkNodeData.TransitionWorkGroupList.Add(createGroup);

            if (createGroup.NodeList == null)
                createGroup.NodeList = new List<WorkNodeData>();

            createGroup.NodeList.Add(createNode);
            createNode.InParentGroupNodeListIndex = (short)(createGroup.NodeList.Count - 1);

            parentWorkNodeData.TransitionWorkGroupList.Sort(GroupSort);
        }

        ResetWorkLayerMainLine(parentWorkNodeData);
    }

    private void MoveNodeInGroup(WorkNodeData workNodeData, WorkNodeData parent, int parentTransIndex, int parentGroupNodeIndex, int moveIndex)
    {
        List<WorkNodeData> nodeList = parent.TransitionWorkGroupList[parentTransIndex].NodeList;

        int newNodeIndex = parentGroupNodeIndex + moveIndex;
        if (newNodeIndex < 0 || newNodeIndex >= nodeList.Count)
            return;

        WorkNodeData changeNode = nodeList[newNodeIndex];

        nodeList[newNodeIndex] = workNodeData;
        workNodeData.InParentGroupNodeListIndex = (short)newNodeIndex;

        nodeList[parentGroupNodeIndex] = changeNode;
        changeNode.InParentGroupNodeListIndex = (short)parentGroupNodeIndex;

        ResetWorkLayerMainLine(parent);
    }

    private void ResetWorkLayerMainLine(WorkNodeData parent)
    {
        if (parent == null || parent.TransitionWorkGroupList == null || parent.TransitionWorkGroupList.Count == 0)
            return;

        for (int i = 0; i < parent.TransitionWorkGroupList.Count; i++)
        {
            var group = parent.TransitionWorkGroupList[i];
            if (group == null || group.NodeList == null || group.NodeList.Count == 0)
                continue;

            bool isMainLine = true;
            for (int j = 0; j < group.NodeList.Count; j++)
            {
                var node = group.NodeList[j];
                if (isMainLine && (j == 0 || !node.IsConcurrent))
                {
                    node.IsMainLine = true;
                }
                else
                {
                    node.IsMainLine = false;
                    isMainLine = false;
                }
            }
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------

    //拓扑图的话就是子节点相当于裂变，节点裂变出下一层的子节点数-1就是增加的节点高度。
    private void FissionNode(WorkNodeData workNodeData, ref int count)
    {
        if (workNodeData == null || workNodeData.TransitionWorkGroupList == null)
            return;

        if (workNodeData.TransitionWorkGroupList.Count == 0)
        {
            workNodeData.TransitionWorkGroupList = null;
            return;
        }

        int transIndex = 0;
        while (transIndex < workNodeData.TransitionWorkGroupList.Count)
        {
            WorkGroupNodeData group = workNodeData.TransitionWorkGroupList[transIndex];
            if (group.NodeList == null || group.NodeList.Count == 0)
            {
                workNodeData.TransitionWorkGroupList.RemoveAt(transIndex);
                continue;
            }

            for (int j = 0; j < group.NodeList.Count; j++)
            {
                var child = group.NodeList[j];

                if (transIndex > 0 || j > 0)
                    count++;

                FissionNode(child, ref count);

                if (!_topoNodeFissionWeightDic.TryGetValue(child, out int childWeight))
                {
                    childWeight = 1;
                    _topoNodeFissionWeightDic[child] = childWeight;
                }

                _topoNodeFissionWeightDic.TryGetValue(workNodeData, out int weight);
                weight += childWeight;
                _topoNodeFissionWeightDic[workNodeData] = weight;
            }

            transIndex++;
        }
    }

    private WorkNodeData GetParent(WorkNodeData checkParent, WorkNodeData checkNode)
    {
        if (checkParent == null)
            return null;
        
        if (checkParent.TransitionWorkGroupList == null || checkParent.TransitionWorkGroupList.Count == 0)
            return null;

        for (int i = 0; i < checkParent.TransitionWorkGroupList.Count; i++)
        {
            var group = checkParent.TransitionWorkGroupList[i];
            if (group == null || group.NodeList == null || group.NodeList.Count == 0)
                continue;

            for (int j = 0; j < group.NodeList.Count; j++)
            {
                var node = group.NodeList[j];
                if (node == checkNode)
                    return checkParent;

                var n = GetParent(node, checkNode);
                if (n != null)
                    return n;
            }
        }

        return null;
    }

    private void SetParent(WorkNodeData parent)
    {
        if (parent == null || parent.TransitionWorkGroupList == null || parent.TransitionWorkGroupList.Count == 0)
            return;

        for (int i = 0; i < parent.TransitionWorkGroupList.Count; i++)
        {
            var group = parent.TransitionWorkGroupList[i];
            if (group == null || group.NodeList == null || group.NodeList.Count == 0)
                continue;
            
            for (int j = 0; j < group.NodeList.Count; j++)
            {
                var node = group.NodeList[j];
                node.Parent = parent;
                SetParent(node);
            }
        }
    }

    private int GroupSort(WorkGroupNodeData a, WorkGroupNodeData b)
    {
        if (a.GroupIndex > b.GroupIndex)
            return 1;
        else if (a.GroupIndex < b.GroupIndex)
            return -1;

        return 0;
    }

    private int NodeSort(WorkNodeData a, WorkNodeData b)
    {
        if (a.GroupIndex > b.GroupIndex)
            return 1;
        else if (a.GroupIndex < b.GroupIndex)
            return -1;

        return 0;
    }

    private int GetIndexByEnum(int workEnum, Dictionary<int, int> dic)
    {
        foreach (var kv in dic)
        {
            if (kv.Value == workEnum)
                return kv.Key;
        }

        return 0;
    }
    
    protected void GetCollectNode(WorkBlockData workBlockData, List<uint> collectIdList)
    {
        if (workBlockData == null || workBlockData.TopWorkNodeData == null)
            return;

        GetCollectNode(workBlockData.TopWorkNodeData, collectIdList);
    }

    protected void GetCollectNode(WorkNodeData workNodeData, List<uint> collectIdList)
    {
        if (workNodeData.NodeType == m_CollectNodeType)
        {
            uint[] uis = CombatHelp.GetStrParseUint1Array(workNodeData.NodeContent);
            if (uis != null)
            {
                foreach (var item in uis)
                {
                    if (collectIdList.Contains(item))
                    {
                        Debug.LogError($"重复收集WorkId:{item.ToString()}");
                        continue;
                    }

                    collectIdList.Add(item);
                }
            }
        }

        if (workNodeData.TransitionWorkGroupList == null || workNodeData.TransitionWorkGroupList.Count == 0)
            return;

        for (int i = 0; i < workNodeData.TransitionWorkGroupList.Count; i++)
        {
            WorkGroupNodeData workGroupNodeData = workNodeData.TransitionWorkGroupList[i];
            if (workGroupNodeData == null || workGroupNodeData.NodeList == null || workGroupNodeData.NodeList.Count == 0)
                continue;

            foreach (var item in workGroupNodeData.NodeList)
            {
                GetCollectNode(item, collectIdList);
            }
        }
    }
    
	public bool IsContainWorkMenu(uint workId)
    {
        foreach (var item in m_WorkMenuList)
        {
            if (item.WorkId == workId)
                return true;
        }

        return false;
    }
#endregion

#region 公用方法
    public void SetWorkStreamEnum<T>(int blockStartIndex) where T : Enum
    {
        List<string> blockStrs = new List<string>() { "---请选择Block类型---" };
        WorkBlockEnumDic = new Dictionary<int, int> { { 0, 0 } };

        List<string> nodeStrs = new List<string>() { "---请选择Node类型---" };
        WorkNodeEnumDic = new Dictionary<int, int> { {0, 0 } };
        
        Type enumType = typeof(T);
        int blockIndex = 1;
        int nodeIndex = 1;
        foreach (var item in Enum.GetValues(enumType))
        {
            string enumName = item.ToString();
            int enumInt = (int)item;
            if (enumInt == 0)
                continue;

            FieldInfo fieldInfo = enumType.GetField(enumName);
            if (fieldInfo == null)
                continue;

            object[] attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attrs == null || attrs.Length <= 0)
                continue;

            string enumDes = ((DescriptionAttribute)attrs[0]).Description;
            
            if (enumInt < blockStartIndex)  //Node处理
            {
                nodeStrs.Add($"{enumDes}{(_debugLog ? $"-{nodeIndex.ToString()}:{enumName}:{enumInt.ToString()}" : null)}");
                WorkNodeEnumDic.Add(nodeIndex, enumInt);

                nodeIndex++;
            }
            else   //block处理
            {
                blockStrs.Add($"{enumDes}{(_debugLog ? $"-{blockIndex.ToString()}:{enumName}:{enumInt.ToString()}" : null)}");
                WorkBlockEnumDic.Add(blockIndex, enumInt);

                blockIndex++;
            }
        }

        WorkBlockEnumStrs = blockStrs.ToArray();
        WorkNodeEnumStrs = nodeStrs.ToArray();

        ParseWorkStreamEnumContentNote();
        ParseGMCmdContentNote();
    }
    
    public virtual void RefreshWorkStreamEnum() { }

    public virtual void DoWorkStream() { }

    public WorkNodeData GetNodeConcurrentNode(WorkBlockData workBlockData, WorkNodeData parentNode, WorkNodeData workNodeData)
    {
        if (parentNode.TransitionWorkGroupList == null)
            return null;

        foreach (WorkGroupNodeData workGroupNodeData in parentNode.TransitionWorkGroupList)
        {
            if (workGroupNodeData.GroupIndex == workNodeData.GroupIndex)
            {
                for (int i = workNodeData.InParentGroupNodeListIndex; i > -1; --i)
                {
                    var bNode = workGroupNodeData.NodeList[i];
                    if (bNode.IsConcurrent)
                    {
                        return bNode;
                    }
                }
            }
        }

        return null;
    }

    private int CustomPopup(Rect position, int selectedIndex, string[] displayedOptions, GUIStyle style, Action<int> action)
    {
        if (action == null)
        {
            return EditorGUI.Popup(position, selectedIndex, displayedOptions, style);
        }
        else
        {
            PopupCustomWindow.ShowPopupCustomWindow(position, selectedIndex, displayedOptions, style, action);
            return selectedIndex;
        }
    }

    private int CustomPopup(int selectedIndex, string[] displayedOptions, GUIStyle style, Action<int> action)
    {
        if (action == null)
        {
            return EditorGUILayout.Popup(selectedIndex, displayedOptions, style);
        }
        else
        {
            PopupCustomWindow.ShowPopupCustomWindow(selectedIndex, displayedOptions, style, action);
            return selectedIndex;
        }
    }
#endregion
}