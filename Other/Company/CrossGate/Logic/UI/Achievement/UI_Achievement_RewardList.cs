using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Achievement_RewardList : UIBase
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
        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                clickData = arg as ClickShowPositionData;
            }
        }
        #endregion
        #region 组件
        Button closeBtn;
        Transform transImage_BG;
        InfinityGrid infinityGrid;
        #endregion
        #region 数据
        ClickShowPositionData clickData;
        AchievementDataCell curAchievementData;
        List<ItemIdCount> dropItems;
        #endregion
        #region 查找组件、注册事件
        private void OnParseComponent()
        {
            closeBtn = transform.Find("Image_off").GetComponent<Button>();
            transImage_BG = transform.Find("Animator");
            infinityGrid = transform.Find("Animator/Image_BG/Scroll View").GetComponent<InfinityGrid>();

            closeBtn.onClick.AddListener(() => { CloseSelf(); });

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        #endregion
        #region 初始化
        private void InitView()
        {
            if (clickData != null)
            {
                curAchievementData = clickData.data;
                SetPosition();
                RefreshCell();
            }
        }
        #endregion
        #region 界面显示
        public void SetPosition()
        {
            if (clickData != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(clickData.parent, clickData.clickTarget.position, null, out Vector2 localPos);
                float posY = localPos.y - clickData.clickTarget.sizeDelta.y;
                posY = posY <= -287f ? -287f : posY;
                Vector3 newPos = new Vector3(localPos.x + clickData.clickTarget.sizeDelta.x / 2, posY, 0);
                transImage_BG.localPosition = newPos;
            }
        }
        private void RefreshCell()
        {
            dropItems = curAchievementData.dropItems;
            infinityGrid.CellCount = dropItems.Count;
            infinityGrid.ForceRefreshActiveCell();
            infinityGrid.MoveToIndex(0);
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            AchievementRewardItemCell entry = new AchievementRewardItemCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            AchievementRewardItemCell entry = cell.mUserData as AchievementRewardItemCell;
            entry.SetData(dropItems[index].id, (int)dropItems[index].count);
        }
        #endregion
    }
}