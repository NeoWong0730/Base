using Logic.Core;
using Lib.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Logic
{
    public class UI_ExchangePointReset : UIBase
    {
        private Button btnClose;
        private PropItem propItem;

        private Text txtCostName;
        private Text txtCostNum;
        private Button btnReset;
        private uint paId;

        private uint costItemId;
        private uint costItemCount;
        private uint totalItemCount;

        protected override void OnLoaded()
        {
            btnClose = transform.Find("Animator/View_TipsBg02_Small/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);
            
            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("Animator/PropItem").gameObject);

            txtCostName = transform.Find("Animator/Text").GetComponent<Text>();
            txtCostNum = transform.Find("Animator/Text_Num").GetComponent<Text>();
            
            btnReset = transform.Find("Animator/Btn_Reset").GetComponent<Button>();
            btnReset.onClick.AddListener(OnClickReset);
        }

        protected override void OnOpen(object arg)
        {
            paId = 0;
            if (arg != null)
                paId = (uint) arg;
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void OnClickClose()
        {
            this.CloseSelf();
        }

        private void OnClickReset()
        {
            if (costItemCount > totalItemCount)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014213));
                return;
            }
            
            Sys_Partner.Instance.OnReDistributePointReq(paId);
            this.CloseSelf();
        }

        private void UpdateInfo()
        {
            CSVParam.Data costPara = CSVParam.Instance.GetConfData(1501);
            string[] values = costPara.str_value.Split('|');
            costItemId = uint.Parse(values[0]);
            costItemCount = uint.Parse(values[1]);

            PropIconLoader.ShowItemData showitem = new PropIconLoader.ShowItemData(costItemId, 1, true, false, false, false, false, false, false, true);
            propItem.SetData(showitem, EUIID.UI_ExchangePointReset);
            CSVItem.Data costItem = CSVItem.Instance.GetConfData(costItemId);
            txtCostName.text = LanguageHelper.GetTextContent(costItem.name_id);

            totalItemCount = (uint)Sys_Bag.Instance.GetItemCount(costItemId);

            txtCostNum.text = string.Format("{0}/{1}", totalItemCount.ToString(), costItemCount.ToString());
        }

    }
}


