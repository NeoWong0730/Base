using UnityEngine;

[System.Serializable]
public class GameObjectData
{
    public int prefabPathIndex;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

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

public class SceneGameObjectData : ScriptableObject
{
    /// <summary>
    /// 所有资源的路径
    /// </summary>
    public string[] prefabPaths;

    /// <summary>
    /// 根据lod确定数组长度
    /// </summary>
    public BlockTree[] gameObjectIndexTree;

    /// <summary>
    /// 所有的tramsformDatas
    /// </summary>
    public GameObjectData[] gameObjectTramsforms;
}