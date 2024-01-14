using Packet;
using Net;

namespace Logic
{
    /// <summary>
    /// 酒吧事件功能///
    /// </summary>
    public class BarFunction : FunctionBase
    {
        protected override void OnExecute()
        {
            base.OnExecute();

            CmdBarRefresghReq cmdBarRefresghReq = new CmdBarRefresghReq();
            NetClient.Instance.SendMessage((ushort)CmdTask.CmdBarRefresghReq, cmdBarRefresghReq);            
        }

        public override bool IsValid()
        {
            return base.IsValid() && Sys_Daily.Instance.IsPubDailyReady();
        }
    }
}
