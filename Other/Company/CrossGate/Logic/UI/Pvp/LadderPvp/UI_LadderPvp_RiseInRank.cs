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
    public partial class UI_LadderPvp_RiseInRank : UIBase, UI_LadderPvp_RiseInRank_Layout.IListener
    {
        UI_LadderPvp_RiseInRank_Layout m_Layout = new UI_LadderPvp_RiseInRank_Layout();

        // private int m_Stage = -1;

        private List<CSVTianTiTask.Data> m_TaskData = new List<CSVTianTiTask.Data>();

        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.GetDanLvUpAward, OnRankRefresh, toRegister);
            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.GetAllDanLvUpAward, OnRankRefresh, toRegister);

        }

        protected override void OnOpen(object arg)
        {

            int count = CSVTianTiTask.Instance.Count;

            for (int i = 0; i < count; i++)
            {
                var data = CSVTianTiTask.Instance.GetByIndex(i);

                if (data != null && data.MissionType == 1)
                {
                    m_TaskData.Add(data);
                }
            }


        }
        protected override void OnShow()
        {
            RefreshInfo();
        }


        private uint GetTaskState(uint id)
        {
            if (Sys_LadderPvp.Instance.MyInfoRes.Task1 == null)
                return (uint)TTDanLvTaskStatus.None;

            var result = Sys_LadderPvp.Instance.MyInfoRes.Task1.Tasks.Find(o => o.TaskId == id);

            if (result == null)
                return (uint)TTDanLvTaskStatus.None;

            return result.Status;
        }
        private void RefreshData()
        {

            int count = m_TaskData.Count;
            m_Layout.m_ItemGroup.SetChildSize(count);

            for (int i = 0; i < count; i++)
            {
                var item = m_Layout.m_ItemGroup.getAt(i);

                item.m_TexLabel.text = LanguageHelper.GetTextContent(m_TaskData[i].Task_Des);
                var rewardids = CSVDrop.Instance.GetDropItem(m_TaskData[i].Reward);
                item.SetReward(rewardids);

                uint state = GetTaskState(m_TaskData[i].id);

                item.SetHadGetActive(state == (uint)TTDanLvTaskStatus.Got);
                item.SetGetActive(state != (uint)TTDanLvTaskStatus.Got, state == (uint)TTDanLvTaskStatus.NotGet);

                item.Index = i;
            }

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
    public partial class UI_LadderPvp_RiseInRank : UIBase, UI_LadderPvp_RiseInRank_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickOneKeyGet()
        {
            Sys_LadderPvp.Instance.Apply_GetAllTaskAward(1);
        }

        public void OnClickRankItemGet(int index)
        {
            if (index >= m_TaskData.Count)
                return;

            var data = m_TaskData[index];
            Sys_LadderPvp.Instance.Apply_GetTaskAward(data.id);
        }


        private void OnRankRefresh()
        {
            RefreshInfo();
        }





    }
}
