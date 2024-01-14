using Lib.Core;
using Table;
using Net;
using Packet;

namespace Logic
{
    /// <summary>
    /// 功能 传送///
    /// </summary>
    public class TransmitFunction : FunctionBase
    {
        public CSVDeliver.Data CSVDeliverData
        {
            get;
            private set;
        }

        public override void Init()
        {
            CSVDeliverData = CSVDeliver.Instance.GetConfData(ID);
        }

        protected override void OnDispose()
        {
            CSVDeliverData = null;

            base.OnDispose();
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVDeliverData == null)
            {
                DebugUtil.LogError($"CSVDeliver.Data is null ID:{ID}");
                return false;
            }

            return true;
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            CmdMapRoleChgMapReq req = new CmdMapRoleChgMapReq();
            req.BUi = true;
            req.Teleporter = ID;

            NetClient.Instance.SendMessage((ushort)CmdMap.RoleChgMapReq, req);
        }
    }
}
