using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_RewardsShow : UIBase {
        public Button btnExit;
        public Transform rewardParent;

        public Transform transGridAward;

        public Transform transTip;

        public UI_RewardList rewardList;
        public IList<ItemIdCount> rewards = new List<ItemIdCount>();


        protected override void OnLoaded() {
            this.btnExit = this.transform.Find("Aniamtor/Image_BG").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnClicked);

            Text textTip = this.transform.Find("Aniamtor/Image_Line/Text_Name").GetComponent<Text>();
            textTip.text = LanguageHelper.GetTextContent(1006069);

            transGridAward = this.transform.Find("Aniamtor/Grid_Award");
            transGridAward.gameObject.SetActive(false);

            rewardParent = this.transform.Find("Aniamtor/Scroll_View/Viewport");

            rewardParent.Find("Item").gameObject.SetActive(false);

            transTip = this.transform.Find("Aniamtor/Text_Title");
        }

        protected override void OnOpen(object arg) {
            rewards = arg as IList<ItemIdCount>;
        }

        protected override void OnOpened() {
            if (rewards != null) {
                if (rewardList == null) {
                    rewardList = new UI_RewardList(rewardParent, EUIID.UI_RewardsShow);
                }
                rewardList.Show(true);
                rewardList.SetRewardList(rewards);
                rewardList.Build();
            }
            else {
                rewardList?.Show(false);
            }

            transTip.gameObject.SetActive(true);
        }

        public void OnBtnClicked() {
            this.CloseSelf();
        }
    }
}
