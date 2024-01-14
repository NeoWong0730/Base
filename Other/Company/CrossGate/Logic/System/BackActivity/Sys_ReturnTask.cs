using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;

namespace Logic
{
    public class Sys_ReturnTask : SystemModuleBase<Sys_ReturnTask>, ISystemModuleUpdate
    {
        public enum EEvents
        {
            OnTaskStateUpdate, //任务状态更新
            OnPassDay, //跨天
            OnUpdatePoint, //更新积分
        }

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public List<ReturnTaskData> returnTaskDatas = new List<ReturnTaskData>(); //长度为天数的长度 

        private Dictionary<uint, SingleTaskData> m_TaskMap = new Dictionary<uint, SingleTaskData>(); //方便数据查询  key:taskId

        public Dictionary<uint, bool> points = new Dictionary<uint, bool>(); //显示的积分id列表 key:积分id  value:是否领取

        public uint openTime;

        public uint activityGroup;

        public int totalDays = 7;

        public uint point;

        public int getPointIndex; //可以领取的积分最大索引值

        private int m_CurDay;

        public int curDay
        {
            get { return m_CurDay; }
            set
            {
                if (m_CurDay != value)
                {
                    m_CurDay = value;
                    eventEmitter.Trigger(EEvents.OnPassDay);
                }
            }
        }

        private bool b_ActivityOpen;

        public bool activityOpen
        {
            get { return b_ActivityOpen; }
            set
            {
                if (b_ActivityOpen != value)
                {
                    b_ActivityOpen = value;
                }
            }
        }

        //一天的任务数据
        public class ReturnTaskData
        {
            public bool unLuck;

            public List<SingleTaskData> singleTaskDatas = new List<SingleTaskData>();


            //可领取>未完成>已领取
            public void Sort()
            {
                singleTaskDatas.Sort((x, y) => x.state.CompareTo(y.state));
            }

            public void Clear()
            {
                singleTaskDatas.Clear();
            }
        }

        //单条任务数据
        public class SingleTaskData
        {
            public uint id;

            private uint m_Num;

            public uint num
            {
                get { return m_Num; }
                set
                {
                    if (m_Num != value)
                    {
                        m_Num = value;
                        Sys_ReturnTask.Instance.eventEmitter.Trigger<int>(EEvents.OnTaskStateUpdate, (int)csv.RankType - 1);
                    }
                }
            }

            public uint need { get; private set; }

            private bool m_TakeReward;

            public bool takeReward
            {
                get { return m_TakeReward; }
                set
                {
                    if (m_TakeReward != value)
                    {
                        m_TakeReward = value;
                        Sys_ReturnTask.Instance.eventEmitter.Trigger<int>(EEvents.OnTaskStateUpdate, (int)csv.RankType - 1);
                    }
                }
            }

            public CSVReturnChanllengeTask.Data csv;

            public int state //0:可领取 1:未完成 2:已领取 
            {
                get
                {
                    if (takeReward)
                    {
                        return 2;
                    }
                    else
                    {
                        return m_Num < need ? 1 : 0;
                    }
                }
            }

            public SingleTaskData(uint id)
            {
                this.id = id;
                csv = CSVReturnChanllengeTask.Instance.GetConfData(id);
                need = csv.ReachTypeAchievement[csv.ReachTypeAchievement.Count - 1];
            }
        }

        public override void Init()
        {
            openTime = 0;
            ProcessEvents(true);
            for (int i = 0; i < totalDays; i++)
            {
                ReturnTaskData data = new ReturnTaskData();
                returnTaskDatas.Add(data);
            }
        }

        public override void Dispose()
        {
            ProcessEvents(false);
        }

        private void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener(0, (ushort) CmdActivityReturn.TaskUpdateNtf, OnTaskUpdateNtf, CmdActivityReturnTaskUpdateNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort) CmdActivityReturn.TaskRewardReq, (ushort) CmdActivityReturn.TaskRewardRes, OnTaskRewardRes, CmdActivityReturnTaskRewardRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort) CmdActivityReturn.TaskPointRewardReq, (ushort) CmdActivityReturn.TaskPointRewardRes, OnTaskPointRewardRes, CmdActivityReturnTaskPointRewardRes.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort) CmdActivityReturn.TaskUpdateNtf, OnTaskUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort) CmdActivityReturn.TaskRewardRes, OnTaskRewardRes);
                EventDispatcher.Instance.RemoveEventListener((ushort) CmdActivityReturn.TaskPointRewardRes, OnTaskPointRewardRes);
            }
        }

        //根据服务器上线发送给客户端的数据 过滤表格 筛查出需要显示的条目
        private void ProcessData(CmdActivityReturnInfoNtf ntf)
        {
            m_TaskMap.Clear();

            for (int i = 0; i < returnTaskDatas.Count; i++)
            {
                returnTaskDatas[i].Clear();
            }

            //task
            for (int i = 0; i < CSVReturnChanllengeTask.Instance.GetAll().Count; i++)
            {
                var data = CSVReturnChanllengeTask.Instance.GetByIndex(i);

                //过滤出表格中group一样的数据
                if (data.Activity_Group != activityGroup)
                    continue;

                int index = (int) data.RankType - 1;

                SingleTaskData sd = new SingleTaskData(data.id);

                returnTaskDatas[index].singleTaskDatas.Add(sd);

                m_TaskMap.Add(data.id, sd);
            }

            for (int i = 0; i < ntf.ReturnTaskData.TaskList.Count; i++)
            {
                var item = ntf.ReturnTaskData.TaskList[i];

                SingleTaskData data = GetSingleTaskDataByTaskID(item.TaskId);

                data.num = item.Num;

                data.takeReward = item.TakeReward;
            }

            //point
            points.Clear();
            for (int i = 0; i < CSVReturnChanllengeIntegral.Instance.GetAll().Count; i++)
            {
                var data = CSVReturnChanllengeIntegral.Instance.GetByIndex(i);

                if (data.Activity_Group != activityGroup)
                    continue;

                points.Add(data.id, false);
            }

            for (int i = 0; i < ntf.ReturnTaskData.PointReward.Count; i++)
            {
                uint pointId = ntf.ReturnTaskData.PointReward[i];

                points[pointId] = true;
            }

            point = ntf.ReturnTaskData.Point;
            UpdateMaxPointCanGet();
        }

        private SingleTaskData GetSingleTaskDataByTaskID(uint id)
        {
            m_TaskMap.TryGetValue(id, out SingleTaskData sd);

            if (sd == null)
            {
                DebugUtil.LogErrorFormat($"returnTaskId not found{id}");

                return null;
            }

            return sd;
        }


        public void UpdateReturnTaskData(CmdActivityReturnInfoNtf ntf, uint openTime)
        {
            this.openTime = openTime;

            if (openTime > 0)
            {
                activityOpen = true;
            }

            activityGroup = ntf.ActivityGroup;

            ProcessData(ntf);
        }


        private void UpdateMaxPointCanGet()
        {
            getPointIndex = -1;

            foreach (var item in points)
            {
                uint requirePoint = CSVReturnChanllengeIntegral.Instance.GetConfData(item.Key).Requiredpoints;

                if (point >= requirePoint)
                    ++getPointIndex;
                else
                    return;
            }
        }

        public void SortTask(int day)
        {
            returnTaskDatas[day].Sort();
        }

        public bool HasRed(int day)
        {
            //先判断当前天是否解锁
            if (day <= curDay)
            {
                for (int i = 0; i < returnTaskDatas[day].singleTaskDatas.Count; i++)
                {
                    SingleTaskData st = returnTaskDatas[day].singleTaskDatas[i];

                    if (st.state == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasAllRed()
        {
            for (int i = 0; i < totalDays; i++)
            {
                if (HasRed(i))
                {
                    return true;
                }
            }
            //积分奖励红点
            foreach (var item in points)
            {
                uint pointId = item.Key;
                CSVReturnChanllengeIntegral.Data data = CSVReturnChanllengeIntegral.Instance.GetConfData(pointId);
                bool canGet = Sys_ReturnTask.Instance.point >= data.Requiredpoints;
                bool isGet = item.Value;
                if(canGet && !isGet)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AllTaskOver(int day)
        {
            for (int i = 0; i < returnTaskDatas[day].singleTaskDatas.Count; i++)
            {
                SingleTaskData st = returnTaskDatas[day].singleTaskDatas[i];

                if (st.state != 2)
                {
                    return false;
                }
            }

            return true;
        }

        #region 服务器消息

        private void OnTaskUpdateNtf(NetMsg netMsg)
        {
            CmdActivityReturnTaskUpdateNtf ntf = NetMsgUtil.Deserialize<CmdActivityReturnTaskUpdateNtf>(CmdActivityReturnTaskUpdateNtf.Parser, netMsg);

            SingleTaskData sd = GetSingleTaskDataByTaskID(ntf.TaskId);
            sd.num = ntf.Num;
            Sys_BackActivity.Instance.eventEmitter.Trigger(Sys_BackActivity.EEvents.OnBackActivityRedPointUpdate);
        }

        public void TaskRewardReq(uint taskId)
        {
            CmdActivityReturnTaskRewardReq req = new CmdActivityReturnTaskRewardReq();
            req.TaskId = taskId;
            NetClient.Instance.SendMessage((ushort) CmdActivityReturn.TaskRewardReq, req);
        }

        private void OnTaskRewardRes(NetMsg netMsg)
        {
            CmdActivityReturnTaskRewardRes ntf = NetMsgUtil.Deserialize<CmdActivityReturnTaskRewardRes>(CmdActivityReturnTaskRewardRes.Parser, netMsg);

            SingleTaskData sd = GetSingleTaskDataByTaskID(ntf.TaskId);

            if (sd != null)
            {
                sd.takeReward = ntf.TakeReward;
            }

            point = ntf.Point;
            eventEmitter.Trigger(EEvents.OnUpdatePoint);
            Sys_BackActivity.Instance.eventEmitter.Trigger(Sys_BackActivity.EEvents.OnBackActivityRedPointUpdate);
        }

        public void TaskPointRewardReq(uint infoId)
        {
            CmdActivityReturnTaskPointRewardReq req = new CmdActivityReturnTaskPointRewardReq();
            req.InfoId = infoId;

            NetClient.Instance.SendMessage((ushort) CmdActivityReturn.TaskPointRewardReq, req);
        }

        private void OnTaskPointRewardRes(NetMsg netMsg)
        {
            CmdActivityReturnTaskPointRewardRes res = NetMsgUtil.Deserialize<CmdActivityReturnTaskPointRewardRes>(CmdActivityReturnTaskPointRewardRes.Parser, netMsg);

            points[res.InfoId] = true;
            
            eventEmitter.Trigger(EEvents.OnUpdatePoint);
            Sys_BackActivity.Instance.eventEmitter.Trigger(Sys_BackActivity.EEvents.OnBackActivityRedPointUpdate);
        }

        #endregion
        public void OnUpdate()
        {
            uint currentTime = Sys_Time.Instance.GetServerTime();

            if (currentTime < openTime)
            {
                curDay = 0;
                return;
            }

            uint passTime = currentTime - openTime;
            
            curDay = (int) passTime / 86400;
            
            activityOpen = curDay <= totalDays - 1;
        }

        /// <summary>
        /// 获取累计奖励积分进度条的宽度
        /// </summary>
        public float GetSevenDaysTargetScoreSliderWidth(uint cellWidth, uint maxWidth)
        {
            var myScore = point;
            float width = 0;
            uint startIndex = 0;
            uint LastScore = 0;//上一阶段分数
            uint diffScore = 0;//当前阶段间的分数差
            uint LastDiffScore = 0;//上一阶段间的分数差
            foreach (var item in points)
            {
                uint pointId = item.Key;
                CSVReturnChanllengeIntegral.Data data = CSVReturnChanllengeIntegral.Instance.GetConfData(pointId);
                if (myScore >= data.Requiredpoints)
                {
                    startIndex++;
                    LastDiffScore = data.Requiredpoints - LastScore;
                    LastScore = data.Requiredpoints;
                }
                else
                {
                    diffScore = data.Requiredpoints - LastScore;
                    break;
                }
            }
            if (diffScore <= 0)
            {
                diffScore = LastDiffScore / 2;
            }
            if (myScore <= 0 || myScore < LastScore)
            {
                width = 0;
            }
            else
            {
                if (startIndex <= 0)
                {
                    //第一段只有一半
                    width = cellWidth / 2 * (myScore - LastScore) / diffScore;
                }
                else
                {

                    width = startIndex * cellWidth - cellWidth / 2 + cellWidth * (myScore - LastScore) / diffScore;
                }
                return width <= 0 ? 0 : (width >= maxWidth ? maxWidth : width);
            }
            return 0;
        }
        /// <summary>
        /// 检测回归试炼是否开启
        /// </summary>
        public bool CheckReturnTaskIsOpen()
        {
            return Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(218);
        }
    }
}