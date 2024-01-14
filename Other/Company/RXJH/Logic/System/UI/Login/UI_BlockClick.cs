using System;
using Framework.UI;
using Lib.Core;
using Logic;
using Logic.Core;
using UnityEngine;

// 全屏防止点击
// 如果想添加时间监听，可以继承，然后实现ProcessEvents，类似UIBlockClickForEvent
namespace Logic.UI {
    public partial class UI_BlockClick : UIBase {
        public class TimeCtrl {
            public float showTime;
            public float forceHideTime;
            public Action<bool> listenAction;

            private Timer showTimer;
            private Timer forceHideTimer;

            public TimeCtrl(float showTime, float forceHideTime, Action<bool> listenAction = null) {
                this.showTime = showTime;
                this.forceHideTime = forceHideTime;
                this.listenAction = listenAction;
            }

            public void Correct() {
                showTime = showTime <= 0.01f ? 0.01f : showTime;
                forceHideTime = forceHideTime <= 0.01f ? 0.01f : forceHideTime;
            }

            public void Setup(Action onShow, Action onHide) {
                showTimer?.Cancel();
                showTimer = Timer.RegisterOrReuse(ref showTimer, showTime, onShow);

                forceHideTimer?.Cancel();
                forceHideTimer = Timer.RegisterOrReuse(ref forceHideTimer, forceHideTime, onHide);
            }

            public void Release() {
                showTimer?.Cancel();
                forceHideTimer?.Cancel();

                showTimer = null;
                forceHideTimer = null;
            }
        }
        
        public UI_BlockClick_Layout layout;
        public TimeCtrl timeCtrl;

        protected override void OnLoaded() {
            layout = UILayoutBase.GetLayout<UI_BlockClick_Layout>(this.transform);
            layout.Init(this.transform);
        }

        protected override void OnOpen(object arg) {
            timeCtrl = arg as TimeCtrl;
            timeCtrl?.Correct();
        }

        private void _OnShowCollidder() {
            layout.imgCollider.gameObject.SetActive(true);
        }

        private void _OnHideCollidder() {
            UIManager.CloseUI((EUIID) nID);
        }

        protected override void OnOpened() {
            layout.imgCollider.gameObject.SetActive(false);
            timeCtrl?.Setup(_OnShowCollidder, _OnHideCollidder);
        }

        protected override void OnDestroy() {
            timeCtrl?.Release();
        }
    }
    
    // 逻辑事件
    public partial class UI_BlockClick {
        protected override void ProcessEvents(bool toRegister) {
            timeCtrl?.listenAction?.Invoke(toRegister);
        }
    }
}