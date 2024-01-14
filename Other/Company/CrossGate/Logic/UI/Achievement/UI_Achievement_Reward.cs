using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Achievement_Reward : UIBase
    {
        #region 组件
        Button closeBtn;
        InfinityGrid infinityGrid;
        #endregion
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            InitView();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 查找组件、事件注册
        private void OnParseComponent()
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            infinityGrid = transform.Find("Animator/Scroll View/").GetComponent<InfinityGrid>();

            closeBtn.onClick.AddListener(() => { CloseSelf(); });

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Achievement.Instance.eventEmitter.Handle(Sys_Achievement.EEvents.OnRefreshReward, OnRefreshReward, toRegister);
            Sys_Achievement.Instance.eventEmitter.Handle(Sys_Achievement.EEvents.OnRefreshLevelAndExp, OnRefreshLevelAndExp, toRegister);
        }
        private void OnRefreshReward()
        {
            RefreshRewardData();
        }
        private void OnRefreshLevelAndExp()
        {
            RefreshRewardData();
        }
        #endregion
        int curIndex;
        #region 初始化
        private void InitView()
        {
            RefreshRewardData();
            curIndex = Sys_Achievement.Instance.GetAchievementTargetValueByLevel(1);
            SetPosView();
        }
        #endregion

        #region 界面显示
        private void RefreshRewardData()
        {
            infinityGrid.CellCount = Sys_Achievement.Instance.achievementRewardList.Count;
            infinityGrid.ForceRefreshActiveCell();
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            AchRewardCell entry = new AchRewardCell();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            AchRewardCell entry = cell.mUserData as AchRewardCell;
            entry.SetData(Sys_Achievement.Instance.achievementRewardList[index]);
        }
        private void SetPosView()
        {
            float cellY = 90;
            float curContentY = curIndex * cellY;
            float maxContentY = Sys_Achievement.Instance.achievementRewardList.Count <= 5 ? 0 : (Sys_Achievement.Instance.achievementRewardList.Count - 5) * cellY;
            float posY = curContentY < maxContentY ? curContentY : maxContentY;
            infinityGrid.Content.anchoredPosition = new Vector2(infinityGrid.Content.anchoredPosition.x, posY);
        }
        #endregion
        public class AchRewardCell
        {
            Text textTitle;
            Text textLv;
            Button getBtn;
            GameObject receivedObj;
            Transform grid;
            GameObject redPoint;
            AchievementRewardData data;
            List<AchievementRewardItemCell> itemCellList = new List<AchievementRewardItemCell>();
            public void Init(Transform tran)
            {
                textTitle = tran.Find("Label").GetComponent<Text>();
                textLv = tran.Find("Text_Lv").GetComponent<Text>();
                getBtn = tran.Find("Btn_01_Small").GetComponent<Button>();
                redPoint = getBtn.transform.Find("Image_Red").gameObject;
                receivedObj = tran.Find("Image_Text").gameObject;
                grid = tran.Find("Grid");
                getBtn.onClick.AddListener(GetRewardOnClick);
            }
            public void SetData(AchievementRewardData data)
            {
                this.data = data;
                getBtn.gameObject.SetActive(data.csvAchievementLevelData.Drop_Id != 0);
                textTitle.text = LanguageHelper.GetAchievementContent(data.csvAchievementLevelData.Level_Test);
                textLv.text = data.csvAchievementLevelData.Level.ToString();
                CheckRewardIsReceive();
                RefreshRewardItem();
            }
            private void CheckRewardIsReceive()
            {
                if (data.state == EAchievementRewardState.Unfinished)
                {
                    getBtn.gameObject.SetActive(false);
                    receivedObj.SetActive(false);
                    redPoint.SetActive(false);
                }
                else if (data.state == EAchievementRewardState.Unreceived)
                {
                    getBtn.gameObject.SetActive(true);
                    receivedObj.SetActive(false);
                    redPoint.SetActive(true);
                }
                else
                {
                    getBtn.gameObject.SetActive(false);
                    receivedObj.SetActive(true);
                    redPoint.SetActive(false);
                }
            }
            private void RefreshRewardItem()
            {
                for (int i = 0; i < itemCellList.Count; i++)
                {
                    AchievementRewardItemCell cell = itemCellList[i];
                    PoolManager.Recycle(cell);
                }
                itemCellList.Clear();
                List<ItemIdCount> dropItems = CSVDrop.Instance.GetDropItem(data.csvAchievementLevelData.Drop_Id);
                int count = dropItems.Count;
                FrameworkTool.CreateChildList(grid, count);
                for (int i = 0; i < count; i++)
                {
                    Transform tran = grid.GetChild(i);
                    AchievementRewardItemCell cell = PoolManager.Fetch<AchievementRewardItemCell>();
                    cell.Init(tran);
                    cell.SetData(dropItems[i].id, (int)dropItems[i].count);
                    itemCellList.Add(cell);
                }
            }
            private void GetRewardOnClick()
            {
                Sys_Achievement.Instance.OnReceiveLevelRewardReq(data.tid);
            }
        }
    }
}