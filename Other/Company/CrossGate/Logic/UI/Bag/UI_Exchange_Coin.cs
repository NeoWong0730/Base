using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;

namespace Logic
{

    public class ExchangeCoinParm
    {
        public uint ExchangeType;
        public long needCount;
    }

    public class UI_Exchange_Coin : UIBase
    {
        private InputField mInputField;
        private Text mTitle;
        private Text mTexthold_pre;
        private Image mholdImg;
        private Image mgetImg;
        private Text mTextGet_pre;
        private Text mTexthold;
        private Text mTextGet;
        private Text mText_tip;
        private Text m_SourceTitle;
        private Text m_SourceDes;
        private Button mCloseBtn;
        private Button mOkBtn;
        private Button mAdd;
        private Button mSub;
        private Slider mSlider;
        private ExchangeCoinParm m_ExchangeCoinParm;
        private long needCount;             //需要兑换的数量
        private long needFromCount;         //需要扣除的数量
        private uint ExchangeType = 0;     // 兑换的是什么(需要用上一个货币兑换)   1:钻石   2:金币   3：银币
        private List<int> rates = new List<int>();
        private int rate;
        private long exchangeCount = 0;
        public long Count
        {
            get
            {
                if (mInputField.text == string.Empty)
                {
                    return 0;
                }
                else
                {
                    return long.Parse(mInputField.text);
                }
            }
        }

        private float addtimer = 0f;
        private float subtimer = 0f;
        private long MaxInputConfigCount;
        private long BagItemCount;
        private long MaxInputCount;

        private GameObject m_ToggleRoot;
        private GameObject m_ToggleGo;
        private CP_ToggleRegistry m_CP_ToggleRegistry;
        private uint m_ExchangeByType;//用什么货币兑换  0:魔币  1：金币

        protected override void OnOpen(object arg)
        {
            m_ExchangeCoinParm = arg as ExchangeCoinParm;
            if (m_ExchangeCoinParm != null)
            {
                ExchangeType = m_ExchangeCoinParm.ExchangeType;
                needCount = m_ExchangeCoinParm.needCount;
            }
            rates.Clear();
            Sys_Ini.Instance.Get<IniElement_IntArray>(241, out IniElement_IntArray array);
            int[] ratesArray = array.value;
            for (int i = 0; i < ratesArray.Length; i++)
            {
                rates.Add(ratesArray[i]);
            }
            rate = rates[(int)ExchangeType - 2];
            string parm = CSVParam.Instance.GetConfData(240).str_value;
            MaxInputConfigCount = long.Parse(parm);
            if (ExchangeType == 2)
            {
                BagItemCount = Sys_Bag.Instance.GetItemCount(ExchangeType - 1);
                if (needCount > 0)
                {
                    if (needCount % 100 > 0)
                    {
                        needFromCount = needCount / 100 + 1;
                    }
                    else
                    {
                        needFromCount = needCount / 100;
                    }
                }
            }
            else if (ExchangeType == 3)
            {
                BagItemCount = Sys_Bag.Instance.GetItemCount(ExchangeType - 1);
                if (BagItemCount > 0)
                {
                    m_ExchangeByType = 1;
                    if (needCount > 0)
                    {
                        if (needCount % 100 > 0)
                        {
                            needFromCount = needCount / 100 + 1;
                        }
                        else
                        {
                            needFromCount = needCount / 100;
                        }
                    }
                }
                else
                {
                    BagItemCount = Sys_Bag.Instance.GetItemCount(ExchangeType - 2);
                    if (BagItemCount > 0)
                    {
                        m_ExchangeByType = 0;
                        if (needCount > 0)
                        {
                            if (needCount % 10000 > 0)
                            {
                                needFromCount = needCount / 10000 + 1;
                            }
                            else
                            {
                                needFromCount = needCount / 10000;
                            }
                        }
                    }
                }
            }
            MaxInputCount = (long)Mathf.Min(BagItemCount, MaxInputConfigCount);
        }

        protected override void OnLoaded()
        {
            mTitle = transform.Find("Animator/View_TipsBg02_Smallest/Text_Title").GetComponent<Text>();
            m_SourceTitle = transform.Find("Animator/View_Tips/Text_Title").GetComponent<Text>();
            m_SourceDes = transform.Find("Animator/View_Tips/Grid/Text").GetComponent<Text>();
            mholdImg = transform.Find("Animator/View_List/View2/Image_My_Diamonds/Image_BG/Image_Icon").GetComponent<Image>();
            mgetImg = transform.Find("Animator/View_List/View4/Image_Get_Coin/Image_BG/Image_Icon").GetComponent<Image>();
            mTexthold_pre = transform.Find("Animator/View_List/View2/Image_My_Diamonds/Image_BG/Text_Title").GetComponent<Text>();
            mTextGet_pre = transform.Find("Animator/View_List/View4/Image_Get_Coin/Image_BG/Text_Title").GetComponent<Text>();
            mTexthold = transform.Find("Animator/View_List/View2/Image_My_Diamonds/Text_Number").GetComponent<Text>();
            mTextGet = transform.Find("Animator/View_List/View4/Image_Get_Coin/Text_Number").GetComponent<Text>();
            mText_tip = transform.Find("Animator/View_List/Text_Tips").GetComponent<Text>();
            mInputField = transform.Find("Animator/View_List/View3/Image_Exchange_Number/InputField_Number").GetComponent<InputField>();
            mCloseBtn = transform.Find("Animator/View_TipsBg02_Smallest/Btn_Close").GetComponent<Button>();
            mOkBtn = transform.Find("Animator/View_List/Button_Sure").GetComponent<Button>();
            mAdd = transform.Find("Animator/View_List/View3/Image_Exchange_Number/Btn_Add").GetComponent<Button>();
            mSub = transform.Find("Animator/View_List/View3/Image_Exchange_Number/Btn_Min").GetComponent<Button>();
            mSlider = transform.Find("Animator/View_List/View3/Image_Exchange_Number/Slider").GetComponent<Slider>();
            m_ToggleRoot = transform.Find("Animator/View_List/View1").gameObject;
            m_ToggleGo = transform.Find("Animator/View_List/View1/Image_Select/Image_BG/ToggleGroup_4").gameObject;
            m_CP_ToggleRegistry = m_ToggleGo.GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegistry.onToggleChange = OnToggleChanged;

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(mAdd);
            eventListener.AddEventListener(EventTriggerType.PointerDown, OnAddPointDown);
            eventListener.AddEventListener(EventTriggerType.PointerUp, OnAddPointUp);

            Lib.Core.EventTrigger eventListener1 = Lib.Core.EventTrigger.Get(mSub);
            eventListener1.AddEventListener(EventTriggerType.PointerDown, OnSubPointDown);
            eventListener1.AddEventListener(EventTriggerType.PointerUp, OnSubPointUp);

            mSlider.onValueChanged.AddListener(OnSliderValueChanged);

            mCloseBtn.onClick.AddListener(OnCloseBtnClicked);
            mOkBtn.onClick.AddListener(OnOkBtnClicked);
            mInputField.onValueChanged.AddListener(str =>
            {
                long count;
                if (str == string.Empty)
                {
                    count = 0;
                }
                else
                {
                    count = long.Parse(str);
                }
                if (count > BagItemCount)
                {
                    //string currencytype = string.Empty;
                    //if (ExchangeType == 2)
                    //{
                    //    currencytype = CSVLanguage.Instance.GetConfData(1000931).words;
                    //}
                    //else if (ExchangeType == 3)
                    //{
                    //    currencytype = CSVLanguage.Instance.GetConfData(1000932).words;
                    //}
                    //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000933, currencytype));
                    count = BagItemCount;
                }
                count = (long)Mathf.Clamp(count, 0, MaxInputConfigCount);

                mInputField.text = count.ToString();
                UpdateSlider((int)count);
                exchangeCount = rate * count;
                mTextGet.text = exchangeCount.ToString();
            });
            mSlider.wholeNumbers = true;
            if (MaxInputCount < 0)
            {
                MaxInputCount = 0;
            }
            mSlider.maxValue = MaxInputCount;
        }

        protected override void OnShow()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            m_ToggleRoot.SetActive(ExchangeType == 3);
            if (ExchangeType == 3)
            {
                m_CP_ToggleRegistry.SwitchTo((int)m_ExchangeByType);
            }
            mInputField.text = needFromCount.ToString();
            mSlider.value = needFromCount;
            uint lanId = 0;
            uint lanId_Des = 0;
            if (ExchangeType == 2)
            {
                lanId_Des = 1012012;
                TextHelper.SetText(mTitle, LanguageHelper.GetTextContent(1000980));
                TextHelper.SetText(mTexthold_pre, 1000912);
                TextHelper.SetText(mTextGet_pre, 1000914);
                mTexthold.text = Sys_Bag.Instance.GetItemCount((ExchangeType - 1)).ToString();
                ImageHelper.SetIcon(mholdImg, CSVItem.Instance.GetConfData(ExchangeType - 1).small_icon_id);
                ImageHelper.SetIcon(mgetImg, CSVItem.Instance.GetConfData(ExchangeType).small_icon_id);
                lanId = 1000911;        //{0}魔币={1}金币      
                TextHelper.SetText(mText_tip, LanguageHelper.GetTextContent(lanId, 1.ToString(), rate.ToString(), CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(ExchangeType).name_id).words));
            }
            else if (ExchangeType == 3)
            {
                lanId_Des = 1012011;
                TextHelper.SetText(mTitle, LanguageHelper.GetTextContent(1000981));
                OnUpdateSliverInfo();
            }
            TextHelper.SetText(m_SourceTitle, LanguageHelper.GetTextContent(2024601, LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData((ExchangeType)).name_id)));
            TextHelper.SetText(m_SourceDes, lanId_Des);
        }

        private void UpdateSlider(int curCount)
        {
            float val = 0;
            if (MaxInputCount == 0)
            {
                val = 0;
            }
            else
            {
                val = curCount;
            }
            mSlider.value = val;
        }

        private void OnSliderValueChanged(float val)
        {
            mInputField.text = val.ToString();
        }

        private void OnToggleChanged(int current, int old)
        {
            if (current == old)
            {
                return;
            }
            m_ExchangeByType = (uint)current;
            OnUpdateSliverInfo();
            mInputField.text = needFromCount.ToString();
            mSlider.value = needFromCount;
        }

        private void OnUpdateSliverInfo()
        {
            uint lanId = 0;
            if (m_ExchangeByType == 0)//用魔币兑换  银币
            {
                TextHelper.SetText(mTexthold_pre, 1000912);
                TextHelper.SetText(mTextGet_pre, 1000919);
                mTexthold.text = Sys_Bag.Instance.GetItemCount((ExchangeType - 2)).ToString();
                ImageHelper.SetIcon(mholdImg, CSVItem.Instance.GetConfData(ExchangeType - 2).small_icon_id);
                ImageHelper.SetIcon(mgetImg, CSVItem.Instance.GetConfData(ExchangeType).small_icon_id);
                rate = rates[2];
                lanId = 1001000;            //{0}魔币={1}银币      
                BagItemCount = Sys_Bag.Instance.GetItemCount(ExchangeType - 2);
                MaxInputCount = (long)Mathf.Min(BagItemCount, MaxInputConfigCount);
                if (MaxInputCount < 0)
                {
                    MaxInputCount = 0;
                }
                mSlider.maxValue = MaxInputCount;
                if (needCount > 0)
                {
                    if (needCount % 10000 > 0)
                    {
                        needFromCount = needCount / 10000 + 1;
                    }
                    else
                    {
                        needFromCount = needCount / 10000;
                    }
                    needFromCount = (long)Mathf.Min(needFromCount, BagItemCount);
                }
            }
            else if (m_ExchangeByType == 1)//用金币兑换  银币
            {
                TextHelper.SetText(mTexthold_pre, 1000917);
                TextHelper.SetText(mTextGet_pre, 1000919);
                mTexthold.text = Sys_Bag.Instance.GetItemCount((ExchangeType - 1)).ToString();
                ImageHelper.SetIcon(mholdImg, CSVItem.Instance.GetConfData(ExchangeType - 1).small_icon_id);
                ImageHelper.SetIcon(mgetImg, CSVItem.Instance.GetConfData(ExchangeType).small_icon_id);
                rate = rates[1];
                lanId = 1000916;        //{0}金币={1}银币                                                                                                    
                BagItemCount = Sys_Bag.Instance.GetItemCount(ExchangeType - 1);
                MaxInputCount = (long)Mathf.Min(BagItemCount, MaxInputConfigCount);
                if (MaxInputCount < 0)
                {
                    MaxInputCount = 0;
                }
                mSlider.maxValue = MaxInputCount;
                if (needCount > 0)
                {
                    if (needCount % 100 > 0)
                    {
                        needFromCount = needCount / 100 + 1;
                    }
                    else
                    {
                        needFromCount = needCount / 100;
                    }
                }
            }
            TextHelper.SetText(mText_tip, LanguageHelper.GetTextContent(lanId, 1.ToString(), rate.ToString(), CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(ExchangeType).name_id).words));
        }

        private void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Exchange_Coin);
        }

        private void OnOkBtnClicked()
        {
            try
            {
                int count = int.Parse(mInputField.text);
                if (count == 0)
                {
                    string content = CSVLanguage.Instance.GetConfData(1000965).words;
                    Sys_Hint.Instance.PushContent_Normal(content);
                    return;
                }
                else if (count > BagItemCount)
                {
                    string currencytype = string.Empty;
                    if (m_ExchangeByType == 0)
                    {
                        currencytype = CSVLanguage.Instance.GetConfData(1000931).words;
                    }
                    else if (m_ExchangeByType == 1)
                    {
                        currencytype = CSVLanguage.Instance.GetConfData(1000932).words;
                    }
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000933, currencytype));
                }
                else
                {
                    Sys_Bag.Instance.ItemExchangeCurrencyReq((int)ExchangeType, (int)m_ExchangeByType, count, exchangeCount);
                }
            }
            catch (System.Exception)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000910));
                return;
            }
            UIManager.CloseUI(EUIID.UI_Exchange_Coin);
        }

        private void Add()
        {
            long count = Count;
            count++;
            count = (long)Mathf.Clamp(count, 0, MaxInputCount);
            UpdateSlider((int)count);
            mInputField.text = count.ToString();
        }

        bool add;
        bool sub;
        private void OnAddPointDown(BaseEventData baseEventData)
        {
            add = true;
        }
        private void OnAddPointUp(BaseEventData baseEventData)
        {
            add = false;
        }
        private void OnSubPointUp(BaseEventData baseEventData)
        {
            sub = false;
        }
        private void OnSubPointDown(BaseEventData baseEventData)
        {
            sub = true;
        }

        private void Sub()
        {
            long count = Count;
            count--;
            count = (long)Mathf.Clamp(count, 0, MaxInputCount);
            UpdateSlider((int)count);
            mInputField.text = count.ToString();
        }

        bool addflag = false;
        bool subflag = false;

        protected override void OnLateUpdate(float dt, float usdt)
        {
            if (add)
            {
                addtimer += deltaTime;
                if (!addflag)
                {
                    Add();
                    addflag = true;
                }
                if (addtimer >= 0.7f)
                {
                    Add();
                }
            }
            else
            {
                addflag = false;
                addtimer = 0;
            }
            if (sub)
            {
                subtimer += deltaTime;
                if (!subflag)
                {
                    Sub();
                    subflag = true;
                }
                if (subtimer >= 0.7f)
                {
                    Sub();
                }
            }
            else
            {
                subflag = false;
                subtimer = 0;
            }
        }

    }
}


