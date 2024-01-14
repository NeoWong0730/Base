using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(UI_ScrollGrid))]
public class UIScrollGridEditor : Editor
{
    [MenuItem("GameObject/UI/ScrollGrid")]
    static void Genetor()
    {
        GameObject gridgo = new GameObject("Scroll Grid");
        RectTransform gridRect =gridgo.AddComponent<RectTransform>();
        UI_ScrollGrid scrollGrid = gridgo.AddComponent<UI_ScrollGrid>();

        GameObject viewgo = new GameObject("Viewport");
        viewgo.AddComponent<Image>();
        Mask viewMask =viewgo.AddComponent<Mask>();

        GameObject contentgo = new GameObject("Content");
        GridLayoutGroup gridGroup = contentgo.AddComponent<GridLayoutGroup>();
        ContentSizeFitter sizeFit = contentgo.AddComponent<ContentSizeFitter>();

        GameObject elementgo = new GameObject("element");
        elementgo.AddComponent<Image>();
        elementgo.AddComponent<UI_ScrollGrid_Element>();

        Text elementTex = new GameObject("Text").AddComponent<Text>();

        viewgo.transform.SetParent(gridRect, false);
        contentgo.transform.SetParent(viewgo.transform, false);
        elementgo.transform.SetParent(contentgo.transform, false);
        elementTex.transform.SetParent(elementgo.transform, false);

        if (Selection.activeTransform != null)
            gridRect.SetParent(Selection.activeTransform, false);

        gridRect.sizeDelta = new Vector2(150, 300);
        RectTransform viewRect = viewgo.transform as RectTransform;
        viewRect.anchorMin = Vector2.zero;
        viewRect.anchorMax = Vector2.one;
        viewRect.offsetMin = Vector2.zero;
        viewRect.offsetMax = Vector2.zero;

        RectTransform contentRect = contentgo.transform as RectTransform;
        contentRect.anchorMin = Vector2.up;
        contentRect.anchorMax = Vector2.one;


        scrollGrid.m_ViewPort = viewRect;
        scrollGrid.m_Content = contentRect;
        scrollGrid.ChildSize = new Vector2(150, 30);
        scrollGrid.Space = 3;
        scrollGrid.ScrollDir = UI_ScrollGrid.EScrollDir.Vertical;

        gridGroup.cellSize = scrollGrid.ChildSize;
        gridGroup.spacing =  new Vector2(0,scrollGrid.Space);
        gridGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridGroup.constraintCount = 1;

        sizeFit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFit.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;



    }


    SerializedProperty mviewPort;
    SerializedProperty mcontent;
    SerializedProperty maxis;
    SerializedProperty mchildSize;
    SerializedProperty mspace;
    SerializedProperty mscrolldir;

    UI_ScrollGrid mtarget;
    GridLayoutGroup mLayoutGroup;
    private void OnEnable()
    {
        mtarget = target as UI_ScrollGrid;

        
        mviewPort = serializedObject.FindProperty("m_ViewPort");
        mcontent = serializedObject.FindProperty("m_Content");
        maxis = serializedObject.FindProperty("m_Axis");
        mchildSize = serializedObject.FindProperty("m_ChildSize");
        mspace = serializedObject.FindProperty("m_Space");
        mscrolldir = serializedObject.FindProperty("m_ScrollDir");

        if (mtarget.m_ViewPort != null)
            mLayoutGroup = mtarget.m_Content.GetComponent<GridLayoutGroup>();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(mviewPort);
        EditorGUILayout.PropertyField(mcontent);
        EditorGUILayout.PropertyField(maxis);
        EditorGUILayout.PropertyField(mchildSize);
        EditorGUILayout.PropertyField(mspace);
        EditorGUILayout.PropertyField(mscrolldir);

        if (EditorGUI.EndChangeCheck())
        {
            RefreshGridGroup();
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);

        serializedObject.ApplyModifiedProperties();
    }


    private void RefreshGridGroup()
    {
        if (mcontent.objectReferenceValue != null)
        {
            RectTransform content = mcontent.objectReferenceValue as RectTransform;
            mLayoutGroup = content?.GetComponent<GridLayoutGroup>();
        }
            

        RefreshchildSize();
        RefreshcSpace();
        RefreshcScroll();
    }

    private void RefreshchildSize()
    {
        mLayoutGroup.cellSize = mchildSize.vector2Value;
    }

    private void RefreshcSpace()
    {
        mLayoutGroup.spacing = mscrolldir.enumValueIndex == 0 ? new Vector2(mspace.floatValue, 0) : new Vector2(0, mspace.floatValue);
    }

    private void RefreshcScroll()
    {
        mLayoutGroup.constraint = mscrolldir.enumValueIndex == 0 ? GridLayoutGroup.Constraint.FixedRowCount : GridLayoutGroup.Constraint.FixedColumnCount;
        RefreshcSpace();
    }
}
