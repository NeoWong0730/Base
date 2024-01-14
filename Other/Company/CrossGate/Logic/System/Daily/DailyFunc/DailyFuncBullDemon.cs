using Logic.Core;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{
    /// <summary>
    /// 牛魔来袭
    /// </summary>
    public class DailyFuncBullDemon : DailyFunc
    {

        public override bool OnJoin()
        {
            //Sys_Map.Instance.ReqChangeMap(1,true);


           // Sys_Map.Instance.ReqTelNpc(1001201, 1001, 0);


          //  GameCenter.mPathFindControlSystem.FindNpc(1001201, (pos) => { });

            return base.OnJoin();
        }
    }
}
