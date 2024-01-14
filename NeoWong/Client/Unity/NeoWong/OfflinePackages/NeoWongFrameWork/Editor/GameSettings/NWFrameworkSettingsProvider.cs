using System.IO;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;
using NWFramework;

namespace NWFramework.Editor
{
    public class NWFrameworkSettingsProvider : SettingsProvider
    {
        const string k_SettingsPath = "Assets//Resources/NWFrameworkGlobalSettings.asset";
        private const string headerName = "NWFramework/NWFrameworkSettings";
        private SerializedObject m_CustomSettings;

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(SettingsUtils.GlobalSettings);
        }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(k_SettingsPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            m_CustomSettings = GetSerializedSettings();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            SaveAssetData(k_SettingsPath);
        }

        void SaveAssetData(string path)
        {
            NWFrameworkSettings old = AssetDatabase.LoadAssetAtPath<NWFrameworkSettings>(k_SettingsPath);
            NWFrameworkSettings data = ScriptableObject.CreateInstance<NWFrameworkSettings>();
            data.Set(old.FrameworkGlobalSettings, old.HybridCLRCustomGlobalSettings);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);
            using var changeCheckScope = new EditorGUI.ChangeCheckScope();
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_FrameworkGlobalSettings"));

            if (GUILayout.Button("Refresh HotUpdateAssemblies"))
            {
                SyncAssemblyContent.RefreshAssembly();
                m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
                m_CustomSettings = null;
                m_CustomSettings = GetSerializedSettings();
            }

            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_HybridCLRCustomGlobalSettings"));
            EditorGUILayout.Space(20);
            if (!changeCheckScope.changed)
            {
                return;
            }
            m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        public NWFrameworkSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingProvider()
        {
            if (IsSettingsAvailable())
            {
                var provider = new NWFrameworkSettingsProvider(headerName, SettingsScope.Project);
                provider.keywords = GetSearchKeywordsFromGUIContentProperties<NWFrameworkSettings>();
                return provider;
            }
            else
            {
                Debug.LogError($"Open NWFramework Settings error,Please Create NWFramework NWFrameworkGlobalSettings.assets File in Path Assets/Resources/");
            }

            return null;
        }
    }
}