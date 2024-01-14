using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件：女神试炼///
    /// </summary>
    public class GodTestCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.GodTest;
            }
        }

        uint stageID;
        uint paramID;

        public override void DeserializeObject(List<int> data)
        {
            stageID = (uint)data[0];
            paramID = (uint)data[1];
        }

        public override bool IsValid()
        {
            //Todo
            return true;
        }

        protected override void OnDispose()
        {
            stageID = 0;
            paramID = 0;
        }
    }
}
