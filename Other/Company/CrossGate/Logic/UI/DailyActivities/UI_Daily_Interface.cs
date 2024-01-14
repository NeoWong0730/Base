using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;

namespace Logic
{
    public class UI_Daily_Interface : UIComponent, UI_Daily_Interface_Layout.IListener
    {
        UI_Daily_Interface_Layout m_Layout = new UI_Daily_Interface_Layout();
        //private Animator animator01;
        private Timer time;


        private List<Sys_Daily.DailyWithLimiteTime> mLimiteDaily;

        private float mLastLimiteUpdateTime;


        protected override void Loaded()
        {
            base.Loaded();
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);

            m_Layout.m_Menu.gameObject.SetActive(Sys_Role.Instance.IsMenuActivityExpand);
            m_Layout.SetOpenIconRoll(Sys_Role.Instance.IsMenuActivityExpand);
        }

        public override void Show()
        {
            base.Show();
            //animator01 = m_Layout.m_Menu.GetComponent<Animator>();

            SetActivityRedState();
            //getLimiteDaily();
            SetDailyNewTipsState();

            CheckMallRed();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);

            Sys_Daily.Instance.eventEmitter.Handle(Sys_Daily.EEvents.ActiveValueChange, OnDailyActiveValueChange, toRegister);
            Sys_Daily.Instance.eventEmitter.Handle<uint>(Sys_Daily.EEvents.RewardAck, OnDailyRewardAck, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, OnBeginEnter, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, OnUIEndExit, toRegister);

            Sys_Daily.Instance.eventEmitter.Handle<uint>(Sys_Daily.EEvents.NewTipsChange, OnDailyNewTipsChange, toRegister);
        }

        private void OnBeginEnter(uint stackID, int nID)
        {
            if (nID == (int)EUIID.UI_Chat)
            {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                gameObject.SetActive(AspectRotioController.IsExpandState);
#else
                gameObject.SetActive(false);
#endif
            }
        }

        private void OnUIEndExit(uint stackID, int nID)
        {
            if (nID == (int)EUIID.UI_Chat)
            {
                gameObject.SetActive(true);
            }
        }



        public void OnClickMall()
        {
            UIManager.OpenUI(EUIID.UI_Mall, false, new MallPrama() { mallId = 101 });
        }

        public void OnClickActivity()
        {
            UIManager.OpenUI(EUIID.UI_DailyActivites);
        }


        public void OnClickActivityName()
        {
            if (mLimiteDaily.Count == 0)
                return;

            Sys_Daily.Instance.GotoDailyNpc(mLimiteDaily[0].DailyID);
        }

        public void OnClickLimitActivity()
        {
            UIManager.OpenUI(EUIID.UI_DailyActivites_Limite);
        }

        public void OnClickOpen()
        {
            bool isExpand = m_Layout.m_Menu.activeInHierarchy;
            m_Layout.m_Menu.SetActive(!isExpand);
            m_Layout.SetOpenIconRoll(!isExpand);
            Sys_Role.Instance.IsMenuActivityExpand = !isExpand;
            ////animator01.enabled = true;
            //if (m_Layout.m_Menu.activeInHierarchy)
            //{
            //    //animator01.Play("Close", -1, 0);
            //    time?.Cancel();
            //    time = Timer.Register(0.11f, () =>
            //    {
            //        m_Layout.m_Menu.SetActive(false);
            //        m_Layout.SetOpenIconRoll();
            //    }, null, false, false);

            //}
            //else
            //{
            //    m_Layout.m_Menu.SetActive(true);
            //    m_Layout.SetOpenIconRoll();
            //    animator01.Play("Open", -1, 0);
            //}
        }

        public void OnClickTrade()
        {
            UIManager.OpenUI(EUIID.UI_Trade);
        }

        private void OnDailyActiveValueChange()
        {
            SetActivityRedState();
        }

        private void OnDailyRewardAck(uint id)
        {
            SetActivityRedState();
        }

        private void SetActivityRedState()
        {
            SetDailyNewTipsState();
        }

        private void OnDailyNewTipsChange(uint id)
        {
            SetDailyNewTipsState();
        }

        private void SetDailyNewTipsState()
        {

            m_Layout.SetDailyRed(Sys_Daily.Instance.HaveReward() || Sys_Daily.Instance.HaveDailyRewardAll() || Sys_Daily.Instance.HaveNotice());

            SetDailyRewardState();
        }

        private void CheckMallRed()
        {
            m_Layout.transMallRed.gameObject.SetActive(Sys_Mall.Instance.IsMallRed(101));
        }

        private void SetDailyRewardState()
        {
            var haveTips = Sys_Daily.Instance.HaveNewTips();

            m_Layout.SetActivityNewActive(haveTips);
        }

        public void OnClickHangup()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(20301, true)) return;

            UIManager.OpenUI(EUIID.UI_HangupFight);
        }
    }
}
