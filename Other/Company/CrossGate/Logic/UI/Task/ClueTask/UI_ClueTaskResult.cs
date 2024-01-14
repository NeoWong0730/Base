using Logic.Core;
using Table;

namespace Logic {
    public class UI_ClueTaskResult : UIBase, UI_ClueTaskResult_Layout.IListener {
        private readonly UI_ClueTaskResult_Layout Layout = new UI_ClueTaskResult_Layout();
        public ClueTask clueTask;
        public UI_RewardList rewardList;

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            this.clueTask = arg as ClueTask;
        }

        protected override void OnShow() {
            if (this.rewardList == null) {
                this.rewardList = new UI_RewardList(this.Layout.node, EUIID.UI_ClueTaskResult);
            }

            this.rewardList.SetRewardList(CSVDrop.Instance.GetDropItem(this.clueTask.csv.Reward));
            this.rewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

            if (this.clueTask != null) {
                TextHelper.SetText(this.Layout.taskName, this.clueTask.csv.TaskName);
                TextHelper.SetText(this.Layout.taskDesc, this.clueTask.csv.TaskCompleteDes);
            }
        }

        public void OnBtnReturnClicked() {
            this.CloseSelf();
        }
    }
}