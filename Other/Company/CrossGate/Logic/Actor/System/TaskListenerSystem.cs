using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic
{
    public class TaskListenerSystem : LevelSystemBase
    {
        public class Element : IReset
        {
            public TaskEntry taskEntry;
            public StateChecker checkerA_MainSub;
            public StateChecker checkerT_MainSub;

            public Element()
            {
                this.checkerA_MainSub = new StateChecker(false, this.OnCheckerA, this.OnValueChanged_CheckerA, 1);
                this.checkerT_MainSub = new StateChecker(false, this.OnCheckerT, this.OnValueChanged_CheckerT, 1);
            }

            private void OnValueChanged_CheckerA(bool oldFlag, bool newFlag)
            {
                var entry = this.taskEntry;
                if (newFlag)
                {
                    if (CanReceive(entry, entry.csvTask))
                    {
                        if (!TaskHelper.HasReceived(entry.id))
                        {
                            Sys_Task.Instance.ReqReceive(entry.id, true);
                        }
                    }
                }
            }

            private void OnValueChanged_CheckerT(bool oldFlag, bool newFlag)
            {
                var entry = this.taskEntry;
                bool isAccepted = TaskHelper.HasReceived(entry.id);
                if (newFlag)
                {
                    if (isAccepted)
                    {
                        bool isTracked = TaskHelper.HasTracked(this.taskEntry.id);
                        if (!isTracked)
                        {
                            Sys_Task.Instance.ReqTrace(entry.id, true, false);
                        }
                    }
                }
                else
                {
                    if (isAccepted && this.taskEntry.csvTask.cancelTrace)
                    {
                        bool isTracked = TaskHelper.HasTracked(this.taskEntry.id);
                        if (isTracked)
                        {
                            Sys_Task.Instance.ReqTrace(entry.id, false, false);
                        }
                    }
                }
            }

            private bool OnCheckerA()
            {
                bool rlt = this.taskEntry.IsInMainMapAcceptArea();
                // rlt |= this.taskEntry.IsInSubMapAcceptArea();
                return rlt;
            }
            private bool OnCheckerT()
            {
                bool rlt = this.taskEntry.IsInMainMapTriggerArea();
                rlt |= this.taskEntry.IsInSubMapTraceArea();
                return rlt;
            }

            public void Reset()
            {
            }

            private bool CanReceive(TaskEntry taskEntry, CSVTask.Data taskData)
            {
                var state = Sys_Task.Instance.GetTaskState(taskData.id);
                return (state == ETaskState.UnReceivedButCanReceive) || (state == ETaskState.Submited && taskData.TaskWhetherRepeat);
            }
        }

        private float lastTime = 0;
        private readonly float cd = 1.9f;

        private List<CSVTask.Data>  lastMapTaskDatas = new List<CSVTask.Data>();
        private List<CSVTask.Data>  curTaskDatas;
        private readonly ReuseableList<Element> curMapCheckers = new ReuseableList<Element>();
        private readonly List<TaskEntry> entries = new List<TaskEntry>();

        public override void OnCreate() {
            this.ProcessEvents(true);
            this.UpdateDistanceTaskInfos();            
        }

        public override void OnDestroy() {
            this.ProcessEvents(false);

            this.lastMapTaskDatas.Clear();
            this.curMapCheckers.Clear();
            this.entries.Clear();
        }
        
        private void ProcessEvents(bool toRegist)
        {
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, this.OnFuncOpen, toRegist);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnHeroTel, OnHeroTel, toRegist);
        }

        private void OnFuncOpen(Sys_FunctionOpen.FunctionOpenData data)
        {
            this.UpdateDistanceTaskInfos();
            this.CheckTask(true);
        }

        private void OnHeroTel() {
            UpdateDistanceTaskInfos();
        }

        public override void OnUpdate()
        {
            if (Time.unscaledTime < lastTime)
                return;

            this.CheckTask(false);
        }

        // 切换地图调用
        private void UpdateDistanceTaskInfos()
        {
            this.CheckTask();

            this.curTaskDatas = Sys_Task.Instance.GetTasksByMapId(Sys_Map.Instance.CurMapId);

            int length = this.curTaskDatas.Count;
            this.entries.Clear();
            for (int i = 0; i < length; i++)
            {
                TaskEntry taskEntry = Sys_Task.Instance.GetTask(this.curTaskDatas[i].id);
                if (taskEntry != null)
                {
                    this.entries.Add(taskEntry);
                }
            }

            void OnRefresh(Element element, int index)
            {
                element.taskEntry = this.entries[index];                
            }

            length = this.entries.Count;
            for (int i = 0; i < length; ++i)
            {
                this.curMapCheckers.TryBuildOrRefresh(i + 1, OnRefresh);
            }

            this.lastMapTaskDatas = Sys_Task.Instance.GetTasksByMapId(Sys_Map.Instance.LastMapId);
            this.UnTraceDistanceTasks();
        }

        // 清除上一个地图的任务
        private void UnTraceDistanceTasks()
        {
            //for (int i = 0, length = this.lastMapTaskDatas.Count; i < length; ++i) {
            //    var taskdata = this.lastMapTaskDatas[i];
            //    TaskEntry taskEntry = Sys_Task.Instance.GetTask(taskdata.id);
            //    if (taskEntry != null) {
            //        bool inArea = taskEntry.IsInAnyArea();
            //        if (taskEntry.csvTask.cancelTrace && !inArea) {
            //            // todo: 这里都考虑了 taskEntry.csvTask.cancelTrace != 0
            //            bool isTracked = TaskHelper.HasTracked(taskEntry.id);
            //            if (isTracked) {
            //                Sys_Task.Instance.ReqTrace(taskdata.id, false);
            //            }
            //        }
            //    }
            //}
        }

        private void CheckTask(bool focrce = false)
        {
            this.lastTime = Time.unscaledTime + cd;

            // 检测当前地图任务
            int length = this.curMapCheckers.RealCount - 1;
            for (int i = length; i >= 0; i--)
            {
                this.curMapCheckers[i]?.checkerA_MainSub.Check(focrce);
            }
            for (int i = length; i >= 0; i--)
            {
                this.curMapCheckers[i]?.checkerT_MainSub.Check(focrce);
            }

            if (Sys_Task.Instance.toBeSubmitTasks.Count > 0)
            {
                for (int i = Sys_Task.Instance.toBeSubmitTasks.Count - 1; i >= 0; --i)
                {
                    Sys_Task.Instance.toBeSubmitTasks[i].ForceSubmitTask();
                }
            }
        }
    }
}