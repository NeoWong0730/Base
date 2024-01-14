using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FlagsAttribute]
public enum EChannelFlags
{
    None = 0,
    DisplayName = 1,
}


[System.Serializable]
public class ChannelConfig
{
    public int sId;
    public string sName;
    public string sVersionUrl;
    public EChannelFlags eChannelFlags;
}

public class ChannelConfigs : ScriptableObject
{
    public List<ChannelConfig> Urls = new List<ChannelConfig>();

    public string[] GetDisplay()
    {
        string[] rlt = new string[Urls.Count];
        for(int i = 0; i < Urls.Count; ++i)
        {
            rlt[i] = Urls[i].sName;
        }
        return rlt;
    }

    



    public static string GetDefaultVersionUrl(int channelType)
    {

        if (channelType == 1)
        {
            //内网测试(主干)
            return "http://cg.dirsvr:8009/gm/external/get_hotfix";
        }
        else if (channelType == 2)
        {
            //外网测试(测试2服 legence)
            return "http://61.160.219.96:7888/test2/gm/external/get_hotfix";
        }
        else if (channelType == 3)
        {
            //外网测试(测试4服 edg)
            return "http://61.160.219.96:7888/test4/gm/external/get_hotfix";
        }
        else if (channelType == 4)
        {
            //外网测试（法兰城主干）
            return "http://61.160.219.96:7888/test1/gm/external/get_hotfix";
        }
        else if (channelType == 5)
        {
            //快手（正式）
            return "https://gamecloud-api.gamed.kuaishou.com/gm/external/get_hotfix";
        }
        else if (channelType == 6)
        {
            //快手（渠道）
            return "https://gamecloud-api.gamed.kuaishou.com/gm/external/get_hotfix";
        }
        else if (channelType == 7)
        {
            //快手 (不加密)
            return "https://gamecloud-api.gamed.kuaishou.com/gm/external/get_hotfix";
            //return "http://61.160.219.96:7888/test1/gm/external/get_hotfix";
        }
        else if (channelType == 8)
        {
            //快手 (二维码登录包)
            return "https://gamecloud-api.gamed.kuaishou.com/gm/external/get_hotfix";
        }
        else
            return "";
    }
    
}
