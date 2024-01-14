using Lib.Core;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using System;

namespace Logic
{
    public class UICostData
    {
        public uint itemId;
        public uint num;
        public Action action;
    }

    public class UI_Cost : UIBase
    {
        private Button btnClose;
        private Button btnConfirm;
        private PropItem propItem;

        private UICostData costData;

        protected override void OnLoaded()
        {
            btnClose = transform.Find("Animator/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_Cost);
            });

            btnConfirm = transform.Find("Animator/Button_Clear_Up").GetComponent<Button>();
            btnConfirm.onClick.AddListener(OnClickConfirm);

            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("Animator/PropItem").gameObject);
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                costData = (UICostData)arg;
            }
            else
            {
                costData = null;
            }
        }

        protected override void OnShow()
        {
            if (costData != null)
                UpdatePanel();
        }

        private void OnClickConfirm()
        {
            if (costData != null)
            {
                if (costData.num > Sys_Bag.Instance.GetItemCount(costData.itemId))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
                }
                else
                {
                    costData.action();
                    UIManager.CloseUI(EUIID.UI_Cost);
                }
            }
        }

        private void UpdatePanel()
        {
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(costData.itemId, costData.num, true, false, false, false, false, true, true, true);
            propItem.SetData(itemData, EUIID.UI_Cost);
            propItem.txtName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(costData.itemId).name_id);
        }
    }
}
