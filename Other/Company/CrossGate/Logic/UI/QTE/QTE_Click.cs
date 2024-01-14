using System;
using Framework;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public enum EQTESource {
        Cutscene = 0,
        Collect = 1,
    }
    public class QTEConfig {

        public bool autoFinishWhenDontOperate = false;
        public float autoFinishTime = 3f;
        public Action onFinish;
        public CutSceneArg arg;
        public EQTESource source;
        public CSVUnlock.Data csv;

        public QTEConfig(bool autoFinishWhenDontOperate, float autoFinishTime, Action onFinish, CutSceneArg arg, CSVUnlock.Data csv, EQTESource source = EQTESource.Cutscene) {
            this.Reset(autoFinishWhenDontOperate, autoFinishTime, onFinish, arg, csv, source);
        }
        public void Reset(bool autoFinishWhenDontOperate, float autoFinishTime, Action onFinish, CutSceneArg arg, CSVUnlock.Data csv, EQTESource source = EQTESource.Cutscene) {
            this.autoFinishWhenDontOperate = autoFinishWhenDontOperate;
            this.autoFinishTime = autoFinishTime;
            this.onFinish = onFinish;
            this.arg = arg;
            this.source = source;
            this.csv = csv;
        }

        public const float OperateTime = 3f;
        public const float TipTime = 5f;

        public static void OnTip() {
            Sys_Hint.Instance.PushContent_Normal("请操作");
        }
    }

    public class QTEBase : UIBase {
        public GameObject fxTip;
        public QTEConfig config;

        protected void OnTip() {
            this.fxTip?.SetActive(true);
        }
        protected override void OnOpen(object arg) {
            this.config = arg as QTEConfig;
        }
        protected override void OnOpened() {
            RectTransform animatorNode = this.transform.Find("Animator").GetComponent<RectTransform>();
            if (animatorNode != null && this.config.arg != null) {
                animatorNode.offsetMin = new Vector2(this.config.arg.offset.x, 0f);
                animatorNode.offsetMax = new Vector2(0f, this.config.arg.offset.y);
            }
        }
    }

    public class QTE_Click : QTEBase {
        public Button btn;
        public AnimationEndTrigger animationEndTrigger;
        public Animator animator;
        public Text clickTips;

        private bool gotResult = false;
        private float startRecordTime;

        protected override void OnLoaded() {
            this.btn = this.gameObject.GetComponentInChildren<Button>();
            this.btn.onClick.AddListener(this.OnBtnClicked);

            this.fxTip = this.gameObject.FindChildByName("FxRemindTip")?.gameObject;
            this.clickTips = this.transform.Find("Text_Tips01/Text_Tips01/Text_Tips02").GetComponent<Text>();

            this.animator = this.gameObject.transform.GetChild(0).GetComponentInChildren<Animator>();
            this.animationEndTrigger = this.animator.gameObject.GetComponent<AnimationEndTrigger>();
            this.animationEndTrigger.onAnimationEnd = this.OnAnimationEnd;
        }

        protected override void OnOpen(object arg) {
            base.OnOpen(arg);

            this.gotResult = false;
            this.startRecordTime = Time.time;
        }
        protected override void OnOpened() {
            base.OnOpened();
            if (this.config.csv != null) {
                TextHelper.SetText(this.clickTips, this.config.csv.QteText);
            }
        }
        protected override void OnUpdate() {
            if (!this.gotResult) {
                if (this.config.autoFinishWhenDontOperate) {
                    if (Time.time - this.startRecordTime > this.config.autoFinishTime) {
                        this.OnBtnClicked();
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
        private void OnBtnClicked() {
            if (this.gotResult) { return; }

            this.gotResult = true;
            this.OnSuccess();
        }

        private void OnSuccess() {
            this.PlayAnimation();
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
