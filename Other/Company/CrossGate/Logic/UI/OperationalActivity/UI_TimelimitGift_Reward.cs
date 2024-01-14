using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_TimelimitGift_Reward : UIBase
    {
        private uint giftId;

        private Button btnClose;
        private Transform content;
        private List<ItemIdCount> dropItems = new List<ItemIdCount>();

        #region 系统函数

        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(uint))
            {
                giftId = (uint)arg;
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
            btnClose = transform.Find("Image_Black").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);

            content = transform.Find("Animator/Scroll_View/Viewport");
        }

        private void UpdateView()
        {
            CSVConditionalGift.Data giftData = CSVConditionalGift.Instance.GetConfData(giftId);
            if (giftData != null)
            {
                uint dropId = giftData.Reward;
                dropItems = CSVDrop.Instance.GetDropItem(dropId);
                int len = dropItems.Count;
                FrameworkTool.CreateChildList(content, len);
                for (int i = 0; i < len; i++)
                {
                    GameObject go = content.GetChild(i).gameObject;
                    PropItem mCell = new PropItem();
                    mCell.BindGameObject(go);
                    var dropItem = dropItems[i];
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(dropItem.id, dropItem.count, true, false, false, false, false, true);
                    mCell.SetData(itemData, EUIID.UI_TimeLimitGift_Reward);
                }
            }
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
