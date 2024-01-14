using Table;
using Lib.Core;

namespace Logic
{
    /// <summary>
    /// 提交物品功能///
    /// </summary>
    public class SubmitItemFunction : FunctionBase
    {
        public CSVSubmit.Data CSVSubmitData
        {
            get;
            private set;
        }

        public override void Init()
        {
            CSVSubmitData = CSVSubmit.Instance.GetConfData(ID);
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVSubmitData == null)
            {
                DebugUtil.LogError($"CSVSubmit.Data is Null, id: {ID}");
                return false;
            }
            return true;
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            Sys_SubmitItem.SubmitData submitData = new Sys_SubmitItem.SubmitData();
            submitData.CsvSubmitID = ID;
            submitData.FunctionSourceType = FunctionSourceType;
            submitData.FunctionHandlerID = HandlerID;
            submitData.TaskId = HandlerID;
            submitData.npcUID = npc.uID;

            Sys_SubmitItem.Instance.OpenUI(submitData);
        }

        protected override void OnDispose()
        {
            CSVSubmitData = null;

            base.OnDispose();
        }
    }
}