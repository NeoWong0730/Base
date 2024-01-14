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
    public partial class UI_LadderPvp_Rank : UIBase
    {
        UI_LadderPvp_Rank_Layout m_Layout = new UI_LadderPvp_Rank_Layout();

        UI_PvpSingle_TodayReward m_RewardItmeShow = new UI_PvpSingle_TodayReward();

        private Vector3[] m_Points = new Vector3[4];

        uint m_State = 0;
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_RewardItmeShow.Init(gameObject.transform.Find("Animator/View_Reward"));

            m_Layout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.RankList, OnRankListRefresh, toRegister);
        }
        protected override void OnShow()
        {            
            Refresh();

            RefreshServerSelect();
        }        

        protected override void OnOpen(object arg)
        {
            Sys_LadderPvp.Instance.Apply_GetRankList(Sys_LadderPvp.Instance.GetLevelStage());
        }

        private void Refresh()
        {
            var rankData = Sys_LadderPvp.Instance.RankInfo;

            if (rankData == null)
            {
                m_Layout.infinityGrid.CellCount = 0;
                m_Layout.infinityGrid.ForceRefreshActiveCell();
                return;
            }

            m_State = rankData.GroupType;

            int rankSize = (rankData == null || rankData.Units == null) ? 0 : rankData.Units.Count;

            m_Layout.infinityGrid.CellCount = rankSize;

            if (Sys_LadderPvp.Instance.RankInfo != null)
            {
                var rankselfdata = Sys_LadderPvp.Instance.GetSelfArenaData();
                var data = rankselfdata == null ? null : rankselfdata.TiantiData;
                RefreshSetRankInfo(data, true, m_Layout.m_OwnRankItem);
            }
               

            int state = Sys_LadderPvp.Instance.GetLevelStage();

            m_Layout.SetMySelfInfoActive(state == Sys_LadderPvp.Instance.RankInfo.GroupType);
            
         
        }


        private void RefreshSetRankInfo(RankDataTianTi data,bool isMyself, UI_LadderPvp_Rank_Layout.RankItem item )
        {
            int winNum ;
            int totalNum;
            int winPre;
            string rateStr;
            string danlvName;
            if (data != null)
            {
                var levelid = Sys_LadderPvp.Instance.GetDanLvIDByScore((uint)data.Score);
                CSVTianTiSegmentInformation.Data infordata = CSVTianTiSegmentInformation.Instance.GetConfData(levelid);

                winNum = data.WinNum;
                totalNum = data.TotalNum;
                winPre = totalNum == 0 ? 0 : (int)(winNum / (totalNum * 1f) * 100);
                rateStr = string.Format("({0}/{1}){2}%", winNum, totalNum, winPre);

                danlvName = infordata == null ? string.Empty : LanguageHelper.GetTextContent(infordata.RankDisplay);
            }
            else
            {

                CSVTianTiSegmentInformation.Data infordata = CSVTianTiSegmentInformation.Instance.GetConfData((uint)Sys_LadderPvp.Instance.LevelID);

                winNum = 0;
                totalNum = 0;

                winPre = totalNum == 0 ? 0 : (int)(winNum / (totalNum * 1f) * 100);
                rateStr = string.Format("({0}/{1}){2}%", winNum, totalNum, winPre);

                danlvName = infordata == null ? string.Empty : LanguageHelper.GetTextContent(infordata.RankDisplay);
            }

            if (isMyself)
            {
                int rank = data == null ? (int)Sys_LadderPvp.Instance.MineRank : (int)data.GlobalRank;
                string name = data == null ? Sys_Role.Instance.Role.Name.ToStringUtf8() : data.Name.ToStringUtf8();
                int start = data == null ? (int)Sys_LadderPvp.Instance.MyInfoRes.RoleInfo.Base.Score : data.Score;
                string servername = string.Empty;
                uint serverid = data == null ? (uint)Sys_Login.Instance.RealServerID : data.ServerId;

                if (serverid != 0)
                {
                    var serverinfo = Sys_Login.Instance.FindServerInfoByID(serverid);
                    servername = serverinfo != null ? serverinfo.ServerName : string.Empty;
                }
                item.Set(rank, name, danlvName, start, rateStr, servername);
            }
            else
            {
                string servername = string.Empty;

                if (data != null)
                {
                    var serverinfo = Sys_Login.Instance.FindServerInfoByID(data.ServerId);
                    servername = serverinfo != null ? serverinfo.ServerName : string.Empty;
                }
              
               item.Set((int)data.GlobalRank, data.Name.ToStringUtf8(), danlvName, data.Score, rateStr, servername);
            }
                
        }

        private void RefreshServerSelect()
        {
            var values = Sys_LadderPvp.Instance.LevelStageValue;

            int count = values.Length-1;

            List<string> namelist = new List<string>();

            int stage = Sys_LadderPvp.Instance.GetLevelStage();

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
    public partial class UI_LadderPvp_Rank : UI_LadderPvp_Rank_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickRankItemGet(UI_LadderPvp_Rank_Layout.RankItem item)
        {

            item.GetRewardPosition(m_Points);

            m_RewardItmeShow.SetRewardShowPosition(m_Points);

            m_RewardItmeShow.Show();


            var rankData = Sys_LadderPvp.Instance.RankInfo;

            if (rankData == null)
                return;

            //var data = index == -1 ? rankData.SelfData : rankData.Units[index];
            var data = item.Index == -1 ? Sys_LadderPvp.Instance.GetSelfArenaData() : rankData.Units[item.Index];

            if (data == null)
                return;
            
            
            var rankingCofigData = CSVTianTiRankingAward.Instance.GetAll();

            uint dropID = 0;

            int count = CSVTianTiRankingAward.Instance.Count;

            for(int i = 0; i < count; i++)
            {
                var value = CSVTianTiRankingAward.Instance.GetByIndex(i);

                if (value.Ranking != null && value.Division == m_State &&
                    value.Ranking.Count > 1&&data.Rank >= value.Ranking[0] && data.Rank <= value.Ranking[1] )
                {
                    dropID = value.Reward;
                    break;
                }
            }

            var droplist = CSVDrop.Instance.GetDropItem(dropID);

            m_RewardItmeShow.SetRewardList(droplist,0);

        }



        public void OnDropdownEvent(int index)
        {
            var id = index+1;
            m_State =  (uint)id;

            var rankData = Sys_LadderPvp.Instance.RankInfo;

            if (rankData == null || id == rankData.Type)
                return;

            Sys_LadderPvp.Instance.Apply_GetRankList(id);
        }

 

        private void OnRankListRefresh()
        {
            Refresh();
        }

        public void OnCreateRankItem(InfinityGridCell cell)
        {
            UI_LadderPvp_Rank_Layout.RankItem rankItem = new UI_LadderPvp_Rank_Layout.RankItem();
            rankItem.Load(cell.mRootTransform);
            rankItem.SetOnClickListener(this.OnClickRankItemGet);
            cell.BindUserData(rankItem);
        }

        public void OnInfinityCellChange(InfinityGridCell cell, int index)
        {
            var rankitem = cell.mUserData as UI_LadderPvp_Rank_Layout.RankItem;

            var data = Sys_LadderPvp.Instance.RankInfo.Units[index].TiantiData;

            rankitem.Index = index;

            RefreshSetRankInfo(data, false, rankitem);
        }
    }
}
