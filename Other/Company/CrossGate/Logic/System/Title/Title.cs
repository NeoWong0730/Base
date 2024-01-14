using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;

namespace Logic
{

    public enum TitleType
    {
        Prestige = 1,       //声望
        Task,           //任务
        Achievement,    //成就
        Special         //特殊
    }

    /// <summary>
    /// 称号数据类
    /// </summary>
    public class Title
    {
        public uint Id { get; private set; }               //配置id

        public CSVTitle.Data cSVTitleData { get; private set; }

        public bool active { get; private set; }          //是否激活

        public bool Dress { get { return Id == Sys_Title.Instance.curUseTitle; } }

        public bool Show { get { return Id == Sys_Title.Instance.curShowTitle; } }

        public TitleType titleType { get; private set; }  //称号类型

        public ulong ExpireTime { get; set; }             //过期时间

        public TitleEndTimer titleEndTimer;

        public bool reachMaxLevel;

        public bool isOnly;//是否是唯一性称号

        public bool read = true;

        public Title(uint id, TitleType _titleType)
        {
            Id = id;
            titleType = _titleType;
            cSVTitleData = CSVTitle.Instance.GetConfData(id);

            isOnly = cSVTitleData.titleType > 0;
            if (isOnly)
            {
                reachMaxLevel = cSVTitleData.titleTypeNum == cSVTitleData.titleTypeLimit;
            }
            else
            {
                reachMaxLevel = true;
            }

            titleEndTimer = new TitleEndTimer(() =>
            {
                Sys_Title.Instance.ExpireTimeReq(id);
            });
        }

        //计算title状态以及到期时间
        public void CalTitleEndTime()
        {
            if (ExpireTime == 0)
            {
                if (Sys_Title.Instance.TryActiveTitle(this))
                {
                    SetActive(true);
                    titleEndTimer.NeedUpdateTime = false;
                    Sys_Title.Instance.RemoveActiveTitle_Limit(this);
                }
            }
            else if (ExpireTime == 1)
            {
                SetActive(false);
                titleEndTimer.NeedUpdateTime = false;
                Sys_Title.Instance.RemoveActiveTitle_Limit(this);
                if (cSVTitleData.titleTypeNum > 1)
                {
                    Sys_Title.Instance.RemoveTitle(this, out bool replaceDress);
                }
            }
            else
            {
                if (Sys_Title.Instance.TryActiveTitle(this))
                {
                    SetActive(true);
                    titleEndTimer.EndTime = ExpireTime;
                    titleEndTimer.NeedUpdateTime = true;
                    Sys_Title.Instance.AddActiveTitle_Limit(this);
                }
            }
        }


        public void SetActive(bool _active)
        {
            active = _active;
        }

        /// <summary>
        /// 用来计时，有限时
        /// </summary>
        public void Update()
        {
            titleEndTimer?.Update();
        }
        /// <summary>
        /// 检查成就称号是否能显示(根据该成就是否达到显示条件)
        /// </summary>
        /// <returns></returns>
        public bool CheckAchievementTitleIsCanShow()
        {
            if (cSVTitleData.titleGo[0] == 10)
                return true;
            if (cSVTitleData.titleGo[0] == 2)
            {
                AchievementDataCell achData = Sys_Achievement.Instance.GetAchievementByTid(cSVTitleData.titleGo[1]);
                if (achData != null)
                {
                    return achData.CheckIsCanShow();
                }
            }
            return false;
        }
    }


    public class TitleEndTimer
    {
        public bool NeedUpdateTime;    //是否需要更新时间

        //public DateTime EndTime;       //到期时间
        public ulong EndTime;

        public long RemainTime;
        //public TimeSpan RemainTime;    

        private Action ontimeOut;

        public TitleEndTimer(Action _ontimeOut)
        {
            ontimeOut = _ontimeOut;
        }

        public void Update()
        {
            if (!NeedUpdateTime)
                return;
            UpdateTime();
        }

        private void UpdateTime()
        {
            uint time = Sys_Time.Instance.GetServerTime();
            RemainTime = (long)(EndTime - time);
            if (RemainTime <= 0)
            {
                NeedUpdateTime = false;
                ontimeOut?.Invoke();
            }

            //if (Sys_Time.ConvertFromTimeStamp(time) >= EndTime)
            //{
            //    NeedUpdateTime = false;
            //}
            //TimeSpan nowTS = new TimeSpan(Sys_Time.ConvertFromTimeStamp(time).Ticks);
            //TimeSpan deleteTS = new TimeSpan(EndTime.Ticks);
            //RemainTime = deleteTS.Subtract(nowTS).Duration();
        }

        public string GetMarkendTimeFormat()
        {
            if (RemainTime < 60)
            {
                return string.Format(CSVLanguage.Instance.GetConfData(2007264).words, RemainTime.ToString());
            }
            string time = LanguageHelper.TimeToString((uint)RemainTime, LanguageHelper.TimeFormat.Type_2);
            return "(" + time + ")";
            //string str = string.Format(CSVLanguage.Instance.GetConfData(1009001).words, EndTime.Year, EndTime.Month, EndTime.Day);
            //return str;
        }
    }
}

