using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CurvesEditorWindow : EditorWindow
{
    public enum EditorCurvesEnum
    {
        Bezier_3D,
    }

    public static CurvesEditorWindow m_Window;

    private BezierCurverController_3D _bc_3D;
    private BezierCurverController_3D _bezierController_3D
    {
        get
        {
            return _bc_3D;
        }
        set
        {
            if (_bc_3D != value)
            {
                _bc_3D = value;

                _selectCurverIndex = -1;
                _selectBezierPIndex = -1;
            }
        }
    }

    private GUIStyle _titleStyle;
    private GUIStyle _sectionStyle;

    private Vector2 _curvesEditorBodyScroll;
    private Vector2 _curvesEditorBezierCurverScroll;
    private EditorCurvesEnum _selectCurvesEnum;
    private bool _isDrawBezierName = true;

    [MenuItem("Tools/Combat/Curves Editor")]
    public static void LauchCurvesEditorWindow()
    {
        m_Window = GetWindow<CurvesEditorWindow>();
        ShowCurvesEditorWindow();
        PlayerPrefs.SetInt("OpenCurvesEditorWindow", 1);
    }

    public static void ShowCurvesEditorWindow()
    {
        m_Window.Clear();
        m_Window.Show();
        m_Window.titleContent = new GUIContent("Curves Editor Window");
        m_Window.InitData();
    }

    private void Clear() { }

    private void Update()
    {
        //开启窗口的重绘，不然窗口信息不会刷新
        Repaint();

        if (!Application.isPlaying)
        {
            if (_bezierController_3D != null)
                _bezierController_3D.Update();
        }
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    public static void Refresh()
    {
        bool isOpen = false;
        if (PlayerPrefs.HasKey("OpenCurvesEditorWindow"))
            isOpen = PlayerPrefs.GetInt("OpenCurvesEditorWindow") == 1 ? true : false;

        if (!isOpen)
            return;

        if (m_Window == null)
        {
            Debug.Log("已打开Window，再打开");
            LauchCurvesEditorWindow();
        }
        else
        {
            Debug.Log("已打开Window，重新刷新");
            ShowCurvesEditorWindow();
        }

        CreateLineMaterial();
    }

    public void InitData()
    {
        GenerateStyles();

        SetBezierFilePaths();

        if (PlayerPrefs.HasKey(_y_maxValPre))
            _y_maxVal = PlayerPrefs.GetFloat(_y_maxValPre);
        
        if (PlayerPrefs.HasKey(_y_minValPre))
            _y_minVal = PlayerPrefs.GetFloat(_y_minValPre);
    }

    private void GenerateStyles()
    {
        _titleStyle = new GUIStyle();
        _titleStyle.border = new RectOffset(3, 3, 3, 3);
        _titleStyle.margin = new RectOffset(2, 2, 2, 2);
        _titleStyle.fontSize = 25;
        _titleStyle.fontStyle = FontStyle.Bold;
        _titleStyle.alignment = TextAnchor.MiddleCenter;

        _sectionStyle = new GUIStyle();
        //_sectionStyle.border = new RectOffset(3, 3, 3, 3);
        //_sectionStyle.margin = new RectOffset(2, 2, 2, 2);
        _sectionStyle.fontSize = 15;
        _sectionStyle.normal.textColor = Color.yellow;
        _sectionStyle.fontStyle = FontStyle.Bold;
        _sectionStyle.alignment = TextAnchor.MiddleLeft;
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui -= OnDuringSceneGUI;
        SceneView.duringSceneGui += OnDuringSceneGUI;
        if (_titleStyle == null)
        {
            GenerateStyles();
        }

        CurvesGlobalEvent.Instance.OnEnableEditorMonoBehaviour();

        Refresh();
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnDuringSceneGUI;

        PlayerPrefs.SetInt("OpenCurvesEditorWindow", 0);

        CurvesGlobalEvent.Instance.OnDisableEditorMonoBehaviour();

        m_Window = null;
    }

    private void OnDuringSceneGUI(SceneView sceneView)
    {
        ProcessInputs();

        sceneView.Repaint();
    }

    private void ProcessInputs()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            if (e.isKey)
            {

            }
        }
    }

    private void OnGUI()
    {
        if (m_Window == null)
            m_Window = this;

        //Header
        GUILayout.BeginVertical();
        GUILayout.Box("Curves Editor Window", _titleStyle, GUILayout.Height(40), GUILayout.ExpandWidth(true));
        var newSelectCurves = (EditorCurvesEnum)EditorGUILayout.EnumPopup(_selectCurvesEnum, GUILayout.Height(30));
        GUILayout.EndVertical();

        //Body
        using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(_curvesEditorBodyScroll))
        {
            if (_selectCurvesEnum != newSelectCurves)
            {
                EndOldCurves(_selectCurvesEnum);
                _selectCurvesEnum = newSelectCurves;
            }
            DrawSelectCurvesGUI(_selectCurvesEnum);

            _curvesEditorBodyScroll = scrollViewScope.scrollPosition;
        }
    }

    private void EndOldCurves(EditorCurvesEnum oldSelectCurves)
    {

    }

    private void DrawSelectCurvesGUI(EditorCurvesEnum selectCurves)
    {
        switch (selectCurves)
        {
            case EditorCurvesEnum.Bezier_3D:
                DrawBezier();
                break;
        }
    }

    public enum PointViewEnum
    {
        None,
        Horizontal,
        Vertical,
        All,
    }

    private string _bezierEditorTrackPrefabPath = $"Assets/Designer_Editor/BezierCurverData/BezierTrackData";
    private string _bezierEditorRatioValDataPath = $"Assets/Designer_Editor/BezierCurverData/BezierRatioValData";

    private PointViewEnum _pointViewEnum = PointViewEnum.All;

    private bool _showRealBezierCurver;

    private float _curverScaleMin = 0.1f;
    private float _curverScaleMax = 3f;
    private float _curverScaleFact = 1f;
    private float _splVal = 0.2f;
    private float _y_minVal = 0f;
    private string _y_minValPre = "CurverEditorWindow_y_minVal";
    private float _y_maxVal = 150f;
    private string _y_maxValPre = "CurverEditorWindow_y_maxVal";
    private bool _isShowHelpRatioPoint;
    private bool _notAllowSelectRatioPoint;
    private float _scaleSpeedRatio;
    private void DrawBezier()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("检测速度", GUILayout.Width(100f)))
        {
            CheckCurversParams();
        }
        GUILayout.Space(15);
        GUILayout.Label($"缩放匀速的比例：", GUILayout.Width(200f));
        _scaleSpeedRatio = EditorGUILayout.FloatField(_scaleSpeedRatio);
        if (GUILayout.Button("执行"))
        {
            UpdateConstantSpeedScale(_scaleSpeedRatio);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (_bezierController_3D == null)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("初始化一个Bezier"))
            {
                GameObject go = new GameObject("BezierController_3D");
                go.transform.position = Vector3.zero;
                go.transform.eulerAngles = Vector3.zero;
                go.transform.localScale = Vector3.one;
                _bezierController_3D = go.AddComponent<BezierCurverController_3D>();
                ResetWindowBezierPointData(_bezierController_3D.BezierTransInfoList, _curverScaleFact);

                Selection.activeObject = go;
            }
            if (GUILayout.Button("SaveAll"))
            {
                SaveAllData();
            }
            GUILayout.EndHorizontal();
        }

        var selectGo = Selection.activeObject as GameObject;
        if (_bezierController_3D != null)
        {
            if (_bezierController_3D.m_SelectGo != selectGo)
            {
                if (selectGo != null)
                {
                    var bc3D = selectGo.GetComponent<BezierCurverController_3D>();
                    if (bc3D != null)
                    {
                        _bezierController_3D = bc3D;
                        ResetWindowBezierPointData(_bezierController_3D.BezierTransInfoList, _curverScaleFact);
                    }
                }

                _bezierController_3D.m_SelectGo = selectGo;
            }
        }
        else
        {
            if (selectGo != null)
            {
                var bc3D = selectGo.GetComponent<BezierCurverController_3D>();
                if (bc3D != null)
                {
                    _bezierController_3D = bc3D;
                    _bezierController_3D.m_SelectGo = selectGo;
                }
            }
        }

        if (_bezierController_3D != null)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(_bezierController_3D.gameObject, typeof(BezierCurverController_3D), true, null);
            if (GUILayout.Button("SaveAll"))
            {
                SaveAllData();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("轨迹路径：", _sectionStyle, GUILayout.Width(80f));
            _bezierController_3D.m_FollowLastBezier = GUILayout.Toggle(_bezierController_3D.m_FollowLastBezier, "添加时自动选最新的Bezier球");
            _isDrawBezierName = GUILayout.Toggle(_isDrawBezierName, "显示名字");
            _bezierController_3D.m_BalanceHelpEnum = (BalanceHelpEnum)EditorGUILayout.EnumPopup(_bezierController_3D.m_BalanceHelpEnum);
            _bezierController_3D.m_IsDrawHelpLine = GUILayout.Toggle(_bezierController_3D.m_IsDrawHelpLine, "画辅助节点连线");
            var isHideBezierSphere = GUILayout.Toggle(_bezierController_3D.m_IsHideBezierSphere, "隐藏Bezier球");
            if (isHideBezierSphere != _bezierController_3D.m_IsHideBezierSphere)
            {
                _bezierController_3D.m_IsHideBezierSphere = isHideBezierSphere;
                _bezierController_3D.HideBezierSphere(isHideBezierSphere);
            }
            _bezierController_3D.m_LoopType = (BezierCurverLoopEnum)EditorGUILayout.EnumPopup(_bezierController_3D.m_LoopType);
            if (_bezierController_3D.m_LoopType != BezierCurverLoopEnum.NoLoop)
            {
                GUILayout.Label("次数:", GUILayout.Width(30f));
                var moveCount = EditorGUILayout.IntField((int)_bezierController_3D.m_MoveCount, GUILayout.Width(50f));
                if (moveCount < 1)
                    moveCount = 1;
                if (moveCount != _bezierController_3D.m_MoveCount)
                {
                    _bezierController_3D.m_MoveCount = (uint)moveCount;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(80f);
            if (GUILayout.Button("添加一个Bezier"))
            {
                _selectCurverIndex = -1;
                _selectBezierPIndex = -1;

                _bezierController_3D.AddBezierFunc();
            }
            if (GUILayout.Button("插入一个Bezier"))
            {
                _selectCurverIndex = -1;
                _selectBezierPIndex = -1;

                _bezierController_3D.InsertBezierFunc();

                RefreshAllBezierCurverInfos();
            }
            if (GUILayout.Button("合并Bezier点"))
            {
                _selectCurverIndex = -1;
                _selectBezierPIndex = -1;

                _bezierController_3D.CombineBezierPoints();

                RefreshAllBezierCurverInfos();
            }
            if (GUILayout.Button("解除合并Bezier点"))
            {
                _selectCurverIndex = -1;
                _selectBezierPIndex = -1;

                _bezierController_3D.DelCombineBezierPoints();

                RefreshAllBezierCurverInfos();
            }
            if (GUILayout.Button("重新开始运动"))
            {
                if (_bezierController_3D != null)
                    _bezierController_3D.IsResetMoveBezier = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);

            //-------
            GUILayout.BeginHorizontal();
            GUILayout.Label("事件：", _sectionStyle, GUILayout.Width(80f));
            bool isShowEventInfo = GUILayout.Toggle(_bezierController_3D.m_IsShowEventInfo, "是否显示事件信息");
            if (_bezierController_3D.m_IsShowEventInfo != isShowEventInfo)
            {
                _bezierController_3D.m_IsShowEventInfo = isShowEventInfo;
                for (int btiIndex = 0, btiCount = _bezierController_3D.BezierTransInfoList.Count; btiIndex < btiCount; btiIndex++)
                {
                    BezierTransInfo_e bezierTransInfo_E = _bezierController_3D.BezierTransInfoList[btiIndex];
                    if (bezierTransInfo_E.m_bezierCuverEvent_e_List.Count == 0)
                        continue;

                    foreach (var eventItem in bezierTransInfo_E.m_bezierCuverEvent_e_List)
                    {
                        if (eventItem.EventTrans == null)
                            continue;

                        eventItem.EventTrans.gameObject.SetActive(_bezierController_3D.m_IsShowEventInfo);
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);

            //-------
            GUILayout.BeginHorizontal();
            GUILayout.Label("界面轨迹：", _sectionStyle, GUILayout.Width(80f));
            _showRealBezierCurver = GUILayout.Toggle(_showRealBezierCurver, string.Empty, GUILayout.Width(30f));
            GUILayout.Label("点限制移动范围类型：", GUILayout.Width(120f));
            _pointViewEnum = (PointViewEnum)EditorGUILayout.EnumPopup(_pointViewEnum, GUILayout.Width(100f));
            GUILayout.Space(5f);
            _isShowHelpRatioPoint = GUILayout.Toggle(_isShowHelpRatioPoint, "显示辅助节点", GUILayout.Width(100f));
            if (!_isShowHelpRatioPoint)
            {
                if (_selectBezierPIndex % 3 != 0)
                {
                    _selectCurverIndex = -1;
                    _selectBezierPIndex = -1;
                }
            }
            GUILayout.Space(5f);
            _notAllowSelectRatioPoint = GUILayout.Toggle(_notAllowSelectRatioPoint, "禁止选取下图节点", GUILayout.Width(120f));
            if (_notAllowSelectRatioPoint)
            {
                _selectCurverIndex = -1;
                _selectBezierPIndex = -1;
            }
            //if (GUILayout.Button("Refresh", GUILayout.Width(100f))) 
            //{
            //    _bezierController_3D.TransformBezierCurverInfoToData();
            //}
            GUILayout.EndHorizontal();

            GUILayout.Space(15f);

            Rect rect = new Rect(10f, 10f, 200f, 120f);
            using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(_curvesEditorBezierCurverScroll,
                GUILayout.Height(rect.height * _curverScaleFact + 120f)))
            {
                GUILayout.BeginVertical();

                #region DrawBezierCurver
                GUILayout.BeginHorizontal();

                float widthSegmentDis = rect.width / 10f;
                float heightSegmentDis = rect.height / 15f;
                for (int btiIndex = 1, btiCount = _bezierController_3D.BezierTransInfoList.Count; btiIndex < btiCount; btiIndex++)
                {
                    var bti = _bezierController_3D.BezierTransInfoList[btiIndex];
                    if (bti.m_BezierRatioInfo_e == null)
                        continue;

                    DrawBezierCurver(rect, widthSegmentDis, heightSegmentDis, _curverScaleFact, btiIndex, _bezierController_3D.BezierTransInfoList);

                    if (btiIndex == 1)
                    {
                        Rect heightSegLabRect = new Rect();
                        float y_valDis = (_y_maxVal - _y_minVal) / 15f;
                        float hsdScale = heightSegmentDis * _curverScaleFact;
                        for (int heightSegIndex = 0; heightSegIndex < 16; heightSegIndex++)
                        {
                            heightSegLabRect.width = 150f;
                            heightSegLabRect.height = hsdScale - 5f;

                            heightSegLabRect.x = rect.x;
                            heightSegLabRect.y = rect.y + heightSegIndex * hsdScale - heightSegLabRect.height * 0.5f;

                            float hslVal = ((int)((_y_maxVal - heightSegIndex * y_valDis) * 100)) / 100f;
                            GUI.Label(heightSegLabRect, $"{hslVal}");
                        }
                    }
                }

                GUILayout.Space(10f + rect.width * _curverScaleFact * (_bezierController_3D.BezierTransInfoList.Count - 1) + 50f);

                rect.width *= _curverScaleFact;
                rect.height *= _curverScaleFact;
                GUILayout.EndHorizontal();
                #endregion

                GUILayout.EndVertical();

                _curvesEditorBezierCurverScroll = scrollViewScope.scrollPosition;
            }

            #region 曲线数据设置
            GUILayout.Space(10f);

            //--------
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Y轴最小数值", GUILayout.Width(80f));
            float yMinVal = EditorGUILayout.FloatField(_y_minVal, GUILayout.Width(40f));
            if (yMinVal != _y_minVal)
            {
                _y_minVal = yMinVal;
                PlayerPrefs.SetFloat(_y_minValPre, _y_minVal);
            }

            GUILayout.Label($"-", GUILayout.Width(10f));

            GUILayout.Label($"Y轴最大数值", GUILayout.Width(80f));
            float yMaxVal = EditorGUILayout.FloatField(_y_maxVal, GUILayout.Width(40f));
            if (yMaxVal != _y_maxVal)
            {
                _y_maxVal = yMaxVal;
                PlayerPrefs.SetFloat(_y_maxValPre, _y_maxVal);
            }

            GUILayout.Box(string.Empty, GUILayout.Width(50f));

            GUILayout.Label($"当前点的值：", GUILayout.Width(80f));
            if (_selectCurverIndex > -1 && _selectBezierPIndex > -1)
            {
                BezierRatioInfo_e selectBezierCurverInfo = _bezierController_3D.BezierTransInfoList[_selectCurverIndex].m_BezierRatioInfo_e;
                BezierRatioPointInfo_e selectBpi = selectBezierCurverInfo.m_BezierRatioPointInfoList_e[_selectBezierPIndex];

                float y_val = EditorGUILayout.FloatField(selectBpi.Y_Val, GUILayout.Width(80f));
                if (y_val != selectBpi.Y_Val)
                {
                    selectBpi.Y_Val = ((int)(y_val * 100)) / 100f;
                    SetBezierPointInfoByY_Val_e(y_val, _selectCurverIndex, selectBezierCurverInfo, selectBpi);
                }
            }
            else
                GUILayout.Label("请先选点", GUI.skin.textArea, GUILayout.Width(80f));
            GUILayout.EndHorizontal();

            //--------
            GUILayout.BeginHorizontal();
            GUILayout.Label($"缩放曲线图：{_curverScaleFact}", GUILayout.Width(100f));
            _curverScaleMin = EditorGUILayout.FloatField(_curverScaleMin, GUILayout.Width(40f));
            if (_curverScaleMin < 0)
                _curverScaleMin = 0f;
            else if (_curverScaleMin > _curverScaleMax)
                _curverScaleMin = _curverScaleMax;

            GUILayout.Label($"-", GUILayout.Width(10f));

            _curverScaleMax = EditorGUILayout.FloatField(_curverScaleMax, GUILayout.Width(40f));
            if (_curverScaleMax < _curverScaleMin)
                _curverScaleMax = _curverScaleMin;

            float curverScaleFact = GUILayout.HorizontalSlider(_curverScaleFact / _curverScaleMax, 0f, 1f) * _curverScaleMax;
            if (curverScaleFact < _curverScaleMin)
                curverScaleFact = _curverScaleMin;
            if (curverScaleFact != _curverScaleFact)
            {
                _curverScaleFact = curverScaleFact;
                ResetWindowBezierPointData(_bezierController_3D.BezierTransInfoList, _curverScaleFact);
            }
            //_bezierPointList.Clear();
            GUILayout.EndHorizontal();

            //--------
            GUILayout.BeginHorizontal();
            GUILayout.Label($"拖动点的灵敏度：{_splVal}", GUILayout.Width(200f));
            _splVal = GUILayout.HorizontalSlider(_splVal, 0.01f, 1f);
            GUILayout.EndHorizontal();
            #endregion

            WindowKeyCodeEvent();
        }
    }

    #region GL画线
    //划线使用的材质球
    private static Material s_lineMaterial;
    /// <summary>
    /// 创建一个材质球
    /// </summary>
    private static void CreateLineMaterial()
    {
        //如果材质球不存在
        if (!s_lineMaterial)
        {
            //用代码的方式实例一个材质球
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            s_lineMaterial = new Material(shader);
            s_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            //设置参数
            s_lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            s_lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //设置参数
            s_lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            //设置参数
            s_lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private Rect _gl_rect;
    private float _gl_widthSegmentDis;
    private float _gl_heightSegmentDis;
    private Color _gl_bgColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    private Color _gl_bgLineColor = new Color(0.28f, 0.28f, 0.28f, 0.8f);
    private Color _gl_curverLineColor = new Color(1f, 1f, 0.4f, 0.8f);
    private Color _gl_needSaveCurverLineColor = new Color(0.7f, 0f, 0.9f, 0.8f);

    private bool _canSelectBezierPoint = true;
    private int _selectCurverIndex = -1;
    private int _selectBezierPIndex = -1;
    public void DrawBezierCurver(Rect originFrameRect, float widthSegmentDis, float heightSegmentDis, float scale,
        int curverIndex, List<BezierTransInfo_e> bezierTransInfoList)
    {
        int bParam = _bezierController_3D.m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams[curverIndex - 1];

        Vector2 mousePos = Event.current.mousePosition;

        BezierTransInfo_e bezierTransInfo = bezierTransInfoList[curverIndex];
        BezierRatioInfo_e bezierCurverInfo = bezierTransInfo.m_BezierRatioInfo_e;

        bezierCurverInfo.StartX = originFrameRect.x;
        bezierCurverInfo.StartY = originFrameRect.y;

        _gl_rect = originFrameRect;

        originFrameRect.x = GetOriginStartX(curverIndex, originFrameRect.x, originFrameRect.width);

        bezierCurverInfo.x = originFrameRect.x;
        bezierCurverInfo.y = originFrameRect.y;
        bezierCurverInfo.width = originFrameRect.width;
        bezierCurverInfo.height = originFrameRect.height;
        bezierCurverInfo.scale = scale;

        if (bezierCurverInfo.Y_MaxVal != _y_maxVal || bezierCurverInfo.Y_MinVal != _y_minVal)
        {
            bezierCurverInfo.Y_MaxVal = _y_maxVal;
            bezierCurverInfo.Y_MinVal = _y_minVal;

            TransformBezierCurverInfoToData();
        }

        _gl_rect.x = GetRealStartX(curverIndex, _gl_rect.x, originFrameRect.width, scale);
        _gl_rect.width *= scale;
        _gl_rect.height *= scale;
        _gl_widthSegmentDis = widthSegmentDis * scale;
        _gl_heightSegmentDis = heightSegmentDis * scale;

        Rect drawBgRect = _gl_rect;
        if (curverIndex == 1)
        {
            drawBgRect.x -= 40f;
            drawBgRect.y -= 1f;
            drawBgRect.width += 40f;
            drawBgRect.height += 2f;
        }
        EditorGUI.DrawRect(drawBgRect, _gl_bgColor);

        uint bezierParam = (uint)bParam;
        bool isExistParam = _ratioValPathDic.ContainsKey(bezierParam);

        float halfLen = 2.5f;

        #region 画背景线
        OnRenderObject();
        #endregion

        #region 设置点的数据
        if (bezierCurverInfo.m_BezierRatioPointInfoList_e.Count == 0)
        {
            BezierRatioPointInfo_e startBpi = new BezierRatioPointInfo_e();
            startBpi.OriginX = bezierCurverInfo.x - halfLen;
            startBpi.OriginY = bezierCurverInfo.y + bezierCurverInfo.height - halfLen;
            startBpi.ShowX = _gl_rect.x - halfLen;
            startBpi.ShowY = bezierCurverInfo.y + bezierCurverInfo.height * scale - halfLen;
            startBpi.Width = halfLen * 2;
            startBpi.Height = halfLen * 2;
            bezierCurverInfo.m_BezierRatioPointInfoList_e.Add(startBpi);

            BezierRatioPointInfo_e rightBpi = new BezierRatioPointInfo_e();
            rightBpi.OriginX = bezierCurverInfo.x + bezierCurverInfo.width * 0.5f - halfLen;
            rightBpi.OriginY = bezierCurverInfo.y + bezierCurverInfo.height - halfLen;
            rightBpi.ShowX = _gl_rect.x + bezierCurverInfo.width * 0.5f * scale - halfLen;
            rightBpi.ShowY = bezierCurverInfo.y + bezierCurverInfo.height * scale - halfLen;
            rightBpi.Width = halfLen * 2;
            rightBpi.Height = halfLen * 2;
            bezierCurverInfo.m_BezierRatioPointInfoList_e.Add(rightBpi);

            BezierRatioPointInfo_e leftBpi = new BezierRatioPointInfo_e();
            leftBpi.OriginX = bezierCurverInfo.x + bezierCurverInfo.width * 0.5f - halfLen;
            leftBpi.OriginY = bezierCurverInfo.y - halfLen;
            leftBpi.ShowX = _gl_rect.x + bezierCurverInfo.width * 0.5f * scale - halfLen;
            leftBpi.ShowY = bezierCurverInfo.y - halfLen;
            leftBpi.Width = halfLen * 2;
            leftBpi.Height = halfLen * 2;
            bezierCurverInfo.m_BezierRatioPointInfoList_e.Add(leftBpi);

            BezierRatioPointInfo_e endBpi = new BezierRatioPointInfo_e();
            endBpi.OriginX = bezierCurverInfo.x + bezierCurverInfo.width - halfLen;
            endBpi.OriginY = bezierCurverInfo.y - halfLen;
            endBpi.ShowX = _gl_rect.x + bezierCurverInfo.width * scale - halfLen;
            endBpi.ShowY = bezierCurverInfo.y - halfLen;
            endBpi.Width = halfLen * 2;
            endBpi.Height = halfLen * 2;
            bezierCurverInfo.m_BezierRatioPointInfoList_e.Add(endBpi);

            TransformBezierCurverInfoToData();
        }
        #endregion

        #region 画贝塞尔曲线功能 / 添加新的点 / 事件处理
        for (int i = 0, count = bezierCurverInfo.m_BezierRatioPointInfoList_e.Count; i < count - 1; i += 3)
        {
            var startBpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[i];
            var rightBpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[i + 1];
            var leftBpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[i + 2];
            var endBpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[i + 3];

            EditorGUI.DrawRect(new Rect(startBpi.ShowX, startBpi.ShowY, startBpi.Width, startBpi.Height), _gl_curverLineColor);

            EditorGUI.DrawRect(new Rect(endBpi.ShowX, endBpi.ShowY, endBpi.Width, endBpi.Height), _gl_curverLineColor);

            if (_isShowHelpRatioPoint && !_notAllowSelectRatioPoint)
            {
                Handles.DrawLine(new Vector3(startBpi.ShowX + halfLen, startBpi.ShowY + halfLen, 0),
                new Vector3(rightBpi.ShowX + halfLen, rightBpi.ShowY + halfLen, 0));

                Handles.DrawLine(new Vector3(endBpi.ShowX + halfLen, endBpi.ShowY + halfLen, 0),
                    new Vector3(leftBpi.ShowX + halfLen, leftBpi.ShowY + halfLen, 0));

                EditorGUI.DrawRect(new Rect(rightBpi.ShowX, rightBpi.ShowY, rightBpi.Width, rightBpi.Height), _gl_curverLineColor);
                EditorGUI.DrawRect(new Rect(leftBpi.ShowX, leftBpi.ShowY, leftBpi.Width, leftBpi.Height), _gl_curverLineColor);
            }

            float startRatioInTotal = GetRatioInTotal(curverIndex, bezierCurverInfo, startBpi);
            float endRatioInTotal = GetRatioInTotal(curverIndex, bezierCurverInfo, endBpi);

            Vector2 movePos = Vector2.zero;
            bool isShowMovePos = false;
            if ((_bezierController_3D.m_BezierCurvesFuncComponent.m_IsFront && _bezierController_3D.m_BezierCurvesFuncComponent.m_CurBezierIndex + 1 == curverIndex) ||
                (!_bezierController_3D.m_BezierCurvesFuncComponent.m_IsFront && _bezierController_3D.m_BezierCurvesFuncComponent.m_CurBezierIndex == curverIndex))
            {
                if (_bezierController_3D.m_BezierCurvesFuncComponent.BezierCurverRatio >= startRatioInTotal &&
                    _bezierController_3D.m_BezierCurvesFuncComponent.BezierCurverRatio < endRatioInTotal)
                {
                    float bRatioInSegm = (_bezierController_3D.m_BezierCurvesFuncComponent.BezierCurverRatio - startRatioInTotal) /
                        (endRatioInTotal - startRatioInTotal);

                    movePos = EditorToolHelp.GetBezierVal(bRatioInSegm,
                                     startBpi.ShowX + halfLen, startBpi.ShowY + halfLen,
                                     rightBpi.ShowX + halfLen, rightBpi.ShowY + halfLen,
                                     leftBpi.ShowX + halfLen, leftBpi.ShowY + halfLen,
                                     endBpi.ShowX + halfLen, endBpi.ShowY + halfLen);

                    isShowMovePos = true;
                }

            }
            if (_showRealBezierCurver)
            {
                EditorToolHelp.DrawCurve(startBpi.ShowX + halfLen, startBpi.ShowY + halfLen,
                                         rightBpi.ShowX + halfLen, rightBpi.ShowY + halfLen,
                                         leftBpi.ShowX + halfLen, leftBpi.ShowY + halfLen,
                                         endBpi.ShowX + halfLen, endBpi.ShowY + halfLen,
                                         Color.green, 2f);

                if (isShowMovePos)
                {
                    EditorGUI.DrawRect(new Rect(movePos.x - 5f, movePos.y - 5f, 10f, 10f), Color.magenta);
                }
            }

            if (isShowMovePos)
            {
                EditorGUI.DrawRect(new Rect(GetRealStartX(curverIndex, bezierCurverInfo) +
                    _bezierController_3D.m_BezierCurvesFuncComponent.BezierCurverRatio * bezierCurverInfo.width * bezierCurverInfo.scale - 5f, movePos.y - 5f, 10f, 10f), Color.magenta);
            }

            StartGLRenderLineSrip();
            int seg = 100;
            for (int sbIndex = 0; sbIndex < seg + 1; sbIndex++)
            {
                float sbFactor = (float)sbIndex / seg;
                Vector2 sbPos = EditorToolHelp.GetBezierVal(sbFactor, startBpi.ShowX + halfLen, startBpi.ShowY + halfLen,
                                     rightBpi.ShowX + halfLen, rightBpi.ShowY + halfLen,
                                     leftBpi.ShowX + halfLen, leftBpi.ShowY + halfLen,
                                     endBpi.ShowX + halfLen, endBpi.ShowY + halfLen);


                GL.Color(isExistParam ? _gl_curverLineColor : _gl_needSaveCurverLineColor);
                GL.Vertex3(GetRatio_CoordX(sbFactor, curverIndex, bezierCurverInfo, startBpi, endBpi), sbPos.y, 0);
            }
            EndGLRenderLineSrip();

            #region 添加新的点
            if (Event.current.alt)
            {
                if (mousePos.x >= startBpi.ShowX + halfLen && mousePos.x <= endBpi.ShowX + halfLen &&
                    mousePos.y >= _gl_rect.y && mousePos.y <= _gl_rect.y + _gl_rect.height)
                {
                    float timeRatioInSegm = GetRatioInSegmentByPointX(mousePos.x, curverIndex, bezierCurverInfo, startBpi, endBpi);

                    Vector2 mouseBezierPoint = EditorToolHelp.GetBezierVal(timeRatioInSegm,
                                                                         startBpi.ShowX + halfLen, startBpi.ShowY + halfLen,
                                                                         rightBpi.ShowX + halfLen, rightBpi.ShowY + halfLen,
                                                                         leftBpi.ShowX + halfLen, leftBpi.ShowY + halfLen,
                                                                         endBpi.ShowX + halfLen, endBpi.ShowY + halfLen);

                    //EditorGUI.DrawRect(new Rect(mousePos.x - 5f,
                    //                mouseBezierPoint.y - 5f, 10f, 10f), Color.red);

                    float ratioX = GetRatio_CoordX(timeRatioInSegm, curverIndex, bezierCurverInfo, startBpi, endBpi);

                    EditorGUI.DrawRect(new Rect(ratioX - 2.5f,
                                    mouseBezierPoint.y - 2.5f, 5f, 5f), Color.blue);

                    if (Event.current.isMouse && Event.current.type == EventType.MouseDown)
                    {
                        if (Event.current.button == 0)
                        {
                            AddBezierGroupPoint_e(ratioX, mouseBezierPoint.y, startBpi.Width, startBpi.Height, curverIndex, i + 2, bezierCurverInfo);

                            return;
                        }
                    }
                }
            }
            #endregion

            #region 检测Event功能节点
            if (_bezierController_3D.m_IsShowEventInfo)
            {
                for (int eventIndex = 0, eventCount = bezierTransInfo.m_bezierCuverEvent_e_List.Count; eventIndex < eventCount; eventIndex++)
                {
                    BezierCurverEvent_e eventInfo = bezierTransInfo.m_bezierCuverEvent_e_List[eventIndex];
                    if ((eventInfo.EventInfo.EventRatio >= startRatioInTotal && eventInfo.EventInfo.EventRatio < endRatioInTotal) ||
                        eventInfo.EventInfo.EventRatio == 1f && endRatioInTotal == 1f)
                    {
                        float timeRatioInSegm = GetRatioInSegmentByRatioInTotal(eventInfo.EventInfo.EventRatio, curverIndex, bezierCurverInfo, startBpi, endBpi);

                        Vector2 eventPoint = EditorToolHelp.GetBezierVal(timeRatioInSegm,
                                                                             startBpi.ShowX + halfLen, startBpi.ShowY + halfLen,
                                                                             rightBpi.ShowX + halfLen, rightBpi.ShowY + halfLen,
                                                                             leftBpi.ShowX + halfLen, leftBpi.ShowY + halfLen,
                                                                             endBpi.ShowX + halfLen, endBpi.ShowY + halfLen);

                        float ratioX = GetRatio_CoordX(timeRatioInSegm, curverIndex, bezierCurverInfo, startBpi, endBpi);

                        if (mousePos.x >= ratioX - 5f && mousePos.x <= ratioX + 5f &&
                            mousePos.y >= eventPoint.y - 5f && mousePos.y <= eventPoint.y + 5f)
                        {
                            bezierTransInfo.m_SelectBezierEventIndex = eventIndex;
                            Selection.activeGameObject = eventInfo.EventTrans.gameObject;

                            if (Event.current.isKey && Event.current.keyCode == KeyCode.Delete)
                            {
                                bezierTransInfo.m_SelectBezierEventIndex = -1;
                                if (eventInfo.EventTrans != null)
                                    DestroyImmediate(eventInfo.EventTrans.gameObject);
                                break;
                            }
                        }

                        if (bezierTransInfo.m_SelectBezierEventIndex == eventIndex)
                        {
                            EditorGUI.DrawRect(new Rect(ratioX - 6.5f,
                                        eventPoint.y - 6.5f, 13f, 13f), Color.red);
                        }

                        EditorGUI.DrawRect(new Rect(ratioX - 5f,
                                        eventPoint.y - 5f, 10f, 10f), Color.green);

                        EditorGUI.LabelField(new Rect(ratioX - 15f,
                                        eventPoint.y - 17f, 40f, 10f), $"{eventInfo.EventInfo.EventId.ToString()}");
                    }
                }
            }
            #endregion
        }
        #endregion

        #region 选取点功能
        if (!_notAllowSelectRatioPoint)
        {
            if (mousePos.x >= _gl_rect.x && mousePos.x <= _gl_rect.x + _gl_rect.width &&
                mousePos.y >= _gl_rect.y && mousePos.y <= _gl_rect.y + _gl_rect.height)
            {
                if (_canSelectBezierPoint)
                {
                    for (int i = 0, count = bezierCurverInfo.m_BezierRatioPointInfoList_e.Count; i < count;)
                    {
                        var checkBpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[i];
                        Rect bpCheckRect = new Rect(checkBpi.ShowX - 2.5f, checkBpi.ShowY - 2.5f, 10f, 10f);
                        if (mousePos.x >= bpCheckRect.x && mousePos.x <= bpCheckRect.x + bpCheckRect.width &&
                            mousePos.y >= bpCheckRect.y && mousePos.y <= bpCheckRect.y + bpCheckRect.height)
                        {
                            _selectBezierPIndex = i;
                            _selectCurverIndex = curverIndex;

                            break;
                        }

                        if (_isShowHelpRatioPoint)
                            ++i;
                        else
                            i += 3;
                    }
                }
            }
        }
        #endregion

        #region 拖动点功能
        if (_selectCurverIndex == curverIndex && _selectBezierPIndex > -1)
        {
            var selectBpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[_selectBezierPIndex];
            Rect bpSelectRect = new Rect(selectBpi.ShowX - 2.5f, selectBpi.ShowY - 2.5f, 10f, 10f);
            EditorGUI.DrawRect(bpSelectRect, Color.red);

            GUI.Label(new Rect(bpSelectRect.x - 40, bpSelectRect.y, 100f, 30f),
                $"{SetY_Val(bezierCurverInfo, selectBpi)}");

            if (Event.current.type == EventType.MouseDrag)
            {
                _canSelectBezierPoint = false;

                Vector2 deltaV2 = Event.current.delta * scale * _splVal;

                if (_selectBezierPIndex > 0 && _selectBezierPIndex < bezierCurverInfo.m_BezierRatioPointInfoList_e.Count - 1)
                    selectBpi.OriginX += deltaV2.x;
                selectBpi.OriginY += deltaV2.y;

                if (_selectBezierPIndex > 0 && _selectBezierPIndex < bezierCurverInfo.m_BezierRatioPointInfoList_e.Count - 1)
                    bpSelectRect.x += deltaV2.x;
                bpSelectRect.y += deltaV2.y;

                if (_selectBezierPIndex > 0 && _selectBezierPIndex < (bezierCurverInfo.m_BezierRatioPointInfoList_e.Count - 1))
                {
                    if ((_selectBezierPIndex % 3) == 0)
                    {
                        var preSelectBpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[_selectBezierPIndex - 3];
                        var nextSelectBpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[_selectBezierPIndex + 3];
                        float pnDisX = bezierCurverInfo.scale;
                        if (selectBpi.OriginX <= (preSelectBpi.OriginX + pnDisX))
                        {
                            selectBpi.OriginX = preSelectBpi.OriginX + pnDisX;
                        }
                        else if (selectBpi.OriginX >= (nextSelectBpi.OriginX - pnDisX))
                        {
                            selectBpi.OriginX = nextSelectBpi.OriginX - pnDisX;
                        }
                    }
                    else if (_pointViewEnum == PointViewEnum.All || _pointViewEnum == PointViewEnum.Horizontal)
                    {
                        int pVal = Mathf.FloorToInt(_selectBezierPIndex / 3f) * 3;
                        var preSelectBpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[pVal];
                        var nextSelectBpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[pVal + 3];
                        if (selectBpi.OriginX < preSelectBpi.OriginX)
                        {
                            selectBpi.OriginX = preSelectBpi.OriginX;
                        }
                        else if (selectBpi.OriginX > nextSelectBpi.OriginX)
                        {
                            selectBpi.OriginX = nextSelectBpi.OriginX;
                        }
                    }
                }
                else if (_pointViewEnum == PointViewEnum.All || _pointViewEnum == PointViewEnum.Horizontal)
                {
                    float centerx = bpSelectRect.x + bpSelectRect.width * 0.5f;

                    if (centerx < _gl_rect.x)
                    {
                        selectBpi.OriginX = bezierCurverInfo.x - selectBpi.Width * 0.5f;
                    }
                    else if (centerx > _gl_rect.x + _gl_rect.width)
                    {
                        selectBpi.OriginX = bezierCurverInfo.x + bezierCurverInfo.width - selectBpi.Width * 0.5f;
                    }
                }

                float centery = bpSelectRect.y + bpSelectRect.height * 0.5f;
                if (_pointViewEnum == PointViewEnum.All || _pointViewEnum == PointViewEnum.Vertical)
                {
                    if (centery < _gl_rect.y)
                    {
                        selectBpi.OriginY = bezierCurverInfo.y - selectBpi.Height * 0.5f;
                    }
                    else if (centery > _gl_rect.y + _gl_rect.height)
                    {
                        selectBpi.OriginY = bezierCurverInfo.y + bezierCurverInfo.height - selectBpi.Height * 0.5f;
                    }
                }

                RefreshBezierPoint_Show(curverIndex, bezierCurverInfo, selectBpi);

                if (Mathf.Abs(deltaV2.y) < 0.31f)
                {
                    AttractEditorCurverPoint(originFrameRect, scale, bezierCurverInfo, selectBpi, _selectCurverIndex, _selectBezierPIndex, bezierTransInfoList);
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                AttractEditorCurverPoint(originFrameRect, scale, bezierCurverInfo, selectBpi, _selectCurverIndex, _selectBezierPIndex, bezierTransInfoList);

                _canSelectBezierPoint = true;
                _selectCurverIndex = -1;
                _selectBezierPIndex = -1;
            }
        }
        else if (_selectCurverIndex < 0 || _selectBezierPIndex < 0)
        {
            _canSelectBezierPoint = true;
        }
        #endregion

        Rect infoStartRect = new Rect(GetRealStartX(curverIndex, bezierCurverInfo) + 20f, bezierCurverInfo.y + bezierCurverInfo.height * _curverScaleFact + 10f,
                        bezierCurverInfo.width * _curverScaleFact - 40f, 20f);
        float endX = infoStartRect.x + infoStartRect.width;

        Rect infoRect = infoStartRect;
        EditorGUI.LabelField(new Rect(infoRect.x, infoRect.y, 10f * _curverScaleFact, 20f), "Id：");

        infoRect.x += 10f * _curverScaleFact + 5f;
        uint parmInt = (uint)EditorGUI.IntField(new Rect(infoRect.x, infoRect.y, 30f * _curverScaleFact, 20f), (int)bezierParam);
        if (parmInt < 0u)
            parmInt = 0u;
        if (parmInt != bezierParam)
        {
            _bezierController_3D.m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams[curverIndex - 1] = (int)parmInt;
        }

        infoRect.x += 30f * _curverScaleFact + 5f;
        if (isExistParam)
        {
            if (GUI.Button(new Rect(infoRect.x, infoRect.y, 20f * _curverScaleFact, 20f), "读取"))
            {
                Bezier1CurvesData bezier1CurvesData = ParseBezierRatioData(parmInt);
                if (bezier1CurvesData == null)
                {
                    EditorUtility.DisplayDialog("提示", $"不存在文件数据Id：{parmInt.ToString()}读取", "OK");
                }
                else if (EditorUtility.DisplayDialog("提示", $"读取数据Id：{parmInt.ToString()}", "读取", "Cancel"))
                {
                    TransformBezierRatio2Editor(curverIndex, bezierCurverInfo, bezier1CurvesData);

                    _bezierController_3D.m_BezierCurvesFuncComponent.m_BezierRatioDataList[curverIndex - 1] = bezier1CurvesData;
                }
            }

            infoRect.x += 20f * _curverScaleFact + 5f;
        }

        if (bParam >= 0 && GUI.Button(new Rect(infoRect.x, infoRect.y, 20f * _curverScaleFact, 20f), $"{(isExistParam ? "覆盖" : "保存")}"))
        {
            if (parmInt == 0u)
            {
                EditorUtility.DisplayDialog("提示", $"先填写Id再保存", "Ok");
            }
            else
            {
                if (EditorUtility.DisplayDialog("提示",
                    $"{(isExistParam ? $"是否覆盖Id：{parmInt.ToString()}" : $"是否保存Id：{parmInt.ToString()}")}",
                    $"{(isExistParam ? "覆盖" : "保存")}", "Cancel"))
                {
                    SaveBezierRatioData(parmInt, _bezierController_3D.m_BezierCurvesFuncComponent.m_BezierRatioDataList[curverIndex - 1]);
                }
            }
        }

        infoRect.x += 20f * _curverScaleFact + 5f;
        EditorGUI.ObjectField(new Rect(infoRect.x, infoRect.y, endX - infoRect.x, 20f), bezierTransInfo.m_Trans, typeof(Transform), true);

        if (bParam < 0)
        {
            Rect constantSpeedRect = infoStartRect;
            constantSpeedRect.y += 25f;

            EditorGUI.LabelField(new Rect(constantSpeedRect.x, constantSpeedRect.y, 50f * _curverScaleFact, 20f), "使用常量值：");
            constantSpeedRect.x += 50f * _curverScaleFact + 5f;
            var pval = EditorGUI.IntField(new Rect(constantSpeedRect.x, constantSpeedRect.y, endX - constantSpeedRect.x, 20f), Mathf.Abs(bParam));
            if (pval != Mathf.Abs(bParam))
            {
                _bezierController_3D.m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams[curverIndex - 1] = -pval;
            }
        }

        if (_bezierController_3D.m_IsShowEventInfo)
        {
            Rect eventRect = infoStartRect;
            eventRect.y += 50f;

            if (GUI.Button(new Rect(eventRect.x, eventRect.y, 40f * _curverScaleFact, 20f), "AddEvent"))
            {
                _bezierController_3D.CreateEventInfo(curverIndex);
            }

            float eventX = eventRect.x;
            eventX += 40f * _curverScaleFact + 5f;
            GUI.Label(new Rect(eventX, eventRect.y, 10f * _curverScaleFact, 20f), $"{bezierTransInfo.m_bezierCuverEvent_e_List.Count}");

            if (bezierTransInfo.m_bezierCuverEvent_e_List.Count > 0)
            {
                if (bezierTransInfo.m_SelectBezierEventIndex >= bezierTransInfo.m_bezierCuverEvent_e_List.Count)
                    bezierTransInfo.m_SelectBezierEventIndex = bezierTransInfo.m_bezierCuverEvent_e_List.Count - 1;

                if (bezierTransInfo.m_SelectBezierEventIndex > -1)
                {
                    eventX += 10f * _curverScaleFact + 5f;
                    var eventInfo_e = bezierTransInfo.m_bezierCuverEvent_e_List[bezierTransInfo.m_SelectBezierEventIndex];
                    eventInfo_e.EventInfo.EventRatio = EditorGUI.Slider(new Rect(eventX, eventRect.y, endX - eventX, 20f),
                                                eventInfo_e.EventInfo.EventRatio, 0f, 1f);

                    eventRect.y += 25f;
                    eventX = eventRect.x;
                    GUI.Label(new Rect(eventX, eventRect.y, 20f * _curverScaleFact, 20f), "次数：");
                    eventX += 20f * _curverScaleFact + 5;
                    int eventCount = EditorGUI.IntField(new Rect(eventX, eventRect.y, (endX - eventX) * 0.5f - 5f, 20f), eventInfo_e.EventInfo.EventCount);
                    if (eventCount < 0)
                        eventCount = 0;
                    eventInfo_e.EventInfo.EventCount = eventCount;

                    eventX += (endX - eventX) * 0.5f;
                    _bezierController_3D.m_BezierCurvesFuncComponent.m_EventTimesDic.TryGetValue(eventInfo_e.EventInfo.EventId, out int curTimes);
                    EditorGUI.IntField(new Rect(eventX, eventRect.y, endX - eventX, 20f), curTimes);
                }
            }
        }
    }

    #region 显示bezier曲线模式类型
    //private
    #endregion

    private void ResetWindowBezierPointData(List<BezierTransInfo_e> bezierTransInfoList, float scale)
    {
        for (int btiIndex = 1, btiCount = bezierTransInfoList.Count; btiIndex < btiCount; btiIndex++)
        {
            var bezierCurverInfo = bezierTransInfoList[btiIndex].m_BezierRatioInfo_e;
            bezierCurverInfo.scale = scale;

            for (int i = 0, count = bezierCurverInfo.m_BezierRatioPointInfoList_e.Count; i < count; i++)
            {
                var bpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[i];

                RefreshBezierPoint_Show(btiIndex, bezierCurverInfo, bpi);
            }
        }
    }

    //让Start/end点能够在一定范围自动吸附在一起
    private void AttractEditorCurverPoint(Rect originFrameRect, float scale, BezierRatioInfo_e curBezierCurverInfo,
        BezierRatioPointInfo_e selectBpi, int selectCurverIndex, int selectBezierPIndex,
        List<BezierTransInfo_e> bezierTransInfoList)
    {
        if (selectBezierPIndex == 0)
        {
            if (selectCurverIndex > 1)
            {
                BezierRatioInfo_e preBci = bezierTransInfoList[_selectCurverIndex - 1].m_BezierRatioInfo_e;
                if (preBci.m_BezierRatioPointInfoList_e.Count > 0)
                {
                    var preBpi = preBci.m_BezierRatioPointInfoList_e[preBci.m_BezierRatioPointInfoList_e.Count - 1];
                    float preSDis = Mathf.Abs(preBpi.ShowY - selectBpi.ShowY);
                    if (preSDis < 10f)
                    {
                        selectBpi.OriginY = preBpi.OriginY;

                        float originDisY = selectBpi.OriginY + selectBpi.Height * 0.5f - originFrameRect.y;
                        selectBpi.ShowY = originFrameRect.y + originDisY * scale - selectBpi.Height * 0.5f;
                    }
                }
            }
        }
        else if (selectBezierPIndex + 1 >= curBezierCurverInfo.m_BezierRatioPointInfoList_e.Count)
        {
            if (selectCurverIndex + 1 < bezierTransInfoList.Count)
            {
                BezierRatioInfo_e nextBci = bezierTransInfoList[_selectCurverIndex + 1].m_BezierRatioInfo_e;
                if (nextBci.m_BezierRatioPointInfoList_e.Count > 0)
                {
                    var nextBpi = nextBci.m_BezierRatioPointInfoList_e[0];
                    float nextSDis = Mathf.Abs(nextBpi.ShowY - selectBpi.ShowY);
                    if (nextSDis < 10f)
                    {
                        selectBpi.OriginY = nextBpi.OriginY;

                        float originDisY = selectBpi.OriginY + selectBpi.Height * 0.5f - originFrameRect.y;
                        selectBpi.ShowY = originFrameRect.y + originDisY * scale - selectBpi.Height * 0.5f;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 使用GL画线的回调
    /// </summary>
    public void OnRenderObject()
    {
        //创建材质球
        CreateLineMaterial();
        //激活第一个着色器通过（在本例中，我们知道它是唯一的通过）
        s_lineMaterial.SetPass(0);
        //渲染入栈  在Push——Pop之间写GL代码
        GL.PushMatrix();
        GL.LoadPixelMatrix();

        // 开始画线  在Begin——End之间写画线方式
        //GL.LINES 画线
        int col = Mathf.FloorToInt(_gl_rect.width / _gl_widthSegmentDis) + 1;
        int row = Mathf.FloorToInt(_gl_rect.height / _gl_heightSegmentDis) + 1;
        for (int i = 0; i < row; i++)
        {
            float rp = i * _gl_heightSegmentDis + _gl_rect.y;

            GL.Begin(GL.LINES);
            GL.Color(_gl_bgLineColor);
            GL.Vertex3(_gl_rect.x, rp, 0);
            GL.Vertex3(_gl_rect.x + _gl_rect.width, rp, 0);
            GL.End();
        }
        for (int i = 0; i < col; i++)
        {
            float cp = i * _gl_widthSegmentDis + _gl_rect.x;

            GL.Begin(GL.LINES);
            GL.Color(i == 0 ? Color.black : _gl_bgLineColor);
            GL.Vertex3(cp, _gl_rect.y, 0);
            GL.Vertex3(cp, _gl_rect.y + _gl_rect.height, 0);
            GL.End();
        }
        //渲染出栈
        GL.PopMatrix();
    }

    public void StartGLRenderLineSrip()
    {
        //创建材质球
        CreateLineMaterial();
        //激活第一个着色器通过（在本例中，我们知道它是唯一的通过）
        s_lineMaterial.SetPass(0);
        //渲染入栈  在Push——Pop之间写GL代码
        GL.PushMatrix();
        GL.LoadPixelMatrix();

        GL.Begin(GL.LINE_STRIP);
    }
    public void EndGLRenderLineSrip()
    {
        GL.End();
        //渲染出栈
        GL.PopMatrix();
    }
    #endregion

    #region 编辑器中曲线编辑接口
    private float SetY_Val(BezierRatioInfo_e bezierCurverInfo, BezierRatioPointInfo_e bezierPointInfo)
    {
        float y_valDis = bezierCurverInfo.Y_MaxVal - bezierCurverInfo.Y_MinVal;
        bezierPointInfo.Y_Val = bezierCurverInfo.Y_MaxVal -
            y_valDis * (bezierPointInfo.OriginY + bezierPointInfo.Height * 0.5f - bezierCurverInfo.y) / bezierCurverInfo.height;

        return bezierPointInfo.Y_Val;
    }

    public void SetBezierPointInfoByY_Val_e(float y_val, int curverIndex, BezierRatioInfo_e bezierCurverInfo, BezierRatioPointInfo_e bezierPointInfo)
    {
        float y_valDis = bezierCurverInfo.Y_MaxVal - bezierCurverInfo.Y_MinVal;
        bezierPointInfo.OriginY = (bezierCurverInfo.Y_MaxVal - y_val) / y_valDis * bezierCurverInfo.height +
            bezierCurverInfo.y - bezierPointInfo.Height * 0.5f;

        RefreshBezierPoint_Show(curverIndex, bezierCurverInfo, bezierPointInfo);
    }

    public float JustGetRatioInTotal(float pointX, int curverIndex, BezierRatioInfo_e bezierCurverInfo)
    {
        return (pointX - GetRealStartX(curverIndex, bezierCurverInfo)) / (bezierCurverInfo.width * bezierCurverInfo.scale);
    }

    public float GetRatioInTotal(float pointX, int curverIndex, BezierRatioInfo_e bezierCurverInfo, int ratioPointIndex)
    {
        if (ratioPointIndex == 0)
            return 0f;
        else if (ratioPointIndex == bezierCurverInfo.m_BezierRatioPointInfoList_e.Count - 1)
            return 1f;

        return (pointX - GetRealStartX(curverIndex, bezierCurverInfo)) / (bezierCurverInfo.width * bezierCurverInfo.scale);
    }

    public float GetRatioInTotal(int curverIndex, BezierRatioInfo_e bezierCurverInfo, BezierRatioPointInfo_e bezierPointInfo)
    {
        int ratioPointIndex = -1;
        for (int i = 0, count = bezierCurverInfo.m_BezierRatioPointInfoList_e.Count; i < count; i++)
        {
            if (bezierCurverInfo.m_BezierRatioPointInfoList_e[i] == bezierPointInfo)
            {
                ratioPointIndex = i;
                break;
            }
        }
        if (ratioPointIndex < 0)
        {
            Debug.LogError($"GetRatioInTotal中有问题");
            return 0f;
        }

        return GetRatioInTotal(bezierPointInfo.ShowX + bezierPointInfo.Width * 0.5f, curverIndex, bezierCurverInfo, ratioPointIndex);
    }

    public float GetRatioInSegmentByPointX(float pointX, int curverIndex, BezierRatioInfo_e bezierCurverInfo, BezierRatioPointInfo_e startBpi, BezierRatioPointInfo_e endBpi)
    {
        float startTimeRatio = GetRatioInTotal(curverIndex, bezierCurverInfo, startBpi);
        float endTimeRatio = GetRatioInTotal(curverIndex, bezierCurverInfo, endBpi);
        float curTimeRatio = JustGetRatioInTotal(pointX, curverIndex, bezierCurverInfo);
        return startTimeRatio == endTimeRatio ? 0f : ((curTimeRatio - startTimeRatio) / (endTimeRatio - startTimeRatio));
    }

    public float GetRatioInSegmentByRatioInTotal(float ratioInTotal, int curverIndex, BezierRatioInfo_e bezierCurverInfo, BezierRatioPointInfo_e startBpi, BezierRatioPointInfo_e endBpi)
    {
        float startTimeRatio = GetRatioInTotal(curverIndex, bezierCurverInfo, startBpi);
        float endTimeRatio = GetRatioInTotal(curverIndex, bezierCurverInfo, endBpi);
        return startTimeRatio == endTimeRatio ? 0f : ((ratioInTotal - startTimeRatio) / (endTimeRatio - startTimeRatio));
    }

    public float GetRatio_CoordX(float ratioInSegm, int curverIndex, BezierRatioInfo_e bezierCurverInfo, BezierRatioPointInfo_e startBpi, BezierRatioPointInfo_e endBpi)
    {
        float startRatioInTotal = GetRatioInTotal(curverIndex, bezierCurverInfo, startBpi);
        float endRatioInTotal = GetRatioInTotal(curverIndex, bezierCurverInfo, endBpi);
        float s2e_rationDisInTotal = endRatioInTotal - startRatioInTotal;
        return GetRealStartX(curverIndex, bezierCurverInfo) + (startRatioInTotal + ratioInSegm * s2e_rationDisInTotal) * bezierCurverInfo.width * bezierCurverInfo.scale;
    }

    public float GetOriginStartX(int curverIndex, float startX, float width)
    {
        return startX + (curverIndex - 1) * width + 40f;
    }

    public float GetOriginStartX(int curverIndex, BezierRatioInfo_e bezierCurverInfo)
    {
        return GetOriginStartX(curverIndex, bezierCurverInfo.StartX, bezierCurverInfo.width);
    }

    public float GetRealStartX(int curverIndex, float startX, float width, float scale)
    {
        return startX + (curverIndex - 1) * width * scale + 40f;
    }

    public float GetRealStartX(int curverIndex, BezierRatioInfo_e bezierCurverInfo)
    {
        return GetRealStartX(curverIndex, bezierCurverInfo.StartX, bezierCurverInfo.width, bezierCurverInfo.scale);
    }

    public void RefreshAllBezierCurverInfos()
    {
        for (int btiIndex = 1, btiCount = _bezierController_3D.BezierTransInfoList.Count; btiIndex < btiCount; btiIndex++)
        {
            BezierRatioInfo_e bci = _bezierController_3D.BezierTransInfoList[btiIndex].m_BezierRatioInfo_e;

            float oldRectX = bci.x;

            bci.x = GetOriginStartX(btiIndex, bci);

            RefreshBezierPointInfo(btiIndex, bci, oldRectX);
        }
    }

    public void RefreshBezierPointInfo(int curverIndex, BezierRatioInfo_e bezierCurverInfo, float oldRectX)
    {
        for (int i = 0, count = bezierCurverInfo.m_BezierRatioPointInfoList_e.Count; i < count; i++)
        {
            var bpi = bezierCurverInfo.m_BezierRatioPointInfoList_e[i];
            RefreshBezierPoint_Origin(bezierCurverInfo, bpi, oldRectX);
            RefreshBezierPoint_Show(curverIndex, bezierCurverInfo, bpi);

            SetY_Val(bezierCurverInfo, bpi);
        }
    }

    public void RefreshBezierPoint_Origin(BezierRatioInfo_e bezierCurverInfo, BezierRatioPointInfo_e bezierPointInfo, float oldRectX)
    {
        float dis = bezierPointInfo.OriginX - oldRectX;
        bezierPointInfo.OriginX = bezierCurverInfo.x + dis;
    }

    public void RefreshBezierPoint_Show(int curverIndex, BezierRatioInfo_e bezierCurverInfo, BezierRatioPointInfo_e bezierPointInfo)
    {
        float originDisX = bezierPointInfo.OriginX + bezierPointInfo.Width * 0.5f - bezierCurverInfo.x;
        float originDisY = bezierPointInfo.OriginY + bezierPointInfo.Height * 0.5f - bezierCurverInfo.y;
        bezierPointInfo.ShowX = GetRealStartX(curverIndex, bezierCurverInfo) + originDisX * bezierCurverInfo.scale - bezierPointInfo.Width * 0.5f;
        bezierPointInfo.ShowY = bezierCurverInfo.y + originDisY * bezierCurverInfo.scale - bezierPointInfo.Height * 0.5f;

        TransformBezierCurverInfoToData();
    }

    public void TransformBezierCurverInfoToData()
    {
        Bezier1CurvesData[] b1cdArray = new Bezier1CurvesData[_bezierController_3D.BezierTransInfoList.Count - 1];
        for (int btiIndex = 1, btiCount = _bezierController_3D.BezierTransInfoList.Count; btiIndex < btiCount; btiIndex++)
        {
            var bci = _bezierController_3D.BezierTransInfoList[btiIndex].m_BezierRatioInfo_e;

            Bezier1CurvesData bezier1CurvesData = new Bezier1CurvesData();
            bezier1CurvesData.Y_MaxVal = bci.Y_MaxVal;
            bezier1CurvesData.Y_MinVal = bci.Y_MinVal;
            bezier1CurvesData.LeftTopY = bci.y;
            bezier1CurvesData.Hight = bci.height;

            int bciIndex_e = 0;
            int bciCount_e = bci.m_BezierRatioPointInfoList_e.Count;
            if (bciCount_e == 0)
                continue;

            int b1pdCount = 2 + (bciCount_e - 4) / 3;
            bezier1CurvesData.Bezier1GroupPosDataArray = new Bezier1GroupPosData[b1pdCount];
            for (int b1pdIndex = 0; b1pdIndex < b1pdCount; b1pdIndex++)
            {
                Bezier1GroupPosData bezier1GroupPosData = new Bezier1GroupPosData();

                if (b1pdIndex == 0)
                {
                    var bpi = bci.m_BezierRatioPointInfoList_e[bciIndex_e];
                    bezier1GroupPosData.Pos = CombatHelp.GetFloat3Decimal(bpi.OriginY + bpi.Height * 0.5f);
                    bezier1GroupPosData.PosRatioInTotal = 0f;

                    var rightBpi = bci.m_BezierRatioPointInfoList_e[bciIndex_e + 1];
                    bezier1GroupPosData.RightPos = CombatHelp.GetFloat3Decimal(rightBpi.OriginY + rightBpi.Height * 0.5f);
                    bezier1GroupPosData.RightRatioInTotal = CombatHelp.GetFloat3Decimal(GetRatioInTotal(btiIndex, bci, rightBpi));

                    bciIndex_e = 2;
                }
                else if (b1pdIndex == b1pdCount - 1)
                {
                    var leftBpi = bci.m_BezierRatioPointInfoList_e[bciIndex_e];
                    bezier1GroupPosData.LeftPos = CombatHelp.GetFloat3Decimal(leftBpi.OriginY + leftBpi.Height * 0.5f);
                    bezier1GroupPosData.LeftRatioInTotal = CombatHelp.GetFloat3Decimal(GetRatioInTotal(btiIndex, bci, leftBpi));

                    var bpi = bci.m_BezierRatioPointInfoList_e[bciIndex_e + 1];
                    bezier1GroupPosData.Pos = CombatHelp.GetFloat3Decimal(bpi.OriginY + bpi.Height * 0.5f);
                    bezier1GroupPosData.PosRatioInTotal = 1f;

                    bciIndex_e += 2;
                }
                else
                {
                    var leftBpi = bci.m_BezierRatioPointInfoList_e[bciIndex_e];
                    bezier1GroupPosData.LeftPos = CombatHelp.GetFloat3Decimal(leftBpi.OriginY + leftBpi.Height * 0.5f);
                    bezier1GroupPosData.LeftRatioInTotal = CombatHelp.GetFloat3Decimal(GetRatioInTotal(btiIndex, bci, leftBpi));

                    var bpi = bci.m_BezierRatioPointInfoList_e[bciIndex_e + 1];
                    bezier1GroupPosData.Pos = CombatHelp.GetFloat3Decimal(bpi.OriginY + bpi.Height * 0.5f);
                    bezier1GroupPosData.PosRatioInTotal = CombatHelp.GetFloat3Decimal(GetRatioInTotal(btiIndex, bci, bpi));

                    var rightBpi = bci.m_BezierRatioPointInfoList_e[bciIndex_e + 2];
                    bezier1GroupPosData.RightPos = CombatHelp.GetFloat3Decimal(rightBpi.OriginY + rightBpi.Height * 0.5f);
                    bezier1GroupPosData.RightRatioInTotal = CombatHelp.GetFloat3Decimal(GetRatioInTotal(btiIndex, bci, rightBpi));

                    bciIndex_e += 3;
                }

                bezier1CurvesData.Bezier1GroupPosDataArray[b1pdIndex] = bezier1GroupPosData;
            }

            b1cdArray[btiIndex - 1] = bezier1CurvesData;
        }

        _bezierController_3D.m_BezierCurvesFuncComponent.m_BezierRatioDataList = b1cdArray.ToList();
    }

    private void AddBezierGroupPoint_e(float ratioX, float mouseBezierPointY, float width, float height, int curverIndex, int insertIndex, BezierRatioInfo_e bezierCurverInfo)
    {
        AddBezierPoint(ratioX + 10f, mouseBezierPointY + 10f, width, height, curverIndex, insertIndex, bezierCurverInfo);

        AddBezierPoint(ratioX, mouseBezierPointY, width, height, curverIndex, insertIndex, bezierCurverInfo);

        AddBezierPoint(ratioX - 10f, mouseBezierPointY - 10f, width, height, curverIndex, insertIndex, bezierCurverInfo);

        TransformBezierCurverInfoToData();
    }

    private void AddBezierPoint(float bezierPointX, float bezierPointY, float width, float height, int curverIndex, int insertIndex, BezierRatioInfo_e bezierCurverInfo)
    {
        BezierRatioPointInfo_e addBpi = new BezierRatioPointInfo_e();

        addBpi.Width = width;
        addBpi.Height = height;

        addBpi.ShowX = bezierPointX - addBpi.Width * 0.5f;
        addBpi.ShowY = bezierPointY - addBpi.Height * 0.5f;

        addBpi.OriginX = bezierCurverInfo.x + (bezierPointX - GetRealStartX(curverIndex, bezierCurverInfo)) / bezierCurverInfo.scale - addBpi.Width * 0.5f;
        addBpi.OriginY = bezierCurverInfo.y + (bezierPointY - bezierCurverInfo.StartY) / bezierCurverInfo.scale - addBpi.Height * 0.5f;

        SetY_Val(bezierCurverInfo, addBpi);

        bezierCurverInfo.m_BezierRatioPointInfoList_e.Insert(insertIndex, addBpi);
    }
    #endregion

    #region Logic
    private Dictionary<uint, string> _ratioValPathDic = new Dictionary<uint, string>();
    private void SetBezierFilePaths()
    {
        if (!Directory.Exists(_bezierEditorTrackPrefabPath))
            Directory.CreateDirectory(_bezierEditorTrackPrefabPath);

        if (Directory.Exists(_bezierEditorRatioValDataPath))
        {
            _ratioValPathDic.Clear();

            string[] paths = Directory.GetFiles(_bezierEditorRatioValDataPath);
            foreach (var path in paths)
            {
                if (Path.GetExtension(path).Contains(".meta"))
                    continue;

                uint ratioValId = uint.Parse(Path.GetFileNameWithoutExtension(path));
                _ratioValPathDic.Add(ratioValId, path);
            }
        }
    }

    public void SaveAllData()
    {
        FileDataOperationReadEntity reader = new FileDataOperationReadEntity();

        FileDataOperationWriteEntity fileDataOperationWriteEntity = new FileDataOperationWriteEntity();

        if (Directory.Exists(_bezierEditorTrackPrefabPath))
        {
            long startPos = 0;
            long endPos = 0;

            string saveTrackFilePath = $"Assets/{CombatConfigManager.BezierTrackDataPath}";

            string[] paths = Directory.GetFiles(_bezierEditorTrackPrefabPath, "*.prefab", SearchOption.AllDirectories);

            fileDataOperationWriteEntity.StartWriter(saveTrackFilePath, (MemoryStream ms, BinaryWriter binaryWriter) =>
            {
                binaryWriter.Write(paths.Length);
            });

            int trackCount = 0;
            foreach (var path in paths)
            {
                string filePath = path.Replace("\\", "/");
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (!uint.TryParse(fileName, out uint bezierTrackId) || bezierTrackId == 0u)
                {
                    Debug.LogError($"文件{filePath}命名不正确");
                    continue;
                }
                var bc = AssetDatabase.LoadAssetAtPath<BezierCurverController_3D>(filePath);
                if (bc == null)
                    continue;

                ++trackCount;

                Bezier3CurvesData bezier3CurvesData = bc.m_BezierCurvesFuncComponent.m_BezierTrackData;

                //下次写入数据的位置+workId+WorkStreamData
                fileDataOperationWriteEntity.AddWriterData(bezier3CurvesData, (MemoryStream ms, BinaryWriter binaryWriter) =>
                {
                    startPos = ms.Position;
                    binaryWriter.Write(0u);
                    binaryWriter.Write(bezierTrackId);
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
                if (trackCount != paths.Length)
                {
                    Debug.Log($"实际写入文件数：{trackCount}，应有文件数：{paths.Length}");

                    ms.Seek(0, SeekOrigin.Begin);
                    binaryWriter.Write(trackCount);
                    ms.Seek(0, SeekOrigin.End);
                }
            }, true);
        }

        if (Directory.Exists(_bezierEditorRatioValDataPath))
        {
            long startPos = 0;
            long endPos = 0;

            string saveRatioFilePath = $"Assets/{CombatConfigManager.BezierRatioDataPath}";
            string[] paths = Directory.GetFiles(_bezierEditorRatioValDataPath, "*.txt", SearchOption.AllDirectories);

            fileDataOperationWriteEntity.StartWriter(saveRatioFilePath, (MemoryStream ms, BinaryWriter binaryWriter) =>
            {
                binaryWriter.Write(paths.Length);
            });

            int ratioCount = 0;
            foreach (var path in paths)
            {
                string filePath = path.Replace("\\", "/");
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (!uint.TryParse(fileName, out uint ratioId) || ratioId == 0u)
                {
                    Debug.LogError($"文件{filePath}命名不正确");
                    continue;
                }

                byte[] datas = File.ReadAllBytes(filePath);
                Bezier1CurvesData bezier1CurvesData = reader.DoRead(new MemoryStream(datas), (BinaryReader br) =>
                {
                    return Bezier1CurvesData_ConfigTool.Load(br);
                });

                if (bezier1CurvesData == null)
                {
                    Debug.LogError($"文件{filePath}获取Bezier1CurvesData类型的数据为null");
                    continue;
                }

                ++ratioCount;

                //下次写入数据的位置+workId+WorkStreamData
                fileDataOperationWriteEntity.AddWriterData(bezier1CurvesData, (MemoryStream ms, BinaryWriter binaryWriter) =>
                {
                    startPos = ms.Position;
                    binaryWriter.Write(0u);
                    binaryWriter.Write(ratioId);
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
                if (ratioCount != paths.Length)
                {
                    Debug.Log($"实际写入文件数：{ratioCount}，应有文件数：{paths.Length}");

                    ms.Seek(0, SeekOrigin.Begin);
                    binaryWriter.Write(ratioCount);
                    ms.Seek(0, SeekOrigin.End);
                }
            }, true);
        }

        AssetDatabase.Refresh();

        if (CombatConfigManager.Instance != null)
            CombatConfigManager.Instance.SetBezierDatas();
    }

    public void SaveBezierRatioData(uint ratioId, Bezier1CurvesData bezier1CurvesData)
    {
        if (ratioId == 0u || bezier1CurvesData == null)
            return;

        if (!Directory.Exists(_bezierEditorRatioValDataPath))
            Directory.CreateDirectory(_bezierEditorRatioValDataPath);

        string path = $"{_bezierEditorRatioValDataPath}/{ratioId.ToString()}.txt";

        if (bezier1CurvesData.Bezier1GroupPosDataArray != null &&
            bezier1CurvesData.Bezier1GroupPosDataArray.Length > 0)
        {
            if (bezier1CurvesData.Bezier1GroupPosDataArray[0].PosRatioInTotal != 0f)
            {
                Debug.LogError($"起始点的PosRatioInTotal为{bezier1CurvesData.Bezier1GroupPosDataArray[0].PosRatioInTotal}，修正为0");
                bezier1CurvesData.Bezier1GroupPosDataArray[0].PosRatioInTotal = 0f;
            }
            Bezier1GroupPosData lastBgpd = bezier1CurvesData.Bezier1GroupPosDataArray[bezier1CurvesData.Bezier1GroupPosDataArray.Length - 1];
            if (lastBgpd.PosRatioInTotal != 1f)
            {
                Debug.LogError($"起始点的PosRatioInTotal为{lastBgpd.PosRatioInTotal}，修正为1");
                lastBgpd.PosRatioInTotal = 1f;
            }
        }

        FileDataOperationWriteEntity fileDataOperationWriteEntity = new FileDataOperationWriteEntity();
        fileDataOperationWriteEntity.DoWriter(path, bezier1CurvesData, null, true);

        SetBezierFilePaths();

        AssetDatabase.Refresh();
    }

    public Bezier1CurvesData ParseBezierRatioData(uint ratioId)
    {
        if (ratioId == 0u)
            return null;

        if (!_ratioValPathDic.TryGetValue(ratioId, out string filePath))
            return null;

        if (!Directory.Exists(_bezierEditorRatioValDataPath))
            Directory.CreateDirectory(_bezierEditorRatioValDataPath);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"不存在文件{filePath}");
            return null;
        }

        byte[] datas = File.ReadAllBytes(filePath);

        FileDataOperationReadEntity fileDataOperationReadEntity = new FileDataOperationReadEntity();
        Bezier1CurvesData bezier1CurvesData = fileDataOperationReadEntity.DoRead(new MemoryStream(datas), (BinaryReader br) =>
        {
            return Bezier1CurvesData_ConfigTool.Load(br);
        });

        return bezier1CurvesData;
    }

    private void TransformBezierRatio2Editor(int curverIndex, BezierRatioInfo_e bezierCurverInfo, Bezier1CurvesData bezier1CurvesData)
    {
        if (bezier1CurvesData == null)
            return;

        float halfLen = 2.5f;

        bezierCurverInfo.m_BezierRatioPointInfoList_e.Clear();
        for (int b1pIndex = 0, b1pCount = bezier1CurvesData.Bezier1GroupPosDataArray.Length; b1pIndex < b1pCount; b1pIndex++)
        {
            Bezier1GroupPosData bezier1PosData = bezier1CurvesData.Bezier1GroupPosDataArray[b1pIndex];

            if (b1pIndex > 0)
            {
                BezierRatioPointInfo_e leftBpi = new BezierRatioPointInfo_e();
                leftBpi.OriginX = bezierCurverInfo.x + bezierCurverInfo.width * bezier1PosData.LeftRatioInTotal - halfLen;
                leftBpi.OriginY = bezier1PosData.LeftPos - halfLen;
                leftBpi.ShowX = _gl_rect.x + bezierCurverInfo.width * bezierCurverInfo.scale * bezier1PosData.LeftRatioInTotal - halfLen;
                leftBpi.ShowY = bezierCurverInfo.y + (bezier1PosData.LeftPos - bezierCurverInfo.y) * bezierCurverInfo.scale - halfLen;
                leftBpi.Width = halfLen * 2;
                leftBpi.Height = halfLen * 2;
                bezierCurverInfo.m_BezierRatioPointInfoList_e.Add(leftBpi);
            }

            BezierRatioPointInfo_e bpi = new BezierRatioPointInfo_e();
            bpi.OriginX = bezierCurverInfo.x + bezierCurverInfo.width * bezier1PosData.PosRatioInTotal - halfLen;
            bpi.OriginY = bezier1PosData.Pos - halfLen;
            bpi.ShowX = _gl_rect.x + bezierCurverInfo.width * bezierCurverInfo.scale * bezier1PosData.PosRatioInTotal - halfLen;
            bpi.ShowY = bezierCurverInfo.y + (bezier1PosData.Pos - bezierCurverInfo.y) * bezierCurverInfo.scale - halfLen;
            bpi.Width = halfLen * 2;
            bpi.Height = halfLen * 2;
            bezierCurverInfo.m_BezierRatioPointInfoList_e.Add(bpi);

            if (b1pIndex < b1pCount - 1)
            {
                BezierRatioPointInfo_e rightBpi = new BezierRatioPointInfo_e();
                rightBpi.OriginX = bezierCurverInfo.x + bezierCurverInfo.width * bezier1PosData.RightRatioInTotal - halfLen;
                rightBpi.OriginY = bezier1PosData.RightPos - halfLen;
                rightBpi.ShowX = _gl_rect.x + bezierCurverInfo.width * bezierCurverInfo.scale * bezier1PosData.RightRatioInTotal - halfLen;
                rightBpi.ShowY = bezierCurverInfo.y + (bezier1PosData.RightPos - bezierCurverInfo.y) * bezierCurverInfo.scale - halfLen;
                rightBpi.Width = halfLen * 2;
                rightBpi.Height = halfLen * 2;
                bezierCurverInfo.m_BezierRatioPointInfoList_e.Add(rightBpi);
            }
        }
    }

    public void DelBezier()
    {
        _selectCurverIndex = -1;
        _selectBezierPIndex = -1;
        if (_bezierController_3D != null)
        {
            _bezierController_3D.DelBezier(_bezierController_3D.m_SelectGo);

            RefreshAllBezierCurverInfos();
        }
    }

    private void UpdateConstantSpeedScale(float scale)
    {
        if (scale <= 0)
            return;

        if (Directory.Exists(_bezierEditorTrackPrefabPath))
        {
            string saveTrackFilePath = $"Assets/{CombatConfigManager.BezierTrackDataPath}";

            string[] paths = Directory.GetFiles(_bezierEditorTrackPrefabPath, "*.prefab", SearchOption.AllDirectories);
            foreach (var path in paths)
            {
                BezierCurverController_3D bezierCurverController_3D = AssetDatabase.LoadAssetAtPath<BezierCurverController_3D>(path);
                if (bezierCurverController_3D.m_BezierCurvesFuncComponent != null && bezierCurverController_3D.m_BezierCurvesFuncComponent.m_BezierTrackData != null &&
                    bezierCurverController_3D.m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams != null)
                {
                    for (int i = 0, count = bezierCurverController_3D.m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams.Length; i < count; i++)
                    {
                        int pa = bezierCurverController_3D.m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams[i];
                        if (pa < 0)
                        {
                            pa = (int)(pa * scale);
                            bezierCurverController_3D.m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams[i] = pa;
                        }
                    }
                }
                EditorUtility.SetDirty(bezierCurverController_3D);
            }
        }

        AssetDatabase.Refresh();
    }

    private void CheckCurversParams()
    {
        if (Directory.Exists(_bezierEditorTrackPrefabPath))
        {
            string saveTrackFilePath = $"Assets/{CombatConfigManager.BezierTrackDataPath}";

            string[] paths = Directory.GetFiles(_bezierEditorTrackPrefabPath, "*.prefab", SearchOption.AllDirectories);
            foreach (var path in paths)
            {
                BezierCurverController_3D bezierCurverController_3D = AssetDatabase.LoadAssetAtPath<BezierCurverController_3D>(path);
                if (bezierCurverController_3D.m_BezierCurvesFuncComponent != null && bezierCurverController_3D.m_BezierCurvesFuncComponent.m_BezierTrackData != null)
                {
                    if (bezierCurverController_3D.m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams == null)
                        Debug.LogError($"{path}没有设置m_BezierParams速度");
                }
            }
        }
    }
    #endregion

    private static GUIStyle _labelSt;
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawGameObjectName(Transform transform, GizmoType gizmoType)
    {
        if (m_Window == null || !m_Window._isDrawBezierName)
            return;

        if (m_Window._bezierController_3D == null || m_Window._bezierController_3D.BezierTransInfoList == null ||
            m_Window._bezierController_3D.BezierTransInfoList.Count == 0)
            return;

        if (_labelSt == null)
        {
            _labelSt = new GUIStyle(EditorStyles.label);
            _labelSt.wordWrap = true;
            _labelSt.alignment = TextAnchor.MiddleCenter;
            _labelSt.fontSize = 15;
        }

        for (int i = 0, count = m_Window._bezierController_3D.BezierTransInfoList.Count; i < count; i++)
        {
            var info = m_Window._bezierController_3D.BezierTransInfoList[i];
            if (info.m_Trans == transform ||
                info.m_LeftHelpTrans == transform ||
                info.m_RightHelpTrans == transform)
            {
                _labelSt.normal.textColor = info.m_Color;
                if (transform.gameObject.activeInHierarchy)
                    Handles.Label(transform.position, transform.name, _labelSt);

                if (m_Window._bezierController_3D.m_IsShowEventInfo && info.m_bezierCuverEvent_e_List.Count > 0 && info.m_Trans == transform)
                {
                    foreach (var eventItem in info.m_bezierCuverEvent_e_List)
                    {
                        if (eventItem.EventTrans.gameObject.activeInHierarchy)
                            Handles.Label(eventItem.EventTrans.position, eventItem.EventTrans.name, _labelSt);
                    }
                }

                break;
            }
        }
    }

    private void WindowKeyCodeEvent()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            if (e.isKey)
            {
                if (_bezierController_3D != null && _bezierController_3D.BezierTransInfoList.Count > 0 &&
                    _selectCurverIndex > -1 && _selectBezierPIndex > -1)
                {
                    if (e.keyCode == KeyCode.LeftArrow)
                    {
                        if (_selectBezierPIndex == 0)
                        {
                            if (_selectCurverIndex > 1)
                            {
                                --_selectCurverIndex;
                                _selectBezierPIndex = _bezierController_3D.BezierTransInfoList[_selectCurverIndex].m_BezierRatioInfo_e.m_BezierRatioPointInfoList_e.Count - 1;
                            }
                        }
                        else
                        {
                            if (_isShowHelpRatioPoint)
                            {
                                --_selectBezierPIndex;
                            }
                            else if (_selectBezierPIndex % 3 == 0)
                            {
                                _selectBezierPIndex -= 3;
                            }
                        }

                        return;
                    }
                    else if (e.keyCode == KeyCode.RightArrow)
                    {
                        if (_selectBezierPIndex == _bezierController_3D.BezierTransInfoList[_selectCurverIndex].m_BezierRatioInfo_e.m_BezierRatioPointInfoList_e.Count - 1)
                        {
                            if (_selectCurverIndex < _bezierController_3D.BezierTransInfoList.Count - 1)
                            {
                                ++_selectCurverIndex;
                                _selectBezierPIndex = 0;
                            }
                        }
                        else
                        {
                            if (_isShowHelpRatioPoint)
                                ++_selectBezierPIndex;
                            else if (_selectBezierPIndex % 3 == 0)
                                _selectBezierPIndex += 3;
                        }

                        return;
                    }
                    else if (e.keyCode == KeyCode.Delete)
                    {
                        BezierRatioInfo_e bezierCurverInfo = _bezierController_3D.BezierTransInfoList[_selectCurverIndex].m_BezierRatioInfo_e;

                        if (_selectBezierPIndex > 0 && _selectBezierPIndex < bezierCurverInfo.m_BezierRatioPointInfoList_e.Count - 1 &&
                            (_selectBezierPIndex % 3 == 0))
                        {
                            bezierCurverInfo.m_BezierRatioPointInfoList_e.RemoveAt(_selectBezierPIndex - 1);
                            bezierCurverInfo.m_BezierRatioPointInfoList_e.RemoveAt(_selectBezierPIndex - 1);
                            bezierCurverInfo.m_BezierRatioPointInfoList_e.RemoveAt(_selectBezierPIndex - 1);

                            Bezier1CurvesData bezier1CurvesData = _bezierController_3D.m_BezierCurvesFuncComponent.m_BezierRatioDataList[_selectCurverIndex - 1];
                            List<Bezier1GroupPosData> list = bezier1CurvesData.Bezier1GroupPosDataArray.ToList();
                            list.RemoveAt(_selectBezierPIndex / 3);

                            bezier1CurvesData.Bezier1GroupPosDataArray = list.ToArray();
                        }
                    }
                }
            }
        }
    }
}

public class CurvesGlobalEvent : EditorAssembliesEvent<CurvesGlobalEvent>
{
    public override void OnGlobalEventHandler(Event e)
    {
        if (e.type == EventType.KeyDown)
        {
            if (e.isKey)
            {
                if (e.keyCode == KeyCode.Delete)
                {
                    CurvesEditorWindow.m_Window.DelBezier();
                }
            }
        }
    }
}
