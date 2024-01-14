using Lib.Core;
using Logic.Core;
using Net;

namespace Logic {
    public class UI_BlockClickTime : UIBase {
        private Timer timer;

        private float life;
        protected override void OnOpen(object arg) {
            life = System.Convert.ToSingle(arg);
            if(life <= 0.05f) {
                life = 0.5f;
            }
        }

        protected override void OnOpened() {
            if (life > 0) {
                // 传递时间参数的时候，timer自动关闭， 否则听从网络回调关闭
                this.timer = Timer.RegisterOrReuse(ref this.timer, life, () => {
                    this.CloseSelf();
                });
            }
        }

        protected override void OnDestroy() {
            this.timer?.Cancel();
        }

        //#region 事件处理
        //protected override void ProcessEvents(bool toRegister) {
        //    if (toRegister) {
        //        NetClient.Instance.AddStateListener(this.OnStatusChnage);
        //    }
        //    else {
        //        NetClient.Instance.RemoveStateListener(this.OnStatusChnage);
        //    }
        //}

        //private void OnStatusChnage(NetClient.ENetState from, NetClient.ENetState to) {
        //    if (to == NetClient.ENetState.ConnectFail || to == NetClient.ENetState.Connected) {
        //        this.CloseSelf();
        //    }
        //}
        //#endregion
    }
}
