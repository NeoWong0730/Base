using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic
{
    public class UI_Daily_LimitActivity : UIBase, UI_Daily_LimitActivity_Layout.IListerner
    {
        UI_Daily_LimitActivity_Layout m_Layout = new UI_Daily_LimitActivity_Layout();

        Timer m_Timer = null;
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetLisitener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Daily.Instance.eventEmitter.Handle(Sys_Daily.EEvents.NewNotice, OnNoticeMsg, toRegister);
            Sys_Daily.Instance.eventEmitter.Handle(Sys_Daily.EEvents.RemoveNotice, OnNoticeMsg, toRegister);
        }
        protected override void OnShow()
        {            
            Refresh();
        }

        protected override void OnHide()
        {
            if (m_Timer != null)
                m_Timer.Cancel();

            m_Timer = null;
        }
        private void DoUpdate(float value)
        {
            int count =  m_Layout.GetSize();

            var notTime = Sys_Daily.Instance.GetTodayTimeSceond();

            for (int i = 0; i < count; i++)
            {
                m_Layout.UpdateOpenTime(i, notTime);
            }
        }
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickItem(uint configid)
        {
            if (configid == 113u || configid == 115u)
            {
                Sys_Daily.Instance.GotoActivity(configid);
            }
            else
            {
                Sys_Daily.Instance.GotoDailyNpc(configid);
            }
            
            CloseSelf();
        }

        public void OnClickClear()
        {
            Sys_Daily.Instance.Apply_ClearLimite();

            CloseSelf();
        }
        private void Refresh(bool isAutoCloseSelf = false)
        {
           var data =  Sys_Daily.Instance.NoticeList;

            int count = data.Count;

            if (isAutoCloseSelf && count == 0)
            {
                CloseSelf();
                return;
            }

            var notTime = Sys_Daily.Instance.GetTodayTimeSceond();

            m_Layout.SetSize(count);

            int maxOffset = 0;

            for (int i = 0; i < count; i++)
            {
                var notice = Sys_Daily.Instance.GetDailyNotice(data[i]);

                var daily = CSVDailyActivity.Instance.GetConfData(notice.ID);

                m_Layout.SetName(i, daily.ActiveName);

                var timeoffset = notice.EndNoticeTime - (int)notTime;

                m_Layout.SetOpenTime(i, notice.EndNoticeTime);
                m_Layout.SetConfigID(i, notice.ID);


                m_Layout.SetActityType(i, notice.Type == 0 ? 11974u : 11975u);

                if (timeoffset > maxOffset)
                    maxOffset = timeoffset;
            }


            if (maxOffset > 0)
            {
                if (m_Timer != null)
                    m_Timer.Cancel();

                m_Timer = Timer.Register(maxOffset, OnTimeOver, DoUpdate);
            }
        }


        private void OnTimeOver()
        {

        }


        private void OnNoticeMsg()
        {
            Refresh(true);
        }
    }
}
