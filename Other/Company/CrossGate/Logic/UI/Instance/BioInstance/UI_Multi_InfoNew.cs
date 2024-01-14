using Framework;
using Logic.Core;
using Packet;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class UI_Multi_InfoNew_Parmas
    {
        //关卡ID
        public uint LevelID = 0;

        public uint InstanceID = 0;

        public uint SeriesID = 0;

    }
    public partial class UI_Multi_InfoNew : UIBase
    {
        private UI_Multi_InfoNew_Layout m_Layout = new UI_Multi_InfoNew_Layout();

        private UI_Multi_InfoNew_Parmas m_Parmas = new UI_Multi_InfoNew_Parmas();

        private List<ItemIdCount> m_FristCrossReward = null;

        private List<ItemIdCount> m_CanGetReward = null;

        private List<CSVInstanceDaily.Data> m_LevelList = null;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.setListener(this);
        }

        protected override void ProcessEventsForEnable(bool enable)
        {
            Sys_Instance_Bio.Instance.eventEmitter.Handle(Sys_Instance_Bio.EEvents.FristPass, OnFristPass, enable);

            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.LeaderSkipBtnUpdateNtf, RefreshSkipTalk, enable);
        }
        protected override void OnOpen(object arg)
        {
            UI_Multi_InfoNew_Parmas parmas = arg as UI_Multi_InfoNew_Parmas;

            if (arg != null)
            {
                m_Parmas = parmas;

                Sys_Instance_Bio.Instance.SendFristPass(m_Parmas.InstanceID);
            }

        }

        protected override void OnShow()
        {
            m_LevelList = InstanceHelper.getDailyByInstanceID(m_Parmas.InstanceID);

            Refresh();


        }
        private void Refresh()
        {
            var InstanceData = CSVInstance.Instance.GetConfData(m_Parmas.InstanceID);
            var seriesData = CSVNewBiographySeries.Instance.GetConfData(m_Parmas.SeriesID);

            m_Layout.SetInstanceSeriesName(seriesData.Name);

            m_Layout.SetInstanceName(InstanceData.Name);

            m_Layout.SetInstanceDescribe(InstanceData.PreviousDes);

            m_Layout.SetInstanceImage(InstanceData.bg);

            m_FristCrossReward = CSVDrop.Instance.GetDropItem(InstanceData.FirstPassReward);
            m_Layout.SetFristReward(m_FristCrossReward);



            m_CanGetReward = CSVDrop.Instance.GetDropItem(InstanceData.FirstReward);
            m_Layout.SetCanGetReward(m_CanGetReward);

            //Packet.InsEntry insEntry = null;

            uint cur = Sys_Instance_Bio.Instance.insData.StageRewardLimit.UsedTimes;
            var dailydata = CSVDailyActivity.Instance.GetConfData(130); ;

            uint total = dailydata.Times;

            //if (Sys_Instance_Bio.Instance.serverInstanceData != null && Sys_Instance_Bio.Instance.serverInstanceData.instanceCommonData != null)         
            //   Sys_Instance_Bio.Instance.serverInstanceData.instanceCommonData.Entries.Find(o => o.InstanceId == m_Parmas.InstanceID);

            //int cur = insEntry == null ? 0 : m_LevelList.FindIndex(o => o.id == insEntry.PerMaxStageId) + 1;

            m_Layout.SetCrossTimes(2022921, cur, total);

            OnFristPass();

            RefreshSkipTalk();
        }

        private void OnFristPass()
        {
            int count = Sys_Instance_Bio.Instance.FristPassInfo == null ? 0 : Sys_Instance_Bio.Instance.FristPassInfo.Players.Count;

            if (Sys_Instance_Bio.Instance.FristPassInfo != null && Sys_Instance_Bio.Instance.FristPassInfo.InstanceId != m_Parmas.InstanceID)
                count = 0;

            m_Layout.SetFristPlayerCount(count);

            for (int i = 0; i < count; i++)
            {
                var playerinfo = Sys_Instance_Bio.Instance.FristPassInfo.Players[i];
                m_Layout.SetFristPlayerName(i, playerinfo.RoleName.ToStringUtf8());
            }

            string dateString = string.Empty;

            if (Sys_Instance_Bio.Instance.FristPassInfo != null && Sys_Instance_Bio.Instance.FristPassInfo.PassTime > 0)
            {
                var fristdate = TimeManager.GetDateTime(Sys_Instance_Bio.Instance.FristPassInfo.PassTime);

                dateString = fristdate.Year.ToString() + "." + fristdate.Month.ToString() + "." + fristdate.Day.ToString();
            }


            m_Layout.SetFristCrossDate(dateString);
        }

        private TaskSkipTalkBtn GetSkipTalkSet()
        {
            var data = Sys_Team.Instance.TeamInfo.TeamInfo.SkipTalkBtnList.Find(o =>
            {


                uint id = Sys_Instance_Bio.Instance.ActivityID * 100000 + m_Parmas.InstanceID;

                if (id == o.Key)
                    return true;

                return false;
            });

            return data;
        }
        private void RefreshSkipTalk()
        {
            var instanceid = Sys_Instance.Instance.GetServerInstanceData(Sys_Instance_Bio.Instance.ActivityID);

            if (instanceid == null || instanceid.GetInsEntry(m_Parmas.InstanceID).PassRecord == false)
            {
                m_Layout.SetSkipTalk(false, false);
                return;
            }

            if (Sys_Team.Instance.HaveTeam == false || Sys_Team.Instance.isPlayerLeave())
            {
               var isskip = Sys_Task.Instance.GetSkipState(GetKey());

                m_Layout.SetSkipTalk(true, isskip);
                return;
            }

            bool isfrist = false;

            var data = GetSkipTalkSet();

            if (data == null)
                isfrist = true;

            if (isfrist && Sys_Team.Instance.isCaptain())
            {
                Sys_Team.Instance.OpenTips(0, LanguageHelper.GetTextContent(2022949), () =>
                {

                    Sys_Team.Instance.SendTalkToggleChange(GetKey(), 0);
                }, () =>
                {
                    Sys_Team.Instance.SendTalkToggleChange(GetKey(), 1);
                }, null, true, 2022951, 2022950);
            }


            m_Layout.SetSkipTalk(true, data == null ? false : data.Skip);
        }
    }
    public partial class UI_Multi_InfoNew : UIBase, UI_Multi_InfoNew_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickCrossDetail()
        {
            m_Layout.SetRuleActive(true);
        }

        public void OnClickRuleClose()
        {
            m_Layout.SetRuleActive(false);
        }
        public void OnClickFastTeam()
        {
            var data = CSVInstance.Instance.GetConfData(m_Parmas.InstanceID);

            Sys_Team.Instance.OpenFastUI(data.TeamID, true);
        }

        public void OnClickGoOn()
        {
            bool result = Sys_Instance.Instance.CheckTeamCondition(m_Parmas.InstanceID);

            if (result == false)
                return;

            Sys_Instance_Bio.Instance.OnSendQueryEnterInfo(m_Parmas.InstanceID, m_Parmas.LevelID);

            CloseSelf();

            var LevelList = Sys_Instance.Instance.getDailyByInstanceID(m_Parmas.InstanceID);

            uint levelid = LevelList.Count == 0 ? 0 : LevelList[0].id;

            UIManager.OpenUI(EUIID.UI_Bio_Vote, false, new UI_Multi_ReadyNew_Parmas() { isCapter = true, InstanceID = m_Parmas.InstanceID, LevelID = levelid, State = 1 });
        }

        public void OnClickRewardView()
        {
            UIManager.OpenUI(EUIID.UI_Bio_LevelReward, false, new UI_Multi_LevelReward_Parmas() { InstanceID = m_Parmas.InstanceID });
        }

        public void OnFristRewardInfinityGridChange(InfinityGridCell infinityGridCell, int index)
        {
            m_Layout.SetFristReward(infinityGridCell, m_FristCrossReward[index]);
        }

        public void OnRewardInfinityGridChange(InfinityGridCell infinityGridCell, int index)
        {
            m_Layout.SetCanGetReward(infinityGridCell, m_CanGetReward[index]);
        }

        public void OnClickTogSkipTalk(bool state)
        {
            bool b = state;

            if (Sys_Team.Instance.HaveTeam && Sys_Team.Instance.isPlayerLeave()==false)
            {
                var data = GetSkipTalkSet();
                b = data == null ? false : data.Skip;
            }
            else
            {
                b = Sys_Task.Instance.GetSkipState(GetKey());
            }


            if (state != b)
            {
                Sys_Team.Instance.SendTalkToggleChange(GetKey(), state ? 1u : 0);
            }
        }

        private uint GetKey()
        {
            return 130u * 100000 + m_Parmas.InstanceID;
        }
    }
}
