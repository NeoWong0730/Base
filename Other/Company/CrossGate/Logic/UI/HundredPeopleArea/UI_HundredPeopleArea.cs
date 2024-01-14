using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine.UI;

namespace Logic {
    public partial class UI_HundredPeopleArea : UIBase, UI_HundredPeopleArea_Layout.IListener {
        #region 数据定义

        private UI_HundredPeopleArea_Layout m_Layout = new UI_HundredPeopleArea_Layout();
        private UI_HundredPeopleArea_Data m_Data = new UI_HundredPeopleArea_Data();

        #endregion

        #region 系统函数

        protected override void OnLoaded() {
            this.OnParseComponent();
        }

        protected override void ProcessEvents(bool toRegister) {
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, this.OnUpdateLevel, toRegister);
            Sys_HundredPeopleArea.Instance.eventEmitter.Handle(Sys_HundredPeopleArea.EEvents.OnGotDailyAward, this.OnGotDailyAward, toRegister);
        }

        protected override void OnOpen(object arg) {
            this.m_Data.Reset();
            this.m_Data.LoadData();
            this.m_Data.UpdateStageIDList();
            this.m_Data.UpdateSubList();
        }

        protected override void OnShow() {
            this.RefreshView();
            this.TryPopReward();
        }

        private void TryPopReward() {
            bool hasGot = Sys_HundredPeopleArea.Instance.HasGotAward();
            if (!hasGot) {
                uint instanceId = 0;
                if (null != Sys_HundredPeopleArea.Instance.towerInsData) {
                    instanceId = Sys_HundredPeopleArea.Instance.towerInsData.RewardStageId;
                }

                CSVInstanceDaily.Data csv = CSVInstanceDaily.Instance.GetConfData(instanceId);
                if (csv != null) {
                    UIManager.OpenUI(EUIID.UI_HundreadPeoplePopReward, false, instanceId);
                }
            }
        }

        protected override void OnOpened() {
            uint id = this.m_Data.unfinishedFirstStageId;
            CSVInstanceDaily.Data cSVInstanceDailyData = CSVInstanceDaily.Instance.GetConfData(id);
            if (cSVInstanceDailyData != null) {
                if (cSVInstanceDailyData.LayerStage <= 1) {
                    this.m_Layout.scrollbar.MoveTo(0f);
                }
                else if (cSVInstanceDailyData.LayerStage >= 10) {
                    this.m_Layout.scrollbar.MoveTo(1f);
                }
                else {
                    this.m_Layout.scrollbar.MoveTo(1f * (cSVInstanceDailyData.LayerStage - 1) / (10 - 1));
                }
            }
        }

        #endregion

        #region 初始化

        private void OnParseComponent() {
            this.m_Layout.Load(this.gameObject.transform);
            this.m_Layout.SetListener(this);

            this.m_Layout.m_Data = this.m_Data;
        }

        private void RefreshView() {
            this.SetTitle();
            this.SetStageList();
            this.SetSubList();
            this.SetSelectMenuView();
        }

        public void SetTitle() {
            uint id = this.m_Data.unfinishedFirstStageId;
            CSVInstanceDaily.Data cSVInstanceDailyData = CSVInstanceDaily.Instance.GetConfData(id);
            string format = LanguageHelper.GetTextContent(1006154);
            string text = cSVInstanceDailyData == null ? string.Empty : string.Format(format, cSVInstanceDailyData.LayerStage.ToString(), cSVInstanceDailyData.Layerlevel.ToString());
            this.m_Layout.text_BestStage.text = text;

            this.m_Layout.go_DayRewardView.gameObject.SetActive(false);

            bool hasGot = Sys_HundredPeopleArea.Instance.HasGotAward();
            this.m_Layout.btnDailyAward.gameObject.SetActive(Sys_HundredPeopleArea.Instance.towerInsData.RewardStageId != 0);
            if (!hasGot) {
                this.m_Layout.fxDailyReward.SetActive(true);
                ImageHelper.SetImageGray(this.m_Layout.btnDailyAward, false, true);
            }
            else {
                this.m_Layout.fxDailyReward.SetActive(false);
                ImageHelper.SetImageGray(this.m_Layout.btnDailyAward, true, true);
            }

            //if (Sys_HundredPeopleArea.Instance.towerInsData == null) {
            //    this.m_Layout.btnDailyAward.gameObject.SetActive(false);
            //}
            //else {
            //    this.m_Layout.btnDailyAward.gameObject.SetActive(!hasGot && Sys_HundredPeopleArea.Instance.towerInsData.RewardStageId != 0);
            //}
        }

        private void SetStageList() {
            for (int i = 0; i < this.m_Layout.list_StageView.Count; i++) {
                this.m_Layout.SetStageItem(i);
            }
        }

        private void SetSubList() {
            for (int i = 0; i < this.m_Layout.list_StageSubs.Count; i++) {
                CSVInstanceDaily.Data cSVInstanceDailyData = this.m_Data.GetStageCofig(i);
                string format = LanguageHelper.GetTextContent(1006199);
                string text = cSVInstanceDailyData == null ? string.Empty : string.Format(format, (((cSVInstanceDailyData.LayerStage - 1) * 10) + cSVInstanceDailyData.Layerlevel).ToString(), "0");
                this.m_Layout.SubItem(i, text);
            }
        }

        private void SetSelectMenuView() {
            CSVInstanceDaily.Data cSVInstanceDailyData = CSVInstanceDaily.Instance.GetConfData(this.m_Data.curSubId);
            if (null == cSVInstanceDailyData) return;

            this.m_Layout.text_SelectStage.text = cSVInstanceDailyData.LayerStage.ToString();
            this.m_Layout.text_Describe.text = LanguageHelper.GetTextContent(cSVInstanceDailyData.Describe);
            bool isValid = Sys_Attr.Instance.rolePower < cSVInstanceDailyData.Score;
            TextHelper.SetText(m_Layout.recommendScore, 
                isValid ? 1006208u : 1006209u, 
                cSVInstanceDailyData.Score.ToString());

            CSVTravellerAwakening.Data csvAwken = CSVTravellerAwakening.Instance.GetConfData(cSVInstanceDailyData.Awakeningid);
            if (csvAwken != null) {
                this.m_Layout.go_awkenLimit.SetActive(true);
                uint curLevel = Sys_TravellerAwakening.Instance.awakeLevel;
                if (curLevel >= csvAwken.id) {
                    // 绿色
                    TextHelper.SetText(this.m_Layout.txt_awkenLimit, 1006197, LanguageHelper.GetTextContent(csvAwken.NameId));
                }
                else {
                    // 红色
                    TextHelper.SetText(this.m_Layout.txt_awkenLimit, 1006196, LanguageHelper.GetTextContent(csvAwken.NameId));
                }
            }
            else {
                this.m_Layout.go_awkenLimit.SetActive(false);
            }

            bool isOver = this.m_Data.IsOver((int) cSVInstanceDailyData.LayerStage, (int) cSVInstanceDailyData.Layerlevel, Sys_HundredPeopleArea.Instance.passedInstanceId);
            if (isOver) {
                // 通关该layer
                TextHelper.SetText(this.m_Layout.go_LockTips, 1006171);
                this.m_Layout.go_LockTips.gameObject.SetActive(true);
                ButtonHelper.Enable(this.m_Layout.button_Fight, false);
            }
            else {
                bool isPreOver = this.m_Data.IsPreStageLayerOver((int) cSVInstanceDailyData.LayerStage, (int) cSVInstanceDailyData.Layerlevel, Sys_HundredPeopleArea.Instance.passedInstanceId);
                bool isLock = this.m_Data.IsLock((int) cSVInstanceDailyData.LayerStage, (int) cSVInstanceDailyData.Layerlevel);
                if (!isPreOver || isLock) {
                    if (!isPreOver) {
                        TextHelper.SetText(this.m_Layout.go_LockTips, 1006169);
                    }
                    else {
                        // 未解锁
                        if (isLock) {
                            TextHelper.SetText(this.m_Layout.go_LockTips, 1006170, cSVInstanceDailyData.LevelLimited.ToString());
                        }
                        else {
                            TextHelper.SetText(this.m_Layout.go_LockTips, 1006169);
                        }
                    }

                    this.m_Layout.go_LockTips.gameObject.SetActive(true);
                    ButtonHelper.Enable(this.m_Layout.button_Fight, false);
                }
                else {
                    this.m_Layout.go_LockTips.gameObject.SetActive(false);
                    ButtonHelper.Enable(this.m_Layout.button_Fight, true);
                }
            }

            void SetRewardItem(PropItem propItem, uint id, long Num, uint equipPara) {
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(id);
                if (null == cSVItemData) {
                    propItem.SetActive(false);
                    return;
                }

                propItem.SetActive(true);
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(id, Num, true, false, false, false, false, true, false, true,  PropItem.OnClickPropItem, false, false);
                // itemData.SetQuality(Sys_Equip.Instance.CalPreviewQuality(equipPara));
                itemData.EquipPara = equipPara;
                propItem.SetData(new MessageBoxEvt(EUIID.UI_HundredPeopleArea, itemData));

                // Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(propItem.transform.Find("Btn_Item/Image_BG").gameObject);
                // eventListener.triggers.Clear();
                // eventListener.AddEventListener(UnityEngine.EventSystems.EventTriggerType.PointerClick, ret => { UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_HundredPeopleArea, itemData)); });
            }

            List<ItemIdCount> list_drop1 = CSVDrop.Instance.GetDropItem(cSVInstanceDailyData.Award);
            List<ItemIdCount> list_drop2 = CSVDrop.Instance.GetDropItem(cSVInstanceDailyData.RandomAward);
            list_drop1.AddRange(list_drop2);
            this.m_Layout.CreatePropItemList(list_drop1.Count);

            for (int i = 0, count = this.m_Layout.list_DropRewardItem.Count; i < count; i++) {
                uint x = i < list_drop1.Count ? list_drop1[i].id : 0;
                long y = i < list_drop1.Count ? list_drop1[i].count : 0;
                uint equipPara = (uint)(i < list_drop1.Count ? list_drop1[i].equipPara : 0);
                SetRewardItem(this.m_Layout.list_DropRewardItem[i], x, y, equipPara);
            }
        }
        #endregion

        #region 响应事件

        private void OnUpdateLevel() {
            this.RefreshView();
        }

        private void OnGotDailyAward() {
            this.RefreshView();
        }

        public void OnClick_Close() {
            this.CloseSelf();
        }

        public void OnClick_Rank() {
            UIManager.OpenUI(EUIID.UI_HundredPeopleAreaRank);
        }

        public void OnClick_OpenDailyRewardView() {
            uint instanceId = 0;
            if (null != Sys_HundredPeopleArea.Instance.towerInsData) {
                instanceId = Sys_HundredPeopleArea.Instance.towerInsData.RewardStageId;
            }

            CSVInstanceDaily.Data cSVInstanceDailyData = CSVInstanceDaily.Instance.GetConfData(instanceId);
            if (null == cSVInstanceDailyData) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1006189));
                this.m_Layout.go_DayRewardView.gameObject.SetActive(false);
                ButtonHelper.Enable(this.m_Layout.btnGotReward, false);
            }
            else {
                this.m_Layout.go_DayRewardView.gameObject.SetActive(true);
                this.m_Layout.SetDayRewardView(LanguageHelper.GetTextContent(1006191, cSVInstanceDailyData.LayerStage.ToString(), (((cSVInstanceDailyData.LayerStage - 1) * 10) + cSVInstanceDailyData.Layerlevel).ToString()), 0);

                var ls = CSVDrop.Instance.GetDropItem(CSVHundredChapter.Instance.GetConfData(instanceId).dropid);
                this.m_Layout.rewardList.SetRewardList(ls);
                this.m_Layout.rewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

                bool hasGot = Sys_HundredPeopleArea.Instance.HasGotAward();
                ButtonHelper.Enable(this.m_Layout.btnGotReward, !hasGot);
            }
        }

        public void OnClick_CloseDailyRewardView() {
            this.m_Layout.go_DayRewardView.gameObject.SetActive(false);
        }

        public void OnClick_Fight() {
            uint activityid = Sys_HundredPeopleArea.Instance.activityid;
            uint instanceId = this.m_Data.curSubId;
            if (activityid == 0 || instanceId == 0) return;

            this.CloseSelf();
            Sys_Instance.Instance.InstanceEnterReq(activityid, instanceId);
        }

        public void OnClick_Stage(Toggle toggle, bool value) {
            int index = this.m_Layout.GetStageIndex(toggle);
            int stageLevel = this.m_Data.GetStageID(index);
            uint passedInstanceId = Sys_HundredPeopleArea.Instance.passedInstanceId;
            int instanceId = this.m_Data.UnlockLayerLevelId(stageLevel, passedInstanceId, out UI_HundredPeopleArea_Data.EStageLockReason reason);

            if (reason == UI_HundredPeopleArea_Data.EStageLockReason.PreNotOver ||
                UI_HundredPeopleArea_Data.EStageLockReason.ConditionNotValid == reason) {
                // 未解锁
                this.m_Layout.PlayAnimation_View(false, false);
            }
            else if (reason == UI_HundredPeopleArea_Data.EStageLockReason.AllOver) {
                // 全部通关
                this.m_Layout.PlayAnimation_View(false, false);
            }
            else {
                if (value) {
                    this.m_Data.SetSelectStageIndex(index);

                    // 默认选中tabIndex
                    int layerToggleIndex = (int) CSVInstanceDaily.Instance.GetConfData((uint) instanceId).Layerlevel - 1;
                    this.m_Layout.list_StageSubs[layerToggleIndex].isOn = true;

                    this.SetSubList();
                    this.SetSelectMenuView();
                }

                if (value) {
                    this.m_Layout.PlayAnimation_View(true, false);
                }
                else if (!this.m_Layout.toggleGroup_Stage.AnyTogglesOn()) {
                    this.m_Layout.PlayAnimation_View(false, false);
                }
            }
        }

        public void OnClick_StageSub(Toggle toggle, bool value) {
            if (value) {
                int index = this.m_Layout.GetStageMenuIndex(toggle);
                this.m_Data.SetSelectSubIndex(index);
                this.SetSelectMenuView();
            }
        }

        public void OnClick_DailyReward() {
            Sys_HundredPeopleArea.Instance.SendTowerInstanceDailyRewardReq();
        }

        public void OnClick_BG() {
            if (this.m_Layout.isPlayed) {
                int index = this.m_Data.selectStageIndex;
                this.m_Layout.list_Stage[index].isOn = false;
                //this.m_Layout.PlayAnimation_View(false, false);
            }
        }

        public void OnBtnAwkenClicked() {
            UIManager.OpenUI(EUIID.UI_Awaken);
        }

        public void OnBtnTipAwkenClicked() {
            this.m_Layout.RefreshTipAwken();
            this.m_Layout.tipAwkenNode.SetActive(true);
        }

        public void OnBtnTipAwkenBgClicked() {
            this.m_Layout.tipAwkenNode.SetActive(false);
        }

        #endregion
    }
}