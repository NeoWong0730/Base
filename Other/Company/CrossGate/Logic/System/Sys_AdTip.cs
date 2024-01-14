using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Packet;
using Table;

namespace Logic {
    public class Sys_AdTip : SystemModuleBase<Sys_AdTip> {
        public enum EEvents {
        }

        #region 系统级别函数

        public override void Init() {
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public override void OnLogin() {
        }

        public override void OnLogout() {
        }

        #endregion
    }
}