using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic {
    public partial class Sys_Task : SystemModuleBase<Sys_Task>, ISystemModuleUpdate {
        #region 数据

        public float lastRefreshTime { get; private set; }

        #region 界面表现

        public enum ETabType {
            Task,
            Dungon,
            FamilyRes,
            Team,
        }

        private static ETabType _currentTabType = ETabType.Task;

        public static ETabType currentTabType {
            get { return _currentTabType; }
            set {
                if (_currentTabType != value) {
                    _currentTabType = value;
                    toggle = false;
                }
            }
        }

        // true表示已经被点击过一次了
        public static bool toggle = false;

        #endregion

        // 等级限制
        private int downLevel = 0;
        private int upLevel = 9999;

        private float overTime = 25f;
        public float lastNotifyTime;

        public const uint clueTipId = int.MaxValue - 1;
        public TaskEntry clueTipTask;

        public const uint lovalTipId = int.MaxValue - 2;
        public TaskEntry loveTipTask;

        public const uint challengeTipId = int.MaxValue - 3;
        public TaskEntry challengeTipTask;

        public const uint emptyTipId = int.MaxValue - 4;
        public TaskEntry emptyTipTask;

        private UnityFileVersionChecker versionChecker = new UnityFileVersionChecker();

        public bool ToOpTaskFlag { get; private set; } = false;

        // 任务处于自动执行模式？
        public bool AutoRunMode { get; private set; } = false;

        public uint lastLimitedTaskId = 0;

        public bool IsSameTaskLimited(uint taskId) {
            return this.lastLimitedTaskId == taskId;
        }

        public bool isLevelValid {
            get {
                if (Sys_Role.Instance.hasSyncFinished) {
                    int playerLevel = (int) Sys_Role.Instance.Role.Level;
                    return this.downLevel <= playerLevel && playerLevel <= this.upLevel;
                }
                else {
                    return false;
                }
            }
        }

        public bool isOverTime {
            get {
                if (Sys_Role.Instance.hasSyncFinished) {
                    return (Time.realtimeSinceStartup - this.lastRefreshTime > this.overTime);
                }
                else {
                    return false;
                }
            }
        }

        private void SetTouched() {
            // 更新任务进度更新信息
            this.lastRefreshTime = Time.realtimeSinceStartup;
            if (this.ToOpTaskFlag) {
                this.ToOpTaskFlag = false;
                this.eventEmitter.Trigger<bool>(EEvents.OnOverTimeNoOpTask, false);
            }
        }

        private TaskEntry _currentTaskEntry;

        public TaskEntry currentTaskEntry {
            get { return this._currentTaskEntry; }
            set {
                if (value != this._currentTaskEntry) {
                    // 设置上一个任务为isDoing = false;
                    this.InterruptCurrentTaskDoing();

                    this._currentTaskEntry = value;
                    this.eventEmitter.Trigger<TaskEntry>(EEvents.OnCurrentTaskChanged, value);
                }
            }
        }

        public uint currentTaskId {
            get {
                if (this._currentTaskEntry != null) {
                    return this._currentTaskEntry.id;
                }

                return 0;
            }
        }

        public bool TrySetCurrentTask(TaskEntry target, bool triggerEvent) {
            if (target != this._currentTaskEntry) {
                this._currentTaskEntry = target;
                if (triggerEvent) {
                    this.eventEmitter.Trigger<TaskEntry>(EEvents.OnCurrentTaskChanged, target);
                }

                return true;
            }

            return false;
        }

        // 点击任务栏执行
        public void TryDoTask(TaskEntry taskEntry, bool set = true, bool auto = false, bool useFailTip = false) {
            if (taskEntry == null) {
                return;
            }

            // 前后两个任务不一样，则需要打断前一个任务
            if (taskEntry != this.currentTaskEntry) {
                this.InterruptCurrentTaskDoing();
            }

            this.AutoRunMode = true;

            Sys_Team.Instance.DoTeamTarget(Sys_Team.DoTeamTargetType.Task, taskEntry.id);

            // 设置
            if (set) {
                this.currentTaskEntry = taskEntry;
            }

            if (taskEntry.currentTaskGoal != null) {
                if (taskEntry.currentTaskGoal.csv.WhetherWayfinding) {
                }
                else {
                    this.eventEmitter.Trigger<bool>(EEvents.OnStartExecTask, auto);
                }
            }

            string reason = taskEntry.TryDoTask(auto);
            this.SetTouched();

            if (useFailTip) {
                if (!string.IsNullOrEmpty(reason)) {
                    Sys_Hint.Instance.PushContent_Static(reason);
                }
            }
        }

        public void TryDoTask(uint taskId, bool set = true, bool auto = false, bool useFailTip = false) {
            TaskEntry taskEntry = this.GetTask(taskId, false);
            if (taskEntry != null) {
                this.TryDoTask(taskEntry, set, auto, useFailTip);
            }
        }

        // 状态变化执行，因为需要判断是否执行下一个阶段
        public void ContinueDoTask(TaskEntry taskEntry) {
            taskEntry = taskEntry ?? this.currentTaskEntry;
            if (this.AutoRunMode && taskEntry != null) {
                this.TryDoTask(taskEntry, true, true);
            }
            else {
                var arg = taskEntry == null ? "" : taskEntry.id.ToString();
                //Debug.LogError("*******队列中存在不能执行的任务******  " + this.AutoRunMode + "  " + taskEntry.id);
                DebugUtil.LogFormat(ELogType.eTask, "*******队列中存在不能执行的任务****** {0} {1}", this.AutoRunMode.ToString(), arg);
            }
        }

        private string _reason = "";

        // 状态变化执行，因为需要判断是否执行下一个阶段
        public void TryContinueDoCurrentTask() {
            if (this.currentTaskEntry == null) {
                return;
            }

            if (this.currentTaskEntry.CanDo(true, ref this._reason)) {
                this.ContinueDoTask(this.currentTaskEntry);
            }
            else {
                if (!this.toBeExecdTasks.Contains(this.currentTaskEntry)) {
                    this.toBeExecdTasks.Add(this.currentTaskEntry);
                    DebugUtil.LogFormat(ELogType.eTask, "*******放入执行队列****** {0}", this.currentTaskEntry.id.ToString());
                    //Debug.LogError("*******放入执行队列******  " + currentTaskEntry.id);
                }
            }
        }

        // 这里将来会修改，使用openUI的时候传递一个closeAction。
        public void WhenDialogueCompleted(uint taskId) {
            TaskEntry taskentry = this.GetTask(taskId);
            if (taskentry != null && taskentry.currentTaskGoal.csv.TargetType == (int) ETaskGoalType.Dialogue) {
                this.ReqStepGoalFinishEx(taskId);
            }
        }

        public void WhenBubbleCompleted(uint taskId) {
            TaskEntry taskentry = this.GetTask(taskId);
            if (taskentry != null && taskentry.currentTaskGoal.csv.TargetType == (int) ETaskGoalType.OpenBubble) {
                this.ReqStepGoalFinishEx(taskId);
            }
        }

        // COW的形式写入的，而不是一次性直接全部处理完毕
        private Dictionary<uint, TaskEntry> allTasks = new Dictionary<uint, TaskEntry>();

        // 已经接受的所有任务
        // SortedDictionary方便排序
        // taskCategory:{ id:TaskEntry }
        public SortedDictionary<int, SortedDictionary<uint, TaskEntry>> receivedTasks = new SortedDictionary<int, SortedDictionary<uint, TaskEntry>>();

        public Dictionary<uint, TaskEntry> receivedTaskMap {
            get {
                Dictionary<uint, TaskEntry> ret = new Dictionary<uint, TaskEntry>();
                foreach (var kvpOuter in this.receivedTasks) {
                    foreach (var kvpInner in kvpOuter.Value) {
                        ret.Add(kvpInner.Key, kvpInner.Value);
                    }
                }

                return ret;
            }
        }

        public List<TaskEntry> receivedTaskList {
            get {
                List<TaskEntry> taskList = new List<TaskEntry>();
                foreach (var kvpOuter in this.receivedTasks) {
                    foreach (var kvpInner in kvpOuter.Value) {
                        taskList.Add(kvpInner.Value);
                    }
                }

                return taskList;
            }
        }

        // 追踪任务
        // taskCategory:{ id:TaskEntry }
        public SortedDictionary<int, List<TaskEntry>> trackedTasks = new SortedDictionary<int, List<TaskEntry>>();

        public List<TaskEntry> trackedTaskList {
            get {
                List<TaskEntry> taskList = new List<TaskEntry>();
                foreach (var kvp in this.trackedTasks) {
                    for (int i = 0, length = kvp.Value.Count; i < length; ++i) {
                        taskList.Add(kvp.Value[i]);
                    }
                }

                taskList.Sort((l, r) => { return l.csvTaskCategory.priority - r.csvTaskCategory.priority; });
                return taskList;
            }
        }


        private uint _trunkTraceId;

        public uint TrunkTraceId {
            get {
                if (currentTaskEntry != null) {
                    return currentTaskEntry.id;
                }

                return this._trunkTraceId;
            }
        }

        private List<uint> branchTaskIds = new List<uint>(8);

        public List<uint> GetTaskIds(ref uint mainTaskId) {
            mainTaskId = this.TrunkTraceId;
            return this.branchTaskIds;
        }

        private void TryReBuildUnTrunkIds() {
            this.branchTaskIds.Clear();
            foreach (var kvp in this.trackedTasks) {
                for (int i = 0, length = kvp.Value.Count; i < length; ++i) {
                    if (kvp.Value[i].csvTask.taskCategory != (int) ETaskCategory.Trunk) {
                        //     mainTaskId = kvp.Value[i].csvTask.id;
                        // }
                        // else {
                        this.branchTaskIds.Add(kvp.Value[i].csvTask.id);
                    }
                }
            }
        }

        public void SortByPrority(List<TaskEntry> tasks) {
            tasks.Sort((left, right) => {
                var leftTab = this.GetTab(left.csvTask.taskCategory);
                var rightTab = this.GetTab(right.csvTask.taskCategory);
                return leftTab.priority - rightTab.priority;
            });
        }

        public void SortByPrority(List<uint> tasks) {
            tasks.Sort((leftId, rightId) => {
                TaskEntry left = this.GetTask(leftId);
                TaskEntry right = this.GetTask(rightId);
                var leftTab = this.GetTab(left.csvTask.taskCategory);
                var rightTab = this.GetTab(right.csvTask.taskCategory);
                return leftTab.priority - rightTab.priority;
            });
        }

        private List<uint> newFinishList = new List<uint>(0);

        // 已完成任务 ids
        public List<uint> finishedTasks = new List<uint>(0);

        // 放弃之后不可重接的任务
        public List<uint> giveupTasks = new List<uint>(0);

        // 共享的任务
        public List<SharedTaskBlock> sharedTasks = new List<SharedTaskBlock>(0) { };

        // taskCategory:TaskTab
        public SortedDictionary<int, TaskTab> taskTabs = new SortedDictionary<int, TaskTab>();

        // 等待执行的任务
        public List<TaskEntry> toBeExecdTasks = new List<TaskEntry>(0);

        // 等待提交的区域性质的任务
        public List<TaskEntry> toBeSubmitTasks = new List<TaskEntry>(0);

        private readonly List<TaskEntry> dungonReceivedTasks = new List<TaskEntry>(0);

        #endregion

        public void OnUpdate() {
            if (this.toBeExecdTasks.Count > 0 && Sys_Role.Instance.hasSyncFinished) {
                // 只取最后一个
                TaskEntry taskEntry = this.toBeExecdTasks[this.toBeExecdTasks.Count - 1];
                string reason = null;
                if (taskEntry.CanDo(true, ref reason)) {
                    DebugUtil.LogFormat(ELogType.eTask, "--------取出执行任务--------- {0}", taskEntry.id.ToString());
                    //Debug.LogError("--------取出执行任务---------" + taskEntry.id);
                    this.ContinueDoTask(taskEntry);
                    this.toBeExecdTasks.Clear();
                }
            }

            if (!this.ToOpTaskFlag && this.isLevelValid && this.isOverTime) {
                // 有队伍，并且是队长，或者 或者无队伍
                if (Sys_Team.Instance.canManualOperate) {
                    // 长时间未操作任务
                    this.ToOpTaskFlag = true;
                    this.lastNotifyTime = Time.realtimeSinceStartup;
                    this.eventEmitter.Trigger<bool>(EEvents.OnOverTimeNoOpTask, true);
                }
            }

            if (this.ToOpTaskFlag && Time.realtimeSinceStartup - this.lastNotifyTime > 2.5f) {
                this.lastNotifyTime = Time.realtimeSinceStartup;
                this.eventEmitter.Trigger<bool>(EEvents.OnOverTimeNoOpTask, true);
            }
        }

        public TaskEntry GetReceivedTask(uint id) {
            CSVTask.Data csvTask = CSVTask.Instance.GetConfData(id);
            TaskEntry entry = null;
            if (csvTask != null) {
                if (this.receivedTasks.TryGetValue(csvTask.taskCategory, out var dict)) {
                    dict.TryGetValue(id, out entry);
                }
            }

            return entry;
        }

        public ETaskState GetTaskState(uint id) {
            TaskEntry taskEntry = this.GetTask(id, false);
            if (taskEntry != null) {
                // 对于UnReceived有两个分支，需要细分一下
                if (taskEntry.taskState == ETaskState.UnReceived) {
                    return TaskHelper.CanReceive(taskEntry.csvTask) ? ETaskState.UnReceivedButCanReceive : ETaskState.UnReceived;
                }
                else {
                    return taskEntry.taskState;
                }
            }
            else {
                if (this.finishedTasks.Contains(id)) {
                    return ETaskState.Submited;
                }
                else {
                    var csvTask = CSVTask.Instance.GetConfData(id);
                    if (csvTask != null) {
                        return TaskHelper.CanReceive(csvTask) ? ETaskState.UnReceivedButCanReceive : ETaskState.UnReceived;
                    }

                    return ETaskState.UnReceived;
                }
            }
        }

        public bool IsFinish(uint id) {
            return this.GetTaskState(id) == ETaskState.Finished;
        }

        public bool IsSubmited(uint id) {
            return this.GetTaskState(id) == ETaskState.Submited;
        }

        /// <param name="id"></param>
        /// <param name="taskIndex">第一个Index为0</param>
        /// <returns></returns>
        public ETaskGoalState GetTaskGoalState(uint id, int taskIndex) {
            ETaskGoalState state = ETaskGoalState.UnFinish;
            TaskEntry taskEntry = this.GetTask(id, false);
            if (taskEntry != null) {
                if (taskEntry.taskState == ETaskState.Submited) {
                    state = ETaskGoalState.Finish;
                }
                else {
                    state = taskEntry.GetState(taskIndex);
                }
            }
            else {
                if (this.finishedTasks.Contains(id)) {
                    state = ETaskGoalState.Finish;
                }
            }

            return state;
        }

        public TaskEntry GetTask(uint taskId, bool createIfNotExist = true) {
            if (taskId == clueTipId) {
                return this.clueTipTask;
            }
            else if (taskId == lovalTipId) {
                return this.loveTipTask;
            }
            else if (taskId == challengeTipId) {
                return this.challengeTipTask;
            }
            else if (taskId == emptyTipId) {
                return this.emptyTipTask;
            }

            if (!this.allTasks.TryGetValue(taskId, out TaskEntry taskEntry)) {
                if (createIfNotExist) {
                    taskEntry = TaskEntry.BuildTaskEntry(taskId);
                    if (taskEntry != null) {
                        this.allTasks.Add(taskId, taskEntry);
                    }
                }
            }

            return taskEntry;
        }

        public TaskGoal GetTaskGoal(uint taskId, int taskGoalIndex) {
            TaskGoal goal = null;
            TaskEntry taskEntry = this.GetTask(taskId);
            if (taskEntry != null) {
                goal = taskEntry.GetTaskGoal(taskGoalIndex);
            }

            return goal;
        }

        public List<TaskEntry> GetDungonReceivedTask(uint instanceId) {
            this.dungonReceivedTasks.Clear();
            if (this.receivedTasks.TryGetValue((int) ETaskCategory.Dungeon, out SortedDictionary<uint, TaskEntry> tasks)) {
                foreach (var kvp in tasks) {
                    if (kvp.Value.csvTask != null && kvp.Value.csvTask.TaskInstanceID != null) {
                        if (kvp.Value.csvTask.TaskInstanceID.Contains(instanceId)) {
                            this.dungonReceivedTasks.Add(kvp.Value);
                        }
                    }
                }
            }

            return this.dungonReceivedTasks;
        }

        public bool CanExec(uint taskId, int taskGoalIndex) {
            if (this.receivedTaskMap.TryGetValue(taskId, out TaskEntry taskEntry)) {
                string reason = "";
                if (taskEntry.CanDo(false, ref reason)) {
                    if (0 <= taskGoalIndex && taskGoalIndex < taskEntry.taskGoals.Count) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsOpen(uint id) {
            bool ret = false;
            CSVTask.Data csv = CSVTask.Instance.GetConfData(id);
            if (csv != null) {
                var tab = this.GetTab(csv.taskCategory);
                if (tab != null) {
                    ret = tab.IsOpen();
                }
            }

            return ret;
        }
    }
}