using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_TaskSharedListItem : UISelectableElement, UI_TaskListItem_Layout.IListener {
        public UI_TaskListItem_Layout Layout;
        public SharedTaskBlock sharedTask;
        public System.Action<int, SharedTaskBlock, CSVTask.Data, bool> clickAction;

        protected override void Loaded() {
            this.Layout = new UI_TaskListItem_Layout();
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        public void Refresh(SharedTaskBlock sharedTask, System.Action<int, SharedTaskBlock, CSVTask.Data, bool> clickAction) {
            this.sharedTask = sharedTask;
            this.id = (int)sharedTask.taskEntry.id;
            this.clickAction = clickAction;

            this.Layout.ListItem_RectTransform.Highlight(false);

            TextHelper.SetTaskText(this.Layout.Text_Menu_Dark_RectTransform, sharedTask.taskEntry.csvTask.taskName);
            TextHelper.SetTaskText(this.Layout.Text_Menu_Light_RectTransform, sharedTask.taskEntry.csvTask.taskName);
#if DEBUG_MODE
            this.Layout.Text_Menu_Dark_RectTransform.text = this.Layout.Text_Menu_Dark_RectTransform.text + this.id.ToString();
            this.Layout.Text_Menu_Light_RectTransform.text = this.Layout.Text_Menu_Light_RectTransform.text + this.id.ToString();
#endif
        }

        public override void OnDestroy() {
            this.Layout = null;
            base.OnDestroy();
        }

        public void OnValueChanged(bool arg) {
            if (arg) {
                this.clickAction?.Invoke(this.id, this.sharedTask, this.sharedTask.taskEntry.csvTask, true);
            }
        }
        public override void SetSelected(bool toSelected, bool force) {
            this.Layout.ListItem_RectTransform.SetSelected(toSelected, true);
        }
    }

    public class UI_TaskShareList : UIBase, UI_TaskShareList_Layout.IListener {
        private readonly UI_TaskShareList_Layout Layout = new UI_TaskShareList_Layout();

        private readonly List<PropItem> rewardList = new List<PropItem>();
        private readonly List<TaskRewardItem> tinyRewards = new List<TaskRewardItem>();
        private readonly List<Text> taskContents = new List<Text>();

        private readonly UIElementContainer<UI_TaskSharedListItem, SharedTaskBlock> menuVds = new UIElementContainer<UI_TaskSharedListItem, SharedTaskBlock>(true);

        public int taskId = -1;
        public SharedTaskBlock currentSharedTask;

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);

            this.taskContents.Add(this.Layout.TaskContent_RectTransform1);
            this.taskContents.Add(this.Layout.TaskContent_RectTransform2);
            this.taskContents.Add(this.Layout.TaskContent_RectTransform3);

            // 最大4个
            this.Layout.awardProto.gameObject.SetActive(false);
            for (int i = 0; i < 4; ++i) {
                GameObject clone = GameObject.Instantiate<GameObject>(this.Layout.awardProto.gameObject);
                clone.transform.SetParent(this.Layout.awardProto.transform.parent);
                clone.transform.localScale = Vector3.one;
                clone.transform.localPosition = Vector3.zero;

                TaskRewardItem item = new TaskRewardItem();
                item.Init(clone.transform);
                this.tinyRewards.Add(item);
            }
        }

        protected override void OnShow() {
            this.taskId = -1;

            this.RefreshMenus();
        }
        protected override void OnDestroy() {
            this.rewardList.Clear();

            this.menuVds.Clear();
            foreach (var i in this.tinyRewards) { i.OnDestroy(); }
            this.tinyRewards.Clear();
        }
        protected override void OnUpdate() {
            this.menuVds.Update();

            // 超时自动接受该共享任务
            if (Sys_Task.Instance.sharedTasks.Count > 0) {
                foreach (var task in Sys_Task.Instance.sharedTasks) {
                    if (UnityEngine.Time.time > task.receivedTime + 30f) {
                        Sys_Task.Instance.ReqOpSharedTask(task.taskEntry.id, 0, task.ownerName);
                        // 迭代器中删除元素是危险的，这里break直接跳出
                        break;
                    }
                }
            }
        }

        private void RefreshMenus() {
            var menuList = Sys_Task.Instance.sharedTasks;
            this.BuildOrRefreshMenu(menuList);
            // keep after dataRefresh
            this.GetCurrentMenuId(menuList);
            this.SetSelectedMenu(menuList);
        }
        private void BuildOrRefreshMenu(List<SharedTaskBlock> menuList) {
            this.menuVds.BuildOrRefresh(this.Layout.tabProto.gameObject, this.Layout.tabProto.parent, menuList, (vd, data, indexOfVdList) => {
                vd.Refresh(Sys_Task.Instance.sharedTasks[indexOfVdList], (clickId, sharedTask, _, force) => {
                    this.RefreshContent(clickId, sharedTask, force);
                });
            });
        }
        private int GetCurrentMenuId(List<SharedTaskBlock> menuList) {
            if (this.taskId != -1) {
                if (!this.menuVds.TryGetVdById(this.taskId, out var vd)) {
                    this.taskId = -1;
                }
            }
            if (this.taskId == -1) {
                if (menuList.Count > 0) {
                    this.taskId = (int)menuList[0].taskEntry.id;
                }
            }
            return this.taskId;
        }
        private void SetSelectedMenu(List<SharedTaskBlock> menuList) {
            if (this.menuVds.TryGetVdById(this.taskId, out var vd)) {
                vd?.SetSelected(true, true);
            }
        }
        public void RefreshContent(int taskId, SharedTaskBlock sharedTask, bool force) {
            if (!force && taskId == this.taskId) {
                return;
            }

            this.currentSharedTask = sharedTask;
            this.taskId = (int)sharedTask.taskEntry.id;

            int realGoalCount = sharedTask.taskEntry.csvTask.taskGoals.Count;
            for (int i = 0; i < this.taskContents.Count; ++i) {
                if (i < realGoalCount) {
                    this.taskContents[i].gameObject.SetActive(true);
                    this.RefreshTaskGoal(sharedTask.taskEntry, i, this.taskContents[i], !sharedTask.taskEntry.csvTask.conditionType, false);
                }
                else { this.taskContents[i].gameObject.SetActive(false); }
            }

            TextHelper.SetTaskText(this.Layout.taskTitle, sharedTask.taskEntry.csvTask.taskName);
            TextHelper.SetTaskText(this.Layout.taskDesc, sharedTask.taskEntry.csvTask.taskDescribe);

            this.Layout.shareOwnDesc.text = sharedTask.ownerName;

            // 刷新奖励展示
            if (sharedTask.taskEntry.rewards.Count > 0) {
                this.Layout.NoReward.gameObject.SetActive(false);
                this.Layout.HaveReward.gameObject.SetActive(true);
                this.BuildRefreshRewards(sharedTask.taskEntry);
            }
            else {
                this.Layout.NoReward.gameObject.SetActive(true);
                this.Layout.HaveReward.gameObject.SetActive(false);
            }

            // 刷新金币等展示
            this.BuildRefreshTinyRewards(sharedTask.taskEntry);
        }
        private void RefreshTaskGoal(TaskEntry taskEntry, int index, Text text, bool andCondition, bool isFinish) {
            TextHelper.SetTaskText(text, taskEntry.csvTask.taskContent[0], taskEntry.TotalProgress.ToString(), "100");
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

            for (; i < this.tinyRewards.Count; ++i) { this.tinyRewards[i].Hide(); }
        }
        private void BuildRefreshRewards(TaskEntry taskEntry) {
            int vdLength = this.rewardList.Count;
            int dataLength = taskEntry.rewards.Count;
            for (int i = vdLength; i < dataLength; ++i) {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData((uint)taskEntry.rewards[i].x, taskEntry.rewards[i].y, true, false, false, false, false);
                this.rewardList.Add(PropIconLoader.GetAsset(itemData, this.Layout.HaveRewardParent));
            }

            vdLength = this.rewardList.Count;
            for (int i = 0; i < vdLength; ++i) {
                if (i < dataLength) {
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData((uint)taskEntry.rewards[i].x, taskEntry.rewards[i].y, true, false, false, false, false);
                    this.rewardList[i].SetActive(true);
                    this.rewardList[i].SetData(itemData, EUIID.UI_SharedTaskList);
                }
                else {
                    this.rewardList[i].SetActive(false);
                }
            }
        }

        public void OnBtnCloseClicked() {
            UIManager.CloseUI(Logic.EUIID.UI_SharedTaskList);
            Sys_Task.Instance.ReqOpAllSharedTask(1);
        }

        public void OnBtnRejectClicked() {
            Sys_Task.Instance.ReqOpSharedTask((uint)this.taskId, 1, this.currentSharedTask.ownerName);
        }

        public void OnBtnAccepetClicked() {
            Sys_Task.Instance.ReqOpSharedTask((uint)this.taskId, 0, this.currentSharedTask.ownerName);
        }

        protected override void ProcessEventsForEnable(bool toRegister) {
            Sys_Task.Instance.eventEmitter.Handle<uint>(Sys_Task.EEvents.OnShared, this.OnShared, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<uint, uint>(Sys_Task.EEvents.OnOpSharedTask, this.OnOpSharedTask, toRegister);
        }

        private void OnShared(uint taskId) {
            this.RefreshMenus();
        }
        private void OnOpSharedTask(uint tId, uint op) {
            if (Sys_Task.Instance.sharedTasks.Count <= 0) {
                UIManager.CloseUI(Logic.EUIID.UI_SharedTaskList);
            }
            else {
                if ((int)tId == this.taskId) {
                    this.taskId = -1;
                    this.currentSharedTask = null;
                }

                this.RefreshMenus();
            }
        }
    }
}