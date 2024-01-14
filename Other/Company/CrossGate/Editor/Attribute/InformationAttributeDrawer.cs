#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(InformationAttribute))]

public class InformationAttributeDrawer : PropertyDrawer
{
    const int spaceBeforeTheTextBox = 5;
    const int spaceAfterTheTextBox = 10;
    const int iconWidth = 55;

    InformationAttribute informationAttribute
    {
        get { return (InformationAttribute)attribute; }
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorStyles.helpBox.richText = true;
        Rect helpPosition = rect;
        Rect textFieldPosition = rect;

        if ((!informationAttribute.MessageAfterProperty))
        {
            helpPosition.height = DeterwineTextboxHeight(informationAttribute.Message);

            textFieldPosition.y += helpPosition.height + spaceBeforeTheTextBox;
            textFieldPosition.height = GetPropertyHeight(property, label);
        }
        else
        {
            textFieldPosition.height = GetPropertyHeight(property, label);

            helpPosition.height = DeterwineTextboxHeight(informationAttribute.Message);
            textFieldPosition.y += GetPropertyHeight(property, label) - DeterwineTextboxHeight(informationAttribute.Message) - spaceAfterTheTextBox;
        }

        EditorGUI.HelpBox(helpPosition, informationAttribute.Message, informationAttribute.Type);
        EditorGUI.PropertyField(textFieldPosition, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) + DeterwineTextboxHeight(informationAttribute.Message) + spaceAfterTheTextBox + spaceBeforeTheTextBox;
    }

    protected virtual float DeterwineTextboxHeight(string message)
    {
        GUIStyle style = new GUIStyle(EditorStyles.helpBox);
        style.richText = true;

        float newHeight = style.CalcHeight(new GUIContent(message), EditorGUIUtility.currentViewWidth - iconWidth);
        return newHeight;
    }
}
#endif




