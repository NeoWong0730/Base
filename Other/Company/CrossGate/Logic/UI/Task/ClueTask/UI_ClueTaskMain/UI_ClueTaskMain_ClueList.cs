using System;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_ClueTaskMain_ClueList : UI_ClueTaskMain.UI_CluetaskSubComponent {
        public class UIPopdownItem : UISelectableElement {
            public uint mapId;
            public string mapName;

            public Text text;
            public Button button;
            public GameObject highlight;

            protected override void Loaded() {
                this.button = this.transform.GetComponent<Button>();
                this.text = this.transform.Find("Text").GetComponent<Text>();
                this.highlight = this.transform.Find("Image").gameObject;
                this.button.onClick.AddListener(this.OnBtnClicked);
            }

            public void SetHighlight(bool setHighLight = false) {
                this.highlight.SetActive(setHighLight);
            }
            public void Refresh(uint mapId, int index) {
                this.mapId = mapId;

                if (mapId != 0) {
                    CSVMapInfo.Data csv = CSVMapInfo.Instance.GetConfData(mapId);
                    if (csv != null) {
                        TextHelper.SetText(this.text, csv.name);
                        this.mapName = this.text.text;
                    }
                }
                else {
                    this.mapName = "所有";
                    TextHelper.SetText(this.text, this.mapName);
                }
            }
            private void OnBtnClicked() {
                this.onSelected?.Invoke((int)this.mapId, true);
            }
            public override void SetSelected(bool toSelected, bool force) { this.OnBtnClicked(); }
        }

        public class UIClueTaskVd : UIElement {
            public ClueTask clueTask;

            public UI_ClueTaskMain_ClueList_ClueProtoLayout layout = new UI_ClueTaskMain_ClueList_ClueProtoLayout();

            protected override void Loaded() {
                this.layout.Parse(this.gameObject);
                this.layout.btn.onClick.AddListener(this.OnBtnClicked);
            }

            public void Refresh(ClueTask clueTask, int index) {
                this.clueTask = clueTask;

                int starCount = (int)clueTask.csv.TaskStar;
                this.layout.starLevel.Build(starCount, (go, idx) => {
                    go.SetActive(idx < starCount);
                });

                clueTask.RefreshStatus();
                TextHelper.SetText(this.layout.clueTaskName, clueTask.csv.TaskName);
#if UNITY_EDITOT
                layout.clueTaskName.text = layout.clueTaskName.text + index;
#endif
                this.layout.newClue.SetActive(clueTask.taskStatus == EClueTaskStatus.GotClue);
                this.layout.tracing.SetActive(clueTask.taskStatus == EClueTaskStatus.Tracing);
                this.layout.noClue.SetActive(clueTask.taskStatus == EClueTaskStatus.NoClue);

                ClueTaskPhase curentPhase = this.clueTask.currentPhase;
                bool canShowTip = (clueTask.taskStatus == EClueTaskStatus.NoClue) && curentPhase.csv.Display;
                this.layout.tipGo.SetActive(canShowTip);
                if (canShowTip) {
                    TextHelper.SetText(this.layout.tipText, curentPhase.csv.TaskUnableReceiveTip);
                }

                this.layout.first.SetActive(false);
                this.layout.rawImageLoader.Set(clueTask.csv.BG);

                if (clueTask.taskStatus == EClueTaskStatus.Tracing) {
                    this.layout.finishTimeNode.SetActive(false);
                    this.layout.pageNode.SetActive(true);
                    this.layout.pageIndexer.Refresh(clueTask.finishGroup.trueCount, clueTask.finishGroup.count);
                }
                else if (clueTask.taskStatus == EClueTaskStatus.GotClue) {
                    this.layout.finishTimeNode.SetActive(false);
                    this.layout.pageNode.SetActive(false);
                }
                else if (clueTask.taskStatus == EClueTaskStatus.NoClue) {
                    this.layout.finishTimeNode.SetActive(false);
                    this.layout.pageNode.SetActive(false);
                }
                if (clueTask.taskStatus == EClueTaskStatus.Finished) {
                    this.layout.finishTimeNode.SetActive(true);
                    this.layout.pageNode.SetActive(false);
                    this.layout.first.SetActive(clueTask.isFirst);
                    DateTime datetime = Sys_Time.ConvertToLocalTime(clueTask.finishTime);
                    TextHelper.SetText(this.layout.finishTimeText, datetime.ToString("yyyy.MM.dd"));
                }
            }
            private void OnBtnClicked() {
                UIManager.OpenUI(EUIID.UI_ClueTaskWall, true, this.clueTask.id);
                Sys_Adventure.Instance.ReportClickEventHitPoint("Task_ClueTaskWall_Open_ClueTaskId:" + this.clueTask.id);
            }
        }

        public UI_ClueTaskMain ui;
        public UI_ClueTaskMain_ClueList_Layout layout = new UI_ClueTaskMain_ClueList_Layout();
        public int mapId = -1;
        public EClueTaskType levelType;

        private int mapIndex = -1;

        public UI_ClueTaskMain_ClueList() { }
        public UI_ClueTaskMain_ClueList(UI_ClueTaskMain ui) { this.ui = ui; }
        public UIElementContainer<UIClueTaskVd, uint> vds = new UIElementContainer<UIClueTaskVd, uint>();
        public UIElementContainer<UIPopdownItem, uint> popdownVds = new UIElementContainer<UIPopdownItem, uint>();

        protected override void Loaded() {
            this.layout.Parse(this.gameObject);
            this.ProcessEventsForAwake(true);
        }
        public override void Reset() {
            this.levelType = EClueTaskType.None;
            this.mapId = -1;
            this.mapIndex = -1;
        }
        public override void Refresh(EClueTaskType type) {
            // if (levelType == type) { return; }
            this.levelType = type;
            //this.layout.popdownList.arrowAnchorPosition = new Vector3(83.25f, 1.64f, 0);
            this.popdownVds.BuildOrRefresh(this.layout.popdownList.optionProto, this.layout.popdownList.optionParent, Sys_ClueTask.Instance.allMaps, (vd, data, index) => {
                vd.SetUniqueId((int)data);
                vd.SetSelectedAction((mapId, force) => {
                    int vdIndex = index;
                    this.mapIndex = vdIndex;

                    this.popdownVds.ForEach((e) => {
                        e.SetHighlight(false);
                    });
                    vd.SetHighlight(true);

                    this.layout.popdownList.Expand(false);
                    this.layout.popdownList.SetSelected(vd.mapName);

                    //if (this.mapId != mapId)
                    {
                        this.mapId = mapId;
                        this.RefreshContent();
                    }
                });
                vd.Refresh(data, index);
                vd.SetHighlight(false);
            });

            if (this.mapIndex == -1) {
                this.mapIndex = 0;
                if (this.popdownVds.Count > 0) {
                    this.popdownVds[this.mapIndex].SetSelected(true, true);
                }
            }
            else {
                if (0 <= this.mapIndex && this.mapIndex < Sys_ClueTask.Instance.allMaps.Count) {
                    this.popdownVds[this.mapIndex].SetSelected(true, true);
                }
            }
        }
        public void RefreshContent() {
            if (this.mapId == -1) { return; }

            // Debug.LogError("mapid: " + mapId + "  levelType: " + levelType);
            var taskIds = Sys_ClueTask.Instance.GetTasks((uint)this.mapId, this.levelType);
            taskIds = taskIds.FindAll((id) => {
                return Sys_ClueTask.Instance.tasks[id].isTriggered == true;
            });
            taskIds.Sort((l, r) => {
                int result = 0;

                ClueTask ll = Sys_ClueTask.Instance.tasks[l];
                ClueTask rr = Sys_ClueTask.Instance.tasks[r];
                ll.RefreshStatus();
                rr.RefreshStatus();

                result = ll.taskStatus - rr.taskStatus;

                return result;
            });

            this.vds.BuildOrRefresh(this.layout.protoGo, this.layout.parent, taskIds, (vd, data, index) => {
                vd.SetUniqueId((int)data);
                vd.Refresh(Sys_ClueTask.Instance.tasks[data], index);
            });
        }

        protected override void Update() {
            this.vds.Update();
            this.popdownVds.Update();
        }
        public override void OnDestroy() {
            this.vds.Clear();
            this.popdownVds.Clear();
            base.OnDestroy();
        }
        protected override void ProcessEventsForAwake(bool toRegister) {
            Sys_ClueTask.Instance.eventEmitter.Handle<ClueTask, EClueTaskStatus, EClueTaskStatus>(Sys_ClueTask.EEvents.OnClueTaskStatusChanged, this.OnClueTaskStatusChanged, toRegister);
            Sys_ClueTask.Instance.eventEmitter.Handle<ClueTask>(Sys_ClueTask.EEvents.OnClueTaskFinished, this.OnClueTaskFinished, toRegister);
            Sys_ClueTask.Instance.eventEmitter.Handle<ClueTask>(Sys_ClueTask.EEvents.OnClueTaskTriggered, this.OnClueTaskTriggered, toRegister);
            Sys_ClueTask.Instance.eventEmitter.Handle<ClueTask>(Sys_ClueTask.EEvents.OnFirstFinish, this.OnFirstFinish, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnTrackedChanged, this.OnTrackedChanged, toRegister);
            Sys_ClueTask.Instance.eventEmitter.Handle<ClueTaskPhase>(Sys_ClueTask.EEvents.OnClueTaskPhaseFinished, this.OnClueTaskPhaseFinished, toRegister);
            //Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, toRegister);
        }
        private void OnClueTaskStatusChanged(ClueTask clueTask, EClueTaskStatus oldStatus, EClueTaskStatus newStatus) {
            this.RefreshContent();
        }
        private void OnClueTaskPhaseFinished(ClueTaskPhase phase) {
            this.RefreshContent();
        }
        private void OnClueTaskFinished(ClueTask clueTask) {
            this.RefreshContent();
        }
        private void OnClueTaskTriggered(ClueTask clueTask) {
            this.RefreshContent();
        }
        private void OnFirstFinish(ClueTask clueTask) {
            this.RefreshContent();
        }
        private void OnTrackedChanged(int _, uint __, TaskEntry ___) {
            this.RefreshContent();
        }
        private void OnTaskStatusChanged(TaskEntry _, ETaskState __, ETaskState ___) {
            this.RefreshContent();
        }
    }
}