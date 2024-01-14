using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using System;

namespace Logic
{
    public class UI_BatchUse_Box : UIBase
    {
        private PropIconLoader.ShowItemData mPropIconLoaderItemData;
        private ItemData mItemData;
        private PropItem mPropIconWrap;
        private Text mItemName;
        private Text mItemCount;
        private Button useBtn;
        private Slider mSlider;
        private InputField mInputField;
        private Button mAdd;
        private Button mSub;
        private Button mCloseBtn;
        private int itemCount;
        private int MaxCount
        {

            get
            {
                if (mItemData == null)
                {
                    return itemCount;
                }
                int batchUse = (int)mItemData.cSVItemData.batch_use;
                //int bagCount= Sys_Bag.Instance.GetItemMaxCountExceptSafeBox(mItemData.Id);
                //int maxCount = batchUse <= bagCount ? batchUse : bagCount;
                //return maxCount;
                return Mathf.Min(batchUse, (int)mItemData.Count);
            }
        }

        protected override void OnOpen(object arg)
        {
            mItemData = arg as ItemData;
            if (mItemData != null)
            {
                itemCount = (int)mItemData.Count;
            }
        }

        protected override void OnLoaded()
        {

            mItemName = transform.Find("Animator/View_Choose_Award/Text_Name").GetComponent<Text>();
            mItemCount = transform.Find("Animator/View_Choose_Award/Text_Count").GetComponent<Text>();

            mAdd = transform.Find("Animator/View_Choose_Award/Button_Add").GetComponent<Button>();
            mAdd.onClick.AddListener(Add);

            mSub = transform.Find("Animator/View_Choose_Award/Button_Sub").GetComponent<Button>();
            mSub.onClick.AddListener(Sub);

            useBtn = transform.Find("Animator/View_Choose_Award/Button_Confirm").GetComponent<Button>();
            useBtn.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_BatchUse_Box);
                uint count = 0;
                try
                {
                    count = uint.Parse(mInputField.text);
                }
                catch (System.Exception)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000910));
                    return;
                }
                if (mItemData.cSVItemData.fun_parameter == "addOptionalBap")
                {
                    UIManager.OpenUI(EUIID.UI_OptionalGift, false, new Tuple<ItemData, uint>(mItemData, count));
                }
                else
                {
                    Sys_Bag.Instance.UsetItem(mItemData.Id, mItemData.Uuid, count);
                }
            });

            mSlider = transform.Find("Animator/View_Choose_Award/Slider").GetComponent<Slider>();
            mSlider.onValueChanged.AddListener(OnSliderValueChanged);
            mSlider.wholeNumbers = true;
            mSlider.maxValue = MaxCount;

            mCloseBtn = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            mCloseBtn.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_BatchUse_Box);
            });

            mInputField = transform.Find("Animator/View_Choose_Award/InputField_Number").GetComponent<InputField>();
            mInputField.onValueChanged.AddListener(str =>
            {
                int count = int.Parse(str);
                count = Mathf.Clamp(count, 1, MaxCount);
                mInputField.text = count.ToString();
                UpdateSlider(count);
            });

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("ClickClose").gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (_) => { UIManager.CloseUI(EUIID.UI_BatchUse_Box); });
        }

        protected override void OnShow()
        {
            int count = MaxCount;
            mInputField.text = count.ToString();
            UpdateSlider(count);
            UpdateInfoUi();
            mPropIconWrap = new PropItem();
            mPropIconWrap.BindGameObject(transform.Find("Animator/View_Choose_Award/ListItem").gameObject);
            mPropIconWrap.SetData(new MessageBoxEvt(EUIID.UI_BatchUse_Box, new PropIconLoader.ShowItemData(mItemData.Id, mItemData.Count, true, mItemData.bBind, mItemData.bNew, false, false)));
        }

        private void UpdateInfoUi()
        {
            TextHelper.SetText(mItemName, CSVItem.Instance.GetConfData(mItemData.Id).name_id);
            mItemCount.text = MaxCount.ToString();
        }

        private void Add()
        {
            int count = 0;
            if (mInputField.text == string.Empty)
            {
                count = 0;
            }
            else
            {
                count = int.Parse(mInputField.text);
            }
            count++;
            count = Mathf.Clamp(count, 1, MaxCount);
            mInputField.text = count.ToString();
            UpdateSlider(count);
        }

        private void Sub()
        {
            int count = 0;
            if (mInputField.text == string.Empty)
            {
                count = 0;
            }
            else
            {
                count = int.Parse(mInputField.text);
            }
            count--;
            count = Mathf.Clamp(count, 1, MaxCount);
            mInputField.text = count.ToString();
            UpdateSlider(count);
        }

        private void UpdateSlider(float curCount)
        {
            //float val = (float)(curCount - 1) / (float)(MaxCount - 1);
            if (curCount < 1)
            {
                curCount = 1;
            }
            mSlider.value = curCount;
        }

        private void OnSliderValueChanged(float val)
        {
            //float rate = val / 1;
            //int count = Mathf.CeilToInt(rate * MaxCount);
            //count = Mathf.Max(1, count);
            if (val < 1)
            {
                val = 1;
                mSlider.value = 1;
            }
            mInputField.text = val.ToString();
        }
    }
}

