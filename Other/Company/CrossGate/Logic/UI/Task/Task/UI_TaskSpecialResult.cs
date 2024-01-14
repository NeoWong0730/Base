using System.Collections.Generic;
using Logic.Core;
using UnityEngine;

namespace Logic {
    public class UI_TaskSpecialResult : UIBase, UI_Task_Special_Result_Layout.IListener {
        private readonly UI_Task_Special_Result_Layout Layout = new UI_Task_Special_Result_Layout();
        public TaskEntry taskEntry;

        private readonly List<PropItem> rewardList = new List<PropItem>();
        private readonly UIElementContainer<UI_TaskNormalResult.TaskRewardItem, Vector2Int> tinyRewards = new UIElementContainer<UI_TaskNormalResult.TaskRewardItem, Vector2Int>();

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            this.taskEntry = arg as TaskEntry;

            if (Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId) || !Sys_Team.Instance.HaveTeam) {
                // 进入交互状态
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterInteractive);
                ActionCtrl.ActionExecuteLockFlag = true;
            }
        }
        protected override void OnClose() {
            if (Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId) || !Sys_Team.Instance.HaveTeam) {// 进入交互状态
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                ActionCtrl.ActionExecuteLockFlag = false;
            }
        }

        protected override void OnShow() {
            if (this.taskEntry == null) {
                return;
            }

            var dataList = this.taskEntry.tinyRewards;
            this.tinyRewards.BuildOrRefresh(this.Layout.AwardItemProto.gameObject, this.Layout.AwardItemProto.parent, dataList, (vd, data, index) => { vd.Refresh(index, (uint)data.x, (uint)data.y); });

            int vdLength = this.rewardList.Count;
            int dataLength = this.taskEntry.rewards.Count;
            for (int i = vdLength; i < dataLength; ++i) {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData((uint)this.taskEntry.rewards[i].x, this.taskEntry.rewards[i].y, true, false, false, false, false, true);
                this.rewardList.Add(PropIconLoader.GetAsset(itemData, this.Layout.RewardItemProtoParent));
            }

            vdLength = this.rewardList.Count;
            Layout.rewardTitle.SetActive(vdLength > 0);
            for (int i = 0; i < vdLength; ++i) {
                if (i < dataLength) {
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData((uint)this.taskEntry.rewards[i].x, this.taskEntry.rewards[i].y, true, false, false, false, false, true);
                    this.rewardList[i].SetActive(true);
                    this.rewardList[i].SetData(itemData, EUIID.UI_TaskSpecialResult);
                }
                else {
                    this.rewardList[i].SetActive(false);
                }
            }

            TextHelper.SetTaskText(this.Layout.TaskName, this.taskEntry.csvTask.taskName);
            TextHelper.SetTaskText(this.Layout.TaskContent, this.taskEntry.csvTask.taskDescribe);

            this.Layout.loveGo.SetActive(false);
            this.Layout.challengeGo.SetActive(false);
            this.Layout.loveIcon.SetActive(false);
            this.Layout.challengeIcon.SetActive(false);
            if (this.taskEntry.csvTask.taskCategory == (int)ETaskCategory.Challenge) {
                this.Layout.challengeIcon.SetActive(true);
                this.Layout.challengeGo.SetActive(true);
            }
            else if (this.taskEntry.csvTask.taskCategory == (int)ETaskCategory.Love) {
                this.Layout.loveIcon.SetActive(true);
                this.Layout.loveGo.SetActive(true);
            }
        }

        protected override void OnDestroy() {
            this.rewardList.Clear();
            this.tinyRewards.Clear();
        }

        protected override void OnUpdate() {
            this.tinyRewards.Update();
        }

        public void OnCloseClicked() {
            UIManager.CloseUI(Logic.EUIID.UI_TaskSpecialResult);
        }
    }
}