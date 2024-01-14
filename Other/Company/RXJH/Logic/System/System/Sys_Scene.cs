using Client;
using Common;
using Lib.Core;
using Logic.Core;
using Net;

namespace Logic {
    // scene
    public partial class Sys_Scene : SystemModuleBase<Sys_Scene> {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        private readonly SystemModuleMsgProcesser _msgProcesser = new SystemModuleMsgProcesser(enClientFirst.Scene);

        public enum EEvents {
        }

        public override void Init() {
            _msgProcesser.Listen(0, (ushort)enSecondScene.NtfIntoFirst, _OnNtfIntoFirst, SecondScene_Ntf_IntoFirst.Parser, true);
            _msgProcesser.Listen(0, (ushort)enSecondScene.NtfIntoFinish, _OnNtfIntoFinish, SecondScene_Ntf_IntoFinish.Parser, true);
            _msgProcesser.Listen(0, (ushort)enSecondScene.NtfModuleData, _OnNtfModuleData, SecondScene_Ntf_IntoFinish.Parser, true);
        }

        private void _OnNtfIntoFirst(NetMsg msg) {
            DebugUtil.Log(ELogType.eNetSendMSG, "_OnNtfIntoFirst");
            var res = msg.data as SecondScene_Ntf_IntoFirst;
        }

        private void _OnNtfIntoFinish(NetMsg msg) {
            DebugUtil.Log(ELogType.eNetSendMSG, "_OnNtfIntoFinish");
            var res = msg.data as SecondScene_Ntf_IntoFinish;
        }

        // 数据派发
        private void _OnNtfModuleData(NetMsg msg) {
            DebugUtil.Log(ELogType.eNetSendMSG, "_OnNtfModuleData");
            var res = msg.data as SecondScene_Ntf_ModuleData;

            Sys_Task.Instance.SetData(res.Task);
        }
    }
}