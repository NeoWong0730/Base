using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
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
            for (int i = 0; i < Urls.Count; ++i)
            {
                rlt[i] = Urls[i].sName;
            }
            return rlt;
        }





        public static string GetDefaultVersionUrl(int channelType)
        {

            if (channelType == 1)
            {
                //��������(����)
                return "http://cg.dirsvr:8009/gm/external/get_hotfix";
            }
            else if (channelType == 2)
            {
                //��������(����2�� legence)
                return "http://61.160.219.96:7888/test2/gm/external/get_hotfix";
            }
            else if (channelType == 3)
            {
                //��������(����4�� edg)
                return "http://61.160.219.96:7888/test4/gm/external/get_hotfix";
            }
            else if (channelType == 4)
            {
                //�������ԣ����������ɣ�
                return "http://61.160.219.96:7888/test1/gm/external/get_hotfix";
            }
            else if (channelType == 5)
            {
                //���֣���ʽ��
                return "https://gamecloud-api.gamed.kuaishou.com/gm/external/get_hotfix";
            }
            else if (channelType == 6)
            {
                //���֣�������
                return "https://gamecloud-api.gamed.kuaishou.com/gm/external/get_hotfix";
            }
            else if (channelType == 7)
            {
                //���� (������)
                return "https://gamecloud-api.gamed.kuaishou.com/gm/external/get_hotfix";
                //return "http://61.160.219.96:7888/test1/gm/external/get_hotfix";
            }
            else if (channelType == 8)
            {
                //���� (��ά���¼��)
                return "https://gamecloud-api.gamed.kuaishou.com/gm/external/get_hotfix";
            }
            else
                return "";
        }

    }
}