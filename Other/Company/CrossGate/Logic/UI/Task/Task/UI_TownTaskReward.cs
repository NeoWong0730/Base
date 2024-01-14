using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_TownTaskReward : UIBase, UI_TownTaskReward.Layout.IListener {
        public class Tab : UIComponent {
            public Text level;

            public Transform rewardParent;
            public UI_RewardList rewardList;

            public Button btnGot;
            public GameObject hasGot;

            public uint townId;
            public uint id;

            protected override void Loaded() {
                this.level = this.transform.Find("Label").GetComponent<Text>();
                this.rewardParent = this.transform.Find("Scroll_View/Grid");
                this.hasGot = this.transform.Find("Image_Text").gameObject;

                this.btnGot = this.transform.Find("Btn_01_Small").GetComponent<Button>();
                this.btnGot.onClick.AddListener(this.OnBtnGot);
            }

            public void Refresh(uint townId, uint id, int index) {
                this.townId = townId;
                this.id = id;
                CSVTownContributeAward.Data csv = CSVTownContributeAward.Instance.GetConfData(id);
                if (csv != null) {
                    // this.Show();
                    if (this.rewardList == null) {
                        this.rewardList = new UI_RewardList(this.rewardParent, EUIID.UI_TownTaskReward);
                    }

                    this.rewardList.SetRewardList(CSVDrop.Instance.GetDropItem(csv.Reward));
                    this.rewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

                    TextHelper.SetText(this.level, 1660000002, csv.TownLV.ToString());

                    if (Sys_TownTask.Instance.towns.TryGetValue(townId, out var town)) {
                        bool got = town.gotRewards.Contains(id);
                        if (got) {
                            this.hasGot.SetActive(true);
                            this.btnGot.gameObject.SetActive(false);
                        }
                        else {
                            this.hasGot.SetActive(false);
                            bool canGot = town.contributeLevel >= csv.TownLV;
                            this.btnGot.gameObject.SetActive(true);
                            ButtonHelper.Enable(this.btnGot, canGot);
                        }
                    }
                    else {
                        ButtonHelper.Enable(this.btnGot, false);
                    }
                }
                else {
                    // this.Hide();
                }
            }

            private void OnBtnGot() {
                Sys_TownTask.Instance.TakeAwardReq(this.id);
            }
        }

        public class Layout : UILayoutBase {
            public Text title;
            public Button btnGotAll;

            public InfinityGrid infinity;

            public void Parse(GameObject root) {
                this.Init(root);

                this.title = this.transform.Find("Animator/View_TipsBgNew02/Text_Title").GetComponent<Text>();
                this.btnGotAll = this.transform.Find("Animator/Btn_01").GetComponent<Button>();
                this.infinity = this.transform.Find("Animator/Scroll_Rank").GetComponent<InfinityGrid>();
            }

            public void RegisterEvents(IListener listener) {
                this.btnGotAll.onClick.AddListener(listener.OnBtnAllClicked);
                this.infinity.onCreateCell += listener.OnCreateCell;
                this.infinity.onCellChange += listener.OnCellChange;
            }

            public interface IListener {
                void OnBtnAllClicked();
                void OnCreateCell(InfinityGridCell cell);
                void OnCellChange(InfinityGridCell cell, int index);
            }
        }

        public Layout layout = new Layout();
        public uint townId;
        public List<uint> rewardIds = new List<uint>();

        protected override void OnLoaded() {
            this.layout.Parse(this.gameObject);
            this.layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            this.townId = Convert.ToUInt32(arg);
            var all = CSVTownContributeAward.Instance.GetAll();
            for (int i = 0, length = all.Count; i < length; ++i) {
                if (all[i].TownId == townId) {
                    this.rewardIds.Add(all[i].id);
                }
            }
        }

        protected override void OnOpened() {
            this.RefreshAll(2);
        }

        #region 事件监听

        protected override void ProcessEventsForEnable(bool toRegister) {
            // 奖励领取事件
            Sys_TownTask.Instance.eventEmitter.Handle<uint, uint>(Sys_TownTask.EEvents.OnTakeAwardRes, this.OnTakeAwardRes, toRegister);
        }

        private void OnTakeAwardRes(uint townId, uint changedCount) {
            if (townId == this.townId) {
                this.RefreshAll(changedCount);
            }
        }

        #endregion

        public void RefreshAll(uint changedCount = 0) {
            this.layout.infinity.CellCount = this.rewardIds.Count;
            this.layout.infinity.ForceRefreshActiveCell();
            
            // if (changedCount > 1) {
            //     // 全部刷新，包括位置更新
            //     this.layout.infinity.ForceRefreshActiveCell();
            // }

            var csv = CSVFavorabilityPlaceReward.Instance.GetConfData(this.townId);
            if (csv != null) {
                string s = LanguageHelper.GetTextContent(csv.PlaceName);
                TextHelper.SetText(this.layout.title, 1660000001, s);
            }

            bool canGot = Sys_TownTask.Instance.towns.TryGetValue(this.townId, out var town);
            if (canGot) {
                canGot = town.CanAnyGot;
            }

            ButtonHelper.Enable(this.layout.btnGotAll, canGot);
        }

        public void OnBtnAllClicked() {
            var id = this.rewardIds[0];
            Sys_TownTask.Instance.TakeAwardReq(id, true);
        }

        public void OnCreateCell(InfinityGridCell cell) {
            Tab entry = new Tab();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        public void OnCellChange(InfinityGridCell cell, int index) {
            Tab entry = cell.mUserData as Tab;
            entry.Refresh(this.townId, this.rewardIds[index], index);
        }
    }
}