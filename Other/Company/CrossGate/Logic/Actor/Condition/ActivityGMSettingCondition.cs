using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 活动紧急开关///
    /// </summary>
    public class ActivityGMSettingCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.ActivityGMSetting;
            }
        }

        uint ActivityID;

        public override void DeserializeObject(List<int> data)
        {
            ActivityID = (uint)data[0];
        }

        public override bool IsValid()
        {
            return Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(ActivityID);
        }

        protected override void OnDispose()
        {
            ActivityID = 0;
        }
    }

}