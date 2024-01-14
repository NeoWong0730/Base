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
    public class UI_ExchangePointChange : UIBase
    {
        private Button btnClose;
        private PropItem propItem;
        private Button btnMinus;
        private Button btnAdd;
        private Button btnMax;
        private Text txtNum;

        private Image imgCost;
        private Text txtCost;
        private Button btnTrans;

        private uint costItemId;
        private uint costItemRate;
        private uint costItemCount;
        private uint curNum;
        private uint maxNum; //最大数量

        protected override void OnLoaded()
        {
            btnClose = transform.Find("Animator/View_TipsBg02_Small/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);
            
            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("Animator/PropItem").gameObject);

            btnMinus = transform.Find("Animator/View_SelectNum/Btn_Min").GetComponent<Button>();
            btnMinus.onClick.AddListener(OnClickMinus);
            btnAdd = transform.Find("Animator/View_SelectNum/Btn_Add").GetComponent<Button>();
            btnAdd.onClick.AddListener(OnClickAdd);
            btnMax = transform.Find("Animator/View_SelectNum/Btn_Max").GetComponent<Button>();
            btnMax.onClick.AddListener(OnClickMax);
            
            txtNum = transform.Find("Animator/View_SelectNum/Btn_Num/Text").GetComponent<Text>();
            
            imgCost = transform.Find("Animator/Text_Consume/Image_Icon").GetComponent<Image>();
            txtCost = transform.Find("Animator/Text_Consume/Text_Value").GetComponent<Text>();
            
            btnTrans = transform.Find("Animator/Btn_Change").GetComponent<Button>();
            btnTrans.onClick.AddListener(OnClickTrans);
        }

        protected override void OnOpen(object arg)
        {

        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void OnClickClose()
        {
            this.CloseSelf();
        }

        private void OnClickMinus()
        {
            if (curNum <= 1)
                return;

            curNum--;
            RefreshTextCost();
        }

        private void OnClickAdd()
        {
            if (curNum >= maxNum)
            {
                if (curNum == 100)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014207));
                    return;
                }
                
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014214));
                return;
            }

            curNum++;
            RefreshTextCost();
        }

        private void OnClickMax()
        {
            if (maxNum == 0 || curNum == maxNum)
            {
                if (curNum == 100)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014207));
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014214));  
                }
            }
            curNum = maxNum == 0 ? 1 : maxNum;
            RefreshTextCost();
        }

        private void OnClickTrans()
        {
            Sys_Partner.Instance.ExchangePointReq(curNum);
            this.CloseSelf();
        }

        private void UpdateInfo()
        {
            // PropIconLoader.ShowItemData showitem = new PropIconLoader.ShowItemData(9, 1, true, false, false, false, false, false, false, false);
            // propItem.SetData(showitem, EUIID.UI_Trade_Search);
            curNum = 1;
            
            CSVParam.Data costPara = CSVParam.Instance.GetConfData(1502);
            string[] values = costPara.str_value.Split('|');
            costItemId = uint.Parse(values[0]);
            costItemRate = uint.Parse(values[1]);
            
            costItemCount = (uint)Sys_Bag.Instance.GetItemCount(costItemId);
            maxNum = costItemCount / costItemRate;
            maxNum = maxNum > 100 ? 100 : maxNum;

            CSVItem.Data costItem = CSVItem.Instance.GetConfData(costItemId);
            ImageHelper.SetIcon(imgCost, costItem.small_icon_id);

            RefreshTextCost();
        }

        private void RefreshTextCost()
        {
            uint valueNum = curNum * costItemRate;
            txtCost.text = valueNum.ToString();

            txtNum.text = curNum.ToString();
        }

    }
}


