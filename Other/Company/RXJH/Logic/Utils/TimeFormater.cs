using System;
using Table;

namespace Logic {
    public static class TimeFormater {
        public enum ETimeFormat {
            /// <summary> xx:xx:xx </summary>
            Type_1,

            /// <summary> xx天xx小时xx分钟后失效 </summary>
            Type_2,

            /// <summary> 在线，刚刚,xx小时前，xx天前，xx周前 </summary>
            Type_3,

            /// <summary> xx天xx小时,xx小时xx分钟,xx分钟xx秒 </summary>
            Type_4,

            /// <summary> 大约n分钟，小于一分钟 </summary>
            Type_5,

            /// <summary> xx小时xx分钟 xx分钟 </summary>
            Type_6,

            /// <summary> xx年xx月xx日xx时xx分 </summary>
            Type_7,

            /// <summary> xx月xx日xx时xx分 </summary>
            Type_8,

            /// <summary> xx天xx小时xx分钟xx秒 </summary>
            Type_9,

            /// <summary> xx:xx:xx</summary>
            Type_10,

            /// <summary> x秒前，x分钟前,x小时前,x天 </summary>
            Type_11,

            /// <summary> 剩余时间：{0}天{0}小时{0}分 </summary>
            Type_12,

            /// <summary> xx/xx/xx </summary>
            Type_13,
        }

        /// <param name="time">单位秒</param>
        /// <param name="format">格式类型</param>
        public static string TimeToString(uint time, ETimeFormat format) {
            switch (format) {
                case ETimeFormat.Type_1: {
                    uint hour = time / 3600;
                    uint minute = (time % 3600) / 60;
                    uint second = time % 60;
                    return string.Format("{0}:{1}:{2}", hour.ToString("D2"), minute.ToString("D2"), second.ToString("D2"));
                }
                case ETimeFormat.Type_2: {
                    uint day = time / 86400;
                    uint hour = (time % 86400) / 3600;
                    uint minute = (time % 3600) / 60;
                    if (day > 0) {
                        //xx天xx小时xx分钟后失效
                        return LanguageHelper.GetContent<CSVLanguage>(2007261, day.ToString(), hour.ToString(), minute.ToString());
                    }
                    else if (hour > 0) {
                        //xx小时xx分钟后失效
                        return LanguageHelper.GetContent<CSVLanguage>(2007262, hour.ToString(), minute.ToString());
                    }
                    else {
                        //xx分钟后失效
                        return LanguageHelper.GetContent<CSVLanguage>(2007263, minute.ToString());
                    }
                }
                case ETimeFormat.Type_3: {
                    uint minute = (time % 3600) / 60;
                    uint hour = (time % 86400) / 3600;
                    uint day = time / 86400;
                    uint week = day / 7;
                    if (week > 0) {
                        //xx周前
                        return LanguageHelper.GetContent<CSVLanguage>(10094, week.ToString());
                    }
                    else if (day > 0) {
                        //xx天前
                        return LanguageHelper.GetContent<CSVLanguage>(10093, day.ToString());
                    }
                    else if (hour > 0) {
                        //xx小时前
                        return LanguageHelper.GetContent<CSVLanguage>(10092, hour.ToString());
                    }
                    else if (time != 0) {
                        //刚刚
                        return LanguageHelper.GetContent<CSVLanguage>(10091);
                    }
                    else {
                        //在线
                        return LanguageHelper.GetContent<CSVLanguage>(10672);
                    }
                }
                case ETimeFormat.Type_4: {
                    uint day = time / 86400;
                    uint hour = (time % 86400) / 3600;
                    uint minute = (time % 3600) / 60;
                    uint second = time % 60;

                    if (day > 0) {
                        //xx天xx小时
                        return LanguageHelper.GetContent<CSVLanguage>(10593, day.ToString(), hour.ToString());
                    }
                    else if (hour > 0) {
                        //xx小时xx分钟
                        return LanguageHelper.GetContent<CSVLanguage>(10594, hour.ToString(), minute.ToString());
                    }
                    else {
                        //xx分钟xx秒
                        return LanguageHelper.GetContent<CSVLanguage>(10595, minute.ToString(), second.ToString());
                    }
                }
                case ETimeFormat.Type_5: {
                    uint day = time / 86400;
                    uint hour = (time % 86400) / 3600;
                    uint minute = (time % 3600) / 60;
                    uint second = time % 60;

                    if (hour <= 0) {
                        if (minute < 1) {
                            return LanguageHelper.GetContent<CSVLanguage>(10871);
                        }
                        else {
                            return LanguageHelper.GetContent<CSVLanguage>(10870, minute.ToString());
                        }
                    }
                    else {
                        return LanguageHelper.GetContent<CSVLanguage>(10870, minute.ToString());
                    }
                }
                case ETimeFormat.Type_6: {
                    uint hour = time / 3600;
                    uint minute = (time % 3600) / 60;
                    if (hour > 0) {
                        //xx小时xx分钟
                        return LanguageHelper.GetContent<CSVLanguage>(20001, hour.ToString(), minute.ToString());
                    }
                    else {
                        //xx分钟
                        return LanguageHelper.GetContent<CSVLanguage>(20002, minute.ToString());
                    }
                }
                case ETimeFormat.Type_7: {
                    // DateTime dateTime = Sys_Time.ConvertToLocalTime(time);
                    // return string.Format(CSVLanguageHelper.GetContent(2004113), dateTime.Year, dateTime.Month.ToString(gTimeFormat_2), dateTime.Day.ToString(gTimeFormat_2),
                    //     dateTime.Hour.ToString(gTimeFormat_2), dateTime.Minute.ToString(gTimeFormat_2));
                    break;
                }
                case ETimeFormat.Type_8: {
                    //待修改
                    // DateTime dateTime = Sys_Time.ConvertToLocalTime(time);
                    // return string.Format(CSVLanguageHelper.GetContent(11903), dateTime.Month.ToString(gTimeFormat_2), dateTime.Day.ToString(gTimeFormat_2), dateTime.Hour.ToString(gTimeFormat_2),
                    //     dateTime.Minute.ToString(gTimeFormat_2));
                    break;
                }
                case ETimeFormat.Type_9: {
                    uint day = time / 86400;
                    uint hour = (time % 86400) / 3600;
                    uint minute = (time % 3600) / 60;
                    uint second = time % 60;
                    return LanguageHelper.GetContent<CSVLanguage>(2011416, day.ToString(), hour.ToString(), minute.ToString(), second.ToString());
                }
                case ETimeFormat.Type_10: {
                    uint hour = time / 3600;
                    uint minute = (time % 3600) / 60;
                    uint second = time % 60;
                    return LanguageHelper.GetContent<CSVLanguage>(590002124, hour.ToString(), minute.ToString(), second.ToString());
                }
                case ETimeFormat.Type_11: {
                    uint minute = (time % 3600) / 60;
                    uint hour = (time % 86400) / 3600;
                    uint day = time / 86400;
                    uint second = time % 60;
                    if (day > 0) {
                        //xx天前
                        return LanguageHelper.GetContent<CSVLanguage>(2024717, day.ToString());
                    }
                    else if (hour > 0) {
                        //xx小时前
                        return LanguageHelper.GetContent<CSVLanguage>(2024716, hour.ToString());
                    }
                    else if (minute > 0) {
                        //xx分钟前
                        return LanguageHelper.GetContent<CSVLanguage>(2024715, minute.ToString());
                    }
                    else {
                        //xx秒前
                        return LanguageHelper.GetContent<CSVLanguage>(2024714, second.ToString());
                    }
                }
                case ETimeFormat.Type_12: {
                    uint day = time / 86400;
                    uint hour = (time % 86400) / 3600;
                    uint minute = (time % 3600) / 60;
                    return LanguageHelper.GetContent<CSVLanguage>(2025731, day.ToString(), hour.ToString(), minute.ToString());
                }
                case ETimeFormat.Type_13: {
                    // DateTime dateTime = Sys_Time.ConvertToLocalTime(time);
                    // return string.Format(CSVLanguageHelper.GetContent(13548), dateTime.Year, dateTime.Month.ToString(gTimeFormat_2), dateTime.Day.ToString(gTimeFormat_2));
                    break;
                }
                default: {
                    return string.Empty;
                }
            }

            return string.Empty;
        }
    }
}