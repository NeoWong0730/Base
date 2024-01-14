using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_Energyspar_Shop_Middle
    {

        private GameObject viewItem;

        private PropItem _propItem;
        private Text textTips;
        private Text textAllServer;
        private Text textAllServerNum;

        private Text textAllServerSellOut;

        private GameObject viewControl;
        private GameObject goInput;
        private InputField input;
        private Button btnSub;
        private Button btnAdd;
        private Button btnMax;

        private Button btnBuy;
        private Image imgBuyItem;
        private Text textBuyCount;

        private GameObject tipTemplate;

        //限购次数
        private Text textLimitNum;
        private Text textLimitNumValue;
        //{0}级限购
        private Text textLimitLevel;

        private GameObject viewNone;

        private uint mShopItemId;
        private CSVShopItem.Data shopItemData;

        private int DefaultBuyCount = 1;
        private int curBuyCount = 0;

        private bool IsFixLevel = false;
        private bool IsLikeAbility = false;
        private bool IsNeedTask = false;
        private bool IsFixRank = false;
        
        public void Init(Transform transform)
        {
            viewItem = transform.Find("View_Item").gameObject;

            _propItem = new PropItem();
            _propItem.BindGameObject(viewItem.transform.Find("PropItem").gameObject);
            textTips = viewItem.transform.Find("Text_Tips").GetComponent<Text>();
            textAllServer = viewItem.transform.Find("Text_AllServer").GetComponent<Text>();
            textAllServerNum = textAllServer.transform.Find("Text").GetComponent<Text>();
            textAllServerSellOut = viewItem.transform.Find("Text_ServerSellout").GetComponent<Text>();

            viewControl = viewItem.transform.Find("View_Controll").gameObject;
            goInput = viewControl.transform.Find("Image_Number").gameObject;
            input = viewControl.transform.Find("Image_Number/InputField_Number").GetComponent<InputField>();
            input.contentType = InputField.ContentType.IntegerNumber;
            input.keyboardType = TouchScreenKeyboardType.NumberPad;
            input.onEndEdit.AddListener(OnInputEnd);

            btnSub = viewControl.transform.Find("Image_Number/Button_Sub").GetComponent<Button>();
            //btnSub.onClick.AddListener(OnClickBtnSub);
            UI_LongPressButton LongPressSubButton = btnSub.gameObject.AddComponent<UI_LongPressButton>();
            LongPressSubButton.interval = 0.3f;
            LongPressSubButton.bPressAcc = true;
            LongPressSubButton.onRelease.AddListener(OnClickBtnSub);
            LongPressSubButton.OnPressAcc.AddListener(OnClickBtnSub);


            btnAdd = viewControl.transform.Find("Image_Number/Button_Add").GetComponent<Button>();
            //btnAdd.onClick.AddListener(OnClickBtnAdd);
            UI_LongPressButton LongPressAddButton = btnAdd.gameObject.AddComponent<UI_LongPressButton>();
            LongPressAddButton.interval = 0.3f;
            LongPressAddButton.bPressAcc = true;
            LongPressAddButton.onRelease.AddListener(OnClickBtnAdd);
            LongPressAddButton.OnPressAcc.AddListener(OnClickBtnAdd);

            btnMax = viewControl.transform.Find("Image_Number/Button_Max").GetComponent<Button>();
            btnMax.onClick.AddListener(OnClickBtnMax);

            btnBuy = viewControl.transform.Find("Btn_01").GetComponent<Button>();
            btnBuy.onClick.AddListener(OnClickBtnBuy);
            imgBuyItem = btnBuy.transform.Find("Image_Icon").GetComponent<Image>();
            textBuyCount = imgBuyItem.transform.Find("Text").GetComponent<Text>();

            tipTemplate = viewControl.transform.Find("Object_Tips/Text_Limit02").gameObject;
            tipTemplate.gameObject.SetActive(false);

            textLimitNum = viewControl.transform.Find("Text_Limit01").GetComponent<Text>();
            textLimitNumValue = textLimitNum.transform.Find("Text").GetComponent<Text>();
            //textLimitLevel = viewControl.transform.Find("Text_Limit02").GetComponent<Text>();

            viewNone = transform.Find("View_None")?.gameObject;

            input.characterLimit = 5;
        }

        private void OnInputEnd(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                int result = int.Parse(s);
                if (result <= 0)
                {
                    curBuyCount = DefaultBuyCount;
                }
                else
                {
                    int maxCount =(int)Sys_Mall.Instance.CalCanBuyMaxCount(mShopItemId);
                    curBuyCount = result > maxCount ? maxCount : result;
                }
            }
            else
            {
                curBuyCount = DefaultBuyCount;
            }

            input.text = curBuyCount.ToString();
            UpdateCostPirce();
        }

        private void OnClickBtnSub()
        {
            if (curBuyCount > 1)
            {
                curBuyCount--;
            }

            input.text = curBuyCount.ToString();
            UpdateCostPirce();
        }

        private void OnClickBtnAdd()
        {
            int maxCount = (int)Sys_Mall.Instance.CalCanBuyMaxCount(mShopItemId);
            if (curBuyCount < maxCount)
            {
                curBuyCount++;
            }

            input.text = curBuyCount.ToString();
            UpdateCostPirce();
        }

        private void OnClickBtnMax()
        {
            curBuyCount = (int)Sys_Mall.Instance.CalCanBuyMaxCount(mShopItemId);
            input.text = curBuyCount.ToString();
            UpdateCostPirce();
        }

        private void OnClickBtnBuy()
        {
            Sys_Mall.Instance.OnBuyReq(mShopItemId, (uint)curBuyCount);
        }

        public void UpdateInfo(uint shopItemId)
        {
            mShopItemId = shopItemId;

            viewItem.gameObject.SetActive(mShopItemId != 0);
            viewNone?.gameObject.SetActive(mShopItemId == 0);

            if (mShopItemId == 0)
                return;

            shopItemData = CSVShopItem.Instance.GetConfData(shopItemId);

            CSVItem.Data itemData = CSVItem.Instance.GetConfData(shopItemData.item_id);
            PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData(itemData.id, 1, false, false, false, false, false, false, false, true);
            _propItem.SetData(new MessageBoxEvt(EUIID.UI_Energyspar, showItem));
            textTips.text = LanguageHelper.GetTextContent(itemData.describe_id);

            //购买按钮货币
            ImageHelper.SetIcon(imgBuyItem, CSVItem.Instance.GetConfData(shopItemData.price_type).small_icon_id);

            curBuyCount = DefaultBuyCount;
            input.text = curBuyCount.ToString();

            UpdateState();
            UpdateCostPirce();
        }

        private void UpdateState()
        {
            textTips.gameObject.SetActive(false);

            textAllServer.gameObject.SetActive(false);
            textAllServerSellOut.gameObject.SetActive(false);

            viewControl.SetActive(true);
            //textLimitLevel.gameObject.SetActive(false);
            textLimitNum.gameObject.SetActive(false);
            goInput.SetActive(false);
            
            GameObject tipParent = tipTemplate.transform.parent.gameObject;
            Lib.Core.GameObjectExtensions.DestoryAllChildren(tipParent, new List<string>() { tipTemplate.name });

            IsFixLevel = Sys_Mall.Instance.IsFixLevel(shopItemData);
            IsLikeAbility = Sys_Mall.Instance.IsFixLikeAbility(shopItemData);
            IsNeedTask = Sys_Mall.Instance.IsNeedTask(shopItemData);
            IsFixRank = Sys_Mall.Instance.IsFixRank(shopItemData);

            if (!IsFixLevel)
            {
                GameObject tipGo = GameObject.Instantiate<GameObject>(tipTemplate, tipParent.transform);
                tipGo.SetActive(true);

                Text text = tipGo.GetComponent<Text>();
                text.text = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(2009806), shopItemData.level_require.ToString());
            }

            if (!IsLikeAbility)
            {
                GameObject tipGo = GameObject.Instantiate<GameObject>(tipTemplate, tipParent.transform);
                tipGo.SetActive(true);

                Text text = tipGo.GetComponent<Text>();
                text.text = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(2009819), "0", shopItemData.likability.ToString());
            }

            if (!IsNeedTask)
            {
                GameObject tipGo = GameObject.Instantiate<GameObject>(tipTemplate, tipParent.transform);
                tipGo.SetActive(true);

                System.Text.StringBuilder str = new System.Text.StringBuilder();
                for (int i = 0; i < shopItemData.need_task.Count; ++i)
                {
                    uint taskId = shopItemData.need_task[i];
                    CSVTask.Data task = CSVTask.Instance.GetConfData(taskId);
                    if (task != null)
                    {
                        str.Append(LanguageHelper.GetTaskTextContent(task.taskName));
                        if (i < shopItemData.need_task.Count - 1)
                            str.Append(".");
                    }
                }

                Text text = tipGo.GetComponent<Text>();
                text.text = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(2009820), str.ToString());
            }
            
            if (!IsFixRank)
            {
                GameObject tipGo = GameObject.Instantiate<GameObject>(tipTemplate, tipParent.transform);
                tipGo.SetActive(true);

                Text text = tipGo.GetComponent<Text>();
                uint lanId = 2009752;
                CSVTianTiSegmentInformation.Data temp =
                    CSVTianTiSegmentInformation.Instance.GetConfData(shopItemData.need_rank);
                text.text = LanguageHelper.GetTextContent(lanId, LanguageHelper.GetTextContent(temp.RankDisplay));
            }

            Lib.Core.FrameworkTool.ForceRebuildLayout(tipParent);

            bool canBuy = IsFixLevel && IsLikeAbility && IsNeedTask && IsFixRank;
            bool isSellOut = Sys_Mall.Instance.IsSellOut(mShopItemId);

            ImageHelper.SetImageGray(btnBuy.image, !canBuy || isSellOut, true);

            if (!canBuy)
                return;

            goInput.SetActive(true);
            textTips.gameObject.SetActive(true);

            bool isSelfType = shopItemData.personal_limit != 0;
            if (isSelfType)
            {
                uint lanId = (shopItemData.limit_type == 1 || shopItemData.limit_type == 3) ? (uint)2009805 : (uint)2009809;

                //新加个人限购历史类型5
                lanId = shopItemData.limit_type == 5 ?  (uint)2009821 : lanId;

                textLimitNum.gameObject.SetActive(true);
                textLimitNum.text = LanguageHelper.GetTextContent(lanId);
                textLimitNumValue.text = string.Format("{0}/{1}", Sys_Mall.Instance.CalSelfLeftNum(mShopItemId).ToString(), shopItemData.personal_limit.ToString());
            }

            bool isAllServerType = shopItemData.limit_type == 3 || shopItemData.limit_type == 4; //每日或每周全服限购类型
            if (isAllServerType)
            {
                int leftNum = Sys_Mall.Instance.CalAllServerLeftNum(mShopItemId);
                if (leftNum == 0)
                {
                    textAllServerSellOut.gameObject.SetActive(true);
                }
                else
                {
                    textAllServer.gameObject.SetActive(true);
                    textAllServerNum.text = leftNum.ToString();
                }
            }
        }

        private void UpdateCostPirce()
        {
            uint price = Sys_Mall.Instance.GetItemPrice(shopItemData.id);
            bool isEnough = Sys_Bag.Instance.GetItemCount(shopItemData.price_type) >= price * curBuyCount;
            uint lanId = isEnough ? (uint)2007203 : (uint)2007204;
            TextHelper.SetText(textBuyCount, lanId, (price * curBuyCount).ToString());
        }

        public void OnBuyScuccess()
        {
            UpdateState();
            UpdateCostPirce();
        }
    }
}


