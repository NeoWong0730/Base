using Framework;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Daily_Common
    {
        public class DailyItem : ButtonIntClickItem
        {
            private Image mImIcon;

            private Text mTexName;

            private Image mImRed;

            private Transform mMarkRecommend; // 推荐

            private Transform mMarkTimeLimit;//限时

            private Transform mMarkFestival;//节日

            private Transform mMarkHadFinish;//已完成

            private Transform mMarkHadOver;//已结束

            private Text mTimes;

            private Text mActivity;
            private Transform mTransActivity;

            private Transform mTransActivityMask;

            private Text mActivityDesc;

            protected Button mBtn;

            private Transform mSignLimit;
            private Text mSignLimitTime;
            private Text mSignLimitLevel;
            public uint ConfigID { get; set; } = 0;

            public List<Transform> mStateList = new List<Transform>();
            public List<Text> mStateTextList = new List<Text>();

            private Image m_IWay;

            private Text m_TexAmount;
            public enum EState
            {
                None = -1,
                Recommend = 0,
                TimeLimit = 1,
                Festival = 2,
               // HadFinish = 3,
                //LimitOver=4,
            }

            public enum EOpState
            {
                
                None = 0,
                Go,
                Finish,
                TimeOver,
                TimeLimite,
                WillOpen

            }
            public DailyItem()
            {
            }
            public DailyItem(Transform root)
            {
                Load(root);
            }
            public override void Load(Transform root)
            {
                base.Load(root);

                mImIcon = root.Find("Image_ICON").GetComponent<Image>();

                mTexName = root.Find("Text_Name").GetComponent<Text>();

                mActivityDesc = root.Find("Image_Des/Text").GetComponent<Text>();

                mImRed = root.Find("Image_RedTips").GetComponent<Image>();

                mMarkRecommend = root.Find("Image_Mark01");
                mStateList.Add(mMarkRecommend);
                mStateTextList.Add(mMarkRecommend.Find("Text").GetComponent<Text>());

                mMarkTimeLimit = root.Find("Image_Mark02");
                mStateList.Add(mMarkTimeLimit);
                mStateTextList.Add(mMarkTimeLimit.Find("Text").GetComponent<Text>());

                mMarkFestival = root.Find("Image_Mark03");
                mStateList.Add(mMarkFestival);
                mStateTextList.Add(mMarkFestival.Find("Text").GetComponent<Text>());

                mMarkHadFinish = root.Find("Text_Finish");
               // mStateList.Add(mMarkHadFinish);

                mMarkHadOver = root.Find("Text_Finish01");
               // mStateList.Add(mMarkHadOver);

                mTimes = root.Find("Message/Text_Time/Text").GetComponent<Text>();
                mActivity = root.Find("Message/Text_Activity/Text").GetComponent<Text>();

                mTransActivity = root.Find("Message/Text_Activity");

                mBtn = root.Find("Btn_01").GetComponent<Button>();

                SetState(EState.None);


                mSignLimit = root.Find("Image_Limit");
                mSignLimitTime = mSignLimit.Find("Text_Limit").GetComponent<Text>();
                mSignLimitLevel = mSignLimit.Find("Text_Level").GetComponent<Text>();

                m_IWay = root.Find("Image_Activity").GetComponent<Image>();

                m_TexAmount = root.Find("Text_Amount").GetComponent<Text>();

                mTransActivityMask = root.Find("Image_Dark");
            }


            public override ClickItem Clone()
            {
                return Clone<DailyItem>(this) as ClickItem;
            }

            public void SetItemName(string name)
            {
                if (mTransform == null)
                    return;

                mTransform.gameObject.name = name;
            }

            public void SetName(string name)
            {
                mTexName.text = name;
            }

            public void SetDesc(string desc)
            {
                mActivityDesc.text = desc;
            }

            public void SetRed(bool b)
            {
                mImRed.gameObject.SetActive(b);
            }

            public void SetTimes(string times)
            {
                mTimes.text = times;
            }

            public void SetActivity(uint cur,uint total ,CSVWordStyle.Data styledata)
            {
         
              //  SetMaskActive(total > 0 && cur >= total);

                mTransActivity.gameObject.SetActive(total > 0);

                string tex = cur.ToString() + "/" + total.ToString();

                TextHelper.SetText(mActivity, tex, styledata);
            }

            public void SetMaskActive(bool active)
            {
                if (mTransActivityMask == null)
                    return;

                mTransActivityMask.gameObject.SetActive(active);
            }
            public void SetIcon(Sprite sprite)
            {
                mImIcon.sprite = sprite;
            }

            public void SetIcon(uint sprite)
            {
                ImageHelper.SetIcon(mImIcon, sprite);
            }

            public void SetState(EState eState,uint langueid = 0)
            {
                int count =  mStateList.Count;

                int state = (int)eState;
                for (int i = 0; i < count; i++)
                {
                    mStateList[i].gameObject.SetActive(state == i);

                    if (state == i && langueid > 0)
                    {
                        mStateTextList[i].text = LanguageHelper.GetTextContent(langueid);
                    }
                }

                
            }

            public void SetOpState(EOpState eOpState)
            {
                mMarkHadFinish.gameObject.SetActive(eOpState == EOpState.Finish);

                mMarkHadOver.gameObject.SetActive(eOpState == EOpState.TimeOver);

                mBtn.gameObject.SetActive(eOpState == EOpState.Go);

                SetLimitActive(eOpState == EOpState.TimeLimite || eOpState == EOpState.WillOpen);
            }
            public void SetLimitLevel(string tex, CSVWordStyle.Data textStyle)
            {
                mSignLimitLevel.gameObject.SetActive(true);
                mSignLimitTime.gameObject.SetActive(false);

               // mSignLimitLevel.text = tex;

                TextHelper.SetText(mSignLimitLevel, tex);
            }

            public void SetLimitTime(string tex, CSVWordStyle.Data textStyle)
            {
                mSignLimitLevel.gameObject.SetActive(false);
                mSignLimitTime.gameObject.SetActive(true);

                //mSignLimitTime.text = tex;
                TextHelper.SetText(mSignLimitTime, tex);
            }

            public void SetLimitActive(bool b)
            {
                mSignLimit.gameObject.SetActive(b);
              
            }

            public void SetPlayerType(uint iconID)
            {
                if (iconID == 0)
                {
                  
                    return ;
                }
                ImageHelper.SetIcon(m_IWay, iconID);
            }


            public void SetAmount(string str)
            {
                m_TexAmount.text = str;
            }
        }


        /// <summary>
        /// 左上角标记 限定
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="data"></param>
        public static  void SetLimiteText(uint type, CSVDailyActivity.Data data ,Action<int,string, CSVWordStyle.Data> callFunc)
        {
            //CSVLanguage.Data dataLanguage = CSVLanguage.Instance.GetConfData(2007204);

            if (type == 2)
            {
                var state = Sys_Daily.Instance.LimitDailyState(data.id);

                if (state != Sys_Daily.ELimitDailyState.Opening)
                    callFunc(2, data.OpeningTimeStringForUI(), LanguageHelper.GetTextStyle(Constants.WORD_STYLE_22));

                return;
            }

            if (type == 3)
            {
                List<int> LevelCondition = new List<int>();
                DailyDataHelper.GetCondition(data.FunctionOpenid, 10202, ref LevelCondition);

                List<int> TaskCondition = new List<int>();
                DailyDataHelper.GetCondition(data.FunctionOpenid, 10091, ref TaskCondition);

                string strvalue = GetLimiteString(LevelCondition, TaskCondition);

                //LanguageHelper.GetTextContent(2010262, data.OpenLevel().ToString())
                callFunc(1, strvalue, LanguageHelper.GetTextStyle(Constants.WORD_STYLE_22));

                return;
            }

          //  callFunc(0,string.Empty, null);
        }

        private static string GetLimiteString(List<int> levels, List<int>tasks)
        {
            var levelCount = levels.Count;
            var taskCount = tasks.Count;

            if (levelCount == 0 && taskCount == 0)
                return string.Empty;

            if (levelCount > 0 && taskCount == 0)
                return LanguageHelper.GetTextContent(2010262, (levels[0]).ToString());

            var data = CSVTask.Instance.GetConfData((uint)tasks[0]);
            if (levelCount == 0 && taskCount > 0)
            {
                return LanguageHelper.GetTextContent(11832, CSVTaskLanguage.Instance.GetConfData(data.taskName).words);
            }

            return LanguageHelper.GetTextContent(11833, (levels[0]).ToString(), CSVTaskLanguage.Instance.GetConfData(data.taskName).words);
        }

        public static void setDailyStateMask(uint type, CSVDailyActivity.Data data,Action<UI_Daily_Common.DailyItem.EState,uint> callfunc)
        {
            UI_Daily_Common.DailyItem.EState eState = UI_Daily_Common.DailyItem.EState.None;

            uint curTimes = Sys_Daily.Instance.getDailyCurTimes(data.id);

            //var limitState = Sys_Daily.Instance.LimitDailyState(data.id);
            uint langueID = 0;

            if (type == 3)
                eState = UI_Daily_Common.DailyItem.EState.None;

            else if (Sys_Daily.Instance.IsRecommendDaily(data.id))
                eState = UI_Daily_Common.DailyItem.EState.Recommend;

            else if (type == 4)
                eState = UI_Daily_Common.DailyItem.EState.Festival;

            else if (data.Active_lan > 0)//limitState == Sys_Daily.ELimitDailyState.Opening
            {
                eState = UI_Daily_Common.DailyItem.EState.TimeLimit;
                langueID = data.Active_lan;
            }
              

            callfunc(eState, langueID);

        }

        public static UI_Daily_Common.DailyItem.EOpState GetDailyOpState(uint type, CSVDailyActivity.Data data)
        {
            UI_Daily_Common.DailyItem.EOpState eState = UI_Daily_Common.DailyItem.EOpState.Go;

            if (type == 3)
                return DailyItem.EOpState.WillOpen;

            if (type != 2)
                return eState;

            var dailystate = Sys_Daily.Instance.GetLimitDailyState(data.id);

            if (dailystate == Sys_Daily.ELimitDailyState.Over)
                eState = UI_Daily_Common.DailyItem.EOpState.Finish;

            else if (dailystate == Sys_Daily.ELimitDailyState.TodayStart)
                eState = UI_Daily_Common.DailyItem.EOpState.TimeLimite;

            return eState;
        }
    }


    public partial class UI_Daily_Common
    {
        //public class DataWithTime
        //{
        //    public DateTime OpenTime { get; set; }
        //    public CSVDailyActivity.Data Data { get; set; }
        //}
        //public List<DataWithTime> getWillStartLimitDaily()
        //{
        //    List<DataWithTime> mDataChace = new List<DataWithTime>();

        //    var list = Sys_Daily.Instance.getConfigATypeDaily(2);

        //    if (list == null)
        //        return mDataChace;
        //    var serverTime = Sys_Time.Instance.GetServerTime();

        //    DateTime time = new DateTime(serverTime);

        //    bool isDoubleWeek = (time.Day / 7) % 2 == 0;

        //    int hour = time.Hour;
        //    int mini = time.Minute;

        //    int offsetTime = 10;

        //    foreach (var item in list)
        //    {
        //        int count = item.OpeningTime.Count;

        //        bool isWeekOk = (item.OpeningMode1 == 2 && isDoubleWeek) || item.OpeningMode1 == 0 || (item.OpeningMode1 == 1 && !isDoubleWeek) ? true : false;

        //        bool isDayOk = time.Day == item.OpeningMode2;

        //        if (count >= 1)
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                var times = item.OpeningTime[i];

        //                if (times.Count <= 1)
        //                {
        //                    break;
        //                }

        //                DateTime willtime = DateTime.Today;

        //                willtime.AddHours(times[0]);
        //                willtime.AddMinutes(times[1]);

        //                var offset = willtime - time;

        //                if (offset.Minutes > 0 && offset.Minutes <= offsetTime)
        //                {
        //                    mDataChace.Add(new DataWithTime() { OpenTime = willtime, Data = item });
        //                }
        //            }
        //        }

        //    }

        //    return mDataChace;
        //}
    }
}
