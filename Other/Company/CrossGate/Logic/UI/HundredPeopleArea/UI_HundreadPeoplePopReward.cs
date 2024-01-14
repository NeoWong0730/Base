using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_HundreadPeoplePopReward : UIBase, UI_HundreadPeoplePopReward.Layout.IListener {
        public class Layout : LayoutBase {
            public Button btnReward;
            public Transform rewardGo;
            public Text title;

            public void Parse(GameObject root) {
                this.Set(root);

                btnReward = transform.Find("Animator/Image_bg01/Btn_01").GetComponent<Button>();
                rewardGo = transform.Find("Animator/Image_bg01/Scroll View/Viewport/Content");
                title = transform.Find("Animator/Image_bg01/Text").GetComponent<Text>();
            }

            public void RegisterEvents(IListener listener) {
                this.btnReward.onClick.AddListener(listener.OnBtnRewardClicked);
            }

            public interface IListener {
                void OnBtnRewardClicked();
            }
        }

        public Layout layout = new Layout();
        public UI_RewardList rewardList;

        private uint instanceId;

        protected override void OnLoaded() {
            this.layout.Parse(this.gameObject);
            this.layout.RegisterEvents(this);
        }

        protected override void OnDestroy() {
            rewardList?.Clear();
        }

        protected override void ProcessEvents(bool toRegister) {
            Sys_HundredPeopleArea.Instance.eventEmitter.Handle(Sys_HundredPeopleArea.EEvents.OnGotDailyAward, OnGotDailyAward, toRegister);
        }

        protected override void OnOpen(object arg) {
            this.instanceId = System.Convert.ToUInt32(arg);
        }

        protected override void OnOpened() {
            var csv = CSVInstanceDaily.Instance.GetConfData(this.instanceId);
            if (csv != null) {
                if (this.rewardList == null) {
                    this.rewardList = new UI_RewardList(this.layout.rewardGo, EUIID.UI_HundreadPeoplePopReward);
                }

                this.layout.title.text = LanguageHelper.GetTextContent(1006191, csv.LayerStage.ToString(), (((csv.LayerStage - 1) * 10) + csv.Layerlevel).ToString());
                var ls = CSVDrop.Instance.GetDropItem(CSVHundredChapter.Instance.GetConfData(instanceId).dropid);
                this.rewardList.SetRewardList(ls);
                this.rewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);
            }
        }

        public void OnBtnRewardClicked() {
            Sys_HundredPeopleArea.Instance.SendTowerInstanceDailyRewardReq();
            this.CloseSelf();
        }

        #region 事件

        private void OnGotDailyAward() {
            this.CloseSelf();
        }

        #endregion
    }
}