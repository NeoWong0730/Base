using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Lib.Core;
using Table;
using Packet;

namespace Logic
{
    public class PetExpeditionResultParam
    {
        public uint taskId;
        public uint rewardType;
        public List<SimpleItem> rewards;
    }
    public class UI_PetExpedition_Result : UIBase
    {
        private uint taskId;
        private uint rewardType;
        public List<SimpleItem> rewards;

        private Button btnClose;
        private GameObject goFail;
        private GameObject goSuccess;
        private GameObject goBigSuccess;
        private Transform rewardParent;
        private Text txtActivityPoint;//冒险点数

        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(PetExpeditionResultParam))
            {
                PetExpeditionResultParam param = arg as PetExpeditionResultParam;
                taskId = param.taskId;
                rewardType = param.rewardType;
                rewards = param.rewards;
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
            btnClose = transform.Find("Animator/Content/off-bg").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            goFail = transform.Find("Animator/Image_Failedbg").gameObject;
            goSuccess = transform.Find("Animator/Image_Successbg").gameObject;
            goBigSuccess = transform.Find("Animator/Image_Successbg2").gameObject;
            rewardParent = transform.Find("Animator/Content/Scroll View/Viewport/Content");
            txtActivityPoint = transform.Find("Animator/Content/NextNode/Image_Reward1/Text_Num").GetComponent<Text>();
        }
        private void UpdateView()
        {
            var petTaskData = CSVPetexploreTaskGroup.Instance.GetConfData(taskId/1000);
            if(petTaskData!= null)
            {
                txtActivityPoint.text = petTaskData.Point.ToString();
            }
            var rewardItems = rewards;
            FrameworkTool.CreateChildList(rewardParent, rewardItems.Count);
            for (int i = 0; i < rewardParent.childCount; i++)
            {
                Transform child = rewardParent.GetChild(i);
                PropItem itemCell = new PropItem();
                itemCell.BindGameObject(child.gameObject);
                var itemData = new PropIconLoader.ShowItemData(rewardItems[i].ItemInfoId, rewardItems[i].Count, true, false, false, false, false, true);
                itemCell.SetData(itemData, EUIID.UI_PetExpedition_RewardList);
            }
            goFail.SetActive(rewardType == 1);
            goSuccess.SetActive(rewardType == 2);
            goBigSuccess.SetActive(rewardType == 3);
        }

        #endregion

        #region event
       
        private void OnBtnCloseClick()
        {
            UIManager.CloseUI(EUIID.UI_PetExpedition_Task);
            this.CloseSelf();
        }
        #endregion
    }
}
