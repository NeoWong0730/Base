using Lib.Core;
using Table;

namespace Logic
{
    /// <summary>
    /// 密语功能///
    /// </summary>
    public class SecretMessageFunction : FunctionBase
    {
        public CSVCode.Data CSVCodeData
        {
            get;
            private set;
        }

        public override void Init()
        {
            CSVCodeData = CSVCode.Instance.GetConfData(ID);
        }

        protected override void OnDispose()
        {
            CSVCodeData = null;

            base.OnDispose();
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVCodeData == null)
            {
                DebugUtil.LogError($"CSVCode.Data is null ID:{ID}");
                return false;
            }

            if (CSVCodeData.condition != 0)
            {
                if (!CSVCheckseq.Instance.GetConfData(CSVCodeData.condition).IsValid())
                {                    
                    return false;
                }
            }

            return true;
        }

        protected override void OnCantExecTip()
        {
            //Sys_Hint.Instance.PushContent_Normal("不符合密语可执行条件，请策划配置具体提示");
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            Sys_SecretMessage.Instance.OpenSecretMessage(CSVCodeData);
        }

        public override bool IsValid()
        {
            return base.IsValid() && (!Sys_SecretMessage.Instance.completedSecretMessageID.Contains(ID)) && Sys_FunctionOpen.Instance.IsOpen(20901, false);
        }
    }
}
