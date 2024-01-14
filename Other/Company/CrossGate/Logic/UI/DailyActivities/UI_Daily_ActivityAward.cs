using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;

namespace Logic
{
    public partial class UI_Daily_ActivityAward:UIBase,UI_Daily_ActivityAward_Layout.IListener
    {
        UI_Daily_ActivityAward_Layout m_Layout = new UI_Daily_ActivityAward_Layout();

        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnShow()
        {            
            Refresh();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Daily.Instance.eventEmitter.Handle<uint>(Sys_Daily.EEvents.RewardAck, OnRewardAck, toRegister);
        }


        private void OnRewardAck(uint id)
        {
            Refresh();
        }
    }

    public partial class UI_Daily_ActivityAward : UIBase, UI_Daily_ActivityAward_Layout.IListener
    {
        private List<CSVDailyActivityReward.Data>  m_DataCache = new List<CSVDailyActivityReward.Data>();
        private void Refresh()
        {
            var data = CSVDailyActivityReward.Instance.GetAll();

            int count = data.Count;

            m_Layout.SetAwardCount(count);

            m_DataCache.Clear();

            foreach (var item in data)
            {
                m_DataCache.Add(item);
            }

            uint curDailyActivty = Sys_Daily.Instance.TotalActivity;

            List<ItemIdCount> rewardList = new List<ItemIdCount>();

            for (int i = 0; i < count; i++)
            {
                m_Layout.SetAwardName(i,curDailyActivty, m_DataCache[i].Activity);

                rewardList.Clear();

                rewardList = CSVDrop.Instance.GetDropItem(m_DataCache[i].Reward);

                m_Layout.SetReward(i, rewardList);

                m_Layout.SetRewardConfigID(i, m_DataCache[i].id);

                int state = curDailyActivty < m_DataCache[i].Activity ? 0 : (Sys_Daily.Instance.HadGetReward(m_DataCache[i].id) ? 2 : 1);

                m_Layout.SetRewardState(i, state);
            }
        }
    }
    public partial class UI_Daily_ActivityAward : UIBase, UI_Daily_ActivityAward_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        private float LastClickTimePoint = 0;
        public void OnClickRewardItem(uint id)
        {
            if (Sys_Daily.Instance.HadGetReward(id))
                return;

            uint curDailyActivty = Sys_Daily.Instance.TotalActivity;

            var data = CSVDailyActivityReward.Instance.GetConfData(id);

            if (data == null)
            {
                Sys_Hint.Instance.PushContent_Normal("非法活跃度奖励编号");
                return;
            }


            if (data.Activity > curDailyActivty)
            {
                return;
            }

            if (Time.time - LastClickTimePoint < 1f)
                return;

            Sys_Daily.Instance.Apply_Reward(id);

            LastClickTimePoint = Time.time;
        }
    }
}
