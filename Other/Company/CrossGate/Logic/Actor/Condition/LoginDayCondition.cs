using System.Collections.Generic;
using Framework;

namespace Logic
{
    /// <summary>
    /// 条件:活跃天数///
    /// </summary>
    public class LoginDayCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.LoginDay;
            }
        }

        uint day;

        public override void DeserializeObject(List<int> data)
        {
            day = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Role.Instance.LoginDay>=day)
            {
                return true;
            }
            return false;
            //if (Sys_Role.Instance.LoginDay + TimeManager.GetOffestDay(Sys_Time.Instance.GetServerTime(), TimeManager._syncServeTime) >= day)
            //{
            //    return true;
            //}
            //return false;
        }

        protected override void OnDispose()
        {
            day = 0;
        }
    }
}
