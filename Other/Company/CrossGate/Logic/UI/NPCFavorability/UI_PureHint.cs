using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // ÌבÊ¾
    public class UI_PureHint : UIBase {
        public string text;

        public Text content;
        public Transform pos;
        public CP_AnimationCurve curve;
        public Timer timer;

        protected override void OnLoaded() {
            this.content = this.transform.Find("Animator/Hint_Normal/Text_Notice").GetComponent<Text>();
            this.pos = this.transform.Find("Animator");

            this.curve = this.transform.GetComponent<CP_AnimationCurve>();
            if (this.curve.useCurve) {
                this.curve.onChange += this.UpdateCurve;
            }
        }
        private void UpdateCurve(float posX, float posY, float posZ) {
            this.pos.localPosition = new Vector3(posX, posY, posZ);
        }

        protected override void OnOpen(object arg) {
            this.text = arg as string;
        }
        protected override void OnClose() {
            this.timer?.Cancel();
        }

        protected override void OnOpened() {
            this.content.text = this.text;

            this.timer?.Cancel();
            this.timer = Timer.Register(this.curve.fadeTime, this.TrySuicide);
        }

        private void TrySuicide() {
            this.timer?.Cancel();
            this.curve.Set(false);

            this.CloseSelf();
        }
    }
}