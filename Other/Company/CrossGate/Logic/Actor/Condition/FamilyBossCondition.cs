using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:牛鬼来袭（家族boss）是否开启
    /// </summary>
    public class FamilyBossCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.HaveTask;
            }
        }

        public override void DeserializeObject(List<int> data)
        {
            //taskID = (uint)data[0];
        }

        public override bool IsValid()
        {
            return Sys_Daily.Instance.isDailyReady(112, false);
        }

        protected override void OnDispose()
        {
            //taskID = 0;
        }
    }
}
