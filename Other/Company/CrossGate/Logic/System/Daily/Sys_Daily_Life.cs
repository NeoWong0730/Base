using System;
using System.Collections.Generic;
using System.Linq;
using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public partial class Sys_Daily : SystemModuleBase<Sys_Daily>
    {
        private uint NextRefreshTimeToServer = 0;

        private void SetNextRefreshTime(uint time)
        {
            NextRefreshTimeToServer = time + 86400;
        }

        private void OnOverRefreshTime()
        {
            NextRefreshTimeToServer = NextRefreshTimeToServer + 86400;

            Apply_InfoReq();

            ReGetAllWork();
        }

        private void OnUpdataLife(uint nowtime)
        {
            if (NextRefreshTimeToServer == 0)
                return;

            if (nowtime >= NextRefreshTimeToServer)
            {
                OnOverRefreshTime();
            }
        }
    }


}
