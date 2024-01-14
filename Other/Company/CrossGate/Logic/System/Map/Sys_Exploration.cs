using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    /// <summary> 探索系统 </summary>
    public partial class Sys_Exploration : SystemModuleBase<Sys_Exploration>, ISystemModuleUpdate
    {
        #region 数据定义
        /// <summary> 事件 </summary>
        public enum EEvents
        {
            NoticeViewState, //通知界面状态
            ShowOrHideView,  //显示或隐藏界面
            ExplorationRewardNotice, //探索奖励通知
        }
        /// <summary> 提示类型 </summary>
        public enum ETipType
        {
            None,          //无提示
            IncompleteTip, //未激活提示
            ActivatitonTip,//已激活提示
        }
        /// <summary> 资源类型显示排序 </summary>
        private List<ENPCMarkType> list_ResTypeSort = new List<ENPCMarkType>()
        {
            ENPCMarkType.LoveTask,      //爱心任务
            ENPCMarkType.ChallengeTask, //挑战任务
            ENPCMarkType.Transmit,      //传送点
            ENPCMarkType.Exploration,   //探索点
            ENPCMarkType.Resources,     //资源点
            ENPCMarkType.Trade,         //贸易点
        };
        /// <summary> 探索进度 </summary>
        public struct ExplorationProcess
        {
            public uint CurNum;     //当前进度
            public uint TargetNum;  //目标进度
            public bool isFinish    //当前是否完成
            {
                get
                {
                    return CurNum >= TargetNum && TargetNum != 0;
                }
            }
        }
        /// <summary> 探索数据 </summary>
        public class ExplorationData
        {
            /// <summary> 类型 1王国 2岛屿 3 地图 </summary>
            public uint mapType;
            /// <summary> 地图编号 </summary>
            public uint mapId;
            /// <summary> 需要显示资源对应的进度 </summary>
            public Dictionary<ENPCMarkType, ExplorationProcess> dict_Process = new Dictionary<ENPCMarkType, ExplorationProcess>();
            /// <summary> 总进度 </summary>
            public ExplorationProcess totalProcess;
            /// <summary> 探索奖励编号 </summary>
            public List<uint> list_ExplorationRewardID = new List<uint>();
            /// <summary> 构建函数 </summary>
            public ExplorationData(CSVMapExplorationReward.Data cSVMapExplorationRewardData)
            {
                this.mapType = cSVMapExplorationRewardData.type;
                this.mapId = cSVMapExplorationRewardData.mapId;
                this.list_ExplorationRewardID.Add(cSVMapExplorationRewardData.id);
                
                for (int i = 0, count = cSVMapExplorationRewardData.resource.Count; i < count; i++)
                {
                    var item = cSVMapExplorationRewardData.resource[i];
                    ENPCMarkType eNPCMarkType = (ENPCMarkType)item[0];
                    uint TargetNum = item[1];
                    ExplorationProcess explorationProcess = new ExplorationProcess();
                    explorationProcess.CurNum = 0;
                    explorationProcess.TargetNum = TargetNum;
                    dict_Process.Add(eNPCMarkType, explorationProcess);
                }
                UpdateTotalProcess();
            }
            /// <summary>
            /// 更新总进度
            /// </summary>
            public void UpdateTotalProcess()
            {
                uint curNum = 0, targetNum = 0;
                foreach (var item in dict_Process.Values)
                {
                    curNum += item.CurNum;
                    targetNum += item.TargetNum;
                }
                totalProcess.CurNum = curNum;
                totalProcess.TargetNum = targetNum;
            }
            /// <summary>
            /// 更新子进度
            /// </summary>
            /// <param name="eNPCMarkType"></param>
            /// <param name="CurNum"></param>
            /// <param name="TargetNum"></param>
            public void UpdateProcess(ENPCMarkType eNPCMarkType, uint CurNum)
            {
                if (dict_Process.ContainsKey(eNPCMarkType))
                {
                    ExplorationProcess explorationProcess = dict_Process[eNPCMarkType];
                    explorationProcess.CurNum = CurNum;
                    dict_Process[eNPCMarkType] = explorationProcess;
                }
            }
            /// <summary> 领奖进度 </summary>
            public ExplorationProcess GetRewardProcess(uint id)
            {
                ExplorationProcess explorationProcess = new ExplorationProcess();
                CSVMapExplorationReward.Data cSVMapExplorationRewardData = CSVMapExplorationReward.Instance.GetConfData(id);
                if (null == cSVMapExplorationRewardData) return explorationProcess;

                explorationProcess.CurNum = totalProcess.CurNum;
                explorationProcess.TargetNum = (uint)Math.Ceiling(totalProcess.TargetNum * ((float)cSVMapExplorationRewardData.ExplorationDegree / (float)10000));
                return explorationProcess;
            }
            /// <summary> 子进度 </summary>
            public ExplorationProcess GetExplorationProcess(ENPCMarkType eNPCMarkType)
            {
                ExplorationProcess explorationProcess;
                dict_Process.TryGetValue(eNPCMarkType, out explorationProcess);
                return explorationProcess;
            }
            /// <summary> 是否包含某个资源类型 </summary>
            public bool IsContainsMarkType(ENPCMarkType eNPCMarkType)
            {
                return dict_Process.ContainsKey(eNPCMarkType);
            }
            /// <summary> 可领取奖励列表 </summary>
            public List<uint> GetReceiveRewardList()
            {
                List<uint> list = new List<uint>();

                for (int i = 0, count = list_ExplorationRewardID.Count; i < count; i++)
                {
                    uint id = list_ExplorationRewardID[i];
                    bool isGetReward = Sys_Npc.Instance.IsGetRewards(id);
                    bool isFinish = GetRewardProcess(id).isFinish;
  
                    if(!isGetReward && isFinish)
                    {
                        list.Add(id);
                    }
                }
                return list;
            }
            /// <summary> 是否全部的奖励都被领取 </summary>
            public bool IsReceivedAllReward()
            {
                bool IsComplete = true;
                for (int i = 0; i < list_ExplorationRewardID.Count; i++)
                {
                    uint id = list_ExplorationRewardID[i];

                    if (!Sys_Npc.Instance.IsGetRewards(id))
                    {
                        IsComplete = false;
                        break;
                    }
                }
                return IsComplete;
            }
        }
        /// <summary> 探索提示 </summary>
        public class ExplorationTipData
        {
            public uint explorationRewardId;   //探索奖励ID
            public uint totalCurNum;           //当前完成数
            public uint totalTargetNum;        //当前目标数

            public uint npcId;              //标记npcID
            public uint markType;           //标记类
            public uint subCurNum;          //标记完成数
            public uint subTargetNum;       //标记目标数

            public CSVMapExplorationReward.Data cSVMapExplorationRewardData { set; get; }
            public CSVNpc.Data cSVNpcData { set; get; }
            public CSVMapExplorationMark.Data cSVMapExplorationMarkData { set; get; }

            public void SetData(uint explorationRewardId, uint totalCurNum, uint totalTargetNum,
                uint npcId, uint markType, uint subCurNum, uint subTargetNum)
            {
                this.explorationRewardId = explorationRewardId;
                this.totalCurNum = totalCurNum;
                this.totalTargetNum = totalTargetNum;
                this.npcId = npcId;
                this.markType = markType;
                this.subCurNum = subCurNum;
                this.subTargetNum = subTargetNum;

                this.cSVMapExplorationRewardData = CSVMapExplorationReward.Instance.GetConfData(explorationRewardId);
                this.cSVNpcData = CSVNpc.Instance.GetConfData(npcId);
                this.cSVMapExplorationMarkData = CSVMapExplorationMark.Instance.GetConfData(markType);
            }

        }
        /// <summary> 提示指令 </summary>
        public class BehaviorOrder
        {
            /// <summary> 提示对应界面打开或关闭状态 </summary>
            public Dictionary<ETipType, bool> dict_ViewState = new Dictionary<ETipType, bool>();
            /// <summary> 提示类型 </summary>
            public ETipType eTipType { get; private set; }
            /// <summary> 是否锁定该指令 </summary>
            public bool islocking;
            /// <summary> 已通知界面显示 </summary>
            public bool isShowed { get; private set; } = false;
            /// <summary> 已通知界面隐藏 </summary>
            public bool isHided { get; private set; } = false;
            /// <summary> 当前数据 </summary>
            public ExplorationTipData curExplorationTipData { get; set; }
            public ENPCMarkType curNPCMarkType { get; set; }
            /// <summary>
            /// 构建函数
            /// </summary>
            public BehaviorOrder()
            {
                var values = System.Enum.GetValues(typeof(ETipType));
                foreach (ETipType eTipType in values)
                {
                    if (eTipType == ETipType.None) continue;
                    dict_ViewState.Add(eTipType, false);
                }
                Reset();
            }
            /// <summary>
            /// 设置当前数据
            /// </summary>
            /// <param name="eTipType"></param>
            public void SetData(ETipType eTipType)
            {
                Reset();
                this.eTipType = eTipType;
                switch (eTipType)
                {
                    case ETipType.ActivatitonTip:
                        curExplorationTipData = Sys_Exploration.Instance.targetExplorationTipData;
                        break;
                    case ETipType.IncompleteTip:
                        curNPCMarkType = Sys_Exploration.Instance.targetNPCMarkType;
                        break;
                }
                TryShowOrHideView(true);
            }
            /// <summary>
            /// 设置界面状态
            /// </summary>
            /// <param name="eTipType"></param>
            /// <param name="isOpen"></param>
            public void SetViewState(ETipType eTipType, bool isOpen)
            {
                if (eTipType == ETipType.None) return;
                dict_ViewState[eTipType] = isOpen;

                if (this.eTipType == eTipType)
                {
                    if (!isOpen)
                        Reset();
                    else
                        TryShowOrHideView(true);
                }
            }
            /// <summary>
            /// 尝试显示或隐藏界面
            /// </summary>
            /// <param name="isShow"></param>
            public void TryShowOrHideView(bool isShow)
            {
                if (isShow)
                {
                    if (!isShowed)
                        ShowOrHideView(true);
                }
                else
                {
                    if (!isHided)
                        ShowOrHideView(false);
                }
            }
            /// <summary>
            /// 实际显示或隐藏界面
            /// </summary>
            /// <param name="isShow"></param>
            public void ShowOrHideView(bool isShow)
            {
                if (eTipType == ETipType.None || !dict_ViewState[eTipType]) return;
                islocking = true;
                if (isShow) { isShowed = true; }
                else { isHided = true; }
                Sys_Exploration.Instance.eventEmitter.Trigger<ETipType, bool>(Sys_Exploration.EEvents.ShowOrHideView, eTipType, isShow);
            }
            /// <summary>
            /// 完成指令
            /// </summary>
            public void CompleteOrder()
            {
                switch (eTipType)
                {
                    case ETipType.ActivatitonTip:
                        {
                            Sys_Exploration.Instance.AddRemoveCache(curExplorationTipData);
                        }
                        break;
                    case ETipType.IncompleteTip:
                        {
                            Sys_Exploration.Instance.targetNPCMarkType = ENPCMarkType.None;
                        }
                        break;
                }
                ClearData();
                Reset();
            }
            /// <summary>
            /// 重置
            /// </summary>
            public void Reset()
            {
                eTipType = ETipType.None;
                islocking = false;
                isShowed = false;
                isHided = false;
            }
            /// <summary>
            /// 清理数据
            /// </summary>
            public void ClearData()
            {
                curExplorationTipData = null;
                curNPCMarkType = ENPCMarkType.None;
            }
        }

        /// <summary> 事件 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        /// <summary> 探索数据字典</summary>
        private Dictionary<uint, ExplorationData> dict_ExplorationData = new Dictionary<uint, ExplorationData>();
        /// <summary> 探索领奖通知列表 </summary>
        public List<uint> list_ExplorationRewardNotice = new List<uint>();
        /// <summary> 限时池 </summary>
        private List<ExplorationTipData> limitedTime = new List<ExplorationTipData>();
        /// <summary> 移除缓存池 </summary>
        private List<ExplorationTipData> removeCache = new List<ExplorationTipData>();
        /// <summary> 当前需要显示的任务数据 </summary>
        public ExplorationTipData targetExplorationTipData { get; private set; }
        /// <summary> 当前需要npc标记 </summary>
        public ENPCMarkType targetNPCMarkType;
        /// <summary> 当前正在执行的指令 </summary>
        public BehaviorOrder curbehaviorOrder = new BehaviorOrder();
        #endregion
        #region 系统函数
        public override void Init()
        {
            OnProcessEvents(true);
        }
        public override void Dispose()
        {
            OnProcessEvents(false);
        }
        public override void OnLogin()
        {
            SetData();
        }
        public override void OnLogout()
        {
            Clear();
        }
        public override void OnSyncFinished()
        {
        }
        public void OnUpdate()
        {
            //获取当前特殊任务数据
            if (removeCache.Count > 0)
            {
                foreach (var item in removeCache)
                {
                    RemoveExplorationTip(item);
                }
                removeCache.Clear();
            }
            if (limitedTime.Count > 0) //限时池有数据
            {
                if (targetExplorationTipData != limitedTime[0])
                {
                    targetExplorationTipData = limitedTime[0];
                }
            }
            else
            {
                if (null != targetExplorationTipData)
                    targetExplorationTipData = null;
            }
            //获取指令
            switch (curbehaviorOrder.eTipType)
            {
                case ETipType.None://指令无状态
                    {
                        if (targetExplorationTipData != null)
                        {
                            curbehaviorOrder.SetData(ETipType.ActivatitonTip);
                        }
                        else if (targetNPCMarkType != ENPCMarkType.None)
                        {
                            curbehaviorOrder.SetData(ETipType.IncompleteTip);//最后是未完成提示
                        }
                    }
                    break;
                case ETipType.ActivatitonTip:
                    {
                        if (curbehaviorOrder.curExplorationTipData != targetExplorationTipData)
                        {
                            if (!curbehaviorOrder.islocking)//未被锁定
                            {
                                curbehaviorOrder.Reset(); //指令重置
                            }
                            else if (!curbehaviorOrder.isHided)//未通知隐藏界面
                            {
                                curbehaviorOrder.TryShowOrHideView(false);//隐藏界面
                            }
                        }
                    }
                    break;
                case ETipType.IncompleteTip:
                    {
                        if (null != targetExplorationTipData || curbehaviorOrder.curNPCMarkType != targetNPCMarkType)
                        {
                            if (!curbehaviorOrder.islocking)//未被锁定
                            {
                                curbehaviorOrder.CompleteOrder(); //指令重置
                            }
                            else if (!curbehaviorOrder.isHided)//未通知隐藏界面
                            {
                                curbehaviorOrder.TryShowOrHideView(false);//隐藏界面
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void OnProcessEvents(bool toRegister)
        {
            Sys_Exploration.Instance.eventEmitter.Handle<ETipType, bool>(Sys_Exploration.EEvents.NoticeViewState, OnNoticeViewState, toRegister);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, OnSwitchMap, toRegister);
            Sys_Npc.Instance.eventEmitter.Handle(Sys_Npc.EEvents.OnUpdateResPoint, OnUpdateResPoint, toRegister);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnCompletedFunctionOpen, toRegister);
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetData()
        {
            dict_ExplorationData.Clear();
            var data = CSVMapExplorationReward.Instance.GetAll();
            foreach (var kvp in data)
            {
                ExplorationData explorationData = null;
                if (!dict_ExplorationData.TryGetValue(kvp.mapId, out explorationData))
                {
                    if (null == kvp || null == kvp.resource || kvp.resource.Count <= 0)
                        continue;

                    explorationData = new ExplorationData(kvp);
                    dict_ExplorationData.Add(kvp.mapId, explorationData);
                }
                else
                {
                    explorationData.list_ExplorationRewardID.Add(kvp.id); //探索奖励ID
                }
            }
        }
        #endregion
        #region 数据处理
        /// <summary>
        /// 更新探索数据
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="pP"></param>
        public void UpdateExplorationData(uint mapId, List<Sys_Npc.NpcActivatedData.MapProcess.PP> ppList)
        {
            ExplorationData explorationData = GetExplorationData(mapId);
            if (null == explorationData) return;
            //子进度更新
            for (int i = 0; i < ppList.Count; i++)
            {
                var pp = ppList[i];
                explorationData.UpdateProcess((ENPCMarkType)pp.markType, pp.markCount);
            }
            //总进度更新
            explorationData.UpdateTotalProcess();
            //领奖通知
            for (int i = 0; i < explorationData.list_ExplorationRewardID.Count; i++)
            {
                uint rewardID = explorationData.list_ExplorationRewardID[i];
                if (explorationData.GetRewardProcess(rewardID).isFinish && !Sys_Npc.Instance.IsGetRewards(rewardID))//完成且未领奖提示
                {
                    OnExplorationRewardNotice(rewardID, true);
                }
            }
        }
        /// <summary>
        /// 通知奖励
        /// </summary>
        /// <param name="rewardID"></param>
        /// <param name="isAddOrRemove"></param>
        public void OnExplorationRewardNotice(uint rewardID, bool isAddOrRemove)
        {
            if (isAddOrRemove)
            {
                if (!list_ExplorationRewardNotice.Contains(rewardID))
                    list_ExplorationRewardNotice.Add(rewardID);
            }
            else
            {
                if (list_ExplorationRewardNotice.Contains(rewardID))
                    list_ExplorationRewardNotice.Remove(rewardID);
            }
            Sys_Exploration.Instance.eventEmitter.Trigger(Sys_Exploration.EEvents.ExplorationRewardNotice);
        }
        /// <summary>
        /// 添加探索数据
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="npcId"></param>
        /// <param name="markType"></param>
        public void AddExplorationTip(uint mapId, uint npcId, uint markType)
        {
            ExplorationData explorationData = GetExplorationData(mapId);
            if (null == explorationData) return;

            ExplorationProcess explorationProcess = explorationData.GetExplorationProcess((ENPCMarkType)markType);
            ExplorationTipData explorationTipData = new ExplorationTipData();
            explorationTipData.SetData(explorationData.list_ExplorationRewardID[0],
                explorationData.totalProcess.CurNum,
                explorationData.totalProcess.TargetNum,
                npcId,
                markType,
                explorationProcess.CurNum,
                explorationProcess.TargetNum);
            limitedTime.Add(explorationTipData);
        }
        /// <summary>
        /// 添加缓冲移除数据
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="removeCacheType"></param>
        public void AddRemoveCache(ExplorationTipData explorationTipData)
        {
            removeCache.Add(explorationTipData);
        }
        /// <summary>
        /// 移除探索数据
        /// </summary>
        /// <param name="removeCacheData"></param>
        public void RemoveExplorationTip(ExplorationTipData explorationTipData)
        {
            int index = limitedTime.FindIndex(x => x.explorationRewardId == explorationTipData.explorationRewardId);
            if (index >= 0) limitedTime.RemoveAt(index);
        }
        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            limitedTime.Clear();
            removeCache.Clear();
            list_ExplorationRewardNotice.Clear();
            dict_ExplorationData.Clear();
            targetExplorationTipData = null;
            targetNPCMarkType = ENPCMarkType.None;
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 切换界面状态
        /// </summary>
        /// <param name="eTipType"></param>
        /// <param name="isShow"></param>
        public void OnNoticeViewState(ETipType eTipType, bool isOpen)
        {
            curbehaviorOrder.SetViewState(eTipType, isOpen);
        }
        /// <summary>
        /// 切换地图
        /// </summary>
        private void OnSwitchMap()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(50302, false))
                return;
            targetNPCMarkType = GetMarkType();
        }
        /// <summary>
        /// 更新资源点时
        /// </summary>
        private void OnUpdateResPoint()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(50302, false)) return;
            if (targetNPCMarkType == ENPCMarkType.None) return; //当前地图已查看过标记时不再显示
            targetNPCMarkType = GetMarkType();
        }
        /// <summary>
        /// 完成功能开启
        /// </summary>
        /// <param name="functionOpenData"></param>
        private void OnCompletedFunctionOpen(Sys_FunctionOpen.FunctionOpenData functionOpenData)
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(50302, false)) return;
            ENPCMarkType eNPCMarkType = (ENPCMarkType)functionOpenData.id;
            if (!list_ResTypeSort.Contains(eNPCMarkType) || targetNPCMarkType != ENPCMarkType.None) return;
            targetNPCMarkType = GetMarkType();
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 设置当前显示的标志类型
        /// </summary>
        private ENPCMarkType GetMarkType()
        {
            uint mapId = Sys_Map.Instance.CurMapId;
            ENPCMarkType eNPCMarkType = ENPCMarkType.None;

            foreach (var markType in list_ResTypeSort)
            {
                if (!Sys_Exploration.Instance.IsOpen_ResPoint((uint)markType)) //资源点已激活
                    continue;

                ExplorationData explorationData = Sys_Exploration.Instance.GetExplorationData(mapId);
                if (null == explorationData) continue;
                if (!explorationData.IsContainsMarkType(markType)) continue;
                if (explorationData.GetExplorationProcess(markType).isFinish) continue;

                eNPCMarkType = markType;
                break;
            }
            return eNPCMarkType;
        }
        /// <summary>
        /// 资源点是否开启功能
        /// </summary>
        /// <param name="markType"></param>
        /// <returns></returns>
        public bool IsOpen_ResPoint(uint markType)
        {
            uint openID = GetResPointFunctionOpenId(markType);
            bool isOpen = openID == 0 ? true : Sys_FunctionOpen.Instance.IsOpen(openID, false);
            return isOpen;
        }
        /// <summary>
        /// 得到资源点功能开启ID
        /// </summary>
        /// <param name="eNPCMarkType"></param>
        /// <returns></returns>
        public uint GetResPointFunctionOpenId(uint markType)
        {
            CSVMapExplorationMark.Data cSVMapExplorationMarkData = CSVMapExplorationMark.Instance.GetConfData(markType);
            return cSVMapExplorationMarkData == null ? 0 : cSVMapExplorationMarkData.function_id;
        }
        /// <summary>
        /// 获取地图进度
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public ExplorationData GetExplorationData(uint mapId)
        {
            ExplorationData explorationData = null;
            dict_ExplorationData.TryGetValue(mapId, out explorationData);
            return explorationData;
        }
        #endregion
    }
}