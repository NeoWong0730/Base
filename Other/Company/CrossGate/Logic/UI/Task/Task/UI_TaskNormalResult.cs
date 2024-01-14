using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_TaskNormalResult : UIBase, UI_Task_Normal_Result_Layout.IListener {
        public class TaskRewardItem : UISelectableElement {
            public Image image;
            public Text textName;
            public Text textCount;

            protected override void Loaded() {
                this.image = this.transform.Find("Image").GetComponent<Image>();
                this.textName = this.transform.Find("Text_Name").GetComponent<Text>();
                this.textCount = this.transform.Find("Text_Number").GetComponent<Text>();
            }

            public void Refresh(int index, uint id, uint count) {
                this.id = index;

                CSVItem.Data csv = CSVItem.Instance.GetConfData(id);
                if (csv != null) {
                    ImageHelper.SetIcon(this.image, csv.small_icon_id);
                    this.textCount.text = count.ToString();
                    TextHelper.SetText(this.textName, csv.name_id);
                }
            }
        }

        private readonly UI_Task_Normal_Result_Layout Layout = new UI_Task_Normal_Result_Layout();
        public TaskEntry taskEntry;

        private readonly List<PropItem> rewardList = new List<PropItem>();
        private readonly UIElementContainer<TaskRewardItem, Vector2Int> tinyRewards = new UIElementContainer<TaskRewardItem, Vector2Int>();

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            this.taskEntry = arg as TaskEntry;

            // 进入交互状态
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterInteractive);
            ActionCtrl.ActionExecuteLockFlag = true;
        }

        protected override void OnClose() {
            // 退出交互状态
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
            ActionCtrl.ActionExecuteLockFlag = false;
        }

        protected override void OnShow() {
            if (this.taskEntry == null) {
                return;
            }

            var dataList = this.taskEntry.tinyRewards;
            this.tinyRewards.BuildOrRefresh(this.Layout.TinyRewardItemProto.gameObject, this.Layout.TinyRewardItemProto.parent, dataList, (vd, data, index) => { vd.Refresh(index, (uint)data.x, (uint)data.y); });

            this.Layout.RewardItemProto.gameObject.SetActive(false);
            int vdLength = this.rewardList.Count;
            int dataLength = this.taskEntry.rewards.Count;
            for (int i = vdLength; i < dataLength; ++i) {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData((uint)this.taskEntry.rewards[i].x, this.taskEntry.rewards[i].y, true, false, false, false, false, true);
                this.rewardList.Add(PropIconLoader.GetAsset(itemData, this.Layout.RewardItemProto.parent));
            }

            vdLength = this.rewardList.Count;
            for (int i = 0; i < vdLength; ++i) {
                if (i < dataLength) {
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData((uint)this.taskEntry.rewards[i].x, this.taskEntry.rewards[i].y, true, false, false, false, false, true);
                    this.rewardList[i].SetActive(true);
                    this.rewardList[i].SetData(itemData, EUIID.UI_TaskNormalResult);
                }
                else {
                    this.rewardList[i].SetActive(false);
                }
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
            UIManager.CloseUI(Logic.EUIID.UI_TaskNormalResult);
        }
    }
}
