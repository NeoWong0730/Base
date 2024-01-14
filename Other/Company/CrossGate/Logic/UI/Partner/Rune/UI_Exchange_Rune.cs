using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Exchange_Rune : UIBase
    {
        private Image icon;
        private InputField input;
        private Button btnClose;
        private Button btnSub;
        private Button btnAdd;
        private Button btnMax;
        private Button btnSure;

        private Image onceImage;
        private Image coutImage;
        private Text onceText;
        private Text coutText;

        private int DefaultComposeCount = 1;
        private int curComposeCount = 0;

        private CSVRuneSynthetise.Data composeBaseData;
        protected override void OnLoaded()
        {
            icon = transform.Find("Animator/View_List/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            onceImage = transform.Find("Animator/View_List/Image_My_Diamonds/Image_BG/Image_Icon").GetComponent<Image>();
            onceText = transform.Find("Animator/View_List/Image_My_Diamonds/Text_Number").GetComponent<Text>();

            coutImage = transform.Find("Animator/View_List/Image_Get_Coin/Image_BG/Image_Icon").GetComponent<Image>();
            coutText = transform.Find("Animator/View_List/Image_Get_Coin/Text_Number").GetComponent<Text>();

            btnClose = transform.Find("Animator/View_TipsBg02_Smallest/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(() => { CloseSelf(); });

            input = transform.Find("Animator/View_List/Image_Exchange_Number/InputField_Number").GetComponent<InputField>();
            input.contentType = InputField.ContentType.IntegerNumber;
            input.keyboardType = TouchScreenKeyboardType.NumberPad;
            input.onEndEdit.AddListener(OnInputEnd);

            btnSub = transform.Find("Animator/View_List/Image_Exchange_Number/Btn_Min").GetComponent<Button>();
            UI_LongPressButton LongPressSubButton = btnSub.gameObject.AddComponent<UI_LongPressButton>();
            LongPressSubButton.interval = 0.3f;
            LongPressSubButton.bPressAcc = true;
            LongPressSubButton.onRelease.AddListener(OnClickBtnSub);
            LongPressSubButton.OnPressAcc.AddListener(OnClickBtnSub);


            btnAdd = transform.Find("Animator/View_List/Image_Exchange_Number/Btn_Add").GetComponent<Button>();
            UI_LongPressButton LongPressAddButton = btnAdd.gameObject.AddComponent<UI_LongPressButton>();
            LongPressAddButton.interval = 0.3f;
            LongPressAddButton.bPressAcc = true;
            LongPressAddButton.onRelease.AddListener(OnClickBtnAdd);
            LongPressAddButton.OnPressAcc.AddListener(OnClickBtnAdd);

            btnMax = transform.Find("Animator/View_List/Image_Exchange_Number/Btn_Max").GetComponent<Button>();
            btnMax.onClick.AddListener(OnClickBtnMax);

            btnSure = transform.Find("Animator/View_List/Button_Sure").GetComponent<Button>();
            btnSure.onClick.AddListener(OnClickBtnSure);
        }

        protected override void OnOpen(object arg)
        {
            composeBaseData = arg as CSVRuneSynthetise.Data;
        }

        protected override void OnShow()
        {
            if (null != composeBaseData && composeBaseData.synthetise_expend.Count >= 2)
            {
                ImageHelper.SetIcon(icon, composeBaseData.baseIcon);
                icon.enabled = true;
                CSVItem.Data resource = CSVItem.Instance.GetConfData(composeBaseData.synthetise_expend[0]);
                ImageHelper.SetIcon(onceImage, resource.icon_id);
                ImageHelper.SetIcon(coutImage, resource.icon_id);
                TextHelper.SetText(onceText, composeBaseData.synthetise_expend[1].ToString());
            }
            curComposeCount = DefaultComposeCount;
            input.text = curComposeCount.ToString();
            UpdateCostPirce();
        }

        private void OnInputEnd(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                int result = int.Parse(s);
                if (result <= 0)
                {
                    curComposeCount = DefaultComposeCount;
                }
                else
                {
                    int maxCount = GetComposeCount();
                    curComposeCount = result > maxCount ? maxCount : result;
                }
            }
            else
            {
                curComposeCount = DefaultComposeCount;
            }

            input.text = curComposeCount.ToString();
            UpdateCostPirce();
        }

        private void UpdateCostPirce()
        {
            if (null != composeBaseData && composeBaseData.synthetise_expend.Count >= 2)
            {
                uint price = composeBaseData.synthetise_expend[1];
                bool isEnough = Sys_Bag.Instance.GetItemCount(composeBaseData.synthetise_expend[0]) >= price * curComposeCount;
                uint lanId = isEnough ? (uint)2007203 : (uint)2007204;
                TextHelper.SetText(coutText, lanId, (price * curComposeCount).ToString());
            }
        }

        private int GetComposeCount()
        {
            if (composeBaseData != null)
            {
                if (composeBaseData.synthetise_expend.Count >= 2)
                {
                    long count = Sys_Bag.Instance.GetItemCount(composeBaseData.synthetise_expend[0]);
                    int max = (int)count / (int)composeBaseData.synthetise_expend[1];
                    return max > composeBaseData.synthetise_maxnum ? (int)composeBaseData.synthetise_maxnum : max;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                return 0;
            }
        }

        private void OnClickBtnSub()
        {
            if (curComposeCount > 1)
            {
                curComposeCount--;
            }

            input.text = curComposeCount.ToString();
            UpdateCostPirce();
        }

        private void OnClickBtnAdd()
        {
            int maxCount = GetComposeCount();
            if (curComposeCount < maxCount)
            {
                curComposeCount++;
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006187));
                return;
            }
            input.text = curComposeCount.ToString();
            UpdateCostPirce();
        }

        private void OnClickBtnMax()
        {
            int maxCount = GetComposeCount();
            if(curComposeCount < maxCount)
            {
                curComposeCount = maxCount;
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006187));
                return;
            }
            input.text = curComposeCount.ToString();
            UpdateCostPirce();
        }

        private void OnClickBtnSure()
        {
            if (null != composeBaseData)
            {
                Sys_Partner.Instance.PartnerRuneComposeReq(composeBaseData.id, (uint)curComposeCount);
                CloseSelf();
            }
        }
    }
}
