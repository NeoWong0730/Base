using Logic.Core;
using UnityEngine.UI;
using UnityEngine;
using System;
using Table;

namespace Logic
{
    public class UI_Transfiguration_Tips : UIBase
    {
        private Text tip;
        private Text copyTip;
        private Text newTip;
        private Text copyCost;
        private Text newCost;
        private Image copyCoin;
        private Image newCoin;
        private Button copyBtn;
        private Button newBtn;
        private Button closeBtn;

        private int limit;
        private uint copyItemID;
        private int copyItemCount;
        private uint newItemID;
        private int newItemCount;

        protected override void OnLoaded()
        {
            tip = transform.Find("Animator/Text_Tip").GetComponent<Text>();
            copyTip = transform.Find("Animator/Text_Tip (1)").GetComponent<Text>();
            newTip = transform.Find("Animator/Text_Tip (2)").GetComponent<Text>();
            copyCost = transform.Find("Animator/Cost_Coin/Text_Cost").GetComponent<Text>();
            newCost = transform.Find("Animator/Cost_Coin1/Text_Cost").GetComponent<Text>();
            copyCoin = transform.Find("Animator/Cost_Coin").GetComponent<Image>();
            newCoin = transform.Find("Animator/Cost_Coin1").GetComponent<Image>();
            copyBtn = transform.Find("Animator/Buttons/Button_Copy").GetComponent<Button>();
            newBtn = transform.Find("Animator/Buttons/Button_Rewash").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();
            copyBtn.onClick.AddListener(OnCopyButtonClicked);
            newBtn.onClick.AddListener(OnNewButtonClicked);
            closeBtn.onClick.AddListener(OnCloseButtonClicked);
        }

        protected override void OnShow()
        {
            bool valid = Sys_Ini.Instance.Get<IniElement_IntArray>(1436, out var rlt) && rlt.value.Length >= 6;
            if (valid)
            {
                limit = rlt.value[1];
                copyItemID = (uint)rlt.value[2];
                copyItemCount = rlt.value[3];
                newItemID = (uint)rlt.value[4];
                newItemCount = rlt.value[5];
                ImageHelper.SetIcon(copyCoin, CSVItem.Instance.GetConfData(copyItemID).icon_id);
                copyCost.text = copyItemCount.ToString();
                ImageHelper.SetIcon(newCoin, CSVItem.Instance.GetConfData(newItemID).icon_id);
                newCost.text = newItemCount.ToString();
            }
            TextHelper.SetText(tip,LanguageHelper.GetTextContent(10013903));
            TextHelper.SetText(copyTip, LanguageHelper.GetTextContent(10013904));
            TextHelper.SetText(newTip, LanguageHelper.GetTextContent(10013905));
        }

        protected override void OnHide()
        {
        }

        private bool CheckLimit()
        {
            if (Sys_Transfiguration.Instance.clientShapeShiftPlans.Count >= limit)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013706));
                return false;
            }
            else
            {
                return true;
            }
        }

        private void OnCopyButtonClicked()
        {
            if (CheckLimit())
            {
                if (Sys_Bag.Instance.GetItemCount(copyItemID) >= copyItemCount)
                {
                    Sys_Transfiguration.Instance.AddPlanReq(true);
                    UIManager.CloseUI(EUIID.UI_Transfiguration_Tips);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101918));
                }
            }
        }

        private void OnNewButtonClicked()
        {
            if (CheckLimit())
            {
                if (Sys_Bag.Instance.GetItemCount(newItemID) >= newItemCount)
                {
                    Sys_Transfiguration.Instance.AddPlanReq(false);
                    UIManager.CloseUI(EUIID.UI_Transfiguration_Tips);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101918));
                }
            }
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Transfiguration_Tips);
        }
    }
}
