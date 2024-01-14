using System.Collections.Generic;
using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// 有限状态机状态基类///
    /// </summary>
    public abstract class FsmState
    {
        private readonly Dictionary<int, FsmEventHandler> m_EventHandlers;

        /// <summary>
        /// 初始化有限状态机状态基类的新实例///
        /// </summary>
        public FsmState()
        {
            m_EventHandlers = new Dictionary<int, FsmEventHandler>();
        }

        /// <summary>
        /// 有限状态机状态初始化时调用///
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal virtual void OnInit(IFsm fsm)
        {

        }

        /// <summary>
        /// 有限状态机状态进入时调用///
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal virtual void OnEnter(IFsm fsm)
        {

        }

        /// <summary>
        /// 有限状态机状态轮询时调用///
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal virtual void OnUpdate(IFsm fsm)
        {

        }

        /// <summary>
        /// 有限状态机状态离开时调用///
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        /// <param name="isShutdown">是否是关闭有限状态机时触发</param>
        protected internal virtual void OnExit(IFsm fsm, bool isShutdown)
        {

        }

        /// <summary>
        /// 有限状态机状态销毁时调用///
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        protected internal virtual void OnDestroy(IFsm fsm)
        {

        }

        /// <summary>
        /// 切换当前有限状态机状态///
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型</typeparam>
        /// <param name="fsm">有限状态机引用</param>
        protected void ChangeState<TState>(IFsm fsm) where TState : FsmState
        {
            Fsm fsmImplement = fsm as Fsm;
            if (fsmImplement != null)
            {
                fsmImplement.ChangeState<TState>();
            }
            else
            {
                DebugUtil.LogError("FSM is invalid");
            }
        }

        /// <summary>
        /// 切换当前有限状态机状态///
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        /// <param name="stateType">要切换到的有限状态机状态类型</param>
        protected void ChangeState(IFsm fsm, Type stateType)
        {
            if (stateType == null)
            {
                DebugUtil.LogError("State type is invalid");
                return;
            }

            if (!typeof(FsmState).IsAssignableFrom(stateType))
            {
                DebugUtil.LogError("State type is invalid");
                return;
            }

            Fsm fsmImplement = fsm as Fsm;
            if (fsmImplement != null)
            {
                fsmImplement.ChangeState(stateType);
            }
            else
            {
                DebugUtil.LogError("FSM is invalid");
            }
        }

        /// <summary>
        /// 订阅有限状态机事件///
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="eventHandler">有限状态机事件响应函数</param>
        protected void RegisterEvent(int eventId, FsmEventHandler eventHandler)
        {
            if (eventHandler == null)
            {
                DebugUtil.LogError("Event handler is invalid");
                return;
            }

            if (!m_EventHandlers.ContainsKey(eventId))
            {
                m_EventHandlers[eventId] = eventHandler;
            }
            else
            {
                m_EventHandlers[eventId] += eventHandler;
            }
        }

        /// <summary>
        /// 取消订阅有限状态机事件///
        /// </summary>
        /// <param name="eventId">事件编号</param>
        /// <param name="eventHandler">有限状态机事件响应函数</param>
        protected void UnRegisterEvent(int eventId, FsmEventHandler eventHandler)
        {
            if (eventHandler == null)
            {
                DebugUtil.LogError("Event handler is invalid");
                return;
            }

            if (m_EventHandlers.ContainsKey(eventId))
            {
                m_EventHandlers[eventId] -= eventHandler;
            }
        }

        /// <summary>
        /// 响应有限状态机事件时调用///
        /// </summary>
        /// <param name="fsm">有限状态机引用</param>
        /// <param name="sender">事件源</param>
        /// <param name="eventId">事件编号</param>
        /// <param name="userData">用户自定义数据</param>
        internal void OnEvent(IFsm fsm, object sender, int eventId, object  userData)
        {
            FsmEventHandler eventHandler = null;
            if (m_EventHandlers.TryGetValue(eventId, out eventHandler))
            {
                if (eventHandler != null)
                {
                    eventHandler(fsm, sender, userData);
                }
            }
        }
    }
}
