using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;

namespace Logic
{
    public class UI_Adventure_RewardInfo : UIBase
    {
        private uint rewardId;
        #region 界面组件
        private Image imgIcon;
        private Text txtName;
        private Text txtTaskDesc;
        private GameObject goTaskCell;
        private GameObject goMinItemCell;
        private GameObject goItemCell;
        private GameObject goNoAward;
        private Button btnGoto;
        private GameObject goBtnGoto;
        private Button btnClose;
        #endregion
        private List<CeilGrid> itemList = new List<CeilGrid>();


        #region 系统函数
        protected override void OnLoaded()
        {            
            Parse();
        }
        protected override void OnShow()
        {            
            UpdateView();
        }
        protected override void OnOpen(object arg)
        {
            rewardId = (uint)arg;
        }
        #endregion

        #region function
        private void Parse()
        {
            imgIcon = transform.Find("Animator/View_Left/bg_Icon/Icon").GetComponent<Image>();
            txtName = transform.Find("Animator/View_Left/Text_Name").GetComponent<Text>();
            txtTaskDesc = transform.Find("Animator/View_Right/View_Desc/TaskDesc").GetComponent<Text>();
            goTaskCell = transform.Find("Animator/View_Right/View_Target/Scroll View/GameObject/TaskContent").gameObject;
            goTaskCell.SetActive(false);
            goMinItemCell = transform.Find("Animator/View_Right/View_Award/Text_Award/Node/Item").gameObject;
            goMinItemCell.SetActive(false);
            goItemCell = transform.Find("Animator/View_Right/View_Award/Image_Award/Scroll_View/Viewport/PropItem").gameObject;
            goItemCell.SetActive(false);
            goNoAward = transform.Find("Animator/View_Right/View_Award/Image_Award/No_Award").gameObject;
            goBtnGoto = transform.Find("Animator/View_Right/Button_Goto").gameObject;
            btnGoto = transform.Find("Animator/View_Right/Button_Goto").GetComponent<Button>();
            btnGoto.onClick.AddListener(OnBtnGotoClick);
            btnClose = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
        }
        private void UpdateView()
        {
            CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(rewardId);
            if (data != null)
            {
                ImageHelper.SetIcon(imgIcon, data.image);
                txtName.text = LanguageHelper.GetTextContent(data.name);
                txtTaskDesc.text = LanguageHelper.GetTextContent(data.detailedDes);
                UpdateTaskCellView(data.nodeTask);
                uint lastTaskId = data.finishTaskId;
                UpdateRewardView(lastTaskId);
            }
        }
        private void UpdateTaskCellView(List<uint> taskIds)
        {
            FrameworkTool.DestroyChildren(goTaskCell.transform.parent.gameObject, goTaskCell.name);
            for (int i = 0; i < taskIds.Count; i++)
            {
                uint taskId = taskIds[i];
                GameObject go = GameObject.Instantiate<GameObject>(goTaskCell, goTaskCell.transform.parent);
                go.SetActive(true);
                GameObject goFinish = go.transform.Find("Finish").gameObject;
                GameObject goUnFinish = go.transform.Find("UnFinish").gameObject;
                Text txtTaskName = go.transform.Find("Text").GetComponent<Text>();
                bool isFinish = TaskHelper.HasSubmited(taskId);
                goFinish.SetActive(isFinish);
                goUnFinish.SetActive(!isFinish);
                CSVTask.Data taskData = CSVTask.Instance.GetConfData(taskId);
                if (taskData != null && isFinish)
                {
                    txtTaskName.text = CSVTaskLanguage.Instance.GetConfData(taskData.taskContent[0]).words;
                }
                else
                {
                    txtTaskName.text = LanguageHelper.GetTextContent(600000198);
                }
            }
        }
        private void UpdateRewardView(uint taskId)
        {
            FrameworkTool.DestroyChildren(goMinItemCell.transform.parent.gameObject, goMinItemCell.name);
            FrameworkTool.DestroyChildren(goItemCell.transform.parent.gameObject, goItemCell.name);
            CSVTask.Data taskData = CSVTask.Instance.GetConfData(taskId);
            bool hasItem = false;
            if (taskData != null)
            {
                List<List<uint>> minRewards = new List<List<uint>>();
                if (taskData.RewardGold != null)
                {
                    minRewards.Add(taskData.RewardGold);
                }
                if (taskData.RewardGold != null)
                {
                    minRewards.Add(taskData.RewardExp);
                }
                for (int i = 0; i < minRewards.Count; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(goMinItemCell, goMinItemCell.transform.parent);
                    go.SetActive(true);
                    Image img = go.transform.Find("Image").GetComponent<Image>();
                    Text txt = go.transform.Find("Text").GetComponent<Text>();
                    CSVItem.Data item = CSVItem.Instance.GetConfData(minRewards[i][0]);
                    ImageHelper.SetIcon(img, item.small_icon_id);
                    txt.text = minRewards[i][1].ToString();
                }
                if (taskData.DropId != null)
                {
                    int length = taskData.DropId.Count;
                    for (int i = 0; i < length; i++)
                    {
                        itemList.Clear();
                        List<ItemIdCount> dropItems = CSVDrop.Instance.GetDropItem(taskData.DropId[i]);
                        if (dropItems != null)
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
                                hasItem = true;
                            }
                        }
                    }
                }
            }
            goNoAward.SetActive(!hasItem);
            goBtnGoto.SetActive(!Sys_Task.Instance.IsSubmited(taskId));
        }
        #endregion

        #region 响应事件
        private void OnBtnGotoClick()
        {
            CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(rewardId);
            List<uint> taskIds = Sys_Adventure.Instance.GetAllTaskId(data.acceptTaskId, data.finishTaskId);
            uint targetTaskId = 0;
            for (int i = 0; i < taskIds.Count; i++)
            {
                uint taskId = taskIds[i];
                if (!TaskHelper.HasSubmited(taskId))
                {
                    targetTaskId = taskId;
                }
                else
                {
                    break;
                }
            }
            DebugUtil.Log(ELogType.eNone, "adventure Reward TryToTask | " + targetTaskId);
            if (Sys_Task.Instance.GetTaskState(targetTaskId) == ETaskState.UnReceived || Sys_Task.Instance.GetTaskState(targetTaskId) == ETaskState.UnReceivedButCanReceive)
            {
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(CSVTask.Instance.GetConfData(targetTaskId).receiveNpc, true);
            }
            else
            {
                Sys_Task.Instance.TryDoTask(targetTaskId, true, false, true);
            }
            UIManager.CloseUI(EUIID.UI_Adventure_RewardInfo);
            Sys_Adventure.Instance.eventEmitter.Trigger(Sys_Adventure.EEvents.OnCLoseAdventureView);
            Sys_Adventure.Instance.ReportClickEventHitPoint("Reward_Info_GotoTask_RewardId:" + rewardId.ToString());
        }
        private void OnItemClick(CeilGrid bagCeilGrid)
        {
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(bagCeilGrid.mItemData.Id, 0, false, false, false, false, false, false, true);
            UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Decompose, itemData));
        }
        private void OnBtnCloseClick()
        {
            UIManager.CloseUI(EUIID.UI_Adventure_RewardInfo);
        }
        #endregion
    }
}
