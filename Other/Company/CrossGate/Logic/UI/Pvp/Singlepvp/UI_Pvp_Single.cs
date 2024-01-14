using Framework;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    /// PVP 主界面
    /// </summary>
    public partial class UI_Pvp_Single : UIBase, UI_Pvp_Single_Layout.IListener
    {
        UI_Pvp_Single_Layout m_Layout = new UI_Pvp_Single_Layout();

        UI_PvpSingle_Rule m_UIRule = new UI_PvpSingle_Rule();
        UI_PvpSingle_TodayReward m_UITodayReward = new UI_PvpSingle_TodayReward();
       // UI_PvpSingle_RankReward m_UIRankReward = new UI_PvpSingle_RankReward();

        //伙伴列表
        private List<uint> m_PartnerList = new List<uint>();

        private float m_LastTime = 0;

        private int m_FocusTodayRewardIndex = -1;


        private int mNextStartHour = 0;
        private int mNextStarMinu = 0;

        private int mNextHaveTime = 0;

        private float mNextTimeRefreshTime = 0;

        private bool mbActive = true;

        private bool mIsClose = false;
        /// <summary>
        /// 限制类型的数量，下标0 为401 ，下标 1 为 601
        /// </summary>
        private List<int> m_OccicountList = new List<int>() { 0, 0 };


        private List<int> mCumulativeVictoryNum = null;
        protected override void OnLoaded()
        {            
            m_Layout.Load(gameObject.transform);
            m_UIRule.Init(gameObject.transform.Find("Animator/View_rule"));
            m_UITodayReward.Init(gameObject.transform.Find("Animator/View_Reward"));
           // m_UIRankReward.Init(gameObject.transform.Find("Animator/View_RankReward"));

            m_Layout.SetListener(this);


            m_UITodayReward.HideEvent = OnTodayRewardHide; 
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Pvp.Instance.eventEmitter.Handle(Sys_Pvp.EEvents.PvpInfoRefresh, OnPvpInfoRefresh,toRegister);

            Sys_Pvp.Instance.eventEmitter.Handle(Sys_Pvp.EEvents.StartMatch, OnPvpStartMatch, toRegister);

            Sys_Pvp.Instance.eventEmitter.Handle(Sys_Pvp.EEvents.GetCumulateAward, RefreshCumulativeVictoryState, toRegister);

            Sys_Pvp.Instance.eventEmitter.Handle<ArenaDanInfo>(Sys_Pvp.EEvents.GetDanLvUpAward, RefreshDanLvUpAward, toRegister);
            Sys_Pvp.Instance.eventEmitter.Handle<List<ArenaDanInfo>>(Sys_Pvp.EEvents.GetAllDanLvUpAward, RefreshDanLvUpAwards,toRegister);

            Sys_Pvp.Instance.eventEmitter.Handle(Sys_Pvp.EEvents.DanLvUpAward, RefreshDanLvUpAward, toRegister);
        }
        protected override void OnShow()
        {
           
            mNextTimeRefreshTime = Time.time;

            m_LastTime = 0;
            
            RefreshPartner();

            RefreshCumulativeVictory();

            RefreshPvpInfo();

            // RefreshTime();
            mIsClose = !Sys_Pvp.Instance.IsPvpActive();

            if (mIsClose)
                RefreshCloseTime();

            m_Layout.m_TransOpenTips.gameObject.SetActive(!PvpStageLvHelper.IsOpenLevel80);

        }
        protected override void OnHide()
        {            
            m_LastTime = 0;
        }

        protected override void OnOpen(object arg)
        {            
            Sys_Pvp.Instance.Apply_ArneaInfo();

            Sys_Pvp.Instance.Apply_RiseInRank();

            mCumulativeVictoryNum = Sys_Pvp.Instance.GetCumulativeVictoryNum();
        }
        protected override void OnUpdate()
        {
            float offsetTime = Time.time - m_LastTime;

            if (offsetTime < 10 && m_LastTime != 0)
                return;

            if (Sys_Pvp.Instance.GetRemainingTime() > 0)
                RefreshTime();
            else
                RefreshNextTime();

            m_LastTime = Time.time;
        }

        protected override void OnClose()
        {
            m_Layout.OnDestory();
        }
        #region 伙伴
        /// <summary>
        /// 处理伙伴列表
        /// </summary>
        private void RefreshPartner()
        {
            
            GetPartnerTypeCount(m_OccicountList);

            GetPartnerList();

            m_Layout.SetLimiteOcci401(m_OccicountList[0]);
            m_Layout.SetLimiteOcci601(m_OccicountList[1]);

            int size = m_PartnerList.Count + 1;

            m_Layout.SetPartnerSize(5);

            if (size == 0)
                return;

            SetAddMode(size);

            SetParnerRoleInfo(0, Sys_Role.Instance.RoleId, Sys_Role.Instance.Role.Name.ToStringUtf8(),Sys_Role.Instance.Role.Career);

            int index = 1;
            foreach (var item in m_PartnerList)
            {
                var  partnerData = CSVPartner.Instance.GetConfData(item);

                SetParnerInfo(index, partnerData.headid, partnerData.name,partnerData.profession);
                index += 1;

            }

           
               

        }

        private void SetAddMode(int hadSize)
        {
            for (int i = 0; i < 5; i++)
            {
                m_Layout.SetParnerAddMode(i, i >= hadSize);
            }
        }
        private void SetParnerInfo(int index, uint headid, uint name,uint occi)
        {
            m_Layout.SetParnerIcon(index, headid);

            m_Layout.SetParnerName(index, name);

            m_Layout.SetParnerProfession(index, OccupationHelper.GetIconID(occi));
        }

        private void SetParnerRoleInfo(int index, ulong headid, string name,uint occi)
        {
            m_Layout.SetParnerRoleIcon(index, headid,true);

            m_Layout.SetParnerName(index, name);

            m_Layout.SetParnerProfession(index, OccupationHelper.GetIconID(occi));
        }

        private void GetPartnerList()
        {
            m_PartnerList.Clear();

            var CurFmID =  Sys_Partner.Instance.GetCurFmList();



            var Value = Sys_Partner.Instance.GetFormationByIndex((int)CurFmID);

            // if (result)
            {
                IList<uint> list = Value.Pa;

                foreach (var item in list)
                {
                    if (item > 0)
                        m_PartnerList.Add(item);
                }
   
            }
        }

        /// <summary>
        /// 判断伙伴的配置是否符合PVP规则
        /// </summary>
        /// <param name="value"></param>
        private bool DoPartnerFormatWithRule(/*Packet.PartnerFormation value*/)
        {
           int count = m_OccicountList[0] + m_OccicountList[1];

            return count < 3;

        }

        private void GetPartnerTypeCount(List<int> countlist)
        {
            int count = 0;

            int ruleCount = 0;

            if (countlist.Count >= 2)
            {
                countlist[0] = 0;
                countlist[1] = 0;
            }

            if (Sys_Role.Instance.Role.Career == 401 || Sys_Role.Instance.Role.Career == 601)
            {
                int index = Sys_Role.Instance.Role.Career == 401 ? 0 : 1;

                countlist[index] = countlist[index] + 1;

                ruleCount += 1;
                count += 1;

            }

            var CurFmID = Sys_Partner.Instance.GetCurFmList();

            var Value = Sys_Partner.Instance.GetFormationByIndex((int)CurFmID);

            foreach (var item in Value.Pa)
            {
                var partnerData = CSVPartner.Instance.GetConfData(item);

                if (partnerData != null)
                {
                    if (partnerData.profession == 401 || partnerData.profession == 601)
                    {
                        int index = partnerData.profession == 401 ? 0 : 1;

                        countlist[index] = countlist[index] + 1;

                        ruleCount += 1;
                    }
                        
                    count += 1;
                }

            }
        }
        /// <summary>
        /// 判断伙伴的配置是否符合PVP规则
        /// </summary>
        /// <param name="value"></param>
        //private bool DoPartnerFormatWithRule( IList<uint> listValue)
        //{
        //    int ruleCount = 0;
        //    int count = 0;
        //    foreach (var item in listValue)
        //    {
        //        var partnerData = CSVPartner.Instance.GetConfData(item);

        //        if (partnerData != null)
        //        {
        //            if (partnerData.profession == 401 || partnerData.profession == 601)
        //                ruleCount += 1;

        //            count += 1;
        //        }

        //    }
        //    return count > 0 && ruleCount <= 3;
        //}

        #endregion

        #region 赛季信息
        private void RefreshPvpInfo()
        {
           
            m_Layout.SetMineRankNum(Sys_Pvp.Instance.MineRank);

            CSVArenaSegmentInformation.Data LastItem = null;
            if (Sys_Pvp.Instance.IsLevelEffect)
            {

                LastItem = CSVArenaSegmentInformation.Instance.GetConfData((uint)Sys_Pvp.Instance.LastLevelID);

                m_Layout.SetLevelUpEffect(LastItem == null ? string.Empty: LastItem.RankIcon, GetLevelIcon());

              
            }
               
            else
                m_Layout.SetMineLevelIcon(GetLevelIcon());


        

            m_Layout.SetMineLevelTex(GetLevelString());

            m_Layout.SetMineBigStarActiv(Sys_Pvp.Instance.isBigStar);


            if (!Sys_Pvp.Instance.isBigStar)
            {
                int maxStarC = 0;
               var infoData = CSVArenaSegmentInformation.Instance.GetConfData((uint)Sys_Pvp.Instance.LevelID);
                if (infoData != null)
                {
                    var maxStar = CSVArenaMaxStars.Instance.GetConfData(infoData.Rank);

                    maxStarC = (int)maxStar.MaxStar;
                }

                m_Layout.SetMineLevleStarMax(maxStarC);

                if (Sys_Pvp.Instance.IsStarEffect && Sys_Pvp.Instance.Star != 0)
                {
                    m_Layout.SetMineLevelStarEffect(Sys_Pvp.Instance.LastStar, Sys_Pvp.Instance.Star, maxStarC);

                    Sys_Pvp.Instance.IsStarEffect = false;
                }
                else
                    m_Layout.SetMineLevleStar(Sys_Pvp.Instance.Star);
            }


            Sys_Pvp.Instance.IsLevelEffect = false;

            if (Sys_Pvp.Instance.isBigStar)
                m_Layout.SetBigStarNum(Sys_Pvp.Instance.Star);


            if (Sys_Pvp.Instance.SeasonNum == 0 && Sys_Pvp.Instance.SeasonWeek == 0)
            {
                m_Layout.SetSeasonNum(string.Empty);
                m_Layout.SetSeasonDate(string.Empty);


            }

            else
            {
               
                m_Layout.SetSeasonNum(LanguageHelper.GetTextContent(10126, Sys_Pvp.Instance.SeasonNum.ToString()) + LanguageHelper.GetTextContent(10127, Sys_Pvp.Instance.SeasonWeek.ToString()));

                string starTime = Sys_Pvp.Instance.PvpStartTimesampe == 0 ? string.Empty : string.Format("{0}/{1}/{2}",
                    Sys_Pvp.Instance.PvpStarTime.Year.ToString(), Sys_Pvp.Instance.PvpStarTime.Month.ToString(), Sys_Pvp.Instance.PvpStarTime.Day.ToString());

                string endTime = Sys_Pvp.Instance.PvpEndTimesampe == 0 ? string.Empty : string.Format("{0}/{1}/{2}",
                    Sys_Pvp.Instance.PvpEndTime.Year.ToString(), Sys_Pvp.Instance.PvpEndTime.Month.ToString(), Sys_Pvp.Instance.PvpEndTime.Day.ToString());

                m_Layout.SetSeasonDate(starTime + " - " + endTime);
            }

            if (Sys_Pvp.Instance.GetRemainingTime() > 0)
                m_Layout.SetRemainingTex(LanguageHelper.GetTextContent(10176));
            else
            {
                int str = Sys_Pvp.Instance.TodayAcitityIsOver() ? 10633 : 10632;
                m_Layout.SetRemainingTex(LanguageHelper.GetTextContent((uint)str));
            }
            m_Layout.SetServerName(Sys_Pvp.Instance.GetCurLevelServerName());

            m_Layout.SetLevelUpRewardFxActive(Sys_Pvp.Instance.IsHaveDanLvUpAward());
        }


        private void RefreshDanLvUpAward(ArenaDanInfo info)
        {
            m_Layout.SetLevelUpRewardFxActive(Sys_Pvp.Instance.IsHaveDanLvUpAward());
        }

        private void RefreshDanLvUpAwards(List<ArenaDanInfo> infos)
        {
            m_Layout.SetLevelUpRewardFxActive(Sys_Pvp.Instance.IsHaveDanLvUpAward());
        }
        private void RefreshDanLvUpAward()
        {
            m_Layout.SetLevelUpRewardFxActive(Sys_Pvp.Instance.IsHaveDanLvUpAward());
        }

        private void NonePvpInfo()
        {

        }
        private string GetLevelIcon()
        {
            return Sys_Pvp.Instance.LevelIcon;
        }

        private string GetLevelString()
        {
            if (Sys_Pvp.Instance.LevelString == 0)
                return string.Empty;

            return LanguageHelper.GetTextContent(Sys_Pvp.Instance.LevelString);
        }

 
        private void RefreshCumulativeVictory()
        {
            for (int i = 0; i < mCumulativeVictoryNum.Count; i++)
            {
                var state = Sys_Pvp.Instance.GetCumulativeVictoryRewardState(mCumulativeVictoryNum[i]);

                m_Layout.SetCumulativeVictory(i, state);

                m_Layout.SetCumulativeVictoryID(i, i);
            }
        }

        private void RefreshCumulativeVictoryState()
        {
            for (int i = 0; i < mCumulativeVictoryNum.Count; i++)
            {
                var state = Sys_Pvp.Instance.GetCumulativeVictoryRewardState(mCumulativeVictoryNum[i]);

                m_Layout.SetCumulativeVictory(i, state);
            }
        }


        private void RefreshTime()
        {
            m_Layout.SetRemainingTex(LanguageHelper.GetTextContent(10176));

            var time = Sys_Pvp.Instance.GetRemainingTime();

            string str = string.Empty;

            int day = (int)(time / 86400);
            int hours = (((int)time) % 86400) / 3600;
            int min = (((int)time) % 86400) % 3600 / 60;

            if (day > 0)
                str += LanguageHelper.GetTextContent(10154, day.ToString());

            if (day > 0 || hours > 0)
                str += LanguageHelper.GetTextContent(10155, hours.ToString());

            str += LanguageHelper.GetTextContent(10156, min.ToString());


            mIsClose = !Sys_Pvp.Instance.IsPvpActive(false);

            if (mIsClose)
                RefreshCloseTime();

            mbActive = false;


            if (!mIsClose)
                m_Layout.SetRemainingTime(str);




        }

        private void RefreshNextTime()
        {
            uint time =  Sys_Pvp.Instance.GetNextStartTime();

            if (time <= 0)
            {
                SetActiveClose();
                return;
            }

            string str = string.Empty;

            int day = (int)(time / 86400);
            int hours = (((int)time) % 86400) / 3600;
            int min = (((int)time) % 86400) % 3600 / 60;

            if (day > 0)
                str += LanguageHelper.GetTextContent(10154, day.ToString());

            if (day > 0 || hours > 0)
                str += LanguageHelper.GetTextContent(10155, hours.ToString());

            str += LanguageHelper.GetTextContent(10156, min.ToString());

            m_Layout.SetRemainingTime(str);

        }

        private void RefreshCloseTime()
        {
            m_Layout.SetRemainingTex(LanguageHelper.GetTextContent(10685));

            m_Layout.SetRemainingTime(string.Empty);
          
        }

        private void SetActiveClose()
        {
            mIsClose = true;
            RefreshCloseTime();
        }

        #endregion
    }

    /// <summary>
    /// 监听处理
    /// </summary>
    public partial class UI_Pvp_Single : UIBase, UI_Pvp_Single_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickRuleDetail()
        {
            m_UIRule.SetSort(nSortingOrder + 10);
            m_UIRule.Show();
        }

        public void OnClickRandkReward()
        {
            UIManager.OpenUI(EUIID.UI_Pvp_SingleRankReward);
        }

        public void OnClickLevelUpReward()
        {
            UIManager.OpenUI(EUIID.UI_Pvp_RiseInRank);
        }

        public void OnClickRank()
        {
             UIManager.OpenUI(EUIID.UI_Pvp_SingleRank);
            //UIManager.OpenUI(EUIID.UI_Pvp_SingleNewSeason);
        }

        public void OnClickMatch()
        {
            //var CurFmID = Sys_Partner.Instance.GetCurFmList();

            //var Value = Sys_Partner.Instance.GetFormationByIndex((int)CurFmID);

            bool result = DoPartnerFormatWithRule();

            if (result == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10194));
                return;
            }
             

            Sys_Pvp.Instance.Apply_Mathch();

            //UIManager.OpenUI(EUIID.UI_Pvp_SingleMatch);
        }
        public void OnClickMemberAdd()
        {
            PartnerUIParam param = new PartnerUIParam();
            param.tabIndex = 1;
            UIManager.OpenUI(EUIID.UI_Partner, false, param);
        }

        public  void OnClickCumulative(int index)
        {
            if (index >= mCumulativeVictoryNum.Count)
                return;

            int id = mCumulativeVictoryNum[index];

            int result = Sys_Pvp.Instance.GetCumulativeVictoryRewardState(id);

            if (result == 1)
            {
                Sys_Pvp.Instance.Apply_GetCumulateAward(id);
                m_Layout.SetCumulativeVictoryItemLostFocus(index);
                return;
            }

            m_FocusTodayRewardIndex = index;

            var rewardlist = Sys_Pvp.Instance.GetCumulativeVictoryReward(id);

            m_UITodayReward.SetSort(nSortingOrder + 10);

            m_UITodayReward.Show();

            m_UITodayReward.SetWinNum(id);

            m_UITodayReward.SetRewardList(rewardlist,result);

        }

        private void OnTodayRewardHide()
        {
            m_Layout.SetCumulativeVictoryItemLostFocus(m_FocusTodayRewardIndex);

            m_FocusTodayRewardIndex = -1;
        }
    }


    /// <summary>
    /// 通知
    /// </summary>
    public partial class UI_Pvp_Single : UIBase, UI_Pvp_Single_Layout.IListener
    {
        /// <summary>
        /// PVP赛季信息刷新通知
        /// </summary>
        private void OnPvpInfoRefresh()
        {
           

            RefreshPvpInfo();

            RefreshCumulativeVictoryState();
        }

        private void OnPvpStartMatch()
        {
            CloseSelf();
           
        }
    }



}
