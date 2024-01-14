using UnityEditor;
using UnityEngine;

namespace NWFramework.Editor
{
    [InitializeOnLoad]
    public class EditorResourceMode
    {
        static class ToolbarStyles
        {
            public static readonly GUIStyle ToolbarExtenderBtnStyle;
            public static readonly GUIStyle ToolbarTextStyle;
            public static readonly GUIStyle ToolbarButtonGUISytle;
            
            static ToolbarStyles()
            {
                ToolbarExtenderBtnStyle = new GUIStyle("Command")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Normal,
                    fixedWidth = 60
                };

                ToolbarTextStyle = new GUIStyle(ButtonStyleName)
                {
                    padding = new RectOffset(2, 8, 2, 2),
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };

                ToolbarButtonGUISytle = new GUIStyle(ButtonStyleName)
                {
                    padding = new RectOffset(2, 8, 2, 2),
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };
            }
        }

        private const string ButtonStyleName = "Tab middle";
        static GUIStyle _buttonGuiStyle;

        private static int _resourceModeIndex = 0;
        public static int ResourceModeIndex => _resourceModeIndex;

        static EditorResourceMode()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
            _resourceModeIndex = EditorPrefs.GetInt("EditorResourceMode", 0);
        }

        private static readonly string[] _resourceModeNames =
        {
            "EditorMode (编辑器下的模拟模式)",
            "OfflinePlayMode (单机模式)",
            "HostPlayMode (联机运行模式)",
            "WebPlayMode (WebGL运行模式)"
        };

        static void OnToolbarGUI()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                GUILayout.Space(10);
                GUILayout.FlexibleSpace();

                // 资源模式
                int selectedIndex = EditorGUILayout.Popup("", _resourceModeIndex, _resourceModeNames, ToolbarStyles.ToolbarButtonGUISytle);
                if (selectedIndex != _resourceModeIndex)
                {
                    Debug.Log($"更改编辑器资源运行模式 : {_resourceModeNames[selectedIndex]}");
                    _resourceModeIndex = selectedIndex;
                    EditorPrefs.SetInt("EditorResourceMode", selectedIndex);
                }

                GUILayout.FlexibleSpace();
                GUILayout.Space(400);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}