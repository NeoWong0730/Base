using System.Collections.Generic;
using Lib.Core;
using Logic;
using Logic.Core;
using Net;

namespace Logic {
    public partial class Sys_RefreshTimerRegistry {
        public readonly IList<NextRefreshTimer> nextTimers = new List<NextRefreshTimer>();

        public void Add(NextRefreshTimer t) {
            if (t != null && !nextTimers.Contains(t)) {
                nextTimers.Add(t);
            }
        }

        public void Remove(NextRefreshTimer t) {
            nextTimers.Remove(t);
        }

        public void Clear() {
            for (int i = 0, length = nextTimers.Count; i < length; ++i) {
                nextTimers[i].Cancel();
            }

            nextTimers.Clear();
        }
    }

    // 倒计时刷新管理
    public partial class Sys_RefreshTimerRegistry : SystemModuleBase<Sys_RefreshTimerRegistry> {
        public override void Init() {
            Sys_Time.Instance.eventEmitter.Handle<ulong, ulong>(Sys_Time.EEvents.OnTimeChanged, this.OnTimeNtf, true);
        }

        private void OnTimeNtf(ulong oldTime, ulong newTime) {
            for (int i = 0, length = nextTimers.Count; i < length; ++i) {
                if (!nextTimers[i].IsDone) {
                    nextTimers[i].OnServerTimeChanged();
                }
            }
        }
    }
}