using System;

namespace NWFramework
{
    /// <summary>
    /// UI层级枚举
    /// </summary>
    public enum UILayer : int
    {
        Bottom = 0,
        UI = 1,
        Top = 2,
        Tips = 3,
        System = 4,
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class WindowAttribute : Attribute
    {
        /// <summary>
        /// 窗口层级
        /// </summary>
        public readonly int WindowLayer;

        /// <summary>
        /// 资源定位地址
        /// </summary>
        public readonly string Location;

        /// <summary>
        /// 全屏窗口标记
        /// </summary>
        public readonly bool FullScreen;

        /// <summary>
        /// 是内部资源无需AB加载
        /// </summary>
        public readonly bool FromResources;

        /// <summary>
        /// 需要缓存
        /// <remarks>关闭界面不立刻释放资源。</remarks>
        public readonly bool NeedCache;

        public WindowAttribute(int windowLayer, string location = "", bool fullScreen = false, bool needCache = false)
        {
            WindowLayer = windowLayer;
            Location = location;
            FullScreen = fullScreen;
            NeedCache = needCache;
        }

        public WindowAttribute(UILayer windowLayer, string location = "", bool fullScreen = false, bool needCache = false)
        {
            WindowLayer = (int)windowLayer;
            Location = location;
            FullScreen = fullScreen;
            NeedCache = needCache;
        }

        public WindowAttribute(UILayer windowLayer, bool fromResources, bool fullScreen = false, bool needCache = false)
        {
            WindowLayer = (int)windowLayer;
            FromResources = fromResources;
            NeedCache = needCache;
        }

        public WindowAttribute(UILayer windowLayer, bool fromResources, string location, bool fullScreen = false, bool needCache = false)
        {
            WindowLayer = (int)windowLayer;
            FromResources = fromResources;
            Location = location;
            NeedCache = needCache;
        }
    }
}