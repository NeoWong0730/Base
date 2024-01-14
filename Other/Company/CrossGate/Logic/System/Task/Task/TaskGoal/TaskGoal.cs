using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic {
    // 对应目标表
    public enum ETaskGoalType {
        None = 0,
        Dialogue = 1, // 对话
        CollectItem = 3, // 收集物品
        FinishOtherTask = 5, // 完成其他嵌套任务
        ReceiveXTask = 6, // 接受n个任务目标
        CollectItemCounter = 7, // 达成收集次数
        PathFindOpenUI = 9, // openUI之前先寻路
        DialogueSelect = 10, // 对话选择

        HaveItem = 12, // 拥有道具完成任务
        SubmitItem = 13, // 上交道具

        FinishBattle = 15, // 完成战斗
        KillMonster = 16, // 击杀怪物
        KillMonsterGroup = 17, // 击杀怪物分组
        Career = 18, // 就职
        Pet = 19, // 宠物

        MapFinished = 20, // 地图信息完成
        NpcDetectFinished = 21, // NPC调查完成
        PetGot = 22, // 获得宠物
        Convoy = 23, // 护送任务
        PetTuJian = 24, // 宠物图鉴
        LotteryCount = 25, // 抽奖次数

        PlayCutScene = 26, // 播放cutscene
        NavToNpc = 27, // 导航到npc处
        FinishSkillUp = 28, // 技能升级，server判断升级

        OpenUI29 = 29,
        OpenUI30 = 30,
        OpenUI31 = 31,
        OpenUI32 = 32,
        OpenUI33 = 33,
        OpenUI34 = 34,
        OpenUI35 = 35,
        OpenUI36 = 36,
        OpenUI37 = 37,
        OpenUI38 = 38,
        OpenUI39 = 39,
        OpenUI40 = 40,
        OpenUI41 = 41,
        OpenUI42 = 42,
        OpenUI43 = 43,
        OpenUI44 = 44,
        OpenUI45 = 45,
        OpenUI46 = 46,
        OpenUI47 = 47,
        Follow = 48,  // 跟随
        FollowTrack = 49,  // 跟踪
        OpenBubble = 50,  // 打开界面棋牌
        FinishHundreadPeopeo = 51,  // 百人道场

        OpenUI52 = 52,

        ReachAwakenLevel = 53, // 达到觉醒等级
        PetPropertyLevel = 54, // 宠物属性等级
        JionFamily = 55, // 加入家族
        ConsuseFamilyCoin = 56, // 家族令牌
        GotLilianPoint = 57, // 历练点数
        SubmitFamilyMenu = 58, // 提交菜品
        LiftSkillLevel = 59,
        UploadMallItem = 60, // 上架
        EquipBuilt = 61, // 装备打造
        EquipTakeOn = 62, // 装备穿戴
    }

    public enum ETaskGoalState {
        UnFinish = 0,
        Finish = 1,
    }

    public abstract class TaskGoal {
        public int goalIndex;
        public int limit;
        public int current;

        public bool isDoing { get; protected set; } = false;

        public float progress {
            get {
                // 0.3333 显示为0.33
                return Mathf.Ceil(this.current * 1f / this.limit * 10000f) / 10000f;
            }
        }

        public bool isFinish {
            get { return this.limit != 0 && this.limit <= this.current; }
        }

        public Timer timer;

        private long _endTime;
        public long endTime {
            get {
                return this._endTime;
            }
            set {
                if (this._endTime != value) {
                    this._endTime = value;

                    this.timer?.Cancel();
                    if (value != 0) {
                        // 显示任务超时失败
                        this.timer = Timer.RegisterOrReuse(ref this.timer, this.RemainTime, () => {
                            Sys_Task.Instance.ReqEndTimeLimit(this.taskEntry.id, (uint)this.goalIndex);
                            this.OnIverLimitTime();
                            
                            // 客户端结束
                            _endTime = 0;
                        }, null, false, true);
                    }
                }
            }
        }

        protected virtual void OnIverLimitTime() {

        }
        public bool IsTimeLimitOpen {
            get {
                return this.csv.LimitTime != 0 && this.endTime != 0;
            }
        }
        public bool IsEnd {
            get {
                return this.endTime != 0 && Sys_Time.Instance.GetServerTime() > this.endTime;
            }
        }
        public long RemainTime {
            get {
                return this.endTime - Sys_Time.Instance.GetServerTime();
            }
        }

        protected string _taskContent = null;
        public string taskContent {
            get {
                if (this._taskContent == null) {
                    this._taskContent = this.GetTaskContent();
                }
                return this._taskContent;
            }
        }

        public virtual string GetTaskContent() {
            return null;
        }

        public TaskEntry taskEntry;
        public CSVTaskGoal.Data csv { get; protected set; }

        public bool autoImpl { get { return this.csv.automaticImplement; } }
        //public bool autoExecNextSeries { get { return this.csv.automaticImplement; } }

        public bool CanAutoImpl(bool auto) {
            return this.autoImpl || !auto;
        }

        // 执行时依赖的goalIndex
        private List<int> _dependencies = null;
        public List<int> dependencies {
            get {
                if (this._dependencies == null) {
                    if (!this.taskEntry.csvTask.taskGoalExecDependency) {
                        if (this.goalIndex == 0) {
                            this._dependencies = EmptyList<int>.Value;
                        }
                        else {
                            this._dependencies = new List<int>(this.goalIndex);
                            for (int j = 0; j < this.goalIndex; ++j) {
                                this._dependencies.Add(j);
                            }
                        }
                    }
                    else {
                        this._dependencies = EmptyList<int>.Value;
                    }
                }
                return this._dependencies;
            }
        }

        public bool IsAlldependencyFinish(out int failGoalIndex) {
            bool ret = true;
            failGoalIndex = -1;
            for (int i = 0, length = this.dependencies.Count; i < length; ++i) {
                int goalIndex = this.dependencies[i];
                ret &= this.taskEntry.taskGoals[goalIndex].isFinish;
                if (!ret) {
                    failGoalIndex = goalIndex;
                    break;
                }
            }

            return ret;
        }

        public bool CanPathFind {
            get { return !this.csv.WhetherWayfinding; }
        }

        public TaskGoal() {
        }

        public virtual TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }

        protected TaskGoal InitTaskEntry(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.taskEntry = taskEntry;
            this.goalIndex = goalIndex;
            this.csv = csv;

            return this;
        }

        public void NotifyStatus(bool oldStatus, bool newStatus) {
            if (oldStatus != newStatus) {
                Sys_Task.Instance.eventEmitter.Trigger<uint, uint, bool, bool>(Sys_Task.EEvents.OnTaskGoalStatusChanged, this.taskEntry.id, this.csv.id, oldStatus, newStatus);
            }
        }

        // 任务目标完成的事件
        public virtual void OnFinished() {
            if (this.csv.PathfindingTargetID != 0) {
                if (this.csv.PathfindingDot) {
                    Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
                }
            }

            if (this.csv.StopTrusteeship) {
                // 停止对应的挂机，抓宠行为
                Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                GameCenter.mPathFindControlSystem?.Interupt();

                Sys_Pet.Instance.SetClientStateAboutSeal(Sys_Pet.Instance.clientStateId, false);
                Sys_Pet.Instance.clientStateId = Sys_Role.EClientState.None;
                Sys_Pet.Instance.TryStopSeal();
            }

            if (this.csv.WayfindingOpenIncense) {
                Sys_Task.Instance.StopHangupTask();
            }

            if (this.csv.TaskAfterPerform != 0) {
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterInteractive);
                WS_TaskGoalManagerEntity.StartTaskGoal<WS_TaskGoalControllerEntity>(this.csv.TaskAfterPerform, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, () => {
                    GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);

                    this.InterruptDoing();
                    Sys_Task.Instance.TryContinueDoCurrentTask();
                }, true, (int)TaskGoalEnum.B_TaskGoalCompleted);
            }
            else {
                this.InterruptDoing();
                Sys_Task.Instance.TryContinueDoCurrentTask();
            }
        }

        public virtual void Refresh(uint crt) {
        }

        protected void BeginDoing() {
            this.isDoing = true;
        }
        public void InterruptDoing() {
            //this.isDoing = false;
            //Sys_Task.Instance.InterruptCurrentTaskDoing();
        }
        public void EndDoing() {
            this.isDoing = false;
        }

        public void TryDoExec(bool auto = true) {
            void StopAuto() {
                Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;

                if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.PlayerCtrl) {
                    GameCenter.mainHero?.movementComponent?.Stop();
                    ActionCtrl.Instance?.currentPlayerCtrlAction?.Interrupt();
                }

                if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto) {
                    if (ActionCtrl.Instance?.currentAutoAction != null && ActionCtrl.Instance.currentAutoAction is PathFindAction) {
                        GameCenter.mainHero?.movementComponent?.Stop();
                        ActionCtrl.Instance?.currentAutoAction?.Interrupt();
                    }
                }

                UIManager.CloseUI(Sys_PathFind.Instance.PathFindId);
            }

            DebugUtil.LogFormat(ELogType.eTask, "开始自动执行任务 Id:{0} finish {1}, autoImpl {2}, index:{3}, CanPathFind {4}", this.taskEntry.id, this.isFinish, this.autoImpl, this.goalIndex, this.CanPathFind);
            //DebugUtil.LogFormat(ELogType.eTask,

            if (this.isFinish) {
                return;
            }

            void DoNormal() {
                Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
                if (this.csv.PathfindingTargetID != 0) {
                    if (this.csv.PathfindingDot) {
                        Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnGenWayPoints, this.csv.PathfindingTargetID);
                    }
                }

                if (this.csv.WayfindingOpenIncense) {
                    Sys_Pet.Instance.SetClientStateAboutSeal(Sys_Role.EClientState.ExecTask, true);
                }

                // 自动执行并且允许自动寻路 或者 手动点击
                bool canPathFind = this.CanPathFind;
                DebugUtil.LogFormat(ELogType.eTask, "canPathFind {0}", canPathFind);
                if (canPathFind) {
                    // 是否该目标的所有依赖项 全被 完成
                    if (this.IsAlldependencyFinish(out int failTaskGoalIndex)) {
                        StopAuto();
                        if (this.CanAutoImpl(auto)) {
                            this.BeginDoing();
                            this.TryGuide();
                            this.DoExec(auto);
                        }
                        else {
                            DebugUtil.Log(ELogType.eTask, "不能自动执行");
                        }
                    }
                    else {
                        if (this.CanAutoImpl(auto)) {
                            string content = string.Format("依赖项 任务目标{0} 未完成", (failTaskGoalIndex + 1).ToString());
                            Sys_Hint.Instance.PushContent_Normal(content);
                            DebugUtil.LogFormat(ELogType.eTask, content);
                        }
                        else {
                            DebugUtil.Log(ELogType.eTask, "不能自动执行");
                        }
                    }
                }
                else {
                    if (this.CanAutoImpl(auto)) {
                        this.TryGuide();
                    }
                }

                if (this.csv.WayfindingTips != 0) {
                    string tips = LanguageHelper.GetTaskTextContent(this.csv.WayfindingTips);
                    if (tips != null) {
                        Sys_Hint.Instance.PushContent_Static(tips);
                    }
                }
            }

            if (this.csv.LimitTime == 0) {
                DoNormal();
            }
            else {
                if (this.endTime == 0) {
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(this.csv.LimitOpenNpc, true);
                    //Sys_Task.Instance.ReqStartTimeLimit(this.taskEntry.id, (uint)this.goalIndex);
                }
                else {
                    DoNormal();
                }
            }
        }

        protected void TryGuide() {
            CSVOpenUi.Data csvUI = CSVOpenUi.Instance.GetConfData(this.csv.TaskOpenUiId);
            if (csvUI != null) {
                // 大地图
                if (csvUI.Uiid == (int)EUIID.UI_Map) {
                    Vector3 pos = new Vector3(csvUI.ui_para1, csvUI.ui_para2, 0f);
                    UIManager.OpenUI(EUIID.UI_Map, false, new Sys_Map.TargetMapParameter(csvUI.ui_para, pos));
                }
                else if (csvUI.Uiid == (int)EUIID.UI_SkillUpgrade) {
                    List<int> arg = new List<int>() { (int)csvUI.ui_para, csvUI.ui_para1, csvUI.ui_para2 };
                    UIManager.OpenUI((int)csvUI.Uiid, false, arg);
                }
                else {
                    if (csvUI.ui_para != 0) {
                        UIManager.OpenUI((int)csvUI.Uiid, false, csvUI.ui_para);
                    }
                    else {
                        UIManager.OpenUI((int)csvUI.Uiid);
                    }
                }
            }

            if (this.csv.TaskOpenGuideId != null && this.csv.TaskOpenGuideId.Count >= 1) {
                if (this.csv.TaskOpenGuideId[0][0] == 0) {
                    Sys_Guide.Instance.TriggerGuideGroup(this.csv.TaskOpenGuideId[0][1]);
                }
                else {
                    uint careerId = (uint)GameCenter.mainHero.careerComponent.CurCarrerType;
                    for (int i = 0, length = this.csv.TaskOpenGuideId.Count; i < length; ++i) {
                        var ls = this.csv.TaskOpenGuideId[i];
                        if (careerId == ls[0]) {
                            Sys_Guide.Instance.TriggerGuideGroup(ls[1]);
                            break;
                        }
                    }
                }
            }
        }

        protected virtual void DoExec(bool auto = true) {
        }

        public bool CanAutoPathFind(bool auto) {
            return this.taskEntry.CanAutoPathFind(auto);
        }

        protected void NavToNpc(uint npcID, bool auto, bool needInteraction = true) {
            CSVNpc.Data csvNpc = CSVNpc.Instance.GetConfData(npcID);
            if (csvNpc == null) {
                this.InterruptDoing();
                return;
            }

            Action<Vector3> onComplete = (pos) => {
                DebugUtil.LogFormat(ELogType.eTask, "寻路成功 {0}", npcID.ToString());
                GameCenter.FindNearestNpc(npcID, out var targetNpc, out var guid, false);
                if (targetNpc != null) {
                    if (this.CanAutoImpl(auto)) {
                        List<ActionBase> actions = new List<ActionBase>();
                        PathFindAction pathFindAction = ActionCtrl.Instance.CreateAction(typeof(PathFindAction)) as PathFindAction;
                        if (pathFindAction != null) {
                            pathFindAction.targetPos = targetNpc.transform.position + new Vector3(targetNpc.cSVNpcData.dialogueParameter[0] / 10000f, targetNpc.cSVNpcData.dialogueParameter[1] / 10000f, targetNpc.cSVNpcData.dialogueParameter[2] / 10000f);
                            pathFindAction.tolerance = targetNpc.cSVNpcData.InteractiveRange / 10000f;
                            actions.Add(pathFindAction);

                            pathFindAction.Init(null, () => {
                                DebugUtil.LogFormat(ELogType.eTask, "interactiveWithNPC ======> {0} id: {1} needInteraction{2}", targetNpc.UID.ToString(), targetNpc.cSVNpcData.id.ToString(), needInteraction.ToString());
                                if (needInteraction) {
                                    List<ActionBase> innerActions = new List<ActionBase>();
                                    InteractiveWithNPCAction interactiveWithNPC = ActionCtrl.Instance.CreateAction(typeof(InteractiveWithNPCAction)) as InteractiveWithNPCAction;
                                    if (interactiveWithNPC != null) {
                                        innerActions.Add(interactiveWithNPC);
                                        interactiveWithNPC.npc = targetNpc;
                                        interactiveWithNPC.currentTaskEntry = this.taskEntry;
                                    }

                                    ActionCtrl.Instance.AddAutoActions(innerActions);
                                }
                                else {
                                    this.InterruptDoing();
                                }
                            }, () => {
                                this.InterruptDoing();
                            });

                            ActionCtrl.Instance.AddAutoActions(actions);
                        }
                        else {
                            this.InterruptDoing();
                        }
                    }
                    else {
                        this.InterruptDoing();
                    }
                }
                else {
                    DebugUtil.Log(ELogType.eTask, "Cant Find Npc!");
                    this.InterruptDoing();
                    return;
                }
            };

            bool canAutoPathFind = this.CanAutoPathFind(auto);
            DebugUtil.LogFormat(ELogType.eTask, "npcid:{0} autoPathFind:{1} auto：{2} autoImpl: {3}", npcID, this.taskEntry.autoPathFind, auto, this.autoImpl);
            if (canAutoPathFind) {
                DebugUtil.LogFormat(ELogType.eTask, "Can autoPathFind");

                Action action = () => {
                    if (auto) {
                        List<ActionBase> actions = new List<ActionBase>();
                        WaitSecondsAction waitSecondsAction = ActionCtrl.Instance.CreateAction(typeof(WaitSecondsAction)) as WaitSecondsAction;
                        if (waitSecondsAction != null) {
                            DebugUtil.LogFormat(ELogType.eTask, "waitSecondsAction");
                            actions.Add(waitSecondsAction);
                            waitSecondsAction.Init(null, () => {
                                DebugUtil.LogFormat(ELogType.eTask, "waitSecondsAction ======> {0}  {1}", GameCenter.mPathFindControlSystem != null, npcID);
                                Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                                GameCenter.mPathFindControlSystem?.FindNpc(npcID, onComplete, this.taskEntry.id);
                            });
                        }
                        ActionCtrl.Instance.AddAutoActions(actions);
                    }
                    else {
                        Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                        GameCenter.mPathFindControlSystem?.FindNpc(npcID, onComplete, this.taskEntry.id);
                    }
                };

                bool canOpenTips = Sys_SurvivalPvp.Instance.OpenTips(action);
                DebugUtil.LogFormat(ELogType.eTask, "canOpenTips: {0}", canOpenTips.ToString());
                if (!canOpenTips) {
                    action.Invoke();
                }
            }
            else {
                DebugUtil.LogFormat(ELogType.eTask, "表格配置无法寻路");
                this.InterruptDoing();
            }
        }

        private void OnFindFail() {
            this.InterruptDoing();
        }

        protected void PartrolNpc(uint teamId, uint mapId, bool auto) {
            if (this.CanAutoPathFind(auto)) {
                List<ActionBase> actions = new List<ActionBase>();
                WaitSecondsAction waitSecondsAction = ActionCtrl.Instance.CreateAction(typeof(WaitSecondsAction)) as WaitSecondsAction;
                if (waitSecondsAction != null) {
                    waitSecondsAction.Init(null, () => {
                        bool autoPathFind = this.taskEntry.autoPathFind;
                        bool autoImpl = this.autoImpl;
                        DebugUtil.LogFormat(ELogType.eTask, "去地图{0} 杀敌组{1}  自动寻路?{2} 自动实现?{3}", mapId, teamId, autoPathFind, autoImpl);
                        GameCenter.mPathFindControlSystem?.FindMonsterTeamId(teamId, mapId, (pos) => {
                            if (this.CanAutoImpl(auto)) {
                                DebugUtil.LogFormat(ELogType.eTask, "寻路已到达目标怪物 Over!");
                                if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal) {
                                    // 如果查找不到目标怪物，则继续寻路
                                    Sys_Task.Instance.ContinueDoTask(this.taskEntry);
                                }
                                else {
                                    this.InterruptDoing();
                                }
                            }
                            else {
                                this.InterruptDoing();
                            }
                        });
                    });

                    actions.Add(waitSecondsAction);
                }

                ActionCtrl.Instance.AddAutoActions(actions);
            }
            else {
                this.InterruptDoing();
            }
        }

        public void FindNearest(uint npcId, out Npc npc, out ulong guid)
        {
            float minDistance = float.MaxValue;
            npc = null;
            guid = 0;

            foreach (var kvp in GameCenter.npcs[npcId])
            {
                Npc tmpNpc = kvp.Value;
                Transform t = tmpNpc.transform;
                if (t != null /*&& kvp.Value.IsColliderEnable()*/)
                {
                    float min = Vector3.Magnitude(t.position - GameCenter.mainHero.transform.position);
                    if (min < minDistance)
                    {
                        //VisualComponent visualComponent = World.GetComponent<VisualComponent>(kvp.Value);
                        VisualComponent visualComponent = tmpNpc.VisualComponent;

                        if (visualComponent != null && visualComponent.Visiable)
                        {
#if false
                            //CollectionComponent cc = World.GetComponent<CollectionComponent>(kvp.Value);
                            CollectionComponent cc = tmpNpc.collectionComponent;

                            bool rlt = (cc != null && cc.IsPrivateCollectItem && cc.Count <= 0);
#else
                            CollectionNpc collectionNpc = kvp.Value as CollectionNpc;
                            bool rlt = (collectionNpc != null && collectionNpc.collectionComponent.IsPrivateCollectItem && collectionNpc.collectionComponent.Count <= 0);
#endif
                            if (!rlt)
                            {
                                minDistance = min;
                                npc = kvp.Value;
                                guid = kvp.Key;
                            }
                        }
                    }
                }
            }
        }
    }
}