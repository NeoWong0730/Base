using Table;
using Logic.Core;
using Lib.Core;
using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 任务功能///
    /// </summary>
    public class TaskFunction : FunctionBase
    {
        public ETaskState State
        {
            get;
            set;
        }

        public uint IconID
        {
            get;
            set;
        }

        public CSVTask.Data CSVTaskData
        {
            get;
            private set;
        }

        public override void Init()
        {
            CSVTaskData = CSVTask.Instance.GetConfData(ID);
        }

        public override void DeserializeObject(List<uint> strs, bool taskCreate = false)
        {
            base.DeserializeObject(strs, taskCreate);

            State = (ETaskState)strs[8];
            IconID = strs[9];
        }

        public override void CreateConditions()
        {
            ConditionIDs = new List<uint>();
            if (State == ETaskState.UnReceived)
            {
                ConditionIDs.Add((uint)EConditionType.TaskUnReceived);
            }
            else if (State == ETaskState.Finished)
            {
                ConditionIDs.Add((uint)EConditionType.TaskCompleted);
            }

            ConditionValues = new Dictionary<uint, List<int>>();
            List<int> values = new List<int>();
            values.Add((int)HandlerID);
            ConditionValues.Add(ConditionIDs[0], values);

            if (State == ETaskState.UnReceived)
            {
                ConditionIDs.Add((uint)EConditionType.DialogueChoose);

                List<int> values2 = new List<int>();
                values2.Add((int)CSVTaskData.TalkChooseId);
                ConditionValues.Add(ConditionIDs[1], values2);
            }
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVTaskData == null)
            {
                DebugUtil.LogError($"CSVTask.Data is null ID:{ID}");
                return false;
            }

            ///组队状态下如果是队员且非暂离不能执行///
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain() && !Sys_Team.Instance.isPlayerLeave())
            {
                return false;
            }

            return true;
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            if (State == ETaskState.UnReceived)
            {               
                Sys_Task.Instance.ReqReceive(ID);
            }
            else if (State == ETaskState.Finished)
            {
                if (CSVTaskData != null && CSVTaskData.DropId.Count > 1)
                {
                    UIManager.OpenUI(Logic.EUIID.UI_TaskSelectBox, true, ID);
                }
                else
                {
                    Sys_Task.Instance.TryReqSubmit(ID);
                }
            }        
        }

        public override bool IsValid()
        {
            return TaskHelper.IsMatchCsvCondition(State, CSVTaskData) && base.IsValid();
        }

        protected override void OnDispose()
        {
            State = ETaskState.UnReceived;
            IconID = 0;
            CSVTaskData = null;

            base.OnDispose();
        }
    }
}
