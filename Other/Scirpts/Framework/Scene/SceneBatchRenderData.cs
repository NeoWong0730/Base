using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SceneBatchRenderData : ScriptableObject
{
    [SerializeField]
    public RenderData[] mRenderDatas;
    [SerializeField]
    public RenderData[] mMatrix4X4s;
}

[System.Serializable]
public struct RenderData : System.IEquatable<RenderData>
{
    public Mesh mesh;
    public Material material;
    public int subMeshIndex;
    public int layer;
    public ShadowCastingMode shadowCastingMode;
    public bool receiveShadows;
    public Bounds bounds;
    public int lod;
    public int instanceCount;

    public bool Equals(RenderData other)
    {
        return
            mesh == other.mesh &&
            material == other.material &&
            subMeshIndex == other.subMeshIndex &&
            layer == other.layer &&
            shadowCastingMode == other.shadowCastingMode &&
            receiveShadows == other.receiveShadows;
    }
}