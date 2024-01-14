using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
namespace Logic
{
    public partial class UI_Daily_Calendar : UIBase,UI_Daily_Calendar_Layout.IListener
    {
        UI_Daily_Calendar_Layout m_Layout = new UI_Daily_Calendar_Layout();
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);

            getData();
        }
        
        protected override void OnShow()
        {            
            RefreshUI();

            m_Layout.SetTitleState(Sys_Daily.Instance.getDayOfWeek());
        }

        private void RefreshUI()
        {
            var count = mDatas.Count;

            m_Layout.SetSize(count);

            int nowDay = Sys_Daily.Instance.getDayOfWeek();

            for (int i = 0; i < count; i++)
            {
                var value = mDatas[i];

                int value0 = (int)value.mTime;
                int hours = value0 / 100;
                int mins = value0 % 100;

                string strHours = hours == 0 ? "00" : hours.ToString();
                string strMins = mins == 0 ? "00" : mins.ToString();

                m_Layout.SetItem(i, 0, string.Format("{0}:{1}", strHours, strMins), 0);

                var ncount = 7;

                for (int n = 1; n <= ncount; n++)
                {
                    var result = value.GetWeekDayActiveId((uint)(n));

                    var dailydata =CSVDailyActivity.Instance.GetConfData(result);

                    uint nameID = dailydata == null ?  0 : dailydata.ActiveName;


                    m_Layout.SetItem(i, n, nameID, (dailydata == null ? 0 :dailydata.id));

                    m_Layout.SetItemState(i, n, n == nowDay);
                }
            }
        }

        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickRandChild(uint id)
        {
            //if (id == 0)
            //    return;

            //UIManager.OpenUI(EUIID.UI_DailyActivites_Detail,false, id);
        }
    }

    public partial class UI_Daily_Calendar : UIBase, UI_Daily_Calendar_Layout.IListener
    {
        class WeekData
        {
            public uint mTime;

            private List<CSVDailyActivityWeek.Data>  mDataConfig = new List<CSVDailyActivityWeek.Data>();

            public WeekData(uint time) {

                mTime = time;
            }

            public void Add(CSVDailyActivityWeek.Data data)
            {
                mDataConfig.Add(data);
            }

            /// <summary>
            /// 获取一个时段，一个星期中某一天的活动
            /// </summary>
            /// <param name="day">星期几 1- 7</param>
            /// <returns></returns>
            public CSVDailyActivityWeek.Data GetWeekDay(uint day)
            {
                var datas = mDataConfig.Find(o => o.week.Contains(day));

                return datas;
            }

            public uint GetWeekDayActiveId(uint day)
            {
                var datas = GetWeekDay(day);

                if (datas == null)
                    return 0;

                int index = datas.week.FindIndex( o => o ==day);

                if (index < 0)
                    return 0;

               return datas.ActiveId[index];
               
            }
        }

        private List<WeekData> mDatas = new List<WeekData>();
        private void getData()
        {
            var data = CSVDailyActivityWeek.Instance.GetAll();

            mDatas.Clear();
            Dictionary<uint, int> dictemp = new Dictionary<uint, int>();

            foreach (var kvp in data)
            {
                if (dictemp.ContainsKey(kvp.time) == false)
                {
                    dictemp.Add(kvp.time, mDatas.Count);
                    mDatas.Add(new WeekData(kvp.time));

                }

                int index = dictemp[kvp.time];

                mDatas[index].Add(kvp);
            }

            mDatas.Sort((x, y) =>
            {
                if (x.mTime > y.mTime)
                    return 1;

                if (x.mTime < y.mTime)
                    return -1;

                return 0;
            });
        }
    }
}
