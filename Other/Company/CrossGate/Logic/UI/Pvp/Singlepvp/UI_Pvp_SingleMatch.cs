using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    /// 匹配
    /// </summary>
    public partial class UI_Pvp_SingleMatch : UIBase, UI_Pvp_SingleMatch_Layout.IListener
    {
        UI_Pvp_SingleMatch_Layout m_Layout = new UI_Pvp_SingleMatch_Layout();

        UI_Pvp_MatcheNPCTip m_NpcTip = new UI_Pvp_MatcheNPCTip();
        float m_StartTime;
        float m_LastTime;

        float m_StartNpcSce = 0;
        float m_NextNpcSce = 0;
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);

            m_NpcTip.Init(gameObject.transform.Find("Animator/NPC"));


           var startData = CSVParam.Instance.GetConfData(1040);
           var nextData = CSVParam.Instance.GetConfData(1041);

            m_StartNpcSce = float.Parse(startData.str_value);
            m_NextNpcSce = float.Parse(nextData.str_value);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Pvp.Instance.eventEmitter.Handle(Sys_Pvp.EEvents.CancleMatch, OnPvpCancleMatch, toRegister);

            Sys_Pvp.Instance.eventEmitter.Handle(Sys_Pvp.EEvents.MatchSuccess, OnPvpMatchSuccess, toRegister);

            Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStart, OnReconnectStart, toRegister);

           

        }
        protected override void OnShow()
        {            
            m_StartTime = Time.time;
            m_LastTime = Time.time;

            m_Layout.SetMatchTimeTex(GetTimeMinAndSecString(0) + ":" + GetTimeMinAndSecString(0));

            Refresh();
        }
        protected override void OnHide()
        {            
            m_NpcTip.Hide();

            m_state = -1;
        }


        protected override void OnUpdate()
        {
            float time = Time.time;

            float value = time - m_LastTime;

            if (value > 0.9)
            {
                float totalTime = time - m_StartTime;

                m_LastTime = time;

                int min = (int)(totalTime / 60);
                int sec = (int)(totalTime - min * 60);

              
                m_Layout.SetMatchTimeTex(GetTimeMinAndSecString(min) + ":" + GetTimeMinAndSecString(sec));

                UpdateTips(totalTime);
            }


           
        }

        private void OnReconnectStart()
        {
            CloseSelf();
        }
        private string GetTimeMinAndSecString(int time)
        {
            string minstr = time == 0 ? ("00") : (time < 10 ? ("0" + time) : time.ToString());

            return minstr;
        }
        private void Refresh()
        {
            m_Layout.SetOwnOcc(Sys_Role.Instance.Role.Career);
            m_Layout.SetOwnName(Sys_Role.Instance.sRoleName);
            m_Layout.SetOwnLevel((int)Sys_Role.Instance.Role.Level);
            m_Layout.SetOwnServerName(Sys_Login.Instance.mSelectedServer.mServerInfo.ServerName);
            m_Layout.SetOwnRoleIcon(Sys_Role.Instance.RoleId);

        }
 
    }

    /// <summary>
    /// 监听处理
    /// </summary>
    public partial class UI_Pvp_SingleMatch : UIBase, UI_Pvp_SingleMatch_Layout.IListener
    {
        public void OnClickMatch()
        {
            Sys_Pvp.Instance.Apply_CancleMathch();
        }

        private void OnPvpCancleMatch()
        {
            CloseSelf();

            UIManager.OpenUI(EUIID.UI_Pvp_Single);
        }

        private void OnPvpMatchSuccess()
        {
            CloseSelf();

           
        }
    }

    public partial class UI_Pvp_SingleMatch : UIBase, UI_Pvp_SingleMatch_Layout.IListener
    {
     
        private int m_state = -1;
        private float m_nextRefreshTime = 0;
        private Timer m_ShowTipsTimer;
        void UpdateTips(float time)
        {
            if (time  >= m_nextRefreshTime)
            {
                RefreshNpcTips(time);
            }
        }

        private void RefreshNpcTips(float nowtime)
        {
            m_state += 1;

            if (m_state > 2)
                m_state = 1;

            RefreshNpcTipsRefreshTime(nowtime);

            if (m_state == 1)
                m_NpcTip.Show();

            if (m_state == 2)
                m_NpcTip.Hide();
        }

        private void  RefreshNpcTipsRefreshTime(float nowtime)
        {
            if (m_state == 0)
            {
                m_nextRefreshTime = nowtime + m_StartNpcSce;
            }

            else if (m_state == 1)
            {
                m_nextRefreshTime = nowtime + 12f;
            }

            else if (m_state == 2)
            {
                m_nextRefreshTime = nowtime + m_NextNpcSce;
            }
        }
        private void ShowTips()
        {
            m_NpcTip.Show();
        }
    }
}
