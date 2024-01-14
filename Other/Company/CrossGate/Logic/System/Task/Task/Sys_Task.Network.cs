using System.Collections.Generic;
using System.IO;
using Framework.Core.UI;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;

namespace Logic {
    public partial class Sys_Task : SystemModuleBase<Sys_Task> {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents {
            OnRefreshAll, // 全量数据
            OnRefreshed, // 增量数据
            OnTargetIndexChanged, // 当前任务目标切换

            OnFinishedTasksGot, // 已完成列表接受

            OnReceivedTarget, // 接受任务
            OnReceived, // 接受任务
            OnSubmited, // 提交任务
            OnSubmitedForChapter, // 提交任务
            OnSubmitedForSubTitle, // 提交任务
            OnRejected, // 拒绝任务
            OnForgoed, // 放弃任务
            OnStartExecTask, // 开始执行任务
            OnTraced, // 追踪任务

            OnShared, // 共享任务
            OnOpSharedTask,
            //OnSharedReconnect, // 共享任务断线重连

            OnTabAdded, // 状态类型添加
            OnTabRemoved, // 状态类型删除

            OnTaskStatusChanged, // 任务状态变化[包括任务完成]
            OnFinished, // 任务完成
            OnCurrentTaskChanged,
            OnTaskGoalStatusChanged, // 任务目标状态变化
            OnTrackedChanged, // 追踪
            OnOverTimeNoOpTask, // 长时间未操作任务

            OnStartExecTaskGoal, // 开始执行任务目标

            OnStartTimeLimit, // 开始显示任务
            OnEndTimeLimit, // 结束显示任务

            OnSubmitedItem, // 提交道具
            OnSubmitedPet, // 提交宠物
        }

        public override void Init() {
            // 全量
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTask.DataNtf, this.OnRefreshAll, CmdTaskDataNtf.Parser);
            // 增量
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTask.UpdateTaskNtf, this.OnRefreshed, CmdTaskUpdateTaskNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTask.AcceptTaskReq, (ushort) CmdTask.AcceptTaskRes, this.OnReceived, CmdTaskAcceptTaskRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTask.CommitTaskReq, (ushort) CmdTask.CommitTaskRes, this.OnSubmited, CmdTaskCommitTaskRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTask.AbandonTaskReq, (ushort) CmdTask.AbandonTaskRes, this.OnForgoed, CmdTaskAbandonTaskRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTask.TraceTaskReq, (ushort) CmdTask.TraceTaskRes, this.OnTraced, CmdTaskTraceTaskRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTask.ShareTaskNtf, this.OnSharedNtf, CmdTaskShareTaskNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTask.ShareTaskOpReq, (ushort) CmdTask.ShareTaskOpRes, this.OnTaskShareTaskOpRes, CmdTaskShareTaskNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTask.ShareTaskFeedbackNtf, this.OnSharedFeedbackNtf, CmdTaskShareTaskFeedbackNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTask.GetFinishedTaskReq, (ushort) CmdTask.GetFinishedTaskRes, this.OnFinishedTasks, CmdTaskGetFinishedTaskRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTask.UpdateFinishedNtf, this.OnUpdateFinishedNtf, CmdTaskUpdateFinishedNtf.Parser);
            // 限时任务
            EventDispatcher.Instance.AddEventListener((ushort) CmdTask.StartTimeLimitReq, (ushort) CmdTask.StartTimeLimitRes, this.OnStartTimeLimit, CmdTaskStartTimeLimitRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTask.EndTimeLimitReq, (ushort) CmdTask.EndTimeLimitRes, this.OnEndTimeLimit, CmdTaskEndTimeLimitRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTask.SubmitPetReq, (ushort) CmdTask.SubmitPetRes, this.OnReqSubmitPet, CmdTaskSubmitPetRes.Parser);
            //跳过开关更新
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTask.SkipTalkBtnUpdateNtf, this.OnSkipUpdateNtf, CmdTaskSkipTalkBtnUpdateNtf.Parser);

            // 对话选择              
            DialogueChooseAnswer.eventEmitter.Handle<DialogueChooseAnswer.CompleteTargetAnswerEvt>(DialogueChooseAnswer.EEvents.CompleteTarget, this.OnChooseAnswerCompleteTask, true);
            DialogueChooseAnswer.eventEmitter.Handle<DialogueChooseAnswer.ReceiveTaskAnswerEvt>(DialogueChooseAnswer.EEvents.ReceiveTask, this.OnChooseReceiveTask, true);

            // Sys_Fight.Instance.eventEmitter.Handle(Sys_Fight.EEvents.OnExitEffect, OnExitEffect, true);
            // ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnAfterEnterFightEffect, OnEnterEffect, true);

            // 功能开启
            Sys_FunctionOpen.Instance.eventEmitter.Handle<bool>(Sys_FunctionOpen.EEvents.StopOtherActions, this.OnFunctionOpen, true);

            // 采集
            Sys_CollectItem.Instance.eventEmitter.Handle<ulong>(Sys_CollectItem.EEvents.OnCollectEnded, this.OnCollectEnded, true);

            // 组队
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.TeamClear, this.OnTeamClear, true);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.BeCaptain, this.OnBeCaptain, true);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.BeMember, this.OnBeMember, true);

            // cutscene
            Sys_CutScene.Instance.eventEmitter.Handle<uint, uint>(Sys_CutScene.EEvents.OnRealStart, this.OnCutSceneStart, true);
            Sys_CutScene.Instance.eventEmitter.Handle<uint, uint>(Sys_CutScene.EEvents.OnRealEnd, this.OnCutSceneEnd, true);

            // 进出副本
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceEnter, this.OnInstanceEnter, true);
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceExit, this.OnInstanceExit, true);

            // 玩家手动操作中断自动任务
            Sys_Input.Instance.onClickOrTouch += this.OnClickOrTouch;
            Sys_Input.Instance.onLeftJoystick += this.OnLeftJoystick;
            Sys_Input.Instance.onRightJoystick += this.OnRightJoystick;

            // 退出战场自动恢复自动任务
            Sys_Fight.Instance.OnExitFight += this.OnExitFight;
            Sys_Fight.Instance.OnEnterFight += this.OnEnterFight;

            // 断线重连
            Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStart, this.OnReconnectStart, true);
            Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnReconnectStartReq, this.OnReconnectStart, true);

            // 条件不达标导致任务中断
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnTeleErr, this.OnConditionInValid, true);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnTransTipIntterupt, this.OnTransTipIntterupt, true);
            CollectionAction.eventEmitter.Handle(CollectionAction.EEvents.InterrputCollect, this.OnInterrputCollect, true);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnInterruptPathFind, this.OnInterruptPathFind, true);
            UIManager.GetStackEventEmitter().Handle<uint, int>(FUIStack.EUIStackEvent.BeginEnter, this.OnUIChange, true);
        }

        private static string STOREDIR = Framework.Consts.persistentDataPath + "/{0}";
        private static string STOREFILE;

        public override void OnLogin() {
            string dir = string.Format(STOREDIR, Sys_Role.Instance.RoleId.ToString());
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            STOREFILE = STOREDIR + "/{0}_FinishedTasks.bin";

            // 清空上一个账号数据，防止数据混淆
            this.ToOpTaskFlag = false;
            this.sharedTasks.Clear();
            this.giveupTasks.Clear();
            this.finishedTasks.Clear();
            this.allTasks.Clear();

            this.ParseNotifyParam();

            this.clueTipTask = new TaskEntry(clueTipId);
            this.loveTipTask = new TaskEntry(lovalTipId);
            this.challengeTipTask = new TaskEntry(challengeTipId);
            this.emptyTipTask = new TaskEntry(emptyTipId);

            this.fromReconnect = false;
        }

        public override void OnLogout() {
            this.lastLimitedTaskId = 0;
            _currentTabType = ETabType.Task;
            this.battleEndTimer?.Cancel();
        }

        public override void OnSyncFinished() {
            this.SetTouched();
            this.fromReconnect = false;
        }

        private string GetFinishedTaskJson() {
            string content = FrameworkTool.ListEncode(this.finishedTasks);
            return content;
        }

        private void SaveFinishTasks(int newVersion) {
            this.versionChecker.TrySave(newVersion, this.GetFinishedTaskJson);
        }

        public TaskTab GetTab(int taskType, bool createIfNotExist = true) {
            return this.GetTab((ETaskCategory) taskType, createIfNotExist);
        }

        public TaskTab GetTab(ETaskCategory taskType, bool createIfNotExist = true) {
            if (!this.taskTabs.TryGetValue((int) taskType, out TaskTab tab)) {
                if (createIfNotExist) {
                    tab = new TaskTab(taskType);
                    this.taskTabs.Add((int) taskType, tab);
                }
            }

            return tab;
        }

        private void ParseNotifyParam() {
            CSVParam.Data csv = CSVParam.Instance.GetConfData(115);
            if (csv != null) {
                string[] segs = csv.str_value.Split('|');
                if (segs != null && segs.Length >= 2) {
                    if (!int.TryParse(segs[0], out this.downLevel)) {
                        this.downLevel = 0;
                    }

                    if (!int.TryParse(segs[1], out this.upLevel)) {
                        this.upLevel = 99999;
                    }
                }
            }

            csv = CSVParam.Instance.GetConfData(116);
            if (csv != null) {
                if (!float.TryParse(csv.str_value, out this.overTime)) {
                    this.overTime = 25f;
                }
            }
        }

        #region 中断自动任务的各种情况

        private void OnChooseAnswerCompleteTask(DialogueChooseAnswer.CompleteTargetAnswerEvt evtData) {
            uint taskGoalIndex = (uint) this.GetTask(evtData.handlerID).currentTaskGoalIndex;
            CmdNpcDialogueChooseReq req = new CmdNpcDialogueChooseReq();
            req.UNpcId = Sys_Interactive.CurInteractiveNPC.uID;
            req.TaskId = evtData.handlerID;
            req.TaskIndex = taskGoalIndex;
            req.DialogueId = evtData.dialogueChooseID;
            req.DialogueOption = (uint) evtData.index;

            NetClient.Instance.SendMessage((ushort) CmdNpc.DialogueChooseReq, req);
        }

        private void OnChooseReceiveTask(DialogueChooseAnswer.ReceiveTaskAnswerEvt evtData) {
            uint taskGoalIndex = (uint) this.GetTask(evtData.handlerID).currentTaskGoalIndex;
            CmdNpcDialogueChooseReq req = new CmdNpcDialogueChooseReq();
            req.UNpcId = Sys_Interactive.CurInteractiveNPC.uID;
            req.TaskId = evtData.handlerID;
            req.TaskIndex = taskGoalIndex;
            req.DialogueId = evtData.dialogueChooseID;
            req.DialogueOption = (uint) evtData.index;

            NetClient.Instance.SendMessage((ushort) CmdNpc.DialogueChooseReq, req);
        }

        private bool fromReconnect = false;

        // 断线重连
        private void OnReconnectStart() {
            this.toBeExecdTasks.Clear();
            this.StopAutoTask(true);
            this.AutoRunMode = false;

            this.fromReconnect = true;
        }

        // 进入副本
        private void OnInstanceEnter() {
            this.toBeExecdTasks.Clear();

            this.AutoRunMode = false;
        }

        // 退出副本
        private void OnInstanceExit() {
            this.toBeExecdTasks.Clear();
        }

        /*
            手动移动操作 不续接寻路
            点击进入队伍 不续接寻路
            进入战斗（巡逻遇怪） 出来场景后 续接寻路
            传送后 不续接寻路

            只有遇怪战斗从战场出来之后的情形下，会自动续借任务
            其他都是打断任务，以及打断任务的自动续借
        */
        // 进入战斗
        private void OnEnterFight(CSVBattleType.Data _) {
            this.ReqNavgation(0);
            this.SetTouched();
            // cuibinbin
            this.StopAutoTask(true);
        }

        private Timer battleEndTimer;

        // 退出战斗
        private void OnExitFight() {
            this.SetTouched();

            if (this.toBeExecdTasks.Count <= 0) {
                if (this.currentTaskEntry != null) {
                    int battleResult = Net_Combat.Instance.GetBattleOverResult();
                    if (battleResult == 1) {
                        this.battleEndTimer?.Cancel();
                        this.battleEndTimer = Timer.RegisterOrReuse(ref this.battleEndTimer, 0.3f, this.OnTimerEnd);
                    }
                }
            }
        }

        private void OnTimerEnd() {
            if (this.currentTaskEntry != null && !this.currentTaskEntry.IsFinish()) {
                this.TryContinueDoCurrentTask();
            }
        }

        // 功能开启
        private void OnFunctionOpen(bool value) {
            // 功能开启的时候，停止后台的任务的自动执行
            // 如果已经在对话了，那gg!
            if (value) {
                this.StopAutoTask(true);

                if (Sys_Pet.Instance.clientStateId != Sys_Role.EClientState.None) {
                    Sys_Pet.Instance.ForceStop();
                }
            }
        }

        // 采集结束的时候重启自动任务
        private void OnCollectEnded(ulong t) {
            this.InterruptCurrentTaskDoing();
            if (ActionCtrl.Instance.actionCtrlStatus != ActionCtrl.EActionCtrlStatus.PlayerCtrl) {
                this.TryContinueDoCurrentTask();
            }
        }

        /// <summary>
        /// stopPlayerMove是否停止玩家的寻路移动
        /// </summary>
        /// <param name="stopPlayerMove"></param>
        public void StopAutoTask(bool stopPlayerMove, bool stopBehaviour = true) {
            DebugUtil.LogFormat(ELogType.eTask, "StopAutoTask {0} {1}", stopPlayerMove.ToString(), stopBehaviour.ToString());
            if (stopBehaviour) {
                ActionCtrl.Instance.InterruptCurrent();
                ActionCtrl.Instance.Reset();
            }

            //this.StopHangupTask();

            UIManager.CloseUI(Sys_PathFind.Instance.PathFindId);
            //GameCenter.mPathFindControlSystem?.Interupt();
            if(GameCenter.mLvPlay != null)
            {
                GameCenter.mPathFindControlSystem.Interupt();
            }

            this.StopAuto(false);

            if (stopPlayerMove) {
                //GameCenter.mPlayerController?.StopMove();
                if (GameCenter.mLvPlay != null)
                {
                    GameCenter.mPlayerControlSystem.StopMove();
                }                    
            }
        }

        public void StopHangupTask() {
            if (Sys_Pet.Instance.clientStateId == Sys_Role.EClientState.ExecTask) {
                if (null != GameMain.Procedure && null != GameMain.Procedure.CurrentProcedure &&
                    GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal) {
                    if (Sys_Pet.Instance.isSeal) {
                        {
                            Sys_Role.Instance.ReqHangUpReportHangUpStatus((uint) Sys_Role.EClientState.ExecTask, false, 0);
                        }
                    }
                }
            }
        }

        public void OnClearExcuteTasks() {
            this.toBeExecdTasks.Clear();
        }

        public void InterruptCurrentTaskDoing() {
            // 中断任务的自动执行flag
            //if (this._currentTaskEntry != null) {
            //    this._currentTaskEntry.EndDoing();
            //}
        }

        private void StopAuto(bool setCurrentTaskNull) {
            this.InterruptCurrentTaskDoing();

            if (setCurrentTaskNull) {
                if (this.currentTaskEntry != null) {
                    this.currentTaskEntry.childTaskEntry = null;
                }

                this._currentTaskEntry = null;
            }
        }

        #region 组队

        private void OnTeamClear() {
            // Bug #107572
            if (Sys_Role.Instance.hasSyncFinished && !this.fromReconnect) {
                this.StopAutoTask(true);
                this.SetTouched();

                if (Sys_Pet.Instance.clientStateId != Sys_Role.EClientState.None) {
                    Sys_Pet.Instance.ForceStop();
                }
            }
        }

        private void OnBeCaptain() {
            if (Sys_Role.Instance.hasSyncFinished && !this.fromReconnect) {
                this.StopAutoTask(true);
                this.SetTouched();

                if (Sys_Pet.Instance.clientStateId != Sys_Role.EClientState.None) {
                    Sys_Pet.Instance.ForceStop();
                }
            }
        }

        private void OnBeMember() {
            if (Sys_Role.Instance.hasSyncFinished && !this.fromReconnect) {
                this.StopAutoTask(true);
                this.SetTouched();

                if (Sys_Pet.Instance.clientStateId != Sys_Role.EClientState.None) {
                    Sys_Pet.Instance.ForceStop();
                }
            }
        }

        #endregion

        #region 条件不达标

        private void OnConditionInValid() {
            this.InterruptCurrentTaskDoing();
        }

        private void OnTransTipIntterupt() {
            this.InterruptCurrentTaskDoing();
        }

        private void OnInterrputCollect() {
            this.InterruptCurrentTaskDoing();
        }

        private void OnInterruptPathFind() {
            this.InterruptCurrentTaskDoing();
        }

        // 某些ui打开的时候，停止寻路
        // 其实可以用中介者各种component组合起来实现，这里先放到sys里面统一处理吧
        private void OnUIChange(uint stackId, int uiId) {
            if (uiId == (int) EUIID.UI_Pub
                || uiId == (int) EUIID.UI_PartnerGet
                || uiId == (int) EUIID.UI_Pet_Get
                || uiId == (int) EUIID.UI_Adventure_LevelUp) {
                DebugUtil.LogFormat(ELogType.eTask, "ui关闭停止任务: {0}", uiId.ToString());
                this.StopAutoTask(true);
                this.AutoRunMode = false;
            }
        }

        #endregion

        #region Cutscene

        private void OnCutSceneStart(uint seriesId, uint id) {
            // 停止任务行为树的执行
            // toBeExecdTasks.Clear();
            this.StopAutoTask(true);
        }

        private void OnCutSceneEnd(uint seriesId, uint cutId) {
            // 停止任务行为树的执行
            // toBeExecdTasks.Clear();
            this.TryContinueDoCurrentTask();
        }

        #endregion

        // 点击地面
        private void OnClickOrTouch(bool down) {
            // 手动点击寻路，或者手动操作摇杆的时候会注销当前任务，其他触屏操作不会引起任务注销
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal) {
                if (this.currentTaskEntry != null && this.currentTaskEntry.csvTask.outdoorFight) {
                    // ReqNavgation(0);
                }

                this.StopAutoTask(false, false);
                this.AutoRunMode = false;
            }
        }

        // 控制摇杆
        private void OnLeftJoystick(Vector2 v2, float f) {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal) {
                this.StopAutoTask(false, false);
                this.AutoRunMode = false;
            }
        }

        private void OnRightJoystick(Vector2 v2, float f) {
            if (GameMain.Procedure != null && GameMain.Procedure.CurrentProcedure != null && GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal) {
                this.StopAutoTask(false, false);
                this.AutoRunMode = false;
            }
        }

        #endregion

        #region 网络相关

        // 接受任务
        // 区域任务，npcid为0, 因为区域任务的时候，没有和npc交互,所以不知道具体npc的uid.
        public void ReqReceive(uint id, bool uidZero = false) {
            if (!this.IsOpen(id)) return;

            if (this.giveupTasks.Contains(id)) {
                DebugUtil.Log(ELogType.eTask, "不能接受被放弃的任务");
                return;
            }

            CmdTaskAcceptTaskReq req = new CmdTaskAcceptTaskReq();
            req.TaskId = id;
            if (!uidZero) {
                req.NpcUId = Sys_Interactive.CurInteractiveNPC.uID;
            }
            else {
                req.NpcUId = 0;
            }

            NetClient.Instance.SendMessage((ushort) CmdTask.AcceptTaskReq, req);
        }

        // 本地新增一个任务
        private void OnReceived(NetMsg msg) {
            CmdTaskAcceptTaskRes response = NetMsgUtil.Deserialize<CmdTaskAcceptTaskRes>(CmdTaskAcceptTaskRes.Parser, msg);
            DebugUtil.LogFormat(ELogType.eTask, "Task: OnReceived:{0}", response.TaskInfo.TaskId);

            TaskEntry taskEntry = this.AddReceivedTask(response.TaskInfo.TaskId, response.TaskInfo, true, out bool newIsCurrent);
            // 不等待server的tracklist,直接追踪
            this.AddOrRefreshTraceTask(taskEntry, true, false);

            if (taskEntry != null) {
                this._OnReceived(taskEntry, newIsCurrent);
                this.eventEmitter.Trigger<int, uint, TaskEntry>(EEvents.OnReceived, taskEntry.csvTask.taskCategory, response.TaskInfo.TaskId, taskEntry);
            }
        }

        public void OnFinish(TaskEntry taskEntry) {
            if (taskEntry != null) {
                if (taskEntry.csvTask.ShowUIId != null && taskEntry.csvTask.ShowUIId[0] == 2) {
                    UIScheduler.Push(taskEntry.csvTask.ShowUIId[1], null, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
                }

                if (taskEntry.csvTask.TaskCompleteTips) {
                    UIScheduler.Push(EUIID.UI_TaskComplete, null, null, true, () => {
                        return UIScheduler.popTypes[EUIPopType.WhenMaininterfaceOpenning].Invoke() &&
                               GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight;
                    });
                }


                this.eventEmitter.Trigger<int, uint, TaskEntry>(EEvents.OnFinished, taskEntry.csvTask.taskCategory, taskEntry.csvTask.id, taskEntry);

                if (taskEntry.csvTask.ChapterExpressionId != null && taskEntry.csvTask.ChapterExpressionId.Count >= 2 && taskEntry.csvTask.ChapterExpressionId[1] != 0) {
                    this.eventEmitter.Trigger<uint>(EEvents.OnSubmitedForChapter, taskEntry.csvTask.ChapterExpressionId[1]);

                    // 停止自动执行任务
                    this.StopAutoTask(false);
                }

                if (taskEntry.csvTask.SubtitleSpeakAside != null && taskEntry.csvTask.SubtitleSpeakAside.Count >= 2 && taskEntry.csvTask.SubtitleSpeakAside[1] != 0) {
                    this.eventEmitter.Trigger<uint>(EEvents.OnSubmitedForSubTitle, taskEntry.csvTask.SubtitleSpeakAside[1]);

                    // 停止自动执行任务
                    this.StopAutoTask(false);
                }
            }
        }

        public void TryReqSubmit(uint id) {
            CSVTask.Data csvTask = CSVTask.Instance.GetConfData(id);
            if (csvTask != null) {
                if (csvTask.DropId == null) {
                    this.ReqSubmit(id, 0);
                }
                else {
                    if (csvTask.DropId.Count <= 1) {
                        this.ReqSubmit(id, csvTask.DropId[0]);
                    }
                    else {
                        // cuibinin 设置当前任务的当前目标 isdong = false
                        UIManager.OpenUI(EUIID.UI_TaskSelectBox, true, id);
                    }
                }
            }
        }

        // 提交任务[没有道具则itemId为0]
        public void ReqSubmit(uint id, uint dropId = 0) {
            DebugUtil.LogFormat(ELogType.eTask, "Task: ReqSubmit:{0} dropId {1}", id.ToString(), dropId.ToString());

            if (!this.IsOpen(id)) {
                return;
            }
            // 当前任务已经提交
            if (this.IsSubmited(id)) {
                return;
            }
            
            CmdTaskCommitTaskReq req = new CmdTaskCommitTaskReq();
            req.TaskId = id;
            req.DropId = dropId;
            if (Sys_Interactive.CurInteractiveNPC != null) {
                req.NpcUId = Sys_Interactive.CurInteractiveNPC.uID;
            }
            else {
                req.NpcUId = 0;
            }

            NetClient.Instance.SendMessage((ushort) CmdTask.CommitTaskReq, req);
        }

        public void ReqSubmitPet(uint taskId, int taskGoalIndex, uint petUid) {
            DebugUtil.LogFormat(ELogType.eTask, "Task: ReqSubmitPet:{0} peyUid {1}", taskId.ToString(), petUid.ToString());

            CmdTaskSubmitPetReq req = new CmdTaskSubmitPetReq();
            req.TaskId = taskId;
            req.PetUId = petUid;
            req.Position = (uint) taskGoalIndex;
            if (Sys_Interactive.CurInteractiveNPC != null) {
                req.NpcUId = Sys_Interactive.CurInteractiveNPC.uID;
            }
            else {
                req.NpcUId = 0;
            }

            NetClient.Instance.SendMessage((ushort) CmdTask.SubmitPetReq, req);
        }

        public void OnReqSubmitPet(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eTask, "Task: OnReqSubmitPet");

            this.eventEmitter.Trigger(EEvents.OnSubmitedPet);
        }

        // 本地刪除一个任务
        private void OnSubmited(NetMsg msg) {
            CmdTaskCommitTaskRes response = NetMsgUtil.Deserialize<CmdTaskCommitTaskRes>(CmdTaskCommitTaskRes.Parser, msg);
            DebugUtil.LogFormat(ELogType.eTask, "Task: OnSubmited:{0}", response.TaskId);

            TaskEntry taskEntry = this.RemoveTask(response.TaskId);
            if (taskEntry != null) {
                this.toBeSubmitTasks.Remove(taskEntry);
                // 添加已完成任务
                this.finishedTasks.Add(response.TaskId);

                // 设置提交状态，触发给npc模块
                taskEntry.taskState = ETaskState.Submited;
                this._OnSubmited(taskEntry);

                // 维护clientVersion
                this.SaveFinishTasks(this.versionChecker.FileVersion + 1);
                this.eventEmitter.Trigger<int, uint, TaskEntry>(EEvents.OnSubmited, taskEntry.csvTask.taskCategory, response.TaskId, taskEntry);
            }
        }

        // 放弃已经receive的任务
        public void ReqForgo(uint id) {
            DebugUtil.LogFormat(ELogType.eTask, "Task: ReqForgo:{0}", id);

            if (!this.IsOpen(id)) return;
            CmdTaskAbandonTaskReq req = new CmdTaskAbandonTaskReq();
            req.TaskId = id;
            NetClient.Instance.SendMessage((ushort) CmdTask.AbandonTaskReq, req);
        }

        private void OnForgoed(NetMsg msg) {
            CmdTaskAbandonTaskRes response = NetMsgUtil.Deserialize<CmdTaskAbandonTaskRes>(CmdTaskAbandonTaskRes.Parser, msg);
            DebugUtil.LogFormat(ELogType.eTask, "Task: OnForgoed:{0}", response.TaskId);

            // 对于可重接的任务，状态为UnReceived，不在giveup和finish列表中
            // 否则 在give列表中,不在 finish列表中，同时是否有一个新的状态呢？
            TaskEntry taskEntry = this.RemoveTask(response.TaskId);
            if (taskEntry != null) {
                if (taskEntry.csvTask.whetherReset) // 可重接
                {
                    taskEntry.taskState = ETaskState.UnReceived;
                }
                else {
                    // 不可重接
                    if (!this.giveupTasks.Contains(response.TaskId)) {
                        taskEntry.taskState = ETaskState.UnReceived;
                        this.giveupTasks.Add(response.TaskId);
                    }
                }

                this.eventEmitter.Trigger<int, uint, TaskEntry>(EEvents.OnForgoed, taskEntry.csvTask.taskCategory, response.TaskId, taskEntry);
            }
        }

        // 追踪任务
        public void ReqTrace(uint id, bool isAddTrace, bool manual) {
            DebugUtil.LogFormat(ELogType.eTask, "Task: ReqTrace:{0} isAddTrace {1} manual:{2}", id.ToString(), isAddTrace.ToString(), manual.ToString());

            if (!this.IsOpen(id)) return;
            
            CmdTaskTraceTaskReq req = new CmdTaskTraceTaskReq();
            req.TaskId = id;
            req.IsAddTrace = isAddTrace;
            req.IsAuto = !manual;
            NetClient.Instance.SendMessage((ushort) CmdTask.TraceTaskReq, req);
        }

        private void OnTraced(NetMsg msg) {
            CmdTaskTraceTaskRes response = NetMsgUtil.Deserialize<CmdTaskTraceTaskRes>(CmdTaskTraceTaskRes.Parser, msg);
            DebugUtil.LogFormat(ELogType.eTask, "OnTraced sub:{0}  add:{1}", response.SubTaskId, response.AddTaskId);
            // 先刪除，后添加
            TaskEntry taskEntry = this.AddOrRefreshTraceTask(response.SubTaskId, false, !response.IsAuto);
            if (taskEntry != null) {
                taskEntry.traceType = ETaskTraceType.UnTrace;
                this.eventEmitter.Trigger<int, uint, TaskEntry>(EEvents.OnTraced, taskEntry.csvTask.taskCategory, response.SubTaskId, taskEntry);
            }

            taskEntry = this.AddOrRefreshTraceTask(response.AddTaskId, true, !response.IsAuto);
            if (taskEntry != null) {
                taskEntry.traceType = ETaskTraceType.Trace;
                if (taskEntry.csvTask.TaskTraceTips) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(taskEntry.csvTask.taskName) + "追踪成功");
                }
                
                this.eventEmitter.Trigger<int, uint, TaskEntry>(EEvents.OnTraced, taskEntry.csvTask.taskCategory, response.AddTaskId, taskEntry);
            }
        }

        public void ReqStepGoalFinish(uint taskId, uint taskGoalIndex = 0, uint arg1 = 0, uint arg2 = 0) {
            DebugUtil.LogFormat(ELogType.eTask, "Task: ReqStepGoalFinish:{0} NpcUId:{1} position{2} arg1:{3} arg2:{4}", taskId, Sys_Interactive.CurInteractiveNPC.uID, taskGoalIndex, arg1, arg2);

            if (!this.IsOpen(taskId)) {
                return;
            }

            // 当前任务已经完成
            if (this.GetTaskGoalState(taskId, (int) taskGoalIndex) == ETaskGoalState.Finish) {
                return;
            }

            // 当前任务已经提交
            if (this.IsSubmited(taskId)) {
                return;
            }

            CmdTaskUpdateTaskStatusReq req = new CmdTaskUpdateTaskStatusReq();
            req.TaskId = taskId;
            req.NpcUId = Sys_Interactive.CurInteractiveNPC.uID;
            req.Position = taskGoalIndex;
            req.Opt = arg1;
            req.TalkChooseId = arg2;
            NetClient.Instance.SendMessage((ushort) CmdTask.UpdateTaskStatusReq, req);
        }

        public void ReqStepGoalFinishEx(uint taskId, uint arg1 = 0, uint arg2 = 0) {
            uint taskGoalIndex = (uint) this.GetTask(taskId).currentTaskGoalIndex;
            this.ReqStepGoalFinish(taskId, taskGoalIndex, arg1, arg2);
        }

        // 设置当前正在寻路任务，用于不遇怪
        public void ReqNavgation(uint id) {
            //if (id == 0) {
            //    DebugUtil.LogFormat(ELogType.eTask, "请求 停止取消遇怪", id);
            //}
            //else {
            //    DebugUtil.LogFormat(ELogType.eTask, "请求 取消遇怪 taskId:{0}", id);
            //}

            //CmdTaskCurdoningTaskReq req = new CmdTaskCurdoningTaskReq();
            //req.TaskId = id;
            //NetClient.Instance.SendMessage((ushort)CmdTask.CurdoningTaskReq, req);
        }

        private void ReqFinishedTasks() {
            DebugUtil.LogFormat(ELogType.eTask, "ReqFinishedTasks");
            CmdTaskGetFinishedTaskReq req = new CmdTaskGetFinishedTaskReq();
            NetClient.Instance.SendMessage((ushort) CmdTask.GetFinishedTaskReq, req);
        }

        private void OnFinishedTasks(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eTask, "OnFinishedTasks");
            CmdTaskGetFinishedTaskRes response = NetMsgUtil.Deserialize<CmdTaskGetFinishedTaskRes>(CmdTaskGetFinishedTaskRes.Parser, msg);

            this.finishedTasks.Clear();
            // 已完成的主线任务 也在里面
            for (int i = 0, length = response.FinishTasks.Count; i < length; ++i) {
                var taskId = response.FinishTasks[i];
                TaskEntry taskEntry = this.GetTask(taskId);
                if (this.receivedTaskList.Contains(taskEntry)) {
                    continue;
                }

                this.finishedTasks.Add(taskId);
                if (taskEntry != null) {
                    taskEntry.taskState = ETaskState.Submited;
                }
            }

            this.SaveFinishTasks((int) response.TaskVersion);
            this.eventEmitter.Trigger(EEvents.OnFinishedTasksGot);
        }

        private void OnUpdateFinishedNtf(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eTask, "OnUpdateFinishedNtf");
            CmdTaskUpdateFinishedNtf response = NetMsgUtil.Deserialize<CmdTaskUpdateFinishedNtf>(CmdTaskUpdateFinishedNtf.Parser, msg);

            this.newFinishList.Clear();
            for (int i = 0, length = this.finishedTasks.Count; i < length; ++i) {
                uint taskId = this.finishedTasks[i];
                CSVTask.Data csv = CSVTask.Instance.GetConfData(taskId);
                if (csv != null) {
                    if (csv.taskCategory != response.TaskClass) {
                        this.newFinishList.Add(taskId);
                    }
                }
            }

            for (int i = 0, length = response.FinishTasks.Count; i < length; ++i) {
                this.newFinishList.Add(response.FinishTasks[i]);
            }

            this.finishedTasks = new List<uint>(this.newFinishList);
            this.SaveFinishTasks(this.versionChecker.FileVersion + 1);
        }

        public bool hasReceivedAll { get; private set; } = false;

        // 全量
        private void OnRefreshAll(NetMsg msg) {
            this.hasReceivedAll = false;

            this.sharedTasks.Clear();
            this.giveupTasks.Clear();

            DebugUtil.LogFormat(ELogType.eTask, "OnRefreshAll");
            CmdTaskDataNtf response = NetMsgUtil.Deserialize<CmdTaskDataNtf>(CmdTaskDataNtf.Parser, msg);
            this.receivedTasks.Clear();
            this.trackedTasks.Clear();
            this._trunkTraceId = 0;
			this.branchTaskIds.Clear();
            this.toBeExecdTasks.Clear();
            this.toBeSubmitTasks.Clear();
            this._currentTaskEntry = null;

            //开关数据处理
            ParseSkips(response);

            // 已完成任务版本
            this.versionChecker.Reset(Sys_Role.Instance.RoleId.ToString(), "{0}FinishedTasksVersionKey", STOREFILE, "{0}FinishedTasksMd5Key");
            if (!this.versionChecker.TryRequest((int) response.TaskVersion, this.ReqFinishedTasks, (content) => {
                this.finishedTasks = FrameworkTool.ListDecode(content);
                for (int i = 0, length = this.finishedTasks.Count; i < length; ++i) {
                    TaskEntry taskEntry = this.GetTask(this.finishedTasks[i]);
                    if (taskEntry != null) {
                        taskEntry.taskState = ETaskState.Submited;
                    }
                }

                this.eventEmitter.Trigger(EEvents.OnFinishedTasksGot);
            })) {
                DebugUtil.Log(ELogType.eTask, "LoadFinishTasks Fail");
            }

            DebugUtil.LogFormat(ELogType.eTask, "serverVersion: {0} clientVersion: {1}", response.TaskVersion, this.versionChecker.FileVersion);

            for (int i = 0, length = response.TaskInfo.Count; i < length; ++i) {
                TaskEntry entry = this.AddReceivedTask(response.TaskInfo[i].TaskId, response.TaskInfo[i], false, out bool _);
                if (entry != null) {
                    int goalIndex = response.TaskInfo[i].TimeLimitIndex;
                    if (goalIndex != -1) {
                        entry.taskGoals[goalIndex].endTime = response.TaskInfo[i].EndTime;
                    }

                    if (entry.hasTriggerArea && entry.IsFinish() && entry.IsInAnyArea()) {
                        this.toBeSubmitTasks.Add(entry);
                    }
                }
            }

            for (int i = 0, length = response.TraceTasks.Count; i < length; ++i) {
                this.AddOrRefreshTraceTask(response.TraceTasks[i], true, false);
            }

            // 放弃任务之后不能再接列表
            for (int i = 0, length = response.DiscardTasks.Count; i < length; ++i) {
                TaskEntry taskEntry = this.GetTask(response.DiscardTasks[i]);
                if (taskEntry != null) {
                    this.giveupTasks.Add(response.DiscardTasks[i]);
                }
            }

            this.eventEmitter.Trigger(EEvents.OnRefreshAll);

            this.hasReceivedAll = true;
        }

        // 增量，任务更新
        private void OnRefreshed(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eTask, "OnRefreshed");
            CmdTaskUpdateTaskNtf response = NetMsgUtil.Deserialize<CmdTaskUpdateTaskNtf>(CmdTaskUpdateTaskNtf.Parser, msg);
            for (int i = 0, length = response.TaskInfo.Count; i < length; ++i) {
                TaskUnit taskUnit = response.TaskInfo[i];
                TaskEntry taskEntry = this.RefreshReceivedTask(taskUnit.TaskId, taskUnit, true);
                this.eventEmitter.Trigger<int, uint, TaskEntry>(EEvents.OnRefreshed, taskEntry.csvTask.taskCategory, taskUnit.TaskId, taskEntry);
            }
        }

        #endregion

        #region 共享任务

        // 请求共享该任务给其他人
        public void ReqShare(uint id) {
            if (!this.IsOpen(id)) return;
            CmdTaskShareTaskReq req = new CmdTaskShareTaskReq();
            req.TaskId = id;
            NetClient.Instance.SendMessage((ushort) CmdTask.ShareTaskReq, req);
        }

        // server通知client，新来共享任务
        private void InnerSharedTaskNtf(CmdTaskShareTaskNtf response) {
            SharedTaskBlock stb = this.sharedTasks.Find((e) => { return e.taskEntry.csvTask.id == response.TaskId; });
            if (stb == null) {
                stb = new SharedTaskBlock(response.TaskId);
                stb.SetData(response.RoleName);
                stb.receivedTime = Time.time;

                this.sharedTasks.Add(stb);
            }

            if (UIManager.IsVisibleAndOpen(EUIID.UI_SharedTaskList)) {
                this.eventEmitter.Trigger<uint>(EEvents.OnShared, response.TaskId);
            }
            else {
                UIScheduler.Push((int) EUIID.UI_SharedTaskList, null, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
            }
        }

        private void OnSharedNtf(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eTask, "OnSharedNtf");
            CmdTaskShareTaskNtf response = NetMsgUtil.Deserialize<CmdTaskShareTaskNtf>(CmdTaskShareTaskNtf.Parser, msg);
            this.InnerSharedTaskNtf(response);
        }

        // 操作共享任务
        public void ReqOpAllSharedTask(uint op) {
            for (int i = 0, length = this.sharedTasks.Count; i < length; ++i) {
                this.ReqOpSharedTask(this.sharedTasks[i].taskEntry.id, op, this.sharedTasks[i].ownerName);
            }
        }

        public void ReqOpSharedTask(uint id, uint op, string roleName) {
            CmdTaskShareTaskOpReq req = new CmdTaskShareTaskOpReq();
            req.TaskId = id;
            req.Op = op;
            req.RoleName = roleName;
            NetClient.Instance.SendMessage((ushort) CmdTask.ShareTaskOpReq, req);

            // 客户端直接操作，不等待server
            this.OnOpSharedTask(id, op);
        }

        private void OnTaskShareTaskOpRes(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eTask, "Task: OnTaskShareTaskOpRes");
            CmdTaskShareTaskOpRes response = NetMsgUtil.Deserialize<CmdTaskShareTaskOpRes>(CmdTaskShareTaskOpRes.Parser, msg);

            this.OnOpSharedTask(response.TaskId, response.Op);
        }

        private void OnOpSharedTask(uint taskId, uint op) {
            this.sharedTasks.RemoveAll((e) => { return e.taskEntry.csvTask.id == taskId; });
            this.eventEmitter.Trigger<uint, uint>(EEvents.OnOpSharedTask, taskId, op);
        }

        private void OnSharedFeedbackNtf(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eTask, "Task: OnSharedFeedbackNtf");
            CmdTaskShareTaskFeedbackNtf response = NetMsgUtil.Deserialize<CmdTaskShareTaskFeedbackNtf>(CmdTaskShareTaskFeedbackNtf.Parser, msg);

            uint contentId = 1670000000u + response.State;
            string content = LanguageHelper.GetTaskTextContent(contentId, response.RoleName);
            Sys_Hint.Instance.PushContent_Normal(content);
        }

        #endregion

        public void ReqStartTimeLimit(uint taskId, uint taskIndex) {
            if (!Sys_Team.Instance.canManualOperate) {
                return;
            }

            CmdTaskStartTimeLimitReq req = new CmdTaskStartTimeLimitReq();
            req.TaskId = taskId;
            req.Position = taskIndex;
            NetClient.Instance.SendMessage((ushort) CmdTask.StartTimeLimitReq, req);
        }

        private void OnStartTimeLimit(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eTask, "Task: OnStartTimeLimit");
            CmdTaskStartTimeLimitRes response = NetMsgUtil.Deserialize<CmdTaskStartTimeLimitRes>(CmdTaskStartTimeLimitRes.Parser, msg);

            TaskEntry taskEntry = this.GetTask(response.TaskId, false);
            if (taskEntry != null) {
                TaskGoal taskgoal = taskEntry.taskGoals[(int) response.Position];
                taskgoal.endTime = response.EndTime;

                this.eventEmitter.Trigger<uint, uint, TaskEntry>(EEvents.OnStartTimeLimit, taskEntry.id, response.Position, taskEntry);

                // 继续推进任务执行
                this.TryContinueDoCurrentTask();
            }
        }

        public void ReqEndTimeLimit(uint taskId, uint taskIndex) {
            //if (!Sys_Team.Instance.canManualOperate) {
            //    return;
            //}

            CmdTaskEndTimeLimitReq req = new CmdTaskEndTimeLimitReq();
            req.TaskId = taskId;
            req.Position = taskIndex;
            NetClient.Instance.SendMessage((ushort) CmdTask.EndTimeLimitReq, req);
        }

        private void OnEndTimeLimit(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eTask, "Task: OnSharedFeedbackNtf");
            CmdTaskEndTimeLimitRes response = NetMsgUtil.Deserialize<CmdTaskEndTimeLimitRes>(CmdTaskEndTimeLimitRes.Parser, msg);

            TaskEntry taskEntry = this.GetTask(response.TaskId, false);
            if (taskEntry != null)
            {
                var goal = taskEntry.taskGoals[(int) response.Position];
                goal.endTime = 0;
                bool isTimeTask = goal.csv.TargetType == 68;//计时任务

                CSVTaskCategory.Data csvTaskCategory = CSVTaskCategory.Instance.GetConfData((uint) taskEntry.csvTask.taskCategory);
                if (csvTaskCategory != null && csvTaskCategory.WhetherTips && taskEntry.csvTask.WhetherTips != 0) {
                    if (!isTimeTask)
                        UIManager.OpenUI(EUIID.UI_TaskFailHint, false, taskEntry.id);
                    this.eventEmitter.Trigger<uint>(EEvents.OnEndTimeLimit, taskEntry.id);
                }
            }
        }

        #region 操作任务

        private TaskEntry AddReceivedTask(uint id, TaskUnit taskUnit, bool triggerEvent, out bool newIsCurrent) {
            newIsCurrent = false;
            if (this.TryAddReceivedTask(id, out TaskEntry taskEntry, triggerEvent)) {
                // 添加模式
                // 新接受任务，状态为未完成
                taskEntry.taskState = ETaskState.UnFinished;
                taskEntry.RefreshData(taskUnit, triggerEvent);
                if (triggerEvent) {
                    this.SetTouched();
                    if (this.currentTaskEntry == null) {
                        this.TrySetCurrentTask(taskEntry, true);
                        newIsCurrent = true;
                    }
                }

                this.giveupTasks.Remove(id);
            }

            return taskEntry;
        }

        private TaskEntry RefreshReceivedTask(uint id, TaskUnit taskUnit, bool triggerEvent) {
            if (!this.TryAddReceivedTask(id, out TaskEntry taskEntry, triggerEvent)) {
                // 刷新模式
                this.SetTouched();
                TaskGoal goal = taskEntry.RefreshData(taskUnit, triggerEvent);
                if (triggerEvent) {
                    if (goal != null) {
                        if (goal.isFinish) {
                            goal.OnFinished();
                        }
                        else {
                            this.TryContinueDoCurrentTask();
                        }
                    }
                }
            }

            return taskEntry;
        }

        private bool TryAddReceivedTask(uint id, out TaskEntry taskEntry, bool triggerEvent) {
            taskEntry = this.GetTask(id, true);
            if (taskEntry != null) {
                CSVTask.Data csv = taskEntry.csvTask;
                if (csv != null) {
                    int category = csv.taskCategory;
                    if (!this.receivedTasks.TryGetValue(category, out SortedDictionary<uint, TaskEntry> dict)) {
                        dict = new SortedDictionary<uint, TaskEntry>();
                        this.receivedTasks.Add(category, dict);

                        if (triggerEvent) {
                            this.eventEmitter.Trigger(EEvents.OnTabAdded, category);
                        }
                    }

                    if (!dict.ContainsKey(id)) {
                        dict.Add(id, taskEntry);
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }

            return false;
        }

        private TaskEntry AddOrRefreshTraceTask(uint taskId, bool trace, bool manualTrace) {
            TaskEntry entry = this.GetReceivedTask(taskId);
            this.AddOrRefreshTraceTask(entry, trace, manualTrace);
            return entry;
        }

        private void AddOrRefreshTraceTask(TaskEntry entry, bool trace, bool manualTrace) {
            if (entry != null) {
                int category = entry.csvTask.taskCategory;
                List<TaskEntry> ls;
                if (!this.trackedTasks.TryGetValue(category, out ls)) {
                    ls = new List<TaskEntry>();
                    this.trackedTasks.Add(category, ls);
                }

                if (trace && !ls.Contains(entry)) {
                    ls.Add(entry);
                }
                else if (!trace && ls.Contains(entry)) {
                    ls.Remove(entry);
                }

                entry.manualTrace = manualTrace;
                entry.isTraced = trace;

                TryReBuildUnTrunkIds();

                if (trace && category == (int)ETaskCategory.Trunk) {
                    this._trunkTraceId = entry.id;
                }
            }
        }

        private TaskEntry RemoveTask(uint id) {
            TaskEntry taskEntry = this.GetReceivedTask(id);
            //this.allTasks.Remove(id);
            if (taskEntry != null) {
                int taskCategory = taskEntry.csvTask.taskCategory;

                this.receivedTasks[taskCategory].Remove(taskEntry.id);
                if (this.receivedTasks[taskCategory].Count <= 0) {
                    this.receivedTasks.Remove(taskCategory);
                    this.eventEmitter.Trigger<int>(EEvents.OnTabRemoved, taskCategory);
                }

                this.trackedTasks[taskCategory].Remove(taskEntry);
            }

            return taskEntry;
        }

        #endregion

        #region 状态变化的操作

        private void _OnReceived(TaskEntry taskEntry, bool newIsCurrent) {
            if (taskEntry != null) {
                if (taskEntry.csvTask.ChapterExpressionId != null && taskEntry.csvTask.ChapterExpressionId.Count >= 1 && taskEntry.csvTask.ChapterExpressionId[0] != 0) {
                    this.eventEmitter.Trigger<uint>(EEvents.OnSubmitedForChapter, taskEntry.csvTask.ChapterExpressionId[0]);

                    // 停止自动执行任务
                    this.StopAutoTask(false);
                }

                if (taskEntry.csvTask.SubtitleSpeakAside != null && taskEntry.csvTask.SubtitleSpeakAside.Count >= 1 && taskEntry.csvTask.SubtitleSpeakAside[0] != 0) {
                    this.eventEmitter.Trigger<uint>(EEvents.OnSubmitedForSubTitle, taskEntry.csvTask.SubtitleSpeakAside[0]);

                    // 停止自动执行任务
                    this.StopAutoTask(false);
                }

                if (taskEntry.taskGoals[0].csv.TaskFrontPerform != 0) {
                    // 不能控制主角，否则可能和任务驱动主角的自动寻路冲突，比如主角正在表演，突然被驱动去寻路了。
                    WS_TaskGoalManagerEntity.StartTaskGoal<WS_TaskGoalControllerEntity>(taskEntry.taskGoals[0].csv.TaskFrontPerform, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, () => {
                        if (taskEntry.csvTask.ShowUIId != null && taskEntry.csvTask.ShowUIId[0] == 1) {
                            UIScheduler.Push(taskEntry.csvTask.ShowUIId[1], null, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
                        }
                    }, true, (int) TaskGoalEnum.B_ReceiveTask);
                }
                else {
                    if (newIsCurrent) {
                        // 如果是新接取任务，上面会处理currentTask
                        // 嵌套任务，如果某个子任务没有接受的时候，要去接受，接收到之后再自动驱动主任务执行
                        this.TryContinueDoCurrentTask();
                    }
                    else {
                        // if (taskEntry.csvTask.taskCategory == (uint)ETaskCategory.Trunk) {
                        //     this.TryDoTask(taskEntry, true, true);
                        // }
                    }
                }

                CSVTaskCategory.Data csvTaskCategory = CSVTaskCategory.Instance.GetConfData((uint) taskEntry.csvTask.taskCategory);
                if (csvTaskCategory != null && csvTaskCategory.WhetherTips && taskEntry.csvTask.WhetherTips != 0) {
                    UIManager.OpenUI(EUIID.UI_TaskHint, false, taskEntry.id);
                    this.eventEmitter.Trigger<uint>(EEvents.OnReceivedTarget, taskEntry.id);
                }
            }
        }

        private void _OnSubmited(TaskEntry taskEntry) {
            if (taskEntry != null) {
                // 设置当前任务为null
                if (taskEntry == this.currentTaskEntry) {
                    if (this.currentTaskEntry != null) {
                        this.currentTaskEntry.childTaskEntry = null;
                    }

                    if (taskEntry.csvTask.TaskExecTask != 0) {
                        var nextEntry = GetTask(taskEntry.csvTask.TaskExecTask);
                        TryDoTask(nextEntry, true, true);
                    }
                    else {
                        this.currentTaskEntry = null;
                    }
                }

                if (taskEntry.csvTask.rewardType == 1) {
                    UIScheduler.Push(EUIID.UI_TaskNormalResult, taskEntry, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
                }
                else if (taskEntry.csvTask.rewardType == 2 || taskEntry.csvTask.rewardType == 3) {
                    UIScheduler.Push(EUIID.UI_TaskSpecialResult, taskEntry, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
                }

                if (taskEntry.csvTask.ShowUIId != null && taskEntry.csvTask.ShowUIId[0] == 3) {
                    UIScheduler.Push(taskEntry.csvTask.ShowUIId[1], taskEntry, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
                }

                if (taskEntry.csvTask.ChapterExpressionId != null && taskEntry.csvTask.ChapterExpressionId.Count >= 3 && taskEntry.csvTask.ChapterExpressionId[2] != 0) {
                    this.eventEmitter.Trigger<uint>(EEvents.OnSubmitedForChapter, taskEntry.csvTask.ChapterExpressionId[2]);

                    // 停止自动执行任务
                    this.StopAutoTask(false);
                }

                if (taskEntry.csvTask.SubtitleSpeakAside != null && taskEntry.csvTask.SubtitleSpeakAside.Count >= 3 && taskEntry.csvTask.SubtitleSpeakAside[2] != 0) {
                    this.eventEmitter.Trigger<uint>(EEvents.OnSubmitedForSubTitle, taskEntry.csvTask.SubtitleSpeakAside[2]);

                    // 停止自动执行任务
                    this.StopAutoTask(false);
                }
            }
        }

        #endregion
    }
}