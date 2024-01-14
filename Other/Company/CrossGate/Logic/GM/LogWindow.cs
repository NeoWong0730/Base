using Lib.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LogWindow : DebugWindowBase
{
    public LogWindow(int id) : base(id) { }
            
    List<ELogType> eLogTypes;
    Vector2 pos = Vector2.zero;
    int rowCount = 3;
    public override void OnAwake()
    {
        Array allLogTypes = Enum.GetValues(typeof(ELogType));
        eLogTypes = new List<ELogType>(allLogTypes.Length);
        for (int i = 0; i < allLogTypes.Length; ++i)
        {
            eLogTypes.Add((ELogType)allLogTypes.GetValue(i));
        }
    }    

    public override void WindowFunction(int id)
    {
        pos = GUILayout.BeginScrollView(pos);
        for (int i = 0; i < eLogTypes.Count; ++i)
        {
            if(i % rowCount == 0)
            {
                GUILayout.BeginHorizontal();
            }

            ELogType logType = eLogTypes[i];
            bool isOpen = DebugUtil.IsOpenLogType((int)logType);
            if (isOpen != GUILayout.Toggle(isOpen, logType.ToString(), GUILayout.Width(300 * fScale)))
            {
                if (isOpen)
                {
                    DebugUtil.CloseLogType((int)logType);
                }
                else
                {
                    DebugUtil.OpenLogType((int)logType);
                }
            }

            if (i % rowCount == rowCount - 1 || i == eLogTypes.Count)
            {
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
    }
}
