using System;
using System.Collections.Generic;

using Logic.Core;
using Packet;
using Table;
using UnityEngine;

namespace Logic
{
    public partial class UI_LadderPvp:UIBase
    {
        UI_LadderPvp_Layout m_Layout = new UI_LadderPvp_Layout();

        private float m_LastTime = 0;

        UI_LadderPvp_MemMenu m_MemMenu = new UI_LadderPvp_MemMenu();
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);


            m_MemMenu.Init(gameObject.transform.Find("View_Teammate"));
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.PvpInfoRefresh, RefreshDanLvInfo,toRegister);
            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.TeamMemberInfo, RefreshTeamInfo, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<TeamMem>(Sys_Team.EEvents.MemEnterNtf, OnTeamMemberChange, toRegister);

            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.TeamClear, RefreshTeamInfo, toRegister);

            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.HaveTeam, RefreshTeamInfo, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemLeaveNtf, OnTeamMemberLeave, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemState, OnTeamMemberState, toRegister);

            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.StartMatch, OnPvpStartMatch, toRegister);


            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.GetDanLvUpAward, OnRewardRefresh,toRegister);
            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.GetTaskAward, OnRewardRefresh,toRegister);
        }
        protected override void OnOpen(object arg)
        {
            Sys_LadderPvp.Instance.Apply_Info();
            Sys_LadderPvp.Instance.Apply_GetTeamMembersInfo();
            Sys_LadderPvp.Instance.Apply_LeaderOpenMainPanelReq();
        }


        protected override void OnShow()
        {
           
            RefreshInfo();
        }

        protected override void OnUpdate()
        {
            float offsetTime = Time.time - m_LastTime;

            if (offsetTime < 1 && m_LastTime != 0)
                return;

            UpdateTimeString();


            m_LastTime = Time.time;
        }

        private void UpdateTimeString()
        {
            uint havetime = 0;

            var result = Sys_LadderPvp.Instance.GetActivityRemainingTime(out havetime);
            string strtime = ToTimeString(havetime);
            if (result == 0)
            {
                m_Layout.TexDanMatchTip.text = havetime > 86400 ? String.Empty: LanguageHelper.GetTextContent(1340001816u, ToTimeString(havetime,false,true));
                m_Layout.TexTimeTips.text = LanguageHelper.GetTextContent(10632); 
                m_Layout.TexTime.text = strtime;
            }
            else if (result == 1)
            {
                m_Layout.TexTime.text = strtime;
                m_Layout.TexDanMatchTip.text = string.Empty;
                m_Layout.TexTimeTips.text = LanguageHelper.GetTextContent(10176); 

            }
            else if (result == 2)
            {

                m_Layout.TexTimeTips.text = LanguageHelper.GetTextContent(10685);

                m_Layout.TexTime.text = strtime;
            }


        }
        private string ToTimeString(uint time,bool bday = true,bool bsec = false)
        {
            string str = string.Empty;

            if (time == 0)
                return str;

            int day = (int)(time / 86400);
            int hours = (((int)time) % 86400) / 3600;
            int min = (((int)time) % 86400) % 3600 / 60;

            int sec = (int)time % 60;

            if (day > 0 && bday)
                str += LanguageHelper.GetTextContent(10154, day.ToString());

            if (day > 0 || hours > 0)
                str += LanguageHelper.GetTextContent(10155, hours.ToString());

            str += LanguageHelper.GetTextContent(10156, min.ToString());


            if (bsec)
                str += LanguageHelper.GetTextContent(10941, (sec < 10 ? ("0" + sec.ToString()) : sec.ToString()));
            return str;
        }
        void RefreshInfo()
        {
            RefreshTeamInfo();
            RefreshDanLvInfo();
        }

        void RefreshTeamInfo()
        {
            int count = Sys_LadderPvp.Instance.TeamMembersInfo == null ? 0 : Sys_Team.Instance.TeamMemsCount;

            if (count == 0)
            {
                RefreshTeamInfoWithoutMems();
                return;
            }

            int showCount = count;

            if (count < 5)
                showCount += 1;

            m_Layout.TeamMemGroup.SetChildSize(showCount);

            int infocount = Sys_LadderPvp.Instance.TeamMembersInfo.Members.Count;

            for (int i = 0; i < count; i++)
            {
                var teamMemberInfo = Sys_Team.Instance.getTeamMem(i);

                var item = m_Layout.TeamMemGroup.getAt(i);

                item.TransAdd.gameObject.SetActive(false);
                item.TransInfo.gameObject.SetActive(true);

                item.TexName.text = teamMemberInfo.Name.ToStringUtf8();
                CharacterHelper.SetHeadAndFrameData(item.ImgIcon, teamMemberInfo.HeroId, teamMemberInfo.Photo, teamMemberInfo.PhotoFrame);


                TextHelper.SetText(item.TexLevel, LanguageHelper.GetTextContent(1000002, teamMemberInfo.Level.ToString()));

                uint icon = OccupationHelper.GetLogonIconID(teamMemberInfo.Career);
                ImageHelper.SetIcon(item.ImgCareeIcon, icon);

                uint score = 0;
                if (Sys_LadderPvp.Instance.TeamMembersInfo != null && i < infocount)
                {
                    score =  Sys_LadderPvp.Instance.TeamMembersInfo.Members[i].Score;
                }
                uint danlv = Sys_LadderPvp.Instance.GetDanLvIDByScore(score);

                string danlvstring = Sys_LadderPvp.Instance.GetDanLvLevelString(danlv);
                item.TexDanLevel.text = danlvstring;

                item.TransLeave.gameObject.SetActive(teamMemberInfo.IsLeave());
                item.TransOffline.gameObject.SetActive(teamMemberInfo.IsOffLine());


                item.ID = (uint)i;
            }

            if (showCount > count)
            {
                var item = m_Layout.TeamMemGroup.getAt(showCount - 1);

                item.TransAdd.gameObject.SetActive(true);
                item.TransInfo.gameObject.SetActive(false);
            }
        }

        void RefreshTeamMemInfoState(ulong roleid)
        {
           int index =  Sys_Team.Instance.MemIndex(roleid);

            if (index == -1)
                return;

            var item = m_Layout.TeamMemGroup.getAt(index);

            if (item == null)
                return;

            var teamMemberInfo = Sys_Team.Instance.getTeamMem(index);

            item.TransLeave.gameObject.SetActive(teamMemberInfo.IsLeave());
            item.TransOffline.gameObject.SetActive(teamMemberInfo.IsOffLine());

        }

        void RefreshTeamInfoWithoutMems()
        {
            m_Layout.TeamMemGroup.SetChildSize(2);

            var item = m_Layout.TeamMemGroup.getAt(0);

            item.TransAdd.gameObject.SetActive(false);
            item.TransInfo.gameObject.SetActive(true);

            item.TexName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();

            item.ID = 0;

            CharacterHelper.SetHeadAndFrameData(item.ImgIcon, Sys_Role.Instance.Role.HeroId, Sys_Head.Instance.clientHead.headId, Sys_Head.Instance.clientHead.headFrameId);


            TextHelper.SetText(item.TexLevel, LanguageHelper.GetTextContent(1000002, Sys_Role.Instance.Role.Level.ToString()));

            uint icon = OccupationHelper.GetLogonIconID(Sys_Role.Instance.Role.Career);
            ImageHelper.SetIcon(item.ImgCareeIcon, icon);

            var score = Sys_LadderPvp.Instance.MyInfoRes.RoleInfo == null ? 0 : Sys_LadderPvp.Instance.MyInfoRes.RoleInfo.Base.Score;

            var danlv = Sys_LadderPvp.Instance.GetDanLvIDByScore(score);

            string danlvstring = Sys_LadderPvp.Instance.GetDanLvLevelString(danlv);
            item.TexDanLevel.text = danlvstring;

            item.TransLeave.gameObject.SetActive(false);
            item.TransOffline.gameObject.SetActive(false);


            var item1 = m_Layout.TeamMemGroup.getAt(1);
            item1.TransAdd.gameObject.SetActive(true);
            item1.TransInfo.gameObject.SetActive(false);

        }
        void RefreshDanLvInfo()
        {

            if (Sys_LadderPvp.Instance.MyInfoRes.RoleInfo == null)
                return;

            var leveldata = CSVTianTiSegmentInformation.Instance.GetConfData(Sys_LadderPvp.Instance.LevelID);
            var nextleveldata = CSVTianTiSegmentInformation.Instance.GetConfData(Sys_LadderPvp.Instance.NextLevelID);

            m_Layout.TexScore.text = Sys_LadderPvp.Instance.MyInfoRes.RoleInfo.Base.Score.ToString();

            m_Layout.TexNextScore.text = nextleveldata == null ? LanguageHelper.GetTextContent(5122u):(nextleveldata.Score).ToString();

            m_Layout.TexRank.text = Sys_LadderPvp.Instance.MineRank == 0 ? LanguageHelper.GetTextContent(10168) : Sys_LadderPvp.Instance.MineRank.ToString();

            m_Layout.TexLevel.text = Sys_LadderPvp.Instance.GetDanLvLevelString((uint)Sys_LadderPvp.Instance.LevelID);

            string seasonnumstring = string.Empty;
            string seasondatestring = string.Empty;

            if (Sys_LadderPvp.Instance.SeasonNum != 0 || Sys_LadderPvp.Instance.SeasonWeek != 0)
            {
                seasonnumstring = LanguageHelper.GetTextContent(10126, Sys_LadderPvp.Instance.SeasonNum.ToString()) + LanguageHelper.GetTextContent(10127, Sys_LadderPvp.Instance.SeasonWeek.ToString());

                string starTime = Sys_LadderPvp.Instance.PvpStartTimesampe == 0 ? string.Empty : string.Format("{0}/{1}/{2}",
                    Sys_LadderPvp.Instance.PvpStarTime.Year.ToString(), Sys_LadderPvp.Instance.PvpStarTime.Month.ToString(), Sys_LadderPvp.Instance.PvpStarTime.Day.ToString());

                string endTime = Sys_LadderPvp.Instance.PvpEndTimesampe == 0 ? string.Empty : string.Format("{0}/{1}/{2}",
                    Sys_LadderPvp.Instance.PvpEndTime.Year.ToString(), Sys_LadderPvp.Instance.PvpEndTime.Month.ToString(), Sys_LadderPvp.Instance.PvpEndTime.Day.ToString());

                seasondatestring = starTime + " - " + endTime;
            }

            m_Layout.TexSeasonNum.text = seasonnumstring;
            m_Layout.TexSeasonTime.text = seasondatestring;

            CSVTianTiSegmentInformation.Data LastItem = null;
            if (Sys_LadderPvp.Instance.IsLevelEffect)
            {

                LastItem = CSVTianTiSegmentInformation.Instance.GetConfData((uint)Sys_LadderPvp.Instance.LastLevelID);

                m_Layout.SetLevelUpEffect(LastItem == null ? string.Empty : LastItem.RankIcon, Sys_LadderPvp.Instance.LevelIcon);
            }
            else
            {
                m_Layout.SetMineLevelIcon(Sys_LadderPvp.Instance.LevelIcon);
            }

            UpdateTimeString();


            m_Layout.TransTaskRewardRed.gameObject.SetActive(Sys_LadderPvp.Instance.IsHaveTaskReward());
            m_Layout.TransLevleUpRewardRed.gameObject.SetActive(Sys_LadderPvp.Instance.IsHaveLeveUpReward());
        }

        private void RefreshRemaining()
        {
            string timetipsstr = string.Empty;
            
            if (Sys_LadderPvp.Instance.GetRemainingTime() > 0)
                timetipsstr = LanguageHelper.GetTextContent(10176);
            else
            {
                int str = Sys_LadderPvp.Instance.TodayAcitityIsOver() ? 10633 : 10632;
                timetipsstr = LanguageHelper.GetTextContent((uint)str);
            }

            m_Layout.TexTimeTips.text = timetipsstr;


        }


        private void OnTeamMemberChange(TeamMem teamMem)
        {
            Sys_LadderPvp.Instance.Apply_GetTeamMembersInfo();

            RefreshTeamInfo();
        }

        private void OnTeamMemberLeave(ulong rolid)
        {
            RefreshTeamInfo();
        }

        private void OnPvpStartMatch()
        {
            CloseSelf();
        }

        private void OnTeamMemberState(ulong roleid)
        {
            RefreshTeamMemInfoState(roleid);
        }

        private void OnRewardRefresh()
        {
            m_Layout.TransTaskRewardRed.gameObject.SetActive(Sys_LadderPvp.Instance.IsHaveTaskReward());
            m_Layout.TransLevleUpRewardRed.gameObject.SetActive(Sys_LadderPvp.Instance.IsHaveLeveUpReward());
        }
    }

    public partial class UI_LadderPvp : UI_LadderPvp_Layout.IListener
    {
        public void OnBtnBagClick()
        {
            UIManager.OpenUI(EUIID.UI_Bag, false, 1);
        }

        public void OnBtnChatClick()
        {
            UIManager.OpenUI(EUIID.UI_Chat);
        }

        public void OnBtnCloseClick()
        {
            CloseSelf();
        }

        public void OnBtnDanMatchClick()
        {
            Sys_LadderPvp.Instance.Apply_Mathch(2);
        }

        public void OnBtnFreeMatchClick()
        {
            Sys_LadderPvp.Instance.Apply_Mathch(1);
        }

        public void OnBtnFriendClick()
        {
            UIManager.OpenUI(EUIID.UI_Society);
        }

        public void OnBtnLevelUpRewardClick()
        {
            UIManager.OpenUI(EUIID.UI_LadderPvp_LevelReward);
        }

        public void OnBtnRankClick()
        {
            UIManager.OpenUI(EUIID.UI_LadderPvp_Rank);
        }

        public void OnBtnRankRewardClick()
        {
            UIManager.OpenUI(EUIID.UI_LadderPvp_RankReward);
        }

        public void OnBtnShopClick()
        {
            MallPrama mall = new MallPrama();
            mall.mallId = 1500;
            mall.shopId = 15001;
            UIManager.OpenUI(EUIID.UI_Mall, false, mall);
        }

        public void OnBtnTaskRewardClick()
        {
            UIManager.OpenUI(EUIID.UI_LadderPvp_TaskReward);
        }

        public void OnBtnTeamMemAdd()
        {
            //if (Sys_Team.Instance.isCaptain() == false)
            //    return;

            int statge = Sys_LadderPvp.Instance.GetLevelStage();
            var data = CSVTianTiWinAward.Instance.GetByIndex(statge -1);

            if (data != null&& data.Teamid > 0)
                Sys_Team.Instance.OpenFastUI(data.Teamid);
        }

        public void OnBtnTeamMemsClick()
        {
            OnBtnTeamMemAdd();
           // UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);
        }

        public void OnBtnTeamMemHead(uint id)
        {
            var mem = Sys_Team.Instance.getTeamMem((int)id);

            if (mem == null)
                return;

            if (mem.MemId == Sys_Role.Instance.RoleId)
                return;

            Sys_Role_Info.Instance.OpenRoleInfo(mem.MemId, Sys_Role_Info.EType.Avatar);
        }
        public void OnBtnTeamMemBG(uint id,Vector3 position)
        {
            var mem = Sys_Team.Instance.getTeamMem((int)id);

            if (mem == null)
                return;

            bool isselectown = (mem.MemId == Sys_Role.Instance.RoleId);
            bool iscap = Sys_Team.Instance.isCaptain();
            bool isleave = mem.IsLeave();

            int count =  m_MemMenu.GetCommands(iscap,isselectown,isleave);

            if (count == 0)
                return;

            m_MemMenu.MemID = mem.MemId;

            m_MemMenu.Open(position);
                
        }
    }
}
