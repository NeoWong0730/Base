using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using Framework;
using Packet;

namespace Logic
{
    public class UI_TeamMain_Mems : UIComponent, UI_TeamMain_Mems_Layout.IListener
    {
        UI_TeamMain_Mems_Layout m_Layout = new UI_TeamMain_Mems_Layout();

        UI_TeamMain_Layout.IListener m_listener;

        private int mselectIndex = 0;        
        protected override void Loaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.ClickItemAction = OnClickItem;

            m_Layout.SetListener(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void Show()
        {
            m_Layout.Show();

            m_Layout.SetItemSize(Sys_Team.Instance.TeamMemsCount);

            for (int i = 0; i < Sys_Team.Instance.TeamMemsCount; i++)
            {
                setItemInfo(i);
            }

            m_Layout.SetAddMemberActive(Sys_Team.Instance.TeamMemsCount < Sys_Team.Instance.TeamMemberCountMax);

        }
        public override void Hide()
        {
            m_Layout.Hide();

        }

        private void OnClickItem(int index)
        {
 
            Vector3[] vectors = new Vector3[4];

            m_Layout.GetItemCorners(index, vectors);

            TeamMem teamMem = Sys_Team.Instance.getTeamMem(index);

            mselectIndex = index;

            m_Layout.SetItemSelected(index, true);

            m_listener.OnClickMemItem(vectors, teamMem.MemId, OnMemItemMenuHide);
        }

        private void OnMemItemMenuHide()
        {
            m_Layout.SetItemSelected(mselectIndex, false);
        }
        public void RegisterEvents(UI_TeamMain_Layout.IListener listener, bool state)
        {
            m_Layout.RegisterEvents(listener, state);

            m_listener = listener;
        }


        private void setItemInfo(int teamIndex)
        {
            TeamMem teamMem = Sys_Team.Instance.getTeamMem(teamIndex);

            if (teamMem != null)
            {
                UI_TeamMain_Mems_Layout.Item item = m_Layout.GetItem(teamIndex);

                item.Name = teamMem.Name.ToStringUtf8();
                item.isSelect = false;
                item.Level = (int)teamMem.Level;
                item.isLeave = teamMem.IsLeave();
                item.isOffline = teamMem.IsOffLine();
                item.HeadIcon = CharacterHelper.getHeadID(teamMem.HeroId, teamMem.Photo);
                item.HeadIconFrame = CharacterHelper.getHeadFrameID(teamMem.PhotoFrame);

                item.OccupationIcon = OccupationHelper.GetLogonIconID(teamMem.Career);

                item.SetHP(teamMem.HpPer / 100f);
                item.SetMagic(teamMem.MpPer / 100f);

                item.Index = teamIndex;
            }
        }
        public void UpdateInfo(int index)
        {
            setItemInfo(index);
        }

        void UI_TeamMain_Mems_Layout.IListener.OnClickItem(int index)
        {
            
        }

        public void OnClickAddMember()
        {
            UIManager.HitButton(EUIID.UI_MainInterface, "Team - AddMember");

            UIManager.OpenUI(EUIID.UI_Team_Invite);
        }
    }
}
