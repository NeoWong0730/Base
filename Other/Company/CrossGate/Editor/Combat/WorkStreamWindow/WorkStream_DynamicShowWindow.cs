using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class WorkStreamWindow
{
    public class RunNodeInfo
    {
        /// <summary>
        /// =1Enter；=2Stay；=4Exit;
        /// </summary>
        public int RunType;
        public int RunBlockType;
        public uint RunWorkNodeId;
        public Color BackColor = Color.yellow;
        public uint CurEnterFrame;
        public uint CurExitFrame;

        public CacheRunNodeInfo m_CacheRunNodeInfo;

        public void Push()
        {
            m_CacheRunNodeInfo = null;

            CombatObjectPool.Instance.Push(this);
        }
    }

    public class CacheRunNodeInfo
    {
        public int RunBlockType;
        public uint RunWorkNodeId;
        public Color BackColor = Color.yellow;
        public uint StartEnterFrame;
        public uint StartExitFrame;
        public uint DeleRunNodeInfoFrame;
    }

    private Dictionary<long, WorkStreamTranstionComponent> _wstcDic = new Dictionary<long, WorkStreamTranstionComponent>();

    private Dictionary<uint, List<RunNodeInfo>> _runNodeInfoDic = new Dictionary<uint, List<RunNodeInfo>>();
    protected uint _cacheCurFrame;
    protected uint _cacheMaxFrameCount = 5000u;
    protected uint _frameCountSlider;
    protected Dictionary<uint, List<CacheRunNodeInfo>> _cacheRunNodeInfoDic = new Dictionary<uint, List<CacheRunNodeInfo>>();
    protected bool _stopCacheFrame;

    protected uint _curEnterFrame;
    protected uint _curExitFrame;
    //下个节点显示的时间
    protected uint _intervalShowRunNodeFrame = 20u;
    //当前节点存活显示的时间
    protected float _showRunNodeScale = 10f;

    private GameObject _selectGo;

    private Vector2 _leftDownSpaceToDynamicShowScrollV2;

    private bool _showCurWorkStream
    {
        get
        {
            return m_SelectWorkId > 0 && ((_runNodeInfoDic != null && _runNodeInfoDic.Count > 0) || (_cacheRunNodeInfoDic != null && _cacheRunNodeInfoDic.Count > 0));
        }
    }

    private Workstream_Test _workstream_Test;
    private Workstream_Test s_Workstream_Test 
    {
        get { return _workstream_Test; }
        set 
        {
            if (_workstream_Test != value) 
            {
                _workstream_Test = value;
                if (_workstream_Test != null)
                {
                    _workstream_Test.SetWorkStreamTranstionComponentList();
                    InitShowDynamicNodeData(_workstream_Test.m_WorkStreamTranstionComponentList);
                }
                else 
                {
                    ClearShowDynamicNodeData();
                }
            }
        }
    }

    private void InitShowDynamicNodeData(List<WorkStreamTranstionComponent> wstcList) 
    {
        ClearShowDynamicNodeData();

        for (int i = 0, count = wstcList.Count; i < count; i++)
        {
            var wstc = wstcList[i];

            _wstcDic[wstc.Id] = wstc;
        }
        if (_wstcDic != null) 
        {
            int wstcIndex = 0;
            foreach (var kv in _wstcDic)
            {
                var wstc = kv.Value;
                if (wstc == null || wstc.m_CurUseEntity == null || wstc.m_WorkId == 0u)
                    continue;

                SetWorkStreamTranformComp(wstc, wstcIndex == 0);

                ++wstcIndex;
            }
        }
    }

    private void SetWorkStreamTranformComp(WorkStreamTranstionComponent wstc, bool isLocationShowWorkId)
    {
        Debug.Log($"<color=yellow>正在监听WorkId：{wstc.m_WorkId.ToString()}</color>");

        if (isLocationShowWorkId)
            LocationShowWorkId(wstc.m_WorkId);

        if (wstc.m_CurUseEntity.m_StartStateAction == null)
            wstc.m_CurUseEntity.m_StartStateAction = StartStateCompAction;
        else
            wstc.m_CurUseEntity.m_StartStateAction += StartStateCompAction;

        if (wstc.m_CurUseEntity.m_EndStateAction == null)
            wstc.m_CurUseEntity.m_EndStateAction = EndStateCompAction;
        else
            wstc.m_CurUseEntity.m_EndStateAction += EndStateCompAction;

        if (wstc.m_CurWorkNodeDataList != null)
        {
            for (int i = 0, count = wstc.m_CurWorkNodeDataList.Count; i < count; i++)
            {
                AddRunNodeInfo(wstc.m_CurWorkBlockData.CurWorkBlockType, wstc.m_CurWorkNodeDataList[i].Id, wstc.m_WorkId);
            }
        }
    }

    private void ClearShowDynamicNodeData() 
    {
        if (_wstcDic != null)
        {
            foreach (var kv in _wstcDic)
            {
                var wstc = kv.Value;
                if (wstc.m_CurUseEntity != null)
                {
                    if (wstc.m_CurUseEntity.m_StartStateAction != null)
                        wstc.m_CurUseEntity.m_StartStateAction -= StartStateCompAction;
                    if (wstc.m_CurUseEntity.m_EndStateAction != null)
                        wstc.m_CurUseEntity.m_EndStateAction -= EndStateCompAction;
                }
            }

            _wstcDic.Clear();
        }

        if (_runNodeInfoDic != null)
        {
            foreach (var kv in _runNodeInfoDic)
            {
                for (int i = 0, count = kv.Value.Count; i < count; i++)
                {
                    kv.Value[i].Push();
                }
            }
            
            _runNodeInfoDic.Clear();
        }
        if (_cacheRunNodeInfoDic != null)
            _cacheRunNodeInfoDic.Clear();
    }

    private void ClearDynamicNodeInfo(uint workId)
    {
        if(_runNodeInfoDic != null)
        {
            if (_runNodeInfoDic.TryGetValue(workId, out List<RunNodeInfo> runList) && runList != null)
            {
                for (int i = 0, count = runList.Count; i < count; i++)
                {
                    runList[i].Push();
                }
                runList.Clear();
            }
        }
        if (_cacheRunNodeInfoDic != null)
        {
            if (_cacheRunNodeInfoDic.TryGetValue(workId, out List<CacheRunNodeInfo> cacheList) && cacheList != null)
            {
                cacheList.Clear();
            }
        }
    }

    private void UpdateDynamicShowFrame() 
    {
        if (!_stopCacheFrame)
        {
            _frameCountSlider = _cacheCurFrame = FrameCount;
        }
    }

    private void UpdateShowDynamicRunNode()
    {
        GameObject go = Selection.activeGameObject;
        if (go != _selectGo) 
        {
            _selectGo = go;
            s_Workstream_Test = Workstream_Test.GetWorkstream_Test(go);
        }

        if (s_Workstream_Test != null)
        {
            s_Workstream_Test.SetWorkStreamTranstionComponentList();
        }

        CheckData();

        if (!_stopCacheFrame)
        {
            if (_runNodeInfoDic.TryGetValue(m_SelectWorkId, out List<RunNodeInfo> runList) && runList != null)
            {
                RunNode(runList, FrameCount);
            }
        }
    }

    private void DrawLeftDownSpaceToDynamicShow(float height)
    {
        using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(height)))
        {
            using (EditorGUILayout.ScrollViewScope svs = new EditorGUILayout.ScrollViewScope(_leftDownSpaceToDynamicShowScrollV2))
            {
                DrawLeftDownSpaceBox();

                int wstcIndex = 0;
                foreach (var kv in _wstcDic)
                {
                    bool isLine = (wstcIndex % 2) == 0;

                    wstcIndex++;

                    if (isLine) 
                    {
                        EditorGUILayout.BeginHorizontal();
                    }

                    var wstc = kv.Value;
                    if (wstc.m_WorkId == 0u)
                        continue;

                    if (GUILayout.Button(wstc.m_WorkId.ToString()))
                    {
                        LocationShowWorkId(wstc.m_WorkId);
                    }

                    if (isLine)
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }

                _leftDownSpaceToDynamicShowScrollV2 = svs.scrollPosition;
            }
        }
    }

    private void DrawLeftDownSpaceBox() 
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        _frameCountSlider = (uint)EditorGUILayout.IntSlider((int)_frameCountSlider, _cacheCurFrame > _cacheMaxFrameCount ? (int)(_cacheCurFrame - _cacheMaxFrameCount) : 0, (int)_cacheCurFrame);
        if (_stopCacheFrame)
        {
            if (_cacheRunNodeInfoDic.TryGetValue(m_SelectWorkId, out List<CacheRunNodeInfo> cacheList) && cacheList != null)
            {
                RunCacheNode(cacheList, _frameCountSlider);
            }
        }

        GUILayout.Space(5.5f);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("间隔显示时间：");
        _intervalShowRunNodeFrame = (uint)EditorGUILayout.IntField((int)_intervalShowRunNodeFrame);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5.5f);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("消失速度：");
        _showRunNodeScale = (uint)EditorGUILayout.IntField((int)_showRunNodeScale);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5.5f);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"当先显示WorkId:{m_SelectWorkId.ToString()}    ");
        _stopCacheFrame = GUILayout.Toggle(_stopCacheFrame, "暂停");
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5.5f);

        EditorGUILayout.EndVertical();
    }

    protected void ShowDynamicGUI() 
    {
        EditorGUILayout.BeginHorizontal();

        var useLeftDownSpace = GUILayout.Toggle(_useLeftDownSpace, "实时展示运行效果");
        if (_useLeftDownSpace != useLeftDownSpace)
        {
            _useLeftDownSpace = useLeftDownSpace;
            ClearShowDynamicNodeData();
        }
        EditorGUILayout.EndVertical();
    }

    private void ShowDynamicRunNode(WorkBlockData workBlockData, WorkNodeData curDrawNode, float x, float y, float width, float height)
    {
        if (!_showCurWorkStream)
            return;

        if (_stopCacheFrame)
        {
            if (_cacheRunNodeInfoDic.TryGetValue(m_SelectWorkId, out List<CacheRunNodeInfo> cacheList) && cacheList != null) 
            {
                foreach (var item in cacheList)
                {
                    if (item.RunBlockType == workBlockData.CurWorkBlockType && item.RunWorkNodeId == curDrawNode.Id)
                    {
                        EditorGUI.DrawRect(new Rect(x - 3f, y - 3f, width + 6f, height + 6f), item.BackColor);
                        return;
                    }
                }
            }
        }
        else
        {
            if (_runNodeInfoDic.TryGetValue(m_SelectWorkId, out List<RunNodeInfo> runList) && runList != null)
            {
                foreach (var item in runList)
                {
                    if (item.RunBlockType == workBlockData.CurWorkBlockType && item.RunWorkNodeId == curDrawNode.Id)
                    {
                        EditorGUI.DrawRect(new Rect(x - 3f, y - 3f, width + 6f, height + 6f), item.BackColor);
                        return;
                    }
                }
            }
        }
    }

    #region Logic
    private void RunNode(List<RunNodeInfo> runNodeInfoList, uint frameCount)
    {
        if (runNodeInfoList != null)
        {
            for (int i = runNodeInfoList.Count - 1; i > -1; i--)
            {
                var runNode = runNodeInfoList[i];
                if ((runNode.RunType & 1) > 0 && runNode.CurEnterFrame < frameCount)
                {
                    runNode.m_CacheRunNodeInfo.StartEnterFrame = runNode.CurEnterFrame;
                    runNode.m_CacheRunNodeInfo.StartExitFrame = uint.MaxValue;

                    runNode.BackColor.a = (frameCount - runNode.CurEnterFrame) * _showRunNodeScale / 255f;
                    if (runNode.BackColor.a >= 1)
                    {
                        runNode.BackColor.a = 1;
                        if ((runNode.RunType & 4) > 0)
                        {
                            runNode.RunType = 4;
                            if (runNode.CurExitFrame < frameCount)
                                runNode.CurExitFrame = frameCount - 1u;
                        }
                        else
                            runNode.RunType = 2;
                    }
                }
                else if ((runNode.RunType & 4) > 0 && runNode.CurExitFrame < frameCount)
                {
                    runNode.m_CacheRunNodeInfo.StartExitFrame = runNode.CurExitFrame;

                    runNode.BackColor.a = 1 - ((frameCount - runNode.CurExitFrame) * _showRunNodeScale / 255f);
                    if (runNode.BackColor.a <= 0)
                    {
                        runNode.m_CacheRunNodeInfo.DeleRunNodeInfoFrame = frameCount;

                        runNode.Push();
                        runNodeInfoList.RemoveAt(i);
                    }
                }
            }
        }
    }

    protected void RunCacheNode(List<CacheRunNodeInfo> cacheRunNodeInfoList, uint frameCount)
    {
        if (cacheRunNodeInfoList != null)
        {
            for (int i = cacheRunNodeInfoList.Count - 1; i > -1; i--)
            {
                var runNode = cacheRunNodeInfoList[i];
                if (frameCount < runNode.StartEnterFrame)
                {
                    runNode.BackColor.a = 0f;
                }
                else if (runNode.StartEnterFrame < frameCount && runNode.StartExitFrame > frameCount)
                {
                    runNode.BackColor.a = (frameCount - runNode.StartEnterFrame) * _showRunNodeScale / 255f;
                }
                else if (runNode.StartExitFrame < frameCount)
                {
                    runNode.BackColor.a = 1 - ((frameCount - runNode.StartExitFrame) * _showRunNodeScale / 255f);
                }
            }
        }
    }

    private void AddRunNodeInfo(int blockType, uint nodeId, uint workId)
    {
        if (!_cacheRunNodeInfoDic.TryGetValue(workId, out List<CacheRunNodeInfo> cacheList) || cacheList == null)
        {
            cacheList = new List<CacheRunNodeInfo>();
            _cacheRunNodeInfoDic[workId] = cacheList;
        }

        if (!_runNodeInfoDic.TryGetValue(workId, out List<RunNodeInfo> runList) || runList == null)
        {
            runList = new List<RunNodeInfo>();
            _runNodeInfoDic[workId] = runList;
        }

        RunNodeInfo runNodeInfo = CombatObjectPool.Instance.Get<RunNodeInfo>();
        runNodeInfo.CurEnterFrame = GetShowFrame(1, runList);
        runNodeInfo.RunType = 1;
        runNodeInfo.RunBlockType = blockType;
        runNodeInfo.RunWorkNodeId = nodeId;
        runNodeInfo.BackColor = Color.blue;
        runNodeInfo.BackColor.a = 0f;

        runNodeInfo.m_CacheRunNodeInfo = new CacheRunNodeInfo();
        runNodeInfo.m_CacheRunNodeInfo.RunBlockType = blockType;
        runNodeInfo.m_CacheRunNodeInfo.RunWorkNodeId = nodeId;

        cacheList.Add(runNodeInfo.m_CacheRunNodeInfo);

        runList.Add(runNodeInfo);
    }

    private void SetDeleRunNodeState(int blockType, uint nodeId, uint workId)
    {
        if (_runNodeInfoDic.TryGetValue(workId, out List<RunNodeInfo> runList) && runList != null)
        {
            for (int i = 0, count = runList.Count; i < count; i++)
            {
                var runNodeInfo = runList[i];
                if (runNodeInfo.RunBlockType == blockType && runNodeInfo.RunWorkNodeId == nodeId)
                {
                    runNodeInfo.CurExitFrame = GetShowFrame(4, runList);
                    if (runNodeInfo.RunType == 1)
                    {
                        runNodeInfo.RunType = 5;
                        uint frame = (uint)(255f / _showRunNodeScale) + runNodeInfo.CurEnterFrame;
                        if (runNodeInfo.CurExitFrame < frame)
                            runNodeInfo.CurExitFrame = frame;
                    }
                    else
                        runNodeInfo.RunType = 4;

                    return;
                }
            }
        }
    }

    private uint GetShowFrame(int runType, List<RunNodeInfo> runList)
    {
        uint maxFrame = 0u;
        foreach (var item in runList)
        {
            if ((runType & item.RunType) > 0)
            {
                if (runType == 1 && item.CurEnterFrame > maxFrame)
                    maxFrame = item.CurEnterFrame;
                else if (runType == 4 && item.CurExitFrame > maxFrame)
                    maxFrame = item.CurExitFrame;
            }
        }

        return maxFrame < FrameCount ? FrameCount : maxFrame + _intervalShowRunNodeFrame;
    }

    private void StartStateCompAction(StateBaseComponent state) 
    {
        WorkStreamTranstionComponent workStreamTranstionComponent = state.m_CurUseEntity?.GetComponent<WorkStreamTranstionComponent>();
        if (workStreamTranstionComponent == null || workStreamTranstionComponent.m_CurWorkBlockData == null ||
            state.m_DataNodeId == 0 || workStreamTranstionComponent.m_WorkId == 0u)
            return;

        AddRunNodeInfo(workStreamTranstionComponent.m_CurWorkBlockData.CurWorkBlockType, state.m_DataNodeId, workStreamTranstionComponent.m_WorkId);
    }

    private void EndStateCompAction(StateBaseComponent state) 
    {
        WorkStreamTranstionComponent workStreamTranstionComponent = state.m_CurUseEntity?.GetComponent<WorkStreamTranstionComponent>();
        if (workStreamTranstionComponent == null || workStreamTranstionComponent.m_CurWorkBlockData == null ||
            state.m_DataNodeId == 0 || workStreamTranstionComponent.m_WorkId == 0u)
            return;

        SetDeleRunNodeState(workStreamTranstionComponent.m_CurWorkBlockData.CurWorkBlockType, state.m_DataNodeId, workStreamTranstionComponent.m_WorkId);
    }

    private Queue<long> _removeIdQueue = new Queue<long>();
    private void CheckData() 
    {
        if (s_Workstream_Test != null && s_Workstream_Test.m_WorkStreamTranstionComponentList != null)
        {
            int addIndex = 0;
            foreach (var testWstc in s_Workstream_Test.m_WorkStreamTranstionComponentList)
            {
                if (!_wstcDic.ContainsKey(testWstc.Id))
                {
                    _wstcDic[testWstc.Id] = testWstc;

                    ClearDynamicNodeInfo(testWstc.m_WorkId);

                    SetWorkStreamTranformComp(testWstc, addIndex == 0);

                    ++addIndex;
                }
            }
        }
        _removeIdQueue.Clear();
        foreach (var wstcKV in _wstcDic)
        {
            var wstc = wstcKV.Value;
            if (wstc == null || wstcKV.Key != wstc.Id)
            {
                _removeIdQueue.Enqueue(wstcKV.Key);
            }
        }
        while (_removeIdQueue.Count > 0)
        {
            _wstcDic.Remove(_removeIdQueue.Dequeue());
        }

        foreach (var kv in _runNodeInfoDic)
        {
            bool isExist = false;
            
            foreach (var wstcKV in _wstcDic)
            {
                var wstc = wstcKV.Value;
                if (wstc.m_WorkId == kv.Key)
                {
                    isExist = true;
                    break;
                }
            }

            if (!isExist) 
            {
                for (int i = kv.Value.Count - 1; i > -1; i--)
                {
                    var runNode = kv.Value[i];

                    runNode.m_CacheRunNodeInfo.StartEnterFrame = runNode.CurEnterFrame;
                    
                    if ((runNode.RunType & 4) == 0)
                    {
                        if (runNode.RunType == 1)
                        {
                            runNode.RunType = 5;
                            uint frame = (uint)(255f / _showRunNodeScale) + runNode.CurEnterFrame;
                            if (runNode.CurExitFrame < frame)
                                runNode.CurExitFrame = frame;
                        }
                        else
                            runNode.RunType = 4;
                    }
                }
            }
        }
    }
    #endregion
}
