using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;

namespace Logic
{
    public class UI_Adventure_Reward : UIComponent
    {
        public EAdventurePageType PageType { get; } = EAdventurePageType.Reward;
        public uint openValue = 0;
        public static uint firstUnFinishRewardId = 0;
        private Dictionary<uint, uint> dictCellIndex = new Dictionary<uint, uint>();
        #region 界面组件
        private GameObject scrollCell;
        private RectTransform content;
        private GridLayoutGroup group;
        #endregion

        #region 系统函数
        protected override void Loaded()
        {
            base.Loaded();
            scrollCell = transform.Find("Scroll View/Viewport/Content/Item").gameObject;
            scrollCell.SetActive(false);
            content = transform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
            group = transform.Find("Scroll View/Viewport/Content").GetComponent<GridLayoutGroup>();
        }

        public override void Show()
        {
            base.Show();
            UpdateView();
        }

        public override void OnDestroy()
        {
            dictCellIndex.Clear();
            base.OnDestroy();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion

        #region function
        private void UpdateView()
        {
            dictCellIndex.Clear();
            firstUnFinishRewardId = 0;
            FrameworkTool.DestroyChildren(scrollCell.transform.parent.gameObject, scrollCell.name);
            List<uint> rewardIds = Sys_Adventure.Instance.GetSortRewardIds();
            for (int i = 0; i < rewardIds.Count; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(scrollCell, scrollCell.transform.parent);
                go.SetActive(true);
                uint rewardId = rewardIds[i];
                UI_RewardScrollCell cell = new UI_RewardScrollCell(rewardId);
                cell.Init(go.transform);
                dictCellIndex.Add(rewardId, (uint)i);
                cell.UpdateCellView(openValue);
            }
            if (openValue > 0)
            {
                firstUnFinishRewardId = openValue;
            }
            openValue = 0;

            UpdateScrollPos();
        }
        private void UpdateScrollPos()
        {
            if (firstUnFinishRewardId != 0)
            {
                dictCellIndex.TryGetValue(firstUnFinishRewardId, out uint index);
                float cellX = group.cellSize.x;
                float spX = group.spacing.x;
                float offSetX = 0 - index * (cellX + spX);
                content.anchoredPosition = new Vector3(offSetX, 0, 0);
            }
        }
        #endregion

        #region 响应事件

        #endregion

        public class UI_RewardScrollCell : UIComponent
        {
            private uint rewardId;
            private bool isUnLock;

            private Image imgHeadIcon;
            private Text txtName;
            private GameObject goLock;
            private Text txtLockDesc;
            private GameObject goItem;
            private GameObject goFinish;
            private Button btnInfo;
            private Button btnSelf;
            private GameObject goBtnInfo;
            private GameObject goUrgency;

            private List<CeilGrid> gridList = new List<CeilGrid>();

            public UI_RewardScrollCell(uint id)
            {
                rewardId = id;
            }
            protected override void Loaded()
            {
                base.Loaded();
                imgHeadIcon = transform.Find("Icon_bg/Icon").GetComponent<Image>();
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                goLock = transform.Find("Lock").gameObject;
                txtLockDesc = transform.Find("Lock/Text").GetComponent<Text>();
                goItem = transform.Find("RewardGrp/PropItem").gameObject;
                goItem.SetActive(false);
                goFinish = transform.Find("Finish").gameObject;
                btnInfo = transform.Find("Btn_01").GetComponent<Button>();
                btnInfo.onClick.AddListener(OnBtnInfoClick);
                btnSelf = transform.GetComponent<Button>();
                btnSelf.onClick.AddListener(OnBtnInfoClick);
                goBtnInfo = transform.Find("Btn_01").gameObject;
                goUrgency = transform.Find("Urgency").gameObject;
            }
            public void UpdateCellView(uint openValue)
            {
                CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(rewardId);
                if (data != null)
                {
                    isUnLock = Sys_Adventure.Instance.CheckRewardIsUnlock(data.id);
                    bool isUrgent = data.isUrgent == 1;
                    txtName.text = LanguageHelper.GetTextContent(data.name);
                    goLock.SetActive(!isUnLock);
                    uint lastTaskId = data.finishTaskId;
                    bool isFinish = TaskHelper.HasSubmited(lastTaskId);
                    goUrgency.SetActive(isUrgent && isUnLock && !isFinish);
                    if (isUnLock)
                    {
                        ImageHelper.SetIcon(imgHeadIcon, data.image);
                        goFinish.SetActive(isFinish);
                        goBtnInfo.SetActive(!isFinish);
                        if (!isFinish && firstUnFinishRewardId == 0)
                        {
                            firstUnFinishRewardId = rewardId;
                        }
                    }
                    else
                    {
                        ImageHelper.SetIcon(imgHeadIcon, data.greyImage);
                        goFinish.SetActive(false);
                        goBtnInfo.SetActive(false);
                        txtLockDesc.text = Sys_Adventure.Instance.GetRewardUnlockConditionText(data.id);
                    }
                    FrameworkTool.DestroyChildren(goItem.transform.parent.gameObject, goItem.name);
                    gridList.Clear();
                    List<ItemData> items = Sys_Adventure.Instance.GetTaskItems(lastTaskId);
                    for (int i = 0; i < items.Count; i++)
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(goItem, goItem.transform.parent);
                        go.SetActive(true);
                        CeilGrid bagCeilGrid = new CeilGrid();
                        bagCeilGrid.BindGameObject(go);
                        bagCeilGrid.AddClickListener(OnItemClick);
                        gridList.Add(bagCeilGrid);
                        bagCeilGrid.SetData(items[i], i, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_Adventure);
                    }
                }
                if (rewardId == openValue)
                {
                    OnBtnInfoClick();
                }
            }
            #region 响应事件
            private void OnBtnInfoClick()
            {
                if (isUnLock)
                {
                    UIManager.OpenUI(EUIID.UI_Adventure_RewardInfo, false, rewardId);
                    Sys_Adventure.Instance.ReportClickEventHitPoint("Reward_Cell_RewardId:" + rewardId);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(600000197));
                }
            }
            private void OnItemClick(CeilGrid bagCeilGrid)
            {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(bagCeilGrid.mItemData.Id, 0, false, false, false, false, false, false, true);
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Decompose, itemData));
            }
            #endregion
        }
    }
}
