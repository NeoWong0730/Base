using Lib.Core;
using Logic.Core;
using Net;
using UnityEngine;

namespace Logic {
    public class UI_BlockClickNetwork : UIBase {
        public GameObject textGo;
        
        private Timer timerForClose;

        protected override void OnLoaded() {
            this.textGo = this.transform.Find("Animator/Text").gameObject;
            this.textGo.SetActive(false);
        }

        private float life;
        protected override void OnOpen(object arg) {
            this._ProcessEvents(false);
            this._ProcessEvents(true);
            
            life = System.Convert.ToSingle(arg);
            if (life <= 0.05f) {
                life = 0.5f;
            }
        }

        protected override void OnOpened() {
            // 传递时间参数的时候，timer自动关闭， 否则听从网络回调关闭
            this.timerForClose = Timer.RegisterOrReuse(ref this.timerForClose, life, () => {
                this.CloseSelf(); 
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3029387));
            });
        }

        protected override void OnClose() {
            this._ProcessEvents(false);
            
            this.timerForClose?.Cancel();
        }

        #region 事件处理

        private void _ProcessEvents(bool toRegister) {
            Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnServerCrash, this.OnServerCrash, toRegister);
            
            if (toRegister) {
                NetClient.Instance.AddStateListener(this.OnStatusChnage);
            }
            else {
                NetClient.Instance.RemoveStateListener(this.OnStatusChnage);
            }
        }

        private void OnStatusChnage(NetClient.ENetState from, NetClient.ENetState to) {
            if (to == NetClient.ENetState.ConnectFail || to == NetClient.ENetState.Connected) {
                this.timerForClose?.Cancel();
                
                this.CloseSelf();
            }
        }

        private void OnServerCrash() {
            this.timerForClose?.Cancel();

            this.CloseSelf();
        }

        #endregion
    }
}