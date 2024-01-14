﻿using System;
using System.Diagnostics;
using System.Text;

namespace NWFramework
{
    /// <summary>
    /// 默认游戏框架日志辅助
    /// </summary>
    public class DefaultLogHelper : NWFrameworkLog.ILogHelper
    {
        private enum ELogLevel
        {
            Info,
            Debug,
            Assert,
            Warning,
            Error,
            Exception,
        }

        private const ELogLevel FilterLevel = ELogLevel.Info;
        private static readonly StringBuilder _stringBuilder = new StringBuilder(1024);

        /// <summary>
        /// 打印游戏日志
        /// </summary>
        /// <param name="level">游戏框架日志等级</param>
        /// <param name="message">日志信息</param>
        public void Log(NWFrameworkLogLevel level, object message)
        {
            switch (level)
            {
                case NWFrameworkLogLevel.Debug:
                    LogImp(ELogLevel.Debug, Utility.Text.Format("<color=#888888>{0}</color>", message));
                    break;
                case NWFrameworkLogLevel.Info:
                    LogImp(ELogLevel.Info, message.ToString());
                    break;
                case NWFrameworkLogLevel.Warning:
                    LogImp(ELogLevel.Warning, message.ToString());
                    break;
                case NWFrameworkLogLevel.Error:
                    LogImp(ELogLevel.Error, message.ToString());
                    break;
                case NWFrameworkLogLevel.Fatal:
                    LogImp(ELogLevel.Exception, message.ToString());
                    break;

                default:
                    throw new NWFrameworkException(message.ToString());
            }
        }

        /// <summary>
        /// 获取日志格式
        /// </summary>
        /// <param name="eLogLevel">日志级别</param>
        /// <param name="logString">日志字符</param>
        /// <param name="bColor">是否使用颜色</param>
        /// <returns>StringBuilder</returns>
        private static StringBuilder GetFormatString(ELogLevel eLogLevel, string logString, bool bColor)
        {
            _stringBuilder.Clear();
            switch (eLogLevel)
            {
                case ELogLevel.Debug:
                    _stringBuilder.AppendFormat(
                        bColor
                            ? "<color=gray><b>[Debug] ► </b></color> - <color=#00FF18>{0}</color>"
                            : "<color=#00FF18><b>[Debug] ► </b></color> - {0}",
                        logString);
                    break;
                case ELogLevel.Info:
                    _stringBuilder.AppendFormat(
                        bColor
                            ? "<color=gray><b>[INFO] ► </b></color> - <color=gray>{0}</color>"
                            : "<color=gray><b>[INFO] ► </b></color> - {0}",
                        logString);
                    break;
                case ELogLevel.Assert:
                    _stringBuilder.AppendFormat(
                        bColor
                            ? "<color=#FF00BD><b>[ASSERT] ► </b></color> - <color=green>{0}</color>"
                            : "<color=#FF00BD><b>[ASSERT] ► </b></color> - {0}",
                        logString);
                    break;
                case ELogLevel.Warning:
                    _stringBuilder.AppendFormat(
                        bColor
                            ? "<color=#FF9400><b>[WARNING] ► </b></color> - <color=yellow>{0}</color>"
                            : "<color=#FF9400><b>[WARNING] ► </b></color> - {0}",
                        logString);
                    break;
                case ELogLevel.Error:
                    _stringBuilder.AppendFormat(
                        bColor
                            ? "<color=red><b>[ERROR] ► </b></color> - <color=red>{0}</color>"
                            : "<color=red><b>[ERROR] ► </b></color>- {0}",
                        logString);
                    break;
                case ELogLevel.Exception:
                    _stringBuilder.AppendFormat(
                        bColor
                            ? "<color=red><b>[EXCEPTION] ► </b></color> - <color=red>{0}</color>"
                            : "<color=red><b>[EXCEPTION] ► </b></color> - {0}",
                        logString);
                    break;
            }

            return _stringBuilder;
        }

        private static void LogImp(ELogLevel type, string logString)
        {
            if (type < FilterLevel)
            {
                return;
            }

            StringBuilder infoBuilder = GetFormatString(type, logString, true);
            string logStr = infoBuilder.ToString();

            //获取C#堆栈,Warning以上级别日志才获取堆栈
            if (type == ELogLevel.Error || type == ELogLevel.Warning || type == ELogLevel.Exception)
            {
                StackFrame[] stackFrames = new StackTrace().GetFrames();
                // ReSharper disable once PossibleNullReferenceException
                for (int i = 0; i < stackFrames.Length; i++)
                {
                    StackFrame frame = stackFrames[i];
                    // ReSharper disable once PossibleNullReferenceException
                    string declaringTypeName = frame.GetMethod().DeclaringType.FullName;
                    string methodName = stackFrames[i].GetMethod().Name;

                    infoBuilder.AppendFormat("[{0}::{1}\n", declaringTypeName, methodName);
                }
            }

            switch (type)
            {
                case ELogLevel.Info:
                case ELogLevel.Debug:
                    UnityEngine.Debug.Log(logStr);
                    break;
                case ELogLevel.Warning:
                    UnityEngine.Debug.LogWarning(logStr);
                    break;
                case ELogLevel.Assert:
                    UnityEngine.Debug.LogAssertion(logStr);
                    break;
                case ELogLevel.Error:
                    UnityEngine.Debug.LogError(logStr);
                    break;
                case ELogLevel.Exception:
                    throw new Exception(logStr);
            }
        }
    }
}
