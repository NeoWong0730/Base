using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Lib
{
    public static class ELogType
    {
        public static DebugHeader eNone = new DebugHeader()
        {
            name = "None",
            option = ELogOption.AttachTime | ELogOption.PrintConsole,
        };

        public static DebugHeader eUIState = new DebugHeader()
        {
            name = "UI State",
            option = ELogOption.AttachTime | ELogOption.PrintConsole,
        };

        public static DebugHeader eScene = new DebugHeader()
        {
            name = "Scene",
            option = ELogOption.AttachTime | ELogOption.PrintConsole,
        };

        public static DebugHeader eHeartBeat = new DebugHeader()
        {
            name = "Heart Beat",
            option = ELogOption.AttachTime | ELogOption.PrintConsole,
        };

        public static DebugHeader eNetProcessMSG = new DebugHeader()
        {
            name = "Net Process MSG",
            option = ELogOption.AttachTime | ELogOption.PrintConsole,
        };

        public static DebugHeader eChat = new DebugHeader()
        {
            name = "Chat",
            option = ELogOption.AttachTime | ELogOption.PrintConsole,
        };

        public static DebugHeader eAssets = new DebugHeader()
        {
            name = "Assets",
            option = ELogOption.AttachTime | ELogOption.PrintConsole,
        };

        public static DebugHeader eNetSendMSG = new DebugHeader()
        {
            name = "Net Send MSG",
            option = ELogOption.AttachTime | ELogOption.PrintConsole,
        };

        public static DebugHeader eExecuteTime = new DebugHeader()
        {
            name = "Execute Time",
            option = ELogOption.AttachTime | ELogOption.PrintConsole,
        };

        public static DebugHeader eTable = new DebugHeader()
        {
            name = "Table",
            option = ELogOption.AttachTime | ELogOption.PrintConsole,
        };
    }

    public static class DebugUtil
    {
        private static HashSet<string> eLogTypeFlags = new HashSet<string>();
        private static string sFormat = "[{0}] [{1}] {2}";
        private static string sFormatThread = "[{0}] [T{1}] {2}";

        [Conditional("DEBUG_MODE")]
        public static void Log(DebugHeader header, Func<DebugHeader, int , string> onContent, int useData = 0, Func<DebugHeader, int , bool> onConditional = null)
        {
            if (!IsOpenLogType(header))
                return;

            if (onContent == null)
                return;

            if (onConditional != null && !onConditional(header, useData))
                return;

            if (ELogOption.None != (header.option & ELogOption.AttachTime))
            {
                Debug.LogFormat(sFormatThread, header.name, System.Threading.Thread.CurrentThread.ManagedThreadId, onContent(header, useData));
            }
            else
            {
                Debug.Log(onContent(header, useData));
            }
        }

        [Conditional("DEBUG_MODE")]
        public static void Log(DebugHeader header, string log)
        {
            if (!IsOpenLogType(header))
                return;

            if (ELogOption.None != (header.option & ELogOption.AttachTime))
            {
                Debug.LogFormat(sFormatThread, header.name, System.Threading.Thread.CurrentThread.ManagedThreadId, log);
            }
            else
            {
                Debug.Log(log);
            }
        }

        [Conditional("DEBUG_MODE")]
        public static void LogFormat(DebugHeader header, string format, params object[] args)
        {
            if (!IsOpenLogType(header))
                return;

            if (ELogOption.None != (header.option & ELogOption.AttachTime))
            {
                Debug.LogFormat(sFormatThread, header.name, System.Threading.Thread.CurrentThread.ManagedThreadId, string.Format(format, args));
            }
            else
            {
                Debug.LogFormat(format, args);
            }
        }

        [Conditional("DEBUG_MODE")]
        public static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        [Conditional("DEBUG_MODE")]
        public static void LogWarningFormat(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }

        public static void LogError(string message)
        {
            Debug.LogError(message);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }

        public static void LogException(Exception exception, UnityEngine.Object context)
        {
            Debug.LogException(exception, context);
        }

        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }

        [Conditional("DEBUG_MODE")]
        public static void OpenLogType(DebugHeader header)
        {
            if (!eLogTypeFlags.Contains(header.name))
            {
                eLogTypeFlags.Add(header.name);
            }
            Save();
        }

        [Conditional("DEBUG_MODE")]
        public static void CloseLogType(DebugHeader header)
        {
            eLogTypeFlags.Remove(header.name);
            Save();
        }

        public static bool IsOpenLogType(DebugHeader header)
        {
            return eLogTypeFlags.Contains(header.name);
        }

        [Conditional("DEBUG_MODE")]
        public static void Save()
        {
            string[] array = new string[eLogTypeFlags.Count];
            eLogTypeFlags.CopyTo(array);
            PlayerPrefs.SetString("LogFlags", string.Join("|", array));
        }

        [Conditional("DEBUG_MODE")]
        public static void Load()
        {
            if (PlayerPrefs.HasKey("LogFlags"))
            {
                string stype = PlayerPrefs.GetString("LogFlags");
                if (!string.IsNullOrWhiteSpace(stype))
                {
                    string[] types = stype.Split("|");
                    for (int i = 0; i < types.Length; ++i)
                    {
                        string flag = types[i];
                        if (!eLogTypeFlags.Contains(flag))
                        {
                            eLogTypeFlags.Add(flag);
                        }
                    }
                }
            }
        }

        [Conditional("DEBUG_MODE")]
        public static void LogTimeCost(DebugHeader header, string content, ref float timePoint, int limit = 0)
        {
            float currentTime = Time.realtimeSinceStartup;
            float dt = (int)((currentTime - timePoint) * 1000);
            if (dt >= limit)
            {
                LogFormat(header, "{0} spend {1}ms", content, dt.ToString());
            }
            timePoint = currentTime;
        }
    }
}
