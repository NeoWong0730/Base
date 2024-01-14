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
    ///排行榜
    /// </summary>
    public partial class UI_Pvp_SingleRank : UIBase, UI_Pvp_SingleRank_Layout.IListener
    {
        UI_Pvp_SingleRank_Layout m_Layout = new UI_Pvp_SingleRank_Layout();

        UI_PvpSingle_TodayReward m_RewardItmeShow = new UI_PvpSingle_TodayReward();

        private Vector3[] m_Points = new Vector3[4];

        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_RewardItmeShow.Init(gameObject.transform.Find("View_Reward"));

            m_Layout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Pvp.Instance.eventEmitter.Handle(Sys_Pvp.EEvents.RankList, OnRankListRefresh, toRegister);
        }
        protected override void OnShow()
        {            
            Refresh();

            RefreshServerSelect();
        }        

        protected override void OnOpen(object arg)
        {         
            Sys_Pvp.Instance.Apply_GetRankList(Sys_Pvp.Instance.GetLevelStage());
        }

        private void Refresh()
        {
            var rankData = Sys_Pvp.Instance.RankInfo;

            if (rankData == null)
            {
                m_Layout.SetRankSize(0);
                return;
            }
               

            int rankSize = (rankData == null || rankData.Units == null) ? 0 : rankData.Units.Count;

            m_Layout.SetRankSize(rankSize);

            for (int i = 0; i < rankSize; i++)
            {
                var data = rankData.Units[i];

                RefreshSetRankInfo(data.ArenaData, false, i);
            }

            if (Sys_Pvp.Instance.RankInfo != null)
            {
                //RankDataArena data = Sys_Pvp.Instance.RankInfo.SelfData == null ? null : Sys_Pvp.Instance.RankInfo.SelfData.ArenaData;
                RankDataArena data = Sys_Pvp.Instance.GetSelfArenaData() == null ? null : Sys_Pvp.Instance.GetSelfArenaData().ArenaData;
                RefreshSetRankInfo(data, true, -1);
            }
               

            int state = Sys_Pvp.Instance.GetLevelStage();

            m_Layout.SetMySelfInfoActive(state == Sys_Pvp.Instance.RankInfo.GroupType);
            
         
        }


        private void RefreshSetRankInfo(RankDataArena data,bool isMyself, int index)
        {
            int winNum ;
            int totalNum;
            int winPre;
            string rateStr;
            string danlvName;
            if (data != null)
            {
                CSVArenaSegmentInformation.Data infordata = CSVArenaSegmentInformation.Instance.GetConfData((uint)data.DanLv);

                winNum = data.WinNum;
                totalNum = data.TotalNum;
                winPre = totalNum == 0 ? 0 : (int)(winNum / (totalNum * 1f) * 100);
                rateStr = string.Format("({0}/{1}){2}%", winNum, totalNum, winPre);

                danlvName = infordata == null ? string.Empty : LanguageHelper.GetTextContent(infordata.RankDisplay);
            }
            else
            {
                CSVArenaSegmentInformation.Data infordata = CSVArenaSegmentInformation.Instance.GetConfData((uint)Sys_Pvp.Instance.Level);

                winNum = Sys_Pvp.Instance.MyInfoRes.WinNum;
                totalNum = Sys_Pvp.Instance.MyInfoRes.TotalNum;
                winPre = totalNum == 0 ? 0 : (int)(winNum / (totalNum * 1f) * 100);
                rateStr = string.Format("({0}/{1}){2}%", winNum, totalNum, winPre);

                danlvName = infordata == null ? string.Empty : LanguageHelper.GetTextContent(infordata.RankDisplay);
            }

            if (isMyself)
            {
                int rank = data == null ? Sys_Pvp.Instance.MineRank : (int)data.GlobalRank;
                string name = data == null ? Sys_Role.Instance.Role.Name.ToStringUtf8() : data.Name.ToStringUtf8();
                int start = data == null ? Sys_Pvp.Instance.Star : data.Star;
                m_Layout.SetMySelfRankInfo(rank, name, danlvName, start, rateStr);
            }
            else
                m_Layout.SetRankInfo(index, (int)data.GlobalRank, data.Name.ToStringUtf8(), danlvName, data.Star, rateStr);
        }

        private void RefreshServerSelect()
        {
            var values = Sys_Pvp.Instance.LevelStageValue;

            int count = values.Length-1;

            List<string> namelist = new List<string>();

            int stage = Sys_Pvp.Instance.GetLevelStage();

            int selectIndex = 0;
            for (int i = 0; i < count; i++)
            {
                var data = CSVArenaWinAward.Instance.GetConfData((uint)(i+1));

                string name = LanguageHelper.GetTextContent(data.CourtId);
                namelist.Add(name);

                if ((i+1) == stage)
                    selectIndex = i;
            }


            m_Layout.SetServerDrop(namelist);

            m_Layout.SetServerDropFocus(selectIndex);

           
        }

    }

    /// <summary>
    /// 监听处理
    /// </summary>
    public partial class UI_Pvp_SingleRank : UIBase, UI_Pvp_SingleRank_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickRankItemGet(int index)
        {
            m_Layout.GetRewardPosition(index, m_Points);

            m_RewardItmeShow.SetRewardShowPosition(m_Points);



            m_RewardItmeShow.Show();


            var rankData = Sys_Pvp.Instance.RankInfo;

            if (rankData == null)
                return;

            //var data = index == -1 ? rankData.SelfData : rankData.Units[index];
            var data = index == -1 ? Sys_Pvp.Instance.GetSelfArenaData() : rankData.Units[index];

            if (data == null)
                return;

            var rankingCofigData = CSVArenaRankingAward.Instance.GetAll();

            uint dropID = 0;

            foreach (var kvp in rankingCofigData)
            {
                if (kvp.Ranking != null && kvp.Ranking.Count > 1&&data.Rank >= kvp.Ranking[0] && data.Rank <= kvp.Ranking[1])
                {
                    dropID = kvp.Reward;
                    break;
                }
            }

            var droplist = CSVDrop.Instance.GetDropItem(dropID);

            m_RewardItmeShow.SetRewardList(droplist,0);

        }


        public void OnDropdownEvent(int index)
        {
            var id = index+1;

            var rankData = Sys_Pvp.Instance.RankInfo;

            if (rankData == null || id == rankData.Type)
                return;

            Sys_Pvp.Instance.Apply_GetRankList(id);
        }

        private void OnRankListRefresh()
        {
            Refresh();
        }
    }
}
