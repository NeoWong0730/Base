using System;
using System.Collections.Generic;
using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic {
    /// <summary> 挂机系统 </summary>
    public partial class Sys_Hangup : SystemModuleBase<Sys_Hangup> {
        #region 数据定义
        /// <summary> 事件枚举 </summary>
        public enum EEvents {
            OnHangupChange,
            OffLineHangupOver,
            OnHangUpDataUpdate,  //更新挂机数据
            OnEnemySwitch,       //引魔香状态改变
            OnActivePoint,       //解锁挂机点
            OnWorkingHourSwitch, //开启or关闭卡时
            OnWorkingHourPoint,  //卡时更新
            OnRestExpUpdate,     //休息时累计经验值更新
            OnResetMonthOfflineReward,      //重置月卡奖励
            OnHangupEnter,
            OnHangupExit,
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        #endregion
        #region 系统函数
        public override void Init() {
            this.ProcessEvents(true);
        }
        public override void Dispose() {
            this.ProcessEvents(false);
        }

        public override void OnLogin() {
            firstAD = true;
            this.SetHangupPointData();
        }
        public override void OnLogout() {
            firstAD = true;
            this.OnLogout_OverHangUp();
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void ProcessEvents(bool toRegister) {
            if (toRegister) {
                /// <summary> 在线|离线挂机 </summary>
                EventDispatcher.Instance.AddEventListener((ushort)CmdHangUp.HangUpOpReq, (ushort)CmdHangUp.HangUpOpRes, this.OnHangUpOpRes, CmdHangUpHangUpOpRes.Parser);
                /// <summary> 挂机打怪 </summary>
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdHangUp.DataNtf, this.OnDataNtf, CmdHangUpDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdHangUp.EnemySwitchReq, (ushort)CmdHangUp.EnemySwitchNtf, this.OnEnemySwitchNtf, CmdHangUpEnemySwitchNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdHangUp.WorkingHourOpNtf, this.OnWorkingHourOpNtf, CmdHangUpWorkingHourOpNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdHangUp.WorkingHourUpdateNtf, this.OnWorkingHourUpdateNtf, CmdHangUpWorkingHourUpdateNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdHangUp.RestExpUpdateNtf, this.OnRestExpUpdateNtf, CmdHangUpRestExpUpdateNtf.Parser);

            }
            else {
                /// <summary> 在线|离线挂机 </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdHangUp.HangUpOpRes, this.OnHangUpOpRes);
                /// <summary> 挂机打怪 </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdHangUp.DataNtf, this.OnDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdHangUp.EnemySwitchNtf, this.OnEnemySwitchNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdHangUp.WorkingHourOpNtf, this.OnWorkingHourOpNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdHangUp.WorkingHourUpdateNtf, this.OnWorkingHourUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdHangUp.RestExpUpdateNtf, this.OnRestExpUpdateNtf);
            }

            Sys_Pet.Instance.eventEmitter.Handle<int, int>(Sys_Pet.EEvents.OnPatrolStateChange, this.OnPatrolStateChange, toRegister);
            Sys_Npc.Instance.eventEmitter.Handle<uint>(Sys_Npc.EEvents.OnActiveNPC, this.OnActiveNPC, toRegister);
            Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStart, this.OnReconnectStart, toRegister);
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, this.OnReconnectResult, toRegister);
        }
        #endregion
    }
    /// <summary> 挂机系统-在线|离线挂机 </summary>
    public partial class Sys_Hangup : SystemModuleBase<Sys_Hangup> {
        public bool isHangup;
        public bool firstAD = true;
        public uint hangUpId = 0;

        public void HangUpOpReq(HangUpOperator HangUpOperator) {
            DebugUtil.LogFormat(ELogType.eTask, "HangUpOpReq: {0}", HangUpOperator);

            CmdHangUpHangUpOpReq req = new CmdHangUpHangUpOpReq();
            req.HangUpOpReq = (uint)HangUpOperator;
            NetClient.Instance.SendMessage((ushort)CmdHangUp.HangUpOpReq, req);

            if (HangUpOperator == HangUpOperator.StartHangUp) {
                Sys_PathFind.Instance.CloseUI();
                eventEmitter.Trigger(EEvents.OnHangupEnter);
            }
            else if (HangUpOperator == HangUpOperator.EndOnlineHangUp) {
                eventEmitter.Trigger(EEvents.OnHangupExit);
                Sys_PathFind.Instance.CloseUI();

                CSVHangupLayerStage.Data cSVHangupLayerStageData = CSVHangupLayerStage.Instance.GetConfData(this.hangUpId);
                if (cSVHangupLayerStageData != null) {
                    Sys_Team.Instance.DoTeamTarget(Sys_Team.DoTeamTargetType.OverHuangUp, cSVHangupLayerStageData.Hangupid);
                }
            }
        }
        private void OnHangUpOpRes(NetMsg msg) {
            CmdHangUpHangUpOpRes res = NetMsgUtil.Deserialize<CmdHangUpHangUpOpRes>(CmdHangUpHangUpOpRes.Parser, msg);
            DebugUtil.LogFormat(ELogType.eTask, "OnHangUpOpRes: {0} {1}", ((HangUpOperator)res.HangUpOpType).ToString(), res.TargetId.ToString());
            if (res.HangUpOpType == (uint)HangUpOperator.StartHangUp) {
                this.isHangup = true;
                //OptionManager.Instance.SwitchEnergySaving(true);

                if (res.TargetId != 0) { 
                    // 挂机
                    this.GotoHangup(res.TargetId);
                }
                else if (res.PetTid != 0) {
                    // 抓宠
                    // this.GotoHangup(res.PetTid);
                }

                eventEmitter.Trigger(EEvents.OnHangupEnter);
            }
            else if (res.HangUpOpType == (uint)HangUpOperator.EndOnlineHangUp) {
                this.isHangup = false;
                //OptionManager.Instance.SwitchEnergySaving(false);

                eventEmitter.Trigger(EEvents.OnHangupExit);

                this.hangUpId = res.TargetId;
            }
        }

        public void OnLogout_OverHangUp() {
            this.isHangup = false;
            this.resetTimer?.Cancel();
        }

        private void OnReconnectResult(bool obj) {
            //OptionManager.Instance.SwitchEnergySaving(false);
        }
    }
    /// <summary> 挂机系统-挂机打怪 </summary>
    public partial class Sys_Hangup : SystemModuleBase<Sys_Hangup> {
        #region 数据定义
        /// <summary>
        /// 挂机点激活状态
        /// </summary>
        public class HangupPointActivateState {
            /// <summary> 挂机点数据 </summary>
            public CSVHangup.Data cSVHangupData;
            /// <summary> 挂机点包含的所有挂机层数据 </summary>
            public List<uint> list_hangupLayerActivateStates = new List<uint>();

            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="id"></param>
            /// <param name="layerIds"></param>
            public void SetData(uint id, List<uint> layerIds) {
                this.cSVHangupData = CSVHangup.Instance.GetConfData(id);
                this.list_hangupLayerActivateStates.Clear();
                this.list_hangupLayerActivateStates.AddRange(layerIds);
            }
            /// <summary>
            /// 挂机点是否激活
            /// </summary>
            /// <returns></returns>
            public bool IsActivate() {
                if (null == this.cSVHangupData)
                    return false;

                for (int i = 0, count = this.list_hangupLayerActivateStates.Count; i < count; i++) {
                    uint id = this.list_hangupLayerActivateStates[i];
                    if (Sys_Hangup.Instance.IsUnlockHangupLayer(id))
                        return true;
                }
                return false;
            }
            /// <summary>
            /// 是否在推荐等级
            /// </summary>
            /// <returns></returns>
            public bool IsRecommendationLevel() {
                if (null == this.cSVHangupData ||
                    null == this.cSVHangupData.RecommendLv ||
                    this.cSVHangupData.RecommendLv.Count < 2)
                    return false;

                uint lowerLv = this.cSVHangupData.RecommendLv[0];
                uint upperLv = this.cSVHangupData.RecommendLv[1];
                return (Sys_Role.Instance.Role.Level >= lowerLv && Sys_Role.Instance.Role.Level <= upperLv);
            }
        }
        /// <summary>
        /// 挂机层激活状态
        /// </summary>
        public class HangupLayerActivateState {
            /// <summary> 挂机层数据 </summary>
            public CSVHangupLayerStage.Data cSVHangupLayerStageData;
            /// <summary>
            /// 设置数据
            /// </summary>
            /// <param name="id"></param>
            public void SetData(CSVHangupLayerStage.Data data)//(uint id)
            {
                this.cSVHangupLayerStageData = data;// CSVHangupLayerStage.Instance.GetConfData(id);
            }
            /// <summary>
            /// 挂机层是否激活
            /// </summary>
            /// <returns></returns>
            public bool IsActivate() {
                if (null == this.cSVHangupLayerStageData ||
                    null == this.cSVHangupLayerStageData.ActivatePara ||
                    this.cSVHangupLayerStageData.ActivatePara.Count <= 0)
                    return false;

                switch (this.cSVHangupLayerStageData.ActivateType) {
                    case 1: //默认-功能推送
                        {
                            uint id = this.cSVHangupLayerStageData.ActivatePara[0];
                            return Sys_FunctionOpen.Instance.IsOpen(id);
                        }
                    case 2: //激活npc
                        {
                            for (int i = 0; i < this.cSVHangupLayerStageData.ActivatePara.Count; i++) {
                                uint npcId = this.cSVHangupLayerStageData.ActivatePara[i];
                                if (Sys_Hangup.Instance.IsUnlockNpc(npcId))
                                    return true;
                            }
                            return false;
                        }
                }
                return false;
            }
            /// <summary>
            /// 是否在推荐等级
            /// </summary>
            /// <returns></returns>
            public bool IsRecommendationLevel() {
                if (null == this.cSVHangupLayerStageData ||
                    null == this.cSVHangupLayerStageData.RecommendLv ||
                    this.cSVHangupLayerStageData.RecommendLv.Count < 2)
                    return false;

                uint lowerLv = this.cSVHangupLayerStageData.RecommendLv[0];
                uint upperLv = this.cSVHangupLayerStageData.RecommendLv[1];
                return (Sys_Role.Instance.Role.Level >= lowerLv && Sys_Role.Instance.Role.Level <= upperLv);
            }
            /// <summary>
            /// 角色等级远高于目标等级
            /// </summary>
            /// <returns></returns>
            public bool IsLowLevel() {
                if (null == this.cSVHangupLayerStageData)
                    return false;
                uint highLv = this.cSVHangupLayerStageData.MonseterLv;
                uint levelDifference = Sys_Hangup.Instance.levelDifference;
                uint targetLv = highLv + levelDifference;
                return Sys_Role.Instance.Role.Level > targetLv;
            }
            /// <summary>
            /// 角色等级远低于目标等级
            /// </summary>
            /// <returns></returns>
            public bool IsHighLevel() {
                if (null == this.cSVHangupLayerStageData)
                    return false;
                uint lowerLv = this.cSVHangupLayerStageData.MonseterLv;
                uint levelDifference = Sys_Hangup.Instance.levelDifference;
                uint targetLv = lowerLv > levelDifference ? lowerLv - levelDifference : 0;
                return Sys_Role.Instance.Role.Level < targetLv;
            }
        }
        /// <summary> 挂机点激活状态 </summary>
        public Dictionary<uint, HangupPointActivateState> dict_HangupPointActivateState = new Dictionary<uint, HangupPointActivateState>();
        /// <summary> 挂机层激活状态 </summary>
        public Dictionary<uint, HangupLayerActivateState> dict_HangupLayerActivateState = new Dictionary<uint, HangupLayerActivateState>();
        /// <summary> 激活npc对应挂机层id </summary>
        public Dictionary<uint, uint> dict_ActivateNpc = new Dictionary<uint, uint>();
        /// <summary> 登录数据下发 </summary>
        public CmdHangUpDataNtf cmdHangUpDataNtf { get; set; } = new CmdHangUpDataNtf();
        /// <summary> 挂机无经验等级差 </summary>
        private uint levelDifference;
        #endregion

        public HangUpMonthReward monthOfflineReward;
        private Timer resetTimer;
        #region 数据处理
        public void SetHangupPointData()
        {
            this.dict_HangupPointActivateState.Clear();
            this.dict_HangupLayerActivateState.Clear();
            this.dict_ActivateNpc.Clear();

            /// <summary> 挂机层激活状态 </summary>
            //Dictionary<uint, CSVHangupLayerStage.Data> dict_hangupLayerStageData = CSVHangupLayerStage.Instance.GetDictData();
            //List<uint> list_hangupLayerStageDataKeys = new List<uint>(dict_hangupLayerStageData.Keys);

            var list_hangupLayerStageDatas = CSVHangupLayerStage.Instance.GetAll();
            for (int i = 0, count = count = list_hangupLayerStageDatas.Count; i < count; i++)
            {
                //uint LayerId = list_hangupLayerStageDataKeys[i];
                HangupLayerActivateState hangupLayerActivateState = new HangupLayerActivateState();
                hangupLayerActivateState.SetData(list_hangupLayerStageDatas[i]);
                this.dict_HangupLayerActivateState.Add(list_hangupLayerStageDatas[i].id, hangupLayerActivateState);
            }
            /// <summary> 筛选处理 </summary>
            Dictionary<uint, List<uint>> dict_Layer = new Dictionary<uint, List<uint>>();
            for (int i = 0, count = count = list_hangupLayerStageDatas.Count; i < count; i++)
            {
                uint layerId = list_hangupLayerStageDatas[i].id;
                uint PointId = layerId / 10;
                if (!dict_Layer.ContainsKey(PointId))
                {
                    dict_Layer.Add(PointId, new List<uint>() { layerId });
                }
                else
                {
                    dict_Layer[PointId].Add(layerId);
                }
            }
            /// <summary> 挂机点激活状态 </summary>
            //Dictionary<uint, CSVHangup.Data> dict_hangupData = CSVHangup.Instance.GetDictData();
            //List<uint> list_hangupDataKeys = new List<uint>(dict_hangupData.Keys);

            var list_hangupDatas = CSVHangup.Instance.GetAll();
            for (int i = 0, count = list_hangupDatas.Count; i < count; i++)
            {
                //uint PointId = list_hangupDataKeys[i];
                //var HangupData = dict_hangupData[PointId];

                var HangupData = list_hangupDatas[i];
                uint PointId = HangupData.id;

                HangupPointActivateState hangupPointActivateState = new HangupPointActivateState();
                hangupPointActivateState.SetData(PointId, dict_Layer[PointId]);
                this.dict_HangupPointActivateState.Add(PointId, hangupPointActivateState);
            }
            /// <summary> 激活npc处理 </summary>
            for (int i = 0, count = count = list_hangupLayerStageDatas.Count; i < count; i++)
            {
                //uint LayerId = list_hangupLayerStageDataKeys[i];
                //CSVHangupLayerStage.Data cSVHangupLayerStageDatad = dict_hangupLayerStageData[LayerId];

                CSVHangupLayerStage.Data cSVHangupLayerStageDatad = list_hangupLayerStageDatas[i];

                if (cSVHangupLayerStageDatad.ActivateType == 2 && null != cSVHangupLayerStageDatad.ActivatePara)
                {
                    for (int k = 0; k < cSVHangupLayerStageDatad.ActivatePara.Count; k++)
                    {
                        uint npcId = cSVHangupLayerStageDatad.ActivatePara[k];

                        if (!this.dict_ActivateNpc.ContainsKey(npcId))
                        {
                            this.dict_ActivateNpc.Add(npcId, cSVHangupLayerStageDatad.id);
                        }
                    }
                }
            }
            /// <summary> 获取挂机无经验等级差 </summary>
            string value = CSVHangupParam.Instance.GetConfData(7).str_value;
            uint.TryParse(value, out this.levelDifference);
        }
        #endregion
        #region 服务器发送消息
        /// <summary>
        /// 设置引魔香状态
        /// </summary>
        /// <param name="enemyOnOff"></param>
        public void SendHangUpEnemySwitchReq(bool enemyOnOff) {
            CmdHangUpEnemySwitchReq req = new CmdHangUpEnemySwitchReq();
            req.EnemyOnOff = enemyOnOff;
            NetClient.Instance.SendMessage((ushort)CmdHangUp.EnemySwitchReq, req);
        }
        /// <summary>
        /// 开启or关闭卡时
        /// </summary>
        /// <param name="open"></param>
        public void SendHangUpWorkingHourOpReq(bool open) {
            CmdHangUpWorkingHourOpReq req = new CmdHangUpWorkingHourOpReq();
            req.Open = open;
            NetClient.Instance.SendMessage((ushort)CmdHangUp.WorkingHourOpReq, req);
        }
        #endregion
        #region 服务器接收消息
        /// <summary>
        /// 登录数据下发
        /// </summary>
        /// <param name="msg"></param>

        public Action PendingAction = null;
        private void OnDataNtf(NetMsg msg) {
            CmdHangUpDataNtf ntf = NetMsgUtil.Deserialize<CmdHangUpDataNtf>(CmdHangUpDataNtf.Parser, msg);
            this.cmdHangUpDataNtf = ntf;
            this.eventEmitter.Trigger(EEvents.OnHangUpDataUpdate);

            if (ntf.LastOfflineReward != null) {
                bool isPrivilegeUsing = Sys_Attr.Instance.privilegeBuffIdList.Contains(3);
                if (isPrivilegeUsing && (ntf.LastOfflineReward.Exp > 0 || ntf.LastOfflineReward.Item.Count > 0 || ntf.LastOfflineReward.Pet.Count > 0 || ntf.LastOfflineReward.AutoSoldPet.Count > 0)) {
                    UIScheduler.Push(EUIID.UI_HangupResult, this.cmdHangUpDataNtf, null, true, () => {
                        return Sys_Role.Instance.hasSyncFinished && UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning].Invoke();
                    });
                }
            }

            if (Sys_Pet.Instance.clientStateId == Sys_Role.EClientState.Hangup && this.cmdHangUpDataNtf.HangUpInfoId != 0) {
                this.PendingAction = () => {
                    Sys_Hangup.Instance.GotoHangup(this.cmdHangUpDataNtf.HangUpInfoId);
                };
            }
            else if (Sys_Pet.Instance.clientStateId == Sys_Role.EClientState.CatchPet && this.cmdHangUpDataNtf.PetInfoId != 0) {
                this.PendingAction = () => {
                    Sys_Pet.Instance.DoSealPositon_CatchPet(this.cmdHangUpDataNtf.PetInfoId);
                };
            }

            this.monthOfflineReward = ntf.MonthOfflineReward;
            if (this.monthOfflineReward.ResetTime > TimeManager.GetServerTime()) {
                uint time = Sys_Hangup.Instance.monthOfflineReward.ResetTime - TimeManager.GetServerTime();
                this.resetTimer?.Cancel();
                this.resetTimer = Timer.Register(time, this.OnComplete);
            }
        }

        private void OnComplete() {
            this.monthOfflineReward.Exp = 0;
            this.monthOfflineReward.LastTime = 0;
            this.eventEmitter.Trigger(EEvents.OnResetMonthOfflineReward);
        }

        /// <summary>
        /// 引魔香状态改变通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnEnemySwitchNtf(NetMsg msg) {
            CmdHangUpEnemySwitchNtf ntf = NetMsgUtil.Deserialize<CmdHangUpEnemySwitchNtf>(CmdHangUpEnemySwitchNtf.Parser, msg);
            this.cmdHangUpDataNtf.EnemyOnOff = ntf.EnemyOnOff;
            this.eventEmitter.Trigger(EEvents.OnEnemySwitch);
        }
        /// <summary>
        /// 开启or关闭卡时
        /// </summary>
        /// <param name="msg"></param>
        private void OnWorkingHourOpNtf(NetMsg msg) {
            CmdHangUpWorkingHourOpNtf ntf = NetMsgUtil.Deserialize<CmdHangUpWorkingHourOpNtf>(CmdHangUpWorkingHourOpNtf.Parser, msg);
            this.cmdHangUpDataNtf.WorkingHourOpened = ntf.Opened;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(ntf.Opened ? (uint)2104006 : 2104007));
            Sys_Hangup.Instance.eventEmitter.Trigger(EEvents.OnWorkingHourSwitch);
        }
        /// <summary>
        /// 卡时更新
        /// </summary>
        /// <param name="msg"></param>
        private void OnWorkingHourUpdateNtf(NetMsg msg) {
            CmdHangUpWorkingHourUpdateNtf ntf = NetMsgUtil.Deserialize<CmdHangUpWorkingHourUpdateNtf>(CmdHangUpWorkingHourUpdateNtf.Parser, msg);
            this.cmdHangUpDataNtf.WorkingHourPoint = ntf.WorkingHourPoint;
            Sys_Hangup.Instance.eventEmitter.Trigger(EEvents.OnWorkingHourPoint);
        }
        /// <summary>
        /// 休息时累计经验值更新
        /// </summary>
        /// <param name="msg"></param>
        private void OnRestExpUpdateNtf(NetMsg msg) {
            CmdHangUpRestExpUpdateNtf ntf = NetMsgUtil.Deserialize<CmdHangUpRestExpUpdateNtf>(CmdHangUpRestExpUpdateNtf.Parser, msg);
            this.cmdHangUpDataNtf.RestExp = ntf.RestExp;
            if (ntf.RestExpTime != 0) {
                this.cmdHangUpDataNtf.RestExpTime = ntf.RestExpTime;
            }
            Sys_Hangup.Instance.eventEmitter.Trigger(EEvents.OnRestExpUpdate);
        }
        #endregion
        #region 事件处理
        /// <summary>
        /// 激活npc
        /// </summary>
        /// <param name="npcId"></param>
        public void OnActiveNPC(uint npcId) {
            if (!Sys_FunctionOpen.Instance.IsOpen(20301))
                return;

            uint layerId = 0;
            if (!this.dict_ActivateNpc.TryGetValue(npcId, out layerId))
                return;

            UIManager.OpenUI(EUIID.UI_UnLockHangup, false, layerId);
        }
        public void OnPatrolStateChange(int oldValue, int newValue) {
            // hangup -> none
            if (oldValue == (int)Sys_Role.EClientState.Hangup && newValue == (int)Sys_Role.EClientState.None) {
                this.HangUpOpReq(HangUpOperator.EndOnlineHangUp);
            }
        }

        /// <summary>
        /// 开启断线重连
        /// </summary>
        public void OnReconnectStart() {
            //if (isHangup)
            //{
            //    isHangup = false;
            //    eventEmitter.Trigger(Sys_Hangup.EEvents.OnHangupChange, Sys_Hangup.Instance.isHangup);
            //}
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 岛屿是否解锁
        /// </summary>
        /// <param name="IslandId"></param>
        /// <returns></returns>
        public bool IsUnlockIsland(uint IslandId) {
            CSVIsland.Data cSVIslandData = CSVIsland.Instance.GetConfData(IslandId);
            if (null == cSVIslandData)
                return false;

            return this.IsUnlockMap(cSVIslandData.mapid);
        }
        /// <summary>
        /// 地图是否解锁
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public bool IsUnlockMap(uint mapId) {
            CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(mapId);
            if (null == cSVMapInfoData)
                return false;

            return Sys_Role.Instance.Role.Level >= cSVMapInfoData.level &&
                (cSVMapInfoData.taskid == 0 || Sys_Task.Instance.IsSubmited(cSVMapInfoData.taskid));
        }
        /// <summary>
        /// 挂机点是否解锁
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsUnlockHangupPoint(uint id) {
            HangupPointActivateState hangupPointActivateState = null;
            if (!this.dict_HangupPointActivateState.TryGetValue(id, out hangupPointActivateState))
                return false;
            return hangupPointActivateState.IsActivate();
        }
        /// <summary>
        /// 挂机层是否解锁
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsUnlockHangupLayer(uint id) {
            HangupLayerActivateState hangupLayerActivateState = null;
            if (!this.dict_HangupLayerActivateState.TryGetValue(id, out hangupLayerActivateState))
                return false;
            return hangupLayerActivateState.IsActivate();
        }
        /// <summary>
        /// 挂机点是否在推荐等级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsRecommendationLevelHangupPoint(uint id) {
            HangupPointActivateState hangupPointActivateState = null;
            if (!this.dict_HangupPointActivateState.TryGetValue(id, out hangupPointActivateState))
                return false;
            return hangupPointActivateState.IsRecommendationLevel();
        }
        /// <summary>
        /// 挂机层是否在推荐等
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsRecommendationLevelHangupLayer(uint id) {
            HangupLayerActivateState hangupLayerActivateState = null;
            if (!this.dict_HangupLayerActivateState.TryGetValue(id, out hangupLayerActivateState))
                return false;
            return hangupLayerActivateState.IsRecommendationLevel();
        }
        /// <summary>
        /// 挂机层是否在低等级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsLowLevelHangupLayer(uint id) {
            HangupLayerActivateState hangupLayerActivateState = null;
            if (!this.dict_HangupLayerActivateState.TryGetValue(id, out hangupLayerActivateState))
                return false;
            return hangupLayerActivateState.IsLowLevel();
        }
        /// <summary>
        /// 挂机层是否在高等级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsHighLevelHangupLayer(uint id) {
            HangupLayerActivateState hangupLayerActivateState = null;
            if (!this.dict_HangupLayerActivateState.TryGetValue(id, out hangupLayerActivateState))
                return false;
            return hangupLayerActivateState.IsHighLevel();
        }
        /// <summary>
        /// npc是否解锁
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public bool IsUnlockNpc(uint npcId) {
            return Sys_Npc.Instance.IsActivatedNpc(npcId);
        }
        /// <summary>
        /// 设置驱(引)魔香状态
        /// </summary>
        public void SetEnemySwitch() {
            //巡逻状态不允许切换
            if (Sys_Pet.Instance.isSeal) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2104029));
                return;
            }
            //地图不允许切换
            CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
            if (null == cSVMapInfoData) return;
            bool targetEnemyOnOff = !Sys_Hangup.Instance.cmdHangUpDataNtf.EnemyOnOff;
            switch (cSVMapInfoData.EnemySwitch) {
                case 1: //强制不遇敌
                    {
                        if (targetEnemyOnOff) //目标为诱魔香
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2104004));
                            return;
                        }
                    }
                    break;
                case 2: //强制遇敌
                    {
                        if (!targetEnemyOnOff) //目标为驱魔香
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2104003));
                            return;
                        }
                    }
                    break;
            }

            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(targetEnemyOnOff ? (uint)2104001 : 2104002).words;
            PromptBoxParameter.Instance.SetConfirm(true, () => {
                Sys_Hangup.Instance.SendHangUpEnemySwitchReq(targetEnemyOnOff);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
        /// <summary>
        /// 设置时卡
        /// </summary>
        public void SetWorkingHour() {
            var cmdHangUpDataNtf = Sys_Hangup.Instance.cmdHangUpDataNtf;
            bool targetState = !cmdHangUpDataNtf.WorkingHourOpened;
            if (targetState) //需要打开卡时
            {
                if (cmdHangUpDataNtf.WorkingHourPoint == 0) //无卡时点数
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2104005));
                    return;
                }
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2104008, Sys_Role.Instance.Role.Name.ToStringUtf8(), cmdHangUpDataNtf.WorkingHourPoint.ToString());
                PromptBoxParameter.Instance.SetConfirm(true, () => {
                    Sys_Hangup.Instance.SendHangUpWorkingHourOpReq(true);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
            }
            else //需要关闭卡时
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2104019, Sys_Role.Instance.Role.Name.ToStringUtf8(), cmdHangUpDataNtf.WorkingHourPoint.ToString());
                PromptBoxParameter.Instance.SetConfirm(true, () => {
                    Sys_Hangup.Instance.SendHangUpWorkingHourOpReq(false);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
            }
        }
        /// <summary>
        /// 前往挂机
        /// </summary>
        /// <param name="layerId"></param>
        /// <returns></returns>
        public bool GotoHangup(uint layerId) {
            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                return false;
            }
            CSVHangupLayerStage.Data cSVHangupLayerStageData = CSVHangupLayerStage.Instance.GetConfData(layerId);
            if (null == cSVHangupLayerStageData) return false;
            bool isUnlockHangupLayer = Sys_Hangup.Instance.IsUnlockHangupLayer(layerId);
            if (!isUnlockHangupLayer) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2104020));
                return false;
            }
            Sys_Pet.Instance.DoSealPosition_HangupFight(layerId);

            Sys_Team.Instance.DoTeamTarget(Sys_Team.DoTeamTargetType.HuangUp, cSVHangupLayerStageData.Hangupid);

            return true;
        }

        public int GetTired() {
            uint level = Sys_Role.Instance.Role.Level;
            CSVCharacterAttribute.Data cSVCharacterAttributeData = CSVCharacterAttribute.Instance.GetConfData(level);
            if (null == cSVCharacterAttributeData) {
                return 0;
            }
            long maxExp = cSVCharacterAttributeData.DailyHangupTotalExp;
            bool isRestTime = !Sys_Time.IsServerSameDay5(Sys_Time.Instance.GetServerTime(), cmdHangUpDataNtf.RestExpTime);
            long curExp = isRestTime ? 0 : this.cmdHangUpDataNtf.RestExp;
            float value = maxExp == 0 ? 0 : (float)(curExp / (double)maxExp);
            int uintValue = (int)(value * 100);
            if (uintValue >= 100) {
                uintValue = 100;
            }
            return uintValue;
        }

        public void TryStopHangup() {
            bool isInFight = Sys_Fight.Instance.IsFight();
            if (isInFight) {
                Sys_Pet.Instance.ForceStop();
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2104052));
            }
            else {
                Sys_PathFind.Instance.CloseUI();
                ActionCtrl.Instance.InterruptCurrent();
                Sys_Pet.Instance.ForceStop();
            }
        }

        #endregion
    }
}
