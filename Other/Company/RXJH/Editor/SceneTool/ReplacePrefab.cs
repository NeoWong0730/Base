using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class ReplacePrefab : EditorWindow
{
    [MenuItem("__Tools__/替换预制体")]
    public static void OpenWindow()
    {
        ReplacePrefab autoPrefab = EditorWindow.GetWindow<ReplacePrefab>("替换预制体");
    }

    private GameObject gameObject;

    private void OnGUI()
    {
        gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), false);

        if (GUILayout.Button("替换"))
        {
            string path = AssetDatabase.GetAssetPath(gameObject);

            List<GameObject> gos = new List<GameObject>(Selection.gameObjects);
            foreach (GameObject go in gos)
            {                
                Transform parent = go.transform.parent;
                GameObject prefab = PrefabUtility.InstantiatePrefab(gameObject, parent) as GameObject;

                prefab.transform.localRotation = go.transform.localRotation;
                prefab.transform.localPosition = go.transform.localPosition;
                prefab.transform.localScale = go.transform.localScale;
                prefab.name = gameObject.name;                

                DestroyImmediate(go);
                EditorUtility.SetDirty(prefab);
            }            
        }
    }

    private void Gen()
    {
        
    }
}
