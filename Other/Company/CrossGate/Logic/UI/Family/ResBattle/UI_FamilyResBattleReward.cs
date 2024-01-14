using Logic;
using Logic.Core;
using Table;
using UnityEngine;

// 家族资源战 奖励预览
public class UI_FamilyResBattleReward : UIBase, UI_FamilyResBattleReward.Layout.IListener {
    public class Layout : LayoutBase {
        public Transform yesParent;
        public Transform noParent;

        public void Parse(GameObject root) {
            this.Set(root);

            this.yesParent = this.transform.Find("Animator/Win/Scroll View/Viewport/Content");
            this.noParent = this.transform.Find("Animator/Lose/Scroll View/Viewport/Content");
        }

        public void RegisterEvents(IListener listener) {
        }

        public interface IListener {
        }
    }

    public Layout layout = new Layout();
    public UI_RewardList yesRewardList;
    public UI_RewardList noRewardList;

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }
    protected override void OnDestroy() {
        this.yesRewardList.Clear();
        this.noRewardList.Clear();
    }

    protected override void OnOpen(object arg) {
    }

    protected override void OnOpened() {
        if (this.yesRewardList == null) {
            this.yesRewardList = new UI_RewardList(this.layout.yesParent, EUIID.UI_FamilyResBattleReward);
        }
        this.yesRewardList.SetRewardList(CSVDrop.Instance.GetDropItem(Sys_FamilyResBattle.Instance.battlewinFamilyDropId));
        this.yesRewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

        if (this.noRewardList == null) {
            this.noRewardList = new UI_RewardList(this.layout.noParent, EUIID.UI_FamilyResBattleReward);
        }
        this.noRewardList.SetRewardList(CSVDrop.Instance.GetDropItem(Sys_FamilyResBattle.Instance.battlefailFamilyDropId));
        this.noRewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);
    }
}