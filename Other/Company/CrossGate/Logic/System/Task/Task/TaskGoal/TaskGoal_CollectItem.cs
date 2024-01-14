using System;
using System.Collections.Generic;
using Lib.Core;
using Table;
using UnityEngine;

namespace Logic {
    /// CollectItem = 3, // 物品收集
    public class TaskGoal_CollectItem : TaskGoal {

        Timer timer;
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = (int)csv.TargetParameter2;
            return base.Init(taskEntry, csv, goalIndex);
        }

        public override void Refresh(uint crt) {
            this.current = (int)crt;
        }

        public override string GetTaskContent() {
            string npcName = "";
            string mapName = "";
            uint npcId = this.csv.PathfindingTargetID;
            CSVNpc.Data csvNpc = CSVNpc.Instance.GetConfData(npcId);
            if (csvNpc != null) {
                npcName = LanguageHelper.GetNpcTextContent(csvNpc.name);
                uint mapId = csvNpc.mapId;
                CSVMapInfo.Data csvMap = CSVMapInfo.Instance.GetConfData(mapId);
                if (csvMap != null) {
                    mapName = LanguageHelper.GetTextContent(csvMap.name);
                }
            }
            else {
                DebugUtil.LogErrorFormat("{0} monster is not exist!", npcId);
            }

            int finalIndex = this.taskEntry.csvTask.conditionType ? 0 : this.goalIndex;
            return LanguageHelper.GetTaskTextContent(this.taskEntry.csvTask.taskContent[finalIndex], mapName, npcName, this.current.ToString(), this.limit.ToString());
        }
        private void OnFindFail() {
            this.InterruptDoing();
        }
        public override void OnFinished() {
            this.timer?.Cancel();
            base.OnFinished();
        }
        protected override void DoExec(bool auto = true) {
            Action onComplete = () => {
                uint npcId = this.csv.PathfindingTargetID;
                DebugUtil.LogFormat(ELogType.eTask, "去采集 {0}", npcId);
                if (!this.HasFindCollectPoint(npcId)) {
                    GameCenter.mPathFindControlSystem.FindNpc(npcId, (pos) => {
                        DebugUtil.LogFormat(ELogType.eTask, "寻路已到达目标采集点!");
                        this.NavTo(npcId, this.taskEntry, auto);
                    }/*, taskEntry.id*/);
                }
                else {
                    DebugUtil.LogFormat(ELogType.eTask, "直接采集，不到采集点中转!");
                    this.NavTo(npcId, this.taskEntry, auto);
                }
            };

            // 自动模式并且允许自动寻路 或者 手动模式
            if (this.CanAutoPathFind(auto)) {
                if (auto) {
                    // List<ActionBase> actions = new List<ActionBase>();
                    // WaitSecondsAction waitSecondsAction = ActionCtrl.Instance.CreateAction(typeof(WaitSecondsAction)) as WaitSecondsAction;
                    // if (waitSecondsAction != null) {
                    //     actions.Add(waitSecondsAction);
                    //     waitSecondsAction.Init(null, () => {
                    //         onComplete?.Invoke();
                    //     });
                    // }
                    // ActionCtrl.Instance.AddAutoActions(actions);
                    
                    this.timer?.Cancel();
                    this.timer = Timer.Register(1f, () => {
                        onComplete?.Invoke();
                    });
                }
                else {
                    onComplete?.Invoke();
                }
            }
            else {
                this.InterruptDoing();
            }
        }

        private bool HasFindCollectPoint(uint npcId) {
            return GameCenter.npcs.TryGetValue(npcId, out var dict) && dict.Count >= 1;
        }
        private void NavTo(uint npcId, TaskEntry taskEntry, bool auto) {
            List<ActionBase> actions = new List<ActionBase>();
            if (GameCenter.npcs.ContainsKey(npcId) && GameCenter.npcs[npcId].Count > 0) {
                Npc targetNpc = null;
                ulong guid = 0;
                this.FindNearest(npcId, out targetNpc, out guid);

                DebugUtil.LogFormat(ELogType.eTask, "找到的最近的npc uid {0}", guid);
                if (targetNpc != null) {
                    PathFindAction findAction = ActionCtrl.Instance.CreateAction(typeof(PathFindAction)) as PathFindAction;
                    if (findAction != null) {
                        actions.Add(findAction);
                        findAction.targetPos = targetNpc.transform.position;
                        findAction.tolerance = targetNpc.cSVNpcData.InteractiveRange / 10000f;
                        findAction.Init(null, () => {
                            actions.Clear();
                            DebugUtil.LogFormat(ELogType.eTask, "寻路已到达 确切 目标采集点!");
                            if (targetNpc != null && targetNpc.gameObject != null) {
                                GameCenter.mainHero.movementComponent?.Stop();
                                GameCenter.mainHero.transform.LookAt(targetNpc.gameObject.transform.position, Vector3.up);
                            }
                            // 防止寻路过去的过程中，npc被杀消失
                            if (GameCenter.npcs.ContainsKey(npcId) && GameCenter.npcs[npcId].ContainsKey(guid)) {
                                InteractiveWithNPCAction interactionAction = ActionCtrl.Instance.CreateAction(typeof(InteractiveWithNPCAction)) as InteractiveWithNPCAction;
                                if (interactionAction != null) {
                                    actions.Add(interactionAction);
                                    interactionAction.npc = GameCenter.npcs[npcId][guid];
                                    interactionAction.currentTaskEntry = taskEntry;
                                }

                                ActionCtrl.Instance.AddAutoActions(actions);
                            }
                            else {
                                DebugUtil.LogFormat(ELogType.eTask, "找不到采集物，可能死亡/消失");
                                this.InterruptDoing();
                                // 递归下去
                                Sys_Task.Instance.ContinueDoTask(taskEntry);
                            }
                        });
                    }
                }
                else {
                    // 递归下去
                    DebugUtil.LogFormat(ELogType.eTask, "找不到采集物，可能死亡/消失");
                    this.InterruptDoing();
                    Sys_Task.Instance.ContinueDoTask(taskEntry);
                }
            }
            else {
                DebugUtil.LogFormat(ELogType.eTask, "{0} 对应的guid 采集物个数 <= 0", npcId);
                this.InterruptDoing();
                
                // 递归下去
                WaitSecondsAction waitSecondsAction = ActionCtrl.Instance.CreateAction(typeof(WaitSecondsAction)) as WaitSecondsAction;
                if (waitSecondsAction != null) {
                    waitSecondsAction.seconds = 2.3f;
                    actions.Add(waitSecondsAction);
                    waitSecondsAction.Init(null, () => {
                        Sys_Task.Instance.ContinueDoTask(taskEntry);
                    });
                }
            }

            ActionCtrl.Instance.AddAutoActions(actions);
        }
    }
}