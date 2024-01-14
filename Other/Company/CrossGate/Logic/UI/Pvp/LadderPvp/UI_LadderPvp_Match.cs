using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    /// 匹配
    /// </summary>
    public partial class UI_LadderPvp_Match : UIBase, UI_LadderPvp_Match_Layout.IListener
    {
        UI_LadderPvp_Match_Layout m_Layout = new UI_LadderPvp_Match_Layout();

       
        float m_StartTime;
        float m_LastTime;

        bool m_TimeOut = false;
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.CancleMatch, OnPvpCancleMatch, toRegister);

            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.MatchSuccess, OnPvpMatchSuccess, toRegister);

            Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStart, OnReconnectStart, toRegister);

           

        }
        protected override void OnShow()
        {            
            m_StartTime = Time.time;
            m_LastTime = Time.time;

            m_Layout.SetMatchTimeTex(GetTimeMinAndSecString(0) + ":" + GetTimeMinAndSecString(0));

            Refresh();
        }
        protected override void OnHide()
        {            
           
        }


        protected override void OnUpdate()
        {
            float time = Time.time;

            float value = time - m_LastTime;

            if (value > 0.9)
            {
                float totalTime = time - m_StartTime;

                m_LastTime = time;

                int min = (int)(totalTime / 60);
                int sec = (int)(totalTime - min * 60);
              
                m_Layout.SetMatchTimeTex(GetTimeMinAndSecString(min) + ":" + GetTimeMinAndSecString(sec));
            }


            if (Sys_Time.Instance.GetServerTime() - Sys_LadderPvp.Instance.MatchStartTime >= 180 && !m_TimeOut && Sys_Team.Instance.isFull() == false
                && (Sys_Team.Instance.HaveTeam == false || Sys_Team.Instance.isCaptain()))
            {
                m_TimeOut = true;
                // OnPvpCancleMatch();
                Sys_LadderPvp.Instance.Apply_CancleMathch();
                Sys_Team.Instance.OpenTips(0, LanguageHelper.GetTextContent(1340001817u), null, null, null,false);
            }
        }

        private void OnReconnectStart()
        {
            CloseSelf();
        }
        private string GetTimeMinAndSecString(int time)
        {
            string minstr = time == 0 ? ("00") : (time < 10 ? ("0" + time) : time.ToString());

            return minstr;
        }
        private void Refresh()
        {
            int count = Sys_Team.Instance.TeamMemsCount;
            int showCount = count;

            if (count == 0)
            {
                RefreshWithout();
                return;
            }
            m_Layout.TeamMemGroup.SetChildSize(showCount);


            int infocount = Sys_LadderPvp.Instance.TeamMembersInfo.Members.Count;

            for (int i = 0; i < showCount; i++)
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

                // var danlv = Sys_LadderPvp.Instance.TeamMembersInfo.Members[i].Score;

                uint score = 0;
                if (Sys_LadderPvp.Instance.TeamMembersInfo != null && i < infocount)
                {
                    score = Sys_LadderPvp.Instance.TeamMembersInfo.Members[i].Score;
                }
                uint danlv = Sys_LadderPvp.Instance.GetDanLvIDByScore(score);

                string danlvstring = Sys_LadderPvp.Instance.GetDanLvLevelString(danlv);
                item.TexDanLevel.text = danlvstring;

                item.TransLeave.gameObject.SetActive(teamMemberInfo.IsLeave());
                item.TransOffline.gameObject.SetActive(teamMemberInfo.IsOffLine());

            }

        }

        private void RefreshWithout()
        {
            m_Layout.TeamMemGroup.SetChildSize(1);

            var item = m_Layout.TeamMemGroup.getAt(0);

            item.TransAdd.gameObject.SetActive(false);
            item.TransInfo.gameObject.SetActive(true);

            item.TexName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();

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
        }


    }

    /// <summary>
    /// 监听处理
    /// </summary>
    public partial class UI_LadderPvp_Match : UIBase, UI_LadderPvp_Match_Layout.IListener
    {
        public void OnClickMatch()
        {
            Sys_LadderPvp.Instance.Apply_CancleMathch();
        }

        private void OnPvpCancleMatch()
        {
            CloseSelf();

            UIManager.OpenUI(EUIID.UI_LadderPvp);
        }

        private void OnPvpMatchSuccess()
        {
            CloseSelf();

           
        }
    }


}
