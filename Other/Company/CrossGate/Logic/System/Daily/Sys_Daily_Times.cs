using System;
using System.Collections.Generic;
using Logic.Core;
using Net;
using Packet;
using SQLite4Unity3d;
using Table;
namespace Logic
{


    public interface IDailyTimes
    {
        int GetDailyTimes(uint dailyID);
    }

    /// <summary>
    /// 用于查询每个活动的次数，次数来自每个活动模块
    /// </summary>

    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        public uint GetDailyTimes(uint dailyID)
        {
            int result = 0;
            switch (dailyID)
            {
                case 1: //单人爬塔
                    result = Sys_Instance.Instance.GetPlayTimes(dailyID);
                    break;
                case 10://经典头目战
                    result = Sys_ClassicBossWar.Instance.GetDailyTimes();
                    break;
                case 20://人物传记
                    result = Sys_Instance.Instance.GetDailyTimes(dailyID);
                    break;
                case 50://地域防范事件
                    result = Sys_Activity.Instance.GetDailyTimes(dailyID);
                    break;
                case 60://每日酒吧事件
                    result = (int)GetPubTimes();
                    break;
                case 100://百人道场
                    result = (int)Sys_HundredPeopleArea.Instance.times;
                    break;
                case 113:// 家族资源战
                    result = (int)(Sys_FamilyResBattle.Instance.weeklyUsedCount);
                    break;
                case 114:
                    result = (int)Sys_Family.Instance.DailyFeedCount;
                    break;
                case 120:
                    result = GetKilledMosterTiems();
                    break;
                case 130:
                    result = (int)Sys_Instance_Bio.Instance.GetDailyTimes();
                    break;

                case 101:
                    result = (int)Sys_SurvivalPvp.Instance.GetDailyTimes();
                    break;
                case 102:
                    result = (int)Sys_LadderPvp.Instance.GetDailyTimes();
                    break;
                case 90://时空幻境
                    result = (int)Sys_GoddnessTrial.Instance.GetDailyTimes();
                    break;

                case 30://恐怖旅团周常
                case 31:
                case 32:
                    CSVTerrorSeriesData.Data infoData = CSVTerrorSeriesData.Instance.GetConfData(dailyID);
                    result = (int)Sys_TerrorSeries.Instance.GetWeekTime(infoData.instanID);
                    break;
                //case 115:
                //    result = (int)Sys_Family.Instance.FightCount;
                //    break;

                case 140:
                    result = (int)Sys_JSBattle.Instance.GetDaliyChallengeTime();
                    break;
                case 40: //恐怖旅团日常
                case 41: //恐怖旅团日常
                case 42:
                case 70:
                case 111:
                case 112:
                case 115:
                    {
                        var curac = getDailyAcitvity(dailyID);
                        var eachac = GetDailyActivityNum(dailyID);

                        result =(int)(eachac == 0 ? 0 : curac / eachac);
                    }
                    break;
                case 210:
                    {
                        result = Sys_Instance_UGA.Instance.GetCurInstanceMaxStageOrder();
                    }
                    break;
                case 141:
                    {
                        result = (int)Sys_MerchantFleet.Instance.MerchantTotalCount;
                    }
                    break;

            }

            bool isWorldBossActivity = Sys_WorldBoss.Instance.TryGetActivityIdByDailyId(dailyID, out var actitityId);
            if (isWorldBossActivity)
            {
                if (Sys_WorldBoss.Instance.usedCount.TryGetValue(actitityId, out BossPlayMode playmode))
                {
                    if (playmode.csv.rewardLimit != 0) {
                        result = (int)playmode.weeklyUsedCount;
                    }
                    else {
                        result = (int)playmode.dailyUsedCount;
                    }
                }
            }
            return result < 0 ? 0 : ((uint)result);
        }


        public uint GetDailyTotalTimes(uint dailyID)
        {
            var data = CSVDailyActivity.Instance.GetConfData(dailyID);

            return data == null ? 0 : data.limite;
        }

        public bool IsDailyTimesMax(uint dailyID)
        {
            var maxnum = GetDailyTotalTimes(dailyID);

            if (maxnum == 0)
                return false;

            var curnum = getDailyCurTimes(dailyID);

            return curnum >= maxnum;
        }
        public string GetDailySpecial(uint dailyID)
        {
            string strvalue = string.Empty;

            var csvdata = CSVDailyActivity.Instance.GetConfData(dailyID);

            switch (dailyID)
            {
                case 100:
                    {
                      //  uint curLevel = Sys_TravellerAwakening.Instance.awakeLevel;

                        int curStage = 0;
                        int curLevel = 0;
                        int totalStage = 0;
                        int totalLevel = 0;

                        Sys_HundredPeopleArea.Instance.GetPassedStage(out curStage, out curLevel);
                        Sys_HundredPeopleArea.Instance.GetMaxStage(out totalStage, out totalLevel);

                        strvalue = LanguageHelper.GetTextContent(csvdata.special_des_lan,curStage.ToString(), curLevel.ToString());
                    }
                    break;
                case 110:
                    {
                       string value = Sys_Bag.Instance.GetItemCount(17u).ToString();
                        strvalue = LanguageHelper.GetTextContent(csvdata.special_des_lan, value);
                    }
                    break;
                case 120:
                    {
                        if (Sys_Hangup.Instance.cmdHangUpDataNtf != null)
                        { 
                            string value = Sys_Hangup.Instance.cmdHangUpDataNtf.WorkingHourPoint.ToString();
                            string triedvalue = Sys_Hangup.Instance.GetTired().ToString();

                            strvalue = LanguageHelper.GetTextContent(csvdata.special_des_lan, value, triedvalue);
                        }
                    }
                    break;
                case 115:
                    {
                        strvalue = LanguageHelper.GetTextContent(csvdata.special_des_lan, Sys_Family.Instance.score.ToString());
                    }
                    break;
                case 150:
                    {
                        var count = Sys_Bag.Instance.GetItemCount(100220u);
                        count += Sys_Bag.Instance.GetItemCount(100221u);
                        count += Sys_Bag.Instance.GetItemCount(100222u);
                        strvalue = LanguageHelper.GetTextContent(csvdata.special_des_lan, count.ToString());
                    }
                    break;
                case 160:
                    {
                        int finishcount = 0;
                        int maxcount = 0;
                        Sys_Adventure.Instance.GetMapFinishAndMaxNum(out finishcount, out maxcount);

                        strvalue = LanguageHelper.GetTextContent(csvdata.special_des_lan, finishcount.ToString(),maxcount.ToString());

                    }
                    break;
                case 170:
                    {
                        strvalue = LanguageHelper.GetTextContent(csvdata.special_des_lan, Sys_NPCFavorability.Instance.Favorability.ToString());
                    }
                    break;
                case 180:
                    {
                        string value = Sys_Bag.Instance.GetItemCount(5u).ToString();
                        strvalue = LanguageHelper.GetTextContent(csvdata.special_des_lan, value);
                    }
                    break;
                case 230:
                    {
                        string value = Sys_NPCFavorability.Instance.Favorability.ToString();
                        strvalue = LanguageHelper.GetTextContent(csvdata.special_des_lan, value);
                    }
                    break;
                case 220:
                    {
                        string value = Sys_ActivityTrialGate.Instance.curStage.ToString();
                        string maxvalue = Sys_ActivityTrialGate.Instance.GetAllTrialStageNum().ToString();
                        strvalue = LanguageHelper.GetTextContent(csvdata.special_des_lan, value,maxvalue);
                    }
                    break;
                case 240:
                    {
                        strvalue = Sys_ActivityBossTower.Instance.GetCurRnakStateDescribe(2);
                    }
                    break;
                case 250:
                    {
                        strvalue = Sys_ActivityBossTower.Instance.GetCurRnakStateDescribe();
                    }
                    break;
            }

            return strvalue;
        }


        public void GetDailyTimesForSevers(uint dailyID)
        {
            var data = CSVDailyActivity.Instance.GetConfData(dailyID);
            if (data == null || Sys_FunctionOpen.Instance.IsOpen(data.FunctionOpenid) == false)
                return;

            switch (dailyID)
            {
                case 115:
                    Sys_Family.Instance.GuildPetGetTrainingScoreReq();
                    break;

            }

        }
    }


}
