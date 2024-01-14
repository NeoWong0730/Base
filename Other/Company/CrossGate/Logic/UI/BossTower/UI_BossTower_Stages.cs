using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;

namespace Logic
{
    //Boss阶段预览界面
    public class UI_BossTower_Stages : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            InitView();
        }
        protected override void OnClose()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 组件
        Button btnClose;
        InfinityGrid infinityGrid;
        #endregion
        #region 数据
        List<BossTowerStageData> bossStageList;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            btnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            infinityGrid = transform.Find("Animator/Tips/Scroll View").GetComponent<InfinityGrid>();

            btnClose.onClick.AddListener(() => { CloseSelf(); });

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
        }
        #endregion
        private void InitView()
        {
            RefreshTipsData();
        }
        private void RefreshTipsData()
        {
            bossStageList = Sys_ActivityBossTower.Instance.GetBossTowerStageDataList();
            if (bossStageList != null && bossStageList.Count > 0)
            {
                infinityGrid.CellCount = bossStageList.Count;
                infinityGrid.ForceRefreshActiveCell();
            }
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            TipsItem entry = new TipsItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            TipsItem entry = cell.mUserData as TipsItem;
            entry.SetData(bossStageList[index].csvData);//索引数据
        }
        #region item
        public class TipsItem
        {
            Text stageNum;
            InfinityGrid infinityGrid;
            Text content;

            CSVBOSSTowerStage.Data data;
            List<ItemIdCount> dropItems;
            public void Init(Transform trans)
            {
                stageNum = trans.Find("Image_bg/Text").GetComponent<Text>();
                infinityGrid = trans.Find("Scroll View").GetComponent<InfinityGrid>();
                content = trans.Find("Text").GetComponent<Text>();

                infinityGrid.onCreateCell += OnCreateCell;
                infinityGrid.onCellChange += OnCellChange;
            }
            public void SetData(CSVBOSSTowerStage.Data data)
            {
                this.data = data;
                stageNum.text = LanguageHelper.GetTextContent(1009045,data.stage_number.ToString());
                content.text = LanguageHelper.GetTextContent(data.diffcultyDetails);
                RefreshReward();
            }
            private void RefreshReward()
            {
                dropItems = CSVDrop.Instance.GetDropItem(data.floor_drop);
                if (dropItems != null)
                {
                    infinityGrid.CellCount = dropItems.Count;
                    infinityGrid.ForceRefreshActiveCell();
                }
            }
            private void OnCreateCell(InfinityGridCell cell)
            {
                PropItem entry = new PropItem();
                entry.BindGameObject(cell.mRootTransform.gameObject);
                cell.BindUserData(entry);
            }
            private void OnCellChange(InfinityGridCell cell, int index)
            {
                PropItem entry = cell.mUserData as PropItem;
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                              (_id: dropItems[index].id,
                              _count: dropItems[index].count,
                              _bUseQuailty: true,
                              _bBind: false,
                              _bNew: false,
                              _bUnLock: false,
                              _bSelected: false,
                              _bShowCount: true,
                              _bShowBagCount: false,
                              _bUseClick: true,
                              _onClick: null,
                              _bshowBtnNo: false);
                entry.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, showItem));
            }
        }
        #endregion
    }
}