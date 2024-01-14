using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:通过关卡///
    /// </summary>
    public class PassStageCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.PassStage;
            }
        }

        uint stageID;
        uint param;

        public override void DeserializeObject(List<int> data)
        {
            stageID = (uint)data[0];
            param = (uint)data[1];
        }

        public override bool IsValid()
        {
            if (param == 0)
            {
                if (Sys_Instance.Instance.IsPassStage(stageID))
                {
                    return false;
                }
                return true;
            }
            else
            {
                if (Sys_Instance.Instance.IsPassStage(stageID))
                {
                    return true;
                }
                return false;
            }
        }

        protected override void OnDispose()
        {
            stageID = 0;
        }
    }
}
