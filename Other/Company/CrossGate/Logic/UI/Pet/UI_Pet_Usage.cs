using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;
using Lib.Core;
using System.Text;
using System;

namespace Logic
{
    public class UI_Pet_Usage_Layout
    {
        private Button closeBtn;
        public PropItem propItem;
        public Text itemNameText;
        public Text itemLvText;
        public Text itemNumText;
        public Text itemDescText;
        public Text inputCaculatreNum;
        public Image itemIcon;
        public InputField inputField;
        public UI_LongPressButton subBtn;
        public UI_LongPressButton addBtn;
        public Button maxBtn;
        public Button confirmBtn;
        public Transform inputCaculatreTrans;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("Animator/View_Choose_Award/Top/PropItem").gameObject);
            itemNameText = transform.Find("Animator/View_Choose_Award/Top/Text_Name").GetComponent<Text>();
            itemLvText = transform.Find("Animator/View_Choose_Award/Top/Text_Grade/Text").GetComponent<Text>();
            itemNumText = transform.Find("Animator/View_Choose_Award/Top/Text_Amount/Text").GetComponent<Text>();
            itemDescText = transform.Find("Animator/View_Choose_Award/Text_Des/Text").GetComponent<Text>();
            itemIcon = transform.Find("Animator/View_Choose_Award/Top/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();

            inputCaculatreNum = transform.Find("Animator/View_Choose_Award/Amount/InputCaculate/Text").GetComponent<Text>();
            inputCaculatreNum.text = "1";
            inputField = transform.Find("Animator/View_Choose_Award/Amount/InputField_Number").GetComponent<InputField>();
            inputField.contentType = InputField.ContentType.IntegerNumber;
            inputField.keyboardType = TouchScreenKeyboardType.NumberPad;
            inputField.text = "1";
            subBtn = transform.Find("Animator/View_Choose_Award/Amount/Button_Sub").gameObject.AddComponent<UI_LongPressButton>();
            subBtn.interval = 0.3f;
            subBtn.bPressAcc = true;

            addBtn = transform.Find("Animator/View_Choose_Award/Amount/Button_Add").gameObject.AddComponent<UI_LongPressButton>();
            addBtn.interval = 0.3f;
            addBtn.bPressAcc = true;

            maxBtn = transform.Find("Animator/View_Choose_Award/Amount/Button_Max").GetComponent<Button>();
            confirmBtn = transform.Find("Animator/View_Choose_Award/Button_Confirm").GetComponent<Button>();
            inputCaculatreTrans = transform.Find("Animator/View_Choose_Award/Amount/InputCaculate").transform;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            inputField.onEndEdit.AddListener(listener.OnInputEnd);
            subBtn.onRelease.AddListener(listener.OnClickBtnSub);
            subBtn.OnPressAcc.AddListener(listener.OnClickBtnSub);
            addBtn.onRelease.AddListener(listener.OnClickBtnAdd);
            addBtn.OnPressAcc.AddListener(listener.OnClickBtnAdd);
            maxBtn.onClick.AddListener(listener.OnClickBtnMax);
            confirmBtn.onClick.AddListener(listener.OnConfirmBtn);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnInputEnd(string s);
            void OnClickBtnMax();
            void OnClickBtnAdd();
            void OnClickBtnSub();
            void OnConfirmBtn();
        }

    }

    public class UI_Pet_Usage : UIBase, UI_Pet_Usage_Layout.IListener
    {
        public class UI_Pet_UsageParam
        {
            public ulong itemUid;
            public Action<long> action;
            public bool isInputCaculate;
            public bool isExpChange;
            public long maxCount;
        }

        private UI_Pet_UsageParam usageParam;
        private UI_Pet_Usage_Layout layout;
        private UI_Common_Num m_Count;

        private long MaxInputCount;
        private long MaxInputCountRealUse;

        private uint inputNum;

        public int Count
        {
            get
            {
                if (layout.inputField.text == "" || layout.inputField.text == "0")
                {
                    return 1;
                }
                else
                {
                    return int.Parse(layout.inputField.text);
                }
            }
        }

        public int caculateCount
        {
            get
            {
                if (layout.inputCaculatreNum.text == "" || layout.inputCaculatreNum.text == "0")
                {
                    return 1;
                }
                else
                {
                    return int.Parse(layout.inputCaculatreNum.text);
                }
            }
        }

        protected override void OnLoaded()
        {
            layout = new UI_Pet_Usage_Layout();
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnOpen(object arg = null)
        {
            usageParam = arg as UI_Pet_UsageParam;
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnShow()
        {

            if (null != usageParam)
            {
                layout.inputCaculatreTrans.gameObject.SetActive(usageParam.isInputCaculate);
                layout.inputField.gameObject.SetActive(!usageParam.isInputCaculate);

                ulong itemUid = usageParam.itemUid;
                CSVItem.Instance.TryGetValue((uint)itemUid, out CSVItem.Data data);

                ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(itemUid);
                if (null != itemData)
                {
                    if (usageParam.isInputCaculate)
                    {
                        MaxInputCount = Sys_Bag.Instance.GetItemCount(itemData.Id);
                        if (usageParam.maxCount < MaxInputCount)
                        {
                            MaxInputCountRealUse = usageParam.maxCount;
                        }
                        else
                        {
                            MaxInputCountRealUse = MaxInputCount;
                        }
                        m_Count = new UI_Common_Num();
                        m_Count.Init(layout.inputCaculatreTrans, (uint)MaxInputCountRealUse);
                        m_Count.RegEnd(OnInputEnd);
                    }
                    else
                    {
                        MaxInputCount = itemData.Count;
                        MaxInputCountRealUse = MaxInputCount;
                    }
                    TextHelper.SetText(layout.itemNameText, itemData.cSVItemData.name_id);
                    TextHelper.SetText(layout.itemDescText, itemData.cSVItemData.describe_id);
                    layout.itemNumText.text = MaxInputCount.ToString();
                    layout.itemLvText.text = itemData.cSVItemData.lv.ToString();
                    layout.propItem.SetData(new PropIconLoader.ShowItemData(itemData.cSVItemData.id, MaxInputCount, true, false, false, false, false, false, false, false), EUIID.UI_Pet_Usage);

                }
                else
                {
                    if(usageParam.isExpChange)
                    {
                        MaxInputCountRealUse = usageParam.maxCount;
                        ImageHelper.SetIcon(layout.itemIcon, 10000004);
                        layout.itemIcon.gameObject.SetActive(true);
                        string[] str = CSVPetNewParam.Instance.GetConfData(22).str_value.Split('|');                 
                        TextHelper.SetText(layout.itemNameText, 10886);
                        TextHelper.SetText(layout.itemDescText, 10887, str[0], str[1]);
                        layout.itemNumText.text = MaxInputCountRealUse.ToString();
                        layout.itemLvText.text = string.Empty;
                    }
                }
            }
        }

        private void OnInputEnd(uint num)
        {
            if (num == inputNum)
            {
                m_Count.SetData((uint)caculateCount);
            }
            else
            {
                inputNum = num;
                m_Count.SetData(num);
            }
        }


        public void OncloseBtnClicked()
        {
            CloseSelf();
        }

        public void OnInputEnd(string str)
        {
            long count;
            try
            {
                if (str == "" || str == "0")
                    count = 1;
                else
                    count = uint.Parse(str);

                if (count > MaxInputCountRealUse)
                {
                    count = MaxInputCount;
                }
                count = (uint)Mathf.Clamp(count, 1, MaxInputCountRealUse);
            }
            catch (System.Exception)
            {
                DebugUtil.Log(ELogType.eBag, "格式不正确");
                count = 1;
            }
            layout.inputField.text = count.ToString();
        }

        public void OnClickBtnMax()
        {
            int count = usageParam.isInputCaculate? caculateCount : Count;
            count = (int)MaxInputCountRealUse;
            layout.inputField.text = count.ToString();
            layout.inputCaculatreNum.text = count.ToString();

        }

        public void OnClickBtnAdd()
        {
            int count = usageParam.isInputCaculate ? caculateCount : Count;
            count++;
            count = (int)Mathf.Clamp(count, 1, MaxInputCountRealUse == 0 ? 1 : MaxInputCountRealUse);
            layout.inputField.text = count.ToString();
            layout.inputCaculatreNum.text = count.ToString();
        }

        public void OnClickBtnSub()
        {
            int count = usageParam.isInputCaculate ? caculateCount : Count;
            count--;
            count = (int)Mathf.Clamp(count, 1, MaxInputCountRealUse == 0 ? 1 : MaxInputCountRealUse);
            layout.inputField.text = count.ToString();
            layout.inputCaculatreNum.text = count.ToString();
        }

        public void OnConfirmBtn()
        {
            int count = usageParam.isInputCaculate ? caculateCount : Count;
            if (null != usageParam)
            {
                usageParam.action?.Invoke(count);
            }
            CloseSelf();
        }
    }
}