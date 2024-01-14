using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 有限状态机接口///
    /// </summary>
    public interface IFsm
    {
        /// <summary>
        /// 获取有限状态机名称///
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机持有者///
        /// </summary>
        object Owner
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机中状态的数量///
        /// </summary>
        int FsmStateCount
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机是否正在运行///
        /// </summary>
        bool IsRunning
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机是否被销毁///
        /// </summary>
        bool IsDestroyed
        {
            get;
        }

        /// <summary>
        /// 获取当前有限状态机状态///
        /// </summary>
        FsmState CurrentState
        {
            get;
        }

        /// <summary>
        /// 开始有限状态机///
        /// </summary>
        /// <typeparam name="TState">要开始的有限状态机状态类型</typeparam>
        void Start<TState>() where TState : FsmState;

        /// <summary>
        /// 开始有限状态机///
        /// </summary>
        /// <param name="stateType">要开始的有限状态机状态类型</param>
        void Start(Type stateType);

        /// <summary>
        /// 是否存在有限状态机状态///
        /// </summary>
        /// <typeparam name="TState">要检查的有限状态机状态类型</typeparam>
        /// <returns>是否存在有限状态机状态</returns>
        bool HasState<TState>() where TState : FsmState;

        /// <summary>
        /// 是否存在有限状态机状态///
        /// </summary>
        /// <param name="stateType">要检查的有限状态机状态类型</param>
        /// <returns>是否存在有限状态机状态</returns>
        bool HasState(Type stateType);

        /// <summary>
        /// 获取有限状态机状态///
        /// </summary>
        /// <typeparam name="TState">要获取的有限状态机状态类型</typeparam>
        /// <returns>要获取的有限状态机状态</returns>
        TState GetState<TState>() where TState : FsmState;

        /// <summary>
        /// 获取有限状态机状态///
        /// </summary>
        /// <param name="stateType">要获取的有限状态机状态类型</param>
        /// <returns>要获取的有限状态机状态</returns>
        FsmState GetState(Type stateType);

        /// <summary>
        /// 获取有限状态机的所有状态///
        /// </summary>
        /// <returns>有限状态机的所有状态</returns>
        FsmState[] GetAllStates();

        /// <summary>
        /// 获取有限状态机的所有状态///
        /// </summary>
        /// <param name="results">有限状态机的所有状态</param>
        void GetAllStates(List<FsmState> results);

        /// <summary>
        /// 抛出有限状态机事件///
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventId">事件编号</param>
        void TriggerEvent(object sender, int eventId);

        /// <summary>
        /// 抛出有限状态机事件///
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="eventId">事件编号</param>
        /// <param name="userData">用户自定义数据</param>
        void TriggerEvent(object sender, int eventId, object userData);
    }
}
