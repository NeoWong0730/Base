using System;
using Framework;
using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FadeInOut : UIBase {
        public Image image;

        public enum EType {
            BlackFadeIn = 1,
            BlackFadeout = 2,
            WhiteFadeIn = 3,
            WhiteFadeout = 4,
        }

        private Timer timer;

        private CutSceneArg config;
        public EType type = EType.BlackFadeIn;
        private float totalTime;

        private float beginAlpha = 0f;
        private float endAlpha = 0f;

        protected override void OnLoaded() {
            this.image = this.transform.Find("Animator/Bg/Image_Up").GetComponent<Image>();
        }
        protected override void OnOpen(object arg) {
            config = arg as CutSceneArg;
            if (config != null) {
                this.totalTime = config.value;
                this.type = (EType)config.id;
            }
        }

        protected override void OnDestroy() {
            this.timer?.Cancel();
        }
        protected override void OnOpened() {
            if (this.type == EType.BlackFadeIn) {
                this.image.color = Color.black;
                this.beginAlpha = 1f;
                this.endAlpha = 0f;
            }
            else if (this.type == EType.BlackFadeout) {
                this.image.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0f);
                this.beginAlpha = 0f;
                this.endAlpha = 1f;
            }
            else if (this.type == EType.WhiteFadeIn) {
                this.image.color = Color.white;
                this.beginAlpha = 1f;
                this.endAlpha = 0f;
            }
            else if (this.type == EType.WhiteFadeout) {
                this.image.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);
                this.beginAlpha = 0f;
                this.endAlpha = 1f;
            }

            this.timer?.Cancel();
            this.timer = Timer.Register(this.totalTime, this.OnTimeOut, this.OnTiming);
        }

        private void OnTimeOut() {
            this.CloseSelf();
        }
        private void OnTiming(float dt) {
            float alpha = Mathf.Lerp(this.beginAlpha, this.endAlpha, dt / this.totalTime);
            Color c = this.image.color;
            this.image.color = new Color(c.r, c.g, c.b, alpha);
        }
    }
}