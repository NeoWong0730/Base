namespace Logic
{
    public class FamilyBattleFunction : FunctionBase
    {
        protected override void OnExecute()
        {
            Sys_FamilyResBattle.Instance.ReqEnterBattleField();
        }
    }
}
