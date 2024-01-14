using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;

public class ModleTool
{
    [MenuItem("Tools/模型平均法线写入切线数据")]
    public static void WirteAverageNormalToTangentToos()
    {
        MeshFilter[] meshFilters = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {
            Mesh mesh = meshFilter.sharedMesh;
            WirteAverageNormalToTangent(mesh);
        }

        SkinnedMeshRenderer[] skinMeshRenders = Selection.activeGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var skinMeshRender in skinMeshRenders)
        {
            Mesh mesh = skinMeshRender.sharedMesh;
            WirteAverageNormalToTangent(mesh);
        }
    }

    private static void WirteAverageNormalToTangent(Mesh mesh)
    {                
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector4[] tangents = mesh.tangents;
        Color[] colors = mesh.colors;

        var averageNormalHash = new Dictionary<Vector3, Vector3>();
        for (var j = 0; j < mesh.vertexCount; j++)
        {
            if (!averageNormalHash.ContainsKey(vertices[j]))
            {
                averageNormalHash.Add(vertices[j], normals[j]);
            }
            else
            {
                averageNormalHash[vertices[j]] =
                    (averageNormalHash[vertices[j]] + normals[j]).normalized;
            }
        }

        Color[] averageNormals = new Color[mesh.vertexCount];
        for (var j = 0; j < mesh.vertexCount; j++)
        {            
            float3 averageNormal = averageNormalHash[vertices[j]];

            /*float3 normal = normals[j];
            float3 tangent = new float3(tangents[j].x, tangents[j].y, tangents[j].z);
            float3 bitangent = math.cross(normal, tangent);

            float3x3 tangentToObject = new float3x3(tangent, bitangent, normal);
            float3x3 objectToTangent = math.inverse(tangentToObject);

            averageNormal = math.mul(averageNormal, objectToTangent);
            */
            if (colors.Length == 0)
            {
                averageNormals[j] = new Color(averageNormal.x, averageNormal.y, averageNormal.z, 1);
            }
            else
            {
                averageNormals[j] = new Color(averageNormal.x, averageNormal.y, averageNormal.z, colors[j].a);                
            }
        }

        //var tangents = new Vector4[mesh.vertexCount];
        //for (var j = 0; j < mesh.vertexCount; j++)
        //{
            //tangents[j] = new Vector4(averageNormals[j].x, averageNormals[j].y, averageNormals[j].z, 0);
        //}

        mesh.colors = averageNormals;
    }
}