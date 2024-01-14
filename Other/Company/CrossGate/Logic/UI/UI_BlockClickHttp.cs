using System;
using Lib.Core;
using Logic.Core;
using Net;
using UnityEngine;

namespace Logic {
    public class UI_BlockClickHttp : UIBase {
        public GameObject bgGo;
        public GameObject contentGo;
        
        private Timer timer;
        private Timer destroyTimer;

        protected override void OnLoaded() {
            this.bgGo = this.transform.Find("BG").gameObject;
            this.contentGo = this.transform.Find("Animator").gameObject;

            this.Show(false);
        }
        
        private void Show(bool toShow) {
            this.bgGo.SetActive(toShow);
            this.contentGo.SetActive(toShow);
        }

        public float showDuration;
        protected override void OnOpen(object arg) {
            this._ProcessEvents(false);
            this._ProcessEvents(true);
            
            showDuration = Convert.ToSingle(arg);
            if (showDuration <= 0.05f) {
                showDuration = 0.5f;
            }
        }

        protected override void OnOpened() {
            if (showDuration > 0f) // 传递时间参数的时候，timer自动关闭， 否则听从网络回调关闭
            {
                this.timer?.Cancel();
                this.timer = Timer.RegisterOrReuse(ref this.timer, showDuration, () => {
                    this.Show(true);
                });
            }
            
            // 10s之后自动关闭
            this.destroyTimer?.Cancel();
            this.destroyTimer = Timer.RegisterOrReuse(ref this.destroyTimer, 7f, () => {
                this.CloseSelf();
            });
        }

        protected override void OnClose() {
            this._ProcessEvents(false);
            
            this.timer?.Cancel();
            this.destroyTimer?.Cancel();
        }

        #region 事件处理

        private void _ProcessEvents(bool toRegister) {
            Sys_Login.Instance.eventEmitter.Handle<bool>(Sys_Login.EEvents.OnLoginGot, this.OnLoginGot, toRegister);
        }

        private void OnLoginGot(bool success) {
            this.CloseSelf();
        }
        
        #endregion
    }
}