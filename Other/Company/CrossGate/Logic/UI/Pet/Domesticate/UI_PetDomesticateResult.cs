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
    public class PetDomesticateResultParam
    {
        public List<ItemIdCount> listRewardItems;
    }

    public class UI_PetDomesticateResult : UIBase
    {
        private Button btnClose;
        private Button btnConfirm;
        private InfinityGrid infinity;
        private List<ItemIdCount> listItem;

        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(PetDomesticateResultParam))
            {
                PetDomesticateResultParam param = arg as PetDomesticateResultParam;
                listItem = param.listRewardItems;
            }
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            UpdateView();
        }
        protected override void OnHide()
        {

        }
        protected override void OnDestroy()
        {
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            btnConfirm = transform.Find("Animator/Btn_01").GetComponent<Button>();
            btnConfirm.onClick.AddListener(OnBtnCloseClick);
            infinity = transform.Find("Animator/Rect").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
        }

        private void UpdateView()
        {

            infinity.CellCount = listItem.Count;
            infinity.ForceRefreshActiveCell();
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
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
        #endregion
    }
}
