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
    public class UI_Decompose : UIBase
    {
        private ItemData mItemData;
        private Image imgSkillBook;
        private InputField mInputField;
        private Image icon;
        private Image imgQuality;
        private Text itemName;
        private Text itemNum;
        private Button mCloseBtn;
        private Button mOkBtn;
        private Button mAdd;
        private Button mSub;
        private Button mButton;
        private Slider mSlider;
        private Transform undoParent;
        private float addtimer = 0f;
        private float subtimer = 0f;

        private uint MaxInputCount;

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
        private List<Button> buttons = new List<Button>();

        protected override void OnOpen(object arg)
        {
            mItemData = arg as ItemData;
            MaxInputCount = mItemData.Count;
        }

        protected override void OnLoaded()
        {
            mButton = transform.Find("Animator/Item/Btn_Item").GetComponent<Button>();
            imgQuality = transform.Find("Animator/Item/Btn_Item/Image_BG").GetComponent<Image>();
            icon = transform.Find("Animator/Item/Btn_Item/Image_Icon").GetComponent<Image>();
            imgSkillBook = transform.Find("Animator/Item/Btn_Item/Image_Skill").GetComponent<Image>();
            itemNum = transform.Find("Animator/Item/Text_Number").GetComponent<Text>();
            itemName = transform.Find("Animator/Text_Name").GetComponent<Text>();
            mCloseBtn = transform.Find("View_TipsBg01_Square/Btn_Close").GetComponent<Button>();
            mOkBtn = transform.Find("Animator/Btn_01").GetComponent<Button>();
            mSlider = transform.Find("Animator/Image_Exchange_Number/Slider").GetComponent<Slider>();

            mAdd = transform.Find("Animator/Image_Exchange_Number/Btn_Add").GetComponent<Button>();
            UI_LongPressButton LongPressAddButton = mAdd.gameObject.AddComponent<UI_LongPressButton>();
            LongPressAddButton.interval = 0.3f;
            LongPressAddButton.bPressAcc = true;
            LongPressAddButton.onRelease.AddListener(Add);
            LongPressAddButton.OnPressAcc.AddListener(Add);
            
            mSub = transform.Find("Animator/Image_Exchange_Number/Btn_Min").GetComponent<Button>();
            UI_LongPressButton LongPressSubButton = mSub.gameObject.AddComponent<UI_LongPressButton>();
            LongPressSubButton.interval = 0.3f;
            LongPressSubButton.bPressAcc = true;
            LongPressSubButton.onRelease.AddListener(Sub);
            LongPressSubButton.OnPressAcc.AddListener(Sub);

            mInputField = transform.Find("Animator/Image_Exchange_Number/InputField_Number").GetComponent<InputField>();
            undoParent = transform.Find("Animator/Scroll_View/Viewport");

            //Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(mAdd);
            //eventListener.AddEventListener(EventTriggerType.PointerDown, OnAddPointDown);
            //eventListener.AddEventListener(EventTriggerType.PointerUp, OnAddPointUp);

            //Lib.Core.EventTrigger eventListener1 = Lib.Core.EventTrigger.Get(mSub);
            //eventListener1.AddEventListener(EventTriggerType.PointerDown, OnSubPointDown);
            //eventListener1.AddEventListener(EventTriggerType.PointerUp, OnSubPointUp);
            mSlider.onValueChanged.AddListener(OnSliderValueChanged);
            mOkBtn.onClick.AddListener(Undo);
            mCloseBtn.onClick.AddListener(() => { UIManager.CloseUI(EUIID.UI_Decompose); });

            mButton.onClick.AddListener(() =>
            {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(mItemData.Id, 0, false, false, false, false, false, false, true);
                itemData.bShowBtnNo = false;
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Decompose, itemData));
            });
            mInputField.onValueChanged.AddListener(str =>
            {
                uint count;
                if (str == string.Empty)
                {
                    count = 0;
                }
                else
                {
                    count = uint.Parse(str);
                }
                if (count > MaxInputCount)
                {
                    count = MaxInputCount;
                }
                count = (uint)Mathf.Clamp(count, 0, MaxInputCount);
                mInputField.text = count.ToString();
                UpdateSlider((int)count);
                UpdateUndoItems();
                UpdateButtonState();
            });
            mSlider.wholeNumbers = true;
            mSlider.maxValue = MaxInputCount;
        }

        protected override void OnShow()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            mInputField.text = MaxInputCount.ToString();
            UpdateSlider((int)MaxInputCount);
            icon.enabled = true;
            itemNum.gameObject.SetActive(true);
            ImageHelper.SetIcon(icon, mItemData.cSVItemData.icon_id);
            TextHelper.SetText(itemName, mItemData.cSVItemData.name_id);
            ImageHelper.GetQualityColor_Frame(imgQuality, (int)mItemData.cSVItemData.quality);
            //技能书等级特殊显示
            Sys_Skill.Instance.ShowPetSkillBook(imgSkillBook, mItemData.cSVItemData);
            UpdateUndoItems();
            UpdateButtonState();
        }

        private void UpdateUndoItems()
        {
            itemNum.text = Count.ToString();
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].onClick.RemoveAllListeners();
            }
            buttons.Clear();
            List<uint> undos = mItemData.cSVItemData.undo_item;
            List<uint> nums = mItemData.cSVItemData.undo_num;
            int count = undos.Count;
            FrameworkTool.CreateChildList(undoParent, count);
            for (int i = 0; i < undoParent.childCount; i++)
            {
                Transform child = undoParent.GetChild(i);
                Image icon = child.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                Image qualityBg = child.Find("Btn_Item/Image_BG").GetComponent<Image>();
                Text num = child.Find("Text_Number").GetComponent<Text>();
                Button button = child.Find("Btn_Item").GetComponent<Button>();
                icon.enabled = true;
                num.gameObject.SetActive(true);

                ImageHelper.SetIcon(icon, CSVItem.Instance.GetConfData(undos[i]).icon_id);
                ImageHelper.GetQualityColor_Frame(qualityBg, (int)CSVItem.Instance.GetConfData(undos[i]).quality);
                num.text = (nums[i] * Count).ToString();
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(undos[i], 0, false, false, false, false, false, false, true);
                itemData.bShowBtnNo = false;
                button.onClick.AddListener(() =>
                {
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Decompose, itemData));
                });
                buttons.Add(button);
            }
        }

        private void UpdateButtonState()
        {
            ButtonHelper.Enable(mOkBtn, Count > 0);
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
            UpdateUndoItems();
            UpdateButtonState();
        }

        private void Undo()
        {
            PromptBoxParameter.Instance.Clear();
            string content1 = CSVLanguage.Instance.GetConfData(1000957).words;
            string content2 = CSVLanguage.Instance.GetConfData(1000958).words;
            PromptBoxParameter.Instance.content = string.Format(content1 + "\n" + content2, Count, CSVLanguage.Instance.GetConfData(mItemData.cSVItemData.name_id).words);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Bag.Instance.OnDecomposeItemReq(mItemData.Uuid, (uint)Count);
                UIManager.CloseUI(EUIID.UI_Decompose);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        private void Add()
        {
            int count = Count;
            count++;
            count = (int)Mathf.Clamp(count, 0, MaxInputCount);
            UpdateSlider((int)count);
            mInputField.text = count.ToString();
        }

        private void Sub()
        {
            int count = Count;
            count--;
            count = (int)Mathf.Clamp(count, 0, MaxInputCount);
            UpdateSlider((int)count);
            mInputField.text = count.ToString();
        }


        //bool add;
        //bool sub;
        //private void OnAddPointDown(BaseEventData baseEventData)
        //{
        //    add = true;
        //}
        //private void OnAddPointUp(BaseEventData baseEventData)
        //{
        //    add = false;
        //}
        //private void OnSubPointUp(BaseEventData baseEventData)
        //{
        //    sub = false;
        //}
        //private void OnSubPointDown(BaseEventData baseEventData)
        //{
        //    sub = true;
        //}

        //bool addflag = false;
        //bool subflag = false;
        //protected override void OnUpdate()
        //{
        //    if (add)
        //    {
        //        addtimer += Sys_HUD.Instance.GetDeltaTime();
        //        if (!addflag)
        //        {
        //            Add();
        //            addflag = true;
        //        }
        //        if (addtimer >= 0.7f)
        //        {
        //            Add();
        //        }
        //    }
        //    else
        //    {
        //        addflag = false;
        //        addtimer = 0;
        //    }
        //    if (sub)
        //    {
        //        subtimer += deltaTime;
        //        if (!subflag)
        //        {
        //            Sub();
        //            subflag = true;
        //        }
        //        if (subtimer >= 0.7f)
        //        {
        //            Sub();
        //        }
        //    }
        //    else
        //    {
        //        subflag = false;
        //        subtimer = 0;
        //    }
        //}
    }
}

