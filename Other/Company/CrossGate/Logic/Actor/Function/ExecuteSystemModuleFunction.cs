using Table;
using Lib.Core;
using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 执行系统功能功能///
    /// </summary>
    public class ExecuteSystemModuleFunction : FunctionBase
    {
        public CSVFunctionOpen.Data CSVFunctionOpenData
        {
            get;
            private set;
        }

        public override void Init()
        {
            CSVFunctionOpenData = CSVFunctionOpen.Instance.GetConfData(ID);           
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVFunctionOpenData == null)
            {
                DebugUtil.LogError($"CSVFunctionOpen.Data is Null, id: {ID}");
                return false;
            }

            if (!Sys_FunctionOpen.Instance.IsOpen(CSVFunctionOpenData.id, true))
            {
                return false;
            }

            //if (CSVFunctionOpenData.id == 20207)
            //{
            //    return Sys_Pvp.Instance.IsPvpActive();
            //}

            return true;
        }

        public override bool IsValid()
        {
            if (CSVFunctionOpenData != null && CSVFunctionOpenData.id == 20207)
            {
                return Sys_Pvp.Instance.IsPvpActive() && base.IsValid();
            }
            return base.IsValid();
        }

        protected override void OnExecute()
        {
            base.OnExecute();
           
            UIManager.OpenUI((EUIID)CSVFunctionOpenData.FunctionUiId, true, CSVFunctionOpenData.FunctionSonId);
        }

        protected override void OnDispose()
        {
            CSVFunctionOpenData = null;

            base.OnDispose();
        }
    }
}
