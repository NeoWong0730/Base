using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using Rotorz.Games.Collections;
using System.Text;
using System;
using System.IO;
using System.Reflection;

public class TextReject : EditorWindow
{
    [MenuItem("__Tools__/分析Text")]
    public static void OpenWindow()
    {
        TextReject textReject = EditorWindow.GetWindow<TextReject>("分析Text");
    }

    private GameObject gameObject;
    private List<Text> texts = new List<Text>();
    private List<GameObject> selectGameObjects = new List<GameObject>();
    private Vector2 _pos = Vector2.zero;


    private void OnGUI()
    {
        gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), false);

        if (GUILayout.Button("剔除 RaycastTarget"))
        {
            if (gameObject == null)
            {
                EditorUtility.DisplayDialog("", "请选择需要操作的对象", "确定");
            }
            else
            {
                texts.Clear();
                gameObject.GetComponentsInChildren<Text>(true, texts);
                foreach (var item in texts)
                {
                    item.raycastTarget = false;
                }
                EditorUtility.SetDirty(gameObject);
                AssetDatabase.SaveAssets();
            }
        }
        EditorGUILayout.LabelField("select count", selectGameObjects.Count.ToString());
        if (Selection.gameObjects.Length>0)
        {
            _pos = EditorGUILayout.BeginScrollView(_pos);
            selectGameObjects = new List<GameObject>(Selection.gameObjects);
            for (int i = 0; i < selectGameObjects.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(selectGameObjects[i], typeof(GameObject), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        else
        {
            selectGameObjects.Clear();
        }
        if (GUILayout.Button("批量剔除 RaycastTarget"))
        {
            texts.Clear();
            if (selectGameObjects.Count == 0)
            {
                EditorUtility.DisplayDialog("", "请选择需要操作的对象", "确定");
            }
            else
            {
                foreach (var item in selectGameObjects)
                {
                    Text[] _texts = item.GetComponentsInChildren<Text>(true);
                    texts.AddRange(_texts);
                    foreach (var _item in texts)
                    {
                        _item.raycastTarget = false;
                        EditorUtility.SetDirty(item);
                    }
                }
                AssetDatabase.SaveAssets();
            }
        }

        if (GUILayout.Button("批量清理Alpha==0的Cull")) {
            foreach (var item in Selection.gameObjects) {
                Graphic[] gs = item.GetComponentsInChildren<Graphic>(true);
                foreach (var g in gs) {
                    if (g.color.a == 1f / 255f || g.color.a == 0f) {
                        CanvasRenderer cr = g.GetComponent<CanvasRenderer>();
                        cr.cullTransparentMesh = true;
                    }
                }

                EditorUtility.SetDirty(item);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
