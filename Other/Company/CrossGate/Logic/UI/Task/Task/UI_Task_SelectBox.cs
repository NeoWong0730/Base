using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_TaskSelectBox : UIBase, UI_Task_SelectBox_Layout.IListener {
        public class UI_SelectableReward : UISelectableElement {
            private CP_Toggle toggle;
            private UI_TaskSelectBox ui;

            private uint dropId;

            private Text number;
            private Text name;
            private Image icon;

            protected override void Loaded() {
                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.number = this.transform.Find("Text_Number").GetComponent<Text>();
                this.name = this.transform.Find("Text_Name").GetComponent<Text>();
                this.icon = this.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();

                this.toggle.onValueChanged.AddListener(this.OnValueChanged);
            }

            public void Refresh(int index, uint id, int count, uint dropId, UI_TaskSelectBox ui) {
                this.ui = ui;
                this.id = index;

                this.dropId = dropId;

                this.toggle.Highlight(false);
                var csv = CSVItem.Instance.GetConfData(id);
                if (csv != null) {
                    ImageHelper.SetIcon(this.icon, csv.icon_id);
                    this.number.text = count.ToString();
                    TextHelper.SetText(this.name, csv.name_id);
                }
            }

            private void OnValueChanged(bool arg) {
                if (arg) {
                    this.ui.selectedDropId = this.dropId;
                }
            }

            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }
        }

        private readonly UI_Task_SelectBox_Layout Layout = new UI_Task_SelectBox_Layout();
        private TaskEntry taskEntry;
        private uint selectedDropId;

        private readonly UIElementContainer<UI_SelectableReward, Vector3Int> rewards = new UIElementContainer<UI_SelectableReward, Vector3Int>();

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            if (arg == null) {
                return;
            }

            this.taskEntry = Sys_Task.Instance.GetReceivedTask((uint)arg);
        }

        protected override void OnShow() {
            if (this.taskEntry == null) {
                return;
            }

            this.Layout.SureNode.gameObject.SetActive(false);
            var rewardList = this.taskEntry.rewards;
            this.rewards.BuildOrRefresh(this.Layout.proto.gameObject, this.Layout.proto.parent, rewardList, (vd, data, index) => { vd.Refresh(index, (uint)data.x, data.y, (uint)data.z, this); });
        }

        protected override void OnClose() {
            this.Layout.Registry.SwitchTo(-1, false);
            this.selectedDropId = 0;
        }

        protected override void OnDestroy() {
            this.rewards.Clear();
        }

        protected override void OnUpdate() {
            this.rewards.Update();
        }

        private void OnButton_Close() {
            UIManager.CloseUI(Logic.EUIID.UI_TaskSelectBox);
        }

        public void OnButton_Cancel_RectTransform() {
            UIManager.CloseUI(Logic.EUIID.UI_TaskSelectBox);
        }

        public void OnButton_Confirm_RectTransform() {
            if (this.selectedDropId == 0) {
                this.Layout.SureNode.gameObject.SetActive(false);
                Sys_Hint.Instance.PushContent_Normal("请选择一个道具");
            }
            else {
                this.Layout.SureNode.gameObject.SetActive(true);
            }
        }

        public void OnButton_Cancel_RectTransform3() {
            this.Layout.SureNode.gameObject.SetActive(false);
        }

        public void OnButton_Confirm_RectTransform4() {
            if (this.selectedDropId != 0) {
                Sys_Task.Instance.ReqSubmit(this.taskEntry.id, this.selectedDropId);
                UIManager.CloseUI(Logic.EUIID.UI_TaskSelectBox);
            }
            else {
                Sys_Hint.Instance.PushContent_Normal("请选择一个道具");
            }
        }
    }
}