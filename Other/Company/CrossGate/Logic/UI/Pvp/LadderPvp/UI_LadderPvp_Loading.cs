using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    ///pvp 加载
    /// </summary>
    public partial class UI_LadderPvp_Loading : UIBase
    {
        UI_LadderPvp_Loading_Layout m_Layout = new UI_LadderPvp_Loading_Layout();

        
       
        private float m_OpenTime;

        private int m_OtherProcess = 0;
        private int m_OwnProcess = 0;

        private float m_OtherProcessTimePoint = 0;
        private float m_OwnProcessTimePoint = 0;

        private int m_OtherProcessMax = 99;
        private int m_OwnProcessMax = 99;

        /// <summary>
        /// 0 是休闲模式 1 段位模式
        /// </summary>
        private int PvpType = 0;

       
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.LoadCompl, OnLoadCompl, toRegister);
            Sys_LadderPvp.Instance.eventEmitter.Handle(Sys_LadderPvp.EEvents.OneReadyOk, OnReadOk, toRegister);
        }
        protected override void OnShow()
        {
            PvpType = (int)Sys_LadderPvp.Instance.PvpType;

            m_OpenTime = Time.time;

            m_OtherProcessTimePoint = m_OpenTime;
            m_OwnProcessTimePoint = m_OpenTime;

            int count = Sys_LadderPvp.Instance.MatchDataNtf.Red.Players.Count;
            m_OwnProcessMax = Mathf.Min(99, 100 / count );
            count = Sys_LadderPvp.Instance.MatchDataNtf.Blue.Players.Count;
            m_OtherProcessMax = Mathf.Min(99, 100 / count);

            Refresh();
        }        

        protected override void OnUpdate()
        {
            float value = Time.time;

            if ((value - m_OpenTime) <= 4)
            {

                UpdateOwnProcess(value);

                UpdateOtherProcess(value);
            }
         
        }

        private void UpdateOwnProcess(float value)
        {
            if (m_OwnProcess >= 100)
                return;

            float diss = value - m_OwnProcessTimePoint;

            m_OwnProcess += (int)((1.1 * 1f * diss / 3f) * 100);

            m_OwnProcess = Mathf.Min(m_OwnProcess, 99);

            m_Layout.TexLeftProcess.text = m_OwnProcess.ToString();

            m_OwnProcessTimePoint = Time.time;

            m_Layout.ImgLeftProcess.fillAmount = m_OwnProcess / 100f;
        }
        private void UpdateOtherProcess(float value)
        {
            if (m_OtherProcess >= 100)
                return;

            float diss = value - m_OtherProcessTimePoint;

            m_OtherProcess += (int)((1f * diss / 3f) * 100);

            m_OtherProcess = Mathf.Min(m_OtherProcess, m_OtherProcessMax);

            m_Layout.TexRightProcess.text = m_OtherProcess.ToString();

            m_OtherProcessTimePoint = Time.time;

            m_Layout.ImgRightProcess.fillAmount = m_OwnProcess / 100f;
        }

        private void SetOwnProcessReady()
        {
            m_OwnProcessTimePoint = Time.time;
            m_OwnProcess = 100;

            m_Layout.TexLeftProcess.text = m_OwnProcess.ToString();
            m_Layout.ImgLeftProcess.fillAmount = 1;
        }

        private void SetOtherProcessReady()
        {
            m_OtherProcessTimePoint = Time.time;
            m_OtherProcess = 100;
            m_Layout.TexRightProcess.text = m_OtherProcess.ToString();
            m_Layout.ImgRightProcess.fillAmount = 1;
        }
        private void Refresh()
        {
            RefreshOwn();

            RefreshOther();
        }

        private void RefreshOwn()
        {
        

            int count = Sys_LadderPvp.Instance.MatchDataNtf.Red.Players.Count;
            m_Layout.LeftTeamGroup.SetChildSize(count);

            for (int i = 0; i < count; i++)
            {
                var teamMemberInfo = Sys_LadderPvp.Instance.MatchDataNtf.Red.Players[i];

                var item = m_Layout.LeftTeamGroup.getAt(i);

                item.TexName.text = teamMemberInfo.Base.Name.ToStringUtf8();
                CharacterHelper.SetHeadAndFrameData(item.ImgHead, teamMemberInfo.Base.HeroId, 0, 0);


                TextHelper.SetText(item.TexLevel, LanguageHelper.GetTextContent(1000002, teamMemberInfo.Base.Level.ToString()));

                uint icon = OccupationHelper.GetLogonIconID(teamMemberInfo.Base.Career);
                ImageHelper.SetIcon(item.ImgCareerIcon, icon);

                var score = PvpType == 0 ? teamMemberInfo.Ttleisure.Score
                    : teamMemberInfo.Ttdanlv.Score;

                var danlv = Sys_LadderPvp.Instance.GetDanLvIDByScore(score);

                string danlvstring = Sys_LadderPvp.Instance.GetDanLvLevelString(danlv);
                item.TexDanLv.text = danlvstring;


            }
        }

        private void RefreshOther()
        {
            int count = Sys_LadderPvp.Instance.MatchDataNtf.Blue.Players.Count;

            m_Layout.RightTeamGroup.SetChildSize(count);

            for (int i = 0; i < count; i++)
            {
                var teamMemberInfo = Sys_LadderPvp.Instance.MatchDataNtf.Blue.Players[i];

                var item = m_Layout.RightTeamGroup.getAt(i);

                item.TexName.text = teamMemberInfo.Base.Name.ToStringUtf8();
                CharacterHelper.SetHeadAndFrameData(item.ImgHead, teamMemberInfo.Base.HeroId, 0, 0);


                TextHelper.SetText(item.TexLevel, LanguageHelper.GetTextContent(1000002, teamMemberInfo.Base.Level.ToString()));

                uint icon = OccupationHelper.GetLogonIconID(teamMemberInfo.Base.Career);
                ImageHelper.SetIcon(item.ImgCareerIcon, icon);


                var score = PvpType == 0 ? teamMemberInfo.Ttleisure.Score
                    : teamMemberInfo.Ttdanlv.Score;

                var danlv = Sys_LadderPvp.Instance.GetDanLvIDByScore(score);

                string danlvstring = Sys_LadderPvp.Instance.GetDanLvLevelString(danlv);
                item.TexDanLv.text = danlvstring;


            }


        }

    }

    /// <summary>
    /// 监听处理
    /// </summary>
    public partial class UI_LadderPvp_Loading : UI_LadderPvp_Loading_Layout.IListener
    {
        private void OnLoadCompl()
        {
            CloseSelf();
        }

        private void OnReadOk()
        {

            var teamleft = Sys_LadderPvp.Instance.PvpTeamList[0];
            int count = teamleft.Items.Count;
            int readlycount = 0;
            for (int i = 0; i < count; i++)
            {
                if (teamleft.Items[i].LoadOK)
                {
                    readlycount += 1;
                }
            }

            if (readlycount == count)
            {
                SetOwnProcessReady();
            }
            else
            {
                m_OwnProcessMax = Mathf.Min(99, 100 / count * (readlycount+1) );
            }


            var teamright = Sys_LadderPvp.Instance.PvpTeamList[1];
             count = teamright.Items.Count;
             readlycount = 0;
            for (int i = 0; i < count; i++)
            {
                if (teamright.Items[i].LoadOK)
                {
                    readlycount += 1;
                }
            }

            if (readlycount == count)
            {
                SetOtherProcessReady();
            }
            else
            {
                m_OtherProcessMax = Mathf.Min(99, 100 / count * (readlycount + 1));
            }

        }
    }
}
