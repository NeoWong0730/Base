using System.Collections.Generic;
using Client;
using Common;
using Lib.Core;
using Logic.Core;
using Net;

namespace Logic {
    public partial class Sys_Task {
        public enum ETaskStatus {
            UnReceived, // 未接受 并且 接取条件不满足
            UnReceivedButCanReceive, // 未接受 但是 条件满足可以去接受
            UnFinished, // 已接收 但是 未完成
            Submited, // 已提交
        }

        public class TaskTarget {
            public uint id;
            public int seqId;

            public int limit;
            public int current;
        }

        public class TaskEntry {
            public uint id;
            public ETaskStatus status = ETaskStatus.UnReceived;
        }
    }

    public partial class Sys_Task : SystemModuleBase<Sys_Task> {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        private readonly SystemModuleMsgProcesser _msgProcesser = new SystemModuleMsgProcesser(enClientFirst.Task);

        public enum EEvents {
            OnRefreshAll,
        }

        public override void Init() {
            _msgProcesser.Listen((ushort)enSecondTask.ReqAccept, (ushort)enSecondTask.NtfAccept, this.OnAccept, SecondTask_Ntf_Accept.Parser, true);
            _msgProcesser.Listen((ushort)enSecondTask.ReqCommit, (ushort)enSecondTask.NtfFinish, this.OnCommit, SecondTask_Ntf_Finish.Parser, true);
            _msgProcesser.Listen(0, (ushort)enSecondTask.NtfUpdate, this.OnUpdateTask, SecondTask_Ntf_Update.Parser, true);
        }

        public void SetData(ModuleTask task) {
            if (task == null) {
                return;
            }

            acceptedTasks.Clear();
            finishedTasks.Clear();

            for (int i = 0, length = task.Tasks.Count; i < length; ++i) {
                var oneTask = task.Tasks[i];
                TaskEntry entry = new TaskEntry() {
                    id = oneTask.Taskid
                };
                acceptedTasks.Add(entry.id, entry);
            }
        }

        public void ReqAccept(uint taskId) {
            var req = new SecondTask_Req_Accept();
            req.Taskid = taskId;
            _msgProcesser.SendMessage((int)(enSecondTask.ReqAccept), req);
        }

        private void OnAccept(NetMsg msg) {
            var res = msg.data as SecondTask_Ntf_Accept;
        }

        public void ReqCommit(uint taskId) {
            var req = new SecondTask_Req_Commit();
            req.Taskid = taskId;
            _msgProcesser.SendMessage((int)(enSecondTask.ReqCommit), req);
        }

        private void OnCommit(NetMsg msg) {
            var res = msg.data as SecondTask_Ntf_Finish;
        }

        private void OnUpdateTask(NetMsg msg) {
            var res = msg.data as SecondTask_Ntf_Update;
        }
    }

    public partial class Sys_Task {
        public Dictionary<uint, TaskEntry> acceptedTasks = new Dictionary<uint, TaskEntry>();
        public List<uint> finishedTasks = new List<uint>();

        public TaskEntry GetTask(uint taskId) {
            acceptedTasks.TryGetValue(taskId, out var task);
            return task;
        }

        public ETaskStatus GetTaskStatus(uint taskId) {
            return ETaskStatus.UnReceived;
        }
    }
}