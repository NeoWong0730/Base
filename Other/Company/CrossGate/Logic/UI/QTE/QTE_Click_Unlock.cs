using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class QTE_Click_Unlock : QTEBase {
        public Button backBtn;
        public Button unlockBtn;
        public AnimationEndTrigger animationEndTrigger;
        public Animator animator;

        private bool gotResult = false;
        private float startRecordTime;

        protected override void OnLoaded() {
            this.backBtn = this.transform.Find("Animator/Button_Off").GetComponent<Button>();
            this.backBtn.onClick.AddListener(this.OnBtnBackClicked);

            this.unlockBtn = this.transform.Find("Black").GetComponent<Button>();
            this.unlockBtn.onClick.AddListener(this.OnBtnUnlockClicked);

            this.fxTip = this.gameObject.FindChildByName("FxRemindTip")?.gameObject;

            this.animator = this.transform.Find("Animator").GetComponent<Animator>();
            this.animationEndTrigger = this.animator.gameObject.GetComponent<AnimationEndTrigger>();
            this.animationEndTrigger.onAnimationEnd = this.OnAnimationEnd;
        }

        protected override void OnOpen(object arg) {
            base.OnOpen(arg);

            this.gotResult = false;
            this.startRecordTime = Time.time;
        }
        protected override void OnUpdate() {
            if (!this.gotResult) {
                if (this.config.autoFinishWhenDontOperate) {
                    if (Time.time - this.startRecordTime > this.config.autoFinishTime) {
                        this.OnBtnUnlockClicked();
                        this.startRecordTime = Time.time;
                    }
                }
                else {
                    if (Time.time - this.startRecordTime > QTEConfig.TipTime) {
                        this.OnTip();
                        this.startRecordTime = Time.time;
                    }
                }
            }
        }
        private void OnBtnUnlockClicked() {
            if (this.gotResult) { return; }

            this.OnSuccess();
        }
        private void OnBtnBackClicked() {
            UIManager.CloseUI((EUIID)this.nID);
            Sys_CollectItem.Instance.eventEmitter.Trigger(Sys_CollectItem.EEvents.OnCollectFaild);
        }

        private void OnSuccess() {
            this.gotResult = true;
            PlayAnimation();
        }
        private void PlayAnimation() {
            this.animator.enabled = true;
            this.animator.Play("Release", -1, 0f);
        }
        private void OnAnimationEnd(string stateName) {
            this.config.onFinish?.Invoke();
            UIManager.CloseUI((EUIID)this.nID);

            Sys_QTE.Instance.eventEmitter.Trigger<EQTESource>(Sys_QTE.EEvents.OnClose, this.config.source);
        }
    }
}
