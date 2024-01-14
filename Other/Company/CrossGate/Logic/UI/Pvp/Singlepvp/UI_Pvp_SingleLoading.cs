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
    public partial class UI_Pvp_SingleLoading : UIBase, UI_Pvp_SingleLoading_Layout.IListener
    {
        UI_Pvp_SingleLoading_Layout m_Layout = new UI_Pvp_SingleLoading_Layout();

        
       
        private float m_OpenTime;

        private int m_OtherProcess = 0;
        private int m_OwnProcess = 0;

        private float m_OtherProcessTimePoint = 0;
        private float m_OwnProcessTimePoint = 0;

        private int m_OtherProcessMax = 99;
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Pvp.Instance.eventEmitter.Handle(Sys_Pvp.EEvents.LoadCompl, OnLoadCompl, toRegister);
            Sys_Pvp.Instance.eventEmitter.Handle<ulong>(Sys_Pvp.EEvents.OneReadyOk, OnReadOk, toRegister);
        }
        protected override void OnShow()
        {            
            m_OpenTime = Time.time;


            m_OtherProcessTimePoint = m_OpenTime;
            m_OwnProcessTimePoint = m_OpenTime;

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

            m_Layout.SetOwnLoadingPercent(m_OwnProcess);

            m_OwnProcessTimePoint = Time.time;
        }
        private void UpdateOtherProcess(float value)
        {
            if (m_OtherProcess >= 100)
                return;

            float diss = value - m_OtherProcessTimePoint;

            m_OtherProcess += (int)((1f * diss / 3f) * 100);

            m_OtherProcess = Mathf.Min(m_OtherProcess, m_OtherProcessMax);

            m_Layout.SetOtherLoadingPercent(m_OtherProcess);

            m_OtherProcessTimePoint = Time.time;
        }

        private void SetOwnProcessReady()
        {
            m_OwnProcessTimePoint = Time.time;
            m_OwnProcess = 100;

            m_Layout.SetOwnLoadingPercent(m_OwnProcess);
        }

        private void SetOtherProcessReady()
        {
            m_OtherProcessTimePoint = Time.time;
            m_OtherProcess = 100;
            m_Layout.SetOtherLoadingPercent(m_OwnProcess);
        }
        private void Refresh()
        {
            RefreshOwn();

            RefreshOther();
        }

        private void RefreshOwn()
        {
            m_Layout.SetOwnName(Sys_Role.Instance.sRoleName);
            m_Layout.SetOwnLevel((int)Sys_Role.Instance.Role.Level);
            m_Layout.SetOwnServerName(Sys_Login.Instance.mSelectedServer.mServerInfo.ServerName);
            m_Layout.SetOwnRoleIcon(Sys_Role.Instance.RoleId);
            m_Layout.SetOwnDanTex(LanguageHelper.GetTextContent(Sys_Pvp.Instance.LevelString));
            m_Layout.SetOwnMemberLevelIcon(Sys_Pvp.Instance.LevelIconID);
            var CurFmID = Sys_Partner.Instance.GetCurFmList();

            var Value = Sys_Partner.Instance.GetFormationByIndex((int)CurFmID);

            IList<uint> list = Value.Pa;

            int count = list.Count;
            int realCount = 0;
            for (int i = 0; i < count; i++)
            {
                if (list[i] > 0)
                {
                    realCount += 1;

                    var partnerData = CSVPartner.Instance.GetConfData(list[i]);


                    m_Layout.SetOwnMember(i, partnerData.battle_headID, OccupationHelper.GetIconID(partnerData.profession));
                }
            }

            m_Layout.SetOwnMemberSize(realCount);
        }

        private void RefreshOther()
        {
            Sys_Pvp.PvpItem item = null;

            if (Sys_Pvp.Instance.PvpObjects == null)
                return;

            foreach (var kvp in Sys_Pvp.Instance.PvpObjects)
            {
                if (kvp.Value.IsPlayerSelf == false)
                {
                    item = kvp.Value;
                    break;
                }
            }

            bool isRobot =  (item.roleID >> 63) > 0;

            m_OtherProcessMax = 100;//item.Info.IsRobot ? 100 : 99;

            m_Layout.SetOtherName(item.Info.Base.Name.ToStringUtf8());
            m_Layout.SetOtherLevel((int)item.Info.Base.Level);

            var serverInfo = Sys_Login.Instance.FindServerInfoByID(item.Info.GamesvrId);

            m_Layout.SetOtherServerName(serverInfo == null ? string.Empty : serverInfo.ServerName);

            m_Layout.SetOtherRoleIcon(item.Info.Base.FashionId,item.Info.Base.HeroId);

           

            CSVArenaSegmentInformation.Data itemdanData = CSVArenaSegmentInformation.Instance.GetConfData((uint)(item.Info.Arena.Dan*10 + item.Info.Arena.Level));
            string danstr = itemdanData == null ? string.Empty : LanguageHelper.GetTextContent(itemdanData.RankDisplay);

            m_Layout.SetOtherDanTex(danstr);

            m_Layout.SetOtherMemberLevelIcon(itemdanData.RankIcon1);

            int realCount = 0;

            if (item.Info.Partner != null)
            {
                var Value = item.Info.Partner.Ptners;
                int count = Value.Count;

                for (int i = 0; i < count; i++)
                {
                    if (Value[i] > 0)
                    {
                        realCount += 1;

                        var partnerData = CSVPartner.Instance.GetConfData(Value[i]);

                        m_Layout.SetOtherMember(i, partnerData.battle_headID, OccupationHelper.GetIconID(partnerData.profession));
                    }
                }
            }


            m_Layout.SetOtherMemberSize(realCount);

        }

    }

    /// <summary>
    /// 监听处理
    /// </summary>
    public partial class UI_Pvp_SingleLoading : UIBase, UI_Pvp_SingleLoading_Layout.IListener
    {
        private void OnLoadCompl()
        {
            CloseSelf();
        }

        private void OnReadOk(ulong id)
        {
            Sys_Pvp.PvpItem pvpItem;

            if (Sys_Pvp.Instance.PvpObjects.TryGetValue(id,out pvpItem) == false)
                return;

            if (pvpItem.IsPlayerSelf)
                SetOwnProcessReady();
            else
                SetOtherProcessReady();

        }
    }
}
