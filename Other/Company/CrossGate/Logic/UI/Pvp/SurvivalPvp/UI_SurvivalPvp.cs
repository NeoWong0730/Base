using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Packet;
using Lib.Core;
using Table;

namespace Logic
{
    public partial class UI_SurvivalPvp:UIBase, UI_SurvivalPvp_Layout.IListener
    {
        class MsgInfo
        {
            public string msgInfo;
            public uint time;
        }

        private UI_SurvivalPvp_Layout m_Layout = new UI_SurvivalPvp_Layout();

        private List<MsgInfo> m_ActiveInfo = new List<MsgInfo>();

      //  private bool m_LastIsMatching = false;

        private Timer m_MatcherTimer = null;

        private Timer m_ActiveTimer = null;

        private float m_ActiveHaveTime = 0;

        private List<CSVSurvivalRankReward.Data>  m_RewardData = new List<CSVSurvivalRankReward.Data>();
        protected override void OnOpen(object arg)
        {
            if (Sys_SurvivalPvp.Instance.isMatching == false)
                Sys_SurvivalPvp.Instance.SendInfoReq();

            if (Sys_Team.Instance.TeamMemsCount > 1)
                Sys_SurvivalPvp.Instance.SendTeamScore();
        }
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

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
            Sys_SurvivalPvp.Instance.eventEmitter.Handle(Sys_SurvivalPvp.EEvents.InfoRefresh, OnPvpInfoRefresh, toRegister);

            Sys_SurvivalPvp.Instance.eventEmitter.Handle(Sys_SurvivalPvp.EEvents.MatchCancle, OnMatchCancle, toRegister);

            Sys_SurvivalPvp.Instance.eventEmitter.Handle(Sys_SurvivalPvp.EEvents.MatchPanleClose, OnLoadPanleOpen, toRegister);

            Sys_SurvivalPvp.Instance.eventEmitter.Handle(Sys_SurvivalPvp.EEvents.MatchPanleOpen, OnSurvivalPvpMatch, toRegister);

            Sys_SurvivalPvp.Instance.eventEmitter.Handle(Sys_SurvivalPvp.EEvents.MemberScore, OnMemberScore, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<TeamMem>(Sys_Team.EEvents.MemEnterNtf, OnTeamMemberChange, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemLeaveNtf, OnTeamMemberLeave, toRegister);
        }
        protected override void OnShow()
        {
            RefreshMemberInfo();

            RefreshPvpInfo();

            RefreshActivyInfo();

            RefreshMatching();
        }


        protected override void OnHide()
        {
            CloseSelf();
        }
        protected override void OnClose()
        {
            if (m_MatcherTimer != null)
                m_MatcherTimer.Cancel();

            m_MatcherTimer = null;

            if (m_ActiveTimer != null)
                m_ActiveTimer.Cancel();

            m_ActiveTimer = null;
        }

        private void RefreshMemberInfo()
        {
            if (Sys_Team.Instance.HaveTeam == false)
            {
                RefreshNoTeamInfo();

                return;
            }
            int count = Sys_Team.Instance.teamMems.Count;

            m_Layout.SetMemberCount(count);

            for (int i = 0; i < count; i++)
            {
                var value = Sys_Team.Instance.teamMems[i];

                m_Layout.SetMemberHead(i, CharacterHelper.getHeadID(value.HeroId, value.Photo));

                m_Layout.SetMemberName(i, value.Name.ToStringUtf8());

                m_Layout.SetMemberProfession(i, value.Career);

                m_Layout.SetMemberRoleID(i,value.MemId);

                m_Layout.SetMemberScore(i, Sys_SurvivalPvp.Instance.GetScore(value.MemId));
            }

            m_Layout.SetMemberAddActive(count < 5);
        }


        private void RefreshNoTeamInfo()
        {
            m_Layout.SetMemberCount(1);

            m_Layout.SetMemberHead(0, CharacterHelper.getHeadID( Sys_Role.Instance.Role.HeroId,0));

            m_Layout.SetMemberName(0, Sys_Role.Instance.Role.Name.ToStringUtf8());

            m_Layout.SetMemberProfession(0, Sys_Role.Instance.Role.Career);

            m_Layout.SetMemberRoleID(0, Sys_Role.Instance.RoleId);

           uint score = Sys_SurvivalPvp.Instance.Info == null ? 0 : Sys_SurvivalPvp.Instance.Info.Base.Score;
            m_Layout.SetMemberScore(0, score);
        }
        private void RefreshPvpInfo()
        {
            var info = Sys_SurvivalPvp.Instance.Info;

            int joinTimes = info == null ? 0 : (int)info.Base.Join;
            int winTimes = info == null ? 0 : (int)info.Base.Win;
            int rankTimes = info == null ? 0 : (int)info.Base.Rank;
            int score = info == null ? 0 : (int)info.Base.Score;

            m_Layout.SetPvpTimes(joinTimes);
            m_Layout.SetWinTimes(winTimes);
            m_Layout.SetRankTimes(rankTimes);
            m_Layout.SetScore(score);


            if (Sys_SurvivalPvp.Instance.Info == null)
            {
                m_Layout.SetTimeCountDown(LanguageHelper.GetTextContent(105103));
                return;
            }
                


            if (m_ActiveTimer != null)
                m_ActiveTimer.Cancel();

            var serverTime = Sys_Time.Instance.GetServerTime();
            var openTime = Sys_SurvivalPvp.Instance.Info.Time.Opentime;

            var time = serverTime < openTime ? 0 :(serverTime -  openTime);

            uint haveTime = 0;

            if (time > 0 && Sys_SurvivalPvp.Instance.Info.Time.Totalsec > time)
            {
                haveTime = Sys_SurvivalPvp.Instance.Info.Time.Totalsec - time;
            }
                

            m_ActiveHaveTime = haveTime;

            if (haveTime > 0)
                m_ActiveTimer = Timer.Register(haveTime, null, RefreshActiveTime);
            else
            {
                var languageID = time <= 0 ? 105103 : 105102;
                m_Layout.SetTimeCountDown(LanguageHelper.GetTextContent((uint)languageID));
            }



        }

        private void RefreshActivyInfo(bool isSelf = false)
        {

            if (Sys_SurvivalPvp.Instance.Info != null)
                m_Layout.SetOnlySelf(Sys_SurvivalPvp.Instance.Info.Flag);

            m_ActiveInfo.Clear();

            if (Sys_SurvivalPvp.Instance.Info == null || Sys_SurvivalPvp.Instance.Info.Msg == null)
            {
                m_Layout.SetActivityInfoCount(0);
             
                return;
            }

            int count = Sys_SurvivalPvp.Instance.Info.Msg.Msgs.Count;

           // Sys_SurvivalPvp.Instance.Info.Msg.Msgs

            for (int i = count-1; i >= 0; i--)
            {
                var value = Sys_SurvivalPvp.Instance.Info.Msg.Msgs[i];

                string info = string.Empty;

                if (value.MsgType == (uint)SurvivalMsgType.MsgTypeFriendWin)
                {
                    info = LanguageHelper.GetTextContent(590000016, value.Player2.Name.ToStringUtf8(), value.Player1.ChangeScore.ToString());
                }
                else if (value.MsgType == (uint)SurvivalMsgType.MsgTypeFriendFail)
                {
                    info = LanguageHelper.GetTextContent(590000017, value.Player2.Name.ToStringUtf8(), value.Player1.ChangeScore.ToString());
                }
                else if (value.MsgType == (uint)SurvivalMsgType.MsgTypeWinFirst)
                {
                    var roleid = Sys_Role.Instance.RoleId;

                    if (Sys_SurvivalPvp.Instance.Info.Flag && (value.Player1.RoleId != roleid && value.Player2.RoleId != roleid))
                        continue;
                    string name0 = value.Player1.Name.ToStringUtf8();
                    string name1 = value.Player2.Name.ToStringUtf8();
                    info = LanguageHelper.GetTextContent(590000018, name0, value.Player1.Score.ToString(), name1, value.Player2.Score.ToString());
                }
                else if (value.MsgType == (uint)SurvivalMsgType.MsgTypeLastWin3)
                {
                    info = LanguageHelper.GetTextContent(590000019, value.Player2.Name.ToStringUtf8(), value.Player1.Score.ToString(), value.Player1.ChangeScore.ToString());
                }

                m_ActiveInfo.Add(new MsgInfo() { msgInfo = info,time = value.Timestamp});
            }

            m_Layout.SetActivityInfo(m_ActiveInfo.Count);


            m_ActiveInfo.Sort((x, y) => {

                if (x.time < y.time)
                    return 1;

                if (x.time > y.time)
                    return -1;

                return 0;
            });

        }

        private void RefreshMatching()
        {
            m_Layout.SetMatching(Sys_SurvivalPvp.Instance.isMatching);

            if (m_MatcherTimer != null)
                m_MatcherTimer.Cancel();

            if(Sys_SurvivalPvp.Instance.isMatching)
             m_MatcherTimer = Timer.Register(600, null, RefreshMatchTime);
        }

        private void RefreshMatchTime(float value)
        {
            m_Layout.SetMatchTime(Time.time - Sys_SurvivalPvp.Instance.MatchTimePoint);
        }


        private void RefreshActiveTime(float value)
        {

            m_Layout.SetTimeCountDown( LanguageHelper.GetTextContent(2022404,Sys_SurvivalPvp.Instance.GetTimeString(m_ActiveHaveTime - value)));
        }

        public void OnActivityInfoInfinityGridChange(InfinityGridCell infinityGridCell, int index)
        {
            var item = infinityGridCell.mUserData as UI_SurvivalPvp_Layout.ActivityInfoItem;

            if (index >= m_ActiveInfo.Count)
                return;

            var value = m_ActiveInfo[index];

            if (value == null)
                return;

            item.SetText(value.msgInfo);
        }
    }

    
    public partial class UI_SurvivalPvp : UIBase, UI_SurvivalPvp_Layout.IListener
    {
        private void RefreshReward()
        {
            int count = m_RewardData.Count;

            m_Layout.SetRewardCount(count);

            bool isGroupPvp = Sys_SurvivalPvp.Instance.IsGroupPvp();

            for (int i = 0; i < count; i++)
            {
                var rewardid = isGroupPvp ? m_RewardData[i].KfDropView : m_RewardData[i].DropView;
                var list = CSVDrop.Instance.GetDropItem(rewardid);

                m_Layout.SetRewardList(i, list);

                m_Layout.SetRewardTitle(i, LanguageHelper.GetTextContent(m_RewardData[i].Rewardshow));
                
            }
        }
    }
    public partial class UI_SurvivalPvp : UIBase, UI_SurvivalPvp_Layout.IListener
    {
        public void OnClickAddMember()
        {
            // UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);

            var level = Sys_SurvivalPvp.Instance.GetPvpLevel();
            uint id = 0;

            switch (level)
            {
                case 1:
                    id = 2200u;
                    break;
                case 2:
                    id = 2210u;
                    break;
            }
            Sys_Team.Instance.OpenFastUI(id,true);
        }

        public void OnClickClose()
        {
            CloseSelf();
        }

       
        public void OnClickMatch()
        {

            if (Sys_SurvivalPvp.Instance.isMatching == false)
            {

                if (Sys_Team.Instance.HaveTeam && Sys_Team.Instance.isCaptain() == false)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2022440));
                    return;
                }

                if (Sys_Team.Instance.TeamMemberCountMax > Sys_Team.Instance.TeamMemsCount)
                {
                    Sys_Team.Instance.OpenTips(0, LanguageHelper.GetTextContent(590000023), () => {

                        Sys_SurvivalPvp.Instance.SendStartMatch();
                    });
                }

                else
                {
                    Sys_SurvivalPvp.Instance.SendStartMatch();
                }
                

                
            }
               
            else
                Sys_SurvivalPvp.Instance.SendCancleMatch();

          
        }

        public void OnClickRank()
        {
            UIManager.OpenUI(EUIID.UI_SurvivalPvp_Rank);
        }

        public void OnClickReward()
        {
            m_Layout.SetRewardActive(true);
            RefreshReward();
        }

        public void OnClickTogSelf(bool state)
        {
            Sys_SurvivalPvp.Instance.SendSetMsgFlagReq(state);

            RefreshActivyInfo();
        }

    }

    public partial class UI_SurvivalPvp : UIBase, UI_SurvivalPvp_Layout.IListener
    {
        private void OnPvpInfoRefresh()
        {
            RefreshPvpInfo();

            RefreshActivyInfo();

            RefreshMemberInfo();
        }

        private void OnMatchCancle()
        {
            RefreshMatching();
        }

        private void OnLoadPanleOpen()
        {
            CloseSelf();
        }

        private void OnMemberScore()
        {
            int count = Sys_SurvivalPvp.Instance.MemberScore.Members.Count;

            for (int i = 0; i < count; i++)
            {
                var value = Sys_SurvivalPvp.Instance.MemberScore.Members[i];

                m_Layout.SetMemberScore(value.RoleId, value.Score);
            }
        }
        private void OnTeamMemberChange(TeamMem teamMem)
        {
            Sys_SurvivalPvp.Instance.SendTeamScore();

            RefreshMemberInfo();
        }

        private void OnTeamMemberLeave(ulong rolid)
        {
            RefreshMemberInfo();
        }
        private void OnSurvivalPvpMatch()
        {
            RefreshMatching();
        }
    }
}
