using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public sealed class MeshBakeUtil
{
    public static Mesh BakeMeshToParentBones(SkinnedMeshRenderer source, HashSet<Transform> modifyBones)
    {
        Mesh target = Object.Instantiate(source.sharedMesh);
        int vertexCount = source.sharedMesh.vertexCount;
        int blendShapeCount = source.sharedMesh.blendShapeCount;

        Bounds bounds = source.sharedMesh.bounds;

        BoneWeight[] boneWeights = source.sharedMesh.boneWeights;
        Vector3[] vertices = source.sharedMesh.vertices;
        Vector3[] normals = source.sharedMesh.normals;
        Vector4[] tangents = source.sharedMesh.tangents;

        BoneWeight[] newBoneWeights = new BoneWeight[vertexCount];
        Vector3[] newVertices = new Vector3[vertexCount];
        Vector3[] newNormals = new Vector3[vertexCount];
        Vector4[] newTangents = new Vector4[vertexCount];
        Matrix4x4[] bindposes = source.sharedMesh.bindposes;

        //Matrix4x4[] bindposes = source.sharedMesh.bindposes;
        Transform[] bones = source.bones;
        int boneCount = bones.Length;
        Matrix4x4[] boneMatrixs = new Matrix4x4[boneCount];

        Dictionary<Transform, int> parentBoneDic = new Dictionary<Transform, int>();
        List<Transform> parentBones = new List<Transform>();       

        for (int i = 0; i < boneCount; i++)
        {
            boneMatrixs[i] = source.rootBone.worldToLocalMatrix * bones[i].localToWorldMatrix * bindposes[i];
            if (!modifyBones.Contains(bones[i]))
            {
                if (!parentBoneDic.ContainsKey(bones[i]))
                {
                    parentBoneDic.Add(bones[i], parentBones.Count);
                    parentBones.Add(bones[i]);
                }
            }
        }
        for (int i = 0; i < vertexCount; i++)
        {
            ApplyBoneMatrix(boneWeights[i], boneMatrixs, vertices[i], normals[i], tangents[i], out newVertices[i], out newNormals[i], out newTangents[i]);
            newBoneWeights[i] = ApplyBoneWeight(boneWeights[i], modifyBones, bones, parentBoneDic);
        }

        target.vertices = newVertices;
        target.normals = newNormals;
        target.tangents = newTangents;
        target.bounds = bounds;
        target.bindposes = bindposes;
        target.boneWeights = newBoneWeights;

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

        return target;
    }

    private static int IndexOfInt4(int4 int4, int v)
    {
        int index = -1;
        for(int i = 0; i < 4; ++i)
        {
            if(int4[i] == v)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    private static BoneWeight ApplyBoneWeight(BoneWeight bw, HashSet<Transform> modifyBones, Transform[] bones, Dictionary<Transform, int> parentBoneDic)
    {
        float4 weights = new float4(bw.weight0, bw.weight1, bw.weight2, bw.weight3);
        int4 boneIndexs = new int4(bw.boneIndex0, bw.boneIndex1, bw.boneIndex2, bw.boneIndex3);

        float4 newWeights = float4.zero;
        int4 newBoneIndexs = int4.zero;
        int count = 0;

        for (int i = 0; i < 4; ++i)
        {
            float weight = weights[i];
            int boneIndex = boneIndexs[i];

            if (weight > 0)
            {
                Transform bone = bones[boneIndex];
                if (modifyBones.Contains(bone))
                {
                    bone = bone.parent;
                }

                if (!parentBoneDic.TryGetValue(bone, out int newBoneIndex))
                {
                    int index = IndexOfInt4(newBoneIndexs, newBoneIndex);
                    if (index < 0)
                    {
                        newBoneIndexs[count] = newBoneIndex;
                        index = count;
                        ++count;
                    }
                    newWeights[index] += weight;
                }
                else
                {
                    //Debug
                }
            }
        }

        BoneWeight boneWeight = new BoneWeight
        {
            boneIndex0 = newBoneIndexs[0],
            boneIndex1 = newBoneIndexs[1],
            boneIndex2 = newBoneIndexs[2],
            boneIndex3 = newBoneIndexs[3],
            weight0 = newWeights[0],
            weight1 = newWeights[1],
            weight2 = newWeights[2],
            weight3 = newWeights[3],
        };

        return boneWeight;
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
