using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Framework;

namespace Logic
{
    public partial class UI_Goddness_Difficult : UIBase, UI_Goddness_Difficult_Layout.IListener
    {
        UI_Goddness_Difficult_Layout m_Layout = new UI_Goddness_Difficult_Layout();

        readonly uint[] DifficultIcon = { 974901, 974902, 974903, 974904 };
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.FristCrossInfo, OnFristCross, toRegister);

            
        }
        protected override void OnShow()
        {            
            Refresh();
        }

        private void Refresh()
        {
            int count = Sys_GoddnessTrial.Instance.m_DifficlyList.Count;

            m_Layout.SetDifficultCount(count);

            uint maxDifficult = Sys_GoddnessTrial.Instance.GetMaxDifficult();

            List<string> names = new List<string>();

            for (int i = 0; i < count; i++)
            {
                uint diffValue = Sys_GoddnessTrial.Instance.m_DifficlyList[i];

                m_Layout.SetDifficultIcon(i, DifficultIcon[diffValue - 1]);

                m_Layout.SetSelect(i,diffValue == Sys_GoddnessTrial.Instance.SelectDifficlyID);

                m_Layout.SetLock(i, diffValue > maxDifficult);

                bool bbtn = (diffValue != Sys_GoddnessTrial.Instance.SelectDifficlyID) && diffValue <= maxDifficult;

                m_Layout.SetSelectBtnActive(i, bbtn);


                var teaminfo = Sys_GoddnessTrial.Instance.GetTeamInfoByDifficult(diffValue);

                names.Clear();

                string strtime = string.Empty;

                bool showreward = true;

                if (teaminfo != null)
                {
                    DateTime time = TimeManager.GetDateTime(teaminfo.Time);

                    strtime = GetDateTimeString(time);

                    int rolescount = teaminfo.RoleList.Count;

                    showreward = false;
                    for (int n = 0; n < rolescount; n++)
                    {
                        names.Add(teaminfo.RoleList[n].Name.ToStringUtf8());
                        if (teaminfo.RoleList[n].RoleId == Sys_Role.Instance.RoleId)
                        {
                            showreward = true;
                        }
;
                    }       
                }

                m_Layout.SetFristTime(i, strtime);
                m_Layout.SetFristName(i, names);

                m_Layout.SetDifficultID(i, diffValue);
                m_Layout.SetDifficultFristReward(i, showreward);
                m_Layout.SetDifficultFristRewardIcon(i, GetFristRewardIcon(diffValue));
            }
            names.Clear();
        }

        private void RefreshTeamInfo()
        {

            List<string> names = new List<string>();

            int count = Sys_GoddnessTrial.Instance.m_DifficlyList.Count;
            for (int i = 0; i < count; i++)
            {
                uint diffValue = Sys_GoddnessTrial.Instance.m_DifficlyList[i];

                var teaminfo = Sys_GoddnessTrial.Instance.GetTeamInfoByDifficult(diffValue);

                names.Clear();

                string strtime = string.Empty;

                bool showreward = true;

                if (teaminfo != null)
                {
                    DateTime time = TimeManager.GetDateTime(teaminfo.Time);
                    strtime = GetDateTimeString(time);
                    int rolescount = teaminfo.RoleList.Count;

                    showreward = false;

                    for (int n = 0; n < rolescount; n++)
                    {
                        names.Add(teaminfo.RoleList[n].Name.ToStringUtf8());

                        if (teaminfo.RoleList[n].RoleId == Sys_Role.Instance.RoleId)
                        {
                            showreward = true;
                        }
                    }
                }

                m_Layout.SetFristTime(i, strtime);
                m_Layout.SetFristName(i, names);
                m_Layout.SetDifficultID(i, diffValue);
                m_Layout.SetDifficultFristReward(i, showreward);
                m_Layout.SetDifficultFristRewardIcon(i,GetFristRewardIcon(diffValue));
            }

            names.Clear();
        }

        private uint GetFristRewardIcon(uint diffvalue)
        {
            int result = GetFristRewardState(diffvalue);

            if(result > 0)
                return 994802u;

            if (result == 0)
                return 994803u;

            return 994801u;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diffvalue"></param>
        /// <returns> 小于 0 没有奖励 大于 0 有奖励可领取  等于0 已领取奖励</returns>
        private int GetFristRewardState(uint diffvalue)
        {
            var teaminfo = Sys_GoddnessTrial.Instance.GetTeamInfoByDifficult(diffvalue);

            if (teaminfo == null)
                return -1;

            var recorddata = teaminfo.RoleList.Find(o => o.RoleId == Sys_Role.Instance.RoleId);

            if (recorddata == null)
                return -1;

            var result = teaminfo.RolesGetedAward.Find(o => o == Sys_Role.Instance.RoleId);

            if (result > 0)
                return 0;

            return 1;
        }


        private string GetDateTimeString(DateTime time)
        {
            string strtime = string.Empty;
            string stryear = time.Year.ToString();
            string strmonth = time.Month.ToString();
            string strday = time.Day.ToString();
            string strhour = time.Hour.ToString();
            string strminute = time.Minute < 10 ? ("0" + time.Minute.ToString()) : time.Minute.ToString();

            strtime = string.Format("{0}/{1}/{2}  {3}:{4}", stryear, strmonth, strday, strhour, strminute);

            return strtime;
        }
    }


    public partial class UI_Goddness_Difficult : UIBase, UI_Goddness_Difficult_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickDifficult(int index, uint id)
        {
            Sys_GoddnessTrial.Instance.SetDifficly(id);

            Refresh();
            //Sys_GoddnessTrial.Instance.SendSetDifficulty(Sys_GoddnessTrial.Instance.SelectID);
        }

        public void OnClickDifficultFristReward(int index, uint id)
        {
            var diffvalue = id;

            var result = GetFristRewardState(diffvalue);

            var data = Sys_GoddnessTrial.Instance.GetGoddessTopicData(id);

            if (result > 0)
            {
                Sys_GoddnessTrial.Instance.SendGetFristCrossReward(data.id);
                return;
            }

        
            UI_Goddness_FristReward_Parma parma = new UI_Goddness_FristReward_Parma() { ID = (data != null ? data.id : 0) };
            UIManager.OpenUI(EUIID.UI_Goddness_FristAward, false, parma);

        }
        public void OnClickRank()
        {
            UIManager.OpenUI(EUIID.UI_Goddess_Rank);
        }
    }

    public partial class UI_Goddness_Difficult : UIBase, UI_Goddness_Difficult_Layout.IListener
    {
        private void OnFristCross()
        {
            RefreshTeamInfo();
        }
    }
}
