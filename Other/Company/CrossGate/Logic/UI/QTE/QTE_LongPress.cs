using System.Collections.Generic;
using Logic.Core;
using System;
using Lib.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class QTE_LongPress : QTEBase {
        public Button closeBtn;
        public AnimationEndTrigger animationEndTrigger;
        public Animator animator;
        public Image slider;
        public UI_LongPressButton longPress;

        private float startRecordTime;
        public bool isPressing = false;

        private bool gotResult = false;

        protected override void OnLoaded() {
            closeBtn = transform.GetComponentInChildren<Button>();
            closeBtn?.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                UIManager.CloseUI((EUIID)(EUIID)nID);

                Sys_QTE.Instance.eventEmitter.Trigger<EQTESource>(Sys_QTE.EEvents.OnClose, config.source);
            }));
            animator = gameObject.transform.Find("Animator").GetComponentInChildren<Animator>();
            slider = transform.Find("Animator/Slider").GetComponentInChildren<Image>();
            Button longPressBtn = slider.gameObject.AddComponent<Button>();
			
			fxTip = gameObject.FindChildByName("FxRemindTip")?.gameObject;

            animationEndTrigger = animator.gameObject.GetComponent<AnimationEndTrigger>();
            animationEndTrigger.onAnimationEnd = OnAnimationEnd;

            UI_LongPressButton longPress = slider.gameObject.AddComponent<UI_LongPressButton>();
            longPress.interval = 0.1f;
            longPress.onStartPress.AddListener(() => {
                OnStartPress();
            });
            longPress.onPress.AddListener(() => {
                OnPress();
            });
            longPress.onRelease.AddListener(() => {
                OnRelease();
            });
        }

        protected override void OnOpen(object arg) {
            base.OnOpen(arg);

            startRecordTime = Time.time;
            gotResult = false;
            isPressing = false;
        }
        protected override void OnOpened() {
            slider.fillAmount = 0f;
        }
        protected override void OnUpdate() {
            if (!gotResult && !isPressing) {
                if (config.autoFinishWhenDontOperate) {
                    if (Time.time - startRecordTime > config.autoFinishTime) {
                        OnSuccess();
                        startRecordTime = Time.time;
                    }
                }
                else {
                    if (Time.time - startRecordTime > QTEConfig.TipTime) {
                        OnTip();
                        startRecordTime = Time.time;
                    }
                }
            }
        }

        private void OnPress() {
            if (gotResult) { return; }

            float diff = Time.time - startRecordTime;
            float rate = diff / QTEConfig.OperateTime;
            gotResult = rate >= 1f;

            if (slider != null) {
                slider.fillAmount = rate;
            }

            if (gotResult) {
                OnSuccess();
            }
        }
        private void OnStartPress() {
            if (gotResult) { return; }

            isPressing = true;

            if (slider != null) {
                slider.fillAmount = 0f;
            }

            startRecordTime = Time.time;
        }
        private void OnRelease() {
            startRecordTime = Time.time;
            isPressing = false;

            if(!gotResult) {
                if (slider != null) {
                    slider.fillAmount = 0f;
                }
            } 
        }

        private void OnSuccess() {
            gotResult = true;
            PlayAnimation();
        }
        private void PlayAnimation() {
            animator.enabled = true;
            animator.Play("Release", -1, 0f);
        }
        private void OnAnimationEnd(string stateName) {
            config.onFinish?.Invoke();
            isPressing = false;
            UIManager.CloseUI((EUIID)nID);

            Sys_QTE.Instance.eventEmitter.Trigger<EQTESource>(Sys_QTE.EEvents.OnClose, config.source);
        }
    }
}
