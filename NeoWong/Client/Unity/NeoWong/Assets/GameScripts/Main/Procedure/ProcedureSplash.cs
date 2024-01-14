using ProcedureOwner = NWFramework.IFsm<NWFramework.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 闪屏
    /// </summary>
    public class ProcedureSplash : ProcedureBase
    {
        public override bool UseNativeDialog => true;

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            //播放 Splash 动画


            ChangeState<ProcedureInitPackage>(procedureOwner);
        }
    }
}