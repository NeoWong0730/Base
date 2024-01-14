using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIScrollCenterEx))]
public class UIScrollCenterExInspector : Editor
{

    UIScrollCenterEx script;
    void OnEnable()
    {
        script = this.target as UIScrollCenterEx;
        spacing = new Vector2(script.spacingX, script.spacingY);
    }

    void OnDisable()
    {
        script = null;
    }

    Vector2 spacing;
    bool paddingFlag = false;
    bool detailFlag = false;
    bool showIdsFlag = true;
    bool showPoolFlag = false;
    int targetIndex = 0;
    bool isQuickly = false;

    int totalCount = 100;
    float offset = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (script != null)
        {
            script.prefab = EditorGUILayout.ObjectField("prefab", script.prefab, typeof(GameObject), true) as GameObject;
            script.poolCount = (int)EditorGUILayout.Slider("PoolCount", script.poolCount, 4, 100);
            script.isPage = GUILayout.Toggle(script.isPage, "    IsPage");
            script.isVertical = GUILayout.Toggle(script.isVertical, "    IsVertical");
            GUILayout.Space(10);
            EditorGUI.BeginDisabledGroup(!script.m_isInit);
            EditorGUI.BeginChangeCheck();
            float tmp_offset = EditorGUILayout.Slider("scrollOffset", script.scrollOffset, 0, script.m_scrollableValue);
            if (EditorGUI.EndChangeCheck())
            {
                script.scrollOffset = tmp_offset;
            }
            EditorGUI.EndDisabledGroup();
            
            paddingFlag = EditorGUILayout.Foldout(paddingFlag, "Padding", true);
            if (paddingFlag)
            {
                EditorGUI.BeginChangeCheck();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                script.paddingLeft = EditorGUILayout.IntField("Left", script.paddingLeft);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                script.paddingRight = EditorGUILayout.IntField("Right", script.paddingRight);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                script.paddingTop = EditorGUILayout.IntField("Top", script.paddingTop);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                script.paddingBottom = EditorGUILayout.IntField("Bottom", script.paddingBottom);
                GUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                }
            }
            EditorGUI.BeginChangeCheck();
            spacing = EditorGUILayout.Vector2Field("Spacing", spacing);
            if (EditorGUI.EndChangeCheck())
            {
                script.spacingX = (int)spacing.x;
                script.spacingY = (int)spacing.y;
            }
            script.startAxis = (UIScrollCenterEx.Axis)EditorGUILayout.EnumPopup("StartAxis", script.startAxis);
            EditorGUI.BeginDisabledGroup(script.isPage);
            script.constraint = (UIScrollCenterEx.Constraint)EditorGUILayout.EnumPopup("Constraint", script.constraint);
            if (script.constraint != UIScrollCenterEx.Constraint.Flexible)
            {
                script.constraintCount = EditorGUILayout.IntField("ConstraintCount", script.constraintCount);
                if (script.constraintCount < 1)
                {
                    script.constraintCount = 1;
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(!script.isPage);
            script.bLock = GUILayout.Toggle(script.bLock, "bLock");
            script.fCheckSpeed = EditorGUILayout.FloatField("fCheckSpeed", script.fCheckSpeed);
            script.fThreshold = EditorGUILayout.FloatField("fThreshold", script.fThreshold);
            script.fCenterSpeed = EditorGUILayout.FloatField("fCenterSpeed", script.fCenterSpeed);
            script.v3ZoomNormal = EditorGUILayout.Vector3Field("v3ZoomNormal", script.v3ZoomNormal);
            script.v3ZoomCenter = EditorGUILayout.Vector3Field("v3ZoomCenter", script.v3ZoomCenter);
            EditorGUI.EndDisabledGroup();
            detailFlag = EditorGUILayout.Foldout(detailFlag, "Detail", true);
            if (detailFlag)
            {
                EditorGUILayout.IntField("m_totalCount",script.m_totalCount);
                EditorGUILayout.FloatField("m_width",script.m_width);
                EditorGUILayout.FloatField("m_height",script.m_height);
                EditorGUILayout.FloatField("m_contentWidth", script.m_contentWidth);
                EditorGUILayout.FloatField("m_contentHeight", script.m_contentHeight);
                EditorGUILayout.FloatField("m_cellWidth", script.m_cellWidth);//预制体的宽
                EditorGUILayout.FloatField("m_cellHeight", script.m_cellHeight);//预制体的高
                EditorGUILayout.IntField("m_rowNum", script.m_rowNum);//最多同时显示多少行
                EditorGUILayout.IntField("m_colNum", script.m_colNum);//最多同时显示多少列
                EditorGUILayout.IntField("m_realRowNum", script.m_realRowNum);//实际上需要显示多少行
                EditorGUILayout.IntField("m_realColNum", script.m_realColNum);//实际上需要显示多少列
                EditorGUILayout.IntField("m_realPageNum", script.m_realPageNum);
                EditorGUILayout.FloatField("m_PageWidth", script.m_PageWidth);
                EditorGUILayout.FloatField("m_PageHeight", script.m_PageHeight);
                EditorGUILayout.FloatField("m_scrollOffset", script.m_scrollOffset);
                EditorGUILayout.FloatField("m_scrollableValue", script.m_scrollableValue);
                if (script.isPage)
                {
                    EditorGUILayout.IntField("centerChildIndex", script.centerChildIndex);
                }
                GUILayout.Toggle(script.m_isInit, "m_isInit");
                showIdsFlag = EditorGUILayout.Foldout(showIdsFlag, "showIds    "+ script.showIds.Count, true);
                if (showIdsFlag)
                {
                    for (int i = 0; i < script.showIds.Count; i++)
                    {
                        EditorGUILayout.IntField(i + "",script.showIds[i]);
                    }
                }
                showPoolFlag = EditorGUILayout.Foldout(showPoolFlag, "showPool", true);
                if (showPoolFlag)
                {
                    if (script.ShowPool != null)
                    {
                        for (int i = 0; i < script.ShowPool.Count; i++)
                        {
                            EditorGUILayout.ObjectField("cell " + i, script.ShowPool[i], typeof(Transform), true);
                        }
                    }
                }
            }
            GUILayout.Label("------------Function---------------");
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button("Awake", GUILayout.Width(120)))
            {
                if (script.isAwake == false)
                {
                    script.SetParam(null, 0);
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("totalCount", GUILayout.Width(80));
            totalCount = EditorGUILayout.IntField(totalCount, GUILayout.Width(80));
            GUILayout.Label("offset", GUILayout.Width(80));
            offset = EditorGUILayout.FloatField(offset, GUILayout.Width(80));
            if (GUILayout.Button("Init", GUILayout.Width(80)))
            {
                script.Init(totalCount);
                script.scrollOffset = offset;
            }
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(!script.m_isInit);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Index", GUILayout.Width(80));
            targetIndex = EditorGUILayout.IntField(targetIndex, GUILayout.Width(80));
            GUILayout.Label("Quickly", GUILayout.Width(80));
            isQuickly = EditorGUILayout.Toggle(isQuickly, GUILayout.Width(80));
            if (GUILayout.Button("MoveTo", GUILayout.Width(80)))
            {
                script.SwitchIndex(targetIndex, isQuickly);
            }
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }
    }
}
