using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Framework
{
    [CustomEditor(typeof(SkeletonModelController), true)]
    public class SkeletonModelControllerEditor : Editor
    {
        SkeletonModelController skeletonModel;

        SerializedObject serializedAnimationClipListData;
        SerializedProperty animationClipDatas;

        public override void OnInspectorGUI()
        {
            skeletonModel = (SkeletonModelController)target;

            base.OnInspectorGUI();

            GUILayout.BeginVertical("AnimationClips", "window");

            skeletonModel.animationClipListData = (AnimationClipListData)EditorGUILayout.ObjectField("animationClipListData", skeletonModel.animationClipListData, typeof(AnimationClipListData), false);
            if (skeletonModel.animationClipListData == null)
            {
                GUILayout.EndVertical();
                return;
            }

            if (serializedAnimationClipListData == null)
                serializedAnimationClipListData = new SerializedObject(skeletonModel.animationClipListData);
            if (animationClipDatas == null)
                animationClipDatas = serializedAnimationClipListData.FindProperty("animationClipDatas");

            EditorGUILayout.TextField("id", skeletonModel.animationClipListData.id.ToString());
            EditorGUILayout.PropertyField(animationClipDatas, true);

            GUILayout.EndVertical();
        }
    }
}
