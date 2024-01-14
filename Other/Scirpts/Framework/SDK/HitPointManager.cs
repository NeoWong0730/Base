using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lib;
using LitJson;
using UnityEngine;

namespace Framework
{
    public static class HitPointManager
    {

        public class SDKHitPointBaseData
        {
            public string appId;
            public string device_id;
            public string channel;
            public string packagechannel;
            public string platform;//android手机 或者 平台
            public string operator_type;//operator:运营商
            public string app_version;//versionName
                                      // public string IsSimulator;//是否模拟器登录
        }

        public class UnityHitPointBaseData
        {
            public string network;
            public string system_version;
            public string phone_model;
            public uint screen_width;
            public uint pixel_density;
            public string cpu;
            public uint memory_size;
            public string account_type;//快手服务器压测和埋点压测为1，其余时间为0；最终包会对账号类型做验收
            public string test_id;//第一次测试填CCB1，之后填CCB2,CCB3...，公测写为OB
        }



        public static SDKHitPointBaseData GetSDKHitPointBaseData()
        {
            SDKHitPointBaseData baseData = new SDKHitPointBaseData();
            string jsonData = SDKManager.SDKGetHitPointBaseInfo();

            if (SDKManager.GetHaveSDK() && !String.IsNullOrEmpty(jsonData))
            {
                DebugUtil.LogFormat(ELogType.eNone, "HitPointBaseData：{0}", jsonData);
                string[] str = jsonData.Split('|');
                baseData.appId = str[0];
                baseData.device_id = str[1];
                baseData.channel = str[2];
                baseData.packagechannel = str[3];
                baseData.platform = str[4];
                baseData.operator_type = str[5];
                baseData.app_version = str[6];
                //baseData.IsSimulator = str[7];
                //baseData = JsonUtility.FromJson<SDKHitPointBaseData>(jsonData);
            }
            else
            {
#if UNITY_EDITOR && SKIP_SDK_Login
            baseData = new SDKHitPointBaseData();
            baseData.appId = SDKManager.GetAppid();
            baseData.device_id = "testdevice";
            baseData.channel = SkipSDKLogin.Instance.channel;
            baseData.packagechannel = SkipSDKLogin.Instance.channel;
            baseData.platform = "1";
            baseData.operator_type = "testOperator";
            baseData.app_version = SkipSDKLogin.Instance.appVersion;
#else
                baseData = new SDKHitPointBaseData();
                baseData.appId = SDKManager.GetAppid();
                baseData.device_id = "testdevice";
                baseData.channel = "ks";
                baseData.packagechannel = "ks";
                baseData.platform = "1";
                baseData.operator_type = "testOperator";
                baseData.app_version = VersionHelper.AppBuildVersion;

#endif
            }

            return baseData;
        }

        public static void HitPoint(string actionName, object hitPoint = null)
        {
#if UNITY_STANDALONE_WIN && USE_PCSDK
        SDKManager.HitPointPC(actionName, hitPoint);
#else
            SDKManager.HitPoint(actionName, hitPoint);
#endif
        }

        public static UnityHitPointBaseData GetUnityHitPointBaseData()
        {
            //network ：网络 
            string network = NetworkHelper.GetNetworkStatus();

            //app_version:客户端版本
            //string app_version = string.Format("{0}.{1}", VersionHelper.PersistentBuildVersion, VersionHelper.PersistentAssetVersion);

            //system_version:移动终端操作系统版本
            string system_version = SystemInfo.operatingSystem;

            //platform : 移动终端机型
            string phone_model = SystemInfo.deviceModel;

            //screen_width:显示屏宽度
            // Screen.dpi = 多少像素点/1英寸，1英寸=2.54cm，Screen.dpi/2.54 = 多少像素点/1cm,屏宽 = screen_width/(Screen.dpi/2.54) = 多少cm
            double num = Screen.dpi / 2.54;
            uint screen_width = (num == 0) ? (uint)num : (uint)(Screen.width / num);

            //pixel_density：像素密度
            uint pixel_density = (uint)Screen.dpi;

            //cpu：cpu类型|频率|核数
            string cpuleixing = SystemInfo.processorType;
            int cpupinlv = SystemInfo.processorFrequency;
            int cpuheshu = SystemInfo.processorCount;

            //memory_size：内存信息单位M
            uint memory_size = (uint)SystemInfo.systemMemorySize;

            UnityHitPointBaseData unityHitPointBaseData = new UnityHitPointBaseData();
            unityHitPointBaseData.network = network;
            unityHitPointBaseData.system_version = system_version;
            unityHitPointBaseData.phone_model = SystemInfo.deviceModel;
            unityHitPointBaseData.screen_width = screen_width;
            unityHitPointBaseData.pixel_density = pixel_density;
            unityHitPointBaseData.cpu = string.Format("{0}|{1}|{2}", cpuleixing, cpupinlv, cpuheshu);
            unityHitPointBaseData.memory_size = memory_size;
            unityHitPointBaseData.account_type = "0";//压测为1,正常用的为0
            unityHitPointBaseData.test_id = "OB";//需要配置
            return unityHitPointBaseData;
        }
    }
}