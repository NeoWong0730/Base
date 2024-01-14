using System.Diagnostics;
using UnityEngine.Profiling;

namespace NWFramework
{
    /// <summary>
    /// 游戏框架Profiler分析器类
    /// </summary>
    public class NWProfiler
    {
        private static int _profileLevel = -1;
        private static int _currentLevel = 0;
        private static int _sampleLevel = 0;

        /// <summary>
        /// 设置分析器等级
        /// </summary>
        /// <param name="level">调试器等级</param>
        private static void SetProfileLevel(int level)
        {
            _profileLevel = level;
        }

        /// <summary>
        /// 开始使用自定义采样分析一段代码
        /// </summary>
        /// <param name="name">用于在Profiler窗口中标识样本的字符串</param>
        [Conditional("NWPROFILER")]
        public static void BeginSample(string name)
        {
            _currentLevel++;
            if (_profileLevel >= 0 && _currentLevel > _profileLevel)
            {
                return;
            }

            _sampleLevel++;
            Profiler.BeginSample(name);
        }

        /// <summary>
        /// 结束本次自定义采样分析
        /// </summary>
        [Conditional("NWPROFILER")]
        public static void EndSample()
        {
            if (_currentLevel <= _sampleLevel)
            {
                Profiler.EndSample();
                _sampleLevel--;
            }

            _currentLevel--;
        }
    }
}
