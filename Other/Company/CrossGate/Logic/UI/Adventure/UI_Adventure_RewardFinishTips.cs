using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Adventure_RewardFinishTips : UIBase
    {
        private uint rewardId;
        #region 界面组件
        private Image imgIcon;
        //private Text txtName;
        private Text txtTaskName;
        private Text txtDesc;
        private Button btnClose;
        private GameObject goMinItemCell;
        private GameObject goItemCell;
        private GameObject goRewardTitle;
        #endregion
        private List<CeilGrid> itemList = new List<CeilGrid>();

        #region 系统函数
        protected override void OnOpen(object arg)
        {            
            rewardId = (uint)arg;
        }
        protected override void OnLoaded()
        {         
            Parse();
        }
        protected override void OnShow()
        {            
            UpdateView();
        }
        #endregion

        #region function
        private void Parse()
        {
            imgIcon = transform.Find("Animator/View_Left/Image_Mask/Image_Icon").GetComponent<Image>();
            //txtName = transform.Find("Animator/View_Left/Image_Name/Text_Name").GetComponent<Text>();
            txtTaskName = transform.Find("Animator/View_Right/Image_Title/Text_Title").GetComponent<Text>();
            txtDesc = transform.Find("Animator/View_Right/Text_Message").GetComponent<Text>();
            goMinItemCell = transform.Find("Animator/View_Right/Grid_Award/AwardItem").gameObject;
            goMinItemCell.SetActive(false);
            goItemCell = transform.Find("Animator/View_Right/Scroll_View/Viewport/PropItem").gameObject;
            goItemCell.SetActive(false);
            btnClose = transform.Find("Black").GetComponent<Button>();
            btnClose.onClick.AddListener(OnbtnCloseClick);
            goRewardTitle = transform.Find("Animator/View_Right/Text_Name").gameObject;
        }
        private void UpdateView()
        {
            CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(rewardId);
            if (data != null)
            {
                ImageHelper.SetIcon(imgIcon, data.image);
                txtTaskName.text = LanguageHelper.GetTextContent(data.name);
                txtDesc.text = LanguageHelper.GetTextContent(data.detailedDes);
                uint lastTaskId = data.finishTaskId;
                UpdateRewardView(lastTaskId);
            }
        }
        private void UpdateRewardView(uint taskId)
        {
            FrameworkTool.DestroyChildren(goMinItemCell.transform.parent.gameObject, goMinItemCell.name);
            FrameworkTool.DestroyChildren(goItemCell.transform.parent.gameObject, goItemCell.name);
            CSVTask.Data taskData = CSVTask.Instance.GetConfData(taskId);
            if (taskData != null)
            {
                List<List<uint>> minRewards = new List<List<uint>>();
                if (taskData.RewardGold != null)
                {
                    minRewards.Add(taskData.RewardGold);
                }
                if (taskData.RewardExp != null)
                {
                    minRewards.Add(taskData.RewardExp);
                }
                for (int i = 0; i < minRewards.Count; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(goMinItemCell, goMinItemCell.transform.parent);
                    go.SetActive(true);
                    Image img = go.transform.Find("Image").GetComponent<Image>();
                    Text txtCount = go.transform.Find("Text_Name").GetComponent<Text>();
                    Text txtName = go.transform.Find("Text_Number").GetComponent<Text>();
                    CSVItem.Data item = CSVItem.Instance.GetConfData(minRewards[i][0]);
                    ImageHelper.SetIcon(img, item.small_icon_id);
                    txtCount.text = minRewards[i][1].ToString();
                    txtName.text = LanguageHelper.GetTextContent(item.name_id);
                }
                int length = taskData.DropId.Count;
                for (int i = 0; i < length; i++)
                {
                    itemList.Clear();
                    List<ItemIdCount> dropItems = CSVDrop.Instance.GetDropItem(taskData.DropId[i]);
                    bool hasReward = dropItems != null;
                    goRewardTitle.SetActive(hasReward);
                    if (hasReward)
                    {
                        int len = dropItems.Count;
                        for (int j = 0; j < len; j++)
                        {
                            ItemData item = new ItemData(0, 0, dropItems[j].id, (uint)dropItems[j].count, 0, false, false, null, null, 0);
                            GameObject go = GameObject.Instantiate<GameObject>(goItemCell, goItemCell.transform.parent);
                            go.SetActive(true);
                            CeilGrid bagCeilGrid = new CeilGrid();
                            bagCeilGrid.BindGameObject(go);
                            bagCeilGrid.AddClickListener(OnItemClick);
                            itemList.Add(bagCeilGrid);
                            bagCeilGrid.SetData(item, i, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_Adventure);
                        }
                    }
                }

            }
        }
        #endregion

        #region 响应事件
        private void OnbtnCloseClick()
        {
            Sys_Adventure.Instance.eventEmitter.Trigger(Sys_Adventure.EEvents.OnTryDoMainTaskByRewardTaskFinish);
            UIManager.CloseUI(EUIID.UI_Adventure_RewardFinishTips);
        }
        private void OnItemClick(CeilGrid bagCeilGrid)
        {
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(bagCeilGrid.mItemData.Id, 0, false, false, false, false, false, false, true);
            UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Decompose, itemData));
        }
        #endregion
    }
}
