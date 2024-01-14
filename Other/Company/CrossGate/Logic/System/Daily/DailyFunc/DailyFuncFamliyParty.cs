using Logic.Core;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{

    public class DailyFuncFamliyParty : DailyFunc
    {

        public override bool OnJoin()
        {
            
            return Sys_Family.Instance.GoToFamilyParty();
        }
    }
}
