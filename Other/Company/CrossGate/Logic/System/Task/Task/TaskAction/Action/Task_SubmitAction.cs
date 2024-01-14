using Table;
using Logic.Core;

namespace Logic
{
    public class Task_SubmitAction : ActionBase
    {
        //public const string TypeName = "Logic.Task_SubmitAction";
        public TaskEntry taskEntry;

        protected override void OnDispose()
        {
            this.taskEntry = null;

            base.OnDispose();
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            if (this.taskEntry != null)
            {
                Sys_Task.Instance.TryReqSubmit(this.taskEntry.id);
            }
        }

        public override bool IsCompleted()
        {
            return true;
        }
    }
}
