using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using System;
using Net;
using Packet;
namespace Logic
{
    /// <summary> 副本系统 </summary>
    public partial class Sys_Instance : SystemModuleBase<Sys_Instance>
    {
       
        #region 数据
        /// <summary> 未知错误原因 </summary>
        private const string unknowErrorReason = "Unknow Error";
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        /// <summary> 副本数据<key:活动ID> </summary>
        public Dictionary<uint, List<InstanceData>> dict_InstanceData = new Dictionary<uint, List<InstanceData>>();
        /// <summary> 活动服务器数据 </summary>
        public Dictionary<uint, ServerInstanceData> dict_ServerInstanceData = new Dictionary<uint, ServerInstanceData>();
        /// <summary> 反向查找副本对应活动 </summary>
        public Dictionary<uint, uint> dict_CheckInstanceToActivity = new Dictionary<uint, uint>();
        /// <summary> 反向查找关卡对应副本 </summary>
        public Dictionary<uint, uint> dict_CheckStageToInstance = new Dictionary<uint, uint>();
        /// <summary> 查找活动对应副本类型 </summary>
        public Dictionary<uint, uint> dict_CheckActivityToInstanceType = new Dictionary<uint, uint>();
        /// <summary> 当前副本 </summary>
        public CurInstance curInstance;
        /// <summary> 是否在副本中 </summary>
        public bool IsInInstance
        {
            get { return curInstance.InstanceId != 0 && curInstance.StageID != 0; }
        }

        public bool NeedTeam()
        {
            if (IsInInstance == false)
                return true;

            var data = CSVInstance.Instance.GetConfData(curInstance.InstanceId);

            if (data == null)
                return true;

            bool bNeedTeam = data.TeamUp == 0 ? false : true;

            return bNeedTeam;
        }

        #endregion
        #region 系统函数
        public override void Init()
        {
            base.Init();
            InitData();
            ProcessEvents(true);
        }
        public override void Dispose()
        {
            base.Dispose();
            ProcessEvents(false);
        }
        public override void OnLogin()
        {
            base.OnLogin();
            curInstance.Reset();
        }
        public override void OnLogout()
        {
            base.OnLogout();
            curInstance.Reset();
            ResetData();
        }
        public override void OnSyncFinished()
        {
            base.OnSyncFinished();
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            ClearData();
            var dict = CSVInstance.Instance.GetAll();
            foreach (var item in dict)
            {
                if (dict_InstanceData.ContainsKey(item.PlayType))
                {
                    dict_InstanceData[item.PlayType].Add(new InstanceData(item.id));
                }
                else
                {
                    dict_InstanceData.Add(item.PlayType, new List<InstanceData>() { new InstanceData(item.id) });
                }

                dict_CheckInstanceToActivity.Add(item.id, item.PlayType);

                if (!dict_CheckActivityToInstanceType.ContainsKey(item.PlayType))
                    dict_CheckActivityToInstanceType.Add(item.PlayType, item.Type);
            }
            var dict2 = CSVInstanceDaily.Instance.GetAll();
            foreach (var item in dict2)
            {
                dict_CheckStageToInstance.Add(item.id, item.InstanceId);
            }
        }
        /// <summary>
        /// 清理数据
        /// </summary>
        public void ClearData()
        {
            dict_InstanceData.Clear();
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        public void ResetData()
        {
            dict_ServerInstanceData.Clear();
        }
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                //通用副本    
                EventDispatcher.Instance.AddEventListener((ushort)CmdInstance.DataReq, (ushort)CmdInstance.DataRes, OnInstanceDataRes, CmdInstanceDataRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.DataNtf, OnInstanceDataNtf, CmdInstanceDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.DataUpdateNtf, OnInstanceDataUpdateNtf, CmdInstanceDataUpdateNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.ResLimitNtf, OnInstanceResLimitNtf, CmdInstanceResLimitNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.EnterNtf, OnInstanceEnterNtf, CmdInstanceEnterNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.EndNtf, OnInstanceEndNtf, CmdInstanceEndNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdInstance.ExitReq, (ushort)CmdInstance.ExitNtf, OnInstanceExitNtf, CmdInstanceExitNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.PassStageNtf, OnInstancePassStageNtf, CmdInstancePassStageNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.SwitchStageNtf, OnInstanceSwitchStageNtf, CmdInstanceSwitchStageNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.StateNtf, OnInstanceStateNtf, CmdInstanceStateNtf.Parser);
                //单人副本
                EventDispatcher.Instance.AddEventListener((ushort)CmdDailyInstance.RankInfoReq, (ushort)CmdDailyInstance.RankInfoRes, OnDailyInstanceRankInfoRes, CmdDailyInstanceRankInfoRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdDailyInstance.BestInfoReq, (ushort)CmdDailyInstance.BestInfoRes, OnDailyInstanceBestInfoRes, CmdDailyInstanceBestInfoRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdDailyInstance.StageUpdateNtf, OnDailyInstanceStageUpdateNtf, CmdDailyInstanceStageUpdateNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdDailyInstance.PlayTimesUpdateNtf, OnDailyInstancePlayTimesUpdateNtf, CmdDailyInstancePlayTimesUpdateNtf.Parser);
                //多人副本
                //EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.PlayerConfirmPush, OnNotify_PlayerConfirmPush, CmdInstancePlayerConfirmPush.Parser);
                // EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.PlayerConfirmNtf, OnNotify_PlayerConfirmNtf, CmdInstancePlayerConfirmNtf.Parser);

                Sys_Vote.Instance.AddVoteLisitener((ushort)VoteType.MultiInsEnter, OnNotify_Vote);

                //EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.PlayerConfirmDismiss, OnNotify_PlayerConfirmDismiss, CmdInstancePlayerConfirmDismiss.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.QueryTeamInstanceProgressRes, OnNotify_TeamMemsInstancePorcess, CmdInstanceQueryTeamInstanceProgressRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.PlayTypeLockRewordNtf, OnNotify_PlayTypeLockRewordNtf, CmdInstancePlayTypeLockRewordNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdInstance.SelectInstanceIdres, OnNotify_SelectInstanceIDRes, CmdInstanceSelectInstanceIDRes.Parser);

            }
            else
            {
                //通用副本
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.DataRes, OnInstanceDataRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.DataNtf, OnInstanceDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.DataUpdateNtf, OnInstanceDataUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.ResLimitNtf, OnInstanceResLimitNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.EnterNtf, OnInstanceEnterNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.EndNtf, OnInstanceEndNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.ExitNtf, OnInstanceExitNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.PassStageNtf, OnInstancePassStageNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.SwitchStageNtf, OnInstanceSwitchStageNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.StateNtf, OnInstanceStateNtf);
                
                //单人副本
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdDailyInstance.RankInfoRes, OnDailyInstanceRankInfoRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdDailyInstance.BestInfoRes, OnDailyInstanceBestInfoRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdDailyInstance.StageUpdateNtf, OnDailyInstanceStageUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdDailyInstance.PlayTimesUpdateNtf, OnDailyInstancePlayTimesUpdateNtf);
                //多人副本
                //EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.PlayerConfirmPush, OnNotify_PlayerConfirmPush);
                //EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.PlayerConfirmNtf, OnNotify_PlayerConfirmNtf);
                //  EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.PlayerConfirmDismiss, OnNotify_PlayerConfirmDismiss);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.QueryTeamInstanceProgressRes, OnNotify_TeamMemsInstancePorcess);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.PlayTypeLockRewordNtf, OnNotify_PlayTypeLockRewordNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdInstance.SelectInstanceIdres, OnNotify_SelectInstanceIDRes);

            }
        }
        #endregion

        #region 功能函数
        /// <summary>
        /// 得到副本数据
        /// </summary>
        /// <param name="activityid"></param>
        /// <returns></returns>
        public List<InstanceData> GetInstanceData(uint activityid)
        {
            List<InstanceData> list;
            dict_InstanceData.TryGetValue(activityid, out list);
            return list;
        }
        /// <summary>
        /// 得到副本服务器数据
        /// </summary>
        /// <param name="activityid"></param>
        /// <returns></returns>
        public ServerInstanceData GetServerInstanceData(uint activityid)
        {
            ServerInstanceData serverInstanceData;
            dict_ServerInstanceData.TryGetValue(activityid, out serverInstanceData);
            return serverInstanceData;
        }
        /// <summary>
        /// 副本是否开启
        /// </summary>
        /// <param name="activityid"></param>
        /// <param name="instanceid"></param>
        /// <param name="isTips"></param>
        /// <returns></returns>
        public bool IsOpen_Instance(uint activityid, uint instanceid, bool isTips = true)
        {
            List<InstanceData> list_InstanceData;
            if (!dict_InstanceData.TryGetValue(activityid, out list_InstanceData))
                return false;

            InstanceData instanceData = list_InstanceData.Find(x => x.instanceid == instanceid);
            if (null == instanceData)
                return false;

            instanceData.condition.Check();
            bool result = instanceData.condition.IsOpen();
            if (!result && isTips)
            {
                Sys_Hint.Instance.PushContent_Normal(instanceData.condition.GetErrorReason());
            }
            return result;
        }
        /// <summary>
        /// 是否通关副本关卡
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns></returns>
        public bool IsPassStage(uint stageId)
        {
            uint instanceId, activityid;
            if (!dict_CheckStageToInstance.TryGetValue(stageId, out instanceId))
                return false;
            if (!dict_CheckInstanceToActivity.TryGetValue(instanceId, out activityid))
                return false;
            ServerInstanceData serverInstanceData = GetServerInstanceData(activityid);
            if (null == serverInstanceData)
                return false;
            InsEntry insEntry = serverInstanceData.GetInsEntry(instanceId);
            if (null == insEntry)
                return false;
            return insEntry.PerMaxStageId >= stageId;
        }
        /// <summary>
        /// 是否过期
        /// </summary>
        /// <param name="activityid"></param>
        /// <returns></returns>
        public bool IsExpireTime(uint activityid)
        {
            DailyActivity dailyActivity = Sys_Daily.Instance.GetDailyActivityCom(activityid);
            if (null == dailyActivity) return true;

            if (Sys_Time.Instance.GetServerTime() > dailyActivity.ResLimit.ExpireTime)
                return true;

            return false;
        }
        #endregion
    }

 

    public partial class Sys_Instance : SystemModuleBase<Sys_Instance>
    {
        public CmdDailyInstanceRankInfoRes cmdDailyInstanceRankInfoRes;
        #region 单人副本
        #region 发送消息
        /// <summary>
        /// 请求通关排行榜信息
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="career"></param>
        public void DailyInstanceRankInfoReq(uint instanceId, uint career)
        {
            CmdDailyInstanceRankInfoReq req = new CmdDailyInstanceRankInfoReq();
            req.InstanceId = instanceId;
            req.Career = career;
            NetClient.Instance.SendMessage((ushort)CmdDailyInstance.RankInfoReq, req);
        }
        /// <summary>
        /// 请求最佳通关信息
        /// </summary>
        /// <param name="instanceId"></param>
        public void DailyInstanceBestInfoReq(uint instanceId)
        {
            CmdDailyInstanceBestInfoReq req = new CmdDailyInstanceBestInfoReq();
            req.InstanceId = instanceId;
            NetClient.Instance.SendMessage((ushort)CmdDailyInstance.BestInfoReq, req);
        }
        #endregion
        #region 接收消息    
        /// <summary>
        /// 职业通关排行结果
        /// </summary>
        /// <param name="msg"></param>
        private void OnDailyInstanceRankInfoRes(NetMsg msg)
        {
            CmdDailyInstanceRankInfoRes res = NetMsgUtil.Deserialize<CmdDailyInstanceRankInfoRes>(CmdDailyInstanceRankInfoRes.Parser, msg);
            cmdDailyInstanceRankInfoRes = res;
            Sys_Instance.Instance.eventEmitter.Trigger(EEvents.DailyInstanceRankInfoRes);
        }
        /// <summary>
        /// 单层最佳通关信息结果
        /// </summary>
        /// <param name="msg"></param>
        private void OnDailyInstanceBestInfoRes(NetMsg msg)
        {
            CmdDailyInstanceBestInfoRes res = NetMsgUtil.Deserialize<CmdDailyInstanceBestInfoRes>(CmdDailyInstanceBestInfoRes.Parser, msg);
            uint activityId;
            if (!dict_CheckInstanceToActivity.TryGetValue(res.InstanceId, out activityId)) return;
            ServerInstanceData serverInstanceData;
            if (!dict_ServerInstanceData.TryGetValue(activityId, out serverInstanceData)) return;
            serverInstanceData.bestInfos = res.BestInfos;
            Sys_Instance.Instance.eventEmitter.Trigger(EEvents.DailyInstanceBestInfoRes);
        }
        /// <summary>
        /// 自己的最佳通关记录更新
        /// </summary>
        /// <param name="msg"></param>
        private void OnDailyInstanceStageUpdateNtf(NetMsg msg)
        {
            CmdDailyInstanceStageUpdateNtf res = NetMsgUtil.Deserialize<CmdDailyInstanceStageUpdateNtf>(CmdDailyInstanceStageUpdateNtf.Parser, msg);
            ServerInstanceData serverInstanceData;
            if (!dict_ServerInstanceData.TryGetValue(res.PlayType, out serverInstanceData))
                return;

            DailyStage dailyStage = serverInstanceData.GetDailyStage(res.InstanceId, res.Stage.StageId);
            if (null == dailyStage)
                return;

            dailyStage.BestTime = res.Stage.BestTime;
            dailyStage.BestRound = res.Stage.BestRound;
        }
        /// <summary>
        /// 对应日常活动玩法次数
        /// </summary>
        /// <param name="msg"></param>
        private void OnDailyInstancePlayTimesUpdateNtf(NetMsg msg)
        {
            CmdDailyInstancePlayTimesUpdateNtf res = NetMsgUtil.Deserialize<CmdDailyInstancePlayTimesUpdateNtf>(CmdDailyInstancePlayTimesUpdateNtf.Parser, msg);
            ServerInstanceData serverInstanceData;
            if (!dict_ServerInstanceData.TryGetValue(1, out serverInstanceData))
                return;

            serverInstanceData.instancePlayTypeData.DailyIns.PlayTimesLimit = res.PlayTimesLimit;
        }
        #endregion
        #endregion
    }

}
