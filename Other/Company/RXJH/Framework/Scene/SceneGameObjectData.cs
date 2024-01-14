using UnityEngine;

/// <summary>
/// 数据树状结构, 用于加快 SceneBlockDatad 的检查, 里面不包含空的节点， 离线数据
/// </summary>
[System.Serializable]
public struct BlockNode
{
    public int TreeIndex;
    /// <summary>
    /// 对应的cull块的index
    /// </summary>
    public int boundIndex;

    /// <summary>
    /// 对应的子节点的起始
    /// </summary>
    public int childStart;
    public int childEnd;

    /// <summary>
    /// 数据的起始
    /// </summary>
    public int dataStart;
    public int dataEnd;
}

[System.Serializable]
public class BlockTree
{
    public int rootCount;
    public BlockNode[] nodes;
}