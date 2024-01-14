using System;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_BossUnlockCampOrBoss : UIBase {
        public GameObject bossNode;
        public GameObject campNode;

        public GameObject bossImg;
        public GameObject campImg;

        public Text campText;
        public Text bossText;
        public Text bossTitleText;

        public uint campId;
        public uint manualId;

        private Timer timer;

        protected override void OnLoaded() {
            this.campText = this.transform.Find("Animator/Text_Camp/Text").GetComponent<Text>();
            this.bossText = this.transform.Find("Animator/Text_Boss/Text").GetComponent<Text>();
            this.bossTitleText = this.transform.Find("Animator/Text_Boss/Text_Boss").GetComponent<Text>();

            this.campNode = this.transform.Find("Animator/Text_Camp").gameObject;
            this.bossNode = this.transform.Find("Animator/Text_Boss").gameObject;
        }

        protected override void OnOpen(object arg) {
            Tuple<uint, uint> tp = arg as Tuple<uint, uint>;
            if (tp != null) {
                this.campId = tp.Item1;
                this.manualId = tp.Item2;
            }
        }
        protected override void OnOpened() {
            timer?.Cancel();
            timer = Timer.Register(3f, () => {
                CloseSelf();
            });
        }
        protected override void OnShow() {
            this.bossNode.SetActive(this.manualId != 0);
            this.campNode.SetActive(this.campId != 0);

            if (this.manualId != 0) {
                CSVBOSSManual.Data csvBossManual = CSVBOSSManual.Instance.GetConfData(this.manualId);
                if (csvBossManual != null) {
                    TextHelper.SetText(this.bossText, csvBossManual.BOSS_name);
                    TextHelper.SetText(this.bossTitleText, csvBossManual.unlockedNotification);
                }
            }
            if (this.campId != 0) {
                CSVCampInformation.Data csvCamp = CSVCampInformation.Instance.GetConfData(this.campId);
                if (csvCamp != null) {
                    TextHelper.SetText(this.campText, csvCamp.camp_name);
                }
            }
        }
    }
}


