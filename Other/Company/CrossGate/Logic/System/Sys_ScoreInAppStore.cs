#if UNITY_IOS || APP_SCORE
using UnityEngine;
using System.Collections;
using Logic.Core;
using Table;
using Lib.Core;
using Net;
using Packet;

namespace Logic
{
    public class Sys_ScoreInAppStore : SystemModuleBase<Sys_ScoreInAppStore>
    {
        public enum ETriggerType
        {
            None,
            /// <summary>升级</summary>
            Enum_UpLevel,
            /// <summary>活跃度奖励</summary>
            Enum_DailyActivityReward
        }
        ulong lastScoreTime = 0;
        uint levelLimite, getRewardID, coolTime;
        const int totlePercentage = 100;
#region 系统函数
        public override void Init()
        {
            InitData();
            ProcessEvents(true);

            EventDispatcher.Instance.AddEventListener((ushort)CmdRole.AppScoreReq, (ushort)CmdRole.AppScoreNtf, this.AppScoreNtf, CmdRoleAppScoreNtf.Parser);
        }

        public override void Dispose()
        {
            ProcessEvents(false);
        }

        private void ProcessEvents(bool toRegister)
        {
            Sys_Daily.Instance.eventEmitter.Handle<uint>(Sys_Daily.EEvents.RewardAck, OnDailyRewardAck, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnUpdatePlayerLevel, toRegister);
            //Sys_Role.Instance.eventEmitter.Handle<uint>(Sys_Role.EEvents.OnUpdateAppScoreTime, OnUpdateAppScoreTime, toRegister);
        }
#endregion

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            levelLimite = uint.Parse(CSVParam.Instance.GetConfData(1392).str_value);
            getRewardID = uint.Parse(CSVParam.Instance.GetConfData(1393).str_value);
            coolTime = uint.Parse(CSVParam.Instance.GetConfData(1394).str_value);
        }

#region Req,Ntf
        private void AppScoreReq()
        {
            CmdRoleAppScoreReq req = new CmdRoleAppScoreReq();
            NetClient.Instance.SendMessage((ushort)CmdRole.AppScoreReq, req);
            lastScoreTime = Sys_Time.Instance.GetServerTime();
            DebugUtil.Log(ELogType.eNone, "AppScoreReq");
        }

        private void AppScoreNtf(NetMsg msg)
        {
            CmdRoleAppScoreNtf ntf = NetMsgUtil.Deserialize<CmdRoleAppScoreNtf>(CmdRoleAppScoreNtf.Parser, msg);
            lastScoreTime = ntf.LastTime;
            DebugUtil.LogFormat(ELogType.eNone, "AppScoreNtf:{0}--{1}", lastScoreTime, ntf.LastTime);
        }

        private void OnUpdateAppScoreTime(uint lastTime)
        {
            lastScoreTime = lastTime;
            DebugUtil.LogFormat(ELogType.eNone,"OnUpdateAppScoreTime:{0}--{1}", lastScoreTime, lastTime);
        }
#endregion

        private void OnUpdatePlayerLevel()
        {
            if (CheckFunctionOpen())
            {
                TriggerCondition(ETriggerType.Enum_UpLevel);
            }
        }

        private void OnDailyRewardAck(uint id)
        {
            DebugUtil.LogFormat(ELogType.eNone,"OnDailyRewardAck:{0}", id);
            if (id == getRewardID && CheckFunctionOpen())
            {
                TriggerCondition(ETriggerType.Enum_DailyActivityReward);
            }
        }

        /// <summary>
        /// 条件触发
        /// </summary>
        private void TriggerCondition(ETriggerType eTriggerType)
        {
            int percentage = 0;
            switch (eTriggerType)
            {
                case ETriggerType.Enum_UpLevel:
                    percentage = int.Parse(CSVParam.Instance.GetConfData(1395).str_value);
                    break;
                case ETriggerType.Enum_DailyActivityReward:
                    percentage = int.Parse(CSVParam.Instance.GetConfData(1396).str_value);
                    break;
                default:
                    break;
            }

            int randomValue = Random.Range(1, totlePercentage + 1);
            //DebugUtil.LogFormat(ELogType.eNone, "TriggerCondition random:{0}", randomValue);
            if (!CheckFunctionOpen()
                || randomValue > percentage
                || !IsGetTime())
                return;
            bool scoreOpen = SDKManager.SDKGetSCoreInAppStore();
            if (scoreOpen)
                AppScoreReq();
#if APP_SCORE && UNITY_EDITOR
            if (!SDKManager.GetHaveSDK())
            {
                AppScoreReq();
                Sys_Hint.Instance.PushContent_Normal("SDK_InAppStore");
            }    
#endif
            DebugUtil.Log(ELogType.eNone, "SDKGetSCoreInAppStore");
        }

        /// <summary>
        /// 条件--评分冷却时间
        /// </summary>
        /// <returns></returns>
        private bool IsGetTime()
        {
            DebugUtil.Log(ELogType.eNone, "IsGetTime");
            if (lastScoreTime == 0)
                return true;
            var serverTime = Sys_Time.Instance.GetServerTime();
            ulong lastTime = lastScoreTime - lastScoreTime % 86400;
            DebugUtil.LogFormat(ELogType.eNone, "lastTime:{0},serverTime:{1},coolTime:{2}", lastTime, serverTime, (serverTime - lastTime) / 86400);
            if ((serverTime - lastTime) / 86400 >= coolTime)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 条件--评分是否开启，以及等级限制
        /// </summary>
        /// <returns></returns>
        private bool CheckFunctionOpen()
        {
            DebugUtil.Log(ELogType.eNone, "CheckFunctionOpen");
            bool isOpen = SDKManager.GetThirdSdkStatus(SDKManager.EThirdSdkType.ScoreInApp);
            bool isGetLevelLimite = Sys_Role.Instance.Role.Level > levelLimite;
            return isOpen && isGetLevelLimite;
        }
    }
}
#endif
