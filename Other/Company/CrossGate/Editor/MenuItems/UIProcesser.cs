using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UIProcesser : EditorWindow
{
    private void OnGUI()
    {
        if (GUILayout.Button("Add ButtonAudio to Button [选中]"))
        {
            GameObject[] gos = Selection.gameObjects;
            bool entered = gos.Length > 0;
            foreach (var go in gos)
            {
                Button[] btns = go.GetComponentsInChildren<Button>(true);
                foreach (var btn in btns)
                {
                    ButtonAudio[] btnAudios = btn.GetComponents<ButtonAudio>();
                    foreach(var btnAudio in btnAudios)
                    {
                        GameObject.DestroyImmediate(btnAudio, true);
                    }

                    ButtonAudio audio = btn.gameObject.AddComponent<ButtonAudio>();
                    audio.audioId = 15002;

                    EditorUtility.SetDirty(btn);
                }
                EditorUtility.SetDirty(go);
            }

            if (entered)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        else if (GUILayout.Button("Remove ButtonAudio from Button [选中]"))
        {
            GameObject[] gos = Selection.gameObjects;
            bool entered = gos.Length > 0;
            foreach (var go in gos)
            {
                Button[] btns = go.GetComponentsInChildren<Button>(true);
                foreach (var btn in btns)
                {
                    ButtonAudio[] btnAudios = btn.GetComponents<ButtonAudio>();
                    foreach (var btnAudio in btnAudios)
                    {
                        GameObject.DestroyImmediate(btnAudio, true);
                    }

                    EditorUtility.SetDirty(btn);
                }
                EditorUtility.SetDirty(go);
            }

            if (entered)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
