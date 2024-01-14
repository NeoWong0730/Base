using Lib.Core;
using Table;

namespace Logic
{
    public class CookFunction : FunctionBase
    {
        public CSVCookFunction.Data CSVCookFunctionData
        {
            get;
            private set;
        }

        public override void Init()
        {
            CSVCookFunctionData = CSVCookFunction.Instance.GetConfData(ID);
        }

        protected override void OnDispose()
        {
            CSVCookFunctionData = null;

            base.OnDispose();
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVCookFunctionData == null)
            {
                DebugUtil.LogError($"CSVCookFunction.Data is null ID:{ID}");
                return false;
            }

            return true;
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            Sys_Cooking.Instance.StartCooking(ID);
        }
    }
}