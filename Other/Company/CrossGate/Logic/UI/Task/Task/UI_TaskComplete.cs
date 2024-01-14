using Lib.Core;
using Logic.Core;

namespace Logic {
    public class UI_TaskComplete : UIBase {
        private Timer timer;

        protected override void OnShow() {
            this.timer = Timer.Register(1.3f, () => {
                this.CloseSelf();
            });
        }

        protected override void OnHide() {
            this.timer?.Cancel();            
        }
    }
}
