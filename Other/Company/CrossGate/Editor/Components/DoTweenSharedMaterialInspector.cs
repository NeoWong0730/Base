using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.DOTweenEditor.Core;
using DG.DOTweenEditor.UI;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Rendering;
using System;
using UnityEditorInternal;

namespace DG.DOTweenEditor
{
    [CustomEditor(typeof(DoTweenSharedMaterial))]
    public class DoTweenSharedMaterialInspector : Editor
    {        
        DoTweenSharedMaterial doTweenSharedMaterial;
        private GUIContent profileGUIContent = new GUIContent("profile");        
        
        Editor profileEditor;
        Tween tween;
        private bool playing = false;

        void OnEnable()
        {
            doTweenSharedMaterial = target as DoTweenSharedMaterial;
            
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5
            UnityEditor.EditorApplication.playmodeStateChanged += StopAllPreviews;
#else
            UnityEditor.EditorApplication.playModeStateChanged += StopAllPreviews;
#endif
        }

        private void OnDisable()
        {
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5
            UnityEditor.EditorApplication.playmodeStateChanged -= StopAllPreviews;
#else
            UnityEditor.EditorApplication.playModeStateChanged -= StopAllPreviews;
#endif

            StopPreview();
            InternalEditorUtility.RepaintAllViews();
        }

        private void OnDestroy()
        {
            if (profileEditor)
            {
                DestroyImmediate(profileEditor);
                Debug.Log("Destroy editor");
            }
        }

        private void StopAllPreviews(PlayModeStateChange obj)
        {
            StopPreview();
        }

        private void StartPreview()
        {
            StopPreview();

            if (!playing)
            {
                DG.DOTweenEditor.DOTweenEditorPreview.Start();
                playing = true;
            }            

            tween = doTweenSharedMaterial.CreateEditorPreview();
            if (tween != null)
            {
                DG.DOTweenEditor.DOTweenEditorPreview.PrepareTweenForPreview(tween);
            }
        }

        private void StopPreview()
        {
            if (tween != null)
            {
                tween.Rewind();
                tween.Kill();
                tween = null;
            }

            if (playing)
            {
                DG.DOTweenEditor.DOTweenEditorPreview.Stop();
                playing = false;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            doTweenSharedMaterial.doTweenSharedMaterialProfile = (DoTweenSharedMaterialProfile)EditorGUILayout.ObjectField(profileGUIContent, doTweenSharedMaterial.doTweenSharedMaterialProfile, typeof(DoTweenSharedMaterialProfile), true);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }

            if (doTweenSharedMaterial.doTweenSharedMaterialProfile == null)
            {
                if (GUILayout.Button("Create"))
                {
                    doTweenSharedMaterial.doTweenSharedMaterialProfile = ScriptableObject.CreateInstance<DoTweenSharedMaterialProfile>();
                    doTweenSharedMaterial.doTweenSharedMaterialProfile.name = string.Format("DoTween_{0}.asset", doTweenSharedMaterial.doTweenSharedMaterialProfile.GetInstanceID());
                    AssetDatabase.CreateAsset(doTweenSharedMaterial.doTweenSharedMaterialProfile, string.Format("Assets/Projects/DoTweenProfiles/{0}", doTweenSharedMaterial.doTweenSharedMaterialProfile.name));

                    EditorUtility.SetDirty(target);
                }
            }
            EditorGUILayout.EndHorizontal();

            Editor.DrawFoldoutInspector(doTweenSharedMaterial.doTweenSharedMaterialProfile, ref profileEditor);

            if (doTweenSharedMaterial.doTweenSharedMaterialProfile == null)
                return;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Play"))
            {
                if (!Application.isPlaying)
                {
                    if (tween == null || !tween.IsActive())
                    {                        
                        StartPreview();
                    }
                }
                else
                {
                    doTweenSharedMaterial.DOPlay();
                }                
            }

            if (GUILayout.Button("Replay"))
            {
                if (!Application.isPlaying)
                {                    
                    StartPreview();
                }
                else
                {
                    doTweenSharedMaterial.DOReplay();
                }                
            }

            if (GUILayout.Button("Stop"))
            {
                if (!Application.isPlaying)
                {
                    StopPreview();
                    InternalEditorUtility.RepaintAllViews();
                }
                else
                {
                    doTweenSharedMaterial.DOStop();
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}