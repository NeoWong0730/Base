using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWFramework
{
    /// <summary>
    /// HybridCLRCustomGlobalSettings
    /// </summary>
    [Serializable]
    public class HybridCLRCustomGlobalSettings
    {
        [Header("Auto sync with [HybridCLRSettings]")]
        [SerializeField]
        private bool m_Enable = false;

        public bool Enable
        {
            get => m_Enable;
            set => m_Enable = value;
        }

        [Header("Auto sync with [HybridCLRSettings]")]
        public List<string> HotUpdateAssemblies = new List<string>()
        {
            "HotFix.dll"
        };

        [Header("Need manual setting!")]
        public List<string> AOTMetaAssemblies = new List<string>()
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll",
            "UnityEngine.CoreModule.dll",
            "YooAsset.dll",
            "UniTask.dll",
            "NWFramework.Runtime.dll"
        };

        /// <summary>
        /// Dll of main business logic assembly
        /// </summary>
        public string LogicMainDllName = "HotFix.dll";

        /// <summary>
        /// 程序集文本资产打包Asset后缀名
        /// </summary>
        public string AssemblyTextAssetExtension = ".bytes";

        /// <summary>
        /// 程序集文本资产资源目录
        /// </summary>
        public string AssemblyTextAssetPath = "AssetRaw/DLL";
    }
}