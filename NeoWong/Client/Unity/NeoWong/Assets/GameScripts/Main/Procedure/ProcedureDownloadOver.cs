using NWFramework;
using ProcedureOwner = NWFramework.IFsm<NWFramework.IProcedureManager>;

namespace GameMain
{
    /// <summary>
    /// 流程 => 下载完成
    /// </summary>
    public class ProcedureDownloadOver : ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        private bool _needClearCache;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Log.Info("下载完成!!!");

            UILoadMgr.Show(UIDefine.UILoadUpdate, $"下载完成...");
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (_needClearCache)
            {
                ChangeState<ProcedureClearCache>(procedureOwner);
            }
            else
            {
                ChangeState<ProcedureLoadAssembly>(procedureOwner);
            }
        }
    }
}