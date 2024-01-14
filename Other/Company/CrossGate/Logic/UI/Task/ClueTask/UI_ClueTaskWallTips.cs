using Logic.Core;

namespace Logic {
    public class UI_ClueTaskWallTips : UIBase, UI_ClueTaskWallTips_Layout.IListener {
        private readonly UI_ClueTaskWallTips_Layout Layout = new UI_ClueTaskWallTips_Layout();
        private ClueTaskPhase phase;

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            this.phase = arg as ClueTaskPhase;
        }

        protected override void OnShow() {
            TextHelper.SetText(this.Layout.name, this.phase.csv.PhasedTasksName);
            TextHelper.SetText(this.Layout.taskDesc, this.phase.csv.PhasedTasksDes);
            if (this.phase.csv.DisplayClueType == 1) {
                this.Layout.clueDesc.gameObject.SetActive(false);
                this.Layout.icon.gameObject.SetActive(true);

                ImageHelper.SetIcon(this.Layout.icon, this.phase.csv.DisplayCluePara);
            }
            else if (this.phase.csv.DisplayClueType == 2) {
                this.Layout.clueDesc.gameObject.SetActive(true);
                this.Layout.icon.gameObject.SetActive(false);

                TextHelper.SetText(this.Layout.clueDesc, this.phase.csv.DisplayCluePara);
            }
        }

        public void OnBtnReturnClicked() {
            this.CloseSelf();
        }
    }
}