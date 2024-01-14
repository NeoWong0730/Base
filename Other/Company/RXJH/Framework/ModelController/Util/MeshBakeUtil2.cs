using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MeshBakeUtil2
{
    public static Mesh BakeMesh(SkinnedMeshRenderer source, HashSet<Transform> boneFilter = null)
    {        
        Mesh target = Object.Instantiate(source.sharedMesh);
        int vertexCount = source.sharedMesh.vertexCount;
        int blendShapeCount = source.sharedMesh.blendShapeCount;

        Bounds bounds = source.sharedMesh.bounds;
        BoneWeight[] boneWeights = source.sharedMesh.boneWeights;
        Vector3[] vertices = source.sharedMesh.vertices;
        Vector3[] normals = source.sharedMesh.normals;
        Vector4[] tangents = source.sharedMesh.tangents;
        Vector3[] newVertices = new Vector3[vertexCount];
        Vector3[] newNormals = new Vector3[vertexCount];
        Vector4[] newTangents = new Vector4[vertexCount];
        Matrix4x4[] bindposes = source.sharedMesh.bindposes;
        Transform[] bones = source.bones;
        int boneCount = bones.Length;

        Dictionary<int, Matrix4x4> cacheMatrix = null;
        if (boneFilter != null)
        {
            //缓存Transform数据
            cacheMatrix = new Dictionary<int, Matrix4x4>();
            for (int i = 0; i < boneCount; i++)
            {
                cacheMatrix[i] = bones[i].localToWorldMatrix;
            }
            //将不烘焙的Bone重置回原点
            ResetBones(source, boneFilter);
        }
        
        //BakeMesh
        Matrix4x4[] boneMatrixs = new Matrix4x4[boneCount];
        for (int i = 0; i < boneCount; i++)
        {
            boneMatrixs[i] = source.rootBone.worldToLocalMatrix * bones[i].localToWorldMatrix * bindposes[i];
        }
        for (int i = 0; i < vertexCount; i++)
        {
            ApplyBoneMatrix(boneWeights[i], boneMatrixs, vertices[i], normals[i], tangents[i], out newVertices[i], out newNormals[i], out newTangents[i]);
        }
        target.vertices = newVertices;
        target.normals = newNormals;
        target.tangents = newTangents;
        target.bounds = bounds;

        //删除已经烘焙的boneWeight
        if (boneFilter != null)
        {
            HashSet<int> boneIndexFilter = null;
            boneIndexFilter = new HashSet<int>();
            for (int i = 0; i < boneCount; i++)
            {
                if (boneFilter.Contains(bones[i]))
                    boneIndexFilter.Add(i);
            }
            for (int i = 0; i < vertexCount; i++)
            {
                BoneWeight bw = boneWeights[i];
                bw.boneIndex0 = boneIndexFilter.Contains(bw.boneIndex0) ? 0 : bw.boneIndex0;
                bw.boneIndex1 = boneIndexFilter.Contains(bw.boneIndex1) ? 0 : bw.boneIndex1;
                bw.boneIndex2 = boneIndexFilter.Contains(bw.boneIndex2) ? 0 : bw.boneIndex2;
                bw.boneIndex3 = boneIndexFilter.Contains(bw.boneIndex3) ? 0 : bw.boneIndex3;
                boneWeights[i] = bw;
            }
            target.boneWeights = boneWeights;
        }
        else
        {
            target.boneWeights = null;
        }

        //修正BlendShape
        if (blendShapeCount > 0)
        {
            target.ClearBlendShapes();
            for (int i = 0; i < blendShapeCount; i++)
            {
                string name = source.sharedMesh.GetBlendShapeName(i);
                int frameCount = source.sharedMesh.GetBlendShapeFrameCount(i);
                Vector3[] deltaVertices = new Vector3[vertexCount];
                Vector3[] deltaNormals = new Vector3[vertexCount];
                Vector3[] deltaTangents = new Vector3[vertexCount];
                for (int j = 0; j < frameCount; j++)
                {
                    source.sharedMesh.GetBlendShapeFrameVertices(i, j, deltaVertices, deltaNormals, deltaTangents);
                    for (int r = 0; r < vertexCount; r++)
                    {
                        Vector3 shapeVector;
                        Vector3 shapeNormal;
                        Vector4 shapeTangent;
                        ApplyBoneMatrix(boneWeights[i], boneMatrixs, vertices[i] + deltaVertices[i], normals[i] + deltaNormals[i], tangents[i] + (Vector4)deltaTangents[i], out shapeVector, out shapeNormal, out shapeTangent);
                        deltaVertices[i] = shapeVector - newVertices[i];
                        deltaNormals[i] = shapeNormal - newNormals[i];
                        deltaTangents[i] = shapeTangent - newTangents[i];
                    }
                    float weight = source.sharedMesh.GetBlendShapeFrameWeight(i, j);
                    target.AddBlendShapeFrame(name, weight, deltaVertices, deltaNormals, deltaTangents);
                }
            }
        }

        //恢复Bone数据
        if (cacheMatrix != null)
        {
            for (int i = 0; i < boneCount; i++)
            {
                SetTransformMatrix(bones[i], cacheMatrix[i]);
            }
        }

        return target;
    }

    //蒙皮
    private static void ApplyBoneMatrix(BoneWeight bw, Matrix4x4[] boneMatrixs, Vector3 vector, Vector3 normal, Vector4 tangent, out Vector3 newVector, out Vector3 newNormal, out Vector4 newTangent)
    {
        Vector3 resultVector = new Vector3();
        Vector3 resultNormal = new Vector3();
        Vector3 resultTangent = new Vector3();
        if (bw.weight0 > 0)
        {
            resultVector += boneMatrixs[bw.boneIndex0].MultiplyPoint3x4(vector) * bw.weight0;
            resultNormal += boneMatrixs[bw.boneIndex0].MultiplyVector(normal) * bw.weight0;
            resultTangent += boneMatrixs[bw.boneIndex0].MultiplyVector(tangent) * bw.weight0;
        }
        if (bw.weight1 > 0)
        {
            resultVector += boneMatrixs[bw.boneIndex1].MultiplyPoint3x4(vector) * bw.weight1;
            resultNormal += boneMatrixs[bw.boneIndex1].MultiplyVector(normal) * bw.weight1;
            resultTangent += boneMatrixs[bw.boneIndex1].MultiplyVector(tangent) * bw.weight1;
        }
        if (bw.weight2 > 0)
        {
            resultVector += boneMatrixs[bw.boneIndex2].MultiplyPoint3x4(vector) * bw.weight2;
            resultNormal += boneMatrixs[bw.boneIndex2].MultiplyVector(normal) * bw.weight2;
            resultTangent += boneMatrixs[bw.boneIndex2].MultiplyVector(tangent) * bw.weight2;
        }
        if (bw.weight3 > 0)
        {
            resultVector += boneMatrixs[bw.boneIndex3].MultiplyPoint3x4(vector) * bw.weight3;
            resultNormal += boneMatrixs[bw.boneIndex3].MultiplyVector(normal) * bw.weight3;
            resultTangent += boneMatrixs[bw.boneIndex3].MultiplyVector(tangent) * bw.weight3;
        }
        newVector = resultVector;
        newNormal = resultNormal;
        newTangent = new Vector4(resultTangent.x, resultTangent.y, resultTangent.z, tangent.w);
    }

    //通过Matrix4x4设置Transform数据
    private static void SetTransformMatrix(Transform trans, Matrix4x4 m)
    {
        trans.position = new Vector3(m.m03, m.m13, m.m23);
        trans.rotation = m.rotation;
        Vector3 curScale = m.lossyScale;
        Vector3 parentScale = trans.parent.lossyScale;
        trans.localScale = new Vector3(curScale.x / parentScale.x, curScale.y / parentScale.y, curScale.z / parentScale.z);
    }

    //重置骨骼
    public static void ResetBones(SkinnedMeshRenderer smr, HashSet<Transform> exclude = null)
    {
        var bones = smr.bones;
        var bindposes = smr.sharedMesh.bindposes;
        int boneCount = bones.Length;
        Dictionary<Transform, Matrix4x4> bindposesDict = new Dictionary<Transform, Matrix4x4>();
        for (int i = 0; i < boneCount; i++)
        {
            bindposesDict.Add(bones[i], bindposes[i]);
        }
        for (int i = 0; i < boneCount; i++)
        {
            Transform bone = bones[i];
            if ((exclude == null || !exclude.Contains(bone)))
            {
                Matrix4x4 m = smr.rootBone.worldToLocalMatrix * bindposes[i].inverse;
                SetTransformMatrix(bone, m);
            }
        }
    }
}
