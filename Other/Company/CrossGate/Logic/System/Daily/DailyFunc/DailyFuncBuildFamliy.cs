using Logic.Core;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{

    public class DailyFuncBuildFamliy : DailyFunc
    {

        public override bool OnJoin()
        {
            UIManager.CloseUI(EUIID.UI_DailyActivites);
            UIManager.OpenUI(EUIID.UI_Family_Construct, true);

            return false;
        }

        public override bool HadUsedTimes()
        {
            var count = Sys_Bag.Instance.GetItemCount(17u);

            if (count > 0)
                return false;

            bool result = base.HadUsedTimes();

            return result;
        }
    }
}
