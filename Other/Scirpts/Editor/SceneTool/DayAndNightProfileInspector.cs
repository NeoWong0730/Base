using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(DayAndNightProfile), true)]
public class DayAndNightProfileInspector : Editor
{
    class Styles
    {
        public static readonly GUIContent WeatherProfiles =
            new GUIContent("Weather Profiles");

        public static readonly GUIContent lightMapTimeStage = 
            new GUIContent("开灯时间", "DayAndNightProfile.lightMapTimeStage.");

        public static readonly GUIContent PassNameField =
            new GUIContent("Name", "Render pass name. This name is the name displayed in Frame Debugger.");

        public static readonly GUIContent MissingFeature = new GUIContent("Missing RendererFeature",
            "Missing reference, due to compilation issues or missing files. you can attempt auto fix or choose to remove the feature.");

        public static GUIStyle BoldLabelSimple;

        static Styles()
        {
            BoldLabelSimple = new GUIStyle(EditorStyles.label);
            BoldLabelSimple.fontStyle = FontStyle.Bold;
        }
    }

    private SerializedProperty m_WeatherProfiles;

    private SerializedProperty m_FalseBool;    
    [SerializeField] private bool falseBool = false;

    List<Editor> m_Editors = new List<Editor>();

    private void OnEnable()
    {        
        m_WeatherProfiles = serializedObject.FindProperty(nameof(DayAndNightProfile.mWeatherProfiles));       
        var editorObj = new SerializedObject(this);
        m_FalseBool = editorObj.FindProperty(nameof(falseBool));
        UpdateEditorList();
    }

    private void OnDisable()
    {
        ClearEditorsList();
    }

    public override void OnInspectorGUI()
    {
        if (m_WeatherProfiles == null)
            OnEnable();
        else if (m_WeatherProfiles.arraySize != m_Editors.Count)
            UpdateEditorList();

        base.OnInspectorGUI();

        EditorGUILayout.Space();        
        serializedObject.Update();

        DrawWeatherProfileList();
    }

    private void DrawWeatherProfileList()
    {
        EditorGUILayout.LabelField(Styles.WeatherProfiles, EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (m_WeatherProfiles.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No Weather Profiles added", MessageType.Info);
        }
        else
        {
            //Draw List
            CoreEditorUtils.DrawSplitter();
            for (int i = 0; i < m_WeatherProfiles.arraySize; i++)
            {
                SerializedProperty weatherProfileProperty = m_WeatherProfiles.GetArrayElementAtIndex(i);
                DrawWeatherProfile(i, ref weatherProfileProperty);
                CoreEditorUtils.DrawSplitter();
            }
        }
        EditorGUILayout.Space();

        //Add Profiles
        //if (GUILayout.Button("Add Weather Profiles", EditorStyles.miniButton))
        //{
        //    
        //}
    }

    private void DrawWeatherProfile(int index, ref SerializedProperty weatherProfileProperty)
    {
        Object weatherProfileObjRef = weatherProfileProperty.objectReferenceValue;
        if (weatherProfileObjRef != null)
        {
            bool hasChangedProperties = false;
            string title = ObjectNames.GetInspectorTitle(weatherProfileObjRef);

            // Get the serialized object for the editor script & update it
            Editor weatherProfileEditor = m_Editors[index];
            SerializedObject serializedweatherProfileEditor = weatherProfileEditor.serializedObject;
            serializedweatherProfileEditor.Update();

            // Foldout header
            EditorGUI.BeginChangeCheck();
            //SerializedProperty activeProperty = serializedweatherProfileEditor.FindProperty("m_Active");

            weatherProfileProperty.serializedObject.Update();
            bool displayContent = weatherProfileProperty.isExpanded = CoreEditorUtils.DrawHeaderFoldout(title, weatherProfileProperty.isExpanded, false);
            weatherProfileProperty.serializedObject.ApplyModifiedProperties();

            //bool displayContent = CoreEditorUtils.DrawHeaderToggle(title, weatherProfileProperty, activeProperty, pos => OnContextClick(pos, index));
            hasChangedProperties |= EditorGUI.EndChangeCheck();

            // ObjectEditor
            if (displayContent)
            {
                EditorGUI.BeginChangeCheck();
                SerializedProperty nameProperty = serializedweatherProfileEditor.FindProperty("m_Name");
                nameProperty.stringValue = EditorGUILayout.DelayedTextField(Styles.PassNameField, nameProperty.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    hasChangedProperties = true;

                    // We need to update sub-asset name
                    weatherProfileObjRef.name = nameProperty.stringValue;
                    AssetDatabase.SaveAssets();

                    // Triggers update for sub-asset name change
                    ProjectWindowUtil.ShowCreatedAsset(target);
                }

                EditorGUI.BeginChangeCheck();
                weatherProfileEditor.OnInspectorGUI();
                hasChangedProperties |= EditorGUI.EndChangeCheck();

                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            }

            // Apply changes and save if the user has modified any settings
            if (hasChangedProperties)
            {
                serializedweatherProfileEditor.ApplyModifiedProperties();
                serializedObject.ApplyModifiedProperties();
                ForceSave();
            }
        }
        else
        {
            CoreEditorUtils.DrawHeaderToggle(Styles.MissingFeature, weatherProfileProperty, m_FalseBool, pos => OnContextClick(pos, index));
            m_FalseBool.boolValue = false; // always make sure false bool is false
            EditorGUILayout.HelpBox(Styles.MissingFeature.tooltip, MessageType.Error);
        }
    }

    private void UpdateEditorList()
    {
        ClearEditorsList();
        for (int i = 0; i < m_WeatherProfiles.arraySize; i++)
        {
            m_Editors.Add(CreateEditor(m_WeatherProfiles.GetArrayElementAtIndex(i).objectReferenceValue));
        }
    }

    private void ClearEditorsList()
    {
        for (int i = m_Editors.Count - 1; i >= 0; --i)
        {
            DestroyImmediate(m_Editors[i]);
        }
        m_Editors.Clear();
    }

    private void ForceSave()
    {
        EditorUtility.SetDirty(target);
    }

    private string ValidateName(string name)
    {
        name = Regex.Replace(name, @"[^a-zA-Z0-9 ]", "");
        return name;
    }

    private void OnContextClick(Vector2 position, int id)
    {
        var menu = new GenericMenu();

        if (id == 0)
            menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Up"));
        else
            menu.AddItem(EditorGUIUtility.TrTextContent("Move Up"), false, () => MoveComponent(id, -1));

        if (id == m_WeatherProfiles.arraySize - 1)
            menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Down"));
        else
            menu.AddItem(EditorGUIUtility.TrTextContent("Move Down"), false, () => MoveComponent(id, 1));

        menu.AddSeparator(string.Empty);
        menu.AddItem(EditorGUIUtility.TrTextContent("Remove"), false, () => RemoveComponent(id));

        menu.DropDown(new Rect(position, Vector2.zero));
    }

    private void RemoveComponent(int id)
    {
        SerializedProperty property = m_WeatherProfiles.GetArrayElementAtIndex(id);
        Object component = property.objectReferenceValue;
        property.objectReferenceValue = null;

        Undo.SetCurrentGroupName(component == null ? "Remove Renderer Feature" : $"Remove {component.name}");

        // remove the array index itself from the list
        m_WeatherProfiles.DeleteArrayElementAtIndex(id);        
        UpdateEditorList();
        serializedObject.ApplyModifiedProperties();

        // Destroy the setting object after ApplyModifiedProperties(). If we do it before, redo
        // actions will be in the wrong order and the reference to the setting object in the
        // list will be lost.
        if (component != null)
        {
            Undo.DestroyObjectImmediate(component);
        }

        // Force save / refresh
        ForceSave();
    }

    private void MoveComponent(int id, int offset)
    {
        Undo.SetCurrentGroupName("Move Render Feature");
        serializedObject.Update();
        m_WeatherProfiles.MoveArrayElement(id, id + offset);        
        UpdateEditorList();
        serializedObject.ApplyModifiedProperties();

        // Force save / refresh
        ForceSave();
    }
}