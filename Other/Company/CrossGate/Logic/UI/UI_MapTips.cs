using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_MapTips : UIBase {
        public Animator animator;

        public Text mapName;
        private string name;

        private Timer timer;

        protected override void OnLoaded() {
            this.mapName = this.transform.Find("Animator/Hint_Convey/Text").GetComponent<Text>();
            this.animator = this.transform.Find("Animator/Hint_Convey").GetComponent<Animator>();
        }

        protected override void OnOpen(object arg) {
            this.name = arg as string;
        }

        protected override void OnShow() {
            this.RefreshAll();
        }

        private void RefreshAll() {
            this.mapName.text = this.name;
            this.animator.Play("UI_Hint_Convey", -1, 0);

            this.timer?.Cancel();
            this.timer = Timer.Register(2.5f, () => { this.CloseSelf(); });
        }

        public override void OnSetData(object arg) {
            this.name = arg as string;
            if (this.bLoaded) {
                this.RefreshAll();
            }
        }

        protected override void OnDestroy() {
            this.timer?.Cancel();
        }
    }
}