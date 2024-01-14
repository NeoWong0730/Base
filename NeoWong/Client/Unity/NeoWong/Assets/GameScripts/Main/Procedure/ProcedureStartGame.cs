using Cysharp.Threading.Tasks;
using NWFramework;

namespace GameMain
{
    /// <summary>
    /// 流程 => 开始游戏
    /// </summary>
    public class ProcedureStartGame : ProcedureBase
    {
        public override bool UseNativeDialog { get; }

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            StartGame().Forget();
        }

        private async UniTaskVoid StartGame()
        {
            await UniTask.Yield();
            UILoadMgr.HideAll();
        }
    }
}