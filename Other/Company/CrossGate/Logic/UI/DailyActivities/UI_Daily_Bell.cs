using System;
using System.Collections.Generic;

using UnityEngine;
using Logic.Core;
using Table;

namespace Logic
{
    public partial class UI_Daily_Bell : UIBase, UI_Daily_Bell_Layout.IListener
    {
        private UI_Daily_Bell_Layout m_Layout = new UI_Daily_Bell_Layout();



        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);
            m_Layout.SetListener(this);

            getDate();
        }

        protected override void OnHide()
        {            
            Sys_Daily.Instance.SaveRedSet();
        }

        protected override void OnShow()
        {            
            Sys_Daily.Instance.LoadRedSet();
        }


        public void OnClickJoin(int id)
        {
            var data =  CSVDailyActivityPush.Instance.GetConfData((uint)id);

            if (data == null)
                return;

            bool result = Sys_Daily.Instance.JoinDaily(data.ActiveId);

            if (result == false)
                return;

            UIManager.CloseUI(EUIID.UI_DailyActivites);
        }

        public void OnMessageTogg(int id, bool state)
        {
           
        }

        public void OnRedTogg(int id, bool state)
        {
            Sys_Daily.Instance.SetRedset((uint)id, state);

          
        }

        public void OnRedState(bool state)
        {
            if (state)
                RefreshRed();
        }

        public void OnMessageState(bool state)
        {
            if (state)
                RefreshMessage();
        }

        public void OnClickClose()
        {
            CloseSelf();
        }
    }


    public partial class UI_Daily_Bell : UIBase, UI_Daily_Bell_Layout.IListener
    {
        private List<CSVDailyActivityPush.Data>  mRedList = new List<CSVDailyActivityPush.Data>();

        private List<CSVDailyActivityPush.Data>  mMessageList = new List<CSVDailyActivityPush.Data>();

        void getDate()
        {
            var data = CSVDailyActivityPush.Instance.GetAll();
            foreach (var kvp in data)
            {
                if (kvp.PrompMethod.Contains(1))
                    mRedList.Add(kvp);

                if (kvp.PrompMethod.Contains(2))
                    mMessageList.Add(kvp);
            }
        }
        void RefreshRed()
        {
           

            m_Layout.SetTipsText(2010256);
            int count = mRedList.Count;
            m_Layout.SetRedCount(count);

            int index = 0;

            foreach (var kvp in mRedList)
            {

                var dailydata = CSVDailyActivity.Instance.GetConfData(kvp.ActiveId);
                m_Layout.SetRedConfigID(index, kvp.id);

                m_Layout.SetRedName(index, dailydata.ActiveName);
                m_Layout.SetRedRound(index, kvp.CycleShow);

                var savedata = Sys_Daily.Instance.getRedSet(kvp.id);

                m_Layout.SetRedToggle(index ,(savedata == null ? false : savedata.State));

                index++;


            }
        }

        void RefreshMessage()
        {
            m_Layout.SetTipsText(2010213);

            int count = mMessageList.Count;
            m_Layout.SetMessageCount(count);

            int index = 0;

            foreach (var kvp in mMessageList)
            {

                var dailydata = CSVDailyActivity.Instance.GetConfData(kvp.ActiveId);

                m_Layout.SetMessageConfigID(index, kvp.id);

                m_Layout.SetMessageName(index, dailydata.ActiveName);
                m_Layout.SetMessageRound(index, kvp.CycleShow);
                m_Layout.SetMessagePeople(index, dailydata.WayDesc);

                string time = kvp.Cycle == 1 ? (LanguageHelper.GetTextContent(2010223)) : (string.Format("{0}:{1}",kvp.TimeShow/100,kvp.TimeShow%100));
                m_Layout.SetMessageTime(index, time);

                m_Layout.SetMessageToggle(index, false);

                index++;


            }
        }


    }
}
