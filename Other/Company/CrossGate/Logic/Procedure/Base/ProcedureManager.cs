using Lib.Core;
using System;
using UnityEngine;
using Logic.Core;

namespace Logic
{
    public sealed class ProcedureComponent : MonoBehaviour
    {
        public ProcedureManager.EProcedureType type;

        private void Update()
        {
            if (GameMain.Procedure.CurrentProcedure != null)
                type = GameMain.Procedure.CurrentProcedure.ProcedureType;
        }
    }

    /// <summary>
    /// 流程管理器///
    /// </summary>
    public sealed class ProcedureManager
    {
        public enum EProcedureType
        {
            Normal = 0,
            Interactive,
            Fight,
            CutScene,
        }

        public enum EEvents
        {
            OnBeforeEnterFightEffect,    //进入战斗,开始播放过渡效果
            OnAfterEnterFightEffect,     //进入战斗,播放完过渡效果
            OnBeforeExitFightEffect,    //退出战斗,开始播放过渡效果
            OnAfterExitFightEffect,    //退出战斗,播放完过渡效果

            OnEnterNormal,
        }

        public static EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        private FsmManager m_FsmManager;
        public IFsm m_ProcedureFsm;

        /// <summary>
        /// 初始化流程管理器的新实例///
        /// </summary>
        public ProcedureManager()
        {
            m_FsmManager = null;
            m_ProcedureFsm = null;
        }

        /// <summary>
        /// 获取当前流程///
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                if (m_ProcedureFsm == null)
                {
                    //DebugUtil.LogError("You must initialize procedure first");
                    return null;
                }

                return (ProcedureBase)m_ProcedureFsm.CurrentState;
            }
        }

        /// <summary>
        /// 关闭并清理流程管理器///
        /// </summary>
        public void Shutdown()
        {
            if (m_FsmManager != null)
            {
                if (m_ProcedureFsm != null)
                {
                    m_FsmManager.DestroyFsm(m_ProcedureFsm);
                    m_ProcedureFsm = null;
                }

                m_FsmManager = null;
            }
        }

        /// <summary>
        /// 初始化流程管理器///
        /// </summary>
        /// <param name="fsmManager">有限状态机管理器</param>
        /// <param name="procedures">流程管理器包含的流程</param>
        public void Initialize(FsmManager fsmManager, params ProcedureBase[] procedures)
        {
            if (fsmManager == null)
            {
                DebugUtil.LogError("FSM manager is invalid");
                return;
            }

            m_FsmManager = fsmManager;
            m_ProcedureFsm = m_FsmManager.CreateFsm(this, procedures);
        }

        /// <summary>
        /// 开始流程///
        /// </summary>
        /// <typeparam name="T">要开始的流程类型</typeparam>
        public void StartProcedure<T>() where T : ProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
                DebugUtil.LogError("You must initialize procedure first");
                return;
            }

            m_ProcedureFsm.Start<T>();
        }

        /// <summary>
        /// 开始流程///
        /// </summary>
        /// <param name="procedureType">要开始的流程类型</param>
        public void StartProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
                DebugUtil.LogError("You must initialize procedure first");
                return;
            }

            m_ProcedureFsm.Start(procedureType);
        }

        /// <summary>
        /// 是否存在流程///
        /// </summary>
        /// <typeparam name="T">要检查的流程类型</typeparam>
        /// <returns>是否存在流程</returns>
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
                DebugUtil.LogError("You must initialize procedure first");
                return false;
            }

            return m_ProcedureFsm.HasState<T>();
        }

        /// <summary>
        /// 是否存在流程///
        /// </summary>
        /// <param name="procedureType">要检查的流程类型</param>
        /// <returns>是否存在流程</returns>
        public bool HasProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
                DebugUtil.LogError("You must initialize procedure first");
                return false;
            }

            return m_ProcedureFsm.HasState(procedureType);
        }

        /// <summary>
        /// 获取流程///
        /// </summary>
        /// <typeparam name="T">要获取的流程类型</typeparam>
        /// <returns>要获取的流程</returns>
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
                DebugUtil.LogError("You must initialize procedure first");
                return null;
            }

            return m_ProcedureFsm.GetState<T>();
        }

        /// <summary>
        /// 获取流程///
        /// </summary>
        /// <param name="procedureType">要获取的流程类型</param>
        /// <returns>要获取的流程</returns>
        public ProcedureBase GetProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
                DebugUtil.LogError("You must initialize procedure first");
                return null;
            }

            return (ProcedureBase)m_ProcedureFsm.GetState(procedureType);
        }

        /// <summary>
        /// 抛出有限状态机事件///
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventId">事件编号</param>
        public void TriggerEvent(object sender, int eventId)
        {
            if (m_ProcedureFsm == null)
            {
                DebugUtil.LogError("You must initialize procedure first");
                return;
            }
            DebugUtil.LogFormat(ELogType.eProcedure, "eventId:{0} ", (EProcedureEvent)eventId);
            m_ProcedureFsm.TriggerEvent(sender, eventId);
        }

        /// <summary>
        /// 抛出有限状态机事件///
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventId">事件编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void TriggerEvent(object sender, int eventId, object userData)
        {
            if (m_ProcedureFsm == null)
            {
                DebugUtil.LogError("You must initialize procedure first");
                return;
            }

            m_ProcedureFsm.TriggerEvent(sender, eventId, userData);
        }
    }
}