using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Packet;
using Table;
namespace Logic
{
    public partial class UI_SurvivalPvp_Rank : UIBase, UI_SurvivalPvp_Rank_Layout.IListener
    {
        private UI_SurvivalPvp_Rank_Layout m_Layout = new UI_SurvivalPvp_Rank_Layout();

        UI_PvpSingle_TodayReward m_UITodayReward = new UI_PvpSingle_TodayReward();

        private List<CSVSurvivalRankReward.Data>  m_RewardData = new List<CSVSurvivalRankReward.Data>();
        protected override void OnOpen(object arg)
        {
            Sys_SurvivalPvp.Instance.SendRankInfo();
        }
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);
            m_UITodayReward.Init(gameObject.transform.Find("View_Reward"));

            m_Layout.SetListener(this);

            var datas = CSVSurvivalRankReward.Instance.GetAll();

            int pvpLevel = Sys_SurvivalPvp.Instance.GetPvpLevel();

            m_RewardData.Clear();
            foreach (var kvp in datas)
            {
                if (kvp.RewardLevel == pvpLevel)
                {
                    m_RewardData.Add(kvp);
                }
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_SurvivalPvp.Instance.eventEmitter.Handle(Sys_SurvivalPvp.EEvents.RankInfo, RefreshInfo, toRegister);
        }
        protected override void OnShow()
        {
            RefreshInfo();
        }


        private void RefreshInfo()
        {
 
            int count = Sys_SurvivalPvp.Instance.RankInfo == null ? 0 : Sys_SurvivalPvp.Instance.RankInfo.Units.Count;

            m_Layout.SetRankSize(count);

            SetSelf();
        }


        private void SetRankInfo(int index ,RankUnitData value, UI_SurvivalPvp_Rank_Layout.RankItem item)
        {
            if (value == null || value.SurvivalData == null)
                return;

            item.SetRankNum(index, value.Rank);
            item.SetName(value.SurvivalData.Name.ToStringUtf8());
            item.SetProfession(value.SurvivalData.Career);
            item.SetScore(value.Score);

            uint serverid = value.SurvivalData.ServerId;
            var serverinfo = Sys_Login.Instance.FindServerInfoByID(serverid);
            item.SetServerName(serverinfo == null ? string.Empty :serverinfo.ServerName);
        }

        private void SetSelf()
        {
            RankUnitData rankUnitData= Sys_SurvivalPvp.Instance.GetSelfSurvivalData();
            if (rankUnitData != null)
            {
                SetRankInfo(0, rankUnitData, m_Layout.m_MyRank);
                m_Layout.m_MyRank.SetNoInfoActive(false);

            }
            //if (Sys_SurvivalPvp.Instance.RankInfo != null && Sys_SurvivalPvp.Instance.RankInfo.SelfData != null)
            //{
            //    SetRankInfo(0, Sys_SurvivalPvp.Instance.RankInfo.SelfData, m_Layout.m_MyRank);
            //    m_Layout.m_MyRank.SetNoInfoActive(false);
            //}
            else
            {
                var item = m_Layout.m_MyRank;

                item.SetNoInfoActive(true);

                //item.SetRankNum(0,0);
                //item.SetName(Sys_Role.Instance.Role.Name.ToStringUtf8());
                //item.SetProfession(Sys_Role.Instance.Role.Career);

                //uint score = Sys_SurvivalPvp.Instance.Info == null ? 0 : Sys_SurvivalPvp.Instance.Info.Base.Score;
                //item.SetScore(score);
            }
        }
    }

    public partial class UI_SurvivalPvp_Rank : UIBase, UI_SurvivalPvp_Rank_Layout.IListener
    {

        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnInfinityGridChange(InfinityGridCell infinityGridCell, int index)
        {
            var item = infinityGridCell.mUserData as UI_SurvivalPvp_Rank_Layout.RankItem;

            if (index >= Sys_SurvivalPvp.Instance.RankInfo.Units.Count)
                return;

            var value =  Sys_SurvivalPvp.Instance.RankInfo.Units[index];

            if (value == null || value.SurvivalData == null)
                return;

            SetRankInfo((int)value.Rank, value, item);
        }

        public void OnClickReward(int num, Vector3[] points)
        {
            int count = m_RewardData.Count;

            bool isGroupPvp = Sys_SurvivalPvp.Instance.IsGroupPvp();
            if (num == 0)
            {
                var rewardid = isGroupPvp ? m_RewardData[count - 1].KfDropView : m_RewardData[count - 1].DropView;

                SetRewardMenu(rewardid, points);
                return;
            }
            for (int i = 0; i < count;i++)
            {
                if (num >= m_RewardData[i].RankRange[0] && num <= m_RewardData[i].RankRange[1])
                {
                    var rewardid = isGroupPvp ? m_RewardData[i].KfDropView : m_RewardData[i].DropView;
                    SetRewardMenu(rewardid, points);
                    break;
                }
            }
        }

        private void SetRewardMenu(uint id, Vector3[] points)
        {
            m_UITodayReward.Show();

            m_UITodayReward.SetRewardShowPosition(points);

            var list = CSVDrop.Instance.GetDropItem(id);

            m_UITodayReward.SetRewardList(list, 1);
        }
    }
}
