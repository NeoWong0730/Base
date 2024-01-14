using Logic.Core;
using Packet;
using System.Collections.Generic;
using UnityEngine;
using Table;
namespace Logic
{
    public class UI_Multi_Ready : UIBase, UI_Multi_Ready_Layout.IListener
    {
        private UI_Multi_Ready_Layout m_Layout = new UI_Multi_Ready_Layout();


        private bool mbCountdown = false;

        private float mWaitTime = 60;

        private Dictionary<ulong, uint> mStateList = new Dictionary<ulong, uint>();

        private int mType = 1; // 1 队长  2 队员

        private int mState = 1;// 1 队长同意前 2 队员同意前 3，队长同意后\队员同意后

        private uint mGoOnStageID = 0;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.setListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.StartVote,OnStartVote,toRegister);

            Sys_Instance.Instance.eventEmitter.Handle<ulong, int>(Sys_Instance.EEvents.PlayerConfirmNtf, OnConfirmNtf,toRegister);

            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemState, OnMemberStateChange, toRegister);
        }
        protected override void OnOpen(object arg) // arg int  1 队长  2 队员
        {
            mStateList.Clear();            

            mType = (int)arg;
        }
        protected override void OnShow()
        {            
            if (mType == 1)
            {
                mState = 1;
                mGoOnStageID = Sys_Instance.Instance.GetNextEnterStageID(Sys_Instance.Instance.CurInstancID);
            }


            if (mType == 2)
            {
                mState = 2;

                mGoOnStageID = Sys_Instance.Instance.CurStageID;
            }
               
            SetMembersInfos();

            RefreshOpertor();

            RefreshTime();

            RefreshInstanceInfo();

            if(mState == 2)
               RefreshMembersState();

            var copyItem = CSVInstance.Instance.GetConfData(Sys_Instance.Instance.CurInstancID);

            m_Layout.SetBg(copyItem.bg);
        }

        protected override void OnUpdate()
        {
            if (mWaitTime > 0 && mbCountdown)
            {
                mWaitTime -= deltaTime;

                if (mWaitTime < 0)
                {
                    StopTime();

                    if(mType == 2 && mState == 2)
                      OnAgree();
                }

                int time = (int)Mathf.Max(0, mWaitTime) +1;

                m_Layout.SetTime(time.ToString());
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

                member.MemType = (UI_Multi_Ready_Layout.Member.EMemType)0;

                var proValue = Sys_Instance.Instance.getTeamMemProcess(teamMem.MemId);

                // var data = Sys_Instance.Instance.getBiographyChapter(proValue.StageID);

                string bestStr = string.Empty;

                if(proValue.StageID != 0)
                {
                    uint stageID = proValue.StageID;
                    var Dailydata = CSVInstanceDaily.Instance.GetConfData(stageID);
                    bestStr = LanguageHelper.GetTextContent(2009617, Dailydata.LayerStage.ToString(), Dailydata.LayerStage.ToString(), Dailydata.Layerlevel.ToString());
                }
                else
                    bestStr = LanguageHelper.GetTextContent(2009635);

                member.Desc = bestStr;

            }
        }

        private void RefreshMembersState()
        {
            int count = Sys_Team.Instance.TeamMemsCount;
            for (int i = 0; i < count; i++)
            {
                var member = m_Layout.getMemberAt(i);

                if (member == null)
                    break;

                var teamMem = Sys_Team.Instance.getTeamMem(i);

                member.MemType = (UI_Multi_Ready_Layout.Member.EMemType)getState(teamMem);
            }
        }

        private void OnMemberStateChange(ulong id)
        {
            RefreshMembersState();
        }
        private void RefreshOpertor()
        {
            bool value0 = true;
            bool value1 = mState == 1;
            bool value2 = mState != 3;

            uint strValue0 =(uint)( mState == 1 ? 2009626: (mState == 2 ? 2009627 : (mState == 3 ? 2009619 : 0)));

            uint strValue1 =(uint) (mState == 1 ? 2009614: 0);

            uint strValue2 = (uint)(mState == 1 ? 2009636 : (mState == 2 ? 2009620:0));

            m_Layout.SetBtnState(value0, strValue0, value1, strValue1, value2, strValue2);
        }


        private void RefreshTime()
        {
            if (mState == 1)
                StopTime();

            if (mState == 2 || mState == 3)
                StartTime();
        }

        private void RefreshInstanceInfo()
        {
            var seriesData = Sys_Instance.Instance.getSeries(Sys_Instance.Instance.CurInstancID);

            var Instancedata = CSVInstance.Instance.GetConfData(Sys_Instance.Instance.CurInstancID);

            var Chapterdata = Sys_Instance.Instance.getBiographyChapterLocal(mGoOnStageID);

            var StageData = CSVInstanceDaily.Instance.GetConfData(mGoOnStageID);

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

           var data = CSVParam.Instance.GetConfData(350);

            mWaitTime = int.Parse(data.str_value) / 1000f;

            mbCountdown = true;
        }

        private void StopTime()
        {
            m_Layout.SetTimeActive(false);
            mbCountdown = false;
        }
        private uint getState(TeamMem teamMem)
        {
            if (teamMem.MemId == Sys_Role.Instance.RoleId)
            {
                return(uint)(mState == 3 ? 1 : 3);
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

            return 3;

        }

        private void OnWaitOther()
        {
            mState = 3;
            RefreshOpertor();
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
        public void OnClickClose()
        {
            if (mState == 2) // 投票拒绝
            {
                Sys_Instance.Instance.OnSendPlayerConfirmres(false);
            }

            if (mState == 3)//放弃进入
            {
                Sys_Instance.Instance.OnSendExitConfirmres();
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
            if (mState == 1)//取消
            {
                CloseSelf();
            }

            if (mState == 2) // 投票拒绝
            {
                Sys_Instance.Instance.OnSendPlayerConfirmres(false);
            }

            if (mState == 3)//放弃进入
            {
                Sys_Instance.Instance.OnSendExitConfirmres();
            }
        }
        public void OnBtn02()
        {
            if (mState == 1)//重新开始
            {
                bool result = Sys_Instance.Instance.CheckTeamCondition(Sys_Instance.Instance.CurInstancID);

                if (result == false)
                    return;

                //OnWaitOther();
                uint stageID = Sys_Instance.Instance.GetFristStageID(Sys_Instance.Instance.CurInstancID);
                Sys_Instance.Instance.InstanceEnterReq(Sys_Instance.Instance.CurInstancID, stageID);
            }

        }
        public void OnBtn03()
        {
            if (mState == 1)//继续冒险
            {
                bool result = Sys_Instance.Instance.CheckTeamCondition(Sys_Instance.Instance.CurInstancID);

                if (result == false)
                    return;      

                Sys_Instance.Instance.InstanceEnterReq(Sys_Instance.Instance.CurInstancID, mGoOnStageID);
            }

            if (mState == 2)//投票同意
            {
                OnWaitOther();
                Sys_Instance.Instance.OnSendPlayerConfirmres(true);
            }

        }

        private void OnStartVote()
        {
            if(mType == 1 && mState == 1)
              OnWaitOther();
        }
    }
}
