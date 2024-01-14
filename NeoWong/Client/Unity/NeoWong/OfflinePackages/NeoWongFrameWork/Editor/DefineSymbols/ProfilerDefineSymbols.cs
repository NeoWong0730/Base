using UnityEditor;

namespace NWFramework.Editor
{
    /// <summary>
    /// Profiler分析器宏定义操作类
    /// </summary>
    public class ProfilerDefineSymbols
    {
        private const string EnableFirstProfiler = "FIRST_PROFILER";
        private const string EnableDinProFiler = "NWPROFILER";

        private static readonly string[] AllProfilerDefineSymbols = new string[]
        {
            EnableFirstProfiler,
            EnableDinProFiler,
        };

        /// <summary>
        /// 禁用所有日志脚本宏定义
        /// </summary>
        [MenuItem("NWFramework/Profiler Define Symbols/Disable All Profiler", false, 30)]
        public static void DisableAllLogs()
        {
            foreach (string aboveLogScriptingDefineSymbol in AllProfilerDefineSymbols)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(aboveLogScriptingDefineSymbol);
            }
        }

        /// <summary>
        /// 开启所有日志脚本宏定义
        /// </summary>
        [MenuItem("NWFramework/Profiler Define Symbols/Enable All Profiler", false, 31)]
        public static void EnableAllLogs()
        {
            DisableAllLogs();
            foreach (string aboveLogScriptingDefineSymbol in AllProfilerDefineSymbols)
            {
                ScriptingDefineSymbols.AddScriptingDefineSymbol(aboveLogScriptingDefineSymbol);
            }
        }
    }
}