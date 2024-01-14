using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLogManager
{
    [Flags]
    public enum DLogEnum
    {
        None = 0,
        Combat = 1 << 0,
        CombatBehave = 1 << 1,
        UIModelShowWorkStream = 1 << 2,
        WorkStream = 1 << 3,
        CombatNetMsg = 1 << 4,
    }

    public class DLogInfo
    {
        public DLogEnum DLogEnum;
        public DateTime SystemTime;
        public float RealtimeSinceStartup;
        public int FrameCount;
        public string LogStr;
    }
    
    public static List<DLogInfo> m_DLogList;

    public static int m_DLogEnumFilterVal = -1;
    public static int m_DLogEnumNeedVal = -1;

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    private static void Log(DLogEnum dLogEnum, string logStr)
    {
        if (m_DLogEnumNeedVal < 0)
        {
            m_DLogEnumNeedVal = 0;
            m_DLogEnumNeedVal = ~0;
        }

        if (m_DLogEnumFilterVal < 0)
        {
            m_DLogEnumFilterVal = 0;
            m_DLogEnumFilterVal |= (int)DLogEnum.Combat;
            m_DLogEnumFilterVal |= (int)DLogEnum.UIModelShowWorkStream;
            m_DLogEnumFilterVal |= (int)DLogEnum.WorkStream;
            m_DLogEnumFilterVal |= (int)DLogEnum.CombatNetMsg;
        }

        if ((m_DLogEnumNeedVal & (int)dLogEnum) == 0)
            return;
        
        if (m_DLogList == null)
            m_DLogList = new List<DLogInfo>();

        DLogInfo dli = null;
        if (m_DLogList.Count >= 511)
        {
            dli = m_DLogList[0];
            m_DLogList.RemoveAt(0);
        }
        else
        {
            dli = new DLogInfo();
        }

        dli.DLogEnum = dLogEnum;
        dli.SystemTime = DateTime.Now;
        dli.RealtimeSinceStartup = Time.realtimeSinceStartup;
        dli.FrameCount = Time.frameCount;
        dli.LogStr = logStr;

        m_DLogList.Add(dli);
    }

#if DEBUG_MODE
    public static string GetLastLog(int dLogEnumInt = 0)
    {
        if (m_DLogList == null || m_DLogList.Count <= 0)
            return null;

        if (dLogEnumInt > 0)
        {
            for (int i = m_DLogList.Count - 1; i > -1; --i)
            {
                var log = m_DLogList[i];
                if (log == null)
                    continue;

                if ((dLogEnumInt & (int)log.DLogEnum) > 0)
                    return log.LogStr;
            }
        }

        return m_DLogList[m_DLogList.Count - 1].LogStr;
    }
#endif

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void Log(Lib.Core.ELogType eLogType, string logStr)
    {
        DLogEnum dLogEnum = DLogEnum.None;
        if (eLogType == Lib.Core.ELogType.eCombat)
            dLogEnum = DLogEnum.Combat;
        else if(eLogType == Lib.Core.ELogType.eCombatBehave)
            dLogEnum = DLogEnum.CombatBehave;
        else if (eLogType == Lib.Core.ELogType.eUIModelShowWorkStream)
            dLogEnum = DLogEnum.UIModelShowWorkStream;
        else if (eLogType == Lib.Core.ELogType.eWorkStream)
            dLogEnum = DLogEnum.WorkStream;

        Log(dLogEnum, logStr);

        Lib.Core.DebugUtil.Log(eLogType, logStr);
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void LogDebugError(string errorStr)
    {
        Lib.Core.DebugUtil.LogError(errorStr);
    }
}