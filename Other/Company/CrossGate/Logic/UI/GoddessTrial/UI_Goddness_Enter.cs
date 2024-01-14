using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public partial class UI_Goddness_Enter : UIBase, UI_Goddness_Enter_Layout.IListener
    {

        UI_Goddness_Enter_Layout m_Layout = new UI_Goddness_Enter_Layout();

       // private int m_DifficlyID = 1;
        //private int m_DifficlySelectIndex = 0;
       // private int m_CurChapterIndex = 0;

       // private CSVGoddessTopic.Data m_SelectData = null;

       // private List<uint> m_DifficlyLangue = new List<uint>();
 
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);



            var data = CSVParam.Instance.GetConfData(350);

            mWaitTime = int.Parse(data.str_value) / 1000f;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.StartVote, OnStartVote, toRegister);

            Sys_GoddnessTrial.Instance.eventEmitter.Handle<ulong, int>(Sys_GoddnessTrial.EEvents.DoVate, OnConfirmNtf, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemState, OnMemberStateChange, toRegister);

            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.VoteUpdate, OnVoteUpdate, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<TeamMem>(Sys_Team.EEvents.MemEnterNtf, OnMemberEnter, toRegister);

            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.TeamMemsRecord, OnTeamMemsRecord, toRegister);
        }

        protected override void OnOpen(object arg)
        {            
            int value = arg == null ? 0 : (int)arg;

            mEState = value == 0 ? EState.BeforeMem : EState.BeforeCaptain;

        }
        protected override void OnShow()
        {            
            Refresh();

            if (mEState == EState.BeforeCaptain)
                StopTime();
            else
                StartTime();
        }

        protected override void OnClose()
        {
            if (m_WaitTimer != null && m_WaitTimer.isDone == false)
                m_WaitTimer.Cancel();
        }
        private void Refresh()
        {
            uint InstanceID = Sys_GoddnessTrial.Instance.SelectInstance;

            var order = Sys_GoddnessTrial.Instance.GetCurSelectTopicChapterIndex(InstanceID);
            var instanData = CSVInstance.Instance.GetConfData(InstanceID);
            string instanName = instanData == null ? string.Empty : LanguageHelper.GetTextContent(instanData.Name);
            string strrecord = LanguageHelper.GetTextContent(2022352, order.ToString(), instanName.ToString());
            m_Layout.SetChapterName(strrecord);

            var data = CSVGoddessTopic.Instance.GetConfData(Sys_GoddnessTrial.Instance.SelectID);
            uint difficLan = data == null ? 0 : Sys_GoddnessTrial.Instance.m_DifficlyLangue[(int)data.topicDifficulty - 1];
            m_Layout.SetDiffic(difficLan);
            m_Layout.SetTopicName(data == null ? 0 : data.topicLan);


            RefreshTeamMembers();

            RefreshOpertor();
        }


        private void RefreshTeamMembers()
        {
            int count = Sys_Team.Instance.TeamMemsCount;

            m_Layout.SetMemsSize(count);


            for (int i = 0; i < count; i++)
            {
                var teamMem = Sys_Team.Instance.getTeamMem(i);

                m_Layout.SetMemLevel(i,teamMem.Level);

                m_Layout.SetMemName(i, teamMem.Name.ToStringUtf8());

                m_Layout.SetMemCareer(i, teamMem.Career, teamMem.CareerRank);

                m_Layout.SetMemIndex(i, (uint)(i + 1));

                m_Layout.SetMemHeadIcon(i, teamMem.HeroId, teamMem.Photo, teamMem.PhotoFrame);

                m_Layout.SetMemSetae(i, (UI_Goddness_Enter_Layout.EMemType)getState(teamMem));

                var InstanceID = Sys_GoddnessTrial.Instance.GetMemRecordIns(teamMem.MemId);

                string strrecord = string.Empty;

                var order = Sys_GoddnessTrial.Instance.GetCurSelectTopicChapterIndex(InstanceID);

                if (InstanceID != 0 && order > 0)
                { 
                    var instanData = CSVInstance.Instance.GetConfData(InstanceID);
                    string instanName = instanData == null ? string.Empty : LanguageHelper.GetTextContent(instanData.Name);
                    strrecord = LanguageHelper.GetTextContent(2022352, (order).ToString(), instanName.ToString());
                }
                else
                {
                    strrecord = LanguageHelper.GetTextContent(2022360);
                }

                m_Layout.SetMemRecord(i, strrecord);
            }
        }

        private void RefreshMembersState()
        {
            int count = Sys_Team.Instance.TeamMemsCount;

            for (int i = 0; i < count; i++)
            {
                var teamMem = Sys_Team.Instance.getTeamMem(i);

                m_Layout.SetMemSetae(i, (UI_Goddness_Enter_Layout.EMemType) getState(teamMem));
               
            }
        }
        private uint getState(TeamMem teamMem)
        {

            if (Sys_Team.Instance.isCaptain() && mState < 3)
                return 0;


            //if (teamMem.MemId == Sys_Role.Instance.RoleId)
            //{
            //    return (uint)(mState == 3 ? 1 : 3);
            //}


            if (teamMem.IsLeave())
                return 5;

            if (teamMem.IsOffLine())
                return 4;

            if (mStateList.ContainsKey(teamMem.MemId))
                return mStateList[teamMem.MemId];

            if (Sys_Team.Instance.isCaptain(teamMem.MemId))
            {
                return 1;
            }

            return 3;

        }

    }

    public partial class UI_Goddness_Enter : UIBase, UI_Goddness_Enter_Layout.IListener
    {
        enum EState
        {
            BeforeCaptain,
            BeforeMem,
            Wait
        }
        private bool mbCountdown = false;

        private float mWaitTime = 60;

        private Dictionary<ulong, uint> mStateList = new Dictionary<ulong, uint>();

       // private int mType = 1; // 1 队长  2 队员

        private int mState = 1;// 1 队长同意前 2 队员同意前 3，队长同意后\队员同意后

        private EState mEState = EState.BeforeMem;

       // private uint mGoOnStageID = 0;


        private Timer m_WaitTimer = null;
        private void RefreshOpertor()
        {
            bool value0 = true;
            bool value1 = false;// mEState == EState.BeforeCaptain;
            bool value2 = mEState != EState.Wait;

            uint strValue0 = (uint)(mEState == EState.BeforeCaptain ? 2009626 : (mEState == EState.BeforeMem ? 2009627 : (mEState == EState.Wait ? 2009619 : 0)));

            uint strValue1 = (uint)(mEState == EState.BeforeCaptain ? 2009614 : 0);

            uint strValue2 = (uint)(mEState == EState.BeforeCaptain ? 2009636 : (mEState == EState.BeforeMem ? 2009620 : 0));

            m_Layout.SetBtnState(value0, strValue0, value1, strValue1, value2, strValue2);
        }

        private void StopTime()
        {
            m_Layout.SetTimeActive(false);
            mbCountdown = false;
        }

        private void StartTime()
        {
            if (mbCountdown)
                return;

            m_Layout.SetTimeActive(true);

            mbCountdown = true;

            if (m_WaitTimer != null && m_WaitTimer.isDone == false)
                m_WaitTimer.Cancel();

            m_WaitTimer = Timer.Register(mWaitTime, OnCompleteTime, OnUpdateTime);
        }

        private void OnWaitOther()
        {
            mState = 3;

            mEState = EState.Wait;

            RefreshOpertor();

            StartTime();

            RefreshMembersState();


        }

        private void OnUpdateTime(float vlaue)
        {
            int time = (int)((mWaitTime + 0.1f) - (vlaue));

            m_Layout.SetTime(time.ToString());
        }

        private void OnCompleteTime()
        {
            StopTime();

        }
    }

    public partial class UI_Goddness_Enter : UIBase, UI_Goddness_Enter_Layout.IListener
    {
        private void OnStartVote()
        {
           // mEState = EState.Wait;

            RefreshOpertor();

            OnWaitOther();


        }

        private void OnVoteUpdate()
        {
            RefreshTeamMembers();

            RefreshMembersState();
        }
        private void OnConfirmNtf(ulong roleID, int result)
        {
            var mem = Sys_Team.Instance.getTeamMem(roleID);

            if (result == 2)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009651, mem.Name.ToStringUtf8()));

            if (mStateList.ContainsKey(roleID) == false)
                mStateList.Add(roleID, (uint)result);
            else
                mStateList[roleID] = (uint)result;

            RefreshMembersState();
        }

        private void OnMemberStateChange(ulong roleid)
        {
            RefreshMembersState();
        }

        private void OnMemberEnter(TeamMem memm)
        {
            RefreshTeamMembers();

            RefreshMembersState();
        }

        private void OnTeamMemsRecord()
        {
            RefreshTeamMembers();
        }
    }
    public partial class UI_Goddness_Enter : UIBase, UI_Goddness_Enter_Layout.IListener
    {
        public void OnBtn01()
        {
            if (mEState == EState.BeforeCaptain)//取消
            {
                CloseSelf();
            }

            if (mEState == EState.BeforeMem) // 投票拒绝
            {
                Sys_GoddnessTrial.Instance.OnSendPlayerConfirmres(false);
            }

            if (mEState == EState.Wait)//放弃进入
            {
                Sys_GoddnessTrial.Instance.OnSendExitConfirmres();
            }
        }

        public void OnBtn02()
        {
            if (mEState == EState.BeforeCaptain)//重新开始
            {
                bool result = Sys_Instance.Instance.CheckTeamCondition(Sys_GoddnessTrial.Instance.SelectInstance);

                if (result == false)
                    return;

                var topicdata = Sys_GoddnessTrial.Instance.GetGoddessTopicData(Sys_GoddnessTrial.Instance.SelectDifficlyID);

                Sys_GoddnessTrial.Instance.SetSelectInstance(topicdata.InstanceId[0]);

                Sys_Instance.Instance.InstanceEnterReq(Sys_GoddnessTrial.Instance.SelectInstance, Sys_GoddnessTrial.Instance.SelectStageID);
            }
        }

        public void OnBtn03()
        {
            if (mEState == EState.BeforeCaptain)//继续冒险
            {
                bool result = Sys_Instance.Instance.CheckTeamCondition(Sys_GoddnessTrial.Instance.SelectInstance);

               // DebugUtil.LogError("check Team condition result : " + result);

                if (result == false)
                    return;

                Sys_GoddnessTrial.Instance.SendInstanceEnterReq(Sys_GoddnessTrial.Instance.SelectInstance, Sys_GoddnessTrial.Instance.SelectStageID);
            }

            if (mEState == EState.BeforeMem)//投票同意
            {
               
                Sys_GoddnessTrial.Instance.OnSendPlayerConfirmres(true);

                OnWaitOther();
            }
        }

        public void OnClickClose()
        {
            if (mEState == EState.BeforeMem) // 投票拒绝
            {
                Sys_GoddnessTrial.Instance.OnSendPlayerConfirmres(false);
            }

            if (mEState == EState.Wait)//放弃进入
            {
                Sys_GoddnessTrial.Instance.OnSendExitConfirmres();
            }

            CloseSelf();
        }
    }
}
