using Client;
using Framework;
using Lib.Core;
using Logic.Core;
using Net;

namespace Logic {
    /// 服务器时间
    public class Sys_Time : SystemModuleBase<Sys_Time> {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        private readonly SystemModuleMsgProcesser _msgProcesser = new SystemModuleMsgProcesser(enClientFirst.Login);

        public enum EEvents {
            OnTimeChanged, // 时间更新
        }

        public override void Init() {
            _msgProcesser.Listen(0, (ushort)enSecondLogin.NtfSyncGameTime, OnServerTimeChanged, SecondLogin_Ntf_SyncGameTime.Parser, true);
        }

        private void OnServerTimeChanged(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eNetSendMSG,  "OnServerTimeChanged");
            SecondLogin_Ntf_SyncGameTime res = msg.data as SecondLogin_Ntf_SyncGameTime;
            ulong oldTimeMS = GetServerTimeMS();
            ulong newTimeMS = res.ServerGameTime;
            CorrectServerTime(newTimeMS);

            eventEmitter.Trigger<ulong, ulong>(EEvents.OnTimeChanged, oldTimeMS, newTimeMS);
        }

        public void CorrectServerTime(ulong newTimeMS) {
            TimeManager.CorrectServerTime((uint)newTimeMS / 1000);
        }

        public ulong GetServerTimeMS() {
            return TimeManager.GetServerTime(false) * 1000;
        }

        public long GetServerTime() {
            return TimeManager.GetServerTime(false);
        }
    }
}