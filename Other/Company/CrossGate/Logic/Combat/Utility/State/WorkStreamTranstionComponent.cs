using System.Collections.Generic;

/// <summary>
/// 1：同一层级中组为-1的可以和其他组并行，同一组中也可以有并行节点；
/// 2.1：同一层级中同一组从第一个节点开始运行，直到该节点所有子节点运行完毕，
/// 2.2：再回到同一层级的同一组的下一个节点，若下一个节点为串行，则运行该节点，如果该节点为并行或者没有下一节点，则该节点关联的该层级主线运行完毕。
/// 2.3：然后回到父节点，执行2.2，循环下去直到整个工作块结束或者某个节点直接跳出该块进入指定块。
/// 3：如果想同一层级中同一组等指定并行节点都运行完才返回父节点，可以通过计数节点来解决，如：
/// 同一层级的同一组的第一个节点为计数节点，和其他需要运行完的节点组成并行节点，
/// 每个并行节点的串行区域最后加一个减计数节点，这样运行完了都会通知计数节点减计数，减到一定数值计数节点结束，
/// 意味着该层级的该组结束，返回父节点。
/// 4：通过3方案，扩展出去，把计数节点改成需要达到一定条件才结束的节点，每个并行节点的串行区域最后加一个凑足其中一个条件的节点。
/// 该扩展方案就能衍生多样丰富的组合。
/// 5:同一组中标记主线,不同组中，选取一条主线作为总主线返回父节点进行运行，选取规则如下：
/// 如果有组为-1的，那么只能是该组中主线做为总主线，
/// 如果没有组为-1，那么就把选取的组中的主线作为总主线
///
/// 
/// 组：
///     概念：-1组（存在的话必运行的组）和其他组，通过编辑器Node中头部填写数值来分组，相同数值的为一组，数值设定范围为（-128 ~ 127（注：-1为特殊组，也是默认组））
///     作用：实现选择执行功能，有-1组的话一定会运行，其他组中最多选取其中一个来执行
///     
/// 直系兄弟组：拥有同一个父类的组
/// 
/// -1组：编辑器默认的特殊组，存在就会运行，它的主线就会成为“总主线”
/// 
/// 并行块：
///     概念：范围为“组”，在“组”里面考虑的概念，编辑器中通过“勾选按钮”来划分并行块，从“最上面的” 或者“打勾的”Node开始到“下个打勾的前一个”Node成为一个并行块
///     作用：实现并行功能，同一组中并行块们同时开始运行
///     
/// 层级：
///     概念：像人中同一辈分的概念，，编辑器中自动分层级
///     作用：编辑器中用到，其他地方暂未用到
///     
/// 主线：
///     概念：范围为“组”，在“组”里面考虑的概念，编辑器中“每一个组”中“第一个”并行块作为主线，“每一个组”中都有“一条”主线
///     作用：用来生成“总主线”
///     
/// 总主线：
///     概念：范围为“直系兄弟组”，“每一个”直系兄弟组中只能有且仅有一条“主线”成为主线，【“有-1组”的情况，-1组的主线为“总主线”】【“没有-1组”，选取的组的主线为“总主线”】
///     作用：用来区分“直系兄弟组”里面“哪个主线”有资格返回父类继续运行。
///     
/// 注：只有进入所属范围，才开始划分概念。
///     必须“所有正在”运行的Node节点运行完才结束WorkBlock的运行，比如：还有正在运行的“任何并行块”，那么该WorkBlock都不算结束。
///         
/// </summary>
#if UNITY_EDITOR
[System.Serializable]
#endif
public class WorkStreamTranstionComponent : StateTranstionComponent, IUpdate
{
    public class TranstionStateInfo
    {
        public int m_RunType;
        public WorkNodeData m_OldWorkNodeData;
        public int m_SelectType;

        public void Push()
        {
            m_RunType = 1;
            m_OldWorkNodeData = null;
            m_SelectType = -1;

            CombatObjectPool.Instance.Push(this);
        }
    }

    public uint m_WorkId;

    public List<WorkBlockData> m_ShareWorkBlockDataList;
    public List<WorkBlockData> m_WorkBlockDataList;

    /// <summary>
    /// WorkBlockData开始运行的GroupIndex
    /// </summary>
    public int m_StartGroupIndex = -1;

    public WorkBlockData m_CurWorkBlockData;

    public List<WorkNodeData> m_CurWorkNodeDataList = new List<WorkNodeData>();

    private Queue<TranstionStateInfo> _prepareTranstionStateInfoQueue = new Queue<TranstionStateInfo>();

    #region 生命周期
    public override void Dispose()
    {
        m_WorkId = 0u;

        m_ShareWorkBlockDataList = null;
        m_WorkBlockDataList.Clear();
        
        m_StartGroupIndex = -1;
        ClearBlockData();

        base.Dispose();
    }

    public void Update()
    {
        while (_prepareTranstionStateInfoQueue.Count > 0)
        {
            TranstionStateInfo transtionStateInfo = _prepareTranstionStateInfoQueue.Dequeue();
            if (transtionStateInfo == null)
                continue;

            DoState(transtionStateInfo.m_RunType, transtionStateInfo.m_OldWorkNodeData, transtionStateInfo.m_SelectType);

            transtionStateInfo.Push();
        }
    }
    #endregion

    public void ClearBlockData()
    {
        if (m_CurWorkNodeDataList.Count > 0)
        {
            m_CurWorkNodeDataList.Clear();
            m_CurUseEntity.ClearAllComponent_Repeat();
        }
        while (_prepareTranstionStateInfoQueue.Count > 0)
        {
            TranstionStateInfo transtionStateInfo = _prepareTranstionStateInfoQueue.Dequeue();
            if (transtionStateInfo == null)
                continue;
            
            transtionStateInfo.Push();
        }
        m_CurWorkBlockData = null;
    }

    public void InitWorkBlockDatas(uint workId, List<WorkBlockData> workBlockDatas)
    {
        m_WorkId = workId;

        m_ShareWorkBlockDataList = workBlockDatas;

        if (m_WorkBlockDataList == null)
            m_WorkBlockDataList = new List<WorkBlockData>();
        for (int i = 0, count = workBlockDatas.Count; i < count; i++)
        {
            m_WorkBlockDataList.Add(workBlockDatas[i]);
        }
    }

    public void UpdateWorkBlockDatas(List<WorkBlockData> workBlockDatas)
    {
        for (int i = 0, count = workBlockDatas.Count; i < count; i++)
        {
            m_WorkBlockDataList.Add(workBlockDatas[i]);
        }
    }

    public void UpdateShareWorkBlockDatas(List<WorkBlockData> workBlockDatas) { }

    public bool StartWorkStream(int blockIndex = 0, WorkBlockData workBlockData = null)
    {
        if (m_WorkBlockDataList == null || m_WorkBlockDataList.Count == 0)
        {
            m_CurUseEntity.StateMachineOver();
            return false;
        }

        WorkBlockData wbd = workBlockData == null ? m_WorkBlockDataList[blockIndex] : workBlockData;

        if (!StartBlock(wbd))
        {
            m_CurUseEntity.StateMachineOver();
            return false;
        }

        return true;
    }

    public bool StartWorkStream(int workBlockType)
    {
        if (m_WorkBlockDataList == null || m_WorkBlockDataList.Count == 0)
        {
            m_CurUseEntity.StateMachineOver();
            return false;
        }

        WorkBlockData wbd = null;
        for (int i = 0, count = m_WorkBlockDataList.Count; i < count; i++)
        {
            WorkBlockData workBlockData = m_WorkBlockDataList[i];
            if (workBlockData.CurWorkBlockType == workBlockType)
            {
                wbd = workBlockData;
                break;
            }
        }

        if (wbd == null)
        {
#if DEBUG_MODE
            StateControllerEntity sce = m_CurUseEntity.m_StateControllerEntity;

            if (sce != null)
            {
                if (sce is WS_CombatBehaveAIControllerEntity || sce is WS_CombatSceneAIControllerEntity)
                {
                    Lib.Core.DebugUtil.LogError($"CombatBehaveAI类型的WorkId:{m_WorkId}数据中没有workBlockType ：{workBlockType.ToString()}");
                }
                else if (sce is WS_UIModelShowControllerEntity)
                {
                    Lib.Core.DebugUtil.LogError($"UI界面模型展示类型的WorkId:{m_WorkId}数据中没有workBlockType ：{workBlockType.ToString()}");
                }
                else if (sce is WS_NPCControllerEntity)
                {
                    Lib.Core.DebugUtil.LogError($"NPC表演类型的WorkId:{m_WorkId}数据中没有workBlockType ：{workBlockType.ToString()}");
                }
                else if (sce is WS_TaskGoalControllerEntity)
                {
                    Lib.Core.DebugUtil.LogError($"TaskGoal类型的WorkId:{m_WorkId}数据中没有workBlockType ：{workBlockType.ToString()}");
                }
                else
                    Lib.Core.DebugUtil.LogError($"{sce.GetType()}类型的WorkId:{m_WorkId}数据中没有workBlockType ：{workBlockType.ToString()}");
            }
            else
                Lib.Core.DebugUtil.LogError($"WorkId:{m_WorkId}数据中没有workBlockType ：{workBlockType.ToString()}");
#endif

            return false;
        }

        if (!StartBlock(wbd))
        {
            m_CurUseEntity.StateMachineOver();
            return false;
        }

        return true;
    }

    /// <summary>
    /// 会获取一个或多个state
    /// runType=-1获取前一个状态，=0获取当前状态，=1获取下一个状态
    /// selectIndex是oldState需要转换到选择种类的states
    /// </summary>
    public override void GetStates(int runType, ushort oldNodeId, int selectType)
    {
        WorkNodeData oldWorkNodeData = null;
        if (oldNodeId > 0 && m_CurWorkNodeDataList.Count > 0)
        {
            for (int i = 0, count = m_CurWorkNodeDataList.Count; i < count; i++)
            {
                WorkNodeData wnd = m_CurWorkNodeDataList[i];
                if (wnd.Id == oldNodeId)
                {
                    oldWorkNodeData = wnd;

                    if (oldWorkNodeData.SkipWorkBlockType != 0)
                    {
                        if (!StartBlock(oldWorkNodeData.SkipWorkBlockType))
                            m_CurUseEntity.StateMachineOver();

                        return;
                    }

                    if (runType != 0)
                        m_CurWorkNodeDataList.RemoveAt(i);

                    break;
                }
            }
        }

        TranstionStateInfo transtionStateInfo = CombatObjectPool.Instance.Get<TranstionStateInfo>();
        transtionStateInfo.m_RunType = runType;
        transtionStateInfo.m_OldWorkNodeData = oldWorkNodeData;
        transtionStateInfo.m_SelectType = selectType;

        _prepareTranstionStateInfoQueue.Enqueue(transtionStateInfo);
    }

    /// <summary>
    /// runType=-1进行前一个节点，=0进行当前节点，=1进行下一个节点, =2进行兄弟节点
    /// </summary>
    private void DoState(int runType, WorkNodeData oldWorkNodeData, int selectType)
    {
        bool checkOver = true;

        if (oldWorkNodeData != null)
        {
            if (runType == -1)
            {
                if (RunState(oldWorkNodeData.Parent, oldWorkNodeData.Parent.Parent))
                    checkOver = false;
            }
            else if (runType == 0)
            {
                if (RunState(oldWorkNodeData, oldWorkNodeData.Parent, false))
                    checkOver = false;
            }
            else if (runType == 2)
            {
                if (RunBrotherState(oldWorkNodeData))
                    checkOver = false;
            }
            else
            {
                if (RunChildState(oldWorkNodeData, selectType) || RunBrotherState(oldWorkNodeData))
                    checkOver = false;
            }
        }

        if (checkOver && m_CurWorkNodeDataList.Count <= 0 && _prepareTranstionStateInfoQueue.Count <= 0)
        {
            if (m_CurWorkBlockData == null || m_CurWorkBlockData.CurWorkBlockType == m_CurWorkBlockData.NextBlockType || !StartBlock(m_CurWorkBlockData.NextBlockType))
                m_CurUseEntity.StateMachineOver();

            return;
        }
    }

    public override void SkipState(ushort oldNodeId, ushort newNodeId, int newWorkBlockType = 0)
    {
        if (oldNodeId > 0 && m_CurWorkNodeDataList.Count > 0)
        {
            for (int i = 0, count = m_CurWorkNodeDataList.Count; i < count; i++)
            {
                WorkNodeData wnd = m_CurWorkNodeDataList[i];
                if (wnd.Id == oldNodeId)
                {
                    m_CurWorkNodeDataList.RemoveAt(i);

                    break;
                }
            }
        }

        if (newNodeId > 0)
        {
            if (newWorkBlockType > 0)
            {
                for (int i = 0, count = m_WorkBlockDataList.Count; i < count; i++)
                {
                    WorkBlockData workBlockData = m_WorkBlockDataList[i];
                    if (workBlockData.CurWorkBlockType == newWorkBlockType)
                    {
                        if (workBlockData != m_CurWorkBlockData)
                        {
                            ClearBlockData();

                            m_CurWorkBlockData = workBlockData;
                        }
                        break;
                    }
                }
            }

            WorkNodeData workNodeData = GetWorkNodeData(m_CurWorkBlockData.TopWorkNodeData, newNodeId);
            if (workNodeData != null)
                RunState(workNodeData, workNodeData.Parent);
            else
            {
                Lib.Core.DebugUtil.LogError($"SkipState跳转oldNodeId:{oldNodeId.ToString()}   newNodeId:{newNodeId.ToString()}    newWorkBlockType:{newWorkBlockType.ToString()}没有数据，结束当前行为");
                m_CurUseEntity.StateMachineOver();
            }
        }
        else
        {
            Lib.Core.DebugUtil.LogError($"注意：WorkId: {m_WorkId.ToString()}的工作流中使用SkipState跳转节点时给定需要跳转的NodeId：{newNodeId.ToString()}");
            DoState(1, null, -1);
        }
    }

    /// <summary>
    /// skipType=1运行当前的skipWorkNodeData， skipType=2运行当前skipWorkNodeData的兄弟节点
    /// </summary>
    public void SkipStateByWorkNodeData(ushort oldNodeId, WorkNodeData skipWorkNodeData, int skipType = 1)
    {
        if (oldNodeId > 0 && m_CurWorkNodeDataList.Count > 0)
        {
            for (int i = 0, count = m_CurWorkNodeDataList.Count; i < count; i++)
            {
                WorkNodeData wnd = m_CurWorkNodeDataList[i];
                if (wnd.Id == oldNodeId)
                {
                    m_CurWorkNodeDataList.RemoveAt(i);

                    break;
                }
            }
        }

        if (skipWorkNodeData != null)
        {
            if (skipType == 1)
                RunState(skipWorkNodeData, skipWorkNodeData.Parent);
            else
                DoState(2, skipWorkNodeData, skipWorkNodeData.GroupIndex);
        }
        else
        {
            Lib.Core.DebugUtil.LogError($"WorkId : {m_WorkId}工作流执行SkipStateByWorkNodeData时跳转的skipWorkNodeData为null");
            DoState(1, null, -1);
        }
    }

    public override bool SkipBlock(int newBlockState)
    {
        return StartWorkStream(newBlockState);
    }

    private bool StartBlock(int blockType)
    {
        if (blockType == 0)
            return false;

        for (int i = 0, count = m_WorkBlockDataList.Count; i < count; i++)
        {
            WorkBlockData workBlockData = m_WorkBlockDataList[i];
            if (workBlockData.CurWorkBlockType == blockType)
            {
                return StartBlock(workBlockData);
            }
        }

        return false;
    }

    public bool StartBlock(WorkBlockData workBlockData)
    {
        ClearBlockData();

        m_CurWorkBlockData = workBlockData;

        bool isRun = RunState(m_CurWorkBlockData.TopWorkNodeData, null);
        m_StartGroupIndex = -1;

        return isRun;
    }

    public void OverCurBlock()
    {
        if (m_CurWorkBlockData == null || m_CurWorkBlockData.CurWorkBlockType == m_CurWorkBlockData.NextBlockType || !StartBlock(m_CurWorkBlockData.NextBlockType))
            m_CurUseEntity.StateMachineOver();
    }

    private bool RunState(WorkNodeData workNodeData, WorkNodeData parent, bool needAddToCurWorkNodeDataList = true)
    {
        if (workNodeData == null)
            return false;

        StateBaseComponent stateBaseComponent = m_CurUseEntity.GetStateComponent(workNodeData.NodeType);
        if (stateBaseComponent == null)
            return false;

        workNodeData.Parent = parent;
        if (needAddToCurWorkNodeDataList)
            m_CurWorkNodeDataList.Add(workNodeData);

#if DEBUG_MODE
        Log_Debug(workNodeData);
#endif

        stateBaseComponent.m_DataNodeId = workNodeData.Id;
        stateBaseComponent.Base_IsPause = false;
        m_CurUseEntity.m_StartStateAction?.Invoke(stateBaseComponent);
        stateBaseComponent.Init(workNodeData.NodeContent);

        return true;
    }

    private bool RunChildState(WorkNodeData workNodeData, int selectGroupIndex, int nodeListIndex = -1)
    {
        if (workNodeData.TransitionWorkGroupList == null || workNodeData.TransitionWorkGroupList.Count == 0)
            return false;

        bool isNotExistNode = true;
        for (int i = 0, twgCount = workNodeData.TransitionWorkGroupList.Count; i < twgCount; i++)
        {
            WorkGroupNodeData workGroupNodeData = workNodeData.TransitionWorkGroupList[i];

            if ((nodeListIndex < 0 && workGroupNodeData.GroupIndex == -1) || workGroupNodeData.GroupIndex == selectGroupIndex)
            {
                if (workGroupNodeData.NodeList == null || workGroupNodeData.NodeList.Count == 0)
                    continue;

                if (nodeListIndex < 0)
                {
                    //---------------------bug-----------------------------优化，编辑器赋值给数组，能很大减少循环次数
                    for (int j = 0, nlCount = workGroupNodeData.NodeList.Count; j < nlCount; j++)
                    {
                        WorkNodeData child = workGroupNodeData.NodeList[j];
                        if (j == 0 || child.IsConcurrent)
                        {
                            m_CurWorkNodeDataList.Add(child);
                        }
                    }

                    for (int j = 0, nlCount = workGroupNodeData.NodeList.Count; j < nlCount; j++)
                    {
                        WorkNodeData child = workGroupNodeData.NodeList[j];
                        if (j == 0 || child.IsConcurrent)
                        {
                            if (RunState(child, workNodeData, false))
                                isNotExistNode = false;
                            else
                                m_CurWorkNodeDataList.Remove(child);
                        }
                    }
                }
                else
                {
                    //下标超出，即组中节点运行完毕或者该节点为并行节点
                    if (nodeListIndex >= workGroupNodeData.NodeList.Count || workGroupNodeData.NodeList[nodeListIndex].IsConcurrent)
                    {
                        //必运行组中判断上个节点是否为总主线
                        if (selectGroupIndex == -1)
                        {
                            if (workGroupNodeData.NodeList[nodeListIndex - 1].IsMainLine)
                                return RunBrotherState(workNodeData);
                            else
                                return false;
                        }
                        else
                        {
                            //如果存在比运行组，那么该组部位总主线，否则判断上个节点是否为总主线
                            if (IsHaveGroup(workNodeData, -1))
                                return false;
                            else
                            {
                                if (workGroupNodeData.NodeList[nodeListIndex - 1].IsMainLine)
                                    return RunBrotherState(workNodeData);
                                else
                                    return false;
                            }
                        }
                    }
                    else
                    {
                        return RunState(workGroupNodeData.NodeList[nodeListIndex], workNodeData);
                    }
                }
            }
        }

        if (isNotExistNode)
            return false;

        return true;
    }

    private bool RunBrotherState(WorkNodeData workNodeData)
    {
        if (workNodeData.Parent == null)
            return false;

        return RunChildState(workNodeData.Parent, workNodeData.GroupIndex, workNodeData.InParentGroupNodeListIndex + 1);
    }

    private bool IsHaveGroup(WorkNodeData parent, int groupIndex)
    {
        for (int i = 0, count = parent.TransitionWorkGroupList.Count; i < count; i++)
        {
            WorkGroupNodeData workGroupNodeData = parent.TransitionWorkGroupList[i];

            if (workGroupNodeData.GroupIndex == groupIndex)
                return true;
        }

        return false;
    }

    private WorkNodeData GetWorkNodeData(WorkNodeData checkNode, ushort nodeId)
    {
        if (checkNode.TransitionWorkGroupList == null || checkNode.TransitionWorkGroupList.Count == 0)
            return null;

        for (int i = 0, twgCount = checkNode.TransitionWorkGroupList.Count; i < twgCount; i++)
        {
            WorkGroupNodeData workGroupNodeData = checkNode.TransitionWorkGroupList[i];

            if (workGroupNodeData.NodeList == null || workGroupNodeData.NodeList.Count == 0)
                continue;

            for (int j = 0, nlCount = workGroupNodeData.NodeList.Count; j < nlCount; j++)
            {
                WorkNodeData child = workGroupNodeData.NodeList[j];
                if (child.Id == nodeId)
                {
                    child.Parent = checkNode;
                    return child;
                }
                else
                {
                    WorkNodeData cc = GetWorkNodeData(child, nodeId);
                    if (cc != null)
                        return cc;
                }
            }
        }

        return null;
    }

    public WorkNodeData GetWorkNodeDataFromCurWorkNodeDataList(ushort nodeId)
    {
        for (int i = 0, count = m_CurWorkNodeDataList.Count; i < count; i++)
        {
            WorkNodeData wnd = m_CurWorkNodeDataList[i];
            if (wnd.Id == nodeId)
            {
                return wnd;
            }
        }

        return null;
    }

    public static string GetWorkNodeContentByType(List<WorkBlockData> workBlockDatas, int blockType, int nodeType)
    {
        if (workBlockDatas != null && workBlockDatas.Count > 0)
        {
            for (int i = 0, count = workBlockDatas.Count; i < count; i++)
            {
                WorkBlockData workBlockData = workBlockDatas[i];
                if (workBlockData == null || workBlockData.CurWorkBlockType != blockType ||
                    workBlockData.TopWorkNodeData == null)
                    continue;

                if (workBlockData.TopWorkNodeData.TransitionWorkGroupList != null &&
                    workBlockData.TopWorkNodeData.TransitionWorkGroupList.Count > 0)
                {
                    for (int groupIndex = 0, groupCount = workBlockData.TopWorkNodeData.TransitionWorkGroupList.Count; groupIndex < groupCount; groupIndex++)
                    {
                        WorkGroupNodeData workGroupNodeData = workBlockData.TopWorkNodeData.TransitionWorkGroupList[groupIndex];
                        if (workGroupNodeData == null || workGroupNodeData.NodeList == null ||
                            workGroupNodeData.NodeList.Count <= 0)
                            continue;

                        for (int nodeIndex = 0, nodeCount = workGroupNodeData.NodeList.Count; nodeIndex < nodeCount; nodeIndex++)
                        {
                            WorkNodeData workNodeData = workGroupNodeData.NodeList[nodeIndex];
                            if (workNodeData == null)
                                continue;

                            if (workNodeData.NodeType == nodeType)
                                return workNodeData.NodeContent;
                        }
                    }
                }
            }
        }

        return null;
    }

#if DEBUG_MODE
    private void Log_Debug(WorkNodeData workNodeData)
    {
        StateControllerEntity sce = m_CurUseEntity.m_StateControllerEntity;

        if (sce is WS_CombatBehaveAIControllerEntity ce)
        {
            string enumDes = GetEnumDes(typeof(CombatBehaveAIEnum), workNodeData.NodeType, ((CombatBehaveAIEnum)workNodeData.NodeType).ToString());
            DLogManager.Log(Lib.Core.ELogType.eCombatBehave, $"{(((MobEntity)ce.m_WorkStreamManagerEntity.Parent) == null || ((MobEntity)ce.m_WorkStreamManagerEntity.Parent).m_Go == null ? null : ((MobEntity)ce.m_WorkStreamManagerEntity.Parent).m_Go.name)}-----{m_CurUseEntity.m_StateControllerEntity.Id.ToString()}-----WorkId:{m_WorkId}-----{enumDes}------{workNodeData.NodeContent}");
            return;
        }
        if (sce is WS_CombatSceneAIControllerEntity cse)
        {
            string enumDes = GetEnumDes(typeof(CombatBehaveAIEnum), workNodeData.NodeType, ((CombatBehaveAIEnum)workNodeData.NodeType).ToString());
            DLogManager.Log(Lib.Core.ELogType.eCombatBehave, $"{(((MobEntity)cse.m_WorkStreamManagerEntity.Parent) == null || ((MobEntity)cse.m_WorkStreamManagerEntity.Parent).m_Go == null ? null : ((MobEntity)cse.m_WorkStreamManagerEntity.Parent).m_Go.name)}-----WorkId:{m_WorkId}-----{enumDes}------{workNodeData.NodeContent}");
            return;
        }
        if (sce is WS_UIModelShowControllerEntity umsce)
        {
            string enumDes = GetEnumDes(typeof(UIModelShowEnum), workNodeData.NodeType, ((UIModelShowEnum)workNodeData.NodeType).ToString());
            DLogManager.Log(Lib.Core.ELogType.eUIModelShowWorkStream, $"WorkId:{m_WorkId}-----{enumDes}------{workNodeData.NodeContent}");
            return;
        }
        if (sce is WS_CommunalAIControllerEntity cace)
        {
            string enumDes = GetEnumDes(typeof(CommunalAIEnum), workNodeData.NodeType, ((CommunalAIEnum)workNodeData.NodeType).ToString());
            DLogManager.Log(Lib.Core.ELogType.eCommunalAIWorkStream, $"WorkId:{m_WorkId}-----{enumDes}------{workNodeData.NodeContent}");
            return;
        }
        if (sce is WS_NPCControllerEntity)
        {
            string enumDes = GetEnumDes(typeof(NPCEnum), workNodeData.NodeType, ((NPCEnum)workNodeData.NodeType).ToString());
            DLogManager.Log(Lib.Core.ELogType.eWorkStream, $"{sce.GetType()}<color=yellow>[{sce.Id}]</color>----WorkId:{m_WorkId}-----{enumDes}------{workNodeData.NodeContent}");
            return;
        }
        if (sce is WS_TaskGoalControllerEntity)
        {
            string enumDes = GetEnumDes(typeof(TaskGoalEnum), workNodeData.NodeType, ((TaskGoalEnum)workNodeData.NodeType).ToString());
            DLogManager.Log(Lib.Core.ELogType.eWorkStream, $"{sce.GetType()}<color=yellow>[{sce.Id}]</color>----WorkId:{m_WorkId}-----{enumDes}------{workNodeData.NodeContent}");
            return;
        }

        Lib.Core.DebugUtil.Log(Lib.Core.ELogType.eWorkStream, $"{sce.GetType()}<color=yellow>[{sce.Id}]</color>----WorkId:{m_WorkId}-----{workNodeData.NodeType.ToString()}------{workNodeData.NodeContent}");
    }

    private class WSEnumTypeEditorInfo
    {
        public System.Type EnumType;
        public List<WSEnumDesEditorInfo> DesInfoList;
    }
    private class WSEnumDesEditorInfo
    {
        public int NodeType;
        public string NodeDes;
    }
    private static List<WSEnumTypeEditorInfo> _workStreamEnumDesList = new List<WSEnumTypeEditorInfo>();
    private List<WSEnumDesEditorInfo> SetEnumTypeInfo(System.Type enumType)
    {
        for (int i = 0, count = _workStreamEnumDesList.Count; i < count; i++)
        {
            WSEnumTypeEditorInfo wsEnumTypeEditorInfo = _workStreamEnumDesList[i];
            if (wsEnumTypeEditorInfo == null || wsEnumTypeEditorInfo.EnumType != enumType)
                continue;

            if (wsEnumTypeEditorInfo.DesInfoList != null)
                return wsEnumTypeEditorInfo.DesInfoList;
        }

        WSEnumTypeEditorInfo wsetdei = new WSEnumTypeEditorInfo();
        wsetdei.EnumType = enumType;
        wsetdei.DesInfoList = new List<WSEnumDesEditorInfo>();

        _workStreamEnumDesList.Add(wsetdei);

        foreach (var item in System.Enum.GetValues(enumType))
        {
            string enumName = item.ToString();
            int enumInt = (int)item;
            if (enumInt == 0)
                continue;

            System.Reflection.FieldInfo fieldInfo = enumType.GetField(enumName);
            if (fieldInfo == null)
                continue;

            object[] attrs = fieldInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attrs == null || attrs.Length <= 0)
                continue;

            string enumDes = ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;

            WSEnumDesEditorInfo wSEnumDesEditorInfo = new WSEnumDesEditorInfo();
            wSEnumDesEditorInfo.NodeType = enumInt;
            wSEnumDesEditorInfo.NodeDes = enumDes;

            wsetdei.DesInfoList.Add(wSEnumDesEditorInfo);
        }

        return wsetdei.DesInfoList;
    }

    private string GetEnumDes(System.Type enumType, int nodeType, string nodeTypeStr)
    {
#if !ILRUNTIME_MODE
        string des = System.Enum.ToObject(enumType, nodeType)?.ToString();

        List<WSEnumDesEditorInfo> enumList = SetEnumTypeInfo(enumType);
        
        for (int i = 0, count = enumList.Count; i < count; i++)
        {
            WSEnumDesEditorInfo wSEnumDesEditorInfo = enumList[i];
            if (wSEnumDesEditorInfo != null && wSEnumDesEditorInfo.NodeType == nodeType)
            {
                des += $"【{wSEnumDesEditorInfo.NodeDes}】";

                break;
            }
        }

        return des;
#else
        return nodeTypeStr;
#endif
    }
#endif
    }

    public class LoopNodeData<T> : object
{
    public List<T> TList;
    public WorkNodeData LoopWorkNode;
    public int LoopCount;
    public int LoopIndex;

    private bool _isAlreadyLoopInit;

    public void Push()
    {
        TList = null;
        LoopWorkNode = null;
        _isAlreadyLoopInit = false;
        LoopCount = 0;
        LoopIndex = 0;

        CombatObjectPool.Instance.Push(this);
    }

    public void StartLoop(List<T> list, WorkNodeData loopWorkNode)
    {
        LoopWorkNode = loopWorkNode;

        if (!_isAlreadyLoopInit)
        {
            TList = list;
            _isAlreadyLoopInit = true;
            LoopCount = list.Count;
            LoopIndex = 0;
        }
    }

    public T Get()
    {
        if (TList == null)
            return default;

        if (LoopIndex < LoopCount)
            return TList[LoopIndex];

        return default;
    }

    public bool EndLoop()
    {
        ++LoopIndex;
        if (LoopIndex < LoopCount)
        {
            return true;
        }
        else
        {
            Push();

            return false;
        }
    }
}
