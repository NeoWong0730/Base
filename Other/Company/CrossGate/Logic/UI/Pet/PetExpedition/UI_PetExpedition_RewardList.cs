using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Lib.Core;
using Table;

namespace Logic
{
    public class PetExpeditionRewardParam
    {
        public uint rewardId;
        public Vector3 v3;
    }
    public class UI_PetExpedition_RewardList : UIBase
    {
        private uint rewardId = 0;
        private Vector3 v3;

        private Button btnClose;
        private Transform rewardParent;
        private Transform self;
        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(PetExpeditionRewardParam))
            {
                PetExpeditionRewardParam param = arg as PetExpeditionRewardParam;
                rewardId = param.rewardId;
                v3 = param.v3;
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
            btnClose = transform.Find("Image_off").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            rewardParent = transform.Find("Animator/Content");
            self = transform.Find("Animator");
        }
        private void UpdateView()
        {

            var rewardItems = CSVDrop.Instance.GetDropItem(rewardId);
            FrameworkTool.CreateChildList(rewardParent, rewardItems.Count);
            for (int i = 0; i < rewardParent.childCount; i++)
            {
                Transform child = rewardParent.GetChild(i);
                PropItem itemCell = new PropItem();
                itemCell.BindGameObject(child.gameObject);
                var itemData = new PropIconLoader.ShowItemData(rewardItems[i].id, rewardItems[i].count, true, false, false, false, false, true);
                itemCell.SetData(itemData, EUIID.UI_PetExpedition_RewardList);
            }
            var oldSelfPos = self.localPosition;
            self.localPosition = new Vector3(transform.worldToLocalMatrix.MultiplyPoint(v3).x, oldSelfPos.y, oldSelfPos.z);
        }

        #endregion

        #region event

        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        #endregion
    }
}
