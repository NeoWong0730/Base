﻿using System;

namespace Logic
{
    /// <summary>
    /// 有限状态机基类///
    /// </summary>
    public abstract class FsmBase
    {
        private readonly string m_Name;

        /// <summary>
        /// 初始化有限状态机基类的新实例///
        /// </summary>
        public FsmBase() : this(null) { }

        /// <summary>
        /// 初始化有限状态机基类的新实例///
        /// </summary>
        /// <param name="name">有限状态机名称</param>
        public FsmBase(string name)
        {
            m_Name = name ?? string.Empty;
        }

        /// <summary>
        /// 获取有限状态机名称///
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// 获取有限状态机名称
        /// </summary>
        public string FullName
        {
            get
            {
                return $"{OwnerType.FullName}_{Name}";
            }
        }

        /// <summary>
        /// 获取有限状态机持有者类型///
        /// </summary>
        public abstract Type OwnerType
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机中状态的数量///
        /// </summary>
        public abstract int FsmStateCount
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机是否正在运行///
        /// </summary>
        public abstract bool IsRunning
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机是否被销毁///
        /// </summary>
        public abstract bool IsDestroyed
        {
            get;
        }

        /// <summary>
        /// 获取当前有限状态机状态名称///
        /// </summary>
        public abstract string CurrentStateName
        {
            get;
        }

        /// <summary>
        /// 有限状态机轮询///
        /// </summary>
        internal abstract void Update();

        /// <summary>
        /// 关闭并清理有限状态机///
        /// </summary>
        internal abstract void Shutdown();
    }
}