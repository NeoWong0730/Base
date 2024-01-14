using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// 有限状态机///
    /// </summary>
    public sealed class Fsm : FsmBase, IFsm
    {
        private readonly object m_Owner;
        private readonly Dictionary<string, FsmState> m_States;
        private FsmState m_CurrentState;
        private bool m_IsDestroyed;

        /// <summary>
        /// 初始化有限状态机的新实例///
        /// </summary>
        /// <param name="name">有限状态机名称</param>
        /// <param name="owner">有限状态机持有者</param>
        /// <param name="states">有限状态机状态集合</param>
        public Fsm(string name, object owner, params FsmState[] states) : base(name)
        {
            if (owner == null)
            {
                DebugUtil.LogError("FSM owner is invalid");
                return;
            }

            if (states == null || states.Length < 1)
            {
                DebugUtil.LogError("FSM states is invalid");
                return;
            }

            m_Owner = owner;
            m_States = new Dictionary<string, FsmState>();

            foreach (FsmState state in states)
            {
                if (state == null)
                {
                    DebugUtil.LogError("FSM states is invalid");
                    continue;
                }

                string stateName = state.GetType().FullName;
                if (m_States.ContainsKey(stateName))
                {
                    DebugUtil.LogError($"FSM {FullName} state {stateName} is already exist");
                    continue;
                }

                m_States.Add(stateName, state);
                state.OnInit(this);
            }

            m_CurrentState = null;
            m_IsDestroyed = false;
        }

        /// <summary>
        /// 获取有限状态机持有者///
        /// </summary>
        public object Owner
        {
            get
            {
                return m_Owner;
            }
        }

        /// <summary>
        /// 获取有限状态机持有者类型///
        /// </summary>
        public override Type OwnerType
        {
            get
            {
                return m_Owner.GetType();
            }
        }

        /// <summary>
        /// 获取有限状态机中状态的数量///
        /// </summary>
        public override int FsmStateCount
        {
            get
            {
                return m_States.Count;
            }
        }

        /// <summary>
        /// 获取有限状态机是否正在运行///
        /// </summary>
        public override bool IsRunning
        {
            get
            {
                return m_CurrentState != null;
            }
        }

        /// <summary>
        /// 获取有限状态机是否被销毁///
        /// </summary>
        public override bool IsDestroyed
        {
            get
            {
                return m_IsDestroyed;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态///
        /// </summary>
        public FsmState CurrentState
        {
            get
            {
                return m_CurrentState;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态名称///
        /// </summary>
        public override string CurrentStateName
        {
            get
            {
                return m_CurrentState != null ? m_CurrentState.GetType().FullName : null;
            }
        }

        /// <summary>
        /// 开始有限状态机///
        /// </summary>
        /// <typeparam name="TState">要开始的有限状态机状态类型</typeparam>
        public void Start<TState>() where TState : FsmState
        {
            if (IsRunning)
            {
                DebugUtil.LogError("FSM is running, can not start again");
                return;
            }

            FsmState state = GetState<TState>();
            if (state == null)
            {
                DebugUtil.LogError($"FSM {FullName} can not start state {typeof(TState).FullName} which is not exist");
                return;
            }

            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 开始有限状态机///
        /// </summary>
        /// <param name="stateType">要开始的有限状态机状态类型</param>
        public void Start(Type stateType)
        {
            if (IsRunning)
            {
                DebugUtil.LogError("FSM is running, can not start again");
                return;
            }

            if (stateType == null)
            {
                DebugUtil.LogError("State type is invalid");
                return;
            }

            if (!typeof(FsmState).IsAssignableFrom(stateType))
            {
                DebugUtil.LogError($"State type {stateType.FullName} is invalid");
                return;
            }

            FsmState state = GetState(stateType);
            if (state == null)
            {
                DebugUtil.LogError($"FSM {FullName} can not start state {stateType.FullName} which is not exist");
                return;
            }

            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }

        /// <summary>
        /// 是否存在有限状态机状态///
        /// </summary>
        /// <typeparam name="TState">要检查的有限状态机状态类型</typeparam>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState<TState>() where TState : FsmState
        {
            return m_States.ContainsKey(typeof(TState).FullName);
        }

        /// <summary>
        /// 是否存在有限状态机状态///
        /// </summary>
        /// <param name="stateType">要检查的有限状态机状态类型</param>
        /// <returns>是否存在有限状态机状态</returns>
        public bool HasState(Type stateType)
        {
            if (stateType == null)
            {
                DebugUtil.LogError("State type is invalid");
                return false;
            }

            if (!typeof(FsmState).IsAssignableFrom(stateType))
            {
                DebugUtil.LogError($"State type {stateType.FullName} is invalid");
                return false;
            }

            return m_States.ContainsKey(stateType.FullName);
        }

        /// <summary>
        /// 获取有限状态机状态///
        /// </summary>
        /// <typeparam name="TState">要获取的有限状态机状态类型</typeparam>
        /// <returns>要获取的有限状态机状态</returns>
        public TState GetState<TState>() where TState : FsmState
        {
            FsmState state = null;
            if (m_States.TryGetValue(typeof(TState).FullName, out state))
            {
                return (TState)state;
            }

            return null;
        }

        /// <summary>
        /// 获取有限状态机状态///
        /// </summary>
        /// <param name="stateType">要获取的有限状态机状态类型</param>
        /// <returns>要获取的有限状态机状态</returns>
        public FsmState GetState(Type stateType)
        {
            if (stateType == null)
            {
                DebugUtil.LogError("State type is invalid");
                return null;
            }

            if (!typeof(FsmState).IsAssignableFrom(stateType))
            {
                DebugUtil.LogError($"State type {stateType.FullName} is invalid");
                return null;
            }

            FsmState state = null;
            if (m_States.TryGetValue(stateType.FullName, out state))
            {
                return state;
            }

            return null;
        }

        /// <summary>
        /// 获取有限状态机的所有状态///
        /// </summary>
        /// <returns>有限状态机的所有状态</returns>
        public FsmState[] GetAllStates()
        {
            int index = 0;
            FsmState[] results = new FsmState[m_States.Count];
            foreach (KeyValuePair<string, FsmState> state in m_States)
            {
                results[index++] = state.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取有限状态机的所有状态///
        /// </summary>
        /// <param name="results">有限状态机的所有状态</param>
        public void GetAllStates(List<FsmState> results)
        {
            if (results == null)
            {
                DebugUtil.LogError("Results is invalid");
                return;
            }

            results.Clear();
            foreach (KeyValuePair<string, FsmState> state in m_States)
            {
                results.Add(state.Value);
            }
        }

        /// <summary>
        /// 抛出有限状态机事件///
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventId">事件编号</param>
        public void TriggerEvent(object sender, int eventId)
        {
            if (m_CurrentState == null)
            {
                DebugUtil.LogError("Current state is invalid.");
                return;
            }

            m_CurrentState.OnEvent(this, sender, eventId, null);
        }

        /// <summary>
        /// 抛出有限状态机事件///
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventId">事件编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void TriggerEvent(object sender, int eventId, object userData)
        {
            if (m_CurrentState == null)
            {
                DebugUtil.LogError("Current state is invalid.");
                return;
            }

            m_CurrentState.OnEvent(this, sender, eventId, userData);
        }

        /// <summary>
        /// 有限状态机轮询///
        /// </summary>
        internal override void Update()
        {
            if (m_CurrentState == null)
            {
                return;
            }

            m_CurrentState.OnUpdate(this);
        }

        /// <summary>
        /// 关闭并清理有限状态机///
        /// </summary>
        internal override void Shutdown()
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.OnExit(this, true);
                m_CurrentState = null;
            }

            foreach (KeyValuePair<string, FsmState> state in m_States)
            {
                state.Value.OnDestroy(this);
            }

            m_States.Clear();

            m_IsDestroyed = true;
        }

        /// <summary>
        /// 切换当前有限状态机状态///
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型</typeparam>
        internal void ChangeState<TState>() where TState : FsmState
        {
            ChangeState(typeof(TState));
        }

        /// <summary>
        /// 切换当前有限状态机状态///
        /// </summary>
        /// <param name="stateType">要切换到的有限状态机状态类型</param>
        internal void ChangeState(Type stateType)
        {
            if (m_CurrentState == null)
            {
                DebugUtil.LogError("Current state is invalid");
                return;
            }

            FsmState state = GetState(stateType);
            if (state == null)
            {
                DebugUtil.LogError($"FSM {FullName} can not change state to {stateType.FullName} which is not exis");
                return;
            }

            m_CurrentState.OnExit(this, false);
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }
    }
}
