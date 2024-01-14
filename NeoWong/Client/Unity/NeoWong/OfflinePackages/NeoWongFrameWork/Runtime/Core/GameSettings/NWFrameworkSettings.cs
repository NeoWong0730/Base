using UnityEngine;

namespace NWFramework
{
    [CreateAssetMenu(fileName = "NWFrameworkGlobalSettings", menuName = "NWFramework/NWFrameworkSettings")]
    public class NWFrameworkSettings : ScriptableObject
    {
        [Header("Framework")]
        [SerializeField]
        private FrameworkGlobalSettings m_FrameworkGlobalSettings;

        public FrameworkGlobalSettings FrameworkGlobalSettings => m_FrameworkGlobalSettings;

        [Header("HybridCLR")]
        [SerializeField]
        private HybridCLRCustomGlobalSettings m_HybridCLRCustomGlobalSettings;

        public HybridCLRCustomGlobalSettings HybridCLRCustomGlobalSettings => m_HybridCLRCustomGlobalSettings;

        public void Set(FrameworkGlobalSettings globalSettings, HybridCLRCustomGlobalSettings hybridClrCustomGlobalSettings)
        {
            m_FrameworkGlobalSettings = globalSettings;
            m_HybridCLRCustomGlobalSettings = hybridClrCustomGlobalSettings;
        }
    }
}