using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class MapTool : EditorWindow
{
    [MenuItem("_Map_/ExportNavMesh")]
    public static void ExportNavMesh()
    {
        Debug.Log("ExportNavMesh Start");

        NavMeshTriangulation meshData = NavMesh.CalculateTriangulation();

        string tmpPath = Application.dataPath + "/" + "test" + ".obj";
        StreamWriter tmpWriter = new StreamWriter(tmpPath);

        //vertices
        for (int i = 0; i < meshData.vertices.Length; ++i)
        {
            tmpWriter.WriteLine("v " + meshData.vertices[i].x + " " + meshData.vertices[i].y + " " + meshData.vertices[i].z);
        }

        tmpWriter.WriteLine("test");

        //indices
        for (int i = 0; i < meshData.indices.Length;)
        {
            tmpWriter.WriteLine("f " + (meshData.indices[i] + 1) + " " + (meshData.indices[i+1] + 1) + " " + (meshData.indices[i+2] + 1));
            i += 3;
        }

        tmpWriter.Flush();
        tmpWriter.Close();

        Debug.Log("ExportNavMesh Success");
    }
}
