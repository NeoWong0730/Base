using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_ChatTaskInfo : UIBase {
        public Text title;
        public Text taskDesc;
        public Text proto;

        public uint taskId;
        public TaskEntry taskEntry;
        public COWComponent<Text> goalVds = new COWComponent<Text>();

        protected override void OnLoaded() {
            this.title = this.transform.Find("Animator/View_Content/Image_Bg/Title").GetComponent<Text>();
            this.taskDesc = this.transform.Find("Animator/View_Content/Grid/Text").GetComponent<Text>();
            this.proto = this.transform.Find("Animator/View_Content/GridGoals/Text").GetComponent<Text>();
        }

        protected override void OnDestroy() {
            goalVds.Clear();
        }

        protected override void OnOpen(object arg) {
            this.taskId = System.Convert.ToUInt32(arg);
        }

        protected override void OnOpened() {
            this.taskEntry = Sys_Task.Instance.GetTask(taskId);
            if (this.taskEntry != null) {
                TextHelper.SetTaskText(this.title, this.taskEntry.csvTask.taskName);
                TextHelper.SetTaskText(this.taskDesc, this.taskEntry.csvTask.taskDescribe);
                this.goalVds.TryBuildOrRefresh(this.proto.gameObject, this.proto.transform.parent, this.taskEntry.taskGoals.Count, this.OnRefreshGoal);
            }
            else {
                // error
            }
        }

        private void OnRefreshGoal(Text text, int index) {
            text.text = this.taskEntry.taskGoals[index].taskContent;
        }
    }
}