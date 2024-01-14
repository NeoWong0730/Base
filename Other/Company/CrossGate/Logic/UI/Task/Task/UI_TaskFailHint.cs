using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_TaskFailHint : UIBase {
        public CP_AnimationCurve curve;
        public Text taskName;
        public Image taskBg;
        public Text catlogy;
        public Animator animator;

        private uint taskId;
        protected Timer timer;

        protected override void OnLoaded() {
            this.curve = this.transform.Find("Animator/Hint_TaskType").GetComponent<CP_AnimationCurve>();
            this.taskName = this.transform.Find("Animator/Hint_TaskType/View_Name/Text_TaskName").GetComponent<Text>();
            this.taskBg = this.transform.Find("Animator/Hint_TaskType/View_Name/Image_Notice_BG").GetComponent<Image>();
            this.catlogy = this.transform.Find("Animator/Hint_TaskType/View_Dis/Text_Dis").GetComponent<Text>();
            this.animator = this.transform.Find("Animator/Hint_TaskType").GetComponent<Animator>();
        }

        protected override void OnOpen(object arg) {
            this.taskId = System.Convert.ToUInt32(arg);
        }
        protected override void OnOpened() {
            this.timer?.Cancel();
            this.timer = Timer.Register(this.curve.fadeTime, () => {
                this.CloseSelf();
            });
        }
        protected override void OnDestroy() {
            this.timer?.Cancel();
        }
        protected override void ProcessEvents(bool toRegister) {
            Sys_Task.Instance.eventEmitter.Handle<uint>(Sys_Task.EEvents.OnReceivedTarget, this.OnReceived, toRegister);
        }
        private void OnReceived(uint taskId) {
            if (!this.bLoaded) {
                return;
            }
            this.taskId = taskId;

            this.timer?.Cancel();
            this.timer = Timer.Register(this.curve.fadeTime, () => {
                this.CloseSelf();
            });

            this.OnShow();
        }
        protected override void OnShow() {
            // 重新播放open动画
            this.animator.Play("Open", -1, 0);

            CSVTask.Data csvTask = CSVTask.Instance.GetConfData(this.taskId);
            if (csvTask != null) {
                TextHelper.SetTaskText(this.taskName, csvTask.taskName);

                TextHelper.SetText(this.catlogy, 1601000007u);
                CSVTaskCategory.Data csvTaskCategory = CSVTaskCategory.Instance.GetConfData((uint)csvTask.taskCategory);
                if (csvTaskCategory != null) {

                    if (csvTaskCategory.taskTypeTipsColour != null && csvTaskCategory.taskTypeTipsColour.Count >= 4) {
                        var ls = csvTaskCategory.taskTypeTipsColour;
                        this.taskBg.color = new Color(ls[0] / 255f, ls[1] / 255f, ls[2] / 255f, ls[3] / 255f);
                    }
                }
            }
        }
    }
}