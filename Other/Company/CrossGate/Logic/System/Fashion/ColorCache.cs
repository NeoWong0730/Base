using Lib.Core;
using Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

public enum OwnerType
{
    None,     //未拥有
    Limit,    //限时拥有  
    Forever,  //永久拥有
}

public class FashionTimer
{
    public FashionTimer(Action _onTimeout)
    {
        onTimeout = _onTimeout;
    }

    public bool bNeedUpdateTime;
    private bool timeout;
    public bool bTimeOut
    {
        get
        {
            return timeout;
        }
        set
        {
            if (timeout != value)
            {
                timeout = value;
                if (timeout)
                {
                    bNeedUpdateTime = false;
                    onTimeout?.Invoke();
                    timeout = false;
                }
            }
        }
    }
    //public DateTime ExpireDateTime;
    //public TimeSpan RemainTime;
    private Action onTimeout;

    public uint _RemainTime;
    public ulong EndTime;
    public uint configRemainTime;

    public void Update()
    {
        if (!bNeedUpdateTime)
            return;
        UpdateRemainTime();
    }

    private void UpdateRemainTime()
    {
        //uint time = Sys_Time.Instance.GetServerTime();
        //if (Sys_Time.ConvertFromTimeStamp(time) >= ExpireDateTime)
        //    bTimeOut = true;
        //TimeSpan nowTS = new TimeSpan(Sys_Time.ConvertFromTimeStamp(time).Ticks);
        //TimeSpan deleteTS = new TimeSpan(ExpireDateTime.Ticks);
        //RemainTime = deleteTS.Subtract(nowTS).Duration();


        uint time = Sys_Time.Instance.GetServerTime();
        if (EndTime >= time)
        {
            _RemainTime = (uint)(EndTime - time);
        }
        else
        {
            bTimeOut = true;
            _RemainTime = 0;
        }
    }

    public void CalRemainTime(uint _configRemainTime)
    {
        configRemainTime = _configRemainTime;
    }

    public string GetRemainTimeFormat()
    {
        if (_RemainTime < 60)
        {
            return CSVLanguage.Instance.GetConfData(2009557).words;
        }
        string time = LanguageHelper.TimeToString(_RemainTime, LanguageHelper.TimeFormat.Type_2);
        return time;
    }
}

//表示每一个部位的颜色缓存列表 (由于现在新规则，一套时装按照多套染色方案 来处理，所以同一套时装每个部位的颜色缓存列表长度严格相等)
public class ColorCache
{
    public Color32[] color = new Color32[2];  //颜色缓存列表

    public bool hasColorCache;
    
    public Color32 GetColor(int index)
    {
        return color[index];
    }

    public void SetColor(int index, Color32 _color)
    {
        color[index] = _color;
        hasColorCache = true;
    }

    private bool bDirty = false;
    public bool Dirty
    {
        get
        {
            return bDirty;
        }
    }

    public void SetDirty(bool dirty)
    {
        bDirty = dirty;
    }
}
