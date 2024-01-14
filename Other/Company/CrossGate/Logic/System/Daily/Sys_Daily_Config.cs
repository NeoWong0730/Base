using System;
using System.Collections.Generic;
using Framework;
using Logic.Core;
using Net;
using Packet;
using SQLite4Unity3d;
using Table;

namespace Logic
{

    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {

        private uint OpenLevelOffset = 0;
        private void ParseConfig()
        {
            ParseDailyType();

            var openleveldata =   CSVParam.Instance.GetConfData(1303);
            OpenLevelOffset = uint.Parse(openleveldata.str_value);

        }

        private void DisposeConfig()
        {
            mDicTypeWith.Clear();

   
            mTempWillOpenList.Clear();

        }

    }
    #region daily type 
    //用于处理本地配置数据
    public partial class Sys_Daily: SystemModuleBase<Sys_Daily>
    {
        private class DailyWithType
        {
            public uint Type { get; set; }

            public List<CSVDailyActivity.Data>  Dataes = new List<CSVDailyActivity.Data>();

            public void Add(CSVDailyActivity.Data data)
            {
                if (data.ActiveType != Type)
                    return;

                Dataes.Add(data);
            }

        }

        const uint NotOpenType = 3;

        private Dictionary<uint, DailyWithType> mDicTypeWith = new Dictionary<uint, DailyWithType>();

        private Dictionary<uint, uint> m_FuncOpen = new Dictionary<uint, uint>();

        public readonly uint DailyFuncOpenID = 20201;

        /// <summary>
        /// 获取日常活动类型
        /// </summary>
        private void ParseDailyType()
        { 

            var dailyActivitys = CSVDailyActivity.Instance.GetAll();

            for (int i = 0, len = dailyActivitys.Count; i < len; i++)
            {
                var value = dailyActivitys[i];

                InitFunc(value.id, value.ActiveType);

                if (m_FuncOpen.ContainsKey(value.FunctionOpenid) == false)
                    m_FuncOpen.Add(value.FunctionOpenid, value.id);

                AddActivityDataType(value);
               
            }
            
        }


        private void AddActivityDataType(CSVDailyActivity.Data value)
        {
            // 类型
            var dailyType = value.ActiveType;

            if (mDicTypeWith.ContainsKey(dailyType) == false)
            {
                mDicTypeWith.Add(dailyType, new DailyWithType() { Type = dailyType });

            }

            mDicTypeWith[dailyType].Dataes.Add(value);
        }

        /// <summary>
        /// 根据活动类型 获取同一类型的日常活动
        /// </summary>
        /// <param name="typeID">活动类型</param>
        /// <returns></returns>
        public List<CSVDailyActivity.Data>  getConfigATypeDaily(uint typeID)
        {
            if (typeID == NotOpenType)
                return getWillOpengDaily();


            DailyWithType value;

            mDicTypeWith.TryGetValue(typeID, out value);

            return value == null ? null : value.Dataes;
        }

        /// <summary>
        /// 获取当天有效的日常活动
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public List<CSVDailyActivity.Data>  getTodayUsefulDailies(uint typeID)
        {
            if (typeID == NotOpenType)
                return getWillOpengDaily();


            var datalist = getConfigATypeDaily(typeID);

            if (datalist == null)
                return null;

            var todaylist = datalist.FindAll(o => isTodayDaily(o.id));

            return todaylist;

        }

        public static bool DayOfWeekCompate(CSVDailyActivity.Data data, int day)
        {
            if (data.OpeningMode2 == null || data.OpeningMode2.Count == 0)
                return true;

            bool result = false;

            int count = data.OpeningMode2.Count;

            for (int i = 0; i < count; i++)
            {
                var value = data.OpeningMode2[i];

                if (value == 0 || value == day)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        public bool isTodayDaily(uint id)
        {
            DailyFunc dailyfunc;

            if (m_DailyFuncDic.TryGetValue(id, out dailyfunc) == false)
                return false;

            if (dailyfunc.isUnLock() && dailyfunc.IsInOpenDay())
                return true;

            return false;
        }

        /// <summary>
        /// 活动的开始时间,根据服务器当前时间计算得出
        /// </summary>
        /// <param name="restTime">根据表格 重置时间  1--自然天， 2--5点</param>
        /// <returns></returns>
        public DateTime getDailyStartTime(uint restTime = 1)
        {
            var nowtime = Sys_Time.Instance.GetServerTime();

            var nowtimetoday = nowtime % 86400;

            DateTime startTime = mTodayDay;

            if (restTime == 2)
            {
                if (nowtimetoday < (5 * 3600))
                    startTime = mYesterDay5;
                else

                    startTime = mTodayDay5;
            }

            return startTime;
        }

        /// <summary>
        /// 获取即将开启活动 
        /// </summary>
        /// <returns></returns>
        private List<CSVDailyActivity.Data>  mTempWillOpenList = new List<CSVDailyActivity.Data>();
        private List<CSVDailyActivity.Data>  getWillOpengDaily()
        {
            mTempWillOpenList.Clear();            

            var dailyActivitys = CSVDailyActivity.Instance.GetAll();

            for (int i = 0, len = dailyActivitys.Count; i < len; i++)
            {
                var item = dailyActivitys[i];

                if (isWillOpenActivity(item))
                {
                    mTempWillOpenList.Add(item);
                }
            }

            return mTempWillOpenList;
        }

        private bool isWillOpenActivity(CSVDailyActivity.Data item)
        {
            if (item == null)
                return false;

            if (Sys_FunctionOpen.Instance.IsOpen(item.FunctionOpenid, false))
                return false;

            var rolelevel = Sys_Role.Instance.Role.Level;

            if (item.HideLevel > 0 && rolelevel >= item.HideLevel)
                return false;

            if (rolelevel + OpenLevelOffset >= item.OpeningLevel)
            {
                return true;
            }

            return false;
        }
        private bool isWillOpenActivity(uint id)
        {
            var item = CSVDailyActivity.Instance.GetConfData(id);

            return isWillOpenActivity(item);
        }

        /// <summary>
        /// 获取日常活动类型
        /// </summary>
        /// <param name="dailyID"></param>
        /// <returns></returns>
        public uint getDailyType(uint dailyID)
        {
            var item = CSVDailyActivity.Instance.GetConfData(dailyID);

            if (/*item.OpeningLevel > Sys_Role.Instance.Role.Level*/ Sys_FunctionOpen.Instance.IsOpen(item.FunctionOpenid, false) == false)
                return NotOpenType;

            return item.ActiveType;
        }
    }
    #endregion

    public static class DailyDataHelper
    {
        public static string GetOpenConditionString(uint functionid)
        {
            List<int> LevelCondition = new List<int>();
            DailyDataHelper.GetCondition(functionid, 10202, ref LevelCondition);

            List<int> TaskCondition = new List<int>();

            DailyDataHelper.GetCondition(functionid, 10091, ref TaskCondition);

            var levelCount = LevelCondition.Count;
            var taskCount = TaskCondition.Count;

            if (levelCount == 0 && taskCount == 0)
                return string.Empty;

            if (levelCount > 0 && taskCount == 0)
                return LanguageHelper.GetTextContent(11831, (LevelCondition[0]).ToString());

            var data = CSVTask.Instance.GetConfData((uint)TaskCondition[0]);
            if (levelCount == 0 && taskCount > 0)
            {
                return LanguageHelper.GetTextContent(11832, CSVTaskLanguage.Instance.GetConfData(data.taskName).words);
            }

            return LanguageHelper.GetTextContent(11833, (LevelCondition[0]).ToString(), CSVTaskLanguage.Instance.GetConfData(data.taskName).words);

        }
        public static int OpenLevel(uint OpenID)
        {
            Sys_FunctionOpen.FunctionOpenData fodata;

            if (Sys_FunctionOpen.Instance.dict_AllFunctionOpen.TryGetValue(OpenID, out fodata) == false)
                return 0;

            int result = checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi1);
            if (result > 0)
                return result;

            result = checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi2);
            if (result > 0)
                return result;

            result = checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi3);
            if (result > 0)
                return result;

            result = checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi4);
            if (result > 0)
                return result;

            result = checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi5);
            if (result > 0)
                return result;

            result = checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi6);
            if (result > 0)
                return result;

            return 0;
        }

        public static int OpenLevel(this CSVDailyActivity.Data value)
        {
            return OpenLevel(value.FunctionOpenid);

        }

        private static int checkCondi(List<List<int>> value)
        {
            if (value == null)
                return -1;

            int count = value.Count;

            for (int i = 0; i < count; i++)
            {
                var list = value[i];

                if (list[0] == 10181)
                {
                    return list[1] + 1;
                }

                if (list[0] == 10202u || list[0] == 10201u)
                {
                    return list[1];
                }

            }

            return -1;
        }

        public static void GetCondition(uint OpenID, uint id,ref List<int> resultList)
        {
            Sys_FunctionOpen.FunctionOpenData fodata;

            if (Sys_FunctionOpen.Instance.dict_AllFunctionOpen.TryGetValue(OpenID, out fodata) == false)
                return ;

            checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi1,id, resultList);


            checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi2,id, resultList);


            checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi3,id, resultList);


            checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi4,id, resultList);
  

            checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi5,id, resultList);


            checkCondi(fodata.checkConditionData.cSVCheckseqData.CheckCondi6,id, resultList);

        }

        private static void checkCondi(List<List<int>> value,uint id,  List<int> resultList)
        {
            if (value == null)
                return;

            int count = value.Count;

            for (int i = 0; i < count; i++)
            {
                var list = value[i];

                if (list[0] == id)
                {
                    resultList.Add(list[1]);
                }

            }
        }
    }
}
