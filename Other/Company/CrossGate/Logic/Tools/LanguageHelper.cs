using Framework;
using Lib.Core;
using Logic.Core;
using System;
using Table;
using UnityEngine;

namespace Logic
{
    public static class LanguageHelper
    {
        private static string sFormat = "<color=#{0}>{1}</color>";
        public static string gTimeFormat_1 = "{0}:{1}:{2}";
        public static string gTimeFormat_2 = "D2";

        public enum TimeFormat
        {
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

        #region 原始语言表
        public static string GetTextContent(CSVLanguage.Data data)
        {
            if (data == null)
                return null;

            return data.words;
        }
        public static string GetTextContent(uint languageID)
        {
            CSVLanguage.Data data = CSVLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return data.words;
        }

        public static string GetTextContent(CSVLanguage.Data data, string param)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetTextContent(uint languageID, string param)
        {
            CSVLanguage.Data data = CSVLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetTextContent(data, param);
        }

        public static string GetTextContent(CSVLanguage.Data data, string param, string param1)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param, param1);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetTextContent(uint languageID, string param, string param1)
        {
            CSVLanguage.Data data = CSVLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetTextContent(data, param, param1);
        }

        public static string GetTextContent(CSVLanguage.Data data, string param, string param1, string param2)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param, param1, param2);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetTextContent(uint languageID, string param, string param1, string param2)
        {
            CSVLanguage.Data data = CSVLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetTextContent(data, param, param1, param2);
        }

        public static string GetTextContent(CSVLanguage.Data data, params string[] param)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetTextContent(uint languageID, params string[] param)
        {
            CSVLanguage.Data data = CSVLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetTextContent(data, param);
        }
        #endregion

        #region 任务语言表
        public static string GetTaskTextContent(CSVTaskLanguage.Data data)
        {
            if (data == null)
                return null;

            return data.words;
        }
        public static string GetTaskTextContent(uint languageID)
        {
            CSVTaskLanguage.Data data = CSVTaskLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVTaskLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return data.words;
        }

        public static string GetTaskTextContent(CSVTaskLanguage.Data data, string param)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVTaskLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetTaskTextContent(uint languageID, string param)
        {
            CSVTaskLanguage.Data data = CSVTaskLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVTaskLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetTaskTextContent(data, param);
        }

        public static string GetTaskTextContent(CSVTaskLanguage.Data data, string param, string param1)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param, param1);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVTaskLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetTaskTextContent(uint languageID, string param, string param1)
        {
            CSVTaskLanguage.Data data = CSVTaskLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVTaskLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetTaskTextContent(data, param, param1);
        }

        public static string GetTaskTextContent(CSVTaskLanguage.Data data, string param, string param1, string param2)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param, param1, param2);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVTaskLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetTaskTextContent(uint languageID, string param, string param1, string param2)
        {
            CSVTaskLanguage.Data data = CSVTaskLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVTaskLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetTaskTextContent(data, param, param1, param2);
        }

        public static string GetTaskTextContent(CSVTaskLanguage.Data data, params string[] param)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVTaskLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetTaskTextContent(uint languageID, params string[] param)
        {
            CSVTaskLanguage.Data data = CSVTaskLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVTaskLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetTaskTextContent(data, param);
        }
        #endregion

        #region NPC语言表

        public static string GetNpcTextContent(CSVNpcLanguage.Data data)
        {
            if (data == null)
                return null;

            return data.words;
        }
        public static string GetNpcTextContent(uint languageID)
        {
            CSVNpcLanguage.Data data = CSVNpcLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVNpcLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return data.words;
        }

        public static string GetNpcTextContent(CSVNpcLanguage.Data data, string param)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVNpcLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetNpcTextContent(uint languageID, string param)
        {
            CSVNpcLanguage.Data data = CSVNpcLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVNpcLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetNpcTextContent(data, param);
        }

        public static string GetNpcTextContent(CSVNpcLanguage.Data data, string param, string param1)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param, param1);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVNpcLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetNpcTextContent(uint languageID, string param, string param1)
        {
            CSVNpcLanguage.Data data = CSVNpcLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVNpcLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetNpcTextContent(data, param, param1);
        }

        public static string GetNpcTextContent(CSVNpcLanguage.Data data, string param, string param1, string param2)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param, param1, param2);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVNpcLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetNpcTextContent(uint languageID, string param, string param1, string param2)
        {
            CSVNpcLanguage.Data data = CSVNpcLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVNpcLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetNpcTextContent(data, param, param1, param2);
        }

        public static string GetNpcTextContent(CSVNpcLanguage.Data data, params string[] param)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVNpcLanguage " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetNpcTextContent(uint languageID, params string[] param)
        {
            CSVNpcLanguage.Data data = CSVNpcLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVNpcLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetNpcTextContent(data, param);
        }

        #endregion

        #region 错误码表
        public static string GetErrorCodeContent(CSVErrorCode.Data data)
        {
            if (data == null)
                return null;

            return data.words;
        }
        public static string GetErrorCodeContent(uint languageID)
        {
            CSVErrorCode.Data data = CSVErrorCode.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVErrorCode.Data not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return data.words;
        }

        public static string GetErrorCodeContent(CSVErrorCode.Data data, string param)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVErrorCode.Data " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetErrorCodeContent(uint languageID, string param)
        {
            CSVErrorCode.Data data = CSVErrorCode.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVErrorCode.Data not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetErrorCodeContent(data, param);
        }

        public static string GetErrorCodeContent(CSVErrorCode.Data data, string param, string param1)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param, param1);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVErrorCode.Data " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetErrorCodeContent(uint languageID, string param, string param1)
        {
            CSVErrorCode.Data data = CSVErrorCode.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVErrorCode.Data not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetErrorCodeContent(data, param, param1);
        }

        public static string GetErrorCodeContent(CSVErrorCode.Data data, string param, string param1, string param2)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param, param1, param2);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVErrorCode.Data " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetErrorCodeContent(uint languageID, string param, string param1, string param2)
        {
            CSVErrorCode.Data data = CSVErrorCode.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVErrorCode.Data not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetErrorCodeContent(data, param, param1, param2);
        }

        public static string GetErrorCodeContent(CSVErrorCode.Data data, params string[] param)
        {
            if (data == null)
                return null;

            try
            {
                string val = string.Format(data.words, param);
                return val;
            }
            catch (System.Exception e)
            {
                DebugUtil.LogException(e);
#if DEBUG_MODE
                return "CSVErrorCode.Data " + data.id.ToString();
#else
                return null;
#endif
            }
        }
        public static string GetErrorCodeContent(uint languageID, params string[] param)
        {
            CSVErrorCode.Data data = CSVErrorCode.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVErrorCode.Data not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return GetErrorCodeContent(data, param);
        }
        #endregion

        #region 成就语言表
        public static string GetAchievementContent(CSVAchievementLanguage.Data data, uint type = 0)
        {
            if (data == null)
                return null;

            return type == 0 ? data.words : data.text;
        }
        /// <summary>
        /// 获取成就文本内容
        /// </summary>
        /// <param name="languageID"></param>
        /// <param name="type">0：CSVAchievement { Achievement_Title || SubClassType || CollectInfo }
        ///                       CSVAchievementType { MainClassTest }  
        ///                       CSVAchievementLevel { Level_Test }  
        ///                    1：CSVAchievement { Task_Test } 
        ///                    </param>
        /// <returns></returns>
        public static string GetAchievementContent(uint languageID, uint type = 0)
        {
            CSVAchievementLanguage.Data data = CSVAchievementLanguage.Instance.GetConfData(languageID);
            if (data == null)
            {
                DebugUtil.LogErrorFormat("CSVAchievementLanguage not find id = {0}", languageID);
#if DEBUG_MODE
                return languageID.ToString();
#else
                return null;
#endif
            }

            return type == 0 ? data.words : data.text;
        }
        #endregion
        public static CSVWordStyle.Data GetTextStyle(uint styleID)
        {
            CSVWordStyle.Data data = CSVWordStyle.Instance.GetConfData(styleID);
            return data;
        }

        public static string GetDialogueLanguageColorWords(uint dialogueLanguageID)
        {
            CSVDialogueLanguage.Data languageData = CSVDialogueLanguage.Instance.GetConfData(dialogueLanguageID);
            if (languageData == null)
                return null;

            string content = languageData.words;
            return content;
        }

        public static string GetTaskLanguageColorWords(uint taskLanguageID)
        {
            CSVTaskLanguage.Data languageData = CSVTaskLanguage.Instance.GetConfData(taskLanguageID);
            if (languageData == null)
                return null;

            string content = languageData.words;
            return content;
        }

        public static string GetLanguageColorWordsFormat(uint languageID)
        {
            CSVLanguage.Data languageData = CSVLanguage.Instance.GetConfData(languageID);
            if (languageData == null)
                return null;

            string content = languageData.words;

            CSVWordStyle.Data stytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            if (stytleData == null)
                return content;

            Color color = stytleData.FontColor;
            string c = ColorUtility.ToHtmlStringRGB(color);
            return string.Format(sFormat, c, content);
        }

        public static string GetLanguageColorWordsFormat(string content, uint langulgeColorID)
        {
            CSVLanguage.Data languageData = CSVLanguage.Instance.GetConfData(langulgeColorID);
            if (languageData == null)
            {
                DebugUtil.LogErrorFormat("GetLanguageColorWordsFormat not find {0}", langulgeColorID.ToString());
                return content;
            }

            CSVWordStyle.Data stytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            if (stytleData == null)
            {
                DebugUtil.LogErrorFormat("CSVLanguageData.wordStytle is Null id= {0}", langulgeColorID.ToString());
                return content;
            }

            Color color = stytleData.FontColor;
            string c = ColorUtility.ToHtmlStringRGB(color);
            return string.Format(sFormat, c, content);
        }
        /// <summary>
        /// 格式化时间文本
        /// </summary>
        /// <param name="time">单位秒</param>
        /// <param name="format">格式类型</param>
        /// <returns></returns>
        public static string TimeToString(uint time, TimeFormat format)
        {
            switch (format)
            {
                case TimeFormat.Type_1:
                    {
                        uint hour = time / 3600;
                        uint minute = (time % 3600) / 60;
                        uint second = time % 60;
                        return string.Format(gTimeFormat_1, hour.ToString(gTimeFormat_2), minute.ToString(gTimeFormat_2), second.ToString(gTimeFormat_2));
                    }
                case TimeFormat.Type_2:
                    {
                        uint day = time / 86400;
                        uint hour = (time % 86400) / 3600;
                        uint minute = (time % 3600) / 60;
                        if (day > 0)
                        {
                            //xx天xx小时xx分钟后失效
                            return LanguageHelper.GetTextContent(2007261, day.ToString(), hour.ToString(), minute.ToString());
                        }
                        else if (hour > 0)
                        {
                            //xx小时xx分钟后失效
                            return LanguageHelper.GetTextContent(2007262, hour.ToString(), minute.ToString());
                        }
                        else
                        {
                            //xx分钟后失效
                            return LanguageHelper.GetTextContent(2007263, minute.ToString());
                        }
                    }
                case TimeFormat.Type_3:
                    {
                        uint minute = (time % 3600) / 60;
                        uint hour = (time % 86400) / 3600;
                        uint day = time / 86400;
                        uint week = day / 7;
                        if (week > 0)
                        {
                            //xx周前
                            return LanguageHelper.GetTextContent(10094, week.ToString());
                        }
                        else if (day > 0)
                        {
                            //xx天前
                            return LanguageHelper.GetTextContent(10093, day.ToString());
                        }
                        else if (hour > 0)
                        {
                            //xx小时前
                            return LanguageHelper.GetTextContent(10092, hour.ToString());
                        }
                        else if (time != 0)
                        {
                            //刚刚
                            return LanguageHelper.GetTextContent(10091);
                        }
                        else
                        {
                            //在线
                            return LanguageHelper.GetTextContent(10672);
                        }
                    }
                case TimeFormat.Type_4:
                    {
                        uint day = time / 86400;
                        uint hour = (time % 86400) / 3600;
                        uint minute = (time % 3600) / 60;
                        uint second = time % 60;

                        if (day > 0)
                        {
                            //xx天xx小时
                            return LanguageHelper.GetTextContent(10593, day.ToString(), hour.ToString());
                        }
                        else if (hour > 0)
                        {
                            //xx小时xx分钟
                            return LanguageHelper.GetTextContent(10594, hour.ToString(), minute.ToString());
                        }
                        else
                        {
                            //xx分钟xx秒
                            return LanguageHelper.GetTextContent(10595, minute.ToString(), second.ToString());
                        }
                    }
                case TimeFormat.Type_5:
                    {
                        uint day = time / 86400;
                        uint hour = (time % 86400) / 3600;
                        uint minute = (time % 3600) / 60;
                        uint second = time % 60;

                        if (hour <= 0)
                        {
                            if (minute < 1)
                            {
                                return LanguageHelper.GetTextContent(10871);
                            }
                            else
                            {
                                return LanguageHelper.GetTextContent(10870, minute.ToString());
                            }
                        }
                        else
                        {
                            return LanguageHelper.GetTextContent(10870, minute.ToString());
                        }
                    }
                case TimeFormat.Type_6:
                    {
                        uint hour = time / 3600;
                        uint minute = (time % 3600) / 60;
                        if (hour > 0)
                        {
                            //xx小时xx分钟
                            return LanguageHelper.GetTextContent(20001, hour.ToString(), minute.ToString());
                        }
                        else
                        {
                            //xx分钟
                            return LanguageHelper.GetTextContent(20002, minute.ToString());
                        }
                    }
                case TimeFormat.Type_7:
                    {
                        DateTime dateTime = Sys_Time.ConvertToLocalTime(time);
                        return string.Format(LanguageHelper.GetTextContent(2004113), dateTime.Year, dateTime.Month.ToString(gTimeFormat_2), dateTime.Day.ToString(gTimeFormat_2), dateTime.Hour.ToString(gTimeFormat_2), dateTime.Minute.ToString(gTimeFormat_2));
                    }
                case TimeFormat.Type_8:
                    {
                        //待修改
                        DateTime dateTime = Sys_Time.ConvertToLocalTime(time);
                        return string.Format(LanguageHelper.GetTextContent(11903), dateTime.Month.ToString(gTimeFormat_2), dateTime.Day.ToString(gTimeFormat_2), dateTime.Hour.ToString(gTimeFormat_2), dateTime.Minute.ToString(gTimeFormat_2));
                    }
                case TimeFormat.Type_9:
                    {
                        uint day = time / 86400;
                        uint hour = (time % 86400) / 3600;
                        uint minute = (time % 3600) / 60;
                        uint second = time % 60;
                        return LanguageHelper.GetTextContent(2011416, day.ToString(), hour.ToString(), minute.ToString(), second.ToString());
                    }
                case TimeFormat.Type_10:
                    {
                        uint hour = time / 3600;
                        uint minute = (time % 3600) / 60;
                        uint second = time % 60;
                        return LanguageHelper.GetTextContent(590002124, hour.ToString(), minute.ToString(), second.ToString());
                    }
                case TimeFormat.Type_11:
                    {
                        uint minute = (time % 3600) / 60;
                        uint hour = (time % 86400) / 3600;
                        uint day = time / 86400;
                        uint second = time % 60;
                        if (day > 0)
                        {
                            //xx天前
                            return LanguageHelper.GetTextContent(2024717, day.ToString());
                        }
                        else if (hour > 0)
                        {
                            //xx小时前
                            return LanguageHelper.GetTextContent(2024716, hour.ToString());
                        }
                        else if (minute > 0)
                        {
                            //xx分钟前
                            return LanguageHelper.GetTextContent(2024715, minute.ToString());
                        }
                        else
                        {
                            //xx秒前
                            return LanguageHelper.GetTextContent(2024714, second.ToString());
                        }
                    }
                case TimeFormat.Type_12:
                    {
                        uint day = time / 86400;
                        uint hour = (time % 86400) / 3600;
                        uint minute = (time % 3600) / 60;
                        return LanguageHelper.GetTextContent(2025731, day.ToString(), hour.ToString(), minute.ToString());
                    }
                case TimeFormat.Type_13:
                    {
                        DateTime dateTime = Sys_Time.ConvertToLocalTime(time);
                        return string.Format(LanguageHelper.GetTextContent(13548), dateTime.Year, dateTime.Month.ToString(gTimeFormat_2), dateTime.Day.ToString(gTimeFormat_2));
                    }
                default:
                    return string.Empty;
            }
        }
    }
}