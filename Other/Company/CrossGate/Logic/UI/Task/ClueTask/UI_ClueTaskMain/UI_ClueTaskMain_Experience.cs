using System.Collections.Generic;
using Table;

namespace Logic {
    public class UI_ClueTaskMain_Experience : UI_ClueTaskMain.UI_CluetaskSubComponent {
        public UI_ClueTaskMain_Experience_Layout layout = new UI_ClueTaskMain_Experience_Layout();

        public UIElementContainer<LevelVd, uint> vds = new UIElementContainer<LevelVd, uint>();

        public LevelVd leftLevelVd = new LevelVd();
        public LevelVd rightLevelVd = new LevelVd();
        public UI_RewardList rewardList;

        public class LevelVd : UISelectableElement {
            public UI_ClueTaskMain_Experience_Proto_Layout layout = new UI_ClueTaskMain_Experience_Proto_Layout();

            public EClueTaskType levelType;
            public uint dropId;
            public UI_ClueTaskMain_Experience component;
            private bool isOverflow;

            protected override void Loaded() {
                this.layout.Parse(this.gameObject);
                if (this.layout.button != null) {
                    this.layout.button.onClick.AddListener(this.OnBtnClicked);
                }
            }
            public void Refresh(EClueTaskType levelType, uint level, UI_ClueTaskMain_Experience component) {
                this.component = component;
                this.levelType = levelType;

                if (levelType == EClueTaskType.Adventure) {
                    CSVClueAdventureLevel.Data currentCSV = CSVClueAdventureLevel.Instance.GetConfData(level);
                    CSVClueAdventureLevel.Data nextCSV = CSVClueAdventureLevel.Instance.GetConfData(level + 1);
                    this.isOverflow = nextCSV == null;
                    this.dropId = nextCSV.Reward;

                    TextHelper.SetText(this.layout.detectiveOrAdventureTitle, "冒险");
                    TextHelper.SetText(this.layout.title, currentCSV.Name);
                    ImageHelper.SetIcon(this.layout.icon, currentCSV.Badge);
                    if (!this.isOverflow) {
                        if (this.layout.slider != null) {
                            this.layout.slider.value = Sys_ClueTask.Instance.adventureExp * 1.0f / nextCSV.UpgradeExp;
                            this.layout.pageIndexer.Refresh(Sys_ClueTask.Instance.adventureExp, nextCSV.UpgradeExp);
                        }
                    }
                }
                else if (levelType == EClueTaskType.Detective) {
                    CSVClueDetectiveLevel.Data currentCSV = CSVClueDetectiveLevel.Instance.GetConfData(level);
                    CSVClueDetectiveLevel.Data nextCSV = CSVClueDetectiveLevel.Instance.GetConfData(level + 1);
                    this.isOverflow = nextCSV == null;
                    this.dropId = nextCSV.Reward;

                    TextHelper.SetText(this.layout.detectiveOrAdventureTitle, "侦探");
                    TextHelper.SetText(this.layout.title, currentCSV.Name);
                    ImageHelper.SetIcon(this.layout.icon, currentCSV.Badge);
                    if (!this.isOverflow) {
                        if (this.layout.slider != null) {
                            this.layout.slider.value = Sys_ClueTask.Instance.detectiveExp * 1.0f / nextCSV.UpgradeExp;
                            this.layout.pageIndexer.Refresh(Sys_ClueTask.Instance.detectiveExp, nextCSV.UpgradeExp);
                        }
                    }
                }

                if (this.layout.slider != null) {
                    this.layout.slider.gameObject.SetActive(!this.isOverflow);
                }
            }
            private void OnBtnClicked() {
                if (!this.isOverflow) {
                    this.component.ShowDetail(true, this.levelType, this.dropId);
                }
                else {
                    // todo
                    Sys_Hint.Instance.PushContent_Normal("已到最大等级");
                }
            }
        }

        protected override void Loaded() {
            this.layout.Parse(this.gameObject);
            this.layout.returnBtn.onClick.AddListener(this.OnBtnrReturnClicked);

            this.leftLevelVd.Init(this.layout.leftGo.transform);
            this.rightLevelVd.Init(this.layout.rightGo.transform);
        }
        public override void Reset() {
        }
        public override void OnDestroy() {
            this.vds.Clear();
            base.OnDestroy();
        }
        protected override void Update() {
            this.vds.Update();
        }
        protected override void ProcessEventsForAwake(bool toRegister) {
            Sys_ClueTask.Instance.eventEmitter.Handle<uint, uint>(Sys_ClueTask.EEvents.OnAdventureExpChanged, this.OnAdventureExpChanged, toRegister);
            Sys_ClueTask.Instance.eventEmitter.Handle<uint, uint>(Sys_ClueTask.EEvents.OnDetectiveExpChanged, this.OnDetectiveExpChanged, toRegister);
        }
        private void OnAdventureExpChanged(uint before, uint after) {
            this.Refresh(EClueTaskType.Experience);
        }
        private void OnDetectiveExpChanged(uint before, uint after) {
            this.Refresh(EClueTaskType.Experience);
        }

        // 入口
        public override void Refresh(EClueTaskType tabType) {
            List<uint> levelList = new List<uint>() { 1, 2 };
            this.vds.BuildOrRefresh(this.layout.protoGo, this.layout.parent, levelList, (vd, data, index) => {
                vd.SetUniqueId((int)data);
                uint level = 1;
                if ((EClueTaskType)(data) == EClueTaskType.Adventure) {
                    level = Sys_ClueTask.Instance.adventureLevel;
                }
                else if ((EClueTaskType)(data) == EClueTaskType.Detective) {
                    level = Sys_ClueTask.Instance.detectiveLevel;
                }

                vd.Refresh((EClueTaskType)(data), level, this);
            });
        }

        private void OnBtnrReturnClicked() {
            this.ShowDetail(false, EClueTaskType.None, 0);
        }
        public void ShowDetail(bool toShow, EClueTaskType levelType, uint dropId) {
            this.layout.expend.SetActive(toShow);
            if (toShow) {
                this.RefreshDetail(levelType);

                if (this.rewardList == null) {
                    this.rewardList = new UI_RewardList(this.layout.rewardNode, EUIID.UI_ClueTaskMain);

                }
                this.rewardList.SetRewardList(CSVDrop.Instance.GetDropItem(dropId));
                this.rewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);
            }
        }
        private void RefreshDetail(EClueTaskType levelType) {
            uint level = 1;
            if (levelType == EClueTaskType.Adventure) {
                level = Sys_ClueTask.Instance.adventureLevel;
            }
            else if (levelType == EClueTaskType.Detective) {
                level = Sys_ClueTask.Instance.detectiveLevel;
            }

            // 1:
            this.leftLevelVd.Refresh(levelType, level, this);

            // 2:
            this.rightLevelVd.Refresh(levelType, level + 1, this);
            if (this.rightLevelVd.layout.slider != null) {
                this.rightLevelVd.layout.slider.gameObject.SetActive(false);
            }
        }
    }
}