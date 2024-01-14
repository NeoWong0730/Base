using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using pbc = global::Google.Protobuf.Collections;
using Net;
using Packet;
using Table;

namespace Logic {
    public class Sys_FuncPreview : SystemModuleBase<Sys_FuncPreview> {
        public enum EEvents {
            OnDel,
        }
        
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public override void Init() {
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdRole.FunctionViewListNtf, this.OnReceiveAll, CmdRoleFunctionViewListNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdRole.FunctionViewReq, (ushort) CmdRole.FunctionViewRes, this.OnReceive, CmdRoleFunctionViewRes.Parser);
        }

        private List<uint> ids = new List<uint>();

        public List<uint> GetUnReadedIds(bool toSort = false) {
            this.ids.Clear();
            for (int i = 0, length = CSVFunForeshow.Instance.Count; i < length; ++i) {
                var line = CSVFunForeshow.Instance.GetByIndex(i);
                if (this.funcList == null || !this.funcList.Contains(line.id)) {
                    this.ids.Add(line.id);
                }
            }

            if (toSort) {
                this.ids.Sort((l, r) => {
                    return (int)((long)CSVFunForeshow.Instance.GetConfData(l).Priority -(long)CSVFunForeshow.Instance.GetConfData(r).Priority);
                });
            }

            return this.ids;
        }

        private pbc::RepeatedField<uint> funcList;
        private void OnReceiveAll(NetMsg msg) {
            CmdRoleFunctionViewListNtf response = NetMsgUtil.Deserialize<CmdRoleFunctionViewListNtf>(CmdRoleFunctionViewListNtf.Parser, msg);
            this.funcList = response.FuncList;
        }

        public void ReqRead(uint id) {
            CmdRoleFunctionViewReq req = new CmdRoleFunctionViewReq();
            req.FuncId = id;
            NetClient.Instance.SendMessage((ushort) CmdRole.FunctionViewReq, req);
        }

        private void OnReceive(NetMsg msg) {
            CmdRoleFunctionViewRes response = NetMsgUtil.Deserialize<CmdRoleFunctionViewRes>(CmdRoleFunctionViewRes.Parser, msg);
            this.funcList?.Add(response.FuncId);
            this.eventEmitter.Trigger<uint>(EEvents.OnDel, response.FuncId);
        }
    }
}