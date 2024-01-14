using Logic.Core;
using Packet;
using System.Collections.Generic;
using UnityEngine;
using Table;
namespace Logic
{
    public class UI_Multi_ReadyNew_Parmas
    {
        public bool isCapter { get; set; } = true;

        public uint InstanceID { get; set; } = 0;

        public uint LevelID { get; set; } = 0;

        /// <summary>
        ///决定当前投票的状态 1 队长同意前 2 队员同意前 3，队长同意后\队员同意后
        /// </summary>
        public int State { get; set; } = 1;

    }
    public class UI_Multi_ReadyNew : UIBase, UI_Multi_ReadyNew_Layout.IListener
    {
        private UI_Multi_ReadyNew_Layout m_Layout = new UI_Multi_ReadyNew_Layout();


        private bool mbCountdown = false;

        private float mWaitTime = 60;

        private Dictionary<ulong, uint> mStateList = new Dictionary<ulong, uint>();

        UI_Multi_ReadyNew_Parmas m_Parmas = new UI_Multi_ReadyNew_Parmas();

        /// <summary>
        ///决定当前投票的状态 1 队长同意前 2 队员同意前 3，队长同意后\队员同意后
        /// </summary>
        private int State { get; set; } = 1;
        private uint LevelID { get; set; } = 0;

        private float ReadyTime = 0;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.setListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Instance_Bio.Instance.eventEmitter.Handle(Sys_Instance_Bio.EEvents.StartVote,OnStartVote,toRegister);

            Sys_Instance_Bio.Instance.eventEmitter.Handle<ulong, int>(Sys_Instance_Bio.EEvents.PlayerConfirmNtf, OnConfirmNtf,toRegister);

            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemState, OnMemberStateChange, toRegister);

            Sys_Instance_Bio.Instance.eventEmitter.Handle(Sys_Instance_Bio.EEvents.VoteUpdate, OnVoteUpdate, toRegister);

            Sys_Instance_Bio.Instance.eventEmitter.Handle(Sys_Instance_Bio.EEvents.VoteData, OnVoteUpdate, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<TeamMem>(Sys_Team.EEvents.MemEnterNtf, OnTeamMemberEnter, toRegister);
        }
        protected override void OnOpen(object arg) // arg int  1 队长  2 队员
        {
            mStateList.Clear();

            var value = arg as UI_Multi_ReadyNew_Parmas;
            if (value != null)
                m_Parmas = value;


            var data = CSVParam.Instance.GetConfData(350);

            ReadyTime = int.Parse(data.str_value) / 1000f;
        }
        protected override void OnShow()
        {

            State = m_Parmas.State;

            SetMembersInfos();

            RefreshOpertor();
            SetOpertorEnable(true);

            RefreshTime();

            RefreshInstanceInfo();

            if(State == 2)
               RefreshMembersState();

            var copyItem = CSVInstance.Instance.GetConfData(m_Parmas.InstanceID);

            m_Layout.SetBg(copyItem.bg);
        }

        protected override void OnHide()
        {
          
           
        }
        protected override void OnUpdate()
        {
            if (mWaitTime > 0 && mbCountdown)
            {
                mWaitTime -= deltaTime;

                if (mWaitTime < 0)
                {
                    StopTime();


                    CloseSelf();

                    if (m_Parmas.isCapter == false && State == 2)
                    {
                        OnAgree();
                        
                    }
                    else if (m_Parmas.isCapter)
                    {
                       // UIManager.OpenUI(EUIID.UI_Bio_PlayType);
                    }


                }

                int time = (int)Mathf.Max(0, mWaitTime) +1;

                m_Layout.SetTime(time.ToString());

                m_Layout.SetTimeProcess(1- (mWaitTime / ReadyTime));
            }

            
        }

        private void SetMembersInfos()
        {
            int count = Sys_Team.Instance.TeamMemsCount;

            m_Layout.SetMemberSize(count);


            for (int i = 0; i < count; i++)
            {
                var member = m_Layout.getMemberAt(i);

                var teamMem = Sys_Team.Instance.getTeamMem(i);

                member.Level = (int)teamMem.Level;

                member.Name = teamMem.Name.ToStringUtf8();

                member.Profession = teamMem.Career;

                member.Rank = teamMem.CareerRank;

                member.Index = i + 1;

                member.Head = CharacterHelper.getHeadID(teamMem.HeroId,teamMem.Photo);

                member.HeadFrame = CharacterHelper.getHeadFrameID(teamMem.PhotoFrame);

                member.MemType = (UI_Multi_ReadyNew_Layout.Member.EMemType)getState(teamMem);

               // var proValue = Sys_Instance.Instance.getTeamMemProcess(teamMem.MemId);

                // var data = Sys_Instance.Instance.getBiographyChapter(proValue.StageID);

                string bestStr = string.Empty;


                uint times = Sys_Instance_Bio.Instance.GetVoteMemberLeftTimes(teamMem.MemId);
                //if(proValue.StageID != 0)
                //{
                //    uint stageID = proValue.StageID;
                //    var Dailydata = CSVInstanceDaily.Instance.GetConfData(stageID);
                //    bestStr = LanguageHelper.GetTextContent(2009617, Dailydata.LayerStage.ToString(), Dailydata.LayerStage.ToString(), Dailydata.Layerlevel.ToString());
                //}
                //else
                    bestStr = LanguageHelper.GetTextContent(2022902,times.ToString());

                member.Desc = bestStr;

            }
        }

        private void RefreshMembersState()
        {
            int count = Sys_Team.Instance.TeamMemsCount;

           // Debug.LogError("RefreshMembersState------");

            for (int i = 0; i < count; i++)
            {
                var member = m_Layout.getMemberAt(i);

                if (member == null)
                    break;

                var teamMem = Sys_Team.Instance.getTeamMem(i);

                var statetype = (UI_Multi_ReadyNew_Layout.Member.EMemType)getState(teamMem);

                member.MemType = statetype;

               // Debug.LogError("state type -- " + statetype.ToString());
            }
        }

        private void OnMemberStateChange(ulong id)
        {
            RefreshMembersState();
        }

        private void OnTeamMemberEnter(TeamMem teamMem)
        {
            SetMembersInfos();

            RefreshMembersState();
        }
        private void RefreshOpertor()
        {
            bool value0 = true;
            bool value1 = State == 1;
            bool value2 = State != 3;

            uint strValue0 =(uint)(State == 1 ? 2009626: (State == 2 ? 2009627 : (State == 3 ? 2009619 : 0)));

            uint strValue1 =(uint) (State == 1 ? 2009614: 0);

            uint strValue2 = (uint)(State == 1 ? 2009636 : (State == 2 ? 2009620:0));

            m_Layout.SetBtnState(value0, strValue0, value1, strValue1, value2, strValue2);
        }

        private void SetOpertorEnable(bool enable)
        {
            bool value0 = true && enable;
            bool value1 = State == 1 && enable;
            bool value2 = State != 3 &&enable;

            //m_Layout.SetBtnEnable(value0, value1, value2);
        }
        private void RefreshTime()
        {
            if (State == 1)
                StopTime();

            if (State == 2 || State == 3)
                StartTime();
        }

        private void RefreshInstanceInfo()
        {
            var seriesData = Sys_Instance_Bio.Instance.getSeries(m_Parmas.InstanceID);

            var Instancedata = CSVInstance.Instance.GetConfData(m_Parmas.InstanceID);

            var Chapterdata = CSVInstanceDaily.Instance.GetConfData (m_Parmas.LevelID);

            var StageData = CSVInstanceDaily.Instance.GetConfData(m_Parmas.LevelID);

            string series = LanguageHelper.GetTextContent(seriesData.Name);
            string inst = LanguageHelper.GetTextContent(Instancedata.Name);
            //string chapr = LanguageHelper.GetTextContent(Chapterdata.Des);
            string num = LanguageHelper.GetTextContent(Chapterdata.Name) + string.Format("{0}.{1}", StageData.LayerStage, StageData.Layerlevel);

            m_Layout.SetTitleInfo(series, inst, num);
        }
        private void StartTime()
        {
            if (mbCountdown)
                return;

            m_Layout.SetTimeActive(true);

            mWaitTime = Sys_Instance_Bio.Instance.GetVoteTime();

            mbCountdown = true;

            m_Layout.SetTime(mWaitTime.ToString());
        }

        private void StopTime()
        {
            m_Layout.SetTimeActive(false);
            mbCountdown = false;
        }
        private uint getState(TeamMem teamMem)
        {
            if (m_Parmas.isCapter && State < 3)
                return 0;

            
            if (teamMem.MemId == Sys_Role.Instance.RoleId)
            {
                return(uint)(State == 3 ? 1 : 3);
            }
                

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

            return (uint) (3);

        }

        private void OnWaitOther()
        {
            State = 3;
            RefreshOpertor();

            SetOpertorEnable(true);

            StartTime();

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

        private void OnVoteUpdate()
        {
            SetMembersInfos();
        }
        public void OnClickClose()
        {
            if (State == 2) // 投票拒绝
            {
                Sys_Instance_Bio.Instance.OnSendPlayerConfirmres(false);
            }

            if (State == 3)//放弃进入
            {
                Sys_Instance_Bio.Instance.OnSendExitConfirmres();
            }

            CloseSelf();
        }

        public void OnAgree()
        {
            //StopTime();
        }

        public void OnRefuse()
        {
            StopTime();
        }

        public void OnBtn01()
        {
            if (State == 1)//取消
            {
                CloseSelf();
            }

            if (State == 2) // 投票拒绝
            {
                Sys_Instance_Bio.Instance.OnSendPlayerConfirmres(false);
            }

            if (State == 3)//放弃进入
            {
                Sys_Instance_Bio.Instance.OnSendExitConfirmres();
            }
        }
        public void OnBtn02()
        {
            if (State == 1)//重新开始
            {
                bool result = Sys_Instance.Instance.CheckTeamCondition(m_Parmas.InstanceID);

                if (result == false)
                    return;

                uint stageID = Sys_Instance.Instance.GetFristStageID(m_Parmas.InstanceID);

                Sys_Instance.Instance.InstanceEnterReq(m_Parmas.InstanceID, stageID);
            }

        }
        public void OnBtn03()
        {
            if (State == 1)//继续冒险
            {
                bool result = Sys_Instance.Instance.CheckTeamCondition(m_Parmas.InstanceID);

                if (result == false)
                    return;      

                Sys_Instance.Instance.InstanceEnterReq(m_Parmas.InstanceID, m_Parmas.LevelID);
            }

            if (State == 2)//投票同意
            {
                OnWaitOther();
                Sys_Instance_Bio.Instance.OnSendPlayerConfirmres(true);
            }

        }

        private void OnStartVote()
        {
            if(m_Parmas.isCapter && State == 1)
              OnWaitOther();
        }
    }
}
