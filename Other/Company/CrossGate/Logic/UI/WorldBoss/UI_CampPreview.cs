using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // 阵营展览
    public class UI_CampPreview : UIBase {
        public class Tab : UIComponent {
            public Transform campRedDot;

            public Button btn;
            public Text campName;

            public Image bg;
            public Image iconBig;
            public Image iconSmall;

            public GameObject lockGo;

            public GameObject unGotGo;
            public Button btnUnGotReward;

            public GameObject gotGo;
            public Button btnGotReward;

            public uint id;
            public bool isUnlocked = false;

            protected override void Loaded() {
                this.campRedDot = this.transform.Find("Image_Red");
                this.btn = this.transform.GetComponent<Button>();
                this.btn.onClick.AddListener(this.OnBtnClicked);

                this.campName = this.transform.Find("Text_name").GetComponent<Text>();
                this.bg = this.transform.Find("Image_Bg").GetComponent<Image>();
                this.iconSmall = this.transform.Find("Image_small").GetComponent<Image>();
                this.iconBig = this.transform.Find("Image_big").GetComponent<Image>();

                this.lockGo = this.transform.Find("Image_lock").gameObject;
                this.unGotGo = this.transform.Find("Image_ungot").gameObject;
                this.gotGo = this.transform.Find("Image_got").gameObject;

                this.btnUnGotReward = this.transform.Find("Image_ungot/Button").GetComponent<Button>();
                this.btnUnGotReward.onClick.AddListener(this.OnBtnUnGotClicked);

                this.btnGotReward = this.transform.Find("Image_got/Button").GetComponent<Button>();
                this.btnGotReward.onClick.AddListener(this.OnBtnGotClicked);
            }

            public void Refresh(uint id) {
                this.id = id;

                CSVCampInformation.Data csv;
                this.isUnlocked = Sys_WorldBoss.Instance.unlockedCamps.TryGetValue(id, out BossCamp camp);
                if (this.isUnlocked) {
                    this.lockGo.SetActive(false);
                    bool hasGotReward = camp.GotReward;
                    this.gotGo.SetActive(hasGotReward);
                    this.unGotGo.SetActive(!hasGotReward);

                    csv = camp.csv;
                    TextHelper.SetText(this.campName, csv.camp_name);
                    campRedDot.gameObject.SetActive(camp.HasRewardUngot);
                    //ImageHelper.SetImageGray(this.transform, false, true);
                }
                else {
                    this.lockGo.SetActive(true);
                    this.unGotGo.SetActive(false);
                    this.gotGo.SetActive(false);

                    csv = CSVCampInformation.Instance.GetConfData(id);
                    TextHelper.SetText(this.campName, 4157000004);
                    campRedDot.gameObject.SetActive(false);
                    //ImageHelper.SetImageGray(this.transform, true, true);
                }

                this.bg.color = csv.ImageColor;
                this.campName.color = csv.FontColor;
                ImageHelper.SetIcon(this.iconSmall, csv.camp_icon);
                ImageHelper.SetIcon(this.iconBig, csv.camp_icon);
            }

            private void OnBtnClicked() {
                if (this.isUnlocked) {
                    UIManager.OpenUI(EUIID.UI_WorldBossCampInfo, false, new Tuple<uint, uint>(this.id, 0));
                }
            }
            private void OnBtnUnGotClicked() {
                uint campId = this.id;
                if (Sys_WorldBoss.Instance.TryGetAnyUnlockedBossIdByCampId(campId, out uint manulId)) {
                    Sys_WorldBoss.Instance.ReqReward(manulId, 4, 0);
                }
            }
            private void OnBtnGotClicked() {
                // 奖励预览
                var csvcamp = CSVCampInformation.Instance.GetConfData(this.id);
                if (csvcamp != null)
                {
                    var tp = new Tuple<List<ItemIdCount>, bool,Button>(CSVDrop.Instance.GetDropItem(csvcamp.campUnlocked_drop), true, this.btnGotReward);
                    UIManager.OpenUI(EUIID.UI_WorldBossRewardList, false, tp);
                }
            }
        }

        public Button btnExit;
        public GameObject proto;
        public Transform protoParent;
        public COWVd<Tab> tabs = new COWVd<Tab>();

        protected override void OnLoaded() {
            this.btnExit = this.transform.Find("Animator/View_Bg05/Btn_Close").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);

            this.proto = this.transform.Find("Animator/Scroll View/Rectlist/Proto").gameObject;
            this.protoParent = this.transform.Find("Animator/Scroll View/Rectlist");
        }
        protected override void OnShow() {
            this.RefreshAll();
        }
        public void RefreshAll() {
            var allCamps = Sys_WorldBoss.Instance.allCamps;
            allCamps.Sort((lId, rId) => {
                int cL = Sys_WorldBoss.Instance.unlockedCamps.ContainsKey(lId) ? 1 : 0;
                int cR = Sys_WorldBoss.Instance.unlockedCamps.ContainsKey(rId) ? 1 : 0;
                if (cL == 1 && cR == 1) {
                    return (int)((long)(lId) - (long)rId);
                }
                else {
                    return cR - cL;
                }
            });
            
            this.tabs.TryBuildOrRefresh(this.proto, this.protoParent, allCamps.Count, (tab, index) => {
                tab.Refresh(allCamps[index]);
            });
        }

        private void OnBtnExitClicked() {
            this.CloseSelf();
        }

        #region 事件通知
        protected override void ProcessEvents(bool toRegister) {
            Sys_WorldBoss.Instance.eventEmitter.Handle(Sys_WorldBoss.EEvents.OnRewardGot, this.OnRewardGot, toRegister);
        }

        private void OnRewardGot() {
            if (this.gameObject.activeInHierarchy) {
                this.RefreshAll();
            }
        }
        #endregion
    }
}
