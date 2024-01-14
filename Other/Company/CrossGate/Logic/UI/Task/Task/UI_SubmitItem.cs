using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_SubmitItem : UIBase {
        public Button btnSubmit;
        public Button btnClose;
        public Transform propParent;
        public UI_RewardList rewardList;

        public Sys_SubmitItem.SubmitData submitData;
        public List<ItemIdCount> rewards = new List<ItemIdCount>();

        protected override void OnLoaded() {
            this.btnSubmit = this.transform.Find("Aniamtor/UI/Button").GetComponent<Button>();
            this.btnClose = this.transform.Find("Aniamtor/UI/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            this.propParent = this.transform.Find("Aniamtor/UI/Grid");

            this.btnSubmit.onClick.AddListener(this.OnBtnSubmitClicked);
            this.btnClose.onClick.AddListener(this.OnBtnCloseClicked);
        }

        protected override void OnOpen(object arg) {
            if (arg != null) {
                this.submitData = arg as Sys_SubmitItem.SubmitData;
            }
        }

        protected override void OnOpened() {
            this.rewardList = this.rewardList ?? new UI_RewardList(this.propParent, EUIID.UI_SubmitItem);
            CSVSubmit.Data csv = CSVSubmit.Instance.GetConfData(this.submitData.CsvSubmitID);
            if (csv != null) {
                this.rewards.Clear();
                if (csv.itemId1 != 0) {
                    this.rewards.Add(new ItemIdCount(csv.itemId1, csv.itemCount1));
                }

                if (csv.itemId2 != 0) {
                    this.rewards.Add(new ItemIdCount(csv.itemId2, csv.itemCount2));
                }

                if (csv.itemId3 != 0) {
                    this.rewards.Add(new ItemIdCount(csv.itemId3, csv.itemCount3));
                }

                if (csv.itemId4 != 0) {
                    this.rewards.Add(new ItemIdCount(csv.itemId4, csv.itemCount4));
                }

                if (csv.itemId5 != 0) {
                    this.rewards.Add(new ItemIdCount(csv.itemId5, csv.itemCount5));
                }

                this.rewardList.SetRewardList(this.rewards);
                this.rewardList?.Build(true, false, false, false, false, true, true, true, null, false, false, true);
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister) {
            Sys_SubmitItem.Instance.eventEmitter.Handle<uint, uint>(Sys_SubmitItem.EEvents.OnSubmited, this.OnSubmited, toRegister);
        }

        protected override void ProcessEvents(bool toRegister) {
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, this.OnItemCountChanged, toRegister);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnHeroTel, this.OnHeroTel, toRegister);
        }

        private void OnItemCountChanged(int changeType, int curBoxId) {
            this.rewardList?.SetRewardList(this.rewards);
            this.rewardList?.Build(true, false, false, false, false, true, true, true, null, false, false, true);
        }

        private void OnHeroTel() {
            this.CloseSelf();
        }

        private void OnSubmited(uint taskId, uint taskIndex) {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1040000002));
            this.OnBtnCloseClicked();
        }

        private void OnBtnCloseClicked() {
            UIManager.CloseUI(EUIID.UI_SubmitItem);
            Sys_Task.Instance.InterruptCurrentTaskDoing();
        }

        private void OnBtnSubmitClicked() {
            if (this.rewardList.isEnough) {
                if (this.submitData.FunctionSourceType == EFunctionSourceType.Task) {
                    TaskEntry taskEntry = Sys_Task.Instance.GetTask(this.submitData.TaskId);
                    if (taskEntry != null) {
                        Sys_SubmitItem.Instance.ReqSubmit(this.submitData.npcUID, taskEntry.id, (uint) taskEntry.currentTaskGoalIndex);
                    }
                    else {
                        DebugUtil.LogFormat(ELogType.eTask, "dont exits task {0}", this.submitData.TaskId.ToString());
                    }
                }
                else if (this.submitData.FunctionSourceType == EFunctionSourceType.None) {
                    // 无任务id和任务index
                    Sys_SubmitItem.Instance.ReqSubmit(this.submitData.npcUID, this.submitData.TaskId, 0, this.submitData.FunctionHandlerID, this.submitData.arg);
                }
            }
            else {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1040000001));
            }
        }
    }
}