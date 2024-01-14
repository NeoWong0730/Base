namespace Logic
{
    /// <summary>
    /// 任务进入区域///
    /// </summary>
    public class TaskEnterAreaFunction : FunctionBase
    {
        protected override void OnExecute()
        {
            base.OnExecute();

            Sys_Task.Instance.ReqStepGoalFinish(this.HandlerID, this.HandlerIndex, 0, 0);
        }
    }
}