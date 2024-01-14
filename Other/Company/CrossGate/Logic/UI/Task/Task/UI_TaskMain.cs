using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;
using static Logic.PropIconLoader;

namespace Logic {
    public class TaskPage {
        public uint id;

        public CSVTaskPage.Data csv {
            get { return CSVTaskPage.Instance.GetConfData(this.id); }
        }

        public bool IsOpen {
            get { return Sys_FunctionOpen.Instance.IsOpen(csv.functionId); }
        }

        public TaskPage(uint id) {
            this.id = id;
        }

        public bool ContainsCategory(uint catId) {
            return this.csv.taskTypes.Contains(catId);
        }

        public bool ContainsCategoryByTaskId(uint targetTaskId) {
            var taskEntry = Sys_Task.Instance.GetTask(targetTaskId);
            if (taskEntry != null) {
                return this.ContainsCategory(taskEntry.csvTaskCategory.id);
            }

            return false;
        }

        public List<uint> taskIds = new List<uint>();

        public List<uint> GetTrackTasks(bool needSort = false) {
            this.taskIds.Clear();
            var dict = Sys_Task.Instance.trackedTasks;
            foreach (var kvp in dict) {
                if (this.ContainsCategory((uint) kvp.Key)) {
                    for (int i = 0, length = kvp.Value.Count; i < length; ++i) {
                        this.taskIds.Add(kvp.Value[i].id);
                    }
                }
            }

            if (needSort && this.taskIds.Count > 1) {
                this.taskIds.Sort((l, r) => { return Sys_Task.Instance.GetTask(l).csvTaskCategory.priority - Sys_Task.Instance.GetTask(r).csvTaskCategory.priority; });
            }

            return this.taskIds;
        }

        public List<uint> HandleTrackTasks(uint taskId) {
            this.taskIds = this.taskIds.FindAll((id) => {
                var taskEntry = Sys_Task.Instance.GetTask(id);
                int taskCategory = taskEntry.csvTask.taskCategory;
                return !Sys_Task.Instance.GetTab(taskCategory).csv.MainPanelShow;
            });

            bool hasFakeLove = false;
            // 爱心挑战
            if (this.ContainsCategory((uint) ETaskCategory.Love) || this.ContainsCategory((uint) ETaskCategory.Challenge)) {
                // bool containsLove = ContainsTaskCat_Love;
                // bool containsChannlenge = ContainsTaskCat_Challenge;
                // 有 没被追踪，但是没完成的爱心挑战任务
                int index = this.taskIds.FindIndex((e) => {
                    var taskEntry = Sys_Task.Instance.GetTask(e);
                    return
                        (taskEntry.csvTask.taskCategory == (int) ETaskCategory.Love) ||
                        (taskEntry.csvTask.taskCategory == (int) ETaskCategory.Challenge);
                });

                if (index == -1) {
                    // 插入特殊id
                    if (Sys_Task.Instance.receivedTasks.TryGetValue((int) ETaskCategory.Love, out var dict) && dict.Count > 0) {
                        hasFakeLove = true;
                        this.taskIds.Insert(0, Sys_Task.lovalTipId);
                    }
                    else if (Sys_Task.Instance.receivedTasks.TryGetValue((int) ETaskCategory.Challenge, out dict) && dict.Count > 0) {
                        hasFakeLove = true;
                        this.taskIds.Insert(0, Sys_Task.challengeTipId);
                    }
                }
            }

            // 找不到线索任务，则提示一个 丢失线索：通过新插入一个类型的线索任务去实现。
            if (this.ContainsCategory((uint) ETaskCategory.Clue)) {
                if (Sys_ClueTask.Instance.IsAnyVisibleButUnFinish()) {
                    int index = this.taskIds.FindIndex((e) => {
                        var taskEntry = Sys_Task.Instance.GetTask(e);
                        return taskEntry.csvTask.taskCategory == (int) ETaskCategory.Clue;
                    });
                    if (index == -1) {
                        this.taskIds.Add(Sys_Task.clueTipId);
                    }
                }
            }

            // 主线切页
            if (this.ContainsCategory((uint) ETaskCategory.Trunk)) {
                // 选中的任务排在主线之下，也就是第2个位置
                int index = this.taskIds.FindIndex((id) => {
                    return Sys_Task.Instance.GetTask(id).csvTask.taskCategory != (int) ETaskCategory.Trunk && id == taskId;
                });
                if (index != -1 && this.taskIds.Count > index) {
                    uint id = this.taskIds[index];
                    this.taskIds.RemoveAt(index);
                    // 主线追踪上限为1
                    this.taskIds.Insert(1, id);
                }
            }
            else {
                // 非主线切页的时候才会插入空tip
                if (this.taskIds.Count <= 0) {
                    // 添加一个空列表提示
                    this.taskIds.Add(Sys_Task.emptyTipId);
                }
                else {
                    int index = this.taskIds.FindIndex((id) => {
                        return id == taskId;
                    });
                    if (index != -1 && this.taskIds.Count > index) {
                        uint id = this.taskIds[index];
                        this.taskIds.RemoveAt(index);
                        if (hasFakeLove) {
                            this.taskIds.Insert(1, id);
                        }
                        else {
                            this.taskIds.Insert(0, id);
                        }
                    }
                }
            }

            return this.taskIds;
        }
    }

    public class TaskPages {
        public Dictionary<uint, TaskPage> pages = new Dictionary<uint, TaskPage>();

        public void Fill() {
            this.pages.Clear();

            var dataList = CSVTaskPage.Instance.GetAll();
            for (int i = 0, length = dataList.Count; i < length; ++i)
            {
                var line = dataList[i];
                var page = new TaskPage(line.id);
                this.pages.Add(line.id, page);
            }
        }

        public void SetCurrentTask(uint pageId, uint taskId) {
            if (this.pages.TryGetValue(pageId, out var page)) {
            }
        }

        public bool CanSelected(uint targetTaskId, out uint catId, out uint pageId) {
            var taskEntry = Sys_Task.Instance.GetTask(targetTaskId);

            pageId = 0;
            catId = 0;
            foreach (var page in this.pages) {
                if (page.Value.ContainsCategory(taskEntry.csvTaskCategory.id)) {
                    catId = taskEntry.csvTaskCategory.id;
                    pageId = page.Key;
                    return page.Value.IsOpen;
                }
            }

            return false;
        }
    }

    public class UI_TaskMainItem : UISelectableElement {
        public int index;
        public TaskEntry taskEntry;
        public UI_TaskMain taskMain;
        public TaskPage page;

        public Text typeText;
        public GameObject remainTimeNode;
        public MonoLife remainTime;
        public Text remainTimeText;
        public Transform remainTimePtr;
        public Image typeImage;
        public Text titleText;
        public Text messageDesc;
        public Transform imageOk;
        public GameObject specialIconNode;
        public Text textNumber;
        public ToggleCD toggleCD;
        public CP_SliderLerp slider;

        public GameObject sliderFx;
        public GameObject guideFx;
        public GameObject selectedFx;
        public GameObject traceFx;
        public Animator animator;

        public ParticleSizeSetter sizeSetter;

        public GameObject fingerGo;
        public GameObject tipGo;
        public Text tipText;

        public PropItem propItem = new PropItem();
        public ShowItemData itemData = new ShowItemData();

        private int animationId_ToOpen;

        private void OnUpdate() {
            if (this.taskEntry?.currentTaskGoal?.timer != null) {
                float remain = this.taskEntry.currentTaskGoal.timer.GetRemainTime();
                float duration = this.taskEntry.currentTaskGoal.timer.duration;
                if (remain > 0) {
                    var z = Mathf.Lerp(0f, 360f, remain / duration);
                    this.remainTimePtr.localEulerAngles = new Vector3(0f, 0f, z);
                    this.remainTimeText.text = LanguageHelper.TimeToString((uint) remain, LanguageHelper.TimeFormat.Type_4);
                }
                else {
                    this.remainTimePtr.localEulerAngles = Vector3.zero;
                    this.remainTimeText.text = "00:00:00";
                }
            }
        }

        protected override void Loaded() {
            this.sizeSetter = this.gameObject.GetComponent<ParticleSizeSetter>();
            this.remainTimeNode = this.transform.Find("Time").gameObject;
            this.remainTimeText = this.transform.Find("Time/Text_Message").GetComponent<Text>();
            this.remainTimePtr = this.transform.Find("Time/Image1/Ptr");
            this.remainTime = this.transform.Find("Time/Text_Message").GetComponent<MonoLife>();
            this.remainTime.onUpdate = this.OnUpdate;

            this.imageOk = this.transform.Find("Image_Black/Image_ok");
            this.typeText = this.transform.Find("Image_Black/Image_Type/Text_Type").GetComponent<Text>();
            this.typeImage = this.transform.Find("Image_Black/Image_Type").GetComponent<Image>();
            this.titleText = this.transform.Find("Image_Black/Text_Title").GetComponent<Text>();
            this.messageDesc = this.transform.Find("Image_Black/Text_Message").GetComponent<Text>();
            this.textNumber = this.transform.Find("Image_Black/Text_Title/Text_Num").GetComponent<Text>();
            this.slider = this.transform.Find("Image_Black/Slider").GetComponent<CP_SliderLerp>();
            this.specialIconNode = this.transform.Find("Image_Black/Image_Award").gameObject;
            this.propItem.BindGameObject(this.transform.Find("Image_Black/Image_Award/PropItem").gameObject);

            this.fingerGo = this.transform.Find("Image_Black/Node/Finger").gameObject;
            this.tipGo = this.transform.Find("Image_Black/Node/Tips").gameObject;
            this.tipText = this.transform.Find("Image_Black/Node/Tips/Text").GetComponent<Text>();

            this.toggleCD = this.gameObject.GetComponent<ToggleCD>();

            this.sliderFx = this.transform.Find("Image_Black/Fx_ui_TaskTrace").gameObject;
            this.guideFx = this.transform.Find("Image_Black/Fx_ui_Select01").gameObject;
            this.traceFx = this.transform.Find("Image_Black/Fx_ui_TaskScroll").gameObject;
            this.selectedFx = this.transform.Find("Image_Black/Image_Select/Fx_ui_Select").gameObject;
            this.animator = this.transform.GetComponent<Animator>();

            this.animationId_ToOpen = Animator.StringToHash("ToOpen");

            this.toggleCD.onValueTrue = this.OnValueChanged;
        }

        private void NormalRefresh(int index, TaskEntry taskEntry, UI_TaskMain taskMain, TaskTab taskTab) {
            bool isFinish = taskEntry.IsFinish();
            bool showSlider = taskEntry.csvTask.WhetherShowProgressBar;
            string _ = null;
            if (index == 0 && taskEntry.CanDo(false, ref _)) {
                this.guideFx.SetActive(Sys_Task.Instance.ToOpTaskFlag);
            }

            // 这里track状态其实不合理，将表现层数据存储在逻辑层。后面修改吧
            if (taskEntry.traceType == ETaskTraceType.Trace) {
                this.animator.SetBool(this.animationId_ToOpen, true);
                taskEntry.traceType = ETaskTraceType.None;
                this.traceFx.SetActive(true);
            }

            if (isFinish) {
                uint submitNpcId = taskEntry.csvTask.submitNpc;
                string mapName = "";
                string npcName = "";
                bool hasSubmitNpc = submitNpcId != 0;
                CSVNpc.Data npcData = CSVNpc.Instance.GetConfData(submitNpcId);
                if (npcData != null) {
                    npcName = LanguageHelper.GetNpcTextContent(npcData.name);

                    uint mapID = npcData.mapId;
                    CSVMapInfo.Data csvMap = CSVMapInfo.Instance.GetConfData(mapID);
                    if (csvMap != null) {
                        mapName = LanguageHelper.GetTextContent(csvMap.name);
                    }
                }

                if (hasSubmitNpc) {
                    TextHelper.SetTaskText(this.messageDesc, 1601000001, mapName, npcName);
                }
                else {
                    TextHelper.SetTaskText(this.messageDesc, 1601000002);
                }

                this.slider.Set(1f);
            }
            else {
                taskEntry.GetProgress(out uint x, out uint y);
                if (!taskEntry.csvTask.conditionType) {
                    if (taskEntry.csvTask.taskCategory == (int) ETaskCategory.ClueTip) {
                        TextHelper.SetTaskText(this.messageDesc, taskEntry.csvTask.taskContent[0]);
                    }
                    else {
                        this.messageDesc.text = taskEntry.currentTaskContent;
                    }

                    TextHelper.SetTaskText(this.textNumber, 1601000003, x.ToString(), y.ToString());
                }
                else {
                    TextHelper.SetTaskText(this.messageDesc, taskEntry.csvTask.taskContent[0], (taskEntry.TotalProgress * 100).ToString(), "100");
                    TextHelper.SetTaskText(this.textNumber, 1601000003, Mathf.FloorToInt(taskEntry.TotalProgress * 100).ToString(), "100");
                }

                if (showSlider) {
                    float totalProgress = taskEntry.TotalProgress;
                    if (totalProgress != this.slider.slider.value) {
                        this.sliderFx.SetActive(true);
                        this.slider.Set(totalProgress);
                    }
                }
            }
        }

        public void Refresh(int index, TaskEntry taskEntry, UI_TaskMain taskMain, TaskPage page) {
            this.index = index;
            this.taskEntry = taskEntry;
            this.id = (int) taskEntry.id;
            this.taskMain = taskMain;
            this.page = page;

            this.traceFx.SetActive(false);
            this.sliderFx.SetActive(false);
            this.guideFx.SetActive(false);

            this.specialIconNode.SetActive(false);
            this.imageOk.gameObject.SetActive(false);
            this.slider.gameObject.SetActive(false);
            this.fingerGo.SetActive(false);
            this.titleText.gameObject.SetActive(false);
            this.remainTimeNode.SetActive(false);

            if (taskEntry.id == Sys_Task.clueTipId) {
                TextHelper.SetText(this.typeText, 1600000013);
                TextHelper.SetTaskText(this.messageDesc, 1729999980);
            }
            else if (taskEntry.id == Sys_Task.lovalTipId || taskEntry.id == Sys_Task.challengeTipId) {
                TextHelper.SetText(this.typeText, 1600000013);
                TextHelper.SetText(this.messageDesc, 1600000014);
            }
            else if (taskEntry.id == Sys_Task.emptyTipId) {
                TextHelper.SetText(this.typeText, 1600000013);
                string pageName = LanguageHelper.GetTextContent(page.csv.name);
                TextHelper.SetText(this.messageDesc, LanguageHelper.GetTextContent(1600000012, pageName));
            }
            else {
                this.specialIconNode.SetActive(taskEntry.csvTask.specialRewardShow != 0);
                this.titleText.gameObject.SetActive(true);

                this.fingerGo.SetActive(taskEntry.csvTask.TraceGuide && taskEntry.isTraced && !taskEntry.clickTrack);

                bool isFinish = taskEntry.IsFinish();
                bool showSlider = taskEntry.csvTask.WhetherShowProgressBar;
                this.imageOk.gameObject.SetActive(isFinish);
                this.slider.gameObject.SetActive(showSlider);
                this.slider.Set(0f);

                TextHelper.SetText(this.textNumber, 1000012, (taskEntry.TotalProgress * 100f).ToString());

                TaskTab taskTab = Sys_Task.Instance.GetTab(taskEntry.csvTask.taskCategory);
                TextHelper.SetText(this.typeText, LanguageHelper.GetTextContent(taskTab.contentId), CSVWordStyle.Instance.GetConfData(taskTab.csv.wordStyle));
                ImageHelper.SetIcon(this.typeImage, taskTab.csv.typeImage);

                bool notShow = taskEntry.currentTaskGoal != null && ((taskEntry.currentTaskGoal.IsEnd) || (taskEntry.currentTaskGoal.endTime == 0) || taskEntry.currentTaskGoal.csv.LimitTime == 0);
                this.remainTimeNode.SetActive(!notShow);

                string taskName = LanguageHelper.GetTaskTextContent(taskEntry.csvTask.taskName);
                TextHelper.SetText(this.titleText, taskName, CSVWordStyle.Instance.GetConfData(taskTab.csv.wordStyle));
#if DEBUG_MODE
                this.titleText.text = this.titleText.text + index + "|" + this.id.ToString();
#endif

                this.NormalRefresh(index, taskEntry, taskMain, taskTab);

                uint specialItemId = this.taskEntry.csvTask.specialRewardShow;
                this.itemData.Refresh(specialItemId, 0, true, false, false, false, false, false, false);
                this.propItem.SetData(this.itemData, EUIID.UI_TaskList);

                string reason = null;
                bool canDoOnlyCSVCondition = taskEntry.CanDoOnlyCSVCondition(ref reason);
                if (!canDoOnlyCSVCondition && taskEntry.csvTask.LvConfineTips != 0 && !Sys_Task.Instance.IsSameTaskLimited(taskEntry.id)) {
                    this.tipGo.SetActive(true);
                    TextHelper.SetTaskText(this.tipText, taskEntry.csvTask.LvConfineTips);
                }
                else {
                    this.tipGo.SetActive(false);
                }

                if (!canDoOnlyCSVCondition) {
                    TextHelper.SetText(this.messageDesc, reason);
                }
            }

            this.animator.SetBool(this.animationId_ToOpen, false);

            bool isSelected = taskEntry.id == taskMain.currentTaskId;
            this.selectedFx.SetActive(isSelected);
            this.toggleCD.toggle.Highlight(isSelected);

            if (this.sizeSetter != null) {
                this.sizeSetter.Set();
            }
        }

        private void OnValueChanged() {
            this.taskEntry.clickTrack = true;

            if (this.taskEntry.id == Sys_Task.clueTipId) {
                UIManager.OpenUI(EUIID.UI_ClueTaskMain, false);
                this.taskMain.RefreshPages();
            }
            else if (this.taskEntry.id == Sys_Task.lovalTipId) {
                // 如果当前地图没有爱心，则查找挑战
                var ls = Sys_Task.Instance.GetReceivedTasksByMapId(Sys_Map.Instance.CurMapId, ETaskCategory.Love);
                uint taskId = 0;
                var taskType = ETaskCategory.Love;
                if (ls == null || ls.Count <= 0) {
                    ls = Sys_Task.Instance.GetReceivedTasksByMapId(Sys_Map.Instance.CurMapId, ETaskCategory.Challenge);
                    if (ls != null && ls.Count > 0) {
                        taskId = ls[0].id;
                        taskType = ETaskCategory.Challenge;
                    }
                }
                else {
                    taskId = ls[0].id;
                }

                UIManager.OpenUI(EUIID.UI_TaskList, false, new Tuple<uint, uint>((uint) taskType, taskId));
                this.taskMain.RefreshPages();
            }
            else if (this.taskEntry.id == Sys_Task.challengeTipId) {
                var ls = Sys_Task.Instance.GetReceivedTasksByMapId(Sys_Map.Instance.CurMapId, ETaskCategory.Challenge);
                uint taskId = ls != null && ls.Count > 0 ? ls[0].id : 0;
                UIManager.OpenUI(EUIID.UI_TaskList, false, new Tuple<uint, uint>((uint) ETaskCategory.Challenge, taskId));
                this.taskMain.RefreshPages();
            }
            else if (this.taskEntry.id == Sys_Task.emptyTipId) {
                // do nothing
                this.taskMain.RefreshPages();
            }
            else {
                this.taskMain.currentTaskId = (int) this.taskEntry.id;
                Sys_Task.Instance.lastLimitedTaskId = this.taskEntry.id;

                var uiID = this.taskEntry.csvTask.LvConfineTipsUI;
                string reason = null;
                if (uiID != 0 && !this.taskEntry.CanDoOnlyCSVCondition(ref reason)) {
                    UIManager.OpenUI((EUIID)uiID, false);
                }

                if (this.taskEntry.csvTask.TraceGuide) {
                    this.fingerGo.SetActive(false);
                    Sys_Task.Instance.TryDoTask(this.taskEntry, true, false, true);
                }
                else {
                    Sys_Task.Instance.TryDoTask(this.taskEntry, true, false, true);
                }
            }
        }

        public override void SetSelected(bool toSelected, bool force) {
            this.toggleCD.toggle.SetSelected(toSelected, force);
        }
    }

    public class UI_TaskPageItem : UISelectableElement {
        public CP_Toggle toggle;
        public Text drakName;
        public Text lightName;
        public GameObject fx;
        public LayoutElementWidthSetter widthSetter;

        protected override void Loaded() {
            this.fx = this.transform.Find("Fx").gameObject;

            this.drakName = this.transform.Find("Drak/Text").GetComponent<Text>();
            this.lightName = this.transform.Find("Light/Text").GetComponent<Text>();
            this.widthSetter = this.transform.GetComponent<LayoutElementWidthSetter>();

            this.toggle = this.transform.GetComponent<CP_Toggle>();
            this.toggle.onValueChanged.AddListener(this.OnSwicth);
        }

        private void OnSwicth(bool status) {
            if (status) {
                this.onSelected?.Invoke(this.id, true);
            }
        }

        public override void SetSelected(bool toSelected, bool force) {
            this.toggle.SetSelected(toSelected, true);
        }

        public void Refresh(uint taskPageId, int pageCount) {
            var csv = CSVTaskPage.Instance.GetConfData(taskPageId);
            if (csv != null) {
                TextHelper.SetText(this.drakName, csv.name);
                this.lightName.text = this.drakName.text;
            }

            // this.fx.SetActive(false);

            this.widthSetter.Set(pageCount);
        }
    }

    public class UI_TaskMain : UIComponent {
        public GameObject itemProto;
        public ScrollRect scrollRect;

        public GameObject pageProto;
        public Transform pageParent;

        public int currentTaskId = -1;
        public UIElementCollector<UI_TaskMainItem> taskVds = new UIElementCollector<UI_TaskMainItem>();

        public int currentPageId = -1;
        public TaskPages pages = new TaskPages();
        public List<uint> pageIds = new List<uint>();
        public UIElementCollector<UI_TaskPageItem> pageVds = new UIElementCollector<UI_TaskPageItem>();

        protected override void Loaded() {
            this.pageProto = this.transform.Find("TaskScroll/Pages/PageProto").gameObject;
            this.pageParent = this.pageProto.transform.parent;

            this.itemProto = this.transform.Find("TaskScroll/Tasks/Viewport/Item").gameObject;
            this.scrollRect = this.transform.Find("TaskScroll/Tasks").GetComponent<ScrollRect>();

            this.pages.Fill();
            this.pageIds = new List<uint>(this.pages.pages.Keys);

            this.currentTaskId = (int) Sys_Task.Instance.currentTaskId;
            this.currentPageId = -1;
            this.ProcessEvents(true);
        }

        public override void OnDestroy() {
            this.currentPageId = -1;
            this.currentTaskId = -1;

            this.taskVds.Clear();
            this.pageVds.Clear();

            this.refreshListTimer?.Cancel();

            this.ProcessEvents(false);

            base.OnDestroy();
        }

        private void OnRefreshPage(UI_TaskPageItem vd, uint pageId, int index) {
            var id = (int) pageId;
            vd.SetUniqueId(id);

            vd.toggle.id = id;
            vd.toggle.UnRegisterCondition();
            vd.toggle.RegisterCondition(() => {
                bool can = false;
                if (this.pages.pages.TryGetValue((uint) vd.id, out var page)) {
                    can = page.IsOpen;
                }

                if (!can) {
                    // 本来不应该在判断的接口中弹出提示，但是toggle没有好的时机，所以暂且这样
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(page.csv.unlockTip));
                }

                return can;
            });

            vd.SetSelectedAction((_, force) => {
                this.currentPageId = (int) pageId;
                this.RefreshTasks();
            });
            vd.Refresh(pageId, this.pageIds.Count);
        }

        private void OnRefreshTask(UI_TaskMainItem vd, uint taskId, int index) {
            TaskEntry taskEntry = Sys_Task.Instance.GetTask(taskId);
            vd.SetUniqueId((int) taskId);
            if (taskEntry.NoBody) {
                vd.Show();
                vd.Refresh(index, taskEntry, this, this.pages.pages[(uint) this.currentPageId]);
                vd.SetName(taskId.ToString());
            }
            else {
                var tab = Sys_Task.Instance.GetTab(taskEntry.csvTask.taskCategory);
                if (tab == null || !tab.IsOpen()) {
                    vd.Hide();
                }
                else {
                    vd.Show();
                    vd.Refresh(index, taskEntry, this, this.pages.pages[(uint) this.currentPageId]);
                    vd.SetName(taskId.ToString());
                }
            }
        }

        public void RefreshPages(bool forceScrollPosition = false) {
            this.pageVds.BuildOrRefresh<uint>(this.pageProto, this.pageParent, this.pageIds, this.OnRefreshPage);

            if (this.currentPageId != -1) {
                if (!this.pageVds.TryGetVdById(this.currentPageId, out var _)) {
                    this.currentPageId = -1;
                }
            }

            if (this.currentPageId == -1) {
                if (this.pageIds.Count > 0) {
                    this.currentPageId = (int) this.pageIds[0];
                }
            }

            if (this.pageVds.TryGetVdById(this.currentPageId, out var pageVd)) {
                pageVd?.SetSelected(true, true);
            }


            if (forceScrollPosition) {
                this.scrollRect.verticalNormalizedPosition = 1f;
            }
        }

        private void RefreshTasks() {
            var currentPage = this.pages.pages[(uint) this.currentPageId];
            currentPage.GetTrackTasks(true);
            currentPage.HandleTrackTasks((uint)this.currentTaskId);

            this.taskVds.BuildOrRefresh(this.itemProto, this.itemProto.transform.parent, currentPage.taskIds, this.OnRefreshTask);

            if (this.currentTaskId != -1) {
                if (!this.taskVds.TryGetVdById(this.currentTaskId, out var _)) {
                    this.currentTaskId = -1;
                }
            }

            if (this.currentTaskId == -1) {
                if (Sys_Task.Instance.currentTaskEntry != null) {
                    this.currentTaskId = (int) Sys_Task.Instance.currentTaskEntry.id;
                }
            }

            if (this.taskVds.TryGetVdById(this.currentTaskId, out var taskVd)) {
                taskVd?.SetSelected(true, false);
            }

            this.refreshListTimer?.Cancel();
            this.refreshListTimer = Timer.RegisterOrReuse(ref this.refreshListTimer, 0.08f, this.OnTimerCompleted);
        }

        private void OnTimerCompleted() {
            FrameworkTool.ForceRebuildLayout(this.gameObject);
        }

        private Timer refreshListTimer;

        #region 事件处理

        public void ProcessEvents(bool toRegister) {
            // 限时任务
            Sys_Task.Instance.eventEmitter.Handle<uint, uint, TaskEntry>(Sys_Task.EEvents.OnStartTimeLimit, this.OnStartTimeLimit, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<uint>(Sys_Task.EEvents.OnEndTimeLimit, this.OnEndTimeLimit, toRegister);
            // 增加删除任务
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnReceived, this.OnReceived, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnSubmited, this.OnSubmited, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnForgoed, this.OnForgeted, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnTrackedChanged, this.OnTrackedChanged, toRegister);

            // 刷新数据
            Sys_Task.Instance.eventEmitter.Handle(Sys_Task.EEvents.OnRefreshAll, this.OnRefreshAll, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnRefreshed, this.OnRefreshed, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnFinished, this.OnFinished, toRegister);
            // 追踪变化
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnTraced, this.OnTraced, toRegister);
            //Sys_Task.Instance.eventEmitter.Handle<int>(Sys_Task.EEvents.OnTabAdded, OnMenuAdded, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<TaskEntry, int, int>(Sys_Task.EEvents.OnTargetIndexChanged, this.OnTargetIndexChanged, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<TaskEntry>(Sys_Task.EEvents.OnCurrentTaskChanged, this.OnCurrentTaskChanged, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<bool>(Sys_Task.EEvents.OnOverTimeNoOpTask, this.OnOverTimeNoOpTask, toRegister);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, this.OnFunctionOpen, toRegister);
            // 这里监听存在问题就是，主线任务先删除后添加的情况下，就导致后面的其他类型的任务顶上，然后高亮在其他任务上，但是后来的主线任务会自动执行
            // 这就会导致奇怪的问题
            // Sys_Task.Instance.eventEmitter.Handle<int>(Sys_Task.EEvents.OnTabRemoved, OnMenuRemoved, toRegister);

            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, this.OnUpdateLevel, toRegister);
        }

        private void TryNavTo(TaskEntry taskEntry, bool setSysCurrentId, out uint pageId) {
            if (this.pages.CanSelected(taskEntry.id, out uint _, out pageId)) {
                this.currentPageId = (int) pageId;
                this.currentTaskId = (int) taskEntry.id;

                if (setSysCurrentId) {
                    Sys_Task.Instance.currentTaskEntry = taskEntry;
                }
            }
        }

        private void OnReceived(int menuId, uint id, TaskEntry taskEntry) {
            if (taskEntry != null) {
                if (this.pages.CanSelected(taskEntry.id, out uint _, out uint pageId)) {
                    if (this.pageVds.TryGetVdById((int)pageId, out var vd)) {
                        vd.fx.SetActive(false);
                        vd.fx.SetActive(true);
                    }

                    if (Sys_Task.Instance.currentTaskEntry == null || Sys_Task.Instance.currentTaskEntry.id == id) {
                        this.TryNavTo(taskEntry, true, out pageId);
                    }
                    else if (taskEntry.csvTask.taskCategory == (int)ETaskCategory.NPCFavorability) {
                        // 可能存在情况就是：点击接取心愿任务的时候， 因为服务器回包的原因，玩家先点击了主线或者其他任务去执行，此时回包回来之后，需要终止正在执行的任务行为，如果不终止
                        // 会存在高亮选中心愿，但是正在执行的却是刚才点的主线的情况
                        Sys_Task.Instance.StopAutoTask(true);
                        this.TryNavTo(taskEntry, true, out pageId);
                    }
                }

                this.RefreshPages();
            }
        }

        private void OnRefreshAll() {
            this.RefreshPages();
        }

        private void OnSubmited(int menuId, uint id, TaskEntry taskEntry) {
            if (this.currentTaskId == id) {
                this.currentTaskId = -1;
            }

            this.RefreshPages();
        }

        private void OnForgeted(int menuId, uint id, TaskEntry taskEntry) {
            this.RefreshPages();
        }

        // 单个任务数据刷新
        private void OnRefreshed(int menuId, uint id, TaskEntry taskEntry) {
            // 手动做任务导致任意一个任务进度刷新，立即选中
            if (taskEntry != null) {
                if (!Sys_Task.Instance.AutoRunMode) {
                    if (id == this.currentTaskId) {
                        this.TryNavTo(taskEntry, true, out uint pageId);
                    }
                    else {
                        // 如果没有当前选中，同时两个任务都刷新了，此时应该按照优先级切换
                        // 这里偷懒了，随机切换
                        // 和策划商量过
                        this.TryNavTo(taskEntry, true, out uint pageId);
                    }
                }
                this.RefreshPages();
            }
        }

        private void OnTraced(int menuId, uint id, TaskEntry taskEntry) {
            this.RefreshPages();
        }

        // 任务追踪状态变化
        private void OnTrackedChanged(int menuId, uint id, TaskEntry taskEntry) {
            if (taskEntry != null) {
                this.RefreshPages();
            }
        }

        private void OnFinished(int menuId, uint taskId, TaskEntry taskEntry) {
            this.RefreshPages();
        }

        private void OnTargetIndexChanged(TaskEntry taskEntry, int oldIndex, int newIndex) {
            this.RefreshPages();
        }

        private void OnCurrentTaskChanged(TaskEntry taskEntry) {
            if (taskEntry != null) {
                this.TryNavTo(taskEntry, false, out uint pageId);

                this.RefreshPages();
            }
        }

        // 长时间未做任务提示特效
        private void OnOverTimeNoOpTask(bool toShow) {
            if (this.currentPageId == 1 && this.taskVds.Count > 0) {
                // 主线栏
                var vd = this.taskVds[0];
                vd.guideFx.SetActive(Sys_Task.Instance.ToOpTaskFlag);
            }
        }

        private void OnUpdateLevel() {
            this.RefreshPages();
        }

        private void OnFunctionOpen(Sys_FunctionOpen.FunctionOpenData funcData) {
            // 线索任务
            if (funcData.id == EFuncOpen.FO_ClueTask) {
                this.RefreshPages();
            }
            else {
                foreach (var kvp in this.pages.pages) {
                    if (funcData.id == kvp.Value.csv.functionId) {
                        this.RefreshPages();
                        break;
                    }
                }
            }
        }

        private void OnStartTimeLimit(uint taskId, uint taskIndex, TaskEntry taskEntry) {
            this.RefreshPages();
        }

        private void OnEndTimeLimit(uint taskId) {
            this.RefreshPages();
        }

        #endregion
    }
}