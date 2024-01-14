using Table;
using System.Collections.Generic;
using Lib.Core;

namespace Logic
{
    /// <summary>
    /// 功能组功能///
    /// </summary>
    public class GroupFunction : FunctionBase
    {
        public List<FunctionBase> Functions = new List<FunctionBase>(8);

        public override void DeserializeObject(List<uint> strs, bool taskCreate = false)
        {
            base.DeserializeObject(strs, taskCreate);

            CSVFunctionGroup.Data cSVFunctionGroupData = CSVFunctionGroup.Instance.GetConfData(strs[1]);
            if (cSVFunctionGroupData.functionGroupData != null && cSVFunctionGroupData.functionGroupData.Count > 0)
            {
                Functions.Clear();
                for (int index = 0, len = cSVFunctionGroupData.functionGroupData.Count; index < len; index++)
                {
                    var re = DesrializeFunction(cSVFunctionGroupData.functionGroupData[index]);
                    if (re != null)
                    {
                        Functions.Add(re);
                    }
                }
            }
        }

        public FunctionBase DesrializeFunction(List<uint> strs)
        {
            FunctionBase functionBase = FunctionManager.CreateFunction((EFunctionType)strs[0]);

            if (functionBase != null)
            {
                functionBase.DeserializeObject(strs);
                functionBase.Init();
            }
            else
            {
                DebugUtil.LogError($"FunctionBase {((EFunctionType)(strs[0])).ToString()} 未解析");
            }
            return functionBase;
        }

        protected override void OnExecute()
        {
            UI_NPC.eventEmitter.Trigger<List<FunctionBase>>(UI_NPC.EEvents.OnClickGroupFunction, Functions);
        }

        protected override void OnDispose()
        {
            for (int index = 0, len = Functions.Count; index < len; index++)
            {
                Functions[index].Dispose();
            }
            Functions.Clear();

            base.OnDispose();
        }
    }
}
