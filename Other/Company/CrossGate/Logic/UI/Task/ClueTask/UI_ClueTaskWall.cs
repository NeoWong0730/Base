using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UIClueTaskWallLeftItem : UIElement {
        public ClueTaskPhase phase;
        public int phaseIndex;

        public Text number;
        public Text phaseName;

        protected override void Loaded() {
            this.phaseName = this.transform.Find("PhaseName").GetComponent<Text>();
            this.number = this.transform.Find("Image_Frame/Index").GetComponent<Text>();
        }
        public void Refresh(ClueTaskPhase phase, int phaseIndex) {
            this.phase = phase;
            this.phaseIndex = phaseIndex;

            TextHelper.SetText(this.number, (phaseIndex + 1).ToString());
            if (phase.CanVisible) {
                TextHelper.SetText(this.phaseName, phase.csv.PhasedTasksName);
            }
            else {
                TextHelper.SetText(this.phaseName, "?");
            }
        }
    }

    public class UIClueTaskWallRightYesItem : UIElement {
        public ClueTaskPhase phase;

        public CP_TransformContainer transformContainer;
        public Transform yes;
        public Image yesIcon;
        public Text yesText;
        public GameObject yesTextNode;

        public Button button;

        protected override void Loaded() {
            this.yes = this.transform.Find("Yes");
            this.yesText = this.transform.Find("Yes/Text_Find/Text").GetComponent<Text>();
            this.yesTextNode = this.transform.Find("Yes/Text_Find").gameObject;
            this.yesIcon = this.transform.Find("Yes/Image_Icon").GetComponent<Image>();

            this.button = this.gameObject.GetComponent<Button>();
            this.button.onClick.AddListener(this.OnBtnClicked);
        }
        private void OnBtnClicked() {
            UIManager.OpenUI(EUIID.UI_ClueTaskWallTips, true, this.phase);
        }
        public void Refresh(ClueTaskPhase phase, int phaseIndex) {
            this.phase = phase;

            this.yes.gameObject.SetActive(true);

            // 完成的时候，根据phaseIndex播放动画
            if (phase.csv.DisplayClueType == 1) {
                this.yesTextNode.gameObject.SetActive(false);
                this.yesIcon.gameObject.SetActive(true);

                ImageHelper.SetIcon(this.yesIcon, phase.csv.DisplayCluePara);
            }
            else {
                this.yesTextNode.gameObject.SetActive(true);
                this.yesIcon.gameObject.SetActive(false);

                TextHelper.SetText(this.yesText, phase.csv.DisplayCluePara);
            }
        }
    }

    public class UIClueTaskWallRightNoItem : UIElement {
        public ClueTaskPhase phase;

        public CP_TransformContainer transformContainer;
        public Transform no;
        public Text noText;

        public Canvas canvas;

        protected override void Loaded() {
            this.no = this.transform.Find("No");
            this.canvas = this.transform.GetComponent<Canvas>();
            this.noText = this.transform.Find("No/Image_Frame/Text").GetComponent<Text>();
        }

        public void Refresh(ClueTaskPhase phase, int phaseIndex) {
            this.phase = phase;

            this.no.gameObject.SetActive(true);
            TextHelper.SetText(this.noText, (phaseIndex + 1).ToString());
        }
        public void SetOrder(int order) {
            this.canvas.sortingOrder = order;
        }
    }

    public class UI_ClueTaskWall : UIBase, UI_ClueTaskWall_Layout.IListener {
        public ClueTask clueTask;

        public UI_ClueTaskWall_Layout Layout = new UI_ClueTaskWall_Layout();
        public UI_RewardList rewardList;
        private UI_CurrencyTitle ui_CurrencyTitle;

        public UIElementContainer<UIClueTaskWallLeftItem, uint> phaseVds = new UIElementContainer<UIClueTaskWallLeftItem, uint>();
        public UIElementContainer<UIClueTaskWallRightYesItem, uint> phaseYesClueVds = new UIElementContainer<UIClueTaskWallRightYesItem, uint>();
        public UIElementContainer<UIClueTaskWallRightNoItem, uint> phaseNoClueVds = new UIElementContainer<UIClueTaskWallRightNoItem, uint>();

        public int phaseIndex;

        public uint phaseId;
        public uint taskId;
        private TaskEntry taskEntry;

        #region
        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }
        protected override void OnOpen(object arg) {
            uint id = Convert.ToUInt32(arg);
            this.clueTask = Sys_ClueTask.Instance.tasks[id];

            this.phaseIndex = 0;
            this.phaseId = 0;
            this.taskId = 0;
            this.taskEntry = null;
        }
        protected override void OnOpened() {
            if (this.ui_CurrencyTitle == null) {
                this.ui_CurrencyTitle = new UI_CurrencyTitle(this.transform.Find("Animator/UI_Property").gameObject);
            }
        }
        protected override void OnShow() {
            if (this.rewardList == null) {
                this.rewardList = new UI_RewardList(this.Layout.rewardNode, EUIID.UI_ClueTaskWall);
            }

            this.rewardList.SetRewardList(CSVDrop.Instance.GetDropItem(this.clueTask.csv.Reward));
            this.rewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

            this.phaseYesClueVds.onFinish += this.OnRightLoaded;
            this.RefreshAll();
        }
        protected override void OnHide() {
            this.phaseYesClueVds.onFinish -= this.OnRightLoaded;            
        }
        private void OnRightLoaded() {
            this.Layout.horCoupleScrollRect.leftRect.horizontalNormalizedPosition = 1f;
        }
        private void RefreshAll() {
            var phaseList = this.clueTask.phases.list;
            this.GetCurrentPhase();

            ui_CurrencyTitle?.InitUi();

            this.RefreshLeft(phaseList);
            this.RefreshRight(phaseList);
            this.RefreshTips();
        }
        private void OnTraced(int menuId, uint id, TaskEntry taskEntry) {
            if (taskEntry == this.taskEntry) {
                this.RefreshTips();
            }
        }
        protected override void OnUpdate() {
            this.phaseVds.Update();
            this.phaseYesClueVds.Update();
            this.phaseNoClueVds.Update();
        }
        protected override void OnDestroy() {
            this.ui_CurrencyTitle?.Dispose();
            for (int i = 0; i < this.phaseVds.Count; ++i) {
                if (this.phaseVds.TryGetVdByIndex(i, out var vd)) {
                    vd.OnDestroy();
                }
            }
            for (int i = 0; i < this.phaseYesClueVds.Count; ++i) {
                if (this.phaseYesClueVds.TryGetVdByIndex(i, out var vd)) {
                    vd.OnDestroy();
                }
            }
            for (int i = 0; i < this.phaseNoClueVds.Count; ++i) {
                if (this.phaseNoClueVds.TryGetVdByIndex(i, out var vd)) {
                    vd.OnDestroy();
                }
            }

            this.phaseVds.Clear();
            this.phaseYesClueVds.Clear();
            this.phaseNoClueVds.Clear();
        }
        #endregion

        public void RefreshLeft(List<uint> phaseIds) {
            // 星星
            this.Layout.starLevel.Build((int)this.clueTask.csv.TaskStar, null);
            TextHelper.SetText(this.Layout.clueTaskName, this.clueTask.csv.TaskName);

            var leftList = new List<uint>(phaseIds);
            this.BuildOrRefreshLeft(leftList);
        }
        private void BuildOrRefreshLeft(List<uint> phaseIds) {
            this.phaseVds.BuildOrRefresh(this.Layout.leftProto, this.Layout.leftParent, phaseIds, (vd, id, indexOfVdList) => {
                vd.SetUniqueId((int)id);
                vd.Refresh(this.clueTask.phases[id], indexOfVdList);
            });
        }
        private void GetCurrentPhase() {
            this.clueTask.GetCurrent(out this.phaseIndex, out this.phaseId);
        }
        public void RefreshRight(List<uint> phaseIds) {
            this.Layout.rawImageLoader.Set(this.clueTask.csv.BG);

            List<uint> yesIds = phaseIds.FindAll((element) => {
                return this.clueTask.phases[element].isFinish;
            });
            List<uint> noIds = phaseIds.FindAll((element) => {
                return !this.clueTask.phases[element].isFinish;
            });

            this.phaseYesClueVds.BuildOrRefresh(this.Layout.rightLeftProto, this.Layout.rightLeftParent, yesIds, (vd, data, indexOfVdList) => {
                vd.SetUniqueId((int)data);
                vd.Refresh(this.clueTask.phases[data], indexOfVdList);
            });
            this.phaseNoClueVds.BuildOrRefresh(this.Layout.rightRightProto, this.Layout.rightRightParent, noIds, (vd, data, indexOfVdList) => {
                vd.SetUniqueId((int)data);
                vd.Refresh(this.clueTask.phases[data], indexOfVdList + yesIds.Count);
                vd.SetOrder(this.nSortingOrder + (noIds.Count - indexOfVdList) + 1);
            });

            int noCount = noIds.Count;
            if (noCount <= 0) {
                this.Layout.horCoupleScrollRect.SetCouple(0f);
            }
            else {
                float width = (noCount - 1) * this.Layout.horCoupleScrollRect.rightGrid.cellSize.x + this.Layout.horCoupleScrollRect.rightCellWidth;
                this.Layout.horCoupleScrollRect.SetCouple(width);
            }
        }
        private void RefreshTips() {
            if (!this.clueTask.CanVisible) {
                this.Layout.View_None.gameObject.SetActive(true);
                this.Layout.View_Finish.gameObject.SetActive(false);
                this.Layout.View_Now.gameObject.SetActive(false);

                TextHelper.SetText(this.Layout.View_None_Text, this.clueTask.phases[this.phaseId].csv.TaskUnableReceiveTip);
            }
            else if (this.clueTask.isFinish) {
                this.Layout.View_None.gameObject.SetActive(false);
                this.Layout.View_Finish.gameObject.SetActive(true);
                this.Layout.View_Now.gameObject.SetActive(false);

                TextHelper.SetText(this.Layout.View_Finish_Text, this.clueTask.csv.TaskCompleteDes);
            }
            else {
                this.Layout.View_None.gameObject.SetActive(false);
                this.Layout.View_Finish.gameObject.SetActive(false);
                this.Layout.View_Now.gameObject.SetActive(true);

                this.clueTask.phases[this.phaseId].GetCurrent(out int index, out this.taskId);
                this.taskEntry = Sys_Task.Instance.GetTask(this.taskId);
                if (this.taskEntry != null) {
#if UNITY_EDITOR
                    TextHelper.SetTaskText(this.Layout.taskName, this.taskEntry.csvTask.taskName);
                    this.Layout.taskName.text = this.Layout.taskName.text + this.taskEntry.id;
#else
                    TextHelper.SetText(Layout.taskName, taskEntry.csvTask.taskName);
#endif
                    TextHelper.SetTaskText(this.Layout.taskDesc, this.taskEntry.csvTask.taskDescribe);
                    if (!this.taskEntry.isTraced) {
                        TextHelper.SetText(this.Layout.gotoText, "追踪");
                    }
                    else {
                        TextHelper.SetText(this.Layout.gotoText, "前往");
                    }
                }
            }
        }

        #region 逻辑
        protected override void ProcessEventsForEnable(bool toRegister) {
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnTraced, this.OnTraced, toRegister);
            Sys_ClueTask.Instance.eventEmitter.Handle<ClueTaskPhase>(Sys_ClueTask.EEvents.OnClueTaskPhaseFinished, this.OnClueTaskPhaseFinished, toRegister);
        }
        private void OnClueTaskPhaseFinished(ClueTaskPhase clueTaskPhase) {
            this.RefreshAll();
        }
        #endregion

        #region UI
        public void OnBtnGotoClicked() {
            if (!this.taskEntry.IsFinish() && this.taskEntry.taskState != ETaskState.Submited) {
                if (!this.taskEntry.isTraced) {
                    Sys_Task.Instance.ReqTrace(this.taskId, true, true);
                }
                else {
                    Sys_Task.Instance.TryDoTask(this.taskEntry, true, false, true);

                    this.CloseSelf();
                    UIManager.CloseUI(EUIID.UI_ClueTaskMain);
                    Sys_Adventure.Instance.eventEmitter.Trigger(Sys_Adventure.EEvents.OnCLoseAdventureView);
                }
            }
            Sys_Adventure.Instance.ReportClickEventHitPoint("Task_ClueTaskWall_Goto:");
        }
        public void OnBtnReturnClicked() {
            this.CloseSelf();
        }
        #endregion
    }
}