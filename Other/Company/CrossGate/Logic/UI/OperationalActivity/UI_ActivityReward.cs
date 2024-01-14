using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;

namespace Logic
{
    public class UI_ActivityReward : UI_OperationalActivityBase
    {
        private InfinityGrid infinityGrid;
        private Button btn_GetRewardAll;

        int visualGridCount = 0;
        List<uint> RewardIdList;
        #region 系统函数
        protected override void InitBeforOnShow()
        {
            btn_GetRewardAll = transform.Find("Btn_01").GetComponent<Button>();
            infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
            btn_GetRewardAll.onClick.AddListener(OnClick_ReceiveAll);

            RewardIdList = Sys_OperationalActivity.Instance.ActivityRewardIdList;
            visualGridCount = RewardIdList.Count;
        }
        public override void Show()
        {
            base.Show();
            ShowView();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateActivityRewardData, UpdateView, toRegister);
        }

        #endregion

        #region Fun

        private void UpdateView()
        {
            for (int i = 0; i < visualGridCount; i++)
            {
                InfinityGridCell cell = infinityGrid.GetItemByIndex(i);
                if (cell == null)
                    continue;
                UI_ActivityRewardItem entry = cell.mUserData as UI_ActivityRewardItem;
                entry.UpdateRewardState();
            }
        }

        public void ShowView()
        {
            infinityGrid.CellCount = visualGridCount;
            infinityGrid.ForceRefreshActiveCell();
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_ActivityRewardItem entry = cell.mUserData as UI_ActivityRewardItem;
            entry.UpdateViewCell(Sys_OperationalActivity.Instance.ActivityRewardIdList[index]);
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_ActivityRewardItem entry = new UI_ActivityRewardItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnClick_ReceiveAll()
        {
            Sys_OperationalActivity.Instance.ActivityReceiveReq(0);
        }
        #endregion
    }

    public class UI_ActivityRewardItem
    {
        GameObject go_Content,go_Get;
        Text text_Title, text_TitleTip;
        Button btn_Receive;

        uint id;
        Sys_OperationalActivity.Enum_RewardState state;
        CSVOperatereward.Data csvOperatereward;
        public void Init(RectTransform mRootTransform)
        {
            go_Content = mRootTransform.Find("Scroll_View/Viewport").gameObject;
            btn_Receive = mRootTransform.Find("Btn_01").GetComponent<Button>();
            go_Get = mRootTransform.Find("Text_Get").gameObject;
            text_Title = mRootTransform.Find("Text").GetComponent<Text>();
            text_TitleTip = mRootTransform.Find("Text01").GetComponent<Text>();

            btn_Receive.onClick.AddListener(OnClick_Receive);
        }

        public void UpdateViewCell(uint id)
        {
            this.id = id;
            if (!CSVOperatereward.Instance.TryGetValue(id, out csvOperatereward))
                return;
            if (!Sys_OperationalActivity.Instance.ActivityRewardStateDic.TryGetValue(id, out state))
                return;
            UpdateItemList();
            UpdateRewardState();
            text_Title.text= CSVLanguage.Instance.GetConfData(csvOperatereward.title).words;
            text_TitleTip.text = CSVLanguage.Instance.GetConfData(csvOperatereward.titleTips).words;
        }

        public void UpdateRewardState()
        {
            if (!Sys_OperationalActivity.Instance.ActivityRewardStateDic.TryGetValue(id, out state))
                return;
            switch (state)
            {
                case Sys_OperationalActivity.Enum_RewardState.Enum_CanReceive:
                    UpdateShow(false);
                    break;
                case Sys_OperationalActivity.Enum_RewardState.Enum_Received:
                    UpdateShow(true);
                    break;
                default:
                    break;
            }
        }

        private void UpdateItemList()
        {
            List<ItemIdCount> propList;
            propList = CSVDrop.Instance.GetDropItem(csvOperatereward.dropId);
            if (propList != null)
            {
                FrameworkTool.CreateChildList(go_Content.transform, propList.Count);
                for (int i = 0; i < propList.Count; i++)
                {
                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(go_Content.transform.GetChild(i).gameObject);
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(propList[i].id, propList[i].count, true, false, false, false, false,
                            _bShowCount: true, _bUseClick: true, _bShowBagCount: false);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, itemData));
                }
            }
        }

        private void UpdateShow(bool isReceived)
        {
            go_Get.SetActive(isReceived);
            btn_Receive.gameObject.SetActive(!isReceived);
        }

        private void OnClick_Receive()
        {
            if (id == 0)
                return; 
            Sys_OperationalActivity.Instance.ActivityReceiveReq(id);
        }
    }
}