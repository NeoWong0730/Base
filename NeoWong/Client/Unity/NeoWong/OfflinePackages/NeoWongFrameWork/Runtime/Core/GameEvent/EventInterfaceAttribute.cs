using System;

namespace NWFramework
{
    /// <summary>
    /// 事件分组枚举
    /// </summary>
    public enum EEventGroup
    {
        /// <summary>
        /// UI相关的交互
        /// </summary>
        GroupUI,

        /// <summary>
        /// 逻辑层内部相关的交互
        /// </summary>
        GroupLogic,
    }

    [AttributeUsage(AttributeTargets.Interface)]
    public class EventInterface : Attribute
    {
        private EEventGroup _group;

        public EventInterface(EEventGroup group)
        {
            _group = group;
        }
    }
}