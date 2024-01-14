using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;

namespace Logic {
    public enum ETaskCategory {
        Trunk = 1, // 主线
        Branch = 2, // 分支
        Love = 3, // 爱心
        Challenge = 4, // 挑战
        Clue = 5, // 线索
        Product = 6, // 生产
        Occupation = 7, // 转职
        Dungeon = 8, // 副本
        Defence = 9, // 防範任務
        Pub = 10, // 酒館

        PetFriend = 11, // 宠物好感度
        PetBackground = 12, // 宠物剧情
        NPCFavorability = 13, // npc好感度
        TerrorTeam = 14, // 恐怖旅团
        Guide = 15, // 引导任务
        Arrest = 16, // 悬赏任务

        Advance = 19, //进阶任务
        Festival = 20, // 节日任务 

        Pet = 100, // 宠物

        ClueTip = 999, // 线索任务提示类型
    }

    public enum ETaskState {
        UnReceived, // 未接受 并且 接取条件不满足
        UnReceivedButCanReceive, // 未接受 但是 条件满足可以去接受
        UnFinished, // 已接收 但是 未完成
        Finished, // 已完成 但是 未提交
        Submited, // 已提交
    }

    public enum ETaskTraceType {
        None,
        Trace,
        UnTrace,
    }

    public class TaskEntry {
        public uint id;

        public CSVTask.Data csvTask {
            get { return CSVTask.Instance.GetConfData(this.id); }
        }

        public CSVTaskCategory.Data csvTaskCategory {
            get {
                if (this.csvTask != null) {
                    return CSVTaskCategory.Instance.GetConfData((uint) this.csvTask.taskCategory);
                }

                return null;
            }
        }

        // 嵌套任务中正在做的子任务
        public TaskEntry childTaskEntry;
        public TaskEntry parentTaskEntry;

        public TaskTab taskTab {
            get {
                if (this.csvTask != null) {
                    return Sys_Task.Instance.GetTab(this.csvTask.taskCategory, false);
                }

                return null;
            }
        }

        public bool autoPathFind {
            get { return this.csvTask.automaticPathfinding; }
        }

        public bool CanAutoPathFind(bool auto) {
            return this.autoPathFind || !auto;
        }

        public List<TaskGoal> _taskGoals = new List<TaskGoal>(0);

        public List<TaskGoal> taskGoals {
            get {
                if (this._taskGoals == null || this._taskGoals.Count <= 0) {
                    if (!NoBody) {
                        var csvGoals = this.csvTask.taskGoals;
                        if (csvGoals != null) {
                            for (int i = 0; i < csvGoals.Count; ++i) {
                                uint taskGoalId = (uint) csvGoals[i];
                                CSVTaskGoal.Data csvGoalData = CSVTaskGoal.Instance.GetConfData(taskGoalId);
                                if (csvGoalData != null) {
                                    string goalTypeName = "Logic.TaskGoal_" + ((ETaskGoalType) (csvGoalData.TargetType)).ToString();
                                    Type goalType = Type.GetType(goalTypeName);
                                    if (goalType == null) {
                                        DebugUtil.LogFormat(ELogType.eTask, "taskId {0} goalTypeName {1} convert {2} is error", this.csvTask.id, csvGoalData.TargetType, goalTypeName);

                                        // 防止报错
                                        goalType = typeof(TaskGoal_NavToNpcInteractive);
                                    }

                                    var goal = Activator.CreateInstance(goalType) as TaskGoal;
                                    goal?.Init(this, csvGoalData, i);
                                    this._taskGoals.Add(goal);
                                }
                            }
                        }
                    }
                }

                return this._taskGoals;
            }
        }

        public ETaskTraceType traceType = ETaskTraceType.None;

        // itemId:Count:DropId
        private List<Vector3Int> _rewards = null;

        public List<Vector3Int> rewards {
            get {
                if (this._rewards == null) {
                    this._rewards = new List<Vector3Int>(0);
                    if (this.csvTask.DropId != null) {
                        for (int i = 0, length = this.csvTask.DropId.Count; i < length; ++i) {
                            var element = this.csvTask.DropId[i];
                            List<ItemIdCount> ls = CSVDrop.Instance.GetDropItem(element);
                            if (ls != null) {
                                for (int j = 0, ll = ls.Count; j < ll; ++j) {
                                    var ele = ls[j];
                                    this._rewards.Add(new Vector3Int((int) ele.id, (int) ele.count, (int) element));
                                }
                            }
                        }
                    }
                }

                return this._rewards;
            }
        }

        private List<Vector2Int> _tinyRewards = null;

        public List<Vector2Int> tinyRewards {
            get {
                if (this._tinyRewards == null) {
                    this._tinyRewards = new List<Vector2Int>();
                    if (this.csvTask.RewardExp != null && this.csvTask.RewardExp.Count >= 2) {
                        this._tinyRewards.Add(new Vector2Int((int) this.csvTask.RewardExp[0], (int) this.csvTask.RewardExp[1]));
                    }

                    if (this.csvTask.RewardGold != null && this.csvTask.RewardGold.Count >= 2) {
                        this._tinyRewards.Add(new Vector2Int((int) this.csvTask.RewardGold[0], (int) this.csvTask.RewardGold[1]));
                    }
                }

                return this._tinyRewards;
            }
        }

        public ETaskState _taskState = ETaskState.UnReceived;

        public ETaskState taskState {
            get { return this._taskState; }
            set {
                if (this._taskState != value) {
                    var old = this._taskState;
                    this._taskState = value;

                    if (Sys_Task.Instance.hasReceivedAll) {
                        Sys_Task.Instance.eventEmitter.Trigger<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, this, old, value);
                        if (value == ETaskState.Finished) {
                            Sys_Task.Instance.OnFinish(this);
                        }
                    }
                }
            }
        }

        public string currentTaskContent {
            get {
                if (this.taskGoals.Count <= 0) {
                    return "";
                }

                return this.taskGoals[this.currentTaskGoalIndex].taskContent;
            }
        }

        public bool IsFinish() {
            return this.taskState == ETaskState.Finished;
        }

        // 是否手动点击追踪的任务
        public bool manualTrace;
        public bool clickTrack = false;
        private bool _isTraced;

        public bool isTraced {
            get { return this._isTraced; }
            set {
                if (this._isTraced != value) {
                    this._isTraced = value;

                    if (Sys_Task.Instance.hasReceivedAll) {
                        this.clickTrack = false;
                        Sys_Task.Instance.eventEmitter.Trigger<int, uint, TaskEntry>(Sys_Task.EEvents.OnTrackedChanged, this.csvTask.taskCategory, this.id, this);
                    }
                }
            }
        }

        // pseudoindex 伪索引
        public bool NoBody {
            get { return id == Sys_Task.clueTipId || id == Sys_Task.lovalTipId || id == Sys_Task.challengeTipId || id == Sys_Task.emptyTipId; }
        }

        public int currentTaskGoalIndex { get; private set; }

        public TaskGoal currentTaskGoal {
            get {
                if (0 <= this.currentTaskGoalIndex && this.currentTaskGoalIndex < this.taskGoals.Count) {
                    return this.taskGoals[this.currentTaskGoalIndex];
                }

                return null;
            }
        }

        // 是否被卡级表卡住
        public bool IsLimited(ETaskCategory cate, ref string reason) {
            return this.csvTask.taskCategory == (int) cate && !this.CanDoOnlyCSVCondition(ref reason);
        }

        public void EndDoing() {
            this.currentTaskGoal?.EndDoing();
            this.childTaskEntry?.EndDoing();
        }

        public TaskEntry() {
        }

        public TaskEntry(uint id) {
            this.Init(id);
        }

        public TaskEntry Init(uint id) {
            this._taskState = ETaskState.UnReceived;
            this.id = id;
            this._taskGoals.Clear();
            return this;
        }

        public TaskGoal GetTaskGoal(int index) {
            TaskGoal goal = null;
            if (0 <= index && index < this.taskGoals.Count) {
                goal = this.taskGoals[index];
            }

            return goal;
        }

        public void SetTaskGoalIndex(int index = 0) {
            if (index != this.currentTaskGoalIndex) {
                Sys_Task.Instance.eventEmitter.Trigger<TaskEntry, int, int>(Sys_Task.EEvents.OnTargetIndexChanged, this, this.currentTaskGoalIndex, index);
                this.currentTaskGoalIndex = index;
            }
        }

        public float GetCurrentProgress() {
            return this.currentTaskGoal.progress;
        }

        public float TotalProgress {
            get {
                float ret = 0f;
                if (this.taskGoals.Count <= 0) {
                    ret = 1f;
                }
                else {
                    if (!this.csvTask.conditionType) {
                        ret = this.currentTaskGoal.progress;
                    }
                    else {
                        for (int i = 0, length = this.taskGoals.Count; i < length; ++i) {
                            ret += this.taskGoals[i].progress;
                        }
                    }
                }

                ret = ret > 1f ? 1f : ret;
                return ret;
            }
        }

        public void GetProgress(out uint x, out uint y) {
            x = 0;
            y = 1;
            if (this.taskGoals.Count <= 0) {
                x = 1;
                y = 1;
            }
            else {
                if (!this.csvTask.conditionType) {
                    x = (uint) this.currentTaskGoal.current;
                    y = (uint) this.currentTaskGoal.limit;
                }
                else {
                    for (int i = 0, length = this.taskGoals.Count; i < length; ++i) {
                        x += (uint) this.taskGoals[i].progress;
                    }

                    y = 100;
                }
            }

            x = x > y ? y : x;
        }

        #region 触发区域相关

        public bool hasTriggerArea {
            get { return !(this.csvTask.taskMap == 0 && this.csvTask.taskTriggerMap == null); }
        }

        public bool IsInAnyArea() {
            return this.IsInMainMapTriggerArea() || this.IsInMainMapAcceptArea() || this.IsInSubMapAcceptArea() || this.IsInSubMapTraceArea();
        }

        // 主地图触发区域：可接受【server给的被动追踪】，下次进入此地图区域的时候，看triggermaps中是否有这个地图id,有就追踪，否则取消追踪
        public bool IsInMainMapTriggerArea() {
            uint currentMapId = Sys_Map.Instance.CurMapId;
            bool inSameMap = currentMapId == this.csvTask.taskMap;

            return inSameMap && TaskHelper.InCurrentMapArea(this, this.csvTask);
        }

        public bool IsInMainMapAcceptArea() {
            uint currentMapId = Sys_Map.Instance.CurMapId;
            bool inSameMap = currentMapId == this.csvTask.taskMap;

            return inSameMap && Sys_Npc.Instance.IsInNpcReceiveArea(Sys_Map.Instance.CurMapId, this.csvTask.receiveNpc, GameCenter.mainHero.transform);
        }

        public bool IsInSubMapAcceptArea() {
            bool isIn = false;
            uint currentMapId = Sys_Map.Instance.CurMapId;
            // 1：判断是否在子地图中
            var subMaps = this.csvTask.taskTriggerMap;
            if (subMaps != null) {
                for (int i = 0, length = subMaps.Count; i < length; ++i) {
                    var subMapId = subMaps[i];
                    // 在其中一张自地图中
                    if (currentMapId == subMapId) {
                        isIn = true;
                        break;
                    }
                }
            }

            return isIn;
        }

        public bool IsInSubMapTraceArea() {
            bool isIn = false;
            uint currentMapId = Sys_Map.Instance.CurMapId;
            // 1：判断是否在子地图中
            var subMaps = this.csvTask.taskTraceMap;
            if (subMaps != null) {
                for (int i = 0, length = subMaps.Count; i < length; ++i) {
                    var subMapId = subMaps[i];
                    // 在其中一张自地图中
                    if (currentMapId == subMapId) {
                        isIn = true;
                        break;
                    }
                }
            }

            return isIn;
        }

        #endregion

        public TaskGoal RefreshData(TaskUnit taskUnit, bool triggerEvent) {
            // 这里展示进度变化的，不是全量的
            TaskGoal goal = null;
            for (int i = 0, count = taskUnit.Process.Count; i < count; ++i) {
                TaskProcess taskProcess = taskUnit.Process[i];
                int targetIndex = (int) taskProcess.Position;
                if (0 <= targetIndex && targetIndex < this.taskGoals.Count) {
                    TaskGoal taskGoal = this.taskGoals[targetIndex];
                    bool oldStatus = taskGoal.isFinish;
                    // isdong设置为false
                    taskGoal.InterruptDoing();

                    taskGoal.Refresh(taskProcess.TargetProcess);
                    bool isGoalFinish = taskGoal.isFinish;
                    if (isGoalFinish) {
                        taskGoal.timer?.Cancel();
                        taskGoal.endTime = 0;
                    }

                    // 同一时刻，
                    if (triggerEvent) {
                        taskGoal.NotifyStatus(oldStatus, isGoalFinish);
                        goal = taskGoal;
                    }
                }
            }

            int finalTargetIndex = -1;
            for (int i = 0, count = this.taskGoals.Count; i < count; ++i) {
                if (this.taskGoals[i].progress < 1f) {
                    finalTargetIndex = i;
                    break;
                }
            }

            bool isFinish = !this.csvTask.conditionType;
            if (this.csvTask.conditionType) {
                isFinish |= (this.TotalProgress >= 1f);
                if (isFinish) {
                    finalTargetIndex = 0;
                }
            }
            else {
                if (finalTargetIndex == -1) {
                    isFinish = true;
                    finalTargetIndex = 0;
                }
                else {
                    isFinish = false;
                }
            }

            this.SetTaskGoalIndex(finalTargetIndex);
            if (isFinish) {
                this.taskState = ETaskState.Finished;
            }
            else {
                this.taskState = ETaskState.UnFinished;
            }

            return goal;
        }

        public bool CanDoOnlyCSVCondition(ref string reason) {
            reason = null;
            if (this.childTaskEntry == null) {
                bool result = true;
                if (this.csvTask.ExecuteLvLowerLimit != 0 || this.csvTask.ExecuteLvUpperLimit != 0) {
                    int level = (int) Sys_Role.Instance.Role.Level;
                    if (this.csvTask.ExecuteLvLowerLimit != 0) {
                        result &= (this.csvTask.ExecuteLvLowerLimit <= level);
                    }

                    if (this.csvTask.ExecuteLvUpperLimit != 0) {
                        result &= (level <= this.csvTask.ExecuteLvUpperLimit);
                    }
                }

                if (this.csvTask.LvUpperTips != 0 && !result)
                    reason = LanguageHelper.GetTaskTextContent(this.csvTask.LvUpperTips);
                return result;
            }
            else {
                return true;
            }
        }

        public bool CanDoForCsv(ref string reason) {
            bool result = true;
            if (this.csvTask.ExecuteLvLowerLimit != 0 || this.csvTask.ExecuteLvUpperLimit != 0) {
                int level = (int) Sys_Role.Instance.Role.Level;
                if (this.csvTask.ExecuteLvLowerLimit != 0) {
                    result &= (this.csvTask.ExecuteLvLowerLimit <= level);
                }

                if (this.csvTask.ExecuteLvUpperLimit != 0) {
                    result &= (level <= this.csvTask.ExecuteLvUpperLimit);
                }
            }

            if (this.csvTask.LvUpperTips != 0 && !result)
                reason = LanguageHelper.GetTaskTextContent(this.csvTask.LvUpperTips);
            return result;
        }

        public bool CanDo(bool auto, ref string reason) {
            if (this.taskState == ETaskState.Submited) {
                return false;
            }

            /*reason = "cutscene正在播放";*/
            if (Sys_CutScene.Instance.isPlaying) {
                return false;
            }

            /*reason = "functionOpen正在播放";*/
            if (Sys_FunctionOpen.Instance.isRunning) {
                return false;
            }

            bool isInFight = Sys_Fight.Instance.IsFight();
            if (isInFight) {
                /* reason = "战斗中";*/
                return false;
            }

            bool isInInteractive = (GameMain.Procedure != null && GameMain.Procedure.CurrentProcedure != null &&
                                    (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Interactive ||
                                     GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.CutScene)
                );
            if (isInInteractive) {
                /*reason = "交互中";*/
                return false;
            }

            if (!Sys_Map.Instance.isLoadOk) {
                return false;
            }

            // 挂机不执行任务
            if (Sys_Pet.Instance.clientStateId != Sys_Role.EClientState.None && Sys_Pet.Instance.clientStateId != Sys_Role.EClientState.ExecTask) {
                return false;
            }

            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                return false;
            }

            // 组队并且 没有暂离，则 不能自我执行任务
            // reason = "在组队中";
            if (!Sys_Team.Instance.canManualOperate) {
                return false;
            }

            // 当前是否在采集
            CollectionAction collectionAction = ActionCtrl.Instance.currentAutoAction as CollectionAction;
            if (collectionAction != null) {
                if (collectionAction.taskId == this.id) {
                    return false;
                }
            }

            // 当前是否调查
            InquiryAction inquiryAction = ActionCtrl.Instance.currentAutoAction as InquiryAction;
            if (inquiryAction != null) {
                if (inquiryAction.taskId == this.id) {
                    return false;
                }
            }

            // 当前是否传送
            TransmitTipAction transmitTipAction = ActionCtrl.Instance.currentAutoAction as TransmitTipAction;
            if (transmitTipAction != null) {
                if (transmitTipAction.taskId == this.id) {
                    return false;
                }
            }

            // 护送中的点击提示
            // if (Sys_Escort.Instance.EscortFlag) {
            //     if (this.currentTaskGoal != null && this.currentTaskGoal.csv.EscortTips != 0) {
            //         reason = LanguageHelper.GetTaskTextContent(this.currentTaskGoal.csv.EscortTips);
            //         return false;
            //     }
            //
            //     return false;
            // }
            //
            // // 跟随的点击提示
            // if (Sys_NpcFollow.Instance.NpcFollowFlag) {
            //     if (this.currentTaskGoal != null && this.currentTaskGoal.csv.FollowTips != 0) {
            //         reason = LanguageHelper.GetTaskTextContent(this.currentTaskGoal.csv.FollowTips);
            //         return false;
            //     }
            //
            //     return false;
            // }

            if (this.childTaskEntry == null) {
                return this.CanDoForCsv(ref reason);
            }
            else {
                return true;
            }
        }

        public virtual bool HasChild(TaskEntry taskEntry) {
            return true;
        }

        public virtual string TryDoTask(bool auto = true) {
            string reason = "";
            if (this.CanDo(auto, ref reason)) {
                if (this._taskState == ETaskState.UnReceived || this._taskState == ETaskState.UnReceivedButCanReceive) {
                    this.TryReceiveTask();
                }
                else if (this._taskState == ETaskState.UnFinished) {
                    {
                        int randomGoalIndex = this.currentTaskGoalIndex;
                        if (this.csvTask.TargetOrder != null && this.csvTask.TargetOrder.Count > 0) {
                            randomGoalIndex = UnityEngine.Random.Range(0, this.csvTask.TargetOrder.Count);
                            randomGoalIndex = this.csvTask.TargetOrder[randomGoalIndex] - 1;
                            if (this.taskGoals[randomGoalIndex].isFinish) {
                                randomGoalIndex = this.currentTaskGoalIndex;
                            }

                            this.SetTaskGoalIndex(randomGoalIndex);
                            DebugUtil.LogFormat(ELogType.eTask, "task id:{0}, currentGoalIndex {1}", this.id, this.currentTaskGoalIndex);
                        }

                        if (!VirtualNpcSystem.IsVirtualNpcActive(this.id, this.taskGoals[randomGoalIndex].csv.id)) {
                            if(this.taskGoals[randomGoalIndex].CanAutoPathFind(auto)) {
                                this.taskGoals[randomGoalIndex].TryDoExec(auto);
                            }
                            else {
                                DebugUtil.LogFormat(ELogType.eTask, "cannot do goal task id:{0}, currentGoalIndex {1}  auto: {2}", this.id, this.currentTaskGoalIndex, auto.ToString());
                            }
                        }
                        else {
                            DebugUtil.LogFormat(ELogType.eTask, "cannot do goal task id:{0}, currentGoalIndex {1} auto: {2}", this.id, this.currentTaskGoalIndex, auto.ToString());
                        }
                    }
                }
                else if (this._taskState == ETaskState.Finished) {
                    if (!auto) {
                        this.TrySubmitTask(auto);
                    }
                }
                else if (this._taskState == ETaskState.Submited) {
#if UNITY_EDITOR
                    DebugUtil.LogErrorFormat("taskid {0} canot run here, because taskstate is submited!", this.id.ToString());
#endif
                }
            }
            else {
                this.EndDoing();
                DebugUtil.LogFormat(ELogType.eTask, "--------暂时不能做------ {0}", reason);
            }

            return reason;
        }

        private void TryReceiveTask() {
            uint receiveNpc = this.csvTask.receiveNpc;
            if (receiveNpc != 0) {
                GameCenter.mPathFindControlSystem?.FindNpc(receiveNpc, (pos) => {
                    DebugUtil.LogFormat(ELogType.eTask, " PathFind receiveNpc npc{0} 成功! taskId: {1}", receiveNpc.ToString(), this.id.ToString());

                    GameCenter.FindNearestNpc(receiveNpc, out var targetNpc, out var guid, false);
                    if (targetNpc != null) {
                        List<ActionBase> innerActions = new List<ActionBase>();
                        InteractiveWithNPCAction interactiveWithNPC = ActionCtrl.Instance.CreateAction(typeof(InteractiveWithNPCAction)) as InteractiveWithNPCAction;
                        if (interactiveWithNPC != null) {
                            innerActions.Add(interactiveWithNPC);
                            interactiveWithNPC.npc = targetNpc;
                            // 不支持嵌套超过两层
                            interactiveWithNPC.currentTaskEntry = this.parentTaskEntry;
                        }

                        ActionCtrl.Instance.AddAutoActions(innerActions);
                    }

                    //if (this._taskState == ETaskState.UnReceived || this._taskState == ETaskState.UnReceivedButCanReceive) {
                    //    Sys_Task.Instance.ReqReceive(this.id, true);
                    //}
                    //else {
                    //    Sys_Task.Instance.TryContinueDoCurrentTask();
                    //}
                });
            }
            else {
                DebugUtil.Log(ELogType.eTask, "receiveNpc npc id 0");
            }
        }

        // 提交npc为0，则直接提交，无寻路或者对话
        // 否则寻路以及对话
        public void TrySubmitTask(bool auto) {
            DebugUtil.LogFormat(ELogType.eTask, "开始提交任务流程");

            // 不在爱心区域，但是完成了任务，同时需要去提交
            if (this.hasTriggerArea && !this.IsInAnyArea()) {
                // 添加到 等待队列中
                Sys_Task.Instance.toBeSubmitTasks.Add(this);
                return;
            }

            uint submitNpcId = this.csvTask.submitNpc;
            DebugUtil.LogFormat(ELogType.eTask, "提交npcid为{0} 自动寻路?{1}", submitNpcId, this.autoPathFind);
            if (submitNpcId != 0) {
                // 自动寻路
                DebugUtil.LogFormat(ELogType.eTask, "auto{0}", auto);
                if (this.CanAutoPathFind(auto)) {
                    DebugUtil.LogFormat(ELogType.eTask, "auto{0}", auto);
                    DebugUtil.LogFormat(ELogType.eTask, "提交任务流程： 寻路到Npc");
                    List<ActionBase> actions = new List<ActionBase>();
                    WaitSecondsAction waitSecondsAction = ActionCtrl.Instance.CreateAction(typeof(WaitSecondsAction)) as WaitSecondsAction;
                    if (waitSecondsAction != null) {
                        actions.Add(waitSecondsAction);
                        waitSecondsAction.Init(null, () => {
                            GameCenter.mPathFindControlSystem.FindNpc(submitNpcId, (pos) => {
                                var actionList = Sys_Task.Instance.GetTaskSubmitActionList(this);

                                DebugUtil.LogFormat(ELogType.eTask, "提交任务流程： 和Npc对话");
                                ActionCtrl.Instance.AddAutoActions(actionList);
                            });
                        });
                    }

                    ActionCtrl.Instance.AddAutoActions(actions);
                }
            }
            else {
                DebugUtil.LogFormat(ELogType.eTask, "提交任务流程： 和Npc对话");
                var actionList = Sys_Task.Instance.GetTaskSubmitActionList(this);
                ActionCtrl.Instance.AddAutoActions(actionList);
            }
        }

        public void ForceSubmitTask() {
            Sys_Task.Instance.TryReqSubmit(this.id);
        }

        public ETaskGoalState GetState(int taskGoalIndex) {
            bool isFinish = false;
            if (0 <= taskGoalIndex && taskGoalIndex < this.taskGoals.Count) {
                isFinish = this.taskGoals[taskGoalIndex].isFinish;
            }

            return isFinish ? ETaskGoalState.Finish : ETaskGoalState.UnFinish;
        }

        public static TaskEntry BuildTaskEntry(uint taskId) {
            TaskEntry taskEntry = null;
            CSVTask.Data csv = CSVTask.Instance.GetConfData(taskId);
            if (csv != null) {
                string typeName = "Logic.TaskEntry_" + ((ETaskCategory) csv.taskCategory).ToString();
                Type taskEntryType = Type.GetType(typeName);
                if (taskEntryType == null) {
                    // 防止出錯
                    taskEntryType = Type.GetType("Logic.TaskEntry");
                }

                taskEntry = Activator.CreateInstance(taskEntryType) as TaskEntry;
                taskEntry?.Init(taskId);
            }

            return taskEntry;
        }
    }
}