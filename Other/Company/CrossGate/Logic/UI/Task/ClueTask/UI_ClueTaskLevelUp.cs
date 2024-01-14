using System;
using Logic.Core;
using Table;

namespace Logic {
    public class UI_ClueTaskLevelUp : UIBase, UI_ClueTaskLevelUp_Layout.IListener {
        private readonly UI_ClueTaskLevelUp_Layout Layout = new UI_ClueTaskLevelUp_Layout();
        public Tuple<int, uint, uint> levelUp;
        public UI_RewardList rewardList;

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            this.levelUp = arg as Tuple<int, uint, uint>;
        }
        protected override void OnShow() {
            if (this.rewardList == null) {
                this.rewardList = new UI_RewardList(this.Layout.node, EUIID.UI_ClueTaskLevelUp);
            }

            if (this.levelUp.Item1 == 1) {
                CSVClueAdventureLevel.Data csvAdventure = CSVClueAdventureLevel.Instance.GetConfData(this.levelUp.Item3);
                if (csvAdventure != null) {
                    this.rewardList.SetRewardList(CSVDrop.Instance.GetDropItem(csvAdventure.Reward));
                    this.rewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

                    TextHelper.SetText(this.Layout.title, LanguageHelper.GetTextContent(2007053));
                    TextHelper.SetText(this.Layout.newLevelDesc, csvAdventure.Name);
                    ImageHelper.SetIcon(this.Layout.newIcon, csvAdventure.Badge);
                }
            }
            else if (this.levelUp.Item1 == 2) {
                CSVClueDetectiveLevel.Data csvDetective = CSVClueDetectiveLevel.Instance.GetConfData(this.levelUp.Item3);
                if (csvDetective != null) {
                    this.rewardList.SetRewardList(CSVDrop.Instance.GetDropItem(csvDetective.Reward));
                    this.rewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

                    TextHelper.SetText(this.Layout.title, LanguageHelper.GetTextContent(2007054));
                    TextHelper.SetText(this.Layout.newLevelDesc, csvDetective.Name);
                    ImageHelper.SetIcon(this.Layout.newIcon, csvDetective.Badge);
                }
            }
        }

        public void OnBtnReturnClicked() {
            this.CloseSelf();
        }
    }
}