namespace Logic
{
    /// <summary>
    /// 打卡功能///
    /// </summary>
    public class ClockInFunction : FunctionBase
    {
        protected override void OnExecute()
        {
            base.OnExecute();

            Sys_Hangup.Instance.SetWorkingHour();
        }
    }
}