using System.Collections.Generic;

public class WorkStreamData
{
    [FileDataOperation(0, 0)]
    public List<WorkBlockData> AttackWorkBlockDatas;
    [FileDataOperation(0, 1)]
    public List<WorkBlockData> TargetWorkBlockDatas;
}

public class WorkBlockData
{
    [FileDataOperation(0, 0)]
    public int CurWorkBlockType;
    /// <summary>
    /// =0施放者；=1被击者（标注：类型只能有255种）
    /// </summary>
    [FileDataOperation(0, 1)]
    public byte AttachType;
    [FileDataOperation(0, 2)]
    public int NextBlockType;
    [FileDataOperation(0, 3)]
    public WorkNodeData TopWorkNodeData;
}

/// <summary>
/// 同一层级可以存在一组或者多组节点
/// </summary>
public class WorkNodeData
{
    [FileDataOperation(0, 0)]
    public ushort Id;
    /// <summary>
    /// 此node在父节点下所在组的NodeList的下标
    /// </summary>
    [FileDataOperation(0, 1)]
    public short InParentGroupNodeListIndex;
    /// <summary>
    /// =-1默认一定执行，>0选择组进行执行（标注：最大组数为127）
    /// </summary>
    [FileDataOperation(0, 2)]
    public sbyte GroupIndex;
    /// <summary>
    /// 同一组中标记是否为主线；
    /// 不同组中，选取一条主线作为总主线返回父节点进行运行，选取规则如下：
    /// 如果有组为-1的，那么只能是该组中主线做为总主线，
    /// 如果没有组为-1，那么就把选取的组中的主线作为总主线
    /// </summary>
    [FileDataOperation(0, 3)]
    public bool IsMainLine;
    /// <summary>
    /// 在同一组中是否并发执行；
    /// 串行的话执行同一组中下一个不是并发的节点，如果是并发的节点，该串行结束
    /// </summary>
    [FileDataOperation(0, 4)]
    public bool IsConcurrent;
    [FileDataOperation(0, 5)]
    public int NodeType;
    [FileDataOperation(0, 6)]
    public string NodeContent;
    [FileDataOperation(0, 7)]
    public sbyte LayerIndex;

    public WorkNodeData Parent;

    [FileDataOperation(0, 8)]
    public List<WorkGroupNodeData> TransitionWorkGroupList;
    [FileDataOperation(0, 9)]
    public int SkipWorkBlockType;
}

public class WorkGroupNodeData
{
    /// <summary>
    /// =-1默认一定执行，>0选择组进行执行（标注：最大组数为127）
    /// </summary>
    [FileDataOperation(0, 0)]
    public sbyte GroupIndex;
    [FileDataOperation(0, 1)]
    public List<WorkNodeData> NodeList;
}
