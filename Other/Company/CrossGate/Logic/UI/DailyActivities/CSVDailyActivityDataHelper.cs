using System;
using System.Collections.Generic;

using Table;

namespace Logic
{
    public static class CSVDailyActivityDataHelper
    {
        public static string OpeningTimeStringForUI(this CSVDailyActivity.Data data)
        {
            int count = data.OpeningTime.Count;

            string str = string.Empty;

            // bool isRight = true;

            var nowTime = Sys_Daily.Instance.GetTodayTimeSceond();

            var dailyfunc = Sys_Daily.Instance.GetDailyFunc(data.id);

            bool isNearTime = false;

            var timedata = dailyfunc.GetOpenTimeData();

            if (timedata != null)
            {
                int hours = timedata.OpenTime / 3600;
                int minute = (timedata.OpenTime % 3600)/60;

                str += ((hours == 0 ? "00" : hours.ToString()) + ":" + (minute == 0 ? "00" : minute.ToString()));
            }

            //for (int i = 0; i < count; i++)
            //{
            //    if (i == 0 && data.OpeningTime[i].Count == 1)
            //        return str;
            //    int hours = data.OpeningTime[i][0];
            //    int minute = data.OpeningTime[i][1];

            //    int time = hours * 3600 + minute * 60;

            //    //if (count > 3 && minute != 0)
            //    //    isRight = false;

            //    if (time >= nowTime)
            //    {
            //        str += ((hours == 0 ? "00" : hours.ToString()) + ":" + (minute == 0 ? "00" : minute.ToString()));
            //        isNearTime = true;
            //        break;
            //    }
            //    //if (i < count - 1)
            //    //    str += " , ";
            //}

            //if (!isNearTime)
            //    str = string.Empty;

                //if (count > 3 && isRight == false)
                //    return LanguageHelper.GetTextContent(10706);

                //if (count > 3)
                //    return LanguageHelper.GetTextContent(10707);

                return str;
        }
    }
}
