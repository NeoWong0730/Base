using System;
using System.Collections.Generic;
using Logic.Core;
using Net;
using Packet;

using Table;
using UnityEngine;

namespace Logic
{

    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        public enum EDailyTimerWorkType
        {
            None,
            PreTips,
            PreTipsClose,
            Open,
            Close,
            SendDataToServer,
        }

        class DailyTimerWork
        {
            /// <summary>
            /// 时间戳，
            /// </summary>
            public uint TimePoint { get; set; } = 0;

            public uint ID { get; set; } = 0;

            public int Index { get; set; } = 0;
            public EDailyTimerWorkType State { get; set; } = 0; 
        }

        /// <summary>
        /// 是一个按时间排序 由大到小 的顺序的 时间表，跨日需要刷新
        /// </summary>
        private List<DailyTimerWork> m_WorkList = new List<DailyTimerWork>();
        private int m_WorkListCount = 0;

        /// <summary>
        /// 添加一个时间工作。isSorted 为true，会导致真个工作列表重新排序。如果有需要添加大量的，应该在添加完在对列表进行重新排序
        /// </summary>
        /// <param name="dailyTimerWork"></param>
        /// <param name="isSorted"></param>
        private void PushTimeWork(DailyTimerWork dailyTimerWork,bool isSorted = false)
        {

            m_WorkList.Add(dailyTimerWork);

            if (isSorted)
                SortWorkList();

            m_WorkListCount = m_WorkList.Count;
        }

        public void PushTimeWork(uint timePoint , int index,uint id, EDailyTimerWorkType workType )
        {
            //if (id == 101)
            //{
            //    Debug.LogError("----加入日常工作101 " + timePoint.ToString() + "--- 类型--- " + workType.ToString());
            //}
            DailyTimerWork dailyTimerWork = new DailyTimerWork();
            dailyTimerWork.TimePoint = timePoint;
            dailyTimerWork.ID = id;
            dailyTimerWork.State = workType;
            dailyTimerWork.Index = index;
            PushTimeWork(dailyTimerWork, false);
        }

        /// <summary>
        /// 插入一个工作，会根据时间点按顺序插入
        /// </summary>
        /// <param name="timePoint"></param>
        /// <param name="index"></param>
        /// <param name="id"></param>
        /// <param name="workType"></param>

        public void InsertTimeWork(uint timePoint, int index, uint id, EDailyTimerWorkType workType)
        {

            DailyTimerWork dailyTimerWork = new DailyTimerWork();
            dailyTimerWork.TimePoint = timePoint;
            dailyTimerWork.ID = id;
            dailyTimerWork.State = workType;
            dailyTimerWork.Index = index;

            int count = m_WorkList.Count;

            bool hadinsert = false;

            for (int i = 0; i < count; i++)
            {
                if (m_WorkList[i].TimePoint <= timePoint)
                {
                    m_WorkList.Insert(i, dailyTimerWork);
                    hadinsert = true;
                    
                    break;
                }
            }

            if (count == 0 || !hadinsert)
            {
                PushTimeWork(dailyTimerWork);
            }

            m_WorkListCount = m_WorkList.Count;
        }


        public void RemoveTimeWork(uint dailyId)
        {
            m_WorkList.RemoveAll(o => o.ID == dailyId);
        }
        /// <summary>
        /// 删除列表底部的一个数据
        /// </summary>
        private void PopTimeWork()
        {
            m_WorkList.RemoveAt(m_WorkListCount - 1);
            m_WorkListCount = m_WorkList.Count;
        }

        /// <summary>
        /// 获得最近的一个时间工作
        /// </summary>
        /// <returns></returns>
        private DailyTimerWork GetLastTimeWork()
        {
            if (m_WorkListCount <= 0)
                return null;

            DailyTimerWork item = null;

            //做循环为了去掉空的时间工作
            for (int i = m_WorkListCount - 1; i >= 0; i--)
            {
                item = m_WorkList[i];

                if (item == null || item.State == EDailyTimerWorkType.None)
                    PopTimeWork();
                else
                {
                    break;
                }

            }
            
            return item;
        }
        private void SortWorkList()
        {
            m_WorkList.Sort((x, y) => {

                if (x.TimePoint < y.TimePoint)
                    return 1;

                if (x.TimePoint > y.TimePoint)
                    return -1;

                return 0;
            });
        }


        private void ClearWorkList()
        {
            m_WorkList.Clear();
            m_WorkListCount = 0;
        }
        /// <summary>
        /// 查找可以执行的工作，并从工作列表中删除
        /// </summary>
        /// <param name="serverTime"></param>
        private void DoTimeWork(uint serverTime)
        {
            
            for (int i = m_WorkListCount - 1; i >= 0; i--)
            {
                var item = m_WorkList[i];

                if (item.TimePoint > serverTime)
                    break;

                if (item != null && item.State != EDailyTimerWorkType.None)
                {

                    OnDoWork(item.ID,item.Index ,item.TimePoint,item.State);
                }

                PopTimeWork();
            }
        }
        private void UpdataTimerWork(uint serverTime)
        {
            if (m_WorkListCount <= 0)
                return;

            var item = GetLastTimeWork();

            if (item == null)
                return;

            //检查最近的一个工作，如果时间到达了，就检查一遍工作列表
            if (serverTime >= item.TimePoint)
            {
                DoTimeWork(serverTime);
            }
        }
    }


    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        private void UpdataTime(uint serverTime)
        {
            if (mTodayZeroTimePoint == 0)
                return;     

            var offset = serverTime - mTodayZeroTimePoint;

            if (offset > 86400)
            {
                var lastDay = mTodayZeroTimePoint;

                UpdataTodayTime();

                if (mTodayZeroTimePoint - lastDay >= 86400)
                {
                    ReGetAllWork();
                }
            }
        }
    }

    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>, ISystemModuleUpdate
    {
        public void OnUpdate()
        {
            var serverTime = Sys_Time.Instance.GetServerTime();

            UpdataTime(serverTime);

            UpdataTimerWork(serverTime);

            OnUpdataLife(serverTime);


            UpdataAfter();
        }

        private void UpdataAfter()
        {
            
        }

        private void ReGetAllWork()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(DailyFuncOpenID) == false)
                return;

            ClearWorkList();

            ClearNotice();

            InitFuncTimeWork();

            SortWorkList();
        }

        private void GetWorkDaily(uint id)
        {
            if (isTodayDaily(id) == false)
                return;

            if (InitFuncTimeWork(id))
                SortWorkList();
        }
        private void OnDoWork(uint id, int index,uint timePoint,EDailyTimerWorkType workType)
        {
            // Debug.LogError("----完成日常工作--- " + id.ToString() +"--- 时间-----" + timePoint.ToString() + "--- 类型--- " + workType.ToString());

            var dailyfunc = GetDailyFunc(id);

            if (workType == EDailyTimerWorkType.Close)
            {
                var data = CSVDailyActivity.Instance.GetConfData(id);

                if (data != null && data.IsShow > 0)
                {
                    var errorData = CSVErrorCode.Instance.GetConfData(data.IsShow);

                    ErrorCodeHelper.PushErrorCode(errorData.pos, errorData.words, string.Empty, data.IsShow, null, true);
                }

                dailyfunc.OnEndTime((uint)index);
            }
            else if (workType == EDailyTimerWorkType.PreTips)
            {
                dailyfunc.OnNotice((uint)index);
                eventEmitter.Trigger(EEvents.NewNotice);
            }
            else if (workType == EDailyTimerWorkType.PreTipsClose)
            {
                dailyfunc.OnNoticeEnd((uint)index);
                eventEmitter.Trigger(EEvents.RemoveNotice);
            }
            else if (workType == EDailyTimerWorkType.Open)
            {
                dailyfunc.OnOpenTime((uint)index);
            }
            else if (workType == EDailyTimerWorkType.SendDataToServer)
            {
                if (id == 101)
                {
                    Sys_SurvivalPvp.Instance.SendFinish();
                }
            }
        
        }

        public void DebugTimeWorkLog()
        {
            int count = m_WorkList.Count;

            Debug.LogError("time worke time " + Sys_Time.Instance.GetServerTime().ToString());
            for (int i = 0; i < count; i++)
            {
                var value = m_WorkList[i];

                Debug.LogError("value id :" + value.ID.ToString() + "  type :" + value.State.ToString() + " time" + value.TimePoint.ToString());
            }
        }
    }
}
