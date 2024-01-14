using System;
using System.Collections.Generic;
using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic {
    public class Sys_HundredPeopleArea : SystemModuleBase<Sys_HundredPeopleArea> {
        #region 数据定义
        public enum EEvents {
            HundredPeopleRankInfoRes,    //排行榜数据
            OnGotDailyAward,    // 每日奖励领取
            OnRefreshTimes,
            DailyRewardTime
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        /// <summary> 活动ID </summary>
        /// 5001
        public uint activityid {
            get {
                return 5001;
            }
        }
        // 5001101
        public uint passedInstanceId {
            get {
                //return 5001710;

                if (this.towerInsData != null) {
                    return this.towerInsData.PassedStageId;
                }
                return 0u;
            }
        }
        /// <summary> 百人道场 </summary>
        public TowerInsData towerInsData { get; set; }
        /// <summary> 百人道场排行数据 </summary>
        public CmdTowerInstanceRankInfoRes cmdTowerInstanceRankInfoRes { get; set; }

        private uint m_LastDailyRewardTime = 0;
        public uint lastDailyRewardTime { 
            get { return m_LastDailyRewardTime; } 
            set { 
                if (value == m_LastDailyRewardTime) 
                    return;

                m_LastDailyRewardTime = value;

                eventEmitter.Trigger(EEvents.DailyRewardTime);
            } 
        }

        public uint times;

        public void GetPassedStage(out int stage, out int layer) {
            stage = 0;
            layer = 0;
            if (this.passedInstanceId != 0u) {
                var csv = CSVInstanceDaily.Instance.GetConfData(this.passedInstanceId);
                stage = (int)csv.LayerStage;
                layer = 10 * (stage - 1) + (int)csv.Layerlevel;
            }
        }
        public void GetMaxStage(out int stage, out int layer) {
            uint level = Sys_Role.Instance.Role.Level;

            stage = 10;
            layer = 100;
            bool find = false;
            foreach (var kvp in CSVInstanceDaily.Instance.GetAll()) {
                if (kvp.InstanceId != this.activityid) {
                    continue;
                }

                if (level < kvp.LevelLimited) {
                    stage = (int)kvp.LayerStage;
                    layer = (int)kvp.Layerlevel;
                    find = true;
                    break;
                }
            }

            if (find) {
                var dict = PopolateData();
                uint instanceId = GetPreInstanceId(dict, stage, layer);
                var csv = CSVInstanceDaily.Instance.GetConfData(instanceId);
                if (csv != null) {
                    stage = (int)csv.LayerStage;
                    layer = 10 * (stage - 1) + (int)csv.Layerlevel;
                }
            }
        }

        public bool IsInstance {
            get {
                return Sys_Instance.Instance.curInstance.InstanceId == activityid;
            }
        }

        #endregion

        #region 系统函数
        public override void Init() {
            this.ProcessEvents(true);
        }
        public override void Dispose() {
            this.ProcessEvents(false);
        }
        public override void OnLogout() {
            this.towerInsData = null;
            this.cmdTowerInstanceRankInfoRes = null;
            this.lastDailyRewardTime = 0;
            this.times = 0;
        }
        #endregion

        #region 初始化
        protected void ProcessEvents(bool toRegister) {
            if (toRegister) {
                EventDispatcher.Instance.AddEventListener((ushort)CmdTowerInstance.RankInfoReq, (ushort)CmdTowerInstance.RankInfoRes, this.OnTowerInstanceRankInfoRes, CmdTowerInstanceRankInfoRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdTowerInstance.DailyRewardReq, (ushort)CmdTowerInstance.DailyRewardRes, this.OnTowerInstanceDailyRewardRes, CmdTowerInstanceDailyRewardRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTowerInstance.PlayTimesUpdateNtf, this.OnTowerInstancePlayTimesUpdateNtf, CmdTowerInstancePlayTimesUpdateNtf.Parser);
            }
            else {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTowerInstance.RankInfoRes, this.OnTowerInstanceRankInfoRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTowerInstance.DailyRewardRes, this.OnTowerInstanceDailyRewardRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTowerInstance.PlayTimesUpdateNtf, this.OnTowerInstancePlayTimesUpdateNtf);
            }
            Sys_Instance.Instance.eventEmitter.Handle<uint, uint, uint>(Sys_Instance.EEvents.PassStage, this.OnPassStage, toRegister);
        }
        #endregion

        #region 服务器发送消息
        /// <summary>
        /// 请求职业通关排行
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="career"></param>
        public void SendTowerInstanceRankInfoReq(uint instanceId, uint career) {
            CmdTowerInstanceRankInfoReq req = new CmdTowerInstanceRankInfoReq();
            req.InstanceId = instanceId;
            req.Career = career;
            NetClient.Instance.SendMessage((ushort)CmdTowerInstance.RankInfoReq, req);
        }
        /// <summary>
        /// 领取每日奖励
        /// </summary>
        public void SendTowerInstanceDailyRewardReq() {
            CmdTowerInstanceDailyRewardReq req = new CmdTowerInstanceDailyRewardReq();
            NetClient.Instance.SendMessage((ushort)CmdTowerInstance.DailyRewardReq, req);
        }
        // 进入下一大关
        public void ReqNextStage() {
            CmdInstanceSwitchNextStageReq req = new CmdInstanceSwitchNextStageReq();
            NetClient.Instance.SendMessage((ushort)CmdInstance.SwitchNextStageReq, req);
        }
        #endregion

        #region 服务器接收消息
        private void OnPassStage(uint PlayType, uint InstanceId, uint StageId) {
            if (PlayType == 100u) {
                var csv = CSVInstanceDaily.Instance.GetConfData(StageId);
                if (csv != null) {
                    if (csv.ChangeUI == 1) {
                        UIScheduler.Push(EUIID.UI_Hundred_Result, StageId, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
                    }
                    else if (csv.ChangeUI == 2) {
                        var map = this.PopolateData();
                        uint nextInstanceId = this.GetNextInstanceId(map, (int)csv.LayerStage, (int)csv.Layerlevel);
                        var csvNext = CSVInstanceDaily.Instance.GetConfData(nextInstanceId);
                        if (csvNext != null && Sys_Role.Instance.Role.Level < csvNext.LevelLimited) {
                            UIScheduler.Push(EUIID.UI_Hundred_Result, StageId, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
                        }
                    }
                }
            }
        }
        private Dictionary<int, Dictionary<int, CSVInstanceDaily.Data>> PopolateData() {
            Dictionary<int, Dictionary<int, CSVInstanceDaily.Data>> map = new Dictionary<int, Dictionary<int, CSVInstanceDaily.Data>>();
            var dict = CSVInstanceDaily.Instance.GetAll();
            foreach (var kvp in dict) {
                if (kvp.InstanceId != Sys_HundredPeopleArea.Instance.activityid) {
                    continue;
                }

                if (!map.TryGetValue((int)kvp.LayerStage, out Dictionary<int, CSVInstanceDaily.Data> dt)) {
                    dt = new Dictionary<int, CSVInstanceDaily.Data>();
                    map.Add((int)kvp.LayerStage, dt);
                }
                dt.Add((int)kvp.Layerlevel, kvp);
            }
            return map;
        }
        public uint GetNextInstanceId(Dictionary<int, Dictionary<int, CSVInstanceDaily.Data>> map, int layerStage, int layerLevel) {
            if (1 <= layerStage && layerStage <= 10) {
                if (1 <= layerLevel && layerLevel < 10) {
                    layerLevel++;
                    return map[layerStage][layerLevel].id;
                }
                else {
                    layerStage++;
                    layerLevel = 1;

                    if (layerStage > 10) {
                        return 0;
                    }
                    else {
                        return map[layerStage][layerLevel].id;
                    }
                }
            }
            return 0;
        }
        public uint GetPreInstanceId(Dictionary<int, Dictionary<int, CSVInstanceDaily.Data>> map, int layerStage, int layerLevel) {
            int oldStage = layerStage;
            int oldLayer = layerLevel;
            if (layerLevel <= 1) {
                layerLevel = 10;
                layerStage--;
            }
            else {
                layerLevel--;
            }

            if (layerStage <= 0) {
                layerLevel = oldLayer;
                layerStage = oldStage;
            }
            return map[layerStage][layerLevel].id;
        }
        /// <summary>
        /// 职业通关排行结果
        /// </summary>
        /// <param name="msg"></param>
        private void OnTowerInstanceRankInfoRes(NetMsg msg) {
            CmdTowerInstanceRankInfoRes res = NetMsgUtil.Deserialize<CmdTowerInstanceRankInfoRes>(CmdTowerInstanceRankInfoRes.Parser, msg);
            this.cmdTowerInstanceRankInfoRes = res;
            Sys_HundredPeopleArea.Instance.eventEmitter.Trigger(EEvents.HundredPeopleRankInfoRes);
        }
        /// <summary>
        /// 领取每日奖励结果
        /// </summary>
        /// <param name="msg"></param>
        private void OnTowerInstanceDailyRewardRes(NetMsg msg) {
            CmdTowerInstanceDailyRewardRes res = NetMsgUtil.Deserialize<CmdTowerInstanceDailyRewardRes>(CmdTowerInstanceDailyRewardRes.Parser, msg);
            this.lastDailyRewardTime = res.LastDailyRewardTime;

            this.eventEmitter.Trigger(EEvents.OnGotDailyAward);
        }
        /// <summary>
        /// 更新日常活动次数
        /// </summary>
        /// <param name="msg"></param>
        private void OnTowerInstancePlayTimesUpdateNtf(NetMsg msg) {
            CmdTowerInstancePlayTimesUpdateNtf ntf = NetMsgUtil.Deserialize<CmdTowerInstancePlayTimesUpdateNtf>(CmdTowerInstancePlayTimesUpdateNtf.Parser, msg);

            this.times = ntf.PlayTimesLimit.UsedTimes;
            this.eventEmitter.Trigger(EEvents.OnRefreshTimes);
        }
        #endregion

        public bool HasGotAward() {
            uint now = Sys_Time.Instance.GetServerTime();
            uint lastGotTime = lastDailyRewardTime;
            
            // DateTime date1 = Consts.START_TIME.AddSeconds(lastGotTime - 5 * 3600);
            // DateTime date2 =  Consts.START_TIME.AddSeconds(now - 5 * 3600);
            // bool isSameDay = (date1.Year == date2.Year && date1.DayOfYear == date2.DayOfYear);

            bool isSameDay = Sys_Time.IsServerSameDay5(lastGotTime, now);
            return isSameDay;
        }

        private UI_HundredPeopleArea_Data data = null;

        private UI_HundredPeopleArea_Data TryLoadData() {
            data = new UI_HundredPeopleArea_Data();
            data.Reset();
            data.LoadData();

            return data;
        }

        public uint GetCurrentInstanceId() {
            var data = TryLoadData();
            return data.unfinishedFirstStageId;
        }

        public bool IsBigerThanAwakeLevel(out uint awakeId, out uint awakeBufferId) {
            bool rlt = false;
            awakeId = 0;
            awakeBufferId = 0;
            uint id = GetCurrentInstanceId();
            CSVInstanceDaily.Data cSVInstanceDailyData = CSVInstanceDaily.Instance.GetConfData(id);
            if (null != cSVInstanceDailyData) {
                CSVTravellerAwakening.Data csvAwken = CSVTravellerAwakening.Instance.GetConfData(cSVInstanceDailyData.Awakeningid);
                if (csvAwken != null) {
                    awakeId = csvAwken.id;
                    awakeBufferId = csvAwken.BuffID;
                    uint curLevel = Sys_TravellerAwakening.Instance.awakeLevel;
                    rlt = curLevel >= csvAwken.id;
                }
            }
            
            return rlt;
        }
    }
}
