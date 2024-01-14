using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Packet;

namespace Logic
{
    /// <summary>
    /// 晋级奖励
    /// </summary>
    public partial class UI_LadderPvp_Task : UIBase
    {
        UI_LadderPvp_Task_Layout m_Layout = new UI_LadderPvp_Task_Layout();

        // private int m_Stage = -1;

        private List<CSVTianTiTask.Data> m_TaskData = new List<CSVTianTiTask.Data>();

        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.GetTaskAward, OnRankRefresh, toRegister);
        }

        protected override void OnOpen(object arg)
        {


            int count = CSVTianTiTask.Instance.Count;

            for (int i = 0; i < count; i++)
            {
                var data = CSVTianTiTask.Instance.GetByIndex(i);

                if (data != null && data.MissionType != 1)
                {
                    m_TaskData.Add(data);
                }
            }


        }
        protected override void OnShow()
        {
            RefreshInfo();
        }

        private void RefreshData()
        {
            int count = m_TaskData.Count;

            m_Layout.m_InfinityGrid.CellCount = count;
            m_Layout.m_InfinityGrid.MoveToIndex(0);
            m_Layout.m_InfinityGrid.ForceRefreshActiveCell();
        }


        private bool RefreshInfo()
        {
            RefreshData();

            return true;
        }

        private void RefreshSelect()
        {

        }

    }

    /// <summary>
    /// 监听处理
    /// </summary>
    public partial class UI_LadderPvp_Task : UIBase, UI_LadderPvp_Task_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickOneKeyGet()
        {
            //Sys_LadderPvp.Instance.Apply_GetAllDanUpAward();
        }

        public void OnClickRankItemGet(int index)
        {
            if (index >= m_TaskData.Count)
                return;

            var tdata = m_TaskData[index];

            Sys_LadderPvp.Instance.Apply_GetTaskAward(tdata.id);
        }

        public void OnInfinityCellCreate(InfinityGridCell cell)
        {
            UI_LadderPvp_Task_Layout.RectItem item = new UI_LadderPvp_Task_Layout.RectItem();

            item.Load(cell.mRootTransform);

            item.SetOnClickListener(OnClickRankItemGet);

            cell.BindUserData(item);
        }

        public void OnInfinityUpdate(InfinityGridCell cell, int index)
        {
            var item = cell.mUserData as UI_LadderPvp_Task_Layout.RectItem;

            if (item == null)
                return;

            var tdata = m_TaskData[index];

            item.Index = index;

            item.m_TexLabel.text = LanguageHelper.GetTextContent(tdata.Task_Des);

            var rewardids = CSVDrop.Instance.GetDropItem(tdata.Reward);

            item.SetReward(rewardids);

            string processtr = string.Empty;

            var taskresult = Sys_LadderPvp.Instance.MyInfoRes.Task2 == null ? null : Sys_LadderPvp.Instance.MyInfoRes.Task2.Tasks.Find(o => o.TaskId == tdata.id);

            uint taskstate = taskresult == null ? (uint)TTDanLvTaskStatus.None : taskresult.Status;

            bool hadGet = taskstate == (uint)TTDanLvTaskStatus.Got;
            if (!hadGet)
            {

                uint process = taskresult == null ? 0 : taskresult.Progress;

                if (tdata.ReachTypeAchievement != null && tdata.ReachTypeAchievement.Count > 0)
                {
                    process = process > tdata.ReachTypeAchievement[0] ? tdata.ReachTypeAchievement[0] : process;

                    processtr = process.ToString();
                    processtr += "/";
                    processtr += tdata.ReachTypeAchievement[0].ToString();
                }

            }

            item.m_TexProcess.text = processtr;

            item.m_TransHadGet.gameObject.SetActive(hadGet);

            bool btnin = taskstate == (uint)TTDanLvTaskStatus.NotGet;

            item.SetGetActive(!hadGet, btnin);
        }

        private void OnRankRefresh()
        {
            m_Layout.m_InfinityGrid.ForceRefreshActiveCell();
            m_Layout.m_InfinityGrid.Apply();
        }





    }
}
