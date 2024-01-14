using System;
using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_CutScenePre : UIBase {
        public Image image;

        public static readonly float TOTAL = 0.65f;
        public static readonly float FADE = 0.3f;
        public Func<bool> judge;
        public float fadeLength;

        public enum EType {
            BlackFadeIn = 1,  // 1: 黑色淡入   
            BlackFadeout = 2, // 2: 黑色淡出
            WhiteFadeIn = 3,  // 3: 白色淡入
            WhiteFadeout = 4, // 4:白色淡出
            FullBlack = 5, // 全黑
            FullWhite = 6, // 全白
        }

        public EType type = EType.BlackFadeIn;
        private Timer timer;
        private bool isTimeout = false;
        private bool useForBegin = true;

        private float beginAlpha = 0f;
        private float endAlpha = 0f;

        protected override void OnLoaded() {
            this.image = this.transform.Find("Animator/Bg/Image_Up").GetComponent<Image>();
        }
        protected override void OnOpen(object arg) {
            this.HandleData(arg);
        }

        private void HandleData(object arg) {
            Tuple<int, int, Func<bool>, float> tp = arg as Tuple<int, int, Func<bool>, float>;
            if (tp != null) {
                this.useForBegin = tp.Item1 == 1;
                this.type = (EType)tp.Item2;
                this.judge = tp.Item3;
                this.fadeLength = tp.Item4;
            }
        }

        protected override void ProcessEvents(bool toRegister) {
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginExit, this.OnBeginEnter, toRegister);
        }

        private void OnBeginEnter(uint stackID, int nID) {
            if (nID == (int)EUIID.UI_Loading2) {
                this.OnTimeOut();
                this.CloseSelf();
            }
        }

        private void SetClose() {
            UIManager.CloseUI(EUIID.UI_CutScenePre, false, false);
        }

        protected override void OnClose() {
            this.timer?.Cancel();
        }

        public override void OnSetData(object arg) {
            //Debug.LogError("==========");
            this.HandleData(arg);
        }

        protected override void OnOpened() {
            if (this.type == EType.BlackFadeIn) {
                this.image.color = Color.black;
                this.beginAlpha = 1f;
                this.endAlpha = 0.4f;
            }
            else if (this.type == EType.BlackFadeout) {
                this.image.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0f);
                this.beginAlpha = 0.4f;
                this.endAlpha = 1f;
            }
            else if (this.type == EType.WhiteFadeIn) {
                this.image.color = Color.white;
                this.beginAlpha = 1f;
                this.endAlpha = 0.4f;
            }
            else if (this.type == EType.WhiteFadeout) {
                this.image.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);
                this.beginAlpha = 0.4f;
                this.endAlpha = 1f;
            }
            else if (this.type == EType.FullBlack) {
                this.image.color = Color.black;
                this.beginAlpha = 1f;
                this.endAlpha = 1f;
            }
            else if (this.type == EType.FullWhite) {
                this.image.color = Color.white;
                this.beginAlpha = 1f;
                this.endAlpha = 1f;
            }

            Color c = this.image.color;
            this.image.color = new Color(c.r, c.g, c.b, this.beginAlpha);

            this.isTimeout = false;
            this.timer?.Cancel();
            this.timer = Timer.Register(this.fadeLength, this.OnTimeOut, this.OnTiming);
        }

        protected override void OnUpdate() {
            if (this.isTimeout) {
                if (this.judge != null) {
                    if (this.judge.Invoke()) {
                        this.SetClose();
                        if (this.useForBegin) {
                            Sys_CutScene.Instance.OnReady();
                        }
                        this.isTimeout = false;
                    }
                }
                else {
                    this.SetClose();
                    if (this.useForBegin) {
                        Sys_CutScene.Instance.OnReady();
                    }
                    this.isTimeout = false;
                }
            }
        }
        private void OnTimeOut() {
            this.isTimeout = true;

            if (!this.useForBegin) {
                Sys_CutScene.Instance.isPlaying = false;
                Sys_CutScene.Instance.isRealPlaying = false;
            }
        }
        private void OnTiming(float dt) {
            float alpha = this.beginAlpha;
            if (dt < FADE) {
                alpha = this.beginAlpha;
            }
            else {
                alpha = Mathf.Lerp(this.beginAlpha, this.endAlpha, (dt - FADE) / (TOTAL - FADE));
            }

            Color c = this.image.color;
            this.image.color = new Color(c.r, c.g, c.b, alpha);
        }
    }
}