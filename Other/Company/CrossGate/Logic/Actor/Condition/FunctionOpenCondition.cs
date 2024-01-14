using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:功能推送///
    /// </summary>
    public class FunctionOpenCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.FunctionOpen;
            }
        }

        uint functionID;
        
        public override void DeserializeObject(List<int> data)
        {
            functionID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(functionID, false))
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            functionID = 0;
        }
    }
}
