using System;
using System.Collections.Generic;

using UnityEngine;
using Logic.Core;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Goddness_Select_Parma
    {
        public int State = 0;
        public uint LevelID = 0;
        public uint InstanceID = 0;
    }
    public partial class UI_Goddness_Select:UIBase, UI_Goddness_Select_Layout.IListener
    {
        private UI_Goddness_Select_Layout m_Layout = new UI_Goddness_Select_Layout();

        private UI_Goddness_Select_Parma m_Parma = null;

        private int mState = 0;  // 0 介绍， 1 选择投票, 3 介绍且自动切关


        private int mInstanceIndex = -1;
        private CSVGoddessTopic.Data mTopicData = null;

        private Timer m_Timer;

        private bool m_IsVoteEnd = false;

        private float m_WaitVoteTime = 10;

        private float m_WaitVoteShowTime = 0;

        private bool m_AutoSwitch = false;

        private uint m_VoteEndTimePoint = 0;


        private bool m_IsFollowCaptonVote = false;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpen(object arg)
        {
            m_Parma = arg as UI_Goddness_Select_Parma;

            int value = m_Parma == null ? 0 : m_Parma.State;

            SetState(value);

            var timedata = CSVParam.Instance.GetConfData(845);

            if (timedata != null)
            {
                int tvalue = 10;
                if (int.TryParse(timedata.str_value, out tvalue))
                {
                    m_WaitVoteTime = tvalue / (1000f);
                }
            }
            m_WaitVoteTime = GetParmaConfigTime(845);
            m_WaitVoteShowTime = GetParmaConfigTime(846);

        }

        private float GetParmaConfigTime(uint id)
        {
            var timedata = CSVParam.Instance.GetConfData(id);

            float time = 0;
            if (timedata != null)
            {
                int tvalue = 10;
                if (int.TryParse(timedata.str_value, out tvalue))
                {
                    time = tvalue / (1000f);

                }
            }

            return time;
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.SelectStageVote, OnSelectStateVote, toRegister);
            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.SelectFollow, OnSelectStateVoteFollow, toRegister);
            Sys_GoddnessTrial.Instance.eventEmitter.Handle<uint>(Sys_GoddnessTrial.EEvents.StageVoteProcess, OnStageVoteProcess, toRegister);
            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.StageVoteEnd, OnStageVoteEnd, toRegister);

            ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnBeforeEnterFightEffect, OnEnterFight, toRegister);
        }


        protected override void ProcessEvents(bool toRegister)
        {
            Sys_GoddnessTrial.Instance.eventEmitter.Handle<uint ,uint>(Sys_GoddnessTrial.EEvents.SelectStartVote, OnSelectStartVote, toRegister);
        }
        protected override void OnShow()
        {
            RefreshTitle();

            if (mState == 1)
                RefreshSelect();
            else
                RefreshDestri();

            var data =  CSVInstanceDaily.Instance.GetConfData(m_Parma.LevelID);

            
        }

        private void RefreshTitle()
        {
            mInstanceIndex = Sys_GoddnessTrial.Instance.GetTopicDataByInstanceID(m_Parma.InstanceID, out mTopicData);

            var stageLan = GetStageIndexLangue();
            string title0 = stageLan == 0 ? string.Empty : LanguageHelper.GetTextContent(stageLan);

            var InstanceData = CSVInstance.Instance.GetConfData(m_Parma.InstanceID);
            string title1 = InstanceData == null ? string.Empty : LanguageHelper.GetTextContent(InstanceData.Name);

            m_Layout.SetTitle(title0, title1);
        }

        private uint GetStageIndexLangue()
        {
            var data = Sys_GoddnessTrial.Instance.GetCurSelectTopicChapterIndex(m_Parma.InstanceID);//csvc.Instance.GetConfData(m_Parma.LevelID);

             if (data == 0)
                return 0;

            return 2022354u + ((uint)data - 1);
        }
        protected override void OnHide()
        {
            CloseSelf();

        }

        protected override void OnClose()
        {
            if (m_Timer != null )
            {
                m_Timer.Cancel();
                m_Timer = null;
            }

            m_AutoSwitch = false;

            m_IsVoteEnd = false;


            hadSelectVoteIndex = -1;

            m_IsFollowCaptonVote = false;

            mWaitFollowOp = -1;
        }

        private void SetState(int value)
        {
            if (value == 1)
            {
                m_AutoSwitch = true;
            }

            mState = value == 1 ? 1 : 0;
        }
        private void RefreshDestri()
        {
            m_Layout.SetViewDisActive(true);
            m_Layout.SetSelectActive(false);


            var data =  CSVGoddessSelect.Instance.GetConfData(m_Parma.LevelID);

            m_Layout.SetViewDisInfoText(data.StageLan);

           // m_Layout.SetViewDisInfoText((mTopicData == null || mInstanceIndex < 0) ? 0: mTopicData.lanID[mInstanceIndex]);
        }

        private void RefreshSelect()
        {

            m_Layout.SetViewDisActive(false);
            m_Layout.SetSelectActive(true);


            var data = CSVGoddessSelect.Instance.GetConfData(m_Parma.LevelID);
            m_Layout.SetSelectInfoText(data.QuestionLan);


            var SelectData = Sys_GoddnessTrial.Instance.GetChapterSelect(m_Parma.LevelID);

            int count = SelectData.StatgeCount();


            //DebugUtil.LogError(" refresh select select count  " + count.ToString() + "level id" + m_Parma.LevelID.ToString());

            m_Layout.SetSelectCount(count);

            for (int i = 0; i < count; i++)
            {
                m_Layout.SetSelect(i, SelectData.GetStatge(i), SelectData.GetStatgeLanID(i), false);
            }

            m_Layout.SetFollow(false);
            m_Layout.SetFollowActive(Sys_Team.Instance.isCaptain() == false);

            m_Layout.SelectTimeActive(true);

            if (m_Timer == null || (m_Timer != null && m_Timer.isDone))
                m_Timer = Timer.Register(m_WaitVoteTime, RefreshTimeOver, RefreshTime);
        }

        private void RefreshTime(float value)
        {
            int time = Mathf.Max(0, (int)(m_WaitVoteTime - value + 0.5f));

            m_Layout.SetSelectTime(time.ToString() + "s");
        }

        private void RefreshTimeOver()
        {

            if (m_Timer != null)
                m_Timer.Cancel();

            m_Timer = null;

           // m_IsVoteEnd = true;

           
        }

        private void WaitShowEndTimeOver()
        {

            OnClickSelectClose();
        }
        private void RefreshSelectEnd()
        {
            m_IsVoteEnd = true;

            m_VoteEndTimePoint = Sys_Time.Instance.GetServerTime();

            RefreshSelectResult();

            if (m_Timer != null)
                m_Timer.Cancel();

            m_Timer = Timer.Register(m_WaitVoteShowTime, WaitShowEndTimeOver);
        }

        private void RefreshSelectResult()
        {
            m_Layout.ShowSelectResultActive();

            //m_Layout.SetSelectCount(1);

            var resultID = Sys_GoddnessTrial.Instance.StateVoteResult.StageIndex;

            var SelectData = Sys_GoddnessTrial.Instance.GetChapterSelect(m_Parma.LevelID);

            int count = SelectData.StatgeCount();

            m_Layout.SetSelectCount(count);

            int votetotlenum = Sys_Team.Instance.TeamMemsCount;

           // DebugUtil.LogError("vote result : " + resultID.ToString());

            int maxVoteNumIndex = 0;
            uint maxVoteNum = 0;

            for (int i = 0; i < count; i++)
            { 

                var id = SelectData.GetStatge(i);

                var votenum = VoteCount((uint)i);

                float process = votenum * 1f / votetotlenum;

                int processInt = ((int)(process * 100));

                string strprocess = LanguageHelper.GetTextContent(2022324, votenum.ToString(),processInt.ToString());

                m_Layout.SetSelectResult(i, strprocess , process);

                m_Layout.SetSelectResultActive(i, true);

                if (votenum > maxVoteNum)
                {
                    maxVoteNumIndex = i;
                    maxVoteNum = votenum;
                }
            }

            hadSelectVoteIndex = maxVoteNumIndex;
            m_Layout.SetSelectToggleOn(maxVoteNumIndex, true, false);
        }

        private uint VoteCount(uint levelindex)
        {
            //当没有投票数量和队伍成员得数量不相等时，那么就将没有投票信息得成员票数加到第一个关卡上

            //var SelectData = Sys_GoddnessTrial.Instance.GetChapterSelect(m_Parma.LevelID);

           // var teammescount = Sys_Team.Instance.TeamMemsCount;

            var votecount = Sys_GoddnessTrial.Instance.StateVoteResult.VoteInfo.Count;

            if (votecount == 0)
            {
                return levelindex == 0 ? 1u : 0;
            }
      
            var data = Sys_GoddnessTrial.Instance.StateVoteResult.VoteInfo.Find(o => o.StageIndex == levelindex);


            //if (votecount < teammescount && levelindex == 0)
            //{
            //    return (uint)(teammescount - votecount + (data == null ? 0 : (int)data.VoteNum));
            //}

            if (data == null)
                return 0;

            return data.VoteNum;
        }

        private void SetSelectedVote()
        {
            if (hadSelectVoteIndex < 0)
                return;

            m_Layout.SetSelectToggleOn(hadSelectVoteIndex, true, false);
        }
    }

    public partial class UI_Goddness_Select : UIBase, UI_Goddness_Select_Layout.IListener
    {
        private int mWaitFollowOp = -1;
        public void OnClickSelect(uint index,uint ID)
        {
            if (m_IsVoteEnd)
            {
                SetSelectedVote();
                return;
            }

            if (mWaitFollowOp > 0 || m_IsFollowCaptonVote)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2022384));
                if (((int)index) != hadSelectVoteIndex)
                {
                    SetSelectedVote();
                }

                return;
            }

            if ( hadSelectVoteIndex >= 0 || hadSelectVote)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2022381));

                if(((int)index )!= hadSelectVoteIndex)
                {
                    SetSelectedVote();
                }
                return;
            }


            hadSelectVoteIndex = (int)index;

            Sys_GoddnessTrial.Instance.SendSelectStageVote(ID,index);
        }

        public void OnClickFollow(bool b)
        {
            if (m_IsVoteEnd)
            {
                m_Layout.SetFollow(!b);
                return;
            }

            if (hadSelectVoteIndex >= 0 || hadSelectVote)
            {
                m_Layout.SetFollow(!b);

                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2022385));

                return;
            }
            
            uint op = (uint)(b ? 1 : 0);

            Sys_GoddnessTrial.Instance.SendSelectStageFollowLeader(op);

            mWaitFollowOp = (int)op;

        }

        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickSelectClose()
        {
            if (m_IsVoteEnd == false)
                return;


            var nowtime = Sys_Time.Instance.GetServerTime();

            if (nowtime - m_VoteEndTimePoint < 1)
                return;

            CloseSelf();

            if (Sys_GoddnessTrial.Instance.IsWaitResult)
            {
                Sys_GoddnessTrial.Instance.OpenResultUI(Sys_GoddnessTrial.Instance.WaitResultInstanceID, Sys_GoddnessTrial.Instance.WaitResultIsPass);
                return;
            }
            if (Sys_Team.Instance.isCaptain() == false)
                return;

            if (Sys_GoddnessTrial.Instance.IsLastLevel(m_Parma.InstanceID, m_Parma.LevelID))

                return;

            Sys_GoddnessTrial.Instance.SendSwitchStage();
        }


    }

    public partial class UI_Goddness_Select : UIBase, UI_Goddness_Select_Layout.IListener
    {
        private bool hadSelectVote = false;

        private int hadSelectVoteIndex = -1;
        private void OnSelectStartVote(uint instanceid,uint levelid)
        {
            m_Parma.State = 1;
            m_Parma.InstanceID = instanceid;
            m_Parma.LevelID = levelid;

            SetState(m_Parma.State);

            RefreshSelect();
        }
        private void OnSelectStateVote()
        {
            hadSelectVote = true;
        }
        private void OnSelectStateVoteFollow()
        {
            bool isfollow = mWaitFollowOp == 1;

            m_IsFollowCaptonVote = isfollow;

            m_Layout.SetFollow(isfollow);

            mWaitFollowOp = -1;
        }

        private void OnStageVoteProcess(uint stageIndex)
        {
           

            
            m_Layout.SetSelectToggleOn(stageIndex, true);
        }

        private void OnStageVoteEnd()
        {
            RefreshSelectEnd();
        }

        private void OnEnterFight()
        {
            CloseSelf();
        }
    }
}
