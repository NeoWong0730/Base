using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace Logic
{
    public class UI_BackGift : UI_BackActivityBase
    {
        private Text txtTips;
        private Button btnGet;
        private GameObject goIsGet;

        private InfinityGrid infinity;
        private List<ItemIdCount> listItem;
        #region 系统函数
        protected override void Loaded()
        {
            Parse();

        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            UpdateView();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_BackActivity.Instance.eventEmitter.Handle(Sys_BackActivity.EEvents.OnBackActivityRedPointUpdate, OnBackActivityRedPointUpdate, toRegister);
            Sys_BackActivity.Instance.eventEmitter.Handle(Sys_BackActivity.EEvents.OnBackActivityInfoUpdate, OnBackActivityInfoUpdate, toRegister);
        }
        public override bool CheckFunctionIsOpen()
        {
            return Sys_BackActivity.Instance.CheckBackGiftIsOpen();
        }
        public override bool CheckTabRedPoint()
        {
            return Sys_BackActivity.Instance.CheckBackGiftCanGet();
        }
        #endregion
        #region func
        private void Parse()
        {
            txtTips = transform.Find("Text_Tips").GetComponent<Text>();
            btnGet = transform.Find("State/Btn_01").GetComponent<Button>();
            btnGet.onClick.AddListener(OnBtnGetClick);
            goIsGet = transform.Find("State/Image").gameObject;
            infinity = transform.Find("Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
        }
        private void UpdateView()
        {
            txtTips.text = LanguageHelper.GetTextContent(2014702, Sys_BackActivity.Instance.GetLeaveDays().ToString());
            CSVReturnGifts.Data data = CSVReturnGifts.Instance.GetConfData(Sys_BackActivity.Instance.ActivityGroup);
            if (data != null)
            {
                listItem = CSVDrop.Instance.GetDropItem(data.Reward);
                infinity.CellCount = listItem.Count;
                infinity.ForceRefreshActiveCell();
            }
            bool canGet = Sys_BackActivity.Instance.CheckBackGiftCanGet();
            btnGet.gameObject.SetActive(canGet);
            goIsGet.SetActive(!canGet);
        }
        #endregion
        #region event
        private void OnBtnGetClick()
        {
            bool canGet = Sys_BackActivity.Instance.CheckBackGiftCanGet();
            if (canGet)
            {
                Sys_BackActivity.Instance.ReqGetBackGift();
            }
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            PropItem mCell = new PropItem();
            mCell.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            PropItem mCell = cell.mUserData as PropItem;
            var dropItem = listItem[index];
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(dropItem.id, dropItem.count, true, false, false, false, false, true);
            mCell.SetData(itemData, EUIID.UI_BackActivity);
        }
        private void OnBackActivityRedPointUpdate()
        {
            UpdateView();
        }
        private void OnBackActivityInfoUpdate()
        {
            UpdateView();
        }
        #endregion
    }
}
