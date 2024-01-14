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
using System;

namespace Logic
{

    public class UI_TeamMain_Layout 
    {
        //public UI_TeamMain_Mems_Layout m_MemsLayout = new UI_TeamMain_Mems_Layout();
        //public UI_TeamMain_AutoFind_Layout m_AtuoFindLayout = new UI_TeamMain_AutoFind_Layout();
        //public UI_TeamMain_WithoutTeam_Layout m_WithoutTeamLayout = new UI_TeamMain_WithoutTeam_Layout();
        //public UI_TeamMain_MenuTeamMem_Layout m_MenuTeamMemLayout = new UI_TeamMain_MenuTeamMem_Layout();
       // public UI_TeamMain_Role_Layout m_RoleLayout = new UI_TeamMain_Role_Layout();
       // public UI_TeamMain_RoleInfo_Layout m_RoleInfoLayout = new UI_TeamMain_RoleInfo_Layout();

        public void Loaded(Transform root)
        {
            //m_MemsLayout.Load(root.Find("TeamScroll"));
           // m_AtuoFindLayout.Load(root.Find("TeamShow/Team_Match"));
           // m_WithoutTeamLayout.Load(root.Find("TeamShow/Team_Create"));
           // m_MenuTeamMemLayout.Load(root.Find("TeamShow/Team_Button"));
           // m_RoleLayout.Load(root.Find("TeamShow/Team_Teammate"));
            //m_RoleInfoLayout.Load(root.Find("TeamShow/Team_Player"));

        }

        public void RegisterEvents(IListener listener,bool state)
        {
            //m_MemsLayout.RegisterEvents(listener, state);
            //m_AtuoFindLayout.RegisterEvents(listener, state);
            //m_WithoutTeamLayout.RegisterEvents(listener, state);
            //m_MenuTeamMemLayout.RegisterEvents(listener, state);
           // m_RoleLayout.RegisterEvents(listener, state);
           // m_RoleInfoLayout.RegisterEvents(listener, state);
        }

        public interface IListener
        {
            void Join();
            void Create();

            void ExitAutoFind();

            void OnClickMemItem(Vector3[] points,ulong roleID,Action hideAc);
            
        }
    }
}
