using UnityEngine;

public class SceneInstanceData : ScriptableObject
{
    /// <summary>
    /// 所有instance的矩阵
    /// </summary>
    public Matrix4x4[] instanceMatrices;

    /// <summary>
    /// 根据物体种类来确定数组长度
    /// </summary>
    public BlockTree[] instanceIndexTree;

    /// <summary>
    /// mesh的路径
    /// </summary>
    public Mesh[] meshPath;

    /// <summary>
    /// 材质的路径
    /// </summary>
    public Material[] materialPath;

    /// <summary>
    /// lod的层级
    /// </summary>
    public int[] lod;

    /// <summary>
    /// 风吹最大弯曲的角度
    /// </summary>
    public float[] windingMaxs;

    /// <summary>
    /// 碰撞最大弯曲的角度
    /// </summary>
    public float[] collideWindingMaxs;
}