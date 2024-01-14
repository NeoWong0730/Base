namespace Logic
{
    public class ReciveFamilyTaskFunction : FunctionBase
    {
        protected override void OnExecute()
        {
            Sys_Family.Instance.TryGetConstructTask(ID);
        }
    }
}