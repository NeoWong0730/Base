using System;
using System.Collections.Generic;

using Table;

namespace Logic
{
    /// <summary>
    /// 对活动类型的处理，排序，根据人物等级解锁。
    /// 目的：为了降低UI加载时要经常进行查询比较或者排序的性能损耗
    /// </summary>
    public partial class Sys_Daily
    {
        Dictionary<uint, DailyWithType> m_DynamicType = new Dictionary<uint, DailyWithType>();

       public void InitDynamicType()
        {
            m_DynamicType.Clear();

            foreach (var kvp in mDicTypeWith)
            {
                int count = kvp.Value.Dataes.Count;

                var datas = kvp.Value.Dataes;

                for (int i = 0; i < count; i++)
                {
                    if (isWillOpenActivity(datas[i]))
                    {
                        AddDynamicType(NotOpenType, datas[i]);
                        continue;
                    }

                    var dailyfunc = GetDailyFunc(datas[i].id);

                    if (dailyfunc.isUnLock() == false)
                        continue;

                    if (kvp.Key != 2)
                    {
                        AddDynamicType(kvp.Key, datas[i]);
                    }

                    else if (kvp.Key == 2 &&
                        isTodayDaily(datas[i].id) /* && dailyfunc.GetOpenTime() > 0*/) 
                    {
                        AddDynamicType(kvp.Key, datas[i]);
                    }
                }
            }

            foreach (var kvp in m_DynamicType)
            {
                SortTypeHelper.Sort(kvp.Key, kvp.Value.Dataes);
            }
        }

        private void AddDynamicType(uint type, CSVDailyActivity.Data data)
        {
            if (m_DynamicType.ContainsKey(type) == false)
            {
                m_DynamicType.Add(type, new DailyWithType());
            }

            m_DynamicType[type].Dataes.Add(data);
        }

        public void RemoveDynamicType(uint type, uint id)
        {
            if (m_DynamicType.ContainsKey(type) == false)
            {
                return;
            }

            m_DynamicType[type].Dataes.RemoveAll(o => o.id == id);
        }



        public List<CSVDailyActivity.Data>  GetVaildDailiesByType(uint type)
        {
            List<CSVDailyActivity.Data>  datas = null;

            if (m_DynamicType.TryGetValue(type, out DailyWithType dailyWithType))
            {
                datas = dailyWithType.Dataes;
            }

            return datas;

        }

        public bool HaveVaildType(uint type)
        {
            return m_DynamicType.ContainsKey(type);
        }

        public bool HaveDailyReward(uint id)
        {
            if (m_DailyFuncDic.TryGetValue(id, out DailyFunc dailyFunc) == false)
                return false;

            return dailyFunc.HaveReward();
        }


        public bool HaveDailyRewardAll()
        {
            int count = CSVDailyActivity.Instance.Count;

            for (int i = 0; i < count; i++)
            {
                CSVDailyActivity.Data data = CSVDailyActivity.Instance.GetByIndex(i);
                if (HaveAward(data.id) && isTodayDaily(data.id))
                    return true;
            }

            return false;
        }
        public bool IsTypeHaveRedPoint(uint typeID)
        {
            bool result = false;

            if (m_DynamicType.TryGetValue(typeID, out DailyWithType dailyWithType))
            {
                int count = dailyWithType.Dataes.Count;

                for (int i = 0; i < count; i++)
                {
                    var func = GetDailyFunc(dailyWithType.Dataes[i].id);

                    if (HaveAward(dailyWithType.Dataes[i].id) || func.HaveNewNotice())
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }


        public bool HaveAward(uint dailyId)
        {

            if (m_DailyFuncDic.TryGetValue(dailyId, out DailyFunc dailyFunc) == false)
                return false;

            bool result = dailyFunc.HaveReward();

            if (result && dailyFunc.DailyType == EDailyType.Limite)
            {
                var dailystate = Sys_Daily.Instance.GetLimitDailyState(dailyId);

                if (dailystate == ELimitDailyState.Over || dailystate == ELimitDailyState.TodayStart)
                    result = false;
            }
          

            return result;
        }

        public bool isTypeNewTips(uint typeID)
        {
            bool result = false;


            if (m_DynamicType.TryGetValue(typeID, out DailyWithType dailyWithType))
            {
                int count = dailyWithType.Dataes.Count;

                for (int i = 0; i < count; i++)
                {
                    if (isNewTips(dailyWithType.Dataes[i].id) && isDailyReady(dailyWithType.Dataes[i].id, false))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }


        class SortTypeHelper
        {
            public static void Sort(uint type, List<CSVDailyActivity.Data>  datalist)
            {
                if (type == 1 || type == 5 || type == 6)
                {
                    datalist.Sort((value0, value1) => Sort1(value0, value1));
                }
                else if (type == 2)
                {
                    datalist.Sort((value0, value1) => Sort2(value0, value1));
                }
                else if (type == 3)
                {
                    datalist.Sort((value0, value1) => Sort3(value0, value1));
                }
            }

            //挑战活动，每日活动 排序
            public static int Sort1(CSVDailyActivity.Data value0, CSVDailyActivity.Data value1)
            {
                var dailyfuc0 = Sys_Daily.Instance.GetDailyFunc(value0.id);
                var dailyfuc1 = Sys_Daily.Instance.GetDailyFunc(value1.id);

                bool valu0isMax = dailyfuc0.HadUsedTimes();
                bool valu1isMax = dailyfuc1.HadUsedTimes();

                if (valu0isMax == valu1isMax)
                {
                    return CompareSortOrder(value0, value1);
                }

                if (valu0isMax && !valu1isMax)
                    return 1;

                return -1;
            }

            private static int CompareSortOrder(CSVDailyActivity.Data value0, CSVDailyActivity.Data value1)
            {
                if (value0.ActiveOrder == value1.ActiveOrder)
                    return 0;

                if (value0.ActiveOrder > value1.ActiveOrder)
                    return -1;

                return 1;
            }

            /// <summary>
            /// 限时活动排序
            /// </summary>
            /// <param name="value0"></param>
            /// <param name="value1"></param>
            /// <returns></returns>
            public static int Sort2(CSVDailyActivity.Data value0, CSVDailyActivity.Data value1)
            {
                var fuc0 =  Sys_Daily.Instance.GetDailyFunc(value0.id);
                var fuc1 = Sys_Daily.Instance.GetDailyFunc(value1.id);

                var time0 = fuc0.GetOpenTime();
                var time1 = fuc1.GetOpenTime();

                if ((time0 == 0 && time1 == 0))
                {
                    return CompareID(value0, value1);
                }

                if (time0 > 0 && time1 > 0)
                {
                    if (time0 < time1)
                        return -1;

                    if (time0 > time1)
                        return 1;

                    return 0;
                }

                if (time0 > time1)
                    return -1;

                return 1;

            }

            private static int CompareID(CSVDailyActivity.Data value0, CSVDailyActivity.Data value1)
            {
                if (value0.id == value1.id)
                    return 0;

                if (value0.id > value1.id)
                    return -1;

                return 1;
            }

            /// <summary>
            /// 即将开启排序
            /// </summary>
            /// <param name="value0"></param>
            /// <param name="value1"></param>
            /// <returns></returns>
            public static int Sort3(CSVDailyActivity.Data value0, CSVDailyActivity.Data value1)
            {
                if (value0.OpeningLevel == value1.OpeningLevel)
                    return 0;

                if (value0.OpeningLevel < value1.OpeningLevel)
                    return -1;

                return 1;
            }
        }
    }
}
