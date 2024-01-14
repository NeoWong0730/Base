using Table;
using Lib.Core;

namespace Logic
{
    /// <summary>
    /// 头目挑战功能///
    /// </summary>
    public class ClassicBossFunction : FunctionBase
    {
        public CSVClassicBoss.Data CSVClassicBossData
        {
            get;
            private set;
        }

        public override void Init()
        {
            CSVClassicBossData = CSVClassicBoss.Instance.GetConfData(ID);
        }

        protected override void OnDispose()
        {
            CSVClassicBossData = null;

            base.OnDispose();
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVClassicBossData == null)
            {
                DebugUtil.LogError($"CSVClassicBoss.Data is null ID:{ID}");
                return false;
            }

            return true;
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            Sys_ClassicBossWar.Instance.GotoClassicBossWar(CSVClassicBossData.id, npc.uID);
        }

    }
}
