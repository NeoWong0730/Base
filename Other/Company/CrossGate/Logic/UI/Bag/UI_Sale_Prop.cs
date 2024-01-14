using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;

namespace Logic
{
    public class UI_Sale_Prop : UIBase
    {
        private ItemData mItemData;

        private Image mIcon;
        private RawImage mImageBg;
        private Image quality;
        private Text mCount;
        private GameObject mNewObj;
        private GameObject mBoundObj;
        private GameObject mLine;
        private Image runeLevelImage;

        private Text mItemName;
        private Text mItemContent;
        private Text mItemContent_WorldView;
        private Text mItemLevel;
        private Text mItemType;
        private Text mItem_CanDeal;
        private Text mItem_Bind;

        private InputField mInputField;
        private Slider mSlider;
        private Button mMaxButton;

        private Button mAdd;
        private Button mSub;
        private Text mPerPrice;
        private Text mTotalPrice;

        private Button confire;
        private Button close;

        public int Count
        {
            get
            {
                if (mInputField.text == "")
                {
                    return 0;
                }
                else
                {
                    return int.Parse(mInputField.text);
                }
            }
        }

        private int MaxCountCanSale
        {
            get
            {
                return (int)mItemData.Count;
            }
        }

        protected override void OnOpen(object arg)
        {
            mItemData = arg as ItemData;
        }
        protected override void OnLoaded()
        {
            mMaxButton = transform.Find("Animator/View_Middle/Image_Sale_Number/Btn_Max").GetComponent<Button>();
            mSlider = transform.Find("Animator/View_Middle/Slider").GetComponent<Slider>();
            mItemName = transform.Find("Animator/View_Message/Text_Name").GetComponent<Text>();
            mIcon = transform.Find("Animator/View_Message/ListItem/Btn_Item/Image_Icon").GetComponent<Image>();
            mImageBg = transform.Find("Animator/View_Message/Image_QualityBG").GetComponent<RawImage>();
            quality = transform.Find("Animator/View_Message/ListItem/Btn_Item/Image_BG").GetComponent<Image>();
            mIcon.enabled = true;
            mCount = transform.Find("Animator/View_Message/ListItem/Text_Number").GetComponent<Text>();
            mNewObj = transform.Find("Animator/View_Message/ListItem/Text_New").gameObject;
            mBoundObj = transform.Find("Animator/View_Message/ListItem/Text_Bound").gameObject;
            mItemLevel = transform.Find("Animator/View_Message/Text_Level").GetComponent<Text>();
            mItemType = transform.Find("Animator/View_Message/Text_Type").GetComponent<Text>();
            mItem_CanDeal = transform.Find("Animator/View_Message/Text_Can_Deal").GetComponent<Text>();
            mItem_Bind = transform.Find("Animator/View_Message/Text_Bound").GetComponent<Text>();
            mItemContent = transform.Find("Animator/View_Message/Image_BG/Text_Ccontent2").GetComponent<Text>();
            mItemContent_WorldView = transform.Find("Animator/View_Message/Image_BG/Text_Ccontent").GetComponent<Text>();
            mInputField = transform.Find("Animator/View_Middle/Image_Sale_Number/InputField_Number").GetComponent<InputField>();
            mAdd = transform.Find("Animator/View_Middle/Image_Sale_Number/Btn_Add").GetComponent<Button>();
            mSub = transform.Find("Animator/View_Middle/Image_Sale_Number/Btn_Min").GetComponent<Button>();
            mPerPrice = transform.Find("Animator/View_Middle/Image_Get_Coin/Text_Number").GetComponent<Text>();
            mTotalPrice = transform.Find("Animator/View_Middle/Image_Price_Total/Text_Number").GetComponent<Text>();
            confire = transform.Find("Animator/View_Middle/Button_Sure").GetComponent<Button>();
            close = transform.Find("Animator/View_Middle/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            mLine = transform.Find("Animator/View_Message/Image_BG/Image_Line").gameObject;
            runeLevelImage = transform.Find("Animator/View_Message/ListItem/Image_RuneRank")?.GetComponent<Image>();
            mAdd.onClick.AddListener(Add);
            mSub.onClick.AddListener(Sub);
            confire.onClick.AddListener(OnConfrie);
            close.onClick.AddListener(Close);
            mMaxButton.onClick.AddListener(OnMaxButtonClick);
            mInputField.onValueChanged.AddListener(str =>
            {
                int count;
                try
                {
                    count = int.Parse(str);
                    count = Mathf.Clamp(count, 0, MaxCountCanSale);
                    mInputField.text = count.ToString();
                    mTotalPrice.text = (mItemData.cSVItemData.sell_price * count).ToString();
                    UpdateSlider(count);
                }
                catch (System.Exception)
                {
                    DebugUtil.Log(ELogType.eBag, "格式不正确");
                }
            });
            mSlider.wholeNumbers = true;
            mSlider.maxValue = MaxCountCanSale;
            mSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        protected override void OnShow()
        {
            InitUi();
            UpdateInfoUi();
        }

        private void InitUi()
        {
            int count = MaxCountCanSale;
            mInputField.text = count.ToString();
            //mInputField.text = "1";
            //int count = int.Parse(mInputField.text);
            mTotalPrice.text = (mItemData.cSVItemData.sell_price * count).ToString();
        }

        private void OnSliderValueChanged(float val)
        {
            //int count = Mathf.CeilToInt(val * MaxInputCount);
            mInputField.text = val.ToString();
        }

        private void UpdateSlider(int curCount)
        {
            float val = 0;
            if (MaxCountCanSale == 0)
            {
                val = 0;
            }
            else
            {
                val = curCount;
            }
            mSlider.value = val;
        }

        private void UpdateInfoUi()
        {
            ImageHelper.SetIcon(mIcon, mItemData.cSVItemData.icon_id);
            ImageHelper.GetQualityColor_Frame(quality, (int)mItemData.Quality);
            mNewObj.SetActive(mItemData.bNew);
            mBoundObj.SetActive(mItemData.bBind);
            if (mItemData.Count > 1)
            {
                mCount.text = mItemData.Count.ToString();
            }
            else
            {
                mCount.text = "";
            }
            TextHelper.SetQuailtyText(mItemName, (uint)mItemData.Quality, CSVLanguage.Instance.GetConfData(mItemData.cSVItemData.name_id).words);
            ImageHelper.SetBgQuality(mImageBg, mItemData.Quality);
            TextHelper.SetText(mItemLevel, string.Format(CSVLanguage.Instance.GetConfData(2007412).words, CSVItem.Instance.GetConfData(mItemData.Id).lv));
            
            mPerPrice.text = mItemData.cSVItemData.sell_price.ToString();

            mLine.SetActive(mItemData.cSVItemData.world_view != 0 && mItemData.cSVItemData.describe_id != 0);
            if (mItemData.cSVItemData.world_view != 0)
            {
                TextHelper.SetText(mItemContent_WorldView, mItemData.cSVItemData.world_view);
                mItemContent_WorldView.gameObject.SetActive(true);
            }
            else
            {
                TextHelper.SetText(mItemContent_WorldView, string.Empty);
                mItemContent_WorldView.gameObject.SetActive(false);
            }
            if (mItemData.cSVItemData.describe_id != 0)
            {
                TextHelper.SetText(mItemContent, mItemData.cSVItemData.describe_id);
                mItemContent.gameObject.SetActive(true);
            }
            else
            {
                TextHelper.SetText(mItemContent, string.Empty);
                mItemContent.gameObject.SetActive(false);
            }
            RefreshPartner();
        }

        public void RefreshPartner()
        {
            CSVRuneInfo.Data runeInfo = CSVRuneInfo.Instance.GetConfData((uint)mItemData.cSVItemData.id);
            if (null != runeInfo)
            {
                if (runeLevelImage != null)
                {
                    ImageHelper.SetIcon(runeLevelImage, Sys_Partner.Instance.GetRuneLevelImageId(runeInfo.rune_lvl));
                    runeLevelImage.gameObject.SetActive(true);
                }
            }
            else
            {
                runeLevelImage.gameObject.SetActive(false);
            }
        }

        private void Add()
        {
            int count = Count;
            count++;
            count = Mathf.Clamp(count, 0, MaxCountCanSale);
            mInputField.text = count.ToString();
            UpdateSlider(count);
        }

        private void Sub()
        {
            int count = Count;
            count--;
            count = Mathf.Clamp(count, 0, MaxCountCanSale);
            mInputField.text = count.ToString();
            UpdateSlider(count);
        }

        private void OnConfrie()
        {
            try
            {
                int count = int.Parse(mInputField.text);
            }
            catch (System.Exception)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000910));
                return;
            }
            UIManager.CloseUI(EUIID.UI_Sale_Prop);
            Sys_Bag.Instance.SaleItem(mItemData.Uuid, (uint)Count);
        }

        private void Close()
        {
            UIManager.CloseUI(EUIID.UI_Sale_Prop);
        }

        private void OnMaxButtonClick()
        {
            int count = MaxCountCanSale;
            mInputField.text = count.ToString();
            UpdateSlider(count);
        }

    }
}


