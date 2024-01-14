using Lib.Core;
using Logic.Core;
using UnityEngine;

namespace Logic
{
    public class QTE_Slide : QTEBase
    {
        public Animator animator;
        public EventTrigger unlock;

        private bool gotResult = false;
        private float startRecordTime;

        protected override void OnLoaded() {
            unlock = gameObject.GetComponentInChildren<EventTrigger>();
            unlock.onDragEnd += OnDragEnd;

            fxTip = gameObject.FindChildByName("FxRemindTip")?.gameObject;

            animator = gameObject.transform.GetChild(0).GetComponentInChildren<Animator>();
            AnimationEndTrigger animationEndTrigger = animator.gameObject.GetComponent<AnimationEndTrigger>();
            animationEndTrigger.onAnimationEnd = OnAnimationEnd;
        }

        protected override void OnOpen(object arg) {
            base.OnOpen(arg);

            gotResult = false;
            startRecordTime = Time.time;
        }
        protected override void OnUpdate() {
            if (!gotResult && !unlock.IsDraging) {
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
        private void OnDragEnd(GameObject go) {
            OnSuccess();
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
            UIManager.CloseUI((EUIID)nID);

            Sys_QTE.Instance.eventEmitter.Trigger<EQTESource>(Sys_QTE.EEvents.OnClose, config.source);
        }
    }
}
