using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class TaskRewardItem : UIComponent {
        public Image image;
        public Text textCount;

        protected override void Loaded() {
            this.image = this.transform.Find("Image").GetComponent<Image>();
            this.textCount = this.transform.Find("Text").GetComponent<Text>();
        }

        public void Refresh(uint id, uint count) {
            CSVItem.Data csv = CSVItem.Instance.GetConfData(id);
            if (csv != null) {
                ImageHelper.SetIcon(this.image, csv.small_icon_id, true);
                this.textCount.text = count.ToString();
            }
        }
    }

    public class UI_TaskTabItem : UISelectableElement {
        public TaskTab taskTab;

        public CP_Toggle toggle;
        public Text normalTypeText;
        public Image normalTypeIcon;
        public Text lightTypeText;
        public Image lightTypeIcon;

        public UI_TaskTabItem() : base() {
        }

        protected override void Loaded() {
            this.toggle = this.gameObject.GetComponent<CP_Toggle>();
            this.toggle.onValueChanged.AddListener(this.Switch);

            this.normalTypeIcon = this.transform.Find("Btn_Menu_Dark/Image_Icon").GetComponent<Image>();
            this.normalTypeText = this.transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();

            this.lightTypeIcon = this.transform.Find("Image_Menu_Light/Image_Icon").GetComponent<Image>();
            this.lightTypeText = this.transform.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
        }

        public void Refresh(int category, bool needSelect) {
            this.id = category;
            this.taskTab = Sys_Task.Instance.GetTab(category);

            this.toggle.Highlight(false);
            TextHelper.SetText(this.normalTypeText, this.taskTab.taskTabDetailName);
            ImageHelper.SetIcon(this.normalTypeIcon, this.taskTab.iconId);

            TextHelper.SetText(this.lightTypeText, this.taskTab.taskTabDetailName);
            ImageHelper.SetIcon(this.lightTypeIcon, this.taskTab.lightIconId);

            if (needSelect) {
                SetSelected(true, true);
            }
        }

        public void Switch(bool arg) {
            if (arg) {
                this.onSelected?.Invoke(this.taskTab.taskCategory, true);
            }
        }

        public override void SetSelected(bool toSelected, bool force) {
            this.toggle.SetSelected(toSelected, true);
        }
    }

    public class UI_TaskListItem_Layout {
        #region UI Variable Statement

        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public Text Text_Menu_Dark_RectTransform { get; private set; }
        public Text Text_Menu_Light_RectTransform { get; private set; }
        public CP_Toggle ListItem_RectTransform { get; private set; }

        #endregion

        public void Parse(GameObject root, IListener listener = null) {
            this.mRoot = root;
            this.mTrans = root.transform;
            this.Text_Menu_Dark_RectTransform = this.mTrans.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            this.Text_Menu_Light_RectTransform = this.mTrans.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
            this.ListItem_RectTransform = this.mTrans.GetComponent<CP_Toggle>();

            if (listener != null) {
                this.RegisterEvents(listener);
            }
        }

        public void RegisterEvents(IListener listener) {
            this.ListItem_RectTransform.onValueChanged.AddListener(listener.OnValueChanged);
        }

        public interface IListener {
            void OnValueChanged(bool arg);
        }
    }

    public class UI_TaskListItem : UISelectableElement, UI_TaskListItem_Layout.IListener {
        public UI_TaskListItem_Layout Layout;
        public TaskEntry taskEntry;
        public System.Action<int, TaskEntry, CSVTask.Data, bool> clickAction;

        protected override void Loaded() {
            this.Layout = new UI_TaskListItem_Layout();
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        public void Refresh(TaskEntry taskEntry, bool needSelect, System.Action<int, TaskEntry, CSVTask.Data, bool> clickAction) {
            this.taskEntry = taskEntry;
            this.id = (int) taskEntry.id;
            this.clickAction = clickAction;

            this.Layout.ListItem_RectTransform.Highlight(false);

            TextHelper.SetTaskText(this.Layout.Text_Menu_Dark_RectTransform, taskEntry.csvTask.taskName);
            TextHelper.SetTaskText(this.Layout.Text_Menu_Light_RectTransform, taskEntry.csvTask.taskName);
#if DEBUG_MODE
            this.Layout.Text_Menu_Dark_RectTransform.text = this.Layout.Text_Menu_Dark_RectTransform.text + this.id.ToString();
            this.Layout.Text_Menu_Light_RectTransform.text = this.Layout.Text_Menu_Light_RectTransform.text + this.id.ToString();
#endif

            if (needSelect) {
                SetSelected(true, true);
            }
        }

        public override void OnDestroy() {
            this.Layout = null;
            base.OnDestroy();
        }

        public void OnValueChanged(bool arg) {
            if (arg) {
                this.clickAction?.Invoke(this.id, this.taskEntry, this.taskEntry.csvTask, true);
            }
        }

        public override void SetSelected(bool toSelected, bool force) {
            this.Layout.ListItem_RectTransform.SetSelected(toSelected, true);
        }
    }

    public class UI_TaskGoalItem : UIComponent {
        public GameObject finishGo;
        public GameObject unFinishGo;
        public Text text;

        protected override void Loaded() {
            this.finishGo = this.transform.Find("Finish").gameObject;
            this.unFinishGo = this.transform.Find("UnFinish").gameObject;
            this.text = this.transform.Find("Text").GetComponent<Text>();

            this.finishGo.SetActive(false);
            this.unFinishGo.SetActive(false);
        }

        public void Refresh(TaskEntry entry, TaskGoal goal) {
            bool taskFinish = entry.IsFinish();
            bool isShowFinish = false;

            if (taskFinish || entry.csvTask.conditionType) {
                // 任务已经完成 或者是 或 条件
                if (taskFinish) {
                    isShowFinish = true;

                    uint submitNpcId = entry.csvTask.submitNpc;
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
                        TextHelper.SetTaskText(this.text, 1601000001, mapName, npcName);
                    }
                    else {
                        TextHelper.SetTaskText(this.text, 1601000002);
                    }
                }
                else {
                    isShowFinish = false;

                    // ||条件的进度展示
                    TextHelper.SetTaskText(this.text, entry.csvTask.taskContent[0], entry.TotalProgress.ToString(), "100");
                }
            }
            else {
                if (goal.IsAlldependencyFinish(out int failIndex)) {
                    this.Show();
                }
                else {
                    this.Hide();
                }

                isShowFinish = goal.isFinish;
                this.text.text = goal.taskContent;
            }

            TextHelper.SetText(this.text, isShowFinish ? 2007211u : 2007212u, this.text.text);
            this.finishGo.SetActive(isShowFinish);
            this.unFinishGo.SetActive(!isShowFinish);
        }
    }

    public class UI_TaskList : UIBase, UI_TaskList_Layout.IListener {
        private readonly UI_TaskList_Layout Layout = new UI_TaskList_Layout();

        public UIElementContainer<UI_TaskTabItem, int> menuVds = new UIElementContainer<UI_TaskTabItem, int>(true, 1);
        public UIElementContainer<UI_TaskListItem, uint> taskVds = new UIElementContainer<UI_TaskListItem, uint>(true, 1);

        private readonly List<PropItem> rewardList = new List<PropItem>();
        private readonly List<TaskRewardItem> tinyRewards = new List<TaskRewardItem>();
        private readonly COWVd<UI_TaskGoalItem> taskContents = new COWVd<UI_TaskGoalItem>();

        public int currentMenuId = -1;
        public int currentTaskId = -1;
        public TaskEntry currentTaskEntry = null;

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);

            this.taskContents.Clear();

            // 最大4个
            this.Layout.RewardItemProto.gameObject.SetActive(false);
            for (int i = 0; i < 4; ++i) {
                GameObject clone = GameObject.Instantiate<GameObject>(this.Layout.RewardItemProto.gameObject);
                clone.transform.SetParent(this.Layout.RewardItemProto.transform.parent);
                clone.transform.localScale = Vector3.one;
                clone.transform.localPosition = Vector3.zero;

                TaskRewardItem item = new TaskRewardItem();
                item.Init(clone.transform);
                this.tinyRewards.Add(item);
            }
        }

        protected override void OnOpen(object arg) {
            Tuple<uint, uint> tp = arg as Tuple<uint, uint>;
            if (tp != null) {
                this.currentMenuId = (int) tp.Item1;
                var item2 = (int) tp.Item2;
                if (item2 != 0) {
                    this.currentTaskId = item2;
                }
                else {
                    this.currentTaskId = -1;
                }
            }
            else {
                this.currentMenuId = -1;
                this.currentTaskId = -1;
            }
        }

        protected override void OnShow() {
            this.RefreshMenus();
        }        

        protected override void OnDestroy() {
            for (int i = 0; i < this.menuVds.Count; ++i) {
                if (this.menuVds.TryGetVdByIndex(i, out var vd)) {
                    vd.OnDestroy();
                }
            }

            for (int i = 0; i < this.taskVds.Count; ++i) {
                if (this.taskVds.TryGetVdByIndex(i, out var vd)) {
                    vd.OnDestroy();
                }
            }

            foreach (var i in this.tinyRewards) {
                i.OnDestroy();
            }

            this.menuVds.Clear();
            this.taskVds.Clear();
            this.rewardList.Clear();
            this.tinyRewards.Clear();            
        }

        protected override void OnUpdate() {
            this.menuVds.Update();
            this.taskVds.Update();
        }

        private List<int> menuList = new List<int>();

        private void RefreshMenus() {
            this.menuList.Clear();
            foreach (var kvp in Sys_Task.Instance.receivedTasks) {
                TaskTab tab = Sys_Task.Instance.GetTab(kvp.Key);
                if (!tab.csv.TaskPanelShow) {
                    this.menuList.Add(kvp.Key);
                }
            }

            this.menuList = this.menuList.FindAll((e) => {
                // 线索任务有自己的条件开放限制
                TaskTab tab = Sys_Task.Instance.GetTab(e);
                uint funcOpenId = tab.funcOpenId;
                if (!tab.IsOpen()) {
                    return false;
                }

                return tab.taskCategory != (int) ETaskCategory.Dungeon && Sys_Task.Instance.receivedTasks[e].Count > 0;
            });

            this.GetCurrentMenuId(this.menuList);
            this.BuildOrRefreshMenu(this.menuList);
        }

        private void BuildOrRefreshMenu(List<int> menuList) {
            this.menuVds.BuildOrRefresh(this.Layout.tabItemProto.gameObject, this.Layout.tabItemProto.parent, menuList, (vd, id, indexOfVdList) => {
                vd.SetUniqueId(id);
                vd.SetSelectedAction((innerId, force) => { this.RefreshTabs(innerId, force); });
                vd.Refresh(id, id == currentMenuId);
            });
        }

        private int GetCurrentMenuId(List<int> menuList) {
            if (this.currentMenuId != -1) {
                // if (!this.menuVds.TryGetVdById(this.currentMenuId, out var vd)) {
                //     this.currentMenuId = -1;
                // }

                if (!menuList.Contains(currentMenuId)) {
                    this.currentMenuId = -1;
                }
            }

            if (this.currentMenuId == -1) {
                if (menuList.Count > 0) {
                    this.currentMenuId = menuList[0];
                }
            }

            return this.currentMenuId;
        }

        private void SetSelectedMenu(List<int> menuList) {
            if (this.menuVds.TryGetVdById(this.currentMenuId, out var vd)) {
                vd?.SetSelected(true, true);
            }
        }

        public void RefreshTabs(int menuId, bool force) {
            if (!force && menuId == this.currentMenuId) {
                return;
            }

            this.currentMenuId = menuId;
            if (Sys_Task.Instance.receivedTasks.TryGetValue(menuId, out var outer)) {
                var tabList = new List<uint>(outer.Keys);
                Sys_Task.Instance.SortByPrority(tabList);
                
                if (this.currentTaskId != -1) {
                    // 当前menu中是否含有特定任务id,否则置顶
                    int index = tabList.IndexOf((uint)this.currentTaskId);
                    if (index != -1) {
                        tabList.RemoveAt(index);
                        tabList.Insert(0, (uint)this.currentTaskId);
                    }
                }

                this.GetCurrentTabId(tabList);
                this.BuildOrRefreshTab(tabList);
            }
        }

        private void BuildOrRefreshTab(List<uint> tabList) {
            this.taskVds.BuildOrRefresh(this.Layout.ListItem.gameObject, this.Layout.ListItem.parent, tabList, OnRefreshTaskTab);
        }

        private void OnRefreshTaskTab(UI_TaskListItem vd, uint taskId, int indexOfVdList) {
            vd.Refresh(Sys_Task.Instance.receivedTasks[this.currentMenuId][taskId], 
                currentTaskId == taskId, (clickId, taskEntry, _, force) => {
                    this.RefreshContent(clickId, taskEntry, force);
                });
        }

        private int GetCurrentTabId(List<uint> tabList) {
            if (this.currentTaskId != -1) {
                // if (!this.taskVds.TryGetVdById(this.currentTaskId, out var vd)) {
                //     this.currentTaskId = -1;
                // }
                if (!tabList.Contains((uint) currentTaskId)) {
                    this.currentTaskId = -1;
                }
            }

            if (this.currentTaskId == -1) {
                if (tabList.Count > 0) {
                    this.currentTaskId = (int) tabList[0];
                }
            }

            return this.currentTaskId;
        }

        private void SetSelectedTab(List<uint> tabList) {
            if (this.taskVds.TryGetVdById(this.currentTaskId, out var vd)) {
                vd?.SetSelected(true, true);
            }
        }

        public void RefreshContent(int taskId, TaskEntry taskEntry, bool force) {
            if (!force && taskId == this.currentTaskId) {
                return;
            }

            this.currentTaskEntry = taskEntry;
            this.currentTaskId = (int) taskEntry.id;

            int realGoalCount = 1;
            bool isFinish = taskEntry.IsFinish();
            if (!taskEntry.csvTask.conditionType && !isFinish) {
                realGoalCount = taskEntry.csvTask.taskGoals.Count;
            }

            this.taskContents.TryBuildOrRefresh(this.Layout.TaskGoalContent, this.Layout.TaskGoalContent.transform.parent, realGoalCount, (vd, index) => { vd.Refresh(taskEntry, taskEntry.taskGoals[index]); });

            TextHelper.SetTaskText(this.Layout.TaskTitle, taskEntry.csvTask.taskName);
            this.Layout.TaskState.gameObject.SetActive(false);

            if (taskEntry.csvTask.taskStep != 0) {
                string catName = LanguageHelper.GetTextContent(taskEntry.csvTaskCategory.name);
                string preText = LanguageHelper.GetTextContent(1600000010u,  taskEntry.csvTask.taskChapter.ToString(), taskEntry.csvTask.taskStep.ToString(), catName);
                string nextText = LanguageHelper.GetTaskTextContent(taskEntry.csvTask.taskDescribe);
                TextHelper.SetText(this.Layout.TaskDesc, 1600000011, preText, nextText);
            }
            else {
                TextHelper.SetTaskText(this.Layout.TaskDesc, taskEntry.csvTask.taskDescribe);
            }

            if (!taskEntry.csvTask.CanManualTrace) {
                this.Layout.toggleTitle.gameObject.SetActive(false);
            }
            else {
                this.Layout.toggleTitle.gameObject.SetActive(true);
                this.Layout.toggleTitle.isOn = taskEntry.isTraced;
            }

            this.Layout.btnShare.gameObject.SetActive(this.currentTaskEntry.csvTask.taskShate);
            this.Layout.btnGiveUp.gameObject.SetActive(this.currentTaskEntry.csvTask.whetherAbandon);

            var taskTab = this.currentTaskEntry?.taskTab;
            if (taskTab != null) {
                this.Layout.btnGoto.gameObject.SetActive(taskTab.csv.TaskButtonType == 0);
                this.Layout.btnWatch.gameObject.SetActive(taskTab.csv.TaskButtonType == 1);
            }
            else {
                this.Layout.btnGoto.gameObject.SetActive(false);
                this.Layout.btnWatch.gameObject.SetActive(false);
            }


            // 刷新奖励展示
            if (taskEntry.rewards.Count > 0) {
                this.Layout.NoReward.gameObject.SetActive(false);
                this.Layout.AwardItemParent.gameObject.SetActive(true);
                this.BuildRefreshRewards(taskEntry);
            }
            else {
                this.Layout.NoReward.gameObject.SetActive(false);
                this.Layout.AwardItemParent.gameObject.SetActive(false);
            }

            // 刷新金币等展示
            this.BuildRefreshTinyRewards(taskEntry);
        }

        private void BuildRefreshTinyRewards(TaskEntry taskEntry) {
            int i = 0;
            if (taskEntry.csvTask.RewardExp != null && taskEntry.csvTask.RewardExp[0] != 0) {
                this.tinyRewards[i].Show();
                this.tinyRewards[i++].Refresh(taskEntry.csvTask.RewardExp[0], taskEntry.csvTask.RewardExp[1]);
            }

            if (taskEntry.csvTask.RewardGold != null && taskEntry.csvTask.RewardGold[0] != 0) {
                this.tinyRewards[i].Show();
                this.tinyRewards[i++].Refresh(taskEntry.csvTask.RewardGold[0], taskEntry.csvTask.RewardGold[1]);
            }

            for (; i < this.tinyRewards.Count; ++i) {
                this.tinyRewards[i].Hide();
            }
        }

        private void BuildRefreshRewards(TaskEntry taskEntry) {
            int vdLength = this.rewardList.Count;
            int dataLength = taskEntry.rewards.Count;
            for (int i = vdLength; i < dataLength; ++i) {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData((uint) taskEntry.rewards[i].x, taskEntry.rewards[i].y, true, false, false, false, false, true);
                this.rewardList.Add(PropIconLoader.GetAsset(itemData, this.Layout.AwardItemParent));
            }

            vdLength = this.rewardList.Count;
            for (int i = 0; i < vdLength; ++i) {
                if (i < dataLength) {
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData((uint) taskEntry.rewards[i].x, taskEntry.rewards[i].y, true, false, false, false, false, true);
                    this.rewardList[i].SetActive(true);
                    this.rewardList[i].SetData(itemData, EUIID.UI_TaskList);
                }
                else {
                    this.rewardList[i].SetActive(false);
                }
            }
        }

        public void OnButton_Goto_RectTransform() {
            if (this.currentTaskEntry != null) {
                string reason = null;
                if (this.currentTaskEntry.csvTask.taskCategory == (int) ETaskCategory.Trunk && !this.currentTaskEntry.CanDoOnlyCSVCondition(ref reason)) {
                    UIDailyActivitesParmas uiParams = new UIDailyActivitesParmas();
                    uiParams.SkipToID = 0;
                    UIManager.OpenUI(EUIID.UI_DailyActivites, false, uiParams);
                }
                else {
                    Sys_Task.Instance.TryDoTask(this.currentTaskEntry, true, false, true);
                }
            }
        }

        public void OnButtonWatch_RectTransform() {
            if (this.currentTaskEntry != null) {
                var args = new Sys_Map.TargetNpcParameter(this.currentTaskEntry.csvTask.taskMap, this.currentTaskEntry.csvTask.receiveNpc);
                UIManager.OpenUI((int) EUIID.UI_Map, false, args);
                this.CloseSelf();
            }
        }

        public void OnButton_Share_RectTransform() {
            Sys_Task.Instance.ReqShare((uint) this.currentTaskId);
        }

        public void OnButton_Give_Up_RectTransform() {
            // 放弃任务
            if (this.currentTaskId != -1) {
                Sys_Task.Instance.ReqForgo((uint) this.currentTaskId);
            }
        }

        public void OnButton_Close_RectTransform() {
            UIManager.CloseUI(EUIID.UI_TaskList);
        }

        public void OnButton_Trace_RectTransform() {
            if (this.currentTaskEntry != null && this.currentTaskEntry.csvTask.CanManualTrace) {
                Sys_Task.Instance.ReqTrace(this.currentTaskEntry.id, !this.currentTaskEntry.isTraced, true);
            }
            else {
                // Cuibinbin
                string desc = "该任务不允许修改追踪状态";
                DebugUtil.LogError(desc);
                Sys_Hint.Instance.PushContent_Normal(desc);
            }
        }

        #region 事件通知

        protected override void ProcessEventsForEnable(bool toRegister) {
            // 增加删除任务
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnReceived, this.OnReceived, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnSubmited, this.OnSubmited, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnForgoed, this.OnForgoed, toRegister);

            // 刷新数据
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnRefreshed, this.OnRefreshed, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnFinished, this.OnFinished, toRegister);
            // 追踪变化
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnTraced, this.OnTraced, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int>(Sys_Task.EEvents.OnTabAdded, this.OnMenuAdded, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int>(Sys_Task.EEvents.OnTabRemoved, this.OnMenuRemoved, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<TaskEntry, int, int>(Sys_Task.EEvents.OnTargetIndexChanged, this.OnTargetIndexChanged, toRegister);
        }

        private void OnForgoed(int menuId, uint taskId, TaskEntry taskEntry) {
            if (menuId == this.currentMenuId && taskId == this.currentTaskId) {
                this.currentTaskId = -1;
            }

            this.RefreshTabs(menuId, true);
        }

        private void OnRefreshed(int menuId, uint taskId, TaskEntry taskEntry) {
            if (menuId == this.currentMenuId && taskId == this.currentTaskId) {
                this.RefreshContent((int) taskId, taskEntry, true);
            }
        }

        private void OnFinished(int menuId, uint taskId, TaskEntry taskEntry) {
            if (menuId == this.currentMenuId && taskId == this.currentTaskId) {
                this.RefreshContent((int) taskId, taskEntry, true);
            }
        }

        private void OnTraced(int menuId, uint taskId, TaskEntry taskEntry) {
            if (this.currentTaskEntry != null && taskId == this.currentTaskEntry.id) {
                this.Layout.toggleTitle.isOn = this.currentTaskEntry.isTraced;
            }
        }

        // tab新增或者删除某些tab，比如时间性质的tab
        private void OnMenuAdded(int menuId) {
            this.RefreshMenus();
        }

        private void OnMenuRemoved(int menuId) {
            if (menuId == this.currentMenuId) {
                this.currentMenuId = -1;
                this.currentTaskId = -1;
            }

            this.RefreshMenus();
        }

        private void OnSubmited(int menuId, uint taskId, TaskEntry taskEntry) {
            if (menuId == this.currentMenuId) {
                if (taskId == this.currentTaskId) {
                    this.currentTaskId = -1;
                }

                this.RefreshTabs(menuId, true);
            }
        }

        // tab的新增/删除总是会在task的add/remove之前触发
        // 新增一个任务【tab新增的可能性不用考虑因为tabadd会被先执行】
        private void OnReceived(int menuId, uint taskId, TaskEntry taskEntry) {
            if (menuId == this.currentMenuId) {
                if (taskEntry.csvTask.taskCategory == (int) ETaskCategory.Trunk) {
                    this.currentMenuId = menuId;
                    this.currentTaskId = (int) taskId;
                }

                this.RefreshTabs(menuId, true);
            }
        }

        private void OnTargetIndexChanged(TaskEntry taskEntry, int oldIndex, int newIndex) {
            this.RefreshContent((int) taskEntry.id, taskEntry, true);
        }

        #endregion
    }
}