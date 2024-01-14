using UnityEditor;

namespace NWFramework.Editor
{
    public static class HotfixDefineSymbols
    {
        private const string EnableHyBridCLRDefineSymbol = "Hotfix_HyBridCLR";
        private const string EnableILRuntimeDefineSymbol = "Hotfix_ILRuntime";
        private const string EnableXLuaDefineSymbol = "Hotfix_XLua";

        private static readonly string[] AllHotfixDefineSymbols = new string[] 
        {
            EnableHyBridCLRDefineSymbol,
            EnableILRuntimeDefineSymbol,
            EnableXLuaDefineSymbol
        };

        private static void DisableAll()
        {
            foreach (var symbol in AllHotfixDefineSymbols)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(symbol);
            }
        }

        /// <summary>
        /// 开启HyBridClR脚本宏定义
        /// </summary>
        [MenuItem("NWFramework/Hotfix Define Symbols/Enable HyBridClR", false, 30)]
        public static void EnableHyBridCLR()
        {
            DisableAll();
            ScriptingDefineSymbols.AddScriptingDefineSymbol(EnableHyBridCLRDefineSymbol);
        }

        /// <summary>
        /// 开启ILRuntime脚本宏定义
        /// </summary>
        [MenuItem("NWFramework/Hotfix Define Symbols/Enable ILRuntime", false, 31)]
        public static void EnableILRuntime()
        {
            DisableAll();
            ScriptingDefineSymbols.AddScriptingDefineSymbol(EnableILRuntimeDefineSymbol);
        }

        /// <summary>
        /// 开启XLua脚本宏定义
        /// </summary>
        [MenuItem("NWFramework/Hotfix Define Symbols/Enable XLua", false, 32)]
        public static void EnableXLua()
        {
            DisableAll();
            ScriptingDefineSymbols.AddScriptingDefineSymbol(EnableXLuaDefineSymbol);
        }
    }
}